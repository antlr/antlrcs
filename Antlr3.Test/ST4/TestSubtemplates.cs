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

    [TestClass]
    public class TestSubtemplates : StringTemplateTestBase
    {
        [TestMethod]
        public void TestSimpleIteration()
        {
            STGroup group = new STGroup();
            group.DefineTemplate("test", "<names:{<it>}>!");
            ST st = group.GetInstanceOf("test");
            st.Add("names", "Ter");
            st.Add("names", "Tom");
            st.Add("names", "Sumana");
            String expected = "TerTomSumana!";
            String result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestSimpleIterationWithArg()
        {
            STGroup group = new STGroup();
            group.DefineTemplate("test", "<names:{n | <n>}>!");
            ST st = group.GetInstanceOf("test");
            st.Add("names", "Ter");
            st.Add("names", "Tom");
            st.Add("names", "Sumana");
            String expected = "TerTomSumana!";
            String result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Test_it_NotDefinedWithArg()
        {
            STGroup group = new STGroup();
            group.DefineTemplate("test", "<names:{n | <it>}>!");
            ST st = group.GetInstanceOf("test");
            st.Add("names", "Ter");
            st.Add("names", "Tom");
            st.Add("names", "Sumana");
            String expected = "!";
            String result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Test_it_NotDefinedWithArgSingleValue()
        {
            STGroup group = new STGroup();
            group.DefineTemplate("test", "<names:{n | <it>}>!");
            ST st = group.GetInstanceOf("test");
            st.Add("names", "Ter");
            String expected = "!";
            String result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestNestedIterationWithArg()
        {
            STGroup group = new STGroup();
            group.DefineTemplate("test", "<users:{u | <u.id:{id | <id>=}><u.name>}>!");
            ST st = group.GetInstanceOf("test");
            st.Add("users", new TestCoreBasics.User(1, "parrt"));
            st.Add("users", new TestCoreBasics.User(2, "tombu"));
            st.Add("users", new TestCoreBasics.User(3, "sri"));
            String expected = "1=parrt2=tombu3=sri!";
            String result = st.Render();
            Assert.AreEqual(expected, result);
        }
    }
}
