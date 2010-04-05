/*
 * [The "BSD licence"]
 * Copyright (c) 2005 Martin Traverso
 * All rights reserved.
 *
 * Conversion to C#:
 * Copyright (c) 2008-2009 Sam Harwell, Pixel Mine, Inc.
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

namespace Antlr3.Targets
{
    using System.Collections.Generic;

    using ArgumentException = System.ArgumentException;
    using ArgumentNullException = System.ArgumentNullException;
    using CodeGenerator = Antlr3.Codegen.CodeGenerator;
    using Console = System.Console;
    using Grammar = Antlr3.Tool.Grammar;
    using GrammarType = Antlr3.Tool.GrammarType;
    using IAttributeRenderer = Antlr3.ST.IAttributeRenderer;
    using Regex = System.Text.RegularExpressions.Regex;
    using StringBuilder = System.Text.StringBuilder;
    using StringTemplate = Antlr3.ST.StringTemplate;
    using StringTemplateGroup = Antlr3.ST.StringTemplateGroup;
    using Target = Antlr3.Codegen.Target;
    using TypeLoadException = System.TypeLoadException;

    public class RubyTarget : Target
    {
        public static readonly HashSet<string> rubyKeywords =
            new HashSet<string>()
            {
                "alias",    "end",     "retry", 
                "and",      "ensure",  "return",
                "BEGIN",    "false",   "self",  
                "begin",    "for",     "super", 
                "break",    "if",      "then",  
                "case",     "in",      "true",  
                "class",    "module",  "undef", 
                "def",      "next",    "unless",
                "defined",  "nil",     "until", 
                "do",       "not",     "when",  
                "else",     "or",      "while", 
                "elsif",    "redo",    "yield", 
                "END",      "rescue",
            };

        public class RubyRenderer : IAttributeRenderer
        {
            public string ToString(object o)
            {
                return o.ToString();
            }

            public string ToString(object o, string formatName)
            {
                if (formatName == null)
                    throw new ArgumentNullException("formatName");

                string idString = o.ToString();
                if (string.IsNullOrEmpty(idString))
                    return idString;

                switch (formatName)
                {
                case "snakecase":
                    return SnakeCase(idString);
                case "camelcase":
                    return CamelCase(idString);
                case "subcamelcase":
                    return SubCamelCase(idString);
                case "constant":
                    return ConstantCase(idString);
                case "platform":
                    return Platform(idString);
                case "lexerRule":
                    return LexerRule(idString);
                case "constantPath":
                    return ConstantPath(idString);
                case "label":
                    return Label(idString);
                case "symbol":
                    return Symbol(idString);
                default:
                    throw new ArgumentException(string.Format("Unsupported format name: '{0}'", formatName), "formatName");
                }
            }

            /* given an input string, which is presumed
             * to contain a word, which may potentially be camelcased,
             * and convert it to snake_case underscore style.
             *
             * algorithm --
             *   iterate through the string with a sliding window 3 chars wide
             *
             * example -- aGUIWhatNot
             *   c   c+1 c+2  action
             *   a   G        << 'a' << '_'  // a lower-upper word edge
             *   G   U   I    << 'g'
             *   U   I   W    << 'w'
             *   I   W   h    << 'i' << '_'  // the last character in an acronym run of uppers
             *   W   h        << 'w'
             *   ... and so on
             */
            private static string SnakeCase(string value)
            {
                if (string.IsNullOrEmpty(value))
                    return value;

                StringBuilder output_buffer = new StringBuilder();
                int l = value.Length;
                int cliff = l - 1;
                char cur;
                char next;
                char peek;

                if (l == 1)
                    return value.ToLowerInvariant();

                for (int i = 0; i < cliff; i++)
                {
                    cur = value[i];
                    next = value[i + 1];

                    if (char.IsLetter(cur))
                    {
                        output_buffer.Append(char.ToLowerInvariant(cur));

                        if (char.IsDigit(next) || char.IsWhiteSpace(next))
                        {
                            output_buffer.Append('_');
                        }
                        else if (char.IsLower(cur) && char.IsUpper(next))
                        {
                            // at camelcase word edge
                            output_buffer.Append('_');
                        }
                        else if ((i < cliff - 1) && char.IsUpper(cur) && char.IsUpper(next))
                        {
                            // cur is part of an acronym

                            peek = value[i + 2];
                            if (char.IsLower(peek))
                            {
                                /* if next is the start of word (indicated when peek is lowercase)
                                             then the acronym must be completed by appending an underscore */
                                output_buffer.Append('_');
                            }
                        }
                    }
                    else if (char.IsDigit(cur))
                    {
                        output_buffer.Append(cur);
                        if (char.IsLetter(next))
                        {
                            output_buffer.Append('_');
                        }
                    }
                    else if (char.IsWhiteSpace(cur))
                    {
                        // do nothing
                    }
                    else
                    {
                        output_buffer.Append(cur);
                    }

                }

                cur = value[cliff];
                if (!char.IsWhiteSpace(cur))
                {
                    output_buffer.Append(char.ToLowerInvariant(cur));
                }

                return output_buffer.ToString();
            }

            private static string ConstantCase(string value)
            {
                return SnakeCase(value).ToUpperInvariant();
            }

            private static string Platform(string value)
            {
                return "__" + value + "__";
            }

            private static string Symbol(string value)
            {
                if (Regex.IsMatch(value, @"[a-zA-Z_]\w*[\?\!\=]?"))
                    return ":" + value;

                return "%s(" + value + ")";
            }

            private static string LexerRule(string value)
            {
                if (value.Equals("Tokens"))
                    return "token!";

                return SnakeCase(value) + "!";
            }

            private static string ConstantPath(string value)
            {
                return value.Replace(".", "::");
            }

            private static string CamelCase(string value)
            {
                if (string.IsNullOrEmpty(value))
                    return value;

                StringBuilder output_buffer = new StringBuilder();
                int cliff = value.Length;
                char cur;
                bool at_edge = true;

                if (cliff == 1)
                    return value.ToUpperInvariant();

                for (int i = 0; i < cliff; i++)
                {
                    cur = value[i];

                    if (char.IsWhiteSpace(cur) || cur == '_')
                    {
                        at_edge = true;
                        continue;
                    }
                    else if (char.IsDigit(cur))
                    {
                        output_buffer.Append(cur);
                        at_edge = true;
                        continue;
                    }

                    if (at_edge)
                    {
                        output_buffer.Append(char.ToUpperInvariant(cur));
                        if (char.IsLetter(cur))
                            at_edge = false;
                    }
                    else
                    {
                        output_buffer.Append(cur);
                    }
                }

                return output_buffer.ToString();
            }

            private static string Label(string value)
            {
                if (rubyKeywords.Contains(value))
                {
                    return Platform(value);
                }
                else if (char.IsUpper(value[0]) && !value.Equals("FILE") && !value.Equals("LINE"))
                {
                    return Platform(value);
                }
                else if (value.Equals("FILE"))
                {
                    return "_FILE_";
                }
                else if (value.Equals("LINE"))
                {
                    return "_LINE_";
                }
                else
                {
                    return value;
                }
            }

            private static string SubCamelCase(string value)
            {
                if (string.IsNullOrEmpty(value))
                    return value;

                value = CamelCase(value);
                char head = char.ToLowerInvariant(value[0]);
                string tail = value.Substring(1);
                return head.ToString() + tail;
            }
        }

        protected override void GenRecognizerFile(AntlrTool tool, CodeGenerator generator, Grammar grammar, StringTemplate outputFileST)
        {
            generator.Templates.RegisterRenderer(typeof(string), new RubyRenderer());
            string fileName = generator.GetRecognizerFileName(grammar.name, grammar.type);
            generator.Write(outputFileST, fileName);
        }

        public override string GetTargetCharLiteralFromANTLRCharLiteral(CodeGenerator generator, string literal)
        {
            literal = literal.Substring(1, literal.Length - 2);

            string result = "?";

            if (literal.Equals(@"\"))
            {
                result += @"\\";
            }
            else if (literal.Equals(" "))
            {
                result += @"\s";
            }
            else if (literal.StartsWith(@"\u"))
            {
                result = "0x" + literal.Substring(2);
            }
            else
            {
                result += literal;
            }

            return result;
        }

        public override int GetMaxCharValue(CodeGenerator generator)
        {
            // Versions before 1.9 do not support unicode
            return 0xFF;
        }

        public override string GetTokenTypeAsTargetLabel(CodeGenerator generator, int ttype)
        {
            string name = generator.grammar.GetTokenDisplayName(ttype);
            // If name is a literal, return the token type instead
            if (name[0] == '\'')
            {
                return generator.grammar.ComputeTokenNameFromLiteral(ttype, name);
            }
            return name;
        }

        /** Is scope in @scope::name {action} valid for this kind of grammar?
         *  Targets like C++ may want to allow new scopes like headerfile or
         *  some such.  The action names themselves are not policed at the
         *  moment so targets can add template actions w/o having to recompile
         *  ANTLR.
         */
        public override bool IsValidActionScope(GrammarType grammarType, string scope)
        {
            switch (grammarType)
            {
            case GrammarType.Lexer:
                switch (scope)
                {
                case "lexer":
                case "token":
                case "module":
                case "overrides":
                    return true;
                }

                break;

            case GrammarType.Parser:
                switch (scope)
                {
                case "parser":
                case "token":
                case "module":
                case "overrides":
                    return true;
                }

                break;

            case GrammarType.Combined:
                switch (scope)
                {
                case "parser":
                case "lexer":
                case "token":
                case "module":
                case "overrides":
                    return true;
                }

                break;

            case GrammarType.TreeParser:
                switch (scope)
                {
                case "treeparser":
                case "token":
                case "module":
                case "overrides":
                    return true;
                }

                break;
            }
            return false;
        }

#if false
        public override string GetTargetStringLiteralFromString(string s)
        {
            Console.Write(s + "\n");
            return base.GetTargetStringLiteralFromString(s);
        }

        public override string GetTargetStringLiteralFromString(string s, bool quoted)
        {
            //Console.Write(s + "\n");
            string ret_value = base.GetTargetStringLiteralFromString(s, quoted);
            Console.Write(ret_value + "\n");
            return ret_value;
        }

        public override string GetTarget64BitStringFromValue(ulong word)
        {
            Console.Write(word.ToString() + "\n");
            string result = base.GetTarget64BitStringFromValue(word);
            Console.Write(result + "\n");
            return result;
        }
#endif

        public override string EncodeIntAsCharEscape(int v)
        {
            int intValue = (v == 65535) ? -1 : v;
            return intValue.ToString();
        }
    }
}
