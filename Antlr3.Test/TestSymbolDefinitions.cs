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

namespace AntlrUnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Antlr.Runtime.JavaExtensions;
    using Antlr3.Tool;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using AntlrTool = Antlr3.AntlrTool;
    using CodeGenerator = Antlr3.Codegen.CodeGenerator;
    using Label = Antlr3.Analysis.Label;
    using StringTemplate = Antlr4.StringTemplate.Template;
    using StringTokenizer = Antlr.Runtime.JavaExtensions.StringTokenizer;

    [TestClass]
    public class TestSymbolDefinitions : BaseTest
    {

        /** Public default constructor used by TestRig */
        public TestSymbolDefinitions()
        {
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestParserSimpleTokens() /*throws Exception*/ {
            Grammar g = new Grammar(
                    "parser grammar t;\n" +
                    "a : A | B;\n" +
                    "b : C ;" );
            string rules = "a, b";
            string tokenNames = "A, B, C";
            checkSymbols( g, rules, tokenNames );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestParserTokensSection() /*throws Exception*/ {
            Grammar g = new Grammar(
                    "parser grammar t;\n" +
                    "tokens {\n" +
                    "  C;\n" +
                    "  D;" +
                    "}\n" +
                    "a : A | B;\n" +
                    "b : C ;" );
            string rules = "a, b";
            string tokenNames = "A, B, C, D";
            checkSymbols( g, rules, tokenNames );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestLexerTokensSection() /*throws Exception*/ {
            Grammar g = new Grammar(
                    "lexer grammar t;\n" +
                    "tokens {\n" +
                    "  C;\n" +
                    "  D;" +
                    "}\n" +
                    "A : 'a';\n" +
                    "C : 'c' ;" );
            string rules = "A, C, Tokens";
            string tokenNames = "A, C, D";
            checkSymbols( g, rules, tokenNames );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestTokensSectionWithAssignmentSection() /*throws Exception*/ {
            Grammar g = new Grammar(
                    "grammar t;\n" +
                    "tokens {\n" +
                    "  C='c';\n" +
                    "  D;" +
                    "}\n" +
                    "a : A | B;\n" +
                    "b : C ;" );
            string rules = "a, b";
            string tokenNames = "A, B, C, D, 'c'";
            checkSymbols( g, rules, tokenNames );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestCombinedGrammarLiterals() /*throws Exception*/ {
            Grammar g = new Grammar(
                    "grammar t;\n" +
                    "a : 'begin' b 'end';\n" +
                    "b : C ';' ;\n" +
                    "ID : 'a' ;\n" +
                    "FOO : 'foo' ;\n" +  // "foo" is not a token name
                    "C : 'c' ;\n" );        // nor is 'c'
            string rules = "a, b";
            string tokenNames = "C, FOO, ID, 'begin', 'end', ';'";
            checkSymbols( g, rules, tokenNames );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestLiteralInParserAndLexer() /*throws Exception*/ {
            // 'x' is token and char in lexer rule
            Grammar g = new Grammar(
                    "grammar t;\n" +
                    "a : 'x' E ; \n" +
                    "E: 'x' '0' ;\n" );        // nor is 'c'
            //String literals = "['x']";
            string[] literals = new string[] { "'x'" };

            var foundLiterals = g.StringLiterals;
            Assert.IsTrue( literals.SequenceEqual(foundLiterals) );

            string implicitLexer =
                "lexer grammar t;" + NewLine +
                "T__5 : 'x' ;" + NewLine +
                "" + NewLine +
                "// $ANTLR src \"<string>\" 3" + NewLine +
                "E: 'x' '0' ;";
            Assert.AreEqual( implicitLexer, g.GetLexerGrammar() );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestCombinedGrammarWithRefToLiteralButNoTokenIDRef() /*throws Exception*/ {
            Grammar g = new Grammar(
                    "grammar t;\n" +
                    "a : 'a' ;\n" +
                    "A : 'a' ;\n" );
            string rules = "a";
            string tokenNames = "A, 'a'";
            checkSymbols( g, rules, tokenNames );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestSetDoesNotMissTokenAliases() /*throws Exception*/ {
            Grammar g = new Grammar(
                    "grammar t;\n" +
                    "a : 'a'|'b' ;\n" +
                    "A : 'a' ;\n" +
                    "B : 'b' ;\n" );
            string rules = "a";
            string tokenNames = "A, 'a', B, 'b'";
            checkSymbols( g, rules, tokenNames );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestSimplePlusEqualLabel() /*throws Exception*/ {
            Grammar g = new Grammar(
                    "parser grammar t;\n" +
                    "a : ids+=ID ( COMMA ids+=ID )* ;\n" );
            string rule = "a";
            string tokenLabels = "ids";
            string ruleLabels = null;
            checkPlusEqualsLabels( g, rule, tokenLabels, ruleLabels );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestMixedPlusEqualLabel() /*throws Exception*/ {
            Grammar g = new Grammar(
                    "grammar t;\n" +
                    "options {output=AST;}\n" +
                    "a : id+=ID ( ',' e+=expr )* ;\n" +
                    "expr : 'e';\n" +
                    "ID : 'a';\n" );
            string rule = "a";
            string tokenLabels = "id";
            string ruleLabels = "e";
            checkPlusEqualsLabels( g, rule, tokenLabels, ruleLabels );
        }

        // T E S T  L I T E R A L  E S C A P E S

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestParserCharLiteralWithEscape() /*throws Exception*/ {
            Grammar g = new Grammar(
                    "grammar t;\n" +
                    "a : '\\n';\n" );
            var literals = g.StringLiterals;
            // must store literals how they appear in the antlr grammar
            Assert.AreEqual( "'\\n'", literals.ToArray()[0] );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestTokenInTokensSectionAndTokenRuleDef() /*throws Exception*/ {
            // this must return A not I to the parser; calling a nonfragment rule
            // from a nonfragment rule does not set the overall token.
            string grammar =
                "grammar P;\n" +
                "tokens { B='}'; }\n" +
                "a : A B {System.out.println(input);} ;\n" +
                "A : 'a' ;\n" +
                "B : '}' ;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;";
            string found = execParser( "P.g", grammar, "PParser", "PLexer",
                                      "a", "a}", false );
            Assert.AreEqual( "a}" + NewLine, found );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestTokenInTokensSectionAndTokenRuleDef2() /*throws Exception*/ {
            // this must return A not I to the parser; calling a nonfragment rule
            // from a nonfragment rule does not set the overall token.
            string grammar =
                "grammar P;\n" +
                "tokens { B='}'; }\n" +
                "a : A '}' {System.out.println(input);} ;\n" +
                "A : 'a' ;\n" +
                "B : '}' {/* */} ;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;";
            string found = execParser( "P.g", grammar, "PParser", "PLexer",
                                      "a", "a}", false );
            Assert.AreEqual( "a}" + NewLine, found );
        }


        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestRefToRuleWithNoReturnValue() /*throws Exception*/ {
            ErrorQueue equeue = new ErrorQueue();
            ErrorManager.SetErrorListener( equeue );

            string grammarStr =
                "grammar P;\n" +
                "a : x=b ;\n" +
                "b : B ;\n" +
                "B : 'b' ;\n";
            Grammar g = new Grammar( grammarStr );

            AntlrTool antlr = newTool();
            CodeGenerator generator = new CodeGenerator( antlr, g, "Java" );
            g.CodeGenerator = generator;
            StringTemplate recogST = generator.GenRecognizer();
            string code = recogST.Render();
            Assert.IsTrue(code.IndexOf("x=b();") < 0, "not expecting label");

            Assert.AreEqual(0, equeue.errors.Count, "unexpected errors: " + equeue);
        }

        // T E S T  E R R O R S

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestParserStringLiterals() /*throws Exception*/ {
            ErrorQueue equeue = new ErrorQueue();
            ErrorManager.SetErrorListener( equeue );
            Grammar g = new Grammar(
                    "parser grammar t;\n" +
                    "a : 'begin' b ;\n" +
                    "b : C ;" );
            object expectedArg = "'begin'";
            int expectedMsgID = ErrorManager.MSG_LITERAL_NOT_ASSOCIATED_WITH_LEXER_RULE;
            GrammarSemanticsMessage expectedMessage =
                new GrammarSemanticsMessage( expectedMsgID, g, null, expectedArg );
            checkGrammarSemanticsError( equeue, expectedMessage );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestParserCharLiterals() /*throws Exception*/ {
            ErrorQueue equeue = new ErrorQueue();
            ErrorManager.SetErrorListener( equeue );
            Grammar g = new Grammar(
                    "parser grammar t;\n" +
                    "a : '(' b ;\n" +
                    "b : C ;" );
            object expectedArg = "'('";
            int expectedMsgID = ErrorManager.MSG_LITERAL_NOT_ASSOCIATED_WITH_LEXER_RULE;
            GrammarSemanticsMessage expectedMessage =
                new GrammarSemanticsMessage( expectedMsgID, g, null, expectedArg );
            checkGrammarSemanticsError( equeue, expectedMessage );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestEmptyNotChar() /*throws Exception*/ {
            ErrorQueue equeue = new ErrorQueue();
            ErrorManager.SetErrorListener( equeue );
            Grammar g = new Grammar(
                    "grammar foo;\n" +
                    "a : (~'x')+ ;\n" );
            g.BuildNFA();
            object expectedArg = "'x'";
            int expectedMsgID = ErrorManager.MSG_EMPTY_COMPLEMENT;
            GrammarSemanticsMessage expectedMessage =
                new GrammarSemanticsMessage( expectedMsgID, g, null, expectedArg );
            checkGrammarSemanticsError( equeue, expectedMessage );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestEmptyNotToken() /*throws Exception*/ {
            ErrorQueue equeue = new ErrorQueue();
            ErrorManager.SetErrorListener( equeue );
            Grammar g = new Grammar(
                    "grammar foo;\n" +
                    "a : (~A)+ ;\n" );
            g.BuildNFA();
            object expectedArg = "A";
            int expectedMsgID = ErrorManager.MSG_EMPTY_COMPLEMENT;
            GrammarSemanticsMessage expectedMessage =
                new GrammarSemanticsMessage( expectedMsgID, g, null, expectedArg );
            checkGrammarSemanticsError( equeue, expectedMessage );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestEmptyNotSet() /*throws Exception*/ {
            ErrorQueue equeue = new ErrorQueue();
            ErrorManager.SetErrorListener( equeue );
            Grammar g = new Grammar(
                    "grammar foo;\n" +
                    "a : (~(A|B))+ ;\n" );
            g.BuildNFA();
            object expectedArg = null;
            int expectedMsgID = ErrorManager.MSG_EMPTY_COMPLEMENT;
            GrammarSemanticsMessage expectedMessage =
                new GrammarSemanticsMessage( expectedMsgID, g, null, expectedArg );
            checkGrammarSemanticsError( equeue, expectedMessage );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestStringLiteralInParserTokensSection() /*throws Exception*/ {
            ErrorQueue equeue = new ErrorQueue();
            ErrorManager.SetErrorListener( equeue ); // unique listener per thread
            Grammar g = new Grammar(
                    "parser grammar t;\n" +
                    "tokens {\n" +
                    "  B='begin';\n" +
                    "}\n" +
                    "a : A B;\n" +
                    "b : C ;" );
            object expectedArg = "'begin'";
            int expectedMsgID = ErrorManager.MSG_LITERAL_NOT_ASSOCIATED_WITH_LEXER_RULE;
            GrammarSemanticsMessage expectedMessage =
                new GrammarSemanticsMessage( expectedMsgID, g, null, expectedArg );
            checkGrammarSemanticsError( equeue, expectedMessage );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestCharLiteralInParserTokensSection() /*throws Exception*/ {
            ErrorQueue equeue = new ErrorQueue();
            ErrorManager.SetErrorListener( equeue ); // unique listener per thread
            Grammar g = new Grammar(
                    "parser grammar t;\n" +
                    "tokens {\n" +
                    "  B='(';\n" +
                    "}\n" +
                    "a : A B;\n" +
                    "b : C ;" );
            object expectedArg = "'('";
            int expectedMsgID = ErrorManager.MSG_LITERAL_NOT_ASSOCIATED_WITH_LEXER_RULE;
            GrammarSemanticsMessage expectedMessage =
                new GrammarSemanticsMessage( expectedMsgID, g, null, expectedArg );
            checkGrammarSemanticsError( equeue, expectedMessage );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestCharLiteralInLexerTokensSection() /*throws Exception*/ {
            ErrorQueue equeue = new ErrorQueue();
            ErrorManager.SetErrorListener( equeue ); // unique listener per thread
            Grammar g = new Grammar(
                    "lexer grammar t;\n" +
                    "tokens {\n" +
                    "  B='(';\n" +
                    "}\n" +
                    "ID : 'a';\n" );
            object expectedArg = "'('";
            int expectedMsgID = ErrorManager.MSG_CANNOT_ALIAS_TOKENS_IN_LEXER;
            GrammarSemanticsMessage expectedMessage =
                new GrammarSemanticsMessage( expectedMsgID, g, null, expectedArg );
            checkGrammarSemanticsError( equeue, expectedMessage );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestRuleRedefinition() /*throws Exception*/ {
            ErrorQueue equeue = new ErrorQueue();
            ErrorManager.SetErrorListener( equeue ); // unique listener per thread
            Grammar g = new Grammar(
                    "parser grammar t;\n" +
                    "a : A | B;\n" +
                    "a : C ;" );

            object expectedArg = "a";
            int expectedMsgID = ErrorManager.MSG_RULE_REDEFINITION;
            GrammarSemanticsMessage expectedMessage =
                new GrammarSemanticsMessage( expectedMsgID, g, null, expectedArg );
            checkGrammarSemanticsError( equeue, expectedMessage );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestLexerRuleRedefinition() /*throws Exception*/ {
            ErrorQueue equeue = new ErrorQueue();
            ErrorManager.SetErrorListener( equeue ); // unique listener per thread
            Grammar g = new Grammar(
                    "lexer grammar t;\n" +
                    "ID : 'a' ;\n" +
                    "ID : 'd' ;" );

            object expectedArg = "ID";
            int expectedMsgID = ErrorManager.MSG_RULE_REDEFINITION;
            GrammarSemanticsMessage expectedMessage =
                new GrammarSemanticsMessage( expectedMsgID, g, null, expectedArg );
            checkGrammarSemanticsError( equeue, expectedMessage );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestCombinedRuleRedefinition() /*throws Exception*/ {
            ErrorQueue equeue = new ErrorQueue();
            ErrorManager.SetErrorListener( equeue ); // unique listener per thread
            Grammar g = new Grammar(
                    "grammar t;\n" +
                    "x : ID ;\n" +
                    "ID : 'a' ;\n" +
                    "x : ID ID ;" );

            object expectedArg = "x";
            int expectedMsgID = ErrorManager.MSG_RULE_REDEFINITION;
            GrammarSemanticsMessage expectedMessage =
                new GrammarSemanticsMessage( expectedMsgID, g, null, expectedArg );
            checkGrammarSemanticsError( equeue, expectedMessage );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestUndefinedToken() /*throws Exception*/ {
            ErrorQueue equeue = new ErrorQueue();
            ErrorManager.SetErrorListener( equeue ); // unique listener per thread
            Grammar g = new Grammar(
                    "grammar t;\n" +
                    "x : ID ;" );

            object expectedArg = "ID";
            int expectedMsgID = ErrorManager.MSG_NO_TOKEN_DEFINITION;
            GrammarSemanticsMessage expectedMessage =
                new GrammarSemanticsMessage( expectedMsgID, g, null, expectedArg );
            checkGrammarSemanticsWarning( equeue, expectedMessage );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestUndefinedTokenOkInParser() /*throws Exception*/ {
            ErrorQueue equeue = new ErrorQueue();
            ErrorManager.SetErrorListener( equeue ); // unique listener per thread
            Grammar g = new Grammar(
                    "parser grammar t;\n" +
                    "x : ID ;" );
            Assert.AreEqual(0, equeue.errors.Count, "should not be an error");
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestUndefinedRule() /*throws Exception*/ {
            ErrorQueue equeue = new ErrorQueue();
            ErrorManager.SetErrorListener( equeue ); // unique listener per thread
            Grammar g = new Grammar(
                    "grammar t;\n" +
                    "x : r ;" );

            object expectedArg = "r";
            int expectedMsgID = ErrorManager.MSG_UNDEFINED_RULE_REF;
            GrammarSemanticsMessage expectedMessage =
                new GrammarSemanticsMessage( expectedMsgID, g, null, expectedArg );
            checkGrammarSemanticsError( equeue, expectedMessage );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestLexerRuleInParser() /*throws Exception*/ {
            ErrorQueue equeue = new ErrorQueue();
            ErrorManager.SetErrorListener( equeue ); // unique listener per thread
            Grammar g = new Grammar(
                    "parser grammar t;\n" +
                    "X : ;" );

            object expectedArg = "X";
            int expectedMsgID = ErrorManager.MSG_LEXER_RULES_NOT_ALLOWED;
            GrammarSemanticsMessage expectedMessage =
                new GrammarSemanticsMessage( expectedMsgID, g, null, expectedArg );
            checkGrammarSemanticsError( equeue, expectedMessage );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestParserRuleInLexer() /*throws Exception*/ {
            ErrorQueue equeue = new ErrorQueue();
            ErrorManager.SetErrorListener( equeue ); // unique listener per thread
            Grammar g = new Grammar(
                    "lexer grammar t;\n" +
                    "a : ;" );

            object expectedArg = "a";
            int expectedMsgID = ErrorManager.MSG_PARSER_RULES_NOT_ALLOWED;
            GrammarSemanticsMessage expectedMessage =
                new GrammarSemanticsMessage( expectedMsgID, g, null, expectedArg );
            checkGrammarSemanticsError( equeue, expectedMessage );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestRuleScopeConflict() /*throws Exception*/ {
            ErrorQueue equeue = new ErrorQueue();
            ErrorManager.SetErrorListener( equeue ); // unique listener per thread
            Grammar g = new Grammar(
                "grammar t;\n" +
                "scope a {\n" +
                "  int n;\n" +
                "}\n" +
                "a : \n" +
                "  ;\n" );

            object expectedArg = "a";
            int expectedMsgID = ErrorManager.MSG_SYMBOL_CONFLICTS_WITH_GLOBAL_SCOPE;
            GrammarSemanticsMessage expectedMessage =
                new GrammarSemanticsMessage( expectedMsgID, g, null, expectedArg );
            checkGrammarSemanticsError( equeue, expectedMessage );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestTokenRuleScopeConflict() /*throws Exception*/ {
            ErrorQueue equeue = new ErrorQueue();
            ErrorManager.SetErrorListener( equeue ); // unique listener per thread
            Grammar g = new Grammar(
                "grammar t;\n" +
                "scope ID {\n" +
                "  int n;\n" +
                "}\n" +
                "ID : 'a'\n" +
                "  ;\n" );

            object expectedArg = "ID";
            int expectedMsgID = ErrorManager.MSG_SYMBOL_CONFLICTS_WITH_GLOBAL_SCOPE;
            GrammarSemanticsMessage expectedMessage =
                new GrammarSemanticsMessage( expectedMsgID, g, null, expectedArg );
            checkGrammarSemanticsError( equeue, expectedMessage );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestTokenScopeConflict() /*throws Exception*/ {
            ErrorQueue equeue = new ErrorQueue();
            ErrorManager.SetErrorListener( equeue ); // unique listener per thread
            Grammar g = new Grammar(
                "grammar t;\n" +
                "tokens { ID; }\n" +
                "scope ID {\n" +
                "  int n;\n" +
                "}\n" +
                "a : \n" +
                "  ;\n" );

            object expectedArg = "ID";
            int expectedMsgID = ErrorManager.MSG_SYMBOL_CONFLICTS_WITH_GLOBAL_SCOPE;
            GrammarSemanticsMessage expectedMessage =
                new GrammarSemanticsMessage( expectedMsgID, g, null, expectedArg );
            checkGrammarSemanticsError( equeue, expectedMessage );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestTokenRuleScopeConflictInLexerGrammar() /*throws Exception*/ {
            ErrorQueue equeue = new ErrorQueue();
            ErrorManager.SetErrorListener( equeue ); // unique listener per thread
            Grammar g = new Grammar(
                "lexer grammar t;\n" +
                "scope ID {\n" +
                "  int n;\n" +
                "}\n" +
                "ID : 'a'\n" +
                "  ;\n" );

            object expectedArg = "ID";
            int expectedMsgID = ErrorManager.MSG_SYMBOL_CONFLICTS_WITH_GLOBAL_SCOPE;
            GrammarSemanticsMessage expectedMessage =
                new GrammarSemanticsMessage( expectedMsgID, g, null, expectedArg );
            checkGrammarSemanticsError( equeue, expectedMessage );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestTokenLabelScopeConflict() /*throws Exception*/ {
            ErrorQueue equeue = new ErrorQueue();
            ErrorManager.SetErrorListener( equeue ); // unique listener per thread
            Grammar g = new Grammar(
                "parser grammar t;\n" +
                "scope s {\n" +
                "  int n;\n" +
                "}\n" +
                "a : s=ID \n" +
                "  ;\n" );

            object expectedArg = "s";
            int expectedMsgID = ErrorManager.MSG_SYMBOL_CONFLICTS_WITH_GLOBAL_SCOPE;
            GrammarSemanticsMessage expectedMessage =
                new GrammarSemanticsMessage( expectedMsgID, g, null, expectedArg );
            checkGrammarSemanticsError( equeue, expectedMessage );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestRuleLabelScopeConflict() /*throws Exception*/ {
            ErrorQueue equeue = new ErrorQueue();
            ErrorManager.SetErrorListener( equeue ); // unique listener per thread
            Grammar g = new Grammar(
                "parser grammar t;\n" +
                "scope s {\n" +
                "  int n;\n" +
                "}\n" +
                "a : s=b \n" +
                "  ;\n" +
                "b : ;\n" );

            object expectedArg = "s";
            int expectedMsgID = ErrorManager.MSG_SYMBOL_CONFLICTS_WITH_GLOBAL_SCOPE;
            GrammarSemanticsMessage expectedMessage =
                new GrammarSemanticsMessage( expectedMsgID, g, null, expectedArg );
            checkGrammarSemanticsError( equeue, expectedMessage );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestLabelAndRuleNameConflict() /*throws Exception*/ {
            ErrorQueue equeue = new ErrorQueue();
            ErrorManager.SetErrorListener( equeue ); // unique listener per thread
            Grammar g = new Grammar(
                "parser grammar t;\n" +
                "a : c=b \n" +
                "  ;\n" +
                "b : ;\n" +
                "c : ;\n" );

            object expectedArg = "c";
            int expectedMsgID = ErrorManager.MSG_LABEL_CONFLICTS_WITH_RULE;
            GrammarSemanticsMessage expectedMessage =
                new GrammarSemanticsMessage( expectedMsgID, g, null, expectedArg );
            checkGrammarSemanticsError( equeue, expectedMessage );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestLabelAndTokenNameConflict() /*throws Exception*/ {
            ErrorQueue equeue = new ErrorQueue();
            ErrorManager.SetErrorListener( equeue ); // unique listener per thread
            Grammar g = new Grammar(
                "parser grammar t;\n" +
                "a : ID=b \n" +
                "  ;\n" +
                "b : ID ;\n" +
                "c : ;\n" );

            object expectedArg = "ID";
            int expectedMsgID = ErrorManager.MSG_LABEL_CONFLICTS_WITH_TOKEN;
            GrammarSemanticsMessage expectedMessage =
                new GrammarSemanticsMessage( expectedMsgID, g, null, expectedArg );
            checkGrammarSemanticsError( equeue, expectedMessage );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestLabelAndArgConflict() /*throws Exception*/ {
            ErrorQueue equeue = new ErrorQueue();
            ErrorManager.SetErrorListener( equeue ); // unique listener per thread
            Grammar g = new Grammar(
                "parser grammar t;\n" +
                "a[int i] returns [int x]: i=ID \n" +
                "  ;\n" );

            object expectedArg = "i";
            int expectedMsgID = ErrorManager.MSG_LABEL_CONFLICTS_WITH_RULE_ARG_RETVAL;
            GrammarSemanticsMessage expectedMessage =
                new GrammarSemanticsMessage( expectedMsgID, g, null, expectedArg );
            checkGrammarSemanticsError( equeue, expectedMessage );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestLabelAndParameterConflict() /*throws Exception*/ {
            ErrorQueue equeue = new ErrorQueue();
            ErrorManager.SetErrorListener( equeue ); // unique listener per thread
            Grammar g = new Grammar(
                "parser grammar t;\n" +
                "a[int i] returns [int x]: x=ID \n" +
                "  ;\n" );

            object expectedArg = "x";
            int expectedMsgID = ErrorManager.MSG_LABEL_CONFLICTS_WITH_RULE_ARG_RETVAL;
            GrammarSemanticsMessage expectedMessage =
                new GrammarSemanticsMessage( expectedMsgID, g, null, expectedArg );
            checkGrammarSemanticsError( equeue, expectedMessage );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestLabelRuleScopeConflict() /*throws Exception*/ {
            ErrorQueue equeue = new ErrorQueue();
            ErrorManager.SetErrorListener( equeue ); // unique listener per thread
            Grammar g = new Grammar(
                "parser grammar t;\n" +
                "a\n" +
                "scope {" +
                "  int n;" +
                "}\n" +
                "  : n=ID\n" +
                "  ;\n" );

            object expectedArg = "n";
            object expectedArg2 = "a";
            int expectedMsgID = ErrorManager.MSG_LABEL_CONFLICTS_WITH_RULE_SCOPE_ATTRIBUTE;
            GrammarSemanticsMessage expectedMessage =
                new GrammarSemanticsMessage( expectedMsgID, g, null, expectedArg, expectedArg2 );
            checkGrammarSemanticsError( equeue, expectedMessage );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestRuleScopeArgConflict() /*throws Exception*/ {
            ErrorQueue equeue = new ErrorQueue();
            ErrorManager.SetErrorListener( equeue ); // unique listener per thread
            Grammar g = new Grammar(
                "parser grammar t;\n" +
                "a[int n]\n" +
                "scope {" +
                "  int n;" +
                "}\n" +
                "  : \n" +
                "  ;\n" );

            object expectedArg = "n";
            object expectedArg2 = "a";
            int expectedMsgID = ErrorManager.MSG_ATTRIBUTE_CONFLICTS_WITH_RULE_ARG_RETVAL;
            GrammarSemanticsMessage expectedMessage =
                new GrammarSemanticsMessage( expectedMsgID, g, null, expectedArg, expectedArg2 );
            checkGrammarSemanticsError( equeue, expectedMessage );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestRuleScopeReturnValueConflict() /*throws Exception*/ {
            ErrorQueue equeue = new ErrorQueue();
            ErrorManager.SetErrorListener( equeue ); // unique listener per thread
            Grammar g = new Grammar(
                "parser grammar t;\n" +
                "a returns [int n]\n" +
                "scope {" +
                "  int n;" +
                "}\n" +
                "  : \n" +
                "  ;\n" );

            object expectedArg = "n";
            object expectedArg2 = "a";
            int expectedMsgID = ErrorManager.MSG_ATTRIBUTE_CONFLICTS_WITH_RULE_ARG_RETVAL;
            GrammarSemanticsMessage expectedMessage =
                new GrammarSemanticsMessage( expectedMsgID, g, null, expectedArg, expectedArg2 );
            checkGrammarSemanticsError( equeue, expectedMessage );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestRuleScopeRuleNameConflict() /*throws Exception*/ {
            ErrorQueue equeue = new ErrorQueue();
            ErrorManager.SetErrorListener( equeue ); // unique listener per thread
            Grammar g = new Grammar(
                "parser grammar t;\n" +
                "a\n" +
                "scope {" +
                "  int a;" +
                "}\n" +
                "  : \n" +
                "  ;\n" );

            object expectedArg = "a";
            object expectedArg2 = null;
            int expectedMsgID = ErrorManager.MSG_ATTRIBUTE_CONFLICTS_WITH_RULE;
            GrammarSemanticsMessage expectedMessage =
                new GrammarSemanticsMessage( expectedMsgID, g, null, expectedArg, expectedArg2 );
            checkGrammarSemanticsError( equeue, expectedMessage );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestBadGrammarOption() /*throws Exception*/ {
            ErrorQueue equeue = new ErrorQueue();
            ErrorManager.SetErrorListener( equeue ); // unique listener per thread
            AntlrTool antlr = newTool();
            Grammar g = new Grammar( antlr,
                                    "grammar t;\n" +
                                    "options {foo=3; language=Java;}\n" +
                                    "a : 'a';\n" );

            object expectedArg = "foo";
            int expectedMsgID = ErrorManager.MSG_ILLEGAL_OPTION;
            GrammarSemanticsMessage expectedMessage =
                new GrammarSemanticsMessage( expectedMsgID, g, null, expectedArg );
            checkGrammarSemanticsError( equeue, expectedMessage );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestBadRuleOption() /*throws Exception*/ {
            ErrorQueue equeue = new ErrorQueue();
            ErrorManager.SetErrorListener( equeue ); // unique listener per thread
            Grammar g = new Grammar(
                    "grammar t;\n" +
                    "a\n" +
                    "options {k=3; tokenVocab=blort;}\n" +
                    "  : 'a';\n" );

            object expectedArg = "tokenVocab";
            int expectedMsgID = ErrorManager.MSG_ILLEGAL_OPTION;
            GrammarSemanticsMessage expectedMessage =
                new GrammarSemanticsMessage( expectedMsgID, g, null, expectedArg );
            checkGrammarSemanticsError( equeue, expectedMessage );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestBadSubRuleOption() /*throws Exception*/ {
            ErrorQueue equeue = new ErrorQueue();
            ErrorManager.SetErrorListener( equeue ); // unique listener per thread
            Grammar g = new Grammar(
                    "grammar t;\n" +
                    "a : ( options {k=3; language=Java;}\n" +
                    "    : 'a'\n" +
                    "    | 'b'\n" +
                    "    )\n" +
                    "  ;\n" );
            object expectedArg = "language";
            int expectedMsgID = ErrorManager.MSG_ILLEGAL_OPTION;
            GrammarSemanticsMessage expectedMessage =
                new GrammarSemanticsMessage( expectedMsgID, g, null, expectedArg );
            checkGrammarSemanticsError( equeue, expectedMessage );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestTokenVocabStringUsedInLexer() /*throws Exception*/ {
            ErrorQueue equeue = new ErrorQueue();
            ErrorManager.SetErrorListener( equeue );
            string tokens =
                "';'=4\n";
            mkdir( tmpdir );
            writeFile( tmpdir, "T.tokens", tokens );

            string importer =
                "lexer grammar B; \n" +
                "options\t{tokenVocab=T;} \n" +
                "SEMI:';' ; \n";
            writeFile( tmpdir, "B.g", importer );
            AntlrTool antlr = newTool( new string[] { "-lib", tmpdir } );
            CompositeGrammar composite = new CompositeGrammar();
            Grammar g = new Grammar( antlr, tmpdir + "/B.g", composite );
            g.ParseAndBuildAST();
            g.composite.AssignTokenTypes();

            string expectedTokenIDToTypeMap = "[SEMI=4]";
            string expectedStringLiteralToTypeMap = "{';'=4}";
            string expectedTypeToTokenList = "[SEMI]";

            Assert.AreEqual( expectedTokenIDToTypeMap,
                         realElements( g.composite.TokenIDToTypeMap ).ToElementString() );
            Assert.AreEqual( expectedStringLiteralToTypeMap, g.composite.StringLiteralToTypeMap.ToElementString() );
            Assert.AreEqual( expectedTypeToTokenList,
                         realElements( g.composite.TypeToTokenList ).ToElementString() );

            Assert.AreEqual(0, equeue.errors.Count, "unexpected errors: " + equeue);
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestTokenVocabStringUsedInCombined() /*throws Exception*/ {
            ErrorQueue equeue = new ErrorQueue();
            ErrorManager.SetErrorListener( equeue );
            string tokens =
                "';'=4\n";
            mkdir( tmpdir );
            writeFile( tmpdir, "T.tokens", tokens );

            string importer =
                "grammar B; \n" +
                "options\t{tokenVocab=T;} \n" +
                "SEMI:';' ; \n";
            writeFile( tmpdir, "B.g", importer );
            AntlrTool antlr = newTool( new string[] { "-lib", tmpdir } );
            CompositeGrammar composite = new CompositeGrammar();
            Grammar g = new Grammar( antlr, tmpdir + "/B.g", composite );
            g.ParseAndBuildAST();
            g.composite.AssignTokenTypes();

            string expectedTokenIDToTypeMap = "[SEMI=4]";
            string expectedStringLiteralToTypeMap = "{';'=4}";
            string expectedTypeToTokenList = "[SEMI]";

            Assert.AreEqual( expectedTokenIDToTypeMap,
                         realElements( g.composite.TokenIDToTypeMap ).ToElementString() );
            Assert.AreEqual( expectedStringLiteralToTypeMap, g.composite.StringLiteralToTypeMap.ToElementString() );
            Assert.AreEqual( expectedTypeToTokenList,
                         realElements( g.composite.TypeToTokenList ).ToElementString() );

            Assert.AreEqual(0, equeue.errors.Count, "unexpected errors: " + equeue);
        }

        protected void checkPlusEqualsLabels( Grammar g,
                                             string ruleName,
                                             string tokenLabelsStr,
                                             string ruleLabelsStr )
        //throws Exception
        {
            // make sure expected += labels are there
            Rule r = g.GetRule( ruleName );
            StringTokenizer st = new StringTokenizer( tokenLabelsStr, ", " );
            ICollection<string> tokenLabels = null;
            while ( st.hasMoreTokens() )
            {
                if ( tokenLabels == null )
                {
                    tokenLabels = new List<string>();
                }
                string labelName = st.nextToken();
                tokenLabels.Add( labelName );
            }
            ICollection<string> ruleLabels = null;
            if ( ruleLabelsStr != null )
            {
                st = new StringTokenizer( ruleLabelsStr, ", " );
                ruleLabels = new List<string>();
                while ( st.hasMoreTokens() )
                {
                    string labelName = st.nextToken();
                    ruleLabels.Add( labelName );
                }
            }
            Assert.IsTrue((tokenLabels != null && r.TokenListLabels != null) ||
                       (tokenLabels == null && r.TokenListLabels == null),
                       "token += labels mismatch; " + tokenLabels + "!=" + r.TokenListLabels);
            Assert.IsTrue((ruleLabels != null && r.RuleListLabels != null) ||
                       (ruleLabels == null && r.RuleListLabels == null),
                       "rule += labels mismatch; " + ruleLabels + "!=" + r.RuleListLabels);
            if ( tokenLabels != null )
            {
                Assert.IsTrue( tokenLabels.SequenceEqual( r.TokenListLabels.Keys ) );
            }
            if ( ruleLabels != null )
            {
                Assert.IsTrue( ruleLabels.SequenceEqual( r.RuleListLabels.Keys ) );
            }
        }

        protected void checkSymbols( Grammar g,
                                    string rulesStr,
                                    string tokensStr )
        //throws Exception
        {
            var tokens = g.GetTokenDisplayNames();

            // make sure expected tokens are there
            //StringTokenizer st = new StringTokenizer( tokensStr, ", " );
            //while ( st.hasMoreTokens() )
            foreach ( string tokenName in tokensStr.Split( new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries ) )
            {
                //String tokenName = st.nextToken();
                Assert.IsTrue(g.GetTokenType(tokenName) != Label.INVALID, "token " + tokenName + " expected");
                tokens.Remove( tokenName );
            }
            // make sure there are not any others (other than <EOF> etc...)
            foreach ( string tokenName in tokens )
            {
                Assert.IsTrue( g.GetTokenType( tokenName ) < Label.MIN_TOKEN_TYPE, "unexpected token name " + tokenName );
            }

            // make sure all expected rules are there
            //st = new StringTokenizer( rulesStr, ", " );
            int n = 0;
            //while ( st.hasMoreTokens() )
            foreach ( string ruleName in rulesStr.Split( new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries ) )
            {
                //String ruleName = st.nextToken();
                Assert.IsNotNull(g.GetRule(ruleName), "rule " + ruleName + " expected");
                n++;
            }
            var rules = g.Rules;
            //System.out.println("rules="+rules);
            // make sure there are no extra rules
            Assert.AreEqual(n, rules.Count, "number of rules mismatch; expecting " + n + "; found " + rules.Count);

        }

    }
}
