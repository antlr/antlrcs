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
    using Antlr.Runtime;
    using Antlr4.StringTemplate.Compiler;
    using Antlr4.StringTemplate.Misc;
    using ArgumentException = System.ArgumentException;
    using Directory = System.IO.Directory;
    using Encoding = System.Text.Encoding;
    using Exception = System.Exception;
    using File = System.IO.File;
    using IOException = System.IO.IOException;
    using NotImplementedException = System.NotImplementedException;
    using Path = System.IO.Path;
    using StreamReader = System.IO.StreamReader;
    using Uri = System.Uri;
    using UriFormatException = System.UriFormatException;

    // TODO: caching?

    /** A directory or directory tree full of templates and/or group files.
     *  We load files on-demand. If we fail to find a file, we look for it via
     *  the CLASSPATH as a resource.  I track everything with URLs not file names.
     */
    public class TemplateGroupDirectory : TemplateGroup
    {
        public readonly string groupDirName;
        public readonly Uri root;

        public TemplateGroupDirectory(string dirName)
            : this(dirName, '<', '>')
        {
        }

        public TemplateGroupDirectory(string dirName, char delimiterStartChar, char delimiterStopChar)
            : base(delimiterStartChar, delimiterStopChar)
        {
            this.groupDirName = dirName;
            try
            {
                if (Directory.Exists(dirName))
                {
                    // we found the directory and it'll be file based
                    root = new Uri(dirName);
                }
                else
                {
                    throw new NotImplementedException();
#if false
                    ClassLoader cl = Thread.CurrentThread.getContextClassLoader();
                    root = cl.getResource(dirName);
                    if (root == null)
                    {
                        cl = this.GetType().getClassLoader();
                        root = cl.getResource(dirName);
                    }
                    if (root == null)
                    {
                        throw new ArgumentException("No such directory: " + dirName);
                    }
#endif
                }
            }
            catch (Exception e)
            {
                ErrorManager.InternalError(null, "can't Load group dir " + dirName, e);
            }
        }

        public TemplateGroupDirectory(string dirName, Encoding encoding)
            : this(dirName, encoding, '<', '>')
        {
        }

        public TemplateGroupDirectory(string dirName, Encoding encoding, char delimiterStartChar, char delimiterStopChar)
            : this(dirName, delimiterStartChar, delimiterStopChar)
        {
            this.Encoding = encoding;
        }

        public TemplateGroupDirectory(Uri root, Encoding encoding, char delimiterStartChar, char delimiterStopChar)
            : base(delimiterStartChar, delimiterStopChar)
        {
            this.root = root;
            this.Encoding = encoding;
        }

        /** Load a template from dir or group file.  Group file is given
         *  precedence over dir with same name.
         */
        protected override CompiledTemplate Load(string name)
        {
            string parent = Utility.GetPrefix(name);

            if (Path.IsPathRooted(parent))
                throw new ArgumentException();

            Uri groupFileURL = null;
            try
            {
                // see if parent of template name is a group file
                groupFileURL = new Uri(Path.Combine(root.LocalPath, parent) + ".stg");
            }
            catch (UriFormatException e)
            {
                ErrorManager.InternalError(null, "bad URL: " + Path.Combine(root.LocalPath, parent) + ".stg", e);
                return null;
            }

            if (!File.Exists(groupFileURL.LocalPath))
                return LoadTemplateFile(parent, name + ".st");
#if false
            InputStream @is = null;
            try
            {
                @is = groupFileURL.openStream();
            }
            catch (FileNotFoundException fnfe)
            {
                // must not be in a group file
                return loadTemplateFile(parent, name + ".st"); // load t.st file
            }
            catch (IOException ioe)
            {
                errMgr.internalError(null, "can't load template file " + name, ioe);
            }

            try
            {
                // clean up
                if (@is != null)
                    @is.close();
            }
            catch (IOException ioe)
            {
                errMgr.internalError(null, "can't close template file stream " + name, ioe);
            }
#endif

            LoadGroupFile(parent, Path.Combine(root.LocalPath, parent) + ".stg");

            return RawGetTemplate(name);
        }

        /** Load full path name .st file relative to root by prefix */
        public virtual CompiledTemplate LoadTemplateFile(string prefix, string fileName)
        {
            if (Path.IsPathRooted(fileName))
                throw new ArgumentException();

            //System.out.println("load "+fileName+" from "+root+" prefix="+prefix);
            string templateName = Path.ChangeExtension(fileName, null);
            Uri f = null;
            try
            {
                f = new Uri(Path.Combine(root.LocalPath, fileName));
            }
            catch (UriFormatException me)
            {
                ErrorManager.RuntimeError(null, 0, ErrorType.INVALID_TEMPLATE_NAME, me, Path.Combine(root.LocalPath, fileName));
                return null;
            }

            ANTLRReaderStream fs = null;
            try
            {
                fs = new ANTLRReaderStream(new StreamReader(f.LocalPath, Encoding ?? Encoding.UTF8));
            }
            catch (IOException)
            {
                // doesn't exist; just return null to say not found
                return null;
            }

            GroupLexer lexer = new GroupLexer(fs);
            fs.name = fileName;
            CommonTokenStream tokens = new CommonTokenStream(lexer);
            GroupParser parser = new GroupParser(tokens);
            parser.Group = this;
            lexer.group = this;
            try
            {
                parser.templateDef(prefix);
            }
            catch (RecognitionException re)
            {
                ErrorManager.GroupSyntaxError(ErrorType.SYNTAX_ERROR, Path.GetFileName(f.LocalPath), re, re.Message);
            }

            return RawGetTemplate(templateName);
        }

        public override string Name
        {
            get
            {
                return groupDirName;
            }
        }

        public override string FileName
        {
            get
            {
                return Path.GetFileName(root.LocalPath);
            }
        }
    }
}
