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

namespace Antlr4.StringTemplate.Misc
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using StringBuilder = System.Text.StringBuilder;

    /** Used during tests to track all errors */
    public class ErrorBuffer : ITemplateErrorListener
    {
        private readonly List<TemplateMessage> errors = new List<TemplateMessage>();

        public ReadOnlyCollection<TemplateMessage> Errors
        {
            get
            {
                return errors.AsReadOnly();
            }
        }

        protected List<TemplateMessage> ErrorList
        {
            get
            {
                return errors;
            }
        }

        public virtual void CompiletimeError(TemplateMessage msg)
        {
            errors.Add(msg);
        }

        public virtual void RuntimeError(TemplateMessage msg)
        {
            // ignore these
            if (msg.Error != ErrorType.NO_SUCH_PROPERTY)
                errors.Add(msg);
        }

        public virtual void IOError(TemplateMessage msg)
        {
            errors.Add(msg);
        }

        public virtual void InternalError(TemplateMessage msg)
        {
            errors.Add(msg);
        }

        public override string ToString()
        {
            StringBuilder buf = new StringBuilder();
            foreach (TemplateMessage m in errors)
                buf.AppendLine(m.ToString());

            return buf.ToString();
        }
    }
}
