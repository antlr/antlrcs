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
    using ArgumentException = System.ArgumentException;
    using Encoding = System.Text.Encoding;
    using Exception = System.Exception;
    using Path = System.IO.Path;
    using StringComparison = System.StringComparison;

    public class TemplateGroupFile : TemplateGroup
    {
        public string fileName;

        public TemplateGroupFile(string fullyQualifiedFileName)
        {
            if (!Path.GetExtension(fileName).Equals(".stg", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Group file names must end in .stg: " + fullyQualifiedFileName);
            }

            this.fileName = Path.GetFileName(fullyQualifiedFileName);
            this.parent = null;
            this.root = this;
            this.fullyQualifiedRootDirName = Path.GetFullPath(Path.GetDirectoryName(fullyQualifiedRootDirName));
        }

        public TemplateGroupFile(TemplateGroupDirectory parent, string fileName)
        {
            if (parent == null)
            {
                throw new ArgumentException("Relative dir " + fileName + " can't have null parent");
            }

            this.fileName = fileName;
            this.parent = parent;
            this.root = parent.root;
        }

        public TemplateGroupFile(TemplateGroupDirectory parent, string fileName, Encoding encoding)
            : this(parent, fileName)
        {
            this.encoding = encoding;
        }

        public override string GetName()
        {
            return Path.GetFileNameWithoutExtension(fileName);
        }

        public override CompiledTemplate LookupTemplate(string name)
        {
            if (name.Length > 0 && (name[0] == Path.DirectorySeparatorChar || name[0] == Path.AltDirectorySeparatorChar))
            {
                if (this != root)
                    return root.LookupTemplate(name);
                // if no root, name must be "/groupfile/templatename"
                string[] names = name.Split(new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar });
                string fname = Path.GetFileName(fileName);
                string @base = fname.Substring(0, fname.LastIndexOf('.'));
                if (names.Length > 2 || !names[0].Equals(@base))
                {
                    throw new ArgumentException("name must be of form /" + @base + "/templatename: " + name);
                }
            }
            if (name.IndexOfAny(new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }) >= 0)
            {
                throw new ArgumentException("can't use relative template name " + name);
            }

            // else plain old template name
            if (!alreadyLoaded)
                Load();

            CompiledTemplate template;
            if (!templates.TryGetValue(name, out template))
                return null;

            return template;
        }

        public override void Load()
        {
            if (alreadyLoaded)
                return;

            string fullFileName = GetPathFromRoot() + ".stg";

            try
            {
                ANTLRFileStream fs = new ANTLRFileStream(fullFileName, encoding);
                GroupLexer lexer = new GroupLexer(fs);
                UnbufferedTokenStream tokens = new UnbufferedTokenStream(lexer);
                GroupParser parser = new GroupParser(tokens);
                parser.group(this);
                alreadyLoaded = true;
            }
            catch (Exception e)
            {
                listener.Error("can't load group file: " + fullFileName, e);
            }
        }

        public override string Show()
        {
            if (!alreadyLoaded)
                Load();
            return base.Show();
        }
    }
}
