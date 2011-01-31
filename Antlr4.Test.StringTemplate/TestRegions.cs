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
    using Antlr4.StringTemplate.Misc;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Path = System.IO.Path;

    [TestClass]
    public class TestRegions : BaseTest
    {
        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
        public void TestDefineRegionInSubgroup()
        {
            string dir = tmpdir;
            string g1 = "a() ::= <<[<@r()>]>>\n";
            writeFile(dir, "g1.stg", g1);
            string g2 = "@a.r() ::= <<foo>>\n";
            writeFile(dir, "g2.stg", g2);

            TemplateGroup group1 = new TemplateGroupFile(Path.Combine(dir, "g1.stg"));
            TemplateGroup group2 = new TemplateGroupFile(Path.Combine(dir, "g2.stg"));
            group2.ImportTemplates(group1); // define r in g2
            Template st = group2.GetInstanceOf("a");
            string expected = "[foo]";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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
            string expected = "g.stg 2:3: region a.r is embedded and thus already implicitly defined" + newline;
            string result = errors.ToString();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
        public void TestRegionOverrideRefSuperRegion3Levels()
        {
            string dir = tmpdir;
            // Bug: This was causing infinite recursion:
            // GetInstanceOf(super::a)
            // GetInstanceOf(sub::a)
            // GetInstanceOf(subsub::a)
            // GetInstanceOf(subsub::region__a__r)
            // GetInstanceOf(subsub::super.region__a__r)
            // GetInstanceOf(subsub::super.region__a__r)
            // GetInstanceOf(subsub::super.region__a__r)
            // ...
            // Somehow, the ref to super in subsub is not moving up the chain
            // to the @super.r(); oh, i introduced a bug when i put setGroup
            // into STG.GetInstanceOf()!

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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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
    }
}
