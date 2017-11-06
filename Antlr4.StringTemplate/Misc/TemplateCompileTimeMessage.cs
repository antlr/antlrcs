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

    /** Used for semantic errors that occur at compile time not during
     *  interpretation. For Template parsing ONLY not group parsing.
     */
    public class TemplateCompiletimeMessage : TemplateMessage
    {
        private readonly IToken _templateToken; // overall token pulled from group file
        private readonly IToken _token;         // token inside template
        private readonly string _sourceName;

        public TemplateCompiletimeMessage(ErrorType error, string sourceName, IToken templateToken, IToken token)
            : this(error, sourceName, templateToken, token, null)
        {
        }

        public TemplateCompiletimeMessage(ErrorType error, string sourceName, IToken templateToken, IToken token, Exception cause)
            : this(error, sourceName, templateToken, token, cause, null)
        {
        }

        public TemplateCompiletimeMessage(ErrorType error, string sourceName, IToken templateToken, IToken token, Exception cause, object arg)
            : this(error, sourceName, templateToken, token, cause, arg, null)
        {
        }

        public TemplateCompiletimeMessage(ErrorType error, string sourceName, IToken templateToken, IToken token, Exception cause, object arg, object arg2)
            : base(error, null, cause, arg, arg2)
        {
            this._templateToken = templateToken;
            this._token = token;
            this._sourceName = sourceName;
        }

        public IToken TemplateToken
        {
            get
            {
                return _templateToken;
            }
        }

        public IToken Token
        {
            get
            {
                return _token;
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
            int line = 0;
            int charPos = -1;
            if (_token != null)
            {
                line = _token.Line;
                charPos = _token.CharPositionInLine;
                // check the input streams - if different then token is embedded in templateToken and we need to adjust the offset
                if (_templateToken != null && !_templateToken.InputStream.Equals(Token.InputStream))
                {
                    int templateDelimiterSize = 1;
                    if (_templateToken.Type == GroupParser.BIGSTRING || _templateToken.Type == GroupParser.BIGSTRING_NO_NL)
                        templateDelimiterSize = 2;

                    line += _templateToken.Line - 1;
                    charPos += _templateToken.CharPositionInLine + templateDelimiterSize;
                }
            }

            string filepos = string.Format("{0}:{1}", line, charPos);
            if (_sourceName != null)
                return string.Format("{0} {1}: {2}", _sourceName, filepos, string.Format(Error.Message, Arg, Arg2));

            return string.Format("{0}: {1}", filepos, string.Format(Error.Message, Arg, Arg2));
        }
    }
}
