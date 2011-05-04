/*
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
    using Antlr3.Analysis;

    using IIntSet = Antlr3.Misc.IIntSet;
    using IntervalSet = Antlr3.Misc.IntervalSet;
    using StringBuilder = System.Text.StringBuilder;
    using StringComparison = System.StringComparison;

    /** Routines to construct StateClusters from EBNF grammar constructs.
     *  No optimization is done to remove unnecessary epsilon edges.
     *
     *  TODO: add an optimization that reduces number of states and transitions
     *  will help with speed of conversion and make it easier to view NFA.  For
     *  example, o-A->o-->o-B->o should be o-A->o-B->o
     */
    public class NFAFactory
    {
        /** This factory is attached to a specifc NFA that it is building.
         *  The NFA will be filled up with states and transitions.
         */
        private readonly NFA _nfa;

        private Rule _currentRule;

        public NFAFactory( NFA nfa )
        {
            this._nfa = nfa;
        }

        public Rule CurrentRule
        {
            get
            {
                return _currentRule;
            }

            set
            {
                _currentRule = value;
            }
        }

        public virtual NFAState NewState()
        {
            NFAState n = new NFAState( _nfa );
            int state = _nfa.GetNewNFAStateNumber();
            n.StateNumber = state;
            _nfa.AddState( n );
            n.enclosingRule = _currentRule;
            return n;
        }

        /** Optimize an alternative (list of grammar elements).
         *
         *  Walk the chain of elements (which can be complicated loop blocks...)
         *  and throw away any epsilon transitions used to link up simple elements.
         *
         *  This only removes 195 states from the java.g's NFA, but every little
         *  bit helps.  Perhaps I can improve in the future.
         */
        public virtual void OptimizeAlternative( StateCluster alt )
        {
            NFAState s = alt.Left;
            while ( s != alt.Right )
            {
                // if it's a block element, jump over it and continue
                if ( s.endOfBlockStateNumber != State.INVALID_STATE_NUMBER )
                {
                    s = _nfa.GetState( s.endOfBlockStateNumber );
                    continue;
                }
                Transition t = s.transition[0];
                if ( t is RuleClosureTransition )
                {
                    s = ( (RuleClosureTransition)t ).FollowState;
                    continue;
                }
                if ( t.Label.IsEpsilon && !t.Label.IsAction && s.NumberOfTransitions == 1 )
                {
                    // bypass epsilon transition and point to what the epsilon's
                    // target points to unless that epsilon transition points to
                    // a block or loop etc..  Also don't collapse epsilons that
                    // point at the last node of the alt. Don't collapse action edges
                    NFAState epsilonTarget = (NFAState)t.Target;
                    if ( epsilonTarget.endOfBlockStateNumber == State.INVALID_STATE_NUMBER &&
                         epsilonTarget.transition[0] != null )
                    {
                        s.SetTransition0( epsilonTarget.transition[0] );
                        //System.Console.Out.WriteLine( "### opt " + s.stateNumber + "->" + epsilonTarget.transition[0].target.stateNumber );
                    }
                }
                s = (NFAState)t.Target;
            }
        }

        /** From label A build Graph o-A->o */
        public virtual StateCluster BuildAtom( int label, GrammarAST associatedAST )
        {
            NFAState left = NewState();
            NFAState right = NewState();
            left.associatedASTNode = associatedAST;
            right.associatedASTNode = associatedAST;
            TransitionBetweenStates( left, right, label );
            StateCluster g = new StateCluster( left, right );
            return g;
        }

        public virtual StateCluster BuildAtom( GrammarAST atomAST )
        {
            int tokenType = _nfa.Grammar.GetTokenType( atomAST.Text );
            return BuildAtom( tokenType, atomAST );
        }

        /** From set build single edge graph o->o-set->o.  To conform to
         *  what an alt block looks like, must have extra state on left.
         */
        public virtual StateCluster BuildSet( IIntSet set, GrammarAST associatedAST )
        {
            NFAState left = NewState();
            NFAState right = NewState();
            left.associatedASTNode = associatedAST;
            right.associatedASTNode = associatedAST;
            Label label = new Label( set );
            Transition e = new Transition( label, right );
            left.AddTransition( e );
            StateCluster g = new StateCluster( left, right );
            return g;
        }

#if false
        /** Can only complement block of simple alts; can complement build_Set()
         *  result, that is.  Get set and complement, replace old with complement.
         */
        public StateCluster BuildAlternativeBlockComplement( StateCluster blk )
        {
            State s0 = blk.left;
            IIntSet set = getCollapsedBlockAsSet( s0 );
            if ( set != null )
            {
                // if set is available, then structure known and blk is a set
                set = nfa.grammar.complement( set );
                Label label = s0.getTransition( 0 ).target.getTransition( 0 ).label;
                label.Set = set;
            }
            return blk;
        }
#endif

        public virtual StateCluster BuildRange( int a, int b )
        {
            NFAState left = NewState();
            NFAState right = NewState();
            Label label = new Label( IntervalSet.Of( a, b ) );
            Transition e = new Transition( label, right );
            left.AddTransition( e );
            StateCluster g = new StateCluster( left, right );
            return g;
        }

        /** From char 'c' build StateCluster o-intValue(c)->o
         */
        public virtual StateCluster BuildCharLiteralAtom( GrammarAST charLiteralAST )
        {
            int c = Grammar.GetCharValueFromGrammarCharLiteral( charLiteralAST.Text );
            return BuildAtom( c, charLiteralAST );
        }

        /** From char 'c' build StateCluster o-intValue(c)->o
         *  can include unicode spec likes '\u0024' later.  Accepts
         *  actual unicode 16-bit now, of course, by default.
         *  TODO not supplemental char clean!
         */
        public virtual StateCluster BuildCharRange( string a, string b )
        {
            int from = Grammar.GetCharValueFromGrammarCharLiteral( a );
            int to = Grammar.GetCharValueFromGrammarCharLiteral( b );
            return BuildRange( from, to );
        }

        /** For a non-lexer, just build a simple token reference atom.
         *  For a lexer, a string is a sequence of char to match.  That is,
         *  "fog" is treated as 'f' 'o' 'g' not as a single transition in
         *  the DFA.  Machine== o-'f'->o-'o'->o-'g'->o and has n+1 states
         *  for n characters.
         */
        public virtual StateCluster BuildStringLiteralAtom( GrammarAST stringLiteralAST )
        {
            if ( _nfa.Grammar.type == GrammarType.Lexer )
            {
                StringBuilder chars =
                    Grammar.GetUnescapedStringFromGrammarStringLiteral( stringLiteralAST.Text );
                NFAState first = NewState();
                NFAState last = null;
                NFAState prev = first;
                for ( int i = 0; i < chars.Length; i++ )
                {
                    int c = chars[i];
                    NFAState next = NewState();
                    TransitionBetweenStates( prev, next, c );
                    prev = last = next;
                }
                return new StateCluster( first, last );
            }

            // a simple token reference in non-Lexers
            int tokenType = _nfa.Grammar.GetTokenType( stringLiteralAST.Text );
            return BuildAtom( tokenType, stringLiteralAST );
        }

        /** For reference to rule r, build
         *
         *  o-e->(r)  o
         *
         *  where (r) is the start of rule r and the trailing o is not linked
         *  to from rule ref state directly (it's done thru the transition(0)
         *  RuleClosureTransition.
         *
         *  If the rule r is just a list of tokens, it's block will be just
         *  a set on an edge o->o->o-set->o->o->o, could inline it rather than doing
         *  the rule reference, but i'm not doing this yet as I'm not sure
         *  it would help much in the NFA->DFA construction.
         *
         *  TODO add to codegen: collapse alt blks that are sets into single matchSet
         */
        public virtual StateCluster BuildRuleRef( Rule refDef, NFAState ruleStart )
        {
            //System.Console.Out.WriteLine( "building ref to rule " + nfa.grammar.name + "." + refDef.name );
            NFAState left = NewState();
            //left.Description = "ref to " + ruleStart.Description;
            NFAState right = NewState();
            //right.Description = "NFAState following ref to " + ruleStart.Description;
            Transition e = new RuleClosureTransition( refDef, ruleStart, right );
            left.AddTransition( e );
            StateCluster g = new StateCluster( left, right );
            return g;
        }

        /** From an empty alternative build StateCluster o-e->o */
        public virtual StateCluster BuildEpsilon()
        {
            NFAState left = NewState();
            NFAState right = NewState();
            TransitionBetweenStates( left, right, Label.EPSILON );
            StateCluster g = new StateCluster( left, right );
            return g;
        }

        /** Build what amounts to an epsilon transition with a semantic
         *  predicate action.  The pred is a pointer into the AST of
         *  the SEMPRED token.
         */
        public virtual StateCluster BuildSemanticPredicate( GrammarAST pred )
        {
            // don't count syn preds
            if ( !pred.Text.StartsWith( Grammar.SynpredRulePrefix, StringComparison.OrdinalIgnoreCase ) )
            {
                _nfa.Grammar.numberOfSemanticPredicates++;
            }
            NFAState left = NewState();
            NFAState right = NewState();
            Transition e = new Transition( new PredicateLabel( pred ), right );
            left.AddTransition( e );
            StateCluster g = new StateCluster( left, right );
            return g;
        }

        /** Build what amounts to an epsilon transition with an action.
         *  The action goes into NFA though it is ignored during analysis.
         *  It slows things down a bit, but I must ignore predicates after
         *  having seen an action (5-5-2008).
         */
        public virtual StateCluster BuildAction( GrammarAST action )
        {
            NFAState left = NewState();
            NFAState right = NewState();
            Transition e = new Transition( new ActionLabel( action ), right );
            left.AddTransition( e );
            return new StateCluster( left, right );
        }

        /** add an EOF transition to any rule end NFAState that points to nothing
         *  (i.e., for all those rules not invoked by another rule).  These
         *  are start symbols then.
         *
         *  Return the number of grammar entry points; i.e., how many rules are
         *  not invoked by another rule (they can only be invoked from outside).
         *  These are the start rules.
         */
        public virtual int BuildEofStates( IEnumerable<Rule> rules )
        {
            int numberUnInvokedRules = 0;
            foreach ( Rule r in rules )
            {
                NFAState endNFAState = r.StopState;
                // Is this rule a start symbol?  (no follow links)
                if ( endNFAState.transition[0] == null )
                {
                    // if so, then don't let algorithm fall off the end of
                    // the rule, make it hit EOF/EOT.
                    BuildEofState( endNFAState );
                    // track how many rules have been invoked by another rule
                    numberUnInvokedRules++;
                }
            }
            return numberUnInvokedRules;
        }

        /** set up an NFA NFAState that will yield eof tokens or,
         *  in the case of a lexer grammar, an EOT token when the conversion
         *  hits the end of a rule.
         */
        private void BuildEofState( NFAState endNFAState )
        {
            NFAState end = NewState();
            int label = Label.EOF;
            if ( _nfa.Grammar.type == GrammarType.Lexer )
            {
                label = Label.EOT;
                end.IsEOTTargetState = true;
            }
            //System.Console.Out.WriteLine( "build " + nfa.grammar.getTokenDisplayName( label ) +
            //                              " loop on end of state " + endNFAState.Description +
            //                              " to state " + end.stateNumber );
            Transition toEnd = new Transition( label, end );
            endNFAState.AddTransition( toEnd );
        }

        /** From A B build A-e->B (that is, build an epsilon arc from right
         *  of A to left of B).
         *
         *  As a convenience, return B if A is null or return A if B is null.
         */
        public virtual StateCluster BuildAB( StateCluster A, StateCluster B )
        {
            if ( A == null )
            {
                return B;
            }
            if ( B == null )
            {
                return A;
            }
            TransitionBetweenStates( A.Right, B.Left, Label.EPSILON );
            StateCluster g = new StateCluster( A.Left, B.Right );
            return g;
        }

        /** From a set ('a'|'b') build
         *
         *  o->o-'a'..'b'->o->o (last NFAState is blockEndNFAState pointed to by all alts)
         */
        public virtual StateCluster BuildAlternativeBlockFromSet( StateCluster set )
        {
            if ( set == null )
            {
                return null;
            }

            // single alt, no decision, just return only alt state cluster
            NFAState startOfAlt = NewState(); // must have this no matter what
            TransitionBetweenStates( startOfAlt, set.Left, Label.EPSILON );

            return new StateCluster( startOfAlt, set.Right );
        }

        /** From A|B|..|Z alternative block build
         *
         *  o->o-A->o->o (last NFAState is blockEndNFAState pointed to by all alts)
         *  |          ^
         *  o->o-B->o--|
         *  |          |
         *  ...        |
         *  |          |
         *  o->o-Z->o--|
         *
         *  So every alternative gets begin NFAState connected by epsilon
         *  and every alt right side points at a block end NFAState.  There is a
         *  new NFAState in the NFAState in the StateCluster for each alt plus one for the
         *  end NFAState.
         *
         *  Special case: only one alternative: don't make a block with alt
         *  begin/end.
         *
         *  Special case: if just a list of tokens/chars/sets, then collapse
         *  to a single edge'd o-set->o graph.
         *
         *  Set alt number (1..n) in the left-Transition NFAState.
         */
        public virtual StateCluster BuildAlternativeBlock( ICollection<StateCluster> alternativeStateClusters )
        {
            StateCluster result = null;
            if ( alternativeStateClusters == null || alternativeStateClusters.Count == 0 )
            {
                return null;
            }

            // single alt case
            if ( alternativeStateClusters.Count == 1 )
            {
                // single alt, no decision, just return only alt state cluster
                StateCluster g = alternativeStateClusters.First();
                NFAState startOfAlt = NewState(); // must have this no matter what
                TransitionBetweenStates( startOfAlt, g.Left, Label.EPSILON );

                //System.Console.Out.WriteLine( "### opt saved start/stop end in (...)" );
                return new StateCluster( startOfAlt, g.Right );
            }

            // even if we can collapse for lookahead purposes, we will still
            // need to predict the alts of this subrule in case there are actions
            // etc...  This is the decision that is pointed to from the AST node
            // (always)
            NFAState prevAlternative = null; // tracks prev so we can link to next alt
            NFAState firstAlt = null;
            NFAState blockEndNFAState = NewState();
            blockEndNFAState.Description = "end block";
            int altNum = 1;
            foreach ( StateCluster g in alternativeStateClusters )
            {
                // add begin NFAState for this alt connected by epsilon
                NFAState left = NewState();
                left.Description = "alt " + altNum + " of ()";
                TransitionBetweenStates( left, g.Left, Label.EPSILON );
                TransitionBetweenStates( g.Right, blockEndNFAState, Label.EPSILON );
                // Are we the first alternative?
                if ( firstAlt == null )
                {
                    firstAlt = left; // track extreme left node of StateCluster
                }
                else
                {
                    // if not first alternative, must link to this alt from previous
                    TransitionBetweenStates( prevAlternative, left, Label.EPSILON );
                }
                prevAlternative = left;
                altNum++;
            }

            // return StateCluster pointing representing entire block
            // Points to first alt NFAState on left, block end on right
            result = new StateCluster( firstAlt, blockEndNFAState );

            firstAlt.decisionStateType = NFAState.BLOCK_START;

            // set EOB markers for Jean
            firstAlt.endOfBlockStateNumber = blockEndNFAState.StateNumber;

            return result;
        }

        /** From (A)? build either:
         *
         *  o--A->o
         *  |     ^
         *  o---->|
         *
         *  or, if A is a block, just add an empty alt to the end of the block
         */
        public virtual StateCluster BuildAoptional( StateCluster A )
        {
            StateCluster g = null;
            int n = _nfa.Grammar.GetNumberOfAltsForDecisionNFA( A.Left );
            if ( n == 1 )
            {
                // no decision, just wrap in an optional path
                //NFAState decisionState = newState();
                NFAState decisionState = A.Left; // resuse left edge
                decisionState.Description = "only alt of ()? block";
                NFAState emptyAlt = NewState();
                emptyAlt.Description = "epsilon path of ()? block";
                NFAState blockEndNFAState = null;
                blockEndNFAState = NewState();
                TransitionBetweenStates( A.Right, blockEndNFAState, Label.EPSILON );
                blockEndNFAState.Description = "end ()? block";
                //transitionBetweenStates(decisionState, A.left, Label.EPSILON);
                TransitionBetweenStates( decisionState, emptyAlt, Label.EPSILON );
                TransitionBetweenStates( emptyAlt, blockEndNFAState, Label.EPSILON );

                // set EOB markers for Jean
                decisionState.endOfBlockStateNumber = blockEndNFAState.StateNumber;
                blockEndNFAState.decisionStateType = NFAState.RIGHT_EDGE_OF_BLOCK;

                g = new StateCluster( decisionState, blockEndNFAState );
            }
            else
            {
                // a decision block, add an empty alt
                NFAState lastRealAlt =
                        _nfa.Grammar.GetNFAStateForAltOfDecision( A.Left, n );
                NFAState emptyAlt = NewState();
                emptyAlt.Description = "epsilon path of ()? block";
                TransitionBetweenStates( lastRealAlt, emptyAlt, Label.EPSILON );
                TransitionBetweenStates( emptyAlt, A.Right, Label.EPSILON );

                // set EOB markers for Jean (I think this is redundant here)
                A.Left.endOfBlockStateNumber = A.Right.StateNumber;
                A.Right.decisionStateType = NFAState.RIGHT_EDGE_OF_BLOCK;

                g = A; // return same block, but now with optional last path
            }
            g.Left.decisionStateType = NFAState.OPTIONAL_BLOCK_START;

            return g;
        }

        /** From (A)+ build
         *
         *     |---|    (Transition 2 from A.right points at alt 1)
         *     v   |    (follow of loop is Transition 1)
         *  o->o-A-o->o
         *
         *  Meaning that the last NFAState in A points back to A's left Transition NFAState
         *  and we add a new begin/end NFAState.  A can be single alternative or
         *  multiple.
         *
         *  During analysis we'll call the follow link (transition 1) alt n+1 for
         *  an n-alt A block.
         */
        public virtual StateCluster BuildAplus( StateCluster A )
        {
            NFAState left = NewState();
            NFAState blockEndNFAState = NewState();
            blockEndNFAState.decisionStateType = NFAState.RIGHT_EDGE_OF_BLOCK;

            // don't reuse A.right as loopback if it's right edge of another block
            if ( A.Right.decisionStateType == NFAState.RIGHT_EDGE_OF_BLOCK )
            {
                // nested A* so make another tail node to be the loop back
                // instead of the usual A.right which is the EOB for inner loop
                NFAState extraRightEdge = NewState();
                TransitionBetweenStates( A.Right, extraRightEdge, Label.EPSILON );
                A.Right = extraRightEdge;
            }

            TransitionBetweenStates( A.Right, blockEndNFAState, Label.EPSILON ); // follow is Transition 1
            // turn A's block end into a loopback (acts like alt 2)
            TransitionBetweenStates( A.Right, A.Left, Label.EPSILON ); // loop back Transition 2
            TransitionBetweenStates( left, A.Left, Label.EPSILON );

            A.Right.decisionStateType = NFAState.LOOPBACK;
            A.Left.decisionStateType = NFAState.BLOCK_START;

            // set EOB markers for Jean
            A.Left.endOfBlockStateNumber = A.Right.StateNumber;

            StateCluster g = new StateCluster( left, blockEndNFAState );
            return g;
        }

        /** From (A)* build
         *
         *     |---|
         *     v   |
         *  o->o-A-o--o (Transition 2 from block end points at alt 1; follow is Transition 1)
         *  |         ^
         *  o---------| (optional branch is 2nd alt of optional block containing A+)
         *
         *  Meaning that the last (end) NFAState in A points back to A's
         *  left side NFAState and we add 3 new NFAStates (the
         *  optional branch is built just like an optional subrule).
         *  See the Aplus() method for more on the loop back Transition.
         *  The new node on right edge is set to RIGHT_EDGE_OF_CLOSURE so we
         *  can detect nested (A*)* loops and insert an extra node.  Previously,
         *  two blocks shared same EOB node.
         *
         *  There are 2 or 3 decision points in a A*.  If A is not a block (i.e.,
         *  it only has one alt), then there are two decisions: the optional bypass
         *  and then loopback.  If A is a block of alts, then there are three
         *  decisions: bypass, loopback, and A's decision point.
         *
         *  Note that the optional bypass must be outside the loop as (A|B)* is
         *  not the same thing as (A|B|)+.
         *
         *  This is an accurate NFA representation of the meaning of (A)*, but
         *  for generating code, I don't need a DFA for the optional branch by
         *  virtue of how I generate code.  The exit-loopback-branch decision
         *  is sufficient to let me make an appropriate enter, exit, loop
         *  determination.  See codegen.g
         */
        public virtual StateCluster BuildAstar( StateCluster A )
        {
            NFAState bypassDecisionState = NewState();
            bypassDecisionState.Description = "enter loop path of ()* block";
            NFAState optionalAlt = NewState();
            optionalAlt.Description = "epsilon path of ()* block";
            NFAState blockEndNFAState = NewState();
            blockEndNFAState.decisionStateType = NFAState.RIGHT_EDGE_OF_BLOCK;

            // don't reuse A.right as loopback if it's right edge of another block
            if ( A.Right.decisionStateType == NFAState.RIGHT_EDGE_OF_BLOCK )
            {
                // nested A* so make another tail node to be the loop back
                // instead of the usual A.right which is the EOB for inner loop
                NFAState extraRightEdge = NewState();
                TransitionBetweenStates( A.Right, extraRightEdge, Label.EPSILON );
                A.Right = extraRightEdge;
            }

            // convert A's end block to loopback
            A.Right.Description = "()* loopback";
            // Transition 1 to actual block of stuff
            TransitionBetweenStates( bypassDecisionState, A.Left, Label.EPSILON );
            // Transition 2 optional to bypass
            TransitionBetweenStates( bypassDecisionState, optionalAlt, Label.EPSILON );
            TransitionBetweenStates( optionalAlt, blockEndNFAState, Label.EPSILON );
            // Transition 1 of end block exits
            TransitionBetweenStates( A.Right, blockEndNFAState, Label.EPSILON );
            // Transition 2 of end block loops
            TransitionBetweenStates( A.Right, A.Left, Label.EPSILON );

            bypassDecisionState.decisionStateType = NFAState.BYPASS;
            A.Left.decisionStateType = NFAState.BLOCK_START;
            A.Right.decisionStateType = NFAState.LOOPBACK;

            // set EOB markers for Jean
            A.Left.endOfBlockStateNumber = A.Right.StateNumber;
            bypassDecisionState.endOfBlockStateNumber = blockEndNFAState.StateNumber;

            StateCluster g = new StateCluster( bypassDecisionState, blockEndNFAState );
            return g;
        }

#if false
        /** Build an NFA predictor for special rule called Tokens manually that
         *  predicts which token will succeed.  The refs to the rules are not
         *  RuleRefTransitions as I want DFA conversion to stop at the EOT
         *  transition on the end of each token, rather than return to Tokens rule.
         *  If I used normal build_alternativeBlock for this, the RuleRefTransitions
         *  would save return address when jumping away from Tokens rule.
         *
         *  All I do here is build n new states for n rules with an epsilon
         *  edge to the rule start states and then to the next state in the
         *  list:
         *
         *   o->(A)  (a state links to start of A and to next in list)
         *   |
         *   o->(B)
         *   |
         *   ...
         *   |
         *   o->(Z)
         *
         *  This is the NFA created for the artificial rule created in
         *  Grammar.addArtificialMatchTokensRule().
         *
         *  11/28/2005: removed so we can use normal rule construction for Tokens.
         */
        public NFAState BuildArtificialMatchTokensRuleNFA()
        {
            int altNum = 1;
            NFAState firstAlt = null; // the start state for the "rule"
            NFAState prevAlternative = null;
            Iterator iter = nfa.grammar.getRules().iterator();
            // TODO: add a single decision node/state for good description
            while ( iter.hasNext() )
            {
                Rule r = (Rule)iter.next();
                string ruleName = r.name;
                string modifier = nfa.grammar.getRuleModifier( ruleName );
                if ( ruleName.Equals( Grammar.ARTIFICIAL_TOKENS_RULENAME ) ||
                     ( modifier != null &&
                      modifier.Equals( Grammar.FRAGMENT_RULE_MODIFIER ) ) )
                {
                    continue; // don't loop to yourself or do nontoken rules
                }
                NFAState ruleStartState = nfa.grammar.getRuleStartState( ruleName );
                NFAState left = newState();
                left.Description = "alt " + altNum + " of artificial rule " + Grammar.ARTIFICIAL_TOKENS_RULENAME;
                transitionBetweenStates( left, ruleStartState, Label.EPSILON );
                // Are we the first alternative?
                if ( firstAlt == null )
                {
                    firstAlt = left; // track extreme top left node as rule start
                }
                else
                {
                    // if not first alternative, must link to this alt from previous
                    transitionBetweenStates( prevAlternative, left, Label.EPSILON );
                }
                prevAlternative = left;
                altNum++;
            }
            firstAlt.decisionStateType = NFAState.BLOCK_START;

            return firstAlt;
        }
#endif

        /** Build an atom with all possible values in its label */
        public virtual StateCluster BuildWildcard( GrammarAST associatedAST )
        {
            NFAState left = NewState();
            NFAState right = NewState();
            left.associatedASTNode = associatedAST;
            right.associatedASTNode = associatedAST;
            Label label = new Label( _nfa.Grammar.TokenTypes ); // char or tokens
            Transition e = new Transition( label, right );
            left.AddTransition( e );
            StateCluster g = new StateCluster( left, right );
            return g;
        }

        /** Build a subrule matching ^(. .*) (any tree or node). Let's use
         *  (^(. .+) | .) to be safe.
         */
        public StateCluster BuildWildcardTree( GrammarAST associatedAST )
        {
            StateCluster wildRoot = BuildWildcard( associatedAST );

            StateCluster down = BuildAtom( Label.DOWN, associatedAST );
            wildRoot = BuildAB( wildRoot, down ); // hook in; . DOWN

            // make .+
            StateCluster wildChildren = BuildWildcard( associatedAST );
            wildChildren = BuildAplus( wildChildren );
            wildRoot = BuildAB( wildRoot, wildChildren ); // hook in; . DOWN .+

            StateCluster up = BuildAtom( Label.UP, associatedAST );
            wildRoot = BuildAB( wildRoot, up ); // hook in; . DOWN .+ UP

            // make optional . alt
            StateCluster optionalNodeAlt = BuildWildcard( associatedAST );

            //List alts = new List<object>();
            var alts = new List<StateCluster>()
            {
                wildRoot,
                optionalNodeAlt
            };
            StateCluster blk = BuildAlternativeBlock( alts );

            return blk;
        }

        /** Given a collapsed block of alts (a set of atoms), pull out
         *  the set and return it.
         */
        protected virtual IIntSet GetCollapsedBlockAsSet( State blk )
        {
            State s0 = blk;
            if ( s0 != null && s0.GetTransition( 0 ) != null )
            {
                State s1 = s0.GetTransition( 0 ).Target;
                if ( s1 != null && s1.GetTransition( 0 ) != null )
                {
                    Label label = s1.GetTransition( 0 ).Label;
                    if ( label.IsSet )
                    {
                        return label.Set;
                    }
                }
            }
            return null;
        }

        private void TransitionBetweenStates( NFAState a, NFAState b, int label )
        {
            Transition e = new Transition( label, b );
            a.AddTransition( e );
        }
    }
}
