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

namespace Antlr4.StringTemplate
{
    using System.Runtime.CompilerServices;
    using Antlr4.StringTemplate.Compiler;
    using ArgumentException = System.ArgumentException;
    using Encoding = System.Text.Encoding;
    using Exception = System.Exception;
    using File = System.IO.File;
    using NotImplementedException = System.NotImplementedException;
    using Path = System.IO.Path;
    using Uri = System.Uri;

    /** The internal representation of a single group file (which must end in
     *  ".stg").  If we fail to find a group file, look for it via the
     *  CLASSPATH as a resource.
     */
    public class TemplateGroupFile : TemplateGroup
    {
        public readonly string fileName;
        public readonly Uri url;

        protected bool alreadyLoaded = false;

        /** Load a file relative to current dir or from root or via CLASSPATH. */
        public TemplateGroupFile(string fileName)
            : this(fileName, '<', '>')
        {
        }

        public TemplateGroupFile(string fileName, char delimiterStartChar, char delimiterStopChar)
            : base(delimiterStartChar, delimiterStopChar)
        {
            if (!fileName.EndsWith(".stg"))
                throw new ArgumentException("Group file names must end in .stg: " + fileName);

            try
            {
                //File f = new File(fileName);
                if (File.Exists(fileName))
                {
                    url = new Uri(fileName);
                }
                else
                {
                    throw new NotImplementedException();
#if false
                    // try in classpath
                    ClassLoader cl = Thread.currentThread().getContextClassLoader();
                    url = cl.getResource(fileName);
                    if (url == null)
                    {
                        cl = this.GetType().getClassLoader();
                        url = cl.getResource(fileName);
                    }
#endif
                }

                if (url == null)
                {
                    throw new ArgumentException("No such group file: " + fileName);
                }
            }
            catch (Exception e)
            {
                errMgr.internalError(null, "can't load group file " + fileName, e);
            }

            this.fileName = fileName;
        }

        public TemplateGroupFile(string fullyQualifiedFileName, Encoding encoding)
            : this(fullyQualifiedFileName, encoding, '<', '>')
        {
        }

        public TemplateGroupFile(string fullyQualifiedFileName, Encoding encoding, char delimiterStartChar, char delimiterStopChar)
            : this(fullyQualifiedFileName, delimiterStartChar, delimiterStopChar)
        {
            this.encoding = encoding;
        }

        public TemplateGroupFile(Uri url, Encoding encoding, char delimiterStartChar, char delimiterStopChar)
            : base(delimiterStartChar, delimiterStopChar)
        {
            this.url = url;
            this.encoding = encoding;
        }

        public override bool isDefined(string name)
        {
            if (!alreadyLoaded)
                load();
            return base.isDefined(name);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public override void unload()
        {
            base.unload();
            alreadyLoaded = false;
        }

        protected override CompiledTemplate load(string name)
        {
            if (!alreadyLoaded)
                load();

            CompiledTemplate template;
            templates.TryGetValue(name, out template);
            return template;
        }

        public override void load()
        {
            if (alreadyLoaded)
                return;

            alreadyLoaded = true; // do before actual load to say we're doing it
            // no prefix since this group file is the entire group, nothing lives
            // beneath it.
            loadGroupFile("", url.ToString());
        }

        public override string show()
        {
            if (!alreadyLoaded)
                load();

            return base.show();
        }

        public override string getName()
        {
            return Path.GetFileNameWithoutExtension(fileName);
        }

        public override string getFileName()
        {
            return fileName;
        }
    }
}
