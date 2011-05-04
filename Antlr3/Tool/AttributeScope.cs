/*
 * [The "BSD licence"]
 * Copyright (c) 2005-2008 Terence Parr
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

namespace Antlr3.Tool
{
    using System.Collections.Generic;
    using System.Linq;

    using CodeGenerator = Antlr3.Codegen.CodeGenerator;
    using IToken = Antlr.Runtime.IToken;

    /** Track the attributes within a scope.  A named scoped has just its list
     *  of attributes.  Each rule has potentially 3 scopes: return values,
     *  parameters, and an implicitly-named scope (i.e., a scope defined in a rule).
     *  Implicitly-defined scopes are named after the rule; rules and scopes then
     *  must live in the same name space--no collisions allowed.
     */
    public class AttributeScope
    {
        /** All token scopes (token labels) share the same fixed scope of
         *  of predefined attributes.  I keep this out of the runtime.Token
         *  object to avoid a runtime space burden.
         */
        public static readonly AttributeScope tokenScope = new AttributeScope( "Token", null );

        static AttributeScope()
        {
            tokenScope.AddAttribute( "text", null );
            tokenScope.AddAttribute( "type", null );
            tokenScope.AddAttribute( "line", null );
            tokenScope.AddAttribute( "index", null );
            tokenScope.AddAttribute( "pos", null );
            tokenScope.AddAttribute( "channel", null );
            tokenScope.AddAttribute( "tree", null );
            tokenScope.AddAttribute( "int", null );
        }

        /** This scope is associated with which input token (for error handling)? */
        private readonly IToken _derivedFromToken;

        private readonly Grammar _grammar;

        /** The scope name */
        private readonly string _name;

        /** Not a rule scope, but visible to all rules "scope symbols { ...}" */
        private bool _isDynamicGlobalScope;

        /** Visible to all rules, but defined in rule "scope { int i; }" */
        private bool _isDynamicRuleScope;

        private bool _isParameterScope;

        private bool _isReturnScope;

        private bool _isPredefinedRuleScope;

        private bool _isPredefinedLexerRuleScope;

        private readonly Dictionary<string, GrammarAST> _actions = new Dictionary<string, GrammarAST>();

        /** The list of Attribute objects */

        // until we have a list-ordered dictionary
        //protected internal IDictionary<string, Attribute> attributes = new SortedList<string, Attribute>();
        private readonly List<Attribute> _attributes = new List<Attribute>();

        public AttributeScope( string name, IToken derivedFromToken )
            : this( null, name, derivedFromToken )
        {
        }

        public AttributeScope( Grammar grammar, string name, IToken derivedFromToken )
        {
            this._grammar = grammar;
            this._name = name;
            this._derivedFromToken = derivedFromToken;
        }

        #region Properties

        public bool IsDynamicGlobalScope
        {
            get
            {
                return _isDynamicGlobalScope;
            }

            set
            {
                _isDynamicGlobalScope = value;
            }
        }

        public bool IsDynamicRuleScope
        {
            get
            {
                return _isDynamicRuleScope;
            }

            set
            {
                _isDynamicRuleScope = value;
            }
        }

        public bool IsParameterScope
        {
            get
            {
                return _isParameterScope;
            }

            set
            {
                _isParameterScope = value;
            }
        }

        public bool IsReturnScope
        {
            get
            {
                return _isReturnScope;
            }

            set
            {
                _isReturnScope = value;
            }
        }

        public bool IsPredefinedRuleScope
        {
            get
            {
                return _isPredefinedRuleScope;
            }

            set
            {
                _isPredefinedRuleScope = value;
            }
        }

        public bool IsPredefinedLexerRuleScope
        {
            get
            {
                return _isPredefinedLexerRuleScope;
            }

            set
            {
                _isPredefinedLexerRuleScope = value;
            }
        }

        public IDictionary<string, GrammarAST> Actions
        {
            get
            {
                return _actions;
            }
        }

        public IList<Attribute> Attributes
        {
            get
            {
                return _attributes.AsReadOnly();
            }
        }

        public int Count
        {
            get
            {
                return _attributes == null ? 0 : _attributes.Count;
            }
        }

        public Grammar Grammar
        {
            get
            {
                return _grammar;
            }
        }

        public string Name
        {
            get
            {
                if ( IsParameterScope )
                    return _name + "_parameter";
                else if ( IsReturnScope )
                    return _name + "_return";

                return _name;
            }
        }

        #endregion

        /** From a chunk of text holding the definitions of the attributes,
         *  pull them apart and create an Attribute for each one.  Add to
         *  the list of attributes for this scope.  Pass in the character
         *  that terminates a definition such as ',' or ';'.  For example,
         *
         *  scope symbols {
         *  	int n;
         *  	List names;
         *  }
         *
         *  would pass in definitions equal to the text in between {...} and
         *  separator=';'.  It results in two Attribute objects.
         */
        public virtual void AddAttributes( string definitions, int separator )
        {
            IList<string> attrs = new List<string>();
            CodeGenerator.GetListOfArgumentsFromAction( definitions, 0, -1, separator, attrs );
            foreach ( string a in attrs )
            {
                Attribute attr = new Attribute( a );
                if ( !IsReturnScope && attr.InitValue != null )
                {
                    ErrorManager.GrammarError( ErrorManager.MSG_ARG_INIT_VALUES_ILLEGAL,
                                              _grammar,
                                              _derivedFromToken,
                                              attr.Name );
                    attr.InitValue = null; // wipe it out
                }
                for ( int i = 0; i <= _attributes.Count; i++ )
                {
                    if ( i < _attributes.Count )
                    {
                        if ( _attributes[i].Name == attr.Name )
                        {
                            _attributes[i] = attr;
                            break;
                        }
                    }
                    else
                    {
                        _attributes.Add( attr );
                        // *must* break since the count changed
                        break;
                        //attributes.put( attr.Name, attr );
                    }
                }
            }
        }

        public virtual void AddAttribute( string name, string decl )
        {
            Attribute attr = new Attribute( name, decl );
            for ( int i = 0; i <= _attributes.Count; i++ )
            {
                if ( i < _attributes.Count )
                {
                    if ( _attributes[i].Name == attr.Name )
                    {
                        _attributes[i] = attr;
                        break;
                    }
                }
                else
                {
                    _attributes.Add( attr );
                    // *must* break since the count changed
                    break;
                    //attributes.put( name, new Attribute( name, decl ) );
                }
            }
        }

        /** Given @scope::name {action} define it for this attribute scope. Later,
         *  the code generator will ask for the actions table.
         */
        public void DefineNamedAction( GrammarAST nameAST, GrammarAST actionAST )
        {
            string actionName = nameAST.Text;
            GrammarAST a;
            if ( Actions.TryGetValue( actionName, out a ) && a != null )
            {
                ErrorManager.GrammarError(
                    ErrorManager.MSG_ACTION_REDEFINITION, _grammar,
                    nameAST.Token, nameAST.Text );
            }
            else
            {
                Actions[actionName] = actionAST;
            }
        }

        public virtual Attribute GetAttribute( string name )
        {
            return _attributes.FirstOrDefault( attr => attr.Name == name );
        }

        /** Return the set of keys that collide from
         *  this and other.
         */
        public virtual HashSet<object> Intersection( AttributeScope other )
        {
            if ( other == null || other.Count == 0 || Count == 0 )
            {
                return null;
            }
            HashSet<object> inter = new HashSet<object>();
            foreach ( Attribute attr in _attributes )
            {
                string key = attr.Name;
                if ( other.GetAttribute( key ) != null )
                {
                    inter.Add( key );
                }
            }
            if ( inter.Count == 0 )
            {
                return null;
            }
            return inter;
        }

        public override string ToString()
        {
            return ( IsDynamicGlobalScope ? "global " : "" ) + Name + ":" + _attributes;
        }
    }
}
