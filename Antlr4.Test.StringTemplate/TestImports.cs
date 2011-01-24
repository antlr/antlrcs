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
    using System.Runtime.CompilerServices;
    using Path = System.IO.Path;

    [TestClass]
    public class TestImports : BaseTest
    {
        [TestMethod]
        public void TestImportTemplate()
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
            group2.importTemplates(group1);
            Template st = group2.getInstanceOf("b");
            string expected = "dir1 b";
            string result = st.Render();
            Assert.AreEqual(expected, result);

            // do it again, but make a template ref imported template
            st = group2.getInstanceOf("a");
            expected = " dir1 b ";
            result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestImportStatementWithDir()
        {
            string dir1 = tmpdir + "/dir1";
            string dir2 = tmpdir + "/dir2";
            string a =
                "import \"" + dir2 + "\"\n" +
                "a() ::= <<dir1 a>>\n";
            writeFile(dir1, "a.stg", a);

            a = "a() ::= <<dir2 a>>\n";
            string b = "b() ::= <<dir2 b>>\n";
            writeFile(dir2, "a.st", a);
            writeFile(dir2, "b.st", b);

            TemplateGroup group = new TemplateGroupFile(dir1 + "/a.stg");
            Template st = group.getInstanceOf("b"); // visible only if import worked
            string expected = "dir2 b";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestImportStatementWithFile()
        {
            string dir = tmpdir;
            string groupFile =
                "import \"" + dir + "/group2.stg\"\n" +
                "a() ::= \"g1 a\"\n" +
                "b() ::= \"<c()>\"\n";
            writeFile(dir, "group1.stg", groupFile);

            groupFile =
                "c() ::= \"g2 c\"\n";
            writeFile(dir, "group2.stg", groupFile);

            TemplateGroup group1 = new TemplateGroupFile(dir + "/group1.stg");
            Template st = group1.getInstanceOf("c"); // should see c()
            string expected = "g2 c";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestImportTemplateInGroupFileFromDir()
        {
            string dir = tmpdir;
            string a = "a() ::= << <b()> >>\n";
            writeFile(dir, "x/a.st", a);

            string groupFile =
                "b() ::= \"group file b\"\n" +
                "c() ::= \"group file c\"\n";
            writeFile(dir, "y/group.stg", groupFile);

            TemplateGroup group1 = new TemplateGroupDirectory(dir + "/x");
            TemplateGroup group2 = new TemplateGroupFile(dir + "/y/group.stg");
            group1.importTemplates(group2);
            Template st = group1.getInstanceOf("a");
            st.impl.dump();
            string expected = " group file b ";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestImportTemplateInGroupFileFromGroupFile()
        {
            string dir = tmpdir;
            string groupFile =
                "a() ::= \"g1 a\"\n" +
                "b() ::= \"<c()>\"\n";
            writeFile(dir, "x/group.stg", groupFile);

            groupFile =
                "b() ::= \"g2 b\"\n" +
                "c() ::= \"g2 c\"\n";
            writeFile(dir, "y/group.stg", groupFile);

            TemplateGroup group1 = new TemplateGroupFile(dir + "/x/group.stg");
            TemplateGroup group2 = new TemplateGroupFile(dir + "/y/group.stg");
            group1.importTemplates(group2);
            Template st = group1.getInstanceOf("b");
            string expected = "g2 c";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestImportTemplateFromSubdir()
        {
            // /randomdir/x/subdir/a and /randomdir/y/subdir/b
            string dir = tmpdir;
            string a = "a() ::= << <subdir/b()> >>\n";
            string b = "b() ::= <<x's subdir/b>>\n";
            writeFile(dir, "x/subdir/a.st", a);
            writeFile(dir, "y/subdir/b.st", b);

            TemplateGroup group1 = new TemplateGroupDirectory(dir + "/x");
            TemplateGroup group2 = new TemplateGroupDirectory(dir + "/y");
            group1.importTemplates(group2);
            Template st = group1.getInstanceOf("subdir/a");
            string expected = " x's subdir/b ";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestImportTemplateFromGroupFile()
        {
            // /randomdir/x/subdir/a and /randomdir/y/subdir.stg which has a and b
            string dir = tmpdir;
            string a = "a() ::= << <subdir/b()> >>\n"; // get b imported from subdir.stg
            writeFile(dir, "x/subdir/a.st", a);

            string groupFile =
                "a() ::= \"group file: a\"\n" +
                "b() ::= \"group file: b\"\n";
            writeFile(dir, "y/subdir.stg", groupFile);

            TemplateGroup group1 = new TemplateGroupDirectory(dir + "/x");
            TemplateGroup group2 = new TemplateGroupDirectory(dir + "/y");
            group1.importTemplates(group2);
            Template st = group1.getInstanceOf("subdir/a");
            string expected = " group file: b ";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
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
            group1.importTemplates(group2);

            // normal lookup; a created from dir2 calls dir2.b
            Template st = group2.getInstanceOf("a");
            string expected = " dir2 b ";
            string result = st.Render();
            Assert.AreEqual(expected, result);

            // polymorphic lookup; a created from dir1 calls dir2.a which calls dir1.b
            st = group1.getInstanceOf("a");
            expected = " dir1 b ";
            result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
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
            group2.importTemplates(group1);
            Template st = group2.getInstanceOf("a");
            string expected = " [dir1 a] ";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
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
            group2.importTemplates(group1);

            Template st = group2.getInstanceOf("a");
            Template st2 = group2.getInstanceOf("b");
            int originalHashCode = RuntimeHelpers.GetHashCode(st);
            int originalHashCode2 = RuntimeHelpers.GetHashCode(st2);
            group1.unload(); // blast cache
            st = group2.getInstanceOf("a");
            int newHashCode = RuntimeHelpers.GetHashCode(st);
            Assert.AreEqual(originalHashCode == newHashCode, false); // diff objects

            string expected = " dir1 b ";
            string result = st.Render();
            Assert.AreEqual(expected, result);

            st = group2.getInstanceOf("b");
            int newHashCode2 = RuntimeHelpers.GetHashCode(st);
            Assert.AreEqual(originalHashCode2 == newHashCode2, false); // diff objects
            result = st.Render();
            expected = "dir1 b";
            Assert.AreEqual(expected, result);
        }
    }
}
