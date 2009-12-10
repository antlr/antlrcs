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
    using System.Linq;
    using Antlr.Runtime;
    using ArgumentException = System.ArgumentException;
    using Directory = System.IO.Directory;
    using Encoding = System.Text.Encoding;
    using Exception = System.Exception;
    using File = System.IO.File;
    using Path = System.IO.Path;

    public class TemplateGroupDirectory : TemplateGroup
    {
        public string dir;

        public TemplateGroupDirectory(string dirName)
        {
            dir = dirName;
            if (!Directory.Exists(dir))
            {
                throw new ArgumentException("No such directory: " + dirName);
            }
        }

        public TemplateGroupDirectory(TemplateGroup root, string dirName)
            : this(dirName)
        {
            this.root = root;
        }

        public TemplateGroupDirectory(TemplateGroup root, string dirName, Encoding encoding)
            : this(root, dirName)
        {
            this.encoding = encoding;
        }

        public override CompiledTemplate LookupTemplate(string name)
        {
            if (name.Length > 0 && (name[0] == Path.DirectorySeparatorChar || name[0] == Path.AltDirectorySeparatorChar))
            {
                if (root != null)
                    return root.LookupTemplate(name);
                // strip '/' and try again; we're root
                //return lookupTemplate(name.substring(1));
                name = name.Substring(1);
                string[] names = name.Split(new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar });
                if (!names[0].Equals(Path.GetFileName(dir)))
                {
                    throw new ArgumentException(names[0] + " doesn't match directory name " + Path.GetFileName(dir));
                }
            }
            if (name.IndexOfAny(new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }) >= 0)
                return LookupQualifiedTemplate(dir, name);

            // else plain old template name, check if already here
            CompiledTemplate code;
            if (templates.TryGetValue(name, out code))
                return code;

            return LookupTemplateFile(name);
        }

        /** Look up template name with '/' anywhere but first char */
        protected virtual CompiledTemplate LookupQualifiedTemplate(string dir, string name)
        {
            // TODO: slow to load a template!
            string[] names = name.Split(new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar });
            string templateFile = Path.Combine(dir, names[0] + ".st");
            if (templates.ContainsKey(names[0]) || File.Exists(templateFile))
            {
                throw new ArgumentException(names[0] + " is a template not a dir or group file");
            }
            // look for a directory or group file called names[0]
            TemplateGroup sub = null;
            string subF = Path.Combine(dir, names[0]);
            if (Directory.Exists(subF))
            {
                sub = new TemplateGroupDirectory(root, Path.Combine( dir , names[0]));
            }
            else if (File.Exists(Path.Combine(dir, names[0] + ".stg")))
            {
                try
                {
                    sub = new TemplateGroupFile(Path.Combine(dir, names[0] + ".stg"));
                }
                catch (Exception e)
                {
                    listener.Error("can't load group file: " + names[0] + ".stg", e);
                }
            }
            else
            {
                throw new ArgumentException("no such subgroup: " + names[0]);
            }
            string allButFirstName = string.Join(Path.DirectorySeparatorChar.ToString(), names.Skip(1).ToArray());
            return sub.LookupTemplate(allButFirstName);
        }

        public virtual CompiledTemplate LookupTemplateFile(string name)
        {
            // not in templates list, load it from disk
            string f = Path.Combine(dir, name + ".st");
            if (!File.Exists(f))
            { // TODO: add tolerance check here
                throw new ArgumentException("no such template: " + name);
            }
            try
            {
                ANTLRFileStream fs = new ANTLRFileStream(f, encoding);
                GroupLexer lexer = new GroupLexer(fs);
                CommonTokenStream tokens = new CommonTokenStream(lexer);
                GroupParser parser = new GroupParser(tokens);
                parser._group = this;
                parser.templateDef();
                return templates[name];
            }
            catch (Exception e)
            {
                listener.Error("can't load template file: " + Path.Combine(f, name), e);
            }
            return null;
        }

        public override string GetName()
        {
            return Path.GetFileName(dir);
        }

    }
}
