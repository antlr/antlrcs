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
    using System.Collections.Generic;
    using System.Linq;
    using Antlr.Runtime.JavaExtensions;

    using DFA = Antlr3.Analysis.DFA;
    using Stats = Antlr.Runtime.Misc.Stats;
    using StringBuilder = System.Text.StringBuilder;

    public class GrammarReport
    {
        /** Because I may change the stats, I need to track that for later
         *  computations to be consistent.
         */
        public const string Version = "4";
        public const string GRAMMAR_STATS_FILENAME = "grammar.stats";
        public const int NUM_GRAMMAR_STATS = 41;

        public static readonly string newline = System.Environment.NewLine; //System.getProperty("line.separator");

        public Grammar grammar;

        public GrammarReport( Grammar grammar )
        {
            this.grammar = grammar;
        }

        /** Create a single-line stats report about this grammar suitable to
         *  send to the notify page at antlr.org
         */
        public virtual string toNotifyString()
        {
            StringBuilder buf = new StringBuilder();
            buf.Append( Version );
            buf.Append( '\t' );
            buf.Append( grammar.name );
            buf.Append( '\t' );
            buf.Append( grammar.GrammarTypeString );
            buf.Append( '\t' );
            buf.Append( grammar.getOption( "language" ) );
            int totalNonSynPredProductions = 0;
            int totalNonSynPredRules = 0;
            foreach ( Rule r in grammar.Rules )
            {
                if ( !r.name.ToUpperInvariant()
                    .StartsWith( Grammar.SYNPRED_RULE_PREFIX.ToUpperInvariant() ) )
                {
                    totalNonSynPredProductions += r.numberOfAlts;
                    totalNonSynPredRules++;
                }
            }
            buf.Append( '\t' );
            buf.Append( totalNonSynPredRules );
            buf.Append( '\t' );
            buf.Append( totalNonSynPredProductions );
            int numACyclicDecisions =
                grammar.NumberOfDecisions - grammar.getNumberOfCyclicDecisions();
            int[] depths = new int[numACyclicDecisions];
            int[] acyclicDFAStates = new int[numACyclicDecisions];
            int[] cyclicDFAStates = new int[grammar.getNumberOfCyclicDecisions()];
            int acyclicIndex = 0;
            int cyclicIndex = 0;
            int numLL1 = 0;
            int numDec = 0;
            for ( int i = 1; i <= grammar.NumberOfDecisions; i++ )
            {
                Grammar.Decision d = grammar.getDecision( i );
                if ( d.dfa == null )
                {
                    continue;
                }
                numDec++;
                if ( !d.dfa.IsCyclic )
                {
                    int maxk = d.dfa.MaxLookaheadDepth;
                    if ( maxk == 1 )
                    {
                        numLL1++;
                    }
                    depths[acyclicIndex] = maxk;
                    acyclicDFAStates[acyclicIndex] = d.dfa.NumberOfStates;
                    acyclicIndex++;
                }
                else
                {
                    cyclicDFAStates[cyclicIndex] = d.dfa.NumberOfStates;
                    cyclicIndex++;
                }
            }
            buf.Append( '\t' );
            buf.Append( numDec );
            buf.Append( '\t' );
            buf.Append( grammar.getNumberOfCyclicDecisions() );
            buf.Append( '\t' );
            buf.Append( numLL1 );
            buf.Append( '\t' );
            buf.Append( depths.Min() );
            buf.Append( '\t' );
            buf.Append( depths.Max() );
            buf.Append( '\t' );
            buf.Append( depths.Average() );
            buf.Append( '\t' );
            buf.Append( Stats.Stddev( depths ) );
            buf.Append( '\t' );
            buf.Append( acyclicDFAStates.Min() );
            buf.Append( '\t' );
            buf.Append( acyclicDFAStates.Max() );
            buf.Append( '\t' );
            buf.Append( acyclicDFAStates.Average() );
            buf.Append( '\t' );
            buf.Append( Stats.Stddev( acyclicDFAStates ) );
            buf.Append( '\t' );
            buf.Append( acyclicDFAStates.Sum() );
            buf.Append( '\t' );
            buf.Append( cyclicDFAStates.Min() );
            buf.Append( '\t' );
            buf.Append( cyclicDFAStates.Max() );
            buf.Append( '\t' );
            buf.Append( cyclicDFAStates.Average() );
            buf.Append( '\t' );
            buf.Append( Stats.Stddev( cyclicDFAStates ) );
            buf.Append( '\t' );
            buf.Append( cyclicDFAStates.Sum() );
            buf.Append( '\t' );
            buf.Append( grammar.TokenTypes.size() );
            buf.Append( '\t' );
            buf.Append( grammar.DFACreationWallClockTimeInMS );
            buf.Append( '\t' );
            buf.Append( grammar.numberOfSemanticPredicates );
            buf.Append( '\t' );
            buf.Append( grammar.numberOfManualLookaheadOptions );
            buf.Append( '\t' );
            buf.Append( grammar.setOfNondeterministicDecisionNumbers.Count );
            buf.Append( '\t' );
            buf.Append( grammar.setOfNondeterministicDecisionNumbersResolvedWithPredicates.Count );
            buf.Append( '\t' );
            buf.Append( grammar.setOfDFAWhoseAnalysisTimedOut.Count );
            buf.Append( '\t' );
            buf.Append( ErrorManager.getErrorState().errors );
            buf.Append( '\t' );
            buf.Append( ErrorManager.getErrorState().warnings );
            buf.Append( '\t' );
            buf.Append( ErrorManager.getErrorState().infos );
            buf.Append( '\t' );
            var synpreds = grammar.SyntacticPredicates;
            int num_synpreds = synpreds != null ? synpreds.Count : 0;
            buf.Append( num_synpreds );
            buf.Append( '\t' );
            buf.Append( grammar.blocksWithSynPreds.Count );
            buf.Append( '\t' );
            buf.Append( grammar.decisionsWhoseDFAsUsesSynPreds.Count );
            buf.Append( '\t' );
            buf.Append( grammar.blocksWithSemPreds.Count );
            buf.Append( '\t' );
            buf.Append( grammar.decisionsWhoseDFAsUsesSemPreds.Count );
            buf.Append( '\t' );
            string output = (string)grammar.getOption( "output" );
            if ( output == null )
            {
                output = "none";
            }
            buf.Append( output );
            buf.Append( '\t' );
            object k = grammar.getOption( "k" );
            if ( k == null )
            {
                k = "none";
            }
            buf.Append( k );
            buf.Append( '\t' );
            string backtrack = (string)grammar.getOption( "backtrack" );
            if ( backtrack == null )
            {
                backtrack = "false";
            }
            buf.Append( backtrack );
            return buf.ToString();
        }

        public virtual string getBacktrackingReport()
        {
            StringBuilder buf = new StringBuilder();
            buf.Append( "Backtracking report:" );
            buf.Append( newline );
            buf.Append( "Number of decisions that backtrack: " );
            buf.Append( grammar.decisionsWhoseDFAsUsesSynPreds.Count );
            buf.Append( newline );
            buf.Append( getDFALocations( grammar.decisionsWhoseDFAsUsesSynPreds ) );
            return buf.ToString();
        }

        public virtual string getAnalysisTimeoutReport()
        {
            StringBuilder buf = new StringBuilder();
            buf.Append( "NFA conversion early termination report:" );
            buf.Append( newline );
            buf.Append( "Number of NFA conversions that terminated early: " );
            buf.Append( grammar.setOfDFAWhoseAnalysisTimedOut.Count );
            buf.Append( newline );
            buf.Append( getDFALocations( grammar.setOfDFAWhoseAnalysisTimedOut ) );
            return buf.ToString();
        }

        protected virtual string getDFALocations( HashSet<DFA> dfas )
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
                buf.Append( dfa.decisionNFAStartState.enclosingRule.name );
                buf.Append( " decision " );
                buf.Append( dfa.decisionNumber );
                buf.Append( " location " );
                GrammarAST decisionAST =
                    dfa.decisionNFAStartState.associatedASTNode;
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
            return toString( toNotifyString() );
        }

        protected static string[] decodeReportData( string data )
        {
            string[] fields = new string[NUM_GRAMMAR_STATS];
            StringTokenizer st = new StringTokenizer( data, "\t" );
            int i = 0;
            while ( st.hasMoreTokens() )
            {
                fields[i] = st.nextToken();
                i++;
            }
            if ( i != NUM_GRAMMAR_STATS )
            {
                return null;
            }
            return fields;
        }

        public static string toString( string notifyDataLine )
        {
            string[] fields = decodeReportData( notifyDataLine );
            if ( fields == null )
            {
                return null;
            }
            StringBuilder buf = new StringBuilder();
            buf.Append( "ANTLR Grammar Report; Stats Version " );
            buf.Append( fields[0] );
            buf.Append( '\n' );
            buf.Append( "Grammar: " );
            buf.Append( fields[1] );
            buf.Append( '\n' );
            buf.Append( "Type: " );
            buf.Append( fields[2] );
            buf.Append( '\n' );
            buf.Append( "Target language: " );
            buf.Append( fields[3] );
            buf.Append( '\n' );
            buf.Append( "Output: " );
            buf.Append( fields[38] );
            buf.Append( '\n' );
            buf.Append( "Grammar option k: " );
            buf.Append( fields[39] );
            buf.Append( '\n' );
            buf.Append( "Grammar option backtrack: " );
            buf.Append( fields[40] );
            buf.Append( '\n' );
            buf.Append( "Rules: " );
            buf.Append( fields[4] );
            buf.Append( '\n' );
            buf.Append( "Productions: " );
            buf.Append( fields[5] );
            buf.Append( '\n' );
            buf.Append( "Decisions: " );
            buf.Append( fields[6] );
            buf.Append( '\n' );
            buf.Append( "Cyclic DFA decisions: " );
            buf.Append( fields[7] );
            buf.Append( '\n' );
            buf.Append( "LL(1) decisions: " );
            buf.Append( fields[8] );
            buf.Append( '\n' );
            buf.Append( "Min fixed k: " );
            buf.Append( fields[9] );
            buf.Append( '\n' );
            buf.Append( "Max fixed k: " );
            buf.Append( fields[10] );
            buf.Append( '\n' );
            buf.Append( "Average fixed k: " );
            buf.Append( fields[11] );
            buf.Append( '\n' );
            buf.Append( "Standard deviation of fixed k: " );
            buf.Append( fields[12] );
            buf.Append( '\n' );
            buf.Append( "Min acyclic DFA states: " );
            buf.Append( fields[13] );
            buf.Append( '\n' );
            buf.Append( "Max acyclic DFA states: " );
            buf.Append( fields[14] );
            buf.Append( '\n' );
            buf.Append( "Average acyclic DFA states: " );
            buf.Append( fields[15] );
            buf.Append( '\n' );
            buf.Append( "Standard deviation of acyclic DFA states: " );
            buf.Append( fields[16] );
            buf.Append( '\n' );
            buf.Append( "Total acyclic DFA states: " );
            buf.Append( fields[17] );
            buf.Append( '\n' );
            buf.Append( "Min cyclic DFA states: " );
            buf.Append( fields[18] );
            buf.Append( '\n' );
            buf.Append( "Max cyclic DFA states: " );
            buf.Append( fields[19] );
            buf.Append( '\n' );
            buf.Append( "Average cyclic DFA states: " );
            buf.Append( fields[20] );
            buf.Append( '\n' );
            buf.Append( "Standard deviation of cyclic DFA states: " );
            buf.Append( fields[21] );
            buf.Append( '\n' );
            buf.Append( "Total cyclic DFA states: " );
            buf.Append( fields[22] );
            buf.Append( '\n' );
            buf.Append( "Vocabulary size: " );
            buf.Append( fields[23] );
            buf.Append( '\n' );
            buf.Append( "DFA creation time in ms: " );
            buf.Append( fields[24] );
            buf.Append( '\n' );
            buf.Append( "Number of semantic predicates found: " );
            buf.Append( fields[25] );
            buf.Append( '\n' );
            buf.Append( "Number of manual fixed lookahead k=value options: " );
            buf.Append( fields[26] );
            buf.Append( '\n' );
            buf.Append( "Number of nondeterministic decisions: " );
            buf.Append( fields[27] );
            buf.Append( '\n' );
            buf.Append( "Number of nondeterministic decisions resolved with predicates: " );
            buf.Append( fields[28] );
            buf.Append( '\n' );
            buf.Append( "Number of DFA conversions terminated early: " );
            buf.Append( fields[29] );
            buf.Append( '\n' );
            buf.Append( "Number of errors: " );
            buf.Append( fields[30] );
            buf.Append( '\n' );
            buf.Append( "Number of warnings: " );
            buf.Append( fields[31] );
            buf.Append( '\n' );
            buf.Append( "Number of infos: " );
            buf.Append( fields[32] );
            buf.Append( '\n' );
            buf.Append( "Number of syntactic predicates found: " );
            buf.Append( fields[33] );
            buf.Append( '\n' );
            buf.Append( "Decisions with syntactic predicates: " );
            buf.Append( fields[34] );
            buf.Append( '\n' );
            buf.Append( "Decision DFAs using syntactic predicates: " );
            buf.Append( fields[35] );
            buf.Append( '\n' );
            buf.Append( "Decisions with semantic predicates: " );
            buf.Append( fields[36] );
            buf.Append( '\n' );
            buf.Append( "Decision DFAs using semantic predicates: " );
            buf.Append( fields[37] );
            buf.Append( '\n' );
            return buf.ToString();
        }

    }
}
