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
            STGroup group = new STGroup();
            group.defineTemplate("test", "names", "<names:{n|<n>}>!");
            ST st = group.getInstanceOf("test");
            st.add("names", "Ter");
            st.add("names", "Tom");
            st.add("names", "Sumana");
            string expected = "TerTomSumana!";
            string result = st.render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestMapIterationIsByKeys()
        {
            STGroup group = new STGroup();
            group.defineTemplate("test", "emails", "<emails:{n|<n>}>!");
            ST st = group.getInstanceOf("test");
            IDictionary<string, string> emails = new Dictionary<string, string>();
            emails["parrt"] = "Ter";
            emails["tombu"] = "Tom";
            emails["dmose"] = "Dan";
            st.add("emails", emails);
            string expected = "parrttombudmose!";
            string result = st.render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestSimpleIterationWithArg()
        {
            STGroup group = new STGroup();
            group.defineTemplate("test", "names", "<names:{n | <n>}>!");
            ST st = group.getInstanceOf("test");
            st.add("names", "Ter");
            st.add("names", "Tom");
            st.add("names", "Sumana");
            string expected = "TerTomSumana!";
            string result = st.render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestNestedIterationWithArg()
        {
            STGroup group = new STGroup();
            group.defineTemplate("test", "users", "<users:{u | <u.id:{id | <id>=}><u.name>}>!");
            ST st = group.getInstanceOf("test");
            st.add("users", new TestCoreBasics.User(1, "parrt"));
            st.add("users", new TestCoreBasics.User(2, "tombu"));
            st.add("users", new TestCoreBasics.User(3, "sri"));
            string expected = "1=parrt2=tombu3=sri!";
            string result = st.render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestParallelAttributeIteration()
        {
            ST e = new ST(
                    "<names,phones,salaries:{n,p,s | <n>@<p>: <s>\n}>"
                );
            e.add("names", "Ter");
            e.add("names", "Tom");
            e.add("phones", "1");
            e.add("phones", "2");
            e.add("salaries", "big");
            e.add("salaries", "huge");
            string expecting = "Ter@1: big" + newline + "Tom@2: huge" + newline;
            Assert.AreEqual(expecting, e.render());
        }

        [TestMethod]
        public void TestParallelAttributeIterationWithNullValue()
        {
            ST e = new ST(
                    "<names,phones,salaries:{n,p,s | <n>@<p>: <s>\n}>"
                );
            e.add("names", "Ter");
            e.add("names", "Tom");
            e.add("names", "Sriram");
            e.add("phones", new List<string>() { "1", null, "3" });
            e.add("salaries", "big");
            e.add("salaries", "huge");
            e.add("salaries", "enormous");
            string expecting = "Ter@1: big" + newline +
                               "Tom@: huge" + newline +
                               "Sriram@3: enormous" + newline;
            Assert.AreEqual(expecting, e.render());
        }

        [TestMethod]
        public void TestParallelAttributeIterationHasI()
        {
            ST e = new ST(
                    "<names,phones,salaries:{n,p,s | <i0>. <n>@<p>: <s>\n}>"
                );
            e.add("names", "Ter");
            e.add("names", "Tom");
            e.add("phones", "1");
            e.add("phones", "2");
            e.add("salaries", "big");
            e.add("salaries", "huge");
            string expecting =
                "0. Ter@1: big" + newline +
                "1. Tom@2: huge" + newline;
            Assert.AreEqual(expecting, e.render());
        }

        [TestMethod]
        public void TestParallelAttributeIterationWithDifferentSizes()
        {
            ST e = new ST(
                    "<names,phones,salaries:{n,p,s | <n>@<p>: <s>}; separator=\", \">"
                );
            e.add("names", "Ter");
            e.add("names", "Tom");
            e.add("names", "Sriram");
            e.add("phones", "1");
            e.add("phones", "2");
            e.add("salaries", "big");
            string expecting = "Ter@1: big, Tom@2: , Sriram@: ";
            Assert.AreEqual(expecting, e.render());
        }

        [TestMethod]
        public void TestParallelAttributeIterationWithSingletons()
        {
            ST e = new ST(
                    "<names,phones,salaries:{n,p,s | <n>@<p>: <s>}; separator=\", \">"
                );
            e.add("names", "Ter");
            e.add("phones", "1");
            e.add("salaries", "big");
            string expecting = "Ter@1: big";
            Assert.AreEqual(expecting, e.render());
        }

        [TestMethod]
        public void TestParallelAttributeIterationWithDifferentSizesTemplateRefInsideToo()
        {
            string templates =
                    "page(names,phones,salaries) ::= " + newline +
                    "	<< <names,phones,salaries:{n,p,s | <value(n)>@<value(p)>: <value(s)>}; separator=\", \"> >>" + newline +
                    "value(x) ::= \"<if(!x)>n/a<else><x><endif>\"" + newline;
            writeFile(tmpdir, "g.stg", templates);

            STGroup group = new STGroupFile(tmpdir + "/g.stg");
            ST p = group.getInstanceOf("page");
            p.add("names", "Ter");
            p.add("names", "Tom");
            p.add("names", "Sriram");
            p.add("phones", "1");
            p.add("phones", "2");
            p.add("salaries", "big");
            string expecting = " Ter@1: big, Tom@2: n/a, Sriram@n/a: n/a ";
            Assert.AreEqual(expecting, p.render());
        }

        [TestMethod]
        public void TestEvalSTIteratingSubtemplateInSTFromAnotherGroup()
        {
            ErrorBuffer errors = new ErrorBuffer();
            STGroup innerGroup = new STGroup();
            innerGroup.setListener(errors);
            innerGroup.defineTemplate("test", "m", "<m:samegroup()>");
            innerGroup.defineTemplate("samegroup", "x", "hi ");
            ST st = innerGroup.getInstanceOf("test");
            st.add("m", new int[] { 1, 2, 3 });

            STGroup outerGroup = new STGroup();
            outerGroup.defineTemplate("errorMessage", "x", "<x>");
            ST outerST = outerGroup.getInstanceOf("errorMessage");
            outerST.add("x", st);

            string expected = "hi hi hi ";
            string result = outerST.render();

            Assert.AreEqual(errors.Errors.Count, 0); // ignores no such prop errors

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestEvalSTIteratingSubtemplateInSTFromAnotherGroupSingleValue()
        {
            ErrorBuffer errors = new ErrorBuffer();
            STGroup innerGroup = new STGroup();
            innerGroup.setListener(errors);
            innerGroup.defineTemplate("test", "m", "<m:samegroup()>");
            innerGroup.defineTemplate("samegroup", "x", "hi ");
            ST st = innerGroup.getInstanceOf("test");
            st.add("m", 10);

            STGroup outerGroup = new STGroup();
            outerGroup.defineTemplate("errorMessage", "x", "<x>");
            ST outerST = outerGroup.getInstanceOf("errorMessage");
            outerST.add("x", st);

            string expected = "hi ";
            string result = outerST.render();

            Assert.AreEqual(errors.Errors.Count, 0); // ignores no such prop errors

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestEvalSTFromAnotherGroup()
        {
            ErrorBuffer errors = new ErrorBuffer();
            STGroup innerGroup = new STGroup();
            innerGroup.setListener(errors);
            innerGroup.defineTemplate("bob", "inner");
            ST st = innerGroup.getInstanceOf("bob");

            STGroup outerGroup = new STGroup();
            outerGroup.setListener(errors);
            outerGroup.defineTemplate("errorMessage", "x", "<x>");
            outerGroup.defineTemplate("bob", "outer"); // should not be visible to test() in innerGroup
            ST outerST = outerGroup.getInstanceOf("errorMessage");
            outerST.add("x", st);

            string expected = "inner";
            string result = outerST.render();

            Assert.AreEqual(errors.Errors.Count, 0); // ignores no such prop errors

            Assert.AreEqual(expected, result);
        }
    }
}
