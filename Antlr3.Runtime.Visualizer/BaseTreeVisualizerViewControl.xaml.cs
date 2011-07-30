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

namespace Antlr3.Runtime.Visualizer
{
    using System.Windows.Controls;
    using System.Windows;
    using Antlr.Runtime;
    using Antlr.Runtime.Tree;
    using System.Windows.Documents;
    using System;
    using Antlr4.StringTemplate.Visualizer.Extensions;
    using System.Windows.Media;

    public partial class BaseTreeVisualizerViewControl : UserControl
    {
        public BaseTreeVisualizerViewControl()
        {
            InitializeComponent();
        }

        public TreeVisualizerViewModel ViewModel
        {
            get
            {
                return DataContext as TreeVisualizerViewModel;
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

                SourceTextBox.Document = new FlowDocument(new Paragraph(new Run(viewModel.SourceText)
                {
                    FontFamily = new FontFamily("Consolas")
                }));
            }

            base.OnPropertyChanged(e);
        }

        private void HandleAstTreeViewSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeVisualizerViewModel viewModel = ViewModel;
            if (viewModel == null)
            {
                Highlight(SourceTextBox.Document, null);
                return;
            }

            TreeViewModel node = AstTreeView.SelectedItem as TreeViewModel;
            if (node == null)
            {
                Highlight(SourceTextBox.Document, null);
                return;
            }

            IToken a = viewModel.GetToken(node.Adaptor.GetTokenStartIndex(node.Tree));
            IToken b = viewModel.GetToken(node.Adaptor.GetTokenStopIndex(node.Tree));
            if (a == null || b == null)
            {
                Highlight(SourceTextBox.Document, null);
                return;
            }

            Highlight(SourceTextBox.Document, Interval.FromBounds(a.StartIndex, b.StopIndex + 1));
        }

        private static TextPointer Highlight(FlowDocument document, Interval interval)
        {
            if (document == null)
                throw new ArgumentNullException("document");

            TextPointer contentStart = document.ContentStart;

            // clear any existing highlight
            TextRange documentRange = new TextRange(document.ContentStart, document.ContentEnd);
            documentRange.ApplyPropertyValue(FlowDocument.BackgroundProperty, FlowDocument.BackgroundProperty.DefaultMetadata.DefaultValue);

            if (interval == null)
                return null;

            // highlight the new text
            int startOffset = interval.Start;
            int endOffset = interval.End;
            TextPointer highlightStart = document.GetPointerFromCharOffset(ref startOffset);
            TextPointer highlightStop = document.GetPointerFromCharOffset(ref endOffset);
            if (startOffset != 0 || endOffset != 0)
                return null;

            var textRange = new TextRange(highlightStart, highlightStop);
            textRange.ApplyPropertyValue(FlowDocument.BackgroundProperty, Brushes.Yellow);
            return textRange.Start;
        }
    }
}
