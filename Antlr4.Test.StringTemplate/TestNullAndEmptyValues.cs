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

    [TestClass]
    public class TestNullAndEmptyValues : BaseTest
    {
        [TestMethod]
        public void TestSeparatorWithNullFirstValue()
        {
            TemplateGroup group = new TemplateGroup();
            group.defineTemplate("test", "name", "hi <name; separator=\", \">!");
            Template st = group.getInstanceOf("test");
            st.add("name", null); // null is added to list, but ignored in iteration
            st.add("name", "Tom");
            st.add("name", "Sumana");
            string expected = "hi Tom, Sumana!";
            string result = st.render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestTemplateAppliedToNullIsEmpty()
        {
            TemplateGroup group = new TemplateGroup();
            group.defineTemplate("test", "name", "<name:t()>");
            group.defineTemplate("t", "x", "<x>");
            Template st = group.getInstanceOf("test");
            st.add("name", null); // null is added to list, but ignored in iteration
            string expected = "";
            string result = st.render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestTemplateAppliedToMissingValueIsEmpty()
        {
            TemplateGroup group = new TemplateGroup();
            group.defineTemplate("test", "name", "<name:t()>");
            group.defineTemplate("t", "x", "<x>");
            Template st = group.getInstanceOf("test");
            string expected = "";
            string result = st.render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestSeparatorWithNull2ndValue()
        {
            TemplateGroup group = new TemplateGroup();
            group.defineTemplate("test", "name", "hi <name; separator=\", \">!");
            Template st = group.getInstanceOf("test");
            st.add("name", "Ter");
            st.add("name", null);
            st.add("name", "Sumana");
            string expected = "hi Ter, Sumana!";
            string result = st.render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestSeparatorWithNullLastValue()
        {
            TemplateGroup group = new TemplateGroup();
            group.defineTemplate("test", "name", "hi <name; separator=\", \">!");
            Template st = group.getInstanceOf("test");
            st.add("name", "Ter");
            st.add("name", "Tom");
            st.add("name", null);
            string expected = "hi Ter, Tom!";
            string result = st.render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestSeparatorWithTwoNullValuesInRow()
        {
            TemplateGroup group = new TemplateGroup();
            group.defineTemplate("test", "name", "hi <name; separator=\", \">!");
            Template st = group.getInstanceOf("test");
            st.add("name", "Ter");
            st.add("name", "Tom");
            st.add("name", null);
            st.add("name", null);
            st.add("name", "Sri");
            string expected = "hi Ter, Tom, Sri!";
            string result = st.render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestTwoNullValues()
        {
            TemplateGroup group = new TemplateGroup();
            group.defineTemplate("test", "name", "hi <name; null=\"x\">!");
            Template st = group.getInstanceOf("test");
            st.add("name", null);
            st.add("name", null);
            string expected = "hi xx!";
            string result = st.render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestNullListItemNotCountedForIteratorIndex()
        {
            TemplateGroup group = new TemplateGroup();
            group.defineTemplate("test", "name", "<name:{n | <i>:<n>}>");
            Template st = group.getInstanceOf("test");
            st.add("name", "Ter");
            st.add("name", null);
            st.add("name", null);
            st.add("name", "Jesse");
            string expected = "1:Ter2:Jesse";
            string result = st.render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestSizeZeroButNonNullListGetsNoOutput()
        {
            TemplateGroup group = new TemplateGroup();
            group.defineTemplate("test", "users",
                "begin\n" +
                "<users>\n" +
                "end\n");
            Template t = group.getInstanceOf("test");
            t.add("users", null);
            string expecting = "begin" + newline + "end";
            string result = t.render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestNullListGetsNoOutput()
        {
            TemplateGroup group = new TemplateGroup();
            group.defineTemplate("test", "users",
                "begin\n" +
                "<users:{u | name: <u>}; separator=\", \">\n" +
                "end\n");
            Template t = group.getInstanceOf("test");
            string expecting = "begin" + newline + "end";
            string result = t.render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestEmptyListGetsNoOutput()
        {
            TemplateGroup group = new TemplateGroup();
            group.defineTemplate("test", "users",
                "begin\n" +
                "<users:{u | name: <u>}; separator=\", \">\n" +
                "end\n");
            Template t = group.getInstanceOf("test");
            t.add("users", new List<string>());
            string expecting = "begin" + newline + "end";
            string result = t.render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestMissingDictionaryValue()
        {
            TemplateGroup group = new TemplateGroup();
            group.defineTemplate("test", "m", "<m.foo>");
            Template t = group.getInstanceOf("test");
            t.add("m", new Dictionary<string, string>());
            string expecting = "";
            string result = t.render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestMissingDictionaryValue2()
        {
            TemplateGroup group = new TemplateGroup();
            group.defineTemplate("test", "m", "<if(m.foo)>[<m.foo>]<endif>");
            Template t = group.getInstanceOf("test");
            t.add("m", new Dictionary<string, string>());
            string expecting = "";
            string result = t.render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestMissingDictionaryValue3()
        {
            TemplateGroup group = new TemplateGroup();
            group.defineTemplate("test", "m", "<if(m.foo)>[<m.foo>]<endif>");
            Template t = group.getInstanceOf("test");
            t.add("m", new Dictionary<string, string>() { { "foo", null } });
            string expecting = "";
            string result = t.render();
            Assert.AreEqual(expecting, result);
        }
    }
}
