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

    [TestClass]
    public class TestDollarDelimiters : BaseTest
    {
        [TestMethod]
        public void TestAttr()
        {
            string template = "hi $name$!";
            Template st = new Template(template, '$', '$');
            st.Add("name", "Ter");
            string expected = "hi Ter!";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestParallelMap()
        {
            TemplateGroup group = new TemplateGroup('$', '$');
            group.DefineTemplate("test", "names,phones", "hi $names,phones:{n,p | $n$:$p$;}$");
            Template st = group.GetInstanceOf("test");
            st.Add("names", "Ter");
            st.Add("names", "Tom");
            st.Add("names", "Sumana");
            st.Add("phones", "x5001");
            st.Add("phones", "x5002");
            st.Add("phones", "x5003");
            string expected =
                "hi Ter:x5001;Tom:x5002;Sumana:x5003;";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestRefToAnotherTemplateInSameGroup()
        {
            string dir = tmpdir;
            string a = "a() ::= << $b()$ >>\n";
            string b = "b() ::= <<bar>>\n";
            writeFile(dir, "a.st", a);
            writeFile(dir, "b.st", b);
            TemplateGroup group = new TemplateGroupDirectory(dir, '$', '$');
            Template st = group.GetInstanceOf("a");
            string expected = " bar ";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestDefaultArgument()
        {
            string templates =
                    "method(name) ::= <<" + newline +
                    "$stat(name)$" + newline +
                    ">>" + newline +
                    "stat(name,value=\"99\") ::= \"x=$value$; // $name$\"" + newline
                    ;
            writeFile(tmpdir, "group.stg", templates);
            TemplateGroup group = new TemplateGroupFile(tmpdir + "/group.stg", '$', '$');
            Template b = group.GetInstanceOf("method");
            b.Add("name", "foo");
            string expecting = "x=99; // foo";
            string result = b.Render();
            Assert.AreEqual(expecting, result);
        }
    }
}
