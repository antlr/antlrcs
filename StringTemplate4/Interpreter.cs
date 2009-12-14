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
    using System.Linq;
    using StringTemplate.Compiler;
    using StringTemplate.Debug;
    using Array = System.Array;
    using ArrayList = System.Collections.ArrayList;
    using Console = System.Console;
    using CultureInfo = System.Globalization.CultureInfo;
    using Environment = System.Environment;
    using Exception = System.Exception;
    using FieldInfo = System.Reflection.FieldInfo;
    using ICollection = System.Collections.ICollection;
    using IDictionary = System.Collections.IDictionary;
    using IList = System.Collections.IList;
    using IOException = System.IO.IOException;
    using Iterator = System.Collections.IEnumerator;
    using Math = System.Math;
    using MethodInfo = System.Reflection.MethodInfo;
    using StringBuilder = System.Text.StringBuilder;
    using StringWriter = System.IO.StringWriter;
    using Type = System.Type;

    public class Interpreter
    {
        // TODO: enum?
        public static readonly int OPTION_ANCHOR = 0;
        public static readonly int OPTION_FORMAT = 1;
        public static readonly int OPTION_NULL = 2;
        public static readonly int OPTION_SEPARATOR = 3;
        public static readonly int OPTION_WRAP = 4;

        public static readonly int DEFAULT_OPERAND_STACK_SIZE = 100;

        public static readonly HashSet<string> predefinedAttributes =
            new HashSet<string>()
        {
            "it",
            "i",
            "i0"
        };

        /** Operand stack, grows upwards */
        object[] operands = new object[DEFAULT_OPERAND_STACK_SIZE];
        int sp = -1;  // stack pointer register
        int nw = 0;   // how many char written on this template line so far? ("number written" register)

        /** Exec st with respect to this group. Once set in Template.toString(),
         *  it should be fixed.  Template has group also.
         */
        TemplateGroup group;

        CultureInfo culture;

        public bool trace = false;

        /// <summary>
        /// Track everything happening in interp if debug
        /// </summary>
        protected internal IList<InterpEvent> events;

        public Interpreter(TemplateGroup group)
            : this(group, CultureInfo.CurrentCulture)
        {
        }

        // TODO: remove out and move back to exec; must avoid creating new interp when deugging same st tree 
        public Interpreter(TemplateGroup group, CultureInfo culture)
        {
            this.group = group;
            this.culture = culture;
            if (group.Debug)
            {
                events = new List<InterpEvent>();
            }
        }

        public IList<InterpEvent> Events
        {
            get
            {
                return events;
            }
        }

        public int Exec(ITemplateWriter @out, Template self)
        {
            int start = @out.Index; // track char we're about to write
            int prevOpcode = 0;
            int n = 0; // how many char we write out
            int nameIndex = 0;
            int addr = 0;
            string name = null;
            object o = null, left = null, right = null;
            Template st = null;
            object[] options = null;
            int ip = 0;
            byte[] code = self.code.instrs;        // which code block are we executing
            while (ip < self.code.codeSize)
            {
                if (trace)
                    Trace(self, ip);
                short opcode = code[ip];
                ip++; //jump to next instruction or first byte of operand
                switch (opcode)
                {
                case Bytecode.INSTR_LOAD_STR:
                    int strIndex = GetShort(code, ip);
                    ip += 2;
                    operands[++sp] = self.code.strings[strIndex];
                    break;
                case Bytecode.INSTR_LOAD_ATTR:
                    nameIndex = GetShort(code, ip);
                    ip += 2;
                    name = self.code.strings[nameIndex];
                    operands[++sp] = self.GetAttribute(name);
                    break;
                case Bytecode.INSTR_LOAD_LOCAL:
                    nameIndex = GetShort(code, ip);
                    ip += 2;
                    name = self.code.strings[nameIndex];
                    if (self.attributes == null || !self.attributes.TryGetValue(name, out o))
                        o = null;
                    operands[++sp] = o;
                    break;
                case Bytecode.INSTR_LOAD_PROP:
                    nameIndex = GetShort(code, ip);
                    ip += 2;
                    o = operands[sp--];
                    name = self.code.strings[nameIndex];
                    operands[++sp] = GetObjectProperty(self, o, name);
                    break;
                case Bytecode.INSTR_LOAD_PROP_IND:
                    object propName = operands[sp--];
                    o = operands[sp];
                    operands[sp] = GetObjectProperty(self, o, propName);
                    break;
                case Bytecode.INSTR_NEW:
                    nameIndex = GetShort(code, ip);
                    ip += 2;
                    name = self.code.strings[nameIndex];
                    st = group.GetEmbeddedInstanceOf(self, name);
                    if (st == null)
                    {
                        ErrorManager.RuntimeError(self, ErrorType.NoSuchTemplate, name);
                        st = Template.Blank;
                    }
                    operands[++sp] = st;
                    break;
                case Bytecode.INSTR_NEW_IND:
                    name = (string)operands[sp--];
                    st = group.GetEmbeddedInstanceOf(self, name);
                    if (st == null)
                    {
                        ErrorManager.RuntimeError(self, ErrorType.NoSuchTemplate, name);
                        st = Template.Blank;
                    }
                    operands[++sp] = st;
                    break;
                case Bytecode.INSTR_SUPER_NEW:
                    nameIndex = GetShort(code, ip);
                    ip += 2;
                    name = self.code.strings[nameIndex];
                    CompiledTemplate imported = group.LookupImportedTemplate(name);
                    if (imported == null)
                    {
                        ErrorManager.RuntimeError(self, ErrorType.NoImportedTemplate, name);
                        operands[++sp] = Template.Blank;
                        break;
                    }
                    // TODO: factor into STGroup
                    st = imported.nativeGroup.CreateStringTemplate();
                    st.groupThatCreatedThisInstance = group;
                    st.code = imported;
                    operands[++sp] = st;
                    break;
                case Bytecode.INSTR_STORE_ATTR:
                    nameIndex = GetShort(code, ip);
                    name = self.code.strings[nameIndex];
                    ip += 2;
                    o = operands[sp--];    // value to store
                    st = (Template)operands[sp]; // store arg in Template on top of stack
                    st.RawSetAttribute(name, o);
                    break;
                case Bytecode.INSTR_STORE_SOLE_ARG:
                    // unnamed arg, set to sole arg (or first if multiple)
                    o = operands[sp--];    // value to store
                    st = (Template)operands[sp]; // store arg in Template on top of stack
                    int nargs = 0;
                    if (st.code.formalArguments != null)
                    {
                        nargs = st.code.formalArguments.Count;
                    }
                    if (nargs != 1)
                    {
                        ErrorManager.RuntimeError(self, ErrorType.ExpectingSingleArgument, st, nargs);
                    }
                    else
                    {
                        name = st.code.formalArguments.Keys.First();
                        st.RawSetAttribute(name, o);
                    }
                    break;
                case Bytecode.INSTR_SET_PASS_THRU:
                    st = (Template)operands[sp]; // Template on top of stack
                    st.passThroughAttributes = true;
                    break;
                case Bytecode.INSTR_STORE_OPTION:
                    int optionIndex = GetShort(code, ip);
                    ip += 2;
                    o = operands[sp--];    // value to store
                    options = (object[])operands[sp]; // get options
                    options[optionIndex] = o; // store value into options on stack
                    break;
                case Bytecode.INSTR_WRITE:
                    int exprStart = GetShort(code, ip);
                    ip += 2;
                    int exprStop = GetShort(code, ip);
                    ip += 2;
                    o = operands[sp--];
                    nw = WriteObjectNoOptions(@out, self, o, exprStart, exprStop);
                    n += nw;
                    break;
                case Bytecode.INSTR_WRITE_OPT:
                    exprStart = GetShort(code, ip);
                    ip += 2;
                    exprStop = GetShort(code, ip);
                    ip += 2;
                    options = (object[])operands[sp--]; // get options
                    o = operands[sp--];                 // get option to write
                    nw = WriteObjectWithOptions(@out, self, o, options, exprStart, exprStop);
                    n += nw;
                    break;
                case Bytecode.INSTR_MAP:
                    name = (string)operands[sp--];
                    o = operands[sp--];
                    Map(self, o, name);
                    break;
                case Bytecode.INSTR_ROT_MAP:
                    int nmaps = GetShort(code, ip);
                    ip += 2;
                    List<string> templates = new List<string>();
                    for (int i = nmaps - 1; i >= 0; i--)
                        templates.Add((string)operands[sp - i]);
                    sp -= nmaps;
                    o = operands[sp--];
                    if (o != null)
                        Rot_map(self, o, templates);
                    break;
                case Bytecode.INSTR_PAR_MAP:
                    name = (string)operands[sp--];
                    nmaps = GetShort(code, ip);
                    ip += 2;
                    List<object> exprs = new List<object>();
                    for (int i = nmaps - 1; i >= 0; i--)
                        exprs.Add(operands[sp - i]);
                    sp -= nmaps;
                    operands[++sp] = Par_map(self, exprs, name);
                    break;
                case Bytecode.INSTR_BR:
                    ip = GetShort(code, ip);
                    break;
                case Bytecode.INSTR_BRF:
                    addr = GetShort(code, ip);
                    ip += 2;
                    o = operands[sp--]; // <if(expr)>...<endif>
                    if (!TestAttributeTrue(o))
                        ip = addr; // jump
                    break;
                case Bytecode.INSTR_OPTIONS:
                    operands[++sp] = new object[TemplateCompiler.NUM_OPTIONS];
                    break;
                case Bytecode.INSTR_LIST:
                    operands[++sp] = new List<object>();
                    break;
                case Bytecode.INSTR_ADD:
                    o = operands[sp--];             // pop value
                    List<object> list = (List<object>)operands[sp]; // don't pop list
                    AddToList(list, o);
                    break;
                case Bytecode.INSTR_TOSTR:
                    // replace with string value; early eval
                    operands[sp] = ToString(self, operands[sp]);
                    break;
                case Bytecode.INSTR_FIRST:
                    operands[sp] = First(operands[sp]);
                    break;
                case Bytecode.INSTR_LAST:
                    operands[sp] = Last(operands[sp]);
                    break;
                case Bytecode.INSTR_REST:
                    operands[sp] = Rest(operands[sp]);
                    break;
                case Bytecode.INSTR_TRUNC:
                    operands[sp] = Trunc(operands[sp]);
                    break;
                case Bytecode.INSTR_STRIP:
                    operands[sp] = Strip(operands[sp]); // TODO: should strip work on strings?
                    break;
                case Bytecode.INSTR_TRIM:
                    o = operands[sp--];
                    if (o.GetType() == typeof(string))
                    {
                        operands[++sp] = ((string)o).Trim();
                    }
                    else
                    {
                        ErrorManager.RuntimeError(self, ErrorType.ExpectingString, "trim", o);
                        operands[++sp] = o;
                    }
                    break;
                case Bytecode.INSTR_LENGTH:
                    operands[sp] = Length(operands[sp]);
                    break;
                case Bytecode.INSTR_STRLEN:
                    o = operands[sp--];
                    if (o.GetType() == typeof(string))
                    {
                        operands[++sp] = ((string)o).Length;
                    }
                    else
                    {
                        ErrorManager.RuntimeError(self, ErrorType.ExpectingString, "strlen", o);
                        operands[++sp] = 0;
                    }
                    break;
                case Bytecode.INSTR_REVERSE:
                    operands[sp] = Reverse(operands[sp]);
                    break;
                case Bytecode.INSTR_NOT:
                    operands[sp] = !TestAttributeTrue(operands[sp]);
                    break;
                case Bytecode.INSTR_OR:
                    right = operands[sp--];
                    left = operands[sp--];
                    operands[++sp] = TestAttributeTrue(left) || TestAttributeTrue(right);
                    break;
                case Bytecode.INSTR_AND:
                    right = operands[sp--];
                    left = operands[sp--];
                    operands[++sp] = TestAttributeTrue(left) && TestAttributeTrue(right);
                    break;
                case Bytecode.INSTR_INDENT:
                    strIndex = GetShort(code, ip);
                    ip += 2;
                    @out.PushIndentation(self.code.strings[strIndex]);
                    break;
                case Bytecode.INSTR_DEDENT:
                    @out.PopIndentation();
                    break;
                case Bytecode.INSTR_NEWLINE:
                    try
                    {
                        if (prevOpcode == Bytecode.INSTR_NEWLINE ||
                             prevOpcode == Bytecode.INSTR_INDENT ||
                             nw > 0)
                        {
                            @out.Write(Environment.NewLine);
                        }
                        nw = -1; // indicate nothing written but no WRITE yet
                    }
                    catch (IOException ioe)
                    {
                        ErrorManager.IOError(self, ErrorType.WriteIoError, ioe);
                    }
                    break;
                default:
                    ErrorManager.InternalError(self, ErrorType.InvalidBytecode, null, opcode, ip - 1);
                    self.code.Dump();
                    break;
                }
                prevOpcode = opcode;
            }

            if (group.Debug)
            {
                int stop = @out.Index - 1;
                EvalTemplateEvent e = new EvalTemplateEvent((DebugTemplate)self, start, stop);
                //Console.WriteLine(e);
                events.Add(e);
                if (self.enclosingInstance != null)
                {
                    ((DebugTemplate)self.enclosingInstance).InterpreterEvents.Add(e);
                }
            }

            return n;
        }

        protected int WriteObjectNoOptions(ITemplateWriter @out, Template self, object o, int exprStart, int exprStop)
        {
            int start = @out.Index; // track char we're about to write
            int n = WriteObject(@out, self, o, null);

            if (group.Debug)
            {
                events.Add(new EvalExprEvent((DebugTemplate)self, start, @out.Index - 1, exprStart, exprStop));
            }

            return n;
        }

        protected int WriteObjectWithOptions(ITemplateWriter @out, Template self, object o, object[] options, int exprStart, int exprStop)
        {
            int start = @out.Index; // track char we're about to write
            // precompute all option values (render all the way to strings)
            string[] optionStrings = null;
            if (options != null)
            {
                optionStrings = new string[options.Length];
                for (int i = 0; i < TemplateCompiler.NUM_OPTIONS; i++)
                {
                    optionStrings[i] = ToString(self, options[i]);
                }
            }

            if (options != null && options[OPTION_ANCHOR] != null)
                @out.PushAnchorPoint();

            int n = WriteObject(@out, self, o, optionStrings);

            if (options != null && options[OPTION_ANCHOR] != null)
                @out.PopAnchorPoint();

            if (group.Debug)
            {
                events.Add(new EvalTemplateEvent((DebugTemplate)self, start, @out.Index - 1));
            }

            return n;
        }

        protected int WriteObject(ITemplateWriter @out, Template self, object o, string[] options)
        {
            int n = 0;
            if (o == null)
            {
                if (options != null && options[OPTION_NULL] != null)
                {
                    o = options[OPTION_NULL];
                }
                else
                {
                    return 0;
                }
            }

            if (o is Template)
            {
                ((Template)o).enclosingInstance = self;
                if (options != null && options[OPTION_WRAP] != null)
                {
                    // if we have a wrap string, then inform writer it might need to wrap
                    try
                    {
                        @out.WriteWrap(options[OPTION_WRAP]);
                    }
                    catch (IOException ioe)
                    {
                        ErrorManager.IOError(self, ErrorType.WriteIoError, ioe);
                    }
                }
                n = Exec(@out, (Template)o);
            }
            else
            {
                o = ConvertAnythingIteratableToIterator(o); // normalize
                try
                {
                    if (o is Iterator)
                        n = WriteIterator(@out, self, o, options);
                    else
                        n = WritePlainObject(@out, o, options);
                }
                catch (IOException ioe)
                {
                    ErrorManager.IOError(self, ErrorType.WriteIoError, ioe, o);
                }
            }

            return n;
        }

        protected int WriteIterator(ITemplateWriter @out, Template self, object o, string[] options)
        {
            if (o == null)
                return 0;
            int n = 0;
            Iterator it = (Iterator)o;
            string separator = null;
            if (options != null)
                separator = options[OPTION_SEPARATOR];
            bool seenAValue = false;
            while (it.MoveNext())
            {
                object iterValue = it.Current;
                // Emit separator if we're beyond first value
                bool needSeparator = seenAValue &&
                    separator != null &&            // we have a separator and
                    (iterValue != null ||           // either we have a value
                     options[OPTION_NULL] != null); // or no value but null option
                if (needSeparator)
                    n += @out.WriteSeparator(separator);
                int nw = WriteObject(@out, self, iterValue, options);
                if (nw > 0)
                    seenAValue = true;
                n += nw;
            }
            return n;
        }

        protected int WritePlainObject(ITemplateWriter @out, object o, string[] options)
        {
            string formatString = null;
            if (options != null)
                formatString = options[OPTION_FORMAT];
            IAttributeRenderer r = group.GetAttributeRenderer(o.GetType());
            string v = null;
            if (r != null)
            {
                v = r.ToString(o, formatString, culture);
            }
            else
            {
                v = o.ToString();
            }

            int n = 0;
            if (options != null && options[OPTION_WRAP] != null)
                n = @out.Write(v, options[OPTION_WRAP]);
            else
                n = @out.Write(v);

            return n;
        }

        protected void Map(Template self, object attr, string name)
        {
            Rot_map(self, attr, new List<string>() { name });
        }

        // <names:a,b>
        protected void Rot_map(Template self, object attr, List<string> templates)
        {
            if (attr == null)
            {
                operands[++sp] = null;
                return;
            }
            attr = ConvertAnythingIteratableToIterator(attr);
            if (attr is Iterator)
            {
                List<Template> mapped = new List<Template>();
                Iterator iter = (Iterator)attr;
                int i0 = 0;
                int i = 1;
                int ti = 0;
                while (iter.MoveNext())
                {
                    object iterValue = iter.Current;
                    if (iterValue == null)
                        continue;
                    int templateIndex = ti % templates.Count; // rotate through
                    ti++;
                    string name = templates[templateIndex];
                    Template st = group.GetEmbeddedInstanceOf(self, name);
                    SetSoleArgument(st, iterValue);
                    st.RawSetAttribute("i0", i0);
                    st.RawSetAttribute("i", i);
                    mapped.Add(st);
                    i0++;
                    i++;
                }
                operands[++sp] = mapped;
                //Console.WriteLine("mapped="+mapped);
            }
            else
            { // if only single value, just apply first template to attribute
                Template st = group.GetInstanceOf(templates[0]);
                SetSoleArgument(st, attr);
                st.RawSetAttribute("i0", 0);
                st.RawSetAttribute("i", 1);
                operands[++sp] = st;
                //            map(self, attr, templates.get(1));
            }
        }

        // <names,phones:{n,p | ...}>
        protected Template.AttributeList Par_map(Template self, List<object> exprs, string template)
        {
            if (exprs == null || template == null || exprs.Count == 0)
            {
                return null; // do not apply if missing templates or empty values
            }
            // make everything iterable
            for (int i = 0; i < exprs.Count; i++)
            {
                object attr = exprs[i];
                if (attr != null)
                    exprs[i] = ConvertAnythingToIterator(attr);
            }

            // ensure arguments line up
            int numAttributes = exprs.Count;
            CompiledTemplate code = group.LookupTemplate(template);
            var formalArguments = code.formalArguments;
            if (formalArguments == null || formalArguments.Count == 0)
            {
                ErrorManager.RuntimeError(self, ErrorType.MissingFormalArguments);
                return null;
            }

            object[] formalArgumentNames = formalArguments.Keys.ToArray();
            if (formalArgumentNames.Length != numAttributes)
            {
                ErrorManager.RuntimeError(self, ErrorType.ArgumentCountMismatch, template);
                // truncate arg list to match smaller size
                int shorterSize = Math.Min(formalArgumentNames.Length, numAttributes);
                numAttributes = shorterSize;
                object[] newFormalArgumentNames = new object[shorterSize];
                Array.Copy(formalArgumentNames, 0,
                                 newFormalArgumentNames, 0,
                                 shorterSize);
                formalArgumentNames = newFormalArgumentNames;
            }

            // keep walking while at least one attribute has values

            {
                Template.AttributeList results = new Template.AttributeList();
                int i = 0; // iteration number from 0
                while (true)
                {
                    // get a value for each attribute in list; put into Template instance
                    int numEmpty = 0;
                    Template embedded = group.GetEmbeddedInstanceOf(self, template);
                    embedded.RawSetAttribute("i0", i);
                    embedded.RawSetAttribute("i", i + 1);
                    for (int a = 0; a < numAttributes; a++)
                    {
                        Iterator it = (Iterator)exprs[a];
                        if (it != null && it.MoveNext())
                        {
                            string argName = (string)formalArgumentNames[a];
                            object iteratedValue = it.Current;
                            embedded.RawSetAttribute(argName, iteratedValue);
                        }
                        else
                        {
                            numEmpty++;
                        }
                    }
                    if (numEmpty == numAttributes)
                        break;
                    results.Add(embedded);
                    i++;
                }
                return results;
            }
        }

        protected void SetSoleArgument(Template st, object attr)
        {
            if (st.code.formalArguments != null)
            {
                string arg = st.code.formalArguments.Keys.First();
                st.RawSetAttribute(arg, attr);
            }
            else
            {
                st.RawSetAttribute("it", attr);
            }
        }

        protected void AddToList(List<object> list, object o)
        {
            if (o == null)
                return; // [a,b,c] lists ignore null values
            o = Interpreter.ConvertAnythingIteratableToIterator(o);
            if (o is Iterator)
            {
                // copy of elements into our temp list
                Iterator it = (Iterator)o;
                while (it.MoveNext())
                    list.Add(it.Current);
            }
            else
            {
                list.Add(o);
            }
        }

        /** Return the first attribute if multiple valued or the attribute
         *  itself if single-valued.  Used in <names:first()>
         */
        public object First(object v)
        {
            if (v == null)
                return null;
            object r = v;
            v = ConvertAnythingIteratableToIterator(v);
            if (v is Iterator)
            {
                Iterator it = (Iterator)v;
                if (it.MoveNext())
                {
                    r = it.Current;
                }
            }
            return r;
        }

        /** Return the last attribute if multiple valued or the attribute
         *  itself if single-valued. Unless it's a list or array, this is pretty
         *  slow as it iterates until the last element.
         */
        public object Last(object v)
        {
            if (v == null)
                return null;
            if (v is IList)
                return ((IList)v)[((IList)v).Count - 1];
            else if (v.GetType().IsArray)
            {
                object[] elems = (object[])v;
                return elems[elems.Length - 1];
            }
            object last = v;
            v = ConvertAnythingIteratableToIterator(v);
            if (v is Iterator)
            {
                Iterator it = (Iterator)v;
                while (it.MoveNext())
                {
                    last = it.Current;
                }
            }
            return last;
        }

        /** Return everything but the first attribute if multiple valued
         *  or null if single-valued.
         */
        public object Rest(object v)
        {
            if (v == null)
                return null;
            if (v is IList)
            { // optimize list case
                IList elems = (IList)v;
                if (elems.Count <= 1)
                    return null;
                return elems.Cast<object>().Skip(1).ToArray();
            }
            object theRest = v; // else iterate and copy 
            v = ConvertAnythingIteratableToIterator(v);
            if (v is Iterator)
            {
                IList a = new ArrayList();
                Iterator it = (Iterator)v;
                if (!it.MoveNext())
                    return null; // if not even one value return null
                //it.Current; // ignore first value
                while (it.MoveNext())
                {
                    object o = it.Current;
                    if (o != null)
                        a.Add(o);
                }
                return a;
            }
            else
            {
                theRest = null;  // rest of single-valued attribute is null
            }
            return theRest;
        }

        /** Return all but the last element.  trunc(x)=null if x is single-valued. */
        public object Trunc(object v)
        {
            if (v == null)
                return null;
            if (v is IList)
            {
                // optimize list case
                IList elems = (IList)v;
                if (elems.Count <= 1)
                    return null;
                return elems.Cast<object>().Take(elems.Count - 1).ToArray();
            }
            v = ConvertAnythingIteratableToIterator(v);
            if (v is Iterator)
            {
                IList a = new ArrayList();
                Iterator it = (Iterator)v;
                if (it.MoveNext())
                {
                    object previous = it.Current;
                    while (it.MoveNext())
                    {
                        a.Add(previous);
                        previous = it.Current;
                    }
                }
                return a;
            }
            return null; // trunc(x)==null when x single-valued attribute
        }

        /** Return a new list w/o null values. */
        public object Strip(object v)
        {
            if (v == null)
                return null;
            v = ConvertAnythingIteratableToIterator(v);
            if (v is Iterator)
            {
                IList a = new ArrayList();
                Iterator it = (Iterator)v;
                while (it.MoveNext())
                {
                    object o = (object)it.Current;
                    if (o != null)
                        a.Add(o);
                }
                return a;
            }
            return v; // strip(x)==x when x single-valued attribute
        }

        /** Return a list with the same elements as v but in reverse order. null
         *  values are NOT stripped out. use reverse(strip(v)) to do that.
         */
        public object Reverse(object v)
        {
            if (v == null)
                return null;
            v = ConvertAnythingIteratableToIterator(v);
            if (v is Iterator)
            {
                IList a = new List<object>();
                Iterator it = (Iterator)v;
                while (it.MoveNext())
                    a.Insert(0, it.Current);
                return a;
            }
            return v;
        }

        /** Return the length of a mult-valued attribute or 1 if it is a
         *  single attribute. If attribute is null return 0.
         *  Special case several common collections and primitive arrays for
         *  speed. This method by Kay Roepke from v3.
         */
        public object Length(object v)
        {
            if (v == null)
                return 0;
            int i = 1;      // we have at least one of something. Iterator and arrays might be empty.
            if (v is IDictionary)
                i = ((IDictionary)v).Count;
            else if (v is ICollection)
                i = ((ICollection)v).Count;
            else if (v is object[])
                i = ((object[])v).Length;
            else if (v is string[])
                i = ((string[])v).Length;
            else if (v is int[])
                i = ((int[])v).Length;
            else if (v is long[])
                i = ((long[])v).Length;
            else if (v is float[])
                i = ((float[])v).Length;
            else if (v is double[])
                i = ((double[])v).Length;
            else if (v is Iterator)
            {
                Iterator it = (Iterator)v;
                i = 0;
                while (it.MoveNext())
                {
                    i++;
                }
            }
            return i;
        }

        public object Strlen(object v)
        {
            //return null;
            // TODO: impl
            throw new System.NotImplementedException();
        }

        protected string ToString(Template self, object value)
        {
            if (value != null)
            {
                string s = value as string;
                if (s != null)
                    return s;

                // if Template, make sure it evaluates with enclosing template as self
                Template t = value as Template;
                if (t != null)
                    t.enclosingInstance = self;

                // if not string already, must evaluate it
                StringWriter sw = new StringWriter();
                //Interpreter interp = new Interpreter(group, new NoIndentWriter(sw), culture);
                //interp.WriteObjectNoOptions(self, value, -1, -1);
                WriteObjectNoOptions(new NoIndentWriter(sw), self, value, -1, -1);
                return sw.ToString();
            }
            return null;
        }

        protected static object ConvertAnythingIteratableToIterator(object o)
        {
            Iterator iter = null;
            if (o == null)
                return null;
            else if (o is IDictionary)
                iter = ((IDictionary)o).Values.GetEnumerator();
            if (o is ICollection)
                iter = ((ICollection)o).GetEnumerator();
            //else if (o.GetType().IsArray)
            //    iter = new ArrayIterator(o);
            else if (o is Iterator)
                iter = (Iterator)o;
            if (iter == null)
                return o;
            return iter;
        }

        protected static Iterator ConvertAnythingToIterator(object o)
        {
            o = ConvertAnythingIteratableToIterator(o);
            if (o is Iterator)
                return (Iterator)o;
            IList singleton = new Template.AttributeList(1);
            singleton.Add(o);
            return singleton.GetEnumerator();
        }

        protected bool TestAttributeTrue(object a)
        {
            if (a == null)
                return false;
            if (a is bool)
                return (bool)a;
            if (a is ICollection)
                return ((ICollection)a).Count > 0;
            //if (a is IDictionary)
            //    return ((IDictionary)a).Count > 0;
            //if (a is Iterator)
            //    return ((Iterator)a).hasNext();
            return true; // any other non-null object, return true--it's present
        }

        protected object GetObjectProperty(Template self, object o, object property)
        {
            if (o == null || property == null)
            {
                // TODO: throw Ill arg if they want
                return null;
            }

            object value = null;

            if (o is IDictionary)
            {
                IDictionary map = (IDictionary)o;
                if (property.ToString() == TemplateGroup.DICT_KEY)
                    value = property;
                else if (property.Equals("keys"))
                    value = map.Keys;
                else if (property.Equals("values"))
                    value = map.Values;
                else if (map.Contains(property))
                    value = map[property];
                else if (map.Contains(ToString(self, property)))
                {
                    // if we can't find the key, toString it
                    value = map[ToString(self, property)];
                }
                else
                    value = map[TemplateGroup.DEFAULT_KEY]; // not found, use default
                if (property.ToString() == TemplateGroup.DICT_KEY)
                {
                    value = property;
                }
                return value;
            }

            var c = o.GetType();

            // try getXXX and isXXX properties

            // look up using reflection
            string propertyName = (string)property;
            string methodSuffix = char.ToUpperInvariant(propertyName[0]) +
                propertyName.Substring(1);
            MethodInfo m = GetMethod(c, "get" + methodSuffix);
            if (m == null)
            {
                m = GetMethod(c, "is" + methodSuffix);
            }
            if (m != null)
            {
                // save to avoid lookup later
                //self.getGroup().cacheClassProperty(c,propertyName,m);
                try
                {
                    value = InvokeMethod(m, o, value);
                }
                catch (Exception e)
                {
                    ErrorManager.RuntimeError(self, ErrorType.CantAccessPropertyMethod, e, m);
                }
            }
            else
            {
                // try for a visible field
                try
                {
                    FieldInfo f = c.GetField(propertyName);
                    //self.getGroup().cacheClassProperty(c,propertyName,f);
                    try
                    {
                        value = AccessField(f, o, value);
                    }
                    catch (Exception e)
                    {
                        ErrorManager.RuntimeError(self, ErrorType.CantAccessPropertyField, e, m);
                    }
                }
                catch
                {
                    ErrorManager.RuntimeError(self, ErrorType.NoSuchProperty, c, propertyName);
                }
            }

            return value;
        }

        protected object AccessField(FieldInfo f, object o, object value)
        {
            value = f.GetValue(o);
            return value;
        }

        protected object InvokeMethod(MethodInfo m, object o, object value)
        {
            value = m.Invoke(o, (object[])null);
            return value;
        }

        protected MethodInfo GetMethod(Type c, string methodName)
        {
            MethodInfo m;
            try
            {
                m = c.GetMethod(methodName, (Type[])null);
            }
            catch
            {
                m = null;
            }
            return m;
        }

        protected void Trace(Template self, int ip)
        {
            BytecodeDisassembler dis = new BytecodeDisassembler(self.code.instrs,
                                                                self.code.instrs.Length,
                                                                self.code.strings);
            StringBuilder buf = new StringBuilder();
            dis.DisassembleInstruction(buf, ip);
            string name = self.code.name + ":";
            if (self.code.name == Template.UnknownName)
                name = "";
            Console.Write(string.Format("{0:-40s}", name + buf));
            Console.Write("\tstack=[");
            for (int i = 0; i <= sp; i++)
            {
                object o = operands[i];
                PrintForTrace(o);
            }
            Console.Write(" ], calls=");
            Console.Write(self.GetEnclosingInstanceStackString());
            Console.Write(", sp=" + sp + ", nw=" + nw);
            Console.WriteLine();
        }

        protected void PrintForTrace(object o)
        {
            if (o is Template)
            {
                Console.Write(" " + ((Template)o).code.name + "()");
                return;
            }
            o = ConvertAnythingIteratableToIterator(o);
            if (o is Iterator)
            {
                Iterator it = (Iterator)o;
                Console.Write(" [");
                while (it.MoveNext())
                {
                    PrintForTrace(it.Current);
                }
                Console.Write(" ]");
            }
            else
            {
                Console.Write(" " + o);
            }
        }

        public static int GetInt(byte[] memory, int index)
        {
            int b1 = memory[index++] & 0xFF; // mask off sign-extended bits
            int b2 = memory[index++] & 0xFF;
            int b3 = memory[index++] & 0xFF;
            int b4 = memory[index++] & 0xFF;
            int word = b1 << (8 * 3) | b2 << (8 * 2) | b3 << (8 * 1) | b4;
            return word;
        }

        public static int GetShort(byte[] memory, int index)
        {
            int b1 = memory[index++] & 0xFF; // mask off sign-extended bits
            int b2 = memory[index++] & 0xFF;
            int word = b1 << (8 * 1) | b2;
            return word;
        }
    }
}
