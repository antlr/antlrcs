/*
 * [The "BSD licence"]
 * Copyright (c) 2007 Ronald Blaschke
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
    using AttributeScope = Antlr3.Tool.AttributeScope;
    using CodeGenerator = Antlr3.Codegen.CodeGenerator;
    using Grammar = Antlr3.Tool.Grammar;
    using Label = Antlr3.Analysis.Label;
    using RuleLabelScope = Antlr3.Tool.RuleLabelScope;
    using StringBuilder = System.Text.StringBuilder;
    using Target = Antlr3.Codegen.Target;

    public class Perl5Target : Target
    {
        public Perl5Target()
        {
            targetCharValueEscape['$'] = "\\$";
            targetCharValueEscape['@'] = "\\@";
            targetCharValueEscape['%'] = "\\%";
            AttributeScope.tokenScope.AddAttribute( "self", null );
            RuleLabelScope.predefinedLexerRulePropertiesScope.AddAttribute( "self", null );
        }

        public override string GetTargetCharLiteralFromANTLRCharLiteral( CodeGenerator generator,
                                                                string literal )
        {
            StringBuilder buf = new StringBuilder( 10 );

            int c = Grammar.GetCharValueFromGrammarCharLiteral( literal );
            if ( c < Label.MIN_CHAR_VALUE )
            {
                buf.Append( "\\x{0000}" );
            }
            else if ( c < targetCharValueEscape.Length &&
                    targetCharValueEscape[c] != null )
            {
                buf.Append( targetCharValueEscape[c] );
            }
            else if ( ( c < 0x7F ) && !char.IsControl( (char)c ) )
            {
                // normal char
                buf.Append( (char)c );
            }
            else
            {
                // must be something unprintable...use \\uXXXX
                // turn on the bit above max "\\uFFFF" value so that we pad with zeros
                // then only take last 4 digits
                string hex = c.ToString( "X4" );
                buf.Append( "\\x{" );
                buf.Append( hex );
                buf.Append( "}" );
            }

            if ( buf.ToString().IndexOf( '\\' ) == -1 )
            {
                // no need for interpolation, use single quotes
                buf.Insert( 0, '\'' );
                buf.Append( '\'' );
            }
            else
            {
                // need string interpolation
                buf.Insert( 0, '\"' );
                buf.Append( '\"' );
            }

            return buf.ToString();
        }

        public override string EncodeIntAsCharEscape( int v )
        {
            int intValue;
            if ( ( v & 0x8000 ) == 0 )
                intValue = v;
            else
                intValue = -( 0x10000 - v );

            return intValue.ToString();
        }
    }
}
