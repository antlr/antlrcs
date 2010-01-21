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
    using STGroupFile = StringTemplate.TemplateGroupFile;
    using String = System.String;

    [TestClass]
    public class TestLists : StringTemplateTestBase
    {
        [TestMethod]
        public void TestJustCat()
        {
            ST e = new ST(
                    "<[names,phones]>"
                );
            e.Add("names", "Ter");
            e.Add("names", "Tom");
            e.Add("phones", "1");
            e.Add("phones", "2");
            String expecting = "TerTom12";
            Assert.AreEqual(expecting, e.Render());
        }

        [TestMethod]
        public void TestCat2Attributes()
        {
            ST e = new ST(
                    "<[names,phones]; separator=\", \">"
                );
            e.Add("names", "Ter");
            e.Add("names", "Tom");
            e.Add("phones", "1");
            e.Add("phones", "2");
            String expecting = "Ter, Tom, 1, 2";
            Assert.AreEqual(expecting, e.Render());
        }

        [TestMethod]
        public void TestCat2AttributesWithApply()
        {
            ST e = new ST(
                    "<[names,phones]:{a|<a>.}>"
                );
            e.Add("names", "Ter");
            e.Add("names", "Tom");
            e.Add("phones", "1");
            e.Add("phones", "2");
            String expecting = "Ter.Tom.1.2.";
            Assert.AreEqual(expecting, e.Render());
        }

        [TestMethod]
        public void TestCat3Attributes()
        {
            ST e = new ST(
                    "<[names,phones,salaries]; separator=\", \">"
                );
            e.Add("names", "Ter");
            e.Add("names", "Tom");
            e.Add("phones", "1");
            e.Add("phones", "2");
            e.Add("salaries", "big");
            e.Add("salaries", "huge");
            String expecting = "Ter, Tom, 1, 2, big, huge";
            Assert.AreEqual(expecting, e.Render());
        }

        [TestMethod]
        public void TestCatWithTemplateApplicationAsElement()
        {
            ST e = new ST(
                    "<[names:{<it>!},phones]; separator=\", \">"
                );
            e.Add("names", "Ter");
            e.Add("names", "Tom");
            e.Add("phones", "1");
            e.Add("phones", "2");
            String expecting = "Ter!, Tom!, 1, 2";
            Assert.AreEqual(expecting, e.Render());
        }

        [TestMethod]
        public void TestCatWithIFAsElement()
        {
            ST e = new ST(
                    "<[{<if(names)>doh<endif>},phones]; separator=\", \">"
                );
            e.Add("names", "Ter");
            e.Add("names", "Tom");
            e.Add("phones", "1");
            e.Add("phones", "2");
            String expecting = "doh, 1, 2";
            Assert.AreEqual(expecting, e.Render());
        }

        [TestMethod]
        public void TestCatWithNullTemplateApplicationAsElement()
        {
            ST e = new ST(
                    "<[names:{<it>!},\"foo\"]:{x}; separator=\", \">"
                );
            e.Add("phones", "1");
            e.Add("phones", "2");
            String expecting = "x";  // only one since template application gives nothing
            Assert.AreEqual(expecting, e.Render());
        }

        [TestMethod]
        public void TestCatWithNestedTemplateApplicationAsElement()
        {
            ST e = new ST(
                    "<[names, [\"foo\",\"bar\"]:{<it>!},phones]; separator=\", \">"
                );
            e.Add("names", "Ter");
            e.Add("names", "Tom");
            e.Add("phones", "1");
            e.Add("phones", "2");
            String expecting = "Ter, Tom, foo!, bar!, 1, 2";
            Assert.AreEqual(expecting, e.Render());
        }

        [TestMethod]
        public void TestListAsTemplateArgument()
        {
            String templates =
                    "test(names,phones) ::= \"<foo([names,phones])>\"" + newline +
                    "foo(items) ::= \"<items:{a | *<a>*}>\"" + newline
                    ;
            WriteFile(tmpdir, "t.stg", templates);
            STGroup group = new STGroupFile(tmpdir + "/" + "t.stg");
            ST e = group.GetInstanceOf("test");
            e.Add("names", "Ter");
            e.Add("names", "Tom");
            e.Add("phones", "1");
            e.Add("phones", "2");
            String expecting = "*Ter**Tom**1**2*";
            String result = e.Render();
            Assert.AreEqual(expecting, result);
        }
    }
}
