/*
 * [The "BSD license"]
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
    using Antlr4.StringTemplate.Debug;
    using Antlr4.StringTemplate.Extensions;
    using Antlr4.StringTemplate.Misc;
    using ArgumentException = System.ArgumentException;
    using ArgumentNullException = System.ArgumentNullException;
    using Array = System.Array;
    using Console = System.Console;
    using CultureInfo = System.Globalization.CultureInfo;
    using IList = System.Collections.IList;
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
        /** &lt;@r()&gt;, &lt;@r&gt;...&lt;@end&gt;, and @t.r() ::= "..." defined manually by coder */
        public enum RegionType
        {
            /// <summary>
            /// The region is defined by &lt;@r()&gt;
            /// </summary>
            Implicit,

            /// <summary>
            /// The region is defined by &lt;@r&gt;...&lt;@end&gt;
            /// </summary>
            Embedded,

            /// <summary>
            /// The region is defined by @t.r ::= "..."
            /// </summary>
            Explicit
        }

        public static readonly string UnknownName = "anonymous";
        public static readonly object EmptyAttribute = new object();

        /** When there are no formal args for template t and you map t across
         *  some values, t implicitly gets arg "it".  E.g., "<b>$it$</b>"
         */
        public static readonly string ImplicitArgumentName = "it";

        /** The implementation for this template among all instances of same tmpelate . */
        public CompiledTemplate impl;

        /** Safe to simultaneously Write via Add, which is synchronized.  Reading
         *  during exec is, however, NOT synchronized.  So, not thread safe to
         *  Add attributes while it is being evaluated.  Initialized to EmptyAttribute
         *  to distinguish null from empty.
         */
        protected internal object[] locals;

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
        private TemplateGroup groupThatCreatedThisInstance;

        /** If Interpreter.trackCreationEvents, track creation, add-attr events
         *  for each object. Create this object on first use.
         */
        private TemplateDebugState _debugState;

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
        internal Template(TemplateGroup group)
        {
            if (group == null)
                throw new ArgumentNullException("group");

            this.groupThatCreatedThisInstance = group;

            if (group.TrackCreationEvents)
            {
                if (_debugState == null)
                    _debugState = new TemplateDebugState();
                _debugState.NewTemplateEvent = new ConstructionEvent();
            }
        }

        /** Used to make templates inline in code for simple things like SQL or log records.
         *  No formal args are set and there is no enclosing instance.
         */
        public Template(string template)
            : this(TemplateGroup.DefaultGroup, template)
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
            if (group == null)
                throw new ArgumentNullException("group");

            groupThatCreatedThisInstance = group;
            impl = groupThatCreatedThisInstance.Compile(group.FileName, null,
                                                        null, template, null);
            impl.HasFormalArgs = false;
            impl.Name = UnknownName;
            impl.DefineImplicitlyDefinedTemplates(groupThatCreatedThisInstance);
        }

        /** Clone a prototype template.
         *  Copy all fields except DebugState.
         */
        public Template(Template prototype)
            : this(prototype, false)
        {
        }

        protected Template(Template prototype, bool shadowLocals)
        {
            if (prototype == null)
                throw new ArgumentNullException("prototype");

            this.impl = prototype.impl;

            if (shadowLocals && prototype.locals != null)
                this.locals = (object[])prototype.locals.Clone();
            else if (impl.FormalArguments != null && impl.FormalArguments.Count > 0)
                this.locals = new object[impl.FormalArguments.Count];

            this.groupThatCreatedThisInstance = prototype.groupThatCreatedThisInstance;
        }

        public TemplateDebugState DebugState
        {
            get
            {
                return _debugState;
            }

            set
            {
                _debugState = value;
            }
        }

        public TemplateGroup Group
        {
            get
            {
                return groupThatCreatedThisInstance;
            }

            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                groupThatCreatedThisInstance = value;
            }
        }

        public virtual Template CreateShadow()
        {
            return new Template(this, true);
        }

        /** Inject an attribute (name/value pair). If there is already an
         *  attribute with that name, this method turns the attribute into an
         *  AttributeList with both the previous and the new attribute as elements.
         *  This method will never alter a List that you inject.  If you send
         *  in a List and then inject a single value element, Add() copies
         *  original list and adds the new value.
         *
         *  Return self so we can chain.  t.add("x", 1).add("y", "hi");
         */
        public virtual Template Add(string name, object value)
        {
            lock (this)
            { if (name == null)
                    throw new ArgumentNullException("name");
                if (name.IndexOf('.') >= 0)
                    throw new ArgumentException("cannot have '.' in attribute names");

                if (Group.TrackCreationEvents)
                {
                    if (_debugState == null)
                        _debugState = new TemplateDebugState();
                    _debugState.AddAttributeEvents.Add(name, new AddAttributeEvent(name, value));
                }

                FormalArgument arg = null;
                if (impl.HasFormalArgs)
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
                            Array.Resize(ref locals, impl.FormalArguments.Count);

                        locals[arg.Index] = EmptyAttribute;
                    }
                }

                object curvalue = locals[arg.Index];
                if (curvalue == EmptyAttribute)
                {
                    // new attribute
                    locals[arg.Index] = value;
                    return this;
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

                return this;
            }
        }

        /** Split "aggrName.{propName1,propName2}" into list [propName1,propName2]
         *  and the aggrName. Spaces are allowed around ','.
         */
        public Template AddMany(string aggrSpec, params object[] values)
        {
            lock (this)
            {
                if (aggrSpec == null)
                    throw new ArgumentNullException("aggrSpec");
                if (values == null)
                    throw new ArgumentNullException("values");

                if (values.Length == 0)
                    throw new ArgumentException(string.Format("missing values for aggregate attribute format: {0}", aggrSpec), "aggrSpec");

                int dot = aggrSpec.IndexOf(".{");
                int finalCurly = aggrSpec.IndexOf('}');
                if (dot < 0 || finalCurly < 0)
                    throw new ArgumentException(string.Format("invalid aggregate attribute format: {0}", aggrSpec), "aggrSpec");

                string aggrName = aggrSpec.Substring(0, dot);
                string propString = aggrSpec.Substring(dot + 2, aggrSpec.Length - dot - 3);
                propString = propString.Trim();
                string[] propNames = Arrays.ConvertAll(propString.Split(','), p => p.Trim());
                if (propNames == null || propNames.Length == 0)
                    throw new ArgumentException(string.Format("invalid aggregate attribute format: {0}", aggrSpec), "aggrSpec");

                if (values.Length != propNames.Length)
                    throw new ArgumentException(string.Format("number of properties and values mismatch for aggregate attribute format: {0}", aggrSpec), "aggrSpec");

                int i = 0;
                Aggregate aggr = new Aggregate();
                foreach (string p in propNames)
                {
                    object value = values[i++];
                    aggr[p] = value;
                }

                Add(aggrName, aggr); // now add as usual
                return this;
            }
        }

        /** Remove an attribute value entirely (can't Remove attribute definitions). */
        public virtual void Remove(string name)
        {
            if (impl.FormalArguments == null)
            {
                if (impl.HasFormalArgs)
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
            if (impl.FormalArguments == null)
                throw new ArgumentException("no such attribute: " + name);

            FormalArgument arg = impl.TryGetFormalArgument(name);
            if (arg == null)
                throw new ArgumentException("no such attribute: " + name);

            locals[arg.Index] = value;
        }

        /** Find an attr in this template only.
         */
        public virtual object GetAttribute(string name)
        {
            FormalArgument localArg = impl.TryGetFormalArgument(name);
            if (localArg != null)
            {
                object o = locals[localArg.Index];
                if (o == Template.EmptyAttribute)
                    o = null;
                return o;
            }

            return null;
            //    if (context != null)
            //    {
            //        p = context.Template;
            //        context = context.Parent;
            //    }
            //    else
            //    {
            //        p = null;
            //    }

            //// got to root template and no definition, try dictionaries in group
            //if (impl.NativeGroup.IsDictionary(name))
            //{
            //    return impl.NativeGroup.RawGetDictionary(name);
            //}

            //throw new TemplateNoSuchPropertyException(name);
        }

        public virtual IDictionary<string, object> GetAttributes()
        {
            if (impl.FormalArguments == null)
                return null;

            IDictionary<string, object> attributes = new Dictionary<string, object>();
            foreach (FormalArgument a in impl.FormalArguments)
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
            else
            {
                // curvalue nonlist and we want to Add an attribute
                // must convert curvalue existing to list
                multi = new AttributeList(); // make list to hold multiple values
                multi.Add(curvalue);                 // Add previous single-valued attribute
            }
            return multi;
        }

        public virtual string Name
        {
            get
            {
                return impl.Name;
            }
        }

        public virtual bool IsAnonymousSubtemplate
        {
            get
            {
                return impl.IsAnonSubtemplate;
            }
        }

        public virtual int Write(ITemplateWriter @out)
        {
            Interpreter interp = new Interpreter(Group, impl.NativeGroup.ErrorManager, false);
            TemplateFrame frame = new TemplateFrame(this, null);
            return interp.Execute(@out, frame);
        }

        public virtual int Write(ITemplateWriter @out, CultureInfo culture)
        {
            Interpreter interp = new Interpreter(Group, culture, impl.NativeGroup.ErrorManager, false);
            TemplateFrame frame = new TemplateFrame(this, null);
            return interp.Execute(@out, frame);
        }

        public virtual int Write(ITemplateWriter @out, ITemplateErrorListener listener)
        {
            Interpreter interp = new Interpreter(Group, new ErrorManager(listener), false);
            TemplateFrame frame = new TemplateFrame(this, null);
            return interp.Execute(@out, frame);
        }

        public virtual int Write(ITemplateWriter @out, CultureInfo culture, ITemplateErrorListener listener)
        {
            Interpreter interp = new Interpreter(Group, culture, new ErrorManager(listener), false);
            TemplateFrame frame = new TemplateFrame(this, null);
            return interp.Execute(@out, frame);
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

        public virtual string Render(CultureInfo culture)
        {
            return Render(culture, AutoIndentWriter.NoWrap);
        }

        public virtual string Render(CultureInfo culture, int lineWidth)
        {
            StringWriter @out = new StringWriter();
            ITemplateWriter wr = new AutoIndentWriter(@out);
            wr.LineWidth = lineWidth;
            Write(wr, culture);
            return @out.ToString();
        }

        // TESTING SUPPORT

        public virtual List<InterpEvent> GetEvents()
        {
            return GetEvents(CultureInfo.CurrentCulture);
        }

        public virtual List<InterpEvent> GetEvents(int lineWidth)
        {
            return GetEvents(CultureInfo.CurrentCulture, lineWidth);
        }

        public virtual List<InterpEvent> GetEvents(ITemplateWriter writer)
        {
            return GetEvents(CultureInfo.CurrentCulture, writer);
        }

        public virtual List<InterpEvent> GetEvents(CultureInfo locale)
        {
            return GetEvents(locale, AutoIndentWriter.NoWrap);
        }

        public virtual List<InterpEvent> GetEvents(CultureInfo locale, int lineWidth)
        {
            StringWriter @out = new StringWriter();
            ITemplateWriter wr = new AutoIndentWriter(@out);
            wr.LineWidth = lineWidth;
            return GetEvents(locale, wr);
        }

        public virtual List<InterpEvent> GetEvents(CultureInfo culture, ITemplateWriter writer)
        {
            Interpreter interp = new Interpreter(Group, culture, true);
            TemplateFrame frame = new TemplateFrame(this, null);
            interp.Execute(writer, frame); // Render and track events
            return interp.GetEvents();
        }

        public override string ToString()
        {
            if (impl == null)
                return "bad-template()";

            string args = string.Empty;
            if (impl.FormalArguments != null)
                args = string.Join(",", impl.FormalArguments.Select(i => i.Name).ToArray());

            string name = Name;
            if (this.impl.IsRegion)
                name = "@" + TemplateGroup.GetUnmangledTemplateName(name);

            return string.Format("{0}({1})", name, args);
        }

        // Template.Format("<%1>:<%2>", n, p);
        public static string Format(string template, params object[] attributes)
        {
            return Format(AutoIndentWriter.NoWrap, template, attributes);
        }

        public static string Format(int lineWidth, string template, params object[] attributes)
        {
            template = Regex.Replace(template, "[0-9]+", "arg$0");
            Template st = new Template(template);
            int i = 1;
            foreach (object a in attributes)
            {
                st.Add("arg" + i, a);
                i++;
            }
            return st.Render(lineWidth);
        }

        /** Events during template hierarchy construction (not evaluation) */
        public class TemplateDebugState
        {
            /** Record who made us? ConstructionEvent creates Exception to grab stack */
            public ConstructionEvent NewTemplateEvent;

            /** Track construction-time add attribute "events"; used for ST user-level debugging */
            public MultiMap<string, AddAttributeEvent> AddAttributeEvents = new MultiMap<string, AddAttributeEvent>();
        }
    }
}
