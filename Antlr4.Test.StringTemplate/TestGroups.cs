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
            TemplateGroup group = new TemplateGroupDirectory(dir);
            Template st = group.GetInstanceOf("a");
            string expected = "foo";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestSimpleGroupFromString()
        {
            string g =
                "a(x) ::= <<foo>>\n" +
                "b() ::= <<bar>>\n";
            TemplateGroup group = new TemplateGroupString(g);
            Template st = group.GetInstanceOf("a");
            string expected = "foo";
            string result = st.Render();
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
            TemplateGroup group = new TemplateGroupDirectory(dir);
            Template st1 = group.GetInstanceOf("a");
            Template st2 = group.GetInstanceOf("b");
            string expected = "foobar";
            string result = st1.Render() + st2.Render();
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
            TemplateGroup group = new TemplateGroupDirectory(dir);
            Template st1 = group.GetInstanceOf("a");
            Template st2 = group.GetInstanceOf("subdir/b");
            string expected = "foobar";
            string result = st1.Render() + st2.Render();
            Assert.AreEqual(expected, result);
            st2 = group.GetInstanceOf("subdir/b"); // should work with / in front too
            expected = "bar";
            result = st2.Render();
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
            TemplateGroup group = new TemplateGroupDirectory(dir);
            Template st = group.GetInstanceOf("a");
            string expected = " bar ";
            string result = st.Render();
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
            TemplateGroup group = new TemplateGroupDirectory(dir);
            Template st1 = group.GetInstanceOf("a");
            Template st2 = group.GetInstanceOf("group/b");
            Template st3 = group.GetInstanceOf("group/c");
            string expected = "foobarduh";
            string result = st1.Render() + st2.Render() + st3.Render();
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
            TemplateGroup group = new TemplateGroupDirectory(dir);
            Template st1 = group.GetInstanceOf("a");
            Template st2 = group.GetInstanceOf("sub1/sub2/b");
            string expected = "foobar";
            string result = st1.Render() + st2.Render();
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
            TemplateGroup group = new TemplateGroupDirectory(dir);
            Template st1 = group.GetInstanceOf("a");
            Template st2 = group.GetInstanceOf("subdir/group/b");
            Template st3 = group.GetInstanceOf("subdir/group/c");
            string expected = "foobarduh";
            string result = st1.Render() + st2.Render() + st3.Render();
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
            TemplateGroup group = new TemplateGroupDirectory(dir);
            Template st = group.GetInstanceOf("a");
            string expected = " bar ";
            string result = st.Render();
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
            TemplateGroup group = new TemplateGroupDirectory(dir);
            Template st = group.GetInstanceOf("subdir/a");
            st.impl.Dump();
            string expected = " bar ";
            string result = st.Render();
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
            TemplateGroupFile group = new TemplateGroupFile(Path.Combine(dir, "group.stg"));
            group.Listener = errors;
            group.Load();
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
            TemplateGroupFile group = new TemplateGroupFile(Path.Combine(dir, "group.stg"));
            Template st = group.GetInstanceOf("b");
            string expected = "bar";
            string result = st.Render();
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
            TemplateGroupFile group = new TemplateGroupFile(Path.Combine(dir, "group.stg"));
            Template st = group.GetInstanceOf("b");
            st.Add("x", 1);
            st.Add("y", 2);
            string expected = "12";
            string result = st.Render();
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
            TemplateGroup group = new TemplateGroupDirectory(dir);
            Template st = group.GetInstanceOf("a");
            string expected = " foo ";
            string result = st.Render();
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
            TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "group.stg"));
            Template b = group.GetInstanceOf("method");
            b.Add("name", "foo");
            string expecting = "x=99; // foo";
            string result = b.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestBooleanDefaultArguments()
        {
            string templates =
                    "method(name) ::= <<" + newline +
                    "<stat(name)>" + newline +
                    ">>" + newline +
                    "stat(name,x=true,y=false) ::= \"<name>; <x> <y>\"" + newline
                    ;
            writeFile(tmpdir, "group.stg", templates);
            TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "group.stg"));
            Template b = group.GetInstanceOf("method");
            b.Add("name", "foo");
            string expecting = "foo; True False";
            string result = b.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestDefaultArgument2()
        {
            string templates =
                    "stat(name,value=\"99\") ::= \"x=<value>; // <name>\"" + newline
                    ;
            writeFile(tmpdir, "group.stg", templates);
            TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "group.stg"));
            Template b = group.GetInstanceOf("stat");
            b.Add("name", "foo");
            string expecting = "x=99; // foo";
            string result = b.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestSubtemplateAsDefaultArgSeesOtherArgs()
        {
            string templates =
                "t(x,y={<x:{s|<s><z>}>},z=\"foo\") ::= <<\n" +
                "x: <x>\n" +
                "y: <y>\n" +
                ">>" + newline
                ;
            writeFile(tmpdir, "group.stg", templates);
            TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "group.stg"));
            Template b = group.GetInstanceOf("t");
            b.Add("x", "a");
            string expecting =
                "x: a" + newline +
                "y: afoo";
            string result = b.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestEarlyEvalOfDefaultArgs()
        {
            string templates =
                "s(x,y={<(x)>}) ::= \"<x><y>\"\n"; // should see x in def arg
            TemplateGroup group = new TemplateGroupString(templates);
            Template b = group.GetInstanceOf("s");
            b.Add("x", "a");
            string expecting = "aa";
            string result = b.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestDefaultArgumentAsSimpleTemplate()
        {
            string templates =
                    "stat(name,value={99}) ::= \"x=<value>; // <name>\"" + newline
                    ;
            writeFile(tmpdir, "group.stg", templates);
            TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "group.stg"));
            Template b = group.GetInstanceOf("stat");
            b.Add("name", "foo");
            string expecting = "x=99; // foo";
            string result = b.Render();
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
            TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "group.stg"));
            Template m = group.GetInstanceOf("method");
            m.Add("fields", new Field());
            string expecting = "x=parrt; // parrt";
            string result = m.Render();
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
            TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "group.stg"));
            Template m = group.GetInstanceOf("method");
            m.Add("fields", new Field());
            string expecting = "x=parrt; // parrt";
            string result = m.Render();
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
            TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "group.stg"));
            Template m = group.GetInstanceOf("method");
            m.Add("fields", new Field());
            string expecting = "x=parrt; // parrt";
            string result = m.Render();
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
            TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "group.stg"));
            Template b = group.GetInstanceOf("method");
            b.Add("name", "foo");
            b.Add("size", "2");
            string expecting = "x=foo; // foo";
            string result = b.Render();
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
            TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "group.stg"));
            Template b = group.GetInstanceOf("method");
            b.Add("name", "foo");
            b.Add("size", "2");
            string expecting = "x=[foo] ; // foo"; // won't see ' ' after '=' since it's an indent not simple string
            string result = b.Render();
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
            TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "group.stg"));
            Template b = group.GetInstanceOf("method");
            b.Add("name", "foo");
            string expecting = "x=34; // foo";
            string result = b.Render();
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
            TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "group.stg"));
            Template a = group.GetInstanceOf("A");
            a.Add("x", new Counter());
            string expecting = "0 1 2 0"; // trace must be false to get these numbers
            string result = a.Render();
            //System.err.println("result='"+result+"'");
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestTrueFalseArgs()
        {
            string groupFile =
                "f(x,y) ::= \"<x><y>\"\n" +
                "g() ::= \"<f(true,{a})>\"";
            writeFile(tmpdir, "group.stg", groupFile);
            TemplateGroupFile group = new TemplateGroupFile(Path.Combine(tmpdir, "group.stg"));
            Template st = group.GetInstanceOf("g");
            string expected = "Truea";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestNamedArgsInOrder()
        {
            string dir = tmpdir;
            string groupFile =
                "f(x,y) ::= \"<x><y>\"\n" +
                "g() ::= \"<f(x={a},y={b})>\"";
            writeFile(dir, "group.stg", groupFile);
            TemplateGroupFile group = new TemplateGroupFile(Path.Combine(dir, "group.stg"));
            Template st = group.GetInstanceOf("g");
            string expected = "ab";
            string result = st.Render();
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
            TemplateGroupFile group = new TemplateGroupFile(Path.Combine(dir, "group.stg"));
            Template st = group.GetInstanceOf("g");
            string expected = "ab";
            string result = st.Render();
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
            TemplateGroupFile group = new TemplateGroupFile(Path.Combine(dir, "group.stg"));
            ErrorBuffer errors = new ErrorBuffer();
            group.Listener = errors;
            Template st = group.GetInstanceOf("g");
            st.Render();
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
            TemplateGroupFile group = new TemplateGroupFile(Path.Combine(dir, "group.stg"));
            ErrorBuffer errors = new ErrorBuffer();
            group.Listener = errors;
            group.Load();
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
            TemplateGroupFile group = new TemplateGroupFile(Path.Combine(dir, "group.stg"));
            ErrorBuffer errors = new ErrorBuffer();
            group.Listener = errors;
            group.Load();
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

            TemplateGroup group1 = new TemplateGroupDirectory(dir);
            Template st = group1.GetInstanceOf("group/a"); // can't see
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
            TemplateGroup group = new TemplateGroupDirectory(dir);
            Template st = group.GetInstanceOf("a");
            string expected = "foo";
            string result = st.Render();
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
            TemplateGroup group = new TemplateGroupDirectory(dir);
            Template st = group.GetInstanceOf("subdir/a");
            string expected = " bar ";
            string result = st.Render();
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
            TemplateGroup group = new TemplateGroupDirectory(dir);
            Template st1 = group.GetInstanceOf("a");
            Template st2 = group.GetInstanceOf("group/c"); // invokes /a
            string expected = " bar  bar ";
            string result = st1.Render() + st2.Render();
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
            TemplateGroup group = new TemplateGroupDirectory(dir);
            group.Load(); // force load
            Template st = group.GetInstanceOf("a");
            int originalHashCode = RuntimeHelpers.GetHashCode(st);
            group.Unload(); // blast cache
            st = group.GetInstanceOf("a");
            int newHashCode = RuntimeHelpers.GetHashCode(st);
            Assert.AreEqual(originalHashCode == newHashCode, false); // diff objects
            string expected = "foo";
            string result = st.Render();
            Assert.AreEqual(expected, result);
            st = group.GetInstanceOf("b");
            expected = "bar";
            result = st.Render();
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
            TemplateGroup group = new TemplateGroupFile(dir + "/a.stg");
            group.Load(); // force load
            Template st = group.GetInstanceOf("a");
            int originalHashCode = RuntimeHelpers.GetHashCode(st);
            group.Unload(); // blast cache
            st = group.GetInstanceOf("a");
            int newHashCode = RuntimeHelpers.GetHashCode(st);
            Assert.AreEqual(originalHashCode == newHashCode, false); // diff objects
            string expected = "foo";
            string result = st.Render();
            Assert.AreEqual(expected, result);
            st = group.GetInstanceOf("b");
            expected = "bar";
            result = st.Render();
            Assert.AreEqual(expected, result);
        }
    }
}
