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
    using StringWriter = System.IO.StringWriter;
    using Antlr4.StringTemplate.Misc;
    using ArgumentException = System.ArgumentException;
    using System.Collections.Generic;

    [TestClass]
    public class TestCoreBasics : BaseTest
    {
        [TestMethod]
        public void TestNullAttr()
        {
            string template = "hi <name>!";
            ST st = new ST(template);
            string expected =
                "hi !";
            string result = st.render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestAttr()
        {
            string template = "hi <name>!";
            ST st = new ST(template);
            st.add("name", "Ter");
            string expected = "hi Ter!";
            string result = st.render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestSetUnknownAttr()
        {
            string templates =
                "t() ::= <<hi <name>!>>\n";
            ErrorBuffer errors = new ErrorBuffer();
            writeFile(tmpdir, "t.stg", templates);
            STGroup group = new STGroupFile(tmpdir + "/" + "t.stg");
            group.setListener(errors);
            ST st = group.getInstanceOf("t");
            string result = null;
            try
            {
                st.add("name", "Ter");
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
            ST st = new ST(template);
            st.add("name", "Ter");
            st.add("name", "Tom");
            string expected =
                "hi TerTom!";
            string result = st.render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestAttrIsList()
        {
            string template = "hi <name>!";
            ST st = new ST(template);
            List<string> names = new List<string>() { "Ter", "Tom" };
            st.add("name", names);
            st.add("name", "Sumana"); // shouldn't alter my version of names list!
            string expected =
                "hi TerTomSumana!";  // ST sees 3 names
            string result = st.render();
            Assert.AreEqual(expected, result);

            Assert.IsTrue(names.Count == 2); // my names list is still just 2
        }

        [TestMethod]
        public void TestAttrIsArray()
        {
            string template = "hi <name>!";
            ST st = new ST(template);
            string[] names = new string[] { "Ter", "Tom" };
            st.add("name", names);
            st.add("name", "Sumana"); // shouldn't alter my version of names list!
            string expected =
                "hi TerTomSumana!";  // ST sees 3 names
            string result = st.render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestProp()
        {
            string template = "<u.id>: <u.name>"; // checks field and method getter
            ST st = new ST(template);
            st.add("u", new User(1, "parrt"));
            string expected = "1: parrt";
            string result = st.render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestPropWithNoAttr()
        {
            string template = "<foo.a>: <ick>"; // checks field and method getter
            ST st = new ST(template);
            st.add("foo", new Dictionary<string, string>() { { "a", "b" } });
            string expected = "b: ";
            string result = st.render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestSTProp()
        {
            string template = "<t.x>"; // get x attr of template t
            ST st = new ST(template);
            ST t = new ST("<x>");
            t.add("x", "Ter");
            st.add("t", t);
            string expected = "Ter";
            string result = st.render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestBooleanISProp()
        {
            string template = "<t.isManager>"; // call isManager
            ST st = new ST(template);
            st.add("t", new User(32, "Ter"));
            string expected = true.ToString();
            string result = st.render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestBooleanHASProp()
        {
            string template = "<t.hasParkingSpot>"; // call hasParkingSpot
            ST st = new ST(template);
            st.add("t", new User(32, "Ter"));
            string expected = true.ToString();
            string result = st.render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestNullAttrProp()
        {
            string template = "<u.id>: <u.name>";
            ST st = new ST(template);
            string expected = ": ";
            string result = st.render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestNoSuchProp()
        {
            ErrorBufferAllErrors errors = new ErrorBufferAllErrors();
            string template = "<u.qqq>";
            STGroup group = new STGroup();
            group.setListener(errors);
            ST st = new ST(group, template);
            st.add("u", new User(1, "parrt"));
            string expected = "";
            string result = st.render();
            Assert.AreEqual(expected, result);
            STRuntimeMessage msg = (STRuntimeMessage)errors.Errors[0];
            STNoSuchPropertyException e = (STNoSuchPropertyException)msg.Cause;
            Assert.AreEqual("Antlr4.Test.StringTemplate.BaseTest+User.qqq", e.PropertyName);
        }

        [TestMethod]
        public void TestNullIndirectProp()
        {
            ErrorBufferAllErrors errors = new ErrorBufferAllErrors();
            STGroup group = new STGroup();
            group.setListener(errors);
            string template = "<u.(qqq)>";
            ST st = new ST(group, template);
            st.add("u", new User(1, "parrt"));
            st.add("qqq", null);
            string expected = "";
            string result = st.render();
            Assert.AreEqual(expected, result);
            STRuntimeMessage msg = (STRuntimeMessage)errors.Errors[0];
            STNoSuchPropertyException e = (STNoSuchPropertyException)msg.Cause;
            Assert.AreEqual("Antlr4.Test.StringTemplate.BaseTest+User.null", e.PropertyName);
        }

        [TestMethod]
        public void TestPropConvertsToString()
        {
            ErrorBufferAllErrors errors = new ErrorBufferAllErrors();
            STGroup group = new STGroup();
            group.setListener(errors);
            string template = "<u.(name)>";
            ST st = new ST(group, template);
            st.add("u", new User(1, "parrt"));
            st.add("name", 100);
            string expected = "";
            string result = st.render();
            Assert.AreEqual(expected, result);
            STRuntimeMessage msg = (STRuntimeMessage)errors.Errors[0];
            STNoSuchPropertyException e = (STNoSuchPropertyException)msg.Cause;
            Assert.AreEqual("Antlr4.Test.StringTemplate.BaseTest+User.100", e.PropertyName);
        }

        [TestMethod]
        public void TestInclude()
        {
            string template = "load <box()>;";
            ST st = new ST(template);
            st.impl.nativeGroup.defineTemplate("box", "kewl\ndaddy");
            string expected =
                "load kewl" + newline +
                "daddy;";
            string result = st.render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestIncludeWithArg()
        {
            string template = "load <box(\"arg\")>;";
            ST st = new ST(template);
            st.impl.nativeGroup.defineTemplate("box", "x", "kewl <x> daddy");
            st.impl.dump();
            st.add("name", "Ter");
            string expected = "load kewl arg daddy;";
            string result = st.render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestIncludeWithArg2()
        {
            string template = "load <box(\"arg\", foo())>;";
            ST st = new ST(template);
            st.impl.nativeGroup.defineTemplate("box", "x,y", "kewl <x> <y> daddy");
            st.impl.nativeGroup.defineTemplate("foo", "blech");
            st.add("name", "Ter");
            string expected = "load kewl arg blech daddy;";
            string result = st.render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestIncludeWithNestedArgs()
        {
            string template = "load <box(foo(\"arg\"))>;";
            ST st = new ST(template);
            st.impl.nativeGroup.defineTemplate("box", "y", "kewl <y> daddy");
            st.impl.nativeGroup.defineTemplate("foo", "x", "blech <x>");
            st.add("name", "Ter");
            string expected = "load kewl blech arg daddy;";
            string result = st.render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestDefineTemplate()
        {
            STGroup group = new STGroup();
            group.defineTemplate("inc", "x", "<x>+1");
            group.defineTemplate("test", "name", "hi <name>!");
            ST st = group.getInstanceOf("test");
            st.add("name", "Ter");
            st.add("name", "Tom");
            st.add("name", "Sumana");
            string expected =
                "hi TerTomSumana!";
            string result = st.render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestMap()
        {
            STGroup group = new STGroup();
            group.defineTemplate("inc", "x", "[<x>]");
            group.defineTemplate("test", "name", "hi <name:inc()>!");
            ST st = group.getInstanceOf("test");
            st.add("name", "Ter");
            st.add("name", "Tom");
            st.add("name", "Sumana");
            string expected =
                "hi [Ter][Tom][Sumana]!";
            string result = st.render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestIndirectMap()
        {
            STGroup group = new STGroup();
            group.defineTemplate("inc", "x", "[<x>]");
            group.defineTemplate("test", "t,name", "<name:(t)()>!");
            ST st = group.getInstanceOf("test");
            st.add("t", "inc");
            st.add("name", "Ter");
            st.add("name", "Tom");
            st.add("name", "Sumana");
            string expected =
                "[Ter][Tom][Sumana]!";
            string result = st.render();
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
            STGroup group = new STGroupFile(tmpdir + "/" + "t.stg");
            ST st = group.getInstanceOf("test");
            st.add("name", "Ter");
            st.add("name", "Tom");
            st.add("name", "Sumana");
            string expected = "*Ter**Tom**Sumana*";
            string result = st.render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestParallelMap()
        {
            STGroup group = new STGroup();
            group.defineTemplate("test", "names,phones", "hi <names,phones:{n,p | <n>:<p>;}>");
            ST st = group.getInstanceOf("test");
            st.add("names", "Ter");
            st.add("names", "Tom");
            st.add("names", "Sumana");
            st.add("phones", "x5001");
            st.add("phones", "x5002");
            st.add("phones", "x5003");
            string expected =
                "hi Ter:x5001;Tom:x5002;Sumana:x5003;";
            string result = st.render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestParallelMapWith3Versus2Elements()
        {
            STGroup group = new STGroup();
            group.defineTemplate("test", "names,phones", "hi <names,phones:{n,p | <n>:<p>;}>");
            ST st = group.getInstanceOf("test");
            st.add("names", "Ter");
            st.add("names", "Tom");
            st.add("names", "Sumana");
            st.add("phones", "x5001");
            st.add("phones", "x5002");
            string expected =
                "hi Ter:x5001;Tom:x5002;Sumana:;";
            string result = st.render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestParallelMapThenMap()
        {
            STGroup group = new STGroup();
            group.defineTemplate("bold", "x", "[<x>]");
            group.defineTemplate("test", "names,phones",
                                 "hi <names,phones:{n,p | <n>:<p>;}:bold()>");
            ST st = group.getInstanceOf("test");
            st.add("names", "Ter");
            st.add("names", "Tom");
            st.add("names", "Sumana");
            st.add("phones", "x5001");
            st.add("phones", "x5002");
            string expected =
                "hi [Ter:x5001;][Tom:x5002;][Sumana:;]";
            string result = st.render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestMapThenParallelMap()
        {
            STGroup group = new STGroup();
            group.defineTemplate("bold", "x", "[<x>]");
            group.defineTemplate("test", "names,phones",
                                 "hi <[names:bold()],phones:{n,p | <n>:<p>;}>");
            ST st = group.getInstanceOf("test");
            st.add("names", "Ter");
            st.add("names", "Tom");
            st.add("names", "Sumana");
            st.add("phones", "x5001");
            st.add("phones", "x5002");
            string expected =
                "hi [Ter]:x5001;[Tom]:x5002;[Sumana]:;";
            string result = st.render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestMapIndexes()
        {
            STGroup group = new STGroup();
            group.defineTemplate("inc", "x,i", "<i>:<x>");
            group.defineTemplate("test", "name", "<name:{n|<inc(n,i)>}; separator=\", \">");
            ST st = group.getInstanceOf("test");
            st.add("name", "Ter");
            st.add("name", "Tom");
            st.add("name", null); // don't count this one
            st.add("name", "Sumana");
            string expected =
                "1:Ter, 2:Tom, 3:Sumana";
            string result = st.render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestMapIndexes2()
        {
            STGroup group = new STGroup();
            group.defineTemplate("test", "name", "<name:{n | <i>:<n>}; separator=\", \">");
            ST st = group.getInstanceOf("test");
            st.add("name", "Ter");
            st.add("name", "Tom");
            st.add("name", null); // don't count this one. still can't apply subtemplate to null value
            st.add("name", "Sumana");
            string expected =
                "1:Ter, 2:Tom, 3:Sumana";
            string result = st.render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestMapSingleValue()
        {
            STGroup group = new STGroup();
            group.defineTemplate("a", "x", "[<x>]");
            group.defineTemplate("test", "name", "hi <name:a()>!");
            ST st = group.getInstanceOf("test");
            st.add("name", "Ter");
            string expected = "hi [Ter]!";
            string result = st.render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestMapNullValue()
        {
            STGroup group = new STGroup();
            group.defineTemplate("a", "x", "[<x>]");
            group.defineTemplate("test", "name", "hi <name:a()>!");
            ST st = group.getInstanceOf("test");
            string expected = "hi !";
            string result = st.render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestMapNullValueInList()
        {
            STGroup group = new STGroup();
            group.defineTemplate("test", "name", "<name; separator=\", \">");
            ST st = group.getInstanceOf("test");
            st.add("name", "Ter");
            st.add("name", "Tom");
            st.add("name", null); // don't print this one
            st.add("name", "Sumana");
            string expected =
                "Ter, Tom, Sumana";
            string result = st.render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestRepeatedMap()
        {
            STGroup group = new STGroup();
            group.defineTemplate("a", "x", "[<x>]");
            group.defineTemplate("b", "x", "(<x>)");
            group.defineTemplate("test", "name", "hi <name:a():b()>!");
            ST st = group.getInstanceOf("test");
            st.add("name", "Ter");
            st.add("name", "Tom");
            st.add("name", "Sumana");
            string expected =
                "hi ([Ter])([Tom])([Sumana])!";
            string result = st.render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestRoundRobinMap()
        {
            STGroup group = new STGroup();
            group.defineTemplate("a", "x", "[<x>]");
            group.defineTemplate("b", "x", "(<x>)");
            group.defineTemplate("test", "name", "hi <name:a(),b()>!");
            ST st = group.getInstanceOf("test");
            st.add("name", "Ter");
            st.add("name", "Tom");
            st.add("name", "Sumana");
            string expected =
                "hi [Ter](Tom)[Sumana]!";
            string result = st.render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestTrueCond()
        {
            string template = "<if(name)>works<endif>";
            ST st = new ST(template);
            st.add("name", "Ter");
            string expected = "works";
            string result = st.render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestEmptyIFTemplate()
        {
            string template = "<if(x)>fail<elseif(name)><endif>";
            ST st = new ST(template);
            st.add("name", "Ter");
            string expected = "";
            string result = st.render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestCondParens()
        {
            string template = "<if(!(x||y)&&!z)>works<endif>";
            ST st = new ST(template);
            string expected = "works";
            string result = st.render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestFalseCond()
        {
            string template = "<if(name)>works<endif>";
            ST st = new ST(template);
            string expected = "";
            string result = st.render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestFalseCond2()
        {
            string template = "<if(name)>works<endif>";
            ST st = new ST(template);
            st.add("name", null);
            string expected = "";
            string result = st.render();
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
            STGroupFile group = new STGroupFile(dir + "/group.stg");
            ST st = group.getInstanceOf("a");
            st.impl.dump();
            string expected = "foo" + newline +
                              "bar";
            string result = st.render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestElseIf2()
        {
            string template =
                "<if(x)>fail1<elseif(y)>fail2<elseif(z)>works<else>fail3<endif>";
            ST st = new ST(template);
            st.add("z", "blort");
            string expected = "works";
            string result = st.render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestElseIf3()
        {
            string template =
                "<if(x)><elseif(y)><elseif(z)>works<else><endif>";
            ST st = new ST(template);
            st.add("z", "blort");
            string expected = "works";
            string result = st.render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestNotTrueCond()
        {
            string template = "<if(!name)>works<endif>";
            ST st = new ST(template);
            st.add("name", "Ter");
            string expected = "";
            string result = st.render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestNotFalseCond()
        {
            string template = "<if(!name)>works<endif>";
            ST st = new ST(template);
            string expected = "works";
            string result = st.render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestTrueCondWithElse()
        {
            string template = "<if(name)>works<else>fail<endif>";
            ST st = new ST(template);
            st.add("name", "Ter");
            string expected = "works";
            string result = st.render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestFalseCondWithElse()
        {
            string template = "<if(name)>fail<else>works<endif>";
            ST st = new ST(template);
            string expected = "works";
            string result = st.render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestElseIf()
        {
            string template = "<if(name)>fail<elseif(id)>works<else>fail<endif>";
            ST st = new ST(template);
            st.add("id", "2DF3DF");
            string expected = "works";
            string result = st.render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestElseIfNoElseAllFalse()
        {
            string template = "<if(name)>fail<elseif(id)>fail<endif>";
            ST st = new ST(template);
            string expected = "";
            string result = st.render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestElseIfAllExprFalse()
        {
            string template = "<if(name)>fail<elseif(id)>fail<else>works<endif>";
            ST st = new ST(template);
            string expected = "works";
            string result = st.render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestOr()
        {
            string template = "<if(name||notThere)>works<else>fail<endif>";
            ST st = new ST(template);
            st.add("name", "Ter");
            string expected = "works";
            string result = st.render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestMapConditionAndEscapeInside()
        {
            string template = "<if(m.name)>works \\\\<endif>";
            ST st = new ST(template);
            IDictionary<string, string> m = new Dictionary<string, string>();
            m["name"] = "Ter";
            st.add("m", m);
            string expected = "works \\";
            string result = st.render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestAnd()
        {
            string template = "<if(name&&notThere)>fail<else>works<endif>";
            ST st = new ST(template);
            st.add("name", "Ter");
            string expected = "works";
            string result = st.render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestAndNot()
        {
            string template = "<if(name&&!notThere)>works<else>fail<endif>";
            ST st = new ST(template);
            st.add("name", "Ter");
            string expected = "works";
            string result = st.render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestCharLiterals()
        {
            ST st = new ST(
                    "Foo <\\n><\\n><\\t> bar\n"
                    );
            StringWriter sw = new StringWriter();
            st.write(new AutoIndentWriter(sw, "\n")); // force \n as newline
            string result = sw.ToString();
            string expecting = "Foo \n\n\t bar\n";     // expect \n in output
            Assert.AreEqual(expecting, result);

            st = new ST(
                    "Foo <\\n><\\t> bar" + newline);
            sw = new StringWriter();
            st.write(new AutoIndentWriter(sw, "\n")); // force \n as newline
            expecting = "Foo \n\t bar\n";     // expect \n in output
            result = sw.ToString();
            Assert.AreEqual(expecting, result);

            st = new ST(
                    "Foo<\\ >bar<\\n>");
            sw = new StringWriter();
            st.write(new AutoIndentWriter(sw, "\n")); // force \n as newline
            result = sw.ToString();
            expecting = "Foo bar\n"; // forced \n
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestUnicodeLiterals()
        {
            ST st = new ST(
                    "Foo <\\uFEA5><\\n><\\u00C2> bar\n"
                    );
            string expecting = "Foo \ufea5" + newline + "\u00C2 bar" + newline;
            string result = st.render();
            Assert.AreEqual(expecting, result);

            st = new ST(
                    "Foo <\\uFEA5><\\n><\\u00C2> bar" + newline);
            expecting = "Foo \ufea5" + newline + "\u00C2 bar" + newline;
            result = st.render();
            Assert.AreEqual(expecting, result);

            st = new ST(
                    "Foo<\\ >bar<\\n>");
            expecting = "Foo bar" + newline;
            result = st.render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestSubtemplateExpr()
        {
            string template = "<{name\n}>";
            ST st = new ST(template);
            string expected =
                "name" + newline;
            string result = st.render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestSeparator()
        {
            STGroup group = new STGroup();
            group.defineTemplate("test", "names", "<names:{n | case <n>}; separator=\", \">");
            ST st = group.getInstanceOf("test");
            st.add("names", "Ter");
            st.add("names", "Tom");
            string expected =
                "case Ter, case Tom";
            string result = st.render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestSeparatorInList()
        {
            STGroup group = new STGroup();
            group.defineTemplate("test", "names", "<names:{n | case <n>}; separator=\", \">");
            ST st = group.getInstanceOf("test");
            st.add("names", new List<string>() { "Ter", "Tom" });
            string expected =
                "case Ter, case Tom";
            string result = st.render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Playing()
        {
            string template = "<a:t(x,y),u()>";
            ST st = new ST(template);
            st.impl.dump();
        }
    }
}
