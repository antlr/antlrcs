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
                "t(x) ::= <<[<u()>]>>\n" +
                "u() ::= << <x> >>\n";

            string tmpdir = Path.GetTempPath();
            File.WriteAllText(Path.Combine(tmpdir, "t.stg"), templates);
            TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
            Template st = group.GetInstanceOf("t");
            st.code.Dump();
            st.Add("x", "foo");
            st.Add("y", "bar");

            StringWriter sw = new StringWriter();
            Interpreter interp = new Interpreter(group, new AutoIndentWriter(sw));
            interp.Debug = true;
            interp.Exec(st);
            IList<Interpreter.DebugEvent> events = interp.Events;

            //STViewFrame m = new STViewFrame();
            //DefaultListModel stackModel = new DefaultListModel();
            //IList<Template> stack = st.GetEnclosingInstanceStack();
            //foreach (ST s in stack) stackModel.addElement(s.getName());
            //m.stack.setModel(stackModel);
            //m.stack.addListSelectionListener(
            //    new ListSelectionListener() {
            //        public void valueChanged(ListSelectionEvent e) {
            //            System.out.println("touched "+e.getFirstIndex());
            //        }
            //    }
            //);

            //DefaultListModel attrModel = new DefaultListModel();
            //IDictionary<string, object> attrs = st.Attributes;
            //foreach (var pair in attrs)
            //{
            //    attrModel.addElement(a + " = " + attrs.get(a));
            //}
            //m.attributes.setModel(attrModel);

            lstStack.DataContext = st.GetEnclosingInstanceStack();
            lstAttributes.DataContext = st.Attributes;
            txtOutput.Text = sw.ToString();
            txtTemplate.Text = st.code.template;
        }
    }
}
