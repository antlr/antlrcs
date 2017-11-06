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
    using Antlr4.StringTemplate.Compiler;
    using Exception = System.Exception;
    using IToken = Antlr.Runtime.IToken;
    using RecognitionException = Antlr.Runtime.RecognitionException;

    /** */
    public class TemplateLexerMessage : TemplateMessage
    {
        private readonly string _message;
        private readonly IToken _templateToken; // overall token pulled from group file
        private readonly string _sourceName;

        public TemplateLexerMessage(string sourceName, string message, IToken templateToken, Exception cause)
            : base(ErrorType.LEXER_ERROR, null, cause, null)
        {
            this._message = message;
            this._templateToken = templateToken;
            this._sourceName = sourceName;
        }

        public string Message
        {
            get
            {
                return _message;
            }
        }

        public IToken TemplateToken
        {
            get
            {
                return _templateToken;
            }
        }

        public string SourceName
        {
            get
            {
                return _sourceName;
            }
        }

        public override string ToString()
        {
            RecognitionException re = (RecognitionException)Cause;
            int line = re.Line;
            int charPos = re.CharPositionInLine;
            if (_templateToken != null)
            {
                int templateDelimiterSize = 1;
                if (_templateToken.Type == GroupParser.BIGSTRING)
                {
                    templateDelimiterSize = 2;
                }
                line += _templateToken.Line - 1;
                charPos += _templateToken.CharPositionInLine + templateDelimiterSize;
            }

            string filepos = line + ":" + charPos;
            if (_sourceName != null)
            {
                return _sourceName + " " + filepos + ": " + string.Format(Error.Message, _message);
            }

            return filepos + ": " + string.Format(Error.Message, _message);
        }
    }
}
