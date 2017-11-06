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
    using Antlr4.Test.StringTemplate.Extensions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Environment = System.Environment;
    using ErrorBuffer = Antlr4.StringTemplate.Misc.ErrorBuffer;
    using Path = System.IO.Path;

    [TestClass]
    public class TestGroupSyntax : BaseTest
    {
        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestSimpleGroup()
        {
            string templates =
                "t() ::= <<foo>>" + Environment.NewLine;

            writeFile(tmpdir, "t.stg", templates);
            TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
            string expected =
                "t() ::= <<" + Environment.NewLine +
                "foo" + Environment.NewLine +
                ">>" + Environment.NewLine;
            string result = group.Show();
            Assert.AreEqual(expected, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestEscapedQuote()
        {
            // setTest(ranges) ::= "<ranges; separator=\"||\">"
            // has to unescape the strings.
            string templates =
                "setTest(ranges) ::= \"<ranges; separator=\\\"||\\\">\"" + Environment.NewLine;

            writeFile(tmpdir, "t.stg", templates);
            TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
            string expected =
                "setTest(ranges) ::= <<" + Environment.NewLine +
                "<ranges; separator=\"||\">" + Environment.NewLine +
                ">>" + Environment.NewLine;
            string result = group.Show();
            Assert.AreEqual(expected, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestMultiTemplates()
        {
            string templates =
                "ta(x) ::= \"[<x>]\"" + Environment.NewLine +
                "duh() ::= <<hi there>>" + Environment.NewLine +
                "wow() ::= <<last>>" + Environment.NewLine;

            writeFile(tmpdir, "t.stg", templates);
            TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
            string expected =
                "ta(x) ::= <<" + Environment.NewLine +
                "[<x>]" + Environment.NewLine +
                ">>" + Environment.NewLine +
                "duh() ::= <<" + Environment.NewLine +
                "hi there" + Environment.NewLine +
                ">>" + Environment.NewLine +
                "wow() ::= <<" + Environment.NewLine +
                "last" + Environment.NewLine +
                ">>" + Environment.NewLine;
            string result = group.Show();
            Assert.AreEqual(expected, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestSetDefaultDelimiters()
        {
            string templates =
                "delimiters \"<\", \">\"" + Environment.NewLine +
                "ta(x) ::= \"[<x>]\"" + Environment.NewLine;

            writeFile(tmpdir, "t.stg", templates);
            TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
            Template st = group.GetInstanceOf("ta");
            st.Add("x", "hi");
            string expected = "[hi]";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestSetNonDefaultDelimiters()
        {
            string templates =
                "delimiters \"%\", \"%\"" + Environment.NewLine +
                "ta(x) ::= \"[%x%]\"" + Environment.NewLine;

            writeFile(tmpdir, "t.stg", templates);
            TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
            Template st = group.GetInstanceOf("ta");
            st.Add("x", "hi");
            string expected = "[hi]";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestSingleTemplateWithArgs()
        {
            string templates =
                "t(a,b) ::= \"[<a>]\"" + Environment.NewLine;

            writeFile(tmpdir, "t.stg", templates);
            TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
            string expected =
                "t(a,b) ::= <<" + Environment.NewLine +
                "[<a>]" + Environment.NewLine +
                ">>" + Environment.NewLine;
            string result = group.Show();
            Assert.AreEqual(expected, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestDefaultValues()
        {
            string templates =
                "t(a={def1},b=\"def2\") ::= \"[<a>]\"" + Environment.NewLine;

            writeFile(tmpdir, "t.stg", templates);
            TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
            string expected =
                "t(a={def1},b=\"def2\") ::= <<" + Environment.NewLine +
                "[<a>]" + Environment.NewLine +
                ">>" + Environment.NewLine;
            string result = group.Show();
            Assert.AreEqual(expected, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestDefaultValues2()
        {
            string templates =
                "t(x, y, a={def1}, b=\"def2\") ::= \"[<a>]\"" + Environment.NewLine;

            writeFile(tmpdir, "t.stg", templates);
            TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
            string expected =
                "t(x,y,a={def1},b=\"def2\") ::= <<" + Environment.NewLine +
                "[<a>]" + Environment.NewLine +
                ">>" + Environment.NewLine;
            string result = group.Show();
            Assert.AreEqual(expected, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestDefaultValueTemplateWithArg()
        {
            string templates =
                "t(a={x | 2*<x>}) ::= \"[<a>]\"" + Environment.NewLine;

            writeFile(tmpdir, "t.stg", templates);
            TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
            string expected =
                "t(a={x | 2*<x>}) ::= <<" + Environment.NewLine +
                "[<a>]" + Environment.NewLine +
                ">>" + Environment.NewLine;
            string result = group.Show();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestDefaultValueBehaviorTrue()
        {
            string templates =
                "t(a=true) ::= <<\n" +
                "<a><if(a)>+<else>-<endif>\n" +
                ">>\n";

            writeFile(tmpdir, "t.stg", templates);
            TemplateGroup group = new TemplateGroupFile(tmpdir + Path.DirectorySeparatorChar + "t.stg");
            Template st = group.GetInstanceOf("t");
            string expected = "true+";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestDefaultValueBehaviorFalse()
        {
            string templates =
                "t(a=false) ::= <<\n" +
                "<a><if(a)>+<else>-<endif>\n" +
                ">>\n";

            writeFile(tmpdir, "t.stg", templates);
            TemplateGroup group = new TemplateGroupFile(tmpdir + Path.DirectorySeparatorChar + "t.stg");
            Template st = group.GetInstanceOf("t");
            string expected = "false-";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestDefaultValueBehaviorEmptyTemplate()
        {
            string templates =
                "t(a={}) ::= <<\n" +
                "<a><if(a)>+<else>-<endif>\n" +
                ">>\n";

            writeFile(tmpdir, "t.stg", templates);
            TemplateGroup group = new TemplateGroupFile(tmpdir + Path.DirectorySeparatorChar + "t.stg");
            Template st = group.GetInstanceOf("t");
            string expected = "+";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestDefaultValueBehaviorEmptyList()
        {
            string templates =
                "t(a=[]) ::= <<\n" +
                "<a><if(a)>+<else>-<endif>\n" +
                ">>\n";

            writeFile(tmpdir, "t.stg", templates);
            TemplateGroup group = new TemplateGroupFile(tmpdir + Path.DirectorySeparatorChar + "t.stg");
            Template st = group.GetInstanceOf("t");
            string expected = "-";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestNestedTemplateInGroupFile()
        {
            string templates =
                "t(a) ::= \"<a:{x | <x:{y | <y>}>}>\"" + Environment.NewLine;

            writeFile(tmpdir, "t.stg", templates);
            TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
            string expected =
                "t(a) ::= <<" + Environment.NewLine +
                "<a:{x | <x:{y | <y>}>}>" + Environment.NewLine +
                ">>" + Environment.NewLine;
            string result = group.Show();
            Assert.AreEqual(expected, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestNestedDefaultValueTemplate()
        {
            string templates =
                "t(a={x | <x:{y|<y>}>}) ::= \"ick\"" + Environment.NewLine;

            writeFile(tmpdir, "t.stg", templates);
            TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
            group.Load();
            string expected =
                "t(a={x | <x:{y|<y>}>}) ::= <<" + Environment.NewLine +
                "ick" + Environment.NewLine +
                ">>" + Environment.NewLine;
            string result = group.Show();
            Assert.AreEqual(expected, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestNestedDefaultValueTemplateWithEscapes()
        {
            string templates =
                "t(a={x | \\< <x:{y|<y>\\}}>}) ::= \"[<a>]\"" + Environment.NewLine;

            writeFile(tmpdir, "t.stg", templates);
            TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
            string expected =
                "t(a={x | \\< <x:{y|<y>\\}}>}) ::= <<" + Environment.NewLine +
                "[<a>]" + Environment.NewLine +
                ">>" + Environment.NewLine;
            string result = group.Show();
            Assert.AreEqual(expected, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestMessedUpTemplateDoesntCauseRuntimeError()
        {
            string templates =
                "main(p) ::= <<\n" +
                "<f(x=\"abc\")>\n" +
                ">>\n" +
                "\n" +
                "f() ::= <<\n" +
                "<x>\n" +
                ">>\n";
            writeFile(tmpdir, "t.stg", templates);

            TemplateGroupFile group;
            ErrorBuffer errors = new ErrorBuffer();
            group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
            group.Listener = errors;
            Template st = group.GetInstanceOf("main");
            st.Render();

            string expected = "[context [/main] 1:1 attribute x isn't defined," +
                              " context [/main] 1:1 passed 1 arg(s) to template /f with 0 declared arg(s)," +
                              " context [/main /f] 1:1 attribute x isn't defined]";
            string result = errors.Errors.ToListString();
            Assert.AreEqual(expected, result);
        }
    }
}
