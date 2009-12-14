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
    public class Bytecode
    {
        public const int MaxOperands = 3;
        public const int OPND_SIZE_IN_BYTES = 2;
        public const int STRING = 1;
        public const int ADDR = 2;
        public const int INT = 3;

        public class Instruction
        {
            internal string name; // E.g., "load_str", "new"
            internal int[] type = new int[MaxOperands];
            internal int n = 0;

            public Instruction(string name)
                : this(name, 0, 0, 0)
            {
                n = 0;
            }

            public Instruction(string name, int a)
                : this(name, a, 0, 0)
            {
                n = 1;
            }

            public Instruction(string name, int a, int b)
                : this(name, a, b, 0)
            {
                n = 2;
            }

            public Instruction(string name, int a, int b, int c)
            {
                this.name = name;
                type[0] = a;
                type[1] = b;
                type[2] = c;
                n = 3;
            }
        }

        // TODO: try an enum here

        // INSTRUCTION BYTECODES (byte is signed; use a short to keep 0..255)
        public const short INSTR_LOAD_STR = 1;
        public const short INSTR_LOAD_ATTR = 2;
        public const short INSTR_LOAD_LOCAL = 3; // load stuff like it, i, i0
        public const short INSTR_LOAD_PROP = 4;
        public const short INSTR_LOAD_PROP_IND = 5;
        public const short INSTR_STORE_ATTR = 6;
        public const short INSTR_STORE_SOLE_ARG = 7;
        public const short INSTR_SET_PASS_THRU = 8;
        public const short INSTR_STORE_OPTION = 9;
        public const short INSTR_NEW = 10;  // create new template instance
        public const short INSTR_NEW_IND = 11;  // create new template instance using value on stack
        public const short INSTR_SUPER_NEW = 12;  // create new template instance using value on stack
        public const short INSTR_WRITE = 13;
        public const short INSTR_WRITE_OPT = 14;
        public const short INSTR_MAP = 15;  // <a:b()>, <a:b():c()>, <a:{...}>
        public const short INSTR_ROT_MAP = 16;  // <a:b(),c()>
        public const short INSTR_PAR_MAP = 17;  // <names,phones:{n,p | ...}>
        public const short INSTR_BR = 18;
        public const short INSTR_BRF = 19;
        public const short INSTR_OPTIONS = 20;  // push options block
        public const short INSTR_LIST = 21;
        public const short INSTR_ADD = 22;
        public const short INSTR_TOSTR = 23;
        // Predefined functions
        public const short INSTR_NOOP = 24; // do nothing
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

        /** Used for assembly/disassembly; describes instruction set */
        // START: instr
        public static Instruction[] instructions =
            new Instruction[]
            {
                null, // <INVALID>
                new Instruction("load_str",STRING), // index is the opcode
                new Instruction("load_attr",STRING),
                new Instruction("load_local",STRING),
                new Instruction("load_prop",STRING),
                new Instruction("load_prop_ind"),
                new Instruction("store_attr",STRING),
                new Instruction("store_sole_arg"),
                new Instruction("set_pass_thru"),
                new Instruction("store_option",INT),
                new Instruction("new",STRING),
                new Instruction("new_ind"),
                new Instruction("super_new",STRING),
                new Instruction("write", INT, INT), // write delimiter-start-in-template, stop
                new Instruction("write_opt", INT, INT),
                new Instruction("map"),
                new Instruction("rot_map", INT),
                new Instruction("par_map", INT),
                new Instruction("br", ADDR),
                new Instruction("brf", ADDR),
                new Instruction("options"),
                new Instruction("list"),
                new Instruction("add"),
                new Instruction("tostr"),
                new Instruction("noop"),
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
                new Instruction("indent", STRING),
                new Instruction("dedent"),
                new Instruction("newline")
            };
        // END: instr
    }
}
