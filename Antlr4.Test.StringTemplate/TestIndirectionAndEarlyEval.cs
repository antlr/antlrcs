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
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Collections.Generic;
    using ErrorBuffer = Antlr4.StringTemplate.Misc.ErrorBuffer;

    [TestClass]
    public class TestIndirectionAndEarlyEval : BaseTest
    {
        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestEarlyEval()
        {
            string template = "<(name)>";
            Template st = new Template(template);
            st.Add("name", "Ter");
            string expected = "Ter";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestIndirectTemplateInclude()
        {
            TemplateGroup group = new TemplateGroup();
            group.DefineTemplate("foo", "bar");
            string template = "<(name)()>";
            group.DefineTemplate("test", template, new string[] { "name" });
            Template st = group.GetInstanceOf("test");
            st.Add("name", "foo");
            string expected = "bar";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestIndirectTemplateIncludeWithArgs()
        {
            TemplateGroup group = new TemplateGroup();
            group.DefineTemplate("foo", "<x><y>", new string[] { "x", "y" });
            string template = "<(name)({1},{2})>";
            group.DefineTemplate("test", template, new string[] { "name" });
            Template st = group.GetInstanceOf("test");
            st.Add("name", "foo");
            string expected = "12";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestIndirectCallWithPassThru()
        {
            // pass-through for dynamic template invocation is not supported by the
            // bytecode representation
            writeFile(tmpdir, "t.stg",
                "t1(x) ::= \"<x>\"\n" +
                "main(x=\"hello\",t=\"t1\") ::= <<\n" +
                "<(t)(...)>\n" +
                ">>");
            TemplateGroup group = new TemplateGroupFile(tmpdir + "/t.stg");
            ErrorBuffer errors = new ErrorBuffer();
            group.Listener = errors;
            Template st = group.GetInstanceOf("main");
            Assert.AreEqual("t.stg 2:34: mismatched input '...' expecting RPAREN" + newline, errors.ToString());
            Assert.IsNull(st);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestIndirectTemplateIncludeViaTemplate()
        {
            TemplateGroup group = new TemplateGroup();
            group.DefineTemplate("foo", "bar");
            group.DefineTemplate("tname", "foo");
            string template = "<(tname())()>";
            group.DefineTemplate("test", template, new string[] { "name" });
            Template st = group.GetInstanceOf("test");
            string expected = "bar";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestIndirectProp()
        {
            string template = "<u.(propname)>: <u.name>";
            Template st = new Template(template);
            st.Add("u", new TestCoreBasics.User(1, "parrt"));
            st.Add("propname", "id");
            string expected = "1: parrt";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestIndirectMap()
        {
            TemplateGroup group = new TemplateGroup();
            group.DefineTemplate("a", "[<x>]", new string[] { "x" });
            group.DefineTemplate("test", "hi <names:(templateName)()>!", new string[] { "names", "templateName" });
            Template st = group.GetInstanceOf("test");
            st.Add("names", "Ter");
            st.Add("names", "Tom");
            st.Add("names", "Sumana");
            st.Add("templateName", "a");
            string expected =
                "hi [Ter][Tom][Sumana]!";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestNonStringDictLookup()
        {
            string template = "<m.(intkey)>";
            Template st = new Template(template);
            IDictionary<int, string> m = new Dictionary<int, string>();
            m[36] = "foo";
            st.Add("m", m);
            st.Add("intkey", 36);
            string expected = "foo";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }
    }
}
