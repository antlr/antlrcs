/*
 * [The "BSD licence"]
 *  Copyright (c) 2010 Terence Parr and Alan Condit
 *  Copyright (c) 2006 Kay Roepke (Objective-C runtime)
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
    using CodeGenerator = Antlr3.Codegen.CodeGenerator;
    using Grammar = Antlr3.Tool.Grammar;
    using StringBuilder = System.Text.StringBuilder;
    using StringTemplate = Antlr3.ST.StringTemplate;
    using Target = Antlr3.Codegen.Target;

    public class ObjCTarget : Target
    {
        protected override void GenRecognizerHeaderFile( AntlrTool tool,
                                               CodeGenerator generator,
                                               Grammar grammar,
                                               StringTemplate headerFileST,
                                               string extName )
        {
            generator.Write( headerFileST, grammar.name + Grammar.grammarTypeToFileNameSuffix[(int)grammar.type] + extName );
        }

        public override string GetTargetCharLiteralFromANTLRCharLiteral( CodeGenerator generator,
                                                               string literal )
        {
            if ( literal.StartsWith( "'\\u" ) )
            {
                literal = "0x" + literal.Substring( 3, 4 );
            }
            else
            {
                int c = literal[1]; // TJP
                if ( c < 32 || c > 127 )
                {
                    literal = "0x" + c.ToString( "x" );
                }
            }

            return literal;
        }

        /** Convert from an ANTLR string literal found in a grammar file to
        *  an equivalent string literal in the target language.  For Java, this
        *  is the translation 'a\n"' -> "a\n\"".  Expect single quotes
        *  around the incoming literal.  Just flip the quotes and replace
        *  double quotes with \"
        */
        public override string GetTargetStringLiteralFromANTLRStringLiteral( CodeGenerator generator,
                                                                   string literal )
        {
            literal = literal.Replace( "\"", "\\\"" );
            StringBuilder buf = new StringBuilder( literal );
            buf[0] = '"';
            buf[literal.Length - 1] = '"';
            buf.Insert( 0, '@' );
            return buf.ToString();
        }

        /** If we have a label, prefix it with the recognizer's name */
        public override string GetTokenTypeAsTargetLabel( CodeGenerator generator, int ttype )
        {
            string name = generator.grammar.GetTokenDisplayName( ttype );
            // If name is a literal, return the token type instead
            if ( name[0] == '\'' )
            {
                return ttype.ToString();
            }
            return name;
            //return generator.grammar.name + Grammar.grammarTypeToFileNameSuffix[(int)generator.grammar.type] + "_" + name;
            //return super.getTokenTypeAsTargetLabel(generator, ttype);
            //return this.getTokenTextAndTypeAsTargetLabel(generator, null, ttype);
        }

        /** Target must be able to override the labels used for token types. Sometimes also depends on the token text.*/
        string GetTokenTextAndTypeAsTargetLabel( CodeGenerator generator, string text, int tokenType )
        {
            string name = generator.grammar.GetTokenDisplayName( tokenType );
            // If name is a literal, return the token type instead
            if ( name[0] == '\'' )
            {
                return tokenType.ToString();
            }
            string textEquivalent = text == null ? name : text;
            if ( textEquivalent[0] >= '0' && textEquivalent[0] <= '9' )
            {
                return textEquivalent;
            }
            else
            {
                return generator.grammar.name + Grammar.grammarTypeToFileNameSuffix[(int)generator.grammar.type] + "_" + textEquivalent;
            }
        }

    }

}
