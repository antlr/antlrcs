/*
 * [The "BSD licence"]
 * Copyright (c) 2003-2008 Terence Parr
 * All rights reserved.
 *
 * Conversion to C#:
 * Copyright (c) 2008 Sam Harwell, Pixel Mine, Inc.
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

namespace Antlr3.ST
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Antlr.Runtime.JavaExtensions;

    using AngleBracketTemplateLexer = Antlr3.ST.Language.AngleBracketTemplateLexer;
    using ANTLRReaderStream = Antlr.Runtime.ANTLRReaderStream;
    using Assembly = System.Reflection.Assembly;
    using CommonTokenStream = Antlr.Runtime.CommonTokenStream;
    using ConstructorInfo = System.Reflection.ConstructorInfo;
    using DefaultTemplateLexer = Antlr3.ST.Language.TemplateLexer;
    using Encoding = System.Text.Encoding;
    using GroupLexer = Antlr3.ST.Language.GroupLexer;
    using GroupParser = Antlr3.ST.Language.GroupParser;
    using IDictionary = System.Collections.IDictionary;
    using IList = System.Collections.IList;
    using IOException = System.IO.IOException;
    using MethodImpl = System.Runtime.CompilerServices.MethodImplAttribute;
    using MethodImplOptions = System.Runtime.CompilerServices.MethodImplOptions;
    using Stream = System.IO.Stream;
    using StreamReader = System.IO.StreamReader;
    using StringBuilder = System.Text.StringBuilder;
    using TextReader = System.IO.TextReader;
    using TextWriter = System.IO.TextWriter;

    /** <summary>
     *  Manages a group of named mutually-referential StringTemplate objects.
     *  Currently the templates must all live under a directory so that you
     *  can reference them as foo.st or gutter/header.st.  To refresh a
     *  group of templates, just create a new StringTemplateGroup and start
     *  pulling templates from there.  Or, set the refresh interval.
     *  </summary>
     *
     *  <remarks>
     *  Use getInstanceOf(template-name) to get a string template
     *  to fill in.
     *
     *  The name of a template is the file name minus ".st" ending if present
     *  unless you name it as you load it.
     *
     *  You can use the group file format also to define a group of templates
     *  (this works better for code gen than for html page gen).  You must give
     *  a Reader to the ctor for it to load the group; this is general and
     *  distinguishes it from the ctors for the old-style "load template files
     *  from the disk".
     *
     *  10/2005 I am adding a StringTemplateGroupLoader concept so people can define supergroups
     *  within a group and have it load that group automatically.
     *  </remarks>
     */
    public class StringTemplateGroup
    {
        /** <summary>What is the group name</summary> */
        string _name;

        /** <summary>Maps template name to StringTemplate object</summary> */
        Dictionary<string, StringTemplate> _templates = new Dictionary<string, StringTemplate>();

        /** <summary>
         *  Maps map names to HashMap objects.  This is the list of maps
         *  defined by the user like typeInitMap ::= ["int":"0"]
         *  </summary>
         */
        Dictionary<string, IDictionary> _maps = new Dictionary<string, IDictionary>();

        /** <summary>How to pull apart a template into chunks?</summary> */
        Type _templateLexerClass = null;

        /** <summary>
         *  You can set the lexer once if you know all of your groups use the
         *  same separator.  If the instance has templateLexerClass set
         *  then it is used as an override.
         *  </summary>
         */
        static Type _defaultTemplateLexerClass = typeof( DefaultTemplateLexer );

        /** <summary>
         *  Under what directory should I look for templates?  If null,
         *  to look into the CLASSPATH for templates as resources.
         *  </summary>
         */
        string _rootDir;
        Assembly _rootAssembly;

        /** <summary>Track all groups by name; maps name to StringTemplateGroup</summary> */
        //static Map _nameToGroupMap = System.Collections.Hashtable.Synchronized( new System.Collections.Hashtable() );
        public static IDictionary<string, StringTemplateGroup> _nameToGroupMap = new Dictionary<string, StringTemplateGroup>();

        /** <summary>Track all interfaces by name; maps name to StringTemplateGroupInterface</summary> */
        //static Map _nameToInterfaceMap = System.Collections.Hashtable.Synchronized( new System.Collections.Hashtable() );
        public static IDictionary<string, StringTemplateGroupInterface> _nameToInterfaceMap = new Dictionary<string, StringTemplateGroupInterface>();

        /** <summary>
         *  Are we derived from another group?  Templates not found in this group
         *  will be searched for in the superGroup recursively.
         *  </summary>
         */
        StringTemplateGroup _superGroup = null;

        /** <summary>Keep track of all interfaces implemented by this group.</summary> */
        List<StringTemplateGroupInterface> _interfaces = null;

        /** <summary>
         *  When templates are files on the disk, the refresh interval is used
         *  to know when to reload.  When a Reader is passed to the ctor,
         *  it is a stream full of template definitions.  The former is used
         *  for web development, but the latter is most likely used for source
         *  code generation for translators; a refresh is unlikely.  Anyway,
         *  I decided to track the source of templates in case such info is useful
         *  in other situations than just turning off refresh interval.  I just
         *  found another: don't ever look on the disk for individual templates
         *  if this group is a group file...immediately look into any super group.
         *  If not in the super group, report no such template.
         *  </summary>
         */
        bool _templatesDefinedInGroupFile = false;

        /** <summary>
         *  Normally AutoIndentWriter is used to filter output, but user can
         *  specify a new one.
         *  </summary>
         */
        Type _userSpecifiedWriter;

        internal bool debugTemplateOutput = false;

        /** <summary>The set of templates to ignore when dumping start/stop debug strings</summary> */
        HashSet<string> _noDebugStartStopStrings;

        /** <summary>
         *  A Map&lt;Class,Object> that allows people to register a renderer for
         *  a particular kind of object to be displayed for any template in this
         *  group.  For example, a date should be formatted differently depending
         *  on the locale.  You can set Date.class to an object whose
         *  toString(Object) method properly formats a Date attribute
         *  according to locale.  Or you can have a different renderer object
         *  for each locale.
         *  </summary>
         *
         *  <remarks>
         *  These render objects are used way down in the evaluation chain
         *  right before an attribute's toString() method would normally be
         *  called in ASTExpr.write().
         *
         *  Synchronized at creation time.
         *  </remarks>
         */
        Dictionary<Type, IAttributeRenderer> _attributeRenderers;

#if false
        /** <summary>
         *  Maps obj.prop to a value to avoid reflection costs; track one
         *  set of all class.property -> Member mappings for all ST usage in VM.
         *  </summary>
         */
        protected static IDictionary classPropertyCache = new Dictionary<object, object>();

        public class ClassPropCacheKey
        {
            Type c;
            string propertyName;
            public ClassPropCacheKey( Type c, string propertyName )
            {
                this.c = c;
                this.propertyName = propertyName;
            }

            public override bool Equals( object other )
            {
                ClassPropCacheKey otherKey = (ClassPropCacheKey)other;
                return c.Equals( otherKey.c ) &&
                    propertyName.Equals( otherKey.propertyName );
            }

            public override int GetHashCode()
            {
                return c.GetHashCode() + propertyName.GetHashCode();
            }
        }
#endif

        /** <summary>
         *  If a group file indicates it derives from a supergroup, how do we
         *  find it?  Shall we make it so the initial StringTemplateGroup file
         *  can be loaded via this loader?  Right now we pass a Reader to ctor
         *  to distinguish from the other variety.
         *  </summary>
         */
        static IStringTemplateGroupLoader _groupLoader = null;

        /** <summary>
         *  Where to report errors.  All string templates in this group
         *  use this error handler by default.
         *  </summary>
         */
        IStringTemplateErrorListener _listener = DEFAULT_ERROR_LISTENER;

        class DefaultErrorListener : IStringTemplateErrorListener
        {
            public virtual void error( string s, Exception e )
            {
                Console.Error.WriteLine( s );
                if ( e != null )
                {
                    e.printStackTrace( Console.Error );
                }
            }

            public virtual void warning( string s )
            {
                Console.Out.WriteLine( s );
            }
        }

        public static IStringTemplateErrorListener DEFAULT_ERROR_LISTENER = new DefaultErrorListener();

        /** <summary>
         *  Used to indicate that the template doesn't exist.
         *  We don't have to check disk for it; we know it's not there.
         *  </summary>
         */
        static readonly StringTemplate NOT_FOUND_ST = new StringTemplate();

        /** <summary>How long before tossing out all templates in seconds.</summary> */
        TimeSpan _refreshIntervalInSeconds = TimeSpan.FromDays( 7 ); // default: no refreshing from disk
        DateTime _lastCheckedDisk = DateTime.MinValue;

        /** <summary>
         *  How are the files encoded (ascii, UTF8, ...)?  You might want to read
         *  UTF8 for example on an ascii machine.
         *  </summary>
         */
        Encoding _fileCharEncoding = Encoding.Default;

        /** <summary>
         *  Create a group manager for some templates, all of which are
         *  at or below the indicated directory.
         *  </summary>
         */
        public StringTemplateGroup( string name, string rootDir )
            : this( name, rootDir, typeof( DefaultTemplateLexer ), Assembly.GetCallingAssembly() )
        {
        }
        public StringTemplateGroup( string name, System.Reflection.Assembly rootAssembly )
            : this( name, null, typeof( DefaultTemplateLexer ), Assembly.GetCallingAssembly() )
        {
        }

        public StringTemplateGroup( string name, string rootDir, Type lexer )
            : this( name, rootDir, lexer, Assembly.GetCallingAssembly() )
        {
        }

        public StringTemplateGroup( string name, string rootDir, Type lexer, Assembly rootAssembly )
        {
            this._name = name;
            this._rootDir = rootDir;
            this._rootAssembly = rootAssembly;
            _lastCheckedDisk = DateTime.Now;
            _nameToGroupMap[name] = this;
            this._templateLexerClass = lexer;
        }

        /** <summary>
         *  Create a group manager for some templates, all of which are
         *  loaded as resources via the classloader.
         *  </summary>
         */
        public StringTemplateGroup( string name )
            : this( name, null, null, Assembly.GetCallingAssembly() )
        {
        }

        public StringTemplateGroup( string name, Type lexer )
            : this( name, null, lexer, Assembly.GetCallingAssembly() )
        {
        }

        /** <summary>
         *  Create a group from the template group defined by a input stream.
         *  The name is pulled from the file.
         *  </summary>
         *
         *  <remarks>
         *  The format is
         *
         *  group name;
         *
         *  t1(args) ::= "..."
         *  t2() ::= &lt;&lt;
         *  >>
         *  ...
         *  </remarks>
         */
        public StringTemplateGroup( TextReader r )
            : this( r, typeof( AngleBracketTemplateLexer ), DEFAULT_ERROR_LISTENER, (StringTemplateGroup)null )
        {
        }

        public StringTemplateGroup( TextReader r, IStringTemplateErrorListener errors )
            : this( r, typeof( AngleBracketTemplateLexer ), errors, (StringTemplateGroup)null )
        {
        }

        public StringTemplateGroup( TextReader r, Type lexer )
            : this( r, lexer, null, (StringTemplateGroup)null )
        {
        }

        public StringTemplateGroup( TextReader r, Type lexer, IStringTemplateErrorListener errors )
            : this( r, lexer, errors, (StringTemplateGroup)null )
        {
        }

        /** <summary>
         *  Create a group from the input stream, but use a nondefault lexer
         *  to break the templates up into chunks.  This is usefor changing
         *  the delimiter from the default $...$ to &lt;...>, for example.
         *  </summary>
         */
        public StringTemplateGroup( TextReader r,
                                   Type lexer,
                                   IStringTemplateErrorListener errors,
                                   StringTemplateGroup superGroup )
        {
            this._templatesDefinedInGroupFile = true;
            // if no lexer specified, then assume <...> when loading from group file
            if ( lexer == null )
            {
                lexer = typeof( AngleBracketTemplateLexer );
            }
            this._templateLexerClass = lexer;
            if ( errors != null )
            { // always have to have a listener
                this._listener = errors;
            }
            SuperGroup = superGroup;
            parseGroup( r );
            _nameToGroupMap[_name] = this;
            verifyInterfaceImplementations();
        }


        #region PIXEL MINE ADDED

        public IStringTemplateErrorListener ErrorListener
        {
            get
            {
                return _listener;
            }
            set
            {
                _listener = value;
            }
        }

        public Encoding FileCharEncoding
        {
            get
            {
                return _fileCharEncoding;
            }
            set
            {
                _fileCharEncoding = value;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        /** <summary>
         *  How often to refresh all templates from disk.  This is a crude
         *  mechanism at the moment--just tosses everything out at this
         *  frequency.  Set interval to 0 to refresh constantly (no caching).
         *  Set interval to a huge number like MAX_INT to have no refreshing
         *  at all (DEFAULT); it will cache stuff.
         *  </summary>
         */
        public TimeSpan RefreshInterval
        {
            get
            {
                return _refreshIntervalInSeconds;
            }
            set
            {
                _refreshIntervalInSeconds = value;
            }
        }

        public string RootDir
        {
            get
            {
                return _rootDir;
            }
            set
            {
                _rootDir = value;
            }
        }

        public StringTemplateGroup SuperGroup
        {
            get
            {
                return _superGroup;
            }
            set
            {
                _superGroup = value;
            }
        }

        /** <summary>
         *  What lexer class to use to break up templates.  If not lexer set
         *  for this group, use static default.
         *  </summary>
         */
        public Type TemplateLexerClass
        {
            get
            {
                if ( _templateLexerClass != null )
                {
                    return _templateLexerClass;
                }
                return _defaultTemplateLexerClass;
            }
        }

        public ICollection<StringTemplate> Templates
        {
            get
            {
                return _templates.Values;
            }
        }

        #endregion

        [Obsolete]
        public Type getTemplateLexerClass()
        {
            return TemplateLexerClass;
        }

        [Obsolete]
        public string getName()
        {
            return Name;
        }

        [Obsolete]
        public void setName( string name )
        {
            Name = name;
        }

        [Obsolete]
        public void setSuperGroup( StringTemplateGroup superGroup )
        {
            SuperGroup = superGroup;
        }

        /** <summary>
         *  Called by group parser when ": supergroupname" is found.
         *  This method forces the supergroup's lexer to be same as lexer
         *  for this (sub) group.
         *  </summary>
         */
        public virtual void setSuperGroup( string superGroupName )
        {
            StringTemplateGroup superGroup =
                (StringTemplateGroup)_nameToGroupMap.get( superGroupName );
            if ( superGroup != null )
            { // we've seen before; just use it
                SuperGroup = superGroup;
                return;
            }
            // else load it using this group's template lexer
            superGroup = loadGroup( superGroupName, this._templateLexerClass, null );
            if ( superGroup != null )
            {
                _nameToGroupMap[superGroupName] = superGroup;
                SuperGroup = superGroup;
            }
            else
            {
                if ( _groupLoader == null )
                {
                    _listener.error( "no group loader registered", null );
                }
            }
        }

        /** <summary>Just track the new interface; check later.  Allows dups, but no biggie.</summary> */
        public virtual void implementInterface( StringTemplateGroupInterface I )
        {
            if ( _interfaces == null )
            {
                _interfaces = new List<StringTemplateGroupInterface>();
            }
            _interfaces.Add( I );
        }

        /** <summary>
         *  Indicate that this group implements this interface; load if necessary
         *  if not in the nameToInterfaceMap.
         *  </summary>
         */
        public virtual void implementInterface( string interfaceName )
        {
            StringTemplateGroupInterface I = _nameToInterfaceMap.get( interfaceName );
            if ( I != null )
            { // we've seen before; just use it
                implementInterface( I );
                return;
            }
            I = loadInterface( interfaceName ); // else load it
            if ( I != null )
            {
                _nameToInterfaceMap[interfaceName] = I;
                implementInterface( I );
            }
            else
            {
                if ( _groupLoader == null )
                {
                    _listener.error( "no group loader registered", null );
                }
            }
        }

        [Obsolete]
        public StringTemplateGroup getSuperGroup()
        {
            return SuperGroup;
        }

        /** <summary>Walk up group hierarchy and show top down to this group</summary> */
        public virtual string getGroupHierarchyStackString()
        {
            System.Collections.Generic.List<string> groupNames = new System.Collections.Generic.List<string>();
            StringTemplateGroup p = this;
            while ( p != null )
            {
                groupNames.Insert( 0, p._name );
                p = p._superGroup;
            }
            return "[" + string.Join( " ", groupNames.ToArray() ) + "]";
        }

        [Obsolete]
        public string getRootDir()
        {
            return RootDir;
        }

        [Obsolete]
        public void setRootDir( string rootDir )
        {
            RootDir = rootDir;
        }

        /** <summary>StringTemplate object factory; each group can have its own.</summary> */
        public virtual StringTemplate createStringTemplate()
        {
            StringTemplate st = new StringTemplate();
            return st;
        }

        /** <summary>
         *  A support routine that gets an instance of name knowing which
         *  ST encloses it for error messages.
         *  </summary>
         */
        protected virtual StringTemplate getInstanceOf( StringTemplate enclosingInstance,
                                               string name )
        {
            //System.out.println("getInstanceOf("+getName()+"::"+name+")");
            StringTemplate st = lookupTemplate( enclosingInstance, name );
            if ( st != null )
            {
                StringTemplate instanceST = st.getInstanceOf();
                return instanceST;
            }
            return null;
        }

        /** <summary>The primary means of getting an instance of a template from this group.</summary> */
        public virtual StringTemplate getInstanceOf( string name )
        {
            return getInstanceOf( null, name );
        }

        /** <summary>
         *  The primary means of getting an instance of a template from this
         *  group when you have a predefined set of attributes you want to
         *  use.
         *  </summary>
         */
        public virtual StringTemplate getInstanceOf( string name, IDictionary<string, object> attributes )
        {
            StringTemplate st = getInstanceOf( name );
            st.attributes = attributes;
            return st;
        }

        public virtual StringTemplate getEmbeddedInstanceOf( StringTemplate enclosingInstance,
                                                    string name )
        {
            /*
            System.out.println("surrounding group is "+
                               enclosingInstance.getGroup().getName()+
                               " with native group "+enclosingInstance.getNativeGroup().getName());
                               */
            StringTemplate st = null;
            // TODO: seems like this should go into lookupTemplate
            if ( name.StartsWith( "super." ) )
            {
                // for super.foo() refs, ensure that we look at the native
                // group for the embedded instance not the current evaluation
                // group (which is always pulled down to the original group
                // from which somebody did group.getInstanceOf("foo");
                st = enclosingInstance.getNativeGroup().getInstanceOf( enclosingInstance, name );
            }
            else
            {
                st = getInstanceOf( enclosingInstance, name );
            }
            // make sure all embedded templates have the same group as enclosing
            // so that polymorphic refs will start looking at the original group
            st.setGroup( this );
            st.setEnclosingInstance( enclosingInstance );
            return st;
        }

        /** <summary>
         *  Get the template called 'name' from the group.  If not found,
         *  attempt to load.  If not found on disk, then try the superGroup
         *  if any.  If not even there, then record that it's
         *  NOT_FOUND so we don't waste time looking again later.  If we've gone
         *  past refresh interval, flush and look again.
         *  </summary>
         *
         *  <remarks>
         *  If I find a template in a super group, copy an instance down here
         *  </remarks>
         */
        [MethodImpl( MethodImplOptions.Synchronized )]
        public virtual StringTemplate lookupTemplate( StringTemplate enclosingInstance, string name )
        {
            //System.out.println("look up "+getName()+"::"+name);
            if ( name.StartsWith( "super." ) )
            {
                if ( _superGroup != null )
                {
                    int dot = name.IndexOf( '.' );
                    name = name.Substring( dot + 1 );
                    StringTemplate superScopeST =
                        _superGroup.lookupTemplate( enclosingInstance, name );
                    /*
                    System.out.println("superScopeST is "+
                                       superScopeST.getGroup().getName()+"::"+name+
                                       " with native group "+superScopeST.getNativeGroup().getName());
                    */
                    return superScopeST;
                }
                throw new ArgumentException( Name + " has no super group; invalid template: " + name );
            }
            checkRefreshInterval();
            StringTemplate st;
            if ( !_templates.TryGetValue( name, out st ) || st == null )
            {
                // not there?  Attempt to load
                if ( !_templatesDefinedInGroupFile )
                {
                    // only check the disk for individual template
                    st = loadTemplateFromBeneathRootDirOrCLASSPATH( getFileNameFromTemplateName( name ) );
                }
                if ( st == null && _superGroup != null )
                {
                    // try to resolve in super group
                    st = _superGroup.getInstanceOf( name );
                    // make sure that when we inherit a template, that it's
                    // group is reset; it's nativeGroup will remain where it was
                    if ( st != null )
                    {
                        st.setGroup( this );
                    }
                }
                if ( st != null )
                { // found in superGroup
                    // insert into this group; refresh will allow super
                    // to change it's def later or this group to add
                    // an override.
                    _templates[name] = st;
                }
                else
                {
                    // not found; remember that this sucker doesn't exist
                    _templates[name] = NOT_FOUND_ST;
                    string context = "";
                    if ( enclosingInstance != null )
                    {
                        context = "; context is " +
                                  enclosingInstance.getEnclosingInstanceStackString();
                    }
                    string hier = getGroupHierarchyStackString();
                    context += "; group hierarchy is " + hier;
                    throw new ArgumentException( "Can't find template " +
                                                       getFileNameFromTemplateName( name ) +
                                                       context );
                }
            }
            else if ( st == NOT_FOUND_ST )
            {
                return null;
            }
            //System.out.println("lookup found "+st.getGroup().getName()+"::"+st.getName());
            return st;
        }

        public virtual StringTemplate lookupTemplate( string name )
        {
            try
            {
                return lookupTemplate( null, name );
            }
            catch ( ArgumentException )
            {
                return null;
            }
        }

        protected virtual void checkRefreshInterval()
        {
            if ( _templatesDefinedInGroupFile )
            {
                return;
            }
            bool timeToFlush = _refreshIntervalInSeconds == TimeSpan.Zero ||
                                ( DateTime.Now - _lastCheckedDisk ) >= _refreshIntervalInSeconds;
            if ( timeToFlush )
            {
                // throw away all pre-compiled references
                _templates.Clear();
                _lastCheckedDisk = DateTime.Now;
            }
        }

        protected virtual StringTemplate loadTemplate( string name, TextReader r )
        {
            string line;
            string nl = Environment.NewLine;
            StringBuilder buf = new StringBuilder( 300 );
            while ( ( line = r.ReadLine() ) != null )
            {
                buf.Append( line );
                buf.Append( nl );
            }
            // strip newlines etc.. from front/back since filesystem
            // may add newlines etc...
            string pattern = buf.ToString().Trim();
            if ( pattern.Length == 0 )
            {
                error( "no text in template '" + name + "'" );
                return null;
            }
            return defineTemplate( name, pattern );
        }

        /** <summary>
         *  Load a template whose name is derived from the template filename.
         *  If there is no root directory, try to load the template from
         *  the classpath.  If there is a rootDir, try to load the file
         *  from there.
         *  </summary>
         */
        protected virtual StringTemplate loadTemplateFromBeneathRootDirOrCLASSPATH( string fileName )
        {
            StringTemplate template = null;
            string name = getTemplateNameFromFileName( fileName );
            // if no rootDir, try to load as a resource in CLASSPATH
            if ( RootDir == null )
            {
                string resourceName = getFileNameFromTemplateName( name.Replace( '/', '.' ) );
                System.Reflection.Assembly assembly = _rootAssembly ?? System.Reflection.Assembly.GetCallingAssembly();
                System.IO.Stream @is = assembly.GetManifestResourceStream( resourceName );
                if ( @is == null )
                {
                    assembly = this.GetType().Assembly;
                    @is = assembly.GetManifestResourceStream( resourceName );
                }
#if false
			ClassLoader cl = Thread.currentThread().getContextClassLoader();
			InputStream @is = cl.getResourceAsStream(fileName);
			if ( @is==null ) {
				cl = this.getClass().getClassLoader();
				@is = cl.getResourceAsStream(fileName);
			}
#endif
                if ( @is == null )
                {
                    return null;
                }
                TextReader br = null;
                try
                {
                    br = getInputStreamReader( new System.IO.BufferedStream( @is ) );
                    template = loadTemplate( name, br );
                }
                catch ( IOException ioe )
                {
                    error( "Problem reading template file: " + fileName, ioe );
                }
                finally
                {
                    if ( br != null )
                    {
                        try
                        {
                            br.Close();
                        }
                        catch ( IOException ioe2 )
                        {
                            error( "Cannot close template file: " + fileName, ioe2 );
                        }
                    }
                }
                return template;
            }
            // load via rootDir
            template = loadTemplate( name, RootDir + "/" + fileName );
            return template;
        }

        /** <summary>
         *  (public so that people can override behavior; not a general
         *  purpose method)
         *  </summary>
         */
        public virtual string getFileNameFromTemplateName( string templateName )
        {
            return templateName + ".st";
        }

        /** <summary>
         *  Convert a filename relativePath/name.st to relativePath/name.
         *  (public so that people can override behavior; not a general
         *  purpose method)
         *  </summary>
         */
        public virtual string getTemplateNameFromFileName( string fileName )
        {
            string name = fileName;
            int suffix = name.LastIndexOf( ".st" );
            if ( suffix >= 0 )
            {
                name = name.Substring( 0, suffix );
            }
            return name;
        }

        protected virtual StringTemplate loadTemplate( string name, string fileName )
        {
            TextReader br = null;
            StringTemplate template = null;
            try
            {
                Stream fin = System.IO.File.OpenRead( fileName );
                StreamReader isr = getInputStreamReader( new System.IO.BufferedStream( fin ) );
                br = isr;
                template = loadTemplate( name, br );
                br.Close();
                br = null;
            }
            catch ( IOException /*ioe*/)
            {
                if ( br != null )
                {
                    try
                    {
                        br.Close();
                    }
                    catch ( IOException /*ioe2*/)
                    {
                        error( "Cannot close template file: " + fileName );
                    }
                }
            }
            return template;
        }

        protected virtual StreamReader getInputStreamReader( Stream @in )
        {
            StreamReader isr = null;
            try
            {
                isr = new StreamReader( @in, _fileCharEncoding );
            }
            catch ( ArgumentException /*uee*/)
            {
                error( "Invalid file character encoding: " + _fileCharEncoding );
            }
            return isr;
        }

        [Obsolete]
        public Encoding getFileCharEncoding()
        {
            return FileCharEncoding;
        }

        [Obsolete]
        public void setFileCharEncoding( Encoding fileCharEncoding )
        {
            FileCharEncoding = fileCharEncoding;
        }

        /** <summary>
         *  Define an examplar template; precompiled and stored
         *  with no attributes.  Remove any previous definition.
         *  </summary>
         */
        [MethodImpl( MethodImplOptions.Synchronized )]
        public virtual StringTemplate defineTemplate( string name, string template )
        {
            //System.out.println("defineTemplate "+getName()+"::"+name);
            if ( name != null && name.IndexOf( '.' ) >= 0 )
            {
                throw new ArgumentException( "cannot have '.' in template names" );
            }
            StringTemplate st = createStringTemplate();
            st.setName( name );
            st.setGroup( this );
            st.setNativeGroup( this );
            st.setTemplate( template );
            st.setErrorListener( _listener );
            _templates[name] = st;
            return st;
        }

        /** <summary>Track all references to regions &lt;@foo>...&lt;@end> or &lt;@foo()>.</summary> */
        public virtual StringTemplate defineRegionTemplate( string enclosingTemplateName,
                                                   string regionName,
                                                   string template,
                                                   int type )
        {
            string mangledName =
                getMangledRegionName( enclosingTemplateName, regionName );
            StringTemplate regionST = defineTemplate( mangledName, template );
            regionST.setIsRegion( true );
            regionST.setRegionDefType( type );
            return regionST;
        }

        /** <summary>Track all references to regions &lt;@foo>...&lt;@end> or &lt;@foo()>.</summary>  */
        public virtual StringTemplate defineRegionTemplate( StringTemplate enclosingTemplate,
                                                   string regionName,
                                                   string template,
                                                   int type )
        {
            StringTemplate regionST =
                defineRegionTemplate( enclosingTemplate.getOutermostName(),
                                     regionName,
                                     template,
                                     type );
            enclosingTemplate.getOutermostEnclosingInstance().addRegionName( regionName );
            return regionST;
        }

        /** <summary>Track all references to regions &lt;@foo()>.  We automatically
         *  define as
         *
         *     @enclosingtemplate.foo() ::= ""
         *  </summary>
         *
         *  <remarks>
         *  You cannot set these manually in the same group; you have to subgroup
         *  to override.
         *  </remarks>
         */
        public virtual StringTemplate defineImplicitRegionTemplate( StringTemplate enclosingTemplate,
                                                           string name )
        {
            return defineRegionTemplate( enclosingTemplate,
                                        name,
                                        "",
                                        StringTemplate.REGION_IMPLICIT );

        }

        /** <summary>The "foo" of t() ::= "&lt;@foo()>" is mangled to "region#t#foo"</summary> */
        public virtual string getMangledRegionName( string enclosingTemplateName,
                                           string name )
        {
            return "region__" + enclosingTemplateName + "__" + name;
        }

        /** <summary>Return "t" from "region__t__foo"</summary> */
        public virtual string getUnMangledTemplateName( string mangledName )
        {
            return mangledName.substring( "region__".Length,
                                         mangledName.LastIndexOf( "__" ) );
        }

        /** <summary>Make name and alias for target.  Replace any previous def of name</summary> */
        [MethodImpl( MethodImplOptions.Synchronized )]
        public virtual StringTemplate defineTemplateAlias( string name, string target )
        {
            StringTemplate targetST = getTemplateDefinition( target );
            if ( targetST == null )
            {
                error( "cannot alias " + name + " to undefined template: " + target );
                return null;
            }
            _templates[name] = targetST;
            return targetST;
        }

        [MethodImpl( MethodImplOptions.Synchronized )]
        public virtual bool isDefinedInThisGroup( string name )
        {
            StringTemplate st;
            if ( _templates.TryGetValue( name, out st ) && st != null )
            {
                if ( st.getIsRegion() )
                {
                    // don't allow redef of @t.r() ::= "..." or <@r>...<@end>
                    if ( st.getRegionDefType() == StringTemplate.REGION_IMPLICIT )
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        /** <summary>Get the ST for 'name' in this group only</summary> */
        [MethodImpl( MethodImplOptions.Synchronized )]
        public virtual StringTemplate getTemplateDefinition( string name )
        {
            return _templates[name];
        }

        /** <summary>
         *  Is there *any* definition for template 'name' in this template
         *  or above it in the group hierarchy?
         *  </summary>
         */
        public virtual bool isDefined( string name )
        {
            try
            {
                return lookupTemplate( name ) != null;
            }
            catch ( ArgumentException /*iae*/)
            {
                return false;
            }
        }

        protected virtual void parseGroup( TextReader r )
        {
            try
            {
                GroupLexer lexer = new GroupLexer( new ANTLRReaderStream( r ) );
                GroupParser parser = new GroupParser( new CommonTokenStream( lexer ) );
                parser.group( this );
                //System.out.println("read group\n"+this.toString());
            }
            catch ( Exception e )
            {
                string name = "<unknown>";
                if ( Name != null )
                {
                    name = Name;
                }
                error( "problem parsing group " + name + ": " + e, e );
            }
        }

        /** <summary>Verify that this group satisfies its interfaces</summary> */
        protected virtual void verifyInterfaceImplementations()
        {
            for ( int i = 0; _interfaces != null && i < _interfaces.Count; i++ )
            {
                StringTemplateGroupInterface I = _interfaces[i];
                IList missing = I.getMissingTemplates( this );
                IList mismatched = I.getMismatchedTemplates( this );
                if ( missing != null )
                {
                    string missingText = "[" + string.Join( ",", missing.Cast<string>().ToArray() ) + "]";
                    error( "group " + Name + " does not satisfy interface " +
                          I.getName() + ": missing templates " + missingText );
                }
                if ( mismatched != null )
                {
                    string mismatchedText = "[" + string.Join( ",", mismatched.Cast<string>().ToArray() ) + "]";
                    error( "group " + Name + " does not satisfy interface " +
                          I.getName() + ": mismatched arguments on these templates " + mismatchedText );
                }
            }
        }

        [Obsolete]
        public TimeSpan getRefreshInterval()
        {
            return RefreshInterval;
        }

        [Obsolete]
        public void setRefreshInterval( TimeSpan refreshInterval )
        {
            RefreshInterval = refreshInterval;
        }

        [Obsolete]
        public void setErrorListener( IStringTemplateErrorListener listener )
        {
            ErrorListener = listener;
        }

        [Obsolete]
        public IStringTemplateErrorListener getErrorListener()
        {
            return ErrorListener;
        }

        /** <summary>
         *  Specify a StringTemplateWriter implementing class to use for
         *  filtering output
         *  </summary>
         */
        public virtual void setStringTemplateWriter( Type c )
        {
            _userSpecifiedWriter = c;
        }

        /** <summary>
         *  Return an instance of a StringTemplateWriter that spits output to w.
         *  If a writer is specified, use it instead of the default.
         *  </summary>
         */
        public virtual IStringTemplateWriter getStringTemplateWriter( TextWriter w )
        {
            IStringTemplateWriter stw = null;
            if ( _userSpecifiedWriter != null )
            {
                try
                {
                    ConstructorInfo ctor =
                        _userSpecifiedWriter.GetConstructor( new Type[] { typeof( TextWriter ) } );
                    stw = (IStringTemplateWriter)ctor.Invoke( new Object[] { w } );
                }
                catch ( Exception e )
                {
                    error( "problems getting StringTemplateWriter", e );
                }
            }
            if ( stw == null )
            {
                stw = new AutoIndentWriter( w );
            }
            return stw;
        }

        /** <summary>
         *  Specify a complete map of what object classes should map to which
         *  renderer objects for every template in this group (that doesn't
         *  override it per template).
         *  </summary>
         */
        public virtual void setAttributeRenderers( Dictionary<Type, IAttributeRenderer> renderers )
        {
            this._attributeRenderers = renderers;
        }

        /** <summary>
         *  Register a renderer for all objects of a particular type for all
         *  templates in this group.
         *  </summary>
         */
        public virtual void registerRenderer( Type attributeClassType, IAttributeRenderer renderer )
        {
            if ( _attributeRenderers == null )
            {
                _attributeRenderers = new Dictionary<Type, IAttributeRenderer>();
            }
            _attributeRenderers[attributeClassType] = renderer;
        }

        /** <summary>
         *  What renderer is registered for this attributeClassType for
         *  this group?  If not found, as superGroup if it has one.
         *  </summary>
         */
        public virtual IAttributeRenderer getAttributeRenderer( Type attributeClassType )
        {
            if ( _attributeRenderers == null )
            {
                if ( _superGroup == null )
                {
                    return null; // no renderers and no parent?  Stop.
                }
                // no renderers; consult super group
                return _superGroup.getAttributeRenderer( attributeClassType );
            }

            IAttributeRenderer renderer;
            if ( !_attributeRenderers.TryGetValue( attributeClassType, out renderer ) || renderer == null )
            {
                if ( _superGroup != null )
                {
                    // no renderer registered for this class, check super group
                    renderer = _superGroup.getAttributeRenderer( attributeClassType );
                }
            }
            return renderer;
        }

#if false
        public virtual void cacheClassProperty(Type c, String propertyName, Member member) {
            Object key = new ClassPropCacheKey(c,propertyName);
            classPropertyCache.put(key,member);
        }

        public virtual Member getCachedClassProperty(Type c, String propertyName) {
            Object key = new ClassPropCacheKey(c,propertyName);
            return (Member)classPropertyCache.get(key);
        }
#endif

        public virtual IDictionary getMap( string name )
        {
            if ( _maps == null )
            {
                if ( _superGroup == null )
                {
                    return null;
                }
                return _superGroup.getMap( name );
            }
            IDictionary m;
            if ( ( !_maps.TryGetValue( name, out m ) || m == null ) && _superGroup != null )
            {
                m = _superGroup.getMap( name );
            }
            return m;
        }

        /** <summary>
         *  Define a map for this group; not thread safe...do not keep adding
         *  these while you reference them.
         *  </summary>
         */
        public virtual void defineMap( string name, IDictionary mapping )
        {
            _maps[name] = mapping;
        }

        public static void registerDefaultLexer( Type lexerClass )
        {
            _defaultTemplateLexerClass = lexerClass;
        }

        public static void registerGroupLoader( IStringTemplateGroupLoader loader )
        {
            _groupLoader = loader;
        }

        public static StringTemplateGroup loadGroup( string name )
        {
            return loadGroup( name, null, null );
        }

        public static StringTemplateGroup loadGroup( string name,
                                                    StringTemplateGroup superGroup )
        {
            return loadGroup( name, null, superGroup );
        }

        public static StringTemplateGroup loadGroup( string name,
                                                    Type lexer,
                                                    StringTemplateGroup superGroup )
        {
            if ( _groupLoader != null )
            {
                return _groupLoader.loadGroup( name, lexer, superGroup );
            }
            return null;
        }

        public static StringTemplateGroupInterface loadInterface( string name )
        {
            if ( _groupLoader != null )
            {
                return _groupLoader.loadInterface( name );
            }
            return null;
        }

        public virtual void error( string msg )
        {
            error( msg, null );
        }

        public virtual void error( string msg, Exception e )
        {
            if ( _listener != null )
            {
                _listener.error( msg, e );
            }
            else
            {
                Console.Error.WriteLine( "StringTemplate: " + msg );
                if ( e != null )
                {
                    e.printStackTrace();
                }
            }
        }

        [MethodImpl( MethodImplOptions.Synchronized )]
        public virtual ICollection<string> getTemplateNames()
        {
            return _templates.Keys;
        }

        /** <summary>
         *  Indicate whether ST should emit &lt;templatename>...&lt;/templatename>
         *  strings for debugging around output for templates from this group.
         *  </summary>
         */
        public virtual void emitDebugStartStopStrings( bool emit )
        {
            this.debugTemplateOutput = emit;
        }

        public virtual void doNotEmitDebugStringsForTemplate( string templateName )
        {
            if ( _noDebugStartStopStrings == null )
            {
                _noDebugStartStopStrings = new HashSet<string>();
            }
            _noDebugStartStopStrings.Add( templateName );
        }

        public virtual void emitTemplateStartDebugString( StringTemplate st,
                                                 IStringTemplateWriter @out )
        {
            if ( _noDebugStartStopStrings == null ||
                 !_noDebugStartStopStrings.Contains( st.getName() ) )
            {
                string groupPrefix = "";
                if ( !st.getName().StartsWith( "if" ) && !st.getName().StartsWith( "else" ) )
                {
                    if ( st.getNativeGroup() != null )
                    {
                        groupPrefix = st.getNativeGroup().Name + ".";
                    }
                    else
                    {
                        groupPrefix = st.getGroup().Name + ".";
                    }
                }
                @out.write( "<" + groupPrefix + st.getName() + ">" );
            }
        }

        public virtual void emitTemplateStopDebugString( StringTemplate st,
                                                IStringTemplateWriter @out )
        {
            if ( _noDebugStartStopStrings == null ||
                 !_noDebugStartStopStrings.Contains( st.getName() ) )
            {
                string groupPrefix = "";
                if ( !st.getName().StartsWith( "if" ) && !st.getName().StartsWith( "else" ) )
                {
                    if ( st.getNativeGroup() != null )
                    {
                        groupPrefix = st.getNativeGroup().Name + ".";
                    }
                    else
                    {
                        groupPrefix = st.getGroup().Name + ".";
                    }
                }
                @out.write( "</" + groupPrefix + st.getName() + ">" );
            }
        }

        public override string ToString()
        {
            return ToString( true );
        }

        string _newline = Environment.NewLine;

        public virtual string ToString( bool showTemplatePatterns )
        {
            StringBuilder buf = new StringBuilder();
            buf.Append( "group " + Name + ";" + _newline );
            StringTemplate formalArgs = new StringTemplate( "$args;separator=\",\"$" );
            foreach ( var template in _templates.OrderBy( item => (string)item.Key ) )
            {
                string tname = template.Key;
                StringTemplate st = template.Value;
                if ( st != NOT_FOUND_ST )
                {
                    formalArgs = formalArgs.getInstanceOf();
                    formalArgs.setAttribute( "args", st.getFormalArguments() );
                    buf.Append( tname + "(" + formalArgs + ")" );
                    if ( showTemplatePatterns )
                    {
                        buf.Append( " ::= <<" );
                        buf.Append( st.getTemplate() );
                        buf.Append( ">>" + _newline );
                    }
                    else
                    {
                        buf.Append( _newline );
                    }
                }
            }
            return buf.ToString();
        }
    }
}
