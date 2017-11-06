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
    using Antlr3.Extensions;

    using CodeGenerator = Antlr3.Codegen.CodeGenerator;
    using Grammar = Antlr3.Tool.Grammar;
    using GrammarType = Antlr3.Tool.GrammarType;
    using StringBuilder = System.Text.StringBuilder;
    using StringTemplate = Antlr4.StringTemplate.Template;
    using Target = Antlr3.Codegen.Target;

    public class CTarget : Target
    {
        List<string> _strings = new List<string>();

        protected override void GenRecognizerFile( AntlrTool tool,
                                    CodeGenerator generator,
                                    Grammar grammar,
                                    StringTemplate outputFileST )
        {

            // Before we write this, and cause it to generate its string,
            // we need to add all the string literals that we are going to match
            //
            outputFileST.SetAttribute( "literals", _strings );
            string fileName = generator.GetRecognizerFileName( grammar.name, grammar.type );
            generator.Write( outputFileST, fileName );
        }

        protected override void GenRecognizerHeaderFile( AntlrTool tool,
                                               CodeGenerator generator,
                                               Grammar grammar,
                                               StringTemplate headerFileST,
                                               string extName )
        {
            // Pick up the file name we are generating. This method will return a 
            // a file suffixed with .c, so we must substring and add the extName
            // to it as we cannot assign into strings in Java.
            //
            string fileName = generator.GetRecognizerFileName( grammar.name, grammar.type );
            fileName = fileName.Substring( 0, fileName.Length - 2 ) + extName;

            generator.Write( headerFileST, fileName );
        }

        /** Is scope in @scope::name {action} valid for this kind of grammar?
         *  Targets like C++ may want to allow new scopes like headerfile or
         *  some such.  The action names themselves are not policed at the
         *  moment so targets can add template actions w/o having to recompile
         *  ANTLR.
         */
        public override bool IsValidActionScope( GrammarType grammarType, string scope )
        {
            switch ( grammarType )
            {
            case GrammarType.Lexer:
                switch ( scope )
                {
                case "lexer":
                case "header":
                case "includes":
                case "preincludes":
                case "overrides":
                    return true;
                }

                break;

            case GrammarType.Parser:
                switch ( scope )
                {
                case "parser":
                case "header":
                case "includes":
                case "preincludes":
                case "overrides":
                    return true;
                }

                break;

            case GrammarType.Combined:
                switch ( scope )
                {
                case "parser":
                case "lexer":
                case "header":
                case "includes":
                case "preincludes":
                case "overrides":
                    return true;
                }

                break;

            case GrammarType.TreeParser:
                switch ( scope )
                {
                case "treeparser":
                case "header":
                case "includes":
                case "preincludes":
                case "overrides":
                    return true;
                }

                break;
            }

            return false;
        }

        public override string GetTargetCharLiteralFromANTLRCharLiteral( CodeGenerator generator, string literal )
        {
            if ( literal.StartsWith( "'\\u" ) )
            {
                literal = "0x" + literal.Substring( 3, 4 );
            }
            else
            {
                int c = literal[1];

                if ( c < 32 || c > 127 )
                {
                    literal = "0x" + c.ToString( "x" );
                }
            }

            return literal;
        }

        /** Convert from an ANTLR string literal found in a grammar file to
         *  an equivalent string literal in the C target.
         *  Because we must support Unicode character sets and have chosen
         *  to have the lexer match UTF32 characters, then we must encode
         *  string matches to use 32 bit character arrays. Here then we
         *  must produce the C array and cater for the case where the 
         *  lexer has been encoded with a string such as 'xyz\n',
         */
        public override string GetTargetStringLiteralFromANTLRStringLiteral( CodeGenerator generator, string literal )
        {
            int index;
            string bytes;
            StringBuilder buf = new StringBuilder();

            buf.Append( "{ " );

            // We need ot lose any escaped characters of the form \x and just
            // replace them with their actual values as well as lose the surrounding
            // quote marks.
            //
            for ( int i = 1; i < literal.Length - 1; i++ )
            {
                buf.Append( "0x" );

                if ( literal[i] == '\\' )
                {
                    i++; // Assume that there is a next character, this will just yield
                    // invalid strings if not, which is what the input would be of course - invalid
                    switch ( literal[i] )
                    {
                    case 'u':
                    case 'U':
                        buf.Append( literal.Substring( i + 1, 4 ) );  // Already a hex string
                        i = i + 5;                                // Move to next string/char/escape
                        break;

                    case 'n':
                    case 'N':

                        buf.Append( "0A" );
                        break;

                    case 'r':
                    case 'R':

                        buf.Append( "0D" );
                        break;

                    case 't':
                    case 'T':

                        buf.Append( "09" );
                        break;

                    case 'b':
                    case 'B':

                        buf.Append( "08" );
                        break;

                    case 'f':
                    case 'F':

                        buf.Append( "0C" );
                        break;

                    default:

                        // Anything else is what it is!
                        //
                        buf.Append( ( (int)literal[i] ).ToString( "X" ) );
                        break;
                    }
                }
                else
                {
                    buf.Append( ( (int)literal[i] ).ToString( "X" ) );
                }
                buf.Append( ", " );
            }
            buf.Append( " ANTLR3_STRING_TERMINATOR}" );

            bytes = buf.ToString();
            index = _strings.IndexOf( bytes );

            if ( index == -1 )
            {
                _strings.Add( bytes );
                index = _strings.IndexOf( bytes );
            }

            string strref = "lit_" + ( index + 1 ).ToString();

            return strref;
        }

        /// <summary>
        /// Overrides the standard grammar analysis so we can prepare the analyser
        /// a little differently from the other targets.
        ///
        /// In particular we want to influence the way the code generator makes assumptions about
        /// switchs vs ifs, vs table driven DFAs. In general, C code should be generated that
        /// has the minimum use of tables, and tha meximum use of large switch statements. This
        /// allows the optimizers to generate very efficient code, it can reduce object code size
        /// by about 30% and give about a 20% performance improvement over not doing this. Hence,
        /// for the C target only, we change the defaults here, but only if they are still set to the
        /// defaults.
        /// </summary>
        /// <param name="generator">An instance of the generic code generator class.</param>
        /// <param name="grammar">The grammar that we are currently analyzing.</param>
        protected override void PerformGrammarAnalysis(CodeGenerator generator, Grammar grammar)
        {
            // Check to see if the maximum inline DFA states is still set to
            // the default size. If it is then whack it all the way up to the maximum that
            // we can sensibly get away with.
            //
            if (CodeGenerator.MaxAcyclicDfaStatesInline == CodeGenerator.DefaultMaxAcyclicDfaStatesInline)
            {
                CodeGenerator.MaxAcyclicDfaStatesInline = 65535;
            }

            // Check to see if the maximum switch size is still set to the default
            // and bring it up much higher if it is. Modern C compilers can handle
            // much bigger switch statements than say Java can and if anyone finds a compiler
            // that cannot deal with such big switches, all the need do is generate the
            // code with a reduced -Xmaxswitchcaselabels nnn
            //
            if (CodeGenerator.MaxSwitchCaseLabels == CodeGenerator.DefaultMaxSwitchCaseLabels)
            {
                CodeGenerator.MaxSwitchCaseLabels = 3000;
            }

            // Check to see if the number of transitions considered a miminum for using
            // a switch is still at the default. Because a switch is still generally faster than
            // an if even with small sets, and given that the optimizer will do the best thing with it
            // anyway, then we simply want to generate a switch for any number of states.
            //
            if (CodeGenerator.MinSwitchAlts == CodeGenerator.DefaultMinSwitchAlts)
            {
                CodeGenerator.MinSwitchAlts = 1;
            }

            // Now we allow the superclass implementation to do whatever it feels it
            // must do.
            //
            base.PerformGrammarAnalysis(generator, grammar);
        }
    }

}
