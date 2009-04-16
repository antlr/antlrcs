/*
 * [The "BSD licence"]
 * Copyright (c) 2005-2008 Terence Parr
 * All rights reserved.
 *
 * Grammar conversion to ANTLR v3 and C#:
 * Copyright (c) 2008 Sam Harwell, Pixel Mine, Inc.
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions
 * are met:
 * 1. Redistributions of source code must retain the above copyright
 *	notice, this list of conditions and the following disclaimer.
 * 2. Redistributions in binary form must reproduce the above copyright
 *	notice, this list of conditions and the following disclaimer in the
 *	documentation and/or other materials provided with the distribution.
 * 3. The name of the author may not be used to endorse or promote products
 *	derived from this software without specific prior written permission.
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

namespace Antlr3.Grammars
{
    using System.Collections.Generic;
    using Antlr.Runtime;
    using Antlr.Runtime.JavaExtensions;
    using Antlr3.Analysis;
    using Antlr3.Tool;

    using ArrayList = System.Collections.Generic.List<object>;
    using CommonTreeNodeStream = Antlr.Runtime.Tree.CommonTreeNodeStream;
    using IIntSet = Antlr3.Misc.IIntSet;
    using IList = System.Collections.IList;
    using ITreeNodeStream = Antlr.Runtime.Tree.ITreeNodeStream;

    partial class TreeToNFAConverter
    {
        /** Factory used to create nodes and submachines */
        protected NFAFactory factory = null;

        /** Which NFA object are we filling in? */
        protected NFA nfa = null;

        /** Which grammar are we converting an NFA for? */
        protected Grammar grammar = null;

        protected string currentRuleName = null;

        protected int outerAltNum = 0;
        protected int blockLevel = 0;

        protected int inTest = 0;

        public TreeToNFAConverter( Grammar g, NFA nfa, NFAFactory factory, ITreeNodeStream input )
            : this( input )
        {
            this.grammar = g;
            this.nfa = nfa;
            this.factory = factory;
        }

        public IIntSet setRule( GrammarAST t )
        {
            TreeToNFAConverter other = new TreeToNFAConverter( grammar, nfa, factory, new CommonTreeNodeStream( t ) );

            other.currentRuleName = currentRuleName;
            other.outerAltNum = outerAltNum;
            other.blockLevel = blockLevel;

            return other.setRule();
        }
        internal int testBlockAsSet( GrammarAST t )
        {
            Rule r = grammar.getLocallyDefinedRule( currentRuleName );
            if ( r.hasRewrite( outerAltNum ) )
                return -1;

            TreeToNFAConverter other = new TreeToNFAConverter( grammar, nfa, factory, new CommonTreeNodeStream( t ) );

            other.state.backtracking++;
            other.currentRuleName = currentRuleName;
            other.outerAltNum = outerAltNum;
            other.blockLevel = blockLevel;

            var result = other.testBlockAsSet();
            if ( other.state.failed )
                return -1;

            return result;
        }
        public int testSetRule( GrammarAST t )
        {
            TreeToNFAConverter other = new TreeToNFAConverter( grammar, nfa, factory, new CommonTreeNodeStream( t ) );

            other.state.backtracking++;
            other.currentRuleName = currentRuleName;
            other.outerAltNum = outerAltNum;
            other.blockLevel = blockLevel;

            var result = other.testSetRule();
            if ( other.state.failed )
                state.failed = true;

            return result;
        }

        protected virtual void addFollowTransition( string ruleName, NFAState following )
        {
            //System.Console.Out.WriteLine( "adding follow link to rule " + ruleName );
            // find last link in FOLLOW chain emanating from rule
            Rule r = grammar.getRule( ruleName );
            NFAState end = r.stopState;
            while ( end.GetTransition( 1 ) != null )
            {
                end = (NFAState)end.GetTransition( 1 ).target;
            }
            if ( end.GetTransition( 0 ) != null )
            {
                // already points to a following node
                // gotta add another node to keep edges to a max of 2
                NFAState n = factory.newState();
                Transition e = new Transition( Label.EPSILON, n );
                end.AddTransition( e );
                end = n;
            }
            Transition followEdge = new Transition( Label.EPSILON, following );
            end.AddTransition( followEdge );
        }

        protected virtual void finish()
        {
            int numEntryPoints = factory.build_EOFStates( grammar.Rules );
            if ( numEntryPoints == 0 )
            {
                ErrorManager.grammarWarning( ErrorManager.MSG_NO_GRAMMAR_START_RULE,
                                           grammar,
                                           null,
                                           grammar.name );
            }
        }

        public override void ReportError( RecognitionException ex )
        {
            if ( inTest > 0 )
                throw ex;

            IToken token = null;
            if ( ex is MismatchedTokenException )
            {
                token = ( (MismatchedTokenException)ex ).token;
            }
            else if ( ex is NoViableAltException )
            {
                token = ( (NoViableAltException)ex ).token;
            }
            ErrorManager.syntaxError(
                ErrorManager.MSG_SYNTAX_ERROR,
                grammar,
                token,
                "buildnfa: " + ex.ToString(),
                ex );
        }
    }
}
