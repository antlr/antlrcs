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
    public enum Bytecode : byte
    {
        Invalid = 0,

        // INSTRUCTION BYTECODES (byte is signed; use a short to keep 0..255)
        INSTR_LOAD_STR = 1,
        INSTR_LOAD_ATTR = 2,
        INSTR_LOAD_LOCAL = 3, // load stuff like it, i, i0
        INSTR_LOAD_PROP = 4,
        INSTR_LOAD_PROP_IND = 5,
        INSTR_STORE_OPTION = 6,
        INSTR_STORE_ARG = 7,
        INSTR_NEW = 8,  // create new template instance
        INSTR_NEW_IND = 9,  // create new instance using value on stack
        INSTR_NEW_BOX_ARGS = 10, // create new instance using args in Map on stack
        INSTR_SUPER_NEW = 11,  // create new instance using value on stack
        INSTR_SUPER_NEW_BOX_ARGS = 12, // create new instance using args in Map on stack
        INSTR_WRITE = 13,
        INSTR_WRITE_OPT = 14,
        INSTR_MAP = 15,  // <a:b()>, <a:b():c()>, <a:{...}>
        INSTR_ROT_MAP = 16,  // <a:b(),c()>
        INSTR_ZIP_MAP = 17,  // <names,phones:{n,p | ...}>
        INSTR_BR = 18,
        INSTR_BRF = 19,
        INSTR_OPTIONS = 20,  // push options map
        INSTR_ARGS = 21,  // push args map
        INSTR_LIST = 22,
        INSTR_ADD = 23,
        INSTR_TOSTR = 24,

        // Predefined functions
        INSTR_FIRST = 25,
        INSTR_LAST = 26,
        INSTR_REST = 27,
        INSTR_TRUNC = 28,
        INSTR_STRIP = 29,
        INSTR_TRIM = 30,
        INSTR_LENGTH = 31,
        INSTR_STRLEN = 32,
        INSTR_REVERSE = 33,

        INSTR_NOT = 34,
        INSTR_OR = 35,
        INSTR_AND = 36,

        INSTR_INDENT = 37,
        INSTR_DEDENT = 38,
        INSTR_NEWLINE = 39,

        INSTR_NOOP = 40, // do nothing
        INSTR_POP = 41,
        INSTR_NULL = 42, // push null value
        INSTR_TRUE = 43, // push true
        INSTR_FALSE = 44,

        // Combined instructions

        INSTR_WRITE_STR = 45,
        INSTR_WRITE_LOCAL = 46,
    }
}
