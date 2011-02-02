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

namespace Antlr4.StringTemplate.Compiler
{
    using System.Collections.Generic;
    using Antlr.Runtime;
    using Antlr.Runtime.Tree;
    using Antlr4.StringTemplate.Misc;

    partial class CodeGenerator
    {
        /// <summary>Name of overall template</summary>
        private readonly string outermostTemplateName;
        /// <summary>Overall template token</summary>
        private readonly IToken templateToken;
        /// <summary>Overall template text</summary>
        private readonly string _template;
        private readonly TemplateCompiler _compiler;
        private CompiledTemplate outermostImpl;

        public CodeGenerator(ITreeNodeStream input, TemplateCompiler compiler, string name, string template, IToken templateToken)
            : this(input, new RecognizerSharedState())
        {
            this._compiler = compiler;
            this.outermostTemplateName = name;
            this._template = template;
            this.templateToken = templateToken;
        }

        public ErrorManager errMgr
        {
            get
            {
                return _compiler.ErrorManager;
            }
        }

        public TemplateGroup Group
        {
            get
            {
                return _compiler.Group;
            }
        }

        // convience funcs to hide offensive sending of emit messages to
        // CompilationState temp data object.

        public void emit1(CommonTree opAST, Bytecode opcode, int arg)
        {
            template_stack.Peek().state.Emit1(opAST, opcode, arg);
        }

        public void emit1(CommonTree opAST, Bytecode opcode, string arg)
        {
            template_stack.Peek().state.Emit1(opAST, opcode, arg);
        }

        public void emit2(CommonTree opAST, Bytecode opcode, int arg, int arg2)
        {
            template_stack.Peek().state.Emit2(opAST, opcode, arg, arg2);
        }

        public void emit2(CommonTree opAST, Bytecode opcode, string s, int arg2)
        {
            template_stack.Peek().state.Emit2(opAST, opcode, s, arg2);
        }

        public void emit(Bytecode opcode)
        {
            template_stack.Peek().state.Emit(opcode);
        }

        public void emit(CommonTree opAST, Bytecode opcode)
        {
            template_stack.Peek().state.Emit(opAST, opcode);
        }

        private readonly Stack<string> _elementIndent = new Stack<string>(new string[] { string.Empty });

        private void Indent(string text)
        {
            string strip = _elementIndent.Peek();
            if (!string.IsNullOrEmpty(strip) && text.StartsWith(strip))
                text = text.Substring(strip.Length);

            template_stack.Peek().state.Indent(text);
            _elementIndent.Push(_elementIndent.Peek() + text);
        }

        private void Dedent()
        {
            template_stack.Peek().state.Emit(Bytecode.INSTR_DEDENT);
            _elementIndent.Pop();
        }

        public void insert(int addr, Bytecode opcode, string s)
        {
            template_stack.Peek().state.Insert(addr, opcode, s);
        }

        public void setOption(CommonTree id)
        {
            template_stack.Peek().state.SetOption(id);
        }

        public void write(int addr, short value)
        {
            template_stack.Peek().state.Write(addr, value);
        }

        public int address()
        {
            return template_stack.Peek().state.ip;
        }

        public void func(CommonTree id)
        {
            template_stack.Peek().state.Function(templateToken, id);
        }

        public void refAttr(CommonTree id)
        {
            template_stack.Peek().state.ReferenceAttribute(templateToken, id);
        }

        public int defineString(string s)
        {
            return template_stack.Peek().state.DefineString(s);
        }
    }
}
