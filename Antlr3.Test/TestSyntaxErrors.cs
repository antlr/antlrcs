/*
 * [The "BSD licence"]
 * Copyright (c) 2011 Terence Parr
 * All rights reserved.
 *
 * Conversion to C#:
 * Copyright (c) 2011 Sam Harwell, Pixel Mine, Inc.
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
    using Console = System.Console;
    using ErrorManager = Antlr3.Tool.ErrorManager;
    using Path = System.IO.Path;
    using Regex = System.Text.RegularExpressions.Regex;

    [TestClass]
    [TestCategory(TestCategories.SkipOnCI)]
    public class TestSyntaxErrors : BaseTest
    {
        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestLL2()
        {
            string grammar =
                "grammar T;\n" +
                "a : 'a' 'b'" +
                "  | 'a' 'c'" +
                ";\n" +
                "q : 'e' ;\n";
            string found = execParser("T.g", grammar, "TParser", "TLexer", "a", "ae", false);
            string expecting = "line 1:1 no viable alternative at input 'e'" + NewLine;
            string result = Regex.Replace(stderrDuringParse, ".*?/input ", "input ");
            Assert.AreEqual(expecting, result);
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestLL3()
        {
            string grammar =
                "grammar T;\n" +
                "a : 'a' 'b'* 'c'" +
                "  | 'a' 'b' 'd'" +
                "  ;\n" +
                "q : 'e' ;\n";
            Console.WriteLine(grammar);
            string found = execParser("T.g", grammar, "TParser", "TLexer", "a", "abe", false);
            string expecting = "line 1:2 no viable alternative at input 'e'" + NewLine;
            string result = Regex.Replace(stderrDuringParse, ".*?/input ", "input ");
            Assert.AreEqual(expecting, result);
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestLLStar()
        {
            string grammar =
                "grammar T;\n" +
                "a : 'a'+ 'b'" +
                "  | 'a'+ 'c'" +
                ";\n" +
                "q : 'e' ;\n";
            string found = execParser("T.g", grammar, "TParser", "TLexer", "a", "aaae", false);
            string expecting = "line 1:3 no viable alternative at input 'e'" + NewLine;
            string result = Regex.Replace(stderrDuringParse, ".*?/input ", "input ");
            Assert.AreEqual(expecting, result);
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestSynPred()
        {
            string grammar =
                "grammar T;\n" +
                "a : (e '.')=> e '.'" +
                "  | (e ';')=> e ';'" +
                "  | 'z'" +
                "  ;\n" +
                "e : '(' e ')'" +
                "  | 'i'" +
                "  ;\n";
            Console.WriteLine(grammar);
            string found = execParser("T.g", grammar, "TParser", "TLexer", "a", "((i))z", false);
            string expecting = "line 1:1 no viable alternative at input '('" + NewLine;
            string result = Regex.Replace(stderrDuringParse, ".*?/input ", "input ");
            Assert.AreEqual(expecting, result);
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestLL1ErrorInfo()
        {
            string grammar =
                "grammar T;\n" +
                "start : animal (AND acClass)? service EOF;\n" +
                "animal : (DOG | CAT );\n" +
                "service : (HARDWARE | SOFTWARE) ;\n" +
                "AND : 'and';\n" +
                "DOG : 'dog';\n" +
                "CAT : 'cat';\n" +
                "HARDWARE: 'hardware';\n" +
                "SOFTWARE: 'software';\n" +
                "WS : ' ' {skip();} ;" +
                "acClass\n" +
                "@init\n" +
                "{ System.out.println(computeContextSensitiveRuleFOLLOW().toString(tokenNames)); }\n" +
                "  : ;\n";
            string result = execParser("T.g", grammar, "TParser", "TLexer", "start", "dog and software", false);
            string expecting = "{HARDWARE,SOFTWARE}" + NewLine;
            Assert.AreEqual(expecting, result);
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestStrayBracketRecovery()
        {
            string grammar =
                "grammar T;\n" +
                "options {output = AST;}\n" +
                "tokens{NODE;}\n" +
                "s : a=ID INT -> ^(NODE[$a]] INT);\n" +
                "ID: 'a'..'z'+;\n" +
                "INT: '0'..'9'+;\n";

            ErrorQueue errorQueue = new ErrorQueue();
            ErrorManager.SetErrorListener(errorQueue);

            bool found =
                rawGenerateAndBuildRecognizer(
                    "T.g", grammar, "TParser", "TLexer", false);

            Assert.IsFalse(found);
            Assert.AreEqual(
                "[error(100): :4:27: syntax error: antlr: dangling ']'? make sure to escape with \\]]",
                '[' + string.Join(", ", errorQueue.errors) + ']');
        }

        /**
         * This is a regression test for antlr/antlr3#61.
         * https://github.com/antlr/antlr3/issues/61
         */
        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestMissingAttributeAccessPreventsCodeGeneration()
        {
            string grammar =
                "grammar T;\n" +
                "options {\n" +
                "    backtrack = true; \n" +
                "}\n" +
                "// if b is rule ref, gens bad void x=null code\n" +
                "a : x=b {Object o = $x; System.out.println(\"alt1\");}\n" +
                "  | y=b\n" +
                "  ;\n" +
                "\n" +
                "b : 'a' ;\n";

            ErrorQueue errorQueue = new ErrorQueue();
            ErrorManager.SetErrorListener(errorQueue);
            bool success = rawGenerateAndBuildRecognizer("T.g", grammar, "TParser", "TLexer", false);
            Assert.IsFalse(success);
            Assert.AreEqual(
                "[error(117): " + tmpdir.ToString() + Path.DirectorySeparatorChar + "T.g:6:9: missing attribute access on rule scope: x]",
                '[' + string.Join(", ", errorQueue.errors) + ']');
        }
    }
}
