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
    using STErrorListener = StringTemplate.ITemplateErrorListener;
    using STGroup = StringTemplate.TemplateGroup;
    using STGroupDir = StringTemplate.TemplateGroupDirectory;
    using STGroupFile = StringTemplate.TemplateGroupFile;
    using String = System.String;
    using StringTemplate;

    [TestClass]
    public class TestSubtemplates : StringTemplateTestBase
    {
        [TestMethod]
        public void TestSimpleIteration()
        {
            STGroup group = new STGroup();
            group.DefineTemplate(new TemplateName("test"), "<names:{<it>}>!");
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
            group.DefineTemplate(new TemplateName("test"), "<names:{n | <n>}>!");
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
            group.DefineTemplate(new TemplateName("test"), "<names:{n | <it>}>!");
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
            group.DefineTemplate(new TemplateName("test"), "<names:{n | <it>}>!");
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
            group.DefineTemplate(new TemplateName("test"), "<users:{u | <u.id:{id | <id>=}><u.name>}>!");
            ST st = group.GetInstanceOf("test");
            st.Add("users", new TestCoreBasics.User(1, "parrt"));
            st.Add("users", new TestCoreBasics.User(2, "tombu"));
            st.Add("users", new TestCoreBasics.User(3, "sri"));
            String expected = "1=parrt2=tombu3=sri!";
            String result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestParallelAttributeIteration()
        {
            ST e = new ST(
                    "<names,phones,salaries:{n,p,s | <n>@<p>: <s>\n}>"
                );
            e.Add("names", "Ter");
            e.Add("names", "Tom");
            e.Add("phones", "1");
            e.Add("phones", "2");
            e.Add("salaries", "big");
            e.Add("salaries", "huge");
            String expecting = "Ter@1: big" + newline + "Tom@2: huge" + newline;
            Assert.AreEqual(expecting, e.Render());
        }

        [TestMethod]
        public void TestParallelAttributeIterationWithNullValue(){
        ST e = new ST(
                "<names,phones,salaries:{n,p,s | <n>@<p>: <s>\n}>"
            );
        e.Add("names", "Ter");
        e.Add("names", "Tom");
        e.Add("names", "Sriram");
        e.Add("phones", new object[] { "1", null, "3" });
        e.Add("salaries", "big");
        e.Add("salaries", "huge");
        e.Add("salaries", "enormous");
        String expecting = "Ter@1: big"+newline+
                           "Tom@: huge"+newline+
                           "Sriram@3: enormous"+newline;
        Assert.AreEqual(expecting, e.Render());
    }

        [TestMethod]
        public void TestParallelAttributeIterationHasI()
        {
            ST e = new ST(
                    "<names,phones,salaries:{n,p,s | <i0>. <n>@<p>: <s>\n}>"
                );
            e.Add("names", "Ter");
            e.Add("names", "Tom");
            e.Add("phones", "1");
            e.Add("phones", "2");
            e.Add("salaries", "big");
            e.Add("salaries", "huge");
            String expecting =
                "0. Ter@1: big" + newline +
                "1. Tom@2: huge" + newline;
            Assert.AreEqual(expecting, e.Render());
        }

        [TestMethod]
        public void TestParallelAttributeIterationWithDifferentSizes()
        {
            ST e = new ST(
                    "<names,phones,salaries:{n,p,s | <n>@<p>: <s>}; separator=\", \">"
                );
            e.Add("names", "Ter");
            e.Add("names", "Tom");
            e.Add("names", "Sriram");
            e.Add("phones", "1");
            e.Add("phones", "2");
            e.Add("salaries", "big");
            String expecting = "Ter@1: big, Tom@2: , Sriram@: ";
            Assert.AreEqual(expecting, e.Render());
        }

        [TestMethod]
        public void TestParallelAttributeIterationWithSingletons()
        {
            ST e = new ST(
                    "<names,phones,salaries:{n,p,s | <n>@<p>: <s>}; separator=\", \">"
                );
            e.Add("names", "Ter");
            e.Add("phones", "1");
            e.Add("salaries", "big");
            String expecting = "Ter@1: big";
            Assert.AreEqual(expecting, e.Render());
        }

        [TestMethod]
        public void TestParallelAttributeIterationWithDifferentSizesTemplateRefInsideToo()
        {
            String templates =
                    "page(names,phones,salaries) ::= " + newline +
                    "	<< <names,phones,salaries:{n,p,s | <value(n)>@<value(p)>: <value(s)>}; separator=\", \"> >>" + newline +
                    "value(x=\"n/a\") ::= \"<x>\"" + newline;
            WriteFile(tmpdir, "g.stg", templates);

            STGroup group = new STGroupFile(tmpdir + "/g.stg");
            ST p = group.GetInstanceOf("page");
            p.Add("names", "Ter");
            p.Add("names", "Tom");
            p.Add("names", "Sriram");
            p.Add("phones", "1");
            p.Add("phones", "2");
            p.Add("salaries", "big");
            String expecting = "Ter@1: big, Tom@2: n/a, Sriram@n/a: n/a";
            Assert.AreEqual(expecting, p.Render());
        }
    }
}
