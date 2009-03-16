/*
 * [The "BSD licence"]
 * Copyright (c) 2005-2008 Terence Parr
 * All rights reserved.
 *
 * Grammar conversion to ANTLR v3 and C#:
 * Copyright (c) 2008 Sam Harwell, Pixel Mine, Inc.
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions
 * are met:
 * 1. Redistributions of source code must retain the above copyright
 *	notice, this list of conditions and the following disclaimer.
 * 2. Redistributions in binary form must reproduce the above copyright
 *	notice, this list of conditions and the following disclaimer in the
 *	documentation and/or other materials provided with the distribution.
 * 3. The name of the author may not be used to endorse or promote products
 *	derived from this software without specific prior written permission.
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

namespace Antlr3.ST.Language
{
    using ANTLRReaderStream = Antlr.Runtime.ANTLRReaderStream;
    using RecognitionException = Antlr.Runtime.RecognitionException;
    using TextReader = System.IO.TextReader;

    partial class AngleBracketTemplateLexer
    {
        protected string currentIndent = null;
        protected StringTemplate self;

        public AngleBracketTemplateLexer( StringTemplate self, TextReader r )
            : this( new ANTLRReaderStream( r ) )
        {
            this.self = self;
        }

        public override void ReportError( RecognitionException e )
        {
            self.Error( "<...> chunk lexer error", e );
        }

        public override string[] GetTokenNames()
        {
            return TemplateParser.tokenNames;
        }

        bool UpcomingAtEND( int i )
        {
            return input.LA( i ) == '<'
                && input.LA( i + 1 ) == '@'
                && input.LA( i + 2 ) == 'e'
                && input.LA( i + 3 ) == 'n'
                && input.LA( i + 4 ) == 'd'
                && input.LA( i + 5 ) == '>';
        }
    }

}
