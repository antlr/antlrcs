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
    using Environment = System.Environment;
    using Antlr4.StringTemplate.Debug;
    using Antlr4.Test.StringTemplate.Extensions;
    using System.Collections.Generic;
    using Path = System.IO.Path;

    [TestClass]
    public class TestDebugEvents : BaseTest
    {
        [TestMethod]
        public void TestString()
        {
            string templates =
                "t() ::= <<foo>>" + Environment.NewLine;

            writeFile(tmpdir, "t.stg", templates);
            TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
            TemplateGroup.debug = true;
            DebugST st = (DebugST)group.GetInstanceOf("t");
            List<InterpEvent> events = st.GetEvents();
            string expected =
                "[EvalExprEvent{self=t(), output=[0..3), expr=foo}," +
                " EvalTemplateEvent{self=t(), output=[0..3)}]";
            string result = events.ToListString();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestAttribute()
        {
            string templates =
                "t(x) ::= << <x> >>" + Environment.NewLine;

            writeFile(tmpdir, "t.stg", templates);
            TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
            TemplateGroup.debug = true;
            DebugST st = (DebugST)group.GetInstanceOf("t");
            List<InterpEvent> events = st.GetEvents();
            string expected =
                "[EvalExprEvent{self=t(), output=[0..0), expr=<x>}," +
                " EvalExprEvent{self=t(), output=[0..1), expr= }," +
                " EvalTemplateEvent{self=t(), output=[0..1)}]";
            string result = events.ToListString();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestTemplateCall()
        {
            string templates =
                "t(x) ::= <<[<u()>]>>\n" +
                "u() ::= << <x> >>\n";

            writeFile(tmpdir, "t.stg", templates);
            TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
            TemplateGroup.debug = true;
            DebugST st = (DebugST)group.GetInstanceOf("t");
            List<InterpEvent> events = st.GetEvents();
            string expected =
                "[EvalExprEvent{self=t(), output=[0..1), expr=[}," +
                " EvalExprEvent{self=u(), output=[1..1), expr=<x>}," +
                " EvalExprEvent{self=u(), output=[1..2), expr= }," +
                " EvalTemplateEvent{self=u(), output=[1..2)}," +
                " EvalExprEvent{self=t(), output=[1..2), expr=<u()>}," +
                " EvalExprEvent{self=t(), output=[2..3), expr=]}," +
                " EvalTemplateEvent{self=t(), output=[0..3)}]";
            string result = events.ToListString();
            Assert.AreEqual(expected, result);
        }
    }
}
