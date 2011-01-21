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
    public class Bytecode
    {
        public const int MAX_OPNDS = 2;
        public const int OPND_SIZE_IN_BYTES = 2;

        public enum OperandType
        {
            NONE,
            STRING,
            ADDR,
            INT
        }

        public class Instruction
        {
            internal readonly string name; // E.g., "load_str", "new"
            internal readonly OperandType[] type = new OperandType[MAX_OPNDS];
            internal readonly int nopnds = 0;

            public Instruction(string name)
                : this(name, OperandType.NONE, OperandType.NONE)
            {
                nopnds = 0;
            }

            public Instruction(string name, OperandType a)
                : this(name, a, OperandType.NONE)
            {
                nopnds = 1;
            }

            public Instruction(string name, OperandType a, OperandType b)
            {
                this.name = name;
                type[0] = a;
                type[1] = b;
                nopnds = MAX_OPNDS;
            }
        }

        // don't use enum for efficiency; don't want CompiledST.instrs to
        // be an array of objects (Bytecode[]). We want it to be byte[].

        // INSTRUCTION BYTECODES (byte is signed; use a short to keep 0..255)
        public const short INSTR_LOAD_STR = 1;
        public const short INSTR_LOAD_ATTR = 2;
        public const short INSTR_LOAD_LOCAL = 3; // load stuff like it, i, i0
        public const short INSTR_LOAD_PROP = 4;
        public const short INSTR_LOAD_PROP_IND = 5;
        public const short INSTR_STORE_OPTION = 6;
        public const short INSTR_STORE_ARG = 7;
        public const short INSTR_NEW = 8;  // create new template instance
        public const short INSTR_NEW_IND = 9;  // create new instance using value on stack
        public const short INSTR_NEW_BOX_ARGS = 10; // create new instance using args in Map on stack
        public const short INSTR_SUPER_NEW = 11;  // create new instance using value on stack
        public const short INSTR_SUPER_NEW_BOX_ARGS = 12; // create new instance using args in Map on stack
        public const short INSTR_WRITE = 13;
        public const short INSTR_WRITE_OPT = 14;
        public const short INSTR_MAP = 15;  // <a:b()>, <a:b():c()>, <a:{...}>
        public const short INSTR_ROT_MAP = 16;  // <a:b(),c()>
        public const short INSTR_ZIP_MAP = 17;  // <names,phones:{n,p | ...}>
        public const short INSTR_BR = 18;
        public const short INSTR_BRF = 19;
        public const short INSTR_OPTIONS = 20;  // push options map
        public const short INSTR_ARGS = 21;  // push args map
        public const short INSTR_LIST = 22;
        public const short INSTR_ADD = 23;
        public const short INSTR_TOSTR = 24;

        // Predefined functions
        public const short INSTR_FIRST = 25;
        public const short INSTR_LAST = 26;
        public const short INSTR_REST = 27;
        public const short INSTR_TRUNC = 28;
        public const short INSTR_STRIP = 29;
        public const short INSTR_TRIM = 30;
        public const short INSTR_LENGTH = 31;
        public const short INSTR_STRLEN = 32;
        public const short INSTR_REVERSE = 33;

        public const short INSTR_NOT = 34;
        public const short INSTR_OR = 35;
        public const short INSTR_AND = 36;

        public const short INSTR_INDENT = 37;
        public const short INSTR_DEDENT = 38;
        public const short INSTR_NEWLINE = 39;

        public const short INSTR_NOOP = 40; // do nothing
        public const short INSTR_POP = 41;
        public const short INSTR_NULL = 42; // push null value

        /** Used for assembly/disassembly; describes instruction set */
        public static Instruction[] instructions =
            new Instruction[]
            {
                null, // <INVALID>
                new Instruction("load_str",OperandType.STRING), // index is the opcode
                new Instruction("load_attr",OperandType.STRING),
                new Instruction("load_local",OperandType.INT),
                new Instruction("load_prop",OperandType.STRING),
                new Instruction("load_prop_ind"),
                new Instruction("store_option",OperandType.INT),
                new Instruction("store_arg",OperandType.STRING),
                new Instruction("new",OperandType.STRING,OperandType.INT),
                new Instruction("new_ind",OperandType.INT),
                new Instruction("new_box_args",OperandType.STRING),
                new Instruction("super_new",OperandType.STRING,OperandType.INT),
                new Instruction("super_new_box_args",OperandType.STRING),
                new Instruction("write"),
                new Instruction("write_opt"),
                new Instruction("map"),
                new Instruction("rot_map", OperandType.INT),
                new Instruction("zip_map", OperandType.INT),
                new Instruction("br", OperandType.ADDR),
                new Instruction("brf", OperandType.ADDR),
                new Instruction("options"),
                new Instruction("args"),
                new Instruction("list"),
                new Instruction("add"),
                new Instruction("tostr"),
                new Instruction("first"),
                new Instruction("last"),
                new Instruction("rest"),
                new Instruction("trunc"),
                new Instruction("strip"),
                new Instruction("trim"),
                new Instruction("length"),
                new Instruction("strlen"),
                new Instruction("reverse"),
                new Instruction("not"),
                new Instruction("or"),
                new Instruction("and"),
                new Instruction("indent", OperandType.STRING),
                new Instruction("dedent"),
                new Instruction("newline"),
                new Instruction("noop"),
                new Instruction("pop"),
                new Instruction("null")
            };
    }
}
