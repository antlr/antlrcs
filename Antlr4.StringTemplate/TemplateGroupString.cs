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
    using Antlr.Runtime;
    using Antlr4.StringTemplate.Compiler;
    using Antlr4.StringTemplate.Misc;

    using Exception = System.Exception;

    /** A group derived from a string not a file or dir. */
    public class TemplateGroupString : TemplateGroup
    {
        private string sourceName;
        private string text;
        private bool alreadyLoaded = false;

        public TemplateGroupString(string text)
            : this("[string]", text, '<', '>')
        {
        }

        public TemplateGroupString(string sourceName, string text)
            : this(sourceName, text, '<', '>')
        {
        }

        public TemplateGroupString(string sourceName, string text, char delimiterStartChar, char delimiterStopChar)
            : base(delimiterStartChar, delimiterStopChar)
        {
            this.sourceName = sourceName;
            this.text = text;
        }

        public override string FileName
        {
            get
            {
                return sourceName;
            }
        }

        public override bool IsDefined(string name)
        {
            if (!alreadyLoaded)
                Load();

            return base.IsDefined(name);
        }

        public override void Load()
        {
            if (alreadyLoaded)
                return;

            alreadyLoaded = true;
            GroupParser parser;
            try
            {
                ANTLRStringStream fs = new ANTLRStringStream(text);
                fs.name = sourceName;
                GroupLexer lexer = new GroupLexer(fs);
                CommonTokenStream tokens = new CommonTokenStream(lexer);
                parser = new GroupParser(tokens);
                // no prefix since this group file is the entire group, nothing lives
                // beneath it.
                parser.group(this, "/");
            }
            catch (Exception e)
            {
                ErrorManager.IOError(null, ErrorType.CANT_LOAD_GROUP_FILE, e, FileName);
            }
        }

        protected override CompiledTemplate Load(string name)
        {
            if (!alreadyLoaded)
                Load();
            return RawGetTemplate(name);
        }
    }
}
