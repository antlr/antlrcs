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
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using AntlrTool = Antlr3.AntlrTool;
    using CodeGenerator = Antlr3.Codegen.CodeGenerator;
    using ErrorManager = Antlr3.Tool.ErrorManager;
    using Grammar = Antlr3.Tool.Grammar;
    using GrammarSyntaxMessage = Antlr3.Tool.GrammarSyntaxMessage;
    using RecognitionException = Antlr.Runtime.RecognitionException;

    /** Tree rewrites in tree parsers are basically identical to rewrites
     *  in a normal grammar except that the atomic element is a node not
     *  a Token.  Tests here ensure duplication of nodes occurs properly
     *  and basic functionality.
     */
    [TestClass]
    public class TestTreeGrammarRewriteAST : BaseTest
    {
        protected bool debug = false;

        [TestMethod]
        public void TestFlatList() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ID INT;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";

            string treeGrammar =
                "tree grammar TP;\n" +
                "options {output=AST; ASTLabelType=CommonTree; tokenVocab=T;}\n" +
                "a : ID INT -> INT ID\n" +
                "  ;\n";

            string found = execTreeParser( "T.g", grammar, "TParser", "TP.g",
                                          treeGrammar, "TP", "TLexer", "a", "a", "abc 34" );
            Assert.AreEqual( "34 abc" + NewLine, found );
        }

        [TestMethod]
        public void TestSimpleTree() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ID INT -> ^(ID INT);\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";

            string treeGrammar =
                "tree grammar TP;\n" +
                "options {output=AST; ASTLabelType=CommonTree; tokenVocab=T;}\n" +
                "a : ^(ID INT) -> ^(INT ID)\n" +
                "  ;\n";

            string found = execTreeParser( "T.g", grammar, "TParser", "TP.g",
                                          treeGrammar, "TP", "TLexer", "a", "a", "abc 34" );
            Assert.AreEqual( "(34 abc)" + NewLine, found );
        }

        [TestMethod]
        public void TestNonImaginaryWithCtor() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : INT ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";

            string treeGrammar =
                "tree grammar TP;\n" +
                "options {output=AST; ASTLabelType=CommonTree; tokenVocab=T;}\n" +
                "a : INT -> INT[\"99\"]\n" + // make new INT node
                "  ;\n";

            string found = execTreeParser( "T.g", grammar, "TParser", "TP.g",
                        treeGrammar, "TP", "TLexer", "a", "a", "34" );
            Assert.AreEqual( "99" + NewLine, found );
        }

        [TestMethod]
        public void TestCombinedRewriteAndAuto() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ID INT -> ^(ID INT) | INT ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";

            string treeGrammar =
                "tree grammar TP;\n" +
                "options {output=AST; ASTLabelType=CommonTree; tokenVocab=T;}\n" +
                "a : ^(ID INT) -> ^(INT ID) | INT\n" +
                "  ;\n";

            string found = execTreeParser( "T.g", grammar, "TParser", "TP.g",
                                          treeGrammar, "TP", "TLexer", "a", "a", "abc 34" );
            Assert.AreEqual( "(34 abc)" + NewLine, found );

            found = execTreeParser( "T.g", grammar, "TParser", "TP.g",
                                   treeGrammar, "TP", "TLexer", "a", "a", "34" );
            Assert.AreEqual( "34" + NewLine, found );
        }

        [TestMethod]
        public void TestAvoidDup() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ID ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";

            string treeGrammar =
                "tree grammar TP;\n" +
                "options {output=AST; ASTLabelType=CommonTree; tokenVocab=T;}\n" +
                "a : ID -> ^(ID ID)\n" +
                "  ;\n";

            string found = execTreeParser( "T.g", grammar, "TParser", "TP.g",
                                          treeGrammar, "TP", "TLexer", "a", "a", "abc" );
            Assert.AreEqual( "(abc abc)" + NewLine, found );
        }

        [TestMethod]
        public void TestLoop() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ID+ INT+ -> (^(ID INT))+ ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";

            string treeGrammar =
                "tree grammar TP;\n" +
                "options {output=AST; ASTLabelType=CommonTree; tokenVocab=T;}\n" +
                "a : (^(ID INT))+ -> INT+ ID+\n" +
                "  ;\n";

            string found = execTreeParser( "T.g", grammar, "TParser", "TP.g",
                                          treeGrammar, "TP", "TLexer", "a", "a", "a b c 3 4 5" );
            Assert.AreEqual( "3 4 5 a b c" + NewLine, found );
        }

        [TestMethod]
        public void TestAutoDup() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ID ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";

            string treeGrammar =
                "tree grammar TP;\n" +
                "options {output=AST; ASTLabelType=CommonTree; tokenVocab=T;}\n" +
                "a : ID \n" +
                "  ;\n";

            string found = execTreeParser( "T.g", grammar, "TParser", "TP.g",
                                          treeGrammar, "TP", "TLexer", "a", "a", "abc" );
            Assert.AreEqual( "abc" + NewLine, found );
        }

        [TestMethod]
        public void TestAutoDupRule() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ID INT ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";

            string treeGrammar =
                "tree grammar TP;\n" +
                "options {output=AST; ASTLabelType=CommonTree; tokenVocab=T;}\n" +
                "a : b c ;\n" +
                "b : ID ;\n" +
                "c : INT ;\n";

            string found = execTreeParser( "T.g", grammar, "TParser", "TP.g",
                                          treeGrammar, "TP", "TLexer", "a", "a", "a 1" );
            Assert.AreEqual( "a 1" + NewLine, found );
        }

        [TestMethod]
        public void TestAutoWildcard() /*throws Exception*/
        {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ID INT ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";

            string treeGrammar =
                "tree grammar TP;\n" +
                "options {output=AST; ASTLabelType=CommonTree; tokenVocab=T;}\n" +
                "a : ID . \n" +
                "  ;\n";

            string found = execTreeParser( "T.g", grammar, "TParser", "TP.g",
                                          treeGrammar, "TP", "TLexer", "a", "a", "abc 34" );
            Assert.AreEqual( "abc 34" + NewLine, found );
        }

        [TestMethod]
        public void TestNoWildcardAsRootError() /*throws Exception*/
        {
            ErrorQueue equeue = new ErrorQueue();
            ErrorManager.SetErrorListener( equeue );

            string treeGrammar =
                "tree grammar TP;\n" +
                "options {output=AST;}\n" +
                "a : ^(. INT) \n" +
                "  ;\n";

            Grammar g = new Grammar( treeGrammar );
            AntlrTool antlr = newTool();
            antlr.SetOutputDirectory( null ); // write to /dev/null
            CodeGenerator generator = new CodeGenerator( antlr, g, "Java" );
            g.CodeGenerator = generator;
            generator.GenRecognizer();

            Assert.AreEqual(1, equeue.errors.Count, "unexpected errors: " + equeue);

            int expectedMsgID = ErrorManager.MSG_WILDCARD_AS_ROOT;
            object expectedArg = null;
            RecognitionException expectedExc = null;
            GrammarSyntaxMessage expectedMessage =
                new GrammarSyntaxMessage( expectedMsgID, g, null, expectedArg, expectedExc );

            checkError( equeue, expectedMessage );
        }

        [TestMethod]
        public void TestAutoWildcard2() /*throws Exception*/
        {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ID INT -> ^(ID INT);\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";

            string treeGrammar =
                "tree grammar TP;\n" +
                "options {output=AST; ASTLabelType=CommonTree; tokenVocab=T;}\n" +
                "a : ^(ID .) \n" +
                "  ;\n";

            string found = execTreeParser( "T.g", grammar, "TParser", "TP.g",
                                          treeGrammar, "TP", "TLexer", "a", "a", "abc 34" );
            Assert.AreEqual( "(abc 34)" + NewLine, found );
        }

        [TestMethod]
        public void TestAutoWildcardWithLabel() /*throws Exception*/
        {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ID INT ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";

            string treeGrammar =
                "tree grammar TP;\n" +
                "options {output=AST; ASTLabelType=CommonTree; tokenVocab=T;}\n" +
                "a : ID c=. \n" +
                "  ;\n";

            string found = execTreeParser( "T.g", grammar, "TParser", "TP.g",
                                          treeGrammar, "TP", "TLexer", "a", "a", "abc 34" );
            Assert.AreEqual( "abc 34" + NewLine, found );
        }

        [TestMethod]
        public void TestAutoWildcardWithListLabel() /*throws Exception*/
        {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ID INT ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";

            string treeGrammar =
                "tree grammar TP;\n" +
                "options {output=AST; ASTLabelType=CommonTree; tokenVocab=T;}\n" +
                "a : ID c+=. \n" +
                "  ;\n";

            string found = execTreeParser( "T.g", grammar, "TParser", "TP.g",
                                          treeGrammar, "TP", "TLexer", "a", "a", "abc 34" );
            Assert.AreEqual( "abc 34" + NewLine, found );
        }

        [TestMethod]
        public void TestAutoDupMultiple() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ID ID INT;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";

            string treeGrammar =
                "tree grammar TP;\n" +
                "options {output=AST; ASTLabelType=CommonTree; tokenVocab=T;}\n" +
                "a : ID ID INT\n" +
                "  ;\n";

            string found = execTreeParser( "T.g", grammar, "TParser", "TP.g",
                                          treeGrammar, "TP", "TLexer", "a", "a", "a b 3" );
            Assert.AreEqual( "a b 3" + NewLine, found );
        }

        [TestMethod]
        public void TestAutoDupTree() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ID INT -> ^(ID INT);\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";

            string treeGrammar =
                "tree grammar TP;\n" +
                "options {output=AST; ASTLabelType=CommonTree; tokenVocab=T;}\n" +
                "a : ^(ID INT)\n" +
                "  ;\n";

            string found = execTreeParser( "T.g", grammar, "TParser", "TP.g",
                                          treeGrammar, "TP", "TLexer", "a", "a", "a 3" );
            Assert.AreEqual( "(a 3)" + NewLine, found );
        }

        [TestMethod]
        public void TestAutoDupTree2() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ID INT INT -> ^(ID INT INT);\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";

            string treeGrammar =
                "tree grammar TP;\n" +
                "options {output=AST; ASTLabelType=CommonTree; tokenVocab=T;}\n" +
                "a : ^(ID b b)\n" +
                "  ;\n" +
                "b : INT ;";

            string found = execTreeParser( "T.g", grammar, "TParser", "TP.g",
                                          treeGrammar, "TP", "TLexer", "a", "a", "a 3 4" );
            Assert.AreEqual( "(a 3 4)" + NewLine, found );
        }

        [TestMethod]
        public void TestAutoDupTreeWithLabels() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ID INT -> ^(ID INT);\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";

            string treeGrammar =
                "tree grammar TP;\n" +
                "options {output=AST; ASTLabelType=CommonTree; tokenVocab=T;}\n" +
                "a : ^(x=ID y=INT)\n" +
                "  ;\n";

            string found = execTreeParser( "T.g", grammar, "TParser", "TP.g",
                                          treeGrammar, "TP", "TLexer", "a", "a", "a 3" );
            Assert.AreEqual( "(a 3)" + NewLine, found );
        }

        [TestMethod]
        public void TestAutoDupTreeWithListLabels() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ID INT -> ^(ID INT);\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";

            string treeGrammar =
                "tree grammar TP;\n" +
                "options {output=AST; ASTLabelType=CommonTree; tokenVocab=T;}\n" +
                "a : ^(x+=ID y+=INT)\n" +
                "  ;\n";

            string found = execTreeParser( "T.g", grammar, "TParser", "TP.g",
                                          treeGrammar, "TP", "TLexer", "a", "a", "a 3" );
            Assert.AreEqual( "(a 3)" + NewLine, found );
        }

        [TestMethod]
        public void TestAutoDupTreeWithRuleRoot() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ID INT -> ^(ID INT);\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";

            string treeGrammar =
                "tree grammar TP;\n" +
                "options {output=AST; ASTLabelType=CommonTree; tokenVocab=T;}\n" +
                "a : ^(b INT) ;\n" +
                "b : ID ;\n";

            string found = execTreeParser( "T.g", grammar, "TParser", "TP.g",
                                          treeGrammar, "TP", "TLexer", "a", "a", "a 3" );
            Assert.AreEqual( "(a 3)" + NewLine, found );
        }

        [TestMethod]
        public void TestAutoDupTreeWithRuleRootAndLabels() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ID INT -> ^(ID INT);\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";

            string treeGrammar =
                "tree grammar TP;\n" +
                "options {output=AST; ASTLabelType=CommonTree; tokenVocab=T;}\n" +
                "a : ^(x=b INT) ;\n" +
                "b : ID ;\n";

            string found = execTreeParser( "T.g", grammar, "TParser", "TP.g",
                                          treeGrammar, "TP", "TLexer", "a", "a", "a 3" );
            Assert.AreEqual( "(a 3)" + NewLine, found );
        }

        [TestMethod]
        public void TestAutoDupTreeWithRuleRootAndListLabels() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ID INT -> ^(ID INT);\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";

            string treeGrammar =
                "tree grammar TP;\n" +
                "options {output=AST; ASTLabelType=CommonTree; tokenVocab=T;}\n" +
                "a : ^(x+=b y+=c) ;\n" +
                "b : ID ;\n" +
                "c : INT ;\n";

            string found = execTreeParser( "T.g", grammar, "TParser", "TP.g",
                                          treeGrammar, "TP", "TLexer", "a", "a", "a 3" );
            Assert.AreEqual( "(a 3)" + NewLine, found );
        }

        [TestMethod]
        public void TestAutoDupNestedTree() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : x=ID y=ID INT -> ^($x ^($y INT));\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";

            string treeGrammar =
                "tree grammar TP;\n" +
                "options {output=AST; ASTLabelType=CommonTree; tokenVocab=T;}\n" +
                "a : ^(ID ^(ID INT))\n" +
                "  ;\n";

            string found = execTreeParser( "T.g", grammar, "TParser", "TP.g",
                                          treeGrammar, "TP", "TLexer", "a", "a", "a b 3" );
            Assert.AreEqual( "(a (b 3))" + NewLine, found );
        }

        [TestMethod]
        public void TestAutoDupTreeWithSubruleInside() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "tokens {OP;}\n" +
                "a : (x=ID|x=INT) -> ^(OP $x) ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";

            string treeGrammar =
                "tree grammar TP;\n" +
                "options {output=AST; ASTLabelType=CommonTree; tokenVocab=T;}\n" +
                "a : ^(OP (b|c)) ;\n" +
                "b : ID ;\n" +
                "c : INT ;\n";

            string found = execTreeParser( "T.g", grammar, "TParser", "TP.g",
                                          treeGrammar, "TP", "TLexer", "a", "a", "a" );
            Assert.AreEqual( "(OP a)" + NewLine, found );
        }

        [TestMethod]
        public void TestDelete() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ID ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";

            string treeGrammar =
                "tree grammar TP;\n" +
                "options {output=AST; ASTLabelType=CommonTree; tokenVocab=T;}\n" +
                "a : ID -> \n" +
                "  ;\n";

            string found = execTreeParser( "T.g", grammar, "TParser", "TP.g",
                                          treeGrammar, "TP", "TLexer", "a", "a", "abc" );
            Assert.AreEqual( "", found );
        }

        [TestMethod]
        public void TestSetMatchNoRewrite() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ID INT ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";

            string treeGrammar =
                "tree grammar TP;\n" +
                "options {output=AST; ASTLabelType=CommonTree; tokenVocab=T;}\n" +
                "a : b INT\n" +
                "  ;\n" +
                "b : ID | INT ;\n";

            string found = execTreeParser( "T.g", grammar, "TParser", "TP.g",
                                          treeGrammar, "TP", "TLexer", "a", "a", "abc 34" );
            Assert.AreEqual( "abc 34" + NewLine, found );
        }

        [TestMethod]
        public void TestSetOptionalMatchNoRewrite() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ID INT ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";

            string treeGrammar =
                "tree grammar TP;\n" +
                "options {output=AST; ASTLabelType=CommonTree; tokenVocab=T;}\n" +
                "a : (ID|INT)? INT ;\n";

            string found = execTreeParser( "T.g", grammar, "TParser", "TP.g",
                                          treeGrammar, "TP", "TLexer", "a", "a", "abc 34" );
            Assert.AreEqual( "abc 34" + NewLine, found );
        }


        [TestMethod]
        public void TestSetMatchNoRewriteLevel2() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : x=ID INT -> ^($x INT);\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";

            string treeGrammar =
                "tree grammar TP;\n" +
                "options {output=AST; ASTLabelType=CommonTree; tokenVocab=T;}\n" +
                "a : ^(ID (ID | INT) ) ;\n";

            string found = execTreeParser( "T.g", grammar, "TParser", "TP.g",
                                          treeGrammar, "TP", "TLexer", "a", "a", "abc 34" );
            Assert.AreEqual( "(abc 34)" + NewLine, found );
        }

        [TestMethod]
        public void TestSetMatchNoRewriteLevel2Root() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : x=ID INT -> ^($x INT);\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";

            string treeGrammar =
                "tree grammar TP;\n" +
                "options {output=AST; ASTLabelType=CommonTree; tokenVocab=T;}\n" +
                "a : ^((ID | INT) INT) ;\n";

            string found = execTreeParser( "T.g", grammar, "TParser", "TP.g",
                                          treeGrammar, "TP", "TLexer", "a", "a", "abc 34" );
            Assert.AreEqual( "(abc 34)" + NewLine, found );
        }


        // REWRITE MODE

        [TestMethod]
        public void TestRewriteModeCombinedRewriteAndAuto() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ID INT -> ^(ID INT) | INT ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";

            string treeGrammar =
                "tree grammar TP;\n" +
                "options {output=AST; ASTLabelType=CommonTree; tokenVocab=T; rewrite=true;}\n" +
                "a : ^(ID INT) -> ^(ID[\"ick\"] INT)\n" +
                "  | INT\n" + // leaves it alone, returning $a.start
                "  ;\n";

            string found = execTreeParser( "T.g", grammar, "TParser", "TP.g",
                                          treeGrammar, "TP", "TLexer", "a", "a", "abc 34" );
            Assert.AreEqual( "(ick 34)" + NewLine, found );

            found = execTreeParser( "T.g", grammar, "TParser", "TP.g",
                                   treeGrammar, "TP", "TLexer", "a", "a", "34" );
            Assert.AreEqual( "34" + NewLine, found );
        }

        [TestMethod]
        public void TestRewriteModeFlatTree() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ID INT -> ID INT | INT ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";

            string treeGrammar =
                "tree grammar TP;\n" +
                "options {output=AST; ASTLabelType=CommonTree; tokenVocab=T; rewrite=true;}\n" +
                "s : ID a ;\n" +
                "a : INT -> INT[\"1\"]\n" +
                "  ;\n";

            string found = execTreeParser( "T.g", grammar, "TParser", "TP.g",
                                          treeGrammar, "TP", "TLexer", "a", "s", "abc 34" );
            Assert.AreEqual( "abc 1" + NewLine, found );
        }

        [TestMethod]
        public void TestRewriteModeChainRuleFlatTree() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ID INT -> ID INT | INT ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";

            string treeGrammar =
                "tree grammar TP;\n" +
                "options {output=AST; ASTLabelType=CommonTree; tokenVocab=T; rewrite=true;}\n" +
                "s : a ;\n" +
                "a : b ;\n" +
                "b : ID INT -> INT ID\n" +
                "  ;\n";

            string found = execTreeParser( "T.g", grammar, "TParser", "TP.g",
                                          treeGrammar, "TP", "TLexer", "a", "s", "abc 34" );
            Assert.AreEqual( "34 abc" + NewLine, found );
        }

        [TestMethod]
        public void TestRewriteModeChainRuleTree() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ID INT -> ^(ID INT) ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";

            string treeGrammar =
                "tree grammar TP;\n" +
                "options {output=AST; ASTLabelType=CommonTree; tokenVocab=T; rewrite=true;}\n" +
                "s : a ;\n" +
                "a : b ;\n" + // a.tree must become b.tree
                "b : ^(ID INT) -> INT\n" +
                "  ;\n";

            string found = execTreeParser( "T.g", grammar, "TParser", "TP.g",
                                          treeGrammar, "TP", "TLexer", "a", "s", "abc 34" );
            Assert.AreEqual( "34" + NewLine, found );
        }

        [TestMethod]
        public void TestRewriteModeChainRuleTree2() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ID INT -> ^(ID INT) ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";

            string treeGrammar =
                "tree grammar TP;\n" +
                "options {output=AST; ASTLabelType=CommonTree; tokenVocab=T; rewrite=true;}\n" +
                "tokens { X; }\n" +
                "s : a* b ;\n" + // only b contributes to tree, but it's after a*; s.tree = b.tree
                "a : X ;\n" +
                "b : ^(ID INT) -> INT\n" +
                "  ;\n";

            string found = execTreeParser( "T.g", grammar, "TParser", "TP.g",
                                          treeGrammar, "TP", "TLexer", "a", "s", "abc 34" );
            Assert.AreEqual( "34" + NewLine, found );
        }

        [TestMethod]
        public void TestRewriteModeChainRuleTree3() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : 'boo' ID INT -> 'boo' ^(ID INT) ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";

            string treeGrammar =
                "tree grammar TP;\n" +
                "options {output=AST; ASTLabelType=CommonTree; tokenVocab=T; rewrite=true;}\n" +
                "tokens { X; }\n" +
                "s : 'boo' a* b ;\n" + // don't reset s.tree to b.tree due to 'boo'
                "a : X ;\n" +
                "b : ^(ID INT) -> INT\n" +
                "  ;\n";

            string found = execTreeParser( "T.g", grammar, "TParser", "TP.g",
                                          treeGrammar, "TP", "TLexer", "a", "s", "boo abc 34" );
            Assert.AreEqual( "boo 34" + NewLine, found );
        }

        [TestMethod]
        public void TestRewriteModeChainRuleTree4() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : 'boo' ID INT -> ^('boo' ^(ID INT)) ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";

            string treeGrammar =
                "tree grammar TP;\n" +
                "options {output=AST; ASTLabelType=CommonTree; tokenVocab=T; rewrite=true;}\n" +
                "tokens { X; }\n" +
                "s : ^('boo' a* b) ;\n" + // don't reset s.tree to b.tree due to 'boo'
                "a : X ;\n" +
                "b : ^(ID INT) -> INT\n" +
                "  ;\n";

            string found = execTreeParser( "T.g", grammar, "TParser", "TP.g",
                                          treeGrammar, "TP", "TLexer", "a", "s", "boo abc 34" );
            Assert.AreEqual( "(boo 34)" + NewLine, found );
        }

        [TestMethod]
        public void TestRewriteModeChainRuleTree5() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : 'boo' ID INT -> ^('boo' ^(ID INT)) ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";

            string treeGrammar =
                "tree grammar TP;\n" +
                "options {output=AST; ASTLabelType=CommonTree; tokenVocab=T; rewrite=true;}\n" +
                "tokens { X; }\n" +
                "s : ^(a b) ;\n" + // s.tree is a.tree
                "a : 'boo' ;\n" +
                "b : ^(ID INT) -> INT\n" +
                "  ;\n";

            string found = execTreeParser( "T.g", grammar, "TParser", "TP.g",
                                          treeGrammar, "TP", "TLexer", "a", "s", "boo abc 34" );
            Assert.AreEqual( "(boo 34)" + NewLine, found );
        }

        [TestMethod]
        public void TestRewriteOfRuleRef() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ID INT -> ID INT | INT ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";

            string treeGrammar =
                "tree grammar TP;\n" +
                "options {output=AST; ASTLabelType=CommonTree; tokenVocab=T; rewrite=true;}\n" +
                "s : a -> a ;\n" +
                "a : ID INT -> ID INT ;\n";

            string found = execTreeParser( "T.g", grammar, "TParser", "TP.g",
                                          treeGrammar, "TP", "TLexer", "a", "s", "abc 34" );
            Assert.AreEqual( "abc 34" + NewLine, found );
        }

        [TestMethod]
        public void TestRewriteOfRuleRefRoot() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ID INT INT -> ^(INT ^(ID INT));\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";

            string treeGrammar =
                "tree grammar TP;\n" +
                "options {output=AST; ASTLabelType=CommonTree; tokenVocab=T; rewrite=true;}\n" +
                "s : ^(a ^(ID INT)) -> a ;\n" +
                "a : INT ;\n";

            string found = execTreeParser( "T.g", grammar, "TParser", "TP.g",
                                          treeGrammar, "TP", "TLexer", "a", "s", "abc 12 34" );
            // emits whole tree when you ref the root since I can't know whether
            // you want the children or not.  You might be returning a whole new
            // tree.  Hmm...still seems weird.  oh well.
            Assert.AreEqual( "(12 (abc 34))" + NewLine, found );
        }

        [TestMethod]
        public void TestRewriteOfRuleRefRootLabeled() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ID INT INT -> ^(INT ^(ID INT));\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";

            string treeGrammar =
                "tree grammar TP;\n" +
                "options {output=AST; ASTLabelType=CommonTree; tokenVocab=T; rewrite=true;}\n" +
                "s : ^(label=a ^(ID INT)) -> a ;\n" +
                "a : INT ;\n";

            string found = execTreeParser( "T.g", grammar, "TParser", "TP.g",
                                          treeGrammar, "TP", "TLexer", "a", "s", "abc 12 34" );
            // emits whole tree when you ref the root since I can't know whether
            // you want the children or not.  You might be returning a whole new
            // tree.  Hmm...still seems weird.  oh well.
            Assert.AreEqual( "(12 (abc 34))" + NewLine, found );
        }

        [TestMethod]
        [Ignore]
        public void TestRewriteOfRuleRefRootListLabeled() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ID INT INT -> ^(INT ^(ID INT));\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";

            string treeGrammar =
                "tree grammar TP;\n" +
                "options {output=AST; ASTLabelType=CommonTree; tokenVocab=T; rewrite=true;}\n" +
                "s : ^(label+=a ^(ID INT)) -> a ;\n" +
                "a : INT ;\n";

            string found = execTreeParser( "T.g", grammar, "TParser", "TP.g",
                                          treeGrammar, "TP", "TLexer", "a", "s", "abc 12 34" );
            // emits whole tree when you ref the root since I can't know whether
            // you want the children or not.  You might be returning a whole new
            // tree.  Hmm...still seems weird.  oh well.
            Assert.AreEqual( "(12 (abc 34))" + NewLine, found );
        }

        [TestMethod]
        public void TestRewriteOfRuleRefChild() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ID INT -> ^(ID ^(INT INT));\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";

            string treeGrammar =
                "tree grammar TP;\n" +
                "options {output=AST; ASTLabelType=CommonTree; tokenVocab=T; rewrite=true;}\n" +
                "s : ^(ID a) -> a ;\n" +
                "a : ^(INT INT) ;\n";

            string found = execTreeParser( "T.g", grammar, "TParser", "TP.g",
                                          treeGrammar, "TP", "TLexer", "a", "s", "abc 34" );
            Assert.AreEqual( "(34 34)" + NewLine, found );
        }

        [TestMethod]
        public void TestRewriteOfRuleRefLabel() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ID INT -> ^(ID ^(INT INT));\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";

            string treeGrammar =
                "tree grammar TP;\n" +
                "options {output=AST; ASTLabelType=CommonTree; tokenVocab=T; rewrite=true;}\n" +
                "s : ^(ID label=a) -> a ;\n" +
                "a : ^(INT INT) ;\n";

            string found = execTreeParser( "T.g", grammar, "TParser", "TP.g",
                                          treeGrammar, "TP", "TLexer", "a", "s", "abc 34" );
            Assert.AreEqual( "(34 34)" + NewLine, found );
        }

        [TestMethod]
        public void TestRewriteOfRuleRefListLabel() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ID INT -> ^(ID ^(INT INT));\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";

            string treeGrammar =
                "tree grammar TP;\n" +
                "options {output=AST; ASTLabelType=CommonTree; tokenVocab=T; rewrite=true;}\n" +
                "s : ^(ID label+=a) -> a ;\n" +
                "a : ^(INT INT) ;\n";

            string found = execTreeParser( "T.g", grammar, "TParser", "TP.g",
                                          treeGrammar, "TP", "TLexer", "a", "s", "abc 34" );
            Assert.AreEqual( "(34 34)" + NewLine, found );
        }

        [TestMethod]
        public void TestRewriteModeWithPredicatedRewrites() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ID INT -> ^(ID[\"root\"] ^(ID INT)) | INT -> ^(ID[\"root\"] INT) ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";

            string treeGrammar =
                "tree grammar TP;\n" +
                "options {output=AST; ASTLabelType=CommonTree; tokenVocab=T; rewrite=true;}\n" +
                "s : ^(ID a) {System.out.println(\"altered tree=\"+$s.start.toStringTree());};\n" +
                "a : ^(ID INT) -> {true}? ^(ID[\"ick\"] INT)\n" +
                "              -> INT\n" +
                "  ;\n";

            string found = execTreeParser( "T.g", grammar, "TParser", "TP.g",
                                          treeGrammar, "TP", "TLexer", "a", "s", "abc 34" );
            Assert.AreEqual( "altered tree=(root (ick 34))" + NewLine +
                         "(root (ick 34))" + NewLine, found );
        }

        [TestMethod]
        public void TestWildcardSingleNode() /*throws Exception*/
        {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ID INT -> ^(ID[\"root\"] INT);\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";

            string treeGrammar =
                "tree grammar TP;\n" +
                "options {output=AST; ASTLabelType=CommonTree; tokenVocab=T;}\n" +
                "s : ^(ID c=.) -> $c\n" +
                "  ;\n";

            string found = execTreeParser( "T.g", grammar, "TParser", "TP.g",
                                          treeGrammar, "TP", "TLexer", "a", "s", "abc 34" );
            Assert.AreEqual( "34" + NewLine, found );
        }

        [TestMethod]
        public void TestWildcardUnlabeledSingleNode() /*throws Exception*/
        {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ID INT -> ^(ID INT);\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";

            string treeGrammar =
                "tree grammar TP;\n" +
                "options {output=AST; ASTLabelType=CommonTree; tokenVocab=T;}\n" +
                "s : ^(ID .) -> ID\n" +
                "  ;\n";

            string found = execTreeParser( "T.g", grammar, "TParser", "TP.g",
                                          treeGrammar, "TP", "TLexer", "a", "s", "abc 34" );
            Assert.AreEqual( "abc" + NewLine, found );
        }

        [TestMethod]
        public void TestWildcardGrabsSubtree() /*throws Exception*/
        {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ID x=INT y=INT z=INT -> ^(ID[\"root\"] ^($x $y $z));\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";

            string treeGrammar =
                "tree grammar TP;\n" +
                "options {output=AST; ASTLabelType=CommonTree; tokenVocab=T;}\n" +
                "s : ^(ID c=.) -> $c\n" +
                "  ;\n";

            string found = execTreeParser( "T.g", grammar, "TParser", "TP.g",
                                          treeGrammar, "TP", "TLexer", "a", "s", "abc 1 2 3" );
            Assert.AreEqual( "(1 2 3)" + NewLine, found );
        }

        [TestMethod]
        public void TestWildcardGrabsSubtree2() /*throws Exception*/
        {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ID x=INT y=INT z=INT -> ID ^($x $y $z);\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";

            string treeGrammar =
                "tree grammar TP;\n" +
                "options {output=AST; ASTLabelType=CommonTree; tokenVocab=T;}\n" +
                "s : ID c=. -> $c\n" +
                "  ;\n";

            string found = execTreeParser( "T.g", grammar, "TParser", "TP.g",
                                          treeGrammar, "TP", "TLexer", "a", "s", "abc 1 2 3" );
            Assert.AreEqual( "(1 2 3)" + NewLine, found );
        }

        [TestMethod]
        public void TestWildcardListLabel() /*throws Exception*/
        {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : INT INT INT ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";

            string treeGrammar =
                "tree grammar TP;\n" +
                "options {output=AST; ASTLabelType=CommonTree; tokenVocab=T;}\n" +
                "s : (c+=.)+ -> $c+\n" +
                "  ;\n";

            string found = execTreeParser( "T.g", grammar, "TParser", "TP.g",
                                          treeGrammar, "TP", "TLexer", "a", "s", "1 2 3" );
            Assert.AreEqual( "1 2 3" + NewLine, found );
        }

        [TestMethod]
        public void TestWildcardListLabel2() /*throws Exception*/
        {
            string grammar =
                "grammar T;\n" +
                "options {output=AST; ASTLabelType=CommonTree;}\n" +
                "a  : x=INT y=INT z=INT -> ^($x ^($y $z) ^($y $z));\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";

            string treeGrammar =
                "tree grammar TP;\n" +
                "options {output=AST; ASTLabelType=CommonTree; tokenVocab=T; rewrite=true;}\n" +
                "s : ^(INT (c+=.)+) -> $c+\n" +
                "  ;\n";

            string found = execTreeParser( "T.g", grammar, "TParser", "TP.g",
                                          treeGrammar, "TP", "TLexer", "a", "s", "1 2 3" );
            Assert.AreEqual( "(2 3) (2 3)" + NewLine, found );
        }

        [TestMethod]
        public void TestRuleResultAsRoot()
        {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ID '=' INT -> ^('=' ID INT);\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "COLON : ':' ;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";

            string treeGrammar =
                "tree grammar TP;\n" +
                "options {output=AST; rewrite=true; ASTLabelType=CommonTree; tokenVocab=T;}\n" +
                "a : ^(eq e1=ID e2=.) -> ^(eq $e2 $e1) ;\n" +
                "eq : '=' | ':' {;} ;\n";  // bug in set match, doesn't add to tree!! booh. force nonset.

            string found = execTreeParser("T.g", grammar, "TParser", "TP.g",
                                          treeGrammar, "TP", "TLexer", "a", "a", "abc = 34");
            Assert.AreEqual("(= 34 abc)" + NewLine, found);
        }
    }
}
