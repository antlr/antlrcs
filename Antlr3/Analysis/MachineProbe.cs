/*
 * [The "BSD licence"]
 * Copyright (c) 2005-2010 Terence Parr
 * All rights reserved.
 *
 * Conversion to C#:
 * Copyright (c) 2010 Sam Harwell, Pixel Mine, Inc.
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
    using Antlr3.Misc;
    using Antlr3.Tool;
    using Antlr.Runtime;

    public class MachineProbe
    {
        private DFA dfa;

        public MachineProbe(DFA dfa)
        {
            this.dfa = dfa;
        }

        internal List<DFAState> GetAnyDFAPathToTarget(DFAState targetState)
        {
            HashSet<DFAState> visited = new HashSet<DFAState>();
            return GetAnyDFAPathToTarget(dfa.startState, targetState, visited);
        }

        public List<DFAState> GetAnyDFAPathToTarget(DFAState startState, DFAState targetState, HashSet<DFAState> visited)
        {
            List<DFAState> dfaStates = new List<DFAState>();
            visited.Add(startState);
            if (startState.Equals(targetState))
            {
                dfaStates.Add(targetState);
                return dfaStates;
            }
            // for (Edge e : startState.edges) { // walk edges looking for valid
            // path
            for (int i = 0; i < startState.NumberOfTransitions; i++)
            {
                Transition e = startState.GetTransition(i);
                if (!visited.Contains((DFAState)e.Target))
                {
                    List<DFAState> path = GetAnyDFAPathToTarget((DFAState)e.target, targetState, visited);
                    if (path != null)
                    { // found path, we're done
                        dfaStates.Add(startState);
                        dfaStates.AddRange(path);
                        return dfaStates;
                    }
                }
            }
            return null;
        }

        /** Return a list of edge labels from start state to targetState. */
        public List<IIntSet> GetEdgeLabels(DFAState targetState)
        {
            List<DFAState> dfaStates = GetAnyDFAPathToTarget(targetState);
            List<IIntSet> labels = new List<IIntSet>();
            for (int i = 0; i < dfaStates.Count - 1; i++)
            {
                DFAState d = dfaStates[i];
                DFAState nextState = dfaStates[i + 1];
                // walk looking for edge whose target is next dfa state
                for (int j = 0; j < d.NumberOfTransitions; j++)
                {
                    Transition e = d.GetTransition(j);
                    if (e.target.stateNumber == nextState.stateNumber)
                    {
                        labels.Add(e.label.Set);
                    }
                }
            }
            return labels;
        }

        /**
         * Given List&lt;IntSet&gt;, return a String with a useful representation of the
         * associated input string. One could show something different for lexers
         * and parsers, for example.
         */
        public string GetInputSequenceDisplay(Grammar g, List<IIntSet> labels)
        {
            List<string> tokens = new List<string>();
            foreach (IIntSet label in labels)
                tokens.Add(label.ToString(g));
            return tokens.ToString();
        }

        /**
         * Given an alternative associated with a DFA state, return the list of
         * tokens (from grammar) associated with path through NFA following the
         * labels sequence. The nfaStates gives the set of NFA states associated
         * with alt that take us from start to stop. One of the NFA states in
         * nfaStates[i] will have an edge intersecting with labels[i].
         */
        public List<IToken> GetGrammarLocationsForInputSequence(List<HashSet<NFAState>> nfaStates, List<IIntSet> labels)
        {
            List<IToken> tokens = new List<IToken>();
            for (int i = 0; i < nfaStates.Count - 1; i++)
            {
                HashSet<NFAState> cur = nfaStates[i];
                HashSet<NFAState> next = nfaStates[i + 1];
                IIntSet label = labels[i];
                // find NFA state with edge whose label matches labels[i]
                foreach (NFAState p in cur)
                {
                    // walk p's transitions, looking for label
                    for (int j = 0; j < p.NumberOfTransitions; j++)
                    {
                        Transition t = p.transition[j];
                        if (!t.IsEpsilon && !t.label.Set.And(label).IsNil && next.Contains((NFAState)t.target))
                        {
                            if (p.associatedASTNode != null)
                            {
                                IToken oldtoken = p.associatedASTNode.Token;
                                CommonToken token = new CommonToken(oldtoken.Type, oldtoken.Text);
                                token.Line = (oldtoken.Line);
                                token.CharPositionInLine = (oldtoken.CharPositionInLine);
                                tokens.Add(token);
                                goto endNfaConfigLoop; // found path, move to next
                                // NFAState set
                            }
                        }
                    }
                }

            endNfaConfigLoop:
                continue;
            }
            return tokens;
        }

#if false
        /** Used to find paths through syntactically ambiguous DFA. If we've
         * seen statement number before, what did we learn?
         */
        protected Dictionary<int, int> stateReachable;

        public Dictionary<DFAState, HashSet<DFAState>> getReachSets(Collection<DFAState> targets)
        {
            Dictionary<DFAState, HashSet<DFAState>> reaches = new Dictionary<DFAState, HashSet<DFAState>>();
            // targets can reach themselves
            foreach (DFAState d in targets)
            {
                reaches[d] = new HashSet<DFAState>() { d };
            }

            bool changed = true;
            while (changed)
            {
                changed = false;
                foreach (DFAState d in dfa.states.values())
                {
                    if (d.NumberOfEdges == 0)
                        continue;

                    HashSet<DFAState> r;
                    if (!reaches.TryGetValue(d, out r))
                    {
                        r = new HashSet<DFAState>();
                        reaches[d] = r;
                    }

                    int before = r.Count;
                    // add all reaches from all edge targets
                    foreach (Edge e in d.edges)
                    {
                        //if ( targets.contains(e.target) ) r.add(e.target);
                        foreach (var state in reaches[e.target])
                            r.Add(state);
                    }
                    int after = r.Count;
                    if (after > before)
                        changed = true;
                }
            }

            return reaches;
        }
#endif
    }
}
