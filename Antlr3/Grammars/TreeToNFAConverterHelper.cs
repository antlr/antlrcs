/*
 * [The "BSD license"]
 * Copyright (c) 2011 Terence Parr
 * All rights reserved.
 *
 * Grammar conversion to ANTLR v3 and C#:
 * Copyright (c) 2011 Sam Harwell, Pixel Mine, Inc.
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
    using Antlr.Runtime;
    using Antlr3.Analysis;
    using Antlr3.Tool;

    using ArgumentNullException = System.ArgumentNullException;
    using CommonTreeNodeStream = Antlr.Runtime.Tree.CommonTreeNodeStream;
    using IIntSet = Antlr3.Misc.IIntSet;
    using IntervalSet = Antlr3.Misc.IntervalSet;
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

        public IIntSet SetRule( GrammarAST t )
        {
            TreeToNFAConverter other = new TreeToNFAConverter( grammar, nfa, factory, new CommonTreeNodeStream( t ) );

            other.currentRuleName = currentRuleName;
            other.outerAltNum = outerAltNum;
            other.blockLevel = blockLevel;

            return other.setRule();
        }
        internal int TestBlockAsSet( GrammarAST t )
        {
            Rule r = grammar.GetLocallyDefinedRule( currentRuleName );
            if ( r.HasRewrite( outerAltNum ) )
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
        public int TestSetRule( GrammarAST t )
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

        protected virtual void AddFollowTransition( string ruleName, NFAState following )
        {
            //System.Console.Out.WriteLine( "adding follow link to rule " + ruleName );
            // find last link in FOLLOW chain emanating from rule
            Rule r = grammar.GetRule( ruleName );
            NFAState end = r.StopState;
            while ( end.GetTransition( 1 ) != null )
            {
                end = (NFAState)end.GetTransition( 1 ).Target;
            }
            if ( end.GetTransition( 0 ) != null )
            {
                // already points to a following node
                // gotta add another node to keep edges to a max of 2
                NFAState n = factory.NewState();
                Transition e = new Transition( Label.EPSILON, n );
                end.AddTransition( e );
                end = n;
            }
            Transition followEdge = new Transition( Label.EPSILON, following );
            end.AddTransition( followEdge );
        }

        protected virtual void Finish()
        {
            int numEntryPoints = factory.BuildEofStates( grammar.Rules );
            if ( numEntryPoints == 0 )
            {
                ErrorManager.GrammarWarning( ErrorManager.MSG_NO_GRAMMAR_START_RULE,
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
                token = ( (MismatchedTokenException)ex ).Token;
            }
            else if ( ex is NoViableAltException )
            {
                token = ( (NoViableAltException)ex ).Token;
            }
            ErrorManager.SyntaxError(
                ErrorManager.MSG_SYNTAX_ERROR,
                grammar,
                token,
                "buildnfa: " + ex.ToString(),
                ex );
        }

        private bool HasElementOptions(GrammarAST node)
        {
            if (node == null)
                throw new ArgumentNullException("node");

            return node.terminalOptions != null && node.terminalOptions.Count > 0;
        }

        private void HandleRuleId(GrammarAST id)
        {
            currentRuleName = id.Text;
            factory.CurrentRule = grammar.GetLocallyDefinedRule(currentRuleName);
        }

        private void HandleRule(GrammarAST start, StateCluster g, GrammarAST blockStart, GrammarAST id)
        {
            if (blockStart.SetValue != null)
            {
                // if block comes back as a set not BLOCK, make it
                // a single ALT block
                g = factory.BuildAlternativeBlockFromSet(g);
            }

            if (Rule.GetRuleType(currentRuleName) == RuleType.Parser || grammar.type == GrammarType.Lexer)
            {
                // attach start node to block for this rule
                Rule thisR = grammar.GetLocallyDefinedRule(currentRuleName);
                NFAState start2 = thisR.StartState;
                start2.associatedASTNode = id;
                start2.AddTransition(new Transition(Label.EPSILON, g.Left));

                // track decision if > 1 alts
                if (grammar.GetNumberOfAltsForDecisionNFA(g.Left) > 1)
                {
                    g.Left.Description = grammar.GrammarTreeToString(start, false);
                    g.Left.SetDecisionASTNode(blockStart);
                    int d = grammar.AssignDecisionNumber(g.Left);
                    grammar.SetDecisionNFA(d, g.Left);
                    grammar.SetDecisionBlockAST(d, blockStart);
                }

                // hook to end of rule node
                NFAState end = thisR.StopState;
                g.Right.AddTransition(new Transition(Label.EPSILON, end));
            }
        }

        private StateCluster HandleAlternativeElement(StateCluster g, StateCluster element)
        {
            return factory.BuildAB(g, element);
        }

        private StateCluster HandleAlternativeEnd(StateCluster g)
        {
            if (g == null)
            {
                // if alt was a list of actions or whatever
                g = factory.BuildEpsilon();
            }
            else
            {
                factory.OptimizeAlternative(g);
            }

            return g;
        }

        private StateCluster HandleEbnfSet(StateCluster g)
        {
            return g;
        }

        private StateCluster HandleEbnfBlock(GrammarAST blk, StateCluster g)
        {
            // track decision if > 1 alts
            if (grammar.GetNumberOfAltsForDecisionNFA(g.Left) > 1)
            {
                g.Left.Description = grammar.GrammarTreeToString(blk, false);
                g.Left.SetDecisionASTNode(blk);
                int d = grammar.AssignDecisionNumber(g.Left);
                grammar.SetDecisionNFA(d, g.Left);
                grammar.SetDecisionBlockAST(d, blk);
            }

            return g;
        }

        private StateCluster HandleEbnfOptionalBlock(GrammarAST start, GrammarAST blk, StateCluster bg)
        {
            if (blk.SetValue != null)
            {
                // if block comes back SET not BLOCK, make it
                // a single ALT block
                bg = factory.BuildAlternativeBlockFromSet(bg);
            }

            StateCluster g = factory.BuildAoptional(bg);
            g.Left.Description = grammar.GrammarTreeToString(start, false);
            // there is always at least one alt even if block has just 1 alt
            int d = grammar.AssignDecisionNumber(g.Left);
            grammar.SetDecisionNFA(d, g.Left);
            grammar.SetDecisionBlockAST(d, blk);
            g.Left.SetDecisionASTNode(start);
            return g;
        }

        private StateCluster HandleEbnfClosureBlock(GrammarAST start, GrammarAST blk, StateCluster bg)
        {
            GrammarAST eob = blk.LastChild;
            if (blk.SetValue != null)
            {
                bg = factory.BuildAlternativeBlockFromSet(bg);
            }

            StateCluster g = factory.BuildAstar(bg);
            // track the loop back / exit decision point
            bg.Right.Description = "()* loopback of " + grammar.GrammarTreeToString(start, false);
            int d = grammar.AssignDecisionNumber(bg.Right);
            grammar.SetDecisionNFA(d, bg.Right);
            grammar.SetDecisionBlockAST(d, blk);
            bg.Right.SetDecisionASTNode(eob);
            // make block entry state also have same decision for interpreting grammar
            NFAState altBlockState = (NFAState)g.Left.GetTransition(0).Target;
            altBlockState.SetDecisionASTNode(start);
            altBlockState.DecisionNumber = d;
            g.Left.DecisionNumber = d; // this is the bypass decision (2 alts)
            g.Left.SetDecisionASTNode(start);
            return g;
        }

        private StateCluster HandleEbnfPositiveClosureBlock(GrammarAST start, GrammarAST blk, StateCluster bg)
        {
            GrammarAST eob = blk.LastChild;
            if (blk.SetValue != null)
            {
                bg = factory.BuildAlternativeBlockFromSet(bg);
            }

            StateCluster g = factory.BuildAplus(bg);
            // don't make a decision on left edge, can reuse loop end decision
            // track the loop back / exit decision point
            bg.Right.Description = "()+ loopback of " + grammar.GrammarTreeToString(start, false);
            int d = grammar.AssignDecisionNumber(bg.Right);
            grammar.SetDecisionNFA(d, bg.Right);
            grammar.SetDecisionBlockAST(d, blk);
            bg.Right.SetDecisionASTNode(eob);
            // make block entry state also have same decision for interpreting grammar
            NFAState altBlockState = (NFAState)g.Left.GetTransition(0).Target;
            altBlockState.SetDecisionASTNode(start);
            altBlockState.DecisionNumber = d;
            return g;
        }

        private StateCluster HandleTreeFirstElement(GrammarAST firstElementStart, StateCluster element, out StateCluster down)
        {
            down = factory.BuildAtom(Label.DOWN, firstElementStart);
            // TODO set following states for imaginary nodes?
            //el.followingNFAState = down.Right;
            return factory.BuildAB(element, down);
        }

        private StateCluster HandleTreeElement(StateCluster g, StateCluster element)
        {
            return factory.BuildAB(g, element);
        }

        private StateCluster HandleTreeAfterLastElement(GrammarAST start, StateCluster g, GrammarAST lastElementStart, StateCluster down)
        {
            StateCluster up = factory.BuildAtom(Label.UP, lastElementStart);
            //el.followingNFAState = up.Right;
            g = factory.BuildAB(g, up);
            // tree roots point at right edge of DOWN for LOOK computation later
            start.NFATreeDownState = down.Left;

            return g;
        }

        private StateCluster HandleNotAtomCharLiteral(GrammarAST notNode, GrammarAST charLiteral)
        {
            int ttype = 0;
            if (grammar.type == GrammarType.Lexer)
            {
                ttype = Grammar.GetCharValueFromGrammarCharLiteral(charLiteral.Text);
            }
            else
            {
                ttype = grammar.GetTokenType(charLiteral.Text);
            }

            IIntSet notAtom = grammar.Complement(ttype);
            if (notAtom.IsNil)
            {
                ErrorManager.GrammarError(
                    ErrorManager.MSG_EMPTY_COMPLEMENT,
                    grammar,
                    charLiteral.Token,
                    charLiteral.Text);
            }

            return factory.BuildSet(notAtom, notNode);
        }

        private StateCluster HandleNotAtomTokenReference(GrammarAST notNode, GrammarAST tokenReference)
        {
            int ttype = 0;
            IIntSet notAtom = null;
            if (grammar.type == GrammarType.Lexer)
            {
                notAtom = grammar.GetSetFromRule(this, tokenReference.Text);
                if (notAtom == null)
                {
                    ErrorManager.GrammarError(
                        ErrorManager.MSG_RULE_INVALID_SET,
                        grammar,
                        tokenReference.Token,
                        tokenReference.Text);
                }
                else
                {
                    notAtom = grammar.Complement(notAtom);
                }
            }
            else
            {
                ttype = grammar.GetTokenType(tokenReference.Text);
                notAtom = grammar.Complement(ttype);
            }

            if (notAtom == null || notAtom.IsNil)
            {
                ErrorManager.GrammarError(
                    ErrorManager.MSG_EMPTY_COMPLEMENT,
                    grammar,
                    tokenReference.Token,
                    tokenReference.Text);
            }

            return factory.BuildSet(notAtom, notNode);
        }

        private StateCluster HandleNotAtomSet(GrammarAST notNode, GrammarAST setNode)
        {
            //IIntSet notSet = grammar.Complement(stNode.SetValue);
            // let code generator complement the sets
            IIntSet s = setNode.SetValue;
            setNode.SetValue = s;
            // let code gen do the complement again; here we compute
            // for NFA construction
            s = grammar.Complement(s);
            if (s.IsNil)
            {
                ErrorManager.GrammarError(
                    ErrorManager.MSG_EMPTY_COMPLEMENT,
                    grammar,
                    notNode.Token);
            }

            return factory.BuildSet(s, notNode);
        }

        private void HandleNotAtomEnd(GrammarAST notNode, StateCluster g)
        {
            notNode.followingNFAState = g.Right;
        }

        private StateCluster HandleAtomRuleReference(string scopeName, GrammarAST ruleReference)
        {
            NFAState start = grammar.GetRuleStartState(scopeName, ruleReference.Text);
            if (start != null)
            {
                Rule rr = grammar.GetRule(scopeName, ruleReference.Text);
                StateCluster g = factory.BuildRuleRef(rr, start);
                ruleReference.followingNFAState = g.Right;
                ruleReference._nfaStartState = g.Left;
                if (g.Left.GetTransition(0) is RuleClosureTransition && grammar.type != GrammarType.Lexer)
                {
                    AddFollowTransition(ruleReference.Text, g.Right);
                }
                // else rule ref got inlined to a set

                return g;
            }

            return null;
        }

        private StateCluster HandleAtomTokenReference(string scopeName, GrammarAST tokenReference)
        {
            if (grammar.type == GrammarType.Lexer)
            {
                NFAState start = grammar.GetRuleStartState(scopeName, tokenReference.Text);
                if (start != null)
                {
                    Rule rr = grammar.GetRule(scopeName, tokenReference.Text);
                    StateCluster g = factory.BuildRuleRef(rr, start);
                    tokenReference._nfaStartState = g.Left;
                    // don't add FOLLOW transitions in the lexer;
                    // only exact context should be used.
                    return g;
                }

                return null;
            }
            else
            {
                StateCluster g = factory.BuildAtom(tokenReference);
                tokenReference.followingNFAState = g.Right;
                return g;
            }
        }

        private StateCluster HandleAtomCharLiteral(GrammarAST charLiteral)
        {
            if (grammar.type == GrammarType.Lexer)
            {
                return factory.BuildCharLiteralAtom(charLiteral);
            }
            else
            {
                StateCluster g = factory.BuildAtom(charLiteral);
                charLiteral.followingNFAState = g.Right;
                return g;
            }
        }

        private StateCluster HandleAtomStringLiteral(GrammarAST stringLiteral)
        {
            if (grammar.type == GrammarType.Lexer)
            {
                return factory.BuildStringLiteralAtom(stringLiteral);
            }
            else
            {
                StateCluster g = factory.BuildAtom(stringLiteral);
                stringLiteral.followingNFAState = g.Right;
                return g;
            }
        }

        private StateCluster HandleAtomWildcard(GrammarAST wildcard)
        {
            if (nfa.Grammar.type == GrammarType.TreeParser
                && (wildcard.ChildIndex > 0 || wildcard.Parent.GetChild(1).Type == EOA))
            {
                return factory.BuildWildcardTree(wildcard);
            }
            else
            {
                return factory.BuildWildcard(wildcard);
            }
        }

        private void HandleSetElementCharLiteral(IIntSet elements, GrammarAST c)
        {
            int ttype;
            if (grammar.type == GrammarType.Lexer)
                ttype = Grammar.GetCharValueFromGrammarCharLiteral(c.Text);
            else
                ttype = grammar.GetTokenType(c.Text);

            if (elements.Contains(ttype))
                ErrorManager.GrammarError(ErrorManager.MSG_DUPLICATE_SET_ENTRY, grammar, c.Token, c.Text);

            elements.Add(ttype);
        }

        private void HandleSetElementTokenReference(IIntSet elements, GrammarAST t)
        {
            int ttype;
            if (grammar.type == GrammarType.Lexer)
            {
                // recursively will invoke this rule to match elements in target rule ref
                IIntSet ruleSet = grammar.GetSetFromRule(this, t.Text);
                if (ruleSet == null)
                    ErrorManager.GrammarError(ErrorManager.MSG_RULE_INVALID_SET, grammar, t.Token, t.Text);
                else
                    elements.AddAll(ruleSet);
            }
            else
            {
                ttype = grammar.GetTokenType(t.Text);
                if (elements.Contains(ttype))
                    ErrorManager.GrammarError(ErrorManager.MSG_DUPLICATE_SET_ENTRY, grammar, t.Token, t.Text);

                elements.Add(ttype);
            }
        }

        private void HandleSetElementStringLiteral(IIntSet elements, GrammarAST s)
        {
            int ttype = grammar.GetTokenType(s.Text);
            if (elements.Contains(ttype))
                ErrorManager.GrammarError(ErrorManager.MSG_DUPLICATE_SET_ENTRY, grammar, s.Token, s.Text);

            elements.Add(ttype);
        }

        private void HandleSetElementCharRange(IIntSet elements, GrammarAST c1, GrammarAST c2)
        {
            if (grammar.type == GrammarType.Lexer)
            {
                int a = Grammar.GetCharValueFromGrammarCharLiteral(c1.Text);
                int b = Grammar.GetCharValueFromGrammarCharLiteral(c2.Text);
                elements.AddAll(IntervalSet.Of(a, b));
            }
        }

        private void HandleSetElementSet(IIntSet elements, StateCluster g)
        {
            Transition setTrans = g.Left.GetTransition(0);
            elements.AddAll(setTrans.Label.Set);
        }

        private void HandleSetElementNotSetElement(IIntSet elements, IIntSet ns)
        {
            IIntSet not = grammar.Complement(ns);
            elements.AddAll(not);
        }
    }
}
