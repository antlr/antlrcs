/*
 * [The "BSD licence"]
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
    using System.Linq;
    using Antlr4.StringTemplate.Debug;
    using Path = System.IO.Path;

    public class TemplateFrameAttributeViewModel : INotifyPropertyChanged
    {
        private static readonly ReadOnlyCollection<AttributeViewModel> EmptyAttributes = new ReadOnlyCollection<AttributeViewModel>(new AttributeViewModel[0]);

        private readonly EvalTemplateEvent _event;
        private readonly ReadOnlyCollection<AttributeViewModel> _attributes = EmptyAttributes;

        public TemplateFrameAttributeViewModel(EvalTemplateEvent @event, HashSet<string> hiddenAttributes)
        {
            if (@event == null)
                throw new ArgumentNullException("event");

            this._event = @event;

            Template template = _event.Frame.Template;
            IDictionary<string, object> attributes = template.GetAttributes();
            if (attributes != null)
            {
                List<AttributeViewModel> attributesList = new List<AttributeViewModel>();
                foreach (var attribute in attributes)
                {
                    bool hidden = !hiddenAttributes.Add(attribute.Key);
                    attributesList.Add(new AttributeViewModel(attribute.Key, attribute.Value, hidden, GetAttributeEvents(template, attribute.Key)));
                }

                _attributes = attributesList.AsReadOnly();
            }
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

        public ReadOnlyCollection<AttributeViewModel> Attributes
        {
            get
            {
                return _attributes;
            }
        }

        private static IEnumerable<AddAttributeEvent> GetAttributeEvents(Template template, string attribute)
        {
            if (template == null || template.DebugState == null)
                return null;

            List<AddAttributeEvent> events;
            if (!template.DebugState.AddAttributeEvents.TryGetValue(attribute, out events))
                return null;

            return events;
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
