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
    using ArrayList = System.Collections.ArrayList;
    using ST = StringTemplate.Template;
    using STErrorListener = StringTemplate.ITemplateErrorListener;
    using STGroup = StringTemplate.TemplateGroup;
    using String = System.String;

    [TestClass]
    public class TestNullAndEmptyValues : StringTemplateTestBase
    {
        [TestMethod]
        public void TestSeparatorWithNullFirstValue()
        {
            STGroup group = new STGroup();
            group.DefineTemplate("test", "hi <name; separator=\", \">!");
            ST st = group.GetInstanceOf("test");
            st.Add("name", null); // null is added to list, but ignored in iteration
            st.Add("name", "Tom");
            st.Add("name", "Sumana");
            String expected = "hi Tom, Sumana!";
            String result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestSeparatorWithNull2ndValue()
        {
            STGroup group = new STGroup();
            group.DefineTemplate("test", "hi <name; separator=\", \">!");
            ST st = group.GetInstanceOf("test");
            st.Add("name", "Ter");
            st.Add("name", null);
            st.Add("name", "Sumana");
            String expected = "hi Ter, Sumana!";
            String result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestSeparatorWithNullLastValue()
        {
            STGroup group = new STGroup();
            group.DefineTemplate("test", "hi <name; separator=\", \">!");
            ST st = group.GetInstanceOf("test");
            st.Add("name", "Ter");
            st.Add("name", "Tom");
            st.Add("name", null);
            String expected = "hi Ter, Tom!";
            String result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestSeparatorWithTwoNullValuesInRow()
        {
            STGroup group = new STGroup();
            group.DefineTemplate("test", "hi <name; separator=\", \">!");
            ST st = group.GetInstanceOf("test");
            st.Add("name", "Ter");
            st.Add("name", "Tom");
            st.Add("name", null);
            st.Add("name", null);
            st.Add("name", "Sri");
            String expected = "hi Ter, Tom, Sri!";
            String result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestSizeZeroButNonNullListGetsNoOutput()
        {
            STGroup group = new STGroup();
            group.DefineTemplate("test",
                "begin\n" +
                "<users>\n" +
                "end\n");
            ST t = group.GetInstanceOf("test");
            t.Add("users", null);
            String expecting = "begin" + newline + "end" + newline;
            String result = t.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestNullListGetsNoOutput()
        {
            STGroup group = new STGroup();
            group.DefineTemplate("test",
                "begin\n" +
                "<users:{name: <it>}; separator=\", \">\n" +
                "end\n");
            ST t = group.GetInstanceOf("test");
            //t.setAttribute("users", new Duh());
            String expecting = "begin" + newline + "end" + newline;
            String result = t.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestEmptyListGetsNoOutput()
        {
            STGroup group = new STGroup();
            group.DefineTemplate("test",
                "begin\n" +
                "<users:{name: <it>}; separator=\", \">\n" +
                "end\n");
            ST t = group.GetInstanceOf("test");
            t.Add("users", new ArrayList());
            String expecting = "begin" + newline + "end" + newline;
            String result = t.Render();
            Assert.AreEqual(expecting, result);
        }
    }
}
