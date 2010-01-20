namespace AntlrUnitTests.ST4
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using StringTemplate;
    using ArrayList = System.Collections.ArrayList;
    using StringWriter = System.IO.StringWriter;

    [TestClass]
    public class TestCoreBasics : StringTemplateTestBase
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
            st.code.Dump();
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
            var names = new ArrayList() { "Ter", "Tom" };
            st.Add("name", names);
            st.Add("name", "Sumana"); // shouldn't alter my version of names list!
            string expected =
                "hi TerTomSumana!";  // ST sees 3 names
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
                "hi TerTomSumana!";  // ST sees 3 names
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestProp()
        {
            string template = "<u.id>: <u.name>";
            Template st = new Template(template);
            st.Add("u", new User(1, "parrt"));
            string expected = "1: parrt";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestInclude()
        {
            string template = "load <box()>;";
            Template st = new Template(template);
            st.code.nativeGroup.DefineTemplate(new TemplateName("box"),
                                    "kewl" + newline +
                                    "daddy"
                                    );
            st.Add("name", "Ter");
            string expected =
                "load kewl" + newline +
                "daddy;";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestIncludeWithArg()
        {
            string template = "load <box(x=\"arg\")>;";
            Template st = new Template(template);
            st.code.nativeGroup.DefineTemplate(new TemplateName("box"), "kewl <x> daddy");
            st.Add("name", "Ter");
            string expected = "load kewl arg daddy;";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestIncludeWithSingleUnnamedArg()
        {
            string template = "load <box(\"arg\")>;";
            Template st = new Template(template);
            st.code.nativeGroup.DefineTemplate(new TemplateName("box"), new string[] { "x" }, "kewl <x> daddy");
            st.Add("name", "Ter");
            string expected = "load kewl arg daddy;";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestIncludeWithArg2()
        {
            string template = "load <box(x=\"arg\", y=foo())>;";
            Template st = new Template(template);
            st.code.nativeGroup.DefineTemplate(new TemplateName("box"), "kewl <x> <y> daddy");
            st.code.nativeGroup.DefineTemplate(new TemplateName("foo"), "blech");
            st.Add("name", "Ter");
            string expected = "load kewl arg blech daddy;";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestIncludeWithNestedArgs()
        {
            string template = "load <box(y=foo(x=\"arg\"))>;";
            Template st = new Template(template);
            st.code.nativeGroup.DefineTemplate(new TemplateName("box"), "kewl <y> daddy");
            st.code.nativeGroup.DefineTemplate(new TemplateName("foo"), "blech <x>");
            st.Add("name", "Ter");
            string expected = "load kewl blech arg daddy;";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestDefineTemplate()
        {
            TemplateGroup group = new TemplateGroup();
            group.DefineTemplate(new TemplateName("inc"), "<it>+1");
            group.DefineTemplate(new TemplateName("test"), "hi <name>!");
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
            group.DefineTemplate(new TemplateName("inc"), "[<it>]");
            group.DefineTemplate(new TemplateName("test"), "hi <name:inc>!");
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
        public void TestParallelMap()
        {
            TemplateGroup group = new TemplateGroup();
            group.DefineTemplate(new TemplateName("test"), "hi <names,phones:{n,p | <n>:<p>;}>");
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
            group.DefineTemplate(new TemplateName("test"), "hi <names,phones:{n,p | <n>:<p>;}>");
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
        public void TestMapIndexes()
        {
            TemplateGroup group = new TemplateGroup();
            group.DefineTemplate(new TemplateName("inc"), "<i>:<it>");
            group.DefineTemplate(new TemplateName("test"), "<name:inc; separator=\", \">");
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
        public void TestMapSingleValue()
        {
            TemplateGroup group = new TemplateGroup();
            group.DefineTemplate(new TemplateName("a"), "[<it>]");
            group.DefineTemplate(new TemplateName("test"), "hi <name:a>!");
            Template st = group.GetInstanceOf("test");
            st.Add("name", "Ter");
            string expected = "hi [Ter]!";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestRepeatedMap()
        {
            TemplateGroup group = new TemplateGroup();
            group.DefineTemplate(new TemplateName("a"), "[<it>]");
            group.DefineTemplate(new TemplateName("b"), "(<it>)");
            group.DefineTemplate(new TemplateName("test"), "hi <name:a:b>!");
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
            group.DefineTemplate(new TemplateName("a"), "[<it>]");
            group.DefineTemplate(new TemplateName("b"), "(<it>)");
            group.DefineTemplate(new TemplateName("test"), "hi <name:a,b>!");
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
        public void TestFalseCond()
        {
            string template = "<if(name)>works<endif>";
            Template st = new Template(template);
            string expected = "";
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
        public void TestITDoesntPropagate()
        {
            TemplateGroup group = new TemplateGroup();
            group.DefineTemplate(new TemplateName("foo"), "<it>");   // <it> not visible
            string template = "<names:{<foo()>}>"; // <it> visible only to {...} here
            group.DefineTemplate(new TemplateName("test"), template);
            Template st = group.GetInstanceOf("test");
            st.Add("names", "Ter");
            st.Add("names", "Tom");
            string expected = "";
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
    }
}
