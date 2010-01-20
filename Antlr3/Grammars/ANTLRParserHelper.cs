/*
 * [The "BSD licence"]
 * Copyright (c) 2005-2008 Terence Parr
 * All rights reserved.
 *
 * Grammar conversion to ANTLR v3 and C#:
 * Copyright (c) 2008-2009 Sam Harwell, Pixel Mine, Inc.
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
    using System;
    using Antlr.Runtime;
    using Antlr.Runtime.JavaExtensions;
    using Antlr.Runtime.Tree;

    using ErrorManager = Antlr3.Tool.ErrorManager;
    using Grammar = Antlr3.Tool.Grammar;
    using GrammarAST = Antlr3.Tool.GrammarAST;
    using GrammarType = Antlr3.Tool.GrammarType;
    using IToken = Antlr.Runtime.IToken;
    using RecognitionException = Antlr.Runtime.RecognitionException;

    partial class ANTLRParser
    {
        protected string currentRuleName = null;
        protected GrammarAST currentBlockAST = null;
        protected bool atTreeRoot; // are we matching a tree root in tree grammar?

        private class GrammarASTErrorNode : GrammarAST
        {
            public IIntStream input;
            public IToken start;
            public IToken stop;
            public RecognitionException trappedException;

            public GrammarASTErrorNode( ITokenStream input, IToken start, IToken stop,
                                   RecognitionException e )
            {
                //Console.Out.WriteLine( "start: " + start + ", stop: " + stop );
                if ( stop == null ||
                     ( stop.TokenIndex < start.TokenIndex &&
                      stop.Type != TokenTypes.EndOfFile ) )
                {
                    // sometimes resync does not consume a token (when LT(1) is
                    // in follow set.  So, stop will be 1 to left to start. adjust.
                    // Also handle case where start is the first token and no token
                    // is consumed during recovery; LT(-1) will return null.
                    stop = start;
                }
                this.input = input;
                this.start = start;
                this.stop = stop;
                this.trappedException = e;
            }

            #region Properties
            public override bool IsNil
            {
                get
                {
                    return false;
                }
            }

            public override string Text
            {
                get
                {
                    string badText = null;
                    if ( start is IToken )
                    {
                        int i = ( (IToken)start ).TokenIndex;
                        int j = ( (IToken)stop ).TokenIndex;
                        if ( ( (IToken)stop ).Type == TokenTypes.EndOfFile )
                        {
                            j = ( (ITokenStream)input ).Count;
                        }
                        badText = ( (ITokenStream)input ).ToString( i, j );
                    }
                    else if ( start is ITree )
                    {
                        badText = ( (ITreeNodeStream)input ).ToString( start, stop );
                    }
                    else
                    {
                        // people should subclass if they alter the tree type so this
                        // next one is for sure correct.
                        badText = "<unknown>";
                    }
                    return badText;
                }
                set
                {
                }
            }

            public override int Type
            {
                get
                {
                    return TokenTypes.Invalid;
                }
                set
                {
                }
            }
            #endregion

            public override string ToString()
            {
                if ( trappedException is MissingTokenException )
                {
                    return "<missing type: " +
                           ( (MissingTokenException)trappedException ).MissingType +
                           ">";
                }
                else if ( trappedException is UnwantedTokenException )
                {
                    return "<extraneous: " +
                           ( (UnwantedTokenException)trappedException ).UnexpectedToken +
                           ", resync=" + Text + ">";
                }
                else if ( trappedException is MismatchedTokenException )
                {
                    return "<mismatched token: " + trappedException.Token + ", resync=" + Text + ">";
                }
                else if ( trappedException is NoViableAltException )
                {
                    return "<unexpected: " + trappedException.Token +
                           ", resync=" + Text + ">";
                }
                return "<error: " + Text + ">";
            }
        }

        internal class grammar_Adaptor : CommonTreeAdaptor
        {
            ANTLRParser _outer;

            public grammar_Adaptor( ANTLRParser outer )
            {
                _outer = outer;
            }
            public override object Create( IToken payload )
            {
                GrammarAST t = new GrammarAST( payload );
                if ( _outer != null )
                    t.enclosingRuleName = _outer.currentRuleName;
                return t;
            }
            public override object ErrorNode( ITokenStream input, IToken start, IToken stop, RecognitionException e )
            {
                GrammarAST t = new GrammarASTErrorNode( input, start, stop, e );
                if ( _outer != null )
                    t.enclosingRuleName = _outer.currentRuleName;
                return t;
            }
        }

        public Grammar Grammar
        {
            get;
            set;
        }

        public GrammarType GrammarType
        {
            get;
            set;
        }

        public string FileName
        {
            get;
            set;
        }

        int LA( int i )
        {
            return input.LA( i );
        }

        IToken LT( int k )
        {
            return input.LT( k );
        }

        partial void InitializeTreeAdaptor()
        {
            TreeAdaptor = new grammar_Adaptor( this );
        }

        protected virtual GrammarAST SetToBlockWithSet( GrammarAST b )
        {
            /*
             * alt = ^(ALT["ALT"] {b} EOA["EOA"])
             * prefixWithSynpred( alt )
             * return ^(BLOCK["BLOCK"] {alt} EOB["<end-of-block>"])
             */
            GrammarAST alt = (GrammarAST)adaptor.Create( ALT, "ALT" );
            adaptor.AddChild( alt, b );
            adaptor.AddChild( alt, adaptor.Create( EOA, "<end-of-alt>" ) );

            PrefixWithSynPred( alt );

            GrammarAST block = (GrammarAST)adaptor.Create( BLOCK, b.Token, "BLOCK" );
            adaptor.AddChild( block, alt );
            adaptor.AddChild( alt, adaptor.Create( EOB, "<end-of-block>" ) );

            return block;
        }

        /** Create a copy of the alt and make it into a BLOCK; all actions,
         *  labels, tree operators, rewrites are removed.
         */
        protected virtual GrammarAST CreateBlockFromDupAlt( GrammarAST alt )
        {
            /*
             * ^(BLOCK["BLOCK"] {GrammarAST.dupTreeNoActions(alt)} EOB["<end-of-block>"])
             */
            GrammarAST nalt = GrammarAST.DupTreeNoActions( alt, null );

            GrammarAST block = (GrammarAST)adaptor.Create( BLOCK, alt.Token, "BLOCK" );
            adaptor.AddChild( block, nalt );
            adaptor.AddChild( block, adaptor.Create( EOB, "<end-of-block>" ) );

            return block;
        }

        /** Rewrite alt to have a synpred as first element;
         *  (xxx)=>xxx
         *  but only if they didn't specify one manually.
         */
        protected virtual void PrefixWithSynPred( GrammarAST alt )
        {
            // if they want backtracking and it's not a lexer rule in combined grammar
            string autoBacktrack = (string)Grammar.GetBlockOption( currentBlockAST, "backtrack" );
            if ( autoBacktrack == null )
            {
                autoBacktrack = (string)Grammar.GetOption( "backtrack" );
            }
            if ( autoBacktrack != null && autoBacktrack.Equals( "true" ) &&
                 !( GrammarType == GrammarType.Combined &&
                 char.IsUpper( currentRuleName[0] ) ) &&
                 alt.GetChild( 0 ).Type != SYN_SEMPRED )
            {
                // duplicate alt and make a synpred block around that dup'd alt
                GrammarAST synpredBlockAST = CreateBlockFromDupAlt( alt );

                // Create a BACKTRACK_SEMPRED node as if user had typed this in
                // Effectively we replace (xxx)=>xxx with {synpredxxx}? xxx
                GrammarAST synpredAST = CreateSynSemPredFromBlock( synpredBlockAST,
                                                                  BACKTRACK_SEMPRED );

                // insert BACKTRACK_SEMPRED as first element of alt
                //synpredAST.getLastSibling().setNextSibling( alt.getFirstChild() );
                //synpredAST.addChild( alt.getFirstChild() );
                //alt.setFirstChild( synpredAST );
                GrammarAST[] children = alt.GetChildrenAsArray();
                adaptor.SetChild( alt, 0, synpredAST );
                for ( int i = 0; i < children.Length; i++ )
                {
                    if ( i < children.Length - 1 )
                        adaptor.SetChild( alt, i + 1, children[i] );
                    else
                        adaptor.AddChild( alt, children[i] );
                }
            }
        }

        protected virtual GrammarAST CreateSynSemPredFromBlock( GrammarAST synpredBlockAST, int synpredTokenType )
        {
            // add grammar fragment to a list so we can make fake rules for them later.
            string predName = Grammar.DefineSyntacticPredicate( synpredBlockAST, currentRuleName );
            // convert (alpha)=> into {synpredN}? where N is some pred count
            // during code gen we convert to function call with templates
            string synpredinvoke = predName;
            GrammarAST p = (GrammarAST)adaptor.Create( synpredTokenType, synpredinvoke );
            // track how many decisions have synpreds
            Grammar.blocksWithSynPreds.Add( currentBlockAST );
            return p;
        }

        public virtual GrammarAST CreateSimpleRuleAST( string name, GrammarAST block, bool fragment )
        {
            GrammarAST modifier = null;
            if ( fragment )
            {
                modifier = (GrammarAST)adaptor.Create( FRAGMENT, "fragment" );
            }

            /*
             * EOBAST = block.getLastChild()
             * ^(RULE[block,"rule"] ID["name"] {modifier} ARG["ARG"] RET["RET"] SCOPE["scope"] {block} EOR[EOBAST,"<end-of-rule>"])
             */
            GrammarAST rule = (GrammarAST)adaptor.Create( RULE, block.Token, "rule" );

            adaptor.AddChild( rule, adaptor.Create( ID, name ) );
            if ( modifier != null )
                adaptor.AddChild( rule, modifier );
            adaptor.AddChild( rule, adaptor.Create( ARG, "ARG" ) );
            adaptor.AddChild( rule, adaptor.Create( RET, "RET" ) );
            adaptor.AddChild( rule, adaptor.Create( SCOPE, "scope" ) );
            adaptor.AddChild( rule, block );
            adaptor.AddChild( rule, adaptor.Create( EOR, block.LastChild.Token, "<end-of-rule>" ) );

            return rule;
        }

        public override void ReportError( RecognitionException ex )
        {
            //Token token = null;
            //try
            //{
            //    token = LT( 1 );
            //}
            //catch ( TokenStreamException tse )
            //{
            //    ErrorManager.internalError( "can't get token???", tse );
            //}
            IToken token = ex.Token;
            ErrorManager.SyntaxError(
                ErrorManager.MSG_SYNTAX_ERROR,
                Grammar,
                token,
                "antlr: " + ex.ToString(),
                ex );
        }

        public virtual void Cleanup( GrammarAST root )
        {
            if ( GrammarType == GrammarType.Lexer )
            {
                string filter = (string)Grammar.GetOption( "filter" );
                GrammarAST tokensRuleAST =
                    Grammar.AddArtificialMatchTokensRule(
                        root,
                        Grammar.lexerRuleNamesInCombined,
                        Grammar.GetDelegateNames(),
                        filter != null && filter.Equals( "true" ) );
            }
        }
    }
}
