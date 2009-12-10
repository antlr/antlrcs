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
        public void TestAttemptToAccessTemplateUnderGroupFile()
        {
            string dir = GetRandomDir();
            string groupFile =
                "a() ::= \"bar\"\n";
            WriteFile(dir, "group.stg", groupFile);
            TemplateGroup group = new TemplateGroupFile(Path.Combine(dir, "group.stg"));
            string error = null;
            try
            {
                group.GetInstanceOf("sub/b"); // can't have sub under group file
            }
            catch (ArgumentException iae)
            {
                error = iae.Message;
            }
            string expected = "can't use relative template name sub/b";
            string result = error;
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestAttemptToUseWrongGroupFileNameFromRoot()
        {
            string dir = GetRandomDir();
            string groupFile =
                "a() ::= \"bar\"\n";
            WriteFile(dir, "group.stg", groupFile);
            TemplateGroup group = new TemplateGroupFile(Path.Combine(dir, "group.stg"));
            string error = null;
            try
            {
                group.GetInstanceOf("/sub/a"); // can't have sub under group file
            }
            catch (ArgumentException iae)
            {
                error = iae.Message;
            }
            string expected = "name must be of form /group/templatename: /sub/a";
            string result = error;
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestAttemptToGoTooDeepUsingGroupFileNameFromRoot()
        {
            string dir = GetRandomDir();
            string groupFile =
                "a() ::= \"bar\"\n";
            WriteFile(dir, "group.stg", groupFile);
            TemplateGroup group = new TemplateGroupFile(Path.Combine(dir, "group.stg"));
            string error = null;
            try
            {
                group.GetInstanceOf("/group/b/b"); // can't have sub under group file
            }
            catch (ArgumentException iae)
            {
                error = iae.Message;
            }
            string expected = "name must be of form /group/templatename: /group/b/b";
            string result = error;
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestAttemptToAccessDirWithSameNameAsTemplate()
        {
            string dir = GetRandomDir();
            string a =
                "a(x) ::= <<foo>>\n";
            WriteFile(dir, "a.st", a);
            TemplateGroup group = new TemplateGroupDirectory(dir);
            string error = null;
            try
            {
                group.GetInstanceOf("a/b"); // 'a' is a template 
            }
            catch (ArgumentException iae)
            {
                error = iae.Message;
            }
            string expected = "a is a template not a dir or group file";
            string result = error;
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestAttemptToAccessSubDirWithWrongRootName()
        {
            string dir = GetRandomDir();
            string a =
                "a(x) ::= <<foo>>\n";
            WriteFile(dir + "/subdir", "a.st", a);
            TemplateGroup group = new TemplateGroupDirectory(Path.Combine(dir, "subdir"));
            string error = null;
            try
            {
                group.GetInstanceOf("/x/b"); // name is subdir not x
            }
            catch (ArgumentException iae)
            {
                error = iae.Message;
            }
            string expected = "no such subdirectory or group file: x";
            string result = error;
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
            string expected = " bar ";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }
    }
}
