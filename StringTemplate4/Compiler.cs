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
    using Antlr.Runtime;
    using Array = System.Array;
    using Console = System.Console;
    using IList = System.Collections.IList;
    using Math = System.Math;

    public class Compiler : ICodeGenerator
    {
        public static readonly string ATTR_NAME_REGEX = "[a-zA-Z/][a-zA-Z0-9_/]*";
        /** Given a template of length n, how much code will result?
         *  For now, let's assume n/5. Later, we can test in practice.
         */
        public static readonly double CODE_SIZE_FACTOR = 5.0;
        public static readonly int SUBTEMPLATE_INITIAL_CODE_SIZE = 15;

        public static readonly IDictionary<string, int> supportedOptions =
            new Dictionary<string, int>()
        {
            {        "anchor",       Interpreter.OPTION_ANCHOR},
            {        "format",       Interpreter.OPTION_FORMAT},
            {        "null",         Interpreter.OPTION_NULL},
            {       "separator",    Interpreter.OPTION_SEPARATOR},
            {"wrap",         Interpreter.OPTION_WRAP}
        };

        public static readonly int NUM_OPTIONS = supportedOptions.Count;

        public static readonly IDictionary<string, string> defaultOptionValues =
            new Dictionary<string, string>()
        {
            {"anchor", "true"},
            {"wrap",   "\n"}
        };

        public static IDictionary<string, short> funcs =
            new Dictionary<string, short>()
        {
            {"first", Bytecode.INSTR_FIRST},
            {"last", Bytecode.INSTR_LAST},
            {"rest", Bytecode.INSTR_REST},
            {"trunc", Bytecode.INSTR_TRUNC},
            {"strip", Bytecode.INSTR_STRIP},
            {"trim", Bytecode.INSTR_TRIM},
            {"length", Bytecode.INSTR_LENGTH},
            {"strlen", Bytecode.INSTR_STRLEN},
            {"reverse", Bytecode.INSTR_REVERSE}
        };

        StringTable strings = new StringTable();
        byte[] instrs;
        int ip = 0;
        CompiledTemplate code = new CompiledTemplate();

        public static int subtemplateCount = 0; // public for testing access

        public Compiler()
        {
        }

        public CompiledTemplate Compile(string template)
        {
            return Compile(template, '<', '>');
        }

        public CompiledTemplate Compile(string template,
                                  char delimiterStartChar,
                                  char delimiterStopChar)
        {
            int initialSize = Math.Max(5, (int)(template.Length / CODE_SIZE_FACTOR));
            instrs = new byte[initialSize];
            code.template = template;

            TemplateLexer lexer = new TemplateLexer(new ANTLRStringStream(template), delimiterStartChar, delimiterStopChar);
            UnbufferedTokenStream tokens = new UnbufferedTokenStream(lexer);
            TemplateParser parser = new TemplateParser(tokens, this);
            try
            {
                parser.templateAndEOF(); // parse, trigger compile actions for single expr
            }
            catch (RecognitionException re)
            {
                string msg = parser.GetErrorMessage(re, parser.TokenNames);
                Console.Error.WriteLine(re.StackTrace);
                throw new TemplateRecognitionException(msg, re);
            }

            if (strings != null)
                code.strings = strings.ToArray();
            code.instrs = instrs;
            code.codeSize = ip;
            return code;
        }

        public CompiledTemplate Compile(ITokenStream tokens, RecognizerSharedState state)
        {
            instrs = new byte[SUBTEMPLATE_INITIAL_CODE_SIZE];
            TemplateParser parser = new TemplateParser(tokens, state, this);
            try
            {
                parser.template(); // parse, trigger compile actions for single expr
            }
            catch (RecognitionException re)
            {
                string msg = parser.GetErrorMessage(re, parser.TokenNames);
                Console.Error.WriteLine(re.StackTrace);
                throw new TemplateRecognitionException(msg, re);
            }

            if (strings != null)
                code.strings = strings.ToArray();
            code.instrs = instrs;
            code.codeSize = ip;
            return code;
        }

        public int DefineString(string s)
        {
            return strings.Add(s);
        }

        // CodeGenerator interface impl.

        public void Emit(short opcode)
        {
            EnsureCapacity();
            instrs[ip++] = (byte)opcode;
        }

        public void Emit(short opcode, int arg)
        {
            EnsureCapacity();
            instrs[ip++] = (byte)opcode;
            WriteShort(instrs, ip, (short)arg);
            ip += 2;
        }

        public void Emit(short opcode, string s)
        {
            int i = DefineString(s);
            Emit(opcode, i);
        }

        public void Write(int addr, short value)
        {
            WriteShort(instrs, addr, value);
        }

        public int Address()
        {
            return ip;
        }

        public string CompileAnonTemplate(ITokenStream input,
                                   IList ids,
                                   RecognizerSharedState state)
        {
            subtemplateCount++;
            string name = "_sub" + subtemplateCount;
            Compiler c = new Compiler();
            CompiledTemplate sub = c.Compile(input, state);
            sub.name = name;
            if (ids != null)
            {
                sub.formalArguments = new Dictionary<string, FormalArgument>();
                foreach (IToken arg in ids)
                {
                    string argName = arg.Text;
                    sub.formalArguments[argName] = new FormalArgument(argName);
                }
            }
            if (code.compiledSubtemplates == null)
            {
                code.compiledSubtemplates = new List<CompiledTemplate>();
            }
            code.compiledSubtemplates.Add(sub);
            return name;
        }

        protected void EnsureCapacity()
        {
            if ((ip + 3) >= instrs.Length)
            { // ensure room for full instruction
                byte[] c = new byte[instrs.Length * 2];
                Array.Copy(instrs, 0, c, 0, instrs.Length);
                instrs = c;
            }
        }

        /** Write value at index into a byte array highest to lowest byte,
         *  left to right.
         */
        public static void WriteShort(byte[] memory, int index, short value)
        {
            memory[index + 0] = (byte)((value >> (8 * 1)) & 0xFF);
            memory[index + 1] = (byte)(value & 0xFF);
        }
    }
}
