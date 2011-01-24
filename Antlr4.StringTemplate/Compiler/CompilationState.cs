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
    using Array = System.Array;
    using Antlr.Runtime;
    using Antlr.Runtime.Tree;
    using Antlr4.StringTemplate.Misc;

    /** temp data used during construction and functions that fill it / use it.
     *  Result is impl CompiledTemplate object.
     */
    public class CompilationState
    {
        /** The compiled code implementation to fill in. */
        internal CompiledTemplate impl = new CompiledTemplate();

        /** Track unique strings; copy into CompiledTemplate's String[] after compilation */
        internal StringTable stringtable = new StringTable();

        /** Track instruction location within code.instrs array; this is
         *  next address to Write to.  Byte-addressable memory.
         */
        internal int ip = 0;

        internal ITokenStream tokens;

        internal ErrorManager errMgr;

        public CompilationState(ErrorManager errMgr, string name, ITokenStream tokens)
        {
            this.errMgr = errMgr;
            this.tokens = tokens;
            impl.name = name;
        }

        public virtual int defineString(string s)
        {
            return stringtable.Add(s);
        }

        public virtual void refAttr(IToken templateToken, CommonTree id)
        {
            string name = id.Text;
            FormalArgument arg = impl.TryGetFormalArgument(name);
            if (arg != null)
            {
                int index = arg.Index;
                emit1(id, Bytecode.INSTR_LOAD_LOCAL, index);
            }
            else
            {
                if (Interpreter.predefinedAnonSubtemplateAttributes.Contains(name))
                {
                    errMgr.compileTimeError(ErrorType.NO_SUCH_ATTRIBUTE, templateToken, id.token);
                    emit(id, Bytecode.INSTR_NULL);
                }
                else
                {
                    emit1(id, Bytecode.INSTR_LOAD_ATTR, name);
                }
            }
        }

        public virtual void setOption(CommonTree id)
        {
            Interpreter.Option O = TemplateCompiler.supportedOptions[id.Text];
            emit1(id, Bytecode.INSTR_STORE_OPTION, (int)O);
        }

        public virtual void func(IToken templateToken, CommonTree id)
        {
            Bytecode funcBytecode;
            if (!TemplateCompiler.funcs.TryGetValue(id.Text, out funcBytecode))
            {
                errMgr.compileTimeError(ErrorType.NO_SUCH_FUNCTION, templateToken, id.token);
                emit(id, Bytecode.INSTR_POP);
            }
            else
            {
                emit(id, funcBytecode);
            }
        }

        public virtual void emit(Bytecode opcode)
        {
            emit(null, opcode);
        }

        public virtual void emit(CommonTree opAST, Bytecode opcode)
        {
            ensureCapacity(1);
            if (opAST != null)
            {
                int i = opAST.TokenStartIndex;
                int j = opAST.TokenStopIndex;
                int p = tokens.Get(i).StartIndex;
                int q = tokens.Get(j).StopIndex;
                if (!(p < 0 || q < 0))
                    impl.sourceMap[ip] = new Interval(p, q);
            }
            impl.instrs[ip++] = (byte)opcode;
        }

        public virtual void emit1(CommonTree opAST, Bytecode opcode, int arg)
        {
            emit(opAST, opcode);
            ensureCapacity(Instruction.OperandSizeInBytes);
            writeShort(impl.instrs, ip, (short)arg);
            ip += Instruction.OperandSizeInBytes;
        }

        public virtual void emit2(CommonTree opAST, Bytecode opcode, int arg, int arg2)
        {
            emit(opAST, opcode);
            ensureCapacity(Instruction.OperandSizeInBytes * 2);
            writeShort(impl.instrs, ip, (short)arg);
            ip += Instruction.OperandSizeInBytes;
            writeShort(impl.instrs, ip, (short)arg2);
            ip += Instruction.OperandSizeInBytes;
        }

        public virtual void emit2(CommonTree opAST, Bytecode opcode, string s, int arg2)
        {
            int i = defineString(s);
            emit2(opAST, opcode, i, arg2);
        }

        public virtual void emit1(CommonTree opAST, Bytecode opcode, string s)
        {
            int i = defineString(s);
            emit1(opAST, opcode, i);
        }

        public virtual void insert(int addr, Bytecode opcode, string s)
        {
            //System.out.println("before insert of "+opcode+"("+s+"):"+ Arrays.toString(impl.instrs));
            ensureCapacity(1 + Instruction.OperandSizeInBytes);
            int instrSize = 1 + Instruction.OperandSizeInBytes;
            // make room for opcode, opnd
            Array.Copy(impl.instrs, addr, impl.instrs, addr + instrSize, ip - addr);
            int save = ip;
            ip = addr;
            emit1(null, opcode, s);
            ip = save + instrSize;
            //System.out.println("after  insert of "+opcode+"("+s+"):"+ Arrays.toString(impl.instrs));
            // adjust addresses for BR and BRF
            int a = addr + instrSize;
            while (a < ip)
            {
                Bytecode op = (Bytecode)impl.instrs[a];
                Instruction I = Instruction.instructions[(int)op];
                if (op == Bytecode.INSTR_BR || op == Bytecode.INSTR_BRF)
                {
                    int opnd = BytecodeDisassembler.getShort(impl.instrs, a + 1);
                    writeShort(impl.instrs, a + 1, (short)(opnd + instrSize));
                }
                a += I.nopnds * Instruction.OperandSizeInBytes + 1;
            }
            //System.out.println("after  insert of "+opcode+"("+s+"):"+ Arrays.toString(impl.instrs));
        }

        public virtual void write(int addr, short value)
        {
            writeShort(impl.instrs, addr, value);
        }

        protected virtual void ensureCapacity(int n)
        {
            if ((ip + n) >= impl.instrs.Length)
            {
                // ensure room for full instruction
                Array.Resize(ref impl.instrs, impl.instrs.Length * 2);
                Array.Resize(ref impl.sourceMap, impl.sourceMap.Length * 2);
            }
        }

        public virtual void indent(string indent)
        {
            emit1(null, Bytecode.INSTR_INDENT, indent);
        }

        /** Write value at index into a byte array highest to lowest byte,
         *  left to right.
         */
        public static void writeShort(byte[] memory, int index, short value)
        {
            memory[index + 0] = (byte)(value & 0xFF);
            memory[index + 1] = (byte)((value >> (8 * 1)) & 0xFF);
        }
    }
}
