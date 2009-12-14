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
    using System.Linq;
    using Antlr.Runtime;
    using StringTemplate.Compiler;
    using StringTemplate.Debug;
    using ArgumentException = System.ArgumentException;
    using Console = System.Console;
    using Encoding = System.Text.Encoding;
    using Exception = System.Exception;
    using Path = System.IO.Path;
    using StringBuilder = System.Text.StringBuilder;
    using Type = System.Type;

    /** A directory or directory tree of .st template files and/or group files.
     *  Individual template files contain formal template definitions. In a sense,
     *  it's like a single group file broken into multiple files, one for each template.
     *  ST v3 had just the pure template inside, not the template name and header.
     *  Name inside must match filename (minus suffix).
     */
    public class TemplateGroup
    {
        /** When we use key as a value in a dictionary, this is how we signify. */
        public static readonly string DICT_KEY = "key";
        public static readonly string DEFAULT_KEY = "default";

        public string fullyQualifiedRootDirName;

        /** Load files using what encoding? */
        public Encoding encoding;

        /// <summary>
        /// Every group can import templates/dictionaries from other groups
        /// </summary>
        protected IList<TemplateGroup> imports;

        public List<string> interfaces;

        public char delimiterStartChar = '<'; // Use <expr> by default
        public char delimiterStopChar = '>';

        /** Maps template name to StringTemplate object */
        protected internal IDictionary<string, CompiledTemplate> templates =
            new Dictionary<string, CompiledTemplate>();

        /** Maps dict names to HashMap objects.  This is the list of dictionaries
         *  defined by the user like typeInitMap ::= ["int":"0"]
         */
        protected internal IDictionary<string, IDictionary<string, object>> dictionaries =
            new Dictionary<string, IDictionary<string, object>>();

        /** A dictionary that allows people to register a renderer for
         *  a particular kind of object for any template evaluated relative to this
         *  group.  For example, a date should be formatted differently depending
         *  on the locale.  You can set Date.class to an object whose
         *  toString(Object) method properly formats a Date attribute
         *  according to locale.  Or you can have a different renderer object
         *  for each locale.
         */
        protected IDictionary<Type, IAttributeRenderer> renderers;

        protected bool alreadyLoaded = false;

        public static TemplateGroup defaultGroup = new TemplateGroup();

        public TemplateGroup()
        {
        }

        public bool Debug
        {
            get;
            set;
        }

        public virtual string Name
        {
            get
            {
                return "<no name>;";
            }
        }

        public IDictionary<string, CompiledTemplate> Templates
        {
            get
            {
                return templates;
            }
        }

        /** The primary means of getting an instance of a template from this
         *  group. Must be absolute name like /a/b
         */
        public virtual Template GetInstanceOf(string name)
        {
            if (name[0] != '/')
                name = '/' + name;

            //Console.WriteLine("GetInstanceOf(" + name + ")");
            CompiledTemplate c = LookupTemplate(name);
            if (c != null)
            {
                Template instanceST = CreateStringTemplate();
                instanceST.groupThatCreatedThisInstance = this;
                instanceST.code = c;
                return instanceST;
            }
            return null;
        }

        public virtual Template GetEmbeddedInstanceOf(Template enclosingInstance, string name)
        {
            Template st = GetInstanceOf(name);
            if (st == null)
            {
                ErrorManager.RuntimeError(enclosingInstance, ErrorType.NoSuchTemplate, name);
                return Template.Blank;
            }
            st.enclosingInstance = enclosingInstance;
            return st;
        }

        public virtual CompiledTemplate LookupTemplate(string name)
        {
            if (!alreadyLoaded)
                Load();

            CompiledTemplate code;
            if (!templates.TryGetValue(name, out code))
            {
                code = LookupImportedTemplate(name);
            }

            return code;
        }

        protected internal CompiledTemplate LookupImportedTemplate(string name)
        {
            //Console.WriteLine("look for " + name + " in " + imports);

            if (imports == null)
                return null;

            foreach (var g in imports)
            {
                CompiledTemplate code = g.LookupTemplate(name);
                if (code != null)
                    return code;
            }

            return null;
        }

        public CompiledTemplate RawGetTemplate(string name)
        {
            CompiledTemplate template;
            if (!templates.TryGetValue(name, out template))
                return null;

            return template;
        }

        public IDictionary<string, object> RawGetDictionary(string name)
        {
            IDictionary<string, object> dictionary;
            if (!dictionaries.TryGetValue(name, out dictionary))
                return null;

            return dictionary;
        }

        // TODO: send in start/stop char or line/col so errors can be relative
        public CompiledTemplate DefineTemplate(string name, string template)
        {
            return DefineTemplate("/", name, null, template);
        }

        public virtual CompiledTemplate DefineTemplate(string name,
                                         List<string> args,
                                         string template)
        {
            IDictionary<string, FormalArgument> margs =
                new Dictionary<string, FormalArgument>();
            foreach (string a in args)
                margs[a] = new FormalArgument(a);
            return DefineTemplate("/", name, margs, template);
        }

        public virtual CompiledTemplate DefineTemplate(string name,
                                         string[] args,
                                         string template)
        {
            IDictionary<string, FormalArgument> margs =
                new Dictionary<string, FormalArgument>();
            foreach (string a in args)
                margs[a] = new FormalArgument(a);
            return DefineTemplate("/", name, margs, template);
        }

        // can't trap recog errors here; don't know where in file template is defined
        public virtual CompiledTemplate DefineTemplate(string prefix, string name, IDictionary<string, FormalArgument> args, string template)
        {
            if (name != null && (name.Length == 0 || name.IndexOf('.') >= 0))
            {
                throw new ArgumentException("cannot have '.' in template names");
            }

            CompiledTemplate code = Compile(prefix, name, template);
            code.name = name;
            code.formalArguments = args;
            RawDefineTemplate(prefix + name, code);
            if (args != null)
            { // compile any default args
                foreach (string a in args.Keys)
                {
                    FormalArgument fa = args[a];
                    if (fa.defaultValueToken != null)
                    {
                        TemplateCompiler c2 = new TemplateCompiler(prefix, name);
                        fa.compiledDefaultValue = c2.Compile(template);
                    }
                }
            }

            // define any anonymous subtemplates
            DefineImplicitlyDefinedTemplates(code);

            return code;
        }

        public CompiledTemplate DefineRegion(string prefix,
                                             string enclosingTemplateName,
                                             string name,
                                             string template)
        {
            CompiledTemplate code = Compile(prefix, enclosingTemplateName, template);
            code.name = prefix + GetMangledRegionName(enclosingTemplateName, name);
            code.isRegion = true;
            code.regionDefType = Template.RegionType.Explicit;
            RawDefineTemplate(code.name, code);
            return code;
        }

        protected void DefineImplicitlyDefinedTemplates(CompiledTemplate code)
        {
            if (code.implicitlyDefinedTemplates != null)
            {
                foreach (CompiledTemplate sub in code.implicitlyDefinedTemplates)
                {
                    RawDefineTemplate(sub.name, sub);
                    DefineImplicitlyDefinedTemplates(sub);
                }
            }
        }

        protected void RawDefineTemplate(string name, CompiledTemplate code)
        {
            CompiledTemplate prev;
            if (templates.TryGetValue(name, out prev))
            {
                if (!prev.isRegion)
                {
                    ErrorManager.CompileTimeError(ErrorType.TemplateRedefinition, name);
                    return;
                }
                if (prev.isRegion && prev.regionDefType == Template.RegionType.Embedded)
                {
                    ErrorManager.CompileTimeError(ErrorType.EmbeddedRegionRedefinition, GetUnmangledTemplateName(name));
                    return;
                }
                else if (prev.isRegion && prev.regionDefType == Template.RegionType.Explicit)
                {
                    ErrorManager.CompileTimeError(ErrorType.RegionRedefinition, GetUnmangledTemplateName(name));
                    return;
                }
            }
            templates[name] = code;
        }

        protected CompiledTemplate Compile(string prefix, string enclosingTemplateName, string template)
        {
            TemplateCompiler c = new TemplateCompiler(prefix, enclosingTemplateName);
            CompiledTemplate code = c.Compile(template);
            code.nativeGroup = this;
            code.template = template;
            return code;
        }

        /** The "foo" of t() ::= "&lt;@foo()&gt;" is mangled to "region#t#foo" */
        public static string GetMangledRegionName(string enclosingTemplateName,
                                                  string name)
        {
            return "region__" + enclosingTemplateName + "__" + name;
        }

        /// <summary>
        /// Return "t.foo" from "region__t__foo"
        /// </summary>
        public static string GetUnmangledTemplateName(string mangledName)
        {
            string t = mangledName.Substring("region__".Length, mangledName.LastIndexOf("__") - "region__".Length + 1);
            string r = mangledName.Substring(mangledName.LastIndexOf("__") + 2, mangledName.Length - mangledName.LastIndexOf("__") + 2 + 1);
            return t + '.' + r;
        }

        /** Define a map for this group; not thread safe...do not keep adding
         *  these while you reference them.
         */
        public virtual void DefineDictionary(string name, IDictionary<string, object> mapping)
        {
            dictionaries[name] = mapping;
        }

        /// <summary>
        /// Make this group import templates/dictionaries from <paramref name="g"/>.
        /// </summary>
        public virtual void ImportTemplates(TemplateGroup g)
        {
            if (g == null)
                return;

            if (imports == null)
                imports = new List<TemplateGroup>();

            imports.Add(g);
        }

        public virtual void Load()
        {
        }

        // TODO: make this happen in background then flip ptr to new list of templates/dictionaries?
        public virtual void LoadGroupFile(string prefix, string fileName)
        {
            string absoluteFileName = Path.Combine(fullyQualifiedRootDirName, fileName);
            //Console.WriteLine("load group file " + absoluteFileName);
            try
            {
                ANTLRFileStream fs = new ANTLRFileStream(absoluteFileName, encoding);
                GroupLexer lexer = new GroupLexer(fs);
                UnbufferedTokenStream tokens = new UnbufferedTokenStream(lexer);
                GroupParser parser = new GroupParser(tokens);
                parser.group(this, prefix);
            }
            catch (Exception e)
            {
                ErrorManager.IOError(null, ErrorType.CantLoadGroupFile, e, absoluteFileName);
            }
        }

        /** Register a renderer for all objects of a particular type for all
         *  templates evaluated relative to this group.
         */
        public void RegisterRenderer(Type attributeType, IAttributeRenderer r)
        {
            if (renderers == null)
                renderers = new Dictionary<Type, IAttributeRenderer>();

            renderers[attributeType] = r;
        }

        public IAttributeRenderer GetAttributeRenderer(Type attributeType)
        {
            if (renderers == null)
                return null;

            IAttributeRenderer renderer;
            if (!renderers.TryGetValue(attributeType, out renderer))
                return null;

            return renderer;
        }

        /// <summary>
        /// StringTemplate object factory; each group can have its own.
        /// </summary>
        public virtual Template CreateStringTemplate()
        {
            // TODO: try making a mem pool
            if (Debug)
                return new DebugTemplate();

            return new Template();
        }

        public override string ToString()
        {
            return Name;
        }

        public virtual string Show()
        {
            if (!alreadyLoaded)
                Load();

            StringBuilder buf = new StringBuilder();
            if (imports != null)
                buf.Append(" : " + imports);
            foreach (string name in templates.Keys)
            {
                CompiledTemplate c = templates[name];
                if (c.IsSubtemplate)
                    continue;

                int slash = name.LastIndexOfAny(new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar });
                string effectiveName = name.Substring(slash + 1);
                buf.Append(effectiveName);
                buf.Append('(');
                if (c.formalArguments != null)
                {
                    buf.Append(string.Join(",", c.formalArguments.Values.Select(value => value.ToString()).ToArray()));
                }
                buf.Append(')');
                buf.AppendLine(" ::= <<");
                buf.AppendLine(c.template);
                buf.AppendLine(">>");
            }
            return buf.ToString();
        }
    }
}
