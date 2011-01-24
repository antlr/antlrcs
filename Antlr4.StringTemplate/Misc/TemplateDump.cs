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

namespace Antlr4.StringTemplate.Misc
{
    using System.Collections.Generic;
    using System.Linq;
    using Antlr.Runtime.JavaExtensions;
    using Environment = System.Environment;
    using StringBuilder = System.Text.StringBuilder;
    using StringComparer = System.StringComparer;

    /** This class dumps out a hierarchy of templates in text form, indented
     *  to show the nested relationship.  Usage:
     *
     *     Template st = ...;
     *     TemplateDump d = new TemplateDump(st);
     *     System.out.println(d.ToString());
     */
    public class TemplateDump
    {
        private readonly Template self;

        public TemplateDump(Template self)
        {
            this.self = self;
        }

        public static string toString(Template self)
        {
            TemplateDump d = new TemplateDump(self);
            return d.ToString();
        }

        public override string ToString()
        {
            return toString(0);
        }

        protected virtual string toString(int n)
        {
            StringBuilder buf = new StringBuilder();
            buf.Append(getTemplateDeclaratorString() + ":");
            n++;
            if (self.getAttributes() != null)
            {
                List<string> attrNames = new List<string>(self.getAttributes().Keys);
                attrNames.Sort(StringComparer.Ordinal);
                string longestName = attrNames.OrderBy(i => i.Length).Last();
                int w = longestName.Length;
                foreach (object attrName in attrNames)
                {
                    string name = (string)attrName;
                    buf.Append(Environment.NewLine);
                    indent(buf, n);
                    buf.Append(string.Format("%-" + w + "s = ", name));
                    buf.Append(string.Format(string.Format("{{0,-{0}}} = ", w), name));
                    object value;
                    self.getAttributes().TryGetValue(name, out value);
                    buf.Append(getValueDebugString(value, n));
                }
            }
            buf.Append(Environment.NewLine);
            n--;
            indent(buf, n);
            buf.Append("]");
            return buf.ToString();
        }

        protected virtual string getValueDebugString(object value, int n)
        {
            StringBuilder buf = new StringBuilder();
            value = Interpreter.convertAnythingIteratableToIterator(value);
            if (value is Template)
            {
                TemplateDump d = new TemplateDump((Template)value);
                buf.Append(d.toString(n));
            }
            else if (value is Iterator)
            {
                Iterator it = (Iterator)value;
                int na = 0;
                while (it.hasNext())
                {
                    string v = getValueDebugString(it.next(), n);
                    if (na > 0)
                        buf.Append(", ");
                    buf.Append(v);
                    na++;
                }
            }
            else
            {
                buf.Append(value);
            }
            return buf.ToString();
        }

        protected virtual string getTemplateDeclaratorString()
        {
            StringBuilder buf = new StringBuilder();
            buf.Append("<");
            buf.Append(self.getName());
            buf.Append("(");
            if (self.impl.formalArguments != null)
                buf.Append(string.Join(",", self.impl.formalArguments.Select(i => i.Name).ToArray()));
            buf.Append(")@");
            buf.Append(GetHashCode());
            buf.Append(">");
            return buf.ToString();
        }

        protected virtual void indent(StringBuilder buf, int n)
        {
            for (int i = 1; i <= n; i++)
                buf.Append("   ");
        }
    }
}
