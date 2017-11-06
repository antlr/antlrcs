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
    using System.Linq;
    using NotImplementedException = System.NotImplementedException;
    using StringWriter = System.IO.StringWriter;

    [TestClass]
    public class TestNullAndEmptyValues : BaseTest
    {
        private class T
        {
            public string template;
            public object x;
            public string expecting;

            public string result;

            public T(string template, object x, string expecting)
            {
                this.template = template;
                this.x = x;
                this.expecting = expecting;
            }

            public T(T t)
            {
                this.template = t.template;
                this.x = t.x;
                this.expecting = t.expecting;
            }

            public override string ToString()
            {
                string s = x.ToString();
                if (x.GetType().IsArray)
                {
                    throw new NotImplementedException();
                    //s = Arrays.toString((object[])x);
                }
                return "('" + template + "', " + s + ", '" + expecting + "', '" + result + "')";
            }
        }

        private static readonly object UNDEF = "<undefined>";
        private static readonly IList<object> LIST0 = new List<object>();

        private static readonly T[] singleValuedTests = new T[] {
        new T("<x>", UNDEF, ""),
        new T("<x>", null, ""),
        new T("<x>", "", ""),
        new T("<x>", LIST0, ""),

        new T("<x:t()>", UNDEF, ""),
        new T("<x:t()>", null, ""),
        new T("<x:t()>", "", ""),
        new T("<x:t()>", LIST0, ""),

        new T("<x; null={y}>", UNDEF, "y"),
        new T("<x; null={y}>", null, "y"),
        new T("<x; null={y}>", "", ""),
        new T("<x; null={y}>", LIST0, ""),

        new T("<x:t(); null={y}>", UNDEF, "y"),
        new T("<x:t(); null={y}>", null, "y"),
        new T("<x:t(); null={y}>", "", ""),
        new T("<x:t(); null={y}>", LIST0, ""),

        new T("<if(x)>y<endif>", UNDEF, ""),
        new T("<if(x)>y<endif>", null, ""),
        new T("<if(x)>y<endif>", "", "y"),
        new T("<if(x)>y<endif>", LIST0, ""),

        new T("<if(x)>y<else>z<endif>", UNDEF, "z"),
        new T("<if(x)>y<else>z<endif>", null, "z"),
        new T("<if(x)>y<else>z<endif>", "", "y"),
        new T("<if(x)>y<else>z<endif>", LIST0, "z"),
    };

        private static readonly string[] LISTa = { "a" };
        private static readonly string[] LISTab = { "a", "b" };
        private static readonly string[] LISTnull = { null };
        private static readonly string[] LISTa_null = { "a", null };
        private static readonly string[] LISTnull_b = { null, "b" };
        private static readonly string[] LISTa_null_b = { "a", null, "b" };

        private static readonly T[] multiValuedTests = new T[] {
        new T("<x>", LIST0,        ""),
        new T("<x>", LISTa,        "a"),
        new T("<x>", LISTab,       "ab"),
        new T("<x>", LISTnull,     ""),
        new T("<x>", LISTnull_b,   "b"),
        new T("<x>", LISTa_null,   "a"),
        new T("<x>", LISTa_null_b, "ab"),

        new T("<x; null={y}>", LIST0,        ""),
        new T("<x; null={y}>", LISTa,        "a"),
        new T("<x; null={y}>", LISTab,       "ab"),
        new T("<x; null={y}>", LISTnull,     "y"),
        new T("<x; null={y}>", LISTnull_b,   "yb"),
        new T("<x; null={y}>", LISTa_null,   "ay"),
        new T("<x; null={y}>", LISTa_null_b, "ayb"),

        new T("<x; separator={,}>", LIST0,        ""),
        new T("<x; separator={,}>", LISTa,        "a"),
        new T("<x; separator={,}>", LISTab,       "a,b"),
        new T("<x; separator={,}>", LISTnull,     ""),
        new T("<x; separator={,}>", LISTnull_b,   "b"),
        new T("<x; separator={,}>", LISTa_null,   "a"),
        new T("<x; separator={,}>", LISTa_null_b, "a,b"),

        new T("<x; null={y}, separator={,}>", LIST0,        ""),
        new T("<x; null={y}, separator={,}>", LISTa,        "a"),
        new T("<x; null={y}, separator={,}>", LISTab,       "a,b"),
        new T("<x; null={y}, separator={,}>", LISTnull,     "y"),
        new T("<x; null={y}, separator={,}>", LISTnull_b,   "y,b"),
        new T("<x; null={y}, separator={,}>", LISTa_null,   "a,y"),
        new T("<x; null={y}, separator={,}>", LISTa_null_b, "a,y,b"),

        new T("<if(x)>y<endif>", LIST0,        ""),
        new T("<if(x)>y<endif>", LISTa,        "y"),
        new T("<if(x)>y<endif>", LISTab,       "y"),
        new T("<if(x)>y<endif>", LISTnull,     "y"),
        new T("<if(x)>y<endif>", LISTnull_b,   "y"),
        new T("<if(x)>y<endif>", LISTa_null,   "y"),
        new T("<if(x)>y<endif>", LISTa_null_b, "y"),

        new T("<x:{it | <it>}>", LIST0,        ""),
        new T("<x:{it | <it>}>", LISTa,        "a"),
        new T("<x:{it | <it>}>", LISTab,       "ab"),
        new T("<x:{it | <it>}>", LISTnull,     ""),
        new T("<x:{it | <it>}>", LISTnull_b,   "b"),
        new T("<x:{it | <it>}>", LISTa_null,   "a"),
        new T("<x:{it | <it>}>", LISTa_null_b, "ab"),

        new T("<x:{it | <it>}; null={y}>", LIST0,        ""),
        new T("<x:{it | <it>}; null={y}>", LISTa,        "a"),
        new T("<x:{it | <it>}; null={y}>", LISTab,       "ab"),
        new T("<x:{it | <it>}; null={y}>", LISTnull,     "y"),
        new T("<x:{it | <it>}; null={y}>", LISTnull_b,   "yb"),
        new T("<x:{it | <it>}; null={y}>", LISTa_null,   "ay"),
        new T("<x:{it | <it>}; null={y}>", LISTa_null_b, "ayb"),

        new T("<x:{it | <i>.<it>}>", LIST0,        ""),
        new T("<x:{it | <i>.<it>}>", LISTa,        "1.a"),
        new T("<x:{it | <i>.<it>}>", LISTab,       "1.a2.b"),
        new T("<x:{it | <i>.<it>}>", LISTnull,     ""),
        new T("<x:{it | <i>.<it>}>", LISTnull_b,   "1.b"),
        new T("<x:{it | <i>.<it>}>", LISTa_null,   "1.a"),
        new T("<x:{it | <i>.<it>}>", LISTa_null_b, "1.a2.b"),

        new T("<x:{it | <i>.<it>}; null={y}>", LIST0,        ""),
        new T("<x:{it | <i>.<it>}; null={y}>", LISTa,        "1.a"),
        new T("<x:{it | <i>.<it>}; null={y}>", LISTab,       "1.a2.b"),
        new T("<x:{it | <i>.<it>}; null={y}>", LISTnull,     "y"),
        new T("<x:{it | <i>.<it>}; null={y}>", LISTnull_b,   "y1.b"),
        new T("<x:{it | <i>.<it>}; null={y}>", LISTa_null,   "1.ay"),
        new T("<x:{it | <i>.<it>}; null={y}>", LISTa_null_b, "1.ay2.b"),

        new T("<x:{it | x<if(!it)>y<endif>}; null={z}>", LIST0,        ""),
        new T("<x:{it | x<if(!it)>y<endif>}; null={z}>", LISTa,        "x"),
        new T("<x:{it | x<if(!it)>y<endif>}; null={z}>", LISTab,       "xx"),
        new T("<x:{it | x<if(!it)>y<endif>}; null={z}>", LISTnull,     "z"),
        new T("<x:{it | x<if(!it)>y<endif>}; null={z}>", LISTnull_b,   "zx"),
        new T("<x:{it | x<if(!it)>y<endif>}; null={z}>", LISTa_null,   "xz"),
        new T("<x:{it | x<if(!it)>y<endif>}; null={z}>", LISTa_null_b, "xzx"),

        new T("<x:t():u(); null={y}>", LIST0,        ""),
        new T("<x:t():u(); null={y}>", LISTa,        "a"),
        new T("<x:t():u(); null={y}>", LISTab,       "ab"),
        new T("<x:t():u(); null={y}>", LISTnull,     "y"),
        new T("<x:t():u(); null={y}>", LISTnull_b,   "yb"),
        new T("<x:t():u(); null={y}>", LISTa_null,   "ay"),
        new T("<x:t():u(); null={y}>", LISTa_null_b, "ayb")
    };

        private static readonly T[] listTests = new T[] {
        new T("<[]>", UNDEF, ""),
        new T("<[]; null={x}>", UNDEF, ""),
        new T("<[]:{it | x}>", UNDEF, ""),
        new T("<[[],[]]:{it| x}>", UNDEF, ""),
        new T("<[]:t()>", UNDEF, ""),
    };

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestSingleValued()
        {
            IList<T> failed = TestMatrix(singleValuedTests);
            IList<T> expecting = new List<T>();
            CollectionAssert.AreEqual(expecting.ToList(), failed.ToList());
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestMultiValued()
        {
            IList<T> failed = TestMatrix(multiValuedTests);
            IList<T> expecting = new List<T>();
            CollectionAssert.AreEqual(expecting.ToList(), failed.ToList());
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestLists()
        {
            IList<T> failed = TestMatrix(listTests);
            IList<T> expecting = new List<T>();
            CollectionAssert.AreEqual(expecting.ToList(), failed.ToList());
        }

        private IList<T> TestMatrix(T[] tests)
        {
            IList<T> failed = new List<T>();
            foreach (T t in tests)
            {
                T test = new T(t); // dup since we might mod with result
                TemplateGroup group = new TemplateGroup();
                //System.out.println("running "+test);
                group.DefineTemplate("t", "<x>", new string[] { "x" });
                group.DefineTemplate("u", "<x>", new string[] { "x" });
                group.DefineTemplate("test", test.template, new string[] { "x" });
                Template st = group.GetInstanceOf("test");
                if (test.x != UNDEF)
                {
                    st.Add("x", test.x);
                }
                string result = st.Render();
                if (!result.Equals(test.expecting))
                {
                    test.result = result;
                    failed.Add(test);
                }
            }
            return failed;
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestSeparatorWithNullFirstValue()
        {
            TemplateGroup group = new TemplateGroup();
            group.DefineTemplate("test", "hi <name; separator=\", \">!", new string[] { "name" });
            Template st = group.GetInstanceOf("test");
            st.Add("name", null); // null is added to list, but ignored in iteration
            st.Add("name", "Tom");
            st.Add("name", "Sumana");
            string expected = "hi Tom, Sumana!";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestTemplateAppliedToNullIsEmpty()
        {
            TemplateGroup group = new TemplateGroup();
            group.DefineTemplate("test", "<name:t()>", new string[] { "name" });
            group.DefineTemplate("t", "<x>", new string[] { "x" });
            Template st = group.GetInstanceOf("test");
            st.Add("name", null); // null is added to list, but ignored in iteration
            string expected = "";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestTemplateAppliedToMissingValueIsEmpty()
        {
            TemplateGroup group = new TemplateGroup();
            group.DefineTemplate("test", "<name:t()>", new string[] { "name" });
            group.DefineTemplate("t", "<x>", new string[] { "x" });
            Template st = group.GetInstanceOf("test");
            string expected = "";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestSeparatorWithNull2ndValue()
        {
            TemplateGroup group = new TemplateGroup();
            group.DefineTemplate("test", "hi <name; separator=\", \">!", new string[] { "name" });
            Template st = group.GetInstanceOf("test");
            st.Add("name", "Ter");
            st.Add("name", null);
            st.Add("name", "Sumana");
            string expected = "hi Ter, Sumana!";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestSeparatorWithNullLastValue()
        {
            TemplateGroup group = new TemplateGroup();
            group.DefineTemplate("test", "hi <name; separator=\", \">!", new string[] { "name" });
            Template st = group.GetInstanceOf("test");
            st.Add("name", "Ter");
            st.Add("name", "Tom");
            st.Add("name", null);
            string expected = "hi Ter, Tom!";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestSeparatorWithTwoNullValuesInRow()
        {
            TemplateGroup group = new TemplateGroup();
            group.DefineTemplate("test", "hi <name; separator=\", \">!", new string[] { "name" });
            Template st = group.GetInstanceOf("test");
            st.Add("name", "Ter");
            st.Add("name", "Tom");
            st.Add("name", null);
            st.Add("name", null);
            st.Add("name", "Sri");
            string expected = "hi Ter, Tom, Sri!";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestTwoNullValues()
        {
            TemplateGroup group = new TemplateGroup();
            group.DefineTemplate("test", "hi <name; null=\"x\">!", new string[] { "name" });
            Template st = group.GetInstanceOf("test");
            st.Add("name", null);
            st.Add("name", null);
            string expected = "hi xx!";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestNullListItemNotCountedForIteratorIndex()
        {
            TemplateGroup group = new TemplateGroup();
            group.DefineTemplate("test", "<name:{n | <i>:<n>}>", new string[] { "name" });
            Template st = group.GetInstanceOf("test");
            st.Add("name", "Ter");
            st.Add("name", null);
            st.Add("name", null);
            st.Add("name", "Jesse");
            string expected = "1:Ter2:Jesse";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestSizeZeroButNonNullListGetsNoOutput()
        {
            TemplateGroup group = new TemplateGroup();
            group.DefineTemplate("test",
                "begin\n" +
                "<users>\n" +
                "end\n", new string[] { "users" });
            Template t = group.GetInstanceOf("test");
            t.Add("users", null);
            string expecting = "begin" + newline + "end";
            string result = t.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestNullListGetsNoOutput()
        {
            TemplateGroup group = new TemplateGroup();
            group.DefineTemplate("test",
                "begin\n" +
                "<users:{u | name: <u>}; separator=\", \">\n" +
                "end\n", new string[] { "users" });
            Template t = group.GetInstanceOf("test");
            string expecting = "begin" + newline + "end";
            string result = t.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestEmptyListGetsNoOutput()
        {
            TemplateGroup group = new TemplateGroup();
            group.DefineTemplate("test",
                "begin\n" +
                "<users:{u | name: <u>}; separator=\", \">\n" +
                "end\n", new string[] { "users" });
            Template t = group.GetInstanceOf("test");
            t.Add("users", new List<string>());
            string expecting = "begin" + newline + "end";
            string result = t.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestMissingDictionaryValue()
        {
            TemplateGroup group = new TemplateGroup();
            group.DefineTemplate("test", "<m.foo>", new string[] { "m" });
            Template t = group.GetInstanceOf("test");
            t.Add("m", new Dictionary<string, string>());
            string expecting = "";
            string result = t.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestMissingDictionaryValue2()
        {
            TemplateGroup group = new TemplateGroup();
            group.DefineTemplate("test", "<if(m.foo)>[<m.foo>]<endif>", new string[] { "m" });
            Template t = group.GetInstanceOf("test");
            t.Add("m", new Dictionary<string, string>());
            string expecting = "";
            string result = t.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestMissingDictionaryValue3()
        {
            TemplateGroup group = new TemplateGroup();
            group.DefineTemplate("test", "<if(m.foo)>[<m.foo>]<endif>", new string[] { "m" });
            Template t = group.GetInstanceOf("test");
            t.Add("m", new Dictionary<string, string>() { { "foo", null } });
            string expecting = "";
            string result = t.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestSeparatorEmittedForEmptyIteratorValue()
        {
            Template st = new Template(
                "<values:{v|<if(v)>x<endif>}; separator=\" \">"
            );
            st.Add("values", new bool[] { true, false, true });
            StringWriter sw = new StringWriter();
            st.Write(new AutoIndentWriter(sw));
            string result = sw.ToString();
            string expecting = "x  x";
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestSeparatorEmittedForEmptyIteratorValu3333e()
        {
            string dir = tmpdir;
            string groupFile =
                "filter ::= [\"b\":, default: key]\n" +
                "t() ::= <%<[\"a\", \"b\", \"c\", \"b\"]:{it | <filter.(it)>}; separator=\",\">%>\n";
            writeFile(dir, "group.stg", groupFile);
            TemplateGroupFile group = new TemplateGroupFile(dir + "/group.stg");

            Template st = group.GetInstanceOf("t");
            StringWriter sw = new StringWriter();
            st.Write(new AutoIndentWriter(sw));
            string result = sw.ToString();
            string expecting = "a,,c,";
            Assert.AreEqual(expecting, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestSeparatorEmittedForEmptyIteratorValue2()
        {
            Template st = new Template(
                "<values; separator=\" \">"
            );
            st.Add("values", new string[] { "x", string.Empty, "y" });
            StringWriter sw = new StringWriter();
            st.Write(new AutoIndentWriter(sw));
            string result = sw.ToString();
            string expecting = "x  y";
            Assert.AreEqual(expecting, result);
        }
    }
}
