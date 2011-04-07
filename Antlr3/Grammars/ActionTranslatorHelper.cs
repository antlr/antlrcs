/*
 * [The "BSD license"]
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
    using System.Collections.Generic;
    using Antlr3.Codegen;

    using ANTLRStringStream = Antlr.Runtime.ANTLRStringStream;
    using Attribute = Antlr3.Tool.Attribute;
    using AttributeScope = Antlr3.Tool.AttributeScope;
    using CommonToken = Antlr.Runtime.CommonToken;
    using ErrorManager = Antlr3.Tool.ErrorManager;
    using Grammar = Antlr3.Tool.Grammar;
    using GrammarAST = Antlr3.Tool.GrammarAST;
    using IToken = Antlr.Runtime.IToken;
    using Rule = Antlr3.Tool.Rule;
    using StringBuilder = System.Text.StringBuilder;
    using StringComparison = System.StringComparison;
    using StringTemplate = Antlr3.ST.StringTemplate;
    using TokenTypes = Antlr.Runtime.TokenTypes;

    partial class ActionTranslator
    {
        public IList<object> chunks = new List<object>();
        Rule enclosingRule;
        int outerAltNum;
        Grammar grammar;
        CodeGenerator generator;
        IToken actionToken;

        public ActionTranslator(CodeGenerator generator,
                                     string ruleName,
                                     GrammarAST actionAST)
            : this(new ANTLRStringStream(actionAST.Token.Text))
        {
            this.generator = generator;
            this.grammar = generator.grammar;
            this.enclosingRule = grammar.GetLocallyDefinedRule(ruleName);
            this.actionToken = actionAST.Token;
            this.outerAltNum = actionAST.outerAltNum;
        }

        public ActionTranslator(CodeGenerator generator,
                                     string ruleName,
                                     IToken actionToken,
                                     int outerAltNum)
            : this(new ANTLRStringStream(actionToken.Text))
        {
            this.generator = generator;
            grammar = generator.grammar;
            this.enclosingRule = grammar.GetRule(ruleName);
            this.actionToken = actionToken;
            this.outerAltNum = outerAltNum;
        }

        /** Return a list of strings and StringTemplate objects that
         *  represent the translated action.
         */
        public IList<object> TranslateToChunks()
        {
            // JSystem.@out.println("###\naction="+action);
            IToken t;
            do
            {
                t = NextToken();
            } while (t.Type != TokenTypes.EndOfFile);
            return chunks;
        }

        public string Translate()
        {
            IList<object> theChunks = TranslateToChunks();
            //JSystem.@out.println("chunks="+a.chunks);
            StringBuilder buf = new StringBuilder();
            for (int i = 0; i < theChunks.Count; i++)
            {
                object o = (object)theChunks[i];
                buf.Append(o);
            }
            //JSystem.@out.println("translated: "+buf.toString());
            return buf.ToString();
        }

        public IList<object> TranslateAction(string action)
        {
            string rname = null;
            if (enclosingRule != null)
            {
                rname = enclosingRule.Name;
            }
            ActionTranslator translator =
                new ActionTranslator(generator,
                                          rname,
                                          new CommonToken(ANTLRParser.ACTION, action), outerAltNum);
            return translator.TranslateToChunks();
        }

        public bool IsTokenRefInAlt(string id)
        {
            return enclosingRule.GetTokenRefsInAlt(id, outerAltNum) != null;
        }
        public bool IsRuleRefInAlt(string id)
        {
            return enclosingRule.GetRuleRefsInAlt(id, outerAltNum) != null;
        }
        public Grammar.LabelElementPair GetElementLabel(string id)
        {
            return enclosingRule.GetLabel(id);
        }

        public void CheckElementRefUniqueness(string @ref, bool isToken)
        {
            IList<GrammarAST> refs = null;
            if (isToken)
            {
                refs = enclosingRule.GetTokenRefsInAlt(@ref, outerAltNum);
            }
            else
            {
                refs = enclosingRule.GetRuleRefsInAlt(@ref, outerAltNum);
            }
            if (refs != null && refs.Count > 1)
            {
                ErrorManager.GrammarError(ErrorManager.MSG_NONUNIQUE_REF,
                                          grammar,
                                          actionToken,
                                          @ref);
            }
        }

        /** For \$rulelabel.name, return the Attribute found for name.  It
         *  will be a predefined property or a return value.
         */
        public Attribute GetRuleLabelAttribute(string ruleName, string attrName)
        {
            Rule r = grammar.GetRule(ruleName);
            AttributeScope scope = r.GetLocalAttributeScope(attrName);
            if (scope != null && !scope.isParameterScope)
            {
                return scope.GetAttribute(attrName);
            }
            return null;
        }

        private AttributeScope ResolveDynamicScope(string scopeName)
        {
            if (grammar.GetGlobalScope(scopeName) != null)
            {
                return grammar.GetGlobalScope(scopeName);
            }
            Rule scopeRule = grammar.GetRule(scopeName);
            if (scopeRule != null)
            {
                return scopeRule.RuleScope;
            }
            return null; // not a valid dynamic scope
        }

        protected StringTemplate Template(string name)
        {
            StringTemplate st = generator.Templates.GetInstanceOf(name);
            chunks.Add(st);
            return st;
        }

        private bool CanMatchSetEnclosingRuleScopeAttribute(string attribute, string member)
        {
            return enclosingRule != null
                && string.Equals(enclosingRule.Name, attribute, StringComparison.Ordinal)
                && enclosingRule.GetLocalAttributeScope(member) != null;
        }

        private void HandleSetEnclosingRuleScopeAttribute(string x, string y, string expr)
        {
            StringTemplate st = null;
            AttributeScope scope = enclosingRule.GetLocalAttributeScope(y);
            if (scope.isPredefinedRuleScope)
            {
                if (y.Equals("st") || y.Equals("tree"))
                {
                    st = Template("ruleSetPropertyRef_" + y);
                    grammar.ReferenceRuleLabelPredefinedAttribute(x);
                    st.SetAttribute("scope", x);
                    st.SetAttribute("attr", y);
                    st.SetAttribute("expr", TranslateAction(expr));
                }
                else
                {
                    ErrorManager.GrammarError(ErrorManager.MSG_WRITE_TO_READONLY_ATTR, grammar, actionToken, x, y);
                }
            }
            else if (scope.isPredefinedLexerRuleScope)
            {
                // this is a better message to emit than the previous one...
                ErrorManager.GrammarError(ErrorManager.MSG_WRITE_TO_READONLY_ATTR, grammar, actionToken, x, y);
            }
            else if (scope.isParameterScope)
            {
                st = Template("parameterSetAttributeRef");
                st.SetAttribute("attr", scope.GetAttribute(y));
                st.SetAttribute("expr", TranslateAction(expr));
            }
            else
            { // must be return value
                st = Template("returnSetAttributeRef");
                st.SetAttribute("ruleDescriptor", enclosingRule);
                st.SetAttribute("attr", scope.GetAttribute(y));
                st.SetAttribute("expr", TranslateAction(expr));
            }
        }

        private bool CanMatchEnclosingRuleScopeAttribute(string x, string y)
        {
            return enclosingRule != null
                && string.Equals(x, enclosingRule.Name, StringComparison.Ordinal)
                && enclosingRule.GetLocalAttributeScope(y) != null;
        }

        private void HandleEnclosingRuleScopeAttribute(string x, string y)
        {
            if (IsRuleRefInAlt(x))
            {
                ErrorManager.GrammarError(ErrorManager.MSG_RULE_REF_AMBIG_WITH_RULE_IN_ALT, grammar, actionToken, x);
            }

            StringTemplate st = null;
            AttributeScope scope = enclosingRule.GetLocalAttributeScope(y);
            if (scope.isPredefinedRuleScope)
            {
                st = Template("rulePropertyRef_" + y);
                grammar.ReferenceRuleLabelPredefinedAttribute(x);
                st.SetAttribute("scope", x);
                st.SetAttribute("attr", y);
            }
            else if (scope.isPredefinedLexerRuleScope)
            {
                // perhaps not the most precise error message to use, but...
                ErrorManager.GrammarError(ErrorManager.MSG_RULE_HAS_NO_ARGS, grammar, actionToken, x);
            }
            else if (scope.isParameterScope)
            {
                st = Template("parameterAttributeRef");
                st.SetAttribute("attr", scope.GetAttribute(y));
            }
            else
            {
                // must be return value
                st = Template("returnAttributeRef");
                st.SetAttribute("ruleDescriptor", enclosingRule);
                st.SetAttribute("attr", scope.GetAttribute(y));
            }
        }

        private bool CanMatchSetTokenScopeAttribute(string x, string y)
        {
            return enclosingRule != null
                && input.LA(1) != '='
                && (enclosingRule.GetTokenLabel(x) != null || IsTokenRefInAlt(x))
                && AttributeScope.tokenScope.GetAttribute(y) != null;
        }

        private void HandleSetTokenScopeAttribute(string x, string y)
        {
            ErrorManager.GrammarError(ErrorManager.MSG_WRITE_TO_READONLY_ATTR, grammar, actionToken, x, y);
        }

        private bool CanMatchTokenScopeAttribute(string x, string y)
        {
            return enclosingRule != null
                && (enclosingRule.GetTokenLabel(x) != null || IsTokenRefInAlt(x))
                && AttributeScope.tokenScope.GetAttribute(y) != null
                && (grammar.type != Tool.GrammarType.Lexer
                    || GetElementLabel(x).elementRef.Token.Type == ANTLRParser.TOKEN_REF
                    || GetElementLabel(x).elementRef.Token.Type == ANTLRParser.STRING_LITERAL);
        }

        private void HandleTokenScopeAttribute(string x, string y)
        {
            string label = x;
            if (enclosingRule.GetTokenLabel(x) == null)
            {
                // \tokenref.attr  gotta get old label or compute new one
                CheckElementRefUniqueness(x, true);
                label = enclosingRule.GetElementLabel(x, outerAltNum, generator);
                if (label == null)
                {
                    ErrorManager.GrammarError(ErrorManager.MSG_FORWARD_ELEMENT_REF, grammar, actionToken, "$" + x + "." + y);
                    label = x;
                }
            }
            StringTemplate st = Template("tokenLabelPropertyRef_" + y);
            st.SetAttribute("scope", label);
            st.SetAttribute("attr", AttributeScope.tokenScope.GetAttribute(y));
        }

        private bool CanMatchSetRuleScopeAttribute(string x, string y)
        {
            // This asserts that if it's a label or a ref to a rule proceed but only if the attribute
            // is valid for that rule's scope
            if (enclosingRule == null || input.LA(1) == '=')
                return false;

            Grammar.LabelElementPair label = enclosingRule.GetRuleLabel(x);
            string ruleName = label != null ? label.referencedRuleName : x;
            string attributeName = y;

            return (label != null || IsRuleRefInAlt(x))
                && GetRuleLabelAttribute(ruleName, attributeName) != null;
        }

        private void HandleSetRuleScopeAttribute(string x, string y)
        {
            ErrorManager.GrammarError(ErrorManager.MSG_WRITE_TO_READONLY_ATTR, grammar, actionToken, x, y);
        }

        private bool CanMatchRuleScopeAttribute(string x, string y)
        {
            if (enclosingRule == null)
                return false;

            Grammar.LabelElementPair label = enclosingRule.GetRuleLabel(x);
            string ruleName = label != null ? label.referencedRuleName : x;
            string attributeName = y;

            return (label != null || IsRuleRefInAlt(x))
                && GetRuleLabelAttribute(ruleName, attributeName) != null;
        }

        private void HandleRuleScopeAttribute(string x, string y)
        {
            Grammar.LabelElementPair pair = enclosingRule.GetRuleLabel(x);
            string refdRuleName = pair != null ? pair.referencedRuleName : x;
            string label = x;
            if (pair == null)
            {
                // $ruleref.attr  gotta get old label or compute new one
                CheckElementRefUniqueness(x, false);
                label = enclosingRule.GetElementLabel(x, outerAltNum, generator);
                if (label == null)
                {
                    ErrorManager.GrammarError(ErrorManager.MSG_FORWARD_ELEMENT_REF, grammar, actionToken, "$" + x + "." + y);
                    label = x;
                }
            }

            StringTemplate st;
            Rule refdRule = grammar.GetRule(refdRuleName);
            AttributeScope scope = refdRule.GetLocalAttributeScope(y);
            if (scope.isPredefinedRuleScope)
            {
                st = Template("ruleLabelPropertyRef_" + y);
                grammar.ReferenceRuleLabelPredefinedAttribute(refdRuleName);
                st.SetAttribute("scope", label);
                st.SetAttribute("attr", y);
            }
            else if (scope.isPredefinedLexerRuleScope)
            {
                st = Template("lexerRuleLabelPropertyRef_" + y);
                grammar.ReferenceRuleLabelPredefinedAttribute(refdRuleName);
                st.SetAttribute("scope", label);
                st.SetAttribute("attr", y);
            }
            else if (scope.isParameterScope)
            {
                // TODO: error!
            }
            else
            {
                st = Template("ruleLabelRef");
                st.SetAttribute("referencedRule", refdRule);
                st.SetAttribute("scope", label);
                st.SetAttribute("attr", scope.GetAttribute(y));
            }
        }

        private bool CanMatchLabelReference(string label)
        {
            return enclosingRule != null
                && GetElementLabel(label) != null
                && enclosingRule.GetRuleLabel(label) == null;
        }

        private void HandleLabelReference(string label)
        {
            StringTemplate st;
            Grammar.LabelElementPair pair = GetElementLabel(label);
            if (pair.type == Tool.LabelType.RuleList ||
                  pair.type == Tool.LabelType.TokenList ||
                  pair.type == Tool.LabelType.WildcardTreeList)
            {
                st = Template("listLabelRef");
            }
            else
            {
                st = Template("tokenLabelRef");
            }

            st.SetAttribute("label", label);
        }

        private bool CanMatchIsolatedTokenReference(string name)
        {
            return enclosingRule != null
                && grammar.type != Tool.GrammarType.Lexer
                && IsTokenRefInAlt(name);
        }

        private void HandleIsolatedTokenReference(string name)
        {
            string label = enclosingRule.GetElementLabel(name, outerAltNum, generator);
            CheckElementRefUniqueness(name, true);
            if (label == null)
            {
                ErrorManager.GrammarError(ErrorManager.MSG_FORWARD_ELEMENT_REF, grammar, actionToken, name);
            }
            else
            {
                StringTemplate st = Template("tokenLabelRef");
                st.SetAttribute("label", label);
            }
        }

        private bool CanMatchIsolatedLexerRuleReference(string name)
        {
            return enclosingRule != null
                && grammar.type == Tool.GrammarType.Lexer
                && IsRuleRefInAlt(name);
        }

        private void HandleIsolatedLexerRuleReference(string name)
        {
            string label = enclosingRule.GetElementLabel(name, outerAltNum, generator);
            CheckElementRefUniqueness(name, false);
            if (label == null)
            {
                ErrorManager.GrammarError(ErrorManager.MSG_FORWARD_ELEMENT_REF, grammar, actionToken, name);
            }
            else
            {
                StringTemplate st = Template("lexerRuleLabel");
                st.SetAttribute("label", label);
            }
        }

        private bool CanMatchSetLocalAttribute(string attributeName)
        {
            return enclosingRule != null
                && enclosingRule.GetLocalAttributeScope(attributeName) != null
                && !enclosingRule.GetLocalAttributeScope(attributeName).isPredefinedLexerRuleScope;
        }

        private void HandleSetLocalAttribute(string attributeName, string expr)
        {
            StringTemplate st;
            AttributeScope scope = enclosingRule.GetLocalAttributeScope(attributeName);
            if (scope.isPredefinedRuleScope)
            {
                if (attributeName.Equals("tree") || attributeName.Equals("st"))
                {
                    st = Template("ruleSetPropertyRef_" + attributeName);
                    grammar.ReferenceRuleLabelPredefinedAttribute(enclosingRule.Name);
                    st.SetAttribute("scope", enclosingRule.Name);
                    st.SetAttribute("attr", attributeName);
                    st.SetAttribute("expr", TranslateAction(expr));
                }
                else
                {
                    ErrorManager.GrammarError(ErrorManager.MSG_WRITE_TO_READONLY_ATTR, grammar, actionToken, attributeName, "");
                }
            }
            else if (scope.isParameterScope)
            {
                st = Template("parameterSetAttributeRef");
                st.SetAttribute("attr", scope.GetAttribute(attributeName));
                st.SetAttribute("expr", TranslateAction(expr));
            }
            else
            {
                st = Template("returnSetAttributeRef");
                st.SetAttribute("ruleDescriptor", enclosingRule);
                st.SetAttribute("attr", scope.GetAttribute(attributeName));
                st.SetAttribute("expr", TranslateAction(expr));
            }
        }

        private bool CanMatchLocalAttribute(string name)
        {
            return enclosingRule != null
                && enclosingRule.GetLocalAttributeScope(name) != null;
        }

        private void HandleLocalAttribute(string name)
        {
            StringTemplate st;
            AttributeScope scope = enclosingRule.GetLocalAttributeScope(name);
            if (scope.isPredefinedRuleScope)
            {
                st = Template("rulePropertyRef_" + name);
                grammar.ReferenceRuleLabelPredefinedAttribute(enclosingRule.Name);
                st.SetAttribute("scope", enclosingRule.Name);
                st.SetAttribute("attr", name);
            }
            else if (scope.isPredefinedLexerRuleScope)
            {
                st = Template("lexerRulePropertyRef_" + name);
                st.SetAttribute("scope", enclosingRule.Name);
                st.SetAttribute("attr", name);
            }
            else if (scope.isParameterScope)
            {
                st = Template("parameterAttributeRef");
                st.SetAttribute("attr", scope.GetAttribute(name));
            }
            else
            {
                st = Template("returnAttributeRef");
                st.SetAttribute("ruleDescriptor", enclosingRule);
                st.SetAttribute("attr", scope.GetAttribute(name));
            }
        }

        private bool CanMatchSetDynamicScopeAttribute(string x, string y)
        {
            AttributeScope dynamicScope = ResolveDynamicScope(x);
            return dynamicScope != null
                && dynamicScope.GetAttribute(y) != null;
        }

        private void HandleSetDynamicScopeAttribute(string x, string y, string expr)
        {
            AttributeScope scope = ResolveDynamicScope(x);
            if (scope != null)
            {
                StringTemplate st = Template("scopeSetAttributeRef");
                st.SetAttribute("scope", x);
                st.SetAttribute("attr", scope.GetAttribute(y));
                st.SetAttribute("expr", TranslateAction(expr));
            }
            else
            {
                // error: invalid dynamic attribute
            }
        }

        private bool CanMatchDynamicScopeAttribute(string x, string y)
        {
            AttributeScope dynamicScope = ResolveDynamicScope(x);
            return dynamicScope != null
                && dynamicScope.GetAttribute(y) != null;
        }

        private void HandleDynamicScopeAttribute(string x, string y)
        {
            AttributeScope scope = ResolveDynamicScope(x);
            if (scope != null)
            {
                StringTemplate st = Template("scopeAttributeRef");
                st.SetAttribute("scope", x);
                st.SetAttribute("attr", scope.GetAttribute(y));
            }
            else
            {
                // error: invalid dynamic attribute
            }
        }

        private void HandleErrorScopedXY(string x, string y)
        {
            chunks.Add(Text);
            generator.IssueInvalidScopeError(x, y, enclosingRule, actionToken, outerAltNum);
        }

        private void HandleDynamicNegativeIndexedScopeAttribute(string x, string y, string expr)
        {
            StringTemplate st = Template("scopeAttributeRef");
            st.SetAttribute("scope", x);
            st.SetAttribute("attr", ResolveDynamicScope(x).GetAttribute(y));
            st.SetAttribute("negIndex", expr);
        }

        private void HandleDynamicAbsoluteIndexedScopeAttribute(string x, string y, string expr)
        {
            StringTemplate st = Template("scopeAttributeRef");
            st.SetAttribute("scope", x);
            st.SetAttribute("attr", ResolveDynamicScope(x).GetAttribute(y));
            st.SetAttribute("index", expr);
        }

        private bool CanMatchIsolatedDynamicScope(string name)
        {
            return ResolveDynamicScope(name) != null;
        }

        private void HandleIsolatedDynamicScope(string name)
        {
            StringTemplate st = Template("isolatedDynamicScopeRef");
            st.SetAttribute("scope", name);
        }

        private void HandleTemplateInstance(string text)
        {
            string action = text.Substring(1, text.Length - 1);
            string ruleName = "<outside-of-rule>";
            if (enclosingRule != null)
                ruleName = enclosingRule.Name;

            StringTemplate st = generator.TranslateTemplateConstructor(ruleName, outerAltNum, actionToken, action);
            if (st != null)
                chunks.Add(st);
        }

        private void HandleIndirectTemplateInstance(string text)
        {
            string action = text.Substring(1, text.Length - 1);
            StringTemplate st = generator.TranslateTemplateConstructor(enclosingRule.Name, outerAltNum, actionToken, action);
            chunks.Add(st);
        }

        private void HandleSetExpressionAttribute(string a, string id, string expr)
        {
            StringTemplate st = Template("actionSetAttribute");
            string action = a;
            action = action.Substring(1, action.Length - 2); // stuff inside {...}
            st.SetAttribute("st", TranslateAction(action));
            st.SetAttribute("attrName", id);
            st.SetAttribute("expr", TranslateAction(expr));
        }

        private void HandleSetAttribute(string x, string y, string expr)
        {
            StringTemplate st = Template("actionSetAttribute");
            st.SetAttribute("st", x);
            st.SetAttribute("attrName", y);
            st.SetAttribute("expr", TranslateAction(expr));
        }

        private void HandleTemplateExpression(string action)
        {
            StringTemplate st = Template("actionStringConstructor");
            action = action.Substring(1, action.Length - 2); // stuff inside {...}
            st.SetAttribute("stringExpr", TranslateAction(action));
        }

        private void HandleEscape(char escapeChar)
        {
            switch (escapeChar)
            {
            case '$':
            case '%':
                chunks.Add(escapeChar.ToString());
                return;

            default:
                chunks.Add(Text);
                return;
            }
        }

        private void HandleErrorXY(string x, string y)
        {
            chunks.Add(Text);
            generator.IssueInvalidAttributeError(x, y, enclosingRule, actionToken, outerAltNum);
        }

        private void HandleErrorX(string x)
        {
            chunks.Add(Text);
            generator.IssueInvalidAttributeError(x, enclosingRule, actionToken, outerAltNum);
        }

        private void HandleUnknownAttributeSyntax()
        {
            chunks.Add(Text);
            // shouldn't need an error here.  Just accept \$ if it doesn't look like anything
        }

        private void HandleUnknownTemplateSyntax()
        {
            chunks.Add(Text);
            ErrorManager.GrammarError(ErrorManager.MSG_INVALID_TEMPLATE_ACTION, grammar, actionToken, Text);
        }

        private void HandleText()
        {
            chunks.Add(Text);
        }
    }
}
