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
    using System.Collections.Generic;
    using ArgumentException = System.ArgumentException;
    using Directory = System.IO.Directory;
    using File = System.IO.File;
    using Path = System.IO.Path;
    using Encoding = System.Text.Encoding;
    using Exception = System.Exception;
    using Console = System.Console;
    using Antlr.Runtime;

    public class TemplateTree
    {
        public string fullyQualifiedRootDirName; // if we're root

        /** Load files using what encoding? */
        public Encoding encoding;

        /** Every group can import templates/dictionaries from other groups */
        protected IList<TemplateGroup> imports;

        /** Maps template name to StringTemplate object */
        protected IDictionary<string, CompiledTemplate> templates =
            new Dictionary<string, CompiledTemplate>();

        public char delimiterStartChar = '<'; // Use <expr> by default
        public char delimiterStopChar = '>';

        protected bool alreadyLoaded = false;

        public TemplateTree(string fullyQualifiedRootDirName)
        {
            this.fullyQualifiedRootDirName = fullyQualifiedRootDirName;
            if (!Directory.Exists(fullyQualifiedRootDirName))
            {
                throw new ArgumentException("No such directory: " + fullyQualifiedRootDirName);
            }
        }

        /** The primary means of getting an instance of a template from this
         *  group. name must be fully qualified, absolute like "/a/b".
         */
        public Template GetInstanceOf(string name)
        {
            CompiledTemplate c = LookupTemplate(name);
            if (c != null)
            {
                Template instanceST = CreateStringTemplate();
                //instanceST.groupThatCreatedThisInstance = this;
                instanceST.name = name;
                instanceST.code = c;
                return instanceST;
            }
            return null;
        }

        public Template GetEmbeddedInstanceOf(Template enclosingInstance, string name)
        {
            Template st = GetInstanceOf(name);
            if (st == null)
            {
                Console.Error.WriteLine("no such template: " + name);
                return Template.Blank;
            }
            st.enclosingInstance = enclosingInstance;
            return st;
        }

        // can't trap recog errors here; don't know where in file template is defined
        public CompiledTemplate DefineTemplate(string name,
                                         IDictionary<string, FormalArgument> args,
                                         string template)
        {
            if (name != null && (name.Length == 0 || name.IndexOf('.') >= 0))
            {
                throw new ArgumentException("cannot have '.' in template names");
            }
            Compiler c = new Compiler();
            CompiledTemplate code = c.Compile(template);
            code.name = name;
            code.formalArguments = args;
            //code.nativeGroup = this;
            templates[name] = code;
            if (args != null)
            { // compile any default args
                foreach (var pair in args)
                {
                    FormalArgument fa = pair.Value;
                    if (fa.defaultValue != null)
                    {
                        Compiler c2 = new Compiler();
                        fa.compiledDefaultValue = c2.Compile(template);
                    }
                }
            }
            // define any anonymous subtemplates
            DefineAnonSubtemplates(code);

            return code;
        }

        public void DefineAnonSubtemplates(CompiledTemplate code)
        {
            if (code.compiledSubtemplates != null)
            {
                foreach (CompiledTemplate sub in code.compiledSubtemplates)
                {
                    templates[sub.name] = sub;
                    DefineAnonSubtemplates(sub);
                }
            }
        }

        public CompiledTemplate LookupTemplate(string name)
        {
            if (!alreadyLoaded)
                Load();

            CompiledTemplate code;
            if (!templates.TryGetValue(name, out code))
                return null;

            return code;
        }

        public void Load()
        {
            // walk dir and all subdir to load templates, group files
            _Load("");
            alreadyLoaded = true;
        }

        protected void _Load(string prefix)
        {
            // walk dir and all subdir to load templates, group files
            string dir = Path.Combine(fullyQualifiedRootDirName, prefix);
            foreach (var d in Directory.GetDirectories(dir))
            {
                _Load(Path.Combine(prefix, Path.GetFileName(d)));
            }

            foreach (var f in Directory.GetFiles(dir))
            {
                if (Path.GetExtension(f).Equals(".st", System.StringComparison.OrdinalIgnoreCase))
                    LoadTemplateFile(prefix, Path.GetFileName(f));
            }
        }

        public CompiledTemplate LoadTemplateFile(string prefix, string fileName)
        {
            // load from disk
            string absoluteFileName = Path.Combine(Path.Combine(fullyQualifiedRootDirName, prefix), fileName);
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
                //parser.group = this;
                //parser.templateDef();
                return LookupTemplate("/" + Path.Combine(prefix, Path.GetFileNameWithoutExtension(fileName)));
            }
            catch (Exception)
            {
                Console.Error.WriteLine("can't load template file: " + absoluteFileName);
            }
            return null;
        }

        /** StringTemplate object factory; each group can have its own. */
        public virtual Template CreateStringTemplate()
        {
            Template st = new Template();
            return st;
        }
    }
}
