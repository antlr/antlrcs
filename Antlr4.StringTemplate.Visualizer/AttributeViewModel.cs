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
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Antlr4.StringTemplate.Debug;
    using Antlr4.StringTemplate.Visualizer.Extensions;

    public class AttributeViewModel
    {
        private readonly string _name;
        private readonly object _value;
        private readonly ReadOnlyCollection<AddAttributeEvent> _events;

        public AttributeViewModel(string name, object value, IEnumerable<AddAttributeEvent> events)
        {
            _name = name;
            _value = value;
            if (events != null)
                _events = events.ToList().AsReadOnly();
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public object Value
        {
            get
            {
                return _value;
            }
        }

        public ReadOnlyCollection<AddAttributeEvent> Events
        {
            get
            {
                return _events;
            }
        }

        // for WPF tree view binding
        public object Attributes
        {
            get
            {
                return null;
            }
        }

        public override string ToString()
        {
            string description = this.GetDescription();
            if (description.Length > 200)
                description = description.Substring(0, 197) + "...";

            return description;
        }
    }
}
