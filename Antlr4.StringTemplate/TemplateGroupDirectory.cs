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

                if (Verbose)
                    Console.WriteLine("TemplateGroupDirectory({0}) found at {1}", dirName, root);
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
            if (encoding == null)
                throw new ArgumentNullException("encoding");

            this.Encoding = encoding;
        }

        public TemplateGroupDirectory(Uri root, Encoding encoding, char delimiterStartChar, char delimiterStopChar)
            : base(delimiterStartChar, delimiterStopChar)
        {
            if (encoding == null)
                throw new ArgumentNullException("encoding");

            this.groupDirName = Path.GetFileName(root.AbsolutePath);
            this.root = root;
            this.Encoding = encoding;
        }

        public override void ImportTemplates(IToken fileNameToken)
        {
            string msg =
                "import illegal in group files embedded in TemplateGroupDirectory; " +
                "import " + fileNameToken.Text + " in TemplateGroupDirectory " + this.Name;
            throw new NotSupportedException(msg);
        }

        /** <summary>
         * Load a template from dir or group file.  Group file is given
         * precedence over dir with same name. <paramref name="name"/> is
         * always fully qualified.
         * </summary>
         */
        protected override CompiledTemplate Load(string name)
        {
            if (Verbose)
                Console.WriteLine("STGroupDir.load(" + name + ")");

            string parent = Utility.GetParent(name); // must have parent; it's fully-qualified
            string prefix = Utility.GetPrefix(name);

            //    	if (parent.isEmpty()) {
            //    		// no need to check for a group file as name has no parent
            //            return loadTemplateFile("/", name+TemplateFileExtension); // load t.st file
            //    	}

            if (!Path.IsPathRooted(parent))
                throw new ArgumentException();

            Uri groupFileURL;
            try
            {
                // see if parent of template name is a group file
                groupFileURL = new Uri(TemplateName.GetTemplatePath(root.LocalPath, parent) + GroupFileExtension);
            }
            catch (UriFormatException e)
            {
                ErrorManager.InternalError(null, "bad URL: " + TemplateName.GetTemplatePath(root.LocalPath, parent) + GroupFileExtension, e);
                return null;
            }

            if (!File.Exists(groupFileURL.LocalPath))
            {
                string unqualifiedName = Path.GetFileName(name);
                return LoadTemplateFile(prefix, unqualifiedName + TemplateFileExtension); // load t.st file
            }
#if false
            InputStream @is = null;
            try
            {
                @is = groupFileURL.openStream();
            }
            catch (FileNotFoundException fnfe)
            {
                // must not be in a group file
                return loadTemplateFile(parent, name + TemplateFileExtension); // load t.st file
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

            LoadGroupFile(prefix, groupFileURL.LocalPath);

            return RawGetTemplate(name);
        }

        /** Load full path name .st file relative to root by prefix */
        public virtual CompiledTemplate LoadTemplateFile(string prefix, string unqualifiedFileName)
        {
            if (Path.IsPathRooted(unqualifiedFileName))
                throw new ArgumentException();

            if (Verbose)
                Console.WriteLine("loadTemplateFile({0}) in groupdir from {1} prefix={2}", unqualifiedFileName, root, prefix);

            string templateName = Path.ChangeExtension(unqualifiedFileName, null);
            Uri f;
            try
            {
                f = new Uri(root.LocalPath + prefix + unqualifiedFileName);
            }
            catch (UriFormatException me)
            {
                ErrorManager.RuntimeError(null, ErrorType.INVALID_TEMPLATE_NAME, me, Path.Combine(root.LocalPath, unqualifiedFileName));
                return null;
            }

            ANTLRReaderStream fs = null;
            try
            {
                fs = new ANTLRReaderStream(new StreamReader(File.OpenRead(f.LocalPath), Encoding));
                fs.name = unqualifiedFileName;
            }
            catch (IOException)
            {
                if (Verbose)
                    Console.WriteLine("{0}/{1} doesn't exist", root, unqualifiedFileName);

                //errMgr.IOError(null, ErrorType.NO_SUCH_TEMPLATE, ioe, unqualifiedFileName);
                return null;
            }

            return LoadTemplateFile(prefix, unqualifiedFileName, fs);
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

        public override Uri RootDirUri
        {
            get
            {
                return root;
            }
        }
    }
}
