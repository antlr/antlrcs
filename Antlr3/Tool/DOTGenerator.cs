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

namespace Antlr3.Tool
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Antlr.Runtime.JavaExtensions;
    using Antlr3.Analysis;

    using AngleBracketTemplateLexer = Antlr3.ST.Language.AngleBracketTemplateLexer;
    using ANTLRParser = Antlr3.Grammars.ANTLRParser;
    using IList = System.Collections.IList;
    using StringBuffer = System.Text.StringBuilder;
    using StringTemplate = Antlr3.ST.StringTemplate;
    using StringTemplateGroup = Antlr3.ST.StringTemplateGroup;
    using Tool = Antlr3.AntlrTool;

    /** The DOT (part of graphviz) generation aspect. */
    public class DOTGenerator
    {
        internal bool STRIP_NONREDUCED_STATES = false;

        protected String arrowhead = "normal";
        protected String rankdir = "LR";

        /** Library of output templates; use <attrname> format */
        public static StringTemplateGroup stlib =
                new StringTemplateGroup( "toollib", typeof( AngleBracketTemplateLexer ) );

        /** To prevent infinite recursion when walking state machines, record
         *  which states we've visited.  Make a new set every time you start
         *  walking in case you reuse this object.
         */
        protected ICollection<object> markedStates = null;

        protected Grammar grammar;

        /** This aspect is associated with a grammar */
        public DOTGenerator( Grammar grammar )
        {
            this.grammar = grammar;
        }

        #region Properties
        public string ArrowheadType
        {
            get
            {
                return getArrowheadType();
            }
        }
        public string RankDir
        {
            get
            {
                return getRankdir();
            }
        }
        #endregion

        /** Return a String containing a DOT description that, when displayed,
         *  will show the incoming state machine visually.  All nodes reachable
         *  from startState will be included.
         */
        public virtual String getDOT( State startState )
        {
            if ( startState == null )
            {
                return null;
            }
            // The output DOT graph for visualization
            StringTemplate dot = null;
            markedStates = new HashSet<object>();
            if ( startState is DFAState )
            {
                dot = stlib.GetInstanceOf( "org/antlr/tool/templates/dot/dfa" );
                dot.SetAttribute( "startState",
                        startState.stateNumber );
                dot.SetAttribute( "useBox",
                                 Tool.internalOption_ShowNFAConfigsInDFA );
                walkCreatingDFADOT( dot, (DFAState)startState );
            }
            else
            {
                dot = stlib.GetInstanceOf( "org/antlr/tool/templates/dot/nfa" );
                dot.SetAttribute( "startState",
                        startState.stateNumber );
                walkRuleNFACreatingDOT( dot, startState );
            }
            dot.SetAttribute( "rankdir", rankdir );
            return dot.ToString();
        }

        /** Return a String containing a DOT description that, when displayed,
         *  will show the incoming state machine visually.  All nodes reachable
         *  from startState will be included.
        public String getRuleNFADOT(State startState) {
            // The output DOT graph for visualization
            StringTemplate dot = stlib.getInstanceOf("org/antlr/tool/templates/dot/nfa");

            markedStates = new HashSet();
            dot.setAttribute("startState",
                    Utils.integer(startState.stateNumber));
            walkRuleNFACreatingDOT(dot, startState);
            return dot.toString();
        }
         */

        /** Do a depth-first walk of the state machine graph and
         *  fill a DOT description template.  Keep filling the
         *  states and edges attributes.
         */
        protected virtual void walkCreatingDFADOT( StringTemplate dot,
                                          DFAState s )
        {
            if ( markedStates.Contains( s.stateNumber ) )
            {
                return; // already visited this node
            }

            markedStates.Add( s.stateNumber ); // mark this node as completed.

            // first add this node
            StringTemplate st;
            if ( s.IsAcceptState )
            {
                st = stlib.GetInstanceOf( "org/antlr/tool/templates/dot/stopstate" );
            }
            else
            {
                st = stlib.GetInstanceOf( "org/antlr/tool/templates/dot/state" );
            }
            st.SetAttribute( "name", getStateLabel( s ) );
            dot.SetAttribute( "states", st );

            // make a DOT edge for each transition
            for ( int i = 0; i < s.NumberOfTransitions; i++ )
            {
                Transition edge = (Transition)s.transition( i );
                /*
                System.out.println("dfa "+s.dfa.decisionNumber+
                    " edge from s"+s.stateNumber+" ["+i+"] of "+s.getNumberOfTransitions());
                */
                if ( STRIP_NONREDUCED_STATES )
                {
                    if ( edge.target is DFAState &&
                        ( (DFAState)edge.target ).AcceptStateReachable != DFA.REACHABLE_YES )
                    {
                        continue; // don't generate nodes for terminal states
                    }
                }
                st = stlib.GetInstanceOf( "org/antlr/tool/templates/dot/edge" );
                st.SetAttribute( "label", getEdgeLabel( edge ) );
                st.SetAttribute( "src", getStateLabel( s ) );
                st.SetAttribute( "target", getStateLabel( edge.target ) );
                st.SetAttribute( "arrowhead", arrowhead );
                dot.SetAttribute( "edges", st );
                walkCreatingDFADOT( dot, (DFAState)edge.target ); // keep walkin'
            }
        }

        /** Do a depth-first walk of the state machine graph and
         *  fill a DOT description template.  Keep filling the
         *  states and edges attributes.  We know this is an NFA
         *  for a rule so don't traverse edges to other rules and
         *  don't go past rule end state.
         */
        protected virtual void walkRuleNFACreatingDOT( StringTemplate dot,
                                              State s )
        {
            if ( markedStates.Contains( s ) )
            {
                return; // already visited this node
            }

            markedStates.Add( s ); // mark this node as completed.

            // first add this node
            StringTemplate stateST;
            if ( s.IsAcceptState )
            {
                stateST = stlib.GetInstanceOf( "org/antlr/tool/templates/dot/stopstate" );
            }
            else
            {
                stateST = stlib.GetInstanceOf( "org/antlr/tool/templates/dot/state" );
            }
            stateST.SetAttribute( "name", getStateLabel( s ) );
            dot.SetAttribute( "states", stateST );

            if ( s.IsAcceptState )
            {
                return; // don't go past end of rule node to the follow states
            }

            // special case: if decision point, then line up the alt start states
            // unless it's an end of block
            if ( ( (NFAState)s ).IsDecisionState )
            {
                GrammarAST n = ( (NFAState)s ).associatedASTNode;
                if ( n != null && n.Type != ANTLRParser.EOB )
                {
                    StringTemplate rankST = stlib.GetInstanceOf( "org/antlr/tool/templates/dot/decision-rank" );
                    NFAState alt = (NFAState)s;
                    while ( alt != null )
                    {
                        rankST.SetAttribute( "states", getStateLabel( alt ) );
                        if ( alt.transition[1] != null )
                        {
                            alt = (NFAState)alt.transition[1].target;
                        }
                        else
                        {
                            alt = null;
                        }
                    }
                    dot.SetAttribute( "decisionRanks", rankST );
                }
            }

            // make a DOT edge for each transition
            StringTemplate edgeST = null;
            for ( int i = 0; i < s.NumberOfTransitions; i++ )
            {
                Transition edge = (Transition)s.getTransition( i );
                if ( edge is RuleClosureTransition )
                {
                    RuleClosureTransition rr = ( (RuleClosureTransition)edge );
                    // don't jump to other rules, but display edge to follow node
                    edgeST = stlib.GetInstanceOf( "org/antlr/tool/templates/dot/edge" );
                    if ( rr.rule.grammar != grammar )
                    {
                        edgeST.SetAttribute( "label", "<" + rr.rule.grammar.name + "." + rr.rule.name + ">" );
                    }
                    else
                    {
                        edgeST.SetAttribute( "label", "<" + rr.rule.name + ">" );
                    }
                    edgeST.SetAttribute( "src", getStateLabel( s ) );
                    edgeST.SetAttribute( "target", getStateLabel( rr.followState ) );
                    edgeST.SetAttribute( "arrowhead", arrowhead );
                    dot.SetAttribute( "edges", edgeST );
                    walkRuleNFACreatingDOT( dot, rr.followState );
                    continue;
                }
                if ( edge.IsAction )
                {
                    edgeST = stlib.GetInstanceOf( "org/antlr/tool/templates/dot/action-edge" );
                }
                else if ( edge.IsEpsilon )
                {
                    edgeST = stlib.GetInstanceOf( "org/antlr/tool/templates/dot/epsilon-edge" );
                }
                else
                {
                    edgeST = stlib.GetInstanceOf( "org/antlr/tool/templates/dot/edge" );
                }
                edgeST.SetAttribute( "label", getEdgeLabel( edge ) );
                edgeST.SetAttribute( "src", getStateLabel( s ) );
                edgeST.SetAttribute( "target", getStateLabel( edge.target ) );
                edgeST.SetAttribute( "arrowhead", arrowhead );
                dot.SetAttribute( "edges", edgeST );
                walkRuleNFACreatingDOT( dot, edge.target ); // keep walkin'
            }
        }

#if false
        public void writeDOTFilesForAllRuleNFAs()
        {
            var rules = grammar.Rules;
            foreach ( var r in rules )
            {
                String ruleName = r.name;
                writeDOTFile(
                        ruleName,
                        getRuleNFADOT(grammar.getRuleStartState(ruleName)));
            }
        }
#endif

#if false
        public void writeDOTFilesForAllDecisionDFAs()
        {
            // for debugging, create a DOT file for each decision in
            // a directory named for the grammar.
            File grammarDir = new File( grammar.name + "_DFAs" );
            grammarDir.mkdirs();
            IList decisionList = grammar.getDecisionNFAStartStateList();
            if ( decisionList == null )
            {
                return;
            }
            int i = 1;
            Iterator iter = decisionList.iterator();
            foreach ( NFAState decisionState in decisionList )
            {
                DFA dfa = decisionState.getDecisionASTNode().getLookaheadDFA();
                if ( dfa != null )
                {
                    String dot = getDOT( dfa.startState );
                    writeDOTFile( grammarDir + "/dec-" + i, dot );
                }
                i++;
            }
        }
#endif

        /** Fix edge strings so they print out in DOT properly;
         *  generate any gated predicates on edge too.
         */
        protected virtual String getEdgeLabel( Transition edge )
        {
            String label = edge.label.ToString( grammar );
            label = label.Replace( "\\", "\\\\" );
            label = label.Replace( "\"", "\\\"" );
            label = label.Replace( "\n", "\\\\n" );
            label = label.Replace( "\r", "" );
            if ( label.Equals( Label.EPSILON_STR ) )
            {
                label = "e";
            }
            State target = edge.target;
            if ( !edge.IsSemanticPredicate && target is DFAState )
            {
                // look for gated predicates; don't add gated to simple sempred edges
                SemanticContext preds =
                    ( (DFAState)target ).getGatedPredicatesInNFAConfigurations();
                if ( preds != null )
                {
                    String predsStr = "";
                    predsStr = "&&{" +
                        preds.genExpr( grammar.generator,
                                      grammar.generator.Templates, null ).ToString()
                        + "}?";
                    label += predsStr;
                }
            }
            return label;
        }

        protected virtual String getStateLabel( State s )
        {
            if ( s == null )
            {
                return "null";
            }
            String stateLabel = s.stateNumber.ToString();
            if ( s is DFAState )
            {
                StringBuffer buf = new StringBuffer( 250 );
                buf.Append( 's' );
                buf.Append( s.stateNumber );
                if ( Tool.internalOption_ShowNFAConfigsInDFA )
                {
                    if ( s is DFAState )
                    {
                        if ( ( (DFAState)s ).abortedDueToRecursionOverflow )
                        {
                            buf.Append( "\\n" );
                            buf.Append( "abortedDueToRecursionOverflow" );
                        }
                    }
                    var alts = ( (DFAState)s ).AltSet;
                    if ( alts != null )
                    {
                        buf.Append( "\\n" );
                        // separate alts
                        //List altList = new ArrayList();
                        //altList.addAll( alts );
                        //Collections.sort( altList );
                        List<int> altList = alts.OrderBy( i => i ).ToList();
                        ICollection<NFAConfiguration> configurations = ( (DFAState)s ).nfaConfigurations;
                        for ( int altIndex = 0; altIndex < altList.Count; altIndex++ )
                        {
                            object altI = altList[altIndex];
                            int alt = (int)altI;
                            if ( altIndex > 0 )
                            {
                                buf.Append( "\\n" );
                            }
                            buf.Append( "alt" );
                            buf.Append( alt );
                            buf.Append( ':' );
                            // get a list of configs for just this alt
                            // it will help us print better later
                            IList configsInAlt = new List<object>();
                            foreach ( NFAConfiguration c in configurations )
                            {
                                if ( c.alt != alt )
                                    continue;
                                configsInAlt.Add( c );
                            }
                            int n = 0;
                            for ( int cIndex = 0; cIndex < configsInAlt.Count; cIndex++ )
                            {
                                NFAConfiguration c =
                                    (NFAConfiguration)configsInAlt[cIndex];
                                n++;
                                buf.Append( c.ToString( false ) );
                                if ( ( cIndex + 1 ) < configsInAlt.Count )
                                {
                                    buf.Append( ", " );
                                }
                                if ( n % 5 == 0 && ( configsInAlt.Count - cIndex ) > 3 )
                                {
                                    buf.Append( "\\n" );
                                }
                            }
                        }
                    }
                }
                stateLabel = buf.ToString();
            }
            if ( ( s is NFAState ) && ( (NFAState)s ).IsDecisionState )
            {
                stateLabel = stateLabel + ",d=" +
                        ( (NFAState)s ).DecisionNumber;
                if ( ( (NFAState)s ).endOfBlockStateNumber != State.INVALID_STATE_NUMBER )
                {
                    stateLabel += ",eob=" + ( (NFAState)s ).endOfBlockStateNumber;
                }
            }
            else if ( ( s is NFAState ) &&
                ( (NFAState)s ).endOfBlockStateNumber != State.INVALID_STATE_NUMBER )
            {
                NFAState n = ( (NFAState)s );
                stateLabel = stateLabel + ",eob=" + n.endOfBlockStateNumber;
            }
            else if ( s is DFAState && ( (DFAState)s ).IsAcceptState )
            {
                stateLabel = stateLabel +
                        "=>" + ( (DFAState)s ).getUniquelyPredictedAlt();
            }
            return '"' + stateLabel + '"';
        }

        public virtual String getArrowheadType()
        {
            return arrowhead;
        }

        public virtual void setArrowheadType( String arrowhead )
        {
            this.arrowhead = arrowhead;
        }

        public virtual String getRankdir()
        {
            return rankdir;
        }

        public virtual void setRankdir( String rankdir )
        {
            this.rankdir = rankdir;
        }
    }
}
