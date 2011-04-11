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
    using CodeGenerator = Antlr3.Codegen.CodeGenerator;
    using Grammar = Antlr3.Tool.Grammar;
    using StringBuilder = System.Text.StringBuilder;
    using StringTemplate = Antlr4.StringTemplate.Template;
    using StringTemplateGroup = Antlr4.StringTemplate.TemplateGroup;
    using Target = Antlr3.Codegen.Target;

    public class CPPTarget : Target
    {
        public string EscapeChar( int c )
        {
            // System.out.println("CPPTarget.escapeChar("+c+")");
            switch ( c )
            {
            case '\n':
                return "\\n";
            case '\t':
                return "\\t";
            case '\r':
                return "\\r";
            case '\\':
                return "\\\\";
            case '\'':
                return "\\'";
            case '"':
                return "\\\"";
            default:
                if ( c < ' ' || c > 126 )
                {
                    if ( c > 255 )
                    {
                        string s = c.ToString( "x" );
                        // put leading zeroes in front of the thing..
                        while ( s.Length < 4 )
                            s = '0' + s;
                        return "\\u" + s;
                    }
                    else
                    {
                        return "\\" + c.ToString( "8" );
                    }
                }
                else
                {
                    return ( (char)c ).ToString();
                }
            }
        }

        /** Converts a String into a representation that can be use as a literal
         * when surrounded by double-quotes.
         *
         * Used for escaping semantic predicate strings for exceptions.
         *
         * @param s The String to be changed into a literal
         */
        public string EscapeString( string s )
        {
            StringBuilder retval = new StringBuilder();
            for ( int i = 0; i < s.Length; i++ )
            {
                retval.Append( EscapeChar( s[i] ) );
            }

            return retval.ToString();
        }

        protected override void GenRecognizerHeaderFile( AntlrTool tool,
                                               CodeGenerator generator,
                                               Grammar grammar,
                                               StringTemplate headerFileST,
                                               string extName )
        {
            generator.Write( headerFileST, grammar.name + extName );
        }

        /** Convert from an ANTLR char literal found in a grammar file to
         *  an equivalent char literal in the target language.  For Java, this
         *  is the identify translation; i.e., '\n' -> '\n'.  Most languages
         *  will be able to use this 1-to-1 mapping.  Expect single quotes
         *  around the incoming literal.
         *  Depending on the charvocabulary the charliteral should be prefixed with a 'L'
         */
        public override string GetTargetCharLiteralFromANTLRCharLiteral( CodeGenerator codegen, string literal )
        {
            int c = Grammar.GetCharValueFromGrammarCharLiteral( literal );
            string prefix = "'";
            if ( codegen.grammar.MaxCharValue > 255 )
                prefix = "L'";
            else if ( ( c & 0x80 ) != 0 )	// if in char mode prevent sign extensions
                return "" + c;
            return prefix + EscapeChar( c ) + "'";
        }

        /** Convert from an ANTLR string literal found in a grammar file to
         *  an equivalent string literal in the target language.  For Java, this
         *  is the identify translation; i.e., "\"\n" -> "\"\n".  Most languages
         *  will be able to use this 1-to-1 mapping.  Expect double quotes 
         *  around the incoming literal.
         *  Depending on the charvocabulary the string should be prefixed with a 'L'
         */
        public override string GetTargetStringLiteralFromANTLRStringLiteral( CodeGenerator codegen, string literal )
        {
            StringBuilder buf = Grammar.GetUnescapedStringFromGrammarStringLiteral( literal );
            string prefix = "\"";
            if ( codegen.grammar.MaxCharValue > 255 )
                prefix = "L\"";
            return prefix + EscapeString( buf.ToString() ) + "\"";
        }
        /** Character constants get truncated to this value.
         * TODO: This should be derived from the charVocabulary. Depending on it
         * being 255 or 0xFFFF the templates should generate normal character
         * constants or multibyte ones.
         */
        public override int GetMaxCharValue( CodeGenerator codegen )
        {
            int maxval = 255; // codegen.grammar.get????();
            if ( maxval <= 255 )
                return 255;
            else
                return maxval;
        }
    }
}
