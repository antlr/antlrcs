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
    using Antlr.Runtime;
    using Console = System.Console;
    using Exception = System.Exception;
    using Path = System.IO.Path;

    /** Track errors per thread; e.g., one server transaction's errors
     *  will go in one grouping since each has it's own thread.
     */
    public class ErrorManager
    {
        public static ITemplateErrorListener DEFAULT_ERROR_LISTENER = new DefaultErrorListener();

        private class DefaultErrorListener : ITemplateErrorListener
        {
            public virtual void compileTimeError(TemplateMessage msg)
            {
                Console.Error.WriteLine(msg);
            }

            public virtual void runTimeError(TemplateMessage msg)
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

            public virtual void internalError(TemplateMessage msg)
            {
                Console.Error.WriteLine(msg);
                // throw new Error("internal error", msg.cause);
            }

            public virtual void error(string s)
            {
                error(s, null);
            }

            public virtual void error(string s, Exception e)
            {
                Console.Error.WriteLine(s);
                if (e != null)
                    Console.Error.WriteLine(e.StackTrace);
            }
        }

        /** As we parse group file, there is no embedded context.
         *  We read entire templates (e.g., <<...>>) as single
         *  tokens and so errors can use the line/charPos of the
         *  GroupLexer.  To parse templates during CodeGenerator pass,
         *  we must assume templates start at charPos=0 even when they
         *  start in middle of line:
         *
         *  t() ::= <<foo>>
         *
         *  Here, the template starts at charPos 10, but the TemplateLexer
         *  thinks that the charPos is 0.  If we want to get correct
         *  error info, we push the template token onto the context
         *  stack and then all errors are relative to that.  Should
         *  only be 0 or 1 deep, but it's general.
         */
        //public Stack<Token> context = new Stack<Token>();

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

        public virtual void compileTimeError(ErrorType error, IToken templateToken, IToken t)
        {
            string srcName = t.InputStream.SourceName;
            if (srcName != null)
                srcName = Path.GetFileName(srcName);

            listener.compileTimeError(new TemplateCompiletimeMessage(error, srcName, templateToken, t, null, t.Text));
        }

        public virtual void lexerError(string srcName, string msg, IToken templateToken, RecognitionException e)
        {
            listener.compileTimeError(new TemplateLexerMessage(srcName, msg, templateToken, e));
        }

        public virtual void compileTimeError(ErrorType error, IToken templateToken, IToken t, object arg)
        {
            string srcName = t.InputStream.SourceName;
            srcName = Path.GetFileName(srcName);
            listener.compileTimeError(new TemplateCompiletimeMessage(error, srcName, templateToken, t, null, arg));
        }

        public virtual void compileTimeError(ErrorType error, IToken templateToken, IToken t, object arg, object arg2)
        {
            string srcName = t.InputStream.SourceName;
            if (srcName != null)
                srcName = Path.GetFileName(srcName);

            listener.compileTimeError(new TemplateCompiletimeMessage(error, srcName, templateToken, t, null, arg, arg2));
        }

        public virtual void groupSyntaxError(ErrorType error, string srcName, RecognitionException e, string msg)
        {
            IToken t = e.Token;
            listener.compileTimeError(new TemplateGroupCompiletimeMessage(error, srcName, e.Token, e, msg));
        }

        public virtual void groupLexerError(ErrorType error, string srcName, RecognitionException e, string msg)
        {
            listener.compileTimeError(new TemplateGroupCompiletimeMessage(error, srcName, e.Token, e, msg));
        }

        public virtual void runTimeError(Template self, int ip, ErrorType error)
        {
            listener.runTimeError(new TemplateRuntimeMessage(error, ip, self));
        }

        public virtual void runTimeError(Template self, int ip, ErrorType error, object arg)
        {
            listener.runTimeError(new TemplateRuntimeMessage(error, ip, self, arg));
        }

        public virtual void runTimeError(Template self, int ip, ErrorType error, Exception e, object arg)
        {
            listener.runTimeError(new TemplateRuntimeMessage(error, ip, self, e, arg));
        }

        public virtual void runTimeError(Template self, int ip, ErrorType error, object arg, object arg2)
        {
            listener.runTimeError(new TemplateRuntimeMessage(error, ip, self, null, arg, arg2));
        }

        public virtual void runTimeError(Template self, int ip, ErrorType error, object arg, object arg2, object arg3)
        {
            listener.runTimeError(new TemplateRuntimeMessage(error, ip, self, null, arg, arg2, arg3));
        }

        public virtual void IOError(Template self, ErrorType error, Exception e)
        {
            listener.IOError(new TemplateMessage(error, self, e));
        }

        public virtual void IOError(Template self, ErrorType error, Exception e, object arg)
        {
            listener.IOError(new TemplateMessage(error, self, e, arg));
        }

        public virtual void internalError(Template self, string msg, Exception e)
        {
            listener.internalError(new TemplateMessage(ErrorType.INTERNAL_ERROR, self, e, msg));
        }
    }
}
