/*
 * [The "BSD licence"]
 * Copyright (c) 2011 Terence Parr
 * All rights reserved.
 *
 * Conversion to C#:
 * Copyright (c) 2011 Sam Harwell, Tunnel Vision Laboratories, LLC
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

namespace Antlr4.Test.StringTemplate
{
    using Antlr4.StringTemplate;
    using Antlr4.StringTemplate.Misc;
    using Antlr4.Test.StringTemplate.Extensions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Path = System.IO.Path;

    [TestClass]
    public class TestGroupSyntaxErrors : BaseTest
    {
        [TestMethod]
        public void TestMissingImportString()
        {
            string templates =
                "import\n" +
                "foo() ::= <<>>\n";
            writeFile(tmpdir, "t.stg", templates);

            ITemplateErrorListener errors = new ErrorBuffer();
            TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
            group.Listener = errors;
            group.Load(); // force load
            string expected = "t.stg 2:0: mismatched input 'foo' expecting STRING" + newline +
                "t.stg 2:3: required (...)+ loop did not match anything at input '('" + newline;
            string result = errors.ToString();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestImportNotString()
        {
            string templates =
                "import Super.stg\n" +
                "foo() ::= <<>>\n";
            writeFile(tmpdir, "t.stg", templates);

            ITemplateErrorListener errors = new ErrorBuffer();
            TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
            group.Listener = errors;
            group.Load(); // force load
            string expected = "t.stg 1:7: mismatched input 'Super' expecting STRING" + newline;
            string result = errors.ToString();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestMissingTemplate()
        {
            string templates =
                "foo() ::= \n";
            writeFile(tmpdir, "t.stg", templates);

            TemplateGroupFile group = null;
            ITemplateErrorListener errors = new ErrorBuffer();
            group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
            group.Listener = errors;
            group.Load(); // force load
            string expected = "t.stg 2:0: missing template at '<EOF>'" + newline;
            string result = errors.ToString();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestUnclosedTemplate()
        {
            string templates =
                "foo() ::= {";
            writeFile(tmpdir, "t.stg", templates);

            TemplateGroupFile group = null;
            ITemplateErrorListener errors = new ErrorBuffer();
            group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
            group.Listener = errors;
            group.Load(); // force load
            string expected = "t.stg 1:11: missing final '}' in {...} anonymous template" + newline +
                              "t.stg 1:10: no viable alternative at input '{'" + newline;
            string result = errors.ToString();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestParen()
        {
            string templates =
                "foo( ::= << >>\n";
            writeFile(tmpdir, "t.stg", templates);

            TemplateGroupFile group = null;
            ITemplateErrorListener errors = new ErrorBuffer();
            group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
            group.Listener = errors;
            group.Load(); // force load
            string expected = "t.stg 1:5: no viable alternative at input '::='" + newline;
            string result = errors.ToString();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestNewlineInString()
        {
            string templates =
                "foo() ::= \"\nfoo\"\n";
            writeFile(tmpdir, "t.stg", templates);

            TemplateGroupFile group = null;
            ITemplateErrorListener errors = new ErrorBuffer();
            group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
            group.Listener = errors;
            group.Load(); // force load
            string expected = "t.stg 1:11: \\n in string" + newline;
            string result = errors.ToString();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestParen2()
        {
            string templates =
                "foo) ::= << >>\n" +
                "bar() ::= <<bar>>\n";
            writeFile(tmpdir, "t.stg", templates);

            TemplateGroupFile group = null;
            ITemplateErrorListener errors = new ErrorBuffer();
            group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
            group.Listener = errors;
            group.Load(); // force load
            string expected = "t.stg 1:0: garbled template definition starting at 'foo'" + newline;
            string result = errors.ToString();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestArg()
        {
            string templates =
                "foo(a,) ::= << >>\n";
            writeFile(tmpdir, "t.stg", templates);

            TemplateGroupFile group = null;
            ITemplateErrorListener errors = new ErrorBuffer();
            group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
            group.Listener = errors;
            group.Load(); // force load
            string expected = "t.stg 1:6: missing ID at ')'" + newline;
            string result = errors.ToString();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestArg2()
        {
            string templates =
                "foo(a,,) ::= << >>\n";
            writeFile(tmpdir, "t.stg", templates);

            TemplateGroupFile group = null;
            ErrorBuffer errors = new ErrorBuffer();
            group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
            group.Listener = errors;
            group.Load(); // force load
            string expected =
                "[t.stg 1:6: missing ID at ',', " +
                "t.stg 1:7: missing ID at ')']";
            string result = errors.Errors.ToListString();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestArg3()
        {
            string templates =
                "foo(a b) ::= << >>\n";
            writeFile(tmpdir, "t.stg", templates);

            TemplateGroupFile group = null;
            ErrorBuffer errors = new ErrorBuffer();
            group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
            group.Listener = errors;
            group.Load(); // force load
            string expected =
                "[t.stg 1:6: no viable alternative at input 'b']";
            string result = errors.Errors.ToListString();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestDefaultArgsOutOfOrder()
        {
            string templates =
                "foo(a={hi}, b) ::= << >>\n";
            writeFile(tmpdir, "t.stg", templates);

            TemplateGroupFile group = null;
            ErrorBuffer errors = new ErrorBuffer();
            group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
            group.Listener = errors;
            group.Load(); // force load
            string expected =
                "[t.stg 1:13: Optional parameters must appear after all required parameters]";
            string result = errors.Errors.ToListString();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestErrorWithinTemplate()
        {
            string templates =
                "foo(a) ::= \"<a b>\"\n";
            writeFile(tmpdir, "t.stg", templates);

            TemplateGroupFile group = null;
            ErrorBuffer errors = new ErrorBuffer();
            group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
            group.Listener = errors;
            group.Load(); // force load
            // TODO: The forced k=2 in TemplateParser results in a message for 'a' instead of 'b'.
            string expected = "[t.stg 1:13: 'a' came as a complete surprise to me]";
            string result = errors.Errors.ToListString();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestMap2()
        {
            string templates =
                "d ::= [\"k\":]\n";
            writeFile(tmpdir, "t.stg", templates);

            TemplateGroupFile group = null;
            ErrorBuffer errors = new ErrorBuffer();
            group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
            group.Listener = errors;
            group.Load(); // force load
            string expected = "[t.stg 1:11: missing value for key at ']']";
            string result = errors.Errors.ToListString();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestMap3()
        {
            string templates =
                "d ::= [\"k\":{dfkj}}]\n"; // extra }
            writeFile(tmpdir, "t.stg", templates);

            TemplateGroupFile group = null;
            ErrorBuffer errors = new ErrorBuffer();
            group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
            group.Listener = errors;
            group.Load(); // force load
            string expected = "[t.stg 1:17: invalid character '}']";
            string result = errors.Errors.ToListString();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestUnterminatedString()
        {
            string templates =
                "f() ::= \""; // extra }
            writeFile(tmpdir, "t.stg", templates);

            TemplateGroupFile group = null;
            ErrorBuffer errors = new ErrorBuffer();
            group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
            group.Listener = errors;
            group.Load(); // force load
            string expected = "[t.stg 1:9: unterminated string, t.stg 1:9: missing template at '<EOF>']";
            string result = errors.Errors.ToListString();
            Assert.AreEqual(expected, result);
        }
    }
}
