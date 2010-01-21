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
    using StringTemplate;
    using StringWriter = System.IO.StringWriter;
    using Path = System.IO.Path;

    [TestClass]
    public class TestLineWrap : StringTemplateTestBase
    {
        [TestMethod]
        public void TestLineWrap1()
        {
            String templates =
                "array(values) ::= <<int[] a = { <values; wrap=\"\\n\", separator=\",\"> };>>" + newline;
            WriteFile(tmpdir, "t.stg", templates);
            STGroup group = new STGroupFile(Path.Combine(tmpdir, "t.stg"));

            ST a = group.GetInstanceOf("array");
            a.Add("values",
                           new int[] {3,9,20,2,1,4,6,32,5,6,77,888,2,1,6,32,5,6,77,
						4,9,20,2,1,4,63,9,20,2,1,4,6,32,5,6,77,6,32,5,6,77,
					    3,9,20,2,1,4,6,32,5,6,77,888,1,6,32,5});
            String expecting =
                "int[] a = { 3,9,20,2,1,4,6,32,5,6,77,888,\n" +
                "2,1,6,32,5,6,77,4,9,20,2,1,4,63,9,20,2,1,\n" +
                "4,6,32,5,6,77,6,32,5,6,77,3,9,20,2,1,4,6,\n" +
                "32,5,6,77,888,1,6,32,5 };";

            StringWriter sw = new StringWriter();
            ITemplateWriter stw = new AutoIndentWriter(sw, "\n"); // force \n as newline
            stw.SetLineWidth(40);
            a.Write(stw);
            String result = sw.ToString();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestLineWrapAnchored()
        {
            String templates =
                    "array(values) ::= <<int[] a = { <values; anchor, wrap, separator=\",\"> };>>" + newline;
            WriteFile(tmpdir, "t.stg", templates);
            STGroup group = new STGroupFile(Path.Combine(tmpdir, "t.stg"));

            ST a = group.GetInstanceOf("array");
            a.Add("values",
                           new int[] {3,9,20,2,1,4,6,32,5,6,77,888,2,1,6,32,5,6,77,
						4,9,20,2,1,4,63,9,20,2,1,4,6,32,5,6,77,6,32,5,6,77,
					    3,9,20,2,1,4,6,32,5,6,77,888,1,6,32,5});
            String expecting =
                "int[] a = { 3,9,20,2,1,4,6,32,5,6,77,888," + newline +
                "            2,1,6,32,5,6,77,4,9,20,2,1,4," + newline +
                "            63,9,20,2,1,4,6,32,5,6,77,6," + newline +
                "            32,5,6,77,3,9,20,2,1,4,6,32," + newline +
                "            5,6,77,888,1,6,32,5 };";
            Assert.AreEqual(expecting, a.Render(40));
        }

        [TestMethod]
        public void TestSubtemplatesAnchorToo()
        {
            String templates =
                    "array(values) ::= <<{ <values; anchor, separator=\", \"> }>>" + newline;
            WriteFile(tmpdir, "t.stg", templates);
            STGroup group = new STGroupFile(Path.Combine(tmpdir, "t.stg"));

            ST x = new ST("<\\n>{ <stuff; anchor, separator=\",\\n\"> }<\\n>");
            x.groupThatCreatedThisInstance = group;
            x.Add("stuff", "1");
            x.Add("stuff", "2");
            x.Add("stuff", "3");
            ST a = group.GetInstanceOf("array");
            a.Add("values", new ArrayList() { "a", x, "b" });
            String expecting =
                "{ a," + newline +
                "  { 1," + newline +
                "    2," + newline +
                "    3 }" + newline +
                "  , b }";
            Assert.AreEqual(expecting, a.Render(40));
        }

        [TestMethod]
        public void TestFortranLineWrap()
        {
            String templates =
                    "func(args) ::= <<       FUNCTION line( <args; wrap=\"\\n      c\", separator=\",\"> )>>" + newline;
            WriteFile(tmpdir, "t.stg", templates);
            STGroup group = new STGroupFile(tmpdir + "/" + "t.stg");

            ST a = group.GetInstanceOf("func");
            a.Add("args",
                           new String[] { "a", "b", "c", "d", "e", "f" });
            String expecting =
                "       FUNCTION line( a,b,c,d," + newline +
                "      ce,f )";
            Assert.AreEqual(expecting, a.Render(30));
        }

        [TestMethod]
        public void TestLineWrapWithDiffAnchor()
        {
            String templates =
                    "array(values) ::= <<int[] a = { <{1,9,2,<values; wrap, separator=\",\">}; anchor> };>>" + newline;
            WriteFile(tmpdir, "t.stg", templates);
            STGroup group = new STGroupFile(tmpdir + "/" + "t.stg");

            ST a = group.GetInstanceOf("array");
            a.Add("values",
                           new int[] {3,9,20,2,1,4,6,32,5,6,77,888,2,1,6,32,5,6,77,
						4,9,20,2,1,4,63,9,20,2,1,4,6});
            String expecting =
                "int[] a = { 1,9,2,3,9,20,2,1,4," + newline +
                "            6,32,5,6,77,888,2," + newline +
                "            1,6,32,5,6,77,4,9," + newline +
                "            20,2,1,4,63,9,20,2," + newline +
                "            1,4,6 };";
            Assert.AreEqual(expecting, a.Render(30));
        }

        [TestMethod]
        public void TestLineWrapEdgeCase()
        {
            String templates =
                    "duh(chars) ::= <<<chars; wrap=\"\\n\"\\>>>" + newline;
            WriteFile(tmpdir, "t.stg", templates);
            STGroup group = new STGroupFile(tmpdir + "/" + "t.stg");

            ST a = group.GetInstanceOf("duh");
            a.Add("chars", new String[] { "a", "b", "c", "d", "e" });
            // lineWidth==3 implies that we can have 3 characters at most
            String expecting =
                "abc" + newline +
                "de";
            Assert.AreEqual(expecting, a.Render(3));
        }

        [TestMethod]
        public void TestLineWrapLastCharIsNewline()
        {
            String templates =
                    "duh(chars) ::= <<<chars; wrap=\"\\n\"\\>>>" + newline;
            WriteFile(tmpdir, "t.stg", templates);
            STGroup group = new STGroupFile(Path.Combine(tmpdir, "t.stg"));

            ST a = group.GetInstanceOf("duh");
            a.Add("chars", new String[] { "a", "b", "\n", "d", "e" });
            // don't do \n if it's last element anyway
            String expecting =
                "ab" + newline +
                "de";
            Assert.AreEqual(expecting, a.Render(3));
        }

        [TestMethod]
        public void TestLineWrapCharAfterWrapIsNewline()
        {
            String templates =
                    "duh(chars) ::= <<<chars; wrap=\"\\n\"\\>>>" + newline;
            WriteFile(tmpdir, "t.stg", templates);
            STGroup group = new STGroupFile(tmpdir + "/" + "t.stg");

            ST a = group.GetInstanceOf("duh");
            a.Add("chars", new String[] { "a", "b", "c", "\n", "d", "e" });
            // Once we wrap, we must dump chars as we see them.  A newline right
            // after a wrap is just an "unfortunate" event.  People will expect
            // a newline if it's in the data.
            String expecting =
                "abc" + newline +
                "" + newline +
                "de";
            Assert.AreEqual(expecting, a.Render(3));
        }

        [TestMethod]
        public void TestLineWrapForList()
        {
            String templates =
                    "duh(data) ::= <<!<data; wrap>!>>" + newline;
            WriteFile(tmpdir, "t.stg", templates);
            STGroup group = new STGroupFile(tmpdir + "/" + "t.stg");

            ST a = group.GetInstanceOf("duh");
            a.Add("data", new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            String expecting =
                "!123" + newline +
                "4567" + newline +
                "89!";
            Assert.AreEqual(expecting, a.Render(4));
        }

        [TestMethod]
        public void TestLineWrapForAnonTemplate()
        {
            String templates =
                    "duh(data) ::= <<!<data:{v|[<v>]}; wrap>!>>" + newline;
            WriteFile(tmpdir, "t.stg", templates);
            STGroup group = new STGroupFile(tmpdir + "/" + "t.stg");

            ST a = group.GetInstanceOf("duh");
            a.Add("data", new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            String expecting =
                "![1][2][3]" + newline + // width=9 is the 3 char; don't break til after ]
                "[4][5][6]" + newline +
                "[7][8][9]!";
            Assert.AreEqual(expecting, a.Render(9));
        }

        [TestMethod]
        public void TestLineWrapForAnonTemplateAnchored()
        {
            String templates =
                    "duh(data) ::= <<!<data:{v|[<v>]}; anchor, wrap>!>>" + newline;
            WriteFile(tmpdir, "t.stg", templates);
            STGroup group = new STGroupFile(tmpdir + "/" + "t.stg");

            ST a = group.GetInstanceOf("duh");
            a.Add("data", new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            String expecting =
                "![1][2][3]" + newline +
                " [4][5][6]" + newline +
                " [7][8][9]!";
            Assert.AreEqual(expecting, a.Render(9));
        }

        [TestMethod]
        public void TestLineWrapForAnonTemplateComplicatedWrap()
        {
            String templates =
                    "top(s) ::= <<  <s>.>>" +
                    "str(data) ::= <<!<data:{v|[<v>]}; wrap=\"!+\\n!\">!>>" + newline;
            WriteFile(tmpdir, "t.stg", templates);
            STGroup group = new STGroupFile(tmpdir + "/" + "t.stg");

            ST t = group.GetInstanceOf("top");
            ST s = group.GetInstanceOf("str");
            s.Add("data", new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            t.Add("s", s);
            String expecting =
                "  ![1][2]!+" + newline +
                "  ![3][4]!+" + newline +
                "  ![5][6]!+" + newline +
                "  ![7][8]!+" + newline +
                "  ![9]!.";
            Assert.AreEqual(expecting, t.Render(9));
        }

        [TestMethod]
        public void TestIndentBeyondLineWidth()
        {
            String templates =
                    "duh(chars) ::= <<    <chars; wrap=\"\\n\"\\>>>" + newline;
            WriteFile(tmpdir, "t.stg", templates);
            STGroup group = new STGroupFile(tmpdir + "/" + "t.stg");

            ST a = group.GetInstanceOf("duh");
            a.Add("chars", new String[] { "a", "b", "c", "d", "e" });
            //
            String expecting =
                "    a" + newline +
                "    b" + newline +
                "    c" + newline +
                "    d" + newline +
                "    e";
            Assert.AreEqual(expecting, a.Render(2));
        }

        [TestMethod]
        public void TestIndentedExpr()
        {
            String templates =
                    "duh(chars) ::= <<    <chars; wrap=\"\\n\"\\>>>" + newline;
            WriteFile(tmpdir, "t.stg", templates);
            STGroup group = new STGroupFile(tmpdir + "/" + "t.stg");

            ST a = group.GetInstanceOf("duh");
            a.Add("chars", new String[] { "a", "b", "c", "d", "e" });
            //
            String expecting =
                "    ab" + newline +
                "    cd" + newline +
                "    e";
            // width=4 spaces + 2 char.
            Assert.AreEqual(expecting, a.Render(6));
        }

        [TestMethod]
        public void TestNestedIndentedExpr()
        {
            String templates =
                    "top(d) ::= <<  <d>!>>" + newline +
                    "duh(chars) ::= <<  <chars; wrap=\"\\n\"\\>>>" + newline;
            WriteFile(tmpdir, "t.stg", templates);
            STGroup group = new STGroupFile(tmpdir + "/" + "t.stg");

            ST top = group.GetInstanceOf("top");
            ST duh = group.GetInstanceOf("duh");
            duh.Add("chars", new String[] { "a", "b", "c", "d", "e" });
            top.Add("d", duh);
            String expecting =
                "    ab" + newline +
                "    cd" + newline +
                "    e!";
            // width=4 spaces + 2 char.
            Assert.AreEqual(expecting, top.Render(6));
        }

        [TestMethod]
        public void TestNestedWithIndentAndTrackStartOfExpr()
        {
            String templates =
                    "top(d) ::= <<  <d>!>>" + newline +
                    "duh(chars) ::= <<x: <chars; anchor, wrap=\"\\n\"\\>>>" + newline;
            WriteFile(tmpdir, "t.stg", templates);
            STGroup group = new STGroupFile(tmpdir + "/" + "t.stg");

            ST top = group.GetInstanceOf("top");
            ST duh = group.GetInstanceOf("duh");
            duh.Add("chars", new String[] { "a", "b", "c", "d", "e" });
            top.Add("d", duh);
            //
            String expecting =
                "  x: ab" + newline +
                "     cd" + newline +
                "     e!";
            Assert.AreEqual(expecting, top.Render(7));
        }

        [TestMethod]
        public void TestLineDoesNotWrapDueToLiteral()
        {
            String templates =
                    "m(args,body) ::= <<[TestMethod] public voidfoo(<args; wrap=\"\\n\",separator=\", \">) throws Ick { <body> }>>" + newline;
            WriteFile(tmpdir, "t.stg", templates);
            STGroup group = new STGroupFile(tmpdir + "/" + "t.stg");

            ST a = group.GetInstanceOf("m");
            a.Add("args",
                           new String[] { "a", "b", "c" });
            a.Add("body", "i=3;");
            // make it wrap because of ") throws Ick { " literal
            int n = "[TestMethod] public voidfoo(a, b, c".Length;
            String expecting =
                "[TestMethod] public voidfoo(a, b, c) throws Ick { i=3; }";
            Assert.AreEqual(expecting, a.Render(n));
        }

        [TestMethod]
        public void TestSingleValueWrap()
        {
            String templates =
                    "m(args,body) ::= <<{ <body; anchor, wrap=\"\\n\"> }>>" + newline;
            WriteFile(tmpdir, "t.stg", templates);
            STGroup group = new STGroupFile(tmpdir + "/" + "t.stg");

            ST m = group.GetInstanceOf("m");
            m.Add("body", "i=3;");
            // make it wrap because of ") throws Ick { " literal
            String expecting =
                "{ " + newline +
                "  i=3; }";
            Assert.AreEqual(expecting, m.Render(2));
        }

        [TestMethod]
        public void TestLineWrapInNestedExpr()
        {
            String templates =
                    "top(arrays) ::= <<Arrays: <arrays>done>>" + newline +
                    "array(values) ::= <<int[] a = { <values; anchor, wrap=\"\\n\", separator=\",\"> };<\\n\\>>>" + newline;
            WriteFile(tmpdir, "t.stg", templates);
            STGroup group = new STGroupFile(tmpdir + "/" + "t.stg");

            ST top = group.GetInstanceOf("top");
            ST a = group.GetInstanceOf("array");
            a.Add("values",
                           new int[] {3,9,20,2,1,4,6,32,5,6,77,888,2,1,6,32,5,6,77,
						4,9,20,2,1,4,63,9,20,2,1,4,6,32,5,6,77,6,32,5,6,77,
					    3,9,20,2,1,4,6,32,5,6,77,888,1,6,32,5});
            top.Add("arrays", a);
            top.Add("arrays", a); // add twice
            String expecting =
                "Arrays: int[] a = { 3,9,20,2,1,4,6,32,5," + newline +
                "                    6,77,888,2,1,6,32,5," + newline +
                "                    6,77,4,9,20,2,1,4,63," + newline +
                "                    9,20,2,1,4,6,32,5,6," + newline +
                "                    77,6,32,5,6,77,3,9,20," + newline +
                "                    2,1,4,6,32,5,6,77,888," + newline +
                "                    1,6,32,5 };" + newline +
                "int[] a = { 3,9,20,2,1,4,6,32,5,6,77,888," + newline +
                "            2,1,6,32,5,6,77,4,9,20,2,1,4," + newline +
                "            63,9,20,2,1,4,6,32,5,6,77,6," + newline +
                "            32,5,6,77,3,9,20,2,1,4,6,32," + newline +
                "            5,6,77,888,1,6,32,5 };" + newline +
                "done";
            Assert.AreEqual(expecting, top.Render(40));
        }
    }
}
