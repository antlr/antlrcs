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

namespace Antlr4.StringTemplate.Misc
{
    using Exception = System.Exception;
    using StringBuilder = System.Text.StringBuilder;

    /** Used to track errors that occur in the Template interpreter. */
    public class TemplateRuntimeMessage : TemplateMessage
    {
        /** Where error occurred in bytecode memory */
        private readonly int ip = -1;

        private readonly TemplateFrame _frame;

        public TemplateRuntimeMessage(ErrorType error, int ip)
            : this(error, ip, null)
        {
        }

        public TemplateRuntimeMessage(ErrorType error, int ip, TemplateFrame frame)
            : this(error, ip, frame, null)
        {
        }

        public TemplateRuntimeMessage(ErrorType error, int ip, TemplateFrame frame, object arg)
            : this(error, ip, frame, null, arg, null)
        {
        }

        public TemplateRuntimeMessage(ErrorType error, int ip, TemplateFrame frame, Exception e, object arg)
            : this(error, ip, frame, e, arg, null)
        {
        }

        public TemplateRuntimeMessage(ErrorType error, int ip, TemplateFrame frame, Exception e, object arg, object arg2)
            : base(error, frame?.Template, e, arg, arg2)
        {
            this.ip = ip;
            this._frame = frame;
        }

        public TemplateRuntimeMessage(ErrorType error, int ip, TemplateFrame frame, Exception e, object arg, object arg2, object arg3)
            : base(error, frame?.Template, e, arg, arg2, arg3)
        {
            this.ip = ip;
            this._frame = frame;
        }

        public TemplateFrame Frame
        {
            get
            {
                return _frame;
            }
        }

        public Interval SourceInterval
        {
            get
            {
                if (ip < 0 || Self == null || Self.impl == null || Self.impl.sourceMap == null || ip >= Self.impl.sourceMap.Length)
                    return null;

                return Self.impl.sourceMap[ip];
            }
        }

        /** Given an ip (code location), get it's range in source template then
         *  return it's template line:col.
         */
        public virtual string GetSourceLocation()
        {
            Interval interval = SourceInterval;
            if (interval == null)
                return null;

            // get left edge and get line/col
            int i = interval.Start;
            Coordinate loc = Utility.GetLineCharPosition(Self.impl.Template, i);
            return loc.ToString();
        }

        public override string ToString()
        {
            StringBuilder buf = new StringBuilder();
            string loc = null;
            if (_frame != null)
            {
                loc = GetSourceLocation();
                buf.Append("context [");
                buf.Append(_frame.GetEnclosingInstanceStackString());
                buf.Append("]");
            }
            if (loc != null)
                buf.Append(" " + loc);
            buf.Append(" " + base.ToString());
            return buf.ToString();
        }
    }
}
