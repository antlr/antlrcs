/*
 * [The "BSD license"]
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
    using Antlr4.StringTemplate.Compiler;
    using Antlr4.StringTemplate.Misc;
    using Antlr4.Test.StringTemplate.Extensions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class TestCompiler : BaseTest
    {
        protected override void setUpImpl()
        {
            TemplateCompiler.subtemplateCount = 0;
            base.setUpImpl();
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestAttr()
        {
            string template = "hi <name>";
            CompiledTemplate code = new TemplateCompiler(new TemplateGroup()).Compile(template);
            string asmExpected =
                "write_str 0, " +
                "load_attr 1, " +
                "write";
            string asmResult = code.GetInstructions();
            Assert.AreEqual(asmExpected, asmResult);
            string stringsExpected = "[hi , name]";
            string stringsResult = code.strings.ToListString();
            Assert.AreEqual(stringsExpected, stringsResult);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestInclude()
        {
            string template = "hi <foo()>";
            CompiledTemplate code = new TemplateCompiler(new TemplateGroup()).Compile(template);
            string asmExpected =
                "write_str 0, new 1 0, write";
            string asmResult = code.GetInstructions();
            Assert.AreEqual(asmExpected, asmResult);
            string stringsExpected = "[hi , foo]";
            string stringsResult = code.strings.ToListString();
            Assert.AreEqual(stringsExpected, stringsResult);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestIncludeWithPassThrough()
        {
            string template = "hi <foo(...)>";
            CompiledTemplate code = new TemplateCompiler(new TemplateGroup()).Compile(template);
            string asmExpected =
                "write_str 0, args, passthru 1, new_box_args 1, write";
            string asmResult = code.GetInstructions();
            Assert.AreEqual(asmExpected, asmResult);
            string stringsExpected = "[hi , foo]";
            string stringsResult = code.strings.ToListString();
            Assert.AreEqual(stringsExpected, stringsResult);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestIncludeWithPartialPassThrough()
        {
            string template = "hi <foo(x=y,...)>";
            CompiledTemplate code = new TemplateCompiler(new TemplateGroup()).Compile(template);
            string asmExpected =
                "write_str 0, args, load_attr 1, store_arg 2, passthru 3, new_box_args 3, write";
            string asmResult = code.GetInstructions();
            Assert.AreEqual(asmExpected, asmResult);
            string stringsExpected = "[hi , y, x, foo]";
            string stringsResult = code.strings.ToListString();
            Assert.AreEqual(stringsExpected, stringsResult);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestSuperInclude()
        {
            string template = "<super.foo()>";
            CompiledTemplate code = new TemplateCompiler(new TemplateGroup()).Compile(template);
            string asmExpected =
                "super_new 0 0, write";
            code.Dump();
            string asmResult = code.GetInstructions();
            Assert.AreEqual(asmExpected, asmResult);
            string stringsExpected = "[foo]";
            string stringsResult = code.strings.ToListString();
            Assert.AreEqual(stringsExpected, stringsResult);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestSuperIncludeWithArgs()
        {
            string template = "<super.foo(a,{b})>";
            CompiledTemplate code = new TemplateCompiler(new TemplateGroup()).Compile(template);
            string asmExpected =
                "load_attr 0, new 1 0, super_new 2 2, write";
            string asmResult = code.GetInstructions();
            Assert.AreEqual(asmExpected, asmResult);
            string stringsExpected = "[a, _sub1, foo]";
            string stringsResult = code.strings.ToListString();
            Assert.AreEqual(stringsExpected, stringsResult);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestSuperIncludeWithNamedArgs()
        {
            string template = "<super.foo(x=a,y={b})>";
            CompiledTemplate code = new TemplateCompiler(new TemplateGroup()).Compile(template);
            string asmExpected =
                "args, load_attr 0, store_arg 1, new 2 0, store_arg 3, super_new_box_args 4, write";
            string asmResult = code.GetInstructions();
            Assert.AreEqual(asmExpected, asmResult);
            string stringsExpected = "[a, x, _sub1, y, foo]";
            string stringsResult = code.strings.ToListString();
            Assert.AreEqual(stringsExpected, stringsResult);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestIncludeWithArgs()
        {
            string template = "hi <foo(a,b)>";
            CompiledTemplate code = new TemplateCompiler(new TemplateGroup()).Compile(template);
            string asmExpected =
                "write_str 0, load_attr 1, load_attr 2, new 3 2, write";
            string asmResult = code.GetInstructions();
            Assert.AreEqual(asmExpected, asmResult);
            string stringsExpected = "[hi , a, b, foo]";
            string stringsResult = code.strings.ToListString();
            Assert.AreEqual(stringsExpected, stringsResult);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestAnonIncludeArgs()
        {
            string template = "<({ a, b | <a><b>})>";
            CompiledTemplate code = new TemplateCompiler(new TemplateGroup()).Compile(template);
            string asmExpected =
                "new 0 0, tostr, write";
            string asmResult = code.GetInstructions();
            Assert.AreEqual(asmExpected, asmResult);
            string stringsExpected = "[_sub1]";
            string stringsResult = code.strings.ToListString();
            Assert.AreEqual(stringsExpected, stringsResult);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestAnonIncludeArgMismatch()
        {
            ITemplateErrorListener errors = new ErrorBuffer();
            string template = "<a:{foo}>";
            CompiledTemplate code = new TemplateCompiler(new TemplateGroup()
            {
                ErrorManager = new ErrorManager(errors)
            }).Compile(template);
            string expected = "1:3: anonymous template has 0 arg(s) but mapped across 1 value(s)" + newline;
            Assert.AreEqual(expected, errors.ToString());
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestAnonIncludeArgMismatch2()
        {
            ITemplateErrorListener errors = new ErrorBuffer();
            string template = "<a,b:{x|foo}>";
            CompiledTemplate code = new TemplateCompiler(new TemplateGroup()
            {
                ErrorManager = new ErrorManager(errors)
            }).Compile(template);
            string expected = "1:5: anonymous template has 1 arg(s) but mapped across 2 value(s)" + newline;
            Assert.AreEqual(expected, errors.ToString());
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestAnonIncludeArgMismatch3()
        {
            ITemplateErrorListener errors = new ErrorBuffer();
            string template = "<a:{x|foo},{bar}>";
            CompiledTemplate code = new TemplateCompiler(new TemplateGroup()
            {
                ErrorManager = new ErrorManager(errors)
            }).Compile(template);
            string expected = "1:11: anonymous template has 0 arg(s) but mapped across 1 value(s)" + newline;
            Assert.AreEqual(expected, errors.ToString());
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestIndirectIncludeWitArgs()
        {
            string template = "hi <(foo)(a,b)>";
            CompiledTemplate code = new TemplateCompiler(new TemplateGroup()).Compile(template);
            string asmExpected =
                "write_str 0, load_attr 1, tostr, load_attr 2, load_attr 3, new_ind 2, write";
            string asmResult = code.GetInstructions();
            Assert.AreEqual(asmExpected, asmResult);
            string stringsExpected = "[hi , foo, a, b]";
            string stringsResult = code.strings.ToListString();
            Assert.AreEqual(stringsExpected, stringsResult);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestProp()
        {
            string template = "hi <a.b>";
            CompiledTemplate code = new TemplateCompiler(new TemplateGroup()).Compile(template);
            string asmExpected =
                "write_str 0, load_attr 1, load_prop 2, write";
            string asmResult = code.GetInstructions();
            Assert.AreEqual(asmExpected, asmResult);
            string stringsExpected = "[hi , a, b]";
            string stringsResult = code.strings.ToListString();
            Assert.AreEqual(stringsExpected, stringsResult);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestProp2()
        {
            string template = "<u.id>: <u.name>";
            CompiledTemplate code = new TemplateCompiler(new TemplateGroup()).Compile(template);
            string asmExpected =
                "load_attr 0, load_prop 1, write, write_str 2, " +
                "load_attr 0, load_prop 3, write";
            string asmResult = code.GetInstructions();
            Assert.AreEqual(asmExpected, asmResult);
            string stringsExpected = "[u, id, : , name]";
            string stringsResult = code.strings.ToListString();
            Assert.AreEqual(stringsExpected, stringsResult);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestMap()
        {
            string template = "<name:bold()>";
            CompiledTemplate code = new TemplateCompiler(new TemplateGroup()).Compile(template);
            string asmExpected =
                "load_attr 0, null, new 1 1, map, write";
            string asmResult = code.GetInstructions();
            Assert.AreEqual(asmExpected, asmResult);
            string stringsExpected = "[name, bold]";
            string stringsResult = code.strings.ToListString();
            Assert.AreEqual(stringsExpected, stringsResult);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestMapAsOption()
        {
            string template = "<a; wrap=name:bold()>";
            CompiledTemplate code = new TemplateCompiler(new TemplateGroup()).Compile(template);
            string asmExpected =
                "load_attr 0, options, load_attr 1, null, new 2 1, map, " +
                "store_option 4, write_opt";
            string asmResult = code.GetInstructions();
            Assert.AreEqual(asmExpected, asmResult);
            string stringsExpected = "[a, name, bold]";
            string stringsResult = code.strings.ToListString();
            Assert.AreEqual(stringsExpected, stringsResult);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestMapArg()
        {
            string template = "<name:bold(x)>";
            CompiledTemplate code = new TemplateCompiler(new TemplateGroup()).Compile(template);
            string asmExpected =
                "load_attr 0, null, load_attr 1, new 2 2, map, write";
            string asmResult = code.GetInstructions();
            Assert.AreEqual(asmExpected, asmResult);
            string stringsExpected = "[name, x, bold]";
            string stringsResult = code.strings.ToListString();
            Assert.AreEqual(stringsExpected, stringsResult);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestIndirectMapArg()
        {
            string template = "<name:(t)(x)>";
            CompiledTemplate code = new TemplateCompiler(new TemplateGroup()).Compile(template);
            string asmExpected =
                "load_attr 0, load_attr 1, tostr, null, load_attr 2, new_ind 2, map, write";
            string asmResult = code.GetInstructions();
            Assert.AreEqual(asmExpected, asmResult);
            string stringsExpected = "[name, t, x]";
            string stringsResult = code.strings.ToListString();
            Assert.AreEqual(stringsExpected, stringsResult);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestRepeatedMap()
        {
            string template = "<name:bold():italics()>";
            CompiledTemplate code = new TemplateCompiler(new TemplateGroup()).Compile(template);
            string asmExpected =
                "load_attr 0, null, new 1 1, map, null, new 2 1, map, write";
            string asmResult = code.GetInstructions();
            Assert.AreEqual(asmExpected, asmResult);
            string stringsExpected = "[name, bold, italics]";
            string stringsResult = code.strings.ToListString();
            Assert.AreEqual(stringsExpected, stringsResult);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestRepeatedMapArg()
        {
            string template = "<name:bold(x):italics(x,y)>";
            CompiledTemplate code = new TemplateCompiler(new TemplateGroup()).Compile(template);
            string asmExpected =
                "load_attr 0, null, load_attr 1, new 2 2, map, " +
                "null, load_attr 1, load_attr 3, new 4 3, map, write";
            string asmResult = code.GetInstructions();
            Assert.AreEqual(asmExpected, asmResult);
            string stringsExpected = "[name, x, bold, y, italics]";
            string stringsResult = code.strings.ToListString();
            Assert.AreEqual(stringsExpected, stringsResult);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestRotMap()
        {
            string template = "<name:bold(),italics()>";
            CompiledTemplate code = new TemplateCompiler(new TemplateGroup()).Compile(template);
            string asmExpected =
                "load_attr 0, null, new 1 1, null, new 2 1, rot_map 2, write";
            string asmResult = code.GetInstructions();
            Assert.AreEqual(asmExpected, asmResult);
            string stringsExpected = "[name, bold, italics]";
            string stringsResult = code.strings.ToListString();
            Assert.AreEqual(stringsExpected, stringsResult);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestRotMapArg()
        {
            string template = "<name:bold(x),italics()>";
            CompiledTemplate code = new TemplateCompiler(new TemplateGroup()).Compile(template);
            string asmExpected =
                "load_attr 0, null, load_attr 1, new 2 2, null, new 3 1, rot_map 2, write";
            string asmResult = code.GetInstructions();
            Assert.AreEqual(asmExpected, asmResult);
            string stringsExpected = "[name, x, bold, italics]";
            string stringsResult = code.strings.ToListString();
            Assert.AreEqual(stringsExpected, stringsResult);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestZipMap()
        {
            string template = "<names,phones:bold()>";
            CompiledTemplate code = new TemplateCompiler(new TemplateGroup()).Compile(template);
            string asmExpected =
                "load_attr 0, load_attr 1, null, null, new 2 2, zip_map 2, write";
            string asmResult = code.GetInstructions();
            Assert.AreEqual(asmExpected, asmResult);
            string stringsExpected = "[names, phones, bold]";
            string stringsResult = code.strings.ToListString();
            Assert.AreEqual(stringsExpected, stringsResult);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestZipMapArg()
        {
            string template = "<names,phones:bold(x)>";
            CompiledTemplate code = new TemplateCompiler(new TemplateGroup()).Compile(template);
            string asmExpected =
                "load_attr 0, load_attr 1, null, null, load_attr 2, new 3 3, zip_map 2, write";
            string asmResult = code.GetInstructions();
            Assert.AreEqual(asmExpected, asmResult);
            string stringsExpected = "[names, phones, x, bold]";
            string stringsResult = code.strings.ToListString();
            Assert.AreEqual(stringsExpected, stringsResult);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestAnonMap()
        {
            string template = "<name:{n | <n>}>";
            CompiledTemplate code = new TemplateCompiler(new TemplateGroup()).Compile(template);
            string asmExpected =
                "load_attr 0, null, new 1 1, map, write";
            string asmResult = code.GetInstructions();
            Assert.AreEqual(asmExpected, asmResult);
            string stringsExpected = "[name, _sub1]";
            string stringsResult = code.strings.ToListString();
            Assert.AreEqual(stringsExpected, stringsResult);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestAnonZipMap()
        {
            string template = "<a,b:{x,y | <x><y>}>";
            CompiledTemplate code = new TemplateCompiler(new TemplateGroup()).Compile(template);
            string asmExpected =
                "load_attr 0, load_attr 1, null, null, new 2 2, zip_map 2, write";
            string asmResult = code.GetInstructions();
            Assert.AreEqual(asmExpected, asmResult);
            string stringsExpected = "[a, b, _sub1]";
            string stringsResult = code.strings.ToListString();
            Assert.AreEqual(stringsExpected, stringsResult);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestIf()
        {
            string template = "go: <if(name)>hi, foo<endif>";
            CompiledTemplate code = new TemplateCompiler(new TemplateGroup()).Compile(template);
            string asmExpected =
                "write_str 0, load_attr 1, brf 12, write_str 2";
            string asmResult = code.GetInstructions();
            Assert.AreEqual(asmExpected, asmResult);
            string stringsExpected = "[go: , name, hi, foo]";
            string stringsResult = code.strings.ToListString();
            Assert.AreEqual(stringsExpected, stringsResult);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestIfElse()
        {
            string template = "go: <if(name)>hi, foo<else>bye<endif>";
            CompiledTemplate code = new TemplateCompiler(new TemplateGroup()).Compile(template);
            string asmExpected =
                "write_str 0, " +
                "load_attr 1, " +
                "brf 15, " +
                "write_str 2, " +
                "br 18, " +
                "write_str 3";
            string asmResult = code.GetInstructions();
            Assert.AreEqual(asmExpected, asmResult);
            string stringsExpected = "[go: , name, hi, foo, bye]";
            string stringsResult = code.strings.ToListString();
            Assert.AreEqual(stringsExpected, stringsResult);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestElseIf()
        {
            string template = "go: <if(name)>hi, foo<elseif(user)>a user<endif>";
            CompiledTemplate code = new TemplateCompiler(new TemplateGroup()).Compile(template);
            string asmExpected =
                "write_str 0, " +
                "load_attr 1, " +
                "brf 15, " +
                "write_str 2, " +
                "br 24, " +
                "load_attr 3, " +
                "brf 24, " +
                "write_str 4";
            string asmResult = code.GetInstructions();
            Assert.AreEqual(asmExpected, asmResult);
            string stringsExpected = "[go: , name, hi, foo, user, a user]";
            string stringsResult = code.strings.ToListString();
            Assert.AreEqual(stringsExpected, stringsResult);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestElseIfElse()
        {
            string template = "go: <if(name)>hi, foo<elseif(user)>a user<else>bye<endif>";
            CompiledTemplate code = new TemplateCompiler(new TemplateGroup()).Compile(template);
            string asmExpected =
                "write_str 0, " +
                "load_attr 1, " +
                "brf 15, " +
                "write_str 2, " +
                "br 30, " +
                "load_attr 3, " +
                "brf 27, " +
                "write_str 4, " +
                "br 30, " +
                "write_str 5";
            string asmResult = code.GetInstructions();
            Assert.AreEqual(asmExpected, asmResult);
            string stringsExpected = "[go: , name, hi, foo, user, a user, bye]";
            string stringsResult = code.strings.ToListString();
            Assert.AreEqual(stringsExpected, stringsResult);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestOption()
        {
            string template = "hi <name; separator=\"x\">";
            CompiledTemplate code = new TemplateCompiler(new TemplateGroup()).Compile(template);
            string asmExpected =
                "write_str 0, load_attr 1, options, load_str 2, store_option 3, write_opt";
            string asmResult = code.GetInstructions();
            Assert.AreEqual(asmExpected, asmResult);
            string stringsExpected = "[hi , name, x]";
            string stringsResult = code.strings.ToListString();
            Assert.AreEqual(stringsExpected, stringsResult);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestOptionAsTemplate()
        {
            string template = "hi <name; separator={, }>";
            CompiledTemplate code = new TemplateCompiler(new TemplateGroup()).Compile(template);
            string asmExpected =
                "write_str 0, load_attr 1, options, new 2 0, store_option 3, write_opt";
            string asmResult = code.GetInstructions();
            Assert.AreEqual(asmExpected, asmResult);
            string stringsExpected = "[hi , name, _sub1]";
            string stringsResult = code.strings.ToListString();
            Assert.AreEqual(stringsExpected, stringsResult);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestOptions()
        {
            string template = "hi <name; anchor, wrap=foo(), separator=\", \">";
            CompiledTemplate code = new TemplateCompiler(new TemplateGroup()).Compile(template);
            string asmExpected =
                "write_str 0, " +
                "load_attr 1, " +
                "options, " +
                "load_str 2, " +
                "store_option 0, " +
                "new 3 0, " +
                "store_option 4, " +
                "load_str 4, " +
                "store_option 3, " +
                "write_opt";
            string stringsExpected = // the ", , ," is the ", " separator string
                "[hi , name, true, foo, , ]";
            string stringsResult = code.strings.ToListString();
            Assert.AreEqual(stringsExpected, stringsResult);
            string asmResult = code.GetInstructions();
            Assert.AreEqual(asmExpected, asmResult);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestEmptyList()
        {
            string template = "<[]>";
            CompiledTemplate code = new TemplateCompiler(new TemplateGroup()).Compile(template);
            string asmExpected = "list, write";
            string asmResult = code.GetInstructions();
            Assert.AreEqual(asmExpected, asmResult);
            string stringsExpected = "[]";
            string stringsResult = code.strings.ToListString();
            Assert.AreEqual(stringsExpected, stringsResult);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestList()
        {
            string template = "<[a,b]>";
            CompiledTemplate code = new TemplateCompiler(new TemplateGroup()).Compile(template);
            string asmExpected = "list, load_attr 0, add, load_attr 1, add, write";
            string asmResult = code.GetInstructions();
            Assert.AreEqual(asmExpected, asmResult);
            string stringsExpected = "[a, b]";
            string stringsResult = code.strings.ToListString();
            Assert.AreEqual(stringsExpected, stringsResult);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestEmbeddedRegion()
        {
            string template = "<@r>foo<@end>";
            // compile as if in root dir and in template 'a'
            CompiledTemplate code = new TemplateCompiler(new TemplateGroup('<', '>')).Compile("a", template);
            string asmExpected =
                "new 0 0, write";
            string asmResult = code.GetInstructions();
            Assert.AreEqual(asmExpected, asmResult);
            string stringsExpected = "[/region__/a__r]";
            string stringsResult = code.strings.ToListString();
            Assert.AreEqual(stringsExpected, stringsResult);
        }

        [TestMethod][TestCategory(TestCategories.ST4)]
        public void TestRegion()
        {
            string template = "x:<@r()>";
            // compile as if in root dir and in template 'a'
            CompiledTemplate code = new TemplateCompiler(new TemplateGroup('<', '>')).Compile("a", template);
            string asmExpected =
                "write_str 0, new 1 0, write";
            string asmResult = code.GetInstructions();
            Assert.AreEqual(asmExpected, asmResult);
            string stringsExpected = "[x:, /region__/a__r]";
            string stringsResult = code.strings.ToListString();
            Assert.AreEqual(stringsExpected, stringsResult);
        }
    }
}
