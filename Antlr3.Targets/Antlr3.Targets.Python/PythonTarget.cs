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

/*

Please excuse my obvious lack of Java experience. The code here is probably
full of WTFs - though IMHO Java is the Real WTF(TM) here...

 */

namespace Antlr3.Targets
{
    using System.Collections.Generic;

    using CodeGenerator = Antlr3.Codegen.CodeGenerator;
    using Console = System.Console;
    using Grammar = Antlr3.Tool.Grammar;
    using IList = System.Collections.IList;
    using IToken = Antlr.Runtime.IToken;
    using Target = Antlr3.Codegen.Target;

    public class PythonTarget : Target
    {
        public override bool UseBaseTemplatesForSynPredFragments
        {
            get
            {
                return false;
            }
        }

        /** Target must be able to override the labels used for token types */
        public override string GetTokenTypeAsTargetLabel( CodeGenerator generator,
                            int ttype )
        {
            // use ints for predefined types;
            // <invalid> <EOR> <DOWN> <UP>
            if ( ttype >= 0 && ttype <= 3 )
            {
                return ttype.ToString();
            }

            string name = generator.Grammar.GetTokenDisplayName( ttype );

            // If name is a literal, return the token type instead
            if ( name[0] == '\'' )
            {
                return ttype.ToString();
            }

            return name;
        }

        public override string GetTargetCharLiteralFromANTLRCharLiteral(
                CodeGenerator generator,
                string literal )
        {
            int c = Grammar.GetCharValueFromGrammarCharLiteral( literal );
            return ( (char)c ).ToString();
        }

        private IList<string> SplitLines( string text )
        {
            return text.Split( '\n' );
        }

        public override IList<object> PostProcessAction( IList<object> chunks, IToken actionToken )
        {
            /* TODO
               - check for and report TAB usage
             */

            //System.out.println("\n*** Action at " + actionToken.getLine() + ":" + actionToken.getColumn());

            /* First I create a new list of chunks. String chunks are splitted into
               lines and some whitespace my be added at the beginning.

               As a result I get a list of chunks
               - where the first line starts at column 0
               - where every LF is at the end of a string chunk
            */

            List<object> nChunks = new List<object>();
            for ( int i = 0; i < chunks.Count; i++ )
            {
                object chunk = chunks[i];

                string text = chunk as string;
                if ( text != null )
                {
                    if ( nChunks.Count == 0 && actionToken.CharPositionInLine > 0 )
                    {
                        // first chunk and some 'virtual' WS at beginning
                        // prepend to this chunk
                        string ws = new string( ' ', actionToken.CharPositionInLine );
                        text = ws + text;
                    }

                    nChunks.AddRange( text.Split( '\n' ) );
                }
                else
                {
                    if ( nChunks.Count == 0 && actionToken.CharPositionInLine > 0 )
                    {
                        // first chunk and some 'virtual' WS at beginning
                        // add as a chunk of its own
                        string ws = new string( ' ', actionToken.CharPositionInLine );
                        nChunks.Add( ws );
                    }

                    nChunks.Add( chunk );
                }
            }

            int lineNo = actionToken.Line;
            int col = 0;

            // strip trailing empty lines
            int lastChunk = nChunks.Count - 1;
            while ( lastChunk > 0
                    && nChunks[lastChunk] is string
                    && ( (string)nChunks[lastChunk] ).Trim().Length == 0 )
            {
                lastChunk--;
            }

            // string leading empty lines
            int firstChunk = 0;
            while ( firstChunk <= lastChunk
                    && nChunks[firstChunk] is string
                    && ( (string)nChunks[firstChunk] ).Trim().Length == 0
                    && ( (string)nChunks[firstChunk] ).EndsWith( "\n" ) )
            {
                lineNo++;
                firstChunk++;
            }

            int indent = -1;
            for ( int i = firstChunk; i <= lastChunk; i++ )
            {
                object chunk = nChunks[i];

                //System.out.println(lineNo + ":" + col + " " + quote(chunk.toString()));

                string text = chunk as string;
                if ( text != null )
                {
                    if ( col == 0 )
                    {
                        if ( indent == -1 )
                        {
                            // first non-blank line
                            // count number of leading whitespaces

                            indent = 0;
                            for ( int j = 0; j < text.Length; j++ )
                            {
                                if ( !char.IsWhiteSpace( text[j] ) )
                                    break;

                                indent++;
                            }
                        }

                        if ( text.Length >= indent )
                        {
                            int j;
                            for ( j = 0; j < indent; j++ )
                            {
                                if ( !char.IsWhiteSpace( text[j] ) )
                                {
                                    // should do real error reporting here...
                                    Console.Error.WriteLine( "Warning: badly indented line " + lineNo + " in action:" );
                                    Console.Error.WriteLine( text );
                                    break;
                                }
                            }

                            nChunks[i] = text.Substring( j );
                        }
                        else if ( text.Trim().Length > 0 )
                        {
                            // should do real error reporting here...
                            Console.Error.WriteLine( "Warning: badly indented line " + lineNo + " in action:" );
                            Console.Error.WriteLine( text );
                        }
                    }

                    if ( text.EndsWith( "\n" ) )
                    {
                        lineNo++;
                        col = 0;
                    }
                    else
                    {
                        col += text.Length;
                    }
                }
                else
                {
                    // not really correct, but all I need is col to increment...
                    col += 1;
                }
            }

            return nChunks;
        }
    }
}
