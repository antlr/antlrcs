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
    using ArgumentException = System.ArgumentException;
    using Path = System.IO.Path;
    using ST = StringTemplate.Template;
    using STErrorListener = StringTemplate.ITemplateErrorListener;
    using STGroup = StringTemplate.TemplateGroup;
    using STGroupDir = StringTemplate.TemplateGroupDirectory;
    using STGroupFile = StringTemplate.TemplateGroupFile;
    using String = System.String;

    [TestClass]
    public class TestGroups : StringTemplateTestBase
    {
        [TestMethod]
        public void TestSimpleGroup()
        {
            string dir = GetRandomDir();
            string a =
                "a(x) ::= <<" + newline +
                "foo" + newline +
                ">>" + newline;
            WriteFile(dir, "a.st", a);
            TemplateGroup group = new TemplateGroupDirectory(dir);
            Template st = group.GetInstanceOf("a");
            string expected = "foo" + newline;
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestGroupWithTwoTemplates()
        {
            string dir = GetRandomDir();
            string a =
                "a(x) ::= <<" + newline +
                "foo" + newline +
                ">>" + newline;
            WriteFile(dir, "a.st", a);
            string b =
                "b() ::= \"bar\"" + newline;
            WriteFile(dir, "b.st", b);
            TemplateGroup group = new TemplateGroupDirectory(dir);
            Template st1 = group.GetInstanceOf("a");
            Template st2 = group.GetInstanceOf("b");
            string expected = "foo" + newline + "bar";
            string result = st1.Render() + st2.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestSubdir()
        {
            // /randomdir/a and /randomdir/subdir/b
            string dir = GetRandomDir();
            string a =
                "a(x) ::= <<" + newline +
                "foo" + newline +
                ">>" + newline;
            WriteFile(dir, "a.st", a);
            string b =
                "b() ::= \"bar\"" + newline;
            WriteFile(Path.Combine(dir, "subdir"), "b.st", b);
            TemplateGroup group = new TemplateGroupDirectory(dir);
            Template st1 = group.GetInstanceOf("a");
            Template st2 = group.GetInstanceOf("subdir/b");
            string expected = "foo" + newline + "bar";
            string result = st1.Render() + st2.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestAbsoluteTemplateRef()
        {
            // /randomdir/a and /randomdir/subdir/b
            string dir = GetRandomDir();
            string a =
                "a(x) ::= << </subdir/b()> >>\n";
            WriteFile(dir, "a.st", a);
            string b =
                "b() ::= <<bar>>\n";
            WriteFile(dir + "/subdir", "b.st", b);
            TemplateGroup group = new TemplateGroupDirectory(dir);
            Template st = group.GetInstanceOf("a");
            st.code.Dump();
            string expected = " bar ";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestGroupFileInDir()
        {
            // /randomdir/a and /randomdir/group.stg with b and c templates
            string dir = GetRandomDir();
            string a =
                "a(x) ::= <<\n" +
                "foo\n" +
                ">>\n";
            WriteFile(dir, "a.st", a);
            string groupFile =
                "b() ::= \"bar\"\n" +
                "c() ::= \"duh\"\n";
            WriteFile(dir, "group.stg", groupFile);
            TemplateGroup group = new TemplateGroupDirectory(dir);
            Template st1 = group.GetInstanceOf("a");
            Template st2 = group.GetInstanceOf("group/b");
            Template st3 = group.GetInstanceOf("group/c");
            string expected = "foo" + newline + "barduh";
            string result = st1.Render() + st2.Render() + st3.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestSubSubdir()
        {
            // /randomdir/a and /randomdir/subdir/b
            string dir = GetRandomDir();
            string a =
                "a(x) ::= <<" + newline +
                "foo" + newline +
                ">>" + newline;
            WriteFile(dir, "a.st", a);
            string b =
                "b() ::= \"bar\"" + newline;
            WriteFile(Path.Combine(Path.Combine(dir, "sub1"), "sub2"), "b.st", b);
            TemplateGroup group = new TemplateGroupDirectory(dir);
            Template st1 = group.GetInstanceOf("a");
            Template st2 = group.GetInstanceOf("sub1/sub2/b");
            string expected = "foo" + newline + "bar";
            string result = st1.Render() + st2.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestGroupFileInSubDir()
        {
            // /randomdir/a and /randomdir/group.stg with b and c templates
            string dir = GetRandomDir();
            string a =
                "a(x) ::= <<\n" +
                "foo\n" +
                ">>\n";
            WriteFile(dir, "a.st", a);
            string groupFile =
                "b() ::= \"bar\"\n" +
                "c() ::= \"duh\"\n";
            WriteFile(dir, Path.Combine("subdir", "group.stg"), groupFile);
            TemplateGroup group = new TemplateGroupDirectory(dir);
            Template st1 = group.GetInstanceOf("a");
            Template st2 = group.GetInstanceOf("subdir/group/b");
            Template st3 = group.GetInstanceOf("subdir/group/c");
            string expected = "foo" + newline + "barduh";
            string result = st1.Render() + st2.Render() + st3.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestRefToAnotherTemplateInSameGroup()
        {
            string dir = GetRandomDir();
            string a = "a() ::= << <b()> >>\n";
            string b = "b() ::= <<bar>>\n";
            WriteFile(dir, "a.st", a);
            WriteFile(dir, "b.st", b);
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
            string dir = GetRandomDir();
            string a = "a() ::= << <b()> >>\n";
            string b = "b() ::= <<bar>>\n";
            WriteFile(dir + "/subdir", "a.st", a);
            WriteFile(dir + "/subdir", "b.st", b);
            TemplateGroup group = new TemplateGroupDirectory(dir);
            Template st = group.GetInstanceOf("subdir/a");
            st.code.Dump();
            string expected = " bar ";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestDupDef()
        {
            string dir = GetRandomDir();
            string groupFile =
                "b() ::= \"bar\"\n" +
                "b() ::= \"duh\"\n";
            WriteFile(dir, "group.stg", groupFile);
            TemplateGroup group = new TemplateGroupFile(dir + "/group.stg");
            group.Load();
        }

        [TestMethod]
        public void TestSimpleDefaultArg()
        {
            string dir = GetRandomDir();
            string a = "a() ::= << <b()> >>\n";
            string b = "b(x=\"foo\") ::= \"<x>\"\n";
            WriteFile(dir, "a.st", a);
            WriteFile(dir, "b.st", b);
            STGroup group = new STGroupDir(dir);
            ST st = group.GetInstanceOf("a");
            string expected = " foo ";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestDefaultArgument()
        {
            string templates =
                    "method(name) ::= <<" + newline +
                    "<stat(...)>" + newline +
                    ">>" + newline +
                    "stat(name,value=\"99\") ::= \"x=<value>; // <name>\"" + newline
                    ;
            WriteFile(tmpdir, "group.stg", templates);
            STGroup group = new STGroupFile(tmpdir + "/group.stg");
            ST b = group.GetInstanceOf("method");
            b.Add("name", "foo");
            string expecting = "x=99; // foo" + newline;
            string result = b.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestDefaultArgument2()
        {
            String templates =
                    "stat(name,value=\"99\") ::= \"x=<value>; // <name>\"" + newline
                    ;
            WriteFile(tmpdir, "group.stg", templates);
            STGroup group = new STGroupFile(tmpdir + "/group.stg");
            ST b = group.GetInstanceOf("stat");
            b.Add("name", "foo");
            String expecting = "x=99; // foo";
            String result = b.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestDefaultArgumentManuallySet()
        {
            String templates =
                    "method(fields) ::= <<" + newline +
                    "<fields:{f | <stat(f=f)>}>" + newline +
                    ">>" + newline +
                    "stat(f,value={<f.name>}) ::= \"x=<value>; // <f.name>\"" + newline
                    ;
            WriteFile(tmpdir, "group.stg", templates);
            STGroup group = new STGroupFile(tmpdir + "/group.stg");
            ST m = group.GetInstanceOf("method");
            m.Add("fields", new Field());
            String expecting = "x=parrt; // parrt" + newline;
            String result = m.Render();
            Assert.AreEqual(expecting, result);
        }

        /** This fails because checkNullAttributeAgainstFormalArguments looks
         *  for a formal argument at the current level not of the original embedded
         *  template. We have defined it all the way in the embedded, but there is
         *  no value so we try to look upwards ala dynamic scoping. When it reaches
         *  the top, it doesn't find a value but it will miss the
         *  formal argument down in the embedded.
         *
         *  By definition, though, the formal parameter exists if we have
         *  a default value. look up the value to see if it's null without
         *  checking checkNullAttributeAgainstFormalArguments.
         */
        [TestMethod]
        public void TestDefaultArgumentImplicitlySet()
        {
            String templates =
                    "method(fields) ::= <<" + newline +
                    "<fields:{f | <stat(...)>}>" + newline +
                    ">>" + newline +
                    "stat(f,value={<f.name>}) ::= \"x=<value>; // <f.name>\"" + newline
                    ;
            WriteFile(tmpdir, "group.stg", templates);
            STGroup group = new STGroupFile(tmpdir + "/group.stg");
            ST m = group.GetInstanceOf("method");
            m.Add("fields", new Field());
            String expecting = "x=parrt; // parrt" + newline;
            String result = m.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestDefaultArgumentImplicitlySet2()
        {
            String templates =
                    "method(fields) ::= <<" + newline +
                    "<fields:{f | <f:stat>}>" + newline +  // THIS SHOULD BE ERROR; >1 arg?
                    ">>" + newline +
                    "stat(f,value={<f.name>}) ::= \"x=<value>; // <f.name>\"" + newline
                    ;
            WriteFile(tmpdir, "group.stg", templates);
            STGroup group = new STGroupFile(tmpdir + "/group.stg");
            ST m = group.GetInstanceOf("method");
            m.Add("fields", new Field());
            String expecting = "x=parrt; // parrt" + newline;
            String result = m.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestDefaultArgumentAsTemplate()
        {
            String templates =
                    "method(name,size) ::= <<" + newline +
                    "<stat(...)>" + newline +
                    ">>" + newline +
                    "stat(name,value={<name>}) ::= \"x=<value>; // <name>\"" + newline
                    ;
            WriteFile(tmpdir, "group.stg", templates);
            STGroup group = new STGroupFile(tmpdir + "/group.stg");
            ST b = group.GetInstanceOf("method");
            b.Add("name", "foo");
            b.Add("size", "2");
            String expecting = "x=foo; // foo" + newline;
            String result = b.Render();
            //System.err.println("result='"+result+"'");
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestDefaultArgumentAsTemplate2()
        {
            String templates =
                    "method(name,size) ::= <<" + newline +
                    "<stat(...)>" + newline +
                    ">>" + newline +
                    "stat(name,value={ [<name>] }) ::= \"x=<value>; // <name>\"" + newline
                    ;
            WriteFile(tmpdir, "group.stg", templates);
            STGroup group = new STGroupFile(tmpdir + "/group.stg");
            ST b = group.GetInstanceOf("method");
            b.Add("name", "foo");
            b.Add("size", "2");
            String expecting = "x=[foo] ; // foo" + newline; // won't see ' ' after '=' since it's an indent not simple string
            String result = b.Render();
            //System.err.println("result='"+result+"'");
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestDoNotUseDefaultArgument()
        {
            String templates =
                    "method(name) ::= <<" + newline +
                    "<stat(value=\"34\",...)>" + newline +
                    ">>" + newline +
                    "stat(name,value=\"99\") ::= \"x=<value>; // <name>\"" + newline
                    ;
            WriteFile(tmpdir, "group.stg", templates);
            STGroup group = new STGroupFile(tmpdir + "/group.stg");
            ST b = group.GetInstanceOf("method");
            b.Add("name", "foo");
            String expecting = "x=34; // foo" + newline;
            String result = b.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestDefaultArgumentInParensToEvalEarly()
        {
            String templates =
                    "A(x) ::= \"<B()>\"" + newline +
                    "B(y={<(x)>}) ::= \"<y> <x> <x> <y>\"" + newline
                    ;
            WriteFile(tmpdir, "group.stg", templates);
            STGroup group = new STGroupFile(tmpdir + "/group.stg");
            ST b = group.GetInstanceOf("A");
            b.Add("x", new Counter());
            String expecting = "0 1 2 0"; // trace must be false to get these numbers
            String result = b.Render();
            //System.err.println("result='"+result+"'");
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestCompatibilityMode()
        {
            ErrorManager.CompatibilityMode = true;

            try
            {
                string dir = GetRandomDir();
                string a =
                    "foo\n" +
                    "bar\n";
                WriteFile(dir, "a.st", a);
                STGroup group = new STGroupDir(dir);
                ST st = group.GetInstanceOf("a");
                string expected =
                    "foo" + newline +
                    "bar";
                string result = st.Render();
                Assert.AreEqual(expected, result);
            }
            finally
            {
                ErrorManager.CompatibilityMode = false;
            }
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

        private class Counter
        {
            int n = 0;
            public override string ToString()
            {
                return (n++).ToString();
            }
        }
    }
}
