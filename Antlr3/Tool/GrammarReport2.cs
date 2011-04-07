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
    using StringBuilder = System.Text.StringBuilder;

    /** Simplifying report dramatically for LL(*) paper.  Old results were
     *  wrong anyway it seems.  We need:
     *
     * 		percent decisions that potentially backtrack
     *  	histogram of regular lookahead depth (int k or *)
     */
    public class GrammarReport2
    {
        public static readonly string newline = Environment.NewLine;

        public Grammar root;

        public GrammarReport2(Grammar rootGrammar)
        {
            this.root = rootGrammar;
        }

        internal virtual void Stats(Grammar g, StringBuilder buf)
        {
            int numDec = g.NumberOfDecisions;
            for (int decision = 1; decision <= numDec; decision++)
            {
                Grammar.Decision d = g.GetDecision(decision);
                if (d.dfa == null)
                { // unusued decisions in auto synpreds
                    //System.err.println("no decision "+decision+" dfa for "+d.blockAST.toStringTree());
                    continue;
                }
                int k = d.dfa.MaxLookaheadDepth;
                Rule enclosingRule = d.dfa.NFADecisionStartState.enclosingRule;
                if (enclosingRule.IsSynPred)
                    continue; // don't count synpred rules
                buf.Append(g.name + "." + enclosingRule.Name + ":" +
                           "");
                GrammarAST decisionAST =
                    d.dfa.NFADecisionStartState.associatedASTNode;
                buf.Append(decisionAST.Line);
                buf.Append(":");
                buf.Append(decisionAST.CharPositionInLine);
                buf.Append(" decision " + decision + ":");

                if (d.dfa.IsCyclic)
                    buf.Append(" cyclic");
                if (k != int.MaxValue)
                    buf.Append(" k=" + k); // fixed, no sempreds
                if (d.dfa.HasSynPred)
                    buf.Append(" backtracks"); // isolated synpred not gated
                if (d.dfa.HasSemPred)
                    buf.Append(" sempred"); // user-defined sempred
                //			else {
                //				buf.append("undefined");
                //				FASerializer serializer = new FASerializer(g);
                //				String result = serializer.serialize(d.dfa.startState);
                //				System.err.println(result);
                //			}
                buf.AppendLine();
            }
        }

        public override string ToString()
        {
            StringBuilder buf = new StringBuilder();
            Stats(root, buf);
            CompositeGrammar composite = root.composite;
            foreach (Grammar g in composite.GetDelegates(root))
            {
                Stats(g, buf);
            }
            return buf.ToString();
        }
    }
}
