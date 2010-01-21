/*
 * [The "BSD licence"]
 * Copyright (c) 2005-2008 Terence Parr
 * All rights reserved.
 *
 * Conversion to C#:
 * Copyright (c) 2008-2009 Sam Harwell
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

namespace StringTemplate
{
    using StringTemplate.Compiler;
    using StringTemplate.Debug;
    using CultureInfo = System.Globalization.CultureInfo;

    public class BlankTemplate : DebugTemplate
    {
        // derive from DebugTemplate not just Template in case we're debugging

        public BlankTemplate()
        {
            code = new CompiledTemplate();
        }

        public BlankTemplate(string template)
            : this()
        {
        }

        public string Name
        {
            get
            {
                return "blank";
            }
        }

        public override void Add(string name, object value)
        {
        }

        protected internal override void RawSetAttribute(string name, object value)
        {
        }

        public override object GetAttribute(string name)
        {
            return null;
        }

        public override string GetEnclosingInstanceStackString()
        {
            return null;
        }

        public override int Write(ITemplateWriter @out)
        {
            return 0;
        }

        public override string Render(CultureInfo culture, int lineWidth)
        {
            return string.Empty;
        }
    }
}
