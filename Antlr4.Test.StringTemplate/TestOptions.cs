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
    using Console = System.Console;

    [TestClass]
    public class TestOptions : BaseTest
    {
        [TestMethod]
        public void TestSeparator()
        {
            TemplateGroup group = new TemplateGroup();
            group.DefineTemplate("test", "hi <name; separator=\", \">!", new string[] { "name" });
            Template st = group.GetInstanceOf("test");
            st.Add("name", "Ter");
            st.Add("name", "Tom");
            st.Add("name", "Sumana");
            string expected = "hi Ter, Tom, Sumana!";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestSeparatorWithSpaces()
        {
            TemplateGroup group = new TemplateGroup();
            group.Debug = true;
            group.DefineTemplate("test", "hi <name; separator= \", \">!", new string[] { "name" });
            Template st = group.GetInstanceOf("test");
            Console.WriteLine(st.impl.ast.ToStringTree());
            st.Add("name", "Ter");
            st.Add("name", "Tom");
            st.Add("name", "Sumana");
            string expected = "hi Ter, Tom, Sumana!";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestAttrSeparator()
        {
            TemplateGroup group = new TemplateGroup();
            group.DefineTemplate("test", "hi <name; separator=sep>!", new string[] { "name", "sep" });
            Template st = group.GetInstanceOf("test");
            st.Add("sep", ", ");
            st.Add("name", "Ter");
            st.Add("name", "Tom");
            st.Add("name", "Sumana");
            string expected = "hi Ter, Tom, Sumana!";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestIncludeSeparator()
        {
            TemplateGroup group = new TemplateGroup();
            group.DefineTemplate("foo", "|");
            group.DefineTemplate("test", "hi <name; separator=foo()>!", new string[] { "name", "sep" });
            Template st = group.GetInstanceOf("test");
            st.Add("sep", ", ");
            st.Add("name", "Ter");
            st.Add("name", "Tom");
            st.Add("name", "Sumana");
            string expected = "hi Ter|Tom|Sumana!";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestSubtemplateSeparator()
        {
            TemplateGroup group = new TemplateGroup();
            group.DefineTemplate("test", "hi <name; separator={<sep> _}>!", new string[] { "name", "sep" });
            Template st = group.GetInstanceOf("test");
            st.Add("sep", ",");
            st.Add("name", "Ter");
            st.Add("name", "Tom");
            st.Add("name", "Sumana");
            string expected = "hi Ter, _Tom, _Sumana!";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestSeparatorWithNullFirstValueAndNullOption()
        {
            TemplateGroup group = new TemplateGroup();
            group.DefineTemplate("test", "hi <name; null=\"n/a\", separator=\", \">!", new string[] { "name" });
            Template st = group.GetInstanceOf("test");
            st.Add("name", null);
            st.Add("name", "Tom");
            st.Add("name", "Sumana");
            string expected = "hi n/a, Tom, Sumana!";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestSeparatorWithNull2ndValueAndNullOption()
        {
            TemplateGroup group = new TemplateGroup();
            group.DefineTemplate("test", "hi <name; null=\"n/a\", separator=\", \">!", new string[] { "name" });
            Template st = group.GetInstanceOf("test");
            st.impl.Dump();
            st.Add("name", "Ter");
            st.Add("name", null);
            st.Add("name", "Sumana");
            string expected = "hi Ter, n/a, Sumana!";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestNullValueAndNullOption()
        {
            TemplateGroup group = new TemplateGroup();
            group.DefineTemplate("test", "<name; null=\"n/a\">", new string[] { "name" });
            Template st = group.GetInstanceOf("test");
            st.Add("name", null);
            string expected = "n/a";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestListApplyWithNullValueAndNullOption()
        {
            TemplateGroup group = new TemplateGroup();
            group.DefineTemplate("test", "<name:{n | <n>}; null=\"n/a\">", new string[] { "name" });
            Template st = group.GetInstanceOf("test");
            st.Add("name", "Ter");
            st.Add("name", null);
            st.Add("name", "Sumana");
            string expected = "Tern/aSumana";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestDoubleListApplyWithNullValueAndNullOption()
        {
            // first apply sends [Template, null, Template] to second apply, which puts [] around
            // the value.  This verifies that null not blank comes out of first apply
            // since we don't get [null].
            TemplateGroup group = new TemplateGroup();
            group.DefineTemplate("test", "<name:{n | <n>}:{n | [<n>]}; null=\"n/a\">", new string[] { "name" });
            Template st = group.GetInstanceOf("test");
            st.Add("name", "Ter");
            st.Add("name", null);
            st.Add("name", "Sumana");
            string expected = "[Ter]n/a[Sumana]";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestMissingValueAndNullOption()
        {
            TemplateGroup group = new TemplateGroup();
            group.DefineTemplate("test", "<name; null=\"n/a\">", new string[] { "name" });
            Template st = group.GetInstanceOf("test");
            string expected = "n/a";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestOptionDoesntApplyToNestedTemplate()
        {
            TemplateGroup group = new TemplateGroup();
            group.DefineTemplate("foo", "<zippo>");
            group.DefineTemplate("test", "<foo(); null=\"n/a\">", new string[] { "zippo" });
            Template st = group.GetInstanceOf("test");
            st.Add("zippo", null);
            string expected = "";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestIllegalOption()
        {
            ErrorBuffer errors = new ErrorBuffer();
            TemplateGroup group = new TemplateGroup();
            group.Listener = errors;
            group.DefineTemplate("test", "<name; bad=\"ugly\">", new string[] { "name" });
            Template st = group.GetInstanceOf("test");
            st.Add("name", "Ter");
            string expected = "Ter";
            string result = st.Render();
            Assert.AreEqual(expected, result);
            expected = "[test 1:7: no such option: bad]";
            Assert.AreEqual(expected, errors.Errors.ToListString());
        }
    }
}
