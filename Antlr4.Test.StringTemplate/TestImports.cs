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
 * NOT LIMITED TO, PROCUREMENT OF SUBTemplateITUTE GOODS OR SERVICES; LOSS OF USE,
 * DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
 * THEORY OF LIABILITY, WHETHER IN CONTRACT, TemplateRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
 * THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

namespace Antlr4.Test.StringTemplate
{
    using System.Runtime.CompilerServices;
    using Antlr4.StringTemplate;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using ErrorBuffer = Antlr4.StringTemplate.Misc.ErrorBuffer;
    using Path = System.IO.Path;

    [TestClass]
    public class TestImports : BaseTest
    {
        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestImportDir()
        {
            /*
            dir1
                g.stg has a() that imports dir2 with absolute path
            dir2
                a.st
                b.st
             */
            string dir1 = Path.Combine(tmpdir, "dir1");
            string dir2 = Path.Combine(tmpdir, "dir2");
            string gstr =
                "import \"" + dir2 + "\"\n" +
                "a() ::= <<dir1 a>>\n";
            writeFile(dir1, "g.stg", gstr);

            string a = "a() ::= <<dir2 a>>\n";
            string b = "b() ::= <<dir2 b>>\n";
            writeFile(dir2, "a.st", a);
            writeFile(dir2, "b.st", b);

            TemplateGroup group = new TemplateGroupFile(Path.Combine(dir1, "g.stg"));
            Template st = group.GetInstanceOf("b"); // visible only if import worked
            string expected = "dir2 b";
            string result = st?.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestImportRelativeDir()
        {
            /*
            dir
                g.stg has a() that imports subdir with relative path
                subdir
                    a.st
                    b.st
                    c.st
             */
            string dir = tmpdir;
            string gstr =
                "import \"subdir\"\n" + // finds subdir in dir
                "a() ::= <<dir1 a>>\n";
            writeFile(dir, "g.stg", gstr);

            string a = "a() ::= <<subdir a>>\n";
            string b = "b() ::= <<subdir b>>\n";
            string c = "c() ::= <<subdir b>>\n";
            writeFile(dir, Path.Combine("subdir", "a.st"), a);
            writeFile(dir, Path.Combine("subdir", "b.st"), b);
            writeFile(dir, Path.Combine("subdir", "c.st"), c);

            TemplateGroup group = new TemplateGroupFile(Path.Combine(dir, "g.stg"));
            Template st = group.GetInstanceOf("b"); // visible only if import worked
            string expected = "subdir b";
            string result = st?.Render();
            Assert.AreEqual(expected, result);
            st = group.GetInstanceOf("c");
            result = st?.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestEmptyGroupImportGroupFileSameDir()
        {
            /*
            dir
                group1.stg		that imports group2.stg in same dir with just filename
                group2.stg		has c()
             */
            string dir = tmpdir;
            string groupFile =
                "import \"group2.stg\"\n";
            writeFile(dir, "group1.stg", groupFile);

            groupFile =
                "c() ::= \"g2 c\"\n";
            writeFile(dir, "group2.stg", groupFile);

            TemplateGroup group1 = new TemplateGroupFile(Path.Combine(dir, "group1.stg"));
            Template st = group1.GetInstanceOf("c"); // should see c()
            string expected = "g2 c";
            string result = st?.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestImportGroupFileSameDir()
        {
            /*
            dir
                group1.stg		that imports group2.stg in same dir with just filename
                group2.stg		has c()
             */
            string dir = tmpdir;
            string groupFile =
                "import \"group2.stg\"\n" +
                "a() ::= \"g1 a\"\n" +
                "b() ::= \"<c()>\"\n";
            writeFile(dir, "group1.stg", groupFile);

            groupFile =
                "c() ::= \"g2 c\"\n";
            writeFile(dir, "group2.stg", groupFile);

            TemplateGroup group1 = new TemplateGroupFile(Path.Combine(dir, "group1.stg"));
            Template st = group1.GetInstanceOf("c"); // should see c()
            string expected = "g2 c";
            string result = st?.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestImportRelativeGroupFile()
        {
            /*
            dir
                group1.stg		that imports group2.stg in same dir with just filename
                subdir
                    group2.stg	has c()
             */
            string dir = tmpdir;
            string groupFile =
                "import \"subdir/group2.stg\"\n" +
                "a() ::= \"g1 a\"\n" +
                "b() ::= \"<c()>\"\n";
            writeFile(dir, "group1.stg", groupFile);

            groupFile =
                "c() ::= \"g2 c\"\n";
            writeFile(dir, Path.Combine("subdir", "group2.stg"), groupFile);

            TemplateGroup group1 = new TemplateGroupFile(Path.Combine(dir, "group1.stg"));
            Template st = group1.GetInstanceOf("c"); // should see c()
            string expected = "g2 c";
            string result = st?.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestImportTemplateFileSameDir()
        {
            /*
            dir
                group1.stg		(that imports c.st)
                c.st
             */
            string dir = tmpdir;
            string groupFile =
                "import \"c.st\"\n" +
                "a() ::= \"g1 a\"\n" +
                "b() ::= \"<c()>\"\n";
            writeFile(dir, "group1.stg", groupFile);
            writeFile(dir, "c.st", "c() ::= \"c\"\n");

            TemplateGroup group1 = new TemplateGroupFile(Path.Combine(dir, "group1.stg"));
            Template st = group1.GetInstanceOf("c"); // should see c()
            string expected = "c";
            string result = st?.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestImportRelativeTemplateFile()
        {
            /*
            dir
                group1.stg		that imports c.st
                subdir
                    c.st
             */
            string dir = tmpdir;
            string groupFile =
                "import \"subdir/c.st\"\n" +
                "a() ::= \"g1 a\"\n" +
                "b() ::= \"<c()>\"\n";
            writeFile(dir, "group1.stg", groupFile);

            string stFile =
                "c() ::= \"c\"\n";
            writeFile(dir, Path.Combine("subdir", "c.st"), stFile);

            TemplateGroup group1 = new TemplateGroupFile(Path.Combine(dir, "group1.stg"));
            Template st = group1.GetInstanceOf("c"); // should see c()
            string expected = "c";
            string result = st?.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestImportTemplateFromAnotherGroupObject()
        {
            /*
            dir1
                a.st
                b.st
            dir2
                a.st
             */
            string dir1 = tmpdir;
            string a = "a() ::= <<dir1 a>>\n";
            string b = "b() ::= <<dir1 b>>\n";
            writeFile(dir1, "a.st", a);
            writeFile(dir1, "b.st", b);
            string dir2 = tmpdir;
            a = "a() ::= << <b()> >>\n";
            writeFile(dir2, "a.st", a);

            TemplateGroup group1 = new TemplateGroupDirectory(dir1);
            TemplateGroup group2 = new TemplateGroupDirectory(dir2);
            group2.ImportTemplates(group1);
            Template st = group2.GetInstanceOf("b");
            string expected = "dir1 b";
            string result = st?.Render();
            Assert.AreEqual(expected, result);

            // do it again, but make a template ref imported template
            st = group2.GetInstanceOf("a");
            expected = " dir1 b ";
            result = st?.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestImportTemplateInGroupFileFromDir()
        {
            string dir = tmpdir;
            string a = "a() ::= << <b()> >>\n";
            writeFile(dir, "x/a.st", a);

            string groupFile =
                "b() ::= \"group file b\"\n" +
                "c() ::= \"group file c\"\n";
            writeFile(dir, Path.Combine("y", "group.stg"), groupFile);

            TemplateGroup group1 = new TemplateGroupDirectory(Path.Combine(dir, "x"));
            TemplateGroup group2 = new TemplateGroupFile(Path.Combine(dir, "y", "group.stg"));
            group1.ImportTemplates(group2);
            Template st = group1.GetInstanceOf("a");
            st.impl.Dump();
            string expected = " group file b ";
            string result = st?.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestImportTemplateInGroupFileFromGroupFile()
        {
            string dir = tmpdir;
            string groupFile =
                "a() ::= \"g1 a\"\n" +
                "b() ::= \"<c()>\"\n";
            writeFile(dir, Path.Combine("x", "group.stg"), groupFile);

            groupFile =
                "b() ::= \"g2 b\"\n" +
                "c() ::= \"g2 c\"\n";
            writeFile(dir, Path.Combine("y", "group.stg"), groupFile);

            TemplateGroup group1 = new TemplateGroupFile(Path.Combine(dir, "x", "group.stg"));
            TemplateGroup group2 = new TemplateGroupFile(Path.Combine(dir, "y", "group.stg"));
            group1.ImportTemplates(group2);
            Template st = group1.GetInstanceOf("b");
            string expected = "g2 c";
            string result = st?.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestImportTemplateFromSubdir()
        {
            // /randomdir/x/subdir/a and /randomdir/y/subdir/b
            string dir = tmpdir;
            string a = "a() ::= << </subdir/b()> >>\n";
            string b = "b() ::= <<x's subdir/b>>\n";
            writeFile(dir, Path.Combine("x", "subdir", "a.st"), a);
            writeFile(dir, Path.Combine("y", "subdir", "b.st"), b);

            TemplateGroup group1 = new TemplateGroupDirectory(Path.Combine(dir, "x"));
            TemplateGroup group2 = new TemplateGroupDirectory(Path.Combine(dir, "y"));
            group1.ImportTemplates(group2);
            Template st = group1.GetInstanceOf("subdir/a");
            string expected = " x's subdir/b ";
            string result = st?.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestImportTemplateFromGroupFile()
        {
            // /randomdir/x/subdir/a and /randomdir/y/subdir.stg which has a and b
            string dir = tmpdir;
            string a = "a() ::= << </subdir/b()> >>\n"; // get b imported from subdir.stg
            writeFile(dir, Path.Combine("x", "subdir", "a.st"), a);

            string groupFile =
                "a() ::= \"group file: a\"\n" +
                "b() ::= \"group file: b\"\n";
            writeFile(dir, Path.Combine("y", "subdir.stg"), groupFile);

            TemplateGroup group1 = new TemplateGroupDirectory(Path.Combine(dir, "x"));
            TemplateGroup group2 = new TemplateGroupDirectory(Path.Combine(dir, "y"));
            group1.ImportTemplates(group2);

            Template st = group1.GetInstanceOf("subdir/a");

            Assert.IsNotNull(st);
            Assert.IsNotNull(group1.GetInstanceOf("subdir/b"));

            string expected = " group file: b ";
            string result = st?.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestPolymorphicTemplateReference()
        {
            string dir1 = Path.Combine(tmpdir, "d1");
            string b = "b() ::= <<dir1 b>>\n";
            writeFile(dir1, "b.st", b);
            string dir2 = Path.Combine(tmpdir, "d2");
            string a = "a() ::= << <b()> >>\n";
            b = "b() ::= <<dir2 b>>\n";
            writeFile(dir2, "a.st", a);
            writeFile(dir2, "b.st", b);

            TemplateGroup group1 = new TemplateGroupDirectory(dir1);
            TemplateGroup group2 = new TemplateGroupDirectory(dir2);
            group1.ImportTemplates(group2);

            // normal lookup; a created from dir2 calls dir2.b
            Template st = group2.GetInstanceOf("a");
            string expected = " dir2 b ";
            string result = st?.Render();
            Assert.AreEqual(expected, result);

            // polymorphic lookup; a created from dir1 calls dir2.a which calls dir1.b
            st = group1.GetInstanceOf("a");
            expected = " dir1 b ";
            result = st?.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestSuper()
        {
            string dir1 = Path.Combine(tmpdir, "d1");
            string a = "a() ::= <<dir1 a>>\n";
            string b = "b() ::= <<dir1 b>>\n";
            writeFile(dir1, "a.st", a);
            writeFile(dir1, "b.st", b);
            string dir2 = Path.Combine(tmpdir, "d2");
            a = "a() ::= << [<super.a()>] >>\n";
            writeFile(dir2, "a.st", a);

            TemplateGroup group1 = new TemplateGroupDirectory(dir1);
            TemplateGroup group2 = new TemplateGroupDirectory(dir2);
            group2.ImportTemplates(group1);
            Template st = group2.GetInstanceOf("a");
            string expected = " [dir1 a] ";
            string result = st?.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestUnloadImportedTemplate()
        {
            string dir1 = Path.Combine(tmpdir, "d1");
            string a = "a() ::= <<dir1 a>>\n";
            string b = "b() ::= <<dir1 b>>\n";
            writeFile(dir1, "a.st", a);
            writeFile(dir1, "b.st", b);
            string dir2 = Path.Combine(tmpdir, "d2");
            a = "a() ::= << <b()> >>\n";
            writeFile(dir2, "a.st", a);

            TemplateGroup group1 = new TemplateGroupDirectory(dir1);
            TemplateGroup group2 = new TemplateGroupDirectory(dir2);
            group2.ImportTemplates(group1);

            Template st = group2.GetInstanceOf("a");
            Template st2 = group2.GetInstanceOf("b");
            int originalHashCode = RuntimeHelpers.GetHashCode(st);
            int originalHashCode2 = RuntimeHelpers.GetHashCode(st2);
            group1.Unload(); // blast cache
            st = group2.GetInstanceOf("a");
            int newHashCode = RuntimeHelpers.GetHashCode(st);
            Assert.AreEqual(originalHashCode == newHashCode, false); // diff objects

            string expected = " dir1 b ";
            string result = st?.Render();
            Assert.AreEqual(expected, result);

            st = group2.GetInstanceOf("b");
            int newHashCode2 = RuntimeHelpers.GetHashCode(st);
            Assert.AreEqual(originalHashCode2 == newHashCode2, false); // diff objects
            result = st?.Render();
            expected = "dir1 b";
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestUnloadImportedTemplatedSpecifiedInGroupFile()
        {
            writeFile(tmpdir, "t.stg",
                    "import \"g1.stg\"\n\nmain() ::= <<\nv1-<f()>\n>>");
            writeFile(tmpdir, "g1.stg", "f() ::= \"g1\"");
            writeFile(tmpdir, "g2.stg", "f() ::= \"g2\"\nf2() ::= \"f2\"\n");
            TemplateGroup group = new TemplateGroupFile(tmpdir + "/t.stg");
            Template st = group.GetInstanceOf("main");
            Assert.AreEqual("v1-g1", st?.Render());

            // Change the imports of group t.
            writeFile(tmpdir, "t.stg",
                    "import \"g2.stg\"\n\nmain() ::= <<\nv2-<f()>;<f2()>\n>>");
            group.Unload(); // will also unload already imported groups
            st = group.GetInstanceOf("main");
            Assert.AreEqual("v2-g2;f2", st?.Render());
        }

        /** Cannot import from a group file unless it's the root.
         */
        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestGroupFileInDirImportsAnotherGroupFile()
        {
            // /randomdir/group.stg with a() imports /randomdir/imported.stg with b()
            // can't have groupdir then groupfile inside that imports
            string dir = tmpdir;
            string groupFile =
                "import \"imported.stg\"\n" +
                "a() ::= \"a: <b()>\"\n";
            writeFile(dir, "group.stg", groupFile);
            string importedFile =
                "b() ::= \"b\"\n";
            writeFile(dir, "imported.stg", importedFile);
            ITemplateErrorListener errors = new ErrorBuffer();
            TemplateGroup group = new TemplateGroupDirectory(dir);
            group.Listener = errors;
            group.GetInstanceOf("/group/a");
            string result = errors.ToString();
            string substring =
                "import illegal in group files embedded in TemplateGroupDirectory; import \"imported.stg\" in TemplateGroupDirectory";
            StringAssert.Contains(result, substring);
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestGroupFileInDirImportsAGroupDir()
        {
            /*
            dir
                g.stg has a() that imports subdir with relative path
                subdir
                    b.st
                    c.st
             */
            string dir = tmpdir;
            string gstr =
                "import \"subdir\"\n" + // finds subdir in dir
                "a() ::= \"a: <b()>\"\n";
            writeFile(dir, "g.stg", gstr);

            writeFile(dir, "subdir/b.st", "b() ::= \"b: <c()>\"\n");
            writeFile(dir, "subdir/c.st", "c() ::= <<subdir c>>\n");

            TemplateGroup group = new TemplateGroupFile(dir + "/g.stg");
            Template st = group.GetInstanceOf("a");
            string expected = "a: b: subdir c";
            string result = st?.Render();
            Assert.AreEqual(expected, result);
        }
    }
}
