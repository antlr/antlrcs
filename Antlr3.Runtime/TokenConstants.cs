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

namespace Antlr.Runtime
{
    public static class TokenChannels
    {
        /** <summary>
         *  All tokens go to the parser (unless skip() is called in that rule)
         *  on a particular "channel".  The parser tunes to a particular channel
         *  so that whitespace etc... can go to the parser on a "hidden" channel.
         *  </summary>
         */
        public const int Default = 0;

        /** <summary>
         *  Anything on different channel than DEFAULT_CHANNEL is not parsed
         *  by parser.
         *  </summary>
         */
        public const int Hidden = 99;
    }

    public static class TokenTypes
    {
        public const int EndOfFile = CharStreamConstants.EndOfFile;
        public const int Invalid = 0;
        public const int EndOfRule = 1;
        /** <summary>imaginary tree navigation type; traverse "get child" link</summary> */
        public const int Down = 2;
        /** <summary>imaginary tree navigation type; finish with a child list</summary> */
        public const int Up = 3;
        public const int Min = Up + 1;
    }

    public static class Tokens
    {
        public static readonly IToken EndOfFile = Tokens<CommonToken>.EndOfFile;

        public static readonly IToken Invalid = new CommonToken( TokenTypes.Invalid );

        /** <summary>
         *  In an action, a lexer rule can set token to this SKIP_TOKEN and ANTLR
         *  will avoid creating a token for this symbol and try to fetch another.
         *  </summary>
         */
        public static readonly IToken Skip = new CommonToken( TokenTypes.Invalid );
    }

    public static class Tokens<T>
        where T : IToken, new()
    {
        public static readonly T EndOfFile = new T()
        {
            Type = TokenTypes.EndOfFile
        };

        public static readonly T Invalid = new T()
        {
            Type = TokenTypes.Invalid
        };

        public static readonly T Skip = new T()
        {
            Type = TokenTypes.Invalid
        };
    }
}
