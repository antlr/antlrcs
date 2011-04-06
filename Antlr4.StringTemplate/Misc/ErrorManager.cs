/*
 * [The "BSD license"]
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
    using Antlr.Runtime;
    using Console = System.Console;
    using Exception = System.Exception;
    using Path = System.IO.Path;

    public class ErrorManager
    {
        public static ITemplateErrorListener DEFAULT_ERROR_LISTENER = new DefaultErrorListener();

        private class DefaultErrorListener : ITemplateErrorListener
        {
            public virtual void CompiletimeError(TemplateMessage msg)
            {
                Console.Error.WriteLine(msg);
            }

            public virtual void RuntimeError(TemplateMessage msg)
            {
                if (msg.Error != ErrorType.NO_SUCH_PROPERTY)
                { // ignore these
                    Console.Error.WriteLine(msg);
                }
            }

            public virtual void IOError(TemplateMessage msg)
            {
                Console.Error.WriteLine(msg);
            }

            public virtual void InternalError(TemplateMessage msg)
            {
                Console.Error.WriteLine(msg);
                // throw new Error("internal error", msg.cause);
            }

            public virtual void Error(string s)
            {
                Error(s, null);
            }

            public virtual void Error(string s, Exception e)
            {
                Console.Error.WriteLine(s);
                if (e != null)
                    Console.Error.WriteLine(e.StackTrace);
            }
        }

        private readonly ITemplateErrorListener listener;

        public ErrorManager()
            : this(DEFAULT_ERROR_LISTENER)
        {
        }

        public ErrorManager(ITemplateErrorListener listener)
        {
            this.listener = listener;
        }

        public ITemplateErrorListener Listener
        {
            get
            {
                return listener;
            }
        }

        public virtual void CompiletimeError(ErrorType error, IToken templateToken, IToken t)
        {
            string srcName = t.InputStream.SourceName;
            if (srcName != null)
                srcName = Path.GetFileName(srcName);

            listener.CompiletimeError(new TemplateCompiletimeMessage(error, srcName, templateToken, t, null, t.Text));
        }

        public virtual void LexerError(string srcName, string msg, IToken templateToken, RecognitionException e)
        {
            if (srcName != null)
                srcName = Path.GetFileName(srcName);

            listener.CompiletimeError(new TemplateLexerMessage(srcName, msg, templateToken, e));
        }

        public virtual void CompiletimeError(ErrorType error, IToken templateToken, IToken t, object arg)
        {
            string srcName = t != null && t.InputStream != null ? t.InputStream.SourceName : null;
            if (srcName != null)
                srcName = Path.GetFileName(srcName);

            listener.CompiletimeError(new TemplateCompiletimeMessage(error, srcName, templateToken, t, null, arg));
        }

        public virtual void CompiletimeError(ErrorType error, IToken templateToken, IToken t, object arg, object arg2)
        {
            string srcName = t.InputStream.SourceName;
            if (srcName != null)
                srcName = Path.GetFileName(srcName);

            listener.CompiletimeError(new TemplateCompiletimeMessage(error, srcName, templateToken, t, null, arg, arg2));
        }

        public virtual void GroupSyntaxError(ErrorType error, string srcName, RecognitionException e, string msg)
        {
            IToken t = e.Token;
            listener.CompiletimeError(new TemplateGroupCompiletimeMessage(error, srcName, e.Token, e, msg));
        }

        public virtual void GroupLexerError(ErrorType error, string srcName, RecognitionException e, string msg)
        {
            listener.CompiletimeError(new TemplateGroupCompiletimeMessage(error, srcName, e.Token, e, msg));
        }

        public virtual void RuntimeError(TemplateFrame frame, int ip, ErrorType error)
        {
            listener.RuntimeError(new TemplateRuntimeMessage(error, ip, frame));
        }

        public virtual void RuntimeError(TemplateFrame frame, int ip, ErrorType error, object arg)
        {
            listener.RuntimeError(new TemplateRuntimeMessage(error, ip, frame, arg));
        }

        public virtual void RuntimeError(TemplateFrame frame, int ip, ErrorType error, Exception e, object arg)
        {
            listener.RuntimeError(new TemplateRuntimeMessage(error, ip, frame, e, arg));
        }

        public virtual void RuntimeError(TemplateFrame frame, int ip, ErrorType error, object arg, object arg2)
        {
            listener.RuntimeError(new TemplateRuntimeMessage(error, ip, frame, null, arg, arg2));
        }

        public virtual void RuntimeError(TemplateFrame frame, int ip, ErrorType error, object arg, object arg2, object arg3)
        {
            listener.RuntimeError(new TemplateRuntimeMessage(error, ip, frame, null, arg, arg2, arg3));
        }

        public virtual void IOError(Template self, ErrorType error, Exception e)
        {
            listener.IOError(new TemplateMessage(error, self, e));
        }

        public virtual void IOError(Template self, ErrorType error, Exception e, object arg)
        {
            listener.IOError(new TemplateMessage(error, self, e, arg));
        }

        public virtual void InternalError(Template self, string msg, Exception e)
        {
            listener.InternalError(new TemplateMessage(ErrorType.INTERNAL_ERROR, self, e, msg));
        }
    }
}
