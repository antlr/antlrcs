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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Antlr.Runtime.JavaExtensions;
    using Antlr3.Misc;

    using ANTLRParser = Antlr3.Grammars.ANTLRParser;
    using ErrorManager = Antlr3.Tool.ErrorManager;
    using Grammar = Antlr3.Tool.Grammar;
    using GrammarAST = Antlr3.Tool.GrammarAST;
    using GrammarType = Antlr3.Tool.GrammarType;
    using IList = System.Collections.IList;
    using IToken = Antlr.Runtime.IToken;
    using Obsolete = System.ObsoleteAttribute;
    using StringBuilder = System.Text.StringBuilder;

    /** Collection of information about what is wrong with a decision as
     *  discovered while building the DFA predictor.
     *
     *  The information is collected during NFA->DFA conversion and, while
     *  some of this is available elsewhere, it is nice to have it all tracked
     *  in one spot so a great error message can be easily had.  I also like
     *  the fact that this object tracks it all for later perusing to make an
     *  excellent error message instead of lots of imprecise on-the-fly warnings
     *  (during conversion).
     *
     *  A decision normally only has one problem; e.g., some input sequence
     *  can be matched by multiple alternatives.  Unfortunately, some decisions
     *  such as
     *
     *  a : ( A | B ) | ( A | B ) | A ;
     *
     *  have multiple problems.  So in general, you should approach a decision
     *  as having multiple flaws each one uniquely identified by a DFAState.
     *  For example, statesWithSyntacticallyAmbiguousAltsSet tracks the set of
     *  all DFAStates where ANTLR has discovered a problem.  Recall that a decision
     *  is represented internall with a DFA comprised of multiple states, each of
     *  which could potentially have problems.
     *
     *  Because of this, you need to iterate over this list of DFA states.  You'll
     *  note that most of the informational methods like
     *  getSampleNonDeterministicInputSequence() require a DFAState.  This state
     *  will be one of the iterated states from stateToSyntacticallyAmbiguousAltsSet.
     *
     *  This class is not thread safe due to shared use of visited maps etc...
     *  Only one thread should really need to access one DecisionProbe anyway.
     */
    public class DecisionProbe
    {
        internal DFA dfa;

        /** Track all DFA states with nondeterministic alternatives.
         *  By reaching the same DFA state, a path through the NFA for some input
         *  is able to reach the same NFA state by starting at more than one
         *  alternative's left edge.  Though, later, we may find that predicates
         *  resolve the issue, but track info anyway.
         *  Note that from the DFA state, you can ask for
         *  which alts are nondeterministic.
         */
        ICollection<DFAState> _statesWithSyntacticallyAmbiguousAltsSet = new HashSet<DFAState>();

        /** Track just like stateToSyntacticallyAmbiguousAltsMap, but only
         *  for nondeterminisms that arise in the Tokens rule such as keyword vs
         *  ID rule.  The state maps to the list of Tokens rule alts that are
         *  in conflict.
         */
        internal IDictionary<DFAState, ICollection<int>> stateToSyntacticallyAmbiguousTokensRuleAltsMap =
            new Dictionary<DFAState, ICollection<int>>();

        /** Was a syntactic ambiguity resolved with predicates?  Any DFA
         *  state that predicts more than one alternative, must be resolved
         *  with predicates or it should be reported to the user.
         */
        ICollection<DFAState> _statesResolvedWithSemanticPredicatesSet = new HashSet<DFAState>();

        /** Track the predicates for each alt per DFA state;
         *  more than one DFA state might have syntactically ambig alt prediction.
         *  Maps DFA state to another map, mapping alt number to a
         *  SemanticContext (pred(s) to execute to resolve syntactic ambiguity).
         */
        IDictionary<DFAState, IDictionary<int, SemanticContext>> _stateToAltSetWithSemanticPredicatesMap =
            new Dictionary<DFAState, IDictionary<int, SemanticContext>>();

        /** Tracks alts insufficiently covered.
         *  For example, p1||true gets reduced to true and so leaves
         *  whole alt uncovered.  This maps DFA state to the set of alts
         */
        IDictionary<DFAState, IDictionary<int, ICollection<IToken>>> _stateToIncompletelyCoveredAltsMap =
            new Dictionary<DFAState, IDictionary<int, ICollection<IToken>>>();

        /** The set of states w/o emanating edges and w/o resolving sem preds. */
        ICollection<DFAState> _danglingStates = new HashSet<DFAState>();

        /** The overall list of alts within the decision that have at least one
         *  conflicting input sequence.
         */
        ICollection<int> _altsWithProblem = new HashSet<int>();

        /** If decision with > 1 alt has recursion in > 1 alt, it's nonregular
         *  lookahead.  The decision cannot be made with a DFA.
         *  the alts are stored in altsWithProblem.
         */
        bool _nonLLStarDecision = false;

        /** Recursion is limited to a particular depth.  If that limit is exceeded
         *  the proposed new NFAConfiguration is recorded for the associated DFA state.
         */
        MultiMap<int, NFAConfiguration> _stateToRecursionOverflowConfigurationsMap =
            new MultiMap<int, NFAConfiguration>();

#if false
        protected IDictionary<int, List<NFAConfiguration>> stateToRecursionOverflowConfigurationsMap =
            new Dictionary<int, List<NFAConfiguration>>();
#endif

#if false
        /** Left recursion discovered.  The proposed new NFAConfiguration
         *  is recorded for the associated DFA state.
         */
        protected IDictionary<int, List<NFAConfiguration>> stateToLeftRecursiveConfigurationsMap =
            new Dictionary<int, List<NFAConfiguration>>();
#endif

        /** Did ANTLR have to terminate early on the analysis of this decision? */
        bool _timedOut = false;

        /** Used to find paths through syntactically ambiguous DFA. If we've
         *  seen statement number before, what did we learn?
         */
        IDictionary<int, int> _stateReachable;

        public const int REACHABLE_BUSY = -1;
        public const int REACHABLE_NO = 0;
        public const int REACHABLE_YES = 1;

        /** Used while finding a path through an NFA whose edge labels match
         *  an input sequence.  Tracks the input position
         *  we were at the last time at this node.  If same input position, then
         *  we'd have reached same state without consuming input...probably an
         *  infinite loop.  Stop.  Set<String>.  The strings look like
         *  stateNumber_labelIndex.
         */
        ICollection<string> _statesVisitedAtInputDepth;

        ICollection<int> _statesVisitedDuringSampleSequence;

        public static bool verbose = false;

        public DecisionProbe( DFA dfa )
        {
            this.dfa = dfa;
        }

        #region Properties
        /** Took too long to analyze a DFA */
        public bool AnalysisOverflowed
        {
            get
            {
                return _stateToRecursionOverflowConfigurationsMap.Count > 0;
            }
        }
        /** Did the analysis complete it's work? */
        public bool AnalysisTimedOut
        {
            get
            {
                return _timedOut;
            }
        }
        /** return set of states w/o emanating edges and w/o resolving sem preds.
         *  These states come about because the analysis algorithm had to
         *  terminate early to avoid infinite recursion for example (due to
         *  left recursion perhaps).
         */
        public ICollection<DFAState> DanglingStates
        {
            get
            {
                return _danglingStates;
            }
        }
        /** Return a string like "3:22: ( A {;} | B )" that describes this
         *  decision.
         */
        public string Description
        {
            get
            {
                return dfa.NFADecisionStartState.Description;
            }
        }
        /** Return all DFA states in this DFA that have NFA configurations that
         *  conflict.  You must report a problem for each state in this set
         *  because each state represents a different input sequence.
         */
        public ICollection<DFAState> DFAStatesWithSyntacticallyAmbiguousAlts
        {
            get
            {
                return _statesWithSyntacticallyAmbiguousAltsSet;
            }
        }
        /** At least one alt refs a sem or syn pred */
        public bool HasPredicate
        {
            get
            {
                return _stateToAltSetWithSemanticPredicatesMap.Count > 0;
            }
        }
        public bool IsCyclic
        {
            get
            {
                return dfa.IsCyclic;
            }
        }
        /** If no states are dead-ends, no alts are unreachable, there are
         *  no nondeterminisms unresolved by syn preds, all is ok with decision.
         */
        public bool IsDeterministic
        {
            get
            {
                if ( _danglingStates.Count == 0 &&
                     _statesWithSyntacticallyAmbiguousAltsSet.Count == 0 &&
                     dfa.UnreachableAlts.Count == 0 )
                {
                    return true;
                }

                if ( _statesWithSyntacticallyAmbiguousAltsSet.Count > 0 )
                {
                    return _statesWithSyntacticallyAmbiguousAltsSet
                        .Except( _statesResolvedWithSemanticPredicatesSet )
                        .Take( 1 )
                        .Count() == 0;
                }

                return false;
            }
        }
        /** Found recursion in > 1 alt */
        public bool IsNonLLStarDecision
        {
            get
            {
                return _nonLLStarDecision;
            }
        }
        public bool IsReduced
        {
            get
            {
                return dfa.IsReduced;
            }
        }
        public ICollection<int> NonDeterministicAlts
        {
            get
            {
                return _altsWithProblem;
            }
        }
        public ICollection<DFAState> NondeterministicStatesResolvedWithSemanticPredicate
        {
            get
            {
                return _statesResolvedWithSemanticPredicatesSet;
            }
        }
        /** How many states does the DFA predictor have? */
        public int NumberOfStates
        {
            get
            {
                return dfa.NumberOfStates;
            }
        }
        /** Get a list of all unreachable alternatives for this decision.  There
         *  may be multiple alternatives with ambiguous input sequences, but this
         *  is the overall list of unreachable alternatives (either due to
         *  conflict resolution or alts w/o accept states).
         */
        public ICollection<int> UnreachableAlts
        {
            get
            {
                return dfa.UnreachableAlts;
            }
        }
        #endregion

        // I N F O R M A T I O N  A B O U T  D E C I S I O N

        /** Return the sorted list of alts that conflict within a single state.
         *  Note that predicates may resolve the conflict.
         */
        public virtual IList<int> GetNonDeterministicAltsForState( DFAState targetState )
        {
            IEnumerable<int> nondetAlts = targetState.GetNonDeterministicAlts();
            if ( nondetAlts == null )
                return null;

            return nondetAlts.OrderBy( i => i ).ToList();

            //HashSet<int> nondetAlts = targetState.getNonDeterministicAlts();
            //if ( nondetAlts == null )
            //{
            //    return null;
            //}
            //List sorted = new LinkedList();
            //sorted.addAll( nondetAlts );
            //Collections.sort( sorted ); // make sure it's 1, 2, ...
            //return sorted;
        }

        /** Which alts were specifically turned off to resolve nondeterminisms?
         *  This is different than the unreachable alts.  Disabled doesn't mean that
         *  the alternative is totally unreachable necessarily, it just means
         *  that for this DFA state, that alt is disabled.  There may be other
         *  accept states for that alt that make an alt reachable.
         */
        public virtual ICollection<int> GetDisabledAlternatives( DFAState d )
        {
            return d.DisabledAlternatives;
        }

        /** If a recursion overflow is resolve with predicates, then we need
         *  to shut off the warning that would be generated.
         */
        public virtual void RemoveRecursiveOverflowState( DFAState d )
        {
            _stateToRecursionOverflowConfigurationsMap.Remove( d.stateNumber );
        }

        /** Return a IList<Label> indicating an input sequence that can be matched
         *  from the start state of the DFA to the targetState (which is known
         *  to have a problem).
         */
        public virtual IList<Label> GetSampleNonDeterministicInputSequence( DFAState targetState )
        {
            HashSet<object> dfaStates = GetDFAPathStatesToTarget( targetState );
            _statesVisitedDuringSampleSequence = new HashSet<int>();
            IList<Label> labels = new List<Label>(); // may access ith element; use array
            if ( dfa == null || dfa.startState == null )
            {
                return labels;
            }
            GetSampleInputSequenceUsingStateSet( dfa.startState,
                                                targetState,
                                                dfaStates,
                                                labels );
            return labels;
        }

        /** Given IList&lt;Label&gt;, return a String with a useful representation
         *  of the associated input string.  One could show something different
         *  for lexers and parsers, for example.
         */
        public virtual String GetInputSequenceDisplay( IList<Label> labels )
        {
            Grammar g = dfa.nfa.grammar;
            StringBuilder buf = new StringBuilder();
            foreach ( Label label in labels )
            {
                buf.Append( label.ToString( g ) );
                if ( /*it.hasNext() &&*/ g.type != GrammarType.Lexer )
                {
                    buf.Append( ' ' );
                }
            }

            // remove the final appended space
            if ( g.type != GrammarType.Lexer )
                buf.Length = buf.Length - 1;

            return buf.ToString();
        }

        /** Given an alternative associated with a nondeterministic DFA state,
         *  find the path of NFA states associated with the labels sequence.
         *  Useful tracing where in the NFA, a single input sequence can be
         *  matched.  For different alts, you should get different NFA paths.
         *
         *  The first NFA state for all NFA paths will be the same: the starting
         *  NFA state of the first nondeterministic alt.  Imagine (A|B|A|A):
         *
         * 	5->9-A->o
         *  |
         *  6->10-B->o
         *  |
         *  7->11-A->o
         *  |
         *  8->12-A->o
         *
         *  There are 3 nondeterministic alts.  The paths should be:
         *  5 9 ...
         *  5 6 7 11 ...
         *  5 6 7 8 12 ...
         *
         *  The NFA path matching the sample input sequence (labels) is computed
         *  using states 9, 11, and 12 rather than 5, 7, 8 because state 5, for
         *  example can get to all ambig paths.  Must isolate for each alt (hence,
         *  the extra state beginning each alt in my NFA structures).  Here,
         *  firstAlt=1.
         */
        public virtual IList GetNFAPathStatesForAlt( int firstAlt,
                                           int alt,
                                           IList<Label> labels )
        {
            NFAState nfaStart = dfa.NFADecisionStartState;
            List<NFAState> path = new List<NFAState>();
            // first add all NFA states leading up to altStart state
            for ( int a = firstAlt; a <= alt; a++ )
            {
                NFAState s =
                    dfa.nfa.grammar.GetNFAStateForAltOfDecision( nfaStart, a );
                path.Add( s );
            }

            // add first state of actual alt
            NFAState altStart = dfa.nfa.grammar.GetNFAStateForAltOfDecision( nfaStart, alt );
            NFAState isolatedAltStart = (NFAState)altStart.transition[0].target;
            path.Add( isolatedAltStart );

            // add the actual path now
            _statesVisitedAtInputDepth = new HashSet<string>();
            GetNFAPath( isolatedAltStart,
                       0,
                       labels,
                       path );
            return path;
        }

        /** Each state in the DFA represents a different input sequence for an
         *  alt of the decision.  Given a DFA state, what is the semantic
         *  predicate context for a particular alt.
         */
        public virtual SemanticContext GetSemanticContextForAlt( DFAState d, int alt )
        {
            var altToPredMap = _stateToAltSetWithSemanticPredicatesMap.get( d );
            if ( altToPredMap == null )
            {
                return null;
            }
            return altToPredMap.get( alt );
        }

        /** Return a list of alts whose predicate context was insufficient to
         *  resolve a nondeterminism for state d.
         */
        public virtual IDictionary<int, ICollection<IToken>> GetIncompletelyCoveredAlts( DFAState d )
        {
            return _stateToIncompletelyCoveredAltsMap.get( d );
        }

        public virtual void IssueWarnings()
        {
            // NONREGULAR DUE TO RECURSION > 1 ALTS
            // Issue this before aborted analysis, which might also occur
            // if we take too long to terminate
            if ( _nonLLStarDecision && !dfa.AutoBacktrackMode )
            {
                ErrorManager.NonLLStarDecision( this );
            }

            if ( AnalysisTimedOut )
            {
                // only report early termination errors if !backtracking
                if ( !dfa.AutoBacktrackMode )
                {
                    ErrorManager.AnalysisAborted( this );
                }
                // now just return...if we bailed out, don't spew other messages
                return;
            }

            IssueRecursionWarnings();

            // generate a separate message for each problem state in DFA
            ICollection<DFAState> resolvedStates = NondeterministicStatesResolvedWithSemanticPredicate;
            ICollection<DFAState> problemStates = DFAStatesWithSyntacticallyAmbiguousAlts;
            if ( problemStates.Count > 0 )
            {
                foreach ( DFAState d in problemStates )
                {
                    if ( dfa.nfa.grammar.NFAToDFAConversionExternallyAborted() )
                        break;

                    IDictionary<int, ICollection<IToken>> insufficientAltToLocations = GetIncompletelyCoveredAlts( d );
                    if ( insufficientAltToLocations != null && insufficientAltToLocations.Count > 0 )
                    {
                        ErrorManager.InsufficientPredicates( this, d, insufficientAltToLocations );
                    }
                    // don't report problem if resolved
                    if ( resolvedStates == null || !resolvedStates.Contains( d ) )
                    {
                        // first strip last alt from disableAlts if it's wildcard
                        // then don't print error if no more disable alts
                        ICollection<int> disabledAlts = GetDisabledAlternatives( d );
                        StripWildCardAlts( disabledAlts );
                        if ( disabledAlts.Count > 0 )
                        {
                            ErrorManager.Nondeterminism( this, d );
                        }
                    }
                }
            }

            ICollection<DFAState> danglingStates = DanglingStates;
            if ( danglingStates.Count > 0 )
            {
                //Console.Error.WriteLine( "no emanating edges for states: " + danglingStates );
                foreach ( DFAState d in danglingStates )
                {
                    ErrorManager.DanglingState( this, d );
                }
            }

            if ( !_nonLLStarDecision )
            {
                var unreachableAlts = dfa.UnreachableAlts;
                if ( unreachableAlts != null && unreachableAlts.Count > 0 )
                {
                    // give different msg if it's an empty Tokens rule from delegate
                    bool isInheritedTokensRule = false;
                    if ( dfa.IsTokensRuleDecision )
                    {
                        foreach ( int altI in unreachableAlts )
                        {
                            GrammarAST decAST = dfa.DecisionASTNode;
                            GrammarAST altAST = (GrammarAST)decAST.GetChild( altI - 1 );
                            GrammarAST delegatedTokensAlt =
                                (GrammarAST)altAST.GetFirstChildWithType( ANTLRParser.DOT );
                            if ( delegatedTokensAlt != null )
                            {
                                isInheritedTokensRule = true;
                                ErrorManager.GrammarWarning( ErrorManager.MSG_IMPORTED_TOKENS_RULE_EMPTY,
                                                            dfa.nfa.grammar,
                                                            null,
                                                            dfa.nfa.grammar.name,
                                                            delegatedTokensAlt.GetChild( 0 ).Text );
                            }
                        }
                    }
                    if ( isInheritedTokensRule )
                    {
                    }
                    else
                    {
                        ErrorManager.UnreachableAlts( this, unreachableAlts );
                    }
                }
            }
        }

        /** Get the last disabled alt number and check in the grammar to see
         *  if that alt is a simple wildcard.  If so, treat like an else clause
         *  and don't emit the error.  Strip out the last alt if it's wildcard.
         */
        protected virtual void StripWildCardAlts( ICollection<int> disabledAlts )
        {
            List<int> sortedDisableAlts = new List<int>( disabledAlts );
            sortedDisableAlts.Sort();
            //Collections.sort( sortedDisableAlts );
            int lastAlt =
                (int)sortedDisableAlts[sortedDisableAlts.Count - 1];
            GrammarAST blockAST =
                dfa.nfa.grammar.GetDecisionBlockAST( dfa.decisionNumber );
            //JSystem.@out.println("block with error = "+blockAST.toStringTree());
            GrammarAST lastAltAST = null;
            if ( blockAST.GetChild( 0 ).Type == ANTLRParser.OPTIONS )
            {
                // if options, skip first child: ( options { ( = greedy false ) )
                lastAltAST = (GrammarAST)blockAST.GetChild( lastAlt );
            }
            else
            {
                lastAltAST = (GrammarAST)blockAST.GetChild( lastAlt - 1 );
            }
            //JSystem.@out.println("last alt is "+lastAltAST.toStringTree());
            // if last alt looks like ( ALT . <end-of-alt> ) then wildcard
            // Avoid looking at optional blocks etc... that have last alt
            // as the EOB:
            // ( BLOCK ( ALT 'else' statement <end-of-alt> ) <end-of-block> )
            if ( lastAltAST.Type != ANTLRParser.EOB &&
                 lastAltAST.GetChild( 0 ).Type == ANTLRParser.WILDCARD &&
                 lastAltAST.GetChild( 1 ).Type == ANTLRParser.EOA )
            {
                //JSystem.@out.println("wildcard");
                disabledAlts.Remove( lastAlt );
            }
        }

        protected virtual void IssueRecursionWarnings()
        {
            // RECURSION OVERFLOW
            ICollection<int> dfaStatesWithRecursionProblems =
                _stateToRecursionOverflowConfigurationsMap.Keys;
            // now walk truly unique (unaliased) list of dfa states with inf recur
            // Goal: create a map from alt to map<target,IList<callsites>>
            // Map<Map<String target, IList<NFAState call sites>>
            IDictionary<int, IDictionary<string, ICollection<NFAState>>> altToTargetToCallSitesMap =
                new Dictionary<int, IDictionary<string, ICollection<NFAState>>>();
            // track a single problem DFA state for each alt
            var altToDFAState = new Dictionary<int, DFAState>();
            ComputeAltToProblemMaps( dfaStatesWithRecursionProblems,
                                    _stateToRecursionOverflowConfigurationsMap,
                                    altToTargetToCallSitesMap, // output param
                                    altToDFAState );            // output param

            // walk each alt with recursion overflow problems and generate error
            ICollection<int> alts = altToTargetToCallSitesMap.Keys;
            List<int> sortedAlts = new List<int>( alts );
            sortedAlts.Sort();
            //Collections.sort( sortedAlts );
            foreach ( int altI in sortedAlts )
            {
                var targetToCallSiteMap =
                    altToTargetToCallSitesMap.get( altI );
                var targetRules = targetToCallSiteMap.Keys;
                var callSiteStates = targetToCallSiteMap.Values;
                DFAState sampleBadState = (DFAState)altToDFAState.get( altI );
                ErrorManager.RecursionOverflow( this,
                                               sampleBadState,
                                               altI,
                                               targetRules,
                                               callSiteStates );
            }
        }

        private void ComputeAltToProblemMaps( IEnumerable<int> dfaStatesUnaliased,
                                             IDictionary<int,IList<NFAConfiguration>> configurationsMap,
                                             IDictionary<int,IDictionary<string,ICollection<NFAState>>> altToTargetToCallSitesMap,
                                             IDictionary<int, DFAState> altToDFAState )
        {
            foreach ( int stateI in dfaStatesUnaliased )
            {
                // walk this DFA's config list
                IList configs = (IList)configurationsMap.get( stateI );
                for ( int i = 0; i < configs.Count; i++ )
                {
                    NFAConfiguration c = (NFAConfiguration)configs[i];
                    NFAState ruleInvocationState = dfa.nfa.GetState( c.state );
                    Transition transition0 = ruleInvocationState.transition[0];
                    RuleClosureTransition @ref = (RuleClosureTransition)transition0;
                    String targetRule = ( (NFAState)@ref.target ).enclosingRule.Name;
                    int altI = c.alt;
                    IDictionary<string, ICollection<NFAState>> targetToCallSiteMap =
                        altToTargetToCallSitesMap.get( altI );
                    if ( targetToCallSiteMap == null )
                    {
                        targetToCallSiteMap = new Dictionary<string, ICollection<NFAState>>();
                        altToTargetToCallSitesMap[altI] = targetToCallSiteMap;
                    }
                    ICollection<NFAState> callSites = targetToCallSiteMap.get( targetRule );
                    if ( callSites == null )
                    {
                        callSites = new HashSet<NFAState>();
                        targetToCallSiteMap[targetRule] = callSites;
                    }
                    callSites.Add( ruleInvocationState );
                    // track one problem DFA state per alt
                    if ( altToDFAState.get( altI ) == null )
                    {
                        DFAState sampleBadState = dfa.GetState( stateI );
                        altToDFAState[altI] = sampleBadState;
                    }
                }
            }
        }

        private HashSet<object> GetUnaliasedDFAStateSet( HashSet<object> dfaStatesWithRecursionProblems )
        {
            HashSet<object> dfaStatesUnaliased = new HashSet<object>();
            foreach ( int stateI in dfaStatesWithRecursionProblems )
            {
                DFAState d = dfa.GetState( stateI );
                dfaStatesUnaliased.Add( d.stateNumber );
            }
            return dfaStatesUnaliased;
        }


        // T R A C K I N G  M E T H O D S

        /** Report the fact that DFA state d is not a state resolved with
         *  predicates and yet it has no emanating edges.  Usually this
         *  is a result of the closure/reach operations being unable to proceed
         */
        public virtual void ReportDanglingState( DFAState d )
        {
            _danglingStates.Add( d );
        }

        public virtual void ReportAnalysisTimeout()
        {
            _timedOut = true;
            dfa.nfa.grammar.setOfDFAWhoseAnalysisTimedOut.Add( dfa );
        }

        /** Report that at least 2 alts have recursive constructs.  There is
         *  no way to build a DFA so we terminated.
         */
        public virtual void ReportNonLLStarDecision( DFA dfa )
        {
            /*
            JSystem.@out.println("non-LL(*) DFA "+dfa.decisionNumber+", alts: "+
                               dfa.recursiveAltSet.toList());
                               */
            _nonLLStarDecision = true;
            _altsWithProblem.addAll( dfa.recursiveAltSet.ToList() );
        }

        public virtual void ReportRecursionOverflow( DFAState d,
                                            NFAConfiguration recursionNFAConfiguration )
        {
            // track the state number rather than the state as d will change
            // out from underneath us; hash wouldn't return any value

            // left-recursion is detected in start state.  Since we can't
            // call resolveNondeterminism() on the start state (it would
            // not look k=1 to get min single token lookahead), we must
            // prevent errors derived from this state.  Avoid start state
            if ( d.stateNumber > 0 )
            {
                int stateI = d.stateNumber;
                _stateToRecursionOverflowConfigurationsMap.Map( stateI, recursionNFAConfiguration );
            }
        }

        public virtual void ReportNondeterminism( DFAState d, HashSet<int> nondeterministicAlts )
        {
            _altsWithProblem.addAll( nondeterministicAlts ); // track overall list
            _statesWithSyntacticallyAmbiguousAltsSet.Add( d );
            dfa.nfa.grammar.setOfNondeterministicDecisionNumbers.Add(
                dfa.DecisionNumber
            );
        }

        /** Currently the analysis reports issues between token definitions, but
         *  we don't print out warnings in favor of just picking the first token
         *  definition found in the grammar ala lex/flex.
         */
        public virtual void ReportLexerRuleNondeterminism( DFAState d, HashSet<int> nondeterministicAlts )
        {
            stateToSyntacticallyAmbiguousTokensRuleAltsMap[d] = nondeterministicAlts;
        }

        public virtual void ReportNondeterminismResolvedWithSemanticPredicate( DFAState d )
        {
            // First, prevent a recursion warning on this state due to
            // pred resolution
            if ( d.abortedDueToRecursionOverflow )
            {
                d.dfa.probe.RemoveRecursiveOverflowState( d );
            }
            _statesResolvedWithSemanticPredicatesSet.Add( d );
            //JSystem.@out.println("resolved with pred: "+d);
            dfa.nfa.grammar.setOfNondeterministicDecisionNumbersResolvedWithPredicates.Add(
                dfa.DecisionNumber
            );
        }

        /** Report the list of predicates found for each alternative; copy
         *  the list because this set gets altered later by the method
         *  tryToResolveWithSemanticPredicates() while flagging NFA configurations
         *  in d as resolved.
         */
        public virtual void ReportAltPredicateContext( DFAState d, IDictionary<int, SemanticContext> altPredicateContext )
        {
            IDictionary<int, SemanticContext> copy = new Dictionary<int, SemanticContext>( altPredicateContext );
            //copy.putAll( altPredicateContext );
            _stateToAltSetWithSemanticPredicatesMap[d] = copy;
        }

        public virtual void ReportIncompletelyCoveredAlts( DFAState d,
                                                  IDictionary<int, ICollection<IToken>> altToLocationsReachableWithoutPredicate )
        {
            _stateToIncompletelyCoveredAltsMap[d] = altToLocationsReachableWithoutPredicate;
        }

        // S U P P O R T

        /** Given a start state and a target state, return true if start can reach
         *  target state.  Also, compute the set of DFA states
         *  that are on a path from start to target; return in states parameter.
         */
        protected virtual bool ReachesState( DFAState startState,
                                       DFAState targetState,
                                       HashSet<object> states )
        {
            if ( startState == targetState )
            {
                states.Add( targetState );
                //JSystem.@out.println("found target DFA state "+targetState.getStateNumber());
                _stateReachable[startState.stateNumber] = REACHABLE_YES;
                return true;
            }

            DFAState s = startState;
            // avoid infinite loops
            _stateReachable[s.stateNumber] = REACHABLE_BUSY;

            // look for a path to targetState among transitions for this state
            // stop when you find the first one; I'm pretty sure there is
            // at most one path to any DFA state with conflicting predictions
            for ( int i = 0; i < s.NumberOfTransitions; i++ )
            {
                Transition t = s.Transition( i );
                DFAState edgeTarget = (DFAState)t.target;

                int targetStatus; //= stateReachable.get( edgeTarget.stateNumber );
                if ( _stateReachable.TryGetValue( edgeTarget.stateNumber, out targetStatus ) )
                {
                    if ( targetStatus == REACHABLE_BUSY )
                    { // avoid cycles; they say nothing
                        continue;
                    }
                    if ( targetStatus == REACHABLE_YES )
                    { // return success!
                        _stateReachable[s.stateNumber] = REACHABLE_YES;
                        return true;
                    }
                    if ( targetStatus == REACHABLE_NO )
                    { // try another transition
                        continue;
                    }
                }

                // if null, target must be REACHABLE_UNKNOWN (i.e., unvisited)
                if ( ReachesState( edgeTarget, targetState, states ) )
                {
                    states.Add( s );
                    _stateReachable[s.stateNumber] = REACHABLE_YES;
                    return true;
                }
            }

            _stateReachable[s.stateNumber] = REACHABLE_NO;
            return false; // no path to targetState found.
        }

        protected virtual HashSet<object> GetDFAPathStatesToTarget( DFAState targetState )
        {
            HashSet<object> dfaStates = new HashSet<object>();
            _stateReachable = new Dictionary<int, int>();
            if ( dfa == null || dfa.startState == null )
            {
                return dfaStates;
            }
            bool reaches = ReachesState( dfa.startState, targetState, dfaStates );
            return dfaStates;
        }

        /** Given a start state and a final state, find a list of edge labels
         *  between the two ignoring epsilon.  Limit your scan to a set of states
         *  passed in.  This is used to show a sample input sequence that is
         *  nondeterministic with respect to this decision.  Return IList<Label> as
         *  a parameter.  The incoming states set must be all states that lead
         *  from startState to targetState and no others so this algorithm doesn't
         *  take a path that eventually leads to a state other than targetState.
         *  Don't follow loops, leading to short (possibly shortest) path.
         */
        protected virtual void GetSampleInputSequenceUsingStateSet( State startState,
                                                           State targetState,
                                                           HashSet<object> states,
                                                           IList<Label> labels )
        {
            _statesVisitedDuringSampleSequence.Add( startState.stateNumber );

            // pick the first edge in states as the one to traverse
            for ( int i = 0; i < startState.NumberOfTransitions; i++ )
            {
                Transition t = startState.GetTransition( i );
                DFAState edgeTarget = (DFAState)t.target;
                if ( states.Contains( edgeTarget ) &&
                     !_statesVisitedDuringSampleSequence.Contains( edgeTarget.stateNumber ) )
                {
                    labels.Add( t.label ); // traverse edge and track label
                    if ( edgeTarget != targetState )
                    {
                        // get more labels if not at target
                        GetSampleInputSequenceUsingStateSet( edgeTarget,
                                                            targetState,
                                                            states,
                                                            labels );
                    }
                    // done with this DFA state as we've found a good path to target
                    return;
                }
            }
            labels.Add( new Label( Label.EPSILON ) ); // indicate no input found
            // this happens on a : {p1}? a | A ;
            //ErrorManager.error(ErrorManager.MSG_CANNOT_COMPUTE_SAMPLE_INPUT_SEQ);
        }

        /** Given a sample input sequence, you usually would like to know the
         *  path taken through the NFA.  Return the list of NFA states visited
         *  while matching a list of labels.  This cannot use the usual
         *  interpreter, which does a deterministic walk.  We need to be able to
         *  take paths that are turned off during nondeterminism resolution. So,
         *  just do a depth-first walk traversing edges labeled with the current
         *  label.  Return true if a path was found emanating from state s.
         */
        protected virtual bool GetNFAPath( NFAState s,     // starting where?
                                     int labelIndex, // 0..labels.size()-1
                                     IList<Label> labels,    // input sequence
                                     IList<NFAState> path )      // output list of NFA states
        {
            // track a visit to state s at input index labelIndex if not seen
            String thisStateKey = GetStateLabelIndexKey( s.stateNumber, labelIndex );
            if ( _statesVisitedAtInputDepth.Contains( thisStateKey ) )
            {
                /*
                JSystem.@out.println("### already visited "+s.stateNumber+" previously at index "+
                               labelIndex);
                */
                return false;
            }
            _statesVisitedAtInputDepth.Add( thisStateKey );

            /*
            JSystem.@out.println("enter state "+s.stateNumber+" visited states: "+
                               statesVisitedAtInputDepth);
            */

            // pick the first edge whose target is in states and whose
            // label is labels[labelIndex]
            for ( int i = 0; i < s.NumberOfTransitions; i++ )
            {
                Transition t = s.transition[i];
                NFAState edgeTarget = (NFAState)t.target;
                Label label = (Label)labels[labelIndex];
                /*
                JSystem.@out.println(s.stateNumber+"-"+
                                   t.label.toString(dfa.nfa.grammar)+"->"+
                                   edgeTarget.stateNumber+" =="+
                                   label.toString(dfa.nfa.grammar)+"?");
                */
                if ( t.label.IsEpsilon || t.label.IsSemanticPredicate )
                {
                    // nondeterministically backtrack down epsilon edges
                    path.Add( edgeTarget );
                    bool found =
                        GetNFAPath( edgeTarget, labelIndex, labels, path );
                    if ( found )
                    {
                        _statesVisitedAtInputDepth.Remove( thisStateKey );
                        return true; // return to "calling" state
                    }
                    path.RemoveAt( path.Count - 1 ); // remove; didn't work out
                    continue; // look at the next edge
                }
                if ( t.label.Matches( label ) )
                {
                    path.Add( edgeTarget );
                    /*
                    JSystem.@out.println("found label "+
                                       t.label.toString(dfa.nfa.grammar)+
                                       " at state "+s.stateNumber+"; labelIndex="+labelIndex);
                    */
                    if ( labelIndex == labels.Count - 1 )
                    {
                        // found last label; done!
                        _statesVisitedAtInputDepth.Remove( thisStateKey );
                        return true;
                    }
                    // otherwise try to match remaining input
                    bool found =
                        GetNFAPath( edgeTarget, labelIndex + 1, labels, path );
                    if ( found )
                    {
                        _statesVisitedAtInputDepth.Remove( thisStateKey );
                        return true;
                    }
                    /*
                    JSystem.@out.println("backtrack; path from "+s.stateNumber+"->"+
                                       t.label.toString(dfa.nfa.grammar)+" didn't work");
                    */
                    path.RemoveAt( path.Count - 1 ); // remove; didn't work out
                    continue; // keep looking for a path for labels
                }
            }
            //JSystem.@out.println("no epsilon or matching edge; removing "+thisStateKey);
            // no edge was found matching label; is ok, some state will have it
            _statesVisitedAtInputDepth.Remove( thisStateKey );
            return false;
        }

        protected virtual String GetStateLabelIndexKey( int s, int i )
        {
            StringBuilder buf = new StringBuilder();
            buf.Append( s );
            buf.Append( '_' );
            buf.Append( i );
            return buf.ToString();
        }

        /** From an alt number associated with artificial Tokens rule, return
         *  the name of the token that is associated with that alt.
         */
        public virtual String GetTokenNameForTokensRuleAlt( int alt )
        {
            NFAState decisionState = dfa.NFADecisionStartState;
            NFAState altState =
                dfa.nfa.grammar.GetNFAStateForAltOfDecision( decisionState, alt );
            NFAState decisionLeft = (NFAState)altState.transition[0].target;
            RuleClosureTransition ruleCallEdge =
                (RuleClosureTransition)decisionLeft.transition[0];
            NFAState ruleStartState = (NFAState)ruleCallEdge.target;
            //JSystem.@out.println("alt = "+decisionLeft.getEnclosingRule());
            return ruleStartState.enclosingRule.Name;
        }

        public virtual void Reset()
        {
            _stateToRecursionOverflowConfigurationsMap.Clear();
        }
    }

}
