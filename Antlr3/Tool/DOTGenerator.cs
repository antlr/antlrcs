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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Antlr3.Analysis;
    using Antlr3.Extensions;

    using ANTLRParser = Antlr3.Grammars.ANTLRParser;
    using Path = System.IO.Path;
    using StringBuffer = System.Text.StringBuilder;
    using StringTemplate = Antlr4.StringTemplate.Template;
    using TemplateGroupFile = Antlr4.StringTemplate.TemplateGroupFile;
    using TemplateGroupDirectory = Antlr4.StringTemplate.TemplateGroupDirectory;
    using TemplateGroup = Antlr4.StringTemplate.TemplateGroup;

    /** The DOT (part of graphviz) generation aspect. */
    public class DOTGenerator : IGraphGenerator
    {
        internal bool StripNonreducedStates = false;

        protected string arrowhead = "normal";
        protected string rankdir = "LR";

        /** <summary>Library of output templates; use &lt;attrname&gt; format</summary> */
        private static TemplateGroup _stlib;

        public string dfaTemplateDirectoryName;

        /** To prevent infinite recursion when walking state machines, record
         *  which states we've visited.  Make a new set every time you start
         *  walking in case you reuse this object.
         */
        protected ICollection<int> markedStates;

        protected Grammar grammar;

        /** This aspect is associated with a grammar */
        public DOTGenerator( Grammar grammar )
        {
            this.grammar = grammar;
            this.dfaTemplateDirectoryName = Path.Combine(Path.Combine(Path.Combine(AntlrTool.ToolPathRoot, "Tool"), "Templates"), "dot");
        }

        #region Properties

        public string FileExtension
        {
            get
            {
                return ".dot";
            }
        }

        public string ArrowheadType
        {
            get
            {
                return arrowhead;
            }

            set
            {
                arrowhead = value;
            }
        }

        [CLSCompliant(false)]
        public string RankDir
        {
            get
            {
                return rankdir;
            }

            set
            {
                rankdir = value;
            }
        }

        #endregion

        public TemplateGroup GetTemplates()
        {
            if (_stlib == null)
                _stlib = new TemplateGroupFile(Path.Combine(dfaTemplateDirectoryName, "dot.stg"));

            return _stlib;
        }

        /** Return a String containing a DOT description that, when displayed,
         *  will show the incoming state machine visually.  All nodes reachable
         *  from startState will be included.
         */
        public virtual string GenerateGraph( State startState )
        {
            if ( startState == null )
            {
                return null;
            }
            // The output DOT graph for visualization
            StringTemplate dot = null;
            markedStates = new HashSet<int>();
            if ( startState is DFAState )
            {
                dot = GetTemplates().GetInstanceOf( "dfa" );
                dot.SetAttribute( "startState",
                        startState.StateNumber );
                dot.SetAttribute( "useBox",
                                 AntlrTool.internalOption_ShowNFAConfigsInDFA );
                WalkCreatingDFADOT( dot, (DFAState)startState );
            }
            else
            {
                dot = GetTemplates().GetInstanceOf( "nfa" );
                dot.SetAttribute( "startState",
                        startState.StateNumber );
                WalkRuleNFACreatingDOT( dot, startState );
            }
            dot.SetAttribute( "rankdir", rankdir );
            return dot.Render();
        }

#if false
        /** Return a String containing a DOT description that, when displayed,
         *  will show the incoming state machine visually.  All nodes reachable
         *  from startState will be included.
         */
        public string GetRuleNFADOT( State startState )
        {
            // The output DOT graph for visualization
            StringTemplate dot = GetTemplates().GetInstanceOf( "nfa" );

            markedStates = new HashSet<object>();
            dot.SetAttribute( "startState", startState.stateNumber );
            walkRuleNFACreatingDOT( dot, startState );
            return dot.Render();
        }
#endif

        /** Do a depth-first walk of the state machine graph and
         *  fill a DOT description template.  Keep filling the
         *  states and edges attributes.
         */
        protected virtual void WalkCreatingDFADOT( StringTemplate dot,
                                          DFAState s )
        {
            if ( markedStates.Contains( s.StateNumber ) )
            {
                return; // already visited this node
            }

            markedStates.Add( s.StateNumber ); // mark this node as completed.

            // first add this node
            StringTemplate st;
            if ( s.IsAcceptState )
            {
                st = GetTemplates().GetInstanceOf( "stopstate" );
            }
            else
            {
                st = GetTemplates().GetInstanceOf( "state" );
            }
            st.SetAttribute( "name", GetStateLabel( s ) );
            dot.SetAttribute( "states", st );

            // make a DOT edge for each transition
            for ( int i = 0; i < s.NumberOfTransitions; i++ )
            {
                Transition edge = (Transition)s.GetTransition( i );
                //Console.Out.WriteLine( "dfa " + s.dfa.decisionNumber + " edge from s"
                //    + s.stateNumber + " [" + i + "] of " + s.NumberOfTransitions );
                if ( StripNonreducedStates )
                {
                    if ( edge.Target is DFAState &&
                        ( (DFAState)edge.Target ).AcceptStateReachable != Reachable.Yes )
                    {
                        continue; // don't generate nodes for terminal states
                    }
                }
                st = GetTemplates().GetInstanceOf( "edge" );
                st.SetAttribute( "label", GetEdgeLabel( edge ) );
                st.SetAttribute( "src", GetStateLabel( s ) );
                st.SetAttribute( "target", GetStateLabel( edge.Target ) );
                st.SetAttribute( "arrowhead", arrowhead );
                dot.SetAttribute( "edges", st );
                WalkCreatingDFADOT( dot, (DFAState)edge.Target ); // keep walkin'
            }
        }

        /** Do a depth-first walk of the state machine graph and
         *  fill a DOT description template.  Keep filling the
         *  states and edges attributes.  We know this is an NFA
         *  for a rule so don't traverse edges to other rules and
         *  don't go past rule end state.
         */
        protected virtual void WalkRuleNFACreatingDOT( StringTemplate dot,
                                              State s )
        {
            if ( markedStates.Contains( s.StateNumber ) )
            {
                return; // already visited this node
            }

            markedStates.Add( s.StateNumber ); // mark this node as completed.

            // first add this node
            StringTemplate stateST;
            if ( s.IsAcceptState )
            {
                stateST = GetTemplates().GetInstanceOf( "stopstate" );
            }
            else
            {
                stateST = GetTemplates().GetInstanceOf( "state" );
            }
            stateST.SetAttribute( "name", GetStateLabel( s ) );
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
                    StringTemplate rankST = GetTemplates().GetInstanceOf( "decision-rank" );
                    NFAState alt = (NFAState)s;
                    while ( alt != null )
                    {
                        rankST.SetAttribute( "states", GetStateLabel( alt ) );
                        if ( alt.transition[1] != null )
                        {
                            alt = (NFAState)alt.transition[1].Target;
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
                Transition edge = (Transition)s.GetTransition( i );
                if ( edge is RuleClosureTransition )
                {
                    RuleClosureTransition rr = ( (RuleClosureTransition)edge );
                    // don't jump to other rules, but display edge to follow node
                    edgeST = GetTemplates().GetInstanceOf( "edge" );
                    if ( rr.Rule.Grammar != grammar )
                    {
                        edgeST.SetAttribute( "label", "<" + rr.Rule.Grammar.name + "." + rr.Rule.Name + ">" );
                    }
                    else
                    {
                        edgeST.SetAttribute( "label", "<" + rr.Rule.Name + ">" );
                    }
                    edgeST.SetAttribute( "src", GetStateLabel( s ) );
                    edgeST.SetAttribute( "target", GetStateLabel( rr.FollowState ) );
                    edgeST.SetAttribute( "arrowhead", arrowhead );
                    dot.SetAttribute( "edges", edgeST );
                    WalkRuleNFACreatingDOT( dot, rr.FollowState );
                    continue;
                }
                if ( edge.IsAction )
                {
                    edgeST = GetTemplates().GetInstanceOf( "action-edge" );
                }
                else if ( edge.IsEpsilon )
                {
                    edgeST = GetTemplates().GetInstanceOf( "epsilon-edge" );
                }
                else
                {
                    edgeST = GetTemplates().GetInstanceOf( "edge" );
                }
                edgeST.SetAttribute( "label", GetEdgeLabel( edge ) );
                edgeST.SetAttribute( "src", GetStateLabel( s ) );
                edgeST.SetAttribute( "target", GetStateLabel( edge.Target ) );
                edgeST.SetAttribute( "arrowhead", arrowhead );
                dot.SetAttribute( "edges", edgeST );
                WalkRuleNFACreatingDOT( dot, edge.Target ); // keep walkin'
            }
        }

#if false
        public void WriteDOTFilesForAllRuleNFAs()
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
        public void WriteDOTFilesForAllDecisionDFAs()
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
        protected virtual string GetEdgeLabel( Transition edge )
        {
            string label = edge.Label.ToString( grammar );
            label = label.Replace( "\\", "\\\\" );
            label = label.Replace( "\"", "\\\"" );
            label = label.Replace( "\n", "\\\\n" );
            label = label.Replace( "\r", "" );
            if ( label.Equals( Label.EPSILON_STR ) )
            {
                label = "e";
            }
            State target = edge.Target;
            if ( !edge.IsSemanticPredicate && target is DFAState )
            {
                // look for gated predicates; don't add gated to simple sempred edges
                SemanticContext preds =
                    ( (DFAState)target ).GetGatedPredicatesInNFAConfigurations();
                if ( preds != null )
                {
                    string predsStr = "";
                    predsStr = "&&{" +
                        preds.GenExpr( grammar.generator,
                                      grammar.generator.Templates, null ).Render()
                        + "}?";
                    label += predsStr;
                }
            }
            return label;
        }

        protected virtual string GetStateLabel( State s )
        {
            if ( s == null )
            {
                return "null";
            }
            string stateLabel = s.StateNumber.ToString();
            if ( s is DFAState )
            {
                StringBuffer buf = new StringBuffer( 250 );
                buf.Append( 's' );
                buf.Append( s.StateNumber );
                if ( AntlrTool.internalOption_ShowNFAConfigsInDFA )
                {
                    if ( s is DFAState )
                    {
                        if ( ( (DFAState)s ).AbortedDueToRecursionOverflow )
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
                        ICollection<NFAConfiguration> configurations = ( (DFAState)s ).NfaConfigurations;
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
                            IList<NFAConfiguration> configsInAlt = new List<NFAConfiguration>();
                            foreach ( NFAConfiguration c in configurations )
                            {
                                if ( c.Alt != alt )
                                    continue;
                                configsInAlt.Add( c );
                            }

                            int n = 0;
                            for ( int cIndex = 0; cIndex < configsInAlt.Count; cIndex++ )
                            {
                                NFAConfiguration c = configsInAlt[cIndex];
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
                        "=>" + ( (DFAState)s ).GetUniquelyPredictedAlt();
            }
            return '"' + stateLabel + '"';
        }
    }
}
