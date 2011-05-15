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

namespace Antlr.Runtime.Debug
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Antlr.Runtime.Debug.Misc;

    using Array = System.Array;
    using CLSCompliantAttribute = System.CLSCompliantAttribute;
    using Console = System.Console;
    using DateTime = System.DateTime;
    using Environment = System.Environment;
    using Math = System.Math;
    using StringBuilder = System.Text.StringBuilder;

    /** <summary>Using the debug event interface, track what is happening in the parser
     *  and record statistics about the runtime.
     */
    public class Profiler : BlankDebugEventListener
    {
        public static readonly string DataSeparator = "\t";
        public static readonly string NewLine = Environment.NewLine;

        internal static bool dump = false;

        /** Because I may change the stats, I need to track that for later
         *  computations to be consistent.
         */
        public static readonly string Version = "3";
        public static readonly string RuntimeStatsFilename = "runtime.stats";

        /** Ack, should not store parser; can't do remote stuff.  Well, we pass
         *  input stream around too so I guess it's ok.
         */
        public DebugParser parser = null;

        // working variables

        [CLSCompliant(false)]
        protected int ruleLevel = 0;
        //protected int decisionLevel = 0;
        protected IToken lastRealTokenTouchedInDecision;
        protected Dictionary<string, bool> uniqueRules = new Dictionary<string, bool>();
        protected Stack<string> currentGrammarFileName = new Stack<string>();
        protected Stack<string> currentRuleName = new Stack<string>();
        protected Stack<int> currentLine = new Stack<int>();
        protected Stack<int> currentPos = new Stack<int>();

        // Vector<DecisionStats>
        //protected Vector decisions = new Vector(200); // need setSize
        protected DoubleKeyMap<string, int, DecisionDescriptor> decisions = new DoubleKeyMap<string, int, DecisionDescriptor>();

        // Record a DecisionData for each decision we hit while parsing
        private List<DecisionEvent> decisionEvents = new List<DecisionEvent>();
        protected Stack<DecisionEvent> decisionStack = new Stack<DecisionEvent>();

        protected int backtrackDepth;

        ProfileStats stats = new ProfileStats();

        public Profiler()
        {
        }

        public Profiler(DebugParser parser)
        {
            this.parser = parser;
        }

        public override void EnterRule(string grammarFileName, string ruleName)
        {
            //System.out.println("enterRule "+grammarFileName+":"+ruleName);
            ruleLevel++;
            stats.numRuleInvocations++;
            uniqueRules.Add(grammarFileName + ":" + ruleName, true);
            stats.maxRuleInvocationDepth = Math.Max(stats.maxRuleInvocationDepth, ruleLevel);
            currentGrammarFileName.Push(grammarFileName);
            currentRuleName.Push(ruleName);
        }

        public override void ExitRule(string grammarFileName, string ruleName)
        {
            ruleLevel--;
            currentGrammarFileName.Pop();
            currentRuleName.Pop();
        }

        /** Track memoization; this is not part of standard debug interface
         *  but is triggered by profiling.  Code gen inserts an override
         *  for this method in the recognizer, which triggers this method.
         *  Called from alreadyParsedRule().
         */
        public virtual void ExamineRuleMemoization(IIntStream input,
                                           int ruleIndex,
                                           int stopIndex, // index or MEMO_RULE_UNKNOWN...
                                           string ruleName)
        {
            if (dump)
                Console.WriteLine("examine memo " + ruleName + " at " + input.Index + ": " + stopIndex);
            if (stopIndex == BaseRecognizer.MemoRuleUnknown)
            {
                //System.out.println("rule "+ruleIndex+" missed @ "+input.index());
                stats.numMemoizationCacheMisses++;
                stats.numGuessingRuleInvocations++; // we'll have to enter
                CurrentDecision().numMemoizationCacheMisses++;
            }
            else
            {
                // regardless of rule success/failure, if in cache, we have a cache hit
                //System.out.println("rule "+ruleIndex+" hit @ "+input.index());
                stats.numMemoizationCacheHits++;
                CurrentDecision().numMemoizationCacheHits++;
            }
        }

        /** Warning: doesn't track success/failure, just unique recording event */
        public virtual void Memoize(IIntStream input,
                            int ruleIndex,
                            int ruleStartIndex,
                            string ruleName)
        {
            // count how many entries go into table
            if (dump)
                Console.WriteLine("memoize " + ruleName);
            stats.numMemoizationCacheEntries++;
        }

        public override void Location(int line, int pos)
        {
            currentLine.Push(line);
            currentPos.Push(pos);
        }

        public override void EnterDecision(int decisionNumber, bool couldBacktrack)
        {
            lastRealTokenTouchedInDecision = null;
            stats.numDecisionEvents++;
            int startingLookaheadIndex = parser.TokenStream.Index;
            ITokenStream input = parser.TokenStream;
            if (dump)
            {
                Console.WriteLine("enterDecision canBacktrack=" + couldBacktrack + " " + decisionNumber +
                      " backtrack depth " + backtrackDepth +
                      " @ " + input.Get(input.Index) +
                      " rule " + LocationDescription());
            }
            string g = currentGrammarFileName.Peek();
            DecisionDescriptor descriptor = decisions.Get(g, decisionNumber);
            if (descriptor == null)
            {
                descriptor = new DecisionDescriptor();
                decisions.Put(g, decisionNumber, descriptor);
                descriptor.decision = decisionNumber;
                descriptor.fileName = currentGrammarFileName.Peek();
                descriptor.ruleName = currentRuleName.Peek();
                descriptor.line = currentLine.Peek();
                descriptor.pos = currentPos.Peek();
                descriptor.couldBacktrack = couldBacktrack;
            }
            descriptor.n++;

            DecisionEvent d = new DecisionEvent();
            decisionStack.Push(d);
            d.decision = descriptor;
            d.startTime = DateTime.Now;
            d.startIndex = startingLookaheadIndex;
        }

        public override void ExitDecision(int decisionNumber)
        {
            DecisionEvent d = decisionStack.Pop();
            d.stopTime = DateTime.Now;

            int lastTokenIndex = lastRealTokenTouchedInDecision.TokenIndex;
            int numHidden = GetNumberOfHiddenTokens(d.startIndex, lastTokenIndex);
            int depth = lastTokenIndex - d.startIndex - numHidden + 1; // +1 counts consuming start token as 1
            d.k = depth;
            d.decision.maxk = Math.Max(d.decision.maxk, depth);

            if (dump)
            {
                Console.WriteLine("exitDecision " + decisionNumber + " in " + d.decision.ruleName +
                                   " lookahead " + d.k + " max token " + lastRealTokenTouchedInDecision);
            }

            decisionEvents.Add(d); // done with decision; track all
        }

        public override void ConsumeToken(IToken token)
        {
            if (dump)
                Console.WriteLine("consume token " + token);

            if (!InDecision)
            {
                stats.numTokens++;
                return;
            }

            if (lastRealTokenTouchedInDecision == null ||
                 lastRealTokenTouchedInDecision.TokenIndex < token.TokenIndex)
            {
                lastRealTokenTouchedInDecision = token;
            }
            DecisionEvent d = CurrentDecision();
            // compute lookahead depth
            int thisRefIndex = token.TokenIndex;
            int numHidden = GetNumberOfHiddenTokens(d.startIndex, thisRefIndex);
            int depth = thisRefIndex - d.startIndex - numHidden + 1; // +1 counts consuming start token as 1
            //d.maxk = Math.max(d.maxk, depth);
            if (dump)
            {
                Console.WriteLine("consume " + thisRefIndex + " " + depth + " tokens ahead in " +
                                   d.decision.ruleName + "-" + d.decision.decision + " start index " + d.startIndex);
            }
        }

        /** The parser is in a decision if the decision depth > 0.  This
         *  works for backtracking also, which can have nested decisions.
         */
        public virtual bool InDecision
        {
            get
            {
                return decisionStack.Count > 0;
            }
        }

        public override void ConsumeHiddenToken(IToken token)
        {
            //System.out.println("consume hidden token "+token);
            if (!InDecision)
                stats.numHiddenTokens++;
        }

        /** Track refs to lookahead if in a fixed/nonfixed decision.
         */
        public override void LT(int i, IToken t)
        {
            if (InDecision && i > 0)
            {
                DecisionEvent d = CurrentDecision();
                if (dump)
                {
                    Console.WriteLine("LT(" + i + ")=" + t + " index " + t.TokenIndex + " relative to " + d.decision.ruleName + "-" +
                             d.decision.decision + " start index " + d.startIndex);
                }

                if (lastRealTokenTouchedInDecision == null ||
                     lastRealTokenTouchedInDecision.TokenIndex < t.TokenIndex)
                {
                    lastRealTokenTouchedInDecision = t;
                    if (dump)
                        Console.WriteLine("set last token " + lastRealTokenTouchedInDecision);
                }
                // get starting index off stack
                //			int stackTop = lookaheadStack.size()-1;
                //			Integer startingIndex = (Integer)lookaheadStack.get(stackTop);
                //			// compute lookahead depth
                //			int thisRefIndex = parser.getTokenStream().index();
                //			int numHidden =
                //				getNumberOfHiddenTokens(startingIndex.intValue(), thisRefIndex);
                //			int depth = i + thisRefIndex - startingIndex.intValue() - numHidden;
                //			/*
                //			System.out.println("LT("+i+") @ index "+thisRefIndex+" is depth "+depth+
                //				" max is "+maxLookaheadInCurrentDecision);
                //			*/
                //			if ( depth>maxLookaheadInCurrentDecision ) {
                //				maxLookaheadInCurrentDecision = depth;
                //			}
                //			d.maxk = currentDecision()/
            }
        }

        /** Track backtracking decisions.  You'll see a fixed or cyclic decision
         *  and then a backtrack.
         *
         * 		enter rule
         * 		...
         * 		enter decision
         * 		LA and possibly consumes (for cyclic DFAs)
         * 		begin backtrack level
         * 		mark m
         * 		rewind m
         * 		end backtrack level, success
         * 		exit decision
         * 		...
         * 		exit rule
         */
        public override void BeginBacktrack(int level)
        {
            if (dump)
                Console.WriteLine("enter backtrack " + level);
            backtrackDepth++;
            DecisionEvent e = CurrentDecision();
            if (e.decision.couldBacktrack)
            {
                stats.numBacktrackOccurrences++;
                e.decision.numBacktrackOccurrences++;
                e.backtracks = true;
            }
        }

        /** Successful or not, track how much lookahead synpreds use */
        public override void EndBacktrack(int level, bool successful)
        {
            if (dump)
                Console.WriteLine("exit backtrack " + level + ": " + successful);
            backtrackDepth--;
        }

        public override void Mark(int i)
        {
            if (dump)
                Console.WriteLine("mark " + i);
        }

        public override void Rewind(int i)
        {
            if (dump)
                Console.WriteLine("rewind " + i);
        }

        public override void Rewind()
        {
            if (dump)
                Console.WriteLine("rewind");
        }

        protected virtual DecisionEvent CurrentDecision()
        {
            return decisionStack.Peek();
        }

        public override void RecognitionException(RecognitionException e)
        {
            stats.numReportedErrors++;
        }

        public override void SemanticPredicate(bool result, string predicate)
        {
            stats.numSemanticPredicates++;
            if (InDecision)
            {
                DecisionEvent d = CurrentDecision();
                d.evalSemPred = true;
                d.decision.numSemPredEvals++;
                if (dump)
                {
                    Console.WriteLine("eval " + predicate + " in " + d.decision.ruleName + "-" +
                                       d.decision.decision);
                }
            }
        }

        public override void Terminate()
        {
            foreach (DecisionEvent e in decisionEvents)
            {
                //System.out.println("decision "+e.decision.decision+": k="+e.k);
                e.decision.avgk += e.k;
                stats.avgkPerDecisionEvent += e.k;
                if (e.backtracks)
                { // doesn't count gated syn preds on DFA edges
                    stats.avgkPerBacktrackingDecisionEvent += e.k;
                }
            }
            stats.averageDecisionPercentBacktracks = 0.0f;
            foreach (DecisionDescriptor d in decisions.Values())
            {
                stats.numDecisionsCovered++;
                d.avgk /= (float)d.n;
                if (d.couldBacktrack)
                {
                    stats.numDecisionsThatPotentiallyBacktrack++;
                    float percentBacktracks = d.numBacktrackOccurrences / (float)d.n;
                    //System.out.println("dec "+d.decision+" backtracks "+percentBacktracks*100+"%");
                    stats.averageDecisionPercentBacktracks += percentBacktracks;
                }
                // ignore rules that backtrack along gated DFA edges
                if (d.numBacktrackOccurrences > 0)
                {
                    stats.numDecisionsThatDoBacktrack++;
                }
            }
            stats.averageDecisionPercentBacktracks /= stats.numDecisionsThatPotentiallyBacktrack;
            stats.averageDecisionPercentBacktracks *= 100; // it's a percentage
            stats.avgkPerDecisionEvent /= stats.numDecisionEvents;
            stats.avgkPerBacktrackingDecisionEvent /= (float)stats.numBacktrackOccurrences;

            Console.Error.WriteLine(ToString());
            Console.Error.WriteLine(GetDecisionStatsDump());

            //		String stats = toNotifyString();
            //		try {
            //			Stats.writeReport(RUNTIME_STATS_FILENAME,stats);
            //		}
            //		catch (IOException ioe) {
            //			System.err.println(ioe);
            //			ioe.printStackTrace(System.err);
            //		}
        }

        public virtual void SetParser(DebugParser parser)
        {
            this.parser = parser;
        }

        // R E P O R T I N G

        public virtual string ToNotifyString()
        {
            StringBuilder buf = new StringBuilder();
            buf.Append(Version);
            buf.Append('\t');
            buf.Append(parser.GetType().Name);
            //		buf.Append('\t');
            //		buf.Append(numRuleInvocations);
            //		buf.Append('\t');
            //		buf.Append(maxRuleInvocationDepth);
            //		buf.Append('\t');
            //		buf.Append(numFixedDecisions);
            //		buf.Append('\t');
            //		buf.Append(Stats.min(decisionMaxFixedLookaheads));
            //		buf.Append('\t');
            //		buf.Append(Stats.max(decisionMaxFixedLookaheads));
            //		buf.Append('\t');
            //		buf.Append(Stats.avg(decisionMaxFixedLookaheads));
            //		buf.Append('\t');
            //		buf.Append(Stats.stddev(decisionMaxFixedLookaheads));
            //		buf.Append('\t');
            //		buf.Append(numCyclicDecisions);
            //		buf.Append('\t');
            //		buf.Append(Stats.min(decisionMaxCyclicLookaheads));
            //		buf.Append('\t');
            //		buf.Append(Stats.max(decisionMaxCyclicLookaheads));
            //		buf.Append('\t');
            //		buf.Append(Stats.avg(decisionMaxCyclicLookaheads));
            //		buf.Append('\t');
            //		buf.Append(Stats.stddev(decisionMaxCyclicLookaheads));
            //		buf.Append('\t');
            //		buf.Append(numBacktrackDecisions);
            //		buf.Append('\t');
            //		buf.Append(Stats.min(toArray(decisionMaxSynPredLookaheads)));
            //		buf.Append('\t');
            //		buf.Append(Stats.max(toArray(decisionMaxSynPredLookaheads)));
            //		buf.Append('\t');
            //		buf.Append(Stats.avg(toArray(decisionMaxSynPredLookaheads)));
            //		buf.Append('\t');
            //		buf.Append(Stats.stddev(toArray(decisionMaxSynPredLookaheads)));
            //		buf.Append('\t');
            //		buf.Append(numSemanticPredicates);
            //		buf.Append('\t');
            //		buf.Append(parser.getTokenStream().size());
            //		buf.Append('\t');
            //		buf.Append(numHiddenTokens);
            //		buf.Append('\t');
            //		buf.Append(numCharsMatched);
            //		buf.Append('\t');
            //		buf.Append(numHiddenCharsMatched);
            //		buf.Append('\t');
            //		buf.Append(numberReportedErrors);
            //		buf.Append('\t');
            //		buf.Append(numMemoizationCacheHits);
            //		buf.Append('\t');
            //		buf.Append(numMemoizationCacheMisses);
            //		buf.Append('\t');
            //		buf.Append(numGuessingRuleInvocations);
            //		buf.Append('\t');
            //		buf.Append(numMemoizationCacheEntries);
            return buf.ToString();
        }

        public override string ToString()
        {
            return ToString(GetReport());
        }

        public virtual ProfileStats GetReport()
        {
            //ITokenStream input = parser.TokenStream;
            //for (int i = 0; i < input.Count && lastRealTokenTouchedInDecision != null && i <= lastRealTokenTouchedInDecision.TokenIndex; i++)
            //{
            //    IToken t = input.Get(i);
            //    if (t.Channel != TokenChannels.Default)
            //    {
            //        stats.numHiddenTokens++;
            //        stats.numHiddenCharsMatched += t.Text.Length;
            //    }
            //}
            stats.Version = Version;
            stats.name = parser.GetType().Name;
            stats.numUniqueRulesInvoked = uniqueRules.Count;
            //stats.numCharsMatched = lastTokenConsumed.getStopIndex() + 1;
            return stats;
        }

        public virtual DoubleKeyMap<string, int, DecisionDescriptor> GetDecisionStats()
        {
            return decisions;
        }

        public virtual ReadOnlyCollection<DecisionEvent> DecisionEvents
        {
            get
            {
                return decisionEvents.AsReadOnly();
            }
        }

        public static string ToString(ProfileStats stats)
        {
            StringBuilder buf = new StringBuilder();
            buf.Append("ANTLR Runtime Report; Profile Version ");
            buf.Append(stats.Version);
            buf.Append(NewLine);
            buf.Append("parser name ");
            buf.Append(stats.name);
            buf.Append(NewLine);
            buf.Append("Number of rule invocations ");
            buf.Append(stats.numRuleInvocations);
            buf.Append(NewLine);
            buf.Append("Number of unique rules visited ");
            buf.Append(stats.numUniqueRulesInvoked);
            buf.Append(NewLine);
            buf.Append("Number of decision events ");
            buf.Append(stats.numDecisionEvents);
            buf.Append(NewLine);
            buf.Append("Number of rule invocations while backtracking ");
            buf.Append(stats.numGuessingRuleInvocations);
            buf.Append(NewLine);
            buf.Append("max rule invocation nesting depth ");
            buf.Append(stats.maxRuleInvocationDepth);
            buf.Append(NewLine);
            //		buf.Append("number of fixed lookahead decisions ");
            //		buf.Append();
            //		buf.Append(newline);
            //		buf.Append("min lookahead used in a fixed lookahead decision ");
            //		buf.Append();
            //		buf.Append(newline);
            //		buf.Append("max lookahead used in a fixed lookahead decision ");
            //		buf.Append();
            //		buf.Append(newline);
            //		buf.Append("average lookahead depth used in fixed lookahead decisions ");
            //		buf.Append();
            //		buf.Append(newline);
            //		buf.Append("standard deviation of depth used in fixed lookahead decisions ");
            //		buf.Append();
            //		buf.Append(newline);
            //		buf.Append("number of arbitrary lookahead decisions ");
            //		buf.Append();
            //		buf.Append(newline);
            //		buf.Append("min lookahead used in an arbitrary lookahead decision ");
            //		buf.Append();
            //		buf.Append(newline);
            //		buf.Append("max lookahead used in an arbitrary lookahead decision ");
            //		buf.Append();
            //		buf.Append(newline);
            //		buf.Append("average lookahead depth used in arbitrary lookahead decisions ");
            //		buf.Append();
            //		buf.Append(newline);
            //		buf.Append("standard deviation of depth used in arbitrary lookahead decisions ");
            //		buf.Append();
            //		buf.Append(newline);
            //		buf.Append("number of evaluated syntactic predicates ");
            //		buf.Append();
            //		buf.Append(newline);
            //		buf.Append("min lookahead used in a syntactic predicate ");
            //		buf.Append();
            //		buf.Append(newline);
            //		buf.Append("max lookahead used in a syntactic predicate ");
            //		buf.Append();
            //		buf.Append(newline);
            //		buf.Append("average lookahead depth used in syntactic predicates ");
            //		buf.Append();
            //		buf.Append(newline);
            //		buf.Append("standard deviation of depth used in syntactic predicates ");
            //		buf.Append();
            //		buf.Append(newline);
            buf.Append("rule memoization cache size ");
            buf.Append(stats.numMemoizationCacheEntries);
            buf.Append(NewLine);
            buf.Append("number of rule memoization cache hits ");
            buf.Append(stats.numMemoizationCacheHits);
            buf.Append(NewLine);
            buf.Append("number of rule memoization cache misses ");
            buf.Append(stats.numMemoizationCacheMisses);
            buf.Append(NewLine);
            //		buf.Append("number of evaluated semantic predicates ");
            //		buf.Append();
            //		buf.Append(newline);
            buf.Append("number of tokens ");
            buf.Append(stats.numTokens);
            buf.Append(NewLine);
            buf.Append("number of hidden tokens ");
            buf.Append(stats.numHiddenTokens);
            buf.Append(NewLine);
            buf.Append("number of char ");
            buf.Append(stats.numCharsMatched);
            buf.Append(NewLine);
            buf.Append("number of hidden char ");
            buf.Append(stats.numHiddenCharsMatched);
            buf.Append(NewLine);
            buf.Append("number of syntax errors ");
            buf.Append(stats.numReportedErrors);
            buf.Append(NewLine);
            return buf.ToString();
        }

        public virtual string GetDecisionStatsDump()
        {
            StringBuilder buf = new StringBuilder();
            buf.Append("location");
            buf.Append(DataSeparator);
            buf.Append("n");
            buf.Append(DataSeparator);
            buf.Append("avgk");
            buf.Append(DataSeparator);
            buf.Append("maxk");
            buf.Append(DataSeparator);
            buf.Append("synpred");
            buf.Append(DataSeparator);
            buf.Append("sempred");
            buf.Append(DataSeparator);
            buf.Append("canbacktrack");
            buf.Append("\n");
            foreach (string fileName in decisions.KeySet())
            {
                foreach (int d in decisions.KeySet(fileName))
                {
                    DecisionDescriptor s = decisions.Get(fileName, d);
                    buf.Append(s.decision);
                    buf.Append("@");
                    buf.Append(LocationDescription(s.fileName, s.ruleName, s.line, s.pos)); // decision number
                    buf.Append(DataSeparator);
                    buf.Append(s.n);
                    buf.Append(DataSeparator);
                    buf.Append(string.Format("{0}", s.avgk));
                    buf.Append(DataSeparator);
                    buf.Append(s.maxk);
                    buf.Append(DataSeparator);
                    buf.Append(s.numBacktrackOccurrences);
                    buf.Append(DataSeparator);
                    buf.Append(s.numSemPredEvals);
                    buf.Append(DataSeparator);
                    buf.Append(s.couldBacktrack ? "1" : "0");
                    buf.Append(NewLine);
                }
            }
            return buf.ToString();
        }

        protected virtual int[] Trim(int[] X, int n)
        {
            if (n < X.Length)
            {
                int[] trimmed = new int[n];
                Array.Copy(X, 0, trimmed, 0, n);
                X = trimmed;
            }
            return X;
        }

        /** Get num hidden tokens between i..j inclusive */
        public virtual int GetNumberOfHiddenTokens(int i, int j)
        {
            int n = 0;
            ITokenStream input = parser.TokenStream;
            for (int ti = i; ti < input.Count && ti <= j; ti++)
            {
                IToken t = input.Get(ti);
                if (t.Channel != TokenChannels.Default)
                {
                    n++;
                }
            }
            return n;
        }

        protected virtual string LocationDescription()
        {
            return LocationDescription(
                currentGrammarFileName.Peek(),
                currentRuleName.Peek(),
                currentLine.Peek(),
                currentPos.Peek());
        }

        protected virtual string LocationDescription(string file, string rule, int line, int pos)
        {
            return file + ":" + line + ":" + pos + "(" + rule + ")";
        }

        public class ProfileStats
        {
            public string Version;
            public string name;
            public int numRuleInvocations;
            public int numUniqueRulesInvoked;
            public int numDecisionEvents;
            public int numDecisionsCovered;
            public int numDecisionsThatPotentiallyBacktrack;
            public int numDecisionsThatDoBacktrack;
            public int maxRuleInvocationDepth;
            public float avgkPerDecisionEvent;
            public float avgkPerBacktrackingDecisionEvent;
            public float averageDecisionPercentBacktracks;
            public int numBacktrackOccurrences; // doesn't count gated DFA edges

            public int numFixedDecisions;
            public int minDecisionMaxFixedLookaheads;
            public int maxDecisionMaxFixedLookaheads;
            public int avgDecisionMaxFixedLookaheads;
            public int stddevDecisionMaxFixedLookaheads;
            public int numCyclicDecisions;
            public int minDecisionMaxCyclicLookaheads;
            public int maxDecisionMaxCyclicLookaheads;
            public int avgDecisionMaxCyclicLookaheads;
            public int stddevDecisionMaxCyclicLookaheads;
            //		int Stats.min(toArray(decisionMaxSynPredLookaheads);
            //		int Stats.max(toArray(decisionMaxSynPredLookaheads);
            //		int Stats.avg(toArray(decisionMaxSynPredLookaheads);
            //		int Stats.stddev(toArray(decisionMaxSynPredLookaheads);
            public int numSemanticPredicates;
            public int numTokens;
            public int numHiddenTokens;
            public int numCharsMatched;
            public int numHiddenCharsMatched;
            public int numReportedErrors;
            public int numMemoizationCacheHits;
            public int numMemoizationCacheMisses;
            public int numGuessingRuleInvocations;
            public int numMemoizationCacheEntries;
        }

        public class DecisionDescriptor
        {
            public int decision;
            public string fileName;
            public string ruleName;
            public int line;
            public int pos;
            public bool couldBacktrack;

            public int n;
            public float avgk; // avg across all decision events
            public int maxk;
            public int numBacktrackOccurrences;
            public int numSemPredEvals;
        }

        // all about a specific exec of a single decision
        public class DecisionEvent
        {
            public DecisionDescriptor decision;
            public int startIndex;
            public int k;
            public bool backtracks; // doesn't count gated DFA edges
            public bool evalSemPred;
            public DateTime startTime;
            public DateTime stopTime;
            public int numMemoizationCacheHits;
            public int numMemoizationCacheMisses;
        }
    }
}
