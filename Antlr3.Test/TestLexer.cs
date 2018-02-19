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
    using Antlr.Runtime.JavaExtensions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using AntlrTool = Antlr3.AntlrTool;
    using CodeGenerator = Antlr3.Codegen.CodeGenerator;
    using Grammar = Antlr3.Tool.Grammar;
    using StringTemplate = Antlr4.StringTemplate.Template;

    [TestClass]
    [TestCategory(TestCategories.SkipOnCI)]
    public class TestLexer : BaseTest
    {
        protected bool debug = false;

        /** Public default constructor used by TestRig */
        public TestLexer()
        {
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestSetText() /*throws Exception*/ {
            // this must return A not I to the parser; calling a nonfragment rule
            // from a nonfragment rule does not set the overall token.
            string grammar =
                "grammar P;\n" +
                "a : A {System.out.println(input);} ;\n" +
                "A : '\\\\' 't' {setText(\"\t\");} ;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;";
            string found = execParser( "P.g", grammar, "PParser", "PLexer",
                        "a", "\\t", debug );
            Assert.AreEqual( "\t" + NewLine, found );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestRefToRuleDoesNotSetTokenNorEmitAnother() /*throws Exception*/ {
            // this must return A not I to the parser; calling a nonfragment rule
            // from a nonfragment rule does not set the overall token.
            string grammar =
                "grammar P;\n" +
                "a : A EOF {System.out.println(input);} ;\n" +
                "A : '-' I ;\n" +
                "I : '0'..'9'+ ;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;";
            string found = execParser( "P.g", grammar, "PParser", "PLexer",
                        "a", "-34", debug );
            Assert.AreEqual( "-34" + NewLine, found );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestRefToRuleDoesNotSetChannel() /*throws Exception*/ {
            // this must set channel of A to HIDDEN.  $channel is local to rule
            // like $type.
            string grammar =
                "grammar P;\n" +
                "a : A EOF {System.out.println($A.text+\", channel=\"+$A.channel);} ;\n" +
                "A : '-' WS I ;\n" +
                "I : '0'..'9'+ ;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;";
            string found = execParser( "P.g", grammar, "PParser", "PLexer",
                        "a", "- 34", debug );
            Assert.AreEqual( "- 34, channel=0" + NewLine, found );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestWeCanSetType() /*throws Exception*/ {
            string grammar =
                "grammar P;\n" +
                "tokens {X;}\n" +
                "a : X EOF {System.out.println(input);} ;\n" +
                "A : '-' I {$type = X;} ;\n" +
                "I : '0'..'9'+ ;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;";
            string found = execParser( "P.g", grammar, "PParser", "PLexer",
                        "a", "-34", debug );
            Assert.AreEqual( "-34" + NewLine, found );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestRefToFragment() /*throws Exception*/ {
            // this must return A not I to the parser; calling a nonfragment rule
            // from a nonfragment rule does not set the overall token.
            string grammar =
                "grammar P;\n" +
                "a : A {System.out.println(input);} ;\n" +
                "A : '-' I ;\n" +
                "fragment I : '0'..'9'+ ;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;";
            string found = execParser( "P.g", grammar, "PParser", "PLexer",
                        "a", "-34", debug );
            Assert.AreEqual( "-34" + NewLine, found );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestMultipleRefToFragment() /*throws Exception*/ {
            // this must return A not I to the parser; calling a nonfragment rule
            // from a nonfragment rule does not set the overall token.
            string grammar =
                "grammar P;\n" +
                "a : A EOF {System.out.println(input);} ;\n" +
                "A : I '.' I ;\n" +
                "fragment I : '0'..'9'+ ;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;";
            string found = execParser( "P.g", grammar, "PParser", "PLexer",
                        "a", "3.14159", debug );
            Assert.AreEqual( "3.14159" + NewLine, found );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestLabelInSubrule() /*throws Exception*/ {
            // can we see v outside?
            string grammar =
                "grammar P;\n" +
                "a : A EOF ;\n" +
                "A : 'hi' WS (v=I)? {$channel=0; System.out.println($v.text);} ;\n" +
                "fragment I : '0'..'9'+ ;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;";
            string found = execParser( "P.g", grammar, "PParser", "PLexer",
                        "a", "hi 342", debug );
            Assert.AreEqual( "342" + NewLine, found );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestRefToTokenInLexer() /*throws Exception*/ {
            string grammar =
                "grammar P;\n" +
                "a : A EOF ;\n" +
                "A : I {System.out.println($I.text);} ;\n" +
                "fragment I : '0'..'9'+ ;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;";
            string found = execParser( "P.g", grammar, "PParser", "PLexer",
                        "a", "342", debug );
            Assert.AreEqual( "342" + NewLine, found );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestListLabelInLexer() /*throws Exception*/ {
            string grammar =
                "grammar P;\n" +
                "a : A ;\n" +
                "A : i+=I+ {for (Object t : $i) System.out.print(\" \"+((Token)t).getText());} ;\n" +
                "fragment I : '0'..'9'+ ;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;";
            string found = execParser( "P.g", grammar, "PParser", "PLexer",
                        "a", "33 297", debug );
            Assert.AreEqual( " 33 297" + NewLine, found );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestDupListRefInLexer() /*throws Exception*/ {
            string grammar =
                "grammar P;\n" +
                "a : A ;\n" +
                "A : i+=I WS i+=I {$channel=0; for (Object t : $i) System.out.print(\" \"+((Token)t).getText());} ;\n" +
                "fragment I : '0'..'9'+ ;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;";
            string found = execParser( "P.g", grammar, "PParser", "PLexer",
                        "a", "33 297", debug );
            Assert.AreEqual( " 33 297" + NewLine, found );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestCharLabelInLexer()
        {
            string grammar =
                "grammar T;\n" +
                "a : B ;\n" +
                "B : x='a' {System.out.println((char)$x);} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                                      "a", "a", debug );
            Assert.AreEqual( "a" + NewLine, found );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestRepeatedLabelInLexer()
        {
            string grammar =
                "lexer grammar T;\n" +
                "B : x='a' x='b' ;\n";
            bool found =
                rawGenerateAndBuildRecognizer(
                    "T.g", grammar, null, "T", false );
            bool expecting = true; // should be ok
            Assert.AreEqual( expecting, found );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestRepeatedRuleLabelInLexer()
        {
            string grammar =
                "lexer grammar T;\n" +
                "B : x=A x=A ;\n" +
                "fragment A : 'a' ;\n";
            bool found =
                rawGenerateAndBuildRecognizer(
                    "T.g", grammar, null, "T", false );
            bool expecting = true; // should be ok
            Assert.AreEqual( expecting, found );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestIsolatedEOTEdge()
        {
            string grammar =
                "lexer grammar T;\n" +
                "QUOTED_CONTENT \n" +
                "        : 'q' (~'q')* (('x' 'q') )* 'q' ; \n";
            bool found =
                rawGenerateAndBuildRecognizer(
                    "T.g", grammar, null, "T", false );
            bool expecting = true; // should be ok
            Assert.AreEqual( expecting, found );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestEscapedLiterals()
        {
            /* Grammar:
                A : '\"' ;  should match a single double-quote: "
                B : '\\\"' ; should match input \"
            */
            string grammar =
                "lexer grammar T;\n" +
                "A : '\\\"' ;\n" +
                "B : '\\\\\\\"' ;\n"; // '\\\"'
            bool found =
                rawGenerateAndBuildRecognizer(
                    "T.g", grammar, null, "T", false );
            bool expecting = true; // should be ok
            Assert.AreEqual( expecting, found );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestNewlineLiterals() /*throws Exception*/
        {
            Grammar g = new Grammar(
                "lexer grammar T;\n" +
                "A : '\\n\\n' ;\n"  // ANTLR sees '\n\n'
            );
            string expecting = "match(\"\\n\\n\")";

            AntlrTool antlr = newTool();
            antlr.SetOutputDirectory( null ); // write to /dev/null
            CodeGenerator generator = new CodeGenerator( antlr, g, "Java" );
            g.CodeGenerator = generator;
            generator.GenRecognizer(); // codegen phase sets some vars we need
            StringTemplate codeST = generator.RecognizerST;
            string code = codeST.Render();
            int m = code.IndexOf( "match(\"" );
            string found = code.Substring( m, expecting.Length );

            Assert.AreEqual( expecting, found );
        }
    }
}
