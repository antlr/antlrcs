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
            _Load("/");
            alreadyLoaded = true;
        }

        protected void _Load(string prefix)
        {
            string dir = Path.Combine(fullyQualifiedRootDirName, prefix);
            //Console.WriteLine("load dir '" + prefix + "' under " + fullyQualifiedRootDirName);

            foreach (var d in Directory.GetDirectories(dir))
            {
                _Load(Path.Combine(prefix, Path.GetFileName(d)) + "/");
            }

            foreach (var f in Directory.GetFiles(dir))
            {
                if (Path.GetExtension(f).Equals(".st", System.StringComparison.OrdinalIgnoreCase))
                    LoadTemplateFile(prefix, Path.GetFileName(f));
                else if (Path.GetExtension(f).Equals(".stg", System.StringComparison.OrdinalIgnoreCase))
                    LoadGroupFile(Path.Combine(prefix, Path.GetFileNameWithoutExtension(f)) + "/", Path.Combine(prefix, Path.GetFileName(f)));
            }
        }

        public virtual CompiledTemplate LoadTemplateFile(string prefix, string fileName)
        {
            // load from disk
            string absoluteFileName = Path.Combine(Path.Combine(fullyQualifiedRootDirName, prefix), fileName);
            //Console.WriteLine("load " + absoluteFileName);
            if (!File.Exists(absoluteFileName))
            {
                // TODO: add tolerance check here
                return null;
            }
            try
            {
                ANTLRFileStream fs = new ANTLRFileStream(absoluteFileName, encoding);
                GroupLexer lexer = new GroupLexer(fs);
                UnbufferedTokenStream tokens = new UnbufferedTokenStream(lexer);
                GroupParser parser = new GroupParser(tokens);
                parser._group = this;
                parser.templateDef(prefix);

                CompiledTemplate code;
                if (!templates.TryGetValue("/" + Path.Combine(prefix, Path.GetFileNameWithoutExtension(fileName)), out code))
                    return null;

                return code;
            }
            catch (Exception e)
            {
                ErrorManager.Error("can't load template file: " + absoluteFileName);
                Console.Error.WriteLine(e.StackTrace);
            }
            return null;
        }
    }
}
