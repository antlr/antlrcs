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
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Path = System.IO.Path;
    using STGroup = StringTemplate.TemplateGroup;
    using STGroupFile = StringTemplate.TemplateGroupFile;
    using String = System.String;
    using StringTemplate;

    [TestClass]
    public class TestGroupSyntaxErrors : StringTemplateTestBase
    {
        [TestMethod]
        public void TestMissingTemplate()
        {
            String templates =
                "foo() ::= \n";
            WriteFile(tmpdir, "t.stg", templates);

            STGroup group = null;
            ITemplateErrorListener errors = new ErrorBuffer();
            group = new STGroupFile(Path.Combine(tmpdir, "t.stg"));
            ErrorManager.ErrorListener = errors;
            group.Load(); // force load
            String expected = "t.stg 2:0: missing template at '<EOF>'" + newline;
            String result = errors.ToString();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestParen()
        {
            String templates =
                "foo( ::= << >>\n";
            WriteFile(tmpdir, "t.stg", templates);

            STGroup group = null;
            var errors = new ErrorBuffer();
            group = new STGroupFile(Path.Combine(tmpdir, "t.stg"));
            ErrorManager.ErrorListener = errors;
            group.Load(); // force load
            String expected = "t.stg 1:5: missing ')' at '::='" + newline;
            String result = errors.ToString();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestNewlineInString()
        {
            String templates =
                "foo() ::= \"\nfoo\"\n";
            WriteFile(tmpdir, "t.stg", templates);

            STGroup group = null;
            var errors = new ErrorBuffer();
            group = new STGroupFile(tmpdir + "/" + "t.stg");
            ErrorManager.ErrorListener = errors;
            group.Load(); // force load
            String expected = "t.stg 1:11: \\n in string" + newline;
            String result = errors.ToString();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestParen2()
        {
            String templates =
                "foo) ::= << >>\n" +
                "bar() ::= <<bar>>\n";
            WriteFile(tmpdir, "t.stg", templates);

            STGroup group = null;
            var errors = new ErrorBuffer();
            group = new STGroupFile(Path.Combine(tmpdir, "t.stg"));
            ErrorManager.ErrorListener = errors;
            group.Load(); // force load
            String expected = "t.stg 1:0: garbled template definition starting at 'foo'" + newline;
            String result = errors.ToString();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestArg()
        {
            String templates =
                "foo(a,) ::= << >>\n";
            WriteFile(tmpdir, "t.stg", templates);

            STGroup group = null;
            var errors = new ErrorBuffer();
            group = new STGroupFile(Path.Combine(tmpdir, "t.stg"));
            ErrorManager.ErrorListener = errors;
            group.Load(); // force load
            String expected = "t.stg 1:6: missing ID at ')'" + newline;
            String result = errors.ToString();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestArg2()
        {
            String templates =
                "foo(a,,) ::= << >>\n";
            WriteFile(tmpdir, "t.stg", templates);

            STGroup group = null;
            ErrorBuffer errors = new ErrorBuffer();
            group = new STGroupFile(Path.Combine(tmpdir, "t.stg"));
            ErrorManager.ErrorListener = errors;
            group.Load(); // force load
            String expected = "t.stg 1:6: missing ID at ','" + newline +
                              "t.stg 1:7: missing ID at ')'" + newline;
            String result = errors.ToString();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestArg3()
        {
            String templates =
                "foo(a b) ::= << >>\n";
            WriteFile(tmpdir, "t.stg", templates);

            STGroup group = null;
            ErrorBuffer errors = new ErrorBuffer();
            group = new STGroupFile(Path.Combine(tmpdir, "t.stg"));
            ErrorManager.ErrorListener = errors;
            group.Load(); // force load
            String expected = "t.stg 1:6: extraneous input 'b' expecting ')'" + newline;
            String result = errors.ToString();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestErrorWithinTemplate()
        {
            String templates =
                "foo(a) ::= \"<a b>\"\n";
            WriteFile(tmpdir, "t.stg", templates);

            STGroup group = null;
            ErrorBuffer errors = new ErrorBuffer();
            group = new STGroupFile(Path.Combine(tmpdir, "t.stg"));
            ErrorManager.ErrorListener = errors;
            group.Load(); // force load
            String expected = "1:15: 'b' came as a complete surprise to me" + newline;
            String result = errors.ToString();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestMap()
        {
            String templates =
                "d ::= []\n";
            WriteFile(tmpdir, "t.stg", templates);

            STGroup group = null;
            ErrorBuffer errors = new ErrorBuffer();
            group = new STGroupFile(tmpdir + "/" + "t.stg");
            ErrorManager.ErrorListener = errors;
            group.Load(); // force load
            String expected = "t.stg 1:7: missing dictionary entry at ']'" + newline;
            String result = errors.ToString();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestMap2()
        {
            String templates =
                "d ::= [\"k\":]\n";
            WriteFile(tmpdir, "t.stg", templates);

            STGroup group = null;
            ErrorBuffer errors = new ErrorBuffer();
            group = new STGroupFile(tmpdir + "/" + "t.stg");
            ErrorManager.ErrorListener = errors;
            group.Load(); // force load
            String expected = "t.stg 1:11: missing value for key at ']'" + newline;
            String result = errors.ToString();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestMap3()
        {
            String templates =
                "d ::= [\"k\":{dfkj}}]\n"; // extra }
            WriteFile(tmpdir, "t.stg", templates);

            STGroup group = null;
            ErrorBuffer errors = new ErrorBuffer();
            group = new STGroupFile(tmpdir + "/" + "t.stg");
            ErrorManager.ErrorListener = errors;
            group.Load(); // force load
            String expected = "t.stg 1:17: invalid character '}'" + newline;
            String result = errors.ToString();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestUnterminatedString()
        {
            String templates =
                "f() ::= \""; // extra }
            WriteFile(tmpdir, "t.stg", templates);

            STGroup group = null;
            ErrorBuffer errors = new ErrorBuffer();
            group = new STGroupFile(tmpdir + "/" + "t.stg");
            ErrorManager.ErrorListener = errors;
            group.Load(); // force load
            String expected = "t.stg 1:9: unterminated string, t.stg 1:9: missing template at '<EOF>'" + newline;
            String result = errors.ToString();
            Assert.AreEqual(expected, result);
        }
    }
}
