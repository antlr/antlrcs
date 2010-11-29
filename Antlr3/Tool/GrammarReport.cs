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

namespace Antlr3.Tool
{
    using System.Collections.Generic;
    using System.Linq;
    using Antlr.Runtime.JavaExtensions;

    using DFA = Antlr3.Analysis.DFA;
    using Stats = Antlr.Runtime.Misc.Stats;
    using StringBuilder = System.Text.StringBuilder;
    using StringComparison = System.StringComparison;
    using System.Reflection;
    using System;
    using Antlr3.Grammars;

    public class GrammarReport
    {
        /** Because I may change the stats, I need to track version for later
         *  computations to be consistent.
         */
        public const string Version = "5";
        public const string GRAMMAR_STATS_FILENAME = "grammar.stats";
        public const int NUM_GRAMMAR_STATS = 41;

        public static readonly string newline = System.Environment.NewLine; //System.getProperty("line.separator");

        public Grammar grammar;

        public GrammarReport( Grammar grammar )
        {
            this.grammar = grammar;
        }

        public static ReportData GetReportData(Grammar g)
        {
            ReportData data = new ReportData();
            data.version = Version;
            data.gname = g.name;

            data.gtype = g.GrammarTypeString;

            data.language = (string)g.GetOption("language");
            data.output = (string)g.GetOption("output");
            if (data.output == null)
            {
                data.output = "none";
            }

            string k = (string)g.GetOption("k");
            if (k == null)
            {
                k = "none";
            }
            data.grammarLevelk = k;

            string backtrack = (string)g.GetOption("backtrack");
            if (backtrack == null)
            {
                backtrack = "false";
            }
            data.grammarLevelBacktrack = backtrack;

            int totalNonSynPredProductions = 0;
            int totalNonSynPredRules = 0;
            ICollection<Rule> rules = g.Rules;
            for (Iterator it = rules.iterator(); it.hasNext(); )
            {
                Rule r = (Rule)it.next();
                if (!r.Name.StartsWith(Grammar.SynpredRulePrefix, StringComparison.OrdinalIgnoreCase))
                {
                    totalNonSynPredProductions += r.numberOfAlts;
                    totalNonSynPredRules++;
                }
            }

            data.numRules = totalNonSynPredRules;
            data.numOuterProductions = totalNonSynPredProductions;

            int numACyclicDecisions =
                g.NumberOfDecisions - g.GetNumberOfCyclicDecisions();
            List<int> depths = new List<int>();
            int[] acyclicDFAStates = new int[numACyclicDecisions];
            int[] cyclicDFAStates = new int[g.GetNumberOfCyclicDecisions()];
            int acyclicIndex = 0;
            int cyclicIndex = 0;
            int numLL1 = 0;
            int blocksWithSynPreds = 0;
            int dfaWithSynPred = 0;
            int numDecisions = 0;
            int numCyclicDecisions = 0;
            for (int i = 1; i <= g.NumberOfDecisions; i++)
            {
                Grammar.Decision d = g.GetDecision(i);
                if (d.dfa == null)
                {
                    //System.out.println("dec "+d.decision+" has no AST");
                    continue;
                }
                Rule r = d.dfa.NFADecisionStartState.enclosingRule;
                if (r.Name.StartsWith(Grammar.SynpredRulePrefix, StringComparison.OrdinalIgnoreCase))
                {
                    //System.out.println("dec "+d.decision+" is a synpred");
                    continue;
                }

                numDecisions++;
                if (BlockHasSynPred(d.blockAST))
                    blocksWithSynPreds++;
                //if (g.decisionsWhoseDFAsUsesSynPreds.contains(d.dfa))
                //    dfaWithSynPred++;
                if (d.dfa.HasSynPred)
                    dfaWithSynPred++;
                //			NFAState decisionStartState = g.getDecisionNFAStartState(d.decision);
                //			int nalts = g.getNumberOfAltsForDecisionNFA(decisionStartState);
                //			for (int alt = 1; alt <= nalts; alt++) {
                //				int walkAlt =
                //					decisionStartState.translateDisplayAltToWalkAlt(alt);
                //				NFAState altLeftEdge = g.getNFAStateForAltOfDecision(decisionStartState, walkAlt);
                //			}
                //			int nalts = g.getNumberOfAltsForDecisionNFA(d.dfa.decisionNFAStartState);
                //			for (int a=1; a<nalts; a++) {
                //				NFAState altStart =
                //					g.getNFAStateForAltOfDecision(d.dfa.decisionNFAStartState, a);
                //			}
                if (!d.dfa.IsCyclic)
                {
                    if (d.dfa.IsClassicDFA)
                    {
                        int maxk = d.dfa.MaxLookaheadDepth;
                        //System.out.println("decision "+d.dfa.decisionNumber+" k="+maxk);
                        if (maxk == 1)
                            numLL1++;
                        depths.Add(maxk);
                    }
                    else
                    {
                        acyclicDFAStates[acyclicIndex] = d.dfa.NumberOfStates;
                        acyclicIndex++;
                    }
                }
                else
                {
                    //System.out.println("CYCLIC decision "+d.dfa.decisionNumber);
                    numCyclicDecisions++;
                    cyclicDFAStates[cyclicIndex] = d.dfa.NumberOfStates;
                    cyclicIndex++;
                }
            }

            data.numLL1 = numLL1;
            data.numberOfFixedKDecisions = depths.Count;
            data.mink = depths.DefaultIfEmpty(int.MaxValue).Min();
            data.maxk = depths.DefaultIfEmpty(int.MinValue).Max();
            data.avgk = depths.DefaultIfEmpty(0).Average();

            data.numberOfDecisionsInRealRules = numDecisions;
            data.numberOfDecisions = g.NumberOfDecisions;
            data.numberOfCyclicDecisions = numCyclicDecisions;

            //		Map synpreds = g.getSyntacticPredicates();
            //		int num_synpreds = synpreds!=null ? synpreds.Count : 0;
            //		data.num_synpreds = num_synpreds;
            data.blocksWithSynPreds = blocksWithSynPreds;
            data.decisionsWhoseDFAsUsesSynPreds = dfaWithSynPred;

            //
            //		data. = Stats.stddev(depths);
            //
            //		data. = Stats.min(acyclicDFAStates);
            //
            //		data. = Stats.max(acyclicDFAStates);
            //
            //		data. = Stats.avg(acyclicDFAStates);
            //
            //		data. = Stats.stddev(acyclicDFAStates);
            //
            //		data. = Stats.sum(acyclicDFAStates);
            //
            //		data. = Stats.min(cyclicDFAStates);
            //
            //		data. = Stats.max(cyclicDFAStates);
            //
            //		data. = Stats.avg(cyclicDFAStates);
            //
            //		data. = Stats.stddev(cyclicDFAStates);
            //
            //		data. = Stats.sum(cyclicDFAStates);

            data.numTokens = g.TokenTypes.Count;

            data.DFACreationWallClockTimeInMS = g.DFACreationWallClockTimeInMS;

            // includes true ones and preds in synpreds I think; strip out. 
            data.numberOfSemanticPredicates = g.numberOfSemanticPredicates;

            data.numberOfManualLookaheadOptions = g.numberOfManualLookaheadOptions;

            data.numNonLLStarDecisions = g.numNonLLStar;
            data.numNondeterministicDecisions = g.setOfNondeterministicDecisionNumbers.Count;
            data.numNondeterministicDecisionNumbersResolvedWithPredicates =
                g.setOfNondeterministicDecisionNumbersResolvedWithPredicates.Count;

            data.errors = ErrorManager.GetErrorState().errors;
            data.warnings = ErrorManager.GetErrorState().warnings;
            data.infos = ErrorManager.GetErrorState().infos;

            data.blocksWithSemPreds = g.blocksWithSemPreds.Count;

            data.decisionsWhoseDFAsUsesSemPreds = g.decisionsWhoseDFAsUsesSemPreds.Count;

            return data;
        }

        /** Create a single-line stats report about this grammar suitable to
         *  send to the notify page at antlr.org
         */
        public virtual string ToNotifyString()
        {
            StringBuilder buf = new StringBuilder();
            ReportData data = GetReportData(grammar);
            FieldInfo[] fields = typeof(ReportData).GetFields();
            int i = 0;
            foreach (FieldInfo f in fields)
            {
                try
                {
                    object v = f.GetValue(data);
                    string s = v != null ? v.ToString() : "null";
                    if (i > 0)
                        buf.Append('\t');
                    buf.Append(s);
                }
                catch (Exception e)
                {
                    ErrorManager.InternalError("Can't get data", e);
                }
                i++;
            }
            return buf.ToString();
        }

        public virtual string GetBacktrackingReport()
        {
            StringBuilder buf = new StringBuilder();
            buf.Append( "Backtracking report:" );
            buf.Append( newline );
            buf.Append( "Number of decisions that backtrack: " );
            buf.Append( grammar.decisionsWhoseDFAsUsesSynPreds.Count );
            buf.Append( newline );
            buf.Append( GetDFALocations( grammar.decisionsWhoseDFAsUsesSynPreds ) );
            return buf.ToString();
        }

        protected virtual string GetDFALocations( HashSet<DFA> dfas )
        {
            HashSet<int> decisions = new HashSet<int>();
            StringBuilder buf = new StringBuilder();
            foreach ( DFA dfa in dfas )
            {
                // if we aborted a DFA and redid with k=1, the backtrackin
                if ( decisions.Contains( dfa.decisionNumber ) )
                {
                    continue;
                }
                decisions.Add( dfa.decisionNumber );
                buf.Append( "Rule " );
                buf.Append( dfa.NFADecisionStartState.enclosingRule.Name );
                buf.Append( " decision " );
                buf.Append( dfa.decisionNumber );
                buf.Append( " location " );
                GrammarAST decisionAST =
                    dfa.NFADecisionStartState.associatedASTNode;
                buf.Append( decisionAST.Line );
                buf.Append( ":" );
                buf.Append( decisionAST.CharPositionInLine );
                buf.Append( newline );
            }
            return buf.ToString();
        }

        /** Given a stats line suitable for sending to the antlr.org site,
         *  return a human-readable version.  Return null if there is a
         *  problem with the data.
         */
        public override string ToString()
        {
            return ToString( ToNotifyString() );
        }

        protected static ReportData DecodeReportData(string dataS)
        {
            ReportData data = new ReportData();
            StringTokenizer st = new StringTokenizer(dataS, "\t");
            FieldInfo[] fields = typeof(ReportData).GetFields();
            foreach (FieldInfo f in fields)
            {
                string v = st.nextToken();
                try
                {
                    if (f.FieldType == typeof(string))
                    {
                        f.SetValue(data, v);
                    }
                    else if (f.FieldType == typeof(double))
                    {
                        f.SetValue(data, double.Parse(v));
                    }
                    else
                    {
                        f.SetValue(data, int.Parse(v));
                    }
                }
                catch (Exception e)
                {
                    ErrorManager.InternalError("Can't get data", e);
                }
            }
            return data;
        }

        public static string ToString(string notifyDataLine)
        {
            ReportData data = DecodeReportData(notifyDataLine);
            if (data == null)
            {
                return null;
            }
            StringBuilder buf = new StringBuilder();
            buf.Append("ANTLR Grammar Report; Stats Version ");
            buf.Append(data.version);
            buf.AppendLine();
            buf.Append("Grammar: ");
            buf.Append(data.gname);
            buf.AppendLine();
            buf.Append("Type: ");
            buf.Append(data.gtype);
            buf.AppendLine();
            buf.Append("Target language: ");
            buf.Append(data.language);
            buf.AppendLine();
            buf.Append("Output: ");
            buf.Append(data.output);
            buf.AppendLine();
            buf.Append("Grammar option k: ");
            buf.Append(data.grammarLevelk);
            buf.AppendLine();
            buf.Append("Grammar option backtrack: ");
            buf.Append(data.grammarLevelBacktrack);
            buf.AppendLine();
            buf.Append("Rules: ");
            buf.Append(data.numRules);
            buf.AppendLine();
            buf.Append("Outer productions: ");
            buf.Append(data.numOuterProductions);
            buf.AppendLine();
            buf.Append("Decisions: ");
            buf.Append(data.numberOfDecisions);
            buf.AppendLine();
            buf.Append("Decisions (ignoring decisions in synpreds): ");
            buf.Append(data.numberOfDecisionsInRealRules);
            buf.AppendLine();
            buf.Append("Fixed k DFA decisions: ");
            buf.Append(data.numberOfFixedKDecisions);
            buf.AppendLine();
            buf.Append("Cyclic DFA decisions: ");
            buf.Append(data.numberOfCyclicDecisions);
            buf.AppendLine();
            buf.Append("LL(1) decisions: ");
            buf.Append(data.numLL1);
            buf.AppendLine();
            buf.Append("Min fixed k: ");
            buf.Append(data.mink);
            buf.AppendLine();
            buf.Append("Max fixed k: ");
            buf.Append(data.maxk);
            buf.AppendLine();
            buf.Append("Average fixed k: ");
            buf.Append(data.avgk);
            buf.AppendLine();
            //		buf.Append("Standard deviation of fixed k: "); buf.Append(fields[12]);
            //		buf.AppendLine();
            //		buf.Append("Min acyclic DFA states: "); buf.Append(fields[13]);
            //		buf.AppendLine();
            //		buf.Append("Max acyclic DFA states: "); buf.Append(fields[14]);
            //		buf.AppendLine();
            //		buf.Append("Average acyclic DFA states: "); buf.Append(fields[15]);
            //		buf.AppendLine();
            //		buf.Append("Standard deviation of acyclic DFA states: "); buf.Append(fields[16]);
            //		buf.AppendLine();
            //		buf.Append("Total acyclic DFA states: "); buf.Append(fields[17]);
            //		buf.AppendLine();
            //		buf.Append("Min cyclic DFA states: "); buf.Append(fields[18]);
            //		buf.AppendLine();
            //		buf.Append("Max cyclic DFA states: "); buf.Append(fields[19]);
            //		buf.AppendLine();
            //		buf.Append("Average cyclic DFA states: "); buf.Append(fields[20]);
            //		buf.AppendLine();
            //		buf.Append("Standard deviation of cyclic DFA states: "); buf.Append(fields[21]);
            //		buf.AppendLine();
            //		buf.Append("Total cyclic DFA states: "); buf.Append(fields[22]);
            //		buf.AppendLine();
            buf.Append("DFA creation time in ms: ");
            buf.Append(data.DFACreationWallClockTimeInMS);
            buf.AppendLine();

            //		buf.Append("Number of syntactic predicates available (including synpred rules): ");
            //		buf.Append(data.num_synpreds);
            //		buf.AppendLine();
            buf.Append("Decisions with available syntactic predicates (ignoring synpred rules): ");
            buf.Append(data.blocksWithSynPreds);
            buf.AppendLine();
            buf.Append("Decision DFAs using syntactic predicates (ignoring synpred rules): ");
            buf.Append(data.decisionsWhoseDFAsUsesSynPreds);
            buf.AppendLine();

            buf.Append("Number of semantic predicates found: ");
            buf.Append(data.numberOfSemanticPredicates);
            buf.AppendLine();
            buf.Append("Decisions with semantic predicates: ");
            buf.Append(data.blocksWithSemPreds);
            buf.AppendLine();
            buf.Append("Decision DFAs using semantic predicates: ");
            buf.Append(data.decisionsWhoseDFAsUsesSemPreds);
            buf.AppendLine();

            buf.Append("Number of (likely) non-LL(*) decisions: ");
            buf.Append(data.numNonLLStarDecisions);
            buf.AppendLine();
            buf.Append("Number of nondeterministic decisions: ");
            buf.Append(data.numNondeterministicDecisions);
            buf.AppendLine();
            buf.Append("Number of nondeterministic decisions resolved with predicates: ");
            buf.Append(data.numNondeterministicDecisionNumbersResolvedWithPredicates);
            buf.AppendLine();

            buf.Append("Number of manual or forced fixed lookahead k=value options: ");
            buf.Append(data.numberOfManualLookaheadOptions);
            buf.AppendLine();

            buf.Append("Vocabulary size: ");
            buf.Append(data.numTokens);
            buf.AppendLine();
            buf.Append("Number of errors: ");
            buf.Append(data.errors);
            buf.AppendLine();
            buf.Append("Number of warnings: ");
            buf.Append(data.warnings);
            buf.AppendLine();
            buf.Append("Number of infos: ");
            buf.Append(data.infos);
            buf.AppendLine();
            return buf.ToString();
        }

        public static bool BlockHasSynPred(GrammarAST blockAST)
        {
            GrammarAST c1 = blockAST.FindFirstType(ANTLRParser.SYN_SEMPRED);
            GrammarAST c2 = blockAST.FindFirstType(ANTLRParser.BACKTRACK_SEMPRED);
            if (c1 != null || c2 != null)
                return true;
            //		System.out.println(blockAST.enclosingRuleName+
            //						   " "+blockAST.getLine()+":"+blockAST.getColumn()+" no preds AST="+blockAST.toStringTree());

            return false;
        }

        public class ReportData
        {
            internal string version;
            internal string gname;
            internal string gtype;
            internal string language;
            internal int numRules;
            internal int numOuterProductions;
            internal int numberOfDecisionsInRealRules;
            internal int numberOfDecisions;
            internal int numberOfCyclicDecisions;
            internal int numberOfFixedKDecisions;
            internal int numLL1;
            internal int mink;
            internal int maxk;
            internal double avgk;
            internal int numTokens;
            internal TimeSpan DFACreationWallClockTimeInMS;
            internal int numberOfSemanticPredicates;
            internal int numberOfManualLookaheadOptions; // TODO: verify
            internal int numNonLLStarDecisions;
            internal int numNondeterministicDecisions;
            internal int numNondeterministicDecisionNumbersResolvedWithPredicates;
            internal int errors;
            internal int warnings;
            internal int infos;
            //internal int num_synpreds;
            internal int blocksWithSynPreds;
            internal int decisionsWhoseDFAsUsesSynPreds;
            internal int blocksWithSemPreds;
            internal int decisionsWhoseDFAsUsesSemPreds;
            internal string output;
            internal string grammarLevelk;
            internal string grammarLevelBacktrack;
        }
    }
}
