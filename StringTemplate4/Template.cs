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

namespace StringTemplate
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using StringTemplate.Compiler;
    using ArgumentException = System.ArgumentException;
    using ArrayList = System.Collections.ArrayList;
    using CultureInfo = System.Globalization.CultureInfo;
    using IList = System.Collections.IList;
    using StringBuilder = System.Text.StringBuilder;
    using StringWriter = System.IO.StringWriter;

    public class Template
    {
        public const string SubtemplatePrefix = "_sub";
        public static readonly TemplateName UnknownName = new TemplateName("anonymous");
        public static readonly Template Blank = new BlankTemplate();

        /** The code to interpret; it pulls from attributes and this template's
         *  group of templates to evaluate to string.
         */
        public CompiledTemplate code; // TODO: is this the right name?

        /** Map an attribute name to its value(s). */
        internal IDictionary<string, object> attributes;

        /** Enclosing instance if I'm embedded within another template.
         *  IF-subtemplates are considered embedded as well.
         */
        internal Template enclosingInstance; // who's your daddy?

        /** Created as instance of which group? We need this to init interpreter
         *  via render.  So, we create st and then it needs to know which
         *  group created it for sake of polymorphism:
         *
         *  st = skin1.getInstanceOf("searchbox");
         *  result = st.render(); // knows skin1 created it
         */
        public TemplateGroup groupThatCreatedThisInstance;

        /** Normally, formal parameters hide any attributes inherited from the
         *  enclosing template with the same name.  This is normally what you
         *  want, but makes it hard to invoke another template passing in all
         *  the data.  Use notation now: <otherTemplate(...)> to say "pass in
         *  all data".  Works great.  Can also say <otherTemplate(foo="xxx",...)>
         */
        protected internal bool passThroughAttributes = false;

        /** Just an alias for ArrayList, but this way I can track whether a
         *  list is something ST created or it's an incoming list.
         */
        public sealed class AttributeList : ArrayList
        {
            public AttributeList(int capacity)
                : base(capacity)
            {
            }

            public AttributeList()
            {
            }
        }

        public Template()
        {
        }

        public Template(string template)
            : this(TemplateGroup.defaultGroup, template)
        {
        }

        public Template(TemplateGroup nativeGroup, string template)
        {
            code = nativeGroup.DefineTemplate(UnknownName, template);
            groupThatCreatedThisInstance = nativeGroup;
        }

        public IDictionary<string, object> Attributes
        {
            get
            {
                return this.attributes;
            }
        }

        public CompiledTemplate CompiledTemplate
        {
            get
            {
                return code;
            }
        }

        [DebuggerHidden]
        public IEnumerable<Template> EnclosingInstanceStack
        {
            get
            {
                return GetEnclosingInstanceStack(false);
            }
        }

        public bool IsSubtemplate
        {
            get
            {
                return CompiledTemplate.IsSubtemplate;
            }
        }

        public TemplateName Name
        {
            get
            {
                return code.Name;
            }
        }

        public virtual void Add(string name, object value)
        {
            if (name == null)
                return; // allow null value

            if (name.IndexOf('.') >= 0)
            {
                throw new ArgumentException("cannot have '.' in attribute names");
            }

            if (value is Template)
                ((Template)value).enclosingInstance = this;

            object curvalue = null;
            if (attributes == null || !attributes.ContainsKey(name))
            { // new attribute
                RawSetAttribute(name, value);
                return;
            }
            if (attributes != null)
                curvalue = attributes[name];

            // attribute will be multi-valued for sure now
            // convert current attribute to list if not already
            // copy-on-write semantics; copy a list injected by user to add new value
            AttributeList multi = ConvertToAttributeList(curvalue);
            RawSetAttribute(name, multi); // replace with list

            // now, add incoming value to multi-valued attribute
            if (value is IList)
            {
                // flatten incoming list into existing list
                multi.AddRange((IList)value);
            }
            else if (value != null && value.GetType().IsArray)
            {
                multi.AddRange((object[])value);
            }
            else
            {
                multi.Add(value);
            }
        }

        protected internal virtual void RawSetAttribute(string name, object value)
        {
            if (attributes == null)
                attributes = new Dictionary<string, object>();

            attributes[name] = value;
        }

        /** Find an attr with dynamic scoping up enclosing ST chain.
         *  If not found, look for a map.  So attributes sent in to a template
         *  override dictionary names.
         */
        public virtual object GetAttribute(string name)
        {
            object o = null;
            if (attributes != null)
                attributes.TryGetValue(name, out o);

            if (o != null)
                return o;

            if (code.formalArguments != null &&
                 code.formalArguments.ContainsKey(name) &&  // no local value && it's a formal arg
                 !passThroughAttributes)                 // but no ... in arg list
            {
                // if you've defined attribute as formal arg for this
                // template and it has no value, do not look up the
                // enclosing dynamic scopes.
                return null;
            }

            Template p = this.enclosingInstance;
            while (p != null)
            {
                if (p.attributes != null)
                    p.attributes.TryGetValue(name, out o);
                if (o != null)
                    return o;
                p = p.enclosingInstance;
            }
            if (code.formalArguments == null || !code.formalArguments.ContainsKey(name))
            {
                // if not hidden by formal args, return any dictionary
                return code.nativeGroup.RawGetDictionary(name);
            }
            return null;
        }

        protected static AttributeList ConvertToAttributeList(object curvalue)
        {
            AttributeList multi;
            if (curvalue == null)
            {
                multi = new AttributeList(); // make list to hold multiple values
                multi.Add(curvalue);         // add previous single-valued attribute
            }
            else if (curvalue.GetType() == typeof(AttributeList))
            { // already a list made by ST
                multi = (AttributeList)curvalue;
            }
            else if (curvalue is IList)
            { // existing attribute is non-ST List
                // must copy to an ST-managed list before adding new attribute
                // (can't alter incoming attributes)
                IList listAttr = (IList)curvalue;
                multi = new AttributeList(listAttr.Count);
                multi.AddRange(listAttr);
            }
            else if (curvalue.GetType().IsArray)
            { // copy array to list
                object[] a = (object[])curvalue;
                multi = new AttributeList(a.Length);
                multi.AddRange(a); // asList doesn't copy as far as I can tell
            }
            else
            {
                // curvalue nonlist and we want to add an attribute
                // must convert curvalue existing to list
                multi = new AttributeList(); // make list to hold multiple values
                multi.Add(curvalue);         // add previous single-valued attribute
            }
            return multi;
        }

        /** If an instance of x is enclosed in a y which is in a z, return
         *  a String of these instance names in order from topmost to lowest;
         *  here that would be "[z y x]".
         */
        public virtual string GetEnclosingInstanceStackString()
        {
            IList<Template> templates = GetEnclosingInstanceStack(true);
            StringBuilder builder = new StringBuilder();
            int i = 0;
            foreach (var st in templates)
            {
                if (i > 0)
                    builder.Append(", ");

                builder.Append(st.Name);
                i++;
            }

            return builder.ToString();
        }

        public IList<Template> GetEnclosingInstanceStack(bool topdown)
        {
            var stack = new List<Template>();
            Template p = this;
            while (p != null)
            {
                if (topdown)
                    stack.Insert(0, p);
                else
                    stack.Add(p);

                p = p.enclosingInstance;
            }

            return stack;
        }

        public virtual int Write(ITemplateWriter @out)
        {
            Interpreter interp = new Interpreter(groupThatCreatedThisInstance);
            interp.SetDefaultArguments(this);
            return interp.Exec(@out, this);
        }

        public virtual int Write(ITemplateWriter @out, CultureInfo culture)
        {
            Interpreter interp = new Interpreter(groupThatCreatedThisInstance, culture);
            interp.SetDefaultArguments(this);
            return interp.Exec(@out, this);
        }

        public string Render()
        {
            return Render(CultureInfo.CurrentCulture);
        }

        public string Render(int lineWidth)
        {
            return Render(CultureInfo.CurrentCulture, lineWidth);
        }

        public string Render(CultureInfo culture)
        {
            return Render(culture, AutoIndentWriter.NoWrap);
        }

        public virtual string Render(CultureInfo culture, int lineWidth)
        {
            StringWriter @out = new StringWriter();
            ITemplateWriter wr = new AutoIndentWriter(@out);
            wr.SetLineWidth(lineWidth);
            Write(wr, culture);
            return @out.ToString();
        }

        public override string ToString()
        {
            if (code == null)
                return "bad-template()";

            return code.Name + "()";
        }

        /** &lt;@r()&gt;, &lt;@r&gt;...&lt;@end&gt;, and @t.r() ::= "..." defined manually by coder */
        public enum RegionType
        {
            Implicit,
            Embedded,
            Explicit
        }
    }
}
