/*
 * [The "BSD license"]
 * Copyright (c) 2012 Terence Parr
 * All rights reserved.
 *
 * Conversion to C#:
 * Copyright (c) 2012 Sam Harwell, Tunnel Vision Laboratories, LLC
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

    using ArgumentException = System.ArgumentException;
    using ArgumentNullException = System.ArgumentNullException;
    using Console = System.Console;
    using Directory = System.IO.Directory;
    using Encoding = System.Text.Encoding;
    using Exception = System.Exception;
    using File = System.IO.File;
    using IOException = System.IO.IOException;
    using NotImplementedException = System.NotImplementedException;
    using NotSupportedException = System.NotSupportedException;
    using Path = System.IO.Path;
    using StreamReader = System.IO.StreamReader;
    using Uri = System.Uri;
    using UriFormatException = System.UriFormatException;

    // TODO: caching?

    /** A dir of templates w/o headers like ST v3 had.  Still allows group files
     *  in dir though like TemplateGroupDirectory parent.
     */
    public class TemplateRawGroupDirectory : TemplateGroupDirectory
    {
        public TemplateRawGroupDirectory(string dirName)
            : base(dirName)
        {
        }

        public TemplateRawGroupDirectory(string dirName, char delimiterStartChar, char delimiterStopChar)
            : base(dirName, delimiterStartChar, delimiterStopChar)
        {
        }

        public TemplateRawGroupDirectory(string dirName, Encoding encoding)
            : base(dirName, encoding)
        {
        }

        public TemplateRawGroupDirectory(string dirName, Encoding encoding, char delimiterStartChar, char delimiterStopChar)
            : base(dirName, encoding, delimiterStartChar, delimiterStopChar)
        {
        }

        public TemplateRawGroupDirectory(Uri root, Encoding encoding, char delimiterStartChar, char delimiterStopChar)
            : base(root, encoding, delimiterStartChar, delimiterStopChar)
        {
        }

        public override CompiledTemplate LoadTemplateFile(string prefix, string unqualifiedFileName,
                                           ICharStream templateStream)
        {
            string template = templateStream.Substring(0, templateStream.Count);
            string templateName = Path.GetFileNameWithoutExtension(unqualifiedFileName);
            string fullyQualifiedTemplateName = prefix + templateName;
            CompiledTemplate impl = new TemplateCompiler(this).Compile(fullyQualifiedTemplateName, template);
            CommonToken nameT = new CommonToken(TemplateLexer.SEMI); // Seems like a hack, best I could come up with.
            nameT.InputStream = templateStream;
            RawDefineTemplate(fullyQualifiedTemplateName, impl, nameT);
            impl.DefineImplicitlyDefinedTemplates(this);
            return impl;
        }
    }
}
