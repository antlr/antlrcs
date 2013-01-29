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

    /** General code generation testing; compilation and/or execution.
     *  These tests are more about avoiding duplicate var definitions
     *  etc... than testing a particular ANTLR feature.
     */
    [TestClass]
    public class TestJavaCodeGeneration : BaseTest
    {
        [TestMethod]
        public void TestDupVarDefForPinchedState()
        {
            // so->s2 and s0->s3->s1 pinches back to s1
            // LA3_1, s1 state for DFA 3, was defined twice in similar scope
            // just wrapped in curlies and it's cool.
            string grammar =
                "grammar T;\n" +
                "a : (| A | B) X Y\n" +
                "  | (| A | B) X Z\n" +
                "  ;\n";
            bool found =
                rawGenerateAndBuildRecognizer(
                    "T.g", grammar, "TParser", null, false );
            bool expecting = true; // should be ok
            Assert.AreEqual( expecting, found );
        }

        [TestMethod]
        public void TestLabeledNotSetsInLexer()
        {
            // d must be an int
            string grammar =
                "lexer grammar T;\n" +
                "A : d=~('x'|'y') e='0'..'9'\n" +
                "  ; \n";
            bool found =
                rawGenerateAndBuildRecognizer(
                    "T.g", grammar, null, "T", false );
            bool expecting = true; // should be ok
            Assert.AreEqual( expecting, found );
        }

        [TestMethod]
        public void TestLabeledSetsInLexer()
        {
            // d must be an int
            string grammar =
                "grammar T;\n" +
                "a : A ;\n" +
                "A : d=('x'|'y') {System.out.println((char)$d);}\n" +
                "  ; \n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                                      "a", "x", false );
            Assert.AreEqual( "x" + NewLine, found );
        }

        [TestMethod]
        public void TestLabeledRangeInLexer()
        {
            // d must be an int
            string grammar =
                "grammar T;\n" +
                "a : A;\n" +
                "A : d='a'..'z' {System.out.println((char)$d);} \n" +
                "  ; \n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                                      "a", "x", false );
            Assert.AreEqual( "x" + NewLine, found );
        }

        [TestMethod]
        public void TestLabeledWildcardInLexer()
        {
            // d must be an int
            string grammar =
                "grammar T;\n" +
                "a : A;\n" +
                "A : d=. {System.out.println((char)$d);}\n" +
                "  ; \n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                                      "a", "x", false );
            Assert.AreEqual( "x" + NewLine, found );
        }

        [TestMethod]
        public void TestSynpredWithPlusLoop()
        {
            string grammar =
                "grammar T; \n" +
                "a : (('x'+)=> 'x'+)?;\n";
            bool found =
                rawGenerateAndBuildRecognizer(
                    "T.g", grammar, "TParser", "TLexer", false );
            bool expecting = true; // should be ok
            Assert.AreEqual( expecting, found );
        }

        [TestMethod]
        public void TestDoubleQuoteEscape()
        {
            string grammar =
                "lexer grammar T; \n" +
                "A : '\\\\\"';\n" +          // this is A : '\\"', which should give "\\\"" at Java level;
                "B : '\\\"';\n" +            // this is B: '\"', which shodl give "\"" at Java level;
                "C : '\\'\\'';\n" +          // this is C: '\'\'', which shoudl give "''" at Java level
                "D : '\\k';\n";              // this is D: '\k', which shoudl give just "k" at Java level;

            bool found =
                rawGenerateAndBuildRecognizer(
                    "T.g", grammar, null, "T", false );
            bool expecting = true; // should be ok
            Assert.AreEqual( expecting, found );
        }

        [TestMethod]
        public void TestUserExceptionInParser()
        {
            string grammar =
                "grammar T;\n" +
                "@parser::header {import java.io.IOException;}" +
                "a throws IOException : 'x' {throw new java.io.IOException();};\n";

            bool found = rawGenerateAndBuildRecognizer( "T.g", grammar, "TParser", "TLexer", false );
            bool expecting = true;
            Assert.AreEqual( expecting, found );
        }

        [TestMethod]
        public void TestBlankRuleGetsNoException()
        {
            string grammar =
                "grammar T;\n" +
                "a : sync (ID sync)* ;\n" +
                "sync : ;\n" +
                "ID : 'a'..'z'+;\n";
            bool found =
                rawGenerateAndBuildRecognizer(
                    "T.g", grammar, "TParser", "TLexer", false);
            bool expecting = true; // should be ok
            Assert.AreEqual(expecting, found);
        }

        /**
         * This is a regression test for antlr/antlr3#20: StackOverflow error when
         * compiling grammar with backtracking.
         * https://github.com/antlr/antlr3/issues/20
         */
        [TestMethod]
        public void TestSemanticPredicateAnalysisStackOverflow()
        {
            string grammar =
                "grammar T;\n"
                + "\n"
                + "options {\n"
                + "  backtrack=true;\n"
                + "}\n"
                + "\n"
                + "main : ('x'*)*;\n";
            bool success = rawGenerateAndBuildRecognizer("T.g", grammar, "TParser", "TLexer", false);
            Assert.IsTrue(success);
        }
    }
}
