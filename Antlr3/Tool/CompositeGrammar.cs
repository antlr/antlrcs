﻿/*
 * [The "BSD license"]
 * Copyright (c) 2011 Terence Parr
 * All rights reserved.
 *
 * Conversion to C#:
 * Copyright (c) 2011 Sam Harwell, Pixel Mine, Inc.
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
    using Antlr3.Grammars;
    using Antlr3.Misc;

    using Label = Antlr3.Analysis.Label;
    using NFAState = Antlr3.Analysis.NFAState;
    using RecognitionException = Antlr.Runtime.RecognitionException;

    /** A tree of component (delegate) grammars.
     *
     *  Rules defined in delegates are "inherited" like multi-inheritance
     *  so you can override them.  All token types must be consistent across
     *  rules from all delegate grammars, so they must be stored here in one
     *  central place.
     *
     *  We have to start out assuming a composite grammar situation as we can't
     *  look into the grammar files a priori to see if there is a delegate
     *  statement.  Because of this, and to avoid duplicating token type tracking
     *  in each grammar, even single noncomposite grammars use one of these objects
     *  to track token types.
     */
    public class CompositeGrammar
    {
        public const int MinRuleIndex = 1;

        private CompositeGrammarTree delegateGrammarTreeRoot;

        /** Used during getRuleReferenceClosure to detect computation cycles */
        private readonly HashSet<NFAState> refClosureBusy = new HashSet<NFAState>();

        /** Used to assign state numbers; all grammars in composite share common
         *  NFA space.  This NFA tracks state numbers number to state mapping.
         */
        private int stateCounter = 0;

        /** The NFA states in the NFA built from rules across grammars in composite.
         *  Maps state number to NFAState object.
         *  This is a Vector instead of a List because I need to be able to grow
         *  this properly.  After talking to Josh Bloch, Collections guy at Sun,
         *  I decided this was easiest solution.
         */
        private readonly List<NFAState> numberToStateList = new List<NFAState>( 1000 );

        /** Token names and literal tokens like "void" are uniquely indexed.
         *  with -1 implying EOF.  Characters are different; they go from
         *  -1 (EOF) to \uFFFE.  For example, 0 could be a binary byte you
         *  want to lexer.  Labels of DFA/NFA transitions can be both tokens
         *  and characters.  I use negative numbers for bookkeeping labels
         *  like EPSILON. Char/String literals and token types overlap in the same
         *  space, however.
         */
        private int maxTokenType = Label.MIN_TOKEN_TYPE - 1;

        /** Map token like ID (but not literals like "while") to its token type */
        private readonly Dictionary<string, int> tokenIDToTypeMap = new Dictionary<string, int>();

        /** Map token literals like "while" to its token type.  It may be that
         *  WHILE="while"=35, in which case both tokenIDToTypeMap and this
         *  field will have entries both mapped to 35.
         */
        private readonly Dictionary<string, int> stringLiteralToTypeMap = new Dictionary<string, int>();

        /** Reverse index for stringLiteralToTypeMap */
        private readonly List<string> typeToStringLiteralList = new List<string>();

        /** Map a token type to its token name.
         *  Must subtract MIN_TOKEN_TYPE from index.
         */
        private readonly List<string> typeToTokenList = new List<string>();

        /** If combined or lexer grammar, track the rules.
         * 	Track lexer rules so we can warn about undefined tokens.
         *  This is combined set of lexer rules from all lexer grammars
         *  seen in all imports.
         */
        private readonly HashSet<string> lexerRules = new HashSet<string>();

        /** Rules are uniquely labeled from 1..n among all grammars */
        private int ruleIndex = MinRuleIndex;

        /** Map a rule index to its name; use a Vector on purpose as new
         *  collections stuff won't let me setSize and make it grow.  :(
         *  I need a specific guaranteed index, which the Collections stuff
         *  won't let me have.
         */
        private readonly List<Rule> ruleIndexToRuleList = new List<Rule>();

        private bool watchNFAConversion = false;

        protected virtual void InitTokenSymbolTables()
        {
            // the faux token types take first NUM_FAUX_LABELS positions
            // then we must have room for the predefined runtime token types
            // like DOWN/UP used for tree parsing.
            typeToTokenList.Resize( Label.NUM_FAUX_LABELS + Label.MIN_TOKEN_TYPE - 1 );
            typeToTokenList[Label.NUM_FAUX_LABELS + Label.INVALID] = "<INVALID>";
            typeToTokenList[Label.NUM_FAUX_LABELS + Label.EOT] = "<EOT>";
            typeToTokenList[Label.NUM_FAUX_LABELS + Label.SEMPRED] = "<SEMPRED>";
            typeToTokenList[Label.NUM_FAUX_LABELS + Label.SET] = "<SET>";
            typeToTokenList[Label.NUM_FAUX_LABELS + Label.EPSILON] = Label.EPSILON_STR;
            typeToTokenList[Label.NUM_FAUX_LABELS + Label.EOF] = "EOF";
            typeToTokenList[Label.NUM_FAUX_LABELS + Label.EOR_TOKEN_TYPE - 1] = "<EOR>";
            typeToTokenList[Label.NUM_FAUX_LABELS + Label.DOWN - 1] = "DOWN";
            typeToTokenList[Label.NUM_FAUX_LABELS + Label.UP - 1] = "UP";
            tokenIDToTypeMap["<INVALID>"] = Label.INVALID;
            tokenIDToTypeMap["<EOT>"] = Label.EOT;
            tokenIDToTypeMap["<SEMPRED>"] = Label.SEMPRED;
            tokenIDToTypeMap["<SET>"] = Label.SET;
            tokenIDToTypeMap["<EPSILON>"] = Label.EPSILON;
            tokenIDToTypeMap["EOF"] = Label.EOF;
            tokenIDToTypeMap["<EOR>"] = Label.EOR_TOKEN_TYPE;
            tokenIDToTypeMap["DOWN"] = Label.DOWN;
            tokenIDToTypeMap["UP"] = Label.UP;
        }

        public CompositeGrammar()
        {
            InitTokenSymbolTables();
        }

        public CompositeGrammar( Grammar g )
            : this()
        {
            SetDelegationRoot( g );
        }

        #region Properties

        public CompositeGrammarTree DelegateGrammarTreeRoot
        {
            get
            {
                return delegateGrammarTreeRoot;
            }
        }

        public Grammar RootGrammar
        {
            get
            {
                if (delegateGrammarTreeRoot == null)
                    return null;

                return delegateGrammarTreeRoot.Grammar;
            }
        }

        public int StateCounter
        {
            get
            {
                return stateCounter;
            }
        }

        public int MaxTokenType
        {
            get
            {
                return maxTokenType;
            }

            set
            {
                maxTokenType = value;
            }
        }

        public IDictionary<string, int> TokenIDToTypeMap
        {
            get
            {
                return tokenIDToTypeMap;
            }
        }

        public IDictionary<string, int> StringLiteralToTypeMap
        {
            get
            {
                return stringLiteralToTypeMap;
            }
        }

        public List<string> TypeToStringLiteralList
        {
            get
            {
                return typeToStringLiteralList;
            }
        }

        public List<string> TypeToTokenList
        {
            get
            {
                return typeToTokenList;
            }
        }

        public ICollection<string> LexerRules
        {
            get
            {
                return lexerRules;
            }
        }

        public List<Rule> RuleIndexToRuleList
        {
            get
            {
                return ruleIndexToRuleList;
            }
        }

        public bool WatchNFAConversion
        {
            get
            {
                return watchNFAConversion;
            }

            set
            {
                watchNFAConversion = value;
            }
        }

        protected internal int RuleIndex
        {
            get
            {
                return ruleIndex;
            }

            set
            {
                ruleIndex = value;
            }
        }

        #endregion

        public virtual void SetDelegationRoot( Grammar root )
        {
            delegateGrammarTreeRoot = new CompositeGrammarTree( root );
            root.compositeTreeNode = delegateGrammarTreeRoot;
        }

        public virtual Rule GetRule( string ruleName )
        {
            return delegateGrammarTreeRoot.GetRule( ruleName );
        }

        public virtual object GetOption( string key )
        {
            return delegateGrammarTreeRoot.GetOption( key );
        }

        /** Add delegate grammar as child of delegator */
#if WTF
        public void AddGrammar( Grammar delegator, Grammar @delegate )
        {
            if ( delegator.compositeTreeNode == null )
            {
                delegator.compositeTreeNode = new CompositeGrammarTree( delegator );
            }
            delegator.compositeTreeNode.addChild( new CompositeGrammarTree( @delegate ) );

            //// find delegator in tree so we can add a child to it
            //CompositeGrammarTree t = delegateGrammarTreeRoot.findNode(delegator);
            //t.addChild();

            // make sure new grammar shares this composite
            @delegate.composite = this;
        }
#else
        public virtual void AddGrammar( Grammar delegator, Grammar @delegate )
        {
            throw new System.NotImplementedException();
        }
#endif

        /** Get parent of this grammar */
        public virtual Grammar GetDelegator( Grammar g )
        {
            CompositeGrammarTree me = delegateGrammarTreeRoot.FindNode( g );
            if ( me == null )
            {
                return null; // not found
            }

            if ( me.Parent != null )
            {
                return me.Parent.Grammar;
            }

            return null;
        }

        /** Get list of all delegates from all grammars in the delegate subtree of g.
         *  The grammars are in delegation tree preorder.  Don't include g itself
         *  in list as it is not a delegate of itself.
         */
        public virtual IList<Grammar> GetDelegates( Grammar g )
        {
            CompositeGrammarTree t = delegateGrammarTreeRoot.FindNode( g );
            if ( t == null )
            {
                return null; // no delegates
            }
            IList<Grammar> grammars = t.GetPostOrderedGrammarList();
            grammars.RemoveAt( grammars.Count - 1 ); // remove g (last one)
            return grammars;
        }

        public virtual IList<Grammar> GetDirectDelegates( Grammar g )
        {
            CompositeGrammarTree t = delegateGrammarTreeRoot.FindNode( g );
            IList<CompositeGrammarTree> children = t.children;
            if ( children == null )
            {
                return null;
            }
            IList<Grammar> grammars = new List<Grammar>();
            for ( int i = 0; children != null && i < children.Count; i++ )
            {
                CompositeGrammarTree child = (CompositeGrammarTree)children[i];
                grammars.Add( child.Grammar );
            }
            return grammars;
        }

        /** Get delegates below direct delegates of g */
        public virtual IList<Grammar> GetIndirectDelegates( Grammar g )
        {
            //IList<Grammar> direct = getDirectDelegates( g );
            //IList<Grammar> delegates = getDelegates( g );
            //delegates.removeAll( direct );
            //return delegates;
            return GetDelegates( g )
                .Except( GetDirectDelegates( g ) ?? Enumerable.Empty<Grammar>() )
                .ToArray();
        }

        /** Return list of delegate grammars from root down to g.
         *  Order is root, ..., g.parent.  (g not included).
         */
        public virtual IList<Grammar> GetDelegators( Grammar g )
        {
            if ( g == delegateGrammarTreeRoot.Grammar )
            {
                return null;
            }
            List<Grammar> grammars = new List<Grammar>();
            CompositeGrammarTree t = delegateGrammarTreeRoot.FindNode( g );
            // walk backwards to root, collecting grammars
            CompositeGrammarTree p = t.Parent;
            while ( p != null )
            {
                grammars.Insert( 0, p.Grammar ); // add to head so in order later
                p = p.Parent;
            }
            return grammars;
        }

        /** Get set of rules for grammar g that need to have manual delegation
         *  methods.  This is the list of rules collected from all direct/indirect
         *  delegates minus rules overridden in grammar g.
         *
         *  This returns null except for the delegate root because it is the only
         *  one that has to have a complete grammar rule interface.  The delegates
         *  should not be instantiated directly for use as parsers (you can create
         *  them to pass to the root parser's ctor as arguments).
         */
        public virtual HashSet<Rule> GetDelegatedRules( Grammar g )
        {
            if ( g != delegateGrammarTreeRoot.Grammar )
            {
                return null;
            }

            HashSet<Rule> rules = GetAllImportedRules( g );
            foreach ( Rule r in rules.ToArray() )
            {
                Rule localRule = g.GetLocallyDefinedRule( r.Name );
                // if locally defined or it's not local but synpred, don't make a delegation method
                if ( localRule != null || r.IsSynPred )
                {
                    // kill overridden rules
                    rules.Remove( r );
                }
            }
            return rules;
        }

        /** Get all rule definitions from all direct/indirect delegate grammars
         *  of g.
         */
        public virtual HashSet<Rule> GetAllImportedRules( Grammar g )
        {
            HashSet<string> ruleNames = new HashSet<string>();
            HashSet<Rule> rules = new HashSet<Rule>();
            CompositeGrammarTree subtreeRoot = delegateGrammarTreeRoot.FindNode( g );
            IList<Grammar> grammars = subtreeRoot.GetPreOrderedGrammarList();
            // walk all grammars preorder, priority given to grammar listed first.
            foreach ( Grammar grammar in grammars )
            {
                // for each rule in grammar, add to rules if no rule with that
                // name as been seen.  (can't use removeAll; wrong hashcode/equals on Rule)
                foreach ( Rule r in grammar.Rules )
                {
                    if ( !ruleNames.Contains( r.Name ) )
                    {
                        ruleNames.Add( r.Name ); // track that we've seen this
                        rules.Add( r );
                    }
                }
            }
            return rules;
        }

        public virtual Grammar GetGrammar( string grammarName )
        {
            CompositeGrammarTree t = delegateGrammarTreeRoot.FindNode( grammarName );
            if ( t != null )
            {
                return t.Grammar;
            }
            return null;
        }

        // NFA spans multiple grammars, must handle here

        public virtual int GetNewNFAStateNumber()
        {
            return stateCounter++;
        }

        public virtual void AddState( NFAState state )
        {
            numberToStateList.Resize( state.StateNumber + 1 ); // make sure we have room
            numberToStateList[state.StateNumber] = state;
        }

        public virtual NFAState GetState( int s )
        {
            return numberToStateList[s];
        }

        public virtual void AssignTokenTypes()
        {
            // ASSIGN TOKEN TYPES for all delegates (same walker)
            //System.Console.Out.WriteLine( "### assign types" );
            //ttypesWalker.setASTNodeClass( "org.antlr.tool.GrammarAST" );
            IList<Grammar> grammars = delegateGrammarTreeRoot.GetPostOrderedGrammarList();
            AssignTokenTypesWalker ttypesWalker = new AssignTokenTypesBehavior();
            for (int i = 0; grammars != null && i < grammars.Count; i++)
            {
                Grammar g = (Grammar)grammars[i];
                ttypesWalker.SetTreeNodeStream(new Antlr.Runtime.Tree.CommonTreeNodeStream(g.Tree));
                try
                {
                    //System.Console.Out.WriteLine( "    walking " + g.name );
                    ttypesWalker.grammar_( g );
                }
                catch ( RecognitionException re )
                {
                    ErrorManager.Error( ErrorManager.MSG_BAD_AST_STRUCTURE,
                                       re );
                }
            }

            // the walker has filled literals, tokens, and alias tables.
            // now tell it to define them in the root grammar
            ttypesWalker.DefineTokens(delegateGrammarTreeRoot.Grammar);
        }

        public virtual void TranslateLeftRecursiveRules()
        {
            IList<Grammar> grammars = delegateGrammarTreeRoot.GetPostOrderedGrammarList();
            for (int i = 0; grammars != null && i < grammars.Count; i++)
            {
                Grammar g = (Grammar)grammars[i];
                if (!(g.type == GrammarType.Parser || g.type == GrammarType.Combined))
                    continue;

                foreach (GrammarAST r in g.Tree.FindAllType(ANTLRParser.RULE))
                {
                    if (Rule.GetRuleType(r.GetChild(0).Text) == RuleType.Parser)
                        g.TranslateLeftRecursiveRule(r);
                }
            }
        }

        public virtual void DefineGrammarSymbols()
        {
            delegateGrammarTreeRoot.TrimLexerImportsIntoCombined();
            IList<Grammar> grammars = delegateGrammarTreeRoot.GetPostOrderedGrammarList();
            for ( int i = 0; grammars != null && i < grammars.Count; i++ )
            {
                Grammar g = (Grammar)grammars[i];
                g.DefineGrammarSymbols();
            }
            for ( int i = 0; grammars != null && i < grammars.Count; i++ )
            {
                Grammar g = (Grammar)grammars[i];
                g.CheckNameSpaceAndActions();
            }
            MinimizeRuleSet();
        }

        public virtual void CreateNFAs()
        {
            if ( ErrorManager.DoNotAttemptAnalysis() )
                return;

            IList<Grammar> grammars = delegateGrammarTreeRoot.GetPostOrderedGrammarList();
            //System.Console.Out.WriteLine( "### createNFAs for composite; grammars: " + names );
            for ( int i = 0; grammars != null && i < grammars.Count; i++ )
            {
                Grammar g = (Grammar)grammars[i];
                g.CreateRuleStartAndStopNFAStates();
            }

            for ( int i = 0; grammars != null && i < grammars.Count; i++ )
            {
                Grammar g = (Grammar)grammars[i];
                g.BuildNFA();
            }
        }

        public void MinimizeRuleSet()
        {
            HashSet<string> ruleDefs = new HashSet<string>();
            MinimizeRuleSetCore( ruleDefs, delegateGrammarTreeRoot );
        }

        protected virtual void MinimizeRuleSetCore( HashSet<string> ruleDefs,
                                     CompositeGrammarTree p )
        {
            HashSet<string> localRuleDefs = new HashSet<string>();
            HashSet<string> overrides = new HashSet<string>();
            // compute set of non-overridden rules for this delegate
            foreach ( Rule r in p.Grammar.Rules )
            {
                if ( !ruleDefs.Contains( r.Name ) )
                {
                    localRuleDefs.Add( r.Name );
                }
                else if ( !r.Name.Equals( Grammar.ArtificialTokensRuleName ) )
                {
                    // record any overridden rule 'cept tokens rule
                    overrides.Add( r.Name );
                }
            }
            //System.Console.Out.WriteLine( "rule defs for " + p.grammar.name + ": " + localRuleDefs );
            //System.Console.Out.WriteLine( "overridden rule for " + p.grammar.name + ": " + overrides );
            p.Grammar.overriddenRules = overrides;

            // make set of all rules defined thus far walking delegation tree.
            // the same rule in two delegates resolves in favor of first found
            // in tree therefore second must not be included
            ruleDefs.UnionWith( localRuleDefs );

            // pass larger set of defined rules to delegates
            if ( p.children != null )
            {
                foreach ( CompositeGrammarTree @delegate in p.children )
                {
                    MinimizeRuleSetCore( ruleDefs, @delegate );
                }
            }
        }

#if false
        public virtual void minimizeRuleSet()
        {
            var refs = _minimizeRuleSet( delegateGrammarTreeRoot );
            System.Console.Out.WriteLine( "all rule refs: " + refs );
        }

        public virtual HashSet<Rule> _minimizeRuleSet( CompositeGrammarTree p )
        {
            var refs = new HashSet<Rule>();
            foreach ( GrammarAST refAST in p.grammar.ruleRefs )
            {
                System.Console.Out.WriteLine( "ref " + refAST.Text + ": " + refAST.NFAStartState +
                                   " enclosing rule: " + refAST.NFAStartState.enclosingRule +
                                   " invoking rule: " + ( (NFAState)refAST.NFAStartState.transition[0].target ).enclosingRule );
                refs.Add( ( (NFAState)refAST.NFAStartState.transition[0].target ).enclosingRule );
            }

            if ( p.children != null )
            {
                foreach ( CompositeGrammarTree @delegate in p.children )
                {
                    var delegateRuleRefs = _minimizeRuleSet( @delegate );
                    refs.addAll( delegateRuleRefs );
                }
            }

            return refs;
        }
#endif

#if false
        public virtual void oldminimizeRuleSet()
        {
            // first walk to remove all overridden rules
            var ruleDefs = new HashSet<string>();
            var ruleRefs = new HashSet<string>();
            foreach ( GrammarAST refAST in delegateGrammarTreeRoot.grammar.ruleRefs )
            {
                string rname = refAST.Text;
                ruleRefs.add( rname );
            }
            _minimizeRuleSet( ruleDefs,
                             ruleRefs,
                             delegateGrammarTreeRoot );
            System.Console.Out.WriteLine( "overall rule defs: " + ruleDefs );
        }

        public virtual void _minimizeRuleSet( HashSet<string> ruleDefs, HashSet<string> ruleRefs, CompositeGrammarTree p )
        {
            var localRuleDefs = new HashSet<string>();
            foreach ( Rule r in p.grammar.Rules )
            {
                if ( !ruleDefs.contains( r.name ) )
                {
                    localRuleDefs.add( r.name );
                    ruleDefs.add( r.name );
                }
            }
            System.Console.Out.WriteLine( "rule defs for " + p.grammar.name + ": " + localRuleDefs );

            // remove locally-defined rules not in ref set
            // find intersection of local rules and references from delegator
            // that is set of rules needed by delegator
            HashSet<string> localRuleDefsSatisfyingRefsFromBelow = new HashSet<string>();
            foreach ( string r in ruleRefs )
            {
                if ( localRuleDefs.contains( r ) )
                {
                    localRuleDefsSatisfyingRefsFromBelow.add( r );
                }
            }

            // now get list of refs from localRuleDefsSatisfyingRefsFromBelow.
            // Those rules are also allowed in this delegate
            foreach ( GrammarAST refAST in p.grammar.ruleRefs )
            {
                if ( localRuleDefsSatisfyingRefsFromBelow.contains( refAST.enclosingRuleName ) )
                {
                    // found rule ref within needed rule
                }
            }

            // remove rule refs not in the new rule def set

            // walk all children, adding rules not already defined
            if ( p.children != null )
            {
                foreach ( CompositeGrammarTree @delegate in p.children )
                {
                    _minimizeRuleSet( ruleDefs, ruleRefs, @delegate );
                }
            }
        }
#endif

#if false
        public virtual void trackNFAStatesThatHaveLabeledEdge( Label label, NFAState stateWithLabeledEdge )
        {
            HashSet<NFAState> states = typeToNFAStatesWithEdgeOfTypeMap.get( label );
            if ( states == null )
            {
                states = new HashSet<NFAState>();
                typeToNFAStatesWithEdgeOfTypeMap[label] = states;
            }
            states.Add( stateWithLabeledEdge );
        }

        public virtual IDictionary<Label, HashSet<NFAState>> getTypeToNFAStatesWithEdgeOfTypeMap()
        {
            return typeToNFAStatesWithEdgeOfTypeMap;
        }

        public HashSet<NFAState> getStatesWithEdge( Label label )
        {
            return typeToNFAStatesWithEdgeOfTypeMap.get( label );
        }
#endif
    }
}
