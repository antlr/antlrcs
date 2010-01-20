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
    using System.Windows.Documents;
    using System.Windows.Media;
    using StringTemplate.Debug;

    public partial class TemplateVisualizer : Window
    {
        internal TemplateVisualizer()
        {
            InitializeComponent();

            StringWriter sw = new StringWriter();
            var template = CreateDefaultTemplate();
            Interpreter interp = new Interpreter(template.groupThatCreatedThisInstance);
            interp.Exec(new AutoIndentWriter(sw), template);
            IList<InterpEvent> events = interp.Events;

            string text = sw.ToString();
            templatesTree.Items.Add(new RootEvent(template, 0, text.Length));
            txtOutput.Document = new FlowDocument(new Paragraph(new Run(text)));
        }

        public TemplateVisualizer(DebugTemplate template, string output, List<InterpEvent> allEvents, List<TemplateMessage> errors)
        {
            InitializeComponent();

            templatesTree.Items.Add(new RootEvent(template, 0, output.Length));
            txtOutput.Document = new FlowDocument(new Paragraph(new Run(output)));
        }

        private static DebugTemplate CreateDefaultTemplate()
        {
            string templates =
                "method(type,name,args,stats) ::= <<\n" +
                "public <type> <ick()> <name>(<args:{a| int <a>}; separator=\", \">) {\n" +
                "    <if(locals)>int locals[<locals>];<endif>\n" +
                "    <stats;separator=\"\\n\">\n" +
                "}\n" +
                ">>\n" +
                "assign(a,b) ::= \"<a> = <b> <a,b:{foo}>;\"\n" +
                "return(x) ::= <<return <x>;>>\n" +
                "paren(x) ::= \"(<x>)\"\n";

            string tmpdir = Path.GetTempPath();
            File.WriteAllText(Path.Combine(tmpdir, "t.stg"), templates);
            TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
            group.Debug = true;
            DebugTemplate st = (DebugTemplate)group.GetInstanceOf("method");
            st.code.Dump();
            st.Add("type", "float");
            st.Add("name", "foo");
            st.Add("locals", 3);
            st.Add("args", new String[] { "x", "y", "z" });
            Template s1 = group.GetInstanceOf("assign");
            Template paren = group.GetInstanceOf("paren");
            paren.Add("x", "x");
            s1.Add("a", paren);
            s1.Add("b", "y");
            Template s2 = group.GetInstanceOf("assign");
            s2.Add("a", "y");
            s2.Add("b", "z");
            Template s3 = group.GetInstanceOf("return");
            s3.Add("x", "3.14159");
            st.Add("stats", s1);
            st.Add("stats", s2);
            st.Add("stats", s3);

            return st;
        }

        private void OnTextTemplateDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            txtTemplate.Document.Blocks.Clear();

            InterpEvent templateEvent = e.NewValue as InterpEvent;
            if (templateEvent == null)
                return;

            Template template = templateEvent.Template;
            if (template != null)
            {
                txtTemplate.Document.Blocks.Add(new Paragraph(new Run(template.CompiledTemplate.Template)));
                if (template.IsSubtemplate)
                {
                    Highlight(txtTemplate.Document, template.CompiledTemplate.embeddedStart, template.CompiledTemplate.embeddedStop - template.CompiledTemplate.embeddedStart + 1);
                }
            }
        }

        private static void Highlight(FlowDocument document, int start, int length)
        {
            var range = new TextRange(document.ContentStart, document.ContentEnd);
            range.ClearAllProperties();
            range = new TextRange(document.ContentStart.GetPositionAtOffset(start + 2), document.ContentStart.GetPositionAtOffset(start + length + 2));
            range.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.LightGray);
        }

        private void OnTemplatesTreeSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            InterpEvent debugEvent = e.NewValue as InterpEvent;
            if (debugEvent == null)
                return;

            Highlight(txtOutput.Document, debugEvent.Start, debugEvent.Stop - debugEvent.Start + 1);
        }

        private class RootEvent : InterpEvent
        {
            public RootEvent(DebugTemplate template, int start, int stop)
                : base(template, start, stop)
            {
            }
        }
    }
}
