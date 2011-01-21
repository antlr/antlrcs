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
    using StringBuilder = System.Text.StringBuilder;
    using Antlr4.StringTemplate.Misc;
    using ArgumentException = System.ArgumentException;

    public class BytecodeDisassembler
    {
        private readonly CompiledST code;

        public BytecodeDisassembler(CompiledST code)
        {
            this.code = code;
        }

        public virtual string instrs()
        {
            StringBuilder buf = new StringBuilder();
            int ip = 0;
            while (ip < code.codeSize)
            {
                if (ip > 0)
                    buf.Append(", ");
                int opcode = code.instrs[ip];
                Bytecode.Instruction I = Bytecode.instructions[opcode];
                buf.Append(I.name);
                ip++;
                for (int opnd = 0; opnd < I.nopnds; opnd++)
                {
                    buf.Append(' ');
                    buf.Append(getShort(code.instrs, ip));
                    ip += Bytecode.OPND_SIZE_IN_BYTES;
                }
            }
            return buf.ToString();
        }

        public virtual string disassemble()
        {
            StringBuilder buf = new StringBuilder();
            int i = 0;
            while (i < code.codeSize)
            {
                i = disassembleInstruction(buf, i);
                buf.AppendLine();
            }
            return buf.ToString();
        }

        public virtual int disassembleInstruction(StringBuilder buf, int ip)
        {
            int opcode = code.instrs[ip];
            if (ip >= code.codeSize)
            {
                throw new ArgumentException("ip out of range: " + ip);
            }
            Bytecode.Instruction I =
                Bytecode.instructions[opcode];
            if (I == null)
            {
                throw new ArgumentException("no such instruction " + opcode + " at address " + ip);
            }
            string instrName = I.name;
            buf.Append(string.Format("{0:0000}:\t{1,-14}", ip, instrName));
            ip++;
            if (I.nopnds == 0)
            {
                buf.Append("  ");
                return ip;
            }

            List<string> operands = new List<string>();
            for (int i = 0; i < I.nopnds; i++)
            {
                int opnd = getShort(code.instrs, ip);
                ip += Bytecode.OPND_SIZE_IN_BYTES;
                switch (I.type[i])
                {
                case Bytecode.OperandType.STRING:
                    operands.Add(showConstPoolOperand(opnd));
                    break;

                case Bytecode.OperandType.ADDR:
                case Bytecode.OperandType.INT:
                    operands.Add(opnd.ToString());
                    break;

                default:
                    operands.Add(opnd.ToString());
                    break;
                }
            }

            for (int i = 0; i < operands.Count; i++)
            {
                string s = operands[i];
                if (i > 0)
                    buf.Append(", ");

                buf.Append(s);
            }
            return ip;
        }

        private string showConstPoolOperand(int poolIndex)
        {
            StringBuilder buf = new StringBuilder();
            buf.Append("#");
            buf.Append(poolIndex);
            string s = "<bad string index>";
            if (poolIndex < code.strings.Length)
            {
                if (code.strings[poolIndex] == null)
                    s = "null";
                else
                {
                    s = code.strings[poolIndex].ToString();
                    if (code.strings[poolIndex] is string)
                    {
                        s = Utility.replaceEscapes(s);
                        s = '"' + s + '"';
                    }
                }
            }
            buf.Append(":");
            buf.Append(s);
            return buf.ToString();
        }

        public static int getShort(byte[] memory, int index)
        {
            int b1 = memory[index] & 0xFF; // mask off sign-extended bits
            int b2 = memory[index + 1] & 0xFF;
            int word = b1 << (8 * 1) | b2;
            return word;
        }

        public virtual string strings()
        {
            StringBuilder buf = new StringBuilder();
            int addr = 0;
            if (code.strings != null)
            {
                foreach (object o in code.strings)
                {
                    if (o is string)
                    {
                        string s = (string)o;
                        s = Utility.replaceEscapes(s);
                        buf.AppendLine(string.Format("{0:0000}: \"{1}\"", addr, s));
                    }
                    else
                    {
                        buf.AppendLine(string.Format("{0:0000}: {1}", addr, o));
                    }
                    addr++;
                }
            }
            return buf.ToString();
        }

        public virtual string sourceMap()
        {
            StringBuilder buf = new StringBuilder();
            int addr = 0;
            foreach (Interval I in code.sourceMap)
            {
                if (I != null)
                {
                    string chunk = code.template.Substring(I.A, I.B + 1 - I.A);
                    buf.AppendLine(string.Format("{0:0000}: {1}\t\"{2}\"", addr, I, chunk));
                }
                addr++;
            }

            return buf.ToString();
        }
    }
}
