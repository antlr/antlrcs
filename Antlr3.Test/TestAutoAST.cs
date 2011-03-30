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
    public class TestAutoAST : BaseTest
    {
        protected bool debug = false;

        [TestMethod]
        public void TestTokenList() /*throws Exception*/ {
            string grammar =
                "grammar foo;\n" +
                "options {output=AST;}\n" +
                "a : ID INT ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "foo.g", grammar, "fooParser", "fooLexer",
                                      "a", "abc 34", debug );
            Assert.AreEqual( "abc 34" + NewLine, found );
        }

        [TestMethod]
        public void TestTokenListInSingleAltBlock() /*throws Exception*/ {
            string grammar =
                "grammar foo;\n" +
                "options {output=AST;}\n" +
                "a : (ID INT) ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "foo.g", grammar, "fooParser", "fooLexer",
                                      "a", "abc 34", debug );
            Assert.AreEqual( "abc 34" + NewLine, found );
        }

        [TestMethod]
        public void TestSimpleRootAtOuterLevel() /*throws Exception*/ {
            string grammar =
                "grammar foo;\n" +
                "options {output=AST;}\n" +
                "a : ID^ INT ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "foo.g", grammar, "fooParser", "fooLexer",
                                      "a", "abc 34", debug );
            Assert.AreEqual( "(abc 34)" + NewLine, found );
        }

        [TestMethod]
        public void TestSimpleRootAtOuterLevelReverse() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : INT ID^ ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                                      "a", "34 abc", debug );
            Assert.AreEqual( "(abc 34)" + NewLine, found );
        }

        [TestMethod]
        public void TestBang() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ID INT! ID! INT ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                                      "a", "abc 34 dag 4532", debug );
            Assert.AreEqual( "abc 4532" + NewLine, found );
        }

        [TestMethod]
        public void TestOptionalThenRoot() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ( ID INT )? ID^ ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                                      "a", "a 1 b", debug );
            Assert.AreEqual( "(b a 1)" + NewLine, found );
        }

        [TestMethod]
        public void TestLabeledStringRoot() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : v='void'^ ID ';' ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                                      "a", "void foo;", debug );
            Assert.AreEqual( "(void foo ;)" + NewLine, found );
        }

        [TestMethod]
        public void TestWildcard() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : v='void'^ . ';' ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                                      "a", "void foo;", debug );
            Assert.AreEqual( "(void foo ;)" + NewLine, found );
        }

        [TestMethod]
        public void TestWildcardRoot() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : v='void' .^ ';' ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                                      "a", "void foo;", debug );
            Assert.AreEqual( "(foo void ;)" + NewLine, found );
        }

        [TestMethod]
        public void TestWildcardRootWithLabel() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : v='void' x=.^ ';' ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                                      "a", "void foo;", debug );
            Assert.AreEqual( "(foo void ;)" + NewLine, found );
        }

        [TestMethod]
        public void TestWildcardRootWithListLabel() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : v='void' x=.^ ';' ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                                      "a", "void foo;", debug );
            Assert.AreEqual( "(foo void ;)" + NewLine, found );
        }

        [TestMethod]
        public void TestWildcardBangWithListLabel() /*throws Exception*/
        {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : v='void' x=.! ';' ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                                      "a", "void foo;", debug );
            Assert.AreEqual( "void ;" + NewLine, found );
        }

        [TestMethod]
        public void TestRootRoot() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ID^ INT^ ID ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                                      "a", "a 34 c", debug );
            Assert.AreEqual( "(34 a c)" + NewLine, found );
        }

        [TestMethod]
        public void TestRootRoot2() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ID INT^ ID^ ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                                      "a", "a 34 c", debug );
            Assert.AreEqual( "(c (34 a))" + NewLine, found );
        }

        [TestMethod]
        public void TestRootThenRootInLoop() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ID^ (INT '*'^ ID)+ ;\n" +
                "ID  : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                                      "a", "a 34 * b 9 * c", debug );
            Assert.AreEqual( "(* (* (a 34) b 9) c)" + NewLine, found );
        }

        [TestMethod]
        public void TestNestedSubrule() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : 'void' (({;}ID|INT) ID | 'null' ) ';' ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                                      "a", "void a b;", debug );
            Assert.AreEqual( "void a b ;" + NewLine, found );
        }

        [TestMethod]
        public void TestInvokeRule() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a  : type ID ;\n" +
                "type : {;}'int' | 'float' ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                                      "a", "int a", debug );
            Assert.AreEqual( "int a" + NewLine, found );
        }

        [TestMethod]
        public void TestInvokeRuleAsRoot() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a  : type^ ID ;\n" +
                "type : {;}'int' | 'float' ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                                      "a", "int a", debug );
            Assert.AreEqual( "(int a)" + NewLine, found );
        }

        [TestMethod]
        public void TestInvokeRuleAsRootWithLabel() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a  : x=type^ ID ;\n" +
                "type : {;}'int' | 'float' ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                                      "a", "int a", debug );
            Assert.AreEqual( "(int a)" + NewLine, found );
        }

        [TestMethod]
        public void TestInvokeRuleAsRootWithListLabel() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a  : x+=type^ ID ;\n" +
                "type : {;}'int' | 'float' ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                                      "a", "int a", debug );
            Assert.AreEqual( "(int a)" + NewLine, found );
        }

        [TestMethod]
        public void TestRuleRootInLoop() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ID ('+'^ ID)* ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                                      "a", "a+b+c+d", debug );
            Assert.AreEqual( "(+ (+ (+ a b) c) d)" + NewLine, found );
        }

        [TestMethod]
        public void TestRuleInvocationRuleRootInLoop() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ID (op^ ID)* ;\n" +
                "op : {;}'+' | '-' ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                                      "a", "a+b+c-d", debug );
            Assert.AreEqual( "(- (+ (+ a b) c) d)" + NewLine, found );
        }

        [TestMethod]
        public void TestTailRecursion() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "s : a ;\n" +
                "a : atom ('exp'^ a)? ;\n" +
                "atom : INT ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                                      "s", "3 exp 4 exp 5", debug );
            Assert.AreEqual( "(exp 3 (exp 4 5))" + NewLine, found );
        }

        [TestMethod]
        public void TestSet() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ID|INT ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                                      "a", "abc", debug );
            Assert.AreEqual( "abc" + NewLine, found );
        }

        [TestMethod]
        public void TestSetRoot() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ('+' | '-')^ ID ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                                      "a", "+abc", debug );
            Assert.AreEqual( "(+ abc)" + NewLine, found );
        }

        [TestMethod]
        public void TestSetRootWithLabel() /*throws Exception*/ {
            // FAILS until I rebuild the antlr.g in v3 (ROOT can't follow a block after an ID assign)
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : x=('+' | '-')^ ID ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                                      "a", "+abc", debug );
            Assert.AreEqual( "(+ abc)" + NewLine, found );
        }

        [TestMethod]
        public void TestSetAsRuleRootInLoop() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ID (('+'|'-')^ ID)* ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                                      "a", "a+b-c", debug );
            Assert.AreEqual( "(- (+ a b) c)" + NewLine, found );
        }

        [TestMethod]
        public void TestNotSet() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ~ID '+' INT ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                                      "a", "34+2", debug );
            Assert.AreEqual( "34 + 2" + NewLine, found );
        }

        [TestMethod]
        public void TestNotSetWithLabel() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : x=~ID '+' INT ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                                      "a", "34+2", debug );
            Assert.AreEqual( "34 + 2" + NewLine, found );
        }

        [TestMethod]
        public void TestNotSetWithListLabel() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : x=~ID '+' INT ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                                      "a", "34+2", debug );
            Assert.AreEqual( "34 + 2" + NewLine, found );
        }

        [TestMethod]
        public void TestNotSetRoot() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ~'+'^ INT ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                                      "a", "34 55", debug );
            Assert.AreEqual( "(34 55)" + NewLine, found );
        }

        [TestMethod]
        public void TestNotSetRootWithLabel() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ~'+'^ INT ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                                      "a", "34 55", debug );
            Assert.AreEqual( "(34 55)" + NewLine, found );
        }

        [TestMethod]
        public void TestNotSetRootWithListLabel() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ~'+'^ INT ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                                      "a", "34 55", debug );
            Assert.AreEqual( "(34 55)" + NewLine, found );
        }

        [TestMethod]
        public void TestNotSetRuleRootInLoop() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : INT (~INT^ INT)* ;\n" +
                "blort : '+' ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                                      "a", "3+4+5", debug );
            Assert.AreEqual( "(+ (+ 3 4) 5)" + NewLine, found );
        }

        [TestMethod]
        public void TestTokenLabelReuse() /*throws Exception*/ {
            // check for compilation problem due to multiple defines
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : id=ID id=ID {System.out.print(\"2nd id=\"+$id.text+';');} ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                                      "a", "a b", debug );
            Assert.AreEqual( "2nd id=b;a b" + NewLine, found );
        }

        [TestMethod]
        public void TestTokenLabelReuse2() /*throws Exception*/ {
            // check for compilation problem due to multiple defines
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : id=ID id=ID^ {System.out.print(\"2nd id=\"+$id.text+';');} ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                                      "a", "a b", debug );
            Assert.AreEqual( "2nd id=b;(b a)" + NewLine, found );
        }

        [TestMethod]
        public void TestTokenListLabelReuse() /*throws Exception*/ {
            // check for compilation problem due to multiple defines
            // make sure ids has both ID tokens
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ids+=ID ids+=ID {System.out.print(\"id list=\"+$ids+';');} ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                                      "a", "a b", debug );
            string expecting = "id list=[[@0,0:0='a',<4>,1:0], [@2,2:2='b',<4>,1:2]];a b" + NewLine;
            Assert.AreEqual( expecting, found );
        }

        [TestMethod]
        public void TestTokenListLabelReuse2() /*throws Exception*/ {
            // check for compilation problem due to multiple defines
            // make sure ids has both ID tokens
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ids+=ID^ ids+=ID {System.out.print(\"id list=\"+$ids+';');} ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                                      "a", "a b", debug );
            string expecting = "id list=[[@0,0:0='a',<4>,1:0], [@2,2:2='b',<4>,1:2]];(a b)" + NewLine;
            Assert.AreEqual( expecting, found );
        }

        [TestMethod]
        public void TestTokenListLabelRuleRoot() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : id+=ID^ ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                                      "a", "a", debug );
            Assert.AreEqual( "a" + NewLine, found );
        }

        [TestMethod]
        public void TestTokenListLabelBang() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : id+=ID! ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                                      "a", "a", debug );
            Assert.AreEqual( "", found );
        }

        [TestMethod]
        public void TestRuleListLabel() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : x+=b x+=b {" +
                "Tree t=(Tree)$x.get(1);" +
                "System.out.print(\"2nd x=\"+t.toStringTree()+';');} ;\n" +
                "b : ID;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                                      "a", "a b", debug );
            Assert.AreEqual( "2nd x=b;a b" + NewLine, found );
        }

        [TestMethod]
        public void TestRuleListLabelRuleRoot() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ( x+=b^ )+ {" +
                "System.out.print(\"x=\"+((CommonTree)$x.get(1)).toStringTree()+';');} ;\n" +
                "b : ID;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                                      "a", "a b", debug );
            Assert.AreEqual( "x=(b a);(b a)" + NewLine, found );
        }

        [TestMethod]
        public void TestRuleListLabelBang() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : x+=b! x+=b {" +
                "System.out.print(\"1st x=\"+((CommonTree)$x.get(0)).toStringTree()+';');} ;\n" +
                "b : ID;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                                      "a", "a b", debug );
            Assert.AreEqual( "1st x=a;b" + NewLine, found );
        }

        [TestMethod]
        public void TestComplicatedMelange() /*throws Exception*/ {
            // check for compilation problem
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : A b=B b=B c+=C c+=C D {String s = $D.text;} ;\n" +
                "A : 'a' ;\n" +
                "B : 'b' ;\n" +
                "C : 'c' ;\n" +
                "D : 'd' ;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                                      "a", "a b b c c d", debug );
            Assert.AreEqual( "a b b c c d" + NewLine, found );
        }

        [TestMethod]
        public void TestReturnValueWithAST() /*throws Exception*/ {
            string grammar =
                "grammar foo;\n" +
                "options {output=AST;}\n" +
                "a : ID b {System.out.println($b.i);} ;\n" +
                "b returns [int i] : INT {$i=Integer.parseInt($INT.text);} ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "foo.g", grammar, "fooParser", "fooLexer",
                                      "a", "abc 34", debug );
            Assert.AreEqual( "34" + NewLine + "abc 34" + NewLine, found );
        }

        [TestMethod]
        public void TestSetLoop() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options { output=AST; }\n" +
                "r : (INT|ID)+ ; \n" +
                "ID : 'a'..'z' + ;\n" +
                "INT : '0'..'9' +;\n" +
                "WS: (' ' | '\\n' | '\\t')+ {$channel = HIDDEN;};\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                                      "r", "abc 34 d", debug );
            Assert.AreEqual( "abc 34 d" + NewLine, found );
        }

        [TestMethod]
        public void TestExtraTokenInSimpleDecl() /*throws Exception*/ {
            string grammar =
                "grammar foo;\n" +
                "options {output=AST;}\n" +
                "decl : type^ ID '='! INT ';'! ;\n" +
                "type : 'int' | 'float' ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "foo.g", grammar, "fooParser", "fooLexer",
                                      "decl", "int 34 x=1;", debug );
            Assert.AreEqual( "line 1:4 extraneous input '34' expecting ID" + NewLine, this.stderrDuringParse );
            Assert.AreEqual( "(int x 1)" + NewLine, found ); // tree gets correct x and 1 tokens
        }

        [TestMethod]
        public void TestMissingIDInSimpleDecl() /*throws Exception*/ {
            string grammar =
                "grammar foo;\n" +
                "options {output=AST;}\n" +
                "tokens {EXPR;}\n" +
                "decl : type^ ID '='! INT ';'! ;\n" +
                "type : 'int' | 'float' ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "foo.g", grammar, "fooParser", "fooLexer",
                                      "decl", "int =1;", debug );
            Assert.AreEqual( "line 1:4 missing ID at '='" + NewLine, this.stderrDuringParse );
            Assert.AreEqual( "(int <missing ID> 1)" + NewLine, found ); // tree gets invented ID token
        }

        [TestMethod]
        public void TestMissingSetInSimpleDecl() /*throws Exception*/ {
            string grammar =
                "grammar foo;\n" +
                "options {output=AST;}\n" +
                "tokens {EXPR;}\n" +
                "decl : type^ ID '='! INT ';'! ;\n" +
                "type : 'int' | 'float' ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "foo.g", grammar, "fooParser", "fooLexer",
                                      "decl", "x=1;", debug );
            Assert.AreEqual( "line 1:0 mismatched input 'x' expecting set null" + NewLine, this.stderrDuringParse );
            Assert.AreEqual( "(<error: x> x 1)" + NewLine, found ); // tree gets invented ID token
        }

        [TestMethod]
        public void TestMissingTokenGivesErrorNode() /*throws Exception*/ {
            string grammar =
                "grammar foo;\n" +
                "options {output=AST;}\n" +
                "a : ID INT ;\n" + // follow is EOF
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "foo.g", grammar, "fooParser", "fooLexer",
                                      "a", "abc", debug );
            Assert.AreEqual( "line 1:3 missing INT at '<EOF>'" + NewLine, this.stderrDuringParse );
            Assert.AreEqual( "abc <missing INT>" + NewLine, found );
        }

        [TestMethod]
        public void TestMissingTokenGivesErrorNodeInInvokedRule() /*throws Exception*/ {
            string grammar =
                "grammar foo;\n" +
                "options {output=AST;}\n" +
                "a : b ;\n" +
                "b : ID INT ;\n" + // follow should see EOF
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "foo.g", grammar, "fooParser", "fooLexer",
                                      "a", "abc", debug );
            Assert.AreEqual( "line 1:3 mismatched input '<EOF>' expecting INT" + NewLine, this.stderrDuringParse );
            Assert.AreEqual( "<mismatched token: [@1,3:3='<EOF>',<-1>,1:3], resync=abc>" + NewLine, found );
        }

        [TestMethod]
        public void TestExtraTokenGivesErrorNode() /*throws Exception*/ {
            string grammar =
                "grammar foo;\n" +
                "options {output=AST;}\n" +
                "a : b c ;\n" +
                "b : ID ;\n" +
                "c : INT ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "foo.g", grammar, "fooParser", "fooLexer",
                                      "a", "abc ick 34", debug );
            Assert.AreEqual( "line 1:4 extraneous input 'ick' expecting INT" + NewLine, this.stderrDuringParse );
            Assert.AreEqual( "abc 34" + NewLine, found );
        }

        [TestMethod]
        public void TestMissingFirstTokenGivesErrorNode() /*throws Exception*/ {
            string grammar =
                "grammar foo;\n" +
                "options {output=AST;}\n" +
                "a : ID INT ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "foo.g", grammar, "fooParser", "fooLexer",
                                      "a", "34", debug );
            Assert.AreEqual( "line 1:0 missing ID at '34'" + NewLine, this.stderrDuringParse );
            Assert.AreEqual( "<missing ID> 34" + NewLine, found );
        }

        [TestMethod]
        public void TestMissingFirstTokenGivesErrorNode2() /*throws Exception*/ {
            string grammar =
                "grammar foo;\n" +
                "options {output=AST;}\n" +
                "a : b c ;\n" +
                "b : ID ;\n" +
                "c : INT ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "foo.g", grammar, "fooParser", "fooLexer",
                                      "a", "34", debug );
            // finds an error at the first token, 34, and re-syncs.
            // re-synchronizing does not consume a token because 34 follows
            // ref to rule b (start of c). It then matches 34 in c.
            Assert.AreEqual( "line 1:0 missing ID at '34'" + NewLine, this.stderrDuringParse );
            Assert.AreEqual( "<missing ID> 34" + NewLine, found );
        }

        [TestMethod]
        public void TestNoViableAltGivesErrorNode() /*throws Exception*/ {
            string grammar =
                "grammar foo;\n" +
                "options {output=AST;}\n" +
                "a : b | c ;\n" +
                "b : ID ;\n" +
                "c : INT ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "S : '*' ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "foo.g", grammar, "fooParser", "fooLexer",
                                      "a", "*", debug );
            Assert.AreEqual( "line 1:0 no viable alternative at input '*'" + NewLine, this.stderrDuringParse );
            Assert.AreEqual( "<unexpected: [@0,0:0='*',<6>,1:0], resync=*>" + NewLine, found );
        }


        // S U P P O R T

        public void _test() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a :  ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer", "a", "abc 34", debug );
            Assert.AreEqual( NewLine, found );
        }

    }
}
