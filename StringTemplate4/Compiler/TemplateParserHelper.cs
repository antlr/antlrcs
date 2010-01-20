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

namespace StringTemplate.Compiler
{
    using System.Collections.Generic;
    using Antlr.Runtime;
    using Console = System.Console;
    using ArgumentNullException = System.ArgumentNullException;

    partial class TemplateParser
    {
        /** The name of the template we are compiling or the name of the
         *  enclosing template.  This template could be a subtemplate or region of
         *  an enclosing template.
         */
        private TemplateName _enclosingTemplateName;

        private static ICodeGenerator NoopGen = new CodeGenerator();
        private ICodeGenerator gen = NoopGen;

        public TemplateParser(ITokenStream input, ICodeGenerator gen, TemplateName enclosingTemplateName)
            : this(input, new RecognizerSharedState(), gen, enclosingTemplateName)
        {
        }

        public TemplateParser(ITokenStream input, RecognizerSharedState state, ICodeGenerator gen, TemplateName enclosingTemplateName)
            : base(null, null) // overcome bug in ANTLR 3.2
        {
            this.input = input;
            this.state = state;
            if (gen != null)
                this.gen = gen;
            this._enclosingTemplateName = enclosingTemplateName;
        }

        public TemplateName PrefixedName(TemplateName t)
        {
            if (t == null)
                return null;

            return TemplateName.Combine(gen.TemplateReferencePrefix, t);
        }

        public void RefAttr(IToken id)
        {
            string name = id.Text;
            if (Interpreter.predefinedAttributes.Contains(name))
            {
                gen.Emit(Bytecode.INSTR_LOAD_LOCAL, name, id.StartIndex, id.StopIndex);
            }
            else
            {
                gen.Emit(Bytecode.INSTR_LOAD_ATTR, name, id.StartIndex, id.StopIndex);
            }
        }

        public void SetOption(IToken id)
        {
            int i;
            if (!TemplateCompiler.supportedOptions.TryGetValue(id.Text, out i))
            {
                ErrorManager.CompileTimeError(ErrorType.NoSuchOption, id);
                return;
            }

            gen.Emit(Bytecode.INSTR_STORE_OPTION, i, id.StartIndex, id.StopIndex);
        }

        public void DefaultOption(IToken id)
        {
            string v;
            if (!TemplateCompiler.defaultOptionValues.TryGetValue(id.Text, out v))
            {
                ErrorManager.CompileTimeError(ErrorType.NoDefaultValue, id);
                return;
            }

            gen.Emit(Bytecode.INSTR_LOAD_STR, v, id.StartIndex, id.StopIndex);
        }

        public void Func(IToken id)
        {
            short funcBytecode;
            if (!TemplateCompiler.funcs.TryGetValue(id.Text, out funcBytecode))
            {
                ErrorManager.CompileTimeError(ErrorType.NoSuchFunction, id);
                gen.Emit(Bytecode.INSTR_NOOP, id.StartIndex, id.StopIndex);
                return;
            }

            gen.Emit(funcBytecode, id.StartIndex, id.StopIndex);
        }

        public void Indent(string indent)
        {
            gen.Emit(Bytecode.INSTR_INDENT, indent);
        }

        protected override object RecoverFromMismatchedToken(IIntStream input, int ttype, BitSet follow)
        {
            throw new MismatchedTokenException(ttype, input);
        }

        /// <summary>
        /// used to parse w/o compilation side-effects
        /// </summary>
        private sealed class CodeGenerator : ICodeGenerator
        {
            public TemplateName TemplateReferencePrefix
            {
                get
                {
                    return null;
                }
            }

            public void Emit(short opcode)
            {
            }

            public void Emit(short opcode, int startIndex, int stopIndex)
            {
            }

            public void Emit(short opcode, int arg)
            {
            }

            public void Emit(short opcode, int arg, int startIndex, int stopIndex)
            {
            }

            public void Emit(short opcode, int arg1, int arg2, int startIndex, int stopIndex)
            {
            }

            public void Emit(short opcode, string s)
            {
            }

            public void Emit(short opcode, string s, int startIndex, int stopIndex)
            {
            }

            public void Write(int addr, short value)
            {
            }

            public int Address()
            {
                return 0;
            }

            public TemplateName CompileAnonTemplate(TemplateName enclosingTemplateName, ITokenStream input, IList<IToken> ids, RecognizerSharedState state)
            {
                TemplateCompiler c = new TemplateCompiler();
                c.Compile(input, state);
                return null;
            }

            public TemplateName CompileRegion(TemplateName enclosingTemplateName, string regionName, ITokenStream input, RecognizerSharedState state)
            {
                TemplateCompiler c = new TemplateCompiler();
                c.Compile(input, state);
                return null;
            }

            public void DefineBlankRegion(TemplateName fullyQualifiedName)
            {
            }
        }
    }
}
