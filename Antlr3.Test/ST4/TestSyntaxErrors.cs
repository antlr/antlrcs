/*
 * [The "BSD licence"]
 * Copyright (c) 2005-2008 Terence Parr
 * All rights reserved.
 *
 * Conversion to C#:
 * Copyright (c) 2008-2009 Sam Harwell
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

namespace AntlrUnitTests.ST4
{
    using Antlr.Runtime;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using StringTemplate;
    using StringTemplate.Compiler;
    using Path = System.IO.Path;
    using String = System.String;

    [TestClass]
    public class TestSyntaxErrors : StringTemplateTestBase
    {
        [TestMethod]
        public void TestEmptyExpr()
        {
            string template = " <> ";
            TemplateGroup group = new TemplateGroup();
            string result = null;
            try
            {
                group.DefineTemplate(new TemplateName("test"), template);
            }
            catch (TemplateException se)
            {
                RecognitionException re = (RecognitionException)se.InnerException;
                result = 1 + ":" + re.charPositionInLine + ": " + se.Message;
            }
            string expected = "1:1: mismatched input '<' expecting EOF";
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestWeirdChar()
        {
            string template = "   <*>";
            TemplateGroup group = new TemplateGroup();
            string result = null;
            try
            {
                group.DefineTemplate(new TemplateName("test"), template);
            }
            catch (TemplateException se)
            {
                RecognitionException re = (RecognitionException)se.InnerException;
                result = 1 + ":" + re.charPositionInLine + ": " + se.Message;
            }
            string expected = "1:4: invalid character: *";
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestValidButOutOfPlaceChar()
        {
            String templates =
                "group t;\n" +
                "foo() ::= <<hi <.> mom>>\n";
            WriteFile(tmpdir, "t.stg", templates);

            ITemplateErrorListener errors = new ErrorBuffer();
            TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
            ErrorManager.ErrorListener = errors;
            group.Load(); // force load
            string expected = "2:15: mismatched input '<' expecting EOF";
            string result = errors.ToString();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestValidButOutOfPlaceCharOnDifferentLine()
        {
            String templates =
                "group t;\n" +
                    "foo() ::= \"hi <\n" +
                    ".> mom\"\n";
            WriteFile(tmpdir, "t.stg", templates);

            ITemplateErrorListener errors = new ErrorBuffer();
            TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
            ErrorManager.ErrorListener = errors;
            group.Load(); // force load
            string expected = "2:14: mismatched input '<' expecting EOF"; // TODO: terrible err message
            string result = errors.ToString();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestErrorInNestedTemplate()
        {
            String templates =
                "group t;\n" +
                "foo() ::= \"hi <name:{[<aaa.bb!>]}> mom\"\n";
            WriteFile(tmpdir, "t.stg", templates);

            TemplateGroup group = null;
            ITemplateErrorListener errors = new ErrorBuffer();
            group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
            ErrorManager.ErrorListener = errors;
            group.Load(); // force load
            string expected = "2:29: no viable alternative at input '!'";
            string result = errors.ToString();
            Assert.AreEqual(expected, result);
        }
    }
}
