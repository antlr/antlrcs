namespace AntlrUnitTests.ST4
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using StringTemplate;

    [TestClass]
    public class TestCompiler : StringTemplateTestBase
    {
        [TestInitialize]
        public void setUp()
        {
            Compiler.subtemplateCount = 0;
        }

        [TestMethod]
        public void TestAttr()
        {
            string template = "hi <name>";
            CompiledTemplate code = new Compiler().Compile(null, template);
            string asmExpected =
                "load_str 0, " +
                "write, " +
                "load_attr 1, " +
                "write";
            string asmResult = code.Instructions();
            Assert.AreEqual(asmExpected, asmResult);
            string stringsExpected = "[hi , name]";
            string stringsResult = Arrays.toString(code.strings);
            Assert.AreEqual(stringsExpected, stringsResult);
        }

        [TestMethod]
        public void TestInclude()
        {
            string template = "hi <foo()>";
            CompiledTemplate code = new Compiler().Compile(null, template);
            string asmExpected =
                "load_str 0, write, new 1, write";
            string asmResult = code.Instructions();
            Assert.AreEqual(asmExpected, asmResult);
            string stringsExpected = "[hi , /foo]";
            string stringsResult = Arrays.toString(code.strings);
            Assert.AreEqual(stringsExpected, stringsResult);
        }

        [TestMethod]
        public void TestProp()
        {
            string template = "hi <a.b>";
            CompiledTemplate code = new Compiler().Compile(null, template);
            string asmExpected =
                "load_str 0, write, load_attr 1, load_prop 2, write";
            string asmResult = code.Instructions();
            Assert.AreEqual(asmExpected, asmResult);
            string stringsExpected = "[hi , a, b]";
            string stringsResult = Arrays.toString(code.strings);
            Assert.AreEqual(stringsExpected, stringsResult);
        }

        [TestMethod]
        public void TestProp2()
        {
            string template = "<u.id>: <u.name>";
            CompiledTemplate code = new Compiler().Compile(null, template);
            string asmExpected =
                "load_attr 0, load_prop 1, write, load_str 2, write, " +
                "load_attr 0, load_prop 3, write";
            string asmResult = code.Instructions();
            Assert.AreEqual(asmExpected, asmResult);
            string stringsExpected = "[u, id, : , name]";
            string stringsResult = Arrays.toString(code.strings);
            Assert.AreEqual(stringsExpected, stringsResult);
        }

        [TestMethod]
        public void TestMap()
        {
            string template = "hi <name:bold>";
            CompiledTemplate code = new Compiler().Compile(null, template);
            string asmExpected =
                "load_str 0, write, load_attr 1, load_str 2, map, write";
            string asmResult = code.Instructions();
            Assert.AreEqual(asmExpected, asmResult);
            string stringsExpected = "[hi , name, /bold]";
            string stringsResult = Arrays.toString(code.strings);
            Assert.AreEqual(stringsExpected, stringsResult);
        }

        [TestMethod]
        public void TestRepeatedMap()
        {
            string template = "hi <name:bold:italics>";
            CompiledTemplate code = new Compiler().Compile(null, template);
            string asmExpected =
                "load_str 0, " +
                "write, " +
                "load_attr 1, " +
                "load_str 2, " +
                "map, " +
                "load_str 3, " +
                "map, " +
                "write";
            string asmResult = code.Instructions();
            Assert.AreEqual(asmExpected, asmResult);
            string stringsExpected = "[hi , name, /bold, /italics]";
            string stringsResult = Arrays.toString(code.strings);
            Assert.AreEqual(stringsExpected, stringsResult);
        }

        [TestMethod]
        public void TestRotMap()
        {
            string template = "hi <name:bold,italics>";
            CompiledTemplate code = new Compiler().Compile(null, template);
            string asmExpected =
                "load_str 0, write, load_attr 1, load_str 2, load_str 3, rot_map 2, write";
            string asmResult = code.Instructions();
            Assert.AreEqual(asmExpected, asmResult);
            string stringsExpected = "[hi , name, /bold, /italics]";
            string stringsResult = Arrays.toString(code.strings);
            Assert.AreEqual(stringsExpected, stringsResult);
        }

        [TestMethod]
        public void TestAnonMap()
        {
            string template = "hi <name:{n | <n>}>";
            CompiledTemplate code = new Compiler().Compile(null, template);
            string asmExpected =
                "load_str 0, write, load_attr 1, load_str 2, map, write";
            string asmResult = code.Instructions();
            Assert.AreEqual(asmExpected, asmResult);
            string stringsExpected = "[hi , name, /_sub1]";
            string stringsResult = Arrays.toString(code.strings);
            Assert.AreEqual(stringsExpected, stringsResult);
        }

        [TestMethod]
        public void TestIf()
        {
            string template = "go: <if(name)>hi, foo<endif>";
            CompiledTemplate code = new Compiler().Compile(null, template);
            string asmExpected =
                "load_str 0, write, load_attr 1, brf 14, load_str 2, write";
            string asmResult = code.Instructions();
            Assert.AreEqual(asmExpected, asmResult);
            string stringsExpected = "[go: , name, hi, foo]";
            string stringsResult = Arrays.toString(code.strings);
            Assert.AreEqual(stringsExpected, stringsResult);
        }

        [TestMethod]
        public void TestIfElse()
        {
            string template = "go: <if(name)>hi, foo<else>bye<endif>";
            CompiledTemplate code = new Compiler().Compile(null, template);
            string asmExpected =
                "load_str 0, " +
                "write, " +
                "load_attr 1, " +
                "brf 17, " +
                "load_str 2, " +
                "write, " +
                "br 21, " +
                "load_str 3, " +
                "write";
            string asmResult = code.Instructions();
            Assert.AreEqual(asmExpected, asmResult);
            string stringsExpected = "[go: , name, hi, foo, bye]";
            string stringsResult = Arrays.toString(code.strings);
            Assert.AreEqual(stringsExpected, stringsResult);
        }

        [TestMethod]
        public void TestElseIf()
        {
            string template = "go: <if(name)>hi, foo<elseif(user)>a user<endif>";
            CompiledTemplate code = new Compiler().Compile(null, template);
            string asmExpected =
                "load_str 0, " +
                "write, " +
                "load_attr 1, " +
                "brf 17, " +
                "load_str 2, " +
                "write, " +
                "br 27, " +
                "load_attr 3, " +
                "brf 27, " +
                "load_str 4, " +
                "write";
            string asmResult = code.Instructions();
            Assert.AreEqual(asmExpected, asmResult);
            string stringsExpected = "[go: , name, hi, foo, user, a user]";
            string stringsResult = Arrays.toString(code.strings);
            Assert.AreEqual(stringsExpected, stringsResult);
        }

        [TestMethod]
        public void TestElseIfElse()
        {
            string template = "go: <if(name)>hi, foo<elseif(user)>a user<else>bye<endif>";
            CompiledTemplate code = new Compiler().Compile(null, template);
            string asmExpected =
                "load_str 0, " +
                "write, " +
                "load_attr 1, " +
                "brf 17, " +
                "load_str 2, " +
                "write, " +
                "br 34, " +
                "load_attr 3, " +
                "brf 30, " +
                "load_str 4, " +
                "write, " +
                "br 34, " +
                "load_str 5, " +
                "write";
            string asmResult = code.Instructions();
            Assert.AreEqual(asmExpected, asmResult);
            string stringsExpected = "[go: , name, hi, foo, user, a user, bye]";
            string stringsResult = Arrays.toString(code.strings);
            Assert.AreEqual(stringsExpected, stringsResult);
        }

        [TestMethod]
        public void TestOption()
        {
            string template = "hi <name; separator=\"x\">";
            CompiledTemplate code = new Compiler().Compile(null, template);
            string asmExpected =
                "load_str 0, write, load_attr 1, options, load_str 2, store_option 3, write_opt";
            string asmResult = code.Instructions();
            Assert.AreEqual(asmExpected, asmResult);
            string stringsExpected = "[hi , name, x]";
            string stringsResult = Arrays.toString(code.strings);
            Assert.AreEqual(stringsExpected, stringsResult);
        }

        [TestMethod]
        public void TestOptionAsTemplate()
        {
            string template = "hi <name; separator={, }>";
            CompiledTemplate code = new Compiler().Compile(null, template);
            string asmExpected =
                "load_str 0, write, load_attr 1, options, new 2, store_option 3, write_opt";
            string asmResult = code.Instructions();
            Assert.AreEqual(asmExpected, asmResult);
            string stringsExpected = "[hi , name, /_sub1]";
            string stringsResult = Arrays.toString(code.strings);
            Assert.AreEqual(stringsExpected, stringsResult);
        }

        [TestMethod]
        public void TestOptions()
        {
            string template = "hi <name; anchor, wrap=foo(), separator=\", \">";
            CompiledTemplate code = new Compiler().Compile(null, template);
            string asmExpected =
                "load_str 0, write, load_attr 1, options, load_str 2, " +
                "store_option 0, new 3, store_option 4, load_str 4, " +
                "store_option 3, write_opt";
            string stringsExpected = // the ", , ," is the ", " separator string
                "[hi , name, true, /foo, , ]";
            string stringsResult = Arrays.toString(code.strings);
            Assert.AreEqual(stringsExpected, stringsResult);
            string asmResult = code.Instructions();
            Assert.AreEqual(asmExpected, asmResult);
        }

        [TestMethod]
        public void TestEmptyList()
        {
            string template = "<[]>";
            CompiledTemplate code = new Compiler().Compile(null, template);
            string asmExpected = "list, write";
            string asmResult = code.Instructions();
            Assert.AreEqual(asmExpected, asmResult);
            string stringsExpected = "[]";
            string stringsResult = Arrays.toString(code.strings);
            Assert.AreEqual(stringsExpected, stringsResult);
        }

        [TestMethod]
        public void TestList()
        {
            string template = "<[a,b]>";
            CompiledTemplate code = new Compiler().Compile(null, template);
            string asmExpected = "list, load_attr 0, add, load_attr 1, add, write";
            string asmResult = code.Instructions();
            Assert.AreEqual(asmExpected, asmResult);
            string stringsExpected = "[a, b]";
            string stringsResult = Arrays.toString(code.strings);
            Assert.AreEqual(stringsExpected, stringsResult);
        }

        private static class Arrays
        {
            public static string toString(string[] array)
            {
                return "[" + string.Join(", ", array) + "]";
            }
        }
    }
}
