/*
 * [The "BSD licence"]
 * Copyright (c) 2005-2008 Terence Parr
 * All rights reserved.
 *
 * Grammar conversion to ANTLR v3 and C#:
 * Copyright (c) 2008 Sam Harwell, Pixel Mine, Inc.
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
    using Antlr.Runtime.JavaExtensions;
    using Antlr3.Codegen;

    using CommonToken = Antlr.Runtime.CommonToken;
    using Console = System.Console;
    using ErrorManager = Antlr3.Tool.ErrorManager;
    using Grammar = Antlr3.Tool.Grammar;
    using GrammarAST = Antlr3.Tool.GrammarAST;
    using GrammarType = Antlr3.Tool.GrammarType;
    using IToken = Antlr.Runtime.IToken;
    using LabelType = Antlr3.Tool.LabelType;
    using MismatchedTokenException = Antlr.Runtime.MismatchedTokenException;
    using NoViableAltException = Antlr.Runtime.NoViableAltException;
    using RecognitionException = Antlr.Runtime.RecognitionException;
    using Rule = Antlr3.Tool.Rule;
    using RuleType = Antlr3.Tool.RuleType;
    using StringTemplate = Antlr3.ST.StringTemplate;
    using StringTemplateGroup = Antlr3.ST.StringTemplateGroup;

    partial class CodeGenTreeWalker
    {
        protected const int RULE_BLOCK_NESTING_LEVEL = 0;
        protected const int OUTER_REWRITE_NESTING_LEVEL = 0;

        protected internal string currentRuleName = null;
        protected int blockNestingLevel = 0;
        protected int rewriteBlockNestingLevel = 0;
        protected internal int outerAltNum = 0;
        protected StringTemplate currentBlockST = null;
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
        protected StringTemplateGroup templates;

        /** The overall lexer/parser template; simulate dynamically scoped
         *  attributes by making this an instance var of the walker.
         */
        protected StringTemplate recognizerST;

        protected StringTemplate outputFileST;
        protected StringTemplate headerFileST;

        protected string outputOption = "";

        protected StringTemplate GetWildcardST( GrammarAST elementAST, GrammarAST ast_suffix, string label )
        {
            string name = "wildcard";
            if ( grammar.type == GrammarType.Lexer )
            {
                name = "wildcardChar";
            }
            return GetTokenElementST( name, name, elementAST, ast_suffix, label );
        }

        protected StringTemplate GetRuleElementST( string name,
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
                 ( r == null || !r.isSynPred ) )
            {
                // we will need a label to do the AST or tracking, make one
                label = generator.CreateUniqueLabel( ruleTargetName );
                CommonToken labelTok = new CommonToken( ANTLRParser.ID, label );
                grammar.DefineRuleRefLabel( currentRuleName, labelTok, elementAST );
            }
            StringTemplate elementST = templates.GetInstanceOf( name );
            if ( label != null )
            {
                elementST.SetAttribute( "label", label );
            }
            return elementST;
        }

        protected StringTemplate GetTokenElementST( string name,
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
                 ( r == null || !r.isSynPred ) )
            {
                label = generator.CreateUniqueLabel( elementName );
                CommonToken labelTok = new CommonToken( ANTLRParser.ID, label );
                grammar.DefineTokenRefLabel( currentRuleName, labelTok, elementAST );
            }

            StringTemplate elementST = null;
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
            if ( ast_suffix != null && !ruleDescr.isSynPred )
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
    }
}
