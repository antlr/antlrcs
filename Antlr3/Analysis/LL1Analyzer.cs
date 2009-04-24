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

namespace Antlr3.Analysis
{
    using System.Collections.Generic;
    using Antlr.Runtime.JavaExtensions;

    using ANTLRParser = Antlr3.Grammars.ANTLRParser;
    using Console = System.Console;
    using Grammar = Antlr3.Tool.Grammar;
    using GrammarType = Antlr3.Tool.GrammarType;
    using IIntSet = Antlr3.Misc.IIntSet;
    using IntervalSet = Antlr3.Misc.IntervalSet;
    using Rule = Antlr3.Tool.Rule;

    /**
     * Created by IntelliJ IDEA.
     * User: parrt
     * Date: Dec 31, 2007
     * Time: 1:31:16 PM
     * To change this template use File | Settings | File Templates.
     */
    public class LL1Analyzer
    {
        /**	0	if we hit end of rule and invoker should keep going (epsilon) */
        public const int DETECT_PRED_EOR = 0;
        /**	1	if we found a nonautobacktracking pred */
        public const int DETECT_PRED_FOUND = 1;
        /**	2	if we didn't find such a pred */
        public const int DETECT_PRED_NOT_FOUND = 2;

        Grammar _grammar;

        /** Used during LOOK to detect computation cycles */
        HashSet<NFAState> _lookBusy = new HashSet<NFAState>();

        IDictionary<NFAState, LookaheadSet> _firstCache = new Dictionary<NFAState, LookaheadSet>();
        IDictionary<Rule, LookaheadSet> _followCache = new Dictionary<Rule, LookaheadSet>();

        public LL1Analyzer( Grammar grammar )
        {
            this._grammar = grammar;
        }

#if false
        public virtual void ComputeRuleFIRSTSets()
        {
            if ( NumberOfDecisions == 0 )
            {
                createNFAs();
            }
            for ( Iterator it = getRules().iterator(); it.hasNext(); )
            {
                Rule r = (Rule)it.next();
                if ( r.isSynPred )
                {
                    continue;
                }
                LookaheadSet s = FIRST( r );
                JSystem.@out.println( "FIRST(" + r.name + ")=" + s );
            }
        }
#endif

#if false
        public HashSet<string> GetOverriddenRulesWithDifferentFIRST() {
            // walk every rule in this grammar and compare FIRST set with
            // those in imported grammars.
            HashSet<string> rules = new HashSet<string>();
            for (Iterator it = getRules().iterator(); it.hasNext();) {
                Rule r = (Rule)it.next();
                //JSystem.@out.println(r.name+" FIRST="+r.FIRST);
                for (int i = 0; i < delegates.size(); i++) {
                    Grammar g = delegates.get(i);
                    Rule importedRule = g.getRule(r.name);
                    if ( importedRule != null ) { // exists in imported grammar
                        // JSystem.@out.println(r.name+" exists in imported grammar: FIRST="+importedRule.FIRST);
                        if ( !r.FIRST.equals(importedRule.FIRST) ) {
                            rules.add(r.name);
                        }
                    }
                }
            }
            return rules;
        }

        public Set<Rule> GetImportedRulesSensitiveToOverriddenRulesDueToLOOK() {
            Set<String> diffFIRSTs = getOverriddenRulesWithDifferentFIRST();
            Set<Rule> rules = new HashSet();
            for (Iterator it = diffFIRSTs.iterator(); it.hasNext();) {
                String r = (String) it.next();
                for (int i = 0; i < delegates.size(); i++) {
                    Grammar g = delegates.get(i);
                    Set<Rule> callers = g.ruleSensitivity.get(r);
                    // somebody invokes rule whose FIRST changed in subgrammar?
                    if ( callers!=null ) {
                        rules.addAll(callers);
                        //JSystem.@out.println(g.name+" rules "+callers+" sensitive to "+r+"; dup 'em");
                    }
                }
            }
            return rules;
        }
#endif

#if false
        public LookaheadSet Look(Rule r) {
            if ( r.FIRST==null ) {
                r.FIRST = FIRST(r.startState);
            }
            return r.FIRST;
        }
#endif

        /** From an NFA state, s, find the set of all labels reachable from s.
         *  Used to compute follow sets for error recovery.  Never computes
         *  a FOLLOW operation.  FIRST stops at end of rules, returning EOR, unless
         *  invoked from another rule.  I.e., routine properly handles
         *
         *     a : b A ;
         *
         *  where b is nullable.
         *
         *  We record with EOR_TOKEN_TYPE if we hit the end of a rule so we can
         *  know at runtime (when these sets are used) to start walking up the
         *  follow chain to compute the real, correct follow set (as opposed to
         *  the FOLLOW, which is a superset).
         *
         *  This routine will only be used on parser and tree parser grammars.
         */
        public LookaheadSet First( NFAState s )
        {
            //JSystem.@out.println("> FIRST("+s.enclosingRule.name+") in rule "+s.enclosingRule);
            _lookBusy.Clear();
            LookaheadSet look = FirstCore( s, false );
            //JSystem.@out.println("< FIRST("+s.enclosingRule.name+") in rule "+s.enclosingRule+"="+look.toString(this.grammar));
            return look;
        }

        public LookaheadSet Follow( Rule r )
        {
            //JSystem.@out.println("> FOLLOW("+r.name+") in rule "+r.startState.enclosingRule);
            LookaheadSet f = _followCache.get( r );
            if ( f != null )
            {
                return f;
            }
            f = FirstCore( r.stopState, true );
            _followCache[r] = f;
            //JSystem.@out.println("< FOLLOW("+r+") in rule "+r.startState.enclosingRule+"="+f.toString(this.grammar));
            return f;
        }

        public LookaheadSet Look( NFAState s )
        {
            if ( NFAToDFAConverter.debug )
            {
                Console.Out.WriteLine( "> LOOK(" + s + ")" );
            }
            _lookBusy.Clear();
            LookaheadSet look = FirstCore( s, true );
            // FOLLOW makes no sense (at the moment!) for lexical rules.
            if ( _grammar.type != GrammarType.Lexer && look.Member( Label.EOR_TOKEN_TYPE ) )
            {
                // avoid altering FIRST reset as it is cached
                LookaheadSet f = Follow( s.enclosingRule );
                f.OrInPlace( look );
                f.Remove( Label.EOR_TOKEN_TYPE );
                look = f;
                //look.orInPlace(FOLLOW(s.enclosingRule));
            }
            else if ( _grammar.type == GrammarType.Lexer && look.Member( Label.EOT ) )
            {
                // if this has EOT, lookahead is all char (all char can follow rule)
                //look = new LookaheadSet(Label.EOT);
                look = new LookaheadSet( IntervalSet.COMPLETE_SET );
            }
            if ( NFAToDFAConverter.debug )
            {
                Console.Out.WriteLine( "< LOOK(" + s + ")=" + look.ToString( _grammar ) );
            }
            return look;
        }

        protected virtual LookaheadSet FirstCore( NFAState s, bool chaseFollowTransitions )
        {
            /*
            JSystem.@out.println("_LOOK("+s+") in rule "+s.enclosingRule);
            if ( s.transition[0] instanceof RuleClosureTransition ) {
                JSystem.@out.println("go to rule "+((NFAState)s.transition[0].target).enclosingRule);
            }
            */
            if ( !chaseFollowTransitions && s.IsAcceptState )
            {
                if ( _grammar.type == GrammarType.Lexer )
                {
                    // FOLLOW makes no sense (at the moment!) for lexical rules.
                    // assume all char can follow
                    return new LookaheadSet( IntervalSet.COMPLETE_SET );
                }
                return new LookaheadSet( Label.EOR_TOKEN_TYPE );
            }

            if ( _lookBusy.Contains( s ) )
            {
                // return a copy of an empty set; we may modify set inline
                return new LookaheadSet();
            }
            _lookBusy.Add( s );

            Transition transition0 = s.transition[0];
            if ( transition0 == null )
            {
                return null;
            }

            if ( transition0.label.IsAtom )
            {
                int atom = transition0.label.Atom;
                return new LookaheadSet( atom );
            }
            if ( transition0.label.IsSet )
            {
                IIntSet sl = transition0.label.Set;
                return new LookaheadSet( sl );
            }

            // compute FIRST of transition 0
            LookaheadSet tset = null;
            // if transition 0 is a rule call and we don't want FOLLOW, check cache
            if ( !chaseFollowTransitions && transition0 is RuleClosureTransition )
            {
                LookaheadSet prev = _firstCache.get( (NFAState)transition0.target );
                if ( prev != null )
                {
                    tset = new LookaheadSet( prev );
                }
            }

            // if not in cache, must compute
            if ( tset == null )
            {
                tset = FirstCore( (NFAState)transition0.target, chaseFollowTransitions );
                // save FIRST cache for transition 0 if rule call
                if ( !chaseFollowTransitions && transition0 is RuleClosureTransition )
                {
                    _firstCache[(NFAState)transition0.target] = tset;
                }
            }

            // did we fall off the end?
            if ( _grammar.type != GrammarType.Lexer && tset.Member( Label.EOR_TOKEN_TYPE ) )
            {
                if ( transition0 is RuleClosureTransition )
                {
                    // we called a rule that found the end of the rule.
                    // That means the rule is nullable and we need to
                    // keep looking at what follows the rule ref.  E.g.,
                    // a : b A ; where b is nullable means that LOOK(a)
                    // should include A.
                    RuleClosureTransition ruleInvocationTrans =
                        (RuleClosureTransition)transition0;
                    // remove the EOR and get what follows
                    //tset.remove(Label.EOR_TOKEN_TYPE);
                    NFAState following = (NFAState)ruleInvocationTrans.followState;
                    LookaheadSet fset = FirstCore( following, chaseFollowTransitions );
                    fset.OrInPlace( tset ); // tset cached; or into new set
                    fset.Remove( Label.EOR_TOKEN_TYPE );
                    tset = fset;
                }
            }

            Transition transition1 = s.transition[1];
            if ( transition1 != null )
            {
                LookaheadSet tset1 =
                    FirstCore( (NFAState)transition1.target, chaseFollowTransitions );
                tset1.OrInPlace( tset ); // tset cached; or into new set
                tset = tset1;
            }

            return tset;
        }

        /** Is there a non-syn-pred predicate visible from s that is not in
         *  the rule enclosing s?  This accounts for most predicate situations
         *  and lets ANTLR do a simple LL(1)+pred computation.
         *
         *  TODO: what about gated vs regular preds?
         */
        public bool DetectConfoundingPredicates( NFAState s )
        {
            _lookBusy.Clear();
            Rule r = s.enclosingRule;
            return DetectConfoundingPredicatesCore( s, r, false ) == DETECT_PRED_FOUND;
        }

        protected virtual int DetectConfoundingPredicatesCore( NFAState s,
                                                   Rule enclosingRule,
                                                   bool chaseFollowTransitions )
        {
            //JSystem.@out.println("_detectNonAutobacktrackPredicates("+s+")");
            if ( !chaseFollowTransitions && s.IsAcceptState )
            {
                if ( _grammar.type == GrammarType.Lexer )
                {
                    // FOLLOW makes no sense (at the moment!) for lexical rules.
                    // assume all char can follow
                    return DETECT_PRED_NOT_FOUND;
                }
                return DETECT_PRED_EOR;
            }

            if ( _lookBusy.Contains( s ) )
            {
                // return a copy of an empty set; we may modify set inline
                return DETECT_PRED_NOT_FOUND;
            }
            _lookBusy.Add( s );

            Transition transition0 = s.transition[0];
            if ( transition0 == null )
            {
                return DETECT_PRED_NOT_FOUND;
            }

            if ( !( transition0.label.IsSemanticPredicate ||
                   transition0.label.IsEpsilon ) )
            {
                return DETECT_PRED_NOT_FOUND;
            }

            if ( transition0.label.IsSemanticPredicate )
            {
                //JSystem.@out.println("pred "+transition0.label);
                SemanticContext ctx = transition0.label.SemanticContext;
                SemanticContext.Predicate p = (SemanticContext.Predicate)ctx;
                if ( p.predicateAST.Type != ANTLRParser.BACKTRACK_SEMPRED )
                {
                    return DETECT_PRED_FOUND;
                }
            }

            /*
            if ( transition0.label.isSemanticPredicate() ) {
                JSystem.@out.println("pred "+transition0.label);
                SemanticContext ctx = transition0.label.getSemanticContext();
                SemanticContext.Predicate p = (SemanticContext.Predicate)ctx;
                // if a non-syn-pred found not in enclosingRule, say we found one
                if ( p.predicateAST.getType() != ANTLRParser.BACKTRACK_SEMPRED &&
                     !p.predicateAST.enclosingRuleName.equals(enclosingRule.name) )
                {
                    JSystem.@out.println("found pred "+p+" not in "+enclosingRule.name);
                    return DETECT_PRED_FOUND;
                }
            }
            */

            int result = DetectConfoundingPredicatesCore( (NFAState)transition0.target,
                                                      enclosingRule,
                                                      chaseFollowTransitions );
            if ( result == DETECT_PRED_FOUND )
            {
                return DETECT_PRED_FOUND;
            }

            if ( result == DETECT_PRED_EOR )
            {
                if ( transition0 is RuleClosureTransition )
                {
                    // we called a rule that found the end of the rule.
                    // That means the rule is nullable and we need to
                    // keep looking at what follows the rule ref.  E.g.,
                    // a : b A ; where b is nullable means that LOOK(a)
                    // should include A.
                    RuleClosureTransition ruleInvocationTrans =
                        (RuleClosureTransition)transition0;
                    NFAState following = (NFAState)ruleInvocationTrans.followState;
                    int afterRuleResult =
                        DetectConfoundingPredicatesCore( following,
                                                     enclosingRule,
                                                     chaseFollowTransitions );
                    if ( afterRuleResult == DETECT_PRED_FOUND )
                    {
                        return DETECT_PRED_FOUND;
                    }
                }
            }

            Transition transition1 = s.transition[1];
            if ( transition1 != null )
            {
                int t1Result =
                    DetectConfoundingPredicatesCore( (NFAState)transition1.target,
                                                 enclosingRule,
                                                 chaseFollowTransitions );
                if ( t1Result == DETECT_PRED_FOUND )
                {
                    return DETECT_PRED_FOUND;
                }
            }

            return DETECT_PRED_NOT_FOUND;
        }

        /** Return predicate expression found via epsilon edges from s.  Do
         *  not look into other rules for now.  Do something simple.  Include
         *  backtracking synpreds.
         */
        public SemanticContext GetPredicates( NFAState altStartState )
        {
            _lookBusy.Clear();
            return GetPredicatesCore( altStartState, altStartState );
        }

        protected virtual SemanticContext GetPredicatesCore( NFAState s, NFAState altStartState )
        {
            //JSystem.@out.println("_getPredicates("+s+")");
            if ( s.IsAcceptState )
            {
                return null;
            }

            // avoid infinite loops from (..)* etc...
            if ( _lookBusy.Contains( s ) )
            {
                return null;
            }
            _lookBusy.Add( s );

            Transition transition0 = s.transition[0];
            // no transitions
            if ( transition0 == null )
            {
                return null;
            }

            // not a predicate and not even an epsilon
            if ( !( transition0.label.IsSemanticPredicate ||
                   transition0.label.IsEpsilon ) )
            {
                return null;
            }

            SemanticContext p = null;
            SemanticContext p0 = null;
            SemanticContext p1 = null;
            if ( transition0.label.IsSemanticPredicate )
            {
                //JSystem.@out.println("pred "+transition0.label);
                p = transition0.label.SemanticContext;
                // ignore backtracking preds not on left edge for this decision
                if ( ( (SemanticContext.Predicate)p ).predicateAST.Type ==
                      ANTLRParser.BACKTRACK_SEMPRED &&
                     s == altStartState.transition[0].target )
                {
                    p = null; // don't count
                }
            }

            // get preds from beyond this state
            p0 = GetPredicatesCore( (NFAState)transition0.target, altStartState );

            // get preds from other transition
            Transition transition1 = s.transition[1];
            if ( transition1 != null )
            {
                p1 = GetPredicatesCore( (NFAState)transition1.target, altStartState );
            }

            // join this&following-right|following-down
            return SemanticContext.And( p, SemanticContext.Or( p0, p1 ) );
        }
    }
}
