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
    using Path = System.IO.Path;
    using STGroup = StringTemplate.TemplateGroup;
    using STGroupFile = StringTemplate.TemplateGroupFile;
    using String = System.String;

    [TestClass]
    public class TestGroupSyntax : StringTemplateTestBase
    {
        [TestMethod]
        public void TestSimpleGroup()
        {
            String templates =
                "t() ::= <<foo>>" + newline;

            WriteFile(tmpdir, "t.stg", templates);
            STGroup group = new STGroupFile(Path.Combine(tmpdir, "t.stg"));
            String expected =
                "t() ::= <<" + newline +
                "foo" + newline +
                ">>" + newline;
            String result = group.Show();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestMultiTemplates()
        {
            String templates =
                "ta() ::= \"[<it>]\"" + newline +
                "duh() ::= <<hi there>>" + newline +
                "wow() ::= <<last>>" + newline;

            WriteFile(tmpdir, "t.stg", templates);
            STGroup group = new STGroupFile(Path.Combine(tmpdir, "t.stg"));
            String expected =
                "ta() ::= <<" + newline +
                "[<it>]" + newline +
                ">>" + newline +
                "duh() ::= <<" + newline +
                "hi there" + newline +
                ">>" + newline +
                "wow() ::= <<" + newline +
                "last" + newline +
                ">>" + newline;
            String result = group.Show();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestSingleTemplateWithArgs()
        {
            String templates =
                "t(a,b) ::= \"[<a>]\"" + newline;

            WriteFile(tmpdir, "t.stg", templates);
            STGroup group = new STGroupFile(Path.Combine(tmpdir, "t.stg"));
            String expected =
                "t(a,b) ::= <<" + newline +
                "[<a>]" + newline +
                ">>" + newline;
            String result = group.Show();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestDefaultValues()
        {
            String templates =
                "t(a={def1},b=\"def2\") ::= \"[<a>]\"" + newline;

            WriteFile(tmpdir, "t.stg", templates);
            STGroup group = new STGroupFile(Path.Combine(tmpdir, "t.stg"));
            String expected =
                "t(a={def1},b=\"def2\") ::= <<" + newline +
                "[<a>]" + newline +
                ">>" + newline;
            String result = group.Show();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestDefaultValueTemplateWithArg()
        {
            String templates =
                "t(a={x | 2*<x>}) ::= \"[<a>]\"" + newline;

            WriteFile(tmpdir, "t.stg", templates);
            STGroup group = new STGroupFile(Path.Combine(tmpdir, "t.stg"));
            String expected =
                "t(a={x | 2*<x>}) ::= <<" + newline +
                "[<a>]" + newline +
                ">>" + newline;
            String result = group.Show();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestNestedTemplateInGroupFile()
        {
            String templates =
                "t(a) ::= \"<a:{x | <x:{<it>}>}>\"" + newline;

            WriteFile(tmpdir, "t.stg", templates);
            STGroup group = new STGroupFile(Path.Combine(tmpdir, "t.stg"));
            String expected =
                "t(a) ::= <<\n" +
                "<a:{x | <x:{<it>}>}>\n" +
                ">>" + newline;
            String result = group.Show();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestNestedDefaultValueTemplate()
        {
            String templates =
                "t(a={x | <x:{<it>}>}) ::= \"ick\"" + newline;

            WriteFile(tmpdir, "t.stg", templates);
            STGroup group = new STGroupFile(Path.Combine(tmpdir, "t.stg"));
            String expected =
                "t(a={x | <x:{<it>}>}) ::= <<\n" +
                "ick\n" +
                ">>" + newline;
            String result = group.Show();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestNestedDefaultValueTemplateWithEscapes()
        {
            String templates =
                "t(a={x | \\< <x:{<it>\\}}>}) ::= \"[<a>]\"" + newline;

            WriteFile(tmpdir, "t.stg", templates);
            STGroup group = new STGroupFile(Path.Combine(tmpdir, "t.stg"));
            String expected =
                "t(a={x | \\< <x:{<it>\\}}>}) ::= <<" + newline +
                "[<a>]" + newline +
                ">>" + newline;
            String result = group.Show();
            Assert.AreEqual(expected, result);
        }
    }
}
