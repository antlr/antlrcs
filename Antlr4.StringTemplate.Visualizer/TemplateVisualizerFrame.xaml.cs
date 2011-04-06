/*
 * [The "BSD license"]
 * Copyright (c) 2011 Terence Parr
 * All rights reserved.
 *
 * Conversion to C#:
 * Copyright (c) 2011 Sam Harwell, Tunnel Vision Laboratories, LLC
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions
 * are met:
 * 1. Redistributions of source code must retain the above copyright
 *    notice, this list of conditions and the following disclaimer.
 * 2. Redistributions in binary form must reproduce the above copyright
 *    notice, this list of conditions and the following disclaimer in the
 *    documentation and/or other materials provided with the distribution.
 * 3. The name of the author may not be used to endorse or promote products
 *    derived from this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE AUTHOR ``AS IS'' AND ANY EXPRESS OR
 * IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
 * OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
 * IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT, INDIRECT,
 * INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
 * NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
 * DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
 * THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
 * THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

namespace Antlr4.StringTemplate.Visualizer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Media;
    using Antlr.Runtime;
    using Antlr.Runtime.Tree;
    using Antlr4.StringTemplate.Debug;
    using Antlr4.StringTemplate.Misc;
    using Antlr4.StringTemplate.Visualizer.Extensions;
    using IList = System.Collections.IList;
    using Path = System.IO.Path;

    public partial class TemplateVisualizerFrame : UserControl
    {
        private TemplateFrame currentTemplate;

        public TemplateVisualizerFrame()
        {
            InitializeComponent();
        }

        public TemplateVisualizerViewModel ViewModel
        {
            get
            {
                return DataContext as TemplateVisualizerViewModel;
            }

            set
            {
                DataContext = value;
            }
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == DataContextProperty)
            {
                var viewModel = ViewModel;
                if (viewModel == null)
                    return;

                currentTemplate = viewModel.Visualizer.RootTemplate;
                OutputTextBox.Document = new FlowDocument(new Paragraph(new Run(viewModel.Output)
                {
                    FontFamily = new FontFamily("Consolas")
                }));
                UpdateCurrentTemplate();
            }

            base.OnPropertyChanged(e);
        }

        private void HandleErrorsListBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int minIndex = ErrorsListBox.SelectedIndex;
            TemplateMessage message = ErrorsListBox.SelectedItem as TemplateMessage;
            TemplateRuntimeMessage runtimeMessage = message as TemplateRuntimeMessage;
            if (runtimeMessage != null)
            {
                Interval interval = runtimeMessage.SourceInterval;
                currentTemplate = runtimeMessage.Frame;
                UpdateCurrentTemplate();
                Highlight(TemplateTextBox.Document, interval);
            }
        }

        private void HandleAttributesListBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // do nothing for now
        }

        private void HandleCallHierarchyTreeViewSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TemplateCallHierarchyViewModel selected = CallHierarchyTreeView.SelectedItem as TemplateCallHierarchyViewModel;
            if (selected != null)
            {
                currentTemplate = selected.Frame;
                UpdateCurrentTemplate();
            }
        }

        private void HandleAstTreeViewSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            CommonTree node = AstTreeView.SelectedItem as CommonTree;
            if (node == null)
                return;

            CommonToken a = (CommonToken)currentTemplate.Template.impl.tokens.Get(node.TokenStartIndex);
            CommonToken b = (CommonToken)currentTemplate.Template.impl.tokens.Get(node.TokenStopIndex);
            if (a == null || b == null)
                return;

            Highlight(TemplateTextBox.Document, Interval.FromBounds(a.StartIndex, b.StopIndex + 1));
        }

        private void HandleOutputTextBoxSelectionChanged(object sender, RoutedEventArgs e)
        {
            InterpEvent de = FindEventAtOutputLocation(ViewModel.AllEvents, OutputTextBox.Document.GetCharOffsetToPosition(OutputTextBox.CaretPosition));
            if (de == null)
                currentTemplate = ViewModel.Visualizer.RootTemplate;
            else
                currentTemplate = de.Frame;
            UpdateCurrentTemplate();
        }

        private static InterpEvent FindEventAtOutputLocation(List<InterpEvent> events, int position)
        {
            if (events == null)
                throw new ArgumentNullException("events");

            foreach (var e in events)
            {
                if (e.OutputInterval.Contains(position))
                    return e;
            }

            return null;
        }

        private static void Highlight(FlowDocument document, Interval interval)
        {
            if (document == null)
                throw new ArgumentNullException("document");

            TextPointer contentStart = document.ContentStart;

            // clear any existing highlight
            TextRange documentRange = new TextRange(document.ContentStart, document.ContentEnd);
            documentRange.ApplyPropertyValue(FlowDocument.BackgroundProperty, FlowDocument.BackgroundProperty.DefaultMetadata.DefaultValue);

            // highlight the new text
            if (interval != null)
            {
                int startOffset = interval.Start;
                int endOffset = interval.End;
                TextPointer highlightStart = document.GetPointerFromCharOffset(ref startOffset);
                TextPointer highlightStop = document.GetPointerFromCharOffset(ref endOffset);
                if (startOffset != 0 || endOffset != 0)
                    return;

                var textRange = new TextRange(highlightStart, highlightStop);
                textRange.ApplyPropertyValue(FlowDocument.BackgroundProperty, Brushes.Yellow);
            }
        }

        private static void SetSelectionPath(TemplateCallHierarchyViewModel treeView, ICollection<Template> selectionPath)
        {
            if (treeView == null || selectionPath == null || selectionPath.Count == 0 || treeView.Template != selectionPath.First())
                return;

            List<TemplateCallHierarchyViewModel> nodes = new List<TemplateCallHierarchyViewModel>();
            nodes.Add(treeView);

            TemplateCallHierarchyViewModel current = treeView;
            foreach (var template in selectionPath.Skip(1))
            {
                current = current.Children.FirstOrDefault(i => i.Template == template);
                if (current == null)
                    return;

                nodes.Add(current);
            }

            for (int i = 0; i < nodes.Count - 1; i++)
                nodes[i].IsExpanded = true;

            nodes[nodes.Count - 1].IsSelected = true;
        }

        private void UpdateCurrentTemplate()
        {
            var viewModel = ViewModel;
            if (viewModel == null)
                return;

            // update all views according to current template
            UpdateStack();
            UpdateAttributes();
            viewModel.Bytecode = currentTemplate.Template.impl.Disassemble();
            TemplateTextBox.Document = new FlowDocument(new Paragraph(new Run(currentTemplate.Template.impl.template)
            {
                FontFamily = new FontFamily("Consolas")
            }));

            #region new stuff

            // update tree view of template hierarchy and select assoc. text substring

            // compute path from root to currentST, create TreePath for tree widget
            //		List<ST> pathST = currentST.getEnclosingInstanceStack(true);
            ////		System.out.println("path="+pathST);
            //		Object[] path = new Object[pathST.size()];
            //		int j = 0;
            //		for (ST s : pathST) path[j++] = new JTreeSTModel.Node(s, interp.getDebugState(s));
            //		m.tree.setSelectionPath(new TreePath(path));

            // highlight output text and, if {...} subtemplate, region in ST src
            // get last event for currentST; it's the event that captures ST eval
            List<InterpEvent> events = currentTemplate.GetDebugState().Events;
            EvalTemplateEvent e = (EvalTemplateEvent)events[events.Count - 1];
            //m.output.moveCaretPosition(e.outputStartChar);
            Highlight(OutputTextBox.Document, e.OutputInterval);
            if (currentTemplate.Template.IsAnonymousSubtemplate)
            {
                Interval r = currentTemplate.Template.impl.TemplateRange;
                //				System.out.println("currentST src range="+r);
                //m.template.moveCaretPosition(r.a);
                //TemplateTextBox.CaretPosition.
                Highlight(TemplateTextBox.Document, r);
            }

            #endregion

#if false
            // update tree view of template hierarchy and select assoc. text substring
            viewModel.Ast = currentTemplate.impl.ast;

            SetSelectionPath(viewModel.TemplateCallHierarchy[0], currentTemplate.GetEnclosingInstanceStack(true));

            Interval r = currentTemplate.impl.TemplateRange;
            if (currentTemplate.EnclosingInstance != null)
            {
                int i = GetIndexOfChild(currentTemplate.EnclosingInstance, currentTemplate);
                if (i == -1)
                {
                    Highlight(OutputTextBox.Document, null);
                    Highlight(TemplateTextBox.Document, r);
                }
                else
                {
                    InterpEvent e = ViewModel.Visualizer.Interpreter.GetEvents(currentTemplate.EnclosingInstance)[i];
                    if (e is EvalTemplateEvent)
                    {
                        if (currentTemplate.IsAnonymousSubtemplate)
                            Highlight(TemplateTextBox.Document, r);

                        Highlight(OutputTextBox.Document, e.OutputInterval);
                    }
                }
            }
            else
            {
                Highlight(OutputTextBox.Document, null);
                Highlight(TemplateTextBox.Document, r);
            }
#endif
        }

        private int GetIndexOfChild(EvalTemplateEvent parent, EvalTemplateEvent child)
        {
            if (parent == null)
                throw new ArgumentNullException("parent");
            if (child == null)
                throw new ArgumentNullException("child");

            TemplateCallHierarchyViewModel hierarchy = new TemplateCallHierarchyViewModel(ViewModel.Visualizer.Interpreter, parent);
            List<TemplateCallHierarchyViewModel> children = hierarchy.Children;
            return children.FindIndex(i => i.Event == child);
        }

        private void UpdateStack()
        {
            List<Template> stack = currentTemplate.GetEnclosingInstanceStack(true);
            ViewModel.Title = string.Format("STViz - [{0}]", string.Join(" ", stack.Select(i => i.ToString()).ToArray()));
        }

        private void UpdateAttributes()
        {
            var viewModel = ViewModel;
            if (viewModel == null)
                return;

            List<string> attributesList = new List<string>();
            IDictionary<string, object> attributes = currentTemplate.Template.GetAttributes();
            if (attributes != null)
            {
                foreach (var attribute in attributes)
                {
                    object value = attribute.Value;
                    IList valueList = value as IList;
                    if (valueList != null)
                        value = valueList.ToListString();

                    if (currentTemplate.Template.DebugState != null && currentTemplate.Template.DebugState.AddAttributeEvents != null)
                    {
                        List<AddAttributeEvent> events;
                        currentTemplate.Template.DebugState.AddAttributeEvents.TryGetValue(attribute.Key, out events);
                        StringBuilder locations = new StringBuilder();
                        int i = 0;
                        if (events != null)
                        {
                            foreach (AddAttributeEvent ae in events)
                            {
                                if (i > 0)
                                    locations.Append(", ");

                                locations.AppendFormat("{0}:{1}", Path.GetFileName(ae.GetFileName()), ae.GetLine());
                                i++;
                            }
                        }

                        if (locations.Length > 0)
                        {
                            attributesList.Add(string.Format("{0} = {1} @ {2}", attribute.Key, value, locations));
                        }
                        else
                        {
                            attributesList.Add(string.Format("{0} = {1}", attribute.Key, value));
                        }
                    }
                    else
                    {
                        attributesList.Add(string.Format("{0} = {1}", attribute.Key, value));
                    }
                }
            }

            viewModel.Attributes = attributesList;
        }
    }
}
