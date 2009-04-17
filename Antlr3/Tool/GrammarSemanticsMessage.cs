/*
 * [The "BSD licence"]
 * Copyright (c) 2005-2008 Terence Parr
 * All rights reserved.
 *
 * Conversion to C#:
 * Copyright (c) 2008 Sam Harwell, Pixel Mine, Inc.
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
    using IToken = Antlr.Runtime.IToken;
    using StringTemplate = Antlr3.ST.StringTemplate;

    /** A problem with the symbols and/or meaning of a grammar such as rule
     *  redefinition.
     */
    public class GrammarSemanticsMessage : Message
    {
        public Grammar g;
        /** Most of the time, we'll have a token such as an undefined rule ref
         *  and so this will be set.
         */
        public IToken offendingToken;

        public GrammarSemanticsMessage( int msgID,
                              Grammar g,
                              IToken offendingToken )
            : this( msgID, g, offendingToken, null, null )
        {
        }

        public GrammarSemanticsMessage( int msgID,
                              Grammar g,
                              IToken offendingToken,
                              object arg )
            : this( msgID, g, offendingToken, arg, null )
        {
        }

        public GrammarSemanticsMessage( int msgID,
                              Grammar g,
                              IToken offendingToken,
                              object arg,
                              object arg2 )
            : base( msgID, arg, arg2 )
        {
            this.g = g;
            this.offendingToken = offendingToken;
        }

        public override string ToString()
        {
            line = 0;
            charPositionInLine = 0;
            if ( offendingToken != null )
            {
                line = offendingToken.Line;
                charPositionInLine = offendingToken.CharPositionInLine;
            }
            if ( g != null )
            {
                file = g.FileName;
            }
            StringTemplate st = GetMessageTemplate();
            if ( arg != null )
            {
                st.SetAttribute( "arg", arg );
            }
            if ( arg2 != null )
            {
                st.SetAttribute( "arg2", arg2 );
            }
            return base.ToString( st );
        }
    }
}
