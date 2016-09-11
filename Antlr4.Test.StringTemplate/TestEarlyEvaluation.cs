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
    using System.Collections.Generic;
    using Antlr4.StringTemplate;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class TestEarlyEvaluation : BaseTest
    {
        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestEarlyEvalInIfExpr()
        {
            string templates = "main(x) ::= << <if((x))>foo<else>bar<endif> >>";
            writeFile(tmpdir, "t.stg", templates);

            TemplateGroup group = new TemplateGroupFile(tmpdir + "/t.stg");

            Template st = group.GetInstanceOf("main");

            string s = st.Render();
            Assert.AreEqual(" bar ", s);

            st.Add("x", "true");
            s = st.Render();
            Assert.AreEqual(" foo ", s);
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestEarlyEvalOfSubtemplateInIfExpr()
        {
            string templates = "main(x) ::= << <if(({a<x>b}))>foo<else>bar<endif> >>";
            writeFile(tmpdir, "t.stg", templates);

            TemplateGroup group = new TemplateGroupFile(tmpdir + "/t.stg");

            Template st = group.GetInstanceOf("main");

            string s = st.Render();
            Assert.AreEqual(" foo ", s);
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestEarlyEvalOfMapInIfExpr()
        {
            string templates =
                    "m ::= [\n" +
                    "	\"parrt\": \"value\",\n" +
                    "	default: \"other\"\n" +
                    "]\n" +
                    "main(x) ::= << p<x>t: <m.({p<x>t})>, <if(m.({p<x>t}))>if<else>else<endif> >>\n";
            writeFile(tmpdir, "t.stg", templates);

            TemplateGroup group = new TemplateGroupFile(tmpdir + "/t.stg");

            Template st = group.GetInstanceOf("main");

            st.Add("x", null);
            string s = st.Render();
            Assert.AreEqual(" pt: other, if ", s);

            st.Add("x", "arr");
            s = st.Render();
            Assert.AreEqual(" parrt: value, if ", s);
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestEarlyEvalOfMapInIfExprPassInHashMap()
        {
            string templates =
                    "main(m,x) ::= << p<x>t: <m.({p<x>t})>, <if(m.({p<x>t}))>if<else>else<endif> >>\n";
            writeFile(tmpdir, "t.stg", templates);

            TemplateGroup group = new TemplateGroupFile(tmpdir + "/t.stg");

            Template st = group.GetInstanceOf("main");
            st.Add("m", new Dictionary<string, string> { { "parrt", "value" } });

            st.Add("x", null);
            string s = st.Render();
            Assert.AreEqual(" pt: , else ", s); // m[null] has no default value so else clause

            st.Add("x", "arr");
            s = st.Render();
            Assert.AreEqual(" parrt: value, if ", s);
        }
    }
}
