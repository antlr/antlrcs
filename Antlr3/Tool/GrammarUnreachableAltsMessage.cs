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
    using Antlr3.Extensions;

    using DecisionProbe = Antlr3.Analysis.DecisionProbe;
    using NFAState = Antlr3.Analysis.NFAState;
    using StringTemplate = Antlr4.StringTemplate.Template;

    /** Reports a potential parsing issue with a decision; the decision is
     *  nondeterministic in some way.
     */
    public class GrammarUnreachableAltsMessage : Message
    {
        public DecisionProbe probe;
        public int[] alts;

        public GrammarUnreachableAltsMessage( DecisionProbe probe,
                                             IEnumerable<int> alts )
            : base( ErrorManager.MSG_UNREACHABLE_ALTS )
        {
            this.probe = probe;
            this.alts = alts.ToArray();
            // flip msg ID if alts are actually token refs in Tokens rule
            if ( probe.Dfa.IsTokensRuleDecision )
            {
                MessageID = ErrorManager.MSG_UNREACHABLE_TOKENS;
            }
        }

        public override String ToString()
        {
            GrammarAST decisionASTNode = probe.Dfa.DecisionASTNode;
            line = decisionASTNode.Line;
            charPositionInLine = decisionASTNode.CharPositionInLine;
            String fileName = probe.Dfa.Nfa.Grammar.FileName;
            if ( fileName != null )
            {
                file = fileName;
            }

            StringTemplate st = GetMessageTemplate();

            if ( probe.Dfa.IsTokensRuleDecision )
            {
                // alts are token rules, convert to the names instead of numbers
                for ( int i = 0; i < alts.Length; i++ )
                {
                    int altI = alts[i];
                    String tokenName = probe.GetTokenNameForTokensRuleAlt( altI );
                    // reset the line/col to the token definition
                    NFAState ruleStart = probe.Dfa.Nfa.Grammar.GetRuleStartState( tokenName );
                    line = ruleStart.associatedASTNode.Line;
                    charPositionInLine = ruleStart.associatedASTNode.CharPositionInLine;
                    st.SetAttribute( "tokens", tokenName );
                }
            }
            else
            {
                // regular alt numbers, show the alts
                st.SetAttribute( "alts", alts );
            }

            return base.ToString( st );
        }

    }
}
