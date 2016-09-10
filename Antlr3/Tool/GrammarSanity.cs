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

    using ANTLRParser = Antlr3.Grammars.ANTLRParser;
    using NFAState = Antlr3.Analysis.NFAState;
    using RuleClosureTransition = Antlr3.Analysis.RuleClosureTransition;
    using Transition = Antlr3.Analysis.Transition;

    /** Factor out routines that check sanity of rules, alts, grammars, etc.. */
    public class GrammarSanity
    {
        /** The checkForLeftRecursion method needs to track what rules it has
         *  visited to track infinite recursion.
         */
        protected HashSet<Rule> visitedDuringRecursionCheck = null;

        protected Grammar grammar;
        public GrammarSanity( Grammar grammar )
        {
            this.grammar = grammar;
        }

        /** Check all rules for infinite left recursion before analysis. Return list
         *  of troublesome rule cycles.  This method has two side-effects: it notifies
         *  the error manager that we have problems and it sets the list of
         *  recursive rules that we should ignore during analysis.
         */
        public virtual IList<HashSet<Rule>> CheckAllRulesForLeftRecursion()
        {
            grammar.BuildNFA(); // make sure we have NFAs
            grammar.leftRecursiveRules = new HashSet<Rule>();
            List<HashSet<Rule>> listOfRecursiveCycles = new List<HashSet<Rule>>();
            for ( int i = 0; i < grammar.composite.RuleIndexToRuleList.Count; i++ )
            {
                Rule r = grammar.composite.RuleIndexToRuleList[i];
                if ( r != null )
                {
                    visitedDuringRecursionCheck = new HashSet<Rule>();
                    visitedDuringRecursionCheck.Add( r );
                    HashSet<object> visitedStates = new HashSet<object>();
                    TraceStatesLookingForLeftRecursion( r.StartState,
                                                       visitedStates,
                                                       listOfRecursiveCycles );
                }
            }
            if ( listOfRecursiveCycles.Count > 0 )
            {
                ErrorManager.LeftRecursionCycles( listOfRecursiveCycles );
            }
            return listOfRecursiveCycles;
        }

        /** From state s, look for any transition to a rule that is currently
         *  being traced.  When tracing r, visitedDuringRecursionCheck has r
         *  initially.  If you reach an accept state, return but notify the
         *  invoking rule that it is nullable, which implies that invoking
         *  rule must look at follow transition for that invoking state.
         *  The visitedStates tracks visited states within a single rule so
         *  we can avoid epsilon-loop-induced infinite recursion here.  Keep
         *  filling the cycles in listOfRecursiveCycles and also, as a
         *  side-effect, set leftRecursiveRules.
         */
        protected virtual bool TraceStatesLookingForLeftRecursion( NFAState s,
                                                             HashSet<object> visitedStates,
                                                             IList<HashSet<Rule>> listOfRecursiveCycles )
        {
            if ( s.IsAcceptState )
            {
                // this rule must be nullable!
                // At least one epsilon edge reached accept state
                return true;
            }
            if ( visitedStates.Contains( s ) )
            {
                // within same rule, we've hit same state; quit looping
                return false;
            }
            visitedStates.Add( s );
            bool stateReachesAcceptState = false;
            Transition t0 = s.transition[0];
            if ( t0 is RuleClosureTransition )
            {
                RuleClosureTransition refTrans = (RuleClosureTransition)t0;
                Rule refRuleDef = refTrans.Rule;
                //String targetRuleName = ((NFAState)t0.target).getEnclosingRule();
                if ( visitedDuringRecursionCheck.Contains( refRuleDef ) )
                {
                    // record left-recursive rule, but don't go back in
                    grammar.leftRecursiveRules.Add( refRuleDef );
                    //System.Console.Out.WriteLine( "already visited " + refRuleDef + ", calling from " + s.enclosingRule );
                    AddRulesToCycle( refRuleDef,
                                    s.enclosingRule,
                                    listOfRecursiveCycles );
                }
                else
                {
                    // must visit if not already visited; send new visitedStates set
                    visitedDuringRecursionCheck.Add( refRuleDef );
                    bool callReachedAcceptState =
                        TraceStatesLookingForLeftRecursion( (NFAState)t0.Target,
                                                           new HashSet<object>(),
                                                           listOfRecursiveCycles );
                    // we're back from visiting that rule
                    visitedDuringRecursionCheck.Remove( refRuleDef );
                    // must keep going in this rule then
                    if ( callReachedAcceptState )
                    {
                        NFAState followingState =
                            ( (RuleClosureTransition)t0 ).FollowState;
                        stateReachesAcceptState |=
                            TraceStatesLookingForLeftRecursion( followingState,
                                                               visitedStates,
                                                               listOfRecursiveCycles );
                    }
                }
            }
            else if ( t0.Label.IsEpsilon || t0.Label.IsSemanticPredicate )
            {
                stateReachesAcceptState |=
                    TraceStatesLookingForLeftRecursion( (NFAState)t0.Target, visitedStates, listOfRecursiveCycles );
            }
            // else it has a labeled edge

            // now do the other transition if it exists
            Transition t1 = s.transition[1];
            if ( t1 != null )
            {
                stateReachesAcceptState |=
                    TraceStatesLookingForLeftRecursion( (NFAState)t1.Target,
                                                       visitedStates,
                                                       listOfRecursiveCycles );
            }
            return stateReachesAcceptState;
        }

        /** enclosingRuleName calls targetRuleName, find the cycle containing
         *  the target and add the caller.  Find the cycle containing the caller
         *  and add the target.  If no cycles contain either, then create a new
         *  cycle.  listOfRecursiveCycles is List&lt;Set&lt;String&gt;&gt; that holds a list
         *  of cycles (sets of rule names).
         */
        protected virtual void AddRulesToCycle( Rule targetRule,
                                       Rule enclosingRule,
                                       IList<HashSet<Rule>> listOfRecursiveCycles )
        {
            bool foundCycle = false;
            for ( int i = 0; i < listOfRecursiveCycles.Count; i++ )
            {
                HashSet<Rule> rulesInCycle = listOfRecursiveCycles[i];
                // ensure both rules are in same cycle
                if ( rulesInCycle.Contains( targetRule ) )
                {
                    rulesInCycle.Add( enclosingRule );
                    foundCycle = true;
                }
                if ( rulesInCycle.Contains( enclosingRule ) )
                {
                    rulesInCycle.Add( targetRule );
                    foundCycle = true;
                }
            }
            if ( !foundCycle )
            {
                HashSet<Rule> cycle = new HashSet<Rule>();
                cycle.Add( targetRule );
                cycle.Add( enclosingRule );
                listOfRecursiveCycles.Add( cycle );
            }
        }

        public virtual void CheckRuleReference( GrammarAST scopeAST,
                                       GrammarAST refAST,
                                       GrammarAST argsAST,
                                       string currentRuleName )
        {
            Rule r = grammar.GetRule( refAST.Text );
            if ( refAST.Type == ANTLRParser.RULE_REF )
            {
                if ( argsAST != null )
                {
                    // rule[args]; ref has args
                    if ( r != null && r.ArgActionAST == null )
                    {
                        // but rule def has no args
                        ErrorManager.GrammarError(
                            ErrorManager.MSG_RULE_HAS_NO_ARGS,
                            grammar,
                            argsAST.Token,
                            r.Name );
                    }
                }
                else
                {
                    // rule ref has no args
                    if ( r != null && r.ArgActionAST != null )
                    {
                        // but rule def has args
                        ErrorManager.GrammarError(
                            ErrorManager.MSG_MISSING_RULE_ARGS,
                            grammar,
                            refAST.Token,
                            r.Name );
                    }
                }
            }
            else if ( refAST.Type == ANTLRParser.TOKEN_REF )
            {
                if ( grammar.type != GrammarType.Lexer )
                {
                    if ( argsAST != null )
                    {
                        // args on a token ref not in a lexer rule
                        ErrorManager.GrammarError(
                            ErrorManager.MSG_ARGS_ON_TOKEN_REF,
                            grammar,
                            refAST.Token,
                            refAST.Text );
                    }
                    return; // ignore token refs in nonlexers
                }
                if ( argsAST != null )
                {
                    // tokenRef[args]; ref has args
                    if ( r != null && r.ArgActionAST == null )
                    {
                        // but token rule def has no args
                        ErrorManager.GrammarError(
                            ErrorManager.MSG_RULE_HAS_NO_ARGS,
                            grammar,
                            argsAST.Token,
                            r.Name );
                    }
                }
                else
                {
                    // token ref has no args
                    if ( r != null && r.ArgActionAST != null )
                    {
                        // but token rule def has args
                        ErrorManager.GrammarError(
                            ErrorManager.MSG_MISSING_RULE_ARGS,
                            grammar,
                            refAST.Token,
                            r.Name );
                    }
                }
            }
        }

        /** Rules in tree grammar that use -> rewrites and are spitting out
         *  templates via output=template and then use rewrite=true must only
         *  use -> on alts that are simple nodes or trees or single rule refs
         *  that match either nodes or trees.  The altAST is the ALT node
         *  for an ALT.  Verify that its first child is simple.  Must be either
         *  ( ALT ^( A B ) &lt;end-of-alt&gt; ) or ( ALT A &lt;end-of-alt&gt; ) or
         *  other element.
         *
         *  Ignore predicates in front and labels.
         */
        public virtual void EnsureAltIsSimpleNodeOrTree( GrammarAST altAST,
                                                GrammarAST elementAST,
                                                int outerAltNum )
        {
            if ( IsValidSimpleElementNode( elementAST ) )
            {
                GrammarAST next = (GrammarAST)elementAST.Parent.GetChild(elementAST.ChildIndex + 1);
                if ( !IsNextNonActionElementEOA( next ) )
                {
                    ErrorManager.GrammarWarning( ErrorManager.MSG_REWRITE_FOR_MULTI_ELEMENT_ALT,
                                                grammar,
                                                next.Token,
                                                outerAltNum );
                }
                return;
            }
            switch ( elementAST.Type )
            {
            case ANTLRParser.ASSIGN:		// labels ok on non-rule refs
            case ANTLRParser.PLUS_ASSIGN:
                if ( IsValidSimpleElementNode( (GrammarAST)elementAST.GetChild( 1 ) ) )
                {
                    return;
                }
                break;
            case ANTLRParser.ACTION:		// skip past actions
            case ANTLRParser.SEMPRED:
            case ANTLRParser.SYN_SEMPRED:
            case ANTLRParser.BACKTRACK_SEMPRED:
            case ANTLRParser.GATED_SEMPRED:
                EnsureAltIsSimpleNodeOrTree( altAST,
                                            (GrammarAST)elementAST.Parent.GetChild(elementAST.ChildIndex + 1),
                                            outerAltNum );
                return;
            }
            ErrorManager.GrammarWarning( ErrorManager.MSG_REWRITE_FOR_MULTI_ELEMENT_ALT,
                                        grammar,
                                        elementAST.Token,
                                        outerAltNum );
        }

        protected virtual bool IsValidSimpleElementNode( GrammarAST t )
        {
            switch ( t.Type )
            {
            case ANTLRParser.TREE_BEGIN:
            case ANTLRParser.TOKEN_REF:
            case ANTLRParser.CHAR_LITERAL:
            case ANTLRParser.STRING_LITERAL:
            case ANTLRParser.WILDCARD:
                return true;
            default:
                return false;
            }
        }

        protected virtual bool IsNextNonActionElementEOA( GrammarAST t )
        {
            while ( t.Type == ANTLRParser.ACTION ||
                    t.Type == ANTLRParser.SEMPRED )
            {
                t = (GrammarAST)t.Parent.GetChild(t.ChildIndex + 1);
            }

            if ( t.Type == ANTLRParser.EOA )
            {
                return true;
            }

            return false;
        }
    }
}
