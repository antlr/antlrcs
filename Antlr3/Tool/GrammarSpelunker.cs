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

namespace Antlr3.Tool
{
    using System.Collections.Generic;

#if BUILD_SPELUNKER
    using Console = System.Console;
#endif
    using Exception = System.Exception;
    using StringBuilder = System.Text.StringBuilder;
    using TextReader = System.IO.TextReader;

    /** Load a grammar file and scan it just until we learn a few items
     *  of interest.  Currently: name, type, imports, tokenVocab, language option.
     *
     *  GrammarScanner (at bottom of this class) converts grammar to stuff like:
     *
     *   grammar Java ; options { backtrack true memoize true }
     *   import JavaDecl JavaAnnotations JavaExpr ;
     *   ... : ...
     *
     *  First ':' or '@' indicates we can stop looking for imports/options.
     *
     *  Then we just grab interesting grammar properties.
     */
    public class GrammarSpelunker
    {
        protected string grammarFileName;
        protected string token;
        protected Scanner scanner;

        // grammar info / properties
        protected string grammarModifier;
        protected string grammarName;
        protected string tokenVocab;
        protected string language = "Java"; // default
        protected List<string> importedGrammars;

        public GrammarSpelunker( string grammarFileName )
        {
            this.grammarFileName = grammarFileName;
        }

        void consume()
        {
            token = scanner.nextToken();
        }

        protected virtual void match( string expecting )
        {
            //System.out.println("match "+expecting+"; is "+token);
            if ( token.Equals( expecting ) )
                consume();
            else
                throw new Exception( "Error parsing " + grammarFileName + ": '" + token +
                                "' not expected '" + expecting + "'" );
        }

        public virtual void parse()
        {
            TextReader r = new System.IO.StreamReader( grammarFileName );
            try
            {
                scanner = new Scanner( r );
                consume();
                grammarHeader();
                // scan until imports or options
                while ( token != null && !token.Equals( "@" ) && !token.Equals( ":" ) &&
                        !token.Equals( "import" ) && !token.Equals( "options" ) )
                {
                    consume();
                }
                if ( token.Equals( "options" ) )
                    options();
                // scan until options or first rule
                while ( token != null && !token.Equals( "@" ) && !token.Equals( ":" ) &&
                        !token.Equals( "import" ) )
                {
                    consume();
                }
                if ( token.Equals( "import" ) )
                    imports();
                // ignore rest of input; close up shop
            }
            finally
            {
                if ( r != null )
                    r.Close();
            }
        }

        protected virtual void grammarHeader()
        {
            if ( token == null )
                return;
            if ( token.Equals( "tree" ) || token.Equals( "parser" ) || token.Equals( "lexer" ) )
            {
                grammarModifier = token;
                consume();
            }
            match( "grammar" );
            grammarName = token;
            consume(); // move beyond name
        }

        // looks like "options { backtrack true ; tokenVocab MyTokens ; }"
        protected virtual void options()
        {
            match( "options" );
            match( "{" );
            while ( token != null && !token.Equals( "}" ) )
            {
                string name = token;
                consume();
                string value = token;
                consume();
                consume(); // kill ';'
                if ( name.Equals( "tokenVocab" ) )
                    tokenVocab = value;
                if ( name.Equals( "language" ) )
                    language = value;
            }
            match( "}" );
        }

        // looks like "import JavaDecl JavaAnnotations JavaExpr ;"
        protected virtual void imports()
        {
            match( "import" );
            importedGrammars = new List<string>();
            while ( token != null && !token.Equals( ";" ) )
            {
                importedGrammars.Add( token );
                consume();
            }
            match( ";" );
            if ( importedGrammars.Count == 0 )
                importedGrammars = null;
        }

        public virtual string getGrammarModifier()
        {
            return grammarModifier;
        }

        public virtual string getGrammarName()
        {
            return grammarName;
        }

        public virtual string getTokenVocab()
        {
            return tokenVocab;
        }

        public virtual string getLanguage()
        {
            return language;
        }

        public virtual List<string> getImportedGrammars()
        {
            return importedGrammars;
        }

        /** Strip comments and then return stream of words and
         *  tokens {';', ':', '{', '}'}
         */
        public class Scanner
        {
            public const int EOF = -1;
            TextReader input;
            int c;

            public Scanner( TextReader input )
            {
                this.input = input;
                consume();
            }

            bool isID_START()
            {
                return c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z';
            }
            bool isID_LETTER()
            {
                return isID_START() || c >= '0' && c <= '9' || c == '_';
            }

            void consume()
            {
                c = input.Read();
            }

            public virtual string nextToken()
            {
                while ( c != EOF )
                {
                    //System.out.println("check "+(char)c);
                    switch ( c )
                    {
                    case ';':
                        consume();
                        return ";";
                    case '{':
                        consume();
                        return "{";
                    case '}':
                        consume();
                        return "}";
                    case ':':
                        consume();
                        return ":";
                    case '@':
                        consume();
                        return "@";
                    case '/':
                        COMMENT();
                        break;
                    default:
                        if ( isID_START() )
                            return ID();
                        consume(); // ignore anything else
                        break;
                    }
                }
                return null;
            }

            /** NAME : LETTER+ ; // NAME is sequence of >=1 letter */
            string ID()
            {
                StringBuilder buf = new StringBuilder();
                while ( isID_LETTER() )
                {
                    buf.Append( (char)c );
                    consume();
                }
                return buf.ToString();
            }

            void COMMENT()
            {
                if ( c == '/' )
                {
                    consume();
                    if ( c == '*' )
                    {
                        consume();
                        for ( ; ; )
                        {
                            if ( c == '*' )
                            {
                                consume();
                                if ( c == '/' )
                                {
                                    consume();
                                    break;
                                }
                            }
                            else
                            {
                                while ( c != '*' )
                                    consume();
                            }
                        }
                    }
                    else if ( c == '/' )
                    {
                        while ( c != '\n' )
                            consume();
                    }
                }
            }
        }

#if BUILD_SPELUNKER
        /** Tester; Give grammar filename as arg */
        public static void Main( string[] args )
        {
            GrammarSpelunker g = new GrammarSpelunker( args[0] );
            g.parse();
            Console.Out.WriteLine( g.grammarModifier + " grammar " + g.grammarName );
            Console.Out.WriteLine( "language=" + g.language );
            Console.Out.WriteLine( "tokenVocab=" + g.tokenVocab );
            Console.Out.WriteLine( "imports=" + g.importedGrammars );
        }
#endif
    }
}
