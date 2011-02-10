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
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using Antlr.Runtime;
    using Antlr4.StringTemplate.Compiler;
    using Antlr4.StringTemplate.Debug;
    using Antlr4.StringTemplate.Misc;
    using ArgumentException = System.ArgumentException;
    using Console = System.Console;
    using Environment = System.Environment;
    using Exception = System.Exception;
    using IDictionary = System.Collections.IDictionary;
    using StringBuilder = System.Text.StringBuilder;
    using Type = System.Type;
    using Uri = System.Uri;

    /** A directory or directory tree of .st template files and/or group files.
     *  Individual template files contain formal template definitions. In a sense,
     *  it's like a single group file broken into multiple files, one for each template.
     *  Template v3 had just the pure template inside, not the template name and header.
     *  Name inside must match filename (minus suffix).
     */
    public class TemplateGroup
    {
        /** When we use key as a value in a dictionary, this is how we signify. */
        public static readonly string DictionaryKey = "key";
        public static readonly string DefaultKey = "default";

        /** Load files using what encoding? */
        private Encoding _encoding;

        /** Every group can import templates/dictionaries from other groups.
         *  The list must be synchronized (see ImportTemplates).
         */
        private readonly List<TemplateGroup> imports = new List<TemplateGroup>();

        public readonly char delimiterStartChar = '<'; // Use <expr> by default
        public readonly char delimiterStopChar = '>';

        /** Maps template name to StringTemplate object. synchronized. */
        private readonly Dictionary<string, CompiledTemplate> templates = new Dictionary<string, CompiledTemplate>();

        /** Maps dict names to HashMap objects.  This is the list of dictionaries
         *  defined by the user like typeInitMap ::= ["int":"0"]
         */
        private readonly Dictionary<string, IDictionary<string, object>> dictionaries = new Dictionary<string, IDictionary<string, object>>();

        /** A dictionary that allows people to register a renderer for
         *  a particular kind of object for any template evaluated relative to this
         *  group.  For example, a date should be formatted differently depending
         *  on the culture.  You can set Date.class to an object whose
         *  ToString(Object) method properly formats a Date attribute
         *  according to culture.  Or you can have a different renderer object
         *  for each culture.
         *
         *  Order of addition is recorded and matters.  If more than one
         *  renderer works for an object, the first registered has priority.
         *
         *  Renderer associated with type t works for object o if
         *
         * 		t.isAssignableFrom(o.getClass()) // would assignment t = o work?
         *
         *  So it works if o is subclass or implements t.
         *
         *  This structure is synchronized.
         */
        protected TypeRegistry<IAttributeRenderer> renderers;

        protected TypeRegistry<ITypeProxyFactory> _proxyFactories;

        /** A dictionary that allows people to register a model adaptor for
         *  a particular kind of object (subclass or implementation). Applies
         *  for any template evaluated relative to this group.
         *
         *  Template initializes with model adaptors that know how to pull
         *  properties out of Objects, Maps, and STs.
         */
        protected readonly TypeRegistry<IModelAdaptor> adaptors =
            new TypeRegistry<IModelAdaptor>()
            {
                {typeof(object), new ObjectModelAdaptor()},
                {typeof(Template), new TemplateModelAdaptor()},
                {typeof(IDictionary), new MapModelAdaptor()},
            };

        public static TemplateGroup defaultGroup = new TemplateGroup();

        /** Used to indicate that the template doesn't exist.
         *  Prevents duplicate group file loads and unnecessary file checks.
         */
        protected static readonly CompiledTemplate NotFoundTemplate = new CompiledTemplate();

        public static readonly ErrorManager DefaultErrorManager = new ErrorManager();

        private bool _debug = false;

        /** The error manager for entire group; all compilations and executions.
         *  This gets copied to parsers, walkers, and interpreters.
         */
        private ErrorManager _errorManager = TemplateGroup.DefaultErrorManager;

        public TemplateGroup()
        {
        }

        public TemplateGroup(char delimiterStartChar, char delimiterStopChar)
        {
            this.delimiterStartChar = delimiterStartChar;
            this.delimiterStopChar = delimiterStopChar;
        }

        public IEnumerable<CompiledTemplate> CompiledTemplates
        {
            get
            {
                return templates.Values;
            }
        }

        public Encoding Encoding
        {
            get
            {
                return _encoding;
            }

            set
            {
                _encoding = value;
            }
        }

        public ErrorManager ErrorManager
        {
            get
            {
                return _errorManager;
            }

            set
            {
                _errorManager = value;
            }
        }

        public bool Debug
        {
            get
            {
                return _debug;
            }

            set
            {
                _debug = value;
            }
        }

        /** The primary means of getting an instance of a template from this
         *  group. Names must be absolute, fully-qualified names like a/b
         */
        public virtual Template GetInstanceOf(string name)
        {
            if (name == null)
                return null;
            //System.out.println("GetInstanceOf("+name+")");
            CompiledTemplate c = LookupTemplate(name);
            if (c != null)
            {
                Template instanceST = CreateStringTemplate();
                instanceST.groupThatCreatedThisInstance = this;
                instanceST.impl = c;
                if (instanceST.impl.formalArguments != null)
                {
                    instanceST.locals = new object[instanceST.impl.formalArguments.Count];
                    for (int i = 0; i < instanceST.locals.Length; i++)
                        instanceST.locals[i] = Template.EmptyAttribute;
                }
                return instanceST;
            }
            return null;
        }

        protected internal virtual Template GetEmbeddedInstanceOf(Template enclosingInstance, int ip, string name)
        {
            Template st = GetInstanceOf(name);
            if (st == null)
            {
                ErrorManager.RuntimeError(enclosingInstance, ip, ErrorType.NO_SUCH_TEMPLATE, name);
                st = CreateStringTemplate();
                st.impl = new CompiledTemplate();
                return st;
            }
            st.EnclosingInstance = enclosingInstance;
            return st;
        }

        /** Create singleton template for use with dictionary values */
        public virtual Template CreateSingleton(IToken templateToken)
        {
            string template;
            if (templateToken.Type == GroupParser.BIGSTRING)
            {
                template = Utility.Strip(templateToken.Text, 2);
            }
            else
            {
                template = Utility.Strip(templateToken.Text, 1);
            }
            Template st = CreateStringTemplate();
            st.groupThatCreatedThisInstance = this;
            st.impl = Compile(FileName, null, null, template, templateToken);
            st.impl.hasFormalArgs = false;
            st.impl.name = Template.UnknownName;
            st.impl.DefineImplicitlyDefinedTemplates(this);
            return st;
        }

        /** Is this template defined in this group or from this group below?
         *  Names must be absolute, fully-qualified names like /a/b
         */
        public virtual bool IsDefined(string name)
        {
            return LookupTemplate(name) != null;
        }

        /** Look up a fully-qualified name */
        public virtual CompiledTemplate LookupTemplate(string name)
        {
            CompiledTemplate code;
            templates.TryGetValue(name, out code);
            if (code == NotFoundTemplate)
                return null;

            // try to load from disk and look up again
            if (code == null)
                code = Load(name);

            if (code == null)
                code = LookupImportedTemplate(name);

            if (code == null)
                templates[name] = NotFoundTemplate;

            return code;
        }

        /** "Unload" all templates and dictionaries but leave renderers, adaptors,
         *  and import relationships.  This essentially forces next GetInstanceOf
         *  to reload templates.
         */
        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual void Unload()
        {
            templates.Clear();
            dictionaries.Clear();
        }

        /** Load st from disk if dir or load whole group file if .stg file (then
         *  return just one template). name is fully-qualified.
         */
        protected virtual CompiledTemplate Load(string name)
        {
            return null;
        }

        /** Force a load if it makes sense for the group */
        public virtual void Load()
        {
        }

        protected internal virtual CompiledTemplate LookupImportedTemplate(string name)
        {
            //System.out.println("look for "+name+" in "+imports);
            if (imports == null)
                return null;
            foreach (TemplateGroup g in imports)
            {
                CompiledTemplate code = g.LookupTemplate(name);
                if (code != null)
                    return code;
            }
            return null;
        }

        public virtual CompiledTemplate RawGetTemplate(string name)
        {
            CompiledTemplate template;
            templates.TryGetValue(name, out template);
            return template;
        }

        public virtual IDictionary<string, object> RawGetDictionary(string name)
        {
            IDictionary<string, object> dictionary;
            dictionaries.TryGetValue(name, out dictionary);
            return dictionary;
        }

        public virtual bool IsDictionary(string name)
        {
            return RawGetDictionary(name) != null;
        }

        // for testing
        public virtual CompiledTemplate DefineTemplate(string name, string template)
        {
            try
            {
                CompiledTemplate impl = DefineTemplate(name, new CommonToken(GroupParser.ID, name), null, template, null);
                return impl;
            }
            catch (TemplateException)
            {
                Console.Error.WriteLine("eh?");
            }

            return null;
        }

        // for testing
        public virtual CompiledTemplate DefineTemplate(string name, string template, string[] arguments)
        {
            List<FormalArgument> a = new List<FormalArgument>();
            foreach (string arg in arguments)
                a.Add(new FormalArgument(arg));

            return DefineTemplate(name, new CommonToken(GroupParser.ID, name), a, template, null);
        }

        public virtual CompiledTemplate DefineTemplate(string templateName,
                                         IToken nameT,
                                         List<FormalArgument> args,
                                         string template,
                                         IToken templateToken)
        {
            if (templateName == null || templateName.Length == 0)
                throw new ArgumentException("empty template name");
            if (templateName.IndexOf('.') >= 0)
                throw new ArgumentException("cannot have '.' in template names");

            template = Utility.TrimOneStartingNewline(template);
            template = Utility.TrimOneTrailingNewline(template);
            // compile, passing in templateName as enclosing name for any embedded regions
            CompiledTemplate code = Compile(FileName, templateName, args, template, templateToken);
            code.name = templateName;
            RawDefineTemplate(templateName, code, nameT);
            code.DefineArgumentDefaultValueTemplates(this);
            code.DefineImplicitlyDefinedTemplates(this); // define any anonymous subtemplates

            return code;
        }

        /** Make name and alias for target.  Replace any previous def of name */
        public virtual CompiledTemplate DefineTemplateAlias(IToken aliasT, IToken targetT)
        {
            string alias = aliasT.Text;
            string target = targetT.Text;
            CompiledTemplate targetCode;
            templates.TryGetValue(target, out targetCode);
            if (targetCode == null)
            {
                ErrorManager.CompiletimeError(ErrorType.ALIAS_TARGET_UNDEFINED, null, aliasT, alias, target);
                return null;
            }

            templates[alias] = targetCode;
            return targetCode;
        }

        public virtual CompiledTemplate DefineRegion(string enclosingTemplateName, IToken regionT, string template, IToken templateToken)
        {
            string name = regionT.Text;
            template = Utility.TrimOneStartingNewline(template);
            template = Utility.TrimOneTrailingNewline(template);
            CompiledTemplate code = Compile(FileName, enclosingTemplateName, null, template, regionT);
            string mangled = GetMangledRegionName(enclosingTemplateName, name);

            if (LookupTemplate(mangled) == null)
            {
                ErrorManager.CompiletimeError(ErrorType.NO_SUCH_REGION, null, regionT, enclosingTemplateName, name);
                return new CompiledTemplate();
            }

            code.name = mangled;
            code.isRegion = true;
            code.regionDefType = Template.RegionType.Explicit;

            RawDefineTemplate(mangled, code, regionT);
            code.DefineArgumentDefaultValueTemplates(this);
            code.DefineImplicitlyDefinedTemplates(this); // define any anonymous subtemplates
            return code;
        }

        public virtual void DefineTemplateOrRegion(
            string templateName,
            string regionSurroundingTemplateName,
            IToken templateToken,
            string template,
            IToken nameToken,
            List<FormalArgument> args)
        {
            try
            {
                if (regionSurroundingTemplateName != null)
                {
                    DefineRegion(regionSurroundingTemplateName, nameToken, template, templateToken);
                }
                else
                {
                    DefineTemplate(templateName, nameToken, args, template, templateToken);
                }
            }
            catch (TemplateException)
            {
                // after getting syntax error in a template, we emit msg
                // and throw exception to blast all the way out here.
            }
        }

        public virtual void RawDefineTemplate(string name, CompiledTemplate code, IToken defT)
        {
            CompiledTemplate prev;
            templates.TryGetValue(name, out prev);
            if (prev != null)
            {
                if (!prev.isRegion)
                {
                    ErrorManager.CompiletimeError(ErrorType.TEMPLATE_REDEFINITION, null, defT);
                    return;
                }
                if (prev.isRegion && prev.regionDefType == Template.RegionType.Embedded)
                {
                    ErrorManager.CompiletimeError(ErrorType.EMBEDDED_REGION_REDEFINITION, null, defT, GetUnmangledTemplateName(name));
                    return;
                }
                else if (prev.isRegion && prev.regionDefType == Template.RegionType.Explicit)
                {
                    ErrorManager.CompiletimeError(ErrorType.REGION_REDEFINITION, null, defT, GetUnmangledTemplateName(name));
                    return;
                }
            }

            templates[name] = code;
        }

        public virtual void UndefineTemplate(string name)
        {
            templates.Remove(name);
        }

        /** Compile a template */
        public virtual CompiledTemplate Compile(string srcName,
                                  string name,
                                  List<FormalArgument> args,
                                  string template,
                                  IToken templateToken) // for error location
        {
            //System.out.println("TemplateGroup.Compile: "+enclosingTemplateName);
            TemplateCompiler c = new TemplateCompiler(this);
            CompiledTemplate code = c.Compile(srcName, name, args, template, templateToken);
            code.nativeGroup = this;
            code.template = template;
            return code;
        }

        /** The "foo" of t() ::= "<@foo()>" is mangled to "region#t#foo" */
        public static string GetMangledRegionName(string enclosingTemplateName, string name)
        {
            return "region__" + enclosingTemplateName + "__" + name;
        }

        /** Return "t.foo" from "region__t__foo" */
        public static string GetUnmangledTemplateName(string mangledName)
        {
            string t = mangledName.Substring("region__".Length, mangledName.LastIndexOf("__") - "region__".Length);
            string r = mangledName.Substring(mangledName.LastIndexOf("__") + 2, mangledName.Length - mangledName.LastIndexOf("__") - 2);
            return t + '.' + r;
        }

        /** Define a map for this group; not thread safe...do not keep adding
         *  these while you reference them.
         */
        public virtual void DefineDictionary(string name, IDictionary<string, object> mapping)
        {
            dictionaries[name] = mapping;
        }

        /** Make this group import templates/dictionaries from g. */
        public virtual void ImportTemplates(TemplateGroup g)
        {
            if (g == null)
                return;

            imports.Add(g);
        }

        /** Load group dir or file (if .stg suffix) and then import templates. Don't hold
         *  an independent ref to the "supergroup".
         *
         *  Override this if you want to look for groups elsewhere (database maybe?)
         *
         *  ImportTemplates("org.foo.proj.G.stg") will try to find file org/foo/proj/G.stg
         *  relative to current dir or in CLASSPATH. The name is not relative to this group.
         *  Can use "/a/b/c/myfile.stg" also or "/a/b/c/mydir".
         *
         *  Pass token so you can give good error if you want.
         */
        public virtual void ImportTemplates(IToken fileNameToken)
        {
            string fileName = fileNameToken.Text;
            // do nothing upon syntax error
            if (fileName == null || fileName.Equals("<missing STRING>"))
                return;
            fileName = Utility.Strip(fileName, 1);
            TemplateGroup g = null;
            if (fileName.EndsWith(".stg"))
            {
                g = new TemplateGroupFile(fileName, delimiterStartChar, delimiterStopChar);
            }
            else
            {
                g = new TemplateGroupDirectory(fileName, delimiterStartChar, delimiterStopChar);
            }
            ImportTemplates(g);
        }

        /** Load a group file with full path fileName; it's relative to root by prefix. */
        public virtual void LoadGroupFile(string prefix, string fileName)
        {
            //System.out.println("load group file prefix="+prefix+", fileName="+fileName);
            GroupParser parser = null;
            try
            {
                Uri f = new Uri(fileName);
                ANTLRReaderStream fs = new ANTLRReaderStream(new System.IO.StreamReader(f.LocalPath, Encoding ?? Encoding.UTF8));
                GroupLexer lexer = new GroupLexer(fs);
                fs.name = fileName;
                CommonTokenStream tokens = new CommonTokenStream(lexer);
                parser = new GroupParser(tokens);
                parser.group(this, prefix);
            }
            catch (Exception e)
            {
                ErrorManager.IOError(null, ErrorType.CANT_LOAD_GROUP_FILE, e, fileName);
            }
        }

        /** Add an adaptor for a kind of object so Template knows how to pull properties
         *  from them. Add adaptors in increasing order of specificity.  Template adds Object,
         *  Map, and Template model adaptors for you first. Adaptors you Add have
         *  priority over default adaptors.
         *
         *  If an adaptor for type T already exists, it is replaced by the adaptor arg.
         *
         *  This must invalidate cache entries, so set your adaptors up before
         *  Render()ing your templates for efficiency.
         */
        public virtual void RegisterModelAdaptor(Type attributeType, IModelAdaptor adaptor)
        {
            adaptors[attributeType] = adaptor;
        }

        public virtual IModelAdaptor GetModelAdaptor(Type attributeType)
        {
            IModelAdaptor adaptor;
            adaptors.TryGetValue(attributeType, out adaptor);
            return adaptor;
        }

        /** Register a renderer for all objects of a particular "kind" for all
         *  templates evaluated relative to this group.  Use r to Render if
         *  object in question is instanceof(attributeType).
         */
        public virtual void RegisterRenderer(Type attributeType, IAttributeRenderer r)
        {
            renderers = renderers ?? new TypeRegistry<IAttributeRenderer>();
            renderers[attributeType] = r;
        }

        public virtual IAttributeRenderer GetAttributeRenderer(Type attributeType)
        {
            if (renderers == null)
                return null;

            IAttributeRenderer renderer;
            renderers.TryGetValue(attributeType, out renderer);
            return renderer;
        }

        public virtual void RegisterTypeProxyFactory(Type targetType, ITypeProxyFactory factory)
        {
            _proxyFactories = _proxyFactories ?? new TypeRegistry<ITypeProxyFactory>();
            _proxyFactories[targetType] = factory;
        }

        public virtual ITypeProxyFactory GetTypeProxyFactory(Type targetType)
        {
            if (_proxyFactories == null)
                return null;

            ITypeProxyFactory factory;
            _proxyFactories.TryGetValue(targetType, out factory);
            return factory;
        }

        /** StringTemplate object factory; each group can have its own. */
        public virtual Template CreateStringTemplate()
        {
            // TODO: try making a mem pool?
            if (Debug)
                return new DebugTemplate();

            return new Template();
        }

        public virtual Template CreateStringTemplate(Template proto)
        {
            if (Debug)
                return new DebugTemplate(proto);

            return new Template(proto);
        }

        public virtual string Name
        {
            get
            {
                return "<no name>;";
            }
        }

        public virtual string FileName
        {
            get
            {
                return null;
            }
        }

        public override string ToString()
        {
            return Name;
        }

        public virtual string Show()
        {
            StringBuilder buf = new StringBuilder();
            if (imports != null && imports.Count > 0)
                buf.Append(" : " + imports);

            foreach (string n in templates.Keys)
            {
                string name = n;
                CompiledTemplate c = templates[name];
                if (c.isAnonSubtemplate || c == NotFoundTemplate)
                    continue;

                int slash = name.LastIndexOf('/');
                name = name.Substring(slash + 1, name.Length - slash - 1);
                buf.Append(name);
                buf.Append('(');
                if (c.formalArguments != null)
                    buf.Append(string.Join(",", c.formalArguments.Select(i => i.ToString()).ToArray()));

                buf.Append(')');
                buf.Append(" ::= <<" + Environment.NewLine);
                buf.Append(c.template + Environment.NewLine);
                buf.Append(">>" + Environment.NewLine);
            }

            return buf.ToString();
        }

        public virtual ITemplateErrorListener Listener
        {
            get
            {
                return ErrorManager.Listener;
            }

            set
            {
                ErrorManager = new ErrorManager(value);
            }
        }
    }
}
