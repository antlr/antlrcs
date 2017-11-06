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
    using StringBuilder = System.Text.StringBuilder;

    partial class GroupLexer
    {
        string ProcessAnonymousTemplate( StringBuilder builder )
        {
            // handle escaped }
            builder.Replace( @"\}", "}" );
            return builder.ToString();
        }

        string ProcessBigString( StringBuilder builder )
        {
            // handle escaped >
            builder.Replace( @"\>", ">" );

            // kill first newline
            int trimStart = 0;
            if ( builder.Length > 0 )
            {
                switch ( builder[0] )
                {
                case '\r':
                    if ( builder.Length > 1 && builder[1] == '\n' )
                        trimStart = 2;
                    else
                        trimStart = 1;
                    break;

                case '\n':
                    trimStart = 1;
                    break;

                default:
                    break;
                }
            }

            // kill last newline
            int trimEnd = 0;
            if ( builder.Length > trimStart )
            {
                switch ( builder[builder.Length - 1] )
                {
                case '\r':
                    trimEnd = 1;
                    break;

                case '\n':
                    if ( builder[builder.Length - 2] == '\r' )
                        trimEnd = 2;
                    else
                        trimEnd = 1;
                    break;

                default:
                    break;
                }
            }

            return builder.ToString( trimStart, builder.Length - trimStart - trimEnd );
        }
    }
}
