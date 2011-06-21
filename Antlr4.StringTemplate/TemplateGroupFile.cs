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
    using System.Runtime.CompilerServices;
    using Antlr4.StringTemplate.Compiler;
    using Antlr4.StringTemplate.Extensions;

    using ArgumentException = System.ArgumentException;
    using ArgumentNullException = System.ArgumentNullException;
    using Console = System.Console;
    using Encoding = System.Text.Encoding;
    using ErrorType = Antlr4.StringTemplate.Misc.ErrorType;
    using Exception = System.Exception;
    using File = System.IO.File;
    using FileNotFoundException = System.IO.FileNotFoundException;
    using Path = System.IO.Path;
    using Uri = System.Uri;
    using UriFormatException = System.UriFormatException;
    using Utility = Antlr4.StringTemplate.Misc.Utility;

    /** The internal representation of a single group file (which must end in
     *  ".stg").  If we fail to find a group file, look for it via the
     *  CLASSPATH as a resource.
     */
    public class TemplateGroupFile : TemplateGroup
    {
        private readonly string _fileName;
        private readonly Uri _url;
        private bool _alreadyLoaded = false;

        /** Load a file relative to current dir or from root or via CLASSPATH. */
        public TemplateGroupFile(string fileName)
            : this(fileName, '<', '>')
        {
        }

        public TemplateGroupFile(string fileName, char delimiterStartChar, char delimiterStopChar)
            : base(delimiterStartChar, delimiterStopChar)
        {
            if (fileName == null)
                throw new ArgumentNullException("fileName");

            try
            {
                if (!fileName.EndsWith(".stg"))
                    throw new ArgumentException("Group file names must end in .stg: " + fileName);

                if (!File.Exists(fileName))
                    throw new FileNotFoundException(string.Format("No such group file: {0}", fileName));

                this._url = new Uri(fileName);
                this._fileName = fileName;

                if (Verbose)
                    Console.WriteLine("STGroupFile({0}) == file {1}", fileName, Path.GetFullPath(fileName));
            }
            catch (Exception e)
            {
                e.PreserveStackTrace();
                if (!e.IsCritical())
                    ErrorManager.InternalError(null, "can't Load group file " + fileName, e);

                throw;
            }
        }

        public TemplateGroupFile(string fullyQualifiedFileName, Encoding encoding)
            : this(fullyQualifiedFileName, encoding, '<', '>')
        {
        }

        public TemplateGroupFile(string fullyQualifiedFileName, Encoding encoding, char delimiterStartChar, char delimiterStopChar)
            : this(fullyQualifiedFileName, delimiterStartChar, delimiterStopChar)
        {
            this.Encoding = encoding;
        }

        public TemplateGroupFile(Uri url, Encoding encoding, char delimiterStartChar, char delimiterStopChar)
            : base(delimiterStartChar, delimiterStopChar)
        {
            if (url == null)
                throw new ArgumentNullException("url");

            this._url = url;
            this.Encoding = encoding;
            this._fileName = _url.AbsolutePath;
        }

        public override bool IsDefined(string name)
        {
            if (!_alreadyLoaded)
                Load();
            return base.IsDefined(name);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public override void Unload()
        {
            base.Unload();
            _alreadyLoaded = false;
        }

        protected override CompiledTemplate Load(string name)
        {
            if (!_alreadyLoaded)
                Load();

            return RawGetTemplate(name);
        }

        public override void Load()
        {
            if (_alreadyLoaded)
                return;

            // do before actual load to say we're doing it
            // no prefix since this group file is the entire group, nothing lives
            // beneath it.
            _alreadyLoaded = true;

            if (Verbose)
                Console.WriteLine("loading group file " + _url.LocalPath);

            LoadGroupFile("/", _url.LocalPath);

            if (Verbose)
                Console.WriteLine("found {0} templates in {1} = {2}", CompiledTemplates.Count, _url.ToString(), CompiledTemplates);
        }

        public override string Show()
        {
            if (!_alreadyLoaded)
                Load();

            return base.Show();
        }

        public override string Name
        {
            get
            {
                return Path.GetFileNameWithoutExtension(_fileName);
            }
        }

        public override string FileName
        {
            get
            {
                return _fileName;
            }
        }

        public override Uri RootDirUri
        {
            get
            {
                //System.out.println("url of "+fileName+" is "+url.toString());
                string parent = Path.GetDirectoryName(_url.ToString());
                try
                {
                    return new Uri(parent);
                }
                catch (UriFormatException mue)
                {
                    ErrorManager.RuntimeError(null, ErrorType.INVALID_TEMPLATE_NAME, mue, parent);
                }

                return null;
            }
        }
    }
}
