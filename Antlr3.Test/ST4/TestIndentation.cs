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
    using Path = System.IO.Path;
    using STErrorListener = StringTemplate.ITemplateErrorListener;
    using STGroup = StringTemplate.TemplateGroup;
    using STGroupFile = StringTemplate.TemplateGroupFile;
    using String = System.String;

    [TestClass]
    public class TestIndentation : StringTemplateTestBase
    {
        [TestMethod]
        public void TestSimpleIndentOfAttributeList()
        {
            String templates =
                    "list(names) ::= <<" +
                    "  <names; separator=\"\\n\">" + newline +
                    ">>" + newline;

            WriteFile(tmpdir, "t.stg", templates);
            STGroup group = new STGroupFile(Path.Combine(tmpdir, "t.stg"));
            Template t = group.GetInstanceOf("list");
            t.Add("names", "Terence");
            t.Add("names", "Jim");
            t.Add("names", "Sriram");
            String expecting =
                    "  Terence" + newline +
                    "  Jim" + newline +
                    "  Sriram" + newline;
            Assert.AreEqual(expecting, t.Render());
        }

        [TestMethod]
        public void TestIndentOfMultilineAttributes()
        {
            String templates =
                    "list(names) ::= <<" +
                    "  <names; separator=\"\n\">" + newline +
                    ">>" + newline;
            WriteFile(tmpdir, "t.stg", templates);
            STGroup group = new STGroupFile(Path.Combine(tmpdir, "t.stg"));
            Template t = group.GetInstanceOf("list");
            t.Add("names", "Terence\nis\na\nmaniac");
            t.Add("names", "Jim");
            t.Add("names", "Sriram\nis\ncool");
            String expecting =
                    "  Terence" + newline +
                    "  is" + newline +
                    "  a" + newline +
                    "  maniac" + newline +
                    "  Jim" + newline +
                    "  Sriram" + newline +
                    "  is" + newline +
                    "  cool" + newline;
            Assert.AreEqual(expecting, t.Render());
        }

        [TestMethod]
        public void TestIndentOfMultipleBlankLines()
        {
            String templates =
                    "list(names) ::= <<" +
                    "  <names>" + newline +
                    ">>" + newline;
            WriteFile(tmpdir, "t.stg", templates);
            STGroup group = new STGroupFile(Path.Combine(tmpdir, "t.stg"));
            Template t = group.GetInstanceOf("list");
            t.Add("names", "Terence\n\nis a maniac");
            String expecting =
                    "  Terence" + newline +
                    "" + newline + // no indent on blank line
                    "  is a maniac" + newline;
            Assert.AreEqual(expecting, t.Render());
        }

        [TestMethod]
        public void TestIndentBetweenLeftJustifiedLiterals()
        {
            String templates =
                    "list(names) ::= <<" +
                    "Before:" + newline +
                    "  <names; separator=\"\\n\">" + newline +
                    "after" + newline +
                    ">>" + newline;
            WriteFile(tmpdir, "t.stg", templates);
            STGroup group = new STGroupFile(Path.Combine(tmpdir, "t.stg"));
            Template t = group.GetInstanceOf("list");
            t.Add("names", "Terence");
            t.Add("names", "Jim");
            t.Add("names", "Sriram");
            String expecting =
                    "Before:" + newline +
                    "  Terence" + newline +
                    "  Jim" + newline +
                    "  Sriram" + newline +
                    "after" + newline;
            Assert.AreEqual(expecting, t.Render());
        }

        [TestMethod]
        public void TestNestedIndent()
        {
            String templates =
                    "method(name,stats) ::= <<" +
                    "void <name>() {" + newline +
                    "\t<stats; separator=\"\\n\">" + newline +
                    "}" + newline +
                    ">>" + newline +
                    "ifstat(expr,stats) ::= <<" + newline +
                    "if (<expr>) {" + newline +
                    "  <stats; separator=\"\\n\">" + newline +
                    "}" +
                    ">>" + newline +
                    "assign(lhs,expr) ::= <<<lhs>=<expr>;>>" + newline
                    ;
            WriteFile(tmpdir, "t.stg", templates);
            STGroup group = new STGroupFile(Path.Combine(tmpdir, "t.stg"));
            Template t = group.GetInstanceOf("method");
            t.Add("name", "foo");
            Template s1 = group.GetInstanceOf("assign");
            s1.Add("lhs", "x");
            s1.Add("expr", "0");
            Template s2 = group.GetInstanceOf("ifstat");
            s2.Add("expr", "x>0");
            Template s2a = group.GetInstanceOf("assign");
            s2a.Add("lhs", "y");
            s2a.Add("expr", "x+y");
            Template s2b = group.GetInstanceOf("assign");
            s2b.Add("lhs", "z");
            s2b.Add("expr", "4");
            s2.Add("stats", s2a);
            s2.Add("stats", s2b);
            t.Add("stats", s1);
            t.Add("stats", s2);
            String expecting =
                    "void foo() {" + newline +
                    "\tx=0;" + newline +
                    "\tif (x>0) {" + newline +
                    "\t  y=x+y;" + newline +
                    "\t  z=4;" + newline +
                    "\t}" + newline +
                    "}" + newline;
            Assert.AreEqual(expecting, t.Render());
        }

        [TestMethod]
        public void TestIndentedIFWithValueExpr()
        {
            Template t = new Template(
                "begin" + newline +
                "    <if(x)>foo<endif>" + newline +
                "end" + newline);
            t.Add("x", "x");
            String expecting = "begin" + newline + "    foo" + newline + "end" + newline;
            String result = t.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestIFWithIndentOnMultipleLines()
        {
            Template t = new Template(
                "begin" + newline +
                "   <if(x)>" + newline +
                "   foo" + newline +
                "   <else>" + newline +
                "   bar" + newline +
                "   <endif>" + newline +
                "end" + newline);
            String expecting = "begin" + newline + "   bar" + newline + "end" + newline;
            String result = t.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestIFWithIndentAndExprOnMultipleLines()
        {
            Template t = new Template(
                "begin" + newline +
                "   <if(x)>" + newline +
                "   <x>" + newline +
                "   <else>" + newline +
                "   <y>" + newline +
                "   <endif>" + newline +
                "end" + newline);
            t.Add("y", "y");
            String expecting = "begin" + newline + "   y" + newline + "end" + newline;
            String result = t.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestIFWithIndentAndExprWithIndentOnMultipleLines()
        {
            Template t = new Template(
                "begin" + newline +
                "   <if(x)>" + newline +
                "     <x>" + newline +
                "   <else>" + newline +
                "     <y>" + newline +
                "   <endif>" + newline +
                "end" + newline);
            t.Add("y", "y");
            String expecting = "begin" + newline + "     y" + newline + "end" + newline;
            String result = t.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestNestedIFWithIndentOnMultipleLines()
        {
            Template t = new Template(
                "begin" + newline +
                "   <if(x)>" + newline +
                "      <if(y)>" + newline +
                "      foo" + newline +
                "      <endif>" + newline +
                "   <else>" + newline +
                "      <if(z)>" + newline +
                "      foo" + newline +
                "      <endif>" + newline +
                "   <endif>" + newline +
                "end" + newline);
            t.Add("x", "x");
            t.Add("y", "y");
            String expecting = "begin" + newline + "      foo" + newline + "end" + newline; // no indent
            String result = t.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestIFInSubtemplate()
        {
            Template t = new Template(
                "<names:{n |" + newline +
                "   <if(x)>" + newline +
                "   <x>" + newline +
                "   <else>" + newline +
                "   <y>" + newline +
                "   <endif>" + newline +
                "}>" + newline);
            t.Add("names", "Ter");
            t.Add("y", "y");
            String expecting = "   y" + newline + newline;
            String result = t.Render();
            Assert.AreEqual(expecting, result);
        }

    }
}
