/*
 * [The "BSD license"]
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
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Path = System.IO.Path;

    [TestClass]
    public class TestScopes : BaseTest
    {
        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestSeesEnclosingAttr()
        {
            string templates =
                "t(x,y) ::= \"<u()>\"\n" +
                "u() ::= \"<x><y>\"";
            ErrorBuffer errors = new ErrorBuffer();
            writeFile(tmpdir, "t.stg", templates);
            TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
            group.Listener = errors;
            Template st = group.GetInstanceOf("t");
            st.Add("x", "x");
            st.Add("y", "y");
            string result = st.Render();

            string expectedError = "";
            Assert.AreEqual(expectedError, errors.ToString());

            string expected = "xy";
            Assert.AreEqual(expected, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestMissingArg()
        {
            string templates =
                "t() ::= \"<u()>\"\n" +
                "u(z) ::= \"\"";
            ErrorBuffer errors = new ErrorBuffer();
            writeFile(tmpdir, "t.stg", templates);
            TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
            group.Listener = errors;
            Template st = group.GetInstanceOf("t");
            string result = st.Render();

            string expectedError = "context [/t] 1:1 passed 0 arg(s) to template /u with 1 declared arg(s)" + newline;
            Assert.AreEqual(expectedError, errors.ToString());
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestUnknownAttr()
        {
            string templates =
                "t() ::= \"<x>\"\n";
            ErrorBuffer errors = new ErrorBuffer();
            writeFile(tmpdir, "t.stg", templates);
            TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
            group.Listener = errors;
            Template st = group.GetInstanceOf("t");
            string result = st.Render();

            string expectedError = "context [/t] 1:1 attribute x isn't defined" + newline;
            Assert.AreEqual(expectedError, errors.ToString());
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestArgWithSameNameAsEnclosing()
        {
            string templates =
                "t(x,y) ::= \"<u(x)>\"\n" +
                "u(y) ::= \"<x><y>\"";
            ErrorBuffer errors = new ErrorBuffer();
            writeFile(tmpdir, "t.stg", templates);
            TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
            group.Listener = errors;
            Template st = group.GetInstanceOf("t");
            st.Add("x", "x");
            st.Add("y", "y");
            string result = st.Render();

            string expectedError = "";
            Assert.AreEqual(expectedError, errors.ToString());

            string expected = "xx";
            Assert.AreEqual(expected, result);
            group.Listener = ErrorManager.DefaultErrorListener;
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestIndexAttrVisibleLocallyOnly()
        {
            string templates =
                "t(names) ::= \"<names:{n | <u(n)>}>\"\n" +
                "u(x) ::= \"<i>:<x>\"";
            ErrorBuffer errors = new ErrorBuffer();
            writeFile(tmpdir, "t.stg", templates);
            TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
            group.Listener = errors;
            Template st = group.GetInstanceOf("t");
            st.Add("names", "Ter");
            string result = st.Render();
            group.GetInstanceOf("u").impl.Dump();

            string expectedError = "t.stg 2:11: implicitly-defined attribute i not visible" + newline;
            Assert.AreEqual(expectedError, errors.ToString());

            string expected = ":Ter";
            Assert.AreEqual(expected, result);
            group.Listener = ErrorManager.DefaultErrorListener;
        }
    }
}
