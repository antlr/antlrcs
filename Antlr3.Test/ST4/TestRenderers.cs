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
    using StringTemplate;
    using ArrayList = System.Collections.ArrayList;
    using DateTime = System.DateTime;
    using IList = System.Collections.IList;

    [TestClass]
    public class TestRenderers : StringTemplateTestBase
    {
        public class DateRenderer : IAttributeRenderer
        {
            public string ToString(object o)
            {
                DateTime dateTime = (DateTime)o;
                return dateTime.ToString("yyyy.MM.dd");
            }

            public string ToString(object o, string formatString)
            {
                return ToString(o);
            }
        }

        public class DateRenderer2 : IAttributeRenderer
        {
            public string ToString(object o)
            {
                DateTime dateTime = (DateTime)o;
                return dateTime.ToString("yyyy/MM/dd");
            }

            public string ToString(object o, string formatString)
            {
                return ToString(o);
            }
        }

        public class DateRenderer3 : IAttributeRenderer
        {
            public string ToString(object o)
            {
                DateTime dateTime = (DateTime)o;
                return dateTime.ToString("yyyy/MM/dd");
            }

            public string ToString(object o, string formatString)
            {
                DateTime dateTime = (DateTime)o;
                return dateTime.ToString(formatString);
            }
        }

        public class StringRenderer : IAttributeRenderer
        {
            public string ToString(object o)
            {
                return (string)o;
            }

            public string ToString(object o, string formatString)
            {
                if (formatString.Equals("upper"))
                {
                    return ((string)o).ToUpperInvariant();
                }
                return ToString(o);
            }
        }

        [TestMethod]
        public void TestRendererForGroup()
        {
            string templates =
                    "dateThing(created) ::= \"date: <created>\"\n";
            WriteFile(tmpdir, "t.stg", templates);
            TemplateGroup group = new TemplateGroupFile(tmpdir + "/t.stg");
            group.RegisterRenderer(typeof(DateTime), new DateRenderer());
            Template st = group.GetInstanceOf("dateThing");
            st.Add("created", new DateTime(2005, 7, 5));
            string expecting = "date: 2005.07.05";
            string result = st.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestRendererWithFormat()
        {
            string templates =
                    "dateThing(created) ::= << date: <created; format=\"yyyy.MM.dd\"> >>\n";
            WriteFile(tmpdir, "t.stg", templates);
            TemplateGroup group = new TemplateGroupFile(tmpdir + "/t.stg");
            group.RegisterRenderer(typeof(DateTime), new DateRenderer3());
            Template st = group.GetInstanceOf("dateThing");
            st.Add("created", new DateTime(2005, 7, 5));
            string expecting = " date: 2005.07.05 ";
            string result = st.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestRendererWithFormatAndList()
        {
            string template =
                    "The names: <names; format=\"upper\">";
            TemplateGroup group = new TemplateGroup();
            group.RegisterRenderer(typeof(string), new StringRenderer());
            Template st = new Template(group, template);
            st.Add("names", "ter");
            st.Add("names", "tom");
            st.Add("names", "sriram");
            string expecting = "The names: TERTOMSRIRAM";
            string result = st.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestRendererWithFormatAndSeparator()
        {
            string template =
                    "The names: <names; separator=\" and \", format=\"upper\">";
            TemplateGroup group = new TemplateGroup();
            group.RegisterRenderer(typeof(string), new StringRenderer());
            Template st = new Template(group, template);
            st.Add("names", "ter");
            st.Add("names", "tom");
            st.Add("names", "sriram");
            string expecting = "The names: TER and TOM and SRIRAM";
            string result = st.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestRendererWithFormatAndSeparatorAndNull()
        {
            string template =
                    "The names: <names; separator=\" and \", null=\"n/a\", format=\"upper\">";
            TemplateGroup group = new TemplateGroup();
            group.RegisterRenderer(typeof(string), new StringRenderer());
            Template st = new Template(group, template);
            IList names = new ArrayList();
            names.Add("ter");
            names.Add(null);
            names.Add("sriram");
            st.Add("names", names);
            string expecting = "The names: TER and N/A and SRIRAM";
            string result = st.Render();
            Assert.AreEqual(expecting, result);
        }
    }
}
