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
    using System.Collections.Generic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using ST = StringTemplate.Template;
    using STGroup = StringTemplate.TemplateGroup;
    using String = System.String;

    [TestClass]
    public class TestIndirectionAndEarlyEvaluation : StringTemplateTestBase
    {
        [TestMethod]
        public void TestEarlyEval()
        {
            String template = "<(name)>";
            ST st = new ST(template);
            st.Add("name", "Ter");
            String expected = "Ter";
            String result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestIndirectTemplateInclude()
        {
            STGroup group = new STGroup();
            group.DefineTemplate("foo", "bar");
            String template = "<(name)()>";
            group.DefineTemplate("test", template);
            ST st = group.GetInstanceOf("test");
            st.Add("name", "foo");
            String expected = "bar";
            String result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestIndirectTemplateIncludeViaTemplate()
        {
            STGroup group = new STGroup();
            group.DefineTemplate("foo", "bar");
            group.DefineTemplate("tname", "foo");
            String template = "<(tname())()>";
            group.DefineTemplate("test", template);
            ST st = group.GetInstanceOf("test");
            String expected = "bar";
            String result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestIndirectProp()
        {
            String template = "<u.(propname)>: <u.name>";
            ST st = new ST(template);
            st.Add("u", new TestCoreBasics.User(1, "parrt"));
            st.Add("propname", "id");
            String expected = "1: parrt";
            String result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestIndirectMap()
        {
            STGroup group = new STGroup();
            group.DefineTemplate("a", "[<it>]");
            group.DefineTemplate("test", "hi <names:(templateName)>!");
            ST st = group.GetInstanceOf("test");
            st.Add("names", "Ter");
            st.Add("names", "Tom");
            st.Add("names", "Sumana");
            st.Add("templateName", "a");
            String expected =
                "hi [Ter][Tom][Sumana]!";
            String result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestNonStringDictLookup()
        {
            String template = "<m.(intkey)>";
            ST st = new ST(template);
            IDictionary<int, string> m = new Dictionary<int, String>();
            m[36] = "foo";
            st.Add("m", m);
            st.Add("intkey", 36);
            String expected = "foo";
            String result = st.Render();
            Assert.AreEqual(expected, result);
        }
    }
}
