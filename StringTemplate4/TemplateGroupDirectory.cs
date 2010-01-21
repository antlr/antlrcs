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
    using Antlr.Runtime;
    using StringTemplate.Compiler;
    using ArgumentException = System.ArgumentException;
    using Console = System.Console;
    using Directory = System.IO.Directory;
    using Encoding = System.Text.Encoding;
    using Exception = System.Exception;
    using File = System.IO.File;
    using Path = System.IO.Path;
    using ArgumentNullException = System.ArgumentNullException;

    public class TemplateGroupDirectory : TemplateGroup
    {
        public string dirName;

        public TemplateGroupDirectory(string fullyQualifiedRootDirName)
        {
            this.fullyQualifiedRootDirName = fullyQualifiedRootDirName;
            this.dirName = "/"; // it's the root
            if (!Directory.Exists(fullyQualifiedRootDirName))
            {
                throw new ArgumentException("No such directory: " + fullyQualifiedRootDirName);
            }
        }

        public TemplateGroupDirectory(string fullyQualifiedRootDirName, Encoding encoding)
            : this(fullyQualifiedRootDirName)
        {
            this.encoding = encoding;
        }

        public override string Name
        {
            get
            {
                return dirName;
            }
        }

        /// <summary>
        /// walk dir and all subdir to load templates, group files
        /// </summary>
        public override void Load()
        {
            _Load(TemplateName.Root);
            alreadyLoaded = true;
        }

        protected void _Load(TemplateName prefix)
        {
            if (prefix == null)
                throw new ArgumentNullException("prefix");
            if (!prefix.IsRooted)
                throw new ArgumentException();

            string relativePrefix = prefix.FullName.Substring(1);

            string dir = Path.Combine(fullyQualifiedRootDirName, relativePrefix);
            //Console.WriteLine("load dir '" + prefix + "' under " + fullyQualifiedRootDirName);

            foreach (var d in Directory.GetDirectories(dir))
            {
                _Load(TemplateName.Combine(prefix, Path.GetFileName(d)));
            }

            foreach (var f in Directory.GetFiles(dir))
            {
                if (Path.GetExtension(f).Equals(".st", System.StringComparison.OrdinalIgnoreCase))
                    LoadTemplateFile(prefix, Path.GetFileName(f));
                else if (Path.GetExtension(f).Equals(".stg", System.StringComparison.OrdinalIgnoreCase))
                    LoadGroupFile(TemplateName.Combine(prefix, Path.GetFileNameWithoutExtension(f)), Path.Combine(relativePrefix, Path.GetFileName(f)));
            }
        }

        public CompiledTemplate LoadTemplateFile(TemplateName prefix, string fileName)
        {
            if (prefix == null)
                throw new ArgumentNullException("prefix");
            if (fileName == null)
                throw new ArgumentNullException("fileName");
            if (!prefix.IsRooted)
                throw new ArgumentException("Expected the prefix to be a rooted name.", "prefix");

            TemplateName templateName = TemplateName.Combine(prefix, new TemplateName(Path.GetFileNameWithoutExtension(fileName)));

            // load from disk
            string absoluteFileName = Path.Combine(Path.Combine(fullyQualifiedRootDirName, prefix.FullName.Substring(1)), fileName);

            //Console.WriteLine("load " + absoluteFileName);
            if (!File.Exists(absoluteFileName))
            {
                // TODO: add tolerance check here
                return null;
            }
            try
            {
                if (ErrorManager.CompatibilityMode)
                {
                    string template = File.ReadAllText(absoluteFileName);
                    template = template.Trim();
                    DefineTemplate(prefix, new CommonToken(GroupParser.ID, templateName.Name), null, template);
                }
                else
                {
                    ANTLRFileStream fs = new ANTLRFileStream(absoluteFileName, encoding);
                    GroupLexer lexer = new GroupLexer(fs);
                    CommonTokenStream tokens = new CommonTokenStream(lexer);
                    GroupParser parser = new GroupParser(tokens);
                    parser._group = this;
                    parser.templateDef(prefix);
                }

                CompiledTemplate code;
                if (!templates.TryGetValue(templateName, out code))
                    return null;

                return code;
            }
            catch (Exception e)
            {
                if (ErrorManager.IsCriticalException(e))
                    throw;

                ErrorManager.IOError(null, ErrorType.CantLoadTemplateFile, e, absoluteFileName);
                Console.Error.WriteLine(e.StackTrace);
            }
            return null;
        }
    }
}
