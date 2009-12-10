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
    using ArgumentException = System.ArgumentException;
    using Path = System.IO.Path;
    using ST = StringTemplate.Template;
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
            String dir = GetRandomDir();
            String a =
                "a(x) ::= <<" + newline +
                "foo" + newline +
                ">>" + newline;
            WriteFile(dir, "a.st", a);
            STGroup group = new STGroupDir(dir);
            ST st = group.GetInstanceOf("a");
            String expected = "foo" + newline;
            String result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestGroupWithTwoTemplates()
        {
            String dir = GetRandomDir();
            String a =
                "a(x) ::= <<" + newline +
                "foo" + newline +
                ">>" + newline;
            WriteFile(dir, "a.st", a);
            String b =
                "b() ::= \"bar\"" + newline;
            WriteFile(dir, "b.st", b);
            STGroup group = new STGroupDir(dir);
            ST st1 = group.GetInstanceOf("a");
            ST st2 = group.GetInstanceOf("b");
            String expected = "foo" + newline + "bar";
            String result = st1.Render() + st2.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestSubdir()
        {
            // /randomdir/a and /randomdir/subdir/b
            String dir = GetRandomDir();
            String a =
                "a(x) ::= <<" + newline +
                "foo" + newline +
                ">>" + newline;
            WriteFile(dir, "a.st", a);
            String b =
                "b() ::= \"bar\"" + newline;
            WriteFile(Path.Combine(dir, "subdir"), "b.st", b);
            STGroup group = new STGroupDir(dir);
            ST st1 = group.GetInstanceOf("a");
            ST st2 = group.GetInstanceOf("subdir/b");
            String expected = "foo" + newline + "bar";
            String result = st1.Render() + st2.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestGroupFileInDir()
        {
            // /randomdir/a and /randomdir/group.stg with b and c templates
            String dir = GetRandomDir();
            String a =
                "a(x) ::= <<\n" +
                "foo\n" +
                ">>\n";
            WriteFile(dir, "a.st", a);
            String groupFile =
                "b() ::= \"bar\"\n" +
                "c() ::= \"duh\"\n";
            WriteFile(dir, "group.stg", groupFile);
            STGroup group = new STGroupDir(dir);
            ST st1 = group.GetInstanceOf("a");
            ST st2 = group.GetInstanceOf("group/b");
            ST st3 = group.GetInstanceOf("group/c");
            String expected = "foo" + newline + "barduh";
            String result = st1.Render() + st2.Render() + st3.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestSubSubdir()
        {
            // /randomdir/a and /randomdir/subdir/b
            String dir = GetRandomDir();
            String a =
                "a(x) ::= <<" + newline +
                "foo" + newline +
                ">>" + newline;
            WriteFile(dir, "a.st", a);
            String b =
                "b() ::= \"bar\"" + newline;
            WriteFile(Path.Combine(Path.Combine(dir, "sub1"), "sub2"), "b.st", b);
            STGroup group = new STGroupDir(dir);
            ST st1 = group.GetInstanceOf("a");
            ST st2 = group.GetInstanceOf("sub1/sub2/b");
            String expected = "foo" + newline + "bar";
            String result = st1.Render() + st2.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestGroupFileInSubDir()
        {
            // /randomdir/a and /randomdir/group.stg with b and c templates
            String dir = GetRandomDir();
            String a =
                "a(x) ::= <<\n" +
                "foo\n" +
                ">>\n";
            WriteFile(dir, "a.st", a);
            String groupFile =
                "b() ::= \"bar\"\n" +
                "c() ::= \"duh\"\n";
            WriteFile(dir, Path.Combine("subdir", "group.stg"), groupFile);
            STGroup group = new STGroupDir(dir);
            ST st1 = group.GetInstanceOf("a");
            ST st2 = group.GetInstanceOf("subdir/group/b");
            ST st3 = group.GetInstanceOf("subdir/group/c");
            String expected = "foo" + newline + "barduh";
            String result = st1.Render() + st2.Render() + st3.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestAttemptToAccessTemplateUnderGroupFile()
        {
            String dir = GetRandomDir();
            String groupFile =
                "a() ::= \"bar\"\n";
            WriteFile(dir, "group.stg", groupFile);
            STGroup group = new STGroupFile(Path.Combine(dir, "group.stg"));
            String error = null;
            try
            {
                group.GetInstanceOf("sub/b"); // can't have sub under group file
            }
            catch (ArgumentException iae)
            {
                error = iae.Message;
            }
            String expected = "can't use relative template name sub/b";
            String result = error;
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestAttemptToUseWrongGroupFileNameFromRoot()
        {
            String dir = GetRandomDir();
            String groupFile =
                "a() ::= \"bar\"\n";
            WriteFile(dir, "group.stg", groupFile);
            STGroup group = new STGroupFile(Path.Combine(dir, "group.stg"));
            String error = null;
            try
            {
                group.GetInstanceOf("/sub/a"); // can't have sub under group file
            }
            catch (ArgumentException iae)
            {
                error = iae.Message;
            }
            String expected = "name must be of form /group/templatename: /sub/a";
            String result = error;
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestAttemptToGoTooDeepUsingGroupFileNameFromRoot()
        {
            String dir = GetRandomDir();
            String groupFile =
                "a() ::= \"bar\"\n";
            WriteFile(dir, "group.stg", groupFile);
            STGroup group = new STGroupFile(Path.Combine(dir, "group.stg"));
            String error = null;
            try
            {
                group.GetInstanceOf("/gropu/b/b"); // can't have sub under group file
            }
            catch (ArgumentException iae)
            {
                error = iae.Message;
            }
            String expected = "name must be of form /group/templatename: /gropu/b/b";
            String result = error;
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestAttemptToAccessDirWithSameNameAsTemplate()
        {
            String dir = GetRandomDir();
            String a =
                "a(x) ::= <<foo>>\n";
            WriteFile(dir, "a.st", a);
            STGroup group = new STGroupDir(dir);
            String error = null;
            try
            {
                group.GetInstanceOf("a/b"); // 'a' is a template 
            }
            catch (ArgumentException iae)
            {
                error = iae.Message;
            }
            String expected = "a is a template not a dir or group file";
            String result = error;
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestAttemptToAccessSubDirWithWrongRootName()
        {
            String dir = GetRandomDir();
            String a =
                "a(x) ::= <<foo>>\n";
            WriteFile(dir + "/subdir", "a.st", a);
            STGroup group = new STGroupDir(Path.Combine(dir, "subdir"));
            String error = null;
            try
            {
                group.GetInstanceOf("/x/b"); // name is subdir not x
            }
            catch (ArgumentException iae)
            {
                error = iae.Message;
            }
            String expected = "x doesn't match directory name subdir";
            String result = error;
            Assert.AreEqual(expected, result);
        }
    }
}
