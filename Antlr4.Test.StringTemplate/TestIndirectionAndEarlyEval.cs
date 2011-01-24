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
    public class TestIndirectionAndEarlyEval : BaseTest
    {
        [TestMethod]
        public void TestEarlyEval()
        {
            string template = "<(name)>";
            Template st = new Template(template);
            st.add("name", "Ter");
            string expected = "Ter";
            string result = st.render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestIndirectTemplateInclude()
        {
            TemplateGroup group = new TemplateGroup();
            group.defineTemplate("foo", "bar");
            string template = "<(name)()>";
            group.defineTemplate("test", "name", template);
            Template st = group.getInstanceOf("test");
            st.add("name", "foo");
            string expected = "bar";
            string result = st.render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestIndirectTemplateIncludeWithArgs()
        {
            TemplateGroup group = new TemplateGroup();
            group.defineTemplate("foo", "x,y", "<x><y>");
            string template = "<(name)({1},{2})>";
            group.defineTemplate("test", "name", template);
            Template st = group.getInstanceOf("test");
            st.add("name", "foo");
            string expected = "12";
            string result = st.render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestIndirectTemplateIncludeViaTemplate()
        {
            TemplateGroup group = new TemplateGroup();
            group.defineTemplate("foo", "bar");
            group.defineTemplate("tname", "foo");
            string template = "<(tname())()>";
            group.defineTemplate("test", "name", template);
            Template st = group.getInstanceOf("test");
            string expected = "bar";
            string result = st.render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestIndirectProp()
        {
            string template = "<u.(propname)>: <u.name>";
            Template st = new Template(template);
            st.add("u", new TestCoreBasics.User(1, "parrt"));
            st.add("propname", "id");
            string expected = "1: parrt";
            string result = st.render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestIndirectMap()
        {
            TemplateGroup group = new TemplateGroup();
            group.defineTemplate("a", "x", "[<x>]");
            group.defineTemplate("test", "names,templateName", "hi <names:(templateName)()>!");
            Template st = group.getInstanceOf("test");
            st.add("names", "Ter");
            st.add("names", "Tom");
            st.add("names", "Sumana");
            st.add("templateName", "a");
            string expected =
                "hi [Ter][Tom][Sumana]!";
            string result = st.render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestNonStringDictLookup()
        {
            string template = "<m.(intkey)>";
            Template st = new Template(template);
            IDictionary<int, string> m = new Dictionary<int, string>();
            m[36] = "foo";
            st.add("m", m);
            st.add("intkey", 36);
            string expected = "foo";
            string result = st.render();
            Assert.AreEqual(expected, result);
        }
    }
}
