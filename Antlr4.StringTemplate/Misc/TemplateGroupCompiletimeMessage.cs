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

    public class TemplateGroupCompiletimeMessage : TemplateMessage
    {
        /// <summary>
        /// token inside group file
        /// </summary>
        private readonly IToken token;
        private readonly string srcName;

        public TemplateGroupCompiletimeMessage(ErrorType error, string srcName, IToken t)
            : this(error, srcName, t, null, null, null)
        {
        }

        public TemplateGroupCompiletimeMessage(ErrorType error, string srcName, IToken t, Exception cause)
            : this(error, srcName, t, cause, null, null)
        {
        }

        public TemplateGroupCompiletimeMessage(ErrorType error, string srcName, IToken t, Exception cause, object arg)
            : this(error, srcName, t, cause, arg, null)
        {
        }

        public TemplateGroupCompiletimeMessage(ErrorType error, string srcName, IToken t, Exception cause, object arg, object arg2)
            : base(error, null, cause, arg, arg2)
        {
            this.token = t;
            this.srcName = srcName;
        }

        public override string ToString()
        {
            RecognitionException re = (RecognitionException)Cause;
            int line = 0;
            int charPos = -1;
            if (token != null)
            {
                line = token.Line;
                charPos = token.CharPositionInLine;
            }
            else if (re != null)
            {
                line = re.Line;
                charPos = re.CharPositionInLine;
            }

            string filepos = line + ":" + charPos;
            if (srcName != null)
            {
                return srcName + " " + filepos + ": " + string.Format(Error.Message, Arg, Arg2);
            }

            return filepos + ": " + string.Format(Error.Message, Arg, Arg2);
        }
    }
}
