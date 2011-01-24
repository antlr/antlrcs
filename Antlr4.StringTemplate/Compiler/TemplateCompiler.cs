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
    using Antlr4.StringTemplate.Misc;
    using Antlr.Runtime;
    using Antlr.Runtime.Tree;

    /** A compiler for a single template. */
    public class Compiler
    {
        public static readonly string SubtemplatePrefix = "_sub";

        public const int InitialCodeSize = 15;

        public static readonly IDictionary<string, Interpreter.Option> supportedOptions =
            new Dictionary<string, Interpreter.Option>()
            {
                {"anchor",       Interpreter.Option.Anchor},
                {"format",       Interpreter.Option.Format},
                {"null",         Interpreter.Option.Null},
                {"separator",    Interpreter.Option.Separator},
                {"wrap",         Interpreter.Option.Wrap},
            };

        public static readonly int NUM_OPTIONS = supportedOptions.Count;

        public static readonly IDictionary<string, string> defaultOptionValues =
            new Dictionary<string, string>()
            {
                {"anchor", "true"},
                {"wrap",   "\n"},
            };

        public static IDictionary<string, Bytecode> funcs =
            new Dictionary<string, Bytecode>()
            {
                {"first", Bytecode.INSTR_FIRST},
                {"last", Bytecode.INSTR_LAST},
                {"rest", Bytecode.INSTR_REST},
                {"trunc", Bytecode.INSTR_TRUNC},
                {"strip", Bytecode.INSTR_STRIP},
                {"trim", Bytecode.INSTR_TRIM},
                {"length", Bytecode.INSTR_LENGTH},
                {"strlen", Bytecode.INSTR_STRLEN},
                {"reverse", Bytecode.INSTR_REVERSE},
            };

        /** Name subtemplates _sub1, _sub2, ... */
        public static int subtemplateCount = 0;

        /** The compiler needs to know how to delimit expressions.
         *  The STGroup normally passes in this information, but we
         *  can set some defaults.
         */
        public char delimiterStartChar = '<'; // Use <expr> by default
        public char delimiterStopChar = '>';

        public ErrorManager errMgr;

        public Compiler()
            : this(STGroup.DefaultErrorManager)
        {
        }

        public Compiler(ErrorManager errMgr)
            : this(errMgr, '<', '>')
        {
        }

        public Compiler(char delimiterStartChar, char delimiterStopChar)
            : this(STGroup.DefaultErrorManager, delimiterStartChar, delimiterStopChar)
        {
        }

        /** To compile a template, we need to know what the
         *  enclosing template is (if any) in case of regions.
         */
        public Compiler(ErrorManager errMgr, char delimiterStartChar, char delimiterStopChar)
        {
            this.errMgr = errMgr;
            this.delimiterStartChar = delimiterStartChar;
            this.delimiterStopChar = delimiterStopChar;
        }

        public virtual CompiledST compile(string template)
        {
            CompiledST code = compile(null, null, null, template, null);
            code.hasFormalArgs = false;
            return code;
        }

        /** Compile full template with unknown formal args. */
        public virtual CompiledST compile(string name, string template)
        {
            CompiledST code = compile(null, name, null, template, null);
            code.hasFormalArgs = false;
            return code;
        }

        /** Compile full template with respect to a list of formal args. */
        public virtual CompiledST compile(string srcName, string name, List<FormalArgument> args, string template, IToken templateToken)
        {
            ANTLRStringStream @is = new ANTLRStringStream(template);
            @is.name = srcName != null ? srcName : name;
            STLexer lexer = new STLexer(errMgr, @is, templateToken, delimiterStartChar, delimiterStopChar);
            CommonTokenStream tokens = new CommonTokenStream(lexer);
            TemplateParser p = new TemplateParser(tokens, errMgr, templateToken);
            TemplateParser.templateAndEOF_return r = null;
            try
            {
                r = p.templateAndEOF();
            }
            catch (RecognitionException re)
            {
                reportMessageAndThrowSTException(tokens, templateToken, p, re);
                return null;
            }

            if (p.NumberOfSyntaxErrors > 0 || r.Tree == null)
            {
                CompiledST impl = new CompiledST();
                impl.defineFormalArgs(args);
                return impl;
            }

            //System.out.println(((CommonTree)r.getTree()).toStringTree());
            CommonTreeNodeStream nodes = new CommonTreeNodeStream(r.Tree);
            nodes.TokenStream = tokens;
            CodeGenerator gen = new CodeGenerator(nodes, errMgr, name, template, templateToken);

            CompiledST impl2 = null;
            try
            {
                impl2 = gen.template(name, args);
                // only save tree/token stream when debugging
                if (STGroup.debug)
                {
                    impl2.ast = (CommonTree)r.Tree;
                    impl2.ast.SetUnknownTokenBoundaries();
                    impl2.tokens = tokens;
                }
            }
            catch (RecognitionException re)
            {
                errMgr.internalError(null, "bad tree structure", re);
            }

            return impl2;
        }

        public static CompiledST defineBlankRegion(CompiledST outermostImpl, string name)
        {
            string outermostTemplateName = outermostImpl.name;
            string mangled = STGroup.getMangledRegionName(outermostTemplateName, name);
            CompiledST blank = new CompiledST();
            blank.isRegion = true;
            blank.regionDefType = ST.RegionType.Implicit;
            blank.name = mangled;
            outermostImpl.addImplicitlyDefinedTemplate(blank);
            return blank;
        }

        public static string getNewSubtemplateName()
        {
            subtemplateCount++;
            return SubtemplatePrefix + subtemplateCount;
        }

        protected virtual void reportMessageAndThrowSTException(ITokenStream tokens, IToken templateToken, Parser parser, RecognitionException re)
        {
            if (re.Token.Type == STLexer.EOF_TYPE)
            {
                string msg = "premature EOF";
                errMgr.compileTimeError(ErrorType.SYNTAX_ERROR, templateToken, re.Token, msg);
            }
            else if (re is NoViableAltException)
            {
                string msg = "'" + re.Token.Text + "' came as a complete surprise to me";
                errMgr.compileTimeError(ErrorType.SYNTAX_ERROR, templateToken, re.Token, msg);
            }
            else if (tokens.Index == 0)
            {
                // couldn't parse anything
                string msg = "this doesn't look like a template: \"" + tokens + "\"";
                errMgr.compileTimeError(ErrorType.SYNTAX_ERROR, templateToken, re.Token, msg);
            }
            else if (tokens.LA(1) == STLexer.LDELIM)
            {
                // couldn't parse expr
                string msg = "doesn't look like an expression";
                errMgr.compileTimeError(ErrorType.SYNTAX_ERROR, templateToken, re.Token, msg);
            }
            else
            {
                string msg = parser.GetErrorMessage(re, parser.TokenNames);
                errMgr.compileTimeError(ErrorType.SYNTAX_ERROR, templateToken, re.Token, msg);
            }

            throw new STException();
        }
    }
}
