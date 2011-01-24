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
    using Antlr4.StringTemplate.Misc;

    [TestClass]
    public class TestSubtemplates : BaseTest
    {
        [TestMethod]
        public void TestSimpleIteration()
        {
            TemplateGroup group = new TemplateGroup();
            group.DefineTemplate("test", "names", "<names:{n|<n>}>!");
            Template st = group.GetInstanceOf("test");
            st.Add("names", "Ter");
            st.Add("names", "Tom");
            st.Add("names", "Sumana");
            string expected = "TerTomSumana!";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestMapIterationIsByKeys()
        {
            TemplateGroup group = new TemplateGroup();
            group.DefineTemplate("test", "emails", "<emails:{n|<n>}>!");
            Template st = group.GetInstanceOf("test");
            IDictionary<string, string> emails = new Dictionary<string, string>();
            emails["parrt"] = "Ter";
            emails["tombu"] = "Tom";
            emails["dmose"] = "Dan";
            st.Add("emails", emails);
            string expected = "parrttombudmose!";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestSimpleIterationWithArg()
        {
            TemplateGroup group = new TemplateGroup();
            group.DefineTemplate("test", "names", "<names:{n | <n>}>!");
            Template st = group.GetInstanceOf("test");
            st.Add("names", "Ter");
            st.Add("names", "Tom");
            st.Add("names", "Sumana");
            string expected = "TerTomSumana!";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestNestedIterationWithArg()
        {
            TemplateGroup group = new TemplateGroup();
            group.DefineTemplate("test", "users", "<users:{u | <u.id:{id | <id>=}><u.name>}>!");
            Template st = group.GetInstanceOf("test");
            st.Add("users", new TestCoreBasics.User(1, "parrt"));
            st.Add("users", new TestCoreBasics.User(2, "tombu"));
            st.Add("users", new TestCoreBasics.User(3, "sri"));
            string expected = "1=parrt2=tombu3=sri!";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestParallelAttributeIteration()
        {
            Template e = new Template(
                    "<names,phones,salaries:{n,p,s | <n>@<p>: <s>\n}>"
                );
            e.Add("names", "Ter");
            e.Add("names", "Tom");
            e.Add("phones", "1");
            e.Add("phones", "2");
            e.Add("salaries", "big");
            e.Add("salaries", "huge");
            string expecting = "Ter@1: big" + newline + "Tom@2: huge" + newline;
            Assert.AreEqual(expecting, e.Render());
        }

        [TestMethod]
        public void TestParallelAttributeIterationWithNullValue()
        {
            Template e = new Template(
                    "<names,phones,salaries:{n,p,s | <n>@<p>: <s>\n}>"
                );
            e.Add("names", "Ter");
            e.Add("names", "Tom");
            e.Add("names", "Sriram");
            e.Add("phones", new List<string>() { "1", null, "3" });
            e.Add("salaries", "big");
            e.Add("salaries", "huge");
            e.Add("salaries", "enormous");
            string expecting = "Ter@1: big" + newline +
                               "Tom@: huge" + newline +
                               "Sriram@3: enormous" + newline;
            Assert.AreEqual(expecting, e.Render());
        }

        [TestMethod]
        public void TestParallelAttributeIterationHasI()
        {
            Template e = new Template(
                    "<names,phones,salaries:{n,p,s | <i0>. <n>@<p>: <s>\n}>"
                );
            e.Add("names", "Ter");
            e.Add("names", "Tom");
            e.Add("phones", "1");
            e.Add("phones", "2");
            e.Add("salaries", "big");
            e.Add("salaries", "huge");
            string expecting =
                "0. Ter@1: big" + newline +
                "1. Tom@2: huge" + newline;
            Assert.AreEqual(expecting, e.Render());
        }

        [TestMethod]
        public void TestParallelAttributeIterationWithDifferentSizes()
        {
            Template e = new Template(
                    "<names,phones,salaries:{n,p,s | <n>@<p>: <s>}; separator=\", \">"
                );
            e.Add("names", "Ter");
            e.Add("names", "Tom");
            e.Add("names", "Sriram");
            e.Add("phones", "1");
            e.Add("phones", "2");
            e.Add("salaries", "big");
            string expecting = "Ter@1: big, Tom@2: , Sriram@: ";
            Assert.AreEqual(expecting, e.Render());
        }

        [TestMethod]
        public void TestParallelAttributeIterationWithSingletons()
        {
            Template e = new Template(
                    "<names,phones,salaries:{n,p,s | <n>@<p>: <s>}; separator=\", \">"
                );
            e.Add("names", "Ter");
            e.Add("phones", "1");
            e.Add("salaries", "big");
            string expecting = "Ter@1: big";
            Assert.AreEqual(expecting, e.Render());
        }

        [TestMethod]
        public void TestParallelAttributeIterationWithDifferentSizesTemplateRefInsideToo()
        {
            string templates =
                    "page(names,phones,salaries) ::= " + newline +
                    "	<< <names,phones,salaries:{n,p,s | <value(n)>@<value(p)>: <value(s)>}; separator=\", \"> >>" + newline +
                    "value(x) ::= \"<if(!x)>n/a<else><x><endif>\"" + newline;
            writeFile(tmpdir, "g.stg", templates);

            TemplateGroup group = new TemplateGroupFile(tmpdir + "/g.stg");
            Template p = group.GetInstanceOf("page");
            p.Add("names", "Ter");
            p.Add("names", "Tom");
            p.Add("names", "Sriram");
            p.Add("phones", "1");
            p.Add("phones", "2");
            p.Add("salaries", "big");
            string expecting = " Ter@1: big, Tom@2: n/a, Sriram@n/a: n/a ";
            Assert.AreEqual(expecting, p.Render());
        }

        [TestMethod]
        public void TestEvalSTIteratingSubtemplateInSTFromAnotherGroup()
        {
            ErrorBuffer errors = new ErrorBuffer();
            TemplateGroup innerGroup = new TemplateGroup();
            innerGroup.Listener = errors;
            innerGroup.DefineTemplate("test", "m", "<m:samegroup()>");
            innerGroup.DefineTemplate("samegroup", "x", "hi ");
            Template st = innerGroup.GetInstanceOf("test");
            st.Add("m", new int[] { 1, 2, 3 });

            TemplateGroup outerGroup = new TemplateGroup();
            outerGroup.DefineTemplate("errorMessage", "x", "<x>");
            Template outerST = outerGroup.GetInstanceOf("errorMessage");
            outerST.Add("x", st);

            string expected = "hi hi hi ";
            string result = outerST.Render();

            Assert.AreEqual(errors.Errors.Count, 0); // ignores no such prop errors

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestEvalSTIteratingSubtemplateInSTFromAnotherGroupSingleValue()
        {
            ErrorBuffer errors = new ErrorBuffer();
            TemplateGroup innerGroup = new TemplateGroup();
            innerGroup.Listener = errors;
            innerGroup.DefineTemplate("test", "m", "<m:samegroup()>");
            innerGroup.DefineTemplate("samegroup", "x", "hi ");
            Template st = innerGroup.GetInstanceOf("test");
            st.Add("m", 10);

            TemplateGroup outerGroup = new TemplateGroup();
            outerGroup.DefineTemplate("errorMessage", "x", "<x>");
            Template outerST = outerGroup.GetInstanceOf("errorMessage");
            outerST.Add("x", st);

            string expected = "hi ";
            string result = outerST.Render();

            Assert.AreEqual(errors.Errors.Count, 0); // ignores no such prop errors

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestEvalSTFromAnotherGroup()
        {
            ErrorBuffer errors = new ErrorBuffer();
            TemplateGroup innerGroup = new TemplateGroup();
            innerGroup.Listener = errors;
            innerGroup.DefineTemplate("bob", "inner");
            Template st = innerGroup.GetInstanceOf("bob");

            TemplateGroup outerGroup = new TemplateGroup();
            outerGroup.Listener = errors;
            outerGroup.DefineTemplate("errorMessage", "x", "<x>");
            outerGroup.DefineTemplate("bob", "outer"); // should not be visible to test() in innerGroup
            Template outerST = outerGroup.GetInstanceOf("errorMessage");
            outerST.Add("x", st);

            string expected = "inner";
            string result = outerST.Render();

            Assert.AreEqual(errors.Errors.Count, 0); // ignores no such prop errors

            Assert.AreEqual(expected, result);
        }
    }
}
