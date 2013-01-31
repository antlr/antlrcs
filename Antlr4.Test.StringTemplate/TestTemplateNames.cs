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
    using Antlr4.StringTemplate;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Path = System.IO.Path;

    [TestClass]
    public class TestTemplateNames : BaseTest
    {
        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestAbsoluteTemplateRefFromOutside()
        {
            // /randomdir/a and /randomdir/subdir/b
            string dir = tmpdir;
            writeFile(dir, "a.st", "a(x) ::= << </subdir/b()> >>\n");
            writeFile(Path.Combine(dir, "subdir"), "b.st", "b() ::= <<bar>>\n");
            TemplateGroup group = new TemplateGroupDirectory(dir);
            Assert.AreEqual(" bar ", group.GetInstanceOf("a").Render());
            Assert.AreEqual(" bar ", group.GetInstanceOf("/a").Render());
            Assert.AreEqual("bar", group.GetInstanceOf("/subdir/b").Render());
        }


        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestRelativeTemplateRefInExpr()
        {
            // /randomdir/a and /randomdir/subdir/b
            string dir = tmpdir;
            writeFile(dir, "a.st", "a(x) ::= << <subdir/b()> >>\n");
            writeFile(Path.Combine(dir, "subdir"), "b.st", "b() ::= <<bar>>\n");
            TemplateGroup group = new TemplateGroupDirectory(dir);
            Assert.AreEqual(" bar ", group.GetInstanceOf("a").Render());
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestAbsoluteTemplateRefInExpr()
        {
            // /randomdir/a and /randomdir/subdir/b
            string dir = tmpdir;
            writeFile(dir, "a.st", "a(x) ::= << </subdir/b()> >>\n");
            writeFile(Path.Combine(dir, "subdir"), "b.st", "b() ::= <<bar>>\n");
            TemplateGroup group = new TemplateGroupDirectory(dir);
            Assert.AreEqual(" bar ", group.GetInstanceOf("a").Render());
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestRefToAnotherTemplateInSameGroup()
        {
            string dir = tmpdir;
            writeFile(dir, "a.st", "a() ::= << <b()> >>\n");
            writeFile(dir, "b.st", "b() ::= <<bar>>\n");
            TemplateGroup group = new TemplateGroupDirectory(dir);
            Template st = group.GetInstanceOf("a");
            string expected = " bar ";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestRefToAnotherTemplateInSameSubdir()
        {
            // /randomdir/a and /randomdir/subdir/b
            string dir = tmpdir;
            writeFile(Path.Combine(dir, "subdir"), "a.st", "a() ::= << <b()> >>\n");
            writeFile(Path.Combine(dir, "subdir"), "b.st", "b() ::= <<bar>>\n");
            TemplateGroup group = new TemplateGroupDirectory(dir);
            group.GetInstanceOf("/subdir/a").impl.Dump();
            Assert.AreEqual(" bar ", group.GetInstanceOf("/subdir/a").Render());
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestFullyQualifiedGetInstanceOf()
        {
            string dir = tmpdir;
            writeFile(dir, "a.st", "a(x) ::= <<foo>>");
            TemplateGroup group = new TemplateGroupDirectory(dir);
            Assert.AreEqual("foo", group.GetInstanceOf("a").Render());
            Assert.AreEqual("foo", group.GetInstanceOf("/a").Render());
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestFullyQualifiedTemplateRef()
        {
            // /randomdir/a and /randomdir/subdir/b
            string dir = tmpdir;
            writeFile(Path.Combine(dir, "subdir"), "a.st", "a() ::= << </subdir/b()> >>\n");
            writeFile(Path.Combine(dir, "subdir"), "b.st", "b() ::= <<bar>>\n");
            TemplateGroup group = new TemplateGroupDirectory(dir);

            Template template = group.GetInstanceOf("/subdir/a");
            Assert.IsNotNull(template);
            Assert.AreEqual(" bar ", template.Render());

            template = group.GetInstanceOf("subdir/a");
            Assert.IsNotNull(template);
            Assert.AreEqual(" bar ", template.Render());
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestFullyQualifiedTemplateRef2()
        {
            // /randomdir/a and /randomdir/group.stg with b and c templates
            string dir = tmpdir;
            writeFile(dir, "a.st", "a(x) ::= << </group/b()> >>\n");
            string groupFile =
                "b() ::= \"bar\"\n" +
                "c() ::= \"</a()>\"\n";
            writeFile(dir, "group.stg", groupFile);
            TemplateGroup group = new TemplateGroupDirectory(dir);

            Template st1 = group.GetInstanceOf("/a");
            Assert.IsNotNull(st1);
            Assert.AreEqual(" bar ", st1.Render());

            Template st2 = group.GetInstanceOf("/group/c"); // invokes /a
            Assert.IsNotNull(st2);
            Assert.AreEqual(" bar ", st2.Render());
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestRelativeInSubdir()
        {
            // /randomdir/a and /randomdir/subdir/b
            string dir = tmpdir;
            writeFile(dir, "a.st", "a(x) ::= << </subdir/c()> >>\n");
            writeFile(Path.Combine(dir, "subdir"), "b.st", "b() ::= <<bar>>\n");
            writeFile(Path.Combine(dir, "subdir"), "c.st", "c() ::= << <b()> >>\n");
            TemplateGroup group = new TemplateGroupDirectory(dir);
            Assert.AreEqual("  bar  ", group.GetInstanceOf("a").Render());
        }

        // TODO: test <a/b()> is RELATIVE NOT ABSOLUTE
    }
}
