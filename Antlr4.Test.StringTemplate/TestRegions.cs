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
    using Path = System.IO.Path;

    [TestClass]
    public class TestRegions : BaseTest
    {
        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestEmbeddedRegion()
        {
            string dir = tmpdir;
            string groupFile =
                "a() ::= <<\n" +
                "[<@r>bar<@end>]\n" +
                ">>\n";
            writeFile(dir, "group.stg", groupFile);
            TemplateGroup group = new TemplateGroupFile(Path.Combine(dir, "group.stg"));
            Template st = group.GetInstanceOf("a");
            string expected = "[bar]";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestRegion()
        {
            string dir = tmpdir;
            string groupFile =
                "a() ::= <<\n" +
                "[<@r()>]\n" +
                ">>\n";
            writeFile(dir, "group.stg", groupFile);
            TemplateGroup group = new TemplateGroupFile(Path.Combine(dir, "group.stg"));
            Template st = group.GetInstanceOf("a");
            string expected = "[]";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestDefineRegionInSubgroup()
        {
            string dir = tmpdir;
            writeFile(dir, "g1.stg", "a() ::= <<[<@r()>]>>\n");
            writeFile(dir, "g2.stg", "@a.r() ::= <<foo>>\n");

            TemplateGroup group1 = new TemplateGroupFile(Path.Combine(dir, "g1.stg"));
            TemplateGroup group2 = new TemplateGroupFile(Path.Combine(dir, "g2.stg"));
            group2.ImportTemplates(group1); // define r in g2
            Template st = group2.GetInstanceOf("a");
            string expected = "[foo]";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestDefineRegionInSubgroupOneInSubdir()
        {
            string dir = tmpdir;
            writeFile(dir, "g1.stg", "a() ::= <<[<@r()>]>>\n");
            writeFile(Path.Combine(dir, "subdir"), "g2.stg", "@a.r() ::= <<foo>>\n");

            TemplateGroup group1 = new TemplateGroupFile(Path.Combine(dir, "g1.stg"));
            TemplateGroup group2 = new TemplateGroupFile(Path.Combine(dir, "subdir", "g2.stg"));
            group2.ImportTemplates(group1); // define r in g2
            Template st = group2.GetInstanceOf("a");
            string expected = "[foo]";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestDefineRegionInSubgroupBothInSubdir()
        {
            string dir = tmpdir;
            writeFile(Path.Combine(dir, "subdir"), "g1.stg", "a() ::= <<[<@r()>]>>\n");
            writeFile(Path.Combine(dir, "subdir"), "g2.stg", "@a.r() ::= <<foo>>\n");

            TemplateGroup group1 = new TemplateGroupFile(Path.Combine(dir, "subdir", "g1.stg"));
            TemplateGroup group2 = new TemplateGroupFile(Path.Combine(dir, "subdir", "g2.stg"));
            group2.ImportTemplates(group1); // define r in g2
            Template st = group2.GetInstanceOf("a");
            string expected = "[foo]";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestDefineRegionInSubgroupThatRefsSuper()
        {
            string dir = tmpdir;
            string g1 = "a() ::= <<[<@r>foo<@end>]>>\n";
            writeFile(dir, "g1.stg", g1);
            string g2 = "@a.r() ::= <<(<@super.r()>)>>\n";
            writeFile(dir, "g2.stg", g2);

            TemplateGroup group1 = new TemplateGroupFile(Path.Combine(dir, "g1.stg"));
            TemplateGroup group2 = new TemplateGroupFile(Path.Combine(dir, "g2.stg"));
            group2.ImportTemplates(group1); // define r in g2
            Template st = group2.GetInstanceOf("a");
            string expected = "[(foo)]";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestDefineRegionInSubgroup2()
        {
            string dir = tmpdir;
            string g1 = "a() ::= <<[<@r()>]>>\n";
            writeFile(dir, "g1.stg", g1);
            string g2 = "@a.r() ::= <<foo>>>\n";
            writeFile(dir, "g2.stg", g2);

            TemplateGroup group1 = new TemplateGroupFile(Path.Combine(dir, "g1.stg"));
            TemplateGroup group2 = new TemplateGroupFile(Path.Combine(dir, "g2.stg"));
            group1.ImportTemplates(group2); // opposite of previous; g1 imports g2
            Template st = group1.GetInstanceOf("a");
            string expected = "[]"; // @a.r implicitly defined in g1; can't see g2's
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestDefineRegionInSameGroup()
        {
            string dir = tmpdir;
            string g = "a() ::= <<[<@r()>]>>\n" +
                       "@a.r() ::= <<foo>>\n";
            writeFile(dir, "g.stg", g);

            TemplateGroup group = new TemplateGroupFile(Path.Combine(dir, "g.stg"));
            Template st = group.GetInstanceOf("a");
            string expected = "[foo]";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestAnonymousTemplateInRegion()
        {
            string dir = tmpdir;
            string g = "a() ::= <<[<@r()>]>>\n" +
                       "@a.r() ::= <<\n"+
                       "<[\"foo\"]:{x|<x>}>\n"+
                       ">>\n";
            writeFile(dir, "g.stg", g);

            TemplateGroup group = new TemplateGroupFile(Path.Combine(dir, "g.stg"));
            Template st = group.GetInstanceOf("a");
            string expected = "[foo]";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestCantDefineEmbeddedRegionAgain()
        {
            string dir = tmpdir;
            string g = "a() ::= <<[<@r>foo<@end>]>>\n" +
                       "@a.r() ::= <<bar>>\n"; // error; dup
            writeFile(dir, "g.stg", g);

            TemplateGroupFile group = new TemplateGroupFile(Path.Combine(dir, "g.stg"));
            ErrorBuffer errors = new ErrorBuffer();
            group.Listener = errors;
            group.Load();
            string expected = "g.stg 2:3: the explicit definition of region /a.r hides an embedded definition in the same group" + newline;
            string result = errors.ToString();
            Assert.AreEqual(expected, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestCantDefineEmbeddedRegionAgainInTemplate()
        {
            string dir = tmpdir;
            string g =
                "a() ::= <<\n" +
                "[\n" +
                "<@r>foo<@end>\n" +
                "<@r()>\n" +
                "]\n" +
                ">>\n"; // error; dup
            writeFile(dir, "g.stg", g);

            TemplateGroupFile group = new TemplateGroupFile(Path.Combine(dir, "g.stg"));
            ErrorBuffer errors = new ErrorBuffer();
            group.Listener = errors;
            group.Load();
            Assert.AreEqual(0, errors.Errors.Count);

            Template template = group.GetInstanceOf("a");
            string expected =
                "[" + newline +
                "foo" + newline +
                "foo" + newline +
                "]";
            string result = template.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestMissingRegionName()
        {
            string dir = tmpdir;
            string g = "@t.() ::= \"\"\n";
            writeFile(dir, "g.stg", g);

            TemplateGroupFile group = new TemplateGroupFile(Path.Combine(dir, "g.stg"));
            ErrorBuffer errors = new ErrorBuffer();
            group.Listener = errors;
            group.Load();
            string expected = "g.stg 1:3: missing ID at '('" + newline;
            string result = errors.ToString();
            Assert.AreEqual(expected, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestIndentBeforeRegionIsIgnored()
        {
            string dir = tmpdir;
            string g = "a() ::= <<[\n" +
                       "  <@r>\n" +
                       "  foo\n" +
                       "  <@end>\n" +
                       "]>>\n";
            writeFile(dir, "g.stg", g);

            TemplateGroupFile group = new TemplateGroupFile(Path.Combine(dir, "g.stg"));
            Template st = group.GetInstanceOf("a");
            string expected = "[" + newline +
                              "  foo" + newline +
                              "]";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestRegionOverrideStripsNewlines()
        {
            string dir = tmpdir;
            string g =
                    "a() ::= \"X<@r()>Y\"" +
                    "@a.r() ::= <<\n" +
                    "foo\n" +
                    ">>\n";
            writeFile(dir, "g.stg", g);

            TemplateGroupFile group = new TemplateGroupFile(Path.Combine(dir, "g.stg"));
            string sub = "@a.r() ::= \"A<@super.r()>B\"" + newline;
            writeFile(dir, "sub.stg", sub);
            TemplateGroupFile subGroup = new TemplateGroupFile(Path.Combine(dir, "sub.stg"));
            subGroup.ImportTemplates(group);
            Template st = subGroup.GetInstanceOf("a");
            string result = st.Render();
            string expecting = "XAfooBY";
            Assert.AreEqual(expecting, result);
        }

        //

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestRegionOverrideRefSuperRegion()
        {
            string dir = tmpdir;
            string g =
                    "a() ::= \"X<@r()>Y\"" +
                    "@a.r() ::= \"foo\"" + newline;
            writeFile(dir, "g.stg", g);
            TemplateGroupFile group = new TemplateGroupFile(Path.Combine(dir, "g.stg"));

            string sub =
                    "@a.r() ::= \"A<@super.r()>B\"" + newline;
            writeFile(dir, "sub.stg", sub);
            TemplateGroupFile subGroup = new TemplateGroupFile(dir + "/sub.stg");
            subGroup.ImportTemplates(group);

            Template st = subGroup.GetInstanceOf("a");
            string result = st.Render();
            string expecting = "XAfooBY";
            Assert.AreEqual(expecting, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestRegionOverrideRefSuperRegion2Levels()
        {
            string g =
                    "a() ::= \"X<@r()>Y\"\n" +
                    "@a.r() ::= \"foo\"\n";
            TemplateGroup group = new TemplateGroupString(g);

            string sub =
                    "@a.r() ::= \"<@super.r()>2\"\n";
            TemplateGroup subGroup = new TemplateGroupString(sub);
            subGroup.ImportTemplates(group);

            Template st = subGroup.GetInstanceOf("a");

            string result = st.Render();
            string expecting = "Xfoo2Y";
            Assert.AreEqual(expecting, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestRegionOverrideRefSuperRegion3Levels()
        {
            string dir = tmpdir;
            string g =
                    "a() ::= \"X<@r()>Y\"" +
                    "@a.r() ::= \"foo\"" + newline;
            writeFile(dir, "g.stg", g);
            TemplateGroupFile group = new TemplateGroupFile(Path.Combine(dir, "g.stg"));

            string sub =
                    "@a.r() ::= \"<@super.r()>2\"" + newline;
            writeFile(dir, "sub.stg", sub);
            TemplateGroupFile subGroup = new TemplateGroupFile(dir + "/sub.stg");
            subGroup.ImportTemplates(group);

            string subsub =
                    "@a.r() ::= \"<@super.r()>3\"" + newline;
            writeFile(dir, "subsub.stg", subsub);
            TemplateGroupFile subSubGroup = new TemplateGroupFile(dir + "/subsub.stg");
            subSubGroup.ImportTemplates(subGroup);

            Template st = subSubGroup.GetInstanceOf("a");

            string result = st.Render();
            string expecting = "Xfoo23Y";
            Assert.AreEqual(expecting, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestRegionOverrideRefSuperImplicitRegion()
        {
            string dir = tmpdir;
            string g =
                    "a() ::= \"X<@r>foo<@end>Y\"" + newline;
            writeFile(dir, "g.stg", g);
            TemplateGroupFile group = new TemplateGroupFile(Path.Combine(dir, "g.stg"));

            string sub =
                    "@a.r() ::= \"A<@super.r()>\"" + newline;
            writeFile(dir, "sub.stg", sub);
            TemplateGroupFile subGroup = new TemplateGroupFile(dir + "/sub.stg");
            subGroup.ImportTemplates(group);

            Template st = subGroup.GetInstanceOf("a");
            string result = st.Render();
            string expecting = "XAfooY";
            Assert.AreEqual(expecting, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestUnknownRegionDefError()
        {
            string dir = tmpdir;
            string g =
                    "a() ::= <<\n" +
                    "X<@r()>Y" +
                    ">>\n" +
                    "@a.q() ::= \"foo\"" + newline;
            ITemplateErrorListener errors = new ErrorBuffer();
            writeFile(dir, "g.stg", g);
            TemplateGroupFile group = new TemplateGroupFile(Path.Combine(dir, "g.stg"));
            group.Listener = errors;
            Template st = group.GetInstanceOf("a");
            st.Render();
            string result = errors.ToString();
            string expecting = "g.stg 3:3: template a doesn't have a region called q" + newline;
            Assert.AreEqual(expecting, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestSuperRegionRefMissingOk()
        {
            string dir = tmpdir;
            string g =
                "a() ::= \"X<@r()>Y\"" +
                "@a.r() ::= \"foo\"" + newline;
            writeFile(dir, "g.stg", g);
            TemplateGroupFile group = new TemplateGroupFile(Path.Combine(dir, "g.stg"));

            string sub =
                "@a.r() ::= \"A<@super.q()>B\"" + newline; // allow this; trap at runtime
            ITemplateErrorListener errors = new ErrorBuffer();
            group.Listener = errors;
            writeFile(dir, "sub.stg", sub);
            TemplateGroupFile subGroup = new TemplateGroupFile(dir + "/sub.stg");
            subGroup.ImportTemplates(group);

            Template st = subGroup.GetInstanceOf("a");
            string result = st.Render();
            string expecting = "XABY";
            Assert.AreEqual(expecting, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestEmbeddedRegionOnOneLine()
        {
            string dir = tmpdir;
            string groupFile =
                "a() ::= <<\n" +
                "[\n" +
                "  <@r>bar<@end>\n" +
                "]\n" +
                ">>\n";
            writeFile(dir, "group.stg", groupFile);
            TemplateGroup group = new TemplateGroupFile(Path.Combine(dir, "group.stg"));
            Template st = group.GetInstanceOf("a");
            st.impl.Dump();
            string expected = "[" + newline + "  bar" + newline + "]";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestEmbeddedRegionTagsOnSeparateLines()
        {
            string dir = tmpdir;
            string groupFile =
                "a() ::= <<\n" +
                "[\n" +
                "  <@r>\n" +
                "  bar\n" +
                "  <@end>\n" +
                "]\n" +
                ">>\n";
            writeFile(dir, "group.stg", groupFile);
            TemplateGroup group = new TemplateGroupFile(Path.Combine(dir, "group.stg"));
            Template st = group.GetInstanceOf("a");
            string expected = "[" + newline + "  bar" + newline + "]";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [Ignore]
        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestEmbeddedSubtemplate()
        {
            // fix so we ignore inside {...}
            string dir = tmpdir;
            string groupFile =
                "a() ::= <<\n" +
                "[\n" +
                "  <{\n" +
                "  bar\n" +
                "  }>\n" +
                "]\n" +
                ">>\n";
            writeFile(dir, "group.stg", groupFile);
            TemplateGroup group = new TemplateGroupFile(Path.Combine(dir, "group.stg"));
            Template st = group.GetInstanceOf("a");
            string expected = "[" + newline + "  bar" + newline + "]";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }
    }
}
