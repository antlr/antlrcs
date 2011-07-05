/*
 * [The "BSD license"]
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
    using System.Collections.Generic;
    using Antlr4.StringTemplate;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using CultureInfo = System.Globalization.CultureInfo;
    using Path = System.IO.Path;

    [TestClass]
    public class TestRenderers : BaseTest
    {
#if false // date renderer
        [TestMethod]
        public void TestRendererForGroup()
        {
            string templates =
                    "dateThing(created) ::= \"datetime: <created>\"\n";
            writeFile(tmpdir, "t.stg", templates);
            STGroup group = new STGroupFile(Path.Combine(tmpdir, "t.stg"));
            group.registerRenderer(typeof(GregorianCalendar), new DateRenderer());
            ST st = group.getInstanceOf("dateThing");
            st.add("created", new GregorianCalendar(2005, 07 - 1, 05));
            string expecting = "datetime: 7/5/05 12:00 AM";
            string result = st.render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestRendererWithFormat()
        {
            string templates =
                    "dateThing(created) ::= << date: <created; format=\"yyyy.MM.dd\"> >>\n";
            writeFile(tmpdir, "t.stg", templates);
            STGroup group = new STGroupFile(Path.Combine(tmpdir, "t.stg"));
            group.registerRenderer(typeof(GregorianCalendar), new DateRenderer());
            ST st = group.getInstanceOf("dateThing");
            st.add("created", new GregorianCalendar(2005, 07 - 1, 05));
            string expecting = " date: 2005.07.05 ";
            string result = st.render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestRendererWithPredefinedFormat()
        {
            string templates =
                    "dateThing(created) ::= << datetime: <created; format=\"short\"> >>\n";
            writeFile(tmpdir, "t.stg", templates);
            STGroup group = new STGroupFile(Path.Combine(tmpdir, "t.stg"));
            group.registerRenderer(typeof(GregorianCalendar), new DateRenderer());
            ST st = group.getInstanceOf("dateThing");
            st.add("created", new GregorianCalendar(2005, 07 - 1, 05));
            string expecting = " datetime: 7/5/05 12:00 AM ";
            string result = st.render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestRendererWithPredefinedFormat2()
        {
            string templates =
                    "dateThing(created) ::= << datetime: <created; format=\"full\"> >>\n";
            writeFile(tmpdir, "t.stg", templates);
            STGroup group = new STGroupFile(Path.Combine(tmpdir, "t.stg"));
            group.registerRenderer(typeof(GregorianCalendar), new DateRenderer());
            ST st = group.getInstanceOf("dateThing");
            st.add("created", new GregorianCalendar(2005, 07 - 1, 05));
            string expecting = " datetime: Tuesday, July 5, 2005 12:00:00 AM PDT ";
            string result = st.render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestRendererWithPredefinedFormat3()
        {
            string templates =
                    "dateThing(created) ::= << date: <created; format=\"date:medium\"> >>\n";

            writeFile(tmpdir, "t.stg", templates);
            STGroup group = new STGroupFile(Path.Combine(tmpdir, "t.stg"));
            group.registerRenderer(typeof(GregorianCalendar), new DateRenderer());
            ST st = group.getInstanceOf("dateThing");
            st.add("created", new GregorianCalendar(2005, 07 - 1, 05));
            string expecting = " date: Jul 5, 2005 ";
            string result = st.render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestRendererWithPredefinedFormat4()
        {
            string templates =
                    "dateThing(created) ::= << time: <created; format=\"time:medium\"> >>\n";

            writeFile(tmpdir, "t.stg", templates);
            STGroup group = new STGroupFile(Path.Combine(tmpdir, "t.stg"));
            group.registerRenderer(typeof(GregorianCalendar), new DateRenderer());
            ST st = group.getInstanceOf("dateThing");
            st.add("created", new GregorianCalendar(2005, 07 - 1, 05));
            string expecting = " time: 12:00:00 AM ";
            string result = st.render();
            Assert.AreEqual(expecting, result);
        }
#endif

        [TestMethod]
        public void TestStringRendererWithPrintfFormat()
        {
            string templates =
                    "foo(x) ::= << <x; format=\"{0,6}\"> >>\n";

            writeFile(tmpdir, "t.stg", templates);
            TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
            group.RegisterRenderer(typeof(string), new StringRenderer());
            Template st = group.GetInstanceOf("foo");
            st.Add("x", "hi");
            string expecting = "     hi ";
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
            List<string> names = new List<string>();
            names.Add("ter");
            names.Add(null);
            names.Add("sriram");
            st.Add("names", names);
            string expecting = "The names: TER and N/A and SRIRAM";
            string result = st.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestStringRendererWithFormat_cap()
        {
            string templates =
                    "foo(x) ::= << <x; format=\"cap\"> >>\n";

            writeFile(tmpdir, "t.stg", templates);
            TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
            group.RegisterRenderer(typeof(string), new StringRenderer());
            Template st = group.GetInstanceOf("foo");
            st.Add("x", "hi");
            string expecting = " Hi ";
            string result = st.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestStringRendererWithFormat_cap_emptyValue()
        {
            string templates =
                    "foo(x) ::= << <x; format=\"cap\"> >>\n";

            writeFile(tmpdir, "t.stg", templates);
            TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
            group.RegisterRenderer(typeof(string), new StringRenderer());
            Template st = group.GetInstanceOf("foo");
            st.Add("x", "");
            string expecting = " ";//FIXME: why not two spaces?
            string result = st.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestStringRendererWithFormat_url_encode()
        {
            string templates =
                    "foo(x) ::= << <x; format=\"url-encode\"> >>\n";

            writeFile(tmpdir, "t.stg", templates);
            TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
            group.RegisterRenderer(typeof(string), new StringRenderer());
            Template st = group.GetInstanceOf("foo");
            st.Add("x", "a b");
            string expecting = " a+b ";
            string result = st.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestStringRendererWithFormat_xml_encode()
        {
            string templates =
                    "foo(x) ::= << <x; format=\"xml-encode\"> >>\n";

            writeFile(tmpdir, "t.stg", templates);
            TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
            group.RegisterRenderer(typeof(string), new StringRenderer());
            Template st = group.GetInstanceOf("foo");
            st.Add("x", "a<b> &\t\b");
            string expecting = " a&lt;b&gt; &amp;\t\b ";
            string result = st.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestStringRendererWithFormat_xml_encode_null()
        {
            string templates =
                    "foo(x) ::= << <x; format=\"xml-encode\"> >>\n";

            writeFile(tmpdir, "t.stg", templates);
            TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
            group.RegisterRenderer(typeof(string), new StringRenderer());
            Template st = group.GetInstanceOf("foo");
            st.Add("x", null);
            string expecting = " ";
            string result = st.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestNumberRendererWithPrintfFormat()
        {
            //string templates = "foo(x,y) ::= << <x; format=\"%d\"> <y; format=\"%2.3f\"> >>\n";
            string templates = "foo(x,y) ::= << <x; format=\"{0}\"> <y; format=\"{0:0.000}\"> >>\n";

            writeFile(tmpdir, "t.stg", templates);
            TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
            group.RegisterRenderer(typeof(int), new NumberRenderer());
            group.RegisterRenderer(typeof(double), new NumberRenderer());
            Template st = group.GetInstanceOf("foo");
            st.Add("x", -2100);
            st.Add("y", 3.14159);
            string expecting = " -2100 3.142 ";
            string result = st.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestInstanceofRenderer()
        {
            string templates =
                    "numberThing(x,y,z) ::= \"numbers: <x>, <y>; <z>\"\n";
            writeFile(tmpdir, "t.stg", templates);
            TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
            group.RegisterRenderer(typeof(int), new NumberRenderer());
            group.RegisterRenderer(typeof(double), new NumberRenderer());
            Template st = group.GetInstanceOf("numberThing");
            st.Add("x", -2100);
            st.Add("y", 3.14159);
            st.Add("z", "hi");
            string expecting = "numbers: -2100, 3.14159; hi";
            string result = st.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestLocaleWithNumberRenderer()
        {
            //string templates = "foo(x,y) ::= << <x; format=\"%,d\"> <y; format=\"%,2.3f\"> >>\n";
            string templates = "foo(x,y) ::= << <x; format=\"{0:#,#}\"> <y; format=\"{0:0.000}\"> >>\n";

            writeFile(tmpdir, "t.stg", templates);
            TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
            group.RegisterRenderer(typeof(int), new NumberRenderer());
            group.RegisterRenderer(typeof(double), new NumberRenderer());
            Template st = group.GetInstanceOf("foo");
            st.Add("x", -2100);
            st.Add("y", 3.14159);
            // Polish uses ' ' (ASCII 160) for ',' and ',' for '.'
            string expecting = " -2 100 3,142 "; // Ê
            string result = st.Render(new CultureInfo("pl"));
            Assert.AreEqual(expecting, result);
        }
    }
}
