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
    public class TestSemanticPredicateEvaluation : BaseTest
    {
        [TestMethod]
        public void TestSimpleCyclicDFAWithPredicate() /*throws Exception*/ {
            string grammar =
                "grammar foo;\n" +
                "a : {false}? 'x'* 'y' {System.out.println(\"alt1\");}\n" +
                "  | {true}?  'x'* 'y' {System.out.println(\"alt2\");}\n" +
                "  ;\n";
            string found = execParser( "foo.g", grammar, "fooParser", "fooLexer",
                        "a", "xxxy", false );
            assertEquals( "alt2" + NewLine, found );
        }

        [TestMethod]
        public void TestSimpleCyclicDFAWithInstanceVarPredicate() /*throws Exception*/ {
            string grammar =
                "grammar foo;\n" +
                "@members {boolean v=true;}\n" +
                "a : {false}? 'x'* 'y' {System.out.println(\"alt1\");}\n" +
                "  | {v}?     'x'* 'y' {System.out.println(\"alt2\");}\n" +
                "  ;\n";
            string found = execParser( "foo.g", grammar, "fooParser", "fooLexer",
                        "a", "xxxy", false );
            assertEquals( "alt2" + NewLine, found );
        }

        [TestMethod]
        public void TestPredicateValidation() /*throws Exception*/ {
            string grammar =
                "grammar foo;\n" +
                "@members {\n" +
                "public void reportError(RecognitionException e) {\n" +
                "    System.out.println(\"error: \"+e.toString());\n" +
                "}\n" +
                "}\n" +
                "\n" +
                "a : {false}? 'x'\n" +
                "  ;\n";
            string found = execParser( "foo.g", grammar, "fooParser", "fooLexer",
                        "a", "x", false );
            assertEquals( "error: FailedPredicateException(a,{false}?)" + NewLine, found );
        }

        [TestMethod]
        public void TestLexerPreds() /*throws Exception*/ {
            string grammar =
                "grammar foo;" +
                "@lexer::members {boolean p=false;}\n" +
                "a : (A|B)+ ;\n" +
                "A : {p}? 'a'  {System.out.println(\"token 1\");} ;\n" +
                "B : {!p}? 'a' {System.out.println(\"token 2\");} ;\n";
            string found = execParser( "foo.g", grammar, "fooParser", "fooLexer",
                        "a", "a", false );
            // "a" is ambig; can match both A, B.  Pred says match 2
            assertEquals( "token 2" + NewLine, found );
        }

        [TestMethod]
        public void TestLexerPreds2() /*throws Exception*/ {
            string grammar =
                "grammar foo;" +
                "@lexer::members {boolean p=true;}\n" +
                "a : (A|B)+ ;\n" +
                "A : {p}? 'a' {System.out.println(\"token 1\");} ;\n" +
                "B : ('a'|'b')+ {System.out.println(\"token 2\");} ;\n";
            string found = execParser( "foo.g", grammar, "fooParser", "fooLexer",
                        "a", "a", false );
            // "a" is ambig; can match both A, B.  Pred says match 1
            assertEquals( "token 1" + NewLine, found );
        }

        [TestMethod]
        public void TestLexerPredInExitBranch() /*throws Exception*/ {
            // p says it's ok to exit; it has precendence over the !p loopback branch
            string grammar =
                "grammar foo;" +
                "@lexer::members {boolean p=true;}\n" +
                "a : (A|B)+ ;\n" +
                "A : ('a' {System.out.print(\"1\");})*\n" +
                "    {p}?\n" +
                "    ('a' {System.out.print(\"2\");})* ;\n";
            string found = execParser( "foo.g", grammar, "fooParser", "fooLexer",
                        "a", "aaa", false );
            assertEquals( "222" + NewLine, found );
        }

        [TestMethod]
        public void TestLexerPredInExitBranch2() /*throws Exception*/ {
            string grammar =
                "grammar foo;" +
                "@lexer::members {boolean p=true;}\n" +
                "a : (A|B)+ ;\n" +
                "A : ({p}? 'a' {System.out.print(\"1\");})*\n" +
                "    ('a' {System.out.print(\"2\");})* ;\n";
            string found = execParser( "foo.g", grammar, "fooParser", "fooLexer",
                        "a", "aaa", false );
            assertEquals( "111" + NewLine, found );
        }

        [TestMethod]
        public void TestLexerPredInExitBranch3() /*throws Exception*/ {
            string grammar =
                "grammar foo;" +
                "@lexer::members {boolean p=true;}\n" +
                "a : (A|B)+ ;\n" +
                "A : ({p}? 'a' {System.out.print(\"1\");} | )\n" +
                "    ('a' {System.out.print(\"2\");})* ;\n";
            string found = execParser( "foo.g", grammar, "fooParser", "fooLexer",
                        "a", "aaa", false );
            assertEquals( "122" + NewLine, found );
        }

        [TestMethod]
        public void TestLexerPredInExitBranch4() /*throws Exception*/ {
            string grammar =
                "grammar foo;" +
                "a : (A|B)+ ;\n" +
                "A @init {int n=0;} : ({n<2}? 'a' {System.out.print(n++);})+\n" +
                "    ('a' {System.out.print(\"x\");})* ;\n";
            string found = execParser( "foo.g", grammar, "fooParser", "fooLexer",
                        "a", "aaaaa", false );
            assertEquals( "01xxx" + NewLine, found );
        }

        [TestMethod]
        public void TestLexerPredsInCyclicDFA() /*throws Exception*/ {
            string grammar =
                "grammar foo;" +
                "@lexer::members {boolean p=false;}\n" +
                "a : (A|B)+ ;\n" +
                "A : {p}? ('a')+ 'x'  {System.out.println(\"token 1\");} ;\n" +
                "B :      ('a')+ 'x' {System.out.println(\"token 2\");} ;\n";
            string found = execParser( "foo.g", grammar, "fooParser", "fooLexer",
                        "a", "aax", false );
            assertEquals( "token 2" + NewLine, found );
        }

        [TestMethod]
        public void TestLexerPredsInCyclicDFA2() /*throws Exception*/ {
            string grammar =
                "grammar foo;" +
                "@lexer::members {boolean p=false;}\n" +
                "a : (A|B)+ ;\n" +
                "A : {p}? ('a')+ 'x' ('y')? {System.out.println(\"token 1\");} ;\n" +
                "B :      ('a')+ 'x' {System.out.println(\"token 2\");} ;\n";
            string found = execParser( "foo.g", grammar, "fooParser", "fooLexer",
                        "a", "aax", false );
            assertEquals( "token 2" + NewLine, found );
        }

        [TestMethod]
        public void TestGatedPred() /*throws Exception*/ {
            string grammar =
                "grammar foo;" +
                "a : (A|B)+ ;\n" +
                "A : {true}?=> 'a' {System.out.println(\"token 1\");} ;\n" +
                "B : {false}?=>('a'|'b')+ {System.out.println(\"token 2\");} ;\n";
            string found = execParser( "foo.g", grammar, "fooParser", "fooLexer",
                        "a", "aa", false );
            // "a" is ambig; can match both A, B.  Pred says match A twice
            assertEquals( "token 1" + NewLine + "token 1" + NewLine, found );
        }

        [TestMethod]
        public void TestGatedPred2() /*throws Exception*/ {
            string grammar =
                "grammar foo;\n" +
                "@lexer::members {boolean sig=false;}\n" +
                "a : (A|B)+ ;\n" +
                "A : 'a' {System.out.print(\"A\"); sig=true;} ;\n" +
                "B : 'b' ;\n" +
                "C : {sig}?=> ('a'|'b') {System.out.print(\"C\");} ;\n";
            string found = execParser( "foo.g", grammar, "fooParser", "fooLexer",
                        "a", "aa", false );
            assertEquals( "AC" + NewLine, found );
        }

        [TestMethod]
        public void TestPredWithActionTranslation() /*throws Exception*/ {
            string grammar =
                "grammar foo;\n" +
                "a : b[2] ;\n" +
                "b[int i]\n" +
                "  : {$i==1}?   'a' {System.out.println(\"alt 1\");}\n" +
                "  | {$b.i==2}? 'a' {System.out.println(\"alt 2\");}\n" +
                "  ;\n";
            string found = execParser( "foo.g", grammar, "fooParser", "fooLexer",
                        "a", "aa", false );
            assertEquals( "alt 2" + NewLine, found );
        }

        [TestMethod]
        public void TestPredicatesOnEOTTarget() /*throws Exception*/ {
            string grammar =
                "grammar foo; \n" +
                "@lexer::members {boolean p=true, q=false;}" +
                "a : B ;\n" +
                "A: '</'; \n" +
                "B: {p}? '<!' {System.out.println(\"B\");};\n" +
                "C: {q}? '<' {System.out.println(\"C\");}; \n" +
                "D: '<';\n";
            string found = execParser( "foo.g", grammar, "fooParser", "fooLexer",
                        "a", "<!", false );
            assertEquals( "B" + NewLine, found );
        }

#if true // my lookahead tests
        [TestMethod]
        public void TestSynpredLookahead()
        {
            string grammar =
                "grammar foo;\n" +
                "a : B EOF {System.out.println(\"B\");};\n" +
                "B : '/*';\n" +
                "    ( ('/' ~'*') => '/'\n" +
                "    | ('*' ~'/') => '*'\n" +
                "    | B\n" +
                "    | (~('*'|'/')) => .\n" +
                "    )*\n" +
                "    '*/'\n" +
                "  ;"
                ;
            string found = execParser( "foo.g", grammar, "fooParser", "fooLexer", "a", "/* */", false );
            Assert.AreEqual( "B" + NewLine, found );
        }

        [TestMethod]
        public void TestPredicatesWithGlobalScope()
        {
            string grammar =
                "grammar foo;\n" +
                "scope S { boolean value; }\n" +
                "a scope S; @init{$S::value = true;} : {$S::value}? => B EOF {System.out.println(\"B\");};\n" +
                "B : 'a'..'z'+;\n";
            string found = execParser( "foo.g", grammar, "fooParser", "fooLexer", "a", "a", false );
            Assert.AreEqual( "B" + NewLine, found );
        }

        [TestMethod]
        public void TestPredicatesWithGlobalScope2()
        {
            string grammar =
                "grammar foo;\n" +
                "scope S { boolean value; }\n" +
                "a\n" +
                "  scope S;\n" +
                "  @init{$S::value = true;}\n" +
                "    : (b {$S::value}?) => b EOF\n" +
                "    | B EOF {System.out.println(\"A\");}\n" +
                "    ;\n" +
                "b\n" +
                "  scope S;\n" +
                "  @init{$S::value = false;}\n" +
                "    : B {!$S::value}? {System.out.println(\"B\");}\n" +
                "    ;\n" +
                "B : 'a'..'z'+;\n";
            string found = execParser( "foo.g", grammar, "fooParser", "fooLexer", "a", "a", false );
            Assert.AreEqual( "B" + NewLine, found );
        }

        [TestMethod]
        public void TestPredicatesWithGlobalScope3()
        {
            Assert.Inconclusive();
            string grammar =
                "grammar foo;\n" +
                "scope S { boolean value; }\n" +
                "a\n" +
                "  scope S;\n" +
                "  @init{$S::value = true;}\n" +
                "    : (b {$S::value}?) => b EOF\n" +
                "    | B EOF {System.out.println(\"A\");}\n" +
                "    ;\n" +
                "b\n" +
                "  scope S;\n" +
                "  @init{$S::value = false;}\n" +
                "    : {!$S::value}? B {System.out.println(\"B\");}\n" +
                "    ;\n" +
                "B : 'a'..'z'+;\n";
            string found = execParser( "foo.g", grammar, "fooParser", "fooLexer", "a", "a", false );
            Assert.AreEqual( "B" + NewLine, found );
        }
#endif

        // S U P P O R T

        public void _test() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a :  ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {channel=99;} ;\n";
            string found = execParser( "t.g", grammar, "T", "TLexer",
                        "a", "abc 34", false );
            assertEquals( "" + NewLine, found );
        }

    }
}
