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

namespace Antlr4.StringTemplate
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text.RegularExpressions;
    using Antlr4.StringTemplate.Compiler;
    using Antlr4.StringTemplate.Misc;
    using ArgumentException = System.ArgumentException;
    using ArgumentNullException = System.ArgumentNullException;
    using Array = System.Array;
    using Console = System.Console;
    using CultureInfo = System.Globalization.CultureInfo;
    using IList = System.Collections.IList;
    using StringBuilder = System.Text.StringBuilder;
    using StringWriter = System.IO.StringWriter;
    using TextWriter = System.IO.TextWriter;

    /** An instance of the StringTemplate. It consists primarily of
     *  a reference to its implementation (shared among all instances)
     *  and a hash table of attributes.  Because of dynamic scoping,
     *  we also need a reference to any enclosing instance. For example,
     *  in a deeply nested template for an HTML page body, we could still reference
     *  the title attribute defined in the outermost page template.
     *
     *  To use templates, you create one (usually via TemplateGroup) and then inject
     *  attributes using Add(). To Render its attacks, use Render().
     */
    public class Template
    {
        /** <@r()>, <@r>...<@end>, and @t.r() ::= "..." defined manually by coder */
        public enum RegionType
        {
            Implicit,
            Embedded,
            Explicit
        }

        public static readonly string UnknownName = "anonymous";
        public static readonly object EmptyAttribute = new object();

        /** The implementation for this template among all instances of same tmpelate . */
        public CompiledTemplate impl;

        /** Safe to simultaneously Write via Add, which is synchronized.  Reading
         *  during exec is, however, NOT synchronized.  So, not thread safe to
         *  Add attributes while it is being evaluated.  Initialized to EmptyAttribute
         *  to distinguish null from empty.
         */
        protected internal object[] locals;

        /** Enclosing instance if I'm embedded within another template.
         *  IF-subtemplates are considered embedded as well. We look up
         *  dynamically scoped attributes with this ptr.
         */
        private Template _enclosingInstance; // who's your daddy?

        /** Created as instance of which group? We need this to init interpreter
         *  via Render.  So, we create st and then it needs to know which
         *  group created it for sake of polymorphism:
         *
         *  st = skin1.GetInstanceOf("searchbox");
         *  result = st.Render(); // knows skin1 created it
         *
         *  Say we have a group, g1, with template t and import t and u templates from
         *  another group, g2.  g1.GetInstanceOf("u") finds u in g2 but remembers
         *  that g1 created it.  If u includes t, it should create g1.t not g2.t.
         *
         *   g1 = {t(), u()}
         *   |
         *   v
         *   g2 = {t()}
         */
        public TemplateGroup groupThatCreatedThisInstance;

        /** Just an alias for ArrayList, but this way I can track whether a
         *  list is something Template created or it's an incoming list.
         */
        public sealed class AttributeList : List<object>
        {
            public AttributeList(int capacity)
                : base(capacity)
            {
            }

            public AttributeList()
            {
            }
        }

        /** Used by group creation routine, not by users */
        public Template()
        {
        }

        /** Used to make templates inline in code for simple things like SQL or log records.
         *  No formal args are set and there is no enclosing instance.
         */
        public Template(string template)
            : this(TemplateGroup.defaultGroup, template)
        {
        }

        /** Create Template using non-default delimiters; each one of these will live
         *  in it's own group since you're overriding a default; don't want to
         *  alter TemplateGroup.defaultGroup.
         */
        public Template(string template, char delimiterStartChar, char delimiterStopChar)
            : this(new TemplateGroup(delimiterStartChar, delimiterStopChar), template)
        {
        }

        public Template(TemplateGroup group, string template)
        {
            groupThatCreatedThisInstance = group;
            impl = groupThatCreatedThisInstance.Compile(group.FileName, null,
                                                        null, template, null);
            impl.hasFormalArgs = false;
            impl.name = UnknownName;
            impl.DefineImplicitlyDefinedTemplates(groupThatCreatedThisInstance);
        }

        /** Clone a prototype template for application in MAP operations; copy all fields */
        public Template(Template prototype)
            : this(prototype, false, prototype != null ? prototype.EnclosingInstance : null)
        {
        }

        protected Template(Template prototype, bool shadowLocals, Template enclosingInstance)
        {
            if (prototype == null)
                throw new ArgumentNullException("prototype");

            this.impl = prototype.impl;
            this.locals = shadowLocals || prototype.locals == null ? prototype.locals : (object[])prototype.locals.Clone();
            this.EnclosingInstance = enclosingInstance;
            this.groupThatCreatedThisInstance = prototype.groupThatCreatedThisInstance;
        }

        public Template EnclosingInstance
        {
            get
            {
                return _enclosingInstance;
            }

            set
            {
                _enclosingInstance = value;
            }
        }

        public virtual Template CreateShadow(Template shadowEnclosingInstance)
        {
            return new Template(this, true, shadowEnclosingInstance);
        }

        /** Inject an attribute (name/value pair). If there is already an
         *  attribute with that name, this method turns the attribute into an
         *  AttributeList with both the previous and the new attribute as elements.
         *  This method will never alter a List that you inject.  If you send
         *  in a List and then inject a single value element, Add() copies
         *  original list and adds the new value.
         */
        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual void Add(string name, object value)
        {
            if (name == null)
                return; // allow null value

            if (name.IndexOf('.') >= 0)
            {
                throw new ArgumentException("cannot have '.' in attribute names");
            }

            FormalArgument arg = null;
            if (impl.hasFormalArgs)
            {
                arg = impl.TryGetFormalArgument(name);
                if (arg == null)
                    throw new ArgumentException("no such attribute: " + name);
            }
            else
            {
                // define and make room in locals (a hack to make new Template("simple template") work.)
                arg = impl.TryGetFormalArgument(name);
                if (arg == null)
                {
                    // not defined
                    arg = new FormalArgument(name);
                    impl.AddArgument(arg);
                    if (locals == null)
                        locals = new object[1];
                    else
                        Array.Resize(ref locals, impl.formalArguments.Count);

                    locals[arg.Index] = EmptyAttribute;
                }
            }

            object curvalue = locals[arg.Index];
            if (curvalue == EmptyAttribute)
            {
                // new attribute
                locals[arg.Index] = value;
                return;
            }

            // attribute will be multi-valued for sure now
            // convert current attribute to list if not already
            // copy-on-Write semantics; copy a list injected by user to Add new value
            AttributeList multi = ConvertToAttributeList(curvalue);
            locals[arg.Index] = multi; // replace with list

            // now, Add incoming value to multi-valued attribute
            IList list = value as IList;
            if (list != null)
            {
                // flatten incoming list into existing list
                multi.AddRange(list.Cast<object>());
            }
            else
            {
                multi.Add(value);
            }
        }

        /** Remove an attribute value entirely (can't Remove attribute definitions). */
        public virtual void Remove(string name)
        {
            if (impl.formalArguments == null)
            {
                if (impl.hasFormalArgs)
                    throw new ArgumentException("no such attribute: " + name);

                return;
            }

            FormalArgument arg = impl.TryGetFormalArgument(name);
            if (arg == null)
                throw new ArgumentException("no such attribute: " + name);

            locals[arg.Index] = EmptyAttribute; // reset value
        }

        /** Set this.locals attr value when you only know the name, not the index.
         *  This is ultimately invoked by calling Template.Add() from outside so toss
         *  an exception to notify them.
         */
        protected internal virtual void RawSetAttribute(string name, object value)
        {
            if (impl.formalArguments == null)
                throw new ArgumentException("no such attribute: " + name);

            FormalArgument arg = impl.TryGetFormalArgument(name);
            if (arg == null)
                throw new ArgumentException("no such attribute: " + name);

            locals[arg.Index] = value;
        }

        /** Find an attr via dynamic scoping up enclosing Template chain.
         *  If not found, look for a map.  So attributes sent in to a template
         *  override dictionary names.
         */
        public virtual object GetAttribute(string name)
        {
            Template p = this;
            while (p != null)
            {
                FormalArgument localArg = p.impl.TryGetFormalArgument(name);
                if (localArg != null)
                {
                    object o = p.locals[localArg.Index];
                    if (o == Template.EmptyAttribute)
                        o = null;
                    return o;
                }

                p = p.EnclosingInstance;
            }
            // got to root template and no definition, try dictionaries in group
            if (impl.nativeGroup.IsDictionary(name))
            {
                return impl.nativeGroup.RawGetDictionary(name);
            }

            throw new TemplateNoSuchPropertyException(name);
        }

        public virtual IDictionary<string, object> GetAttributes()
        {
            if (impl.formalArguments == null)
                return null;

            IDictionary<string, object> attributes = new Dictionary<string, object>();
            foreach (FormalArgument a in impl.formalArguments)
            {
                object o = locals[a.Index];
                if (o == Template.EmptyAttribute)
                    o = null;

                attributes[a.Name] = o;
            }

            return attributes;
        }

        protected static AttributeList ConvertToAttributeList(object curvalue)
        {
            AttributeList multi;
            if (curvalue == null)
            {
                multi = new AttributeList(); // make list to hold multiple values
                multi.Add(curvalue);                 // Add previous single-valued attribute
            }
            else if (curvalue.GetType() == typeof(AttributeList))
            {
                // already a list made by Template
                multi = (AttributeList)curvalue;
            }
            else if (curvalue is IList)
            {
                // existing attribute is non-Template List
                // must copy to an Template-managed list before adding new attribute
                // (can't alter incoming attributes)
                IList listAttr = (IList)curvalue;
                multi = new AttributeList(listAttr.Count);
                multi.AddRange(listAttr.Cast<object>());
            }
            else if (curvalue.GetType().IsArray)
            { // copy array to list
                object[] a = (object[])curvalue;
                multi = new AttributeList(a.Length);
                multi.AddRange(a); // asList doesn't copy as far as I can tell
            }
            else
            {
                // curvalue nonlist and we want to Add an attribute
                // must convert curvalue existing to list
                multi = new AttributeList(); // make list to hold multiple values
                multi.Add(curvalue);                 // Add previous single-valued attribute
            }
            return multi;
        }

        /** If an instance of x is enclosed in a y which is in a z, return
         *  a String of these instance names in order from topmost to lowest;
         *  here that would be "[z y x]".
         */
        public virtual string GetEnclosingInstanceStackString()
        {
            List<Template> templates = GetEnclosingInstanceStack(true);
            StringBuilder buf = new StringBuilder();
            int i = 0;
            foreach (Template st in templates)
            {
                if (i > 0)
                    buf.Append(" ");
                buf.Append(st.Name);
                i++;
            }

            return buf.ToString();
        }

        public virtual List<Template> GetEnclosingInstanceStack(bool topdown)
        {
            List<Template> stack = new List<Template>();
            Template p = this;
            while (p != null)
            {
                if (topdown)
                    stack.Insert(0, p);
                else
                    stack.Add(p);

                p = p.EnclosingInstance;
            }
            return stack;
        }

        public virtual string Name
        {
            get
            {
                return impl.name;
            }
        }

        public virtual bool IsAnonymousSubtemplate
        {
            get
            {
                return impl.isAnonSubtemplate;
            }
        }

        public virtual int Write(ITemplateWriter @out)
        {
            Interpreter interp = new Interpreter(groupThatCreatedThisInstance, impl.nativeGroup.ErrorManager);
            interp.SetDefaultArguments(this);
            return interp.Execute(@out, this);
        }

        public virtual int Write(ITemplateWriter @out, CultureInfo locale)
        {
            Interpreter interp = new Interpreter(groupThatCreatedThisInstance, locale, impl.nativeGroup.ErrorManager);
            interp.SetDefaultArguments(this);
            return interp.Execute(@out, this);
        }

        public virtual int Write(ITemplateWriter @out, ITemplateErrorListener listener)
        {
            Interpreter interp = new Interpreter(groupThatCreatedThisInstance, new ErrorManager(listener));
            interp.SetDefaultArguments(this);
            return interp.Execute(@out, this);
        }

        public virtual int Write(ITemplateWriter @out, CultureInfo locale, ITemplateErrorListener listener)
        {
            Interpreter interp = new Interpreter(groupThatCreatedThisInstance, locale, new ErrorManager(listener));
            interp.SetDefaultArguments(this);
            return interp.Execute(@out, this);
        }

        public virtual int Write(TextWriter writer, ITemplateErrorListener listener)
        {
            return Write(writer, CultureInfo.CurrentCulture, listener, AutoIndentWriter.NoWrap);
        }

        public virtual int Write(TextWriter writer, ITemplateErrorListener listener, int lineWidth)
        {
            return Write(writer, CultureInfo.CurrentCulture, listener, lineWidth);
        }

        public virtual int Write(TextWriter writer, CultureInfo culture, ITemplateErrorListener listener, int lineWidth)
        {
            ITemplateWriter templateWriter = new AutoIndentWriter(writer)
            {
                LineWidth = lineWidth
            };

            return Write(templateWriter, culture, listener);
        }

        public virtual string Render()
        {
            return Render(CultureInfo.CurrentCulture);
        }

        public virtual string Render(int lineWidth)
        {
            return Render(CultureInfo.CurrentCulture, lineWidth);
        }

        public virtual string Render(CultureInfo locale)
        {
            return Render(locale, AutoIndentWriter.NoWrap);
        }

        public virtual string Render(CultureInfo locale, int lineWidth)
        {
            StringWriter @out = new StringWriter();
            ITemplateWriter wr = new AutoIndentWriter(@out);
            wr.LineWidth = lineWidth;
            Write(wr, locale);
            return @out.ToString();
        }

        public override string ToString()
        {
            if (impl == null)
                return "bad-template()";

            string args = string.Empty;
            if (impl.formalArguments != null)
                args = string.Join(",", impl.formalArguments.Select(i => i.Name).ToArray());

            return string.Format("{0}({1})", Name, args);
        }

        // Template.Format("name, phone | <name>:<phone>", n, p);
        // Template.Format("<%1>:<%2>", n, p);
        // Template.Format("<name>:<phone>", "name", x, "phone", y);
        public static string Format(string template, params object[] attributes)
        {
            return Format(AutoIndentWriter.NoWrap, template, attributes);
        }

        public static string Format(int lineWidth, string template, params object[] attributes)
        {
            template = Regex.Replace(template, "[0-9]+", @"arg\0");
            Console.WriteLine(template);

            Template st = new Template(template);
            int i = 1;
            foreach (object a in attributes)
            {
                st.Add("arg" + i, a);
                i++;
            }
            return st.Render(lineWidth);
        }
    }
}
