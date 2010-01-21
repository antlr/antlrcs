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
    using ST = StringTemplate.Template;
    using STGroup = StringTemplate.TemplateGroup;
    using String = System.String;
    using StringTemplate;

    [TestClass]
    public class TestOptions : StringTemplateTestBase
    {
        [TestMethod]
        public void TestSeparator()
        {
            STGroup group = new STGroup();
            group.DefineTemplate(new TemplateName("test"), "hi <name; separator=\", \">!");
            ST st = group.GetInstanceOf("test");
            st.Add("name", "Ter");
            st.Add("name", "Tom");
            st.Add("name", "Sumana");
            String expected = "hi Ter, Tom, Sumana!";
            String result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestAttrSeparator()
        {
            STGroup group = new STGroup();
            group.DefineTemplate(new TemplateName("test"), "hi <name; separator=sep>!");
            ST st = group.GetInstanceOf("test");
            st.Add("sep", ", ");
            st.Add("name", "Ter");
            st.Add("name", "Tom");
            st.Add("name", "Sumana");
            String expected = "hi Ter, Tom, Sumana!";
            String result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestIncludeSeparator()
        {
            STGroup group = new STGroup();
            group.DefineTemplate(new TemplateName("foo"), "|");
            group.DefineTemplate(new TemplateName("test"), "hi <name; separator=foo()>!");
            ST st = group.GetInstanceOf("test");
            st.Add("sep", ", ");
            st.Add("name", "Ter");
            st.Add("name", "Tom");
            st.Add("name", "Sumana");
            String expected = "hi Ter|Tom|Sumana!";
            String result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestSubtemplateSeparator()
        {
            STGroup group = new STGroup();
            group.DefineTemplate(new TemplateName("test"), "hi <name; separator={<sep> _}>!");
            ST st = group.GetInstanceOf("test");
            st.Add("sep", ",");
            st.Add("name", "Ter");
            st.Add("name", "Tom");
            st.Add("name", "Sumana");
            String expected = "hi Ter, _Tom, _Sumana!";
            String result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestSeparatorWithNullFirstValueAndNullOption()
        {
            STGroup group = new STGroup();
            group.DefineTemplate(new TemplateName("test"), "hi <name; null=\"n/a\", separator=\", \">!");
            ST st = group.GetInstanceOf("test");
            st.Add("name", null);
            st.Add("name", "Tom");
            st.Add("name", "Sumana");
            String expected = "hi n/a, Tom, Sumana!";
            String result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestSeparatorWithNull2ndValueAndNullOption()
        {
            STGroup group = new STGroup();
            group.DefineTemplate(new TemplateName("test"), "hi <name; null=\"n/a\", separator=\", \">!");
            ST st = group.GetInstanceOf("test");
            st.Add("name", "Ter");
            st.Add("name", null);
            st.Add("name", "Sumana");
            String expected = "hi Ter, n/a, Sumana!";
            String result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestNullValueAndNullOption()
        {
            STGroup group = new STGroup();
            group.DefineTemplate(new TemplateName("test"), "<name; null=\"n/a\">");
            ST st = group.GetInstanceOf("test");
            st.Add("name", null);
            String expected = "n/a";
            String result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestMissingValueAndNullOption()
        {
            STGroup group = new STGroup();
            group.DefineTemplate(new TemplateName("test"), "<name; null=\"n/a\">");
            ST st = group.GetInstanceOf("test");
            String expected = "n/a";
            String result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestOptionDoesntApplyToNestedTemplate()
        {
            STGroup group = new STGroup();
            group.DefineTemplate(new TemplateName("foo"), "<zippo>");
            group.DefineTemplate(new TemplateName("test"), "<foo(); null=\"n/a\">");
            ST st = group.GetInstanceOf("test");
            st.Add("zippo", null);
            String expected = "";
            String result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestIllegalOption()
        {
            ErrorBuffer errors = new ErrorBuffer();
            ErrorManager.ErrorListener = errors;
            STGroup group = new STGroup();
            group.DefineTemplate(new TemplateName("test"), "<name; bad=\"ugly\">");
            ST st = group.GetInstanceOf("test");
            st.Add("name", "Ter");
            String expected = "Ter";
            String result = st.Render();
            Assert.AreEqual(expected, result);
            expected = "1:7: no such option: bad" + newline;
            Assert.AreEqual(expected, errors.ToString());
        }
    }
}
