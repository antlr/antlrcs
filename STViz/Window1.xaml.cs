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

namespace STViz
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using StringTemplate;
    using File = System.IO.File;
    using Path = System.IO.Path;
    using StringWriter = System.IO.StringWriter;

    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();

            string templates =
                "method(type,name,args,stats) ::= <<\n" +
                "public <type> <name>(<args:{a| int <a>}; separator=\", \">) {\n" +
                "    <stats;separator=\"\\n\">\n" +
                "}\n" +
                ">>\n" +
                "assign(a,b) ::= \"<a> = <b>;\"\n" +
                "return(x) ::= <<return <x>;>>\n";

            string tmpdir = Path.GetTempPath();
            File.WriteAllText(Path.Combine(tmpdir, "t.stg"), templates);
            TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
            Template st = group.GetInstanceOf("method");
            st.code.Dump();
            st.Add("type", "float");
            st.Add("name", "foo");
            st.Add("args", new String[] { "x", "y", "z" });
            Template s1 = group.GetInstanceOf("assign");
            s1.Add("a", "x");
            s1.Add("b", "y");
            Template s2 = group.GetInstanceOf("assign");
            s2.Add("a", "y");
            s2.Add("b", "z");
            Template s3 = group.GetInstanceOf("return");
            s3.Add("x", "3.14159");
            st.Add("stats", s1);
            st.Add("stats", s2);
            st.Add("stats", s3);

            StringWriter sw = new StringWriter();
            Interpreter interp = new Interpreter(group, new AutoIndentWriter(sw));
            interp.Debug = true;
            interp.Exec(st);
            IList<Interpreter.DebugEvent> events = interp.Events;

            templatesTree.Items.Add(st);
            txtOutput.Text = sw.ToString();
        }
    }
}
