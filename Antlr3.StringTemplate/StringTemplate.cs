/*
 * [The "BSD licence"]
 * Copyright (c) 2003-2008 Terence Parr
 * All rights reserved.
 *
 * Conversion to C#:
 * Copyright (c) 2008-2009 Sam Harwell, Pixel Mine, Inc.
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
    using Antlr3.ST.Language;

    using CommonToken = Antlr.Runtime.CommonToken;
    using CommonTokenStream = Antlr.Runtime.CommonTokenStream;
    using CommonTree = Antlr.Runtime.Tree.CommonTree;
    using ConstructorInfo = System.Reflection.ConstructorInfo;
    using ICollection = System.Collections.ICollection;
    using IDictionary = System.Collections.IDictionary;
    using IList = System.Collections.IList;
    using IOException = System.IO.IOException;
    using ITree = Antlr.Runtime.Tree.ITree;
    using Lexer = Antlr.Runtime.Lexer;
    using RecognitionException = Antlr.Runtime.RecognitionException;
    using StringBuilder = System.Text.StringBuilder;
    using StringReader = System.IO.StringReader;
    using StringWriter = System.IO.StringWriter;
    using TargetInvocationException = System.Reflection.TargetInvocationException;
    using TextReader = System.IO.TextReader;

    /** <summary>
     *  A <tt>StringTemplate</tt> is a "document" with holes in it where you can stick
     *  values.  <tt>StringTemplate</tt> breaks up your template into chunks of text and
     *  attribute expressions.  <tt>StringTemplate</tt> ignores everything outside
     *  of attribute expressions, treating it as just text to spit
     *  out when you call <tt>StringTemplate.toString()</tt>.
     *  </summary>
     */
    public class StringTemplate
    {
        public const string VERSION = "3.3a"; // August 11, 2008

        /** <summary>&lt;@r()&gt;</summary> */
        public const int REGION_IMPLICIT = 1;
        /** <summary>&lt;@r&gt;...&lt;@end&gt;</summary> */
        public const int REGION_EMBEDDED = 2;
        /** <summary>@t.r() ::= "..." defined manually by coder</summary> */
        public const int REGION_EXPLICIT = 3;

        /** <summary>An automatically created aggregate of properties.</summary>
         *
         *  <remarks>
         *  I often have lists of things that need to be formatted, but the list
         *  items are actually pieces of data that are not already in an object.  I
         *  need ST to do something like:
         *
         *  Ter=3432
         *  Tom=32234
         *  ....
         *
         *  using template:
         *
         *  $items:{$attr.name$=$attr.type$}$
         *
         *  This example will call getName() on the objects in items attribute, but
         *  what if they aren't objects?  I have perhaps two parallel arrays
         *  instead of a single array of objects containing two fields.  One
         *  solution is allow Maps to be handled like properties so that it.name
         *  would fail getName() but then see that it's a Map and do
         *  it.get("name") instead.
         *
         *  This very clean approach is espoused by some, but the problem is that
         *  it's a hole in my separation rules.  People can put the logic in the
         *  view because you could say: "go get bob's data" in the view:
         *
         *  Bob's Phone: $db.bob.phone$
         *
         *  A view should not be part of the program and hence should never be able
         *  to go ask for a specific person's data.
         *
         *  After much thought, I finally decided on a simple solution.  I've
         *  added setAttribute variants that pass in multiple property values,
         *  with the property names specified as part of the name using a special
         *  attribute name syntax: "name.{propName1,propName2,...}".  This
         *  object is a special kind of HashMap that hopefully prevents people
         *  from passing a subclass or other variant that they have created as
         *  it would be a loophole.  Anyway, the ASTExpr.getObjectProperty()
         *  method looks for Aggregate as a special case and does a get() instead
         *  of getPropertyName.
         *  </remarks>
         */
        public sealed class Aggregate
        {
            Dictionary<string, object> properties = new Dictionary<string, object>();
            /** <summary>
             *  Allow StringTemplate to add values, but prevent the end
             *  user from doing so.
             *  </summary>
             */
            public void Put( string propName, object propValue )
            {
                properties[propName] = propValue;
            }
            public object Get( string propName )
            {
                object value;
                if ( properties.TryGetValue( propName, out value ) )
                    return value;

                return null;
            }
            public override string ToString()
            {
                return properties.ToString();
            }
        }

        /** <summary>
         *  Just an alias for ArrayList, but this way I can track whether a
         *  list is something ST created or it's an incoming list.
         *  </summary>
         */
        public sealed class STAttributeList : List<object>
        {
            public STAttributeList( int size ) : base( size )
            {
            }
            public STAttributeList()
            {
            }
        }

        public const string ANONYMOUS_ST_NAME = "anonymous";

        /** <summary>Track probable issues like setting attribute that is not referenced.</summary> */
        static bool _lintMode = false;

        List<string> _referencedAttributes = null;

        /** <summary>What's the name of this template?</summary> */
        string _name = ANONYMOUS_ST_NAME;

        static int _templateCounter = 0;
        static int GetNextTemplateCounter()
        {
            return System.Threading.Interlocked.Increment( ref _templateCounter );
        }
        /** <summary>
         *  Reset the template ID counter to 0; public so that testing routine
         *  can access but not really of interest to the user.
         *  </summary>
         */
        public static void ResetTemplateCounter()
        {
            _templateCounter = 0;
        }

        int _templateID = GetNextTemplateCounter();

        /** <summary>
         *  Enclosing instance if I'm embedded within another template.
         *  IF-subtemplates are considered embedded as well.
         *  </summary>
         */
        StringTemplate _enclosingInstance = null;

        /** <summary>
         *  If this template is an embedded template such as when you apply
         *  a template to an attribute, then the arguments passed to this
         *  template represent the argument context--a set of values
         *  computed by walking the argument assignment list.  For example,
         *  &lt;name:bold(item=name, foo="x")&gt; would result in an
         *  argument context of {[item=name], [foo="x"]} for this
         *  template.  This template would be the bold() template and
         *  the enclosingInstance would point at the template that held
         *  that &lt;name:bold(...)&gt; template call.  When you want to get
         *  an attribute value, you first check the attributes for the
         *  'self' template then the arg context then the enclosingInstance
         *  like resolving variables in pascal-like language with nested
         *  procedures.
         *  </summary>
         *
         *  <remarks>
         *  With multi-valued attributes such as &lt;faqList:briefFAQDisplay()&gt;
         *  attribute "i" is set to 1..n.
         *  </remarks>
         */
        Dictionary<string, object> _argumentContext = null;

        /** <summary>
         *  If this template is embedded in another template, the arguments
         *  must be evaluated just before each application when applying
         *  template to a list of values.  The "it" attribute must change
         *  with each application so that $names:bold(item=it)$ works.  If
         *  you evaluate once before starting the application loop then it
         *  has a single fixed value.  Eval.g saves the AST rather than evaluating
         *  before invoking applyListOfAlternatingTemplates().  Each iteration
         *  of a template application to a multi-valued attribute, these args
         *  are re-evaluated with an initial context of {[it=...], [i=...]}.
         *  </summary>
         */
        StringTemplateAST _argumentsAST = null;

        /** <summary>
         *  When templates are defined in a group file format, the attribute
         *  list is provided including information about attribute cardinality
         *  such as present, optional, ...  When this information is available,
         *  rawSetAttribute should do a quick existence check as should the
         *  invocation of other templates.  So if you ref bold(item="foo") but
         *  item is not defined in bold(), then an exception should be thrown.
         *  When actually rendering the template, the cardinality is checked.
         *  This is a Map&lt;String,FormalArgument>.
         *  </summary>
         */
        IList<FormalArgument> _formalArguments = FormalArgument.UNKNOWN;

        /** <summary>
         *  How many formal arguments to this template have default values
         *  specified?
         *  </summary>
         */
        int _numberOfDefaultArgumentValues = 0;

        /** <summary>
         *  Normally, formal parameters hide any attributes inherited from the
         *  enclosing template with the same name.  This is normally what you
         *  want, but makes it hard to invoke another template passing in all
         *  the data.  Use notation now: &lt;otherTemplate(...)> to say "pass in
         *  all data".  Works great.  Can also say &lt;otherTemplate(foo="xxx",...)>
         *  </summary>
         */
        bool _passThroughAttributes = false;

        /** <summary>
         *  What group originally defined the prototype for this template?
         *  This affects the set of templates I can refer to.  super.t() must
         *  always refer to the super of the original group.
         *  </summary>
         *
         *  <remarks>
         *  group base;
         *  t ::= "base";
         *
         *  group sub;
         *  t ::= "super.t()2"
         *
         *  group subsub;
         *  t ::= "super.t()3"
         *  </remarks>
         */
        StringTemplateGroup _nativeGroup;

        /** <summary>
         *  This template was created as part of what group?  Even if this
         *  template was created from a prototype in a supergroup, its group
         *  will be the subgroup.  That's the way polymorphism works.
         *  </summary>
         */
        StringTemplateGroup _group;


        /** <summary>If this template is defined within a group file, what line number?</summary> */
        int _groupFileLine;

        /** <summary>Where to report errors</summary> */
        IStringTemplateErrorListener _listener = null;

        /** <summary>
         *  The original, immutable pattern/language (not really used again after
         *  initial "compilation", setup/parsing).
         *  </summary>
         */
        string _pattern;

        /** <summary>
         *  Map an attribute name to its value(s).  These values are set by outside
         *  code via st.setAttribute(name, value).  StringTemplate is like self in
         *  that a template is both the "class def" and "instance".  When you
         *  create a StringTemplate or setTemplate, the text is broken up into chunks
         *  (i.e., compiled down into a series of chunks that can be evaluated later).
         *  You can have multiple
         *  </summary>
         */
        protected internal IDictionary<string, object> attributes;

        /** <summary>
         *  A Map&lt;Class,Object> that allows people to register a renderer for
         *  a particular kind of object to be displayed in this template.  This
         *  overrides any renderer set for this template's group.
         *  </summary>
         *
         *  <remarks>
         *  Most of the time this map is not used because the StringTemplateGroup
         *  has the general renderer map for all templates in that group.
         *  Sometimes though you want to override the group's renderers.
         *  </remarks>
          */
        Dictionary<Type, IAttributeRenderer> _attributeRenderers;

        /** <summary>
         *  A list of alternating string and ASTExpr references.
         *  This is compiled to when the template is loaded/defined and walked to
         *  write out a template instance.
         *  </summary>
         */
        List<Expr> _chunks;

        /** <summary>If someone refs &lt;@r()> in template t, an implicit
         *
         *   @t.r() ::= ""
         *
         *  is defined, but you can overwrite this def by defining your
         *  own.  We need to prevent more than one manual def though.  Between
         *  this var and isEmbeddedRegion we can determine these cases.
         *  </summary>
         */
        int _regionDefType;

        /** <summary>
         *  Does this template come from a &lt;@region>...&lt;@end> embedded in
         *  another template?
         *  </summary>
         */
        bool _isRegion;

        /** <summary>Set of implicit and embedded regions for this template</summary> */
        HashSet<object> _regions;

        public static StringTemplateGroup defaultGroup =
            new StringTemplateGroup( "defaultGroup", "." );

        /** <summary>Create a blank template with no pattern and no attributes</summary> */
        public StringTemplate()
        {
            _group = defaultGroup; // make sure has a group even if default
        }

        /** <summary>
         *  Create an anonymous template.  It has no name just
         *  chunks (which point to this anonymous template) and attributes.
         *  </summary>
         */
        public StringTemplate( string template )
            : this( null, template )
        {
        }

        public StringTemplate( string template, Type lexer )
            : this()
        {
            Group = new StringTemplateGroup( "defaultGroup", lexer );
            Template = template;
        }

        /** <summary>Create an anonymous template with no name, but with a group</summary> */
        public StringTemplate( StringTemplateGroup group, string template )
            : this()
        {
            if ( group != null )
            {
                Group = group;
            }
            Template = template;
        }

        public StringTemplate( StringTemplateGroup group,
                              string template,
                              IDictionary<string, object> attributes )
            : this( group, template )
        {
            this.attributes = attributes;
        }

        #region PIXEL MINE ADDED

        public Dictionary<string, object> ArgumentContext
        {
            get
            {
                return _argumentContext;
            }
            set
            {
                _argumentContext = value;
            }
        }

        public StringTemplateAST ArgumentsAST
        {
            get
            {
                return _argumentsAST;
            }
            set
            {
                _argumentsAST = value;
            }
        }

        public IDictionary<string, object> Attributes
        {
            get
            {
                return attributes;
            }
            set
            {
                attributes = value;
            }
        }

        /** <summary>
         *  Get a list of the strings and subtemplates and attribute
         *  refs in a template.
         *  </summary>
         */
        public IList<Expr> Chunks
        {
            get
            {
                return _chunks;
            }
        }

        public StringTemplate EnclosingInstance
        {
            get
            {
                return _enclosingInstance;
            }
            set
            {
                if ( this == value )
                    throw new ArgumentException( "cannot embed template " + Name + " in itself" );

                // set the parent for this template
                _enclosingInstance = value;
            }
        }

        public IStringTemplateErrorListener ErrorListener
        {
            get
            {
                if ( _listener == null )
                    return _group.ErrorListener;

                return _listener;
            }
            set
            {
                _listener = value;
            }
        }

        public StringTemplateGroup Group
        {
            get
            {
                return _group;
            }
            set
            {
                _group = value;
            }
        }

        /** <summary>Gets or sets the outermost template's group file line number</summary> */
        public int GroupFileLine
        {
            get
            {
                if ( _enclosingInstance != null )
                    return _enclosingInstance.GroupFileLine;

                return _groupFileLine;
            }
            set
            {
                _groupFileLine = value;
            }
        }

        public IList<FormalArgument> FormalArguments
        {
            get
            {
                return _formalArguments;
            }
            private set
            {
                _formalArguments = value;
            }
        }

        public bool IsRegion
        {
            get
            {
                return _isRegion;
            }
            set
            {
                _isRegion = value;
            }
        }

        /** <summary>
         *  Make StringTemplate check your work as it evaluates templates.
         *  Problems are sent to error listener.   Currently warns when
         *  you set attributes that are not used.
         *  </summary>
         */
        public static bool LintMode
        {
            get
            {
                return _lintMode;
            }
            set
            {
                _lintMode = value;
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

        public StringTemplateGroup NativeGroup
        {
            get
            {
                return _nativeGroup;
            }
            set
            {
                _nativeGroup = value;
            }
        }

        public StringTemplate OutermostEnclosingInstance
        {
            get
            {
                if ( _enclosingInstance != null )
                    return _enclosingInstance.OutermostEnclosingInstance;

                return this;
            }
        }

        public string OutermostName
        {
            get
            {
                if ( _enclosingInstance != null )
                    return _enclosingInstance.OutermostName;

                return Name;
            }
        }

        public int RegionDefType
        {
            get
            {
                return _regionDefType;
            }
            set
            {
                _regionDefType = value;
            }
        }

        public string Template
        {
            get
            {
                return _pattern;
            }
            set
            {
                _pattern = value;
                BreakTemplateIntoChunks();
            }
        }

        public string TemplateDeclaratorString
        {
            get
            {
                return GetTemplateDeclaratorString();
            }
        }

        public int TemplateID
        {
            get
            {
                return _templateID;
            }
        }

        #endregion

        /** <summary>
         *  Make the 'to' template look exactly like the 'from' template
         *  except for the attributes.  This is like creating an instance
         *  of a class in that the executable code is the same (the template
         *  chunks), but the instance data is blank (the attributes).  Do
         *  not copy the enclosingInstance pointer since you will want this
         *  template to eval in a context different from the examplar.
         *  </summary>
         */
        protected virtual void Dup( StringTemplate from, StringTemplate to )
        {
            to._attributeRenderers = from._attributeRenderers;
            to._pattern = from._pattern;
            to._chunks = from._chunks;
            to._formalArguments = from._formalArguments;
            to._numberOfDefaultArgumentValues = from._numberOfDefaultArgumentValues;
            to._name = from._name;
            to._group = from._group;
            to._nativeGroup = from._nativeGroup;
            to._listener = from._listener;
            to._regions = from._regions;
            to._isRegion = from._isRegion;
            to._regionDefType = from._regionDefType;
        }

        /** <summary>
         *  Make an instance of this template; it contains an exact copy of
         *  everything (except the attributes and enclosing instance pointer).
         *  So the new template refers to the previously compiled chunks of this
         *  template but does not have any attribute values.
         *  </summary>
         */
        public virtual StringTemplate GetInstanceOf()
        {
            StringTemplate t = null;
            if ( _nativeGroup != null )
            {
                // create a template using the native group for this template
                // but it's "group" is set to this.group by dup after creation so
                // polymorphism still works.
                t = _nativeGroup.CreateStringTemplate();
            }
            else
            {
                t = _group.CreateStringTemplate();
            }
            Dup( this, t );
            return t;
        }

        public virtual void Reset()
        {
            attributes = new Dictionary<string, object>(); // just throw out table and make new one
        }

        public virtual void SetPredefinedAttributes()
        {
            if ( !LintMode )
            {
                return; // only do this method so far in lint mode
            }
        }

        public virtual void RemoveAttribute( string name )
        {
            if ( attributes != null )
                attributes.Remove( name );
        }

        /** <summary>
         *  Set an attribute for this template.  If you set the same
         *  attribute more than once, you get a multi-valued attribute.
         *  If you send in a StringTemplate object as a value, it's
         *  enclosing instance (where it will inherit values from) is
         *  set to 'this'.  This would be the normal case, though you
         *  can set it back to null after this call if you want.
         *  If you send in a List plus other values to the same
         *  attribute, they all get flattened into one List of values.
         *  This will be a new list object so that incoming objects are
         *  not altered.
         *  </summary>
         *
         *  <remarks>
         *  If you send in an array, it is converted to an ArrayIterator.
         *  </remarks>
         */
        public virtual void SetAttribute( string name, object value )
        {
            if ( value == null || name == null )
            {
                return;
            }
            if ( name.IndexOf( '.' ) >= 0 )
            {
                throw new ArgumentException( "cannot have '.' in attribute names" );
            }
            if ( attributes == null )
            {
                attributes = new Dictionary<string, object>();
            }

            if ( value is StringTemplate )
            {
                ( (StringTemplate)value ).EnclosingInstance = this;
            }
            else if ( value is HashSet<object> )
            {
                value = ( (HashSet<object>)value ).ToArray();
            }
            else
            {
                // convert value if array
                value = ASTExpr.ConvertArrayToList( value );
            }

            // convert plain collections
            // get exactly in this scope (no enclosing)
            object o = this.attributes.get( name );
            if ( o == null )
            { // new attribute
                RawSetAttribute( this.attributes, name, value );
                return;
            }
            // it will be a multi-value attribute
            //System.out.println("exists: "+name+"="+o);
            STAttributeList v = null;
            if ( o.GetType() == typeof( STAttributeList ) )
            { // already a list made by ST
                v = (STAttributeList)o;
            }
            else if ( o is IList )
            { // existing attribute is non-ST List
                // must copy to an ST-managed list before adding new attribute
                IList listAttr = (IList)o;
                v = new STAttributeList( listAttr.Count );
                v.AddRange( listAttr.Cast<object>() );
                RawSetAttribute( this.attributes, name, v ); // replace attribute w/list
            }
            else
            {
                // non-list second attribute, must convert existing to ArrayList
                v = new STAttributeList(); // make list to hold multiple values
                // make it point to list now
                RawSetAttribute( this.attributes, name, v ); // replace attribute w/list
                v.Add( o );  // add previous single-valued attribute
            }
            if ( value is IList )
            {
                // flatten incoming list into existing
                if ( v != value )
                { // avoid weird cyclic add
                    v.AddRange( ( (IList)value ).Cast<object>() );
                }
            }
            else
            {
                v.Add( value );
            }
        }

        /** <summary>
         *  Set an aggregate attribute with two values.  The attribute name
         *  must have the format: "name.{propName1,propName2}".
         *  </summary>
         */
        public virtual void SetAttribute( string aggrSpec, object v1, object v2 )
        {
            SetAttribute( aggrSpec, new object[] { v1, v2 } );
        }

        public virtual void SetAttribute( string aggrSpec, object v1, object v2, object v3 )
        {
            SetAttribute( aggrSpec, new object[] { v1, v2, v3 } );
        }

        public virtual void SetAttribute( string aggrSpec, object v1, object v2, object v3, object v4 )
        {
            SetAttribute( aggrSpec, new object[] { v1, v2, v3, v4 } );
        }

        public virtual void SetAttribute( string aggrSpec, object v1, object v2, object v3, object v4, object v5 )
        {
            SetAttribute( aggrSpec, new object[] { v1, v2, v3, v4, v5 } );
        }

        /** <summary>
         *  Create an aggregate from the list of properties in aggrSpec and fill
         *  with values from values array.  This is not publically visible because
         *  it conflicts semantically with setAttribute("foo",new Object[] {...});
         *  </summary>
         */
        protected virtual void SetAttribute( string aggrSpec, params object[] values )
        {
            if ( values.Length < 2 )
                throw new ArgumentException();

            List<string> properties = new List<string>();
            string aggrName = ParseAggregateAttributeSpec( aggrSpec, properties );
            if ( values == null || properties.Count == 0 )
            {
                throw new ArgumentException( "missing properties or values for '" + aggrSpec + "'" );
            }
            if ( values.Length != properties.Count )
            {
                throw new ArgumentException( "number of properties in '" + aggrSpec + "' != number of values" );
            }
            Aggregate aggr = new Aggregate();
            for ( int i = 0; i < values.Length; i++ )
            {
                object value = values[i];
                if ( value is StringTemplate )
                {
                    ( (StringTemplate)value ).EnclosingInstance = this;
                }
                else
                {
                    value = ASTExpr.ConvertArrayToList( value );
                }
                aggr.Put( properties[i], value );
            }
            SetAttribute( aggrName, aggr );
        }

        /** <summary>
         *  Split "aggrName.{propName1,propName2}" into list [propName1,propName2]
         *  and the aggrName. Space is allowed around ','.
         *  </summary>
         */
        protected virtual string ParseAggregateAttributeSpec( string aggrSpec, List<string> properties )
        {
            int dot = aggrSpec.IndexOf( '.' );
            if ( dot <= 0 )
            {
                throw new ArgumentException( "invalid aggregate attribute format: " +
                        aggrSpec );
            }
            string aggrName = aggrSpec.Substring( 0, dot );
            string propString = aggrSpec.Substring( dot + 1 );
            bool error = true;
            StringTokenizer tokenizer = new StringTokenizer( propString, "{,}", true );
            //match:
            if ( tokenizer.hasMoreTokens() )
            {
                string token = tokenizer.nextToken(); // advance to {
                token = token.Trim();
                if ( token.Equals( "{" ) )
                {
                    token = tokenizer.nextToken();    // advance to first prop name
                    token = token.Trim();
                    properties.Add( token );
                    token = tokenizer.nextToken();    // advance to a comma
                    token = token.Trim();
                    while ( token.Equals( "," ) )
                    {
                        token = tokenizer.nextToken();    // advance to a prop name
                        token = token.Trim();
                        properties.Add( token );
                        token = tokenizer.nextToken();    // advance to a "," or "}"
                        token = token.Trim();
                    }
                    if ( token.Equals( "}" ) )
                    {
                        error = false;
                    }
                }
            }
            if ( error )
            {
                throw new ArgumentException( "invalid aggregate attribute format: " +
                        aggrSpec );
            }
            return aggrName;
        }

        /** <summary>
         *  Map a value to a named attribute.  Throw NoSuchElementException if
         *  the named attribute is not formally defined in self's specific template
         *  and a formal argument list exists.
         *  </summary>
         */
        protected virtual void RawSetAttribute( IDictionary<string, object> attributes,
                                       string name,
                                       object value )
        {
            if ( _formalArguments != FormalArgument.UNKNOWN &&
                GetFormalArgument( name ) == null )
            {
                // a normal call to setAttribute with unknown attribute
                throw new ArgumentException( "no such attribute: " + name +
                                                 " in template context " +
                                                 GetEnclosingInstanceStackString() );
            }
            if ( value == null )
            {
                return;
            }
            attributes[name] = value;
        }

        /** <summary>
         *  Argument evaluation such as foo(x=y), x must
         *  be checked against foo's argument list not this's (which is
         *  the enclosing context).  So far, only eval.g uses arg self as
         *  something other than "this".
         *  </summary>
         */
        public virtual void RawSetArgumentAttribute( StringTemplate embedded,
                                            IDictionary attributes,
                                            string name,
                                            object value )
        {
            if ( embedded._formalArguments != FormalArgument.UNKNOWN &&
                 embedded.GetFormalArgument( name ) == null )
            {
                throw new ArgumentException( "template " + embedded.Name +
                                                 " has no such attribute: " + name +
                                                 " in template context " +
                                                 GetEnclosingInstanceStackString() );
            }
            if ( value == null )
            {
                return;
            }
            attributes[name] = value;
        }

        public virtual object GetAttribute( string name )
        {
            return Get( this, name );
        }

        /** <summary>
         *  Walk the chunks, asking them to write themselves out according
         *  to attribute values of 'this.attributes'.  This is like evaluating or
         *  interpreting the StringTemplate as a program using the
         *  attributes.  The chunks will be identical (point at same list)
         *  for all instances of this template.
         *  </summary>
         */
        public virtual int Write( IStringTemplateWriter writer )
        {
            if ( _group.debugTemplateOutput )
            {
                _group.EmitTemplateStartDebugString( this, writer );
            }
            int n = 0;
            SetPredefinedAttributes();
            SetDefaultArgumentValues();
            int chunkCount = _chunks != null ? _chunks.Count : 0;
            for ( int i = 0; _chunks != null && i < chunkCount; i++ )
            {
                Expr a = _chunks[i];
                int chunkN = a.Write( this, writer );
                // expr-on-first-line-with-no-output NEWLINE => NEWLINE
                if ( chunkN == 0 && i == 0 && ( i + 1 ) < chunkCount &&
                     _chunks[i + 1] is NewlineRef )
                {
                    //System.out.println("found pure first-line-blank \\n pattern");
                    i++; // skip next NEWLINE;
                    continue;
                }
                // NEWLINE expr-with-no-output NEWLINE => NEWLINE
                // Indented $...$ have the indent stored with the ASTExpr
                // so the indent does not come out as a StringRef
                if ( chunkN == 0 &&
                    ( i - 1 ) >= 0 && _chunks[i - 1] is NewlineRef &&
                    ( i + 1 ) < chunkCount && _chunks[i + 1] is NewlineRef )
                {
                    //System.out.println("found pure \\n blank \\n pattern");
                    i++; // make it skip over the next chunk, the NEWLINE
                }
                n += chunkN;
            }
            if ( _group.debugTemplateOutput )
            {
                _group.EmitTemplateStopDebugString( this, writer );
            }
            if ( LintMode )
            {
                CheckForTrouble();
            }
            return n;
        }

        /** <summary>Resolve an attribute reference.</summary>
         *
         *  <remarks>
         *  It can be in four possible places:
         *
         *  1. the attribute list for the current template
         *  2. if self is an embedded template, somebody invoked us possibly
         *     with arguments--check the argument context
         *  3. if self is an embedded template, the attribute list for the enclosing
         *     instance (recursively up the enclosing instance chain)
         *  4. if nothing is found in the enclosing instance chain, then it might
         *     be a map defined in the group or the its supergroup etc...
         *
         *  Attribute references are checked for validity.  If an attribute has
         *  a value, its validity was checked before template rendering.
         *  If the attribute has no value, then we must check to ensure it is a
         *  valid reference.  Somebody could reference any random value like $xyz$;
         *  formal arg checks before rendering cannot detect this--only the ref
         *  can initiate a validity check.  So, if no value, walk up the enclosed
         *  template tree again, this time checking formal parameters not
         *  attributes Map.  The formal definition must exist even if no value.
         *
         *  To avoid infinite recursion in toString(), we have another condition
         *  to check regarding attribute values.  If your template has a formal
         *  argument, foo, then foo will hide any value available from "above"
         *  in order to prevent infinite recursion.
         *
         *  This method is not static so people can override functionality.
         *  </remarks>
         */
        public virtual object Get( StringTemplate self, string attribute )
        {
            //System.out.println("### get("+self.getEnclosingInstanceStackString()+", "+attribute+")");
            //System.out.println("attributes="+(self.attributes!=null?self.attributes.keySet().toString():"none"));
            if ( self == null )
            {
                return null;
            }

            if ( LintMode )
            {
                self.TrackAttributeReference( attribute );
            }

            // is it here?
            object o = null;
            if ( self.attributes != null )
            {
                self.attributes.TryGetValue( attribute, out o );
            }

            // nope, check argument context in case embedded
            if ( o == null )
            {
                IDictionary<string, object> argContext = self.ArgumentContext;
                if ( argContext != null )
                {
                    argContext.TryGetValue( attribute, out o );
                }
            }

            if ( o == null &&
                 !self._passThroughAttributes &&
                 self.GetFormalArgument( attribute ) != null )
            {
                // if you've defined attribute as formal arg for this
                // template and it has no value, do not look up the
                // enclosing dynamic scopes.  This avoids potential infinite
                // recursion.
                return null;
            }

            // not locally defined, check enclosingInstance if embedded
            if ( o == null && self._enclosingInstance != null )
            {
                //System.Console.Out.WriteLine( "looking for " + Name + "." + attribute + " in super=" + _enclosingInstance.Name );
                object valueFromEnclosing = Get( self._enclosingInstance, attribute );
                if ( valueFromEnclosing == null )
                {
                    CheckNullAttributeAgainstFormalArguments( self, attribute );
                }
                o = valueFromEnclosing;
            }

            // not found and no enclosing instance to look at
            else if ( o == null && self._enclosingInstance == null )
            {
                // It might be a map in the group or supergroup...
                o = self._group.GetMap( attribute );
            }

            return o;
        }

        /** <summary>
         *  Walk a template, breaking it into a list of
         *  chunks: Strings and actions/expressions.
         *  </summary>
         */
        protected virtual void BreakTemplateIntoChunks()
        {
            //System.out.println("parsing template: "+pattern);
            if ( _pattern == null )
            {
                return;
            }
            try
            {
                // instead of creating a specific template lexer, use
                // an instance of the class specified by the user.
                // The default is DefaultTemplateLexer.
                // The only constraint is that you use an ANTLR lexer
                // so I can use the special ChunkToken.
                Lexer chunkStream = _group.CreateLexer( this, new StringReader( _pattern ) );
                TemplateParser chunkifier = new TemplateParser( new CommonTokenStream( chunkStream ) );
                chunkifier.template( this );
                //System.out.println("chunks="+chunks);
            }
            catch ( Exception e )
            {
                string name = "<unknown>";
                string outerName = OutermostName;
                if ( Name != null )
                {
                    name = Name;
                }
                if ( outerName != null && !name.Equals( outerName ) )
                {
                    name = name + " nested in " + outerName;
                }
                Error( "problem parsing template '" + name + "'", e );
            }
        }

        public virtual ASTExpr ParseAction( string action )
        {
            //Console.Out.WriteLine( "parse action " + action );
            ActionLexer lexer = new ActionLexer( new Antlr.Runtime.ANTLRStringStream( action ) );
            ActionParser parser = new ActionParser( lexer, this );
            ASTExpr a = null;
            try
            {
                var result = parser.action();
                IDictionary<string, object> options = result.opts;
                ITree tree = (ITree)result.Tree;
                if ( tree != null )
                {
                    if ( tree.Type == ActionParser.CONDITIONAL )
                    {
                        a = new ConditionalExpr( this, tree );
                    }
                    else
                    {
                        a = new ASTExpr( this, tree, options );
                    }
                }
            }
            catch ( RecognitionException re )
            {
                Error( "Can't parse chunk: " + action, re );
            }

            return a;
        }

        public virtual void AddChunk( Expr e )
        {
            if ( _chunks == null )
            {
                _chunks = new List<Expr>();
            }
            _chunks.Add( e );
        }

        #region Formal Arg Stuff

        /** <summary>
         *  Set any default argument values that were not set by the
         *  invoking template or by setAttribute directly.  Note
         *  that the default values may be templates.  Their evaluation
         *  context is the template itself and, hence, can see attributes
         *  within the template, any arguments, and any values inherited
         *  by the template.
         *  </summary>
         *
         *  <remarks>
         *  Default values are stored in the argument context rather than
         *  the template attributes table just for consistency's sake.
         *  </remarks>
         */
        public virtual void SetDefaultArgumentValues()
        {
            if ( _numberOfDefaultArgumentValues == 0 )
            {
                return;
            }
            if ( _argumentContext == null )
            {
                _argumentContext = new Dictionary<string, object>();
            }
            if ( _formalArguments != FormalArgument.UNKNOWN )
            {
                foreach ( var arg in _formalArguments )
                {
                    string argName = arg.name;
                    // use the default value then
                    if ( arg.defaultValueST != null )
                    {
                        object existingValue = GetAttribute( argName );
                        if ( existingValue == null )
                        { // value unset?
                            // if no value for attribute, set arg context
                            // to the default value.  We don't need an instance
                            // here because no attributes can be set in
                            // the arg templates by the user.
                            _argumentContext[argName] = arg.defaultValueST;
                        }
                    }
                }
            }
        }

        /** <summary>
         *  From this template upward in the enclosing template tree,
         *  recursively look for the formal parameter.
         *  </summary>
         */
        public virtual FormalArgument LookupFormalArgument( string name )
        {
            FormalArgument arg = GetFormalArgument( name );
            if ( arg == null && _enclosingInstance != null )
            {
                arg = _enclosingInstance.LookupFormalArgument( name );
            }
            return arg;
        }

        public virtual FormalArgument GetFormalArgument( string name )
        {
            foreach ( var arg in FormalArguments )
            {
                if ( arg.name == name )
                    return arg;
            }

            return null;
        }

        public virtual void DefineEmptyFormalArgumentList()
        {
            FormalArguments = new List<FormalArgument>();
        }

        public virtual void DefineFormalArgument( string name )
        {
            DefineFormalArgument( name, null );
        }

        public virtual void DefineFormalArguments( IList names )
        {
            if ( names == null )
            {
                return;
            }
            for ( int i = 0; i < names.Count; i++ )
            {
                string name = (string)names[i];
                DefineFormalArgument( name );
            }
        }

        public virtual void DefineFormalArgument( string name, StringTemplate defaultValue )
        {
            if ( defaultValue != null )
            {
                _numberOfDefaultArgumentValues++;
            }
            FormalArgument a = new FormalArgument( name, defaultValue );
            if ( _formalArguments == FormalArgument.UNKNOWN )
            {
                _formalArguments = new List<FormalArgument>();
            }
            _formalArguments.Add( a );
        }

        #endregion

        /** <summary>
         *  Normally if you call template y from x, y cannot see any attributes
         *  of x that are defined as formal parameters of y.  Setting this
         *  passThroughAttributes to true, will override that and allow a
         *  template to see through the formal arg list to inherited values.
         *  </summary>
         */
        public virtual void SetPassThroughAttributes( bool passThroughAttributes )
        {
            this._passThroughAttributes = passThroughAttributes;
        }

        /** <summary>
         *  Specify a complete map of what object classes should map to which
         *  renderer objects.
         *  </summary>
         */
        public virtual void SetAttributeRenderers( Dictionary<Type, IAttributeRenderer> renderers )
        {
            this._attributeRenderers = renderers;
        }

        /** <summary>
         *  Register a renderer for all objects of a particular type.  This
         *  overrides any renderer set in the group for this class type.
         *  </summary>
         */
        public virtual void RegisterRenderer( Type attributeClassType, IAttributeRenderer renderer )
        {
            if ( _attributeRenderers == null )
            {
                _attributeRenderers = new Dictionary<Type, IAttributeRenderer>();
            }
            _attributeRenderers[attributeClassType] = renderer;
        }

        /** <summary>
         *  What renderer is registered for this attributeClassType for
         *  this template.  If not found, the template's group is queried.
         *  </summary>
         */
        public virtual IAttributeRenderer GetAttributeRenderer( Type attributeClassType )
        {
            IAttributeRenderer renderer = null;
            if ( _attributeRenderers != null )
            {
                if ( !_attributeRenderers.TryGetValue( attributeClassType, out renderer ) )
                    renderer = null;
            }
            if ( renderer != null )
            {
                // found it!
                return renderer;
            }

            // we have no renderer overrides for the template or none for class arg
            // check parent template if we are embedded
            if ( _enclosingInstance != null )
            {
                return _enclosingInstance.GetAttributeRenderer( attributeClassType );
            }
            // else check group
            return _group.GetAttributeRenderer( attributeClassType );
        }

        #region Utility routines

        public virtual void Error( string msg )
        {
            Error( msg, null );
        }

        public virtual void Warning( string msg )
        {
            if ( ErrorListener != null )
            {
                ErrorListener.Warning( msg );
            }
            else
            {
                Console.Error.WriteLine( "StringTemplate: warning: " + msg );
            }
        }

        public virtual void Error( string msg, Exception e )
        {
            if ( ErrorListener != null )
            {
                ErrorListener.Error( msg, e );
            }
            else
            {
                if ( e != null )
                {
                    Console.Error.WriteLine( "StringTemplate: error: " + msg + ": " + e.ToString() );
                    if ( e is TargetInvocationException )
                    {
                        e = e.InnerException ?? e;
                    }
                    e.PrintStackTrace( Console.Error );
                }
                else
                {
                    Console.Error.WriteLine( "StringTemplate: error: " + msg );
                }
            }
        }

        /** <summary>Indicates that 'name' has been referenced in this template.</summary> */
        protected virtual void TrackAttributeReference( string name )
        {
            if ( _referencedAttributes == null )
            {
                _referencedAttributes = new List<string>();
            }
            _referencedAttributes.Add( name );
        }

        /** <summary>
         *  Look up the enclosing instance chain (and include this) to see
         *  if st is a template already in the enclosing instance chain.
         *  </summary>
         */
        public static bool IsRecursiveEnclosingInstance( StringTemplate st )
        {
            if ( st == null )
            {
                return false;
            }
            StringTemplate p = st._enclosingInstance;
            if ( p == st )
            {
                return true; // self-recursive
            }
            // now look for indirect recursion
            while ( p != null )
            {
                if ( p == st )
                {
                    return true;
                }
                p = p._enclosingInstance;
            }
            return false;
        }

        string _newline = Environment.NewLine;

        public virtual string GetEnclosingInstanceStackTrace()
        {
            StringBuilder buf = new StringBuilder();
            HashSet<object> seen = new HashSet<object>();
            StringTemplate p = this;
            while ( p != null )
            {
                if ( seen.Contains( p ) )
                {
                    buf.Append( p.GetTemplateDeclaratorString() );
                    buf.Append( " (start of recursive cycle)" );
                    buf.Append( _newline );
                    buf.Append( "..." );
                    break;
                }
                seen.Add( p );
                buf.Append( p.GetTemplateDeclaratorString() );
                if ( p.attributes != null )
                {
                    buf.Append( ", attributes=[" );
                    int i = 0;
                    foreach ( string attrName in p.attributes.Keys )
                    {
                        if ( i > 0 )
                        {
                            buf.Append( ", " );
                        }
                        i++;
                        buf.Append( attrName );
                        object o = p.attributes.get( attrName );
                        if ( o is StringTemplate )
                        {
                            StringTemplate st = (StringTemplate)o;
                            buf.Append( "=" );
                            buf.Append( "<" );
                            buf.Append( st.Name );
                            buf.Append( "()@" );
                            buf.Append( st.TemplateID.ToString() );
                            buf.Append( ">" );
                        }
                        else if ( o is IList )
                        {
                            buf.Append( "=List[.." );
                            IList list = (IList)o;
                            int n = 0;
                            for ( int j = 0; j < list.Count; j++ )
                            {
                                object listValue = list[j];
                                if ( listValue is StringTemplate )
                                {
                                    if ( n > 0 )
                                    {
                                        buf.Append( ", " );
                                    }
                                    n++;
                                    StringTemplate st = (StringTemplate)listValue;
                                    buf.Append( "<" );
                                    buf.Append( st.Name );
                                    buf.Append( "()@" );
                                    buf.Append( st.TemplateID.ToString() );
                                    buf.Append( ">" );
                                }
                            }
                            buf.Append( "..]" );
                        }
                    }
                    buf.Append( "]" );
                }
                if ( p._referencedAttributes != null )
                {
                    buf.Append( ", references=" );
                    buf.Append( "[" + string.Join( ", ", p._referencedAttributes.ToArray() ) + "]" );
                }
                buf.Append( ">" + _newline );
                p = p._enclosingInstance;
            }
            /*
                    if ( enclosingInstance!=null ) {
                    buf.append(enclosingInstance.getEnclosingInstanceStackTrace());
                    }
                    */
            return buf.ToString();
        }

        public virtual string GetTemplateDeclaratorString()
        {
            StringBuilder buf = new StringBuilder();
            buf.Append( "<" );
            buf.Append( Name );
            buf.Append( "(" );
            buf.Append( _formalArguments.Select( fa => fa.name ).ToList().ToElementString() );
            buf.Append( ")@" );
            buf.Append( TemplateID.ToString() );
            buf.Append( ">" );
            return buf.ToString();
        }

        protected virtual string GetTemplateHeaderString( bool showAttributes )
        {
            if ( showAttributes )
            {
                StringBuilder buf = new StringBuilder();
                buf.Append( Name );
                if ( attributes != null )
                {
                    buf.Append( "[" + string.Join( ", ", attributes.Keys.Cast<object>().Select( o => o.ToString() ).ToArray() ) + "]" );
                }
                return buf.ToString();
            }
            return Name;
        }

#if false
        /** <summary>
         *  Find "missing attribute" and "cardinality mismatch" errors.
         *  Excecuted before a template writes its chunks out.
         *  When you find a problem, throw an IllegalArgumentException.
         *  We must check the attributes as well as the incoming arguments
         *  in argumentContext.
         *  </summary>
         */
        protected void CheckAttributesAgainstFormalArguments()
        {
            var args = FormalArguments;
            /*
            if ( (attributes==null||attributes.size()==0) &&
                 (argumentContext==null||argumentContext.size()==0) &&
                 formalArguments.size()!=0 )
            {
                throw new IllegalArgumentException("missing argument(s): "+args+" in template "+getName());
            }
            Iterator iter = args.iterator();
            while ( iter.hasNext() )
            {
                String argName = (String)iter.next();
                FormalArgument arg = getFormalArgument(argName);
                int expectedCardinality = arg.getCardinality();
                Object value = getAttribute(argName);
                int actualCardinality = getActualArgumentCardinality(value);
                // if intersection of expected and actual is empty, mismatch
                if ( (expectedCardinality&actualCardinality)==0 )
                {
                    throw new IllegalArgumentException("cardinality mismatch: "+
                            argName+"; expected "+
                            FormalArgument.getCardinalityName(expectedCardinality)+
                            " found cardinality="+getObjectLength(value));
                }
            }
        }
#endif

        /** <summary>
         *  A reference to an attribute with no value, must be compared against
         *  the formal parameter to see if it exists; if it exists all is well,
         *  but if not, throw an exception.
         *  </summary>
         *
         *  <remarks>
         *  Don't do the check if no formal parameters exist for this template;
         *  ask enclosing.
         *  </remarks>
         */
        protected virtual void CheckNullAttributeAgainstFormalArguments(
                StringTemplate self,
                string attribute )
        {
            if ( self.FormalArguments == FormalArgument.UNKNOWN )
            {
                // bypass unknown arg lists
                if ( self._enclosingInstance != null )
                {
                    CheckNullAttributeAgainstFormalArguments(
                            self._enclosingInstance,
                            attribute );
                }
                return;
            }
            FormalArgument formalArg = self.LookupFormalArgument( attribute );
            if ( formalArg == null )
            {
                throw new ArgumentException( "no such attribute: " + attribute +
                                                 " in template context " + GetEnclosingInstanceStackString() );
            }
        }

        /** <summary>
         *  Executed after evaluating a template.  For now, checks for setting
         *  of attributes not reference.
         *  </summary>
         */
        protected virtual void CheckForTrouble()
        {
            // we have table of set values and list of values referenced
            // compare, looking for SET BUT NOT REFERENCED ATTRIBUTES
            if ( attributes == null )
            {
                return;
            }
            // if in names and not in referenced attributes, trouble
            foreach ( string name in attributes.Keys )
            {
                if ( _referencedAttributes != null &&
                    !_referencedAttributes.Contains( name ) )
                {
                    Warning( Name + ": set but not used: " + name );
                }
            }
            // can do the reverse, but will have lots of false warnings :(
        }

        /** <summary>
         *  If an instance of x is enclosed in a y which is in a z, return
         *  a String of these instance names in order from topmost to lowest;
         *  here that would be "[z y x]".
         *  </summary>
         */
        public virtual string GetEnclosingInstanceStackString()
        {
            System.Collections.Generic.List<string> names = new System.Collections.Generic.List<string>();
            StringTemplate p = this;
            while ( p != null )
            {
                string name = p.Name;
                names.Insert( 0, name + ( p._passThroughAttributes ? "(...)" : "" ) );
                p = p._enclosingInstance;
            }
            return "[" + string.Join( " ", names.ToArray() ) + "]";
        }

        public virtual void AddRegionName( string name )
        {
            if ( _regions == null )
            {
                _regions = new HashSet<object>();
            }
            _regions.Add( name );
        }

        /** <summary>Does this template ref or embed region name?</summary> */
        public virtual bool ContainsRegionName( string name )
        {
            if ( _regions == null )
            {
                return false;
            }
            return _regions.Contains( name );
        }

        public virtual string ToDebugString()
        {
            StringBuilder buf = new StringBuilder();
            buf.Append( "template-" + GetTemplateDeclaratorString() + ":" );
            buf.Append( "chunks=" );
            if ( _chunks != null )
            {
                buf.Append( _chunks.ToString() );
            }
            buf.Append( "attributes=[" );
            if ( attributes != null )
            {
                int n = 0;
                foreach ( string name in attributes.Keys )
                {
                    if ( n > 0 )
                    {
                        buf.Append( ',' );
                    }
                    buf.Append( name + "=" );
                    object value = attributes.get( name );
                    if ( value is StringTemplate )
                    {
                        buf.Append( ( (StringTemplate)value ).ToDebugString() );
                    }
                    else
                    {
                        buf.Append( value );
                    }
                    n++;
                }
                buf.Append( "]" );
            }
            return buf.ToString();
        }

        /** <summary>
         *  Don't print values, just report the nested structure with attribute names.
         *  Follow (nest) attributes that are templates only.
         *  </summary>
         */
        public virtual string ToStructureString()
        {
            return ToStructureString( 0 );
        }

        public virtual string ToStructureString( int indent )
        {
            StringBuilder buf = new StringBuilder();
            for ( int i = 1; i <= indent; i++ )
            { // indent
                buf.Append( "  " );
            }
            buf.Append( Name );
            buf.Append( attributes.Keys );
            buf.Append( ":" + _newline );
            if ( attributes != null )
            {
                foreach ( var attr in attributes )
                {
                    string name = attr.Key;
                    object value = attr.Value;
                    if ( value is StringTemplate )
                    { // descend
                        buf.Append( ( (StringTemplate)value ).ToStructureString( indent + 1 ) );
                    }
                    else
                    {
                        if ( value is IList )
                        {
                            IList alist = (IList)value;
                            for ( int i = 0; i < alist.Count; i++ )
                            {
                                object o = (object)alist[i];
                                if ( o is StringTemplate )
                                { // descend
                                    buf.Append( ( (StringTemplate)o ).ToStructureString( indent + 1 ) );
                                }
                            }
                        }
                        else if ( value is IDictionary )
                        {
                            IDictionary m = (IDictionary)value;
                            ICollection mvalues = m.Values;
                            foreach ( object o in mvalues )
                            {
                                if ( o is StringTemplate )
                                { // descend
                                    buf.Append( ( (StringTemplate)o ).ToStructureString( indent + 1 ) );
                                }
                            }
                        }
                    }
                }
            }
            return buf.ToString();
        }

#if false
        public String GetDOTForDependencyGraph(bool showAttributes) {
            StringBuffer buf = new StringBuffer();
            buf.append("digraph prof {\n");
            HashMap edges = new HashMap();
            this.getDependencyGraph(edges, showAttributes);
            Set sourceNodes = edges.keySet();
            // for each source template
            for (Iterator it = sourceNodes.iterator(); it.hasNext();) {
                String src = (String) it.next();
                Set targetNodes = (Set)edges.get(src);
                // for each target template
                for (Iterator it2 = targetNodes.iterator(); it2.hasNext();) {
                    String trg = (String) it2.next();
                    buf.append('"');
                    buf.append(src);
                    buf.append('"');
                    buf.append("->");
                    buf.append('"');
                    buf.append(trg);
                    buf.append("\"\n");
                }
            }
            buf.append("}");
            return buf.toString();
        }
#endif

        /** <summary>
         *  Generate a DOT file for displaying the template enclosure graph.
         *  </summary>
         *  
         *  <remarks>
         *  For example:
         * 
         *      digraph prof {
         *          "t1" -> "t2"
         *          "t1" -> "t3"
         *          "t4" -> "t5"
         *      }
         *  </remarks>
         */
        public virtual StringTemplate GetDOTForDependencyGraph( bool showAttributes )
        {
            string structure =
                "digraph StringTemplateDependencyGraph {" + _newline +
                "node [shape=$shape$, $if(width)$width=$width$,$endif$" +
                "      $if(height)$height=$height$,$endif$ fontsize=$fontsize$];" + _newline +
                "$edges:{e|\"$e.src$\" -> \"$e.trg$\"" + _newline + "}$" +
                "}" + _newline;
            StringTemplate graphST = new StringTemplate( structure );
            Dictionary<object, object> edges = new Dictionary<object, object>();
            this.GetDependencyGraph( edges, showAttributes );
            var sourceNodes = edges.Keys;
            // for each source template
            foreach ( string src in sourceNodes )
            {
                HashSet<object> targetNodes = (HashSet<object>)edges.get( src );
                // for each target template
                foreach ( string trg in targetNodes )
                {
                    graphST.SetAttribute( "edges.{src,trg}", src, trg );
                }
            }
            graphST.SetAttribute( "shape", "none" );
            graphST.SetAttribute( "fontsize", "11" );
            graphST.SetAttribute( "height", "0" ); // make height
            return graphST;
        }

        /** <summary>
         *  Get a list of n->m edges where template n contains template m.
         *  The map you pass in is filled with edges: key->value.  Useful
         *  for having DOT print out an enclosing template graph.  It
         *  finds all direct template invocations too like &lt;foo()> but not
         *  indirect ones like &lt;(name)()>.
         *  </summary>
         *
         *  <remarks>
         *  Ack, I just realized that this is done statically and hence
         *  cannot see runtime arg values on statically included templates.
         *  Hmm...someday figure out to do this dynamically as if we were
         *  evaluating the templates.  There will be extra nodes in the tree
         *  because we are static like method and method[...] with args.
         *  </remarks>
         */
        public virtual void GetDependencyGraph( IDictionary edges, bool showAttributes )
        {
            string srcNode = this.GetTemplateHeaderString( showAttributes );
            if ( attributes != null )
            {
                foreach ( var attr in attributes )
                {
                    string name = attr.Key;
                    object value = attr.Value;
                    if ( value is StringTemplate )
                    {
                        string targetNode =
                            ( (StringTemplate)value ).GetTemplateHeaderString( showAttributes );
                        PutToMultiValuedMap( edges, srcNode, targetNode );
                        ( (StringTemplate)value ).GetDependencyGraph( edges, showAttributes ); // descend
                    }
                    else
                    {
                        if ( value is IList )
                        {
                            IList alist = (IList)value;
                            for ( int i = 0; i < alist.Count; i++ )
                            {
                                object o = (object)alist[i];
                                if ( o is StringTemplate )
                                {
                                    string targetNode =
                                        ( (StringTemplate)o ).GetTemplateHeaderString( showAttributes );
                                    PutToMultiValuedMap( edges, srcNode, targetNode );
                                    ( (StringTemplate)o ).GetDependencyGraph( edges, showAttributes ); // descend
                                }
                            }
                        }
                        else if ( value is IDictionary )
                        {
                            IDictionary m = (IDictionary)value;
                            ICollection mvalues = m.Values;
                            foreach ( object o in mvalues )
                            {
                                if ( o is StringTemplate )
                                {
                                    string targetNode =
                                        ( (StringTemplate)o ).GetTemplateHeaderString( showAttributes );
                                    PutToMultiValuedMap( edges, srcNode, targetNode );
                                    ( (StringTemplate)o ).GetDependencyGraph( edges, showAttributes ); // descend
                                }
                            }
                        }
                    }
                }
            }
            // look in chunks too for template refs
            for ( int i = 0; _chunks != null && i < _chunks.Count; i++ )
            {
                Expr expr = _chunks[i];
                if ( expr is ASTExpr )
                {
                    ASTExpr e = (ASTExpr)expr;
                    ITree tree = e.AST;
                    ITree includeAST =
                        new CommonTree( new CommonToken( ActionEvaluator.INCLUDE, "include" ) );
                    var it = tree.findAllPartial( includeAST );
                    foreach ( ITree t in it )
                    {
                        string templateInclude = t.GetChild( 0 ).Text;
                        Console.Out.WriteLine( "found include " + templateInclude );
                        PutToMultiValuedMap( edges, srcNode, templateInclude );
                        StringTemplateGroup group = Group;
                        if ( group != null )
                        {
                            StringTemplate st = group.GetInstanceOf( templateInclude );
                            // descend into the reference template
                            st.GetDependencyGraph( edges, showAttributes );
                        }
                    }
                }
            }
        }

        /** <summary>Manage a hash table like it has multiple unique values.  Map&lt;Object,Set>.</summary> */
        protected virtual void PutToMultiValuedMap( IDictionary map, object key, object value )
        {
            HashSet<object> bag = (HashSet<object>)map[key];
            if ( bag == null )
            {
                bag = new HashSet<object>();
                map[key] = bag;
            }
            bag.Add( value );
        }

        public virtual void PrintDebugString()
        {
            Console.Out.WriteLine( "template-" + Name + ":" );
            Console.Out.Write( "chunks=" );
            Console.Out.WriteLine( _chunks.ToString() );
            if ( attributes == null )
            {
                return;
            }
            Console.Out.Write( "attributes=[" );
            int n = 0;
            foreach ( var attr in attributes )
            {
                if ( n > 0 )
                {
                    Console.Out.Write( ',' );
                }
                string name = attr.Key;
                object value = attr.Value;
                if ( value is StringTemplate )
                {
                    Console.Out.Write( name + "=" );
                    ( (StringTemplate)value ).PrintDebugString();
                }
                else
                {
                    if ( value is IList )
                    {
                        List<object> alist = (List<object>)value;
                        for ( int i = 0; i < alist.Count; i++ )
                        {
                            object o = (object)alist[i];
                            Console.Out.Write( name + "[" + i + "] is " + o.GetType().Name + "=" );
                            if ( o is StringTemplate )
                            {
                                ( (StringTemplate)o ).PrintDebugString();
                            }
                            else
                            {
                                Console.Out.WriteLine( o );
                            }
                        }
                    }
                    else
                    {
                        Console.Out.Write( name + "=" );
                        Console.Out.WriteLine( value );
                    }
                }
                n++;
            }
            Console.Out.Write( "]" + _newline );
        }

        public override string ToString()
        {
            return ToString( StringTemplateWriterConstants.NO_WRAP );
        }

        public virtual string ToString( int lineWidth )
        {
            StringWriter @out = new StringWriter();
            // Write the output to a StringWriter
            IStringTemplateWriter wr = _group.GetStringTemplateWriter( @out );
            wr.SetLineWidth( lineWidth );
            try
            {
                Write( wr );
            }
            catch ( IOException /*io*/)
            {
                Error( "Got IOException writing to writer " + wr.GetType().Name );
            }
            // reset so next toString() does not wrap; normally this is a new writer
            // each time, but just in case they override the group to reuse the
            // writer.
            wr.SetLineWidth( StringTemplateWriterConstants.NO_WRAP );
            return @out.ToString();
        }

        #endregion

    }
}
