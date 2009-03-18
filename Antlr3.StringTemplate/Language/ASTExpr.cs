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

namespace Antlr3.ST.Language
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Antlr.Runtime.JavaExtensions;

    using FieldInfo = System.Reflection.FieldInfo;
    using ICollection = System.Collections.ICollection;
    using IDictionary = System.Collections.IDictionary;
    using IList = System.Collections.IList;
    using InvalidOperationException = System.InvalidOperationException;
    using IOException = System.IO.IOException;
    using ITree = Antlr.Runtime.Tree.ITree;
    using MemberInfo = System.Reflection.MemberInfo;
    using MethodImpl = System.Runtime.CompilerServices.MethodImplAttribute;
    using MethodImplOptions = System.Runtime.CompilerServices.MethodImplOptions;
    using MethodInfo = System.Reflection.MethodInfo;
    using RecognitionException = Antlr.Runtime.RecognitionException;
    using StringWriter = System.IO.StringWriter;

    /** <summary>
     *  A single string template expression enclosed in $...; separator=...$
     *  parsed into an AST chunk to be evaluated.
     *  </summary>
     */
    public class ASTExpr : Expr
    {
        public const string DefaultAttributeName = "it";
        public const string DefaultAttributeNameDeprecated = "attr";
        public const string DefaultIndexVariableName = "i";
        public const string DefaultIndex0VariableName = "i0";
        public const string DefaultMapValueName = "_default_";
        public const string DefaultMapKeyName = "key";

        /** <summary>Used to indicate "default:key" in maps within groups</summary> */
        public static readonly StringTemplate MapKeyValue = new StringTemplate();

        /** <summary>
         *  Using an expr option w/o value, makes options table hold EMPTY_OPTION
         *  value for that key.
         *  </summary>
         */
        public const string EmptyOption = "empty expr option";

        static readonly IDictionary<string, StringTemplateAST> _defaultOptionValues =
            new Dictionary<string, StringTemplateAST>()
            {
                { "anchor", new StringTemplateAST( ActionEvaluator.STRING, "true" ) },
                { "wrap", new StringTemplateAST( ActionEvaluator.STRING, Environment.NewLine ) },
            };

        /** <summary>John Snyders gave me an example implementation for this checking</summary> */
        static readonly HashSet<string> _supportedOptions =
            new HashSet<string>()
            {
                "anchor",
                "format",
                "null",
                "separator",
                "wrap"
            };

        static readonly Dictionary<Type, Dictionary<string, Func<object, object>>> _memberAccessors = new Dictionary<Type, Dictionary<string, Func<object, object>>>();

        ITree _exprTree;

        /** <summary>store separator etc...</summary> */
        IDictionary<string, object> _options;

        /** <summary>
         *  A cached value of wrap=expr from the &lt;...> expression.
         *  Computed in write(StringTemplate, StringTemplateWriter) and used
         *  in writeAttribute.
         *  </summary>
         */
        string _wrapString;

        /** <summary>
         *  For null values in iterated attributes and single attributes that
         *  are null, use this value instead of skipping.  For single valued
         *  attributes like &lt;name; null="n/a"> it's a shorthand for
         *  &lt;if(name)>&lt;name>&lt;else>n/a&lt;endif>
         *  For iterated values &lt;values; null="0", separator=",">, you get 0 for
         *  for null list values.  Works for template application like:
         *  &lt;values:{v| &lt;v>}; null="0"> also.
         *  </summary>
         */
        string _nullValue;

        /** <summary>
         *  A cached value of separator=expr from the &lt;...> expression.
         *  Computed in write(StringTemplate, StringTemplateWriter) and used
         *  in writeAttribute.
         *  </summary>
         */
        string _separatorString;

        /** <summary>A cached value of option format=expr</summary> */
        string _formatString;

        public ASTExpr( StringTemplate enclosingTemplate, ITree exprTree, IDictionary<string, object> options ) :
            base( enclosingTemplate )
        {
            this._exprTree = exprTree;
            this._options = options;
        }

        #region Properties
        /** <summary>Return the tree interpreted when this template is written out.</summary> */
        public ITree AST
        {
            get
            {
                return _exprTree;
            }
        }
        #endregion

        /** <summary>
         *  To write out the value of an ASTExpr, invoke the evaluator in eval.g
         *  to walk the tree writing out the values.  For efficiency, don't
         *  compute a bunch of strings and then pack them together.  Write out directly.
         *  </summary>
         *
         *  <remarks>
         *  Compute separator and wrap expressions, save as strings so we don't
         *  recompute for each value in a multi-valued attribute or expression.
         *
         *  If they set anchor option, then inform the writer to push current
         *  char position.
         *  </remarks>
         */
        public override int Write( StringTemplate self, IStringTemplateWriter @out )
        {
            if ( _exprTree == null || self == null || @out == null )
            {
                return 0;
            }
            // handle options, anchor, wrap, separator...
            StringTemplateAST anchorAST = (StringTemplateAST)GetOption( "anchor" );
            if ( anchorAST != null )
            { // any non-empty expr means true; check presence
                @out.PushAnchorPoint();
            }
            @out.PushIndentation( Indentation );
            HandleExprOptions( self );
            //System.out.println("evaluating tree: "+exprTree.toStringList());
            ActionEvaluator eval =
                    new ActionEvaluator( self, this, @out, _exprTree );
            int n = 0;
            try
            {
                n = eval.action();
                //n = eval.action( exprTree ); // eval and write out tree
            }
            catch ( RecognitionException re )
            {
                self.Error( "can't evaluate tree: " + _exprTree.ToStringTree(), re );
            }
            @out.PopIndentation();
            if ( anchorAST != null )
            {
                @out.PopAnchorPoint();
            }
            return n;
        }

        /** <summary>Grab and cache options; verify options are valid</summary> */
        protected virtual void HandleExprOptions( StringTemplate self )
        {
            // make sure options don't use format / renderer.  They are usually
            // strings which might invoke a string renderer etc...
            _formatString = null;
            StringTemplateAST wrapAST = (StringTemplateAST)GetOption( "wrap" );
            if ( wrapAST != null )
            {
                _wrapString = EvaluateExpression( self, wrapAST );
            }
            StringTemplateAST nullValueAST = (StringTemplateAST)GetOption( "null" );
            if ( nullValueAST != null )
            {
                _nullValue = EvaluateExpression( self, nullValueAST );
            }
            StringTemplateAST separatorAST = (StringTemplateAST)GetOption( "separator" );
            if ( separatorAST != null )
            {
                _separatorString = EvaluateExpression( self, separatorAST );
            }
            // following addition inspired by John Snyders
            StringTemplateAST formatAST =
                (StringTemplateAST)GetOption( "format" );
            if ( formatAST != null )
            {
                _formatString = EvaluateExpression( self, formatAST );
            }

            // Check that option is valid
            if ( _options != null )
            {
                foreach ( string option in _options.Keys )
                {
                    if ( !_supportedOptions.Contains( option ) )
                        self.Warning( "ignoring unsupported option: " + option );
                }
            }
        }

        #region Help routines called by the tree walker

        /** <summary>
         *  For &lt;names,phones:{n,p | ...}> treat the names, phones as lists
         *  to be walked in lock step as n=names[i], p=phones[i].
         *  </summary>
         */
        public virtual object ApplyTemplateToListOfAttributes( StringTemplate self,
                                                      IList attributes,
                                                      StringTemplate templateToApply )
        {
            if ( attributes == null || templateToApply == null || attributes.Count == 0 )
            {
                return null; // do not apply if missing templates or empty values
            }
            Dictionary<string, object> argumentContext = null;
            // indicate it's an ST-created list
            IList results = new StringTemplate.STAttributeList();

            // convert all attributes to iterators even if just one value
            for ( int a = 0; a < attributes.Count; a++ )
            {
                object o = attributes[a];
                if ( o != null )
                {
                    o = ConvertAnythingToIterator( o );
                    attributes[a] = o; // alter the list in place
                }
            }

            int numAttributes = attributes.Count;

            // ensure arguments line up
            var formalArguments = templateToApply.FormalArguments;
            if ( formalArguments == null || formalArguments.Count == 0 )
            {
                self.Error( "missing arguments in anonymous" +
                           " template in context " + self.GetEnclosingInstanceStackString() );
                return null;
            }
            string[] formalArgumentNames = formalArguments.Select( fa => fa.name ).ToArray();
            if ( formalArgumentNames.Length != numAttributes )
            {
                string formalArgumentsText = formalArguments.Select( fa => fa.name ).ToList().ToElementString();
                self.Error( "number of arguments " + formalArgumentsText +
                           " mismatch between attribute list and anonymous" +
                           " template in context " + self.GetEnclosingInstanceStackString() );
                // truncate arg list to match smaller size
                int shorterSize = Math.Min( formalArgumentNames.Length, numAttributes );
                numAttributes = shorterSize;
                string[] newFormalArgumentNames = new string[shorterSize];
                Array.Copy( formalArgumentNames, 0,
                                 newFormalArgumentNames, 0,
                                 shorterSize );
                formalArgumentNames = newFormalArgumentNames;
            }

            // keep walking while at least one attribute has values
            int i = 0; // iteration number from 0
            for ( ; ; )
            {
                argumentContext = new Dictionary<string, object>();
                // get a value for each attribute in list; put into arg context
                // to simulate template invocation of anonymous template
                int numEmpty = 0;
                for ( int a = 0; a < numAttributes; a++ )
                {
                    Iterator it = (Iterator)attributes[a];
                    if ( it != null && it.hasNext() )
                    {
                        string argName = (string)formalArgumentNames[a];
                        object iteratedValue = it.next();
                        argumentContext[argName] = iteratedValue;
                    }
                    else
                    {
                        numEmpty++;
                    }
                }
                if ( numEmpty == numAttributes )
                {
                    break;
                }
                argumentContext[DefaultIndexVariableName] = i + 1;
                argumentContext[DefaultIndex0VariableName] = i;
                StringTemplate embedded = templateToApply.GetInstanceOf();
                embedded.EnclosingInstance = self;
                embedded.ArgumentContext = argumentContext;
                results.Add( embedded );
                i++;
            }

            return results;
        }

        public virtual object ApplyListOfAlternatingTemplates( StringTemplate self,
                                                      object attributeValue,
                                                      IList<StringTemplate> templatesToApply )
        {
            if ( attributeValue == null || templatesToApply == null || templatesToApply.Count == 0 )
            {
                return null; // do not apply if missing templates or empty value
            }
            StringTemplate embedded = null;
            Dictionary<string, object> argumentContext = null;

            // normalize collections and such to use iterators
            // anything iteratable can be used for "APPLY"
            attributeValue = ConvertArrayToList( attributeValue );
            attributeValue = ConvertAnythingIteratableToIterator( attributeValue );

            if ( attributeValue is Iterator )
            {
                // results can be treated list an attribute, indicate ST created list
                IList resultVector = new StringTemplate.STAttributeList();
                Iterator iter = (Iterator)attributeValue;
                int i = 0;
                while ( iter.hasNext() )
                {
                    object ithValue = iter.next();
                    if ( ithValue == null )
                    {
                        if ( _nullValue == null )
                        {
                            continue;
                        }
                        ithValue = _nullValue;
                    }
                    int templateIndex = i % templatesToApply.Count; // rotate through
                    embedded = (StringTemplate)templatesToApply[templateIndex];
                    // template to apply is an actual StringTemplate (created in
                    // eval.g), but that is used as the examplar.  We must create
                    // a new instance of the embedded template to apply each time
                    // to get new attribute sets etc...
                    StringTemplateAST args = embedded.ArgumentsAST;
                    embedded = embedded.GetInstanceOf(); // make new instance
                    embedded.EnclosingInstance = self;
                    embedded.ArgumentsAST = args;
                    argumentContext = new Dictionary<string, object>();
                    var formalArgs = embedded.FormalArguments;
                    bool isAnonymous =
                        embedded.Name == StringTemplate.ANONYMOUS_ST_NAME;
                    SetSoleFormalArgumentToIthValue( embedded, argumentContext, ithValue );
                    // if it's an anonymous template with a formal arg, don't set it/attr
                    if ( !( isAnonymous && formalArgs != null && formalArgs.Count > 0 ) )
                    {
                        argumentContext[DefaultAttributeName] = ithValue;
                        argumentContext[DefaultAttributeNameDeprecated] = ithValue;
                    }
                    argumentContext[DefaultIndexVariableName] = i + 1;
                    argumentContext[DefaultIndex0VariableName] = i;
                    embedded.ArgumentContext = argumentContext;
                    EvaluateArguments( embedded );
                    /*
                    System.err.println("i="+i+": applyTemplate("+embedded.getName()+
                            ", args="+argumentContext+
                            " to attribute value "+ithValue);
                    */
                    resultVector.Add( embedded );
                    i++;
                }
                if ( resultVector.Count == 0 )
                {
                    resultVector = null;
                }
                return resultVector;
            }
            else
            {
                /*
                System.out.println("setting attribute "+DEFAULT_ATTRIBUTE_NAME+" in arg context of "+
                embedded.getName()+
                " to "+attributeValue);
                */
                embedded = (StringTemplate)templatesToApply[0];
                argumentContext = new Dictionary<string, object>();
                var formalArgs = embedded.FormalArguments;
                StringTemplateAST args = embedded.ArgumentsAST;
                SetSoleFormalArgumentToIthValue( embedded, argumentContext, attributeValue );
                bool isAnonymous =
                    embedded.Name == StringTemplate.ANONYMOUS_ST_NAME;
                // if it's an anonymous template with a formal arg, don't set it/attr
                if ( !( isAnonymous && formalArgs != null && formalArgs.Count > 0 ) )
                {
                    argumentContext[DefaultAttributeName] = attributeValue;
                    argumentContext[DefaultAttributeNameDeprecated] = attributeValue;
                }
                argumentContext[DefaultIndexVariableName] = 1;
                argumentContext[DefaultIndex0VariableName] = 0;
                embedded.ArgumentContext = argumentContext;
                EvaluateArguments( embedded );
                return embedded;
            }
        }

        protected virtual void SetSoleFormalArgumentToIthValue( StringTemplate embedded, IDictionary argumentContext, object ithValue )
        {
            var formalArgs = embedded.FormalArguments;
            if ( formalArgs != null )
            {
                string soleArgName = null;
                bool isAnonymous =
                    embedded.Name == StringTemplate.ANONYMOUS_ST_NAME;
                if ( formalArgs.Count == 1 || ( isAnonymous && formalArgs.Count > 0 ) )
                {
                    if ( isAnonymous && formalArgs.Count > 1 )
                    {
                        embedded.Error( "too many arguments on {...} template: " + formalArgs );
                    }
                    // if exactly 1 arg or anonymous, give that the value of
                    // "it" as a convenience like they said
                    // $list:template(arg=it)$
                    var argNames = formalArgs.Select( fa => fa.name );
                    soleArgName = (string)argNames.ToArray()[0];
                    argumentContext[soleArgName] = ithValue;
                }
            }
        }

        /** <summary>Return o.getPropertyName() given o and propertyName.  If o is
         *  a stringtemplate then access it's attributes looking for propertyName
         *  instead (don't check any of the enclosing scopes; look directly into
         *  that object).  Also try isXXX() for booleans.  Allow Map
         *  as special case (grab value for key).
         *  </summary>
         *
         *  <remarks>
         *  Cache repeated requests for obj.prop within same group.
         *  </remarks>
         */
        public virtual object GetObjectProperty( StringTemplate self,
                                        object o,
                                        object propertyName )
        {
            if ( o == null )
                return null;

            if ( propertyName == null )
            {
                if ( o is IDictionary && ( (IDictionary)o ).Contains( DefaultMapValueName ) )
                    propertyName = DefaultMapValueName;
                else
                    return null;
            }

            /*
            // see if property is cached in group's cache
            Object cachedValue =
                self.getGroup().getCachedObjectProperty(o,propertyName);
            if ( cachedValue!=null ) {
                return cachedValue;
            }
            Object value = rawGetObjectProperty(self, o, propertyName);
            // take care of array properties...convert to a List so we can
            // apply templates to the elements etc...
            value = convertArrayToList(value);
            self.getGroup().cacheObjectProperty(o,propertyName,value);
            */
            object value = RawGetObjectProperty( self, o, propertyName );
            // take care of array properties...convert to a List so we can
            // apply templates to the elements etc...
            value = ConvertArrayToList( value );
            return value;
        }

        [MethodImpl( MethodImplOptions.Synchronized )]
        static Func<object, object> FindMember( Type type, string name )
        {
            if ( type == null || name == null )
                throw new ArgumentNullException();

            Dictionary<string, Func<object, object>> members;
            Func<object, object> accessor = null;

            if ( _memberAccessors.TryGetValue( type, out members ) )
            {
                if ( members.TryGetValue( name, out accessor ) )
                    return accessor;
            }
            else
            {
                members = new Dictionary<string, Func<object, object>>();
                _memberAccessors[type] = members;
            }

            // must look up using reflection
            string methodSuffix = char.ToUpperInvariant( name[0] ) + name.Substring( 1 );

            // BEGIN ADDED FOR C#
            MethodInfo method = null;
            if ( method == null )
            {
                System.Reflection.PropertyInfo p = type.GetProperty( methodSuffix );
                if ( p != null )
                    method = p.GetGetMethod();
            }
            if ( method == null )
            {
                method = GetMethod( type, "Get" + methodSuffix );
            }
            if ( method == null )
            {
                method = GetMethod( type, "Is" + methodSuffix );
            }
            // END ADDED
            if ( method == null )
            {
                method = GetMethod( type, "get" + methodSuffix );
            }
            if ( method == null )
            {
                method = GetMethod( type, "is" + methodSuffix );
            }

            if ( method != null )
            {
                accessor = BuildAccessor( method );
            }
            else
            {
                // try for a visible field
                FieldInfo field = type.GetField( name );
                // also check .NET naming convention for fields
                if ( field == null )
                    field = type.GetField( "_" + name );

                if ( field != null )
                    accessor = BuildAccessor( field );
            }

            members[name] = accessor;
            return accessor;
        }

        static Func<object, object> BuildAccessor( MethodInfo method )
        {
            System.Reflection.Emit.DynamicMethod dm = new System.Reflection.Emit.DynamicMethod( method.DeclaringType.Name + method.Name + "MethodAccessor", typeof( object ), new Type[] { typeof( object ) }, method.DeclaringType );
            var gen = dm.GetILGenerator();

            if ( !method.IsStatic )
            {
                gen.Emit( System.Reflection.Emit.OpCodes.Ldarg_0 );
                gen.Emit( System.Reflection.Emit.OpCodes.Castclass, method.DeclaringType );
            }

            if ( method.IsVirtual && !method.IsFinal )
                gen.EmitCall( System.Reflection.Emit.OpCodes.Callvirt, method, null );
            else
                gen.EmitCall( System.Reflection.Emit.OpCodes.Call, method, null );

            if ( method.ReturnType.IsValueType )
                gen.Emit( System.Reflection.Emit.OpCodes.Box, method.ReturnType );

            gen.Emit( System.Reflection.Emit.OpCodes.Ret );
            return (Func<object, object>)dm.CreateDelegate( typeof( Func<object, object> ) );
        }

        static Func<object, object> BuildAccessor( FieldInfo field )
        {
            System.Reflection.Emit.DynamicMethod dm = new System.Reflection.Emit.DynamicMethod( field.DeclaringType.Name + field.Name + "FieldAccessor", typeof( object ), new Type[] { typeof( object ) }, field.DeclaringType );

            var gen = dm.GetILGenerator();
            if ( field.IsStatic )
            {
                gen.Emit( System.Reflection.Emit.OpCodes.Ldsfld, field );
            }
            else
            {
                gen.Emit( System.Reflection.Emit.OpCodes.Ldarg_0 );
                gen.Emit( System.Reflection.Emit.OpCodes.Castclass, field.DeclaringType );
                gen.Emit( System.Reflection.Emit.OpCodes.Ldfld, field );
            }

            if ( field.FieldType.IsValueType )
                gen.Emit( System.Reflection.Emit.OpCodes.Box, field.FieldType );

            gen.Emit( System.Reflection.Emit.OpCodes.Ret );
            return (Func<object, object>)dm.CreateDelegate( typeof( Func<object, object> ) );
        }

        protected virtual object RawGetObjectProperty( StringTemplate self, object o, object property )
        {
            Type c = o.GetType();
            object value = null;

            // Special case: our automatically created Aggregates via
            // attribute name: "{obj.{prop1,prop2}}"
            if ( c == typeof( StringTemplate.Aggregate ) )
            {
                string propertyName2 = (string)property;
                value = ( (StringTemplate.Aggregate)o ).Get( (string)propertyName2 );
                return value;
            }

            // Special case: if it's a template, pull property from
            // it's attribute table.
            // TODO: TJP just asked himself why we can't do inherited attr here?
            else if ( c == typeof( StringTemplate ) )
            {
                var attributes = ( (StringTemplate)o ).Attributes;
                if ( attributes != null )
                {
                    string propertyName2 = (string)property;
                    value = attributes.get( propertyName2 );
                    return value;
                }
            }

            // Special case: if it's a Map then pull using
            // key not the property method.
            if ( o is IDictionary )
            {
                IDictionary map = (IDictionary)o;
                if ( property.Equals( "keys" ) )
                {
                    value = map.Keys;
                }
                else if ( property.Equals( "values" ) )
                {
                    value = map.Values;
                }
                else if ( map.Contains( property ) )
                {
                    value = map[property];
                }
                else if ( map.Contains( property.ToString() ) )
                {
                    // if we can't find the key, toString it
                    value = map[property.ToString()];
                }
                else
                {
                    if ( map.Contains( DefaultMapValueName ) )
                    {
                        value = map[DefaultMapValueName];
                    }
                }
                if ( value == MapKeyValue )
                {
                    value = property;
                }
                return value;
            }

            string propertyName = (string)property;
            var accessor = FindMember( c, propertyName );
            if ( accessor != null )
            {
                try
                {
                    value = accessor( o );
                }
                catch ( Exception e )
                {
                    self.Error( "Can't access property " + propertyName + " using method get/is" + propertyName +
                        " or direct field access from " + c.Name + " instance", e );
                }
            }
            else
            {
                self.Error( "Class " + c.Name + " has no such attribute: " + propertyName +
                    " in template context " + self.GetEnclosingInstanceStackString(), null );
            }

            return value;
        }

        protected static MethodInfo GetMethod( Type c, string methodName )
        {
            // we want a getter method
            return c.GetMethod( methodName, new Type[0] );
        }

        /** <summary>
         *  Normally StringTemplate tests presence or absence of attributes
         *  for adherence to my principles of separation, but some people
         *  disagree and want to change.
         *  </summary>
         *
         *  <remarks>
         *  For 2.0, if the object is a boolean, do something special. $if(boolean)$
         *  will actually test the value.  Now, this breaks my rules of entanglement
         *  listed in my paper, but it truly surprises programmers to have booleans
         *  always true.  Further, the key to isolating logic in the model is avoiding
         *  operators (for which you need attribute values).  But, no operator is
         *  needed to use boolean values.  Well, actually I guess "!" (not) is
         *  an operator.  Regardless, for practical reasons, I'm going to technically
         *  violate my rules as I currently have them defined.  Perhaps for a future
         *  version of the paper I will refine the rules.
         *
         *  Post 2.1, I added a check for non-null Iterators, Collections, ...
         *  with size==0 to return false. TJP 5/1/2005
         *  </remarks>
         */
        public virtual bool TestAttributeTrue( object a )
        {
            if ( a == null )
            {
                return false;
            }
            if ( a is Boolean )
            {
                return (bool)a;
            }
            if ( a is ICollection )
            {
                return ( (ICollection)a ).Count > 0;
            }
            if ( a is IDictionary )
            {
                return ( (IDictionary)a ).Count > 0;
            }
            if ( a is Iterator )
            {
                return ( (Iterator)a ).hasNext();
            }
            return true; // any other non-null object, return true--it's present
        }

        /** <summary>
         *  For now, we can only add two objects as strings; convert objects to
         *  Strings then cat.
         *  </summary>
         */
        public virtual object Add( object a, object b )
        {
            if ( a == null )
            { // a null value means don't do cat, just return other value
                return b;
            }
            else if ( b == null )
            {
                return a;
            }
            return a.ToString() + b.ToString();
        }

        /** <summary>
         *  Call a string template with args and return result.  Do not convert
         *  to a string yet.  It may need attributes that will be available after
         *  this is inserted into another template.
         *  </summary>
         */
        public virtual StringTemplate GetTemplateInclude( StringTemplate enclosing,
                                                 string templateName,
                                                 StringTemplateAST argumentsAST )
        {
            //System.out.println("getTemplateInclude: look up "+enclosing.getGroup().getName()+"::"+templateName);
            StringTemplateGroup group = enclosing.Group;
            StringTemplate embedded = group.GetEmbeddedInstanceOf( enclosing, templateName );
            if ( embedded == null )
            {
                enclosing.Error( "cannot make embedded instance of " + templateName +
                        " in template " + enclosing.Name );
                return null;
            }
            embedded.ArgumentsAST = argumentsAST;
            EvaluateArguments( embedded );
            return embedded;
        }

        /** <summary>
         *  How to spit out an object.  If it's not a StringTemplate nor a
         *  List, just do o.toString().  If it's a StringTemplate,
         *  do o.write(out).  If it's a Vector, do a write(out,
         *  o.elementAt(i)) for all elements.  Note that if you do
         *  something weird like set the values of a multivalued tag
         *  to be vectors, it will effectively flatten it.
         *  </summary>
         *
         *  <remarks>
         *  If self is an embedded template, you might have specified
         *  a separator arg; used when is a vector.
         *  </remarks>
         */
        public virtual int WriteAttribute( StringTemplate self, object o, IStringTemplateWriter @out )
        {
            return Write( self, o, @out );
        }

        /** <summary>Write o relative to self to out.</summary>
         *
         *  <remarks>
         *  John Snyders fixes here for formatString.  Basically, any time
         *  you are about to write a value, check formatting.
         *  </remarks>
         */
        protected virtual int Write( StringTemplate self,
                            object o,
                            IStringTemplateWriter @out )
        {
            if ( o == null )
            {
                if ( _nullValue == null )
                {
                    return 0;
                }
                o = _nullValue; // continue with null option if specified
            }
            int n = 0;
            try
            {
                if ( o is StringTemplate )
                {
                    StringTemplate stToWrite = (StringTemplate)o;
                    // failsafe: perhaps enclosing instance not set
                    // Or, it could be set to another context!  This occurs
                    // when you store a template instance as an attribute of more
                    // than one template (like both a header file and C file when
                    // generating C code).  It must execute within the context of
                    // the enclosing template.
                    stToWrite.EnclosingInstance = self;
                    // if self is found up the enclosing instance chain, then
                    // infinite recursion
                    if ( StringTemplate.LintMode &&
                         StringTemplate.IsRecursiveEnclosingInstance( stToWrite ) )
                    {
                        // throw exception since sometimes eval keeps going
                        // even after I ignore this write of o.
                        throw new InvalidOperationException( "infinite recursion to " +
                                stToWrite.GetTemplateDeclaratorString() + " referenced in " +
                                stToWrite.EnclosingInstance.GetTemplateDeclaratorString() +
                                "; stack trace:" + Environment.NewLine + stToWrite.GetEnclosingInstanceStackTrace() );
                    }
                    else
                    {
                        // if we have a wrap string, then inform writer it
                        // might need to wrap
                        if ( _wrapString != null )
                        {
                            n = @out.WriteWrapSeparator( _wrapString );
                        }
                        // check if formatting needs to be applied to the stToWrite
                        if ( _formatString != null )
                        {
                            IAttributeRenderer renderer =
                                self.GetAttributeRenderer( typeof( string ) );
                            if ( renderer != null )
                            {
                                // you pay a penalty for applying format option to a template
                                // because the template must be written to a temp StringWriter so it can
                                // be formatted before being written to the real output.
                                StringWriter buf = new StringWriter();
                                IStringTemplateWriter sw =
                                    self.Group.GetStringTemplateWriter( buf );
                                stToWrite.Write( sw );
                                n = @out.Write( renderer.ToString( buf.ToString(), _formatString ) );
                                return n;
                            }
                        }
                        n = stToWrite.Write( @out );
                    }
                    return n;
                }
                // normalize anything iteratable to iterator
                o = ConvertAnythingIteratableToIterator( o );
                if ( o is Iterator )
                {
                    Iterator iter = (Iterator)o;
                    bool seenPrevValue = false;
                    while ( iter.hasNext() )
                    {
                        object iterValue = iter.next();
                        if ( iterValue == null )
                        {
                            iterValue = _nullValue;
                        }
                        if ( iterValue != null )
                        {
                            if ( seenPrevValue /*prevIterValue!=null*/
                                && _separatorString != null )
                            {
                                n += @out.WriteSeparator( _separatorString );
                            }
                            seenPrevValue = true;
                            int nw = Write( self, iterValue, @out );
                            n += nw;
                        }
                    }
                }
                else
                {
                    IAttributeRenderer renderer =
                        self.GetAttributeRenderer( o.GetType() );
                    string v = null;
                    if ( renderer != null )
                    {
                        if ( _formatString != null )
                        {
                            v = renderer.ToString( o, _formatString );
                        }
                        else
                        {
                            v = renderer.ToString( o );
                        }
                    }
                    else
                    {
                        v = o.ToString();
                    }
                    if ( _wrapString != null )
                    {
                        n = @out.Write( v, _wrapString );
                    }
                    else
                    {
                        n = @out.Write( v );
                    }
                    return n;
                }
            }
            catch ( IOException io )
            {
                self.Error( "problem writing object: " + o, io );
            }
            return n;
        }

        /** <summary>
         *  A expr is normally just a string literal, but is still an AST that
         *  we must evaluate.  The expr can be any expression such as a template
         *  include or string cat expression etc...  Evaluate with its own writer
         *  so that we can convert to string and then reuse, don't want to compute
         *  all the time; must precompute w/o writing to output buffer.
         *  </summary>
         */
        protected virtual string EvaluateExpression( StringTemplate self,
                                            object expr )
        {
            if ( expr == null )
            {
                return null;
            }
            if ( expr is StringTemplateAST )
            {
                StringTemplateAST exprAST = (StringTemplateAST)expr;
                // must evaluate, writing to a string so we can hang on to it
                StringWriter buf = new StringWriter();
                IStringTemplateWriter sw =
                    self.Group.GetStringTemplateWriter( buf );
                {
                    ActionEvaluator eval =
                            new ActionEvaluator( self, this, sw, exprAST );
                    try
                    {
                        eval.action(); // eval tree
                    }
                    catch ( RecognitionException re )
                    {
                        self.Error( "can't evaluate tree: " + _exprTree.ToStringTree(), re );
                    }
                }
                return buf.ToString();
            }
            else
            {
                // just in case we expand in the future and it's something else
                return expr.ToString();
            }
        }

        /** <summary>
         *  Evaluate an argument list within the context of the enclosing
         *  template but store the values in the context of self, the
         *  new embedded template.  For example, bold(item=item) means
         *  that bold.item should get the value of enclosing.item.
         *  </summary>
         */
        protected virtual void EvaluateArguments( StringTemplate self )
        {
            StringTemplateAST argumentsAST = self.ArgumentsAST;
            if ( argumentsAST == null || argumentsAST.GetChild( 0 ) == null )
            {
                // return immediately if missing tree or no actual args
                return;
            }

            // Evaluate args in the context of the enclosing template, but we
            // need the predefined args like 'it', 'attr', and 'i' to be
            // available as well so we put a dummy ST between the enclosing
            // context and the embedded context.  The dummy has the predefined
            // context as does the embedded.
            StringTemplate enclosing = self.EnclosingInstance;
            StringTemplate argContextST = new StringTemplate( self.Group, "" );
            argContextST.Name = "<invoke " + self.Name + " arg context>";
            argContextST.EnclosingInstance = enclosing;
            argContextST.ArgumentContext = self.ArgumentContext;

            ActionEvaluator eval =
                    new ActionEvaluator( argContextST, this, null, argumentsAST );
            /*
            System.out.println("eval args: "+argumentsAST.toStringList());
            System.out.println("ctx is "+self.getArgumentContext());
            */
            try
            {
                // using any initial argument context (such as when obj is set),
                // evaluate the arg list like bold(item=obj).  Since we pass
                // in any existing arg context, that context gets filled with
                // new values.  With bold(item=obj), context becomes:
                // {[obj=...],[item=...]}.
                Dictionary<string, object> ac = eval.argList( self, self.ArgumentContext );
                self.ArgumentContext = ac;
            }
            catch ( RecognitionException re )
            {
                self.Error( "can't evaluate tree: " + argumentsAST.ToStringTree(), re );
            }
        }

#if false
        public static readonly HashSet<Type> arraysConvertibleToList = new HashSet<Type>()
            {
                typeof(int[]),
                typeof(long[]),
                typeof(float[]),
                typeof(double[])
            };
#endif

        /** <summary>
         *  Do a standard conversion of array attributes to a List.  Wrap the
         *  array instead of copying like old version.  Make an
         *  ArrayWrappedInList that knows to create an ArrayIterator.
         *  </summary>
         */
        public static object ConvertArrayToList( object value )
        {
            if ( value == null )
            {
                return null;
            }
            if ( value.GetType().IsArray )
            {
                if ( value.GetType().GetElementType().IsPrimitive )
                {
                    return (IList)value;
                }
                return (IList)(object[])value;
            }
            return value;
        }

        protected internal static object ConvertAnythingIteratableToIterator( object o )
        {
            Iterator iter = null;
            if ( o is IDictionary )
            {
                iter = ( (IDictionary)o ).Values.iterator();
            }
            else if ( o is System.Collections.IEnumerable && !(o is string) )
            {
                iter = ( (System.Collections.IEnumerable)o ).Cast<object>().iterator();
            }
            else if ( o is Iterator )
            {
                iter = (Iterator)o;
            }
            if ( iter == null )
            {
                return o;
            }
            return iter;
        }

        protected static Iterator ConvertAnythingToIterator( object o )
        {
            Iterator iter = null;
            if ( o is ICollection )
            {
                iter = ( (ICollection)o ).iterator();
            }
            else if ( o is IDictionary )
            {
                iter = ( (IDictionary)o ).Values.iterator();
            }
            else if ( o is Iterator )
            {
                iter = (Iterator)o;
            }
            if ( iter == null )
            {
                IList singleton = new StringTemplate.STAttributeList( 1 );
                singleton.Add( o );
                return singleton.iterator();
            }
            return iter;
        }

        /** <summary>
         *  Return the first attribute if multiple valued or the attribute
         *  itself if single-valued.  Used in &lt;names:first()>
         *  </summary>
         */
        public virtual object First( object attribute )
        {
            if ( attribute == null )
            {
                return null;
            }
            object f = attribute;
            attribute = ConvertAnythingIteratableToIterator( attribute );
            if ( attribute is Iterator )
            {
                Iterator it = (Iterator)attribute;
                if ( it.hasNext() )
                {
                    f = it.next();
                }
            }

            return f;
        }

        /** <summary>
         *  Return the everything but the first attribute if multiple valued
         *  or null if single-valued.  Used in &lt;names:rest()>.
         *  </summary>
         */
        public virtual object Rest( object attribute )
        {
            if ( attribute == null )
            {
                return null;
            }
            object theRest = attribute;
            attribute = ConvertAnythingIteratableToIterator( attribute );
            if ( attribute is Iterator )
            {
                IList a = new List<object>();
                Iterator it = (Iterator)attribute;
                if ( !it.hasNext() )
                {
                    return null; // if not even one value return null
                }
                it.next(); // ignore first value
                while ( it.hasNext() )
                {
                    object o = (object)it.next();
                    if ( o != null )
                        a.Add( o );
                }
                return a;
            }
            else
            {
                theRest = null;  // rest of single-valued attribute is null
            }

            return theRest;
        }

        /** <summary>
         *  Return the last attribute if multiple valued or the attribute
         *  itself if single-valued.  Used in &lt;names:last()>.  This is pretty
         *  slow as it iterates until the last element.  Ultimately, I could
         *  make a special case for a List or Vector.
         *  </summary>
         */
        public virtual object Last( object attribute )
        {
            if ( attribute == null )
            {
                return null;
            }
            object last = attribute;
            attribute = ConvertAnythingIteratableToIterator( attribute );
            if ( attribute is Iterator )
            {
                Iterator it = (Iterator)attribute;
                while ( it.hasNext() )
                {
                    last = it.next();
                }
            }

            return last;
        }

        /** <summary>Return a new list w/o null values.</summary> */
        public virtual object Strip( object attribute )
        {
            if ( attribute == null )
            {
                return null;
            }
            attribute = ConvertAnythingIteratableToIterator( attribute );
            if ( attribute is Iterator )
            {
                IList a = new List<object>();
                Iterator it = (Iterator)attribute;
                while ( it.hasNext() )
                {
                    object o = (object)it.next();
                    if ( o != null )
                        a.Add( o );
                }
                return a;
            }
            return attribute; // strip(x)==x when x single-valued attribute
        }

        /** <summary>Return all but the last element.  trunc(x)=null if x is single-valued.</summary> */
        public virtual object Trunc( object attribute )
        {
            if ( attribute == null )
            {
                return null;
            }
            attribute = ConvertAnythingIteratableToIterator( attribute );
            if ( attribute is Iterator )
            {
                IList a = new List<object>();
                Iterator it = (Iterator)attribute;
                while ( it.hasNext() )
                {
                    object o = (object)it.next();
                    if ( it.hasNext() )
                        a.Add( o ); // only add if not last one
                }
                return a;
            }
            return null; // trunc(x)==null when x single-valued attribute
        }

        /** <summary>
         *  Return the length of a multiple valued attribute or 1 if it is a
         *  single attribute. If attribute is null return 0.
         *  Special case several common collections and primitive arrays for
         *  speed.  This method by Kay Roepke.
         *  </summary>
         */
        public virtual object Length( object attribute )
        {
            if ( attribute == null )
            {
                return 0;
            }
            int i = 1;		// we have at least one of something. Iterator and arrays might be empty.
            if ( attribute is IDictionary )
            {
                i = ( (IDictionary)attribute ).Count;
            }
            else if ( attribute is ICollection )
            {
                i = ( (ICollection)attribute ).Count;
            }
            else if ( attribute is object[] )
            {
                object[] list = (object[])attribute;
                i = list.Length;
            }
            else if ( attribute is int[] )
            {
                int[] list = (int[])attribute;
                i = list.Length;
            }
            else if ( attribute is long[] )
            {
                long[] list = (long[])attribute;
                i = list.Length;
            }
            else if ( attribute is float[] )
            {
                float[] list = (float[])attribute;
                i = list.Length;
            }
            else if ( attribute is double[] )
            {
                double[] list = (double[])attribute;
                i = list.Length;
            }
            else if ( attribute is Iterator )
            {
                Iterator it = (Iterator)attribute;
                i = 0;
                while ( it.hasNext() )
                {
                    it.next();
                    i++;
                }
            }
            return i;
        }

        public object GetOption( string name )
        {
            object value = null;
            if ( _options != null )
            {
                if ( _options.TryGetValue( name, out value ) )
                {
                    string s = value as string;
                    if ( s != null && s == EmptyOption )
                    {
                        StringTemplateAST st;
                        if ( _defaultOptionValues.TryGetValue( name, out st ) )
                            return st;
                        return null;
                    }
                }
            }
            return value;
        }

        public override string ToString()
        {
            return _exprTree.ToStringTree();
        }

        #endregion
    }
}
