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
    using Path = System.IO.Path;
    using StringTemplate;

    [TestClass]
    public class TestRuntimeErrors : StringTemplateTestBase
    {
        public class UserHiddenName
        {
            private string name;

            public UserHiddenName(string name)
            {
                this.name = name;
            }

            protected string Name
            {
                get
                {
                    return name;
                }
            }
        }

        public class UserHiddenNameField
        {
            private string name;

            public UserHiddenNameField(string name)
            {
                this.name = name;
            }
        }

        [TestMethod]
        public void TestMissingEmbeddedTemplate()
        {
            ErrorBuffer errors = new ErrorBuffer();
            ErrorManager.ErrorListener = errors;

            String templates =
                "t() ::= \"<foo()>\"" + newline;

            WriteFile(tmpdir, "t.stg", templates);
            STGroup group = new STGroupFile(Path.Combine(tmpdir, "t.stg"));
            ST st = group.GetInstanceOf("t");
            st.Render();
            String expected = "context [t] 1:0 no such template: foo" + newline;
            String result = errors.ToString();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestMissingSuperTemplate()
        {
            ErrorBuffer errors = new ErrorBuffer();
            ErrorManager.ErrorListener = errors;

            String templates =
                "t() ::= \"<super.t()>\"" + newline;

            WriteFile(tmpdir, "t.stg", templates);
            STGroup group = new STGroupFile(Path.Combine(tmpdir, "t.stg"));
            String templates2 =
                "u() ::= \"blech\"" + newline;

            WriteFile(tmpdir, "t2.stg", templates2);
            STGroup group2 = new STGroupFile(Path.Combine(tmpdir, "t2.stg"));
            group.ImportTemplates(group2);
            ST st = group.GetInstanceOf("t");
            st.Render();
            String expected = "context [t] 1:1 no such template: super.t" + newline;
            String result = errors.ToString();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestNoPropertyNotError()
        {
            ErrorBuffer errors = new ErrorBuffer();
            ErrorManager.ErrorListener = errors;

            String templates =
                "t(u) ::= \"<u.x>\"" + newline;

            WriteFile(tmpdir, "t.stg", templates);
            STGroup group = new STGroupFile(Path.Combine(tmpdir, "t.stg"));
            ST st = group.GetInstanceOf("t");
            st.Add("u", new User(32, "parrt"));
            st.Render();
            String expected = "";
            String result = errors.ToString();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestHiddenPropertyNotError()
        {
            ErrorBuffer errors = new ErrorBuffer();
            ErrorManager.ErrorListener = errors;

            String templates =
                "t(u) ::= \"<u.name>\"" + newline;

            WriteFile(tmpdir, "t.stg", templates);
            STGroup group = new STGroupFile(Path.Combine(tmpdir, "t.stg"));
            ST st = group.GetInstanceOf("t");
            st.Add("u", new UserHiddenName("parrt"));
            st.Render();
            String expected = "";
            String result = errors.ToString();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestHiddenFieldNotError()
        {
            ErrorBuffer errors = new ErrorBuffer();
            ErrorManager.ErrorListener = errors;

            String templates =
                "t(u) ::= \"<u.name>\"" + newline;

            WriteFile(tmpdir, "t.stg", templates);
            STGroup group = new STGroupFile(Path.Combine(tmpdir, "t.stg"));
            ST st = group.GetInstanceOf("t");
            st.Add("u", new UserHiddenNameField("parrt"));
            st.Render();
            String expected = "";
            String result = errors.ToString();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestSoleArg()
        {
            ErrorBuffer errors = new ErrorBuffer();
            ErrorManager.ErrorListener = errors;

            string templates =
                "t() ::= \"<u({9})>\"\n" +
                "u(x,y) ::= \"<x>\"\n";

            WriteFile(tmpdir, "t.stg", templates);
            STGroup group = new STGroupFile(Path.Combine(tmpdir, "t.stg"));
            ST st = group.GetInstanceOf("t");
            st.Render();
            string expected = "context [t] 1:3 expecting single arg in template reference u() (not 2 args)" + newline;
            string result = errors.ToString();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestParallelAttributeIterationWithMismatchArgListSizes()
        {
            ErrorBuffer errors = new ErrorBuffer();
            ErrorManager.ErrorListener = errors;
            Template e = new Template(
                    "<names,phones,salaries:{n,p | <n>@<p>}; separator=\", \">"
                );
            e.Add("names", "Ter");
            e.Add("names", "Tom");
            e.Add("phones", "1");
            e.Add("phones", "2");
            e.Add("salaries", "big");
            e.Render();
            string errorExpecting = "context [anonymous] 1:1 iterating through 3 arguments but parallel map has 2 formal arguments" + newline;
            Assert.AreEqual(errorExpecting, errors.ToString());
            string expecting = "Ter@1, Tom@2";
            Assert.AreEqual(expecting, e.Render());
        }

        [TestMethod]
        public void TestParallelAttributeIterationWithMissingArgs()
        {
            ErrorBuffer errors = new ErrorBuffer();
            ErrorManager.ErrorListener = errors;
            Template e = new Template(
                    "<names,phones,salaries:{<n>@<p>}; separator=\", \">"
                );
            e.Add("names", "Tom");
            e.Add("phones", "2");
            e.Add("salaries", "big");
            e.Render(); // generate the error
            string errorExpecting = "context [anonymous] 1:1 missing argument definitions" + newline;
            Assert.AreEqual(errorExpecting, errors.ToString());
        }
    }
}
