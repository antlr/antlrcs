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
    public class TestRegions : StringTemplateTestBase
    {
        [TestMethod]
        public void TestEmbeddedRegion()
        {
            string dir = GetRandomDir();
            string groupFile =
                "a() ::= <<\n" +
                "[<@r>bar<@end>]\n" +
                ">>\n";
            WriteFile(dir, "group.stg", groupFile);
            TemplateGroup group = new TemplateGroupFile(dir + "/group.stg");
            Template st = group.GetInstanceOf("a");
            string expected = "[bar]" + newline;
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestRegion()
        {
            string dir = GetRandomDir();
            string groupFile =
                "a() ::= <<\n" +
                "[<@r()>]\n" +
                ">>\n";
            WriteFile(dir, "group.stg", groupFile);
            TemplateGroup group = new TemplateGroupFile(dir + "/group.stg");
            Template st = group.GetInstanceOf("a");
            string expected = "[]" + newline;
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestDefineRegionInSubgroup()
        {
            string dir = GetRandomDir();
            string g1 = "a() ::= <<[<@r()>]>>\n";
            WriteFile(dir, "g1.stg", g1);
            string g2 = "@a.r() ::= <<foo>>\n";
            WriteFile(dir, "g2.stg", g2);

            TemplateGroup group1 = new TemplateGroupFile(dir + "/g1.stg");
            TemplateGroup group2 = new TemplateGroupFile(dir + "/g2.stg");
            group2.ImportTemplates(group1); // define r in g2
            Template st = group2.GetInstanceOf("a");
            string expected = "[foo]";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestDefineRegionInSubgroupThatRefsSuper()
        {
            string dir = GetRandomDir();
            string g1 = "a() ::= <<[<@r>foo<@end>]>>\n";
            WriteFile(dir, "g1.stg", g1);
            string g2 = "@a.r() ::= <<(<@super.r()>)>>\n";
            WriteFile(dir, "g2.stg", g2);

            TemplateGroup group1 = new TemplateGroupFile(dir + "/g1.stg");
            TemplateGroup group2 = new TemplateGroupFile(dir + "/g2.stg");
            group2.ImportTemplates(group1); // define r in g2
            Template st = group2.GetInstanceOf("a");
            string expected = "[(foo)]";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestDefineRegionInSubgroup2()
        {
            string dir = GetRandomDir();
            string g1 = "a() ::= <<[<@r()>]>>\n";
            WriteFile(dir, "g1.stg", g1);
            string g2 = "@a.r() ::= <<foo>>>\n";
            WriteFile(dir, "g2.stg", g2);

            TemplateGroup group1 = new TemplateGroupFile(dir + "/g1.stg");
            TemplateGroup group2 = new TemplateGroupFile(dir + "/g2.stg");
            group1.ImportTemplates(group2); // opposite of previous; g1 imports g2
            Template st = group1.GetInstanceOf("a");
            string expected = "[]"; // @a.r implicitly defined in g1; can't see g2's
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestDefineRegionInSameGroup()
        {
            string dir = GetRandomDir();
            string g = "a() ::= <<[<@r()>]>>\n" +
                       "@a.r() ::= <<foo>>\n";
            WriteFile(dir, "g.stg", g);

            TemplateGroup group = new TemplateGroupFile(dir + "/g.stg");
            Template st = group.GetInstanceOf("a");
            string expected = "[foo]";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestCantDefineEmbeddedRegionAgain()
        {
            string dir = GetRandomDir();
            string g = "a() ::= <<[<@r>foo<@end>]>>\n" +
                       "@a.r() ::= <<bar>>\n"; // error; dup
            WriteFile(dir, "g.stg", g);

            TemplateGroup group = new TemplateGroupFile(dir + "/g.stg");
            ErrorBuffer errors = new ErrorBuffer();
            ErrorManager.ErrorListener = errors;
            group.Load();
            string expected = "redefinition of template /region__a__r" + newline;
            string result = errors.ToString();
            Assert.AreEqual(expected, result);
        }
    }
}
