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
    using ArgumentException = System.ArgumentException;
    using Console = System.Console;
    using Directory = System.IO.Directory;
    using Encoding = System.Text.Encoding;
    using Exception = System.Exception;
    using Path = System.IO.Path;
    using StringBuilder = System.Text.StringBuilder;
    using Type = System.Type;
    using Antlr.Runtime;

    public class TemplateGroup
    {
        /** When we use key as a value in a dictionary, this is how we signify. */
        public static readonly string DICT_KEY = "key";
        public static readonly string DEFAULT_KEY = "default";

        private class DefaultErrorListenerImpl : ITemplateErrorListener
        {
            public void Error(string message, Exception e)
            {
                Console.Error.WriteLine(message);
                if (e != null)
                    Console.Error.WriteLine(e.StackTrace);
            }

            public void Error(string message)
            {
                Error(message, null);
            }

            public void Warning(string message)
            {
                Console.WriteLine(message);
            }
        }

        public static readonly ITemplateErrorListener DefaultErrorListener = new DefaultErrorListenerImpl();

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

        /** Where to report errors.  All string templates in this group
         *  use this error handler by default.
         */
        public ITemplateErrorListener listener = DefaultErrorListener;

        public static ErrorTolerance DEFAULT_ERROR_TOLERANCE = new ErrorTolerance();
        public ErrorTolerance tolerance = DEFAULT_ERROR_TOLERANCE;

        public static TemplateGroup defaultGroup = new TemplateGroup();

        public TemplateGroup()
        {
        }

        public virtual string Name
        {
            get
            {
                return "<no name>;";
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
                Console.Error.WriteLine("no such template: " + name);
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

        protected CompiledTemplate LookupImportedTemplate(string name)
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

        // TODO: send in start/stop char or line/col so errors can be relative
        public CompiledTemplate DefineTemplate(string name, string template)
        {
            return DefineTemplate("/", name, (IDictionary<string, FormalArgument>)null, template);
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
            Compiler c = new Compiler(prefix);
            CompiledTemplate code = c.Compile(template);
            code.name = name;
            code.formalArguments = args;
            code.nativeGroup = this;
            templates[prefix + name] = code;
            if (args != null)
            { // compile any default args
                foreach (string a in args.Keys)
                {
                    FormalArgument fa = args[a];
                    if (fa.defaultValue != null)
                    {
                        Compiler c2 = new Compiler(prefix);
                        fa.compiledDefaultValue = c2.Compile(template);
                    }
                }
            }
            // define any anonymous subtemplates
            DefineAnonSubtemplates(code);

            return code;
        }

        public virtual void DefineAnonSubtemplates(CompiledTemplate code)
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
                listener.Error("can't load group file: " + absoluteFileName, e);
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
            Template st = new Template();
            return st;
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
            //if ( supergroup!=null ) buf.append(" : "+supergroup);
            foreach (string name in templates.Keys)
            {
                if (name.StartsWith("/_sub"))
                    continue;
                CompiledTemplate c = templates[name];
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

        public virtual void SetErrorListener(ITemplateErrorListener listener)
        {
            this.listener = listener;
        }

        public virtual void SetErrorTolerance(ErrorTolerance errors)
        {
            this.tolerance = errors;
        }

        public virtual bool Detects(int x)
        {
            return tolerance.Detects(x);
        }

        public virtual void Detect(int x)
        {
            tolerance.Detect(x);
        }

        public virtual void Ignore(int x)
        {
            tolerance.Ignore(x);
        }
    }
}
