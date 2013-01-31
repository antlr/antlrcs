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

    /** Test hetero trees in parsers and tree parsers */
    [TestClass]
    public class TestHeteroAST : BaseTest
    {
        protected bool debug = false;

        // PARSERS -- AUTO AST

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestToken() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "@members {static class V extends CommonTree {\n" +
                "  public V(Token t) { token=t;}\n" +
                "  public String toString() { return token.getText()+\"<V>\";}\n" +
                "}\n" +
                "}\n" +
                "a : ID<node=V> ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                        "a", "a", debug );
            Assert.AreEqual( "a<V>" + NewLine, found );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestTokenCommonTree()
        {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ID<node=CommonTree> ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser("T.g", grammar, "TParser", "TLexer",
                        "a", "a", debug);
            Assert.AreEqual("a" + NewLine, found);
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestTokenWithQualifiedType() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "@members {static class V extends CommonTree {\n" +
                "  public V(Token t) { token=t;}\n" +
                "  public String toString() { return token.getText()+\"<V>\";}\n" +
                "}\n" +
                "}\n" +
                "a : ID<node=TParser.V> ;\n" + // TParser.V is qualified name
                "ID : 'a'..'z'+ ;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                        "a", "a", debug );
            Assert.AreEqual( "a<V>" + NewLine, found );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestTokenWithLabel() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "@members {static class V extends CommonTree {\n" +
                "  public V(Token t) { token=t;}\n" +
                "  public String toString() { return token.getText()+\"<V>\";}\n" +
                "}\n" +
                "}\n" +
                "a : x=ID<node=V> ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                        "a", "a", debug );
            Assert.AreEqual( "a<V>" + NewLine, found );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestTokenWithListLabel() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "@members {static class V extends CommonTree {\n" +
                "  public V(Token t) { token=t;}\n" +
                "  public String toString() { return token.getText()+\"<V>\";}\n" +
                "}\n" +
                "}\n" +
                "a : x+=ID<node=V> ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                        "a", "a", debug );
            Assert.AreEqual( "a<V>" + NewLine, found );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestTokenRoot() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "@members {static class V extends CommonTree {\n" +
                "  public V(Token t) { token=t;}\n" +
                "  public String toString() { return token.getText()+\"<V>\";}\n" +
                "}\n" +
                "}\n" +
                "a : ID<node=V>^ ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                        "a", "a", debug );
            Assert.AreEqual( "a<V>" + NewLine, found );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestTokenRootWithListLabel() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "@members {static class V extends CommonTree {\n" +
                "  public V(Token t) { token=t;}\n" +
                "  public String toString() { return token.getText()+\"<V>\";}\n" +
                "}\n" +
                "}\n" +
                "a : x+=ID<node=V>^ ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                        "a", "a", debug );
            Assert.AreEqual( "a<V>" + NewLine, found );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestString() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "@members {static class V extends CommonTree {\n" +
                "  public V(Token t) { token=t;}\n" +
                "  public String toString() { return token.getText()+\"<V>\";}\n" +
                "}\n" +
                "}\n" +
                "a : 'begin'<node=V> ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                        "a", "begin", debug );
            Assert.AreEqual( "begin<V>" + NewLine, found );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestStringRoot() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "@members {static class V extends CommonTree {\n" +
                "  public V(Token t) { token=t;}\n" +
                "  public String toString() { return token.getText()+\"<V>\";}\n" +
                "}\n" +
                "}\n" +
                "a : 'begin'<node=V>^ ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                        "a", "begin", debug );
            Assert.AreEqual( "begin<V>" + NewLine, found );
        }

        // PARSERS -- REWRITE AST

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestRewriteToken() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "@members {static class V extends CommonTree {\n" +
                "  public V(Token t) { token=t;}\n" +
                "  public String toString() { return token.getText()+\"<V>\";}\n" +
                "}\n" +
                "}\n" +
                "a : ID -> ID<node=V> ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                        "a", "a", debug );
            Assert.AreEqual( "a<V>" + NewLine, found );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestRewriteTokenWithArgs() /*throws Exception*/ {
            // arg to ID<V>[42,19,30] means you're constructing node not associated with ID
            // so must pass in token manually
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "@members {\n" +
                "static class V extends CommonTree {\n" +
                "  public int x,y,z;\n" +
                "  public V(int ttype, int x, int y, int z) { this.x=x; this.y=y; this.z=z; token=new CommonToken(ttype,\"\"); }\n" +
                "  public V(int ttype, Token t, int x) { token=t; this.x=x;}\n" +
                "  public String toString() { return (token!=null?token.getText():\"\")+\"<V>;\"+x+y+z;}\n" +
                "}\n" +
                "}\n" +
                "a : ID -> ID<node=V>[42,19,30] ID<node=V>[$ID,99] ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                        "a", "a", debug );
            Assert.AreEqual( "<V>;421930 a<V>;9900" + NewLine, found );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestRewriteTokenRoot() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "@members {static class V extends CommonTree {\n" +
                "  public V(Token t) { token=t;}\n" +
                "  public String toString() { return token.getText()+\"<V>\";}\n" +
                "}\n" +
                "}\n" +
                "a : ID INT -> ^(ID<node=V> INT) ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+ ;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                        "a", "a 2", debug );
            Assert.AreEqual( "(a<V> 2)" + NewLine, found );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestRewriteString() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "@members {static class V extends CommonTree {\n" +
                "  public V(Token t) { token=t;}\n" +
                "  public String toString() { return token.getText()+\"<V>\";}\n" +
                "}\n" +
                "}\n" +
                "a : 'begin' -> 'begin'<node=V> ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                        "a", "begin", debug );
            Assert.AreEqual( "begin<V>" + NewLine, found );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestRewriteStringRoot() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "@members {static class V extends CommonTree {\n" +
                "  public V(Token t) { token=t;}\n" +
                "  public String toString() { return token.getText()+\"<V>\";}\n" +
                "}\n" +
                "}\n" +
                "a : 'begin' INT -> ^('begin'<node=V> INT) ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+ ;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                        "a", "begin 2", debug );
            Assert.AreEqual( "(begin<V> 2)" + NewLine, found );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestRewriteRuleResults() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "tokens {LIST;}\n" +
                "@members {\n" +
                "static class V extends CommonTree {\n" +
                "  public V(Token t) { token=t;}\n" +
                "  public String toString() { return token.getText()+\"<V>\";}\n" +
                "}\n" +
                "static class W extends CommonTree {\n" +
                "  public W(int tokenType, String txt) { super(new CommonToken(tokenType,txt)); }\n" +
                "  public W(Token t) { token=t;}\n" +
                "  public String toString() { return token.getText()+\"<W>\";}\n" +
                "}\n" +
                "}\n" +
                "a : id (',' id)* -> ^(LIST<node=W>[\"LIST\"] id+);\n" +
                "id : ID -> ID<node=V>;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                        "a", "a,b,c", debug );
            Assert.AreEqual( "(LIST<W> a<V> b<V> c<V>)" + NewLine, found );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestCopySemanticsWithHetero() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "@members {\n" +
                "static class V extends CommonTree {\n" +
                "  public V(Token t) { token=t;}\n" +  // for 'int'<V>
                "  public V(V node) { super(node); }\n\n" + // for dupNode
                "  public Tree dupNode() { return new V(this); }\n" + // for dup'ing type
                "  public String toString() { return token.getText()+\"<V>\";}\n" +
                "}\n" +
                "}\n" +
                "a : type ID (',' ID)* ';' -> ^(type ID)+;\n" +
                "type : 'int'<node=V> ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                        "a", "int a, b, c;", debug );
            Assert.AreEqual( "(int<V> a) (int<V> b) (int<V> c)" + NewLine, found );
        }

        // TREE PARSERS -- REWRITE AST

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestTreeParserRewriteFlatList() /*throws Exception*/ {
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
                "@members {\n" +
                "static class V extends CommonTree {\n" +
                "  public V(Object t) { super((CommonTree)t); }\n" +
                "  public String toString() { return token.getText()+\"<V>\";}\n" +
                "}\n" +
                "static class W extends CommonTree {\n" +
                "  public W(Object t) { super((CommonTree)t); }\n" +
                "  public String toString() { return token.getText()+\"<W>\";}\n" +
                "}\n" +
                "}\n" +
                "a : ID INT -> INT<node=V> ID<node=W>\n" +
                "  ;\n";

            string found = execTreeParser( "T.g", grammar, "TParser", "TP.g",
                        treeGrammar, "TP", "TLexer", "a", "a", "abc 34" );
            Assert.AreEqual( "34<V> abc<W>" + NewLine, found );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestTreeParserRewriteTree() /*throws Exception*/ {
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
                "@members {\n" +
                "static class V extends CommonTree {\n" +
                "  public V(Object t) { super((CommonTree)t); }\n" +
                "  public String toString() { return token.getText()+\"<V>\";}\n" +
                "}\n" +
                "static class W extends CommonTree {\n" +
                "  public W(Object t) { super((CommonTree)t); }\n" +
                "  public String toString() { return token.getText()+\"<W>\";}\n" +
                "}\n" +
                "}\n" +
                "a : ID INT -> ^(INT<node=V> ID<node=W>)\n" +
                "  ;\n";

            string found = execTreeParser( "T.g", grammar, "TParser", "TP.g",
                        treeGrammar, "TP", "TLexer", "a", "a", "abc 34" );
            Assert.AreEqual( "(34<V> abc<W>)" + NewLine, found );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestTreeParserRewriteImaginary() /*throws Exception*/ {
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
                "tokens { ROOT; }\n" +
                "@members {\n" +
                "class V extends CommonTree {\n" +
                "  public V(int tokenType) { super(new CommonToken(tokenType)); }\n" +
                "  public String toString() { return tokenNames[token.getType()]+\"<V>\";}\n" +
                "}\n" +
                "}\n" +
                "a : ID -> ROOT<node=V> ID\n" +
                "  ;\n";

            string found = execTreeParser( "T.g", grammar, "TParser", "TP.g",
                        treeGrammar, "TP", "TLexer", "a", "a", "abc" );
            Assert.AreEqual( "ROOT<V> abc" + NewLine, found );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestTreeParserRewriteImaginaryWithArgs() /*throws Exception*/ {
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
                "tokens { ROOT; }\n" +
                "@members {\n" +
                "class V extends CommonTree {\n" +
                "  public int x;\n" +
                "  public V(int tokenType, int x) { super(new CommonToken(tokenType)); this.x=x;}\n" +
                "  public String toString() { return tokenNames[token.getType()]+\"<V>;\"+x;}\n" +
                "}\n" +
                "}\n" +
                "a : ID -> ROOT<node=V>[42] ID\n" +
                "  ;\n";

            string found = execTreeParser( "T.g", grammar, "TParser", "TP.g",
                        treeGrammar, "TP", "TLexer", "a", "a", "abc" );
            Assert.AreEqual( "ROOT<V>;42 abc" + NewLine, found );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestTreeParserRewriteImaginaryRoot() /*throws Exception*/ {
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
                "tokens { ROOT; }\n" +
                "@members {\n" +
                "class V extends CommonTree {\n" +
                "  public V(int tokenType) { super(new CommonToken(tokenType)); }\n" +
                "  public String toString() { return tokenNames[token.getType()]+\"<V>\";}\n" +
                "}\n" +
                "}\n" +
                "a : ID -> ^(ROOT<node=V> ID)\n" +
                "  ;\n";

            string found = execTreeParser( "T.g", grammar, "TParser", "TP.g",
                        treeGrammar, "TP", "TLexer", "a", "a", "abc" );
            Assert.AreEqual( "(ROOT<V> abc)" + NewLine, found );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestTreeParserRewriteImaginaryFromReal() /*throws Exception*/ {
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
                "tokens { ROOT; }\n" +
                "@members {\n" +
                "class V extends CommonTree {\n" +
                "  public V(int tokenType) { super(new CommonToken(tokenType)); }\n" +
                "  public V(int tokenType, Object tree) { super((CommonTree)tree); token.setType(tokenType); }\n" +
                "  public String toString() { return tokenNames[token.getType()]+\"<V>@\"+token.getLine();}\n" +
                "}\n" +
                "}\n" +
                "a : ID -> ROOT<node=V>[$ID]\n" +
                "  ;\n";

            string found = execTreeParser( "T.g", grammar, "TParser", "TP.g",
                        treeGrammar, "TP", "TLexer", "a", "a", "abc" );
            Assert.AreEqual( "ROOT<V>@1" + NewLine, found ); // at line 1; shows copy of ID's stuff
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestTreeParserAutoHeteroAST() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ID ';' ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";

            string treeGrammar =
                "tree grammar TP;\n" +
                "options {output=AST; ASTLabelType=CommonTree; tokenVocab=T;}\n" +
                "tokens { ROOT; }\n" +
                "@members {\n" +
                "class V extends CommonTree {\n" +
                "  public V(CommonTree t) { super(t); }\n" + // NEEDS SPECIAL CTOR
                "  public String toString() { return super.toString()+\"<V>\";}\n" +
                "}\n" +
                "}\n" +
                "a : ID<node=V> ';'<node=V>\n" +
                "  ;\n";

            string found = execTreeParser( "T.g", grammar, "TParser", "TP.g",
                        treeGrammar, "TP", "TLexer", "a", "a", "abc;" );
            Assert.AreEqual( "abc<V> ;<V>" + NewLine, found );
        }

    }
}
