/*
 * [The "BSD licence"]
 * Copyright (c) 2005-2008 Terence Parr
 * All rights reserved.
 *
 * Conversion to C#:
 * Copyright (c) 2008-2009 Sam Harwell
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

namespace StringTemplate
{
    using Exception = System.Exception;
    using StringBuilder = System.Text.StringBuilder;

    public class TemplateMessage
    {
        /** if in debug mode, has create instance, add attr events and eval
         *  template events.
         */
        private ErrorType error;
        private object arg1;
        private object arg2;

        public TemplateMessage(ErrorType error)
            : this(error, null, null, null, null)
        {
        }

        public TemplateMessage(ErrorType error, Template template)
            : this(error, template, null, null, null)
        {
        }

        public TemplateMessage(ErrorType error, Template template, Exception source)
            : this(error, template, source, null, null)
        {
        }

        public TemplateMessage(ErrorType error, Template template, Exception source, object arg)
            : this(error, template, source, arg, null)
        {
        }

        public TemplateMessage(ErrorType error, Template template, Exception source, object arg1, object arg2)
        {
            this.error = error;
            this.Template = template;
            this.Source = source;
            this.arg1 = arg1;
            this.arg2 = arg2;
        }

        public Template Template
        {
            get;
            private set;
        }

        public string Message
        {
            get
            {
                return string.Format(error.MessageFormat, arg1, arg2);
            }
        }

        public Exception Source
        {
            get;
            private set;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            if (Template != null)
                builder.Append(Template.GetEnclosingInstanceStackString() + " ");

            builder.AppendLine(Message);

            if (Source != null)
            {
                builder.AppendLine(Source.StackTrace);
            }

            return builder.ToString();
        }
    }
}
