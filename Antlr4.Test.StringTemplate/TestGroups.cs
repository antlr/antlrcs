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
    using Antlr4.StringTemplate.Misc;
    using System.Runtime.CompilerServices;
    using Path = System.IO.Path;

    [TestClass]
    public class TestGroups : BaseTest
    {
        [TestMethod]
        public void TestSimpleGroup()
        {
            string dir = tmpdir;
            string a =
                "a(x) ::= <<" + newline +
                "foo" + newline +
                ">>" + newline;
            writeFile(dir, "a.st", a);
            STGroup group = new STGroupDir(dir);
            ST st = group.getInstanceOf("a");
            string expected = "foo";
            string result = st.render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestGroupWithTwoTemplates()
        {
            string dir = tmpdir;
            string a =
                "a(x) ::= <<" + newline +
                "foo" + newline +
                ">>" + newline;
            writeFile(dir, "a.st", a);
            string b =
                "b() ::= \"bar\"" + newline;
            writeFile(dir, "b.st", b);
            STGroup group = new STGroupDir(dir);
            ST st1 = group.getInstanceOf("a");
            ST st2 = group.getInstanceOf("b");
            string expected = "foobar";
            string result = st1.render() + st2.render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestSubdir()
        {
            // /randomdir/a and /randomdir/subdir/b
            string dir = tmpdir;
            string a =
                "a(x) ::= <<" + newline +
                "foo" + newline +
                ">>" + newline;
            writeFile(dir, "a.st", a);
            string b =
                "b() ::= \"bar\"" + newline;
            writeFile(Path.Combine(dir, "subdir"), "b.st", b);
            STGroup group = new STGroupDir(dir);
            ST st1 = group.getInstanceOf("a");
            ST st2 = group.getInstanceOf("subdir/b");
            string expected = "foobar";
            string result = st1.render() + st2.render();
            Assert.AreEqual(expected, result);
            st2 = group.getInstanceOf("subdir/b"); // should work with / in front too
            expected = "bar";
            result = st2.render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestAbsoluteTemplateRef()
        {
            // /randomdir/a and /randomdir/subdir/b
            string dir = tmpdir;
            string a =
                "a(x) ::= << <subdir/b()> >>\n";
            writeFile(dir, "a.st", a);
            string b =
                "b() ::= <<bar>>\n";
            writeFile(dir + "/subdir", "b.st", b);
            STGroup group = new STGroupDir(dir);
            ST st = group.getInstanceOf("a");
            string expected = " bar ";
            string result = st.render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestGroupFileInDir()
        {
            // /randomdir/a and /randomdir/group.stg with b and c templates
            string dir = tmpdir;
            string a =
                "a(x) ::= <<\n" +
                "foo\n" +
                ">>\n";
            writeFile(dir, "a.st", a);
            string groupFile =
                "b() ::= \"bar\"\n" +
                "c() ::= \"duh\"\n";
            writeFile(dir, "group.stg", groupFile);
            STGroup group = new STGroupDir(dir);
            ST st1 = group.getInstanceOf("a");
            ST st2 = group.getInstanceOf("group/b");
            ST st3 = group.getInstanceOf("group/c");
            string expected = "foobarduh";
            string result = st1.render() + st2.render() + st3.render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestSubSubdir()
        {
            // /randomdir/a and /randomdir/subdir/b
            string dir = tmpdir;
            string a =
                "a(x) ::= <<" + newline +
                "foo" + newline +
                ">>" + newline;
            writeFile(dir, "a.st", a);
            string b =
                "b() ::= \"bar\"" + newline;
            writeFile(dir + "/sub1/sub2", "b.st", b);
            STGroup group = new STGroupDir(dir);
            ST st1 = group.getInstanceOf("a");
            ST st2 = group.getInstanceOf("sub1/sub2/b");
            string expected = "foobar";
            string result = st1.render() + st2.render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestGroupFileInSubDir()
        {
            // /randomdir/a and /randomdir/group.stg with b and c templates
            string dir = tmpdir;
            string a =
                "a(x) ::= <<\n" +
                "foo\n" +
                ">>\n";
            writeFile(dir, "a.st", a);
            string groupFile =
                "b() ::= \"bar\"\n" +
                "c() ::= \"duh\"\n";
            writeFile(dir, "subdir/group.stg", groupFile);
            STGroup group = new STGroupDir(dir);
            ST st1 = group.getInstanceOf("a");
            ST st2 = group.getInstanceOf("subdir/group/b");
            ST st3 = group.getInstanceOf("subdir/group/c");
            string expected = "foobarduh";
            string result = st1.render() + st2.render() + st3.render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestRefToAnotherTemplateInSameGroup()
        {
            string dir = tmpdir;
            string a = "a() ::= << <b()> >>\n";
            string b = "b() ::= <<bar>>\n";
            writeFile(dir, "a.st", a);
            writeFile(dir, "b.st", b);
            STGroup group = new STGroupDir(dir);
            ST st = group.getInstanceOf("a");
            string expected = " bar ";
            string result = st.render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestRefToAnotherTemplateInSameSubdir()
        {
            // /randomdir/a and /randomdir/subdir/b
            string dir = tmpdir;
            string a = "a() ::= << <subdir/b()> >>\n";
            string b = "b() ::= <<bar>>\n";
            writeFile(dir + "/subdir", "a.st", a);
            writeFile(dir + "/subdir", "b.st", b);
            STGroup group = new STGroupDir(dir);
            ST st = group.getInstanceOf("subdir/a");
            st.impl.dump();
            string expected = " bar ";
            string result = st.render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestDupDef()
        {
            string dir = tmpdir;
            string groupFile =
                "b() ::= \"bar\"\n" +
                "b() ::= \"duh\"\n";
            writeFile(dir, "group.stg", groupFile);
            ITemplateErrorListener errors = new ErrorBuffer();
            STGroupFile group = new STGroupFile(Path.Combine(dir, "group.stg"));
            group.setListener(errors);
            group.load();
            string expected = "group.stg 2:0: redefinition of template b" + newline;
            string result = errors.ToString();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestAlias()
        {
            string dir = tmpdir;
            string groupFile =
                "a() ::= \"bar\"\n" +
                "b ::= a\n";
            writeFile(dir, "group.stg", groupFile);
            STGroupFile group = new STGroupFile(Path.Combine(dir, "group.stg"));
            ST st = group.getInstanceOf("b");
            string expected = "bar";
            string result = st.render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestAliasWithArgs()
        {
            string dir = tmpdir;
            string groupFile =
                "a(x,y) ::= \"<x><y>\"\n" +
                "b ::= a\n";
            writeFile(dir, "group.stg", groupFile);
            STGroupFile group = new STGroupFile(Path.Combine(dir, "group.stg"));
            ST st = group.getInstanceOf("b");
            st.add("x", 1);
            st.add("y", 2);
            string expected = "12";
            string result = st.render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestSimpleDefaultArg()
        {
            string dir = tmpdir;
            string a = "a() ::= << <b()> >>\n";
            string b = "b(x=\"foo\") ::= \"<x>\"\n";
            writeFile(dir, "a.st", a);
            writeFile(dir, "b.st", b);
            STGroup group = new STGroupDir(dir);
            ST st = group.getInstanceOf("a");
            string expected = " foo ";
            string result = st.render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestDefaultArgument()
        {
            string templates =
                    "method(name) ::= <<" + newline +
                    "<stat(name)>" + newline +
                    ">>" + newline +
                    "stat(name,value=\"99\") ::= \"x=<value>; // <name>\"" + newline
                    ;
            writeFile(tmpdir, "group.stg", templates);
            STGroup group = new STGroupFile(tmpdir + "/group.stg");
            ST b = group.getInstanceOf("method");
            b.add("name", "foo");
            string expecting = "x=99; // foo";
            string result = b.render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestDefaultArgument2()
        {
            string templates =
                    "stat(name,value=\"99\") ::= \"x=<value>; // <name>\"" + newline
                    ;
            writeFile(tmpdir, "group.stg", templates);
            STGroup group = new STGroupFile(tmpdir + "/group.stg");
            ST b = group.getInstanceOf("stat");
            b.add("name", "foo");
            string expecting = "x=99; // foo";
            string result = b.render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestDefaultArgumentAsSimpleTemplate()
        {
            string templates =
                    "stat(name,value={99}) ::= \"x=<value>; // <name>\"" + newline
                    ;
            writeFile(tmpdir, "group.stg", templates);
            STGroup group = new STGroupFile(tmpdir + "/group.stg");
            ST b = group.getInstanceOf("stat");
            b.add("name", "foo");
            string expecting = "x=99; // foo";
            string result = b.render();
            Assert.AreEqual(expecting, result);
        }

        private class Field
        {
            public string name = "parrt";
            public int n = 0;

            public override string ToString()
            {
                return "Field";
            }
        }

        [TestMethod]
        public void TestDefaultArgumentManuallySet()
        {
            // set arg f manually for stat(f=f)
            string templates =
                    "method(fields) ::= <<" + newline +
                    "<fields:{f | <stat(f)>}>" + newline +
                    ">>" + newline +
                    "stat(f,value={<f.name>}) ::= \"x=<value>; // <f.name>\"" + newline
                    ;
            writeFile(tmpdir, "group.stg", templates);
            STGroup group = new STGroupFile(tmpdir + "/group.stg");
            ST m = group.getInstanceOf("method");
            m.add("fields", new Field());
            string expecting = "x=parrt; // parrt";
            string result = m.render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestDefaultArgumentSeesVarFromDynamicScoping()
        {
            string templates =
                    "method(fields) ::= <<" + newline +
                    "<fields:{f | <stat()>}>" + newline +
                    ">>" + newline +
                    "stat(value={<f.name>}) ::= \"x=<value>; // <f.name>\"" + newline
                    ;
            writeFile(tmpdir, "group.stg", templates);
            STGroup group = new STGroupFile(tmpdir + "/group.stg");
            ST m = group.getInstanceOf("method");
            m.add("fields", new Field());
            string expecting = "x=parrt; // parrt";
            string result = m.render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestDefaultArgumentImplicitlySet2()
        {
            // f of stat is implicit first arg
            string templates =
                    "method(fields) ::= <<" + newline +
                    "<fields:{f | <f:stat()>}>" + newline +
                    ">>" + newline +
                    "stat(f,value={<f.name>}) ::= \"x=<value>; // <f.name>\"" + newline
                    ;
            writeFile(tmpdir, "group.stg", templates);
            STGroup group = new STGroupFile(tmpdir + "/group.stg");
            ST m = group.getInstanceOf("method");
            m.add("fields", new Field());
            string expecting = "x=parrt; // parrt";
            string result = m.render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestDefaultArgumentAsTemplate()
        {
            string templates =
                    "method(name,size) ::= <<" + newline +
                    "<stat(name)>" + newline +
                    ">>" + newline +
                    "stat(name,value={<name>}) ::= \"x=<value>; // <name>\"" + newline
                    ;
            writeFile(tmpdir, "group.stg", templates);
            STGroup group = new STGroupFile(tmpdir + "/group.stg");
            ST b = group.getInstanceOf("method");
            b.add("name", "foo");
            b.add("size", "2");
            string expecting = "x=foo; // foo";
            string result = b.render();
            //System.err.println("result='"+result+"'");
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestDefaultArgumentAsTemplate2()
        {
            string templates =
                    "method(name,size) ::= <<" + newline +
                    "<stat(name)>" + newline +
                    ">>" + newline +
                    "stat(name,value={ [<name>] }) ::= \"x=<value>; // <name>\"" + newline
                    ;
            writeFile(tmpdir, "group.stg", templates);
            STGroup group = new STGroupFile(tmpdir + "/group.stg");
            ST b = group.getInstanceOf("method");
            b.add("name", "foo");
            b.add("size", "2");
            string expecting = "x=[foo] ; // foo"; // won't see ' ' after '=' since it's an indent not simple string
            string result = b.render();
            //System.err.println("result='"+result+"'");
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestDoNotUseDefaultArgument()
        {
            string templates =
                    "method(name) ::= <<" + newline +
                    "<stat(name,\"34\")>" + newline +
                    ">>" + newline +
                    "stat(name,value=\"99\") ::= \"x=<value>; // <name>\"" + newline
                    ;
            writeFile(tmpdir, "group.stg", templates);
            STGroup group = new STGroupFile(tmpdir + "/group.stg");
            ST b = group.getInstanceOf("method");
            b.add("name", "foo");
            string expecting = "x=34; // foo";
            string result = b.render();
            Assert.AreEqual(expecting, result);
        }

        private class Counter
        {
            int n = 0;
            public override string ToString()
            {
                return (n++).ToString();
            }
        }

        [TestMethod]
        public void TestDefaultArgumentInParensToEvalEarly()
        {
            string templates =
                    "A(x) ::= \"<B()>\"" + newline +
                    "B(y={<(x)>}) ::= \"<y> <x> <x> <y>\"" + newline
                    ;
            writeFile(tmpdir, "group.stg", templates);
            STGroup group = new STGroupFile(tmpdir + "/group.stg");
            ST a = group.getInstanceOf("A");
            a.add("x", new Counter());
            string expecting = "0 1 2 0"; // trace must be false to get these numbers
            string result = a.render();
            //System.err.println("result='"+result+"'");
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestNamedArgsInOrder()
        {
            string dir = tmpdir;
            string groupFile =
                "f(x,y) ::= \"<x><y>\"\n" +
                "g() ::= \"<f(x={a},y={b})>\"";
            writeFile(dir, "group.stg", groupFile);
            STGroupFile group = new STGroupFile(Path.Combine(dir, "group.stg"));
            ST st = group.getInstanceOf("g");
            string expected = "ab";
            string result = st.render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestNamedArgsOutOfOrder()
        {
            string dir = tmpdir;
            string groupFile =
                "f(x,y) ::= \"<x><y>\"\n" +
                "g() ::= \"<f(y={b},x={a})>\"";
            writeFile(dir, "group.stg", groupFile);
            STGroupFile group = new STGroupFile(Path.Combine(dir, "group.stg"));
            ST st = group.getInstanceOf("g");
            string expected = "ab";
            string result = st.render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestUnknownNamedArg()
        {
            string dir = tmpdir;
            string groupFile =
                "f(x,y) ::= \"<x><y>\"\n" +
                "g() ::= \"<f(x={a},z={b})>\"";
            //012345678901234567

            writeFile(dir, "group.stg", groupFile);
            STGroupFile group = new STGroupFile(Path.Combine(dir, "group.stg"));
            ErrorBuffer errors = new ErrorBuffer();
            group.setListener(errors);
            ST st = group.getInstanceOf("g");
            st.render();
            string expected = "context [g] 1:1 attribute z isn't defined" + newline;
            string result = errors.ToString();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestMissingNamedArg()
        {
            string dir = tmpdir;
            string groupFile =
                "f(x,y) ::= \"<x><y>\"\n" +
                "g() ::= \"<f(x={a},{b})>\"";
            //012345678901234567

            writeFile(dir, "group.stg", groupFile);
            STGroupFile group = new STGroupFile(Path.Combine(dir, "group.stg"));
            ErrorBuffer errors = new ErrorBuffer();
            group.setListener(errors);
            group.load();
            string expected = "group.stg 2:18: mismatched input '{' expecting ID" + newline;
            string result = errors.ToString();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestNamedArgsNotAllowInIndirectInclude()
        {
            string dir = tmpdir;
            string groupFile =
                "f(x,y) ::= \"<x><y>\"\n" +
                "g(name) ::= \"<(name)(x={a},y={b})>\"";
            //0123456789012345678901234567890
            writeFile(dir, "group.stg", groupFile);
            STGroupFile group = new STGroupFile(Path.Combine(dir, "group.stg"));
            ErrorBuffer errors = new ErrorBuffer();
            group.setListener(errors);
            group.load();
            string expected = "group.stg 2:22: '=' came as a complete surprise to me" + newline;
            string result = errors.ToString();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestCantSeeGroupDirIfGroupFileOfSameName()
        {
            string dir = tmpdir;
            string a = "a() ::= <<dir1 a>>\n";
            writeFile(dir, "group/a.st", a); // can't see this file

            string groupFile =
                "b() ::= \"group file b\"\n";
            writeFile(dir, "group.stg", groupFile);

            STGroup group1 = new STGroupDir(dir);
            ST st = group1.getInstanceOf("group/a"); // can't see
            Assert.AreEqual(null, st);
        }

        // test fully-qualified template refs

        [TestMethod]
        public void TestFullyQualifiedGetInstanceOf()
        {
            string dir = tmpdir;
            string a =
                "a(x) ::= <<" + newline +
                "foo" + newline +
                ">>" + newline;
            writeFile(dir, "a.st", a);
            STGroup group = new STGroupDir(dir);
            ST st = group.getInstanceOf("a");
            string expected = "foo";
            string result = st.render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestFullyQualifiedTemplateRef()
        {
            // /randomdir/a and /randomdir/subdir/b
            string dir = tmpdir;
            string a = "a() ::= << <subdir/b()> >>\n";
            string b = "b() ::= <<bar>>\n";
            writeFile(dir + "/subdir", "a.st", a);
            writeFile(dir + "/subdir", "b.st", b);
            STGroup group = new STGroupDir(dir);
            ST st = group.getInstanceOf("subdir/a");
            string expected = " bar ";
            string result = st.render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestFullyQualifiedTemplateRef2()
        {
            // /randomdir/a and /randomdir/group.stg with b and c templates
            string dir = tmpdir;
            string a =
                "a(x) ::= << <group/b()> >>\n";
            writeFile(dir, "a.st", a);
            string groupFile =
                "b() ::= \"bar\"\n" +
                "c() ::= \"<a()>\"\n";
            writeFile(dir, "group.stg", groupFile);
            STGroup group = new STGroupDir(dir);
            ST st1 = group.getInstanceOf("a");
            ST st2 = group.getInstanceOf("group/c"); // invokes /a
            string expected = " bar  bar ";
            string result = st1.render() + st2.render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestUnloadingSimpleGroup()
        {
            string dir = tmpdir;
            string a =
                "a(x) ::= <<foo>>\n";
            string b =
                "b() ::= <<bar>>\n";
            writeFile(dir, "a.st", a);
            writeFile(dir, "b.st", b);
            STGroup group = new STGroupDir(dir);
            group.load(); // force load
            ST st = group.getInstanceOf("a");
            int originalHashCode = RuntimeHelpers.GetHashCode(st);
            group.unload(); // blast cache
            st = group.getInstanceOf("a");
            int newHashCode = RuntimeHelpers.GetHashCode(st);
            Assert.AreEqual(originalHashCode == newHashCode, false); // diff objects
            string expected = "foo";
            string result = st.render();
            Assert.AreEqual(expected, result);
            st = group.getInstanceOf("b");
            expected = "bar";
            result = st.render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestUnloadingGroupFile()
        {
            string dir = tmpdir;
            string a =
                "a(x) ::= <<foo>>\n" +
                "b() ::= <<bar>>\n";
            writeFile(dir, "a.stg", a);
            STGroup group = new STGroupFile(dir + "/a.stg");
            group.load(); // force load
            ST st = group.getInstanceOf("a");
            int originalHashCode = RuntimeHelpers.GetHashCode(st);
            group.unload(); // blast cache
            st = group.getInstanceOf("a");
            int newHashCode = RuntimeHelpers.GetHashCode(st);
            Assert.AreEqual(originalHashCode == newHashCode, false); // diff objects
            string expected = "foo";
            string result = st.render();
            Assert.AreEqual(expected, result);
            st = group.getInstanceOf("b");
            expected = "bar";
            result = st.render();
            Assert.AreEqual(expected, result);
        }
    }
}
