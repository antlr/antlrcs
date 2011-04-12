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
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using Antlr.Runtime;
    using Antlr.Runtime.Tree;
    using Antlr4.StringTemplate.Debug;
    using Antlr4.StringTemplate.Misc;

    public class TemplateVisualizerViewModel : INotifyPropertyChanged
    {
        private static readonly PropertyChangedEventArgs AstPropertyChangedEventArgs = new PropertyChangedEventArgs("Ast");
        private static readonly PropertyChangedEventArgs BytecodePropertyChangedEventArgs = new PropertyChangedEventArgs("Bytecode");
        private static readonly PropertyChangedEventArgs AttributesPropertyChangedEventArgs = new PropertyChangedEventArgs("Attributes");
        private static readonly PropertyChangedEventArgs AttributeStackPropertyChangedEventArgs = new PropertyChangedEventArgs("AttributeStack");
        private static readonly PropertyChangedEventArgs TitlePropertyChangedEventArgs = new PropertyChangedEventArgs("Title");

        private readonly TemplateVisualizer _visualizer;
        private readonly List<TemplateCallHierarchyViewModel> _templateCallHierarchy;
        private List<TemplateFrameAttributeViewModel> _attributeStack;
        private readonly List<InterpEvent> _allEvents;
        private CommonTree _ast;
        private string _title;
        private string _bytecode;
        private List<string> _attributes;

        public event PropertyChangedEventHandler PropertyChanged;

        public TemplateVisualizerViewModel(TemplateVisualizer visualizer)
        {
            if (visualizer == null)
                throw new ArgumentNullException("visualizer");

            _visualizer = visualizer;
            _allEvents = visualizer.Interpreter.GetEvents();
            List<InterpEvent> events = visualizer.RootTemplate.GetDebugState().Events;
            _templateCallHierarchy =
                new List<TemplateCallHierarchyViewModel>()
                {
                    new TemplateCallHierarchyViewModel(visualizer.Interpreter, (EvalTemplateEvent)events[events.Count - 1])
                };
        }

        public TemplateVisualizer Visualizer
        {
            get
            {
                return _visualizer;
            }
        }

        public string Output
        {
            get
            {
                return _visualizer.Output;
            }
        }

        public ReadOnlyCollection<TemplateMessage> Errors
        {
            get
            {
                return Visualizer.Errors;
            }
        }

        public List<string> Trace
        {
            get
            {
                return _visualizer.Trace;
            }
        }

        public List<InterpEvent> AllEvents
        {
            get
            {
                return _allEvents;
            }
        }

        public List<TemplateCallHierarchyViewModel> TemplateCallHierarchy
        {
            get
            {
                return _templateCallHierarchy;
            }
        }

        public List<TemplateFrameAttributeViewModel> AttributeStack
        {
            get
            {
                return _attributeStack;
            }

            set
            {
                if (value == _attributeStack)
                    return;

                _attributeStack = value;
                OnPropertyChanged(AttributeStackPropertyChangedEventArgs);
            }
        }

        public CommonTree Ast
        {
            get
            {
                return _ast;
            }

            set
            {
                if (_ast != null && _ast.Children[0] == value)
                    return;

                if (value == null)
                {
                    _ast = null;
                }
                else
                {
                    _ast = new CommonTree();
                    _ast.AddChild(new CommonTree(new CommonToken(TokenTypes.Invalid)));
                    _ast.Children[0] = value;
                }

                OnPropertyChanged(AstPropertyChangedEventArgs);
            }
        }

        public string Title
        {
            get
            {
                return _title;
            }

            set
            {
                if (_title == value)
                    return;

                _title = value;
                OnPropertyChanged(TitlePropertyChangedEventArgs);
            }
        }

        public string Bytecode
        {
            get
            {
                return _bytecode;
            }

            set
            {
                if (_bytecode == value)
                    return;

                _bytecode = value;
                OnPropertyChanged(BytecodePropertyChangedEventArgs);
            }
        }

        public List<string> Attributes
        {
            get
            {
                return _attributes;
            }

            set
            {
                if (_attributes == value)
                    return;

                _attributes = value;
                OnPropertyChanged(AttributesPropertyChangedEventArgs);
            }
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            var t = PropertyChanged;
            if (t != null)
                t(this, e);
        }
    }
}
