/*
 * [The "BSD license"]
 *  Copyright (c) 2011 Terence Parr
 *  All rights reserved.
 *
 *  Redistribution and use in source and binary forms, with or without
 *  modification, are permitted provided that the following conditions
 *  are met:
 *  1. Redistributions of source code must retain the above copyright
 *     notice, this list of conditions and the following disclaimer.
 *  2. Redistributions in binary form must reproduce the above copyright
 *     notice, this list of conditions and the following disclaimer in the
 *     documentation and/or other materials provided with the distribution.
 *  3. The name of the author may not be used to endorse or promote products
 *     derived from this software without specific prior written permission.
 *
 *  THIS SOFTWARE IS PROVIDED BY THE AUTHOR ``AS IS'' AND ANY EXPRESS OR
 *  IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
 *  OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
 *  IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT, INDIRECT,
 *  INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
 *  NOT LIMITED TO, PROCUREMENT OF SUBTemplateITUTE GOODS OR SERVICES; LOSS OF USE,
 *  DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
 *  THEORY OF LIABILITY, WHETHER IN CONTRACT, TemplateRICT LIABILITY, OR TORT
 *  (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
 *  THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

namespace Antlr4.Test.StringTemplate
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Antlr4.StringTemplate;
    using Path = System.IO.Path;

    [TestClass]
    public class TestNoNewlineTemplates : BaseTest
    {
        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestNoNewlineTemplate()
        {
            string template =
                "t(x) ::= <%" + newline +
                "[  <if(!x)>" +
                "<else>" +
                "<x>" + newline +
                "<endif>" +
                "" + newline +
                "" + newline +
                "]" + newline +
                "" + newline +
                "%>" + newline;
            TemplateGroup g = new TemplateGroupString(template);
            Template st = g.GetInstanceOf("t");
            st.Add("x", 99);
            string expected = "[  99]";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestWSNoNewlineTemplate()
        {
            string template =
                "t(x) ::= <%" + newline +
                "" + newline +
                "%>" + newline;
            TemplateGroup g = new TemplateGroupString(template);
            Template st = g.GetInstanceOf("t");
            st.Add("x", 99);
            string expected = "";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestEmptyNoNewlineTemplate()
        {
            string template =
                "t(x) ::= <%%>" + newline;
            TemplateGroup g = new TemplateGroupString(template);
            Template st = g.GetInstanceOf("t");
            st.Add("x", 99);
            string expected = "";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestIgnoreIndent()
        {
            string template =
                "t(x) ::= <%" + newline +
                "	foo" + newline +
                "	<x>" + newline +
                "%>" + newline;
            TemplateGroup g = new TemplateGroupString(template);
            Template st = g.GetInstanceOf("t");
            st.Add("x", 99);
            string expected = "foo99";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestIgnoreIndentInIF()
        {
            string template =
                "t(x) ::= <%" + newline +
                "	<if(x)>" + newline +
                "		foo" + newline +
                "	<endif>" + newline +
                "	<x>" + newline +
                "%>" + newline;
            TemplateGroup g = new TemplateGroupString(template);
            Template st = g.GetInstanceOf("t");
            st.Add("x", 99);
            string expected = "foo99";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestKeepWS()
        {
            string template =
                "t(x) ::= <%" + newline +
                "	<x> <x> hi" + newline +
                "%>" + newline;
            TemplateGroup g = new TemplateGroupString(template);
            Template st = g.GetInstanceOf("t");
            st.Add("x", 99);
            string expected = "99 99 hi";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestRegion()
        {
            string template =
                "t(x) ::= <%\n" +
                "<@r>\n" +
                "	Ignore\n" +
                "	newlines and indents\n" +
                "<x>\n\n\n" +
                "<@end>\n" +
                "%>\n";
            TemplateGroup g = new TemplateGroupString(template);
            Template st = g.GetInstanceOf("t");
            st.Add("x", 99);
            string expected = "Ignorenewlines and indents99";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestDefineRegionInSubgroup()
        {
            string dir = tmpdir;
            string g1 = "a() ::= <<[<@r()>]>>\n";
            writeFile(dir, "g1.stg", g1);
            string g2 = "@a.r() ::= <%\n" +
            "	foo\n\n\n" +
            "%>\n";
            writeFile(dir, "g2.stg", g2);

            TemplateGroup group1 = new TemplateGroupFile(Path.Combine(dir, "g1.stg"));
            TemplateGroup group2 = new TemplateGroupFile(Path.Combine(dir, "g2.stg"));
            group2.ImportTemplates(group1); // define r in g2
            Template st = group2.GetInstanceOf("a");
            string expected = "[foo]";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }
    }
}
