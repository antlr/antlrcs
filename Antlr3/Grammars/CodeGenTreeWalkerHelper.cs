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
    using Antlr3.Extensions;

    using ArgumentException = System.ArgumentException;
    using CommonToken = Antlr.Runtime.CommonToken;
    using Console = System.Console;
    using ErrorManager = Antlr3.Tool.ErrorManager;
    using Grammar = Antlr3.Tool.Grammar;
    using GrammarAST = Antlr3.Tool.GrammarAST;
    using GrammarType = Antlr3.Tool.GrammarType;
    using IIntSet = Antlr3.Misc.IIntSet;
    using IToken = Antlr.Runtime.IToken;
    using Label = Antlr3.Analysis.Label;
    using LabelType = Antlr3.Tool.LabelType;
    using MismatchedTokenException = Antlr.Runtime.MismatchedTokenException;
    using NoViableAltException = Antlr.Runtime.NoViableAltException;
    using RecognitionException = Antlr.Runtime.RecognitionException;
    using Rule = Antlr3.Tool.Rule;
    using RuleType = Antlr3.Tool.RuleType;
    using Template = Antlr4.StringTemplate.Template;
    using TemplateGroup = Antlr4.StringTemplate.TemplateGroup;

    partial class CodeGenTreeWalker
    {
        protected const int RULE_BLOCK_NESTING_LEVEL = 0;
        protected const int OUTER_REWRITE_NESTING_LEVEL = 0;

        protected internal string currentRuleName = null;
        protected int blockNestingLevel = 0;
        protected int rewriteBlockNestingLevel = 0;
        protected internal int outerAltNum = 0;
        protected Template currentBlockST = null;
        protected bool currentAltHasASTRewrite = false;
        protected int rewriteTreeNestingLevel = 0;
        protected HashSet<object> rewriteRuleRefs = null;

        public override void ReportError( RecognitionException ex )
        {
            IToken token = null;
            if ( ex is MismatchedTokenException )
            {
                token = ( (MismatchedTokenException)ex ).Token;
            }
            else if ( ex is NoViableAltException )
            {
                token = ( (NoViableAltException)ex ).Token;
            }
            ErrorManager.SyntaxError(
                ErrorManager.MSG_SYNTAX_ERROR,
                grammar,
                token,
                "codegen: " + ex.ToString(),
                ex );
        }

        public void ReportError( string s )
        {
            Console.Out.WriteLine( "codegen: error: " + s );
        }

        protected CodeGenerator generator;
        protected Grammar grammar;
        protected TemplateGroup templates;

        /** The overall lexer/parser template; simulate dynamically scoped
         *  attributes by making this an instance var of the walker.
         */
        protected Template recognizerST;

        protected Template outputFileST;
        protected Template headerFileST;

        protected string outputOption = "";

        protected Template GetWildcardST( GrammarAST elementAST, GrammarAST ast_suffix, string label )
        {
            string name = "wildcard";
            if ( grammar.type == GrammarType.Lexer )
            {
                name = "wildcardChar";
            }
            return GetTokenElementST( name, name, elementAST, ast_suffix, label );
        }

        protected Template GetRuleElementST( string name,
                                                  string ruleTargetName,
                                                  GrammarAST elementAST,
                                                  GrammarAST ast_suffix,
                                                  string label )
        {
            string suffix = GetSTSuffix( elementAST, ast_suffix, label );
            name += suffix;
            // if we're building trees and there is no label, gen a label
            // unless we're in a synpred rule.
            Rule r = grammar.GetRule( currentRuleName );
            if ( ( grammar.BuildAST || suffix.Length > 0 ) && label == null &&
                 ( r == null || !r.IsSynPred ) )
            {
                // we will need a label to do the AST or tracking, make one
                label = generator.CreateUniqueLabel( ruleTargetName );
                CommonToken labelTok = new CommonToken( ANTLRParser.ID, label );
                grammar.DefineRuleRefLabel( currentRuleName, labelTok, elementAST );
            }
            Template elementST = templates.GetInstanceOf( name );
            if ( label != null )
            {
                elementST.SetAttribute( "label", label );
            }
            return elementST;
        }

        protected Template GetTokenElementST( string name,
                                                   string elementName,
                                                   GrammarAST elementAST,
                                                   GrammarAST ast_suffix,
                                                   string label )
        {
            bool tryUnchecked = false;
            if (name == "matchSet" && !string.IsNullOrEmpty(elementAST.enclosingRuleName) && Rule.GetRuleType(elementAST.enclosingRuleName) == RuleType.Lexer)
            {
                if ( ( elementAST.Parent.Type == ANTLRLexer.ALT && elementAST.Parent.Parent.Parent.Type == RULE && elementAST.Parent.Parent.ChildCount == 2 )
                    || ( elementAST.Parent.Type == ANTLRLexer.NOT && elementAST.Parent.Parent.Parent.Parent.Type == RULE && elementAST.Parent.Parent.Parent.ChildCount == 2 ) )
                {
                    // single alt at the start of the rule needs to be checked
                }
                else
                {
                    tryUnchecked = true;
                }
            }

            string suffix = GetSTSuffix( elementAST, ast_suffix, label );
            // if we're building trees and there is no label, gen a label
            // unless we're in a synpred rule.
            Rule r = grammar.GetRule( currentRuleName );
            if ( ( grammar.BuildAST || suffix.Length > 0 ) && label == null &&
                 ( r == null || !r.IsSynPred ) )
            {
                label = generator.CreateUniqueLabel( elementName );
                CommonToken labelTok = new CommonToken( ANTLRParser.ID, label );
                grammar.DefineTokenRefLabel( currentRuleName, labelTok, elementAST );
            }

            Template elementST = null;
            if ( tryUnchecked && templates.IsDefined( name + "Unchecked" + suffix ) )
                elementST = templates.GetInstanceOf( name + "Unchecked" + suffix );
            if ( elementST == null )
                elementST = templates.GetInstanceOf( name + suffix );

            if ( label != null )
            {
                elementST.SetAttribute( "label", label );
            }
            return elementST;
        }

        public bool IsListLabel( string label )
        {
            bool hasListLabel = false;
            if ( label != null )
            {
                Rule r = grammar.GetRule( currentRuleName );
                //String stName = null;
                if ( r != null )
                {
                    Grammar.LabelElementPair pair = r.GetLabel( label );
                    if ( pair != null &&
                         ( pair.type == LabelType.TokenList ||
                          pair.type == LabelType.RuleList ||
                          pair.type == LabelType.WildcardTreeList ) )
                    {
                        hasListLabel = true;
                    }
                }
            }
            return hasListLabel;
        }

        /** Return a non-empty template name suffix if the token is to be
         *  tracked, added to a tree, or both.
         */
        protected string GetSTSuffix( GrammarAST elementAST, GrammarAST ast_suffix, string label )
        {
            if ( grammar.type == GrammarType.Lexer )
            {
                return "";
            }
            // handle list label stuff; make element use "Track"

            string operatorPart = "";
            string rewritePart = "";
            string listLabelPart = "";
            Rule ruleDescr = grammar.GetRule( currentRuleName );
            if ( ast_suffix != null && !ruleDescr.IsSynPred )
            {
                if ( ast_suffix.Type == ANTLRParser.ROOT )
                {
                    operatorPart = "RuleRoot";
                }
                else if ( ast_suffix.Type == ANTLRParser.BANG )
                {
                    operatorPart = "Bang";
                }
            }
            if ( currentAltHasASTRewrite && elementAST.Type != WILDCARD )
            {
                rewritePart = "Track";
            }
            if ( IsListLabel( label ) )
            {
                listLabelPart = "AndListLabel";
            }
            string STsuffix = operatorPart + rewritePart + listLabelPart;
            //JSystem.@out.println("suffix = "+STsuffix);

            return STsuffix;
        }

        /** Convert rewrite AST lists to target labels list */
        protected IList<string> GetTokenTypesAsTargetLabels( HashSet<GrammarAST> refs )
        {
            if ( refs == null || refs.Count == 0 )
                return null;

            IList<string> labels = new List<string>( refs.Count );
            foreach ( GrammarAST t in refs )
            {
                string label;
                if ( t.Type == ANTLRParser.RULE_REF || t.Type == ANTLRParser.TOKEN_REF || t.Type == ANTLRParser.LABEL)
                {
                    label = t.Text;
                }
                else
                {
                    // must be char or string literal
                    label = generator.GetTokenTypeAsTargetLabel(grammar.GetTokenType(t.Text));
                }
                labels.Add( label );
            }
            return labels;
        }

        public void Init( Grammar g )
        {
            this.grammar = g;
            this.generator = grammar.CodeGenerator;
            this.templates = generator.Templates;
        }

        private void HandleGrammarInit(Grammar g, Template recognizerST, Template outputFileST, Template headerFileST)
        {
            if (state.backtracking == 0)
            {
                Init(g);
                this.recognizerST = recognizerST;
                this.outputFileST = outputFileST;
                this.headerFileST = headerFileST;
                string superClass = (string)g.GetOption("superClass");
                outputOption = (string)g.GetOption("output");
                if (superClass != null)
                    recognizerST.SetAttribute("superClass", superClass);

                if (g.type != GrammarType.Lexer)
                {
                    object labelType = g.GetOption("ASTLabelType");
                    if (labelType != null)
                        recognizerST.SetAttribute("ASTLabelType", labelType);
                }

                if (g.type == GrammarType.TreeParser && g.GetOption("ASTLabelType") == null)
                    ErrorManager.GrammarWarning(ErrorManager.MSG_MISSING_AST_TYPE_IN_TREE_GRAMMAR, g, null, g.name);

                if (g.type != GrammarType.TreeParser)
                {
                    object labelType = g.GetOption("TokenLabelType");
                    if (labelType != null)
                        recognizerST.SetAttribute("labelType", labelType);
                }

                recognizerST.SetAttribute("numRules", grammar.Rules.Count);
                outputFileST.SetAttribute("numRules", grammar.Rules.Count);
                headerFileST.SetAttribute("numRules", grammar.Rules.Count);
            }
        }

        private void HandleGrammarSpecAfterName(GrammarAST comment)
        {
            if (comment != null)
            {
                outputFileST.SetAttribute("docComment", comment.Text);
                headerFileST.SetAttribute("docComment", comment.Text);
            }

            recognizerST.SetAttribute("name", grammar.GetRecognizerName());
            outputFileST.SetAttribute("name", grammar.GetRecognizerName());
            headerFileST.SetAttribute("name", grammar.GetRecognizerName());
            recognizerST.SetAttribute("scopes", grammar.GlobalScopes);
            headerFileST.SetAttribute("scopes", grammar.GlobalScopes);
        }

        private void HandleRulesInit(GrammarAST start, out string ruleName, out bool generated)
        {
            ruleName = start.GetChild(0).Text;
            generated = grammar.GenerateMethodForRule(ruleName);
        }

        private void HandleRulesRule(Template ruleTemplate)
        {
            if (ruleTemplate != null)
            {
                recognizerST.SetAttribute("rules", ruleTemplate);
                outputFileST.SetAttribute("rules", ruleTemplate);
                headerFileST.SetAttribute("rules", ruleTemplate);
            }
        }

        private void HandleRulesForcedAfterRule(ref string ruleName, ref bool generated)
        {
            if (input.LA(1) == RULE)
            {
                ruleName = ((GrammarAST)input.LT(1)).GetChild(0).Text;
                //System.Diagnostics.Debug.Assert( ruleName == ((GrammarAST)input.LT(1)).enclosingRuleName );
                generated = grammar.GenerateMethodForRule(ruleName);
            }
        }

        private void HandleRuleInit(GrammarAST start, out string initAction, out GrammarAST block2, out Antlr3.Analysis.DFA dfa, out Rule ruleDescr, out string description)
        {
            initAction = null;
            // get the dfa for the BLOCK
            block2 = (GrammarAST)start.GetFirstChildWithType(BLOCK);
            dfa = block2.LookaheadDFA;
            // init blockNestingLevel so it's block level RULE_BLOCK_NESTING_LEVEL
            // for alts of rule
            blockNestingLevel = RULE_BLOCK_NESTING_LEVEL - 1;
            ruleDescr = grammar.GetRule(start.GetChild(0).Text);
            currentRuleName = start.GetChild(0).Text;
            description = string.Empty;
        }

        private void HandleRuleAfterId(GrammarAST id)
        {
            System.Diagnostics.Debug.Assert(currentRuleName == id.Text);
        }

        private void HandleRuleAfterBlock(out Template code, GrammarAST start, GrammarAST block2, Rule ruleDescr, string description, Template blockCode)
        {
            description = grammar.GrammarTreeToString((GrammarAST)start.GetFirstChildWithType(BLOCK), false);
            description = generator.Target.GetTargetStringLiteralFromString(description);
            blockCode.SetAttribute("description", description);
            // do not generate lexer rules in combined grammar
            string stName = null;
            if (ruleDescr.IsSynPred)
            {
                stName = "synpredRule";
            }
            else if (grammar.type == GrammarType.Lexer)
            {
                if (currentRuleName.Equals(Grammar.ArtificialTokensRuleName))
                    stName = "tokensRule";
                else
                    stName = "lexerRule";
            }
            else
            {
                if (!(grammar.type == GrammarType.Combined && Rule.GetRuleType(currentRuleName) == RuleType.Lexer))
                    stName = "rule";
            }

            code = templates.GetInstanceOf(stName);
            if (code.Name.Equals("/rule"))
                code.SetAttribute("emptyRule", grammar.IsEmptyRule(block2));

            code.SetAttribute("ruleDescriptor", ruleDescr);
            string memo = (string)grammar.GetBlockOption(start, "memoize");
            if (memo == null)
                memo = (string)grammar.GetOption("memoize");

            if (memo != null && memo.Equals("true") && (stName.Equals("rule") || stName.Equals("lexerRule")))
                code.SetAttribute("memoize", memo != null && memo.Equals("true"));
        }

        private void HandleRuleEnd(Template code, GrammarAST start, string initAction, string description, GrammarAST modifier, Template blockCode)
        {
            if (code != null)
            {
                if (grammar.type == GrammarType.Lexer)
                {
                    bool naked =
                        currentRuleName.Equals(Grammar.ArtificialTokensRuleName) ||
                        (modifier != null && modifier.Text.Equals(Grammar.FragmentRuleModifier));
                    code.SetAttribute("nakedBlock", naked);
                }
                else
                {
                    description = grammar.GrammarTreeToString(start, false);
                    description = generator.Target.GetTargetStringLiteralFromString(description);
                    code.SetAttribute("description", description);
                }

                Rule theRule = grammar.GetRule(currentRuleName);
                generator.TranslateActionAttributeReferencesForSingleScope(theRule, theRule.Actions);
                code.SetAttribute("ruleName", currentRuleName);
                code.SetAttribute("block", blockCode);
                if (initAction != null)
                {
                    code.SetAttribute("initAction", initAction);
                }
            }
        }

        private void HandleBlockInit(string blockTemplateName, Antlr3.Analysis.DFA dfa, ref Template code, out int altNum)
        {
            altNum = 0;
            blockNestingLevel++;

            if (state.backtracking == 0)
            {
                Template decision = null;
                if (dfa != null)
                {
                    code = templates.GetInstanceOf(blockTemplateName);
                    decision = generator.GenLookaheadDecision(recognizerST, dfa);
                    code.SetAttribute("decision", decision);
                    code.SetAttribute("decisionNumber", dfa.NfaStartStateDecisionNumber);
                    code.SetAttribute("maxK", dfa.MaxLookaheadDepth);
                    code.SetAttribute("maxAlt", dfa.NumberOfAlts);
                }
                else
                {
                    code = templates.GetInstanceOf(blockTemplateName + "SingleAlt");
                }
                code.SetAttribute("blockLevel", blockNestingLevel);
                code.SetAttribute("enclosingBlockLevel", blockNestingLevel - 1);
                altNum = 1;
                if (this.blockNestingLevel == RULE_BLOCK_NESTING_LEVEL)
                {
                    this.outerAltNum = 1;
                }
            }
        }

        private void HandleBlockSetBlock(Template code, Template setBlockCode)
        {
            code.SetAttribute("alts", setBlockCode);
        }

        private void HandleBlockAlternative(Template code, ref int altNum, Template altCode, GrammarAST rewStart, Template rewCode)
        {
            if (this.blockNestingLevel == RULE_BLOCK_NESTING_LEVEL)
            {
                this.outerAltNum++;
            }

            // add the rewrite code as just another element in the alt :)
            // (unless it's a " -> ..." rewrite
            // ( -> ... )
            GrammarAST firstRewriteAST = (GrammarAST)rewStart.FindFirstType(REWRITE);
            bool etc =
                rewStart.Type == REWRITES &&
                firstRewriteAST.GetChild(0) != null &&
                firstRewriteAST.GetChild(0).Type == ETC;

            if (rewCode != null && !etc)
            {
                altCode.SetAttribute("rew", rewCode);
            }

            // add this alt to the list of alts for this block
            code.SetAttribute("alts", altCode);
            altCode.SetAttribute("altNum", altNum);
            altCode.SetAttribute("outerAlt", blockNestingLevel == RULE_BLOCK_NESTING_LEVEL);
            altNum++;
        }

        private void HandleSetBlock(out Template code, GrammarAST start, GrammarAST s)
        {
            if (blockNestingLevel == RULE_BLOCK_NESTING_LEVEL && grammar.BuildAST)
            {
                Rule r = grammar.GetRule(currentRuleName);
                currentAltHasASTRewrite = r.HasRewrite(outerAltNum);
                if (currentAltHasASTRewrite)
                    r.TrackTokenReferenceInAlt(start, outerAltNum);
            }

            Template setcode;
            int i = s.Token.TokenIndex;
            if (blockNestingLevel == RULE_BLOCK_NESTING_LEVEL)
                setcode = GetTokenElementST("matchRuleBlockSet", "set", s, null, null);
            else
                setcode = GetTokenElementST("matchSet", "set", s, null, null);

            setcode.SetAttribute("elementIndex", i);
            //if ( grammar.type!=GrammarType.Lexer )
            //{
            //	generator.GenerateLocalFollow($s,"set",currentRuleName,i);
            //}

            setcode.SetAttribute("s", generator.GenSetExpr(templates, s.SetValue, 1, false));
            Template altcode = templates.GetInstanceOf("alt");
            altcode.SetAttribute("elements.{el,line,pos}", setcode, s.Line, s.CharPositionInLine + 1);
            altcode.SetAttribute("altNum", 1);
            altcode.SetAttribute("outerAlt", blockNestingLevel == RULE_BLOCK_NESTING_LEVEL);
            if (!currentAltHasASTRewrite && grammar.BuildAST)
                altcode.SetAttribute("autoAST", true);

            altcode.SetAttribute("treeLevel", rewriteTreeNestingLevel);
            code = altcode;
        }

        private void HandleBlockFinally()
        {
            blockNestingLevel--;
        }

        private void HandleExceptionHandler(Template ruleTemplate, GrammarAST argAction, GrammarAST action)
        {
            IList<object> chunks = generator.TranslateAction(currentRuleName, action);
            ruleTemplate.SetAttribute("exceptions.{decl,action}", argAction.Text, chunks);
        }

        private void HandleFinallyClause(Template ruleTemplate, GrammarAST action)
        {
            IList<object> chunks = generator.TranslateAction(currentRuleName, action);
            ruleTemplate.SetAttribute("finally", chunks);
        }

        private void HandleAlternativeInit(out Template code, GrammarAST start)
        {
            code = null;
            if (state.backtracking == 0)
            {
                code = templates.GetInstanceOf("alt");
                if (blockNestingLevel == RULE_BLOCK_NESTING_LEVEL && grammar.BuildAST)
                {
                    Rule r = grammar.GetRule(currentRuleName);
                    currentAltHasASTRewrite = r.HasRewrite(outerAltNum);
                }

                string description = grammar.GrammarTreeToString(start, false);
                description = generator.Target.GetTargetStringLiteralFromString(description);
                code.SetAttribute("description", description);
                code.SetAttribute("treeLevel", rewriteTreeNestingLevel);
                if (!currentAltHasASTRewrite && grammar.BuildAST)
                {
                    code.SetAttribute("autoAST", true);
                }
            }
        }

        private void HandleAlternativeElement(Template code, GrammarAST elementStart, Template elementCode)
        {
            if (elementStart != null && elementCode != null)
            {
                code.SetAttribute("elements.{el,line,pos}", elementCode, elementStart.Line, elementStart.CharPositionInLine + 1);
            }
        }

        private void HandleElementCharRange(GrammarAST label, out Template code, GrammarAST a, GrammarAST b)
        {
            code = templates.GetInstanceOf("charRangeRef");
            string low = generator.Target.GetTargetCharLiteralFromANTLRCharLiteral(generator, a.Text);
            string high = generator.Target.GetTargetCharLiteralFromANTLRCharLiteral(generator, b.Text);
            code.SetAttribute("a", low);
            code.SetAttribute("b", high);
            if (label != null)
                code.SetAttribute("label", label.Text);
        }

        private void HandleElementSemanticPredicate(out Template code, GrammarAST sp)
        {
            code = templates.GetInstanceOf("validateSemanticPredicate");
            code.SetAttribute("pred", generator.TranslateAction(currentRuleName, sp));
            string description = generator.Target.GetTargetStringLiteralFromString(sp.Text);
            code.SetAttribute("description", description);
        }

        private void HandleElementAction(out Template code, GrammarAST action, bool forced)
        {
            code = templates.GetInstanceOf(forced ? "execForcedAction" : "execAction");
            code.SetAttribute("action", generator.TranslateAction(currentRuleName, action));
        }

        private void HandleNotElementCharLiteral(out IIntSet elements, GrammarAST assign_c)
        {
            int ttype = 0;
            if (grammar.type == GrammarType.Lexer)
                ttype = Grammar.GetCharValueFromGrammarCharLiteral(assign_c.Text);
            else
                ttype = grammar.GetTokenType(assign_c.Text);

            elements = grammar.Complement(ttype);
        }

        private void HandleNotElementStringLiteral(out IIntSet elements, GrammarAST assign_s)
        {
            int ttype = 0;
            if (grammar.type == GrammarType.Lexer)
            {
                // TODO: error!
            }
            else
            {
                ttype = grammar.GetTokenType(assign_s.Text);
            }

            elements = grammar.Complement(ttype);
        }

        private void HandleNotElementTokenReference(out IIntSet elements, GrammarAST assign_t)
        {
            int ttype = grammar.GetTokenType(assign_t.Text);
            elements = grammar.Complement(ttype);
        }

        private void HandleNotElementBlock(out IIntSet elements, GrammarAST assign_st)
        {
            elements = assign_st.SetValue;
            elements = grammar.Complement(elements);
        }

        private void HandleNotElementEnd(GrammarAST n, GrammarAST label, GrammarAST astSuffix, out Template code, IIntSet elements, GrammarAST start)
        {
            if (n.GetChild(0) != start)
                throw new System.InvalidOperationException();

            string labelText = null;
            if (label != null)
                labelText = label.Text;

            code = GetTokenElementST("matchSet", "set", (GrammarAST)n.GetChild(0), astSuffix, labelText);
            code.SetAttribute("s", generator.GenSetExpr(templates, elements, 1, false));
            int i = n.Token.TokenIndex;
            code.SetAttribute("elementIndex", i);
            if (grammar.type != GrammarType.Lexer)
                generator.GenerateLocalFollow(n, "set", currentRuleName, i);
        }

        private void HandleAtomInit(GrammarAST start, GrammarAST label, ref GrammarAST astSuffix, out string labelText)
        {
            labelText = null;
            if (state.backtracking == 0)
            {
                if (label != null)
                    labelText = label.Text;

                if (grammar.type != GrammarType.Lexer)
                {
                    switch (start.Type)
                    {
                    case RULE_REF:
                    case TOKEN_REF:
                    case CHAR_LITERAL:
                    case STRING_LITERAL:
                        Rule encRule = grammar.GetRule(start.enclosingRuleName);
                        if (encRule != null && encRule.HasRewrite(outerAltNum) && astSuffix != null)
                        {
                            ErrorManager.GrammarError(ErrorManager.MSG_AST_OP_IN_ALT_WITH_REWRITE, grammar, start.Token, start.enclosingRuleName, outerAltNum);
                            astSuffix = null;
                        }

                        break;

                    default:
                        break;
                    }
                }
            }
        }

        private void HandleAtomRuleReference(GrammarAST scope, GrammarAST label, GrammarAST astSuffix, out Template code, string labelText, GrammarAST r, GrammarAST rarg)
        {
            grammar.CheckRuleReference(scope, r, rarg, currentRuleName);
            string scopeName = null;
            if (scope != null)
            {
                scopeName = scope.Text;
            }

            Rule rdef = grammar.GetRule(scopeName, r.Text);
            // don't insert label=r() if $label.attr not used, no ret value, ...
            if (!rdef.HasReturnValue)
            {
                labelText = null;
            }

            code = GetRuleElementST("ruleRef", r.Text, r, astSuffix, labelText);
            code.SetAttribute("rule", rdef);
            if (scope != null)
            {
                // scoped rule ref
                Grammar scopeG = grammar.composite.GetGrammar(scope.Text);
                code.SetAttribute("scope", scopeG);
            }
            else if (rdef.Grammar != this.grammar)
            {
                // nonlocal
                // if rule definition is not in this grammar, it's nonlocal
                IList<Grammar> rdefDelegates = rdef.Grammar.GetDelegates();
                if (rdefDelegates.Contains(this.grammar))
                {
                    code.SetAttribute("scope", rdef.Grammar);
                }
                else
                {
                    // defining grammar is not a delegate, scope all the
                    // back to root, which has delegate methods for all
                    // rules.  Don't use scope if we are root.
                    if (this.grammar != rdef.Grammar.composite.DelegateGrammarTreeRoot.Grammar)
                    {
                        code.SetAttribute("scope",
                                          rdef.Grammar.composite.DelegateGrammarTreeRoot.Grammar);
                    }
                }
            }

            if (rarg != null)
            {
                IList<object> args = generator.TranslateAction(currentRuleName, rarg);
                code.SetAttribute("args", args);
            }
            int i = r.Token.TokenIndex;
            code.SetAttribute("elementIndex", i);
            generator.GenerateLocalFollow(r, r.Text, currentRuleName, i);
            r.code = code;
        }

        private void HandleAtomTokenReference(GrammarAST scope, GrammarAST label, GrammarAST astSuffix, out Template code, string labelText, GrammarAST t, GrammarAST targ)
        {
            if (currentAltHasASTRewrite && t.terminalOptions != null && t.terminalOptions[Grammar.defaultTokenOption] != null)
            {
                ErrorManager.GrammarError(ErrorManager.MSG_HETERO_ILLEGAL_IN_REWRITE_ALT, grammar, t.Token, t.Text);
            }

            grammar.CheckRuleReference(scope, t, targ, currentRuleName);
            if (grammar.type == GrammarType.Lexer)
            {
                if (grammar.GetTokenType(t.Text) == Label.EOF)
                {
                    code = templates.GetInstanceOf("lexerMatchEOF");
                }
                else
                {
                    code = templates.GetInstanceOf("lexerRuleRef");
                    if (IsListLabel(labelText))
                        code = templates.GetInstanceOf("lexerRuleRefAndListLabel");

                    string scopeName = null;
                    if (scope != null)
                        scopeName = scope.Text;

                    Rule rdef2 = grammar.GetRule(scopeName, t.Text);
                    code.SetAttribute("rule", rdef2);
                    if (scope != null)
                    {
                        // scoped rule ref
                        Grammar scopeG = grammar.composite.GetGrammar(scope.Text);
                        code.SetAttribute("scope", scopeG);
                    }
                    else if (rdef2.Grammar != this.grammar)
                    {
                        // nonlocal
                        // if rule definition is not in this grammar, it's nonlocal
                        code.SetAttribute("scope", rdef2.Grammar);
                    }

                    if (targ != null)
                    {
                        IList<object> args = generator.TranslateAction(currentRuleName, targ);
                        code.SetAttribute("args", args);
                    }
                }

                int i = t.Token.TokenIndex;
                code.SetAttribute("elementIndex", i);
                if (label != null)
                    code.SetAttribute("label", labelText);
            }
            else
            {
                code = GetTokenElementST("tokenRef", t.Text, t, astSuffix, labelText);
                string tokenLabel = generator.GetTokenTypeAsTargetLabel(grammar.GetTokenType(t.Text));
                code.SetAttribute("token", tokenLabel);
                if (!currentAltHasASTRewrite && t.terminalOptions != null)
                    code.SetAttribute("terminalOptions", t.terminalOptions);

                int i = t.Token.TokenIndex;
                code.SetAttribute("elementIndex", i);
                generator.GenerateLocalFollow(t, tokenLabel, currentRuleName, i);
            }

            t.code = code;
        }

        private void HandleAtomCharLiteral(GrammarAST label, GrammarAST astSuffix, out Template code, string labelText, GrammarAST c)
        {
            if (grammar.type == GrammarType.Lexer)
            {
                code = templates.GetInstanceOf("charRef");
                code.SetAttribute("char", generator.Target.GetTargetCharLiteralFromANTLRCharLiteral(generator, c.Text));
                if (label != null)
                    code.SetAttribute("label", labelText);
            }
            else
            {
                // else it's a token type reference
                code = GetTokenElementST("tokenRef", "char_literal", c, astSuffix, labelText);
                string tokenLabel = generator.GetTokenTypeAsTargetLabel(grammar.GetTokenType(c.Text));
                code.SetAttribute("token", tokenLabel);
                if (c.terminalOptions != null)
                    code.SetAttribute("terminalOptions", c.terminalOptions);

                int i = c.Token.TokenIndex;
                code.SetAttribute("elementIndex", i);
                generator.GenerateLocalFollow(c, tokenLabel, currentRuleName, i);
            }
        }

        private void HandleAtomStringLiteral(GrammarAST label, GrammarAST astSuffix, out Template code, string labelText, GrammarAST s)
        {
            int i = s.Token.TokenIndex;
            if (grammar.type == GrammarType.Lexer)
            {
                code = templates.GetInstanceOf("lexerStringRef");
                code.SetAttribute("string",
                    generator.Target.GetTargetStringLiteralFromANTLRStringLiteral(generator, s.Text));
                code.SetAttribute("elementIndex", i);
                if (label != null)
                    code.SetAttribute("label", labelText);
            }
            else
            {
                // else it's a token type reference
                code = GetTokenElementST("tokenRef", "string_literal", s, astSuffix, labelText);
                string tokenLabel =
                    generator.GetTokenTypeAsTargetLabel(grammar.GetTokenType(s.Text));
                code.SetAttribute("token", tokenLabel);
                if (s.terminalOptions != null)
                {
                    code.SetAttribute("terminalOptions", s.terminalOptions);
                }
                code.SetAttribute("elementIndex", i);
                generator.GenerateLocalFollow(s, tokenLabel, currentRuleName, i);
            }
        }

        private void HandleAtomWildcard(GrammarAST astSuffix, out Template code, string labelText, GrammarAST w)
        {
            code = GetWildcardST(w, astSuffix, labelText);
            code.SetAttribute("elementIndex", w.Token.TokenIndex);
        }

        private void HandleSet(GrammarAST label, GrammarAST astSuffix, out Template code, GrammarAST s)
        {
            string labelText = null;
            if (label != null)
                labelText = label.Text;

            code = GetTokenElementST("matchSet", "set", s, astSuffix, labelText);
            int i = s.Token.TokenIndex;
            code.SetAttribute("elementIndex", i);
            if (grammar.type != GrammarType.Lexer)
                generator.GenerateLocalFollow(s, "set", currentRuleName, i);

            code.SetAttribute("s", generator.GenSetExpr(templates, s.SetValue, 1, false));
        }

        private void HandleRewriteInit(out Template code, GrammarAST start)
        {
            code = null;

            if (state.backtracking == 0)
            {
                if (start.Type == REWRITES)
                {
                    if (generator.Grammar.BuildTemplate)
                    {
                        code = templates.GetInstanceOf("rewriteTemplate");
                    }
                    else
                    {
                        code = templates.GetInstanceOf("rewriteCode");
                        code.SetAttribute("treeLevel", OUTER_REWRITE_NESTING_LEVEL);
                        code.SetAttribute("rewriteBlockLevel", OUTER_REWRITE_NESTING_LEVEL);
                        code.SetAttribute("referencedElementsDeep", GetTokenTypesAsTargetLabels(start.rewriteRefsDeep));
                        HashSet<string> tokenLabels = grammar.GetLabels(start.rewriteRefsDeep, LabelType.Token);
                        HashSet<string> tokenListLabels = grammar.GetLabels(start.rewriteRefsDeep, LabelType.TokenList);
                        HashSet<string> ruleLabels = grammar.GetLabels(start.rewriteRefsDeep, LabelType.Rule);
                        HashSet<string> ruleListLabels = grammar.GetLabels(start.rewriteRefsDeep, LabelType.RuleList);
                        HashSet<string> wildcardLabels = grammar.GetLabels(start.rewriteRefsDeep, LabelType.WildcardTree);
                        HashSet<string> wildcardListLabels = grammar.GetLabels(start.rewriteRefsDeep, LabelType.WildcardTreeList);
                        // just in case they ref $r for "previous value", make a stream
                        // from retval.tree
                        Template retvalST = templates.GetInstanceOf("prevRuleRootRef");
                        ruleLabels.Add(retvalST.Render());
                        code.SetAttribute("referencedTokenLabels", tokenLabels);
                        code.SetAttribute("referencedTokenListLabels", tokenListLabels);
                        code.SetAttribute("referencedRuleLabels", ruleLabels);
                        code.SetAttribute("referencedRuleListLabels", ruleListLabels);
                        code.SetAttribute("referencedWildcardLabels", wildcardLabels);
                        code.SetAttribute("referencedWildcardListLabels", wildcardListLabels);
                    }
                }
                else
                {
                    code = templates.GetInstanceOf("noRewrite");
                    code.SetAttribute("treeLevel", OUTER_REWRITE_NESTING_LEVEL);
                    code.SetAttribute("rewriteBlockLevel", OUTER_REWRITE_NESTING_LEVEL);
                }
            }
        }

        private void HandleRewriteBeforeRewrite()
        {
            rewriteRuleRefs = new HashSet<object>();
        }

        private void HandleRewriteAfterRewrite(Template code, GrammarAST r, ref GrammarAST pred, Template alt)
        {
            rewriteBlockNestingLevel = OUTER_REWRITE_NESTING_LEVEL;
            IList<object> predChunks = null;
            if (pred != null)
                predChunks = generator.TranslateAction(currentRuleName, pred);

            string description = grammar.GrammarTreeToString(r, false);
            description = generator.Target.GetTargetStringLiteralFromString(description);
            code.SetAttribute("alts.{pred,alt,description}", predChunks, alt, description);
            pred = null;
        }

        private void HandleRewriteEbnf(out Template code, GrammarAST start, Template rewriteBlockCode)
        {
            code = rewriteBlockCode;
            string description = grammar.GrammarTreeToString(start, false);
            description = generator.Target.GetTargetStringLiteralFromString(description);
            code.SetAttribute("description", description);
        }

        private void HandleRewriteAtomRuleReference(bool isRoot, out Template code, GrammarAST r)
        {
            string ruleRefName = r.Text;
            string stName = "rewriteRuleRef";
            if (isRoot)
                stName += "Root";

            code = templates.GetInstanceOf(stName);
            code.SetAttribute("rule", ruleRefName);
            if (grammar.GetRule(ruleRefName) == null)
            {
                ErrorManager.GrammarError(ErrorManager.MSG_UNDEFINED_RULE_REF, grammar, r.Token, ruleRefName);
                // blank; no code gen
                code = new Template(string.Empty);
            }
            else if (grammar.GetRule(currentRuleName).GetRuleRefsInAlt(ruleRefName, outerAltNum) == null)
            {
                ErrorManager.GrammarError(ErrorManager.MSG_REWRITE_ELEMENT_NOT_PRESENT_ON_LHS, grammar, r.Token, ruleRefName);
                // blank; no code gen
                code = new Template(string.Empty);
            }
            else
            {
                // track all rule refs as we must copy 2nd ref to rule and beyond
                if (!rewriteRuleRefs.Contains(ruleRefName))
                    rewriteRuleRefs.Add(ruleRefName);
            }
        }

        private void HandleRewriteAtomTokenReference(bool isRoot, out Template code, GrammarAST start, GrammarAST term, GrammarAST arg)
        {
            string tokenName = start.Token.Text;
            string stName = "rewriteTokenRef";
            Rule rule = grammar.GetRule(currentRuleName);
            ICollection<string> tokenRefsInAlt = rule.GetTokenRefsInAlt(outerAltNum);
            bool createNewNode = !tokenRefsInAlt.Contains(tokenName) || arg != null;
            if (createNewNode)
                stName = "rewriteImaginaryTokenRef";

            if (isRoot)
                stName += "Root";

            code = templates.GetInstanceOf(stName);
            code.SetAttribute("terminalOptions", term.terminalOptions);
            if (arg != null)
            {
                IList<object> args = generator.TranslateAction(currentRuleName, arg);
                code.SetAttribute("args", args);
            }

            code.SetAttribute("elementIndex", start.Token.TokenIndex);
            int ttype = grammar.GetTokenType(tokenName);
            string tok = generator.GetTokenTypeAsTargetLabel(ttype);
            code.SetAttribute("token", tok);
            if (grammar.GetTokenType(tokenName) == Label.INVALID)
            {
                ErrorManager.GrammarError(ErrorManager.MSG_UNDEFINED_TOKEN_REF_IN_REWRITE, grammar, start.Token, tokenName);
                // blank; no code gen
                code = new Template(string.Empty);
            }
        }

        private void HandleRewriteAtomLabel(bool isRoot, out Template code, GrammarAST label)
        {
            string labelName = label.Text;
            Rule rule = grammar.GetRule(currentRuleName);
            Grammar.LabelElementPair pair = rule.GetLabel(labelName);
            if (labelName.Equals(currentRuleName))
            {
                // special case; ref to old value via $ rule
                if (rule.HasRewrite(outerAltNum) && rule.GetRuleRefsInAlt(outerAltNum).Contains(labelName))
                    ErrorManager.GrammarError(ErrorManager.MSG_RULE_REF_AMBIG_WITH_RULE_IN_ALT, grammar, label.Token, labelName);

                Template labelST = templates.GetInstanceOf("prevRuleRootRef");
                code = templates.GetInstanceOf("rewriteRuleLabelRef" + (isRoot ? "Root" : ""));
                code.SetAttribute("label", labelST);
            }
            else if (pair == null)
            {
                ErrorManager.GrammarError(ErrorManager.MSG_UNDEFINED_LABEL_REF_IN_REWRITE, grammar, label.Token, labelName);
                code = new Template(string.Empty);
            }
            else
            {
                string stName = null;
                switch (pair.type)
                {
                case LabelType.Token:
                    stName = "rewriteTokenLabelRef";
                    break;

                case LabelType.WildcardTree:
                    stName = "rewriteWildcardLabelRef";
                    break;

                case LabelType.WildcardTreeList:
                    stName = "rewriteRuleListLabelRef"; // acts like rule ref list for ref
                    break;

                case LabelType.Rule:
                    stName = "rewriteRuleLabelRef";
                    break;

                case LabelType.TokenList:
                    stName = "rewriteTokenListLabelRef";
                    break;

                case LabelType.RuleList:
                    stName = "rewriteRuleListLabelRef";
                    break;
                }

                if (isRoot)
                    stName += "Root";

                code = templates.GetInstanceOf(stName);
                code.SetAttribute("label", labelName);
            }
        }

        private void HandleRewriteAtomAction(bool isRoot, out Template code, GrammarAST action)
        {
            // actions in rewrite rules yield a tree object
            string actText = action.Text;
            IList<object> chunks = generator.TranslateAction(currentRuleName, action);
            code = templates.GetInstanceOf("rewriteNodeAction" + (isRoot ? "Root" : ""));
            code.SetAttribute("action", chunks);
        }

        private void HandleRewriteTemplateAlt(out Template code)
        {
            code = templates.GetInstanceOf("rewriteEmptyTemplate");
        }

        private void HandleRewriteTemplateAfterId(out Template code, GrammarAST id, GrammarAST ind)
        {
            if (id != null && id.Text.Equals("template"))
            {
                code = templates.GetInstanceOf("rewriteInlineTemplate");
            }
            else if (id != null)
            {
                code = templates.GetInstanceOf("rewriteExternalTemplate");
                code.SetAttribute("name", id.Text);
            }
            else if (ind != null)
            {
                // must be \%({expr})(args)
                code = templates.GetInstanceOf("rewriteIndirectTemplate");
                IList<object> chunks = generator.TranslateAction(currentRuleName, ind);
                code.SetAttribute("expr", chunks);
            }
            else
            {
                throw new ArgumentException();
            }
        }

        private void HandleRewriteTemplateArg(Template code, GrammarAST arg, GrammarAST action)
        {
            // must set alt num here rather than in define.g
            // because actions like \%foo(name={\$ID.text}) aren't
            // broken up yet into trees.
            action.outerAltNum = this.outerAltNum;
            IList<object> chunks = generator.TranslateAction(currentRuleName, action);
            code.SetAttribute("args.{name,value}", arg.Text, chunks);
        }

        private void HandleRewriteTemplateLiteral(Template code, GrammarAST literal, int delimiterWidth)
        {
            string sl = literal.Text;
            // strip quotes
            string t = sl.Substring(delimiterWidth, sl.Length - 2 * delimiterWidth);
            t = generator.Target.GetTargetStringLiteralFromString(t);
            code.SetAttribute("template", t);
        }

        private void HandleRewriteTemplateAction(out Template code, GrammarAST action)
        {
            // set alt num for same reason as ARGLIST above
            action.outerAltNum = this.outerAltNum;
            code = templates.GetInstanceOf("rewriteAction");
            code.SetAttribute("action", generator.TranslateAction(currentRuleName, action));
        }
    }
}
