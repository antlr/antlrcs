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
    using Antlr4.StringTemplate.Misc;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using ArgumentException = System.ArgumentException;
    using Path = System.IO.Path;
    using StringWriter = System.IO.StringWriter;

    [TestClass]
    public class TestCoreBasics : BaseTest
    {
        [TestMethod]
        public void TestNullAttr()
        {
            string template = "hi <name>!";
            Template st = new Template(template);
            string expected =
                "hi !";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestAttr()
        {
            string template = "hi <name>!";
            Template st = new Template(template);
            st.Add("name", "Ter");
            string expected = "hi Ter!";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestChainAttr()
        {
            string template = "<x>:<names>!";
            Template st = new Template(template);
            st.Add("names", "Ter").Add("names", "Tom").Add("x", 1);
            string expected = "1:TerTom!";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestSetUnknownAttr()
        {
            string templates =
                "t() ::= <<hi <name>!>>\n";
            ErrorBuffer errors = new ErrorBuffer();
            writeFile(tmpdir, "t.stg", templates);
            TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
            group.Listener = errors;
            Template st = group.GetInstanceOf("t");
            string result = null;
            try
            {
                st.Add("name", "Ter");
            }
            catch (ArgumentException iae)
            {
                result = iae.Message;
            }
            string expected = "no such attribute: name";
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestMultiAttr()
        {
            string template = "hi <name>!";
            Template st = new Template(template);
            st.Add("name", "Ter");
            st.Add("name", "Tom");
            string expected =
                "hi TerTom!";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestAttrIsList()
        {
            string template = "hi <name>!";
            Template st = new Template(template);
            List<string> names = new List<string>() { "Ter", "Tom" };
            st.Add("name", names);
            st.Add("name", "Sumana"); // shouldn't alter my version of names list!
            string expected =
                "hi TerTomSumana!";  // Template sees 3 names
            string result = st.Render();
            Assert.AreEqual(expected, result);

            Assert.IsTrue(names.Count == 2); // my names list is still just 2
        }

        [TestMethod]
        public void TestAttrIsArray()
        {
            string template = "hi <name>!";
            Template st = new Template(template);
            string[] names = new string[] { "Ter", "Tom" };
            st.Add("name", names);
            st.Add("name", "Sumana"); // shouldn't alter my version of names list!
            string expected =
                "hi TerTomSumana!";  // Template sees 3 names
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestProp()
        {
            string template = "<u.id>: <u.name>"; // checks field and method getter
            Template st = new Template(template);
            st.Add("u", new User(1, "parrt"));
            string expected = "1: parrt";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestPropWithNoAttr()
        {
            string template = "<foo.a>: <ick>"; // checks field and method getter
            Template st = new Template(template);
            st.Add("foo", new Dictionary<string, string>() { { "a", "b" } });
            string expected = "b: ";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestMapAcrossDictionaryUsesKeys()
        {
            string template = "<foo:{f | <f>}>"; // checks field and method getter
            Template st = new Template(template);
            st.Add("foo", new SortedDictionary<string, string>() { { "a", "b" }, { "c", "d" } });
            string expected = "ac";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestSTProp()
        {
            string template = "<t.x>"; // get x attr of template t
            Template st = new Template(template);
            Template t = new Template("<x>");
            t.Add("x", "Ter");
            st.Add("t", t);
            string expected = "Ter";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestBooleanISProp()
        {
            string template = "<t.isManager>"; // call isManager
            Template st = new Template(template);
            st.Add("t", new User(32, "Ter"));
            string expected = true.ToString();
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestBooleanHASProp()
        {
            string template = "<t.hasParkingSpot>"; // call hasParkingSpot
            Template st = new Template(template);
            st.Add("t", new User(32, "Ter"));
            string expected = true.ToString();
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestNullAttrProp()
        {
            string template = "<u.id>: <u.name>";
            Template st = new Template(template);
            string expected = ": ";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestNoSuchProp()
        {
            ErrorBufferAllErrors errors = new ErrorBufferAllErrors();
            string template = "<u.qqq>";
            TemplateGroup group = new TemplateGroup();
            group.Listener = errors;
            Template st = new Template(group, template);
            st.Add("u", new User(1, "parrt"));
            string expected = "";
            string result = st.Render();
            Assert.AreEqual(expected, result);
            TemplateRuntimeMessage msg = (TemplateRuntimeMessage)errors.Errors[0];
            TemplateNoSuchPropertyException e = (TemplateNoSuchPropertyException)msg.Cause;
            Assert.AreEqual("Antlr4.Test.StringTemplate.BaseTest+User.qqq", e.PropertyName);
        }

        [TestMethod]
        public void TestNullIndirectProp()
        {
            ErrorBufferAllErrors errors = new ErrorBufferAllErrors();
            TemplateGroup group = new TemplateGroup();
            group.Listener = errors;
            string template = "<u.(qqq)>";
            Template st = new Template(group, template);
            st.Add("u", new User(1, "parrt"));
            st.Add("qqq", null);
            string expected = "";
            string result = st.Render();
            Assert.AreEqual(expected, result);
            TemplateRuntimeMessage msg = (TemplateRuntimeMessage)errors.Errors[0];
            TemplateNoSuchPropertyException e = (TemplateNoSuchPropertyException)msg.Cause;
            Assert.AreEqual("Antlr4.Test.StringTemplate.BaseTest+User.null", e.PropertyName);
        }

        [TestMethod]
        public void TestPropConvertsToString()
        {
            ErrorBufferAllErrors errors = new ErrorBufferAllErrors();
            TemplateGroup group = new TemplateGroup();
            group.Listener = errors;
            string template = "<u.(name)>";
            Template st = new Template(group, template);
            st.Add("u", new User(1, "parrt"));
            st.Add("name", 100);
            string expected = "";
            string result = st.Render();
            Assert.AreEqual(expected, result);
            TemplateRuntimeMessage msg = (TemplateRuntimeMessage)errors.Errors[0];
            TemplateNoSuchPropertyException e = (TemplateNoSuchPropertyException)msg.Cause;
            Assert.AreEqual("Antlr4.Test.StringTemplate.BaseTest+User.100", e.PropertyName);
        }

        [TestMethod]
        public void TestInclude()
        {
            string template = "Load <box()>;";
            Template st = new Template(template);
            st.impl.NativeGroup.DefineTemplate("box", "kewl\ndaddy");
            string expected =
                "Load kewl" + newline +
                "daddy;";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestIncludeWithArg()
        {
            string template = "Load <box(\"arg\")>;";
            Template st = new Template(template);
            st.impl.NativeGroup.DefineTemplate("box", "kewl <x> daddy", new string[] { "x" });
            st.impl.Dump();
            st.Add("name", "Ter");
            string expected = "Load kewl arg daddy;";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestIncludeWithArg2()
        {
            string template = "Load <box(\"arg\", foo())>;";
            Template st = new Template(template);
            st.impl.NativeGroup.DefineTemplate("box", "kewl <x> <y> daddy", new string[] { "x", "y" });
            st.impl.NativeGroup.DefineTemplate("foo", "blech");
            st.Add("name", "Ter");
            string expected = "Load kewl arg blech daddy;";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestIncludeWithNestedArgs()
        {
            string template = "Load <box(foo(\"arg\"))>;";
            Template st = new Template(template);
            st.impl.NativeGroup.DefineTemplate("box", "kewl <y> daddy", new string[] { "y" });
            st.impl.NativeGroup.DefineTemplate("foo", "blech <x>", new string[] { "x" });
            st.Add("name", "Ter");
            string expected = "Load kewl blech arg daddy;";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestDefineTemplate()
        {
            TemplateGroup group = new TemplateGroup();
            group.DefineTemplate("inc", "<x>+1", new string[] { "x" });
            group.DefineTemplate("test", "hi <name>!", new string[] { "name" });
            Template st = group.GetInstanceOf("test");
            st.Add("name", "Ter");
            st.Add("name", "Tom");
            st.Add("name", "Sumana");
            string expected =
                "hi TerTomSumana!";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestMap()
        {
            TemplateGroup group = new TemplateGroup();
            group.DefineTemplate("inc", "[<x>]", new string[] { "x" });
            group.DefineTemplate("test", "hi <name:inc()>!", new string[] { "name" });
            Template st = group.GetInstanceOf("test");
            st.Add("name", "Ter");
            st.Add("name", "Tom");
            st.Add("name", "Sumana");
            string expected =
                "hi [Ter][Tom][Sumana]!";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestIndirectMap()
        {
            TemplateGroup group = new TemplateGroup();
            group.DefineTemplate("inc", "[<x>]", new string[] { "x" });
            group.DefineTemplate("test", "<name:(t)()>!", new string[] { "t", "name" });
            Template st = group.GetInstanceOf("test");
            st.Add("t", "inc");
            st.Add("name", "Ter");
            st.Add("name", "Tom");
            st.Add("name", "Sumana");
            string expected =
                "[Ter][Tom][Sumana]!";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestMapWithExprAsTemplateName()
        {
            string templates =
                "d ::= [\"foo\":\"bold\"]\n" +
                "test(name) ::= \"<name:(d.foo)()>\"\n" +
                "bold(x) ::= <<*<x>*>>\n";
            writeFile(tmpdir, "t.stg", templates);
            TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
            Template st = group.GetInstanceOf("test");
            st.Add("name", "Ter");
            st.Add("name", "Tom");
            st.Add("name", "Sumana");
            string expected = "*Ter**Tom**Sumana*";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestParallelMap()
        {
            TemplateGroup group = new TemplateGroup();
            group.DefineTemplate("test", "hi <names,phones:{n,p | <n>:<p>;}>", new string[] { "names", "phones" });
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
        public void TestParallelMapWith3Versus2Elements()
        {
            TemplateGroup group = new TemplateGroup();
            group.DefineTemplate("test", "hi <names,phones:{n,p | <n>:<p>;}>", new string[] { "names", "phones" });
            Template st = group.GetInstanceOf("test");
            st.Add("names", "Ter");
            st.Add("names", "Tom");
            st.Add("names", "Sumana");
            st.Add("phones", "x5001");
            st.Add("phones", "x5002");
            string expected =
                "hi Ter:x5001;Tom:x5002;Sumana:;";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestParallelMapThenMap()
        {
            TemplateGroup group = new TemplateGroup();
            group.DefineTemplate("bold", "[<x>]", new string[] { "x" });
            group.DefineTemplate("test", "hi <names,phones:{n,p | <n>:<p>;}:bold()>", new string[] { "names", "phones" });
            Template st = group.GetInstanceOf("test");
            st.Add("names", "Ter");
            st.Add("names", "Tom");
            st.Add("names", "Sumana");
            st.Add("phones", "x5001");
            st.Add("phones", "x5002");
            string expected =
                "hi [Ter:x5001;][Tom:x5002;][Sumana:;]";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestMapThenParallelMap()
        {
            TemplateGroup group = new TemplateGroup();
            group.DefineTemplate("bold", "[<x>]", new string[] { "x" });
            group.DefineTemplate("test", "hi <[names:bold()],phones:{n,p | <n>:<p>;}>", new string[] { "names", "phones" });
            Template st = group.GetInstanceOf("test");
            st.Add("names", "Ter");
            st.Add("names", "Tom");
            st.Add("names", "Sumana");
            st.Add("phones", "x5001");
            st.Add("phones", "x5002");
            string expected =
                "hi [Ter]:x5001;[Tom]:x5002;[Sumana]:;";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestMapIndexes()
        {
            TemplateGroup group = new TemplateGroup();
            group.DefineTemplate("inc", "<i>:<x>", new string[] { "x", "i" });
            group.DefineTemplate("test", "<name:{n|<inc(n,i)>}; separator=\", \">", new string[] { "name" });
            Template st = group.GetInstanceOf("test");
            st.Add("name", "Ter");
            st.Add("name", "Tom");
            st.Add("name", null); // don't count this one
            st.Add("name", "Sumana");
            string expected =
                "1:Ter, 2:Tom, 3:Sumana";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestMapIndexes2()
        {
            TemplateGroup group = new TemplateGroup();
            group.DefineTemplate("test", "<name:{n | <i>:<n>}; separator=\", \">", new string[] { "name" });
            Template st = group.GetInstanceOf("test");
            st.Add("name", "Ter");
            st.Add("name", "Tom");
            st.Add("name", null); // don't count this one. still can't apply subtemplate to null value
            st.Add("name", "Sumana");
            string expected =
                "1:Ter, 2:Tom, 3:Sumana";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestMapSingleValue()
        {
            TemplateGroup group = new TemplateGroup();
            group.DefineTemplate("a", "[<x>]", new string[] { "x" });
            group.DefineTemplate("test", "hi <name:a()>!", new string[] { "name" });
            Template st = group.GetInstanceOf("test");
            st.Add("name", "Ter");
            string expected = "hi [Ter]!";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestMapNullValue()
        {
            TemplateGroup group = new TemplateGroup();
            group.DefineTemplate("a", "[<x>]", new string[] { "x" });
            group.DefineTemplate("test", "hi <name:a()>!", new string[] { "name" });
            Template st = group.GetInstanceOf("test");
            string expected = "hi !";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestMapNullValueInList()
        {
            TemplateGroup group = new TemplateGroup();
            group.DefineTemplate("test", "<name; separator=\", \">", new string[] { "name" });
            Template st = group.GetInstanceOf("test");
            st.Add("name", "Ter");
            st.Add("name", "Tom");
            st.Add("name", null); // don't print this one
            st.Add("name", "Sumana");
            string expected =
                "Ter, Tom, Sumana";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestRepeatedMap()
        {
            TemplateGroup group = new TemplateGroup();
            group.DefineTemplate("a", "[<x>]", new string[] { "x" });
            group.DefineTemplate("b", "(<x>)", new string[] { "x" });
            group.DefineTemplate("test", "hi <name:a():b()>!", new string[] { "name" });
            Template st = group.GetInstanceOf("test");
            st.Add("name", "Ter");
            st.Add("name", "Tom");
            st.Add("name", "Sumana");
            string expected =
                "hi ([Ter])([Tom])([Sumana])!";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestRoundRobinMap()
        {
            TemplateGroup group = new TemplateGroup();
            group.DefineTemplate("a", "[<x>]", new string[] { "x" });
            group.DefineTemplate("b", "(<x>)", new string[] { "x" });
            group.DefineTemplate("test", "hi <name:a(),b()>!", new string[] { "name" });
            Template st = group.GetInstanceOf("test");
            st.Add("name", "Ter");
            st.Add("name", "Tom");
            st.Add("name", "Sumana");
            string expected =
                "hi [Ter](Tom)[Sumana]!";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestTrueCond()
        {
            string template = "<if(name)>works<endif>";
            Template st = new Template(template);
            st.Add("name", "Ter");
            string expected = "works";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestEmptyIFTemplate()
        {
            string template = "<if(x)>fail<elseif(name)><endif>";
            Template st = new Template(template);
            st.Add("name", "Ter");
            string expected = "";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestCondParens()
        {
            string template = "<if(!(x||y)&&!z)>works<endif>";
            Template st = new Template(template);
            string expected = "works";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestFalseCond()
        {
            string template = "<if(name)>works<endif>";
            Template st = new Template(template);
            string expected = "";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestFalseCond2()
        {
            string template = "<if(name)>works<endif>";
            Template st = new Template(template);
            st.Add("name", null);
            string expected = "";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestFalseCondWithFormalArgs()
        {
            // insert of indent instr was not working; ok now
            string dir = tmpdir;
            string groupFile =
                "a(scope) ::= <<\n" +
                "foo\n" +
                "    <if(scope)>oops<endif>\n" +
                "bar\n" +
                ">>\n";
            writeFile(dir, "group.stg", groupFile);
            TemplateGroupFile group = new TemplateGroupFile(dir + "/group.stg");
            Template st = group.GetInstanceOf("a");
            st.impl.Dump();
            string expected = "foo" + newline +
                              "bar";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestElseIf2()
        {
            string template =
                "<if(x)>fail1<elseif(y)>fail2<elseif(z)>works<else>fail3<endif>";
            Template st = new Template(template);
            st.Add("z", "blort");
            string expected = "works";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestElseIf3()
        {
            string template =
                "<if(x)><elseif(y)><elseif(z)>works<else><endif>";
            Template st = new Template(template);
            st.Add("z", "blort");
            string expected = "works";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestNotTrueCond()
        {
            string template = "<if(!name)>works<endif>";
            Template st = new Template(template);
            st.Add("name", "Ter");
            string expected = "";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestNotFalseCond()
        {
            string template = "<if(!name)>works<endif>";
            Template st = new Template(template);
            string expected = "works";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestParensInConditonal()
        {
            string template = "<if((a||b)&&(c||d))>works<endif>";
            Template st = new Template(template);
            st.Add("a", true);
            st.Add("b", true);
            st.Add("c", true);
            st.Add("d", true);
            string expected = "works";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestParensInConditonal2()
        {
            string template = "<if((!a||b)&&!(c||d))>broken<else>works<endif>";
            Template st = new Template(template);
            st.Add("a", true);
            st.Add("b", true);
            st.Add("c", true);
            st.Add("d", true);
            string expected = "works";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestTrueCondWithElse()
        {
            string template = "<if(name)>works<else>fail<endif>";
            Template st = new Template(template);
            st.Add("name", "Ter");
            string expected = "works";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestFalseCondWithElse()
        {
            string template = "<if(name)>fail<else>works<endif>";
            Template st = new Template(template);
            string expected = "works";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestElseIf()
        {
            string template = "<if(name)>fail<elseif(id)>works<else>fail<endif>";
            Template st = new Template(template);
            st.Add("id", "2DF3DF");
            string expected = "works";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestElseIfNoElseAllFalse()
        {
            string template = "<if(name)>fail<elseif(id)>fail<endif>";
            Template st = new Template(template);
            string expected = "";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestElseIfAllExprFalse()
        {
            string template = "<if(name)>fail<elseif(id)>fail<else>works<endif>";
            Template st = new Template(template);
            string expected = "works";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestOr()
        {
            string template = "<if(name||notThere)>works<else>fail<endif>";
            Template st = new Template(template);
            st.Add("name", "Ter");
            string expected = "works";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestMapConditionAndEscapeInside()
        {
            string template = "<if(m.name)>works \\\\<endif>";
            Template st = new Template(template);
            IDictionary<string, string> m = new Dictionary<string, string>();
            m["name"] = "Ter";
            st.Add("m", m);
            string expected = "works \\";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestAnd()
        {
            string template = "<if(name&&notThere)>fail<else>works<endif>";
            Template st = new Template(template);
            st.Add("name", "Ter");
            string expected = "works";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestAndNot()
        {
            string template = "<if(name&&!notThere)>works<else>fail<endif>";
            Template st = new Template(template);
            st.Add("name", "Ter");
            string expected = "works";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestCharLiterals()
        {
            Template st = new Template(
                    "Foo <\\n><\\n><\\t> bar\n"
                    );
            StringWriter sw = new StringWriter();
            st.Write(new AutoIndentWriter(sw, "\n")); // force \n as newline
            string result = sw.ToString();
            string expecting = "Foo \n\n\t bar\n";     // expect \n in output
            Assert.AreEqual(expecting, result);

            st = new Template(
                    "Foo <\\n><\\t> bar" + newline);
            sw = new StringWriter();
            st.Write(new AutoIndentWriter(sw, "\n")); // force \n as newline
            expecting = "Foo \n\t bar\n";     // expect \n in output
            result = sw.ToString();
            Assert.AreEqual(expecting, result);

            st = new Template(
                    "Foo<\\ >bar<\\n>");
            sw = new StringWriter();
            st.Write(new AutoIndentWriter(sw, "\n")); // force \n as newline
            result = sw.ToString();
            expecting = "Foo bar\n"; // forced \n
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestUnicodeLiterals()
        {
            Template st = new Template(
                    "Foo <\\uFEA5><\\n><\\u00C2> bar\n"
                    );
            string expecting = "Foo \ufea5" + newline + "\u00C2 bar" + newline;
            string result = st.Render();
            Assert.AreEqual(expecting, result);

            st = new Template(
                    "Foo <\\uFEA5><\\n><\\u00C2> bar" + newline);
            expecting = "Foo \ufea5" + newline + "\u00C2 bar" + newline;
            result = st.Render();
            Assert.AreEqual(expecting, result);

            st = new Template(
                    "Foo<\\ >bar<\\n>");
            expecting = "Foo bar" + newline;
            result = st.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestSubtemplateExpr()
        {
            string template = "<{name\n}>";
            Template st = new Template(template);
            string expected =
                "name" + newline;
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestSeparator()
        {
            TemplateGroup group = new TemplateGroup();
            group.DefineTemplate("test", "<names:{n | case <n>}; separator=\", \">", new string[] { "names" });
            Template st = group.GetInstanceOf("test");
            st.Add("names", "Ter");
            st.Add("names", "Tom");
            string expected =
                "case Ter, case Tom";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestSeparatorInList()
        {
            TemplateGroup group = new TemplateGroup();
            group.DefineTemplate("test", "<names:{n | case <n>}; separator=\", \">", new string[] { "names" });
            Template st = group.GetInstanceOf("test");
            st.Add("names", new List<string>() { "Ter", "Tom" });
            string expected =
                "case Ter, case Tom";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        /** (...) forces early eval to string.
         * We need an STWriter so I must pick one.  toString(...) is used to
         * ensure b is property name in &lt;a.b&gt;.  It's used to eval default args
         * (usually strings). It's use to eval option values (usually strings).
         * So in general no-indent is fine.  Now, if I used indent-writer, it
         * would mostly work too.  What about &lt;(t())&gt; when t() is huge and indented
         * but you had called render() with a no-indent-writer?  now *part* your
         * input is indented!
         */
        [TestMethod]
        public void TestEarlyEvalIndent()
        {
            string templates =
                "t() ::= <<  abc>>\n" +
                "main() ::= <<\n" +
                "<t()>\n" +
                "<(t())>\n" + // early eval ignores indents; mostly for simply strings
                "  <t()>\n" +
                "  <(t())>\n" +
                ">>\n";

            writeFile(tmpdir, "t.stg", templates);
            TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
            Template st = group.GetInstanceOf("main");
            string result = st.Render();
            string expected =
                "  abc" + newline +
                "abc" + newline +
                "    abc" + newline +
                "  abc";
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestArrayOfTemplates()
        {
            string template = "<foo>!";
            Template st = new Template(template);
            Template[] t = new Template[] { new Template("hi"), new Template("mom") };
            st.Add("foo", t);
            string expected = "himom!";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestArrayOfTemplatesInTemplate()
        {
            string template = "<foo>!";
            Template st = new Template(template);
            Template[] t = new Template[] { new Template("hi"), new Template("mom") };
            st.Add("foo", t);
            Template wrapper = new Template("<x>");
            wrapper.Add("x", st);
            string expected = "himom!";
            string result = wrapper.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestListOfTemplates()
        {
            string template = "<foo>!";
            Template st = new Template(template);
            List<Template> t = new List<Template>() { new Template("hi"), new Template("mom") };
            st.Add("foo", t);
            string expected = "himom!";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestListOfTemplatesInTemplate()
        {
            string template = "<foo>!";
            Template st = new Template(template);
            List<Template> t = new List<Template>() { new Template("hi"), new Template("mom") };
            st.Add("foo", t);
            Template wrapper = new Template("<x>");
            wrapper.Add("x", st);
            string expected = "himom!";
            string result = wrapper.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Playing()
        {
            string template = "<a:t(x,y),u()>";
            Template st = new Template(template);
            st.impl.Dump();
        }
    }
}
