namespace AntlrUnitTests.ST4
{
    using System.Collections.Generic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using StringTemplate;
    using Path = System.IO.Path;

    [TestClass]
    public class TestDictionaries : StringTemplateTestBase
    {
        [TestMethod]
        public void TestDict()
        {
            string templates =
                    "typeInit ::= [\"int\":\"0\", \"float\":\"0.0\"] " + newline +
                    "var(type,name) ::= \"<type> <name> = <typeInit.(type)>;\"" + newline
                    ;
            WriteFile(tmpdir, "test.stg", templates);
            TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "test.stg"));
            Template st = group.GetInstanceOf("var");
            st.Add("type", "int");
            st.Add("name", "x");
            string expecting = "int x = 0;";
            string result = st.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestDictValuesAreTemplates()
        {
            string templates =
                    "typeInit ::= [\"int\":{0<w>}, \"float\":{0.0<w>}] " + newline +
                    "var(type,w,name) ::= \"<type> <name> = <typeInit.(type)>;\"" + newline
                    ;
            WriteFile(tmpdir, "test.stg", templates);
            TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "test.stg"));
            Template st = group.GetInstanceOf("var");
            st.Add("w", "L");
            st.Add("type", "int");
            st.Add("name", "x");
            string expecting = "int x = 0L;";
            string result = st.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestDictKeyLookupViaTemplate()
        {
            // Make sure we try rendering stuff to string if not found as regular object
            string templates =
                    "typeInit ::= [\"int\":{0<w>}, \"float\":{0.0<w>}] " + newline +
                    "var(type,w,name) ::= \"<type> <name> = <typeInit.(type)>;\"" + newline
                    ;
            WriteFile(tmpdir, "test.stg", templates);
            TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "test.stg"));
            Template st = group.GetInstanceOf("var");
            st.Add("w", "L");
            st.Add("type", new Template("int"));
            st.Add("name", "x");
            string expecting = "int x = 0L;";
            string result = st.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestDictKeyLookupAsNonToStringableObject()
        {
            // Make sure we try rendering stuff to string if not found as regular object
            string templates =
                    "foo(m,k) ::= \"<m.(k)>\"" + newline
                    ;
            WriteFile(tmpdir, "test.stg", templates);
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

        [TestMethod]
        public void TestDictMissingDefaultValueIsEmpty()
        {
            string templates =
                    "typeInit ::= [\"int\":\"0\", \"float\":\"0.0\"] " + newline +
                    "var(type,w,name) ::= \"<type> <name> = <typeInit.(type)>;\"" + newline
                    ;
            WriteFile(tmpdir, "test.stg", templates);
            TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "test.stg"));
            Template st = group.GetInstanceOf("var");
            st.Add("w", "L");
            st.Add("type", "double"); // double not in typeInit map
            st.Add("name", "x");
            string expecting = "double x = ;"; // weird, but tests default value is key
            string result = st.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestDictHiddenByFormalArg()
        {
            string templates =
                    "typeInit ::= [\"int\":\"0\", \"float\":\"0.0\"] " + newline +
                    "var(typeInit,type,name) ::= \"<type> <name> = <typeInit.(type)>;\"" + newline
                    ;
            WriteFile(tmpdir, "test.stg", templates);
            TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "test.stg"));
            Template st = group.GetInstanceOf("var");
            st.Add("type", "int");
            st.Add("name", "x");
            string expecting = "int x = ;";
            string result = st.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestDictEmptyValueAndAngleBracketStrings()
        {
            string templates =
                    "typeInit ::= [\"int\":\"0\", \"float\":, \"double\":<<0.0L>>] " + newline +
                    "var(type,name) ::= \"<type> <name> = <typeInit.(type)>;\"" + newline
                    ;
            WriteFile(tmpdir, "test.stg", templates);
            TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "test.stg"));
            Template st = group.GetInstanceOf("var");
            st.Add("type", "float");
            st.Add("name", "x");
            string expecting = "float x = ;";
            string result = st.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestDictDefaultValue()
        {
            string templates =
                    "typeInit ::= [\"int\":\"0\", default:\"null\"] " + newline +
                    "var(type,name) ::= \"<type> <name> = <typeInit.(type)>;\"" + newline
                    ;
            WriteFile(tmpdir, "test.stg", templates);
            TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "test.stg"));
            Template st = group.GetInstanceOf("var");
            st.Add("type", "UserRecord");
            st.Add("name", "x");
            string expecting = "UserRecord x = null;";
            string result = st.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestDictEmptyDefaultValue()
        {
            string templates =
                    "typeInit ::= [\"int\":\"0\", default:] " + newline +
                    "var(type,name) ::= \"<type> <name> = <typeInit.(type)>;\"" + newline
                    ;
            WriteFile(tmpdir, "test.stg", templates);
            TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "test.stg"));
            Template st = group.GetInstanceOf("var");
            st.Add("type", "UserRecord");
            st.Add("name", "x");
            string expecting = "UserRecord x = ;";
            string result = st.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestDictDefaultValueIsKey()
        {
            string templates =
                    "typeInit ::= [\"int\":\"0\", default:key] " + newline +
                    "var(type,name) ::= \"<type> <name> = <typeInit.(type)>;\"" + newline
                    ;
            WriteFile(tmpdir, "test.stg", templates);
            TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "test.stg"));
            Template st = group.GetInstanceOf("var");
            st.Add("type", "UserRecord");
            st.Add("name", "x");
            string expecting = "UserRecord x = UserRecord;";
            string result = st.Render();
            Assert.AreEqual(expecting, result);
        }

        /**
         * Test that a map can have only the default entry.
         */
        [TestMethod]
        public void TestDictDefaultStringAsKey()
        {
            string templates =
                    "typeInit ::= [\"default\":\"foo\"] " + newline +
                    "var(type,name) ::= \"<type> <name> = <typeInit.(type)>;\"" + newline
                    ;
            WriteFile(tmpdir, "test.stg", templates);
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
        [TestMethod]
        public void TestDictDefaultIsDefaultString()
        {
            string templates =
                    "map ::= [default: \"default\"] " + newline +
                    "t() ::= << <map.(\"1\")> >>" + newline
                    ;
            WriteFile(tmpdir, "test.stg", templates);
            TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "test.stg"));
            Template st = group.GetInstanceOf("t");
            string expecting = " default ";
            string result = st.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestDictViaEnclosingTemplates()
        {
            string templates =
                    "typeInit ::= [\"int\":\"0\", \"float\":\"0.0\"] " + newline +
                    "intermediate(type,name) ::= \"<var(...)>\"" + newline +
                    "var(type,name) ::= \"<type> <name> = <typeInit.(type)>;\"" + newline
                    ;
            WriteFile(tmpdir, "test.stg", templates);
            TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "test.stg"));
            Template st = group.GetInstanceOf("intermediate");
            st.Add("type", "int");
            st.Add("name", "x");
            string expecting = "int x = 0;";
            string result = st.Render();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestDictViaEnclosingTemplates2()
        {
            string templates =
                    "typeInit ::= [\"int\":\"0\", \"float\":\"0.0\"] " + newline +
                    "intermediate(stuff) ::= \"<stuff>\"" + newline +
                    "var(type,name) ::= \"<type> <name> = <typeInit.(type)>;\"" + newline
                    ;
            WriteFile(tmpdir, "test.stg", templates);
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
    }
}
