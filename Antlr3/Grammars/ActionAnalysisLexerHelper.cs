/*
 * [The "BSD licence"]
 * Copyright (c) 2011 Terence Parr
 * All rights reserved.
 *
 * Grammar conversion to ANTLR v3 and C#:
 * Copyright (c) 2011 Sam Harwell, Pixel Mine, Inc.
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions
 * are met:
 * 1. Redistributions of source code must retain the above copyright
 *	notice, this list of conditions and the following disclaimer.
 * 2. Redistributions in binary form must reproduce the above copyright
 *	notice, this list of conditions and the following disclaimer in the
 *	documentation and/or other materials provided with the distribution.
 * 3. The name of the author may not be used to endorse or promote products
 *	derived from this software without specific prior written permission.
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

namespace Antlr3.Grammars
{
    using Antlr.Runtime;
    using Antlr3.Tool;

    partial class ActionAnalysisLexer
    {
        private readonly Rule enclosingRule;
        private readonly Grammar grammar;
        private readonly IToken actionToken;
        private readonly int outerAltNum = 0;

        public ActionAnalysisLexer(Grammar grammar, string ruleName, GrammarAST actionAST)
            : this(new ANTLRStringStream(actionAST.Token.Text))
        {
            this.grammar = grammar;
            this.enclosingRule = grammar.GetLocallyDefinedRule(ruleName);
            this.actionToken = actionAST.Token;
            this.outerAltNum = actionAST.outerAltNum;
        }

        public void Analyze()
        {
            //System.Console.Out.WriteLine( "###\naction=" + actionToken );
            IToken t;
            do
            {
                t = NextToken();
            } while (t.Type != TokenTypes.EndOfFile);
        }

        private void HandleAttributeMemberReference(string attribute, string member)
        {
            AttributeScope scope = null;
            string refdRuleName = null;
            if (attribute.Equals(enclosingRule.Name))
            {
                // ref to enclosing rule.
                refdRuleName = attribute;
                scope = enclosingRule.GetLocalAttributeScope(member);
            }
            else if (enclosingRule.GetRuleLabel(attribute) != null)
            {
                // ref to rule label
                Grammar.LabelElementPair pair = enclosingRule.GetRuleLabel(attribute);
                pair.actionReferencesLabel = true;
                refdRuleName = pair.referencedRuleName;
                Rule refdRule = grammar.GetRule(refdRuleName);
                if (refdRule != null)
                {
                    scope = refdRule.GetLocalAttributeScope(member);
                }
            }
            else if (enclosingRule.GetRuleRefsInAlt(attribute, outerAltNum) != null)
            {
                // ref to rule referenced in this alt
                refdRuleName = attribute;
                Rule refdRule = grammar.GetRule(refdRuleName);
                if (refdRule != null)
                {
                    scope = refdRule.GetLocalAttributeScope(member);
                }
            }

            if (scope != null && (scope.isPredefinedRuleScope || scope.isPredefinedLexerRuleScope))
            {
                grammar.ReferenceRuleLabelPredefinedAttribute(refdRuleName);
                //System.Console.WriteLine("referenceRuleLabelPredefinedAttribute for " + refdRuleName);
            }
        }

        private void HandleRuleLabelReference(string ruleLabel)
        {
            Grammar.LabelElementPair pair = enclosingRule.GetRuleLabel(ruleLabel);
            pair.actionReferencesLabel = true;
        }

        private void HandleAttributeReference(string attributeName)
        {
            AttributeScope scope = enclosingRule.GetLocalAttributeScope(attributeName);
            if (scope != null && (scope.isPredefinedRuleScope || scope.isPredefinedLexerRuleScope))
            {
                grammar.ReferenceRuleLabelPredefinedAttribute(enclosingRule.Name);
                //System.Console.WriteLine("referenceRuleLabelPredefinedAttribute for " + attributeName);
            }
        }
    }
}
