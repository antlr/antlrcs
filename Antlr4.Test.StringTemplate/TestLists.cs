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

    [TestClass]
    public class TestLists : BaseTest
    {
        [TestMethod]
        public void TestJustCat()
        {
            ST e = new ST(
                    "<[names,phones]>"
                );
            e.add("names", "Ter");
            e.add("names", "Tom");
            e.add("phones", "1");
            e.add("phones", "2");
            string expecting = "TerTom12";
            Assert.AreEqual(expecting, e.render());
        }

        [TestMethod]
        public void TestListLiteralWithEmptyElements()
        {
            ST e = new ST(
                "<[\"Ter\",,\"Jesse\"]:{n | <i>:<n>}; separator=\", \", null={foo}>"
            );
            string expecting = "1:Ter, foo, 2:Jesse";
            Assert.AreEqual(expecting, e.render());
        }

        [TestMethod]
        public void TestListLiteralWithEmptyFirstElement()
        {
            ST e = new ST(
                "<[,\"Ter\",\"Jesse\"]:{n | <i>:<n>}; separator=\", \", null={foo}>"
            );
            string expecting = "foo, 1:Ter, 2:Jesse";
            Assert.AreEqual(expecting, e.render());
        }

        [TestMethod]
        public void TestLength()
        {
            ST e = new ST(
                    "<length([names,phones])>"
                );
            e.add("names", "Ter");
            e.add("names", "Tom");
            e.add("phones", "1");
            e.add("phones", "2");
            string expecting = "4";
            Assert.AreEqual(expecting, e.render());
        }

        [TestMethod]
        public void TestCat2Attributes()
        {
            ST e = new ST(
                    "<[names,phones]; separator=\", \">"
                );
            e.add("names", "Ter");
            e.add("names", "Tom");
            e.add("phones", "1");
            e.add("phones", "2");
            string expecting = "Ter, Tom, 1, 2";
            Assert.AreEqual(expecting, e.render());
        }

        [TestMethod]
        public void TestCat2AttributesWithApply()
        {
            ST e = new ST(
                    "<[names,phones]:{a|<a>.}>"
                );
            e.add("names", "Ter");
            e.add("names", "Tom");
            e.add("phones", "1");
            e.add("phones", "2");
            string expecting = "Ter.Tom.1.2.";
            Assert.AreEqual(expecting, e.render());
        }

        [TestMethod]
        public void TestCat3Attributes()
        {
            ST e = new ST(
                    "<[names,phones,salaries]; separator=\", \">"
                );
            e.add("names", "Ter");
            e.add("names", "Tom");
            e.add("phones", "1");
            e.add("phones", "2");
            e.add("salaries", "big");
            e.add("salaries", "huge");
            string expecting = "Ter, Tom, 1, 2, big, huge";
            Assert.AreEqual(expecting, e.render());
        }

        [TestMethod]
        public void TestCatWithTemplateApplicationAsElement()
        {
            ST e = new ST(
                    "<[names:{n|<n>!},phones]; separator=\", \">"
                );
            e.add("names", "Ter");
            e.add("names", "Tom");
            e.add("phones", "1");
            e.add("phones", "2");
            string expecting = "Ter!, Tom!, 1, 2";
            Assert.AreEqual(expecting, e.render());
        }

        [TestMethod]
        public void TestCatWithIFAsElement()
        {
            ST e = new ST(
                    "<[{<if(names)>doh<endif>},phones]; separator=\", \">"
                );
            e.add("names", "Ter");
            e.add("names", "Tom");
            e.add("phones", "1");
            e.add("phones", "2");
            string expecting = "doh, 1, 2";
            Assert.AreEqual(expecting, e.render());
        }

        [TestMethod]
        public void TestCatNullValues()
        {
            // [a, b] must behave like <a><b>; if a==b==null, blank output
            // unless null argument.
            ST e = new ST(
                    "<[no,go]; null=\"foo\", separator=\", \">"
                );
            e.add("phones", "1");
            e.add("phones", "2");
            string expecting = "foo, foo";  // only one since template application gives nothing
            Assert.AreEqual(expecting, e.render());
        }

        [TestMethod]
        public void TestCatWithNullTemplateApplicationAsElement()
        {
            ST e = new ST(
                    "<[names:{n|<n>!},\"foo\"]:{a|x}; separator=\", \">"
                );
            e.add("phones", "1");
            e.add("phones", "2");
            string expecting = "x";  // only one since template application gives nothing
            Assert.AreEqual(expecting, e.render());
        }

        [TestMethod]
        public void TestCatWithNestedTemplateApplicationAsElement()
        {
            ST e = new ST(
                    "<[names, [\"foo\",\"bar\"]:{x | <x>!},phones]; separator=\", \">"
                );
            e.add("names", "Ter");
            e.add("names", "Tom");
            e.add("phones", "1");
            e.add("phones", "2");
            string expecting = "Ter, Tom, foo!, bar!, 1, 2";
            Assert.AreEqual(expecting, e.render());
        }

        [TestMethod]
        public void TestListAsTemplateArgument()
        {
            string templates =
                    "test(names,phones) ::= \"<foo([names,phones])>\"" + newline +
                    "foo(items) ::= \"<items:{a | *<a>*}>\"" + newline
                    ;
            writeFile(tmpdir, "t.stg", templates);
            STGroup group = new STGroupFile(tmpdir + "/" + "t.stg");
            ST e = group.getInstanceOf("test");
            e.add("names", "Ter");
            e.add("names", "Tom");
            e.add("phones", "1");
            e.add("phones", "2");
            string expecting = "*Ter**Tom**1**2*";
            string result = e.render();
            Assert.AreEqual(expecting, result);
        }
    }
}
