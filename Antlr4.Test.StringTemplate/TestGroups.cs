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
    using Antlr4.StringTemplate;
    using Antlr4.StringTemplate.Misc;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Path = System.IO.Path;

    [TestClass]
    public class TestGroups : BaseTest
    {
        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestSimpleGroup()
        {
            string dir = tmpdir;
            writeFile(dir, "a.st", "a(x) ::= <<foo>>");
            TemplateGroup group = new TemplateGroupDirectory(dir);
            Template st = group.GetInstanceOf("a");
            string expected = "foo";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestEscapeOneRightAngle()
        {
            string dir = tmpdir;
            writeFile(dir, "a.st", "a(x) ::= << > >>");
            TemplateGroup group = new TemplateGroupDirectory(dir);
            Template st = group.GetInstanceOf("a");
            st.Add("x", "parrt");
            string expected = " > ";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestEscapeJavaRightShift()
        {
            string dir = tmpdir;
            writeFile(dir, "a.st", "a(x) ::= << \\>> >>");
            TemplateGroup group = new TemplateGroupDirectory(dir);
            Template st = group.GetInstanceOf("a");
            st.Add("x", "parrt");
            string expected = " >> ";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestEscapeJavaRightShift2()
        {
            string dir = tmpdir;
            writeFile(dir, "a.st", "a(x) ::= << >\\> >>");
            TemplateGroup group = new TemplateGroupDirectory(dir);
            Template st = group.GetInstanceOf("a");
            st.Add("x", "parrt");
            string expected = " >> ";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestEscapeJavaRightShiftAtRightEdge()
        {
            string dir = tmpdir;
            writeFile(dir, "a.st", "a(x) ::= <<\\>>>"); // <<\>>>
            TemplateGroup group = new TemplateGroupDirectory(dir);
            Template st = group.GetInstanceOf("a");
            st.Add("x", "parrt");
            string expected = "\\>";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestEscapeJavaRightShiftAtRightEdge2()
        {
            string dir = tmpdir;
            writeFile(dir, "a.st", "a(x) ::= <<>\\>>>");
            TemplateGroup group = new TemplateGroupDirectory(dir);
            Template st = group.GetInstanceOf("a");
            st.Add("x", "parrt");
            string expected = ">>";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
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

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestGroupWithTwoTemplates()
        {
            string dir = tmpdir;
            writeFile(dir, "a.st", "a(x) ::= <<foo>>");
            writeFile(dir, "b.st", "b() ::= \"bar\"");
            TemplateGroup group = new TemplateGroupDirectory(dir);
            Template st1 = group.GetInstanceOf("a");
            Template st2 = group.GetInstanceOf("b");
            string expected = "foobar";
            string result = st1.Render() + st2.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestSubdir()
        {
            // /randomdir/a and /randomdir/subdir/b
            string dir = tmpdir;
            writeFile(dir, "a.st", "a(x) ::= <<foo>>");
            writeFile(Path.Combine(dir, "subdir"), "b.st", "b() ::= \"bar\"");
            TemplateGroup group = new TemplateGroupDirectory(dir);
            Assert.AreEqual("foo", group.GetInstanceOf("a").Render());
            Assert.AreEqual("bar", group.GetInstanceOf("/subdir/b").Render());
            Assert.AreEqual("bar", group.GetInstanceOf("subdir/b").Render());
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestSubdirWithSubtemplate()
        {
            // /randomdir/a and /randomdir/subdir/b
            string dir = tmpdir;
            writeFile(Path.Combine(dir, "subdir"), "a.st", "a(x) ::= \"<x:{y|<y>}>\"");
            TemplateGroup group = new TemplateGroupDirectory(dir);
            Template st = group.GetInstanceOf("/subdir/a");
            st.Add("x", new string[] { "a", "b" });
            Assert.AreEqual("ab", st.Render());
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestGroupFileInDir()
        {
            // /randomdir/a and /randomdir/group.stg with b and c templates
            string dir = tmpdir;
            writeFile(dir, "a.st", "a(x) ::= <<foo>>");
            string groupFile =
                "b() ::= \"bar\"\n" +
                "c() ::= \"duh\"\n";
            writeFile(dir, "group.stg", groupFile);
            TemplateGroup group = new TemplateGroupDirectory(dir);
            Assert.AreEqual("foo", group.GetInstanceOf("a").Render());
            Assert.AreEqual("bar", group.GetInstanceOf("/group/b").Render());
            Assert.AreEqual("duh", group.GetInstanceOf("/group/c").Render());
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestSubSubdir()
        {
            // /randomdir/a and /randomdir/subdir/b
            string dir = tmpdir;
            writeFile(dir, "a.st", "a(x) ::= <<foo>>");
            writeFile(Path.Combine(dir, "sub1", "sub2"), "b.st", "b() ::= \"bar\"");
            TemplateGroup group = new TemplateGroupDirectory(dir);
            Template st1 = group.GetInstanceOf("a");
            Template st2 = group.GetInstanceOf("/sub1/sub2/b");
            string expected = "foobar";
            string result = st1.Render() + st2.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestGroupFileInSubDir()
        {
            // /randomdir/a and /randomdir/group.stg with b and c templates
            string dir = tmpdir;
            writeFile(dir, "a.st", "a(x) ::= <<foo>>");
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

        [TestMethod][TestCategory(TestCategories.ST4)]
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

        [TestMethod][TestCategory(TestCategories.ST4)]
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

        [TestMethod][TestCategory(TestCategories.ST4)]
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

        [TestMethod][TestCategory(TestCategories.ST4)]
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

        [TestMethod][TestCategory(TestCategories.ST4)]
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

        [TestMethod][TestCategory(TestCategories.ST4)]
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
            string expecting = "foo; true false";
            string result = b.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
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

        [TestMethod][TestCategory(TestCategories.ST4)]
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

        [TestMethod][TestCategory(TestCategories.ST4)]
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

        [TestMethod][TestCategory(TestCategories.ST4)]
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

        [TestMethod][TestCategory(TestCategories.ST4)]
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

        [TestMethod][TestCategory(TestCategories.ST4)]
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

        [TestMethod][TestCategory(TestCategories.ST4)]
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

        [TestMethod][TestCategory(TestCategories.ST4)]
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

        [TestMethod][TestCategory(TestCategories.ST4)]
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

        [TestMethod][TestCategory(TestCategories.ST4)]
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

        /**
         * When the anonymous template specified as a default value for a formalArg
         * contains a syntax error Template 4.0.2 emits a NullPointerException error
         * (after the syntax error)
         * 
         * @throws Exception
         */
        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestHandleBuggyDefaultArgument()
        {
            string templates = "main(a={(<\"\")>}) ::= \"\"";
            writeFile(tmpdir, "t.stg", templates);

            ErrorBuffer errors = new ErrorBuffer();
            TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
            group.Listener = errors;

            Template st = group.GetInstanceOf("main");
            string s = st.Render();

            // Check the errors. This contained an "NullPointerException" before
            Assert.AreEqual(
                    "t.stg 1:12: mismatched input ')' expecting RDELIM" + newline,
                    errors.ToString());
        }

        private class Counter
        {
            int n = 0;
            public override string ToString()
            {
                return (n++).ToString();
            }
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
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

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestTrueFalseArgs()
        {
            string groupFile =
                "f(x,y) ::= \"<x><y>\"\n" +
                "g() ::= \"<f(true,{a})>\"";
            writeFile(tmpdir, "group.stg", groupFile);
            TemplateGroupFile group = new TemplateGroupFile(Path.Combine(tmpdir, "group.stg"));
            Template st = group.GetInstanceOf("g");
            string expected = "truea";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
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

        [TestMethod][TestCategory(TestCategories.ST4)]
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

        [TestMethod][TestCategory(TestCategories.ST4)]
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
            string expected = "context [/g] 1:1 attribute z isn't defined" + newline;
            string result = errors.ToString();
            Assert.AreEqual(expected, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
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
            string expected = "group.stg 2:18: mismatched input '{' expecting ELLIPSIS" + newline;
            string result = errors.ToString();
            Assert.AreEqual(expected, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
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
            // TODO: this could be more informative about the incorrect use of named arguments
            string expected = "group.stg 2:22: '=' came as a complete surprise to me" + newline;
            string result = errors.ToString();
            Assert.AreEqual(expected, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
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

        [TestMethod][TestCategory(TestCategories.ST4)]
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

        [TestMethod][TestCategory(TestCategories.ST4)]
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

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestGroupFileImport()
        {
            // /randomdir/group1.stg (a template) and /randomdir/group2.stg with b.
            // group1 imports group2, a includes b
            string dir = tmpdir;
            string groupFile1 =
                "import \"group2.stg\"\n" +
                "a(x) ::= <<\n" +
                "foo<b()>\n" +
                ">>\n";
            writeFile(dir, "group1.stg", groupFile1);
            string groupFile2 =
                "b() ::= \"bar\"\n";
            writeFile(dir, "group2.stg", groupFile2);
            TemplateGroup group1 = new TemplateGroupFile(Path.Combine(dir, "group1.stg"));

            // Is the imported template b found? 
            Template stb = group1.GetInstanceOf("b");
            Assert.AreEqual("bar", stb.Render());

            // Is the include of b() resolved?
            Template sta = group1.GetInstanceOf("a");
            Assert.AreEqual("foobar", sta.Render());

            // Are the correct "ThatCreatedThisInstance" groups assigned 
            Assert.AreEqual("group1", sta.Group.Name);
            Assert.AreEqual("group1", stb.Group.Name);

            // Are the correct (native) groups assigned for the templates 
            Assert.AreEqual("group1", sta.impl.NativeGroup.Name);
            Assert.AreEqual("group2", stb.impl.NativeGroup.Name);
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestGetTemplateNames()
        {
            string templates =
                "t() ::= \"foo\"\n" +
                "main() ::= \"<t()>\"";
            writeFile(tmpdir, "t.stg", templates);

            TemplateGroup group = new TemplateGroupFile(tmpdir + "/t.stg");
            // try to get an undefined template.
            // This will add an entry to the "templates" field in STGroup, however
            // this should not be returned.
            group.LookupTemplate("t2");

            HashSet<string> names = group.GetTemplateNames();

            // Should only contain "t" and "main" (not "t2")
            Assert.AreEqual(2, names.Count);
            CollectionAssert.Contains(names.ToList(), "/t");
            CollectionAssert.Contains(names.ToList(), "/main");
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestUnloadWithImports()
        {
            writeFile(tmpdir, "t.stg",
                    "import \"g1.stg\"\n\nmain() ::= <<\nv1-<f()>\n>>");
            writeFile(tmpdir, "g1.stg", "f() ::= \"g1\"");
            writeFile(tmpdir, "g2.stg", "f() ::= \"g2\"\nf2() ::= \"f2\"\n");
            TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
            Template st = group.GetInstanceOf("main");
            Assert.AreEqual("v1-g1", st.Render());

            // Change the text of group t, including the imports.
            writeFile(tmpdir, "t.stg", "import \"g2.stg\"\n\nmain() ::= <<\nv2-<f()>;<f2()>\n>>");
            group.Unload();
            st = group.GetInstanceOf("main");
            Assert.AreEqual("v2-g2;f2", st.Render());
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestLineBreakInGroup()
        {
            string templates =
                "t() ::= <<" + newline +
                "Foo <\\\\>" + newline +
                "  \t  bar" + newline +
                ">>" + newline;
            writeFile(tmpdir, "t.stg", templates);
            TemplateGroup group = new TemplateGroupFile(tmpdir + Path.DirectorySeparatorChar + "t.stg");
            Template st = group.GetInstanceOf("t");
            Assert.IsNotNull(st);
            string expecting = "Foo bar";     // expect \n in output
            Assert.AreEqual(expecting, st.Render());
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestLineBreakInGroup2()
        {
            string templates =
                "t() ::= <<" + newline +
                "Foo <\\\\>       " + newline +
                "  \t  bar" + newline +
                ">>" + newline;
            writeFile(tmpdir, "t.stg", templates);
            TemplateGroup group = new TemplateGroupFile(tmpdir + Path.DirectorySeparatorChar + "t.stg");
            Template st = group.GetInstanceOf("t");
            Assert.IsNotNull(st);
            string expecting = "Foo bar";     // expect \n in output
            Assert.AreEqual(expecting, st.Render());
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestLineBreakMissingTrailingNewline()
        {
            writeFile(tmpdir, "t.stg", "a(x) ::= <<<\\\\>\r\n>>"); // that is <<<\\>>> not an escaped >>
            ErrorBuffer errors = new ErrorBuffer();
            TemplateGroup group = new TemplateGroupFile(tmpdir + "/" + "t.stg");
            group.Listener = errors;
            Template st = group.GetInstanceOf("a");
            Assert.AreEqual("t.stg 1:15: Missing newline after newline escape <\\\\>" + newline, errors.ToString());
            st.Add("x", "parrt");
            string expected = "";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestLineBreakWithScarfedTrailingNewline()
        {
            writeFile(tmpdir, "t.stg", "a(x) ::= <<<\\\\>\r\n>>"); // \r\n removed as trailing whitespace
            ErrorBuffer errors = new ErrorBuffer();
            TemplateGroup group = new TemplateGroupFile(tmpdir + "/" + "t.stg");
            group.Listener = errors;
            Template st = group.GetInstanceOf("a");
            Assert.AreEqual("t.stg 1:15: Missing newline after newline escape <\\\\>" + newline, errors.ToString());
            st.Add("x", "parrt");
            string expected = "";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }
    }
}
