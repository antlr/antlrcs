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

    [TestClass]
    public class TestTreeParsing : BaseTest
    {
        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestFlatList() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ID INT;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";

            string treeGrammar =
                "tree grammar TP; options {ASTLabelType=CommonTree;}\n" +
                "a : ID INT\n" +
                "    {System.out.println($ID+\", \"+$INT);}\n" +
                "  ;\n";

            string found = execTreeParser( "T.g", grammar, "TParser", "TP.g",
                        treeGrammar, "TP", "TLexer", "a", "a", "abc 34" );
            Assert.AreEqual( "abc, 34" + NewLine, found );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestSimpleTree() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ID INT -> ^(ID INT);\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";

            string treeGrammar =
                "tree grammar TP; options {ASTLabelType=CommonTree;}\n" +
                "a : ^(ID INT)\n" +
                "    {System.out.println($ID+\", \"+$INT);}\n" +
                "  ;\n";

            string found = execTreeParser( "T.g", grammar, "TParser", "TP.g",
                        treeGrammar, "TP", "TLexer", "a", "a", "abc 34" );
            Assert.AreEqual( "abc, 34" + NewLine, found );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestFlatVsTreeDecision() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : b c ;\n" +
                "b : ID INT -> ^(ID INT);\n" +
                "c : ID INT;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";

            string treeGrammar =
                "tree grammar TP; options {ASTLabelType=CommonTree;}\n" +
                "a : b b ;\n" +
                "b : ID INT    {System.out.print($ID+\" \"+$INT);}\n" +
                "  | ^(ID INT) {System.out.print(\"^(\"+$ID+\" \"+$INT+')');}\n" +
                "  ;\n";

            string found = execTreeParser( "T.g", grammar, "TParser", "TP.g",
                        treeGrammar, "TP", "TLexer", "a", "a", "a 1 b 2" );
            Assert.AreEqual( "^(a 1)b 2" + NewLine, found );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestFlatVsTreeDecision2() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : b c ;\n" +
                "b : ID INT+ -> ^(ID INT+);\n" +
                "c : ID INT+;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";

            string treeGrammar =
                "tree grammar TP; options {ASTLabelType=CommonTree;}\n" +
                "a : b b ;\n" +
                "b : ID INT+    {System.out.print($ID+\" \"+$INT);}\n" +
                "  | ^(x=ID (y=INT)+) {System.out.print(\"^(\"+$x+' '+$y+')');}\n" +
                "  ;\n";

            string found = execTreeParser( "T.g", grammar, "TParser", "TP.g",
                        treeGrammar, "TP", "TLexer", "a", "a",
                        "a 1 2 3 b 4 5" );
            Assert.AreEqual( "^(a 3)b 5" + NewLine, found );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestCyclicDFALookahead() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ID INT+ PERIOD;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "SEMI : ';' ;\n" +
                "PERIOD : '.' ;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";

            string treeGrammar =
                "tree grammar TP; options {ASTLabelType=CommonTree;}\n" +
                "a : ID INT+ PERIOD {System.out.print(\"alt 1\");}" +
                "  | ID INT+ SEMI   {System.out.print(\"alt 2\");}\n" +
                "  ;\n";

            string found = execTreeParser( "T.g", grammar, "TParser", "TP.g",
                        treeGrammar, "TP", "TLexer", "a", "a", "a 1 2 3." );
            Assert.AreEqual( "alt 1" + NewLine, found );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestTemplateOutput() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ID INT;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";

            string treeGrammar =
                "tree grammar TP;\n" +
                "options {output=template; ASTLabelType=CommonTree;}\n" +
                "s : a {System.out.println($a.st);};\n" +
                "a : ID INT -> {new StringTemplate($INT.text)}\n" +
                "  ;\n";

            string found = execTreeParser( "T.g", grammar, "TParser", "TP.g",
                        treeGrammar, "TP", "TLexer", "a", "s", "abc 34" );
            Assert.AreEqual( "34" + NewLine, found );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestNullableChildList() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ID INT? -> ^(ID INT?);\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";

            string treeGrammar =
                "tree grammar TP; options {ASTLabelType=CommonTree;}\n" +
                "a : ^(ID INT?)\n" +
                "    {System.out.println($ID);}\n" +
                "  ;\n";

            string found = execTreeParser( "T.g", grammar, "TParser", "TP.g",
                        treeGrammar, "TP", "TLexer", "a", "a", "abc" );
            Assert.AreEqual( "abc" + NewLine, found );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestNullableChildList2() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ID INT? SEMI -> ^(ID INT?) SEMI ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "SEMI : ';' ;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";

            string treeGrammar =
                "tree grammar TP; options {ASTLabelType=CommonTree;}\n" +
                "a : ^(ID INT?) SEMI\n" +
                "    {System.out.println($ID);}\n" +
                "  ;\n";

            string found = execTreeParser( "T.g", grammar, "TParser", "TP.g",
                        treeGrammar, "TP", "TLexer", "a", "a", "abc;" );
            Assert.AreEqual( "abc" + NewLine, found );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestNullableChildList3() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : x=ID INT? (y=ID)? SEMI -> ^($x INT? $y?) SEMI ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "SEMI : ';' ;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";

            string treeGrammar =
                "tree grammar TP; options {ASTLabelType=CommonTree;}\n" +
                "a : ^(ID INT? b) SEMI\n" +
                "    {System.out.println($ID+\", \"+$b.text);}\n" +
                "  ;\n" +
                "b : ID? ;\n";

            string found = execTreeParser( "T.g", grammar, "TParser", "TP.g",
                        treeGrammar, "TP", "TLexer", "a", "a", "abc def;" );
            Assert.AreEqual( "abc, def" + NewLine, found );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestActionsAfterRoot() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : x=ID INT? SEMI -> ^($x INT?) ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "SEMI : ';' ;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";

            string treeGrammar =
                "tree grammar TP; options {ASTLabelType=CommonTree;}\n" +
                "a @init {int x=0;} : ^(ID {x=1;} {x=2;} INT?)\n" +
                "    {System.out.println($ID+\", \"+x);}\n" +
                "  ;\n";

            string found = execTreeParser( "T.g", grammar, "TParser", "TP.g",
                        treeGrammar, "TP", "TLexer", "a", "a", "abc;" );
            Assert.AreEqual( "abc, 2" + NewLine, found );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestWildcardLookahead() /*throws Exception*/
        {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ID '+'^ INT;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "SEMI : ';' ;\n" +
                "PERIOD : '.' ;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";

            string treeGrammar =
                "tree grammar TP; options {tokenVocab=T; ASTLabelType=CommonTree;}\n" +
                "a : ^('+' . INT) {System.out.print(\"alt 1\");}" +
                "  ;\n";

            string found = execTreeParser( "T.g", grammar, "TParser", "TP.g",
                        treeGrammar, "TP", "TLexer", "a", "a", "a + 2" );
            Assert.AreEqual( "alt 1" + NewLine, found );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestWildcardLookahead2() /*throws Exception*/
        {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ID '+'^ INT;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "SEMI : ';' ;\n" +
                "PERIOD : '.' ;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";

            string treeGrammar =
                "tree grammar TP; options {tokenVocab=T; ASTLabelType=CommonTree;}\n" +
                "a : ^('+' . INT) {System.out.print(\"alt 1\");}" +
                "  | ^('+' . .)   {System.out.print(\"alt 2\");}\n" +
                "  ;\n";

            // AMBIG upon '+' DOWN INT UP etc.. but so what.

            string found = execTreeParser( "T.g", grammar, "TParser", "TP.g",
                        treeGrammar, "TP", "TLexer", "a", "a", "a + 2" );
            Assert.AreEqual( "alt 1" + NewLine, found );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestWildcardLookahead3() /*throws Exception*/
        {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ID '+'^ INT;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "SEMI : ';' ;\n" +
                "PERIOD : '.' ;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";

            string treeGrammar =
                "tree grammar TP; options {tokenVocab=T; ASTLabelType=CommonTree;}\n" +
                "a : ^('+' ID INT) {System.out.print(\"alt 1\");}" +
                "  | ^('+' . .)   {System.out.print(\"alt 2\");}\n" +
                "  ;\n";

            // AMBIG upon '+' DOWN INT UP etc.. but so what.

            string found = execTreeParser( "T.g", grammar, "TParser", "TP.g",
                        treeGrammar, "TP", "TLexer", "a", "a", "a + 2" );
            Assert.AreEqual( "alt 1" + NewLine, found );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestWildcardPlusLookahead() /*throws Exception*/
        {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ID '+'^ INT;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "SEMI : ';' ;\n" +
                "PERIOD : '.' ;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";

            string treeGrammar =
                "tree grammar TP; options {tokenVocab=T; ASTLabelType=CommonTree;}\n" +
                "a : ^('+' INT INT ) {System.out.print(\"alt 1\");}" +
                "  | ^('+' .+)   {System.out.print(\"alt 2\");}\n" +
                "  ;\n";

            // AMBIG upon '+' DOWN INT UP etc.. but so what.

            string found = execTreeParser( "T.g", grammar, "TParser", "TP.g",
                        treeGrammar, "TP", "TLexer", "a", "a", "a + 2" );
            Assert.AreEqual( "alt 2" + NewLine, found );
        }

    }
}
