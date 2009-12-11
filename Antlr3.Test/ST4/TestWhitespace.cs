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
    using ST = StringTemplate.Template;
    using STErrorListener = StringTemplate.ITemplateErrorListener;
    using STGroup = StringTemplate.TemplateGroup;
    using String = System.String;
    using StringWriter = System.IO.StringWriter;

    [TestClass]
    public class TestWhitespace : StringTemplateTestBase
    {
        [TestMethod]
        public void TestTrimmedSubtemplates()
        {
            STGroup group = new STGroup();
            group.DefineTemplate("test", "<names:{n | <n>}>!");
            ST st = group.GetInstanceOf("test");
            st.Add("names", "Ter");
            st.Add("names", "Tom");
            st.Add("names", "Sumana");
            String expected = "TerTomSumana!";
            String result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestTrimJustOneWSInSubtemplates()
        {
            STGroup group = new STGroup();
            group.DefineTemplate("test", "<names:{n |  <n> }>!");
            ST st = group.GetInstanceOf("test");
            st.Add("names", "Ter");
            st.Add("names", "Tom");
            st.Add("names", "Sumana");
            String expected = " Ter  Tom  Sumana !";
            String result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestTrimNewlineInSubtemplates()
        {
            STGroup group = new STGroup();
            group.DefineTemplate("test", "<names:{n |\n" +
                                         "<n>}>!");
            ST st = group.GetInstanceOf("test");
            st.Add("names", "Ter");
            st.Add("names", "Tom");
            st.Add("names", "Sumana");
            String expected = "TerTomSumana!";
            String result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestLeaveNewlineOnEndInSubtemplates()
        {
            STGroup group = new STGroup();
            group.DefineTemplate("test", "<names:{n |\n" +
                                         "<n>\n" +
                                         "}>!");
            ST st = group.GetInstanceOf("test");
            st.Add("names", "Ter");
            st.Add("names", "Tom");
            st.Add("names", "Sumana");
            String expected = "Ter" + newline + "Tom" + newline + "Sumana" + newline + "!";
            String result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestEmptyExprAsFirstLineGetsNoOutput()
        {
            ST t = new ST(
                "<users>\n" +
                "end\n");
            String expecting = "end" + newline;
            String result = t.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestEmptyLineWithIndent()
        {
            ST t = new ST(
                "begin\n" +
                "    \n" +
                "end\n");
            String expecting = "begin" + newline + newline + "end" + newline;
            String result = t.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestSizeZeroOnLineByItselfGetsNoOutput()
        {
            ST t = new ST(
                "begin\n" +
                "<name>\n" +
                "<users>\n" +
                "<users>\n" +
                "end\n");
            String expecting = "begin" + newline + "end" + newline;
            String result = t.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestSizeZeroOnLineWithIndentGetsNoOutput()
        {
            ST t = new ST(
                "begin\n" +
                "  <name>\n" +
                "	<users>\n" +
                "	<users>\n" +
                "end\n");
            String expecting = "begin" + newline + "end" + newline;
            String result = t.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestSizeZeroOnLineWithMultipleExpr()
        {
            ST t = new ST(
                "begin\n" +
                "  <name>\n" +
                "	<users><users>\n" +
                "end\n");
            String expecting = "begin" + newline + "end" + newline;
            String result = t.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestIFExpr()
        {
            ST t = new ST(
                "begin\n" +
                "<if(x)><endif>\n" +
                "end\n");
            String expecting = "begin"+newline+"end"+newline;
            String result = t.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestIndentedIFExpr()
        {
            ST t = new ST(
                "begin\n" +
                "    <if(x)><endif>\n" +
                "end\n");
            String expecting = "begin" + newline + "end" + newline;
            String result = t.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestIFElseExpr()
        {
            ST t = new ST(
                "begin\n" +
                "<if(users)><else><endif>\n" +
                "end\n");
            String expecting = "begin" + newline + "end" + newline;
            String result = t.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestIFOnMultipleLines()
        {
            ST t = new ST(
                "begin\n" +
                "<if(users)>\n" +
                "foo\n" +
                "<else>\n" +
                "bar\n" +
                "<endif>\n" +
                "end\n");
            String expecting = "begin"+newline+"bar"+newline+"end"+newline;
            String result = t.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestNestedIFOnMultipleLines()
        {
            ST t = new ST(
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
            String expecting = "begin"+newline+"bar"+newline+"end"+newline;
            String result = t.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestLineBreak()
        {
            ST st = new ST(
                    "Foo <\\\\>" + newline +
                    "  \t  bar" + newline
                    );
            StringWriter sw = new StringWriter();
            st.Write(new AutoIndentWriter(sw, "\n")); // force \n as newline
            String result = sw.ToString();
            String expecting = "Foo bar\n";     // expect \n in output
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestLineBreak2()
        {
            ST st = new ST(
                    "Foo <\\\\>       " + newline +
                    "  \t  bar" + newline
                    );
            StringWriter sw = new StringWriter();
            st.Write(new AutoIndentWriter(sw, "\n")); // force \n as newline
            String result = sw.ToString();
            String expecting = "Foo bar\n";
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestLineBreakNoWhiteSpace()
        {
            ST st = new ST(
                    "Foo <\\\\>" + newline +
                    "bar\n"
                    );
            StringWriter sw = new StringWriter();
            st.Write(new AutoIndentWriter(sw, "\n")); // force \n as newline
            String result = sw.ToString();
            String expecting = "Foo bar\n";
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestNewlineNormalizationInTemplateString()
        {
            ST st = new ST(
                    "Foo\r\n" +
                    "Bar\n"
                    );
            StringWriter sw = new StringWriter();
            st.Write(new AutoIndentWriter(sw, "\n")); // force \n as newline
            String result = sw.ToString();
            String expecting = "Foo\nBar\n";     // expect \n in output
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestNewlineNormalizationInTemplateStringPC()
        {
            ST st = new ST(
                    "Foo\r\n" +
                    "Bar\n"
                    );
            StringWriter sw = new StringWriter();
            st.Write(new AutoIndentWriter(sw, "\r\n")); // force \r\n as newline
            String result = sw.ToString();
            String expecting = "Foo\r\nBar\r\n";     // expect \r\n in output
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestNewlineNormalizationInAttribute()
        {
            ST st = new ST(
                    "Foo\r\n" +
                    "<name>\n"
                    );
            st.Add("name", "a\nb\r\nc");
            StringWriter sw = new StringWriter();
            st.Write(new AutoIndentWriter(sw, "\n")); // force \n as newline
            String result = sw.ToString();
            String expecting = "Foo\na\nb\nc\n";     // expect \n in output
            Assert.AreEqual(expecting, result);
        }
    }
}
