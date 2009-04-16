/*
 * [The "BSD licence"]
 * Copyright (c) 2008 Erik van Bilsen
 * Copyright (c) 2006 Kunle Odutola
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
    using CodeGenerator = Antlr3.Codegen.CodeGenerator;
    using Grammar = Antlr3.Tool.Grammar;
    using Label = Antlr3.Analysis.Label;
    using StringBuilder = System.Text.StringBuilder;
    using Target = Antlr3.Codegen.Target;

    public class DelphiTarget : Target
    {
        public DelphiTarget()
        {
            targetCharValueEscape['\n'] = "'#10'";
            targetCharValueEscape['\r'] = "'#13'";
            targetCharValueEscape['\t'] = "'#9'";
            targetCharValueEscape['\b'] = "\\b";
            targetCharValueEscape['\f'] = "\\f";
            targetCharValueEscape['\\'] = "\\";
            targetCharValueEscape['\''] = "''";
            targetCharValueEscape['"'] = "'";
        }

        public override string EncodeIntAsCharEscape( int v )
        {
            string hex = ( v <= 127 ) ? v.ToString( "x2" ) : v.ToString( "x4" );
            return "'#$" + hex + "'";
        }

        public override string GetTargetCharLiteralFromANTLRCharLiteral( CodeGenerator generator, string literal )
        {
            StringBuilder buf = new StringBuilder();
            int c = Grammar.getCharValueFromGrammarCharLiteral( literal );
            if ( c < Label.MIN_CHAR_VALUE )
            {
                return "0";
            }
            // normal char
            buf.Append( c );

            return buf.ToString();
        }

        public override string GetTargetStringLiteralFromString( string s, bool quoted )
        {
            if ( s == null )
            {
                return null;
            }
            StringBuilder buf = new StringBuilder();
            if ( quoted )
            {
                buf.Append( '\'' );
            }
            for ( int i = 0; i < s.Length; i++ )
            {
                int c = s[i];
                if ( c != '"' && // don't escape double quotes in strings for Delphi
                   c < targetCharValueEscape.Length &&
                   targetCharValueEscape[c] != null )
                {
                    buf.Append( targetCharValueEscape[c] );
                }
                else
                {
                    buf.Append( (char)c );
                }
                if ( ( i & 127 ) == 127 )
                {
                    // Concatenate string literals because Delphi doesn't support literals over 255 characters,
                    // and the code editor doesn't support lines over 1023 characters
                    buf.Append( "\' + \r\n  \'" );
                }
            }
            if ( quoted )
            {
                buf.Append( '\'' );
            }
            return buf.ToString();
        }

        public override string GetTargetStringLiteralFromANTLRStringLiteral( CodeGenerator generator, string literal )
        {
            literal = literal.Replace( "\\\'", "''" ); // \' to ' to normalize
            literal = literal.Replace( "\\r\\n", "'#13#10'" );
            literal = literal.Replace( "\\r", "'#13'" );
            literal = literal.Replace( "\\n", "'#10'" );
            StringBuilder buf = new StringBuilder( literal );
            buf[0] = '\'';
            buf[literal.Length - 1] = '\'';
            return buf.ToString();
        }

        public override string GetTarget64BitStringFromValue( ulong word )
        {
            int numHexDigits = 8 * 2;
            StringBuilder buf = new StringBuilder( numHexDigits + 2 );
            buf.Append( "$" );
            string digits = word.ToString( "X" );
            int padding = numHexDigits - digits.Length;
            // pad left with zeros
            for ( int i = 1; i <= padding; i++ )
            {
                buf.Append( '0' );
            }
            buf.Append( digits );
            return buf.ToString();
        }

    }
}
