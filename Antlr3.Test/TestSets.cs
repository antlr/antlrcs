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

    /** Test the set stuff in lexer and parser */
    [TestClass]
    public class TestSets : BaseTest
    {
        protected bool debug = false;

        /** Public default constructor used by TestRig */
        public TestSets()
        {
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestSeqDoesNotBecomeSet() /*throws Exception*/ {
            // this must return A not I to the parser; calling a nonfragment rule
            // from a nonfragment rule does not set the overall token.
            string grammar =
                "grammar P;\n" +
                "a : C {System.out.println(input);} ;\n" +
                "fragment A : '1' | '2';\n" +
                "fragment B : '3' '4';\n" +
                "C : A | B;\n";
            string found = execParser( "P.g", grammar, "PParser", "PLexer",
                                      "a", "34", debug );
            Assert.AreEqual( "34" + NewLine, found );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestParserSet() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "a : t=('x'|'y') {System.out.println($t.text);} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                                      "a", "x", debug );
            Assert.AreEqual( "x" + NewLine, found );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestParserNotSet() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "a : t=~('x'|'y') 'z' {System.out.println($t.text);} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                                      "a", "zz", debug );
            Assert.AreEqual( "z" + NewLine, found );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestParserNotToken() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "a : ~'x' 'z' {System.out.println(input);} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                                      "a", "zz", debug );
            Assert.AreEqual( "zz" + NewLine, found );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestParserNotTokenWithLabel() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "a : t=~'x' 'z' {System.out.println($t.text);} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                                      "a", "zz", debug );
            Assert.AreEqual( "z" + NewLine, found );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestRuleAsSet() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "a @after {System.out.println(input);} : 'a' | 'b' |'c' ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                                      "a", "b", debug );
            Assert.AreEqual( "b" + NewLine, found );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestRuleAsSetAST() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : 'a' | 'b' |'c' ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                                      "a", "b", debug );
            Assert.AreEqual( "b" + NewLine, found );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestNotChar() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "a : A {System.out.println($A.text);} ;\n" +
                "A : ~'b' ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                                      "a", "x", debug );
            Assert.AreEqual( "x" + NewLine, found );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestOptionalSingleElement() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "a : A? 'c' {System.out.println(input);} ;\n" +
                "A : 'b' ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                                      "a", "bc", debug );
            Assert.AreEqual( "bc" + NewLine, found );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestOptionalLexerSingleElement() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "a : A {System.out.println(input);} ;\n" +
                "A : 'b'? 'c' ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                                      "a", "bc", debug );
            Assert.AreEqual( "bc" + NewLine, found );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestStarLexerSingleElement() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "a : A {System.out.println(input);} ;\n" +
                "A : 'b'* 'c' ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                                      "a", "bbbbc", debug );
            Assert.AreEqual( "bbbbc" + NewLine, found );
            found = execParser( "T.g", grammar, "TParser", "TLexer",
                                      "a", "c", debug );
            Assert.AreEqual( "c" + NewLine, found );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestPlusLexerSingleElement() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "a : A {System.out.println(input);} ;\n" +
                "A : 'b'+ 'c' ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                                      "a", "bbbbc", debug );
            Assert.AreEqual( "bbbbc" + NewLine, found );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestOptionalSet() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "a : ('a'|'b')? 'c' {System.out.println(input);} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                                      "a", "ac", debug );
            Assert.AreEqual( "ac" + NewLine, found );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestStarSet() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "a : ('a'|'b')* 'c' {System.out.println(input);} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                                      "a", "abaac", debug );
            Assert.AreEqual( "abaac" + NewLine, found );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestPlusSet() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "a : ('a'|'b')+ 'c' {System.out.println(input);} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                                      "a", "abaac", debug );
            Assert.AreEqual( "abaac" + NewLine, found );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestLexerOptionalSet() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "a : A {System.out.println(input);} ;\n" +
                "A : ('a'|'b')? 'c' ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                                      "a", "ac", debug );
            Assert.AreEqual( "ac" + NewLine, found );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestLexerStarSet() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "a : A {System.out.println(input);} ;\n" +
                "A : ('a'|'b')* 'c' ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                                      "a", "abaac", debug );
            Assert.AreEqual( "abaac" + NewLine, found );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestLexerPlusSet() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "a : A {System.out.println(input);} ;\n" +
                "A : ('a'|'b')+ 'c' ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                                      "a", "abaac", debug );
            Assert.AreEqual( "abaac" + NewLine, found );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestNotCharSet() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "a : A {System.out.println($A.text);} ;\n" +
                "A : ~('b'|'c') ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                                      "a", "x", debug );
            Assert.AreEqual( "x" + NewLine, found );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestNotCharSetWithLabel() /*throws Exception*/ {
            // This doesn't work in lexer yet.
            // Generates: h=input.LA(1); but h is defined as a Token
            string grammar =
                "grammar T;\n" +
                "a : A {System.out.println($A.text);} ;\n" +
                "A : h=~('b'|'c') ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                                      "a", "x", debug );
            Assert.AreEqual( "x" + NewLine, found );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestNotCharSetWithRuleRef() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "a : A {System.out.println($A.text);} ;\n" +
                "A : ~('a'|B) ;\n" +
                "B : 'b' ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                                      "a", "x", debug );
            Assert.AreEqual( "x" + NewLine, found );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestNotCharSetWithRuleRef2() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "a : A {System.out.println($A.text);} ;\n" +
                "A : ~('a'|B) ;\n" +
                "B : 'b'|'c' ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                                      "a", "x", debug );
            Assert.AreEqual( "x" + NewLine, found );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestNotCharSetWithRuleRef3() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "a : A {System.out.println($A.text);} ;\n" +
                "A : ('a'|B) ;\n" +
                "fragment\n" +
                "B : ~('a'|'c') ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                                      "a", "x", debug );
            Assert.AreEqual( "x" + NewLine, found );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestNotCharSetWithRuleRef4() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "a : A {System.out.println($A.text);} ;\n" +
                "A : ('a'|B) ;\n" +
                "fragment\n" +
                "B : ~('a'|C) ;\n" +
                "fragment\n" +
                "C : 'c'|'d' ;\n ";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                                      "a", "x", debug );
            Assert.AreEqual( "x" + NewLine, found );
        }

    }
}
