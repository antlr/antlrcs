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
    using Antlr.Runtime.JavaExtensions;
    using Antlr4.StringTemplate.Compiler;
    using Antlr4.StringTemplate.Debug;
    using Antlr4.StringTemplate.Misc;
    using Array = System.Array;
    using BitConverter = System.BitConverter;
    using Console = System.Console;
    using CultureInfo = System.Globalization.CultureInfo;
    using Environment = System.Environment;
    using ICollection = System.Collections.ICollection;
    using IDictionary = System.Collections.IDictionary;
    using IEnumerable = System.Collections.IEnumerable;
    using IList = System.Collections.IList;
    using IOException = System.IO.IOException;
    using Math = System.Math;
    using StringBuilder = System.Text.StringBuilder;
    using StringWriter = System.IO.StringWriter;

    /** This class knows how to execute template bytecodes relative to a
     *  particular TemplateGroup. To execute the byte codes, we need an output stream
     *  and a reference to an Template an instance. That instance's impl field points at
     *  a CompiledTemplate, which contains all of the byte codes and other information
     *  relevant to execution.
     *
     *  This interpreter is a stack-based bytecode interpreter.  All operands
     *  go onto an operand stack.
     *
     *  If the group that we're executing relative to has debug set, we track
     *  interpreter events. For now, I am only tracking instance creation events.
     *  These are used by STViz to pair up output chunks with the template
     *  expressions that generate them.
     *
     *  We create a new interpreter for each Template.Render(), DebugTemplate.Visualize, or
     *  DebugTemplate.GetEvents() invocation.
     */
    public class Interpreter
    {
        public enum Option
        {
            Anchor,
            Format,
            Null,
            Separator,
            Wrap
        }

        public const int DefaultOperandStackSize = 100;

        public static readonly HashSet<string> predefinedAnonSubtemplateAttributes = new HashSet<string>() { "i", "i0" };

        /** Dump bytecode instructions as we execute them? */
        public static bool trace = false;

        /** Exec st with respect to this group. Once set in Template.ToString(),
         *  it should be fixed. Template has group also.
         */
        private readonly TemplateGroup group;

        /** For renderers, we have to pass in the culture */
        private readonly CultureInfo culture;

        private readonly ErrorManager errMgr;

        /** Operand stack, grows upwards */
        private object[] operands = new object[DefaultOperandStackSize];
        private int sp = -1;        // stack pointer register
        private int current_ip = 0; // mirrors ip in exec(), but visible to all methods
        private int nwline = 0;     // how many char written on this template LINE so far?

        /** Track everything happening in interp if debug across all templates */
        private List<InterpEvent> events;

        /** If debug mode, track trace here */
        // TODO: track the pieces not a string and track what it contributes to output
        private List<string> executeTrace;

        private IDictionary<Template, List<InterpEvent>> debugInfo;

        public Interpreter(TemplateGroup group)
            : this(group, CultureInfo.CurrentCulture, group.ErrorManager)
        {
        }

        public Interpreter(TemplateGroup group, CultureInfo culture)
            : this(group, culture, group.ErrorManager)
        {
        }

        public Interpreter(TemplateGroup group, ErrorManager errMgr)
            : this(group, CultureInfo.CurrentCulture, errMgr)
        {
        }

        public Interpreter(TemplateGroup group, CultureInfo culture, ErrorManager errMgr)
        {
            this.group = group;
            this.culture = culture;
            this.errMgr = errMgr;
            if (group.Debug)
            {
                events = new List<InterpEvent>();
                executeTrace = new List<string>();
                debugInfo = new Dictionary<Template, List<InterpEvent>>();
            }
        }

        /** Execute template self and return how many characters it wrote to out */
        public virtual int Execute(ITemplateWriter @out, Template self)
        {
            int save_ip = current_ip;
            try
            {
                return ExecuteImpl(@out, self);
            }
            finally
            {
                current_ip = save_ip;
            }
        }

        protected virtual int ExecuteImpl(ITemplateWriter @out, Template self)
        {
            int start = @out.Index; // track char we're about to Write
            Bytecode prevOpcode = Bytecode.Invalid;
            int n = 0; // how many char we Write out
            int nargs;
            int nameIndex;
            int addr;
            string name;
            object o, left, right;
            Template st;
            object[] options;
            byte[] code = self.impl.instrs;        // which code block are we executing
            int ip = 0;
            while (ip < self.impl.codeSize)
            {
                if (trace || group.Debug)
                    Trace(self, ip);

                Bytecode opcode = (Bytecode)code[ip];
                current_ip = ip;
                ip++; //jump to next instruction or first byte of operand
                switch (opcode)
                {
                case Bytecode.INSTR_LOAD_STR:
                    int strIndex = GetShort(code, ip);
                    ip += Instruction.OperandSizeInBytes;
                    operands[++sp] = self.impl.strings[strIndex];
                    break;

                case Bytecode.INSTR_LOAD_ATTR:
                    nameIndex = GetShort(code, ip);
                    ip += Instruction.OperandSizeInBytes;
                    name = self.impl.strings[nameIndex];
                    try
                    {
                        o = self.GetAttribute(name);
                    }
                    catch (TemplateNoSuchPropertyException)
                    {
                        errMgr.RuntimeError(self, current_ip, ErrorType.NO_SUCH_ATTRIBUTE, name);
                        o = null;
                    }
                    operands[++sp] = o;
                    break;

                case Bytecode.INSTR_LOAD_LOCAL:
                    int valueIndex = GetShort(code, ip);
                    ip += Instruction.OperandSizeInBytes;
                    o = self.locals[valueIndex];
                    if (o == Template.EmptyAttribute)
                        o = null;
                    operands[++sp] = o;
                    break;

                case Bytecode.INSTR_LOAD_PROP:
                    nameIndex = GetShort(code, ip);
                    ip += Instruction.OperandSizeInBytes;
                    o = operands[sp--];
                    name = self.impl.strings[nameIndex];
                    operands[++sp] = GetObjectProperty(self, o, name);
                    break;

                case Bytecode.INSTR_LOAD_PROP_IND:
                    object propName = operands[sp--];
                    o = operands[sp];
                    operands[sp] = GetObjectProperty(self, o, propName);
                    break;

                case Bytecode.INSTR_NEW:
                    nameIndex = GetShort(code, ip);
                    ip += Instruction.OperandSizeInBytes;
                    name = self.impl.strings[nameIndex];
                    nargs = GetShort(code, ip);
                    ip += Instruction.OperandSizeInBytes;
                    // look up in original hierarchy not enclosing template (variable group)
                    // see TestSubtemplates.testEvalSTFromAnotherGroup()
                    st = self.groupThatCreatedThisInstance.GetEmbeddedInstanceOf(self, ip, name);
                    // get n args and store into st's attr list
                    StoreArguments(self, nargs, st);
                    sp -= nargs;
                    operands[++sp] = st;
                    break;

                case Bytecode.INSTR_NEW_IND:
                    nargs = GetShort(code, ip);
                    ip += Instruction.OperandSizeInBytes;
                    name = (string)operands[sp - nargs];
                    st = self.groupThatCreatedThisInstance.GetEmbeddedInstanceOf(self, ip, name);
                    StoreArguments(self, nargs, st);
                    sp -= nargs;
                    sp--; // pop template name
                    operands[++sp] = st;
                    break;

                case Bytecode.INSTR_NEW_BOX_ARGS:
                    nameIndex = GetShort(code, ip);
                    ip += Instruction.OperandSizeInBytes;
                    name = self.impl.strings[nameIndex];
                    IDictionary<string, object> attrs = (IDictionary<string, object>)operands[sp--];
                    // look up in original hierarchy not enclosing template (variable group)
                    // see TestSubtemplates.testEvalSTFromAnotherGroup()
                    st = self.groupThatCreatedThisInstance.GetEmbeddedInstanceOf(self, ip, name);
                    // get n args and store into st's attr list
                    StoreArguments(self, attrs, st);
                    operands[++sp] = st;
                    break;

                case Bytecode.INSTR_SUPER_NEW:
                    nameIndex = GetShort(code, ip);
                    ip += Instruction.OperandSizeInBytes;
                    name = self.impl.strings[nameIndex];
                    nargs = GetShort(code, ip);
                    ip += Instruction.OperandSizeInBytes;
                    SuperNew(self, name, nargs);
                    break;

                case Bytecode.INSTR_SUPER_NEW_BOX_ARGS:
                    nameIndex = GetShort(code, ip);
                    ip += Instruction.OperandSizeInBytes;
                    name = self.impl.strings[nameIndex];
                    attrs = (IDictionary<string, object>)operands[sp--];
                    SuperNew(self, name, attrs);
                    break;

                case Bytecode.INSTR_STORE_OPTION:
                    int optionIndex = GetShort(code, ip);
                    ip += Instruction.OperandSizeInBytes;
                    o = operands[sp--];    // value to store
                    options = (object[])operands[sp]; // get options
                    options[optionIndex] = o; // store value into options on stack
                    break;

                case Bytecode.INSTR_STORE_ARG:
                    nameIndex = GetShort(code, ip);
                    name = self.impl.strings[nameIndex];
                    ip += Instruction.OperandSizeInBytes;
                    o = operands[sp--];
                    attrs = (IDictionary<string, object>)operands[sp];
                    attrs[name] = o; // leave attrs on stack
                    break;

                case Bytecode.INSTR_WRITE:
                    o = operands[sp--];
                    int n1 = WriteObjectNoOptions(@out, self, o);
                    n += n1;
                    nwline += n1;
                    break;

                case Bytecode.INSTR_WRITE_OPT:
                    options = (object[])operands[sp--]; // get options
                    o = operands[sp--];                 // get option to Write
                    int n2 = WriteObjectWithOptions(@out, self, o, options);
                    n += n2;
                    nwline += n2;
                    break;

                case Bytecode.INSTR_MAP:
                    st = (Template)operands[sp--]; // get prototype off stack
                    o = operands[sp--];		 // get object to map prototype across
                    Map(self, o, st);
                    break;

                case Bytecode.INSTR_ROT_MAP:
                    int nmaps = GetShort(code, ip);
                    ip += Instruction.OperandSizeInBytes;
                    List<Template> templates = new List<Template>();
                    for (int i = nmaps - 1; i >= 0; i--)
                        templates.Add((Template)operands[sp - i]);
                    sp -= nmaps;
                    o = operands[sp--];
                    if (o != null)
                        RotateMap(self, o, templates);
                    break;

                case Bytecode.INSTR_ZIP_MAP:
                    st = (Template)operands[sp--];
                    nmaps = GetShort(code, ip);
                    ip += Instruction.OperandSizeInBytes;
                    List<object> exprs = new List<object>();
                    for (int i = nmaps - 1; i >= 0; i--)
                        exprs.Add(operands[sp - i]);

                    sp -= nmaps;
                    operands[++sp] = ZipMap(self, exprs, st);
                    break;

                case Bytecode.INSTR_BR:
                    ip = GetShort(code, ip);
                    break;

                case Bytecode.INSTR_BRF:
                    addr = GetShort(code, ip);
                    ip += Instruction.OperandSizeInBytes;
                    o = operands[sp--]; // <if(expr)>...<endif>
                    if (!TestAttributeTrue(o))
                        ip = addr; // jump

                    break;

                case Bytecode.INSTR_OPTIONS:
                    operands[++sp] = new object[Compiler.TemplateCompiler.NUM_OPTIONS];
                    break;

                case Bytecode.INSTR_ARGS:
                    operands[++sp] = new Dictionary<string, object>();
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
                    operands[sp] = Strip(operands[sp]);
                    break;

                case Bytecode.INSTR_TRIM:
                    o = operands[sp--];
                    if (o.GetType() == typeof(string))
                    {
                        operands[++sp] = ((string)o).Trim();
                    }
                    else
                    {
                        errMgr.RuntimeError(self, current_ip, ErrorType.EXPECTING_STRING, "trim", o.GetType());
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
                        errMgr.RuntimeError(self, current_ip, ErrorType.EXPECTING_STRING, "strlen", o.GetType());
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
                    ip += Instruction.OperandSizeInBytes;
                    Indent(@out, self, strIndex);
                    break;

                case Bytecode.INSTR_DEDENT:
                    @out.PopIndentation();
                    break;

                case Bytecode.INSTR_NEWLINE:
                    try
                    {
                        if (prevOpcode == Bytecode.INSTR_NEWLINE ||
                            prevOpcode == Bytecode.INSTR_INDENT ||
                            nwline > 0)
                        {
                            @out.Write(Environment.NewLine);
                        }
                        nwline = 0;
                    }
                    catch (IOException ioe)
                    {
                        errMgr.IOError(self, ErrorType.WRITE_IO_ERROR, ioe);
                    }
                    break;

                case Bytecode.INSTR_NOOP:
                    break;

                case Bytecode.INSTR_POP:
                    sp--; // throw away top of stack
                    break;

                case Bytecode.INSTR_NULL:
                    operands[++sp] = null;
                    break;

                case Bytecode.INSTR_TRUE:
                    operands[++sp] = true;
                    break;

                case Bytecode.INSTR_FALSE:
                    operands[++sp] = false;
                    break;

                case Bytecode.INSTR_WRITE_STR:
                    strIndex = GetShort(code, ip);
                    ip += Instruction.OperandSizeInBytes;
                    o = self.impl.strings[strIndex];
                    n1 = WriteObjectNoOptions(@out, self, o);
                    n += n1;
                    nwline += n1;
                    break;

                case Bytecode.INSTR_WRITE_LOCAL:
                    valueIndex = GetShort(code, ip);
                    ip += Instruction.OperandSizeInBytes;
                    o = self.locals[valueIndex];
                    if (o == Template.EmptyAttribute)
                        o = null;

                    n1 = WriteObjectNoOptions(@out, self, o);
                    n += n1;
                    nwline += n1;
                    break;

                default:
                    errMgr.InternalError(self, "invalid bytecode @ " + (ip - 1) + ": " + opcode, null);
                    self.impl.Dump();
                    break;
                }

                prevOpcode = opcode;
            }

            if (group.Debug)
            {
                EvalTemplateEvent e = new EvalTemplateEvent((DebugTemplate)self, Interval.FromBounds(start, @out.Index));
                //Console.WriteLine(e);
                events.Add(e);
                if (self.EnclosingInstance != null)
                {
                    DebugTemplate parent = (DebugTemplate)self.EnclosingInstance;
                    GetEvents(parent).Add(e);
                }
            }
            return n;
        }

        // TODO: refactor to Remove dup'd code

        internal virtual void SuperNew(Template self, string name, int nargs)
        {
            Template st = null;
            CompiledTemplate imported = self.impl.NativeGroup.LookupImportedTemplate(name);
            if (imported == null)
            {
                errMgr.RuntimeError(self, current_ip, ErrorType.NO_IMPORTED_TEMPLATE,
                                    name);
                st = self.groupThatCreatedThisInstance.CreateStringTemplate();
                st.impl = new CompiledTemplate();
                sp -= nargs;
                operands[++sp] = st;
                return;
            }

            st = imported.NativeGroup.CreateStringTemplate();
            st.EnclosingInstance = self; // self invoked super.name()
            st.groupThatCreatedThisInstance = group;
            st.impl = imported;

            // get n args and store into st's attr list
            StoreArguments(self, nargs, st);
            sp -= nargs;
            operands[++sp] = st;
        }

        internal virtual void SuperNew(Template self, string name, IDictionary<string, object> attrs)
        {
            Template st = null;
            CompiledTemplate imported = self.impl.NativeGroup.LookupImportedTemplate(name);
            if (imported == null)
            {
                errMgr.RuntimeError(self, current_ip, ErrorType.NO_IMPORTED_TEMPLATE, name);
                st = self.groupThatCreatedThisInstance.CreateStringTemplate();
                st.impl = new CompiledTemplate();
                operands[++sp] = st;
                return;
            }

            st = imported.NativeGroup.CreateStringTemplate();
            st.EnclosingInstance = self; // self invoked super.name()
            st.groupThatCreatedThisInstance = group;
            st.impl = imported;

            // get n args and store into st's attr list
            StoreArguments(self, attrs, st);
            operands[++sp] = st;
        }

        internal virtual void StoreArguments(Template self, IDictionary<string, object> attrs, Template st)
        {
            int nformalArgs = 0;
            if (st.impl.FormalArguments != null)
                nformalArgs = st.impl.FormalArguments.Count;
            int nargs = 0;
            if (attrs != null)
                nargs = attrs.Count;

            if (nargs < (nformalArgs - st.impl.NumberOfArgsWithDefaultValues) || nargs > nformalArgs)
            {
                errMgr.RuntimeError(self,
                                    current_ip,
                                    ErrorType.ARGUMENT_COUNT_MISMATCH,
                                    nargs,
                                    st.impl.name,
                                    nformalArgs);
            }

            foreach (string argName in attrs.Keys)
            {
                // don't let it throw an exception in RawSetAttribute
                if (st.impl.FormalArguments == null || !st.impl.FormalArguments.Any(i => i.Name == argName))
                {
                    errMgr.RuntimeError(self, current_ip, ErrorType.NO_SUCH_ATTRIBUTE, argName);
                    continue;
                }

                object o = attrs[argName];
                st.RawSetAttribute(argName, o);
            }
        }

        internal virtual void StoreArguments(Template self, int nargs, Template st)
        {
            int nformalArgs = 0;
            if (st.impl.FormalArguments != null)
                nformalArgs = st.impl.FormalArguments.Count;
            int firstArg = sp - (nargs - 1);
            int numToStore = Math.Min(nargs, nformalArgs);
            if (st.impl.isAnonSubtemplate)
                nformalArgs -= predefinedAnonSubtemplateAttributes.Count;

            if (nargs < (nformalArgs - st.impl.NumberOfArgsWithDefaultValues) ||
                 nargs > nformalArgs)
            {
                errMgr.RuntimeError(self,
                                    current_ip,
                                    ErrorType.ARGUMENT_COUNT_MISMATCH,
                                    nargs,
                                    st.impl.name,
                                    nformalArgs);
            }

            if (st.impl.FormalArguments == null)
                return;

            for (int i = 0; i < numToStore; i++)
            {
                object o = operands[firstArg + i];
                string argName = st.impl.FormalArguments[i].Name;
                st.RawSetAttribute(argName, o);
            }
        }

        protected void Indent(ITemplateWriter @out, Template self, int strIndex)
        {
            string indent = self.impl.strings[strIndex];
            if (group.Debug)
            {
                int start = @out.Index; // track char we're about to write
                EvalExprEvent e = new IndentEvent((DebugTemplate)self, new Interval(start, indent.Length), GetExpressionInterval(self));
                //Console.WriteLine(e);
                events.Add(e);
            }

            @out.PushIndentation(indent);
        }

        /** Write out an expression result that doesn't use expression options.
         *  E.g., &lt;name&gt;
         */
        protected virtual int WriteObjectNoOptions(ITemplateWriter @out, Template self, object o)
        {
            int start = @out.Index; // track char we're about to Write
            int n = WriteObject(@out, self, o, null);
            if (group.Debug)
            {
                Interval templateLocation = self.impl.sourceMap[current_ip];
                EvalExprEvent e = new EvalExprEvent((DebugTemplate)self, Interval.FromBounds(start, @out.Index), templateLocation);
                Console.WriteLine(e);
                events.Add(e);
            }
            return n;
        }

        /** Write out an expression result that uses expression options.
         *  E.g., &lt;names; separator=", "&gt;
         */
        protected virtual int WriteObjectWithOptions(ITemplateWriter @out, Template self, object o,
                                             object[] options)
        {
            int start = @out.Index; // track char we're about to Write
            // precompute all option values (Render all the way to strings)
            string[] optionStrings = null;
            if (options != null)
            {
                optionStrings = new string[options.Length];
                for (int i = 0; i < Compiler.TemplateCompiler.NUM_OPTIONS; i++)
                {
                    optionStrings[i] = ToString(self, options[i]);
                }
            }

            if (options != null && options[(int)Option.Anchor] != null)
            {
                @out.PushAnchorPoint();
            }

            int n = WriteObject(@out, self, o, optionStrings);

            if (options != null && options[(int)Option.Anchor] != null)
            {
                @out.PopAnchorPoint();
            }

            if (group.Debug)
            {
                Interval templateLocation = self.impl.sourceMap[current_ip];
                EvalExprEvent e = new EvalExprEvent((DebugTemplate)self, Interval.FromBounds(start, @out.Index), templateLocation);
                Console.WriteLine(e);
                events.Add(e);
            }

            return n;
        }

        /** Generic method to emit text for an object. It differentiates
         *  between templates, iterable objects, and plain old Java objects (POJOs)
         */
        protected virtual int WriteObject(ITemplateWriter @out, Template self, object o, string[] options)
        {
            int n = 0;
            if (o == null)
            {
                if (options != null && options[(int)Option.Null] != null)
                    o = options[(int)Option.Null];
                else
                    return 0;
            }

            ITypeProxyFactory proxyFactory = self.groupThatCreatedThisInstance.GetTypeProxyFactory(o.GetType());
            if (proxyFactory != null)
                o = proxyFactory.CreateProxy(o);

            Template template = o as Template;
            if (template != null)
            {
                if (template.EnclosingInstance != self)
                    template = template.CreateShadow(self);

                SetDefaultArguments(template);
                if (options != null && options[(int)Option.Wrap] != null)
                {
                    // if we have a wrap string, then inform writer it
                    // might need to wrap
                    try
                    {
                        @out.WriteWrap(options[(int)Option.Wrap]);
                    }
                    catch (IOException ioe)
                    {
                        errMgr.IOError(self, ErrorType.WRITE_IO_ERROR, ioe);
                    }
                }
                n = Execute(@out, template);
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
                    errMgr.IOError(self, ErrorType.WRITE_IO_ERROR, ioe, o);
                }
            }

            return n;
        }

        protected virtual int WriteIterator(ITemplateWriter @out, Template self, object o, string[] options)
        {
            if (o == null)
                return 0;
            int n = 0;
            Iterator it = (Iterator)o;
            string separator = null;
            if (options != null)
                separator = options[(int)Option.Separator];
            bool seenAValue = false;
            while (it.hasNext())
            {
                object iterValue = it.next();
                // Emit separator if we're beyond first value
                bool needSeparator = seenAValue &&
                    separator != null &&            // we have a separator and
                    (iterValue != null ||           // either we have a value
                        options[(int)Option.Null] != null); // or no value but null option
                if (needSeparator)
                    n += @out.WriteSeparator(separator);
                int nw = WriteObject(@out, self, iterValue, options);
                if (nw > 0)
                    seenAValue = true;
                n += nw;
            }
            return n;
        }

        protected virtual int WritePlainObject(ITemplateWriter @out, object o, string[] options)
        {
            string formatString = null;
            if (options != null)
                formatString = options[(int)Option.Format];
            IAttributeRenderer r = group.GetAttributeRenderer(o.GetType());
            string v;
            if (r != null)
                v = r.ToString(o, formatString, culture);
            else
                v = o.ToString();
            int n;
            if (options != null && options[(int)Option.Wrap] != null)
            {
                n = @out.Write(v, options[(int)Option.Wrap]);
            }
            else
            {
                n = @out.Write(v);
            }
            return n;
        }

        protected virtual Interval GetExpressionInterval(Template self)
        {
            return self.impl.sourceMap[current_ip];
        }

        protected virtual void Map(Template self, object attr, Template st)
        {
            RotateMap(self, attr, new List<Template>() { st });
        }

        // <names:a> or <names:a,b>
        protected virtual void RotateMap(Template self, object attr, List<Template> prototypes)
        {
            if (attr == null)
            {
                operands[++sp] = null;
                return;
            }
            attr = ConvertAnythingIteratableToIterator(attr);
            Iterator iterator = attr as Iterator;
            if (iterator != null)
            {
                List<Template> mapped = RotateMapIterator(self, iterator, prototypes);
                operands[++sp] = mapped;
            }
            else
            {
                // if only single value, just apply first template to sole value
                Template proto = prototypes[0];
                Template st = group.CreateStringTemplate(proto);
                if (st != null)
                {
                    SetFirstArgument(self, st, attr);
                    if (st.impl.isAnonSubtemplate)
                    {
                        st.RawSetAttribute("i0", 0);
                        st.RawSetAttribute("i", 1);
                    }

                    operands[++sp] = st;
                }
                else
                {
                    operands[++sp] = null;
                }
            }
        }

        protected virtual List<Template> RotateMapIterator(Template self, Iterator iterator, List<Template> prototypes)
        {
            List<Template> mapped = new List<Template>();
            int i0 = 0;
            int i = 1;
            int ti = 0;
            while (iterator.hasNext())
            {
                object iterValue = iterator.next();
                if (iterValue == null)
                {
                    mapped.Add(null);
                    continue;
                }

                int templateIndex = ti % prototypes.Count; // rotate through
                ti++;
                Template proto = prototypes[templateIndex];
                Template st = group.CreateStringTemplate(proto);
                SetFirstArgument(self, st, iterValue);
                if (st.impl.isAnonSubtemplate)
                {
                    st.RawSetAttribute("i0", i0);
                    st.RawSetAttribute("i", i);
                }

                mapped.Add(st);
                i0++;
                i++;
            }

            return mapped;
        }

        // <names,phones:{n,p | ...}> or <a,b:t()>
        // todo: i, i0 not set unless mentioned? map:{k,v | ..}?
        protected virtual Template.AttributeList ZipMap(Template self, List<object> exprs, Template prototype)
        {
            if (exprs == null || prototype == null || exprs.Count == 0)
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
            int numExprs = exprs.Count;
            CompiledTemplate code = prototype.impl;
            List<FormalArgument> formalArguments = code.FormalArguments;
            if (!code.hasFormalArgs || formalArguments == null)
            {
                errMgr.RuntimeError(self, current_ip, ErrorType.MISSING_FORMAL_ARGUMENTS);
                return null;
            }

            // todo: track formal args not names for efficient filling of locals
            object[] formalArgumentNames = formalArguments.Select(i => i.Name).ToArray();
            int nformalArgs = formalArgumentNames.Length;
            if (prototype.IsAnonymousSubtemplate)
                nformalArgs -= predefinedAnonSubtemplateAttributes.Count;

            if (nformalArgs != numExprs)
            {
                errMgr.RuntimeError(self, current_ip, ErrorType.MAP_ARGUMENT_COUNT_MISMATCH, numExprs, nformalArgs);
                // TODO just fill first n
                // truncate arg list to match smaller size
                int shorterSize = Math.Min(formalArgumentNames.Length, numExprs);
                numExprs = shorterSize;
                Array.Resize(ref formalArgumentNames, shorterSize);
            }

            // keep walking while at least one attribute has values

            Template.AttributeList results = new Template.AttributeList();
            int i2 = 0; // iteration number from 0
            while (true)
            {
                // get a value for each attribute in list; put into Template instance
                int numEmpty = 0;
                Template embedded = group.CreateStringTemplate(prototype);
                embedded.RawSetAttribute("i0", i2);
                embedded.RawSetAttribute("i", i2 + 1);
                for (int a = 0; a < numExprs; a++)
                {
                    Iterator it = (Iterator)exprs[a];
                    if (it != null && it.hasNext())
                    {
                        string argName = (string)formalArgumentNames[a];
                        object iteratedValue = it.next();
                        embedded.RawSetAttribute(argName, iteratedValue);
                    }
                    else
                    {
                        numEmpty++;
                    }
                }

                if (numEmpty == numExprs)
                    break;

                results.Add(embedded);
                i2++;
            }
            return results;
        }

        protected virtual void SetFirstArgument(Template self, Template st, object attr)
        {
            if (st.impl.FormalArguments == null)
            {
                errMgr.RuntimeError(self, current_ip, ErrorType.ARGUMENT_COUNT_MISMATCH, 1, st.impl.name, 0);
                return;
            }

            st.locals[0] = attr;
        }

        protected virtual void AddToList(List<object> list, object o)
        {
            o = Interpreter.ConvertAnythingIteratableToIterator(o);
            if (o is Iterator)
            {
                // copy of elements into our temp list
                Iterator it = (Iterator)o;
                while (it.hasNext())
                    list.Add(it.next());
            }
            else
            {
                list.Add(o);
            }
        }

        /** Return the first attribute if multiple valued or the attribute
         *  itself if single-valued.  Used in &lt;names:First()&gt;
         */
        public virtual object First(object v)
        {
            if (v == null)
                return null;
            object r = v;
            v = ConvertAnythingIteratableToIterator(v);
            if (v is Iterator)
            {
                Iterator it = (Iterator)v;
                if (it.hasNext())
                {
                    r = it.next();
                }
            }
            return r;
        }

        /** Return the last attribute if multiple valued or the attribute
         *  itself if single-valued. Unless it's a list or array, this is pretty
         *  slow as it iterates until the last element.
         */
        public virtual object Last(object v)
        {
            if (v == null)
                return null;

            IList list = v as IList;
            if (list != null)
                return list[list.Count - 1];

            object last = v;
            v = ConvertAnythingIteratableToIterator(v);
            Iterator it = v as Iterator;
            if (it != null)
            {
                while (it.hasNext())
                    last = it.next();
            }

            return last;
        }

        /** Return everything but the first attribute if multiple valued
         *  or null if single-valued.
         */
        public virtual object Rest(object v)
        {
            if (v == null)
                return null;

            v = ConvertAnythingIteratableToIterator(v);
            Iterator it = v as Iterator;
            if (it != null)
            {
                if (!it.hasNext())
                    return null; // if not even one value return null

                List<object> a = new List<object>();
                it.next(); // ignore first value
                while (it.hasNext())
                {
                    object o = it.next();
                    if (o != null)
                        a.Add(o);
                }

                return a;
            }

            // rest of single-valued attribute is null
            return null;
        }

        /** Return all but the last element.  Trunc(x)=null if x is single-valued. */
        public virtual object Trunc(object v)
        {
            if (v == null)
                return null;

            v = ConvertAnythingIteratableToIterator(v);
            if (v is Iterator)
            {
                List<object> a = new List<object>();
                Iterator it = (Iterator)v;
                while (it.hasNext())
                {
                    object o = it.next();
                    if (it.hasNext())
                        a.Add(o); // only Add if not last one
                }

                return a;
            }

            // Trunc(x)==null when x single-valued attribute
            return null;
        }

        /** Return a new list w/o null values. */
        public virtual object Strip(object v)
        {
            if (v == null)
                return null;

            v = ConvertAnythingIteratableToIterator(v);
            if (v is Iterator)
            {
                List<object> a = new List<object>();
                Iterator it = (Iterator)v;
                while (it.hasNext())
                {
                    object o = it.next();
                    if (o != null)
                        a.Add(o);
                }

                return a;
            }

            return v; // Strip(x)==x when x single-valued attribute
        }

        /** Return a list with the same elements as v but in reverse order. null
         *  values are NOT stripped out. use Reverse(Strip(v)) to do that.
         */
        public virtual object Reverse(object v)
        {
            if (v == null)
                return null;

            v = ConvertAnythingIteratableToIterator(v);
            Iterator it = v as Iterator;
            if (it != null)
            {
                List<object> a = new List<object>();
                while (it.hasNext())
                    a.Add(it.next());

                a.Reverse();
                return a;
            }

            return v;
        }

        /** Return the length of a mult-valued attribute or 1 if it is a
         *  single attribute. If attribute is null return 0.
         *  Special case several common collections and primitive arrays for
         *  speed. This method by Kay Roepke from v3.
         */
        public virtual object Length(object v)
        {
            if (v == null)
                return 0;

            string str = v as string;
            if (str != null)
                return 1;

            ICollection collection = v as ICollection;
            if (collection != null)
                return collection.Count;

            IDictionary dictionary = v as IDictionary;
            if (dictionary != null)
                return dictionary.Count;

            IEnumerable enumerable = v as IEnumerable;
            if (enumerable != null)
                return enumerable.Cast<object>().Count();

            Iterator iterator = v as Iterator;
            if (iterator != null)
            {
                int i = 0;
                while (iterator.hasNext())
                {
                    iterator.next();
                    i++;
                }
                return i;
            }

            return 1;
        }

        protected virtual string ToString(Template self, object value)
        {
            if (value != null)
            {
                if (value.GetType() == typeof(string))
                    return (string)value;

                // if Template, make sure it evaluates with enclosing template as self
                Template template = value as Template;
                if (template != null)
                {
                    if (template.EnclosingInstance != self)
                        value = template = template.CreateShadow(self);
                }

                // if not string already, must evaluate it
                StringWriter sw = new StringWriter();
                /*
                            Interpreter interp = new Interpreter(group, new NoIndentWriter(sw), culture);
                            interp.WriteObjectNoOptions(self, value, -1, -1);
                            */
                WriteObjectNoOptions(new NoIndentWriter(sw), self, value);

                return sw.ToString();
            }
            return null;
        }

        public static object ConvertAnythingIteratableToIterator(object o)
        {
            if (o == null)
                return null;

            string str = o as string;
            if (str != null)
                return str;

            IDictionary dictionary = o as IDictionary;
            if (dictionary != null)
                return dictionary.Keys.iterator();

            ICollection collection = o as ICollection;
            if (collection != null)
                return collection.iterator();

            IEnumerable enumerable = o as IEnumerable;
            if (enumerable != null)
                return enumerable.Cast<object>().iterator();

            //// This code is implied in the last line
            //Iterator iterator = o as Iterator;
            //if ( iterator != null )
            //    return iterator;

            return o;
        }

        public static Iterator ConvertAnythingToIterator(object o)
        {
            o = ConvertAnythingIteratableToIterator(o);

            Iterator iter = o as Iterator;
            if (iter != null)
                return iter;

            Template.AttributeList singleton = new Template.AttributeList(1);
            singleton.Add(o);
            return singleton.iterator();
        }

        protected virtual bool TestAttributeTrue(object a)
        {
            if (a == null)
                return false;

            if (a is bool)
                return (bool)a;

            string str = a as string;
            if (str != null)
                return true;

            ICollection collection = a as ICollection;
            if (collection != null)
                return collection.Count > 0;

            IDictionary dictionary = a as IDictionary;
            if (dictionary != null)
                return dictionary.Count > 0;

            IEnumerable enumerable = a as IEnumerable;
            if (enumerable != null)
                return enumerable.Cast<object>().Any();

            Iterator iterator = a as Iterator;
            if (iterator != null)
                return iterator.hasNext();

            // any other non-null object, return true--it's present
            return true;
        }

        protected virtual object GetObjectProperty(Template self, object o, object property)
        {
            if (o == null)
            {
                errMgr.RuntimeError(self, current_ip, ErrorType.NO_SUCH_PROPERTY,
                                          "null attribute");
                return null;
            }

            try
            {
                ITypeProxyFactory proxyFactory = self.groupThatCreatedThisInstance.GetTypeProxyFactory(o.GetType());
                if (proxyFactory != null)
                    o = proxyFactory.CreateProxy(o);

                IModelAdaptor adap = self.groupThatCreatedThisInstance.GetModelAdaptor(o.GetType());
                return adap.GetProperty(self, o, property, ToString(self, property));
            }
            catch (TemplateNoSuchPropertyException e)
            {
                errMgr.RuntimeError(self, current_ip, ErrorType.NO_SUCH_PROPERTY,
                                          e, o.GetType().Name + "." + property);
            }
            return null;
        }

        /** Set any default argument values that were not set by the
         *  invoking template or by setAttribute directly.  Note
         *  that the default values may be templates.
         *
         *  The evaluation context is the template enclosing invokedST.
         */
        public virtual void SetDefaultArguments(Template invokedST)
        {
            if (invokedST.impl.FormalArguments == null || invokedST.impl.NumberOfArgsWithDefaultValues == 0)
                return;

            foreach (FormalArgument arg in invokedST.impl.FormalArguments)
            {
                // if no value for attribute and default arg, inject default arg into self
                if (invokedST.locals[arg.Index] != Template.EmptyAttribute || arg.DefaultValueToken == null)
                    continue;

                if (arg.DefaultValueToken.Type == GroupParser.ANONYMOUS_TEMPLATE)
                {
                    Template defaultArgST = group.CreateStringTemplate();
                    // default arg template must see other args so it's enclosing
                    // instance is the template we are invoking.
                    defaultArgST.EnclosingInstance = invokedST;
                    defaultArgST.groupThatCreatedThisInstance = group;
                    defaultArgST.impl = arg.CompiledDefaultValue;
                    // If default arg is template with single expression
                    // wrapped in parens, x={<(...)>}, then eval to string
                    // rather than setting x to the template for later
                    // eval.
                    string defArgTemplate = arg.DefaultValueToken.Text;
                    if (defArgTemplate.StartsWith("{" + group.delimiterStartChar + "(")
                        && defArgTemplate.EndsWith(")" + group.delimiterStopChar + "}"))
                    {
                        invokedST.RawSetAttribute(arg.Name, ToString(invokedST, defaultArgST));
                    }
                    else
                    {
                        invokedST.RawSetAttribute(arg.Name, defaultArgST);
                    }
                }
                else
                {
                    invokedST.RawSetAttribute(arg.Name, arg.DefaultValue);
                }
            }
        }

        protected virtual void Trace(Template self, int ip)
        {
            StringBuilder tr = new StringBuilder();
            BytecodeDisassembler dis = new BytecodeDisassembler(self.impl);
            StringBuilder buf = new StringBuilder();
            dis.DisassembleInstruction(buf, ip);
            string name = self.impl.name + ":";
            if (self.impl.name == Template.UnknownName)
                name = string.Empty;

            tr.Append(string.Format("{0,-40}", name + buf));
            tr.Append("\tstack=[");
            for (int i = 0; i <= sp; i++)
            {
                object o = operands[i];
                PrintForTrace(tr, o);
            }

            tr.Append(" ], calls=");
            tr.Append(self.GetEnclosingInstanceStackString());
            tr.Append(", sp=" + sp + ", nw=" + nwline);
            string s = tr.ToString();

            if (group.Debug)
                executeTrace.Add(s);

            if (trace)
                Console.WriteLine(s);
        }

        protected virtual void PrintForTrace(StringBuilder tr, object o)
        {
            if (o is Template)
            {
                if (((Template)o).impl == null)
                    tr.Append("bad-template()");
                else
                    tr.Append(" " + ((Template)o).impl.name + "()");
                return;
            }
            o = ConvertAnythingIteratableToIterator(o);
            if (o is Iterator)
            {
                Iterator it = (Iterator)o;
                tr.Append(" [");
                while (it.hasNext())
                {
                    object iterValue = it.next();
                    PrintForTrace(tr, iterValue);
                }
                tr.Append(" ]");
            }
            else
            {
                tr.Append(" " + o);
            }
        }

        public virtual List<InterpEvent> GetEvents()
        {
            return events;
        }

        public virtual List<InterpEvent> GetEvents(Template st)
        {
            List<InterpEvent> events;
            if (!debugInfo.TryGetValue(st, out events) || events == null)
                debugInfo[st] = events = new List<InterpEvent>();

            return events;
        }

        public virtual List<string> GetExecutionTrace()
        {
            return executeTrace;
        }

        private static int GetShort(byte[] value, int startIndex)
        {
            return BitConverter.ToInt16(value, startIndex);
        }
    }
}
