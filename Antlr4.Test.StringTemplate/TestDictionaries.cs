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
    using System.Collections.Generic;
    using Antlr4.StringTemplate.Misc;
    using Antlr4.Test.StringTemplate.Extensions;
    using Path = System.IO.Path;

    [TestClass]
    public class TestDictionaries : BaseTest
    {
        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestDict()
        {
            string templates =
                    "typeInit ::= [\"int\":\"0\", \"float\":\"0.0\"] " + newline +
                    "var(type,name) ::= \"<type> <name> = <typeInit.(type)>;\"" + newline
                    ;
            writeFile(tmpdir, "test.stg", templates);
            TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "test.stg"));
            Template st = group.GetInstanceOf("var");
            st.Add("type", "int");
            st.Add("name", "x");
            string expecting = "int x = 0;";
            string result = st.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestEmptyDictionary()
        {
            string templates =
                "d ::= []\n";
            writeFile(tmpdir, "t.stg", templates);

            TemplateGroupFile group = null;
            ErrorBuffer errors = new ErrorBuffer();
            group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
            group.Listener = errors;
            group.Load(); // force load
            Assert.AreEqual(0, errors.Errors.Count);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestDictValuesAreTemplates()
        {
            string templates =
                    "typeInit ::= [\"int\":{0<w>}, \"float\":{0.0<w>}] " + newline +
                    "var(type,w,name) ::= \"<type> <name> = <typeInit.(type)>;\"" + newline
                    ;
            writeFile(tmpdir, "test.stg", templates);
            TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "test.stg"));
            Template st = group.GetInstanceOf("var");
            st.impl.Dump();
            st.Add("w", "L");
            st.Add("type", "int");
            st.Add("name", "x");
            string expecting = "int x = 0L;";
            string result = st.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestDictKeyLookupViaTemplate()
        {
            // Make sure we try rendering stuff to string if not found as regular object
            string templates =
                    "typeInit ::= [\"int\":{0<w>}, \"float\":{0.0<w>}] " + newline +
                    "var(type,w,name) ::= \"<type> <name> = <typeInit.(type)>;\"" + newline
                    ;
            writeFile(tmpdir, "test.stg", templates);
            TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "test.stg"));
            Template st = group.GetInstanceOf("var");
            st.Add("w", "L");
            st.Add("type", new Template("int"));
            st.Add("name", "x");
            string expecting = "int x = 0L;";
            string result = st.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestDictKeyLookupAsNonToStringableObject()
        {
            // Make sure we try rendering stuff to string if not found as regular object
            string templates =
                    "foo(m,k) ::= \"<m.(k)>\"" + newline
                    ;
            writeFile(tmpdir, "test.stg", templates);
            TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "test.stg"));
            Template st = group.GetInstanceOf("foo");
            IDictionary<HashableUser, string> m = new Dictionary<HashableUser, string>();
            m[new HashableUser(99, "parrt")] = "first";
            m[new HashableUser(172036, "tombu")] = "second";
            m[new HashableUser(391, "sriram")] = "third";
            st.Add("m", m);
            st.Add("k", new HashableUser(172036, "tombu"));
            string expecting = "second";
            string result = st.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestDictMissingDefaultValueIsEmpty()
        {
            string templates =
                    "typeInit ::= [\"int\":\"0\", \"float\":\"0.0\"] " + newline +
                    "var(type,w,name) ::= \"<type> <name> = <typeInit.(type)>;\"" + newline
                    ;
            writeFile(tmpdir, "test.stg", templates);
            TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "test.stg"));
            Template st = group.GetInstanceOf("var");
            st.Add("w", "L");
            st.Add("type", "double"); // double not in typeInit map
            st.Add("name", "x");
            string expecting = "double x = ;";
            string result = st.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestDictMissingDefaultValueIsEmptyForNullKey()
        {
            string templates =
                    "typeInit ::= [\"int\":\"0\", \"float\":\"0.0\"] " + newline +
                    "var(type,w,name) ::= \"<type> <name> = <typeInit.(type)>;\"" + newline
                    ;
            writeFile(tmpdir, "test.stg", templates);
            TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "test.stg"));
            Template st = group.GetInstanceOf("var");
            st.Add("w", "L");
            st.Add("type", null); // double not in typeInit map
            st.Add("name", "x");
            string expecting = " x = ;";
            string result = st.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestDictHiddenByFormalArg()
        {
            string templates =
                    "typeInit ::= [\"int\":\"0\", \"float\":\"0.0\"] " + newline +
                    "var(typeInit,type,name) ::= \"<type> <name> = <typeInit.(type)>;\"" + newline
                    ;
            writeFile(tmpdir, "test.stg", templates);
            TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "test.stg"));
            Template st = group.GetInstanceOf("var");
            st.Add("type", "int");
            st.Add("name", "x");
            string expecting = "int x = ;";
            string result = st.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestDictEmptyValueAndAngleBracketStrings()
        {
            string templates =
                    "typeInit ::= [\"int\":\"0\", \"float\":, \"double\":<<0.0L>>] " + newline +
                    "var(type,name) ::= \"<type> <name> = <typeInit.(type)>;\"" + newline
                    ;
            writeFile(tmpdir, "test.stg", templates);
            TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "test.stg"));
            Template st = group.GetInstanceOf("var");
            st.Add("type", "float");
            st.Add("name", "x");
            string expecting = "float x = ;";
            string result = st.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestDictDefaultValue()
        {
            string templates =
                    "typeInit ::= [\"int\":\"0\", default:\"null\"] " + newline +
                    "var(type,name) ::= \"<type> <name> = <typeInit.(type)>;\"" + newline
                    ;
            writeFile(tmpdir, "test.stg", templates);
            TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "test.stg"));
            Template st = group.GetInstanceOf("var");
            st.Add("type", "UserRecord");
            st.Add("name", "x");
            string expecting = "UserRecord x = null;";
            string result = st.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestDictNullKeyGetsDefaultValue()
        {
            string templates =
                    "typeInit ::= [\"int\":\"0\", default:\"null\"] " + newline +
                    "var(type,name) ::= \"<type> <name> = <typeInit.(type)>;\"" + newline
                    ;
            writeFile(tmpdir, "test.stg", templates);
            TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "test.stg"));
            Template st = group.GetInstanceOf("var");
            // missing or set to null: st.Add("type", null);
            st.Add("name", "x");
            string expecting = " x = null;";
            string result = st.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestDictEmptyDefaultValue()
        {
            string templates =
                    "typeInit ::= [\"int\":\"0\", default:] " + newline +
                    "var(type,name) ::= \"<type> <name> = <typeInit.(type)>;\"" + newline
                    ;
            writeFile(tmpdir, "test.stg", templates);
            ErrorBuffer errors = new ErrorBuffer();
            TemplateGroupFile group = new TemplateGroupFile(Path.Combine(tmpdir, "test.stg"));
            group.Listener = errors;
            group.Load();
            string expected = "[test.stg 1:33: missing value for key at ']']";
            string result = errors.Errors.ToListString();
            Assert.AreEqual(expected, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestDictDefaultValueIsKey()
        {
            string templates =
                    "typeInit ::= [\"int\":\"0\", default:key] " + newline +
                    "var(type,name) ::= \"<type> <name> = <typeInit.(type)>;\"" + newline
                    ;
            writeFile(tmpdir, "test.stg", templates);
            TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "test.stg"));
            Template st = group.GetInstanceOf("var");
            st.Add("type", "UserRecord");
            st.Add("name", "x");
            string expecting = "UserRecord x = UserRecord;";
            string result = st.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestDictWithoutIteration()
        {
            string templates =
                "t2(adr,line2={<adr.zip> <adr.city>}) ::= <<" + newline +
                "<adr.firstname> <adr.lastname>" + newline +
                "<line2>" + newline +
                ">>";

            writeFile(tmpdir, "test.stg", templates);
            TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "test.stg"));
            Template st = group.GetInstanceOf("t2");
            st.Add("adr", new Dictionary<string, string>()
                {
                    {"firstname","Terence"},
                    {"lastname","Parr"},
                    {"zip","99999"},
                    {"city","San Francisco"},
                });
            string expecting =
                "Terence Parr" + newline +
                "99999 San Francisco";
            string result = st.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestDictWithoutIteration2()
        {
            string templates =
                "t2(adr,line2={<adr.zip> <adr.city>}) ::= <<" + newline +
                "<adr.firstname> <adr.lastname>" + newline +
                "<line2>" + newline +
                ">>";

            writeFile(tmpdir, "test.stg", templates);
            TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "test.stg"));
            Template st = group.GetInstanceOf("t2");
            st.Add("adr", new Dictionary<string, string>()
                {
                    {"firstname","Terence"},
                    {"lastname","Parr"},
                    {"zip","99999"},
                    {"city","San Francisco"},
                });
            st.Add("line2", new Template("<adr.city>, <adr.zip>"));
            string expecting =
                "Terence Parr" + newline +
                "San Francisco, 99999";
            string result = st.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestDictWithoutIteration3()
        {
            string templates =
                "t2(adr,line2={<adr.zip> <adr.city>}) ::= <<" + newline +
                "<adr.firstname> <adr.lastname>" + newline +
                "<line2>" + newline +
                ">>" + newline +
                "t3(adr) ::= <<" + newline +
                "<t2(adr=adr,line2={<adr.city>, <adr.zip>})>" + newline +
                ">>" + newline;

            writeFile(tmpdir, "test.stg", templates);
            TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "test.stg"));
            Template st = group.GetInstanceOf("t3");
            st.Add("adr", new Dictionary<string, string>()
                {
                    {"firstname","Terence"},
                    {"lastname","Parr"},
                    {"zip","99999"},
                    {"city","San Francisco"},
                });
            string expecting =
                "Terence Parr" + newline +
                "San Francisco, 99999";
            string result = st.Render();
            Assert.AreEqual(expecting, result);
        }

        /**
         * Test that a map can have only the default entry.
         */
        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestDictDefaultStringAsKey()
        {
            string templates =
                    "typeInit ::= [\"default\":\"foo\"] " + newline +
                    "var(type,name) ::= \"<type> <name> = <typeInit.(type)>;\"" + newline
                    ;
            writeFile(tmpdir, "test.stg", templates);
            TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "test.stg"));
            Template st = group.GetInstanceOf("var");
            st.Add("type", "default");
            st.Add("name", "x");
            string expecting = "default x = foo;";
            string result = st.Render();
            Assert.AreEqual(expecting, result);
        }

        /**
         * Test that a map can return a <b>string</b> with the word: default.
         */
        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestDictDefaultIsDefaultString()
        {
            string templates =
                    "map ::= [default: \"default\"] " + newline +
                    "t() ::= << <map.(\"1\")> >>" + newline
                    ;
            writeFile(tmpdir, "test.stg", templates);
            TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "test.stg"));
            Template st = group.GetInstanceOf("t");
            string expecting = " default ";
            string result = st.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestDictViaEnclosingTemplates()
        {
            string templates =
                    "typeInit ::= [\"int\":\"0\", \"float\":\"0.0\"] " + newline +
                    "intermediate(type,name) ::= \"<var(type,name)>\"" + newline +
                    "var(type,name) ::= \"<type> <name> = <typeInit.(type)>;\"" + newline
                    ;
            writeFile(tmpdir, "test.stg", templates);
            TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "test.stg"));
            Template st = group.GetInstanceOf("intermediate");
            st.Add("type", "int");
            st.Add("name", "x");
            string expecting = "int x = 0;";
            string result = st.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestDictViaEnclosingTemplates2()
        {
            string templates =
                    "typeInit ::= [\"int\":\"0\", \"float\":\"0.0\"] " + newline +
                    "intermediate(stuff) ::= \"<stuff>\"" + newline +
                    "var(type,name) ::= \"<type> <name> = <typeInit.(type)>;\"" + newline
                    ;
            writeFile(tmpdir, "test.stg", templates);
            TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "test.stg"));
            Template interm = group.GetInstanceOf("intermediate");
            Template var = group.GetInstanceOf("var");
            var.Add("type", "int");
            var.Add("name", "x");
            interm.Add("stuff", var);
            string expecting = "int x = 0;";
            string result = interm.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestAccessDictionaryFromAnonymousTemplate()
        {
            string dir = tmpdir;
            string g =
                "a() ::= <<[<[\"foo\",\"a\"]:{x|<if(values.(x))><x><endif>}>]>>\n" +
                "values ::= [\n" +
                "    \"a\":false,\n" +
                "    default:true\n" +
                "]\n";
            writeFile(dir, "g.stg", g);

            TemplateGroup group = new TemplateGroupFile(Path.Combine(dir, "g.stg"));
            Template st = group.GetInstanceOf("a");
            string expected = "[foo]";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestAccessDictionaryFromAnonymousTemplateInRegion()
        {
            string dir = tmpdir;
            string g =
                "a() ::= <<[<@r()>]>>\n" +
                "@a.r() ::= <<\n" +
                "<[\"foo\",\"a\"]:{x|<if(values.(x))><x><endif>}>\n" +
                ">>\n" +
                "values ::= [\n" +
                "    \"a\":false,\n" +
                "    default:true\n" +
                "]\n";
            writeFile(dir, "g.stg", g);

            TemplateGroup group = new TemplateGroupFile(Path.Combine(dir, "g.stg"));
            Template st = group.GetInstanceOf("a");
            string expected = "[foo]";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }
    }
}
