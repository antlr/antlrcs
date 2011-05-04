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

namespace Antlr3.Analysis
{
    using System.Collections.Generic;
    using System.Linq;
    using Antlr3.Misc;

    using ANTLRParser = Antlr3.Grammars.ANTLRParser;

    /** A special DFA that is exactly LL(1) or LL(1) with backtracking mode
     *  predicates to resolve edge set collisions.
     */
    public class LL1DFA : DFA
    {
        /** From list of lookahead sets (one per alt in decision), create
         *  an LL(1) DFA.  One edge per set.
         *
         *  s0-{alt1}->:o=>1
         *  | \
         *  |  -{alt2}->:o=>2
         *  |
         *  ...
         */
        public LL1DFA( int decisionNumber, NFAState decisionStartState, LookaheadSet[] altLook )
            : base(decisionNumber, decisionStartState)
        {
            DFAState s0 = NewState();
            StartState = s0;
            UnreachableAlts.Clear();
            for ( int alt = 1; alt < altLook.Length; alt++ )
            {
                DFAState acceptAltState = NewState();
                acceptAltState.IsAcceptState = true;
                SetAcceptState( alt, acceptAltState );
                acceptAltState.LookaheadDepth = 1;
                acceptAltState.CachedUniquelyPredicatedAlt = alt;
                Label e = GetLabelForSet( altLook[alt].TokenTypeSet );
                s0.AddTransition( acceptAltState, e );
            }
        }

        /** From a set of edgeset->list-of-alts mappings, create a DFA
         *  that uses syn preds for all |list-of-alts|>1.
         */
        public LL1DFA(int decisionNumber, NFAState decisionStartState, MultiMap<IntervalSet, int> edgeMap)
            : base(decisionNumber, decisionStartState)
        {
            DFAState s0 = NewState();
            StartState = s0;
            UnreachableAlts.Clear();
            foreach ( var edgeVar in edgeMap )
            {
                IntervalSet edge = edgeVar.Key;
                IList<int> alts = edgeVar.Value;
                alts = alts.OrderBy( i => i ).ToList();
                //Collections.sort( alts ); // make sure alts are attempted in order
                //JSystem.@out.println(edge+" -> "+alts);
                DFAState s = NewState();
                s.LookaheadDepth = 1;
                Label e = GetLabelForSet( edge );
                s0.AddTransition( s, e );
                if ( alts.Count == 1 )
                {
                    s.IsAcceptState = true;
                    int alt = alts[0];
                    SetAcceptState( alt, s );
                    s.CachedUniquelyPredicatedAlt = alt;
                }
                else
                {
                    // resolve with syntactic predicates.  Add edges from
                    // state s that test predicates.
                    s.IsResolvedWithPredicates = true;
                    for ( int i = 0; i < alts.Count; i++ )
                    {
                        int alt = (int)alts[i];
                        s.CachedUniquelyPredicatedAlt = NFA.INVALID_ALT_NUMBER;
                        DFAState predDFATarget = GetAcceptState( alt );
                        if ( predDFATarget == null )
                        {
                            predDFATarget = NewState(); // create if not there.
                            predDFATarget.IsAcceptState = true;
                            predDFATarget.CachedUniquelyPredicatedAlt = alt;
                            SetAcceptState( alt, predDFATarget );
                        }
                        // add a transition to pred target from d
                        /*
                        int walkAlt =
                            decisionStartState.translateDisplayAltToWalkAlt(alt);
                        NFAState altLeftEdge = nfa.grammar.getNFAStateForAltOfDecision(decisionStartState, walkAlt);
                        NFAState altStartState = (NFAState)altLeftEdge.transition[0].target;
                        SemanticContext ctx = nfa.grammar.ll1Analyzer.getPredicates(altStartState);
                        JSystem.@out.println("sem ctx = "+ctx);
                        if ( ctx == null ) {
                            ctx = new SemanticContext.TruePredicate();
                        }
                        s.addTransition(predDFATarget, new Label(ctx));
                        */
                        SemanticContext.Predicate synpred =
                            GetSynPredForAlt( decisionStartState, alt );
                        if ( synpred == null )
                        {
                            synpred = new SemanticContext.TruePredicate();
                        }
                        s.AddTransition( predDFATarget, new PredicateLabel( synpred ) );
                    }
                }
            }
            //JSystem.@out.println("dfa for preds=\n"+this);
        }

        protected virtual Label GetLabelForSet( IntervalSet edgeSet )
        {
            Label e = null;
            int atom = edgeSet.GetSingleElement();
            if ( atom != Label.INVALID )
            {
                e = new Label( atom );
            }
            else
            {
                e = new Label( edgeSet );
            }
            return e;
        }

        protected virtual SemanticContext.Predicate GetSynPredForAlt( NFAState decisionStartState,
                                                             int alt )
        {
            int walkAlt =
                decisionStartState.TranslateDisplayAltToWalkAlt( alt );
            NFAState altLeftEdge =
                Nfa.Grammar.GetNFAStateForAltOfDecision( decisionStartState, walkAlt );
            NFAState altStartState = (NFAState)altLeftEdge.transition[0].Target;
            //JSystem.@out.println("alt "+alt+" start state = "+altStartState.stateNumber);
            if ( altStartState.transition[0].IsSemanticPredicate )
            {
                SemanticContext ctx = altStartState.transition[0].Label.SemanticContext;
                if ( ctx.IsSyntacticPredicate )
                {
                    SemanticContext.Predicate p = (SemanticContext.Predicate)ctx;
                    if ( p.PredicateAST.Type == ANTLRParser.BACKTRACK_SEMPRED )
                    {
                        /*
                        JSystem.@out.println("syn pred for alt "+walkAlt+" "+
                                           ((SemanticContext.Predicate)altStartState.transition[0].label.getSemanticContext()).predicateAST);
                        */
                        if ( ctx.IsSyntacticPredicate )
                        {
                            Nfa.Grammar.SynPredUsedInDFA( this, ctx );
                        }
                        return (SemanticContext.Predicate)altStartState.transition[0].Label.SemanticContext;
                    }
                }
            }
            return null;
        }
    }
}
