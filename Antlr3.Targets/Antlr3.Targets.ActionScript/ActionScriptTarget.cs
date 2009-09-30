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
    using CLSCompliant = System.CLSCompliantAttribute;
    using CodeGenerator = Antlr3.Codegen.CodeGenerator;
    using Console = System.Console;
    using Grammar = Antlr3.Tool.Grammar;
    using StringBuilder = System.Text.StringBuilder;
    using Target = Antlr3.Codegen.Target;

    public class ActionScriptTarget : Target
    {

        public override string GetTargetCharLiteralFromANTLRCharLiteral(
                CodeGenerator generator,
                string literal )
        {

            int c = Grammar.GetCharValueFromGrammarCharLiteral( literal );
            return c.ToString();
        }

        public override string GetTokenTypeAsTargetLabel( CodeGenerator generator,
                                                int ttype )
        {
            // use ints for predefined types;
            // <invalid> <EOR> <DOWN> <UP>
            if ( ttype >= 0 && ttype <= 3 )
            {
                return ttype.ToString();
            }

            string name = generator.grammar.GetTokenDisplayName( ttype );

            // If name is a literal, return the token type instead
            if ( name[0] == '\'' )
            {
                return ttype.ToString();
            }

            return name;
        }

        /**
         * ActionScript doesn't support Unicode String literals that are considered "illegal"
         * or are in the surrogate pair ranges.  For example "/uffff" will not encode properly
         * nor will "/ud800".  To keep things as compact as possible we use the following encoding
         * if the int is below 255, we encode as hex literal
         * If the int is between 255 and 0x7fff we use a single unicode literal with the value
         * If the int is above 0x7fff, we use a unicode literal of 0x80hh, where hh is the high-order
         * bits followed by \xll where ll is the lower order bits of a 16-bit number.
         *
         * Ideally this should be improved at a future date.  The most optimal way to encode this
         * may be a compressed AMF encoding that is embedded using an Embed tag in ActionScript.
         *
         * @param v
         * @return
         */
        public override string EncodeIntAsCharEscape( int v )
        {
            // encode as hex
            if ( v <= 255 )
            {
                return "\\x" + v.ToString( "x2" );
            }
            if ( v <= 0x7fff )
            {
                string hex = v.ToString( "x4" );
                return "\\u" + hex;
            }
            if ( v > 0xffff )
            {
                Console.Error.WriteLine( "Warning: character literal out of range for ActionScript target " + v );
                return "";
            }
            StringBuilder buf = new StringBuilder( "\\u80" );
            buf.Append( ( v >> 8 ).ToString( "x2" ) ); // high - order bits
            buf.Append( "\\x" );
            buf.Append( ( v & 0xff ).ToString( "x2" ) ); // low -order bits
            return buf.ToString();
        }

        /** Convert long to two 32-bit numbers separted by a comma.
         *  ActionScript does not support 64-bit numbers, so we need to break
         *  the number into two 32-bit literals to give to the Bit.  A number like
         *  0xHHHHHHHHLLLLLLLL is broken into the following string:
         *  "0xLLLLLLLL, 0xHHHHHHHH"
         *  Note that the low order bits are first, followed by the high order bits.
         *  This is to match how the BitSet constructor works, where the bits are
         *  passed in in 32-bit chunks with low-order bits coming first.
         */
        [CLSCompliant(false)]
        public override string GetTarget64BitStringFromValue(ulong word)
        {
            StringBuilder buf = new StringBuilder( 22 ); // enough for the two "0x", "," and " "
            buf.Append( "0x" );
            WriteHexWithPadding( buf, ( (uint)word ).ToString( "x" ) );
            buf.Append( ", 0x" );
            WriteHexWithPadding( buf, ( word >> 32 ).ToString( "x" ) );

            return buf.ToString();
        }

        private void WriteHexWithPadding( StringBuilder buf, string digits )
        {
            // pad left with zeros
            int padding = 8 - digits.Length;
            buf.Append( new string( '0', padding ) );

            digits = digits.ToUpperInvariant();
            buf.Append( digits );
        }
    }

}
