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
    using StringWriter = System.IO.StringWriter;
    using System.Collections.Generic;
    using Environment = System.Environment;

    [TestClass]
    public class TestLineWrap : BaseTest
    {
        [TestMethod]
        public void TestLineWrap1()
        {
            string templates =
                    "array(values) ::= <<int[] a = { <values; wrap=\"\\n\", separator=\",\"> };>>" + newline;
            writeFile(tmpdir, "t.stg", templates);
            STGroup group = new STGroupFile(tmpdir + "/" + "t.stg");

            ST a = group.getInstanceOf("array");
            a.add("values",
                           new int[] {3,9,20,2,1,4,6,32,5,6,77,888,2,1,6,32,5,6,77,
						4,9,20,2,1,4,63,9,20,2,1,4,6,32,5,6,77,6,32,5,6,77,
					    3,9,20,2,1,4,6,32,5,6,77,888,1,6,32,5});
            string expecting =
                "int[] a = { 3,9,20,2,1,4,6,32,5,6,77,888,\n" +
                "2,1,6,32,5,6,77,4,9,20,2,1,4,63,9,20,2,1,\n" +
                "4,6,32,5,6,77,6,32,5,6,77,3,9,20,2,1,4,6,\n" +
                "32,5,6,77,888,1,6,32,5 };";

            StringWriter sw = new StringWriter();
            STWriter stw = new AutoIndentWriter(sw, "\n"); // force \n as newline
            stw.setLineWidth(40);
            a.write(stw);
            string result = sw.ToString();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestLineWrapAnchored()
        {
            string templates =
                "array(values) ::= <<int[] a = { <values; anchor, wrap, separator=\",\"> };>>" + newline;
            writeFile(tmpdir, "t.stg", templates);
            STGroup group = new STGroupFile(tmpdir + "/" + "t.stg");

            ST a = group.getInstanceOf("array");
            a.add("values",
                new int[] {3,9,20,2,1,4,6,32,5,6,77,888,2,1,6,32,5,6,77,
            4,9,20,2,1,4,63,9,20,2,1,4,6,32,5,6,77,6,32,5,6,77,
            3,9,20,2,1,4,6,32,5,6,77,888,1,6,32,5});
            string expecting =
                "int[] a = { 3,9,20,2,1,4,6,32,5,6,77,888," + Environment.NewLine +
                "            2,1,6,32,5,6,77,4,9,20,2,1,4," + Environment.NewLine +
                "            63,9,20,2,1,4,6,32,5,6,77,6," + Environment.NewLine +
                "            32,5,6,77,3,9,20,2,1,4,6,32," + Environment.NewLine +
                "            5,6,77,888,1,6,32,5 };";
            Assert.AreEqual(expecting, a.render(40));
        }

        [TestMethod]
        public void TestSubtemplatesAnchorToo()
        {
            string templates =
                    "array(values) ::= <<{ <values; anchor, separator=\", \"> }>>" + newline;
            writeFile(tmpdir, "t.stg", templates);
            STGroup group = new STGroupFile(tmpdir + "/" + "t.stg");

            ST x = new ST("<\\n>{ <stuff; anchor, separator=\",\\n\"> }<\\n>");
            x.groupThatCreatedThisInstance = group;
            x.add("stuff", "1");
            x.add("stuff", "2");
            x.add("stuff", "3");
            ST a = group.getInstanceOf("array");
            a.add("values", new List<object>() { "a", x, "b" });
            string expecting =
                "{ a, " + Environment.NewLine +
                "  { 1," + Environment.NewLine +
                "    2," + Environment.NewLine +
                "    3 }" + Environment.NewLine +
                "  , b }";
            Assert.AreEqual(expecting, a.render(40));
        }

        [TestMethod]
        public void TestFortranLineWrap()
        {
            string templates =
                    "func(args) ::= <<       FUNCTION line( <args; wrap=\"\\n      c\", separator=\",\"> )>>" + newline;
            writeFile(tmpdir, "t.stg", templates);
            STGroup group = new STGroupFile(tmpdir + "/" + "t.stg");

            ST a = group.getInstanceOf("func");
            a.add("args",
                           new string[] { "a", "b", "c", "d", "e", "f" });
            string expecting =
                "       FUNCTION line( a,b,c,d," + Environment.NewLine +
                "      ce,f )";
            Assert.AreEqual(expecting, a.render(30));
        }

        [TestMethod]
        public void TestLineWrapWithDiffAnchor()
        {
            string templates =
                    "array(values) ::= <<int[] a = { <{1,9,2,<values; wrap, separator=\",\">}; anchor> };>>" + newline;
            writeFile(tmpdir, "t.stg", templates);
            STGroup group = new STGroupFile(tmpdir + "/" + "t.stg");

            ST a = group.getInstanceOf("array");
            a.add("values",
                           new int[] {3,9,20,2,1,4,6,32,5,6,77,888,2,1,6,32,5,6,77,
						4,9,20,2,1,4,63,9,20,2,1,4,6});
            string expecting =
                "int[] a = { 1,9,2,3,9,20,2,1,4," + Environment.NewLine +
                "            6,32,5,6,77,888,2," + Environment.NewLine +
                "            1,6,32,5,6,77,4,9," + Environment.NewLine +
                "            20,2,1,4,63,9,20,2," + Environment.NewLine +
                "            1,4,6 };";
            Assert.AreEqual(expecting, a.render(30));
        }

        [TestMethod]
        public void TestLineWrapEdgeCase()
        {
            string templates =
                    "duh(chars) ::= <<<chars; wrap=\"\\n\"\\>>>" + newline;
            writeFile(tmpdir, "t.stg", templates);
            STGroup group = new STGroupFile(tmpdir + "/" + "t.stg");

            ST a = group.getInstanceOf("duh");
            a.add("chars", new string[] { "a", "b", "c", "d", "e" });
            // lineWidth==3 implies that we can have 3 characters at most
            string expecting =
                "abc" + Environment.NewLine +
                "de";
            Assert.AreEqual(expecting, a.render(3));
        }

        [TestMethod]
        public void TestLineWrapLastCharIsNewline()
        {
            string templates =
                    "duh(chars) ::= <<<chars; wrap=\"\\n\"\\>>>" + newline;
            writeFile(tmpdir, "t.stg", templates);
            STGroup group = new STGroupFile(tmpdir + "/" + "t.stg");

            ST a = group.getInstanceOf("duh");
            a.add("chars", new string[] { "a", "b", "\n", "d", "e" });
            // don't do \n if it's last element anyway
            string expecting =
                "ab" + Environment.NewLine +
                "de";
            Assert.AreEqual(expecting, a.render(3));
        }

        [TestMethod]
        public void TestLineWrapCharAfterWrapIsNewline()
        {
            string templates =
                    "duh(chars) ::= <<<chars; wrap=\"\\n\"\\>>>" + newline;
            writeFile(tmpdir, "t.stg", templates);
            STGroup group = new STGroupFile(tmpdir + "/" + "t.stg");

            ST a = group.getInstanceOf("duh");
            a.add("chars", new string[] { "a", "b", "c", "\n", "d", "e" });
            // Once we wrap, we must dump chars as we see them.  A newline right
            // after a wrap is just an "unfortunate" event.  People will expect
            // a newline if it's in the data.
            string expecting =
                "abc" + Environment.NewLine +
                "" + Environment.NewLine +
                "de";
            Assert.AreEqual(expecting, a.render(3));
        }

        [TestMethod]
        public void TestLineWrapForList()
        {
            string templates =
                    "duh(data) ::= <<!<data; wrap>!>>" + newline;
            writeFile(tmpdir, "t.stg", templates);
            STGroup group = new STGroupFile(tmpdir + "/" + "t.stg");

            ST a = group.getInstanceOf("duh");
            a.add("data", new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            string expecting =
                "!123" + Environment.NewLine +
                "4567" + Environment.NewLine +
                "89!";
            Assert.AreEqual(expecting, a.render(4));
        }

        [TestMethod]
        public void TestLineWrapForAnonTemplate()
        {
            string templates =
                    "duh(data) ::= <<!<data:{v|[<v>]}; wrap>!>>" + newline;
            writeFile(tmpdir, "t.stg", templates);
            STGroup group = new STGroupFile(tmpdir + "/" + "t.stg");

            ST a = group.getInstanceOf("duh");
            a.add("data", new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            string expecting =
                "![1][2][3]" + Environment.NewLine + // width=9 is the 3 char; don't break til after ]
                "[4][5][6]" + Environment.NewLine +
                "[7][8][9]!";
            Assert.AreEqual(expecting, a.render(9));
        }

        [TestMethod]
        public void TestLineWrapForAnonTemplateAnchored()
        {
            string templates =
                    "duh(data) ::= <<!<data:{v|[<v>]}; anchor, wrap>!>>" + newline;
            writeFile(tmpdir, "t.stg", templates);
            STGroup group = new STGroupFile(tmpdir + "/" + "t.stg");

            ST a = group.getInstanceOf("duh");
            a.add("data", new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            string expecting =
                "![1][2][3]" + Environment.NewLine +
                " [4][5][6]" + Environment.NewLine +
                " [7][8][9]!";
            Assert.AreEqual(expecting, a.render(9));
        }

        [TestMethod]
        public void TestLineWrapForAnonTemplateComplicatedWrap()
        {
            string templates =
                    "top(s) ::= <<  <s>.>>" +
                    "str(data) ::= <<!<data:{v|[<v>]}; wrap=\"!+\\n!\">!>>" + newline;
            writeFile(tmpdir, "t.stg", templates);
            STGroup group = new STGroupFile(tmpdir + "/" + "t.stg");

            ST t = group.getInstanceOf("top");
            ST s = group.getInstanceOf("str");
            s.add("data", new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            t.add("s", s);
            string expecting =
                "  ![1][2]!+" + Environment.NewLine +
                "  ![3][4]!+" + Environment.NewLine +
                "  ![5][6]!+" + Environment.NewLine +
                "  ![7][8]!+" + Environment.NewLine +
                "  ![9]!.";
            Assert.AreEqual(expecting, t.render(9));
        }

        [TestMethod]
        public void TestIndentBeyondLineWidth()
        {
            string templates =
                    "duh(chars) ::= <<    <chars; wrap=\"\\n\"\\>>>" + newline;
            writeFile(tmpdir, "t.stg", templates);
            STGroup group = new STGroupFile(tmpdir + "/" + "t.stg");

            ST a = group.getInstanceOf("duh");
            a.add("chars", new string[] { "a", "b", "c", "d", "e" });
            //
            string expecting =
                "    a" + Environment.NewLine +
                "    b" + Environment.NewLine +
                "    c" + Environment.NewLine +
                "    d" + Environment.NewLine +
                "    e";
            Assert.AreEqual(expecting, a.render(2));
        }

        [TestMethod]
        public void TestIndentedExpr()
        {
            string templates =
                    "duh(chars) ::= <<    <chars; wrap=\"\\n\"\\>>>" + newline;
            writeFile(tmpdir, "t.stg", templates);
            STGroup group = new STGroupFile(tmpdir + "/" + "t.stg");

            ST a = group.getInstanceOf("duh");
            a.add("chars", new string[] { "a", "b", "c", "d", "e" });
            //
            string expecting =
                "    ab" + Environment.NewLine +
                "    cd" + Environment.NewLine +
                "    e";
            // width=4 spaces + 2 char.
            Assert.AreEqual(expecting, a.render(6));
        }

        [TestMethod]
        public void TestNestedIndentedExpr()
        {
            string templates =
                    "top(d) ::= <<  <d>!>>" + newline +
                    "duh(chars) ::= <<  <chars; wrap=\"\\n\"\\>>>" + newline;
            writeFile(tmpdir, "t.stg", templates);
            STGroup group = new STGroupFile(tmpdir + "/" + "t.stg");

            ST top = group.getInstanceOf("top");
            ST duh = group.getInstanceOf("duh");
            duh.add("chars", new string[] { "a", "b", "c", "d", "e" });
            top.add("d", duh);
            string expecting =
                "    ab" + Environment.NewLine +
                "    cd" + Environment.NewLine +
                "    e!";
            // width=4 spaces + 2 char.
            Assert.AreEqual(expecting, top.render(6));
        }

        [TestMethod]
        public void TestNestedWithIndentAndTrackStartOfExpr()
        {
            string templates =
                    "top(d) ::= <<  <d>!>>" + newline +
                    "duh(chars) ::= <<x: <chars; anchor, wrap=\"\\n\"\\>>>" + newline;
            writeFile(tmpdir, "t.stg", templates);
            STGroup group = new STGroupFile(tmpdir + "/" + "t.stg");

            ST top = group.getInstanceOf("top");
            ST duh = group.getInstanceOf("duh");
            duh.add("chars", new string[] { "a", "b", "c", "d", "e" });
            top.add("d", duh);
            //
            string expecting =
                "  x: ab" + Environment.NewLine +
                "     cd" + Environment.NewLine +
                "     e!";
            Assert.AreEqual(expecting, top.render(7));
        }

        [TestMethod]
        public void TestLineDoesNotWrapDueToLiteral()
        {
            string templates =
                    "m(args,body) ::= <<[TestMethod] public voidfoo(<args; wrap=\"\\n\",separator=\", \">) throws Ick { <body> }>>" + newline;
            writeFile(tmpdir, "t.stg", templates);
            STGroup group = new STGroupFile(tmpdir + "/" + "t.stg");

            ST a = group.getInstanceOf("m");
            a.add("args",
                           new string[] { "a", "b", "c" });
            a.add("body", "i=3;");
            // make it wrap because of ") throws Ick { " literal
            int n = "[TestMethod] public voidfoo(a, b, c".Length;
            string expecting =
                "[TestMethod] public voidfoo(a, b, c) throws Ick { i=3; }";
            Assert.AreEqual(expecting, a.render(n));
        }

        [TestMethod]
        public void TestSingleValueWrap()
        {
            string templates =
                    "m(args,body) ::= <<{ <body; anchor, wrap=\"\\n\"> }>>" + newline;
            writeFile(tmpdir, "t.stg", templates);
            STGroup group = new STGroupFile(tmpdir + "/" + "t.stg");

            ST m = group.getInstanceOf("m");
            m.add("body", "i=3;");
            // make it wrap because of ") throws Ick { " literal
            string expecting =
                "{ " + Environment.NewLine +
                "  i=3; }";
            Assert.AreEqual(expecting, m.render(2));
        }

        [TestMethod]
        public void TestLineWrapInNestedExpr()
        {
            string templates =
                    "top(arrays) ::= <<Arrays: <arrays>done>>" + newline +
                    "array(values) ::= <<int[] a = { <values; anchor, wrap=\"\\n\", separator=\",\"> };<\\n\\>>>" + newline;
            writeFile(tmpdir, "t.stg", templates);
            STGroup group = new STGroupFile(tmpdir + "/" + "t.stg");

            ST top = group.getInstanceOf("top");
            ST a = group.getInstanceOf("array");
            a.add("values",
                           new int[] {3,9,20,2,1,4,6,32,5,6,77,888,2,1,6,32,5,6,77,
						4,9,20,2,1,4,63,9,20,2,1,4,6,32,5,6,77,6,32,5,6,77,
					    3,9,20,2,1,4,6,32,5,6,77,888,1,6,32,5});
            top.add("arrays", a);
            top.add("arrays", a); // add twice
            string expecting =
                "Arrays: int[] a = { 3,9,20,2,1,4,6,32,5," + Environment.NewLine +
                "                    6,77,888,2,1,6,32,5," + Environment.NewLine +
                "                    6,77,4,9,20,2,1,4,63," + Environment.NewLine +
                "                    9,20,2,1,4,6,32,5,6," + Environment.NewLine +
                "                    77,6,32,5,6,77,3,9,20," + Environment.NewLine +
                "                    2,1,4,6,32,5,6,77,888," + Environment.NewLine +
                "                    1,6,32,5 };" + Environment.NewLine +
                "int[] a = { 3,9,20,2,1,4,6,32,5,6,77,888," + Environment.NewLine +
                "            2,1,6,32,5,6,77,4,9,20,2,1,4," + Environment.NewLine +
                "            63,9,20,2,1,4,6,32,5,6,77,6," + Environment.NewLine +
                "            32,5,6,77,3,9,20,2,1,4,6,32," + Environment.NewLine +
                "            5,6,77,888,1,6,32,5 };" + Environment.NewLine +
                "done";
            Assert.AreEqual(expecting, top.render(40));
        }
    }
}
