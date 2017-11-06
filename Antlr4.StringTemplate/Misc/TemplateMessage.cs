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
    using StringWriter = System.IO.StringWriter;

    /** Upon error, Template creates an TemplateMessage or subclass instance and notifies
     *  the listener.  This root class is used for IO and internal errors.
     *
     *  @see TemplateRuntimeMessage
     *  @see TemplateCompiletimeMessage
     */
    public class TemplateMessage
    {
        /** if in debug mode, has created instance, Add attr events and eval
         *  template events.
         */
        private readonly Template self;
        private readonly ErrorType error;
        private readonly object arg;
        private readonly object arg2;
        private readonly object arg3;
        private readonly Exception cause;

        public TemplateMessage(ErrorType error)
        {
            this.error = error;
        }

        public TemplateMessage(ErrorType error, Template self)
            : this(error)
        {
            this.self = self;
        }

        public TemplateMessage(ErrorType error, Template self, Exception cause)
            : this(error, self)
        {
            this.cause = cause;
        }

        public TemplateMessage(ErrorType error, Template self, Exception cause, object arg)
            : this(error, self, cause)
        {
            this.arg = arg;
        }

        public TemplateMessage(ErrorType error, Template self, Exception cause, IToken where, object arg)
            : this(error, self, cause, where)
        {
            this.arg = arg;
        }

        public TemplateMessage(ErrorType error, Template self, Exception cause, object arg, object arg2)
            : this(error, self, cause, arg)
        {
            this.arg2 = arg2;
        }

        public TemplateMessage(ErrorType error, Template self, Exception cause, object arg, object arg2, object arg3)
            : this(error, self, cause, arg, arg2)
        {
            this.arg3 = arg3;
        }

        public Template Self
        {
            get
            {
                return self;
            }
        }

        public ErrorType Error
        {
            get
            {
                return error;
            }
        }

        public object Arg
        {
            get
            {
                return arg;
            }
        }

        public object Arg2
        {
            get
            {
                return arg2;
            }
        }

        public object Arg3
        {
            get
            {
                return arg3;
            }
        }

        public Exception Cause
        {
            get
            {
                return cause;
            }
        }

        public override string ToString()
        {
            StringWriter sw = new StringWriter();
            string msg = string.Format(error.Message, arg, arg2, arg3);
            sw.Write(msg);
            if (cause != null)
            {
                sw.WriteLine();
                sw.Write("Caused by: ");
                sw.WriteLine(cause.Message);
                sw.Write(cause.StackTrace);
            }
            return sw.ToString();
        }
    }
}
