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
    using System.Collections.Generic;
    using Antlr4.StringTemplate;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using StringWriter = System.IO.StringWriter;

    [TestClass]
    public class TestWhitespace : BaseTest
    {
        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestTrimmedSubtemplates()
        {
            TemplateGroup group = new TemplateGroup();
            group.DefineTemplate("test", "<names:{n | <n>}>!", new string[] { "names" });
            Template st = group.GetInstanceOf("test");
            st.Add("names", "Ter");
            st.Add("names", "Tom");
            st.Add("names", "Sumana");
            string expected = "TerTomSumana!";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestTrimmedNewlinesBeforeAfterInTemplate()
        {
            string templates =
                "a(x) ::= <<" + newline +
                "foo" + newline +
                ">>" + newline;
            TemplateGroupString group = new TemplateGroupString(templates);
            Template st = group.GetInstanceOf("a");
            string expected = "foo";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestDontTrimJustSpaceBeforeAfterInTemplate()
        {
            string templates =
                "a(x) ::= << foo >>\n";
            TemplateGroupString group = new TemplateGroupString(templates);
            Template st = group.GetInstanceOf("a");
            string expected = " foo ";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestTrimmedSubtemplatesNoArgs()
        {
            TemplateGroup group = new TemplateGroup();
            group.DefineTemplate("test", "[<foo({ foo })>]");
            group.DefineTemplate("foo", "<x>", new string[] { "x" });
            Template st = group.GetInstanceOf("test");
            string expected = "[ foo ]";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestTrimmedSubtemplatesArgs()
        {
            TemplateGroup group = new TemplateGroup();
            group.DefineTemplate("test", "<names:{x|  foo }>", new string[] { "names" });
            Template st = group.GetInstanceOf("test");
            st.Add("names", "Ter");
            st.Add("names", "Tom");
            st.Add("names", "Sumana");
            string expected = " foo  foo  foo ";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestTrimJustOneWSInSubtemplates()
        {
            TemplateGroup group = new TemplateGroup();
            group.DefineTemplate("test", "<names:{n |  <n> }>!", new string[] { "names" });
            Template st = group.GetInstanceOf("test");
            st.Add("names", "Ter");
            st.Add("names", "Tom");
            st.Add("names", "Sumana");
            string expected = " Ter  Tom  Sumana !";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestTrimNewlineInSubtemplates()
        {
            TemplateGroup group = new TemplateGroup();
            group.DefineTemplate("test", "<names:{n |\n" +
                                         "<n>}>!", new string[] { "names" });
            Template st = group.GetInstanceOf("test");
            st.Add("names", "Ter");
            st.Add("names", "Tom");
            st.Add("names", "Sumana");
            string expected = "TerTomSumana!";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestLeaveNewlineOnEndInSubtemplates()
        {
            TemplateGroup group = new TemplateGroup();
            group.DefineTemplate("test", "<names:{n |\n" +
                                         "<n>\n" +
                                         "}>!", new string[] { "names" });
            Template st = group.GetInstanceOf("test");
            st.Add("names", "Ter");
            st.Add("names", "Tom");
            st.Add("names", "Sumana");
            string expected = "Ter" + newline + "Tom" + newline + "Sumana" + newline + "!";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [Ignore]
        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestTabBeforeEndInSubtemplates()
        {
            // fails since it counts indent from outer too
            TemplateGroup group = new TemplateGroup();
            group.DefineTemplate("test", "  <names:{n |\n" +
                                         "    <n>\n" +
                                         "  }>!", new string[] { "names" });
            Template st = group.GetInstanceOf("test");
            st.Add("names", "Ter");
            st.Add("names", "Tom");
            st.Add("names", "Sumana");
            string expected =
                "    Ter" + newline +
                "    Tom" + newline +
                "    Sumana" + newline +
                "!";
            string result = st.Render();
            st.impl.Dump();
            Assert.AreEqual(expected, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestEmptyExprAsFirstLineGetsNoOutput()
        {
            Template t = new Template(
                "<users>\n" +
                "end\n");
            string expecting = "end" + newline;
            string result = t.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestEmptyLineWithIndent()
        {
            Template t = new Template(
                "begin\n" +
                "    \n" +
                "end\n");
            string expecting = "begin" + newline + newline + "end" + newline;
            string result = t.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestEmptyLine()
        {
            Template t = new Template(
                "begin\n" +
                "\n" +
                "end\n");
            string expecting = "begin" + newline + newline + "end" + newline;
            string result = t.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestSizeZeroOnLineByItselfGetsNoOutput()
        {
            Template t = new Template(
                "begin\n" +
                "<name>\n" +
                "<users>\n" +
                "<users>\n" +
                "end\n");
            string expecting = "begin" + newline + "end" + newline;
            string result = t.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestSizeZeroOnLineWithIndentGetsNoOutput()
        {
            Template t = new Template(
                "begin\n" +
                "  <name>\n" +
                "	<users>\n" +
                "	<users>\n" +
                "end\n");
            string expecting = "begin" + newline + "end" + newline;
            string result = t.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestSizeZeroOnLineWithMultipleExpr()
        {
            Template t = new Template(
                "begin\n" +
                "  <name>\n" +
                "	<users><users>\n" +
                "end\n");
            string expecting = "begin" + newline + "end" + newline;
            string result = t.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestIFExpr()
        {
            Template t = new Template(
                "begin\n" +
                "<if(x)><endif>\n" +
                "end\n");
            string expecting = "begin" + newline + "end" + newline;
            string result = t.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestIndentedIFExpr()
        {
            Template t = new Template(
                "begin\n" +
                "    <if(x)><endif>\n" +
                "end\n");
            string expecting = "begin" + newline + "end" + newline;
            string result = t.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestIFElseExprOnSingleLine()
        {
            Template t = new Template(
                "begin\n" +
                "<if(users)><else><endif>\n" +
                "end\n");
            string expecting = "begin" + newline + "end" + newline;
            string result = t.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestIFOnMultipleLines()
        {
            Template t = new Template(
                "begin\n" +
                "<if(users)>\n" +
                "foo\n" +
                "<else>\n" +
                "bar\n" +
                "<endif>\n" +
                "end\n");
            string expecting = "begin" + newline + "bar" + newline + "end" + newline;
            string result = t.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestElseifOnMultipleLines()
        {
            Template t = new Template(
                "begin\n" +
                "<if(a)>\n" +
                "foo\n" +
                "<elseif(b)>\n" +
                "bar\n" +
                "<endif>\n" +
                "end\n");
            string expecting = "begin" + newline + "end" + newline;
            string result = t.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestElseifOnMultipleLines2()
        {
            Template t = new Template(
                "begin\n" +
                "<if(a)>\n" +
                "foo\n" +
                "<elseif(b)>\n" +
                "bar\n" +
                "<endif>\n" +
                "end\n");
            t.Add("b", true);
            string expecting = "begin" + newline + "bar" + newline + "end" + newline;
            string result = t.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestElseifOnMultipleLines3()
        {
            Template t = new Template(
                "begin\n" +
                "  <if(a)>\n" +
                "  foo\n" +
                "  <elseif(b)>\n" +
                "  bar\n" +
                "  <endif>\n" +
                "end\n");
            t.Add("a", true);
            string expecting = "begin" + newline + "  foo" + newline + "end" + newline;
            string result = t.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestEndifNotOnLineAlone()
        {
            Template t = new Template(
                "begin\n" +
                "  <if(users)>\n" +
                "  foo\n" +
                "  <else>\n" +
                "  bar\n" +
                "  <endif>end\n");
            string expecting = "begin" + newline + "  bar" + newline + "end" + newline;
            string result = t.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestNestedIFOnMultipleLines()
        {
            Template t = new Template(
                "begin\n" +
                "<if(x)>\n" +
                "<if(y)>\n" +
                "foo\n" +
                "<else>\n" +
                "bar\n" +
                "<endif>\n" +
                "<endif>\n" +
                "end\n");
            t.Add("x", "x");
            string expecting = "begin" + newline + "bar" + newline + "end" + newline;
            string result = t.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestIfElseifOnMultipleLines()
        {
            Template t = new Template(
                "begin\n" +
                "<if(x&&y)>\n" +
                "foo\n" +
                "<elseif(x)>\n" +
                "bar\n" +
                "<endif>\n" +
                "end\n");
            t.Add("x", "x");
            string expecting = "begin" + newline + "bar" + newline + "end" + newline;
            string result = t.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestLineBreak()
        {
            Template st = new Template(
                    "Foo <\\\\>" + newline +
                    "  \t  bar" + newline
                    );
            StringWriter sw = new StringWriter();
            st.Write(new AutoIndentWriter(sw, "\n")); // force \n as newline
            string result = sw.ToString();
            string expecting = "Foo bar\n";     // expect \n in output
            Assert.AreEqual(expecting, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestLineBreak2()
        {
            Template st = new Template(
                    "Foo <\\\\>       " + newline +
                    "  \t  bar" + newline
                    );
            StringWriter sw = new StringWriter();
            st.Write(new AutoIndentWriter(sw, "\n")); // force \n as newline
            string result = sw.ToString();
            string expecting = "Foo bar\n";
            Assert.AreEqual(expecting, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestLineBreakNoWhiteSpace()
        {
            Template st = new Template(
                    "Foo <\\\\>" + newline +
                    "bar\n"
                    );
            StringWriter sw = new StringWriter();
            st.Write(new AutoIndentWriter(sw, "\n")); // force \n as newline
            string result = sw.ToString();
            string expecting = "Foo bar\n";
            Assert.AreEqual(expecting, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestNewlineNormalizationInTemplateString()
        {
            Template st = new Template(
                    "Foo\r\n" +
                    "Bar\n"
                    );
            StringWriter sw = new StringWriter();
            st.Write(new AutoIndentWriter(sw, "\n")); // force \n as newline
            string result = sw.ToString();
            string expecting = "Foo\nBar\n";     // expect \n in output
            Assert.AreEqual(expecting, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestNewlineNormalizationInTemplateStringPC()
        {
            Template st = new Template(
                    "Foo\r\n" +
                    "Bar\n"
                    );
            StringWriter sw = new StringWriter();
            st.Write(new AutoIndentWriter(sw, "\r\n")); // force \r\n as newline
            string result = sw.ToString();
            string expecting = "Foo\r\nBar\r\n";     // expect \r\n in output
            Assert.AreEqual(expecting, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestNewlineNormalizationInAttribute()
        {
            Template st = new Template(
                    "Foo\r\n" +
                    "<name>\n"
                    );
            st.Add("name", "a\nb\r\nc");
            StringWriter sw = new StringWriter();
            st.Write(new AutoIndentWriter(sw, "\n")); // force \n as newline
            string result = sw.ToString();
            string expecting = "Foo\na\nb\nc\n";     // expect \n in output
            Assert.AreEqual(expecting, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestNullIterationLineGivesNoOutput()
        {
            Template t = new Template(
                "begin\n" +
                "<items:{x|<x>}>\n" +
                "end\n");
            string expecting = "begin" + newline + "end" + newline;
            string result = t.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestEmptyIterationLineGivesNoOutput()
        {
            Template t = new Template(
                "begin\n" +
                "  <items:{x|<x>}>\n" +
                "end\n");
            t.Add("items", new List<object>());
            string expecting = "begin" + newline + "end" + newline;
            string result = t.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestCommentOnlyLineGivesNoOutput()
        {
            Template t = new Template(
                "begin\n" +
                "<! ignore !>\n" +
                "end\n");
            string expecting = "begin" + newline + "end" + newline;
            string result = t.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestCommentOnlyLineGivesNoOutput2()
        {
            Template t = new Template(
                "begin\n" +
                "    <! ignore !>\n" +
                "end\n");
            string expecting = "begin" + newline + "end" + newline;
            string result = t.Render();
            Assert.AreEqual(expecting, result);
        }
    }
}
