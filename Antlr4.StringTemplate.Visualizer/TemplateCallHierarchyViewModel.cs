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
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using Antlr4.StringTemplate.Debug;
    using ArgumentNullException = System.ArgumentNullException;
    using Path = System.IO.Path;

    public class TemplateCallHierarchyViewModel : INotifyPropertyChanged
    {
        private static readonly PropertyChangedEventArgs IsExpandedPropertyChangedEventArgs = new PropertyChangedEventArgs("IsExpanded");
        private static readonly PropertyChangedEventArgs IsSelectedPropertyChangedEventArgs = new PropertyChangedEventArgs("IsSelected");

        private readonly Interpreter _interpreter;
        private readonly EvalTemplateEvent _event;
        private List<TemplateCallHierarchyViewModel> _children;

        private bool _isExpanded;
        private bool _isSelected;

        public TemplateCallHierarchyViewModel(Interpreter interpreter, EvalTemplateEvent @event)
        {
            if (interpreter == null)
                throw new ArgumentNullException("interpreter");
            if (@event == null)
                throw new ArgumentNullException("event");

            this._interpreter = interpreter;
            this._event = @event;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public EvalTemplateEvent Event
        {
            get
            {
                return _event;
            }
        }

        public TemplateFrame Frame
        {
            get
            {
                return _event.Frame;
            }
        }

        public Template Template
        {
            get
            {
                return _event.Template;
            }
        }

        public bool IsExpanded
        {
            get
            {
                return _isExpanded;
            }

            set
            {
                if (_isExpanded == value)
                    return;

                _isExpanded = value;
                OnPropertyChanged(IsExpandedPropertyChangedEventArgs);
            }
        }

        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }

            set
            {
                if (_isSelected == value)
                    return;

                _isSelected = value;
                OnPropertyChanged(IsSelectedPropertyChangedEventArgs);
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public List<TemplateCallHierarchyViewModel> Children
        {
            get
            {
                if (_children == null)
                {
                    _children = new List<TemplateCallHierarchyViewModel>();
                    foreach (var @event in _event.Frame.GetDebugState().ChildEvalTemplateEvents)
                        _children.Add(new TemplateCallHierarchyViewModel(_interpreter, @event));
                }

                return _children;
            }
        }

        public override string ToString()
        {
            Template template = _event.Template;
            if (template.IsAnonymousSubtemplate)
                return "{...}";

            if (template.DebugState == null || template.DebugState.NewTemplateEvent == null)
                return string.Format("{0}", template);

            return string.Format("{0} @ {1}:{2}", template, Path.GetFileName(template.DebugState.NewTemplateEvent.GetFileName()), template.DebugState.NewTemplateEvent.GetLine());
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            var t = PropertyChanged;
            if (t != null)
                t(this, e);
        }
    }
}
