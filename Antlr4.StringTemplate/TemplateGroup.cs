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
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using Antlr.Runtime;
    using Antlr4.StringTemplate.Compiler;
    using Antlr4.StringTemplate.Extensions;
    using Antlr4.StringTemplate.Misc;

    using ArgumentException = System.ArgumentException;
    using ArgumentNullException = System.ArgumentNullException;
    using Console = System.Console;
    using Directory = System.IO.Directory;
    using Environment = System.Environment;
    using Exception = System.Exception;
    using File = System.IO.File;
    using IDictionary = System.Collections.IDictionary;
    using IOException = System.IO.IOException;
    using Path = System.IO.Path;
    using Stream = System.IO.Stream;
    using StringBuilder = System.Text.StringBuilder;
    using Type = System.Type;
    using Uri = System.Uri;
    using UriFormatException = System.UriFormatException;

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
        private readonly List<TemplateGroup> _imports = new List<TemplateGroup>();

        private readonly List<TemplateGroup> _importsToClearOnUnload = new List<TemplateGroup>();

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
        private TypeRegistry<IAttributeRenderer> renderers;

        private TypeRegistry<ITypeProxyFactory> _proxyFactories;

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
                {typeof(Aggregate), new AggregateModelAdaptor()},
            };

        /** Watch loading of groups and templates */
        private bool _verbose = false;

        /** For debugging with STViz. Records where in code an ST was created
         *  and where code added attributes.
         */
        private bool _trackCreationEvents = false;

        /** v3 compatibility; used to iterate across values not keys like v4.
         *  But to convert ANTLR templates, it's too hard to find without
         *  static typing in templates.
         */
        private bool _iterateAcrossValues = false;

        /** Used to indicate that the template doesn't exist.
         *  Prevents duplicate group file loads and unnecessary file checks.
         */
        protected static readonly CompiledTemplate NotFoundTemplate = new CompiledTemplate();

        private static readonly ErrorManager _defaultErrorManager = new ErrorManager();

        public static TemplateGroup defaultGroup = new TemplateGroup();

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

        public static ErrorManager DefaultErrorManager
        {
            get
            {
                return _defaultErrorManager;
            }
        }

        public ICollection<CompiledTemplate> CompiledTemplates
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
                if (value == null)
                    throw new ArgumentNullException("value");

                _errorManager = value;
            }
        }

        public bool Verbose
        {
            get
            {
                return _verbose;
            }

            set
            {
                _verbose = value;
            }
        }

        public bool TrackCreationEvents
        {
            get
            {
                return _trackCreationEvents;
            }

            set
            {
                _trackCreationEvents = value;
            }
        }

        public bool IterateAcrossValues
        {
            get
            {
                return _iterateAcrossValues;
            }

            set
            {
                _iterateAcrossValues = value;
            }
        }

        /** The primary means of getting an instance of a template from this
         *  group. Names must be absolute, fully-qualified names like a/b
         */
        public virtual Template GetInstanceOf(string name)
        {
            if (name == null)
                return null;
            if (Verbose)
                Console.WriteLine(string.Format("{0}.GetInstanceOf({1})", Name, name));

            CompiledTemplate c = LookupTemplate(name);
            if (c != null)
                return CreateStringTemplate(c);

            return null;
        }

        protected internal virtual Template GetEmbeddedInstanceOf(TemplateFrame frame, string name)
        {
            string fullyQualifiedName = name;
            if (name[0] != '/')
                fullyQualifiedName = frame.Template.impl.Prefix + name;

            if (Verbose)
                Console.WriteLine(string.Format("getEmbeddedInstanceOf({0})", fullyQualifiedName));

            Template st = GetInstanceOf(fullyQualifiedName);
            if (st == null)
            {
                ErrorManager.RuntimeError(frame, ErrorType.NO_SUCH_TEMPLATE, fullyQualifiedName);
                return CreateStringTemplateInternally(new CompiledTemplate());
            }

            // this is only called internally. wack any debug ST create events
            if (TrackCreationEvents)
            {
                // toss it out
                st.DebugState.NewTemplateEvent = null;
            }

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

            CompiledTemplate impl = Compile(FileName, null, null, template, templateToken);
            Template st = CreateStringTemplateInternally(impl);
            st.Group = this;
            st.impl.HasFormalArgs = false;
            st.impl.Name = Template.UnknownName;
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
            if (name[0] != '/')
                name = "/" + name;

            if (Verbose)
                Console.WriteLine(string.Format("{0}.LookupTemplate({1})", Name, name));

            CompiledTemplate code;
            templates.TryGetValue(name, out code);
            if (code == NotFoundTemplate)
            {
                if (Verbose)
                    Console.WriteLine(string.Format("{0} previously seen as not found", name));

                return null;
            }

            // try to load from disk and look up again
            if (code == null)
                code = Load(name);

            if (code == null)
                code = LookupImportedTemplate(name);

            if (code == null)
            {
                if (Verbose)
                    Console.WriteLine(string.Format("{0} recorded not found", name));

                templates[name] = NotFoundTemplate;
            }

            if (Verbose && code != null)
                Console.WriteLine(string.Format("{0}.LookupTemplate({1}) found", Name, name));

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

            foreach (var import in _imports)
                import.Unload();

            foreach (var import in _importsToClearOnUnload)
                _imports.Remove(import);

            _importsToClearOnUnload.Clear();
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
            if (_imports == null)
                return null;

            foreach (TemplateGroup g in _imports)
            {
                if (Verbose)
                    Console.WriteLine(string.Format("checking {0} for imported {1}", g.Name, name));

                CompiledTemplate code = g.LookupTemplate(name);
                if (code != null)
                {
                    if (Verbose)
                        Console.WriteLine(string.Format("{0}.LookupImportedTemplate({1}) found", g.Name, name));

                    return code;
                }
            }

            if (Verbose)
                Console.WriteLine(string.Format("{0} not found in {1} imports", name, Name));

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
            if (name[0] != '/')
                name = "/" + name;

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
            if (name[0] != '/')
                name = "/" + name;

            List<FormalArgument> a = new List<FormalArgument>();
            foreach (string arg in arguments)
                a.Add(new FormalArgument(arg));

            return DefineTemplate(name, new CommonToken(GroupParser.ID, name), a, template, null);
        }

        public virtual CompiledTemplate DefineTemplate(string fullyQualifiedTemplateName,
                                         IToken nameT,
                                         List<FormalArgument> args,
                                         string template,
                                         IToken templateToken)
        {
            if (Verbose)
                Console.WriteLine(string.Format("DefineTemplate({0})", fullyQualifiedTemplateName));

            if (fullyQualifiedTemplateName == null)
                throw new ArgumentNullException("fullyQualifiedTemplateName");
            if (fullyQualifiedTemplateName.Length == 0)
                throw new ArgumentException("empty template name", "fullyQualifiedTemplateName");
            if (fullyQualifiedTemplateName.IndexOf('.') >= 0)
                throw new ArgumentException("cannot have '.' in template names", "fullyQualifiedTemplateName");
            if (fullyQualifiedTemplateName[0] != '/')
                throw new ArgumentException("Expected a fully qualified template name.", "fullyQualifiedTemplateName");

            template = Utility.TrimOneStartingNewline(template);
            template = Utility.TrimOneTrailingNewline(template);
            // compile, passing in templateName as enclosing name for any embedded regions
            CompiledTemplate code = Compile(FileName, fullyQualifiedTemplateName, args, template, templateToken);
            code.Name = fullyQualifiedTemplateName;
            RawDefineTemplate(fullyQualifiedTemplateName, code, nameT);
            code.DefineArgumentDefaultValueTemplates(this);
            code.DefineImplicitlyDefinedTemplates(this); // define any anonymous subtemplates

            return code;
        }

        /** Make name and alias for target.  Replace any previous def of name */
        public virtual CompiledTemplate DefineTemplateAlias(IToken aliasT, IToken targetT)
        {
            string alias = aliasT.Text;
            string target = targetT.Text;
            CompiledTemplate targetCode = RawGetTemplate("/" + target);
            if (targetCode == null)
            {
                ErrorManager.CompiletimeError(ErrorType.ALIAS_TARGET_UNDEFINED, null, aliasT, alias, target);
                return null;
            }

            RawDefineTemplate("/" + alias, targetCode, aliasT);
            return targetCode;
        }

        public virtual CompiledTemplate DefineRegion(string enclosingTemplateName, IToken regionT, string template, IToken templateToken)
        {
            string name = regionT.Text;
            template = Utility.TrimOneStartingNewline(template);
            template = Utility.TrimOneTrailingNewline(template);
            CompiledTemplate code = Compile(FileName, enclosingTemplateName, null, template, templateToken);
            string mangled = GetMangledRegionName(enclosingTemplateName, name);

            if (LookupTemplate(mangled) == null)
            {
                ErrorManager.CompiletimeError(ErrorType.NO_SUCH_REGION, null, regionT, enclosingTemplateName, name);
                return new CompiledTemplate();
            }

            code.Name = mangled;
            code.IsRegion = true;
            code.RegionDefType = Template.RegionType.Explicit;
            code.TemplateDefStartToken = regionT;

            RawDefineTemplate(mangled, code, regionT);
            code.DefineArgumentDefaultValueTemplates(this);
            code.DefineImplicitlyDefinedTemplates(this); // define any anonymous subtemplates
            return code;
        }

        public virtual void DefineTemplateOrRegion(
            string fullyQualifiedTemplateName,
            string regionSurroundingTemplateName,
            IToken templateToken,
            string template,
            IToken nameToken,
            List<FormalArgument> args)
        {
            if (fullyQualifiedTemplateName[0] != '/')
                throw new ArgumentException("Expected a fully qualified template name.", "fullyQualifiedTemplateName");

            try
            {
                if (regionSurroundingTemplateName != null)
                {
                    DefineRegion(regionSurroundingTemplateName, nameToken, template, templateToken);
                }
                else
                {
                    DefineTemplate(fullyQualifiedTemplateName, nameToken, args, template, templateToken);
                }
            }
            catch (TemplateException)
            {
                // after getting syntax error in a template, we emit msg
                // and throw exception to blast all the way out to here.
            }
        }

        public virtual void RawDefineTemplate(string name, CompiledTemplate code, IToken defT)
        {
            CompiledTemplate prev;
            templates.TryGetValue(name, out prev);
            if (prev != null)
            {
                if (!prev.IsRegion)
                {
                    ErrorManager.CompiletimeError(ErrorType.TEMPLATE_REDEFINITION, null, defT);
                    return;
                }

                /* If this region was previously defined, the following actions should be taken:
                 *
                 *      Previous type   Current type   Result   Applied     Reason
                 *      -------------   ------------   ------   -------     ------
                 *      Implicit        Implicit       Success  Previous    A rule may make multiple implicit references to the same region.
                 *                                                          Keeping either has the same semantics, so the existing one is
                 *                                                          used for slightly less overhead.
                 *      Implicit        Explicit       Success  Current     A region with previous implicit references is now being explicitly
                 *                                                          defined.
                 *      Implicit        Embedded       Success  Current     A region with previous implicit references is now being defined
                 *                                                          with an embedded region.
                 *      Explicit        Implicit       Success  Previous    An explicitly defined region is now being implicitly referenced.
                 *                                                          Make sure to keep the previous explicit definition as the actual
                 *                                                          definition.
                 *      Explicit        Explicit       Error    Previous    Multiple explicit definitions exist for the same region (template
                 *                                                          redefinition error). Give an error and use the previous one.
                 *      Explicit        Embedded       Warning  Previous    An explicit region definition already exists for the current
                 *                                                          embedded region definition. The explicit definition overrides the
                 *                                                          embedded definition and a warning is given since the embedded
                 *                                                          definition is hidden.
                 *      Embedded        Implicit       Success  Previous    A region with an embedded definition is now being implicitly
                 *                                                          referenced. The embedded definition should be used.
                 *      Embedded        Explicit       Warning  Current     A region with an embedded definition is now being explicitly
                 *                                                          defined. The explicit definition overrides the embedded
                 *                                                          definition and a warning is given since the embedded definition
                 *                                                          is hidden.
                 *      Embedded        Embedded       Error    Previous    Multiple embedded definitions of the same region were given in a
                 *                                                          template. Give an error and use the previous one.
                 */

                // handle the Explicit/Explicit and Embedded/Embedded error cases
                if (code.RegionDefType != Template.RegionType.Implicit && code.RegionDefType == prev.RegionDefType)
                {
                    if (code.RegionDefType == Template.RegionType.Embedded)
                        ErrorManager.CompiletimeError(ErrorType.EMBEDDED_REGION_REDEFINITION, null, defT, GetUnmangledTemplateName(name));
                    else
                        ErrorManager.CompiletimeError(ErrorType.REGION_REDEFINITION, null, defT, GetUnmangledTemplateName(name));

                    // keep the previous one
                    return;
                }
                // handle the Explicit/Embedded and Embedded/Explicit warning cases
                else if ((code.RegionDefType == Template.RegionType.Embedded && prev.RegionDefType == Template.RegionType.Explicit)
                    || (code.RegionDefType == Template.RegionType.Explicit && prev.RegionDefType == Template.RegionType.Embedded))
                {
                    // TODO: can we make this a warning?
                    ErrorManager.CompiletimeError(ErrorType.HIDDEN_EMBEDDED_REGION_DEFINITION, null, defT, GetUnmangledTemplateName(name));
                    // keep the previous one only if that's the explicit definition
                    if (prev.RegionDefType == Template.RegionType.Explicit)
                        return;
                }
                // else if the current definition type is implicit, keep the previous one
                else if (code.RegionDefType == Template.RegionType.Implicit)
                {
                    return;
                }
            }

            code.NativeGroup = this;
            code.TemplateDefStartToken = defT;
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
            return c.Compile(srcName, name, args, template, templateToken);
        }

        /** The "foo" of t() ::= "&lt;@foo()&gt;" is mangled to "/region__/t__foo" */
        public static string GetMangledRegionName(string enclosingTemplateName, string name)
        {
            if (enclosingTemplateName[0] != '/')
                enclosingTemplateName = '/' + enclosingTemplateName;

            return "/region__" + enclosingTemplateName + "__" + name;
        }

        /** Return "t.foo" from "/region__/t__foo" */
        public static string GetUnmangledTemplateName(string mangledName)
        {
            string t = mangledName.Substring("/region__".Length, mangledName.LastIndexOf("__") - "/region__".Length);
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
            ImportTemplates(g, false);
        }

        /** Make this group import templates/dictionaries from g. */
        private void ImportTemplates(TemplateGroup group, bool clearOnUnload)
        {
            if (group == null)
                return;

            _imports.Add(group);
            if (clearOnUnload)
                _importsToClearOnUnload.Add(group);
        }

        /** Import template files, directories, and group files.
         *  Priority is given to templates defined in the current group;
         *  this, in effect, provides inheritance. Polymorphism is in effect so
         *  that if an inherited template references template t() then we
         *  search for t() in the subgroup first.
         *
         *  Templates are loaded on-demand from import dirs.  Imported groups are
         *  loaded on-demand when searching for a template.
         *
         *  The listener of this group is passed to the import group so errors
         *  found while loading imported element are sent to listener of this group.
         */
        public virtual void ImportTemplates(IToken fileNameToken)
        {
            string fileName = fileNameToken.Text;

            if (Verbose)
                Console.WriteLine("ImportTemplates({0})", fileName);

            // do nothing upon syntax error
            if (fileName == null || fileName.Equals("<missing STRING>"))
                return;

            fileName = Utility.Strip(fileName, 1);

            //Console.WriteLine("import {0}", fileName);
            bool isGroupFile = fileName.EndsWith(".stg");
            bool isTemplateFile = fileName.EndsWith(".st");
            bool isGroupDir = !(isGroupFile || isTemplateFile);

            TemplateGroup g = null;

            // search path is: working dir, g.stg's dir, CLASSPATH
            Uri thisRoot = RootDirUri;
            Uri fileUnderRoot = null;
            //Console.WriteLine("thisRoot={0}", thisRoot);
            try
            {
                fileUnderRoot = new Uri(thisRoot + "/" + fileName);
            }
            catch (UriFormatException mfe)
            {
                ErrorManager.InternalError(null, string.Format("can't build URL for {0}/{1}", thisRoot, fileName), mfe);
                return;
            }

            if (isTemplateFile)
            {
                g = new TemplateGroup();
                g.Listener = this.Listener;
                Uri fileURL = null;
                if (File.Exists(fileUnderRoot.LocalPath))
                    fileURL = fileUnderRoot;

                if (fileURL != null)
                {
                    try
                    {
                        Stream s = File.OpenRead(fileURL.LocalPath);
                        ANTLRInputStream templateStream = new ANTLRInputStream(s);
                        templateStream.name = fileName;
                        CompiledTemplate code = g.LoadTemplateFile("/", fileName, templateStream);
                        if (code == null)
                            g = null;
                    }
                    catch (IOException ioe)
                    {
                        ErrorManager.InternalError(null, string.Format("can't read from {0}", fileURL), ioe);
                        g = null;
                    }
                }
                else
                {
                    g = null;
                }
            }
            else if (isGroupFile)
            {
                //System.out.println("look for fileUnderRoot: "+fileUnderRoot);
                if (File.Exists(fileUnderRoot.LocalPath))
                {
                    g = new TemplateGroupFile(fileUnderRoot, Encoding, delimiterStartChar, delimiterStopChar);
                    g.Listener = this.Listener;
                }
                else
                {
                    g = new TemplateGroupFile(fileName, delimiterStartChar, delimiterStopChar);
                    g.Listener = this.Listener;
                }
            }
            else if (isGroupDir)
            {
                //			System.out.println("try dir "+fileUnderRoot);
                if (Directory.Exists(fileUnderRoot.LocalPath))
                {
                    g = new TemplateGroupDirectory(fileUnderRoot, Encoding, delimiterStartChar, delimiterStopChar);
                    g.Listener = this.Listener;
                }
                else
                {
                    // try in CLASSPATH
                    //				System.out.println("try dir in CLASSPATH "+fileName);
                    g = new TemplateGroupDirectory(fileName, delimiterStartChar, delimiterStopChar);
                    g.Listener = this.Listener;
                }
            }

            if (g == null)
            {
                ErrorManager.CompiletimeError(ErrorType.CANT_IMPORT, null, fileNameToken, fileName);
            }
            else
            {
                ImportTemplates(g, true);
            }
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
                e.PreserveStackTrace();
                if (!e.IsCritical())
                    ErrorManager.IOError(null, ErrorType.CANT_LOAD_GROUP_FILE, e, fileName);

                throw;
            }
        }

        /** Load template file into this group using absolute filename */
        public virtual CompiledTemplate LoadAbsoluteTemplateFile(string fileName)
        {
            ANTLRFileStream fs;
            try
            {
                fs = new ANTLRFileStream(fileName, Encoding);
                fs.name = fileName;
            }
            catch (IOException)
            {
                // doesn't exist
                //errMgr.IOError(null, ErrorType.NO_SUCH_TEMPLATE, ioe, fileName);
                return null;
            }

            return LoadTemplateFile("", fileName, fs);
        }

        /** Load template stream into this group. unqualifiedFileName is "a.st".
         *  The prefix is path from group root to unqualifiedFileName like /subdir
         *  if file is in /subdir/a.st
         */
        public CompiledTemplate LoadTemplateFile(string prefix, string unqualifiedFileName, ICharStream templateStream)
        {
            GroupLexer lexer = new GroupLexer(templateStream);
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
                ErrorManager.GroupSyntaxError(ErrorType.SYNTAX_ERROR, unqualifiedFileName, re, re.Message);
            }

            string templateName = Path.GetFileNameWithoutExtension(unqualifiedFileName);
            if (!string.IsNullOrEmpty(prefix))
                templateName = prefix + templateName;

            CompiledTemplate impl = RawGetTemplate(templateName);
            impl.Prefix = prefix;
            return impl;
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
            return new Template(this);
        }

        public virtual Template CreateStringTemplate(CompiledTemplate impl)
        {
            Template st = new Template(this);
            st.impl = impl;
            if (impl.FormalArguments != null)
            {
                st.locals = new object[impl.FormalArguments.Count];
                for (int i = 0; i < st.locals.Length; i++)
                    st.locals[i] = Template.EmptyAttribute;
            }

            return st;
        }

        /** differentiate so we can avoid having creation events for regions,
         *  map operations, and other "new ST" events used during interp.
         */
        public Template CreateStringTemplateInternally(CompiledTemplate impl)
        {
            Template template = CreateStringTemplate(impl);
            if (TrackCreationEvents && template.DebugState != null)
            {
                // toss it out
                template.DebugState.NewTemplateEvent = null;
            }

            return template;
        }

        public Template CreateStringTemplateInternally(Template prototype)
        {
            // no need to wack debugState; not set in ST(proto).
            return new Template(prototype);
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

        /** Return root dir if this is group dir; return dir containing group file
         *  if this is group file.  This is derived from original incoming
         *  dir or filename.  If it was absolute, this should come back
         *  as full absolute path.  If only a URL is available, return URL of
         *  one dir up.
         */
        public virtual Uri RootDirUri
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
            if (_imports != null && _imports.Count > 0)
                buf.Append(" : " + _imports);

            foreach (string n in templates.Keys)
            {
                string name = n;
                CompiledTemplate c = templates[name];
                if (c.IsAnonSubtemplate || c == NotFoundTemplate)
                    continue;

                int slash = name.LastIndexOf('/');
                name = name.Substring(slash + 1, name.Length - slash - 1);
                buf.Append(name);
                buf.Append('(');
                if (c.FormalArguments != null)
                    buf.Append(string.Join(",", c.FormalArguments.Select(i => i.ToString()).ToArray()));

                buf.Append(')');
                buf.Append(" ::= <<" + Environment.NewLine);
                buf.Append(c.Template + Environment.NewLine);
                buf.Append(">>" + Environment.NewLine);
            }

            return buf.ToString();
        }

        public virtual ITemplateErrorListener Listener
        {
            get
            {
                if (ErrorManager == null)
                    return null;

                return ErrorManager.Listener;
            }

            set
            {
                ErrorManager = new ErrorManager(value);
            }
        }
    }
}
