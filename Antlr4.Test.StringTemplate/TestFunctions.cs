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
 * THEORY OF LIABILITY, WHETHER IN CONTRACT, TemplateRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
 * THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

namespace Antlr4.Test.StringTemplate
{
    using System.Collections.Generic;
    using Antlr4.StringTemplate;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Path = System.IO.Path;

    [TestClass]
    public class TestFunctions : BaseTest
    {
        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestFirst()
        {
            string template = "<first(names)>";
            Template st = new Template(template);
            List<string> names = new List<string>() { "Ter", "Tom" };
            st.Add("names", names);
            string expected = "Ter";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestLength()
        {
            string template = "<length(names)>";
            Template st = new Template(template);
            List<string> names = new List<string>() { "Ter", "Tom" };
            st.Add("names", names);
            string expected = "2";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestLengthWithNullValues()
        {
            string template = "<length(names)>";
            Template st = new Template(template);
            List<string> names = new List<string>() { "Ter", null, "Tom", null };
            st.Add("names", names);
            string expected = "4";
            string result = st.Render();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestFirstOp()
        {
            Template e = new Template(
                    "<first(names)>"
                );
            e.Add("names", "Ter");
            e.Add("names", "Tom");
            e.Add("names", "Sriram");
            string expecting = "Ter";
            Assert.AreEqual(expecting, e.Render());
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestFirstOpList()
        {
            Template e = new Template(
                    "<first(names)>"
                );
            e.Add("names", new List<string>(new string[] { "Ter", "Tom", "Sriram" }));
            string expecting = "Ter";
            Assert.AreEqual(expecting, e.Render());
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestFirstOpArray()
        {
            Template e = new Template(
                    "<first(names)>"
                );
            e.Add("names", new string[] { "Ter", "Tom", "Sriram" });
            string expecting = "Ter";
            Assert.AreEqual(expecting, e.Render());
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestFirstOpPrimitiveArray()
        {
            Template e = new Template(
                    "<first(names)>"
                );
            e.Add("names", new int[] { 0, 1, 2 });
            string expecting = "0";
            Assert.AreEqual(expecting, e.Render());
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestTruncOp()
        {
            Template e = new Template(
                    "<trunc(names); separator=\", \">"
                );
            e.Add("names", "Ter");
            e.Add("names", "Tom");
            e.Add("names", "Sriram");
            string expecting = "Ter, Tom";
            Assert.AreEqual(expecting, e.Render());
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestTruncOpList()
        {
            Template e = new Template(
                    "<trunc(names); separator=\", \">"
                );
            e.Add("names", new List<string>(new string[] { "Ter", "Tom", "Sriram" }));
            string expecting = "Ter, Tom";
            Assert.AreEqual(expecting, e.Render());
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestTruncOpArray()
        {
            Template e = new Template(
                    "<trunc(names); separator=\", \">"
                );
            e.Add("names", new string[] { "Ter", "Tom", "Sriram" });
            string expecting = "Ter, Tom";
            Assert.AreEqual(expecting, e.Render());
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestTruncOpPrimitiveArray()
        {
            Template e = new Template(
                    "<trunc(names); separator=\", \">"
                );
            e.Add("names", new int[] { 0, 1, 2 });
            string expecting = "0, 1";
            Assert.AreEqual(expecting, e.Render());
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestRestOp()
        {
            Template e = new Template(
                    "<rest(names); separator=\", \">"
                );
            e.Add("names", "Ter");
            e.Add("names", "Tom");
            e.Add("names", "Sriram");
            string expecting = "Tom, Sriram";
            Assert.AreEqual(expecting, e.Render());
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestRestOpList()
        {
            Template e = new Template(
                    "<rest(names); separator=\", \">"
                );
            e.Add("names", new List<string>(new string[] { "Ter", "Tom", "Sriram" }));
            string expecting = "Tom, Sriram";
            Assert.AreEqual(expecting, e.Render());
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestRestOpArray()
        {
            Template e = new Template(
                    "<rest(names); separator=\", \">"
                );
            e.Add("names", new string[] { "Ter", "Tom", "Sriram" });
            string expecting = "Tom, Sriram";
            Assert.AreEqual(expecting, e.Render());
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestRestOpPrimitiveArray()
        {
            Template e = new Template(
                    "<rest(names); separator=\", \">"
                );
            e.Add("names", new int[] { 0, 1, 2 });
            string expecting = "1, 2";
            Assert.AreEqual(expecting, e.Render());
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestRestOpEmptyList()
        {
            Template e = new Template(
                    "<rest(names); separator=\", \">"
                );
            e.Add("names", new List<string>());
            string expecting = "";
            Assert.AreEqual(expecting, e.Render());
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestRestOpEmptyArray()
        {
            Template e = new Template(
                    "<rest(names); separator=\", \">"
                );
            e.Add("names", new string[0]);
            string expecting = "";
            Assert.AreEqual(expecting, e.Render());
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestRestOpEmptyPrimitiveArray()
        {
            Template e = new Template(
                    "<rest(names); separator=\", \">"
                );
            e.Add("names", new int[0]);
            string expecting = "";
            Assert.AreEqual(expecting, e.Render());
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestReUseOfRestResult()
        {
            string templates =
                "a(names) ::= \"<b(rest(names))>\"" + newline +
                "b(x) ::= \"<x>, <x>\"" + newline
                ;
            writeFile(tmpdir, "t.stg", templates);
            TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
            Template e = group.GetInstanceOf("a");
            List<string> names = new List<string>();
            names.Add("Ter");
            names.Add("Tom");
            e.Add("names", names);
            string expecting = "Tom, Tom";
            Assert.AreEqual(expecting, e.Render());
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestReUseOfRestPrimitiveArrayResult()
        {
            string templates =
                "a(names) ::= \"<b(rest(names))>\"" + newline +
                "b(x) ::= \"<x>, <x>\"" + newline
                ;
            writeFile(tmpdir, "t.stg", templates);
            TemplateGroup group = new TemplateGroupFile(tmpdir + "/" + "t.stg");
            Template e = group.GetInstanceOf("a");
            e.Add("names", new int[] { 0, 1 });
            string expecting = "1, 1";
            Assert.AreEqual(expecting, e.Render());
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestLastOp()
        {
            Template e = new Template(
                    "<last(names)>"
                );
            e.Add("names", "Ter");
            e.Add("names", "Tom");
            e.Add("names", "Sriram");
            string expecting = "Sriram";
            Assert.AreEqual(expecting, e.Render());
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestLastOpList()
        {
            Template e = new Template(
                    "<last(names)>"
                );
            e.Add("names", new List<string>(new string[] { "Ter", "Tom", "Sriram" }));
            string expecting = "Sriram";
            Assert.AreEqual(expecting, e.Render());
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestLastOpArray()
        {
            Template e = new Template(
                    "<last(names)>"
                );
            e.Add("names", new string[] { "Ter", "Tom", "Sriram" });
            string expecting = "Sriram";
            Assert.AreEqual(expecting, e.Render());
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestLastOpPrimitiveArray()
        {
            Template e = new Template(
                    "<last(names)>"
                );
            e.Add("names", new int[] { 0, 1, 2 });
            string expecting = "2";
            Assert.AreEqual(expecting, e.Render());
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestStripOp()
        {
            Template e = new Template(
                    "<strip(names); null=\"n/a\">"
                );
            e.Add("names", null);
            e.Add("names", "Tom");
            e.Add("names", null);
            e.Add("names", null);
            e.Add("names", "Sriram");
            e.Add("names", null);
            string expecting = "TomSriram";
            Assert.AreEqual(expecting, e.Render());
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestStripOpList()
        {
            Template e = new Template(
                    "<strip(names); null=\"n/a\">"
                );
            e.Add("names", new List<string>(new string[] { null, "Tom", null, null, "Sriram", null }));
            string expecting = "TomSriram";
            Assert.AreEqual(expecting, e.Render());
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestStripOpArray()
        {
            Template e = new Template(
                    "<strip(names); null=\"n/a\">"
                );
            e.Add("names", new string[] { null, "Tom", null, null, "Sriram", null });
            string expecting = "TomSriram";
            Assert.AreEqual(expecting, e.Render());
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestLengthStrip()
        {
            Template e = new Template(
                    "<length(strip(names))>"
                );
            e.Add("names", null);
            e.Add("names", "Tom");
            e.Add("names", null);
            e.Add("names", null);
            e.Add("names", "Sriram");
            e.Add("names", null);
            string expecting = "2";
            Assert.AreEqual(expecting, e.Render());
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestLengthStripList()
        {
            Template e = new Template(
                    "<length(strip(names))>"
                );
            e.Add("names", new List<string>(new string[] { null, "Tom", null, null, "Sriram", null }));
            string expecting = "2";
            Assert.AreEqual(expecting, e.Render());
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestLengthStripArray()
        {
            Template e = new Template(
                    "<length(strip(names))>"
                );
            e.Add("names", new string[] { null, "Tom", null, null, "Sriram", null });
            string expecting = "2";
            Assert.AreEqual(expecting, e.Render());
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestCombinedOp()
        {
            // replace first of yours with first of mine
            Template e = new Template(
                    "<[first(mine),rest(yours)]; separator=\", \">"
                );
            e.Add("mine", "1");
            e.Add("mine", "2");
            e.Add("mine", "3");
            e.Add("yours", "a");
            e.Add("yours", "b");
            string expecting = "1, b";
            Assert.AreEqual(expecting, e.Render());
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestCombinedOpList()
        {
            // replace first of yours with first of mine
            Template e = new Template(
                    "<[first(mine),rest(yours)]; separator=\", \">"
                );
            e.Add("mine", new List<string>(new string[] { "1", "2", "3" }));
            e.Add("yours", "a");
            e.Add("yours", "b");
            string expecting = "1, b";
            Assert.AreEqual(expecting, e.Render());
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestCombinedOpArray()
        {
            // replace first of yours with first of mine
            Template e = new Template(
                    "<[first(mine),rest(yours)]; separator=\", \">"
                );
            e.Add("mine", new string[] { "1", "2", "3" });
            e.Add("yours", "a");
            e.Add("yours", "b");
            string expecting = "1, b";
            Assert.AreEqual(expecting, e.Render());
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestCombinedOpPrimitiveArray()
        {
            // replace first of yours with first of mine
            Template e = new Template(
                    "<[first(mine),rest(yours)]; separator=\", \">"
                );
            e.Add("mine", new int[] { 1, 2, 3 });
            e.Add("yours", "a");
            e.Add("yours", "b");
            string expecting = "1, b";
            Assert.AreEqual(expecting, e.Render());
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestCatListAndSingleAttribute()
        {
            // replace first of yours with first of mine
            Template e = new Template(
                    "<[mine,yours]; separator=\", \">"
                );
            e.Add("mine", "1");
            e.Add("mine", "2");
            e.Add("mine", "3");
            e.Add("yours", "a");
            string expecting = "1, 2, 3, a";
            Assert.AreEqual(expecting, e.Render());
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestCatListAndSingleAttribute2()
        {
            // replace first of yours with first of mine
            Template e = new Template(
                    "<[mine,yours]; separator=\", \">"
                );
            e.Add("mine", new List<string>(new string[] { "1", "2", "3" }));
            e.Add("yours", "a");
            string expecting = "1, 2, 3, a";
            Assert.AreEqual(expecting, e.Render());
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestCatArrayAndSingleAttribute()
        {
            // replace first of yours with first of mine
            Template e = new Template(
                    "<[mine,yours]; separator=\", \">"
                );
            e.Add("mine", new string[] { "1", "2", "3" });
            e.Add("yours", "a");
            string expecting = "1, 2, 3, a";
            Assert.AreEqual(expecting, e.Render());
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestCatPrimitiveArrayAndSingleAttribute()
        {
            // replace first of yours with first of mine
            Template e = new Template(
                    "<[mine,yours]; separator=\", \">"
                );
            e.Add("mine", new int[] { 1, 2, 3 });
            e.Add("yours", "a");
            string expecting = "1, 2, 3, a";
            Assert.AreEqual(expecting, e.Render());
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestReUseOfCat()
        {
            string templates =
                "a(mine,yours) ::= \"<b([mine,yours])>\"" + newline +
                "b(x) ::= \"<x>, <x>\"" + newline
                ;
            writeFile(tmpdir, "t.stg", templates);
            TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
            Template e = group.GetInstanceOf("a");
            List<string> mine = new List<string>();
            mine.Add("Ter");
            mine.Add("Tom");
            e.Add("mine", mine);
            List<string> yours = new List<string>();
            yours.Add("Foo");
            e.Add("yours", yours);
            string expecting = "TerTomFoo, TerTomFoo";
            Assert.AreEqual(expecting, e.Render());
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestCatListAndEmptyAttributes()
        {
            // + is overloaded to be cat strings and cat lists so the
            // two operands (from left to right) determine which way it
            // goes.  In this case, x+mine is a list so everything from their
            // to the right becomes list cat.
            Template e = new Template(
                    "<[x,mine,y,yours,z]; separator=\", \">"
                );
            e.Add("mine", "1");
            e.Add("mine", "2");
            e.Add("mine", "3");
            e.Add("yours", "a");
            string expecting = "1, 2, 3, a";
            Assert.AreEqual(expecting, e.Render());
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestCatListAndEmptyAttributes2()
        {
            // + is overloaded to be cat strings and cat lists so the
            // two operands (from left to right) determine which way it
            // goes.  In this case, x+mine is a list so everything from their
            // to the right becomes list cat.
            Template e = new Template(
                    "<[x,mine,y,yours,z]; separator=\", \">"
                );
            e.Add("mine", new List<string>(new string[] { "1", "2", "3" }));
            e.Add("yours", "a");
            string expecting = "1, 2, 3, a";
            Assert.AreEqual(expecting, e.Render());
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestCatArrayAndEmptyAttributes2()
        {
            // + is overloaded to be cat strings and cat lists so the
            // two operands (from left to right) determine which way it
            // goes.  In this case, x+mine is a list so everything from their
            // to the right becomes list cat.
            Template e = new Template(
                    "<[x,mine,y,yours,z]; separator=\", \">"
                );
            e.Add("mine", new string[] { "1", "2", "3" });
            e.Add("yours", "a");
            string expecting = "1, 2, 3, a";
            Assert.AreEqual(expecting, e.Render());
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestCatPrimitiveArrayAndEmptyAttributes()
        {
            // + is overloaded to be cat strings and cat lists so the
            // two operands (from left to right) determine which way it
            // goes.  In this case, x+mine is a list so everything from their
            // to the right becomes list cat.
            Template e = new Template(
                    "<[x,mine,y,yours,z]; separator=\", \">"
                );
            e.Add("mine", new int[] { 1, 2, 3 });
            e.Add("yours", "a");
            string expecting = "1, 2, 3, a";
            Assert.AreEqual(expecting, e.Render());
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestNestedOp()
        {
            Template e = new Template(
                    "<first(rest(names))>" // gets 2nd element
                );
            e.Add("names", "Ter");
            e.Add("names", "Tom");
            e.Add("names", "Sriram");
            string expecting = "Tom";
            Assert.AreEqual(expecting, e.Render());
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestNestedOpList()
        {
            Template e = new Template(
                    "<first(rest(names))>" // gets 2nd element
                );
            e.Add("names", new List<string>(new string[] { "Ter", "Tom", "Sriram" }));
            string expecting = "Tom";
            Assert.AreEqual(expecting, e.Render());
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestNestedOpArray()
        {
            Template e = new Template(
                    "<first(rest(names))>" // gets 2nd element
                );
            e.Add("names", new string[] { "Ter", "Tom", "Sriram" });
            string expecting = "Tom";
            Assert.AreEqual(expecting, e.Render());
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestNestedOpPrimitiveArray()
        {
            Template e = new Template(
                    "<first(rest(names))>" // gets 2nd element
                );
            e.Add("names", new int[] { 0, 1, 2 });
            string expecting = "1";
            Assert.AreEqual(expecting, e.Render());
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestFirstWithOneAttributeOp()
        {
            Template e = new Template(
                    "<first(names)>"
                );
            e.Add("names", "Ter");
            string expecting = "Ter";
            Assert.AreEqual(expecting, e.Render());
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestLastWithOneAttributeOp()
        {
            Template e = new Template(
                    "<last(names)>"
                );
            e.Add("names", "Ter");
            string expecting = "Ter";
            Assert.AreEqual(expecting, e.Render());
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestLastWithLengthOneListAttributeOp()
        {
            Template e = new Template(
                    "<last(names)>"
                );
            e.Add("names", new List<string>() { "Ter" });
            string expecting = "Ter";
            Assert.AreEqual(expecting, e.Render());
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestLastWithLengthOneArrayAttributeOp()
        {
            Template e = new Template(
                    "<last(names)>"
                );
            e.Add("names", new string[] { "Ter" });
            string expecting = "Ter";
            Assert.AreEqual(expecting, e.Render());
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestLastWithLengthOnePrimitiveArrayAttributeOp()
        {
            Template e = new Template(
                    "<last(names)>"
                );
            e.Add("names", new int[] { 0 });
            string expecting = "0";
            Assert.AreEqual(expecting, e.Render());
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestRestWithOneAttributeOp()
        {
            Template e = new Template(
                    "<rest(names)>"
                );
            e.Add("names", "Ter");
            string expecting = "";
            Assert.AreEqual(expecting, e.Render());
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestRestWithLengthOneListAttributeOp()
        {
            Template e = new Template(
                    "<rest(names)>"
                );
            e.Add("names", new List<string>() { "Ter" });
            string expecting = "";
            Assert.AreEqual(expecting, e.Render());
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestRestWithLengthOneArrayAttributeOp()
        {
            Template e = new Template(
                    "<rest(names)>"
                );
            e.Add("names", new string[] { "Ter" });
            string expecting = "";
            Assert.AreEqual(expecting, e.Render());
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestRestWithLengthOnePrimitiveArrayAttributeOp()
        {
            Template e = new Template(
                    "<rest(names)>"
                );
            e.Add("names", new int[] { 0 });
            string expecting = "";
            Assert.AreEqual(expecting, e.Render());
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestRepeatedRestOp()
        {
            Template e = new Template(
                    "<rest(names)>, <rest(names)>" // gets 2nd element
                );
            e.Add("names", "Ter");
            e.Add("names", "Tom");
            string expecting = "Tom, Tom";
            Assert.AreEqual(expecting, e.Render());
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestRepeatedRestOpList()
        {
            Template e = new Template(
                    "<rest(names)>, <rest(names)>" // gets 2nd element
                );
            e.Add("names", new List<string>(new string[] { "Ter", "Tom" }));
            string expecting = "Tom, Tom";
            Assert.AreEqual(expecting, e.Render());
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestRepeatedRestOpArray()
        {
            Template e = new Template(
                    "<rest(names)>, <rest(names)>" // gets 2nd element
                );
            e.Add("names", new string[] { "Ter", "Tom" });
            string expecting = "Tom, Tom";
            Assert.AreEqual(expecting, e.Render());
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestRepeatedRestOpPrimitiveArray()
        {
            Template e = new Template(
                    "<rest(names)>, <rest(names)>" // gets 2nd element
                );
            e.Add("names", new int[] { 0, 1 });
            string expecting = "1, 1";
            Assert.AreEqual(expecting, e.Render());
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestIncomingLists()
        {
            Template e = new Template(
                    "<rest(names)>, <rest(names)>" // gets 2nd element
                );
            e.Add("names", "Ter");
            e.Add("names", "Tom");
            string expecting = "Tom, Tom";
            Assert.AreEqual(expecting, e.Render());
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestFirstWithCatAttribute()
        {
            Template e = new Template(
                    "<first([names,phones])>"
                );
            e.Add("names", "Ter");
            e.Add("names", "Tom");
            e.Add("phones", "1");
            e.Add("phones", "2");
            string expecting = "Ter";
            Assert.AreEqual(expecting, e.Render());
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestFirstWithListOfMaps()
        {
            Template e = new Template(
                    "<first(maps).Ter>"
                );
            IDictionary<string, string> m1 = new Dictionary<string, string>();
            IDictionary<string, string> m2 = new Dictionary<string, string>();
            m1["Ter"] = "x5707";
            e.Add("maps", m1);
            m2["Tom"] = "x5332";
            e.Add("maps", m2);
            string expecting = "x5707";
            Assert.AreEqual(expecting, e.Render());

            List<IDictionary<string, string>> list = new List<IDictionary<string, string>>() { m1, m2 };
            e.Add("maps", list);
            expecting = "x5707";
            Assert.AreEqual(expecting, e.Render());
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestFirstWithListOfMaps2()
        {
            Template e = new Template(
                    "<first(maps):{ m | <m>!}>"
                );
            IDictionary<string, string> m1 = new Dictionary<string, string>();
            IDictionary<string, string> m2 = new Dictionary<string, string>();
            m1["Ter"] = "x5707";
            e.Add("maps", m1);
            m2["Tom"] = "x5332";
            e.Add("maps", m2);
            string expecting = "Ter!";
            Assert.AreEqual(expecting, e.Render());
            List<IDictionary<string, string>> list = new List<IDictionary<string, string>>() { m1, m2 };
            e.Add("maps", list);
            expecting = "Ter!";
            Assert.AreEqual(expecting, e.Render());
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestTrim()
        {
            Template e = new Template(
                    "<trim(name)>"
                );
            e.Add("name", " Ter  \n");
            string expecting = "Ter";
            Assert.AreEqual(expecting, e.Render());
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestStrlen()
        {
            Template e = new Template(
                    "<strlen(name)>"
                );
            e.Add("name", "012345");
            string expecting = "6";
            Assert.AreEqual(expecting, e.Render());
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestReverse()
        {
            Template e = new Template(
                    "<reverse(names); separator=\", \">"
                );
            e.Add("names", "Ter");
            e.Add("names", "Tom");
            e.Add("names", "Sriram");
            string expecting = "Sriram, Tom, Ter";
            Assert.AreEqual(expecting, e.Render());
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestReverseList()
        {
            Template e = new Template(
                    "<reverse(names); separator=\", \">"
                );
            e.Add("names", new List<string>(new string[] { "Ter", "Tom", "Sriram" }));
            string expecting = "Sriram, Tom, Ter";
            Assert.AreEqual(expecting, e.Render());
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestReverseArray()
        {
            Template e = new Template(
                    "<reverse(names); separator=\", \">"
                );
            e.Add("names", new string[] { "Ter", "Tom", "Sriram" });
            string expecting = "Sriram, Tom, Ter";
            Assert.AreEqual(expecting, e.Render());
        }

        [TestMethod]
        [TestCategory(TestCategories.ST4)]
        public void TestReversePrimitiveArray()
        {
            Template e = new Template(
                    "<reverse(names); separator=\", \">"
                );
            e.Add("names", new int[] { 0, 1, 2 });
            string expecting = "2, 1, 0";
            Assert.AreEqual(expecting, e.Render());
        }
    }
}
