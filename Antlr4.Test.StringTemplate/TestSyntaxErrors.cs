/*
 [The "BSD license"]
 Copyright (c) 2009 Terence Parr
 All rights reserved.

 Redistribution and use in source and binary forms, with or without
 modification, are permitted provided that the following conditions
 are met:
 1. Redistributions of source code must retain the above copyright
    notice, this list of conditions and the following disclaimer.
 2. Redistributions in binary form must reproduce the above copyright
    notice, this list of conditions and the following disclaimer in the
    documentation and/or other materials provided with the distribution.
 3. The name of the author may not be used to endorse or promote products
    derived from this software without specific prior written permission.

 THIS SOFTWARE IS PROVIDED BY THE AUTHOR ``AS IS'' AND ANY EXPRESS OR
 IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
 OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
 IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT, INDIRECT,
 INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
 NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
 DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
 THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
 THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
namespace Antlr4.Test.StringTemplate
{
    using Antlr4.StringTemplate;
    using Antlr4.Test.StringTemplate.Extensions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Antlr4.StringTemplate.Misc;
    using Antlr4.StringTemplate.Compiler;

    [TestClass]
    public class TestSyntaxErrors : BaseTest
    {
        [TestMethod]
        public void TestEmptyExpr()
        {
            string template = " <> ";
            TemplateGroup group = new TemplateGroup();
            ErrorBuffer errors = new ErrorBuffer();
            group.Listener = errors;
            try
            {
                group.DefineTemplate("test", template);
            }
            catch (TemplateException)
            {
            }
            string result = errors.ToString();
            string expected = "test 1:0: this doesn't look like a template: \" <> \"" + newline;
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestEmptyExpr2()
        {
            string template = "hi <> ";
            TemplateGroup group = new TemplateGroup();
            ErrorBuffer errors = new ErrorBuffer();
            group.Listener = errors;
            try
            {
                group.DefineTemplate("test", template);
            }
            catch (TemplateException)
            {
            }
            string result = errors.ToString();
            string expected = "test 1:3: doesn't look like an expression" + newline;
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestWeirdChar()
        {
            string template = "   <*>";
            TemplateGroup group = new TemplateGroup();
            ErrorBuffer errors = new ErrorBuffer();
            group.Listener = errors;
            try
            {
                group.DefineTemplate("test", template);
            }
            catch (TemplateException)
            {
            }
            string result = errors.ToString();
            string expected = "test 1:4: invalid character '*'" + newline +
                              "test 1:0: this doesn't look like a template: \"   <*>\"" + newline;
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestValidButOutOfPlaceChar()
        {
            string templates =
                "foo() ::= <<hi <.> mom>>\n";
            writeFile(tmpdir, "t.stg", templates);

            ITemplateErrorListener errors = new ErrorBuffer();
            TemplateGroupFile group = new TemplateGroupFile(tmpdir + "/" + "t.stg");
            group.Listener = errors;
            group.Load(); // force load
            string expected = "t.stg 1:15: doesn't look like an expression" + newline;
            string result = errors.ToString();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestValidButOutOfPlaceCharOnDifferentLine()
        {
            string templates =
                    "foo() ::= \"hi <\n" +
                    ".> mom\"\n";
            writeFile(tmpdir, "t.stg", templates);

            ErrorBuffer errors = new ErrorBuffer();
            TemplateGroupFile group = new TemplateGroupFile(tmpdir + "/" + "t.stg");
            group.Listener = errors;
            group.Load(); // force load
            string expected = "[t.stg 1:15: \\n in string, t.stg 1:14: doesn't look like an expression]";
            string result = errors.Errors.ToListString();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestErrorInNestedTemplate()
        {
            string templates =
                "foo() ::= \"hi <name:{[<aaa.bb!>]}> mom\"\n";
            writeFile(tmpdir, "t.stg", templates);

            TemplateGroupFile group = null;
            ITemplateErrorListener errors = new ErrorBuffer();
            group = new TemplateGroupFile(tmpdir + "/" + "t.stg");
            group.Listener = errors;
            group.Load(); // force load
            string expected = "t.stg 1:29: '!' came as a complete surprise to me" + newline;
            string result = errors.ToString();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestEOFInExpr()
        {
            string templates =
                "foo() ::= \"hi <name:{x|[<aaa.bb>]}\"\n";
            writeFile(tmpdir, "t.stg", templates);

            TemplateGroupFile group = null;
            ITemplateErrorListener errors = new ErrorBuffer();
            group = new TemplateGroupFile(tmpdir + "/" + "t.stg");
            group.Listener = errors;
            group.Load(); // force load
            string expected = "t.stg 1:34: premature EOF" + newline;
            string result = errors.ToString();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestMissingRPAREN()
        {
            string templates =
                "foo() ::= \"hi <foo(>\"\n";
            writeFile(tmpdir, "t.stg", templates);

            TemplateGroupFile group = null;
            ITemplateErrorListener errors = new ErrorBuffer();
            group = new TemplateGroupFile(tmpdir + "/" + "t.stg");
            group.Listener = errors;
            group.Load(); // force load
            string expected = "t.stg 1:19: '>' came as a complete surprise to me" + newline;
            string result = errors.ToString();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestRotPar()
        {
            string templates =
                "foo() ::= \"<a,b:t(),u()>\"\n";
            writeFile(tmpdir, "t.stg", templates);

            TemplateGroupFile group = null;
            ITemplateErrorListener errors = new ErrorBuffer();
            group = new TemplateGroupFile(tmpdir + "/" + "t.stg");
            group.Listener = errors;
            group.Load(); // force load
            string expected = "t.stg 1:19: mismatched input ',' expecting RDELIM" + newline;
            string result = errors.ToString();
            Assert.AreEqual(expected, result);
        }
    }
}
