/*
 * [The "BSD licence"]
 * Copyright (c) 2005-2008 Terence Parr
 * All rights reserved.
 *
 * Grammar conversion to ANTLR v3 and C#:
 * Copyright (c) 2008-2009 Sam Harwell, Pixel Mine, Inc.
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

namespace Antlr3.Grammars
{
    using Antlr.Runtime;
    using Antlr3.Tool;

    using CommonTreeNodeStream = Antlr.Runtime.Tree.CommonTreeNodeStream;
    using RecognitionException = Antlr.Runtime.RecognitionException;
    using StringBuilder = System.Text.StringBuilder;

    partial class ANTLRTreePrinter
    {
        protected Grammar grammar;
        protected bool showActions;
        protected StringBuilder buf = new StringBuilder( 300 );

        public ANTLRTreePrinter.block_return Block( GrammarAST t, bool forceParens )
        {
            ANTLRTreePrinter other = new ANTLRTreePrinter(new CommonTreeNodeStream(t));
            other.buf = buf;
            return other.block( forceParens );
        }
        public int CountAltsForBlock(GrammarAST t)
        {
            int n = 0;
            for ( int i = 0; i < t.ChildCount; i++ )
            {
                if ( t.GetChild( i ).Type == ALT )
                    n++;
            }

            return n;
        }

        public virtual void @out( string s )
        {
            buf.Append( s );
        }

        public override void ReportError( RecognitionException ex )
        {
            IToken token = null;
            if ( ex is MismatchedTokenException )
            {
                token = ( (MismatchedTokenException)ex ).Token;
            }
            else if ( ex is NoViableAltException )
            {
                token = ( (NoViableAltException)ex ).Token;
            }
            ErrorManager.SyntaxError(
                ErrorManager.MSG_SYNTAX_ERROR,
                grammar,
                token,
                "antlr.print: " + ex.ToString(),
                ex );
        }

        /** Normalize a grammar print out by removing all double spaces
         *  and trailing/beginning stuff.  FOr example, convert
         *
         *  ( A  |  B  |  C )*
         *
         *  to
         *
         *  ( A | B | C )*
         */
        public static string Normalize( string g )
        {
            // the trim appears to take just the last \n off
            return string.Join( " ", g.Split( new char[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries ) ).Trim();
        }
    }
}
