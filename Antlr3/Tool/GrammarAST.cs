/*
 * [The "BSD licence"]
 * Copyright (c) 2005-2008 Terence Parr
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

namespace Antlr3.Tool
{
    using System.Collections.Generic;
    using System.Linq;
    using Antlr.Runtime.JavaExtensions;
    using Antlr3.Extensions;

    using ANTLRParser = Antlr3.Grammars.ANTLRParser;
    using CommonToken = Antlr.Runtime.CommonToken;
    using DFA = Antlr3.Analysis.DFA;
    using IIntSet = Antlr3.Misc.IIntSet;
    using IToken = Antlr.Runtime.IToken;
    using ITree = Antlr.Runtime.Tree.ITree;
    using NFAState = Antlr3.Analysis.NFAState;
    using Obsolete = System.ObsoleteAttribute;
    using StringTemplate = Antlr3.ST.StringTemplate;

    /** Grammars are first converted to ASTs using this class and then are
     *  converted to NFAs via a tree walker.
     *
     *  The reader may notice that I have made a very non-OO decision in this
     *  class to track variables for many different kinds of nodes.  It wastes
     *  space for nodes that don't need the values and OO principles cry out
     *  for a new class type for each kind of node in my tree.  I am doing this
     *  on purpose for a variety of reasons.  I don't like using the type
     *  system for different node types; it yields too many damn class files
     *  which I hate.  Perhaps if I put them all in one file.  Most importantly
     *  though I hate all the type casting that would have to go on.  I would
     *  have all sorts of extra work to do.  Ick.  Anyway, I'm doing all this
     *  on purpose, not out of ignorance. ;)
     */
    public class GrammarAST : Antlr.Runtime.Tree.CommonTree
    {
        static int count = 0;

        public int ID = ++count;

        /** This AST node was created from what token? */
        //public Token token = null;

        public string enclosingRuleName;

        /** If this is a decision node, what is the lookahead DFA? */
        public DFA lookaheadDFA = null;

        /** What NFA start state was built from this node? */
        public NFAState _nfaStartState = null;

        /** This is used for TREE_BEGIN nodes to point into
         *  the NFA.  TREE_BEGINs point at left edge of DOWN for LOOK computation
         *  purposes (Nullable tree child list needs special code gen when matching).
         */
        public NFAState NFATreeDownState = null;

        /** Rule ref nodes, token refs, set, and NOT set refs need to track their
         *  location in the generated NFA so that local FOLLOW sets can be
         *  computed during code gen for automatic error recovery.
         */
        public NFAState followingNFAState = null;

        /** If this is a SET node, what are the elements? */
        protected IIntSet setValue = null;

        /** If this is a BLOCK node, track options here */
        protected IDictionary<string, object> blockOptions;

        /** If this is a BLOCK node for a rewrite rule, track referenced
         *  elements here.  Don't track elements in nested subrules.
         */
        public HashSet<GrammarAST> rewriteRefsShallow;

        /*	If REWRITE node, track EVERY element and label ref to right of ->
         *  for this rewrite rule.  There could be multiple of these per
         *  rule:
         *
         *     a : ( ... -> ... | ... -> ... ) -> ... ;
         *
         *  We may need a list of all refs to do definitions for whole rewrite
         *  later.
         *
         *  If BLOCK then tracks every element at that level and below.
         */
        public HashSet<GrammarAST> rewriteRefsDeep;

        public IDictionary<string, object> terminalOptions;

        /** if this is an ACTION node, this is the outermost enclosing
         *  alt num in rule.  For actions, define.g sets these (used to
         *  be codegen.g).  We need these set so we can examine actions
         *  early, before code gen, for refs to rule predefined properties
         *  and rule labels.  For most part define.g sets outerAltNum, but
         *  codegen.g does the ones for %foo(a={$ID.text}) type refs as
         *  the {$ID...} is not seen as an action until code gen pulls apart.
         */
        public int outerAltNum;

        /** if this is a TOKEN_REF or RULE_REF node, this is the code StringTemplate
         *  generated for this node.  We need to update it later to add
         *  a label if someone does $tokenref or $ruleref in an action.
         */
        public StringTemplate code;

        public GrammarAST()
        {
        }

        public GrammarAST( int t, string txt )
        {
            Initialize( t, txt );
        }

        public GrammarAST( IToken token )
        {
            Initialize( token );
        }

        #region Properties
        public IDictionary<string, object> BlockOptions
        {
            get
            {
                return blockOptions;
            }
            set
            {
                blockOptions = value;
            }
        }
        public GrammarAST LastChild
        {
            get
            {
                if ( ChildCount == 0 )
                    return null;
                return (GrammarAST)GetChild( ChildCount - 1 );
            }
        }
        public GrammarAST LastSibling
        {
            get
            {
                ITree parent = Parent;
                if ( parent == null )
                    return null;
                return (GrammarAST)parent.GetChild( parent.ChildCount - 1 );
            }
        }
        public DFA LookaheadDFA
        {
            get
            {
                return lookaheadDFA;
            }
            set
            {
                lookaheadDFA = value;
            }
        }
        public NFAState NFAStartState
        {
            get
            {
                return _nfaStartState;
            }
            set
            {
                _nfaStartState = value;
            }
        }
        public IIntSet SetValue
        {
            get
            {
                return setValue;
            }
            set
            {
                setValue = value;
            }
        }
        #endregion

        public virtual void Initialize( int i, string s )
        {
            token = new CommonToken( i, s );
        }

        public virtual void Initialize( ITree ast )
        {
            GrammarAST t = ( (GrammarAST)ast );
            this.token = t.token;
            this.enclosingRuleName = t.enclosingRuleName;
            this.TokenStartIndex = ast.TokenStartIndex;
            this.TokenStopIndex = ast.TokenStopIndex;
            this.setValue = t.setValue;
            this.blockOptions = t.blockOptions;
            this.outerAltNum = t.outerAltNum;
        }

        public virtual void Initialize( IToken token )
        {
            this.token = token;
        }

        /** Save the option key/value pair and process it; return the key
         *  or null if invalid option.
         */
        public virtual string SetBlockOption( Grammar grammar, string key, object value )
        {
            if ( blockOptions == null )
            {
                blockOptions = new Dictionary<string, object>();
            }
            return SetOption( blockOptions, Grammar.legalBlockOptions, grammar, key, value );
        }

        public virtual string SetTerminalOption( Grammar grammar, string key, object value )
        {
            if ( terminalOptions == null )
            {
                terminalOptions = new Dictionary<string, object>();
            }
            return SetOption( terminalOptions, Grammar.legalTokenOptions, grammar, key, value );
        }

        public virtual string SetOption( IDictionary<string, object> options, HashSet<string> legalOptions, Grammar grammar, string key, object value )
        {
            if ( !legalOptions.Contains( key ) )
            {
                ErrorManager.GrammarError( ErrorManager.MSG_ILLEGAL_OPTION,
                                          grammar,
                                          token,
                                          key );
                return null;
            }
            if ( value is string )
            {
                string vs = (string)value;
                if ( vs[0] == '"' )
                {
                    value = vs.Substring( 1, vs.Length - 2 ); // strip quotes
                }
            }
            if ( key.Equals( "k" ) )
            {
                grammar.numberOfManualLookaheadOptions++;
            }
            if ( key == "backtrack" && value.ToString() == "true" )
            {
                grammar.composite.GetRootGrammar().atLeastOneBacktrackOption = true;
            }
            options[key] = value;
            return key;
        }

        public virtual object GetBlockOption( string key )
        {
            object value = null;
            if ( blockOptions != null )
            {
                value = blockOptions.get( key );
            }
            return value;
        }

        public virtual void SetOptions( Grammar grammar, IDictionary<string, object> options )
        {
            if ( options == null )
            {
                this.blockOptions = null;
                return;
            }
            foreach ( string optionName in options.Keys.ToArray() )
            {
                string stored = SetBlockOption( grammar, optionName, options.get( optionName ) );
                if ( stored == null )
                    options.Remove( optionName );
            }
        }

        public override string Text
        {
            get
            {
                if ( token == null )
                    return string.Empty;

                return token.Text;
            }
            set
            {
                token.Text = value;
            }
        }
        public override int Type
        {
            get
            {
                if ( token == null )
                    return -1;

                return token.Type;
            }
            set
            {
                token.Type = value;
            }
        }
        public override int Line
        {
            get
            {
                int line = 0;
                if ( token != null )
                {
                    line = token.Line;
                }
                if ( line == 0 )
                {
                    ITree child = this.GetChild( 0 );
                    if ( child != null )
                    {
                        line = child.Line;
                    }
                }
                return line;
            }
            set
            {
                token.Line = value;
            }
        }
        public override int CharPositionInLine
        {
            get
            {
                int col = 0;
                if ( token != null )
                {
                    col = token.CharPositionInLine;
                }
                if ( col == 0 )
                {
                    ITree child = this.GetChild( 0 );
                    if ( child != null )
                    {
                        col = child.CharPositionInLine;
                    }
                }
                return col;
            }
            set
            {
                token.CharPositionInLine = value;
            }
        }

        public virtual GrammarAST[] GetChildrenAsArray()
        {
            return Children.CastListDown<GrammarAST, ITree>().ToArray();
        }

        // used in enumerating the descendants of a node
        static readonly GrammarAST DescendantDownNode = new GrammarAST( Antlr.Runtime.TokenConstants.Down, "DOWN" );
        static readonly GrammarAST DescendantUpNode = new GrammarAST( Antlr.Runtime.TokenConstants.Up, "UP" );

        public static IEnumerable<ITree> Descendants( ITree root )
        {
            return Descendants( root, false );
        }
        public static IEnumerable<ITree> Descendants( ITree root, bool insertDownUpNodes )
        {
            int count = root.ChildCount;

            if ( insertDownUpNodes )
            {
                yield return root;

                yield return DescendantDownNode;

                for ( int i = 0; i < count; i++ )
                {
                    ITree child = root.GetChild( i );
                    foreach ( ITree subchild in Descendants( child, true ) )
                        yield return subchild;
                }

                yield return DescendantUpNode;
            }
            else
            {
                for ( int i = 0; i < count; i++ )
                {
                    ITree child = root.GetChild( i );

                    yield return child;

                    foreach ( ITree subchild in Descendants( child, false ) )
                        yield return subchild;
                }
            }
        }

        /** Return a reference to the first node (depth-first) that has
         *  token type ttype.  Assume 'this' is a root node; don't visit siblings
         *  of root.  Return null if no node found with ttype.
         */
        public GrammarAST FindFirstType( int ttype )
        {
            // check this node (the root) first
            if ( this.Type == ttype )
                return this;

            // else check children
            return Descendants( this ).OfType<GrammarAST>().FirstOrDefault( child => child.Type == ttype );
        }

        /** Make nodes unique based upon Token so we can add them to a Set; if
         *  not a GrammarAST, check type.
         */
        public override bool Equals( object ast )
        {
            if ( this == ast )
            {
                return true;
            }

            GrammarAST t = (GrammarAST)ast;
            if ( t == null )
            {
                ITree a = ast as ITree;
                return a != null && Type == a.Type;
            }

            return token.Line == t.Line &&
                   token.CharPositionInLine == t.CharPositionInLine;
        }

        public override int GetHashCode()
        {
            return ObjectExtensions.ShiftPrimeXOR( token.Line, token.CharPositionInLine );
        }

        /** See if tree has exact token types and structure; no text */
        public bool HasSameTreeStructure( ITree other )
        {
            // check roots first.
            if ( Type != other.Type )
                return false;

            // if roots match, do full list match test on children.
            return Descendants( this, true ).SequenceEqual( Descendants( other, true ), ( a, b ) => a.Type == b.Type );
        }

        public static GrammarAST Dup( ITree t )
        {
            if ( t == null )
            {
                return null;
            }
            GrammarAST dup_t = new GrammarAST();
            dup_t.Initialize( t );
            return dup_t;
        }

        public override ITree DupNode()
        {
            return Dup( this );
        }

        static IEnumerable<GrammarAST> GetChildrenForDupTree( GrammarAST t )
        {
            for ( int i = 0; i < t.ChildCount; i++ )
            {
                GrammarAST child = (GrammarAST)t.GetChild( i );
                int ttype = child.Type;
                if ( ttype == ANTLRParser.REWRITE )
                {
                    continue;
                }
                else if ( ttype == ANTLRParser.BANG || ttype == ANTLRParser.ROOT )
                {
                    foreach ( GrammarAST subchild in GetChildrenForDupTree( child ) )
                        yield return subchild;
                }
                else
                {
                    yield return child;
                }
            }
        }

        /**Duplicate a tree, assuming this is a root node of a tree--
         * duplicate that node and what's below; ignore siblings of root node.
         */
        public static GrammarAST DupTreeNoActions( GrammarAST t, GrammarAST parent )
        {
            GrammarAST d = (GrammarAST)t.DupNode();
            foreach ( GrammarAST subchild in GetChildrenForDupTree( t ) )
                d.AddChild( DupTreeNoActions( subchild, d ) );

            return d;
        }

        public void SetTreeEnclosingRuleNameDeeply( string rname )
        {
            enclosingRuleName = rname;
            foreach ( GrammarAST child in Descendants( this ).OfType<GrammarAST>() )
                child.enclosingRuleName = rname;
        }

        internal string ToStringList()
        {
            throw new System.NotImplementedException();
        }
    }
}
