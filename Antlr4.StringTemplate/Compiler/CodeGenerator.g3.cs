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
    using Antlr.Runtime;
    using Antlr4.StringTemplate.Misc;
    using Antlr.Runtime.Tree;

    partial class CodeGenerator
    {
        string outermostTemplateName;	// name of overall template
        CompiledST outermostImpl;
        IToken templateToken;			// overall template token
        string _template;  				// overall template text
        ErrorManager errMgr;

        public CodeGenerator(ITreeNodeStream input, ErrorManager errMgr, string name, string template, IToken templateToken)
            : this(input, new RecognizerSharedState())
        {
            this.errMgr = errMgr;
            this.outermostTemplateName = name;
            this._template = template;
            this.templateToken = templateToken;
        }

        // convience funcs to hide offensive sending of emit messages to
        // CompilationState temp data object.

        public void emit1(CommonTree opAST, short opcode, int arg)
        {
            template_stack.Peek().state.emit1(opAST, opcode, arg);
        }

        public void emit1(CommonTree opAST, short opcode, string arg)
        {
            template_stack.Peek().state.emit1(opAST, opcode, arg);
        }

        public void emit2(CommonTree opAST, short opcode, int arg, int arg2)
        {
            template_stack.Peek().state.emit2(opAST, opcode, arg, arg2);
        }

        public void emit2(CommonTree opAST, short opcode, string s, int arg2)
        {
            template_stack.Peek().state.emit2(opAST, opcode, s, arg2);
        }

        public void emit(short opcode)
        {
            template_stack.Peek().state.emit(opcode);
        }

        public void emit(CommonTree opAST, short opcode)
        {
            template_stack.Peek().state.emit(opAST, opcode);
        }

        public void insert(int addr, short opcode, string s)
        {
            template_stack.Peek().state.insert(addr, opcode, s);
        }

        public void setOption(CommonTree id)
        {
            template_stack.Peek().state.setOption(id);
        }

        public void write(int addr, short value)
        {
            template_stack.Peek().state.write(addr, value);
        }

        public int address()
        {
            return template_stack.Peek().state.ip;
        }

        public void func(CommonTree id)
        {
            template_stack.Peek().state.func(templateToken, id);
        }

        public void refAttr(CommonTree id)
        {
            template_stack.Peek().state.refAttr(templateToken, id);
        }

        public int defineString(string s)
        {
            return template_stack.Peek().state.defineString(s);
        }
    }
}
