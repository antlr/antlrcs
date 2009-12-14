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

    [TestClass]
    public class TestImports : StringTemplateTestBase
    {
        [TestMethod]
        public void TestImportTemplate()
        {
            string dir1 = GetRandomDir();
            string a = "a() ::= <<dir1 a>>\n";
            string b = "b() ::= <<dir1 b>>\n";
            WriteFile(dir1, "a.st", a);
            WriteFile(dir1, "b.st", b);
            string dir2 = GetRandomDir();
            a = "a() ::= << <b()> >>\n";
            WriteFile(dir2, "a.st", a);

            TemplateGroup group1 = new TemplateGroupDirectory(dir1);
            TemplateGroup group2 = new TemplateGroupDirectory(dir2);
            group2.ImportTemplates(group1);
            Template st = group2.GetInstanceOf("b");
            string expected = "dir1 b";
            string result = st.Render();
            Assert.AreEqual(expected, result);

            // do it again, but make a template ref imported template
            st = group2.GetInstanceOf("a");
            expected = " dir1 b ";
            result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestImportTemplateInGroupFileFromDir()
        {
            string dir = GetRandomDir();
            string a = "a() ::= << <b()> >>\n";
            WriteFile(dir, "x/a.st", a);

            string groupFile =
                "b() ::= \"group file b\"\n" +
                "c() ::= \"group file c\"\n";
            WriteFile(dir, "y/group.stg", groupFile);

            TemplateGroup group1 = new TemplateGroupDirectory(dir + "/x");
            TemplateGroup group2 = new TemplateGroupFile(dir + "/y/group.stg");
            group1.ImportTemplates(group2);
            Template st = group1.GetInstanceOf("/a");
            st.code.Dump();
            string expected = " group file b ";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestImportTemplateInDirFromGroupFile()
        {
            string dir = GetRandomDir();
            string a = "a() ::= <<dir1 a>>\n";
            WriteFile(dir, "group/a.st", a);

            string groupFile =
                "b() ::= \"<a()>\"\n";
            WriteFile(dir, "group.stg", groupFile);

            TemplateGroup group1 = new TemplateGroupDirectory(dir);
            TemplateGroup group2 = new TemplateGroupFile(dir + "/group.stg");
            group2.ImportTemplates(group1);
            Template st = group2.GetInstanceOf("/group/b");
            string expected = "dir1 a";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestImportTemplateInGroupFileFromGroupFile()
        {
            string dir = GetRandomDir();
            string groupFile =
                "a() ::= \"g1 a\"\n" +
                "b() ::= \"<c()>\"\n";
            WriteFile(dir, "x/group.stg", groupFile);

            groupFile =
                "b() ::= \"g2 b\"\n" +
                "c() ::= \"g2 c\"\n";
            WriteFile(dir, "y/group.stg", groupFile);

            TemplateGroup group1 = new TemplateGroupFile(dir + "/x/group.stg");
            TemplateGroup group2 = new TemplateGroupFile(dir + "/y/group.stg");
            group1.ImportTemplates(group2);
            Template st = group1.GetInstanceOf("/b");
            string expected = "g2 c";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestImportTemplateFromSubdir()
        {
            // /randomdir/x/subdir/a and /randomdir/y/subdir/b
            string dir = GetRandomDir();
            string a = "a() ::= << <b()> >>\n";
            string b = "b() ::= <<x/subdir/b>>\n";
            WriteFile(dir, "x/subdir/a.st", a);
            WriteFile(dir, "y/subdir/b.st", b);

            TemplateGroup group1 = new TemplateGroupDirectory(dir + "/x");
            TemplateGroup group2 = new TemplateGroupDirectory(dir + "/y");
            group1.ImportTemplates(group2);
            Template st = group1.GetInstanceOf("/subdir/a");
            string expected = " x/subdir/b ";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestImportTemplateFromGroupFile()
        {
            // /randomdir/x/subdir/a and /randomdir/y/subdir.stg which has a and b
            string dir = GetRandomDir();
            string a = "a() ::= << <b()> >>\n"; // get b imported from subdir.stg
            WriteFile(dir, "x/subdir/a.st", a);

            string groupFile =
                "a() ::= \"group file a\"\n" +
                "b() ::= \"group file b\"\n";
            WriteFile(dir, "y/subdir.stg", groupFile);

            TemplateGroup group1 = new TemplateGroupDirectory(dir + "/x");
            TemplateGroup group2 = new TemplateGroupDirectory(dir + "/y");
            group1.ImportTemplates(group2);
            Template st = group1.GetInstanceOf("/subdir/a");
            string expected = " group file b ";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestPolymorphicTemplateReference()
        {
            string dir1 = GetRandomDir();
            string b = "b() ::= <<dir1 b>>\n";
            WriteFile(dir1, "b.st", b);
            string dir2 = GetRandomDir();
            string a = "a() ::= << <b()> >>\n";
            b = "b() ::= <<dir2 b>>\n";
            WriteFile(dir2, "a.st", a);
            WriteFile(dir2, "b.st", b);

            TemplateGroup group1 = new TemplateGroupDirectory(dir1);
            TemplateGroup group2 = new TemplateGroupDirectory(dir2);
            group1.ImportTemplates(group2);

            // normal lookup; a created from dir2 calls dir2.b
            Template st = group2.GetInstanceOf("a");
            string expected = " dir2 b ";
            string result = st.Render();
            Assert.AreEqual(expected, result);

            // polymorphic lookup; a created from dir1 calls dir2.a which calls dir1.b
            st = group1.GetInstanceOf("a");
            expected = " dir1 b ";
            result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestSuper()
        {
            string dir1 = GetRandomDir();
            string a = "a() ::= <<dir1 a>>\n";
            string b = "b() ::= <<dir1 b>>\n";
            WriteFile(dir1, "a.st", a);
            WriteFile(dir1, "b.st", b);
            string dir2 = GetRandomDir();
            a = "a() ::= << [<super.a()>] >>\n";
            WriteFile(dir2, "a.st", a);

            TemplateGroup group1 = new TemplateGroupDirectory(dir1);
            TemplateGroup group2 = new TemplateGroupDirectory(dir2);
            group2.ImportTemplates(group1);
            Template st = group2.GetInstanceOf("a");
            string expected = " [dir1 a] ";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }
    }
}
