/*
 * [The "BSD licence"]
 * Copyright (c) 2005-2008 Terence Parr
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
    using CLSCompliantAttribute = System.CLSCompliantAttribute;
    using CodeGenerator = Antlr3.Codegen.CodeGenerator;
    using CultureInfo = System.Globalization.CultureInfo;
    using Grammar = Antlr3.Tool.Grammar;
    using IAttributeRenderer = Antlr3.ST.IAttributeRenderer;
    using StringTemplate = Antlr3.ST.StringTemplate;
    using Target = Antlr3.Codegen.Target;

    public class CSharp3Target : Target
    {
        private static readonly HashSet<string> _languageKeywords = new HashSet<string>()
            {
                "abstract", "event", "new", "struct",
                "as", "explicit", "null", "switch",
                "base", "extern", "object", "this",
                "bool", "false", "operator", "throw",
                "break", "finally", "out", "true",
                "byte", "fixed", "override", "try",
                "case", "float", "params", "typeof",
                "catch", "for", "private", "uint",
                "char", "foreach", "protected", "ulong",
                "checked", "goto", "public", "unchecked",
                "class", "if", "readonly", "unsafe",
                "const", "implicit", "ref", "ushort",
                "continue", "in", "return", "using",
                "decimal", "int", "sbyte", "virtual",
                "default", "interface", "sealed", "volatile",
                "delegate", "internal", "short", "void",
                "do", "is", "sizeof", "while",
                "double", "lock", "stackalloc",
                "else", "long", "static",
                "enum", "namespace", "string",
            };

        public override string EncodeIntAsCharEscape( int v )
        {
            return "\\x" + v.ToString( "X" );
        }

        [CLSCompliant(false)]
        public override string GetTarget64BitStringFromValue(ulong word)
        {
            return "0x" + word.ToString( "X" );
        }

        protected override void GenRecognizerFile(AntlrTool tool, CodeGenerator generator, Grammar grammar, StringTemplate outputFileST)
        {
            if (!grammar.IsRoot)
            {
                Grammar rootGrammar = grammar.composite.RootGrammar;
                string actionScope = grammar.GetDefaultActionScope(grammar.type);
                IDictionary<string, object> actions;
                object rootNamespace;
                if (rootGrammar.Actions.TryGetValue(actionScope, out actions) && actions.TryGetValue("namespace", out rootNamespace))
                {
                    if (!grammar.Actions.TryGetValue(actionScope, out actions))
                    {
                        actions = new Dictionary<string, object>();
                        grammar.Actions[actionScope] = actions;
                    }

                    actions["namespace"] = rootNamespace;
                }
            }

            generator.Templates.RegisterRenderer(typeof(string), new StringRenderer(generator, this));
            base.GenRecognizerFile(tool, generator, grammar, outputFileST);
        }

        public class StringRenderer : IAttributeRenderer
        {
            private readonly CodeGenerator _generator;
            private readonly CSharp3Target _target;

            public StringRenderer(CodeGenerator generator, CSharp3Target target)
            {
                _generator = generator;
                _target = target;
            }

            public string ToString(string value)
            {
                return value;
            }

            public string ToString(string value, string formatName)
            {
                if (string.IsNullOrEmpty(value))
                    return value;

                switch (formatName)
                {
                case "id":
                    if (_languageKeywords.Contains(value))
                        return "@" + value;

                    return value;

                case "cap":
                    return char.ToUpper(value[0], CultureInfo.CurrentCulture) + value.Substring(1);

                case "string":
                    return _target.GetTargetStringLiteralFromString(value, true);

                default:
                    throw new ArgumentException(string.Format("Unsupported format name: '{0}'", formatName), "formatName");
                }
            }

            string IAttributeRenderer.ToString(object o)
            {
                return (string)o;
            }

            string IAttributeRenderer.ToString(object o, string formatName)
            {
                if (formatName == null)
                    throw new ArgumentNullException("formatName");

                return ToString((string)o, formatName);
            }
        }
    }
}
