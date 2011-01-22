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
    using Array = System.Array;
    using Console = System.Console;
    using CultureInfo = System.Globalization.CultureInfo;
    using IList = System.Collections.IList;
    using StringBuilder = System.Text.StringBuilder;
    using StringWriter = System.IO.StringWriter;

    /** An instance of the StringTemplate. It consists primarily of
     *  a reference to its implementation (shared among all instances)
     *  and a hash table of attributes.  Because of dynamic scoping,
     *  we also need a reference to any enclosing instance. For example,
     *  in a deeply nested template for an HTML page body, we could still reference
     *  the title attribute defined in the outermost page template.
     *
     *  To use templates, you create one (usually via STGroup) and then inject
     *  attributes using add(). To render its attacks, use render().
     */
    public class ST
    {
        /** <@r()>, <@r>...<@end>, and @t.r() ::= "..." defined manually by coder */
        public enum RegionType
        {
            Implicit,
            Embedded,
            Explicit
        }

        public static readonly string UNKNOWN_NAME = "anonymous";
        public static readonly object EMPTY_ATTR = new object();

        /** The implementation for this template among all instances of same tmpelate . */
        public CompiledST impl;

        /** Safe to simultaneously write via add, which is synchronized.  Reading
         *  during exec is, however, NOT synchronized.  So, not thread safe to
         *  add attributes while it is being evaluated.  Initialized to EMPTY_ATTR
         *  to distinguish null from empty.
         */
        protected internal object[] locals;

        /** Enclosing instance if I'm embedded within another template.
         *  IF-subtemplates are considered embedded as well. We look up
         *  dynamically scoped attributes with this ptr.
         */
        public ST enclosingInstance; // who's your daddy?

        /** Created as instance of which group? We need this to init interpreter
         *  via render.  So, we create st and then it needs to know which
         *  group created it for sake of polymorphism:
         *
         *  st = skin1.getInstanceOf("searchbox");
         *  result = st.render(); // knows skin1 created it
         *
         *  Say we have a group, g1, with template t and import t and u templates from
         *  another group, g2.  g1.getInstanceOf("u") finds u in g2 but remembers
         *  that g1 created it.  If u includes t, it should create g1.t not g2.t.
         *
         *   g1 = {t(), u()}
         *   |
         *   v
         *   g2 = {t()}
         */
        public STGroup groupThatCreatedThisInstance;

        /** Just an alias for ArrayList, but this way I can track whether a
         *  list is something ST created or it's an incoming list.
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
        public ST()
        {
        }

        /** Used to make templates inline in code for simple things like SQL or log records.
         *  No formal args are set and there is no enclosing instance.
         */
        public ST(string template)
            : this(STGroup.defaultGroup, template)
        {
        }

        /** Create ST using non-default delimiters; each one of these will live
         *  in it's own group since you're overriding a default; don't want to
         *  alter STGroup.defaultGroup.
         */
        public ST(string template, char delimiterStartChar, char delimiterStopChar)
            : this(new STGroup(delimiterStartChar, delimiterStopChar), template)
        {
        }

        public ST(STGroup group, string template)
        {
            groupThatCreatedThisInstance = group;
            impl = groupThatCreatedThisInstance.compile(group.getFileName(), null,
                                                        null, template, null);
            impl.hasFormalArgs = false;
            impl.name = UNKNOWN_NAME;
            impl.defineImplicitlyDefinedTemplates(groupThatCreatedThisInstance);
        }

        /** Clone a prototype template for application in MAP operations; copy all fields */
        public ST(ST proto)
        {
            this.impl = proto.impl;
            if (proto.locals != null)
                this.locals = (object[])proto.locals.Clone();

            this.enclosingInstance = proto.enclosingInstance;
            this.groupThatCreatedThisInstance = proto.groupThatCreatedThisInstance;
        }

        /** Inject an attribute (name/value pair). If there is already an
         *  attribute with that name, this method turns the attribute into an
         *  AttributeList with both the previous and the new attribute as elements.
         *  This method will never alter a List that you inject.  If you send
         *  in a List and then inject a single value element, add() copies
         *  original list and adds the new value.
         */
        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual void add(string name, object value)
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
                // define and make room in locals (a hack to make new ST("simple template") work.)
                arg = impl.TryGetFormalArgument(name);
                if (arg == null)
                {
                    // not defined
                    arg = new FormalArgument(name);
                    impl.addArg(arg);
                    if (locals == null)
                        locals = new object[1];
                    else
                        Array.Resize(ref locals, impl.formalArguments.Count);

                    locals[arg.Index] = EMPTY_ATTR;
                }
            }

            if (value is ST)
                ((ST)value).enclosingInstance = this;

            object curvalue = locals[arg.Index];
            if (curvalue == EMPTY_ATTR)
            {
                // new attribute
                locals[arg.Index] = value;
                return;
            }

            // attribute will be multi-valued for sure now
            // convert current attribute to list if not already
            // copy-on-write semantics; copy a list injected by user to add new value
            AttributeList multi = convertToAttributeList(curvalue);
            locals[arg.Index] = multi; // replace with list

            // now, add incoming value to multi-valued attribute
            if (value is IList)
            {
                // flatten incoming list into existing list
                multi.AddRange(((IList)value).Cast<object>());
            }
            else if (value != null && value.GetType().IsArray)
            {
                multi.AddRange(((Array)value).Cast<object>());
            }
            else
            {
                multi.Add(value);
            }
        }

        /** Remove an attribute value entirely (can't remove attribute definitions). */
        public virtual void remove(string name)
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

            locals[arg.Index] = EMPTY_ATTR; // reset value
        }

        /** Set this.locals attr value when you only know the name, not the index.
         *  This is ultimately invoked by calling ST.add() from outside so toss
         *  an exception to notify them.
         */
        protected internal virtual void rawSetAttribute(string name, object value)
        {
            if (impl.formalArguments == null)
                throw new ArgumentException("no such attribute: " + name);

            FormalArgument arg = impl.TryGetFormalArgument(name);
            if (arg == null)
                throw new ArgumentException("no such attribute: " + name);

            locals[arg.Index] = value;
        }

        /** Find an attr via dynamic scoping up enclosing ST chain.
         *  If not found, look for a map.  So attributes sent in to a template
         *  override dictionary names.
         */
        public virtual object getAttribute(string name)
        {
            ST p = this;
            while (p != null)
            {
                FormalArgument localArg = p.impl.TryGetFormalArgument(name);
                if (localArg != null)
                {
                    object o = p.locals[localArg.Index];
                    if (o == ST.EMPTY_ATTR)
                        o = null;
                    return o;
                }

                p = p.enclosingInstance;
            }
            // got to root template and no definition, try dictionaries in group
            if (impl.nativeGroup.isDictionary(name))
            {
                return impl.nativeGroup.rawGetDictionary(name);
            }

            throw new STNoSuchPropertyException(name);
        }

        public virtual IDictionary<string, object> getAttributes()
        {
            if (impl.formalArguments == null)
                return null;

            IDictionary<string, object> attributes = new Dictionary<string, object>();
            foreach (FormalArgument a in impl.formalArguments)
            {
                object o = locals[a.Index];
                if (o == ST.EMPTY_ATTR)
                    o = null;

                attributes[a.Name] = o;
            }

            return attributes;
        }

        protected static AttributeList convertToAttributeList(object curvalue)
        {
            AttributeList multi;
            if (curvalue == null)
            {
                multi = new AttributeList(); // make list to hold multiple values
                multi.Add(curvalue);                 // add previous single-valued attribute
            }
            else if (curvalue.GetType() == typeof(AttributeList))
            {
                // already a list made by ST
                multi = (AttributeList)curvalue;
            }
            else if (curvalue is IList)
            {
                // existing attribute is non-ST List
                // must copy to an ST-managed list before adding new attribute
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
                // curvalue nonlist and we want to add an attribute
                // must convert curvalue existing to list
                multi = new AttributeList(); // make list to hold multiple values
                multi.Add(curvalue);                 // add previous single-valued attribute
            }
            return multi;
        }

        /** If an instance of x is enclosed in a y which is in a z, return
         *  a String of these instance names in order from topmost to lowest;
         *  here that would be "[z y x]".
         */
        public virtual string getEnclosingInstanceStackString()
        {
            List<ST> templates = getEnclosingInstanceStack(true);
            StringBuilder buf = new StringBuilder();
            int i = 0;
            foreach (ST st in templates)
            {
                if (i > 0)
                    buf.Append(" ");
                buf.Append(st.getName());
                i++;
            }

            return buf.ToString();
        }

        public virtual List<ST> getEnclosingInstanceStack(bool topdown)
        {
            List<ST> stack = new List<ST>();
            ST p = this;
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

        public virtual string getName()
        {
            return impl.name;
        }

        public virtual bool isAnonSubtemplate()
        {
            return impl.isAnonSubtemplate;
        }

        public virtual int write(STWriter @out)
        {
            Interpreter interp = new Interpreter(groupThatCreatedThisInstance, impl.nativeGroup.errMgr);
            interp.setDefaultArguments(this);
            return interp.Execute(@out, this);
        }

        public virtual int write(STWriter @out, CultureInfo locale)
        {
            Interpreter interp = new Interpreter(groupThatCreatedThisInstance, locale, impl.nativeGroup.errMgr);
            interp.setDefaultArguments(this);
            return interp.Execute(@out, this);
        }

        public virtual int write(STWriter @out, STErrorListener listener)
        {
            Interpreter interp = new Interpreter(groupThatCreatedThisInstance, new ErrorManager(listener));
            interp.setDefaultArguments(this);
            return interp.Execute(@out, this);
        }

        public virtual int write(STWriter @out, CultureInfo locale, STErrorListener listener)
        {
            Interpreter interp = new Interpreter(groupThatCreatedThisInstance, locale, new ErrorManager(listener));
            interp.setDefaultArguments(this);
            return interp.Execute(@out, this);
        }

        public virtual string render()
        {
            return render(CultureInfo.CurrentCulture);
        }

        public virtual string render(int lineWidth)
        {
            return render(CultureInfo.CurrentCulture, lineWidth);
        }

        public virtual string render(CultureInfo locale)
        {
            return render(locale, AutoIndentWriter.NO_WRAP);
        }

        public virtual string render(CultureInfo locale, int lineWidth)
        {
            StringWriter @out = new StringWriter();
            STWriter wr = new AutoIndentWriter(@out);
            wr.setLineWidth(lineWidth);
            write(wr, locale);
            return @out.ToString();
        }

        public override string ToString()
        {
            if (impl == null)
                return "bad-template()";

            return impl.name + "()";
        }

        // ST.format("name, phone | <name>:<phone>", n, p);
        // ST.format("<%1>:<%2>", n, p);
        // ST.format("<name>:<phone>", "name", x, "phone", y);
        public static string format(string template, params object[] attributes)
        {
            return format(AutoIndentWriter.NO_WRAP, template, attributes);
        }

        public static string format(int lineWidth, string template, params object[] attributes)
        {
            template = Regex.Replace(template, "[0-9]+", @"arg\0");
            Console.WriteLine(template);

            ST st = new ST(template);
            int i = 1;
            foreach (object a in attributes)
            {
                st.add("arg" + i, a);
                i++;
            }
            return st.render(lineWidth);
        }
    }
}
