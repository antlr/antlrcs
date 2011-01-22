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
    using Path = System.IO.Path;

    [TestClass]
    public class TestIndentation : BaseTest
    {
        [TestMethod]
        public void TestIndentInFrontOfTwoExpr()
        {
            string templates =
                    "list(a,b) ::= <<" +
                    "  <a><b>" + newline +
                    ">>" + newline;

            writeFile(tmpdir, "t.stg", templates);
            STGroup group = new STGroupFile(Path.Combine(tmpdir, "t.stg"));
            ST t = group.getInstanceOf("list");
            t.impl.dump();
            t.add("a", "Terence");
            t.add("b", "Jim");
            string expecting =
                    "  TerenceJim";
            Assert.AreEqual(expecting, t.render());
        }

        [TestMethod]
        public void TestSimpleIndentOfAttributeList()
        {
            string templates =
                    "list(names) ::= <<" +
                    "  <names; separator=\"\\n\">" + newline +
                    ">>" + newline;

            writeFile(tmpdir, "t.stg", templates);
            STGroup group = new STGroupFile(Path.Combine(tmpdir, "t.stg"));
            ST t = group.getInstanceOf("list");
            t.add("names", "Terence");
            t.add("names", "Jim");
            t.add("names", "Sriram");
            string expecting =
                    "  Terence" + newline +
                    "  Jim" + newline +
                    "  Sriram";
            Assert.AreEqual(expecting, t.render());
        }

        [TestMethod]
        public void TestIndentOfMultilineAttributes()
        {
            string templates =
                    "list(names) ::= <<" +
                    "  <names; separator=\"\n\">" + newline +
                    ">>" + newline;
            writeFile(tmpdir, "t.stg", templates);
            STGroup group = new STGroupFile(Path.Combine(tmpdir, "t.stg"));
            ST t = group.getInstanceOf("list");
            t.add("names", "Terence\nis\na\nmaniac");
            t.add("names", "Jim");
            t.add("names", "Sriram\nis\ncool");
            string expecting =
                    "  Terence" + newline +
                    "  is" + newline +
                    "  a" + newline +
                    "  maniac" + newline +
                    "  Jim" + newline +
                    "  Sriram" + newline +
                    "  is" + newline +
                    "  cool";
            Assert.AreEqual(expecting, t.render());
        }

        [TestMethod]
        public void TestIndentOfMultipleBlankLines()
        {
            string templates =
                    "list(names) ::= <<" +
                    "  <names>" + newline +
                    ">>" + newline;
            writeFile(tmpdir, "t.stg", templates);
            STGroup group = new STGroupFile(Path.Combine(tmpdir, "t.stg"));
            ST t = group.getInstanceOf("list");
            t.add("names", "Terence\n\nis a maniac");
            string expecting =
                    "  Terence" + newline +
                    "" + newline + // no indent on blank line
                    "  is a maniac";
            Assert.AreEqual(expecting, t.render());
        }

        [TestMethod]
        public void TestIndentBetweenLeftJustifiedLiterals()
        {
            string templates =
                    "list(names) ::= <<" +
                    "Before:" + newline +
                    "  <names; separator=\"\\n\">" + newline +
                    "after" + newline +
                    ">>" + newline;
            writeFile(tmpdir, "t.stg", templates);
            STGroup group = new STGroupFile(Path.Combine(tmpdir, "t.stg"));
            ST t = group.getInstanceOf("list");
            t.add("names", "Terence");
            t.add("names", "Jim");
            t.add("names", "Sriram");
            string expecting =
                    "Before:" + newline +
                    "  Terence" + newline +
                    "  Jim" + newline +
                    "  Sriram" + newline +
                    "after";
            Assert.AreEqual(expecting, t.render());
        }

        [TestMethod]
        public void TestNestedIndent()
        {
            string templates =
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
            writeFile(tmpdir, "t.stg", templates);
            STGroup group = new STGroupFile(Path.Combine(tmpdir, "t.stg"));
            ST t = group.getInstanceOf("method");
            t.add("name", "foo");
            ST s1 = group.getInstanceOf("assign");
            s1.add("lhs", "x");
            s1.add("expr", "0");
            ST s2 = group.getInstanceOf("ifstat");
            s2.add("expr", "x>0");
            ST s2a = group.getInstanceOf("assign");
            s2a.add("lhs", "y");
            s2a.add("expr", "x+y");
            ST s2b = group.getInstanceOf("assign");
            s2b.add("lhs", "z");
            s2b.add("expr", "4");
            s2.add("stats", s2a);
            s2.add("stats", s2b);
            t.add("stats", s1);
            t.add("stats", s2);
            string expecting =
                    "void foo() {" + newline +
                    "\tx=0;" + newline +
                    "\tif (x>0) {" + newline +
                    "\t  y=x+y;" + newline +
                    "\t  z=4;" + newline +
                    "\t}" + newline +
                    "}";
            Assert.AreEqual(expecting, t.render());
        }

        [TestMethod]
        public void TestIndentedIFWithValueExpr()
        {
            ST t = new ST(
                "begin" + newline +
                "    <if(x)>foo<endif>" + newline +
                "end" + newline);
            t.add("x", "x");
            string expecting =
                "begin" + newline +
                "    foo" + newline +
                "end" + newline;
            string result = t.render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestIFWithIndentOnMultipleLines()
        {
            ST t = new ST(
                "begin" + newline +
                "   <if(x)>" + newline +
                "   foo" + newline +
                "   <else>" + newline +
                "   bar" + newline +
                "   <endif>" + newline +
                "end" + newline);
            string expecting =
                "begin" + newline +
                "   bar" + newline +
                "end" + newline;
            string result = t.render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestIFWithIndentAndExprOnMultipleLines()
        {
            ST t = new ST(
                "begin" + newline +
                "   <if(x)>" + newline +
                "   <x>" + newline +
                "   <else>" + newline +
                "   <y>" + newline +
                "   <endif>" + newline +
                "end" + newline);
            t.add("y", "y");
            string expecting =
                "begin" + newline +
                "   y" + newline +
                "end" + newline;
            string result = t.render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestIFWithIndentAndExprWithIndentOnMultipleLines()
        {
            ST t = new ST(
                "begin" + newline +
                "   <if(x)>" + newline +
                "     <x>" + newline +
                "   <else>" + newline +
                "     <y>" + newline +
                "   <endif>" + newline +
                "end" + newline);
            t.add("y", "y");
            string expecting =
                "begin" + newline +
                "     y" + newline +
                "end" + newline;
            string result = t.render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestNestedIFWithIndentOnMultipleLines()
        {
            ST t = new ST(
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
            t.add("x", "x");
            t.add("y", "y");
            string expecting =
                "begin" + newline +
                "      foo" + newline +
                "end" + newline; // no indent
            string result = t.render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestIFInSubtemplate()
        {
            ST t = new ST(
                "<names:{n |" + newline +
                "   <if(x)>" + newline +
                "   <x>" + newline +
                "   <else>" + newline +
                "   <y>" + newline +
                "   <endif>" + newline +
                "}>" + newline);
            t.add("names", "Ter");
            t.add("y", "y");
            string expecting = "   y" + newline + newline;
            string result = t.render();
            Assert.AreEqual(expecting, result);
        }
    }
}
