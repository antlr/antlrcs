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

namespace Antlr4.StringTemplate
{
    using System.Collections.Generic;
    using Antlr4.StringTemplate.Debug;
    using StringBuilder = System.Text.StringBuilder;

    public sealed class TemplateFrame
    {
        private readonly Template _template;
        private readonly TemplateFrame _parent;
        private readonly int _depth;
        private int _ip;
        private DebugEvents _debugState;

        public TemplateFrame(Template template, TemplateFrame parent)
        {
            _template = template;
            _parent = parent;

            _depth = (parent != null) ? parent._depth + 1 : 1;

            if (parent != null && parent._debugState != null && parent._debugState.IsEarlyEval)
                GetDebugState().IsEarlyEval = true;
        }

        public Template Template
        {
            get
            {
                return _template;
            }
        }

        public TemplateFrame Parent
        {
            get
            {
                return _parent;
            }
        }

        public int StackDepth
        {
            get
            {
                return _depth;
            }
        }

        public int InstructionPointer
        {
            get
            {
                return _ip;
            }

            set
            {
                _ip = value;
            }
        }

        public DebugEvents GetDebugState()
        {
            _debugState = _debugState ?? new DebugEvents();
            return _debugState;
        }

        /** If an instance of x is enclosed in a y which is in a z, return
         *  a String of these instance names in order from topmost to lowest;
         *  here that would be "[z y x]".
         */
        public string GetEnclosingInstanceStackString()
        {
            List<Template> templates = GetEnclosingInstanceStack(true);
            StringBuilder buf = new StringBuilder();
            int i = 0;
            foreach (Template st in templates)
            {
                if (i > 0)
                    buf.Append(" ");
                buf.Append(st.Name);
                i++;
            }

            return buf.ToString();
        }

        public List<Template> GetEnclosingInstanceStack(bool topdown)
        {
            List<Template> stack = new List<Template>();
            TemplateFrame p = this;
            while (p != null)
            {
                if (topdown)
                    stack.Insert(0, p.Template);
                else
                    stack.Add(p.Template);

                p = p.Parent;
            }
            return stack;
        }

        public List<TemplateFrame> GetFrameStack(bool topdown)
        {
            List<TemplateFrame> stack = new List<TemplateFrame>();
            for (TemplateFrame p = this; p != null; p = p.Parent)
                stack.Add(p);

            if (topdown)
                stack.Reverse();

            return stack;
        }

        public List<EvalTemplateEvent> GetEvalTemplateEventStack(bool topdown)
        {
            List<EvalTemplateEvent> stack = new List<EvalTemplateEvent>();
            for (TemplateFrame p = this; p != null; p = p.Parent)
            {
                EvalTemplateEvent eval = (EvalTemplateEvent)p.GetDebugState().Events[p.GetDebugState().Events.Count - 1];
                stack.Add(eval);
            }

            if (topdown)
                stack.Reverse();

            return stack;
        }
    }
}
