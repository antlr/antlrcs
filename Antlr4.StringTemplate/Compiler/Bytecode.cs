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
        INSTR_LOAD_STR,
        INSTR_LOAD_ATTR,
        INSTR_LOAD_LOCAL, // load stuff like it, i, i0
        INSTR_LOAD_PROP,
        INSTR_LOAD_PROP_IND,
        INSTR_STORE_OPTION,
        INSTR_STORE_ARG,
        INSTR_NEW,  // create new template instance
        INSTR_NEW_IND,  // create new instance using value on stack
        INSTR_NEW_BOX_ARGS, // create new instance using args in Map on stack
        INSTR_SUPER_NEW,  // create new instance using value on stack
        INSTR_SUPER_NEW_BOX_ARGS, // create new instance using args in Map on stack
        INSTR_WRITE,
        INSTR_WRITE_OPT,
        INSTR_MAP,  // <a:b()>, <a:b():c()>, <a:{...}>
        INSTR_ROT_MAP,  // <a:b(),c()>
        INSTR_ZIP_MAP,  // <names,phones:{n,p | ...}>
        INSTR_BR,
        INSTR_BRF,
        INSTR_OPTIONS,  // push options map
        INSTR_ARGS,  // push args map
        INSTR_PASSTHRU,
        //INSTR_PASSTHRU_IND,
        INSTR_LIST,
        INSTR_ADD,
        INSTR_TOSTR,

        // Predefined functions
        INSTR_FIRST,
        INSTR_LAST,
        INSTR_REST,
        INSTR_TRUNC,
        INSTR_STRIP,
        INSTR_TRIM,
        INSTR_LENGTH,
        INSTR_STRLEN,
        INSTR_REVERSE,

        INSTR_NOT,
        INSTR_OR,
        INSTR_AND,

        INSTR_INDENT,
        INSTR_DEDENT,
        INSTR_NEWLINE,

        INSTR_NOOP, // do nothing
        INSTR_POP,
        INSTR_NULL, // push null value
        INSTR_TRUE, // push true
        INSTR_FALSE,

        // Combined instructions

        INSTR_WRITE_STR, // load_str n, write
        INSTR_WRITE_LOCAL, // TODO load_local n, write
    }
}
