/*
 * [The "BSD license"]
 * Copyright (c) 2011 Terence Parr
 * All rights reserved.
 *
 * Conversion to C#:
 * Copyright (c) 2011 Sam Harwell, Pixel Mine, Inc.
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

namespace Antlr3.Codegen
{
    using System.Collections.Generic;

    using AntlrTool = Antlr3.AntlrTool;
    using ArgumentException = System.ArgumentException;
    using CLSCompliant = System.CLSCompliantAttribute;
    using Grammar = Antlr3.Tool.Grammar;
    using GrammarType = Antlr3.Tool.GrammarType;
    using IToken = Antlr.Runtime.IToken;
    using Label = Antlr3.Analysis.Label;
    using StringBuilder = System.Text.StringBuilder;
    using StringTemplate = Antlr3.ST.StringTemplate;

    /** The code generator for ANTLR can usually be retargeted just by providing
     *  a new X.stg file for language X, however, sometimes the files that must
     *  be generated vary enough that some X-specific functionality is required.
     *  For example, in C, you must generate header files whereas in Java you do not.
     *  Other languages may want to keep DFA separate from the main
     *  generated recognizer file.
     *
     *  The notion of a Code Generator target abstracts out the creation
     *  of the various files.  As new language targets get added to the ANTLR
     *  system, this target class may have to be altered to handle more
     *  functionality.  Eventually, just about all language generation issues
     *  will be expressible in terms of these methods.
     *
     *  If org.antlr.codegen.XTarget class exists, it is used else
     *  Target base class is used.  I am using a superclass rather than an
     *  interface for this target concept because I can add functionality
     *  later without breaking previously written targets (extra interface
     *  methods would force adding dummy functions to all code generator
     *  target classes).
     *
     */
    public abstract class Target
    {
        /** For pure strings of Java 16-bit unicode char, how can we display
         *  it in the target language as a literal.  Useful for dumping
         *  predicates and such that may refer to chars that need to be escaped
         *  when represented as strings.  Also, templates need to be escaped so
         *  that the target language can hold them as a string.
         *
         *  I have defined (via the constructor) the set of typical escapes,
         *  but your Target subclass is free to alter the translated chars or
         *  add more definitions.  This is nonstatic so each target can have
         *  a different set in memory at same time.
         */
        protected string[] targetCharValueEscape = new string[255];

        public Target()
        {
            targetCharValueEscape['\n'] = "\\n";
            targetCharValueEscape['\r'] = "\\r";
            targetCharValueEscape['\t'] = "\\t";
            targetCharValueEscape['\b'] = "\\b";
            targetCharValueEscape['\f'] = "\\f";
            targetCharValueEscape['\\'] = "\\\\";
            targetCharValueEscape['\''] = "\\'";
            targetCharValueEscape['"'] = "\\\"";
        }

        protected internal virtual void GenRecognizerFile( AntlrTool tool,
                                         CodeGenerator generator,
                                         Grammar grammar,
                                         StringTemplate outputFileST )
        {
            string fileName =
                generator.GetRecognizerFileName( grammar.name, grammar.type );
            generator.Write( outputFileST, fileName );
        }

        protected internal virtual void GenRecognizerHeaderFile( AntlrTool tool,
                                               CodeGenerator generator,
                                               Grammar grammar,
                                               StringTemplate headerFileST,
                                               string extName ) // e.g., ".h"
        {
            // no header file by default
        }

        protected internal virtual void PerformGrammarAnalysis( CodeGenerator generator,
                                              Grammar grammar )
        {
            // Build NFAs from the grammar AST
            grammar.BuildNFA();

            // Create the DFA predictors for each decision
            grammar.CreateLookaheadDFAs();
        }

        /** Is scope in @scope::name {action} valid for this kind of grammar?
         *  Targets like C++ may want to allow new scopes like headerfile or
         *  some such.  The action names themselves are not policed at the
         *  moment so targets can add template actions w/o having to recompile
         *  ANTLR.
         */
        public virtual bool IsValidActionScope( GrammarType grammarType, string scope )
        {
            switch ( grammarType )
            {
            case GrammarType.Lexer:
                if ( scope.Equals( "lexer" ) )
                {
                    return true;
                }
                break;
            case GrammarType.Parser:
                if ( scope.Equals( "parser" ) )
                {
                    return true;
                }
                break;
            case GrammarType.Combined:
                if ( scope.Equals( "parser" ) )
                {
                    return true;
                }
                if ( scope.Equals( "lexer" ) )
                {
                    return true;
                }
                break;
            case GrammarType.TreeParser:
                if ( scope.Equals( "treeparser" ) )
                {
                    return true;
                }
                break;
            }
            return false;
        }

        /** Target must be able to override the labels used for token types */
        public virtual string GetTokenTypeAsTargetLabel( CodeGenerator generator, int ttype )
        {
            string name = generator.grammar.GetTokenDisplayName( ttype );
            // If name is a literal, return the token type instead
            if ( name[0] == '\'' )
            {
                return ttype.ToString(); //String.valueOf( ttype );
            }
            return name;
        }

        /** Convert from an ANTLR char literal found in a grammar file to
         *  an equivalent char literal in the target language.  For most
         *  languages, this means leaving 'x' as 'x'.  Actually, we need
         *  to escape '\u000A' so that it doesn't get converted to \n by
         *  the compiler.  Convert the literal to the char value and then
         *  to an appropriate target char literal.
         *
         *  Expect single quotes around the incoming literal.
         */
        public virtual string GetTargetCharLiteralFromANTLRCharLiteral(
            CodeGenerator generator,
            string literal )
        {
            StringBuilder buf = new StringBuilder();
            buf.Append( '\'' );
            int c = Grammar.GetCharValueFromGrammarCharLiteral( literal );
            if ( c < Label.MIN_CHAR_VALUE )
            {
                return "'\u0000'";
            }
            if ( c < targetCharValueEscape.Length &&
                 targetCharValueEscape[c] != null )
            {
                buf.Append( targetCharValueEscape[c] );
            }
            else if ( c <= 0x7f && !char.IsControl( (char)c ) )
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
                buf.Append( "\\u" );
                buf.Append( hex );
            }

            buf.Append( '\'' );
            return buf.ToString();
        }

        /** Convert from an ANTLR string literal found in a grammar file to
         *  an equivalent string literal in the target language.  For Java, this
         *  is the translation 'a\n"' -> "a\n\"".  Expect single quotes
         *  around the incoming literal.  Just flip the quotes and replace
         *  double quotes with \"
         * 
         *  Note that we have decided to allow poeple to use '\"' without
         *  penalty, so we must build the target string in a loop as Utils.replae
         *  cannot handle both \" and " without a lot of messing around.
         * 
         */
        public virtual string GetTargetStringLiteralFromANTLRStringLiteral(
            CodeGenerator generator,
            string literal )
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder @is = new StringBuilder( literal );

            // Opening quote
            //
            sb.Append( '"' );

            for ( int i = 1; i < @is.Length - 1; i++ )
            {

                if ( @is[i] == '\\' )
                {

                    // Anything escaped is what it is! We assume that 
                    // people know how to escape characters correctly. However
                    // we catch anything that does not need an escape in Java (which
                    // is what the default implementation is dealing with and remove 
                    // the escape. The C target does this for instance.
                    //
                    switch ( @is[i + 1] )
                    {

                    // Pass through any escapes that Java also needs
                    //
                    case '"':
                    case 'n':
                    case 'r':
                    case 't':
                    case 'b':
                    case 'f':
                    case '\\':
                    case 'u':    // Assume unnnn

                        sb.Append( '\\' );    // Pass the escape through
                        break;

                    default:

                        // Remove the escape by virtue of not adding it here
                        // Thus \' becomes ' and so on
                        //
                        break;
                    }

                    // Go past the \ character
                    //
                    i++;

                }
                else
                {

                    // Chracters that don't need \ in ANTLR 'strings' but do in Java
                    //
                    if ( @is[i] == '"' )
                    {

                        // We need to escape " in Java
                        //
                        sb.Append( '\\' );
                    }
                }

                // Add in the next character, which may have been escaped
                //
                sb.Append( @is[i] );
            }

            // Append closing " and return
            //
            sb.Append( '"' );

            return sb.ToString();
        }

        /** Given a random string of Java unicode chars, return a new string with
         *  optionally appropriate quote characters for target language and possibly
         *  with some escaped characters.  For example, if the incoming string has
         *  actual newline characters, the output of this method would convert them
         *  to the two char sequence \n for Java, C, C++, ...  The new string has
         *  double-quotes around it as well.  Example String in memory:
         *
         *     a"[newlinechar]b'c[carriagereturnchar]d[tab]e\f
         *
         *  would be converted to the valid Java s:
         *
         *     "a\"\nb'c\rd\te\\f"
         *
         *  or
         *
         *     a\"\nb'c\rd\te\\f
         *
         *  depending on the quoted arg.
         */
        public virtual string GetTargetStringLiteralFromString( string s, bool quoted )
        {
            if ( s == null )
            {
                return null;
            }
            StringBuilder buf = new StringBuilder();
            if ( quoted )
            {
                buf.Append( '"' );
            }
            for ( int i = 0; i < s.Length; i++ )
            {
                int c = s[i];
                if ( c != '\'' && // don't escape single quotes in strings for java
                     c < targetCharValueEscape.Length &&
                     targetCharValueEscape[c] != null )
                {
                    buf.Append( targetCharValueEscape[c] );
                }
                else
                {
                    buf.Append( (char)c );
                }
            }
            if ( quoted )
            {
                buf.Append( '"' );
            }
            return buf.ToString();
        }

        public virtual string GetTargetStringLiteralFromString( string s )
        {
            return GetTargetStringLiteralFromString( s, false );
        }

        /** Convert long to 0xNNNNNNNNNNNNNNNN by default for spitting out
         *  with bitsets.  I.e., convert bytes to hex string.
         */
        [CLSCompliant(false)]
        public virtual string GetTarget64BitStringFromValue( ulong word )
        {
            int numHexDigits = 8 * 2;
            StringBuilder buf = new StringBuilder( numHexDigits + 2 );
            buf.Append( "0x" );
            //String digits = Long.toHexString( word );
            //digits = digits.toUpperCase();
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

        public string ToOctalString( int value )
        {
            if ( value < 0 )
                throw new ArgumentException( "The value cannot be negative", "value" );

            if ( value == 0 )
                return "0";

            StringBuilder builder = new StringBuilder( "           " );
            int index = builder.Length - 1;
            while ( value > 0 )
            {
                builder[index] = (char)( '0' + ( value % 8 ) );
                value /= 8;
                index--;
            }
            return builder.ToString().Trim();
        }

        public virtual string EncodeIntAsCharEscape( int v )
        {
            if ( v <= 127 )
            {
                return "\\" + ToOctalString( v );
            }
            //String hex = Integer.toHexString( v | 0x10000 ).substring( 1, 5 );
            string hex = v.ToString( "x4" );
            return "\\u" + hex;
        }

        /** Some targets only support ASCII or 8-bit chars/strings.  For example,
         *  C++ will probably want to return 0xFF here.
         */
        public virtual int GetMaxCharValue( CodeGenerator generator )
        {
            return Label.MAX_CHAR_VALUE;
        }

        /** Give target a chance to do some postprocessing on actions.
         *  Python for example will have to fix the indention.
         */
        public virtual IList<object> PostProcessAction( IList<object> chunks, IToken actionToken )
        {
            return chunks;
        }

    }

}
