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
    using ArgumentException = System.ArgumentException;
    using StringBuilder = System.Text.StringBuilder;

    public class BytecodeDisassembler
    {
        // TODO: make disassembler point at compiledST code?
        byte[] code;
        int codeSize;
        protected object[] strings;
        Bytecode def;

        public BytecodeDisassembler(byte[] code,
                                    int codeSize,
                                    string[] strings)
        {
            this.code = code;
            this.codeSize = codeSize;
            this.strings = strings;
        }

        public virtual string Instructions()
        {
            StringBuilder buf = new StringBuilder();
            int ip = 0;
            while (ip < codeSize)
            {
                if (ip > 0)
                    buf.Append(", ");
                int opcode = code[ip];
                Bytecode.Instruction I = Bytecode.instructions[opcode];
                buf.Append(I.name);
                ip++;
                for (int opnd = 0; opnd < I.n; opnd++)
                {
                    buf.Append(' ');
                    buf.Append(GetShort(code, ip));
                    ip += Bytecode.OPND_SIZE_IN_BYTES;
                }
            }
            return buf.ToString();
        }

        public virtual string Disassemble()
        {
            StringBuilder buf = new StringBuilder();
            int i = 0;
            while (i < codeSize)
            {
                i = DisassembleInstruction(buf, i);
                buf.Append('\n');
            }
            return buf.ToString();
        }

        public virtual int DisassembleInstruction(StringBuilder buf, int ip)
        {
            int opcode = code[ip];
            if (ip >= codeSize)
            {
                throw new ArgumentException("ip out of range: " + ip);
            }
            Bytecode.Instruction I =
                Bytecode.instructions[opcode];
            if (I == null)
            {
                throw new ArgumentException("no such instruction " + opcode +
                    " at address " + ip);
            }
            string instrName = I.name;
            buf.Append(string.Format("%04d:\t%-14s", ip, instrName));
            ip++;
            if (I.n == 0)
            {
                buf.Append(string.Format("  "));
                return ip;
            }
            List<string> operands = new List<string>();
            for (int i = 0; i < I.n; i++)
            {
                int opnd = GetShort(code, ip);
                ip += Bytecode.OPND_SIZE_IN_BYTES;
                switch (I.type[i])
                {
                case Bytecode.STRING:
                    operands.Add(ShowConstPoolOperand(opnd));
                    break;
                case Bytecode.ADDR:
                case Bytecode.INT:
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

        private string ShowConstPoolOperand(int poolIndex)
        {
            StringBuilder buf = new StringBuilder();
            buf.Append("#");
            buf.Append(poolIndex);
            string s = "<bad string index>";
            if (poolIndex < strings.Length)
            {
                if (strings[poolIndex] == null)
                    s = "null";
                else
                {
                    s = strings[poolIndex].ToString();
                    if (strings[poolIndex] is string)
                    {
                        s = s.Replace("\n", "\\\\n");
                        s = s.Replace("\r", "\\\\r");
                        s = s.Replace("\t", "\\\\t");
                        s = '"' + s + '"';
                    }
                }
            }
            buf.Append(":");
            buf.Append(s);
            return buf.ToString();
        }

        public static int GetShort(byte[] memory, int index)
        {
            int b1 = memory[index++] & 0xFF; // mask off sign-extended bits
            int b2 = memory[index++] & 0xFF;
            int word = b1 << (8 * 1) | b2;
            return word;
        }

        public virtual string Strings()
        {
            StringBuilder buf = new StringBuilder();
            int addr = 0;
            foreach (object o in strings)
            {
                if (o is string)
                {
                    string s = (string)o;
                    s = s.Replace("\n", "\\\\n");
                    s = s.Replace("\r", "\\\\r");
                    s = s.Replace("\t", "\\\\t");
                    buf.Append(string.Format("%04d: \"%s\"\n", addr, s));
                }
                else
                {
                    buf.Append(string.Format("%04d: %s\n", addr, o));
                }
                addr++;
            }
            return buf.ToString();
        }
    }
}
