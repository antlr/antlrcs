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
    using System.Collections.Generic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using StringTemplate;
    using Interpreter = StringTemplate.Interpreter;
    using ST = StringTemplate.Template;
    using STGroup = StringTemplate.TemplateGroup;
    using STGroupFile = StringTemplate.TemplateGroupFile;
    using String = System.String;
    using StringWriter = System.IO.StringWriter;

    [TestClass]
    public class TestDebugEvents : StringTemplateTestBase
    {
        [TestMethod]
        public void TestString()
        {
            string templates =
                "t() ::= <<foo>>" + newline;

            WriteFile(tmpdir, "t.stg", templates);
            STGroup group = new STGroupFile(tmpdir + "/" + "t.stg");
            ST st = group.GetInstanceOf("t");
            st.code.Dump();
            StringWriter sw = new StringWriter();
            Interpreter interp = new Interpreter(group, new AutoIndentWriter(sw));
            interp.Debug = true;
            interp.Exec(st);
            String expected = "";
            IList<Interpreter.DebugEvent> events = interp.Events;
            String result = events.ToString();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestAttribute()
        {
            String templates =
                "t(x) ::= << <x> >>" + newline;

            WriteFile(tmpdir, "t.stg", templates);
            STGroup group = new STGroupFile(tmpdir + "/" + "t.stg");
            ST st = group.GetInstanceOf("t");
            st.code.Dump();
            st.Add("x", "foo");
            StringWriter sw = new StringWriter();
            Interpreter interp = new Interpreter(group, new AutoIndentWriter(sw));
            interp.Debug = true;
            interp.Exec(st);
            String expected = "";
            IList<Interpreter.DebugEvent> events = interp.Events;
            String result = events.ToString();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestTemplateCall()
        {
            String templates =
                "t(x) ::= <<[<u()>]>>\n" +
                "u() ::= << <x> >>\n";

            WriteFile(tmpdir, "t.stg", templates);
            STGroup group = new STGroupFile(tmpdir + "/" + "t.stg");
            ST st = group.GetInstanceOf("t");
            st.code.Dump();
            st.Add("x", "foo");
            StringWriter sw = new StringWriter();
            Interpreter interp = new Interpreter(group, new AutoIndentWriter(sw));
            interp.Debug = true;
            interp.Exec(st);
            String expected = "";
            IList<Interpreter.DebugEvent> events = interp.Events;
            String result = events.ToString();
            Assert.AreEqual(expected, result);
        }
    }
}
