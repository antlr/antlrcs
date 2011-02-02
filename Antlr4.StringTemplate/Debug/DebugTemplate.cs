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

namespace Antlr4.StringTemplate.Debug
{
    using System.Collections.Generic;
    using Antlr4.StringTemplate.Misc;
    using CultureInfo = System.Globalization.CultureInfo;
    using StringWriter = System.IO.StringWriter;

    /** To avoid polluting Template instances with debug info when not debugging.
     *  Setting debug mode in TemplateGroup makes it create these instead of STs.
     */
    public class DebugST : Template
    {
        public class State
        {
            /** Track all events that occur during rendering. */
            public List<InterpEvent> interpEvents = new List<InterpEvent>();
        }

        /** Record who made us? ConstructionEvent creates Exception to grab stack */
        public ConstructionEvent newSTEvent = new ConstructionEvent();

        /** Track construction-time Add attribute "events"; used for Template user-level debugging */
        public MultiMap<string, AddAttributeEvent> addAttrEvents = new MultiMap<string, AddAttributeEvent>();

        //public Interpreter interp; // set when we start interpreter in inspect()

        public DebugST()
        {
        }

        public DebugST(Template proto) : base(proto)
        {
        }

        public override void Add(string name, object value)
        {
            if (groupThatCreatedThisInstance.Debug)
                addAttrEvents.Add(name, new AddAttributeEvent(name, value));

            base.Add(name, value);
        }

        // TESTING SUPPORT

        public virtual List<InterpEvent> GetEvents()
        {
            return GetEvents(CultureInfo.CurrentCulture);
        }

        public virtual List<InterpEvent> GetEvents(int lineWidth)
        {
            return GetEvents(CultureInfo.CurrentCulture, lineWidth);
        }

        public virtual List<InterpEvent> GetEvents(CultureInfo locale)
        {
            return GetEvents(locale, AutoIndentWriter.NoWrap);
        }

        public virtual List<InterpEvent> GetEvents(CultureInfo locale, int lineWidth)
        {
            StringWriter @out = new StringWriter();
            ITemplateWriter wr = new AutoIndentWriter(@out);
            wr.LineWidth = lineWidth;
            Interpreter interp = new Interpreter(groupThatCreatedThisInstance, locale);
            interp.Execute(wr, this); // Render and track events
            return interp.GetEvents();
        }
    }
}
