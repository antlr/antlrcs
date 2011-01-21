/*
 * [The "BSD licence"]
 * Copyright (c) 2011 Terence Parr
 * All rights reserved.
 *
 * Conversion to C#:
 * Copyright (c) 2011 Sam Harwell, Tunnel Vision Laboratories, LLC
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

namespace Antlr4.StringTemplate.Misc
{
    using Exception = System.Exception;
    using IToken = Antlr.Runtime.IToken;
    using RecognitionException = Antlr.Runtime.RecognitionException;
    using Antlr4.StringTemplate.Compiler;

    /** */
    public class STLexerMessage : STMessage
    {
        string msg;
        IToken templateToken; // overall token pulled from group file
        string srcName;

        public STLexerMessage(string srcName, string msg, IToken templateToken, Exception cause)
            : base(ErrorType.LEXER_ERROR, null, cause, null)
        {
            this.msg = msg;
            this.templateToken = templateToken;
            this.srcName = srcName;
        }

        public override string ToString()
        {
            RecognitionException re = (RecognitionException)Cause;
            int line = re.Line;
            int charPos = re.CharPositionInLine;
            if (templateToken != null)
            {
                int templateDelimiterSize = 1;
                if (templateToken.Type == GroupParser.BIGSTRING)
                {
                    templateDelimiterSize = 2;
                }
                line += templateToken.Line - 1;
                charPos += templateToken.CharPositionInLine + templateDelimiterSize;
            }

            string filepos = line + ":" + charPos;
            if (srcName != null)
            {
                return srcName + " " + filepos + ": " + string.Format(Error.Message, msg);
            }

            return filepos + ": " + string.Format(Error.Message, msg);
        }
    }
}
