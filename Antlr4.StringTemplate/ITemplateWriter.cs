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

namespace Antlr4.StringTemplate
{
    /** Generic StringTemplate output writer filter.
     *
     *  Literals and the elements of expressions are emitted via Write().
     *  Separators are emitted via WriteSeparator() because they must be
     *  handled specially when wrapping lines (we don't want to wrap
     *  in between an element and it's separator).
     */
    public interface ITemplateWriter
    {
        /** Return the absolute char index into the output of the char
         *  we're about to Write.  Returns 0 if no char written yet.
         */
        int Index
        {
            get;
        }

        int LineWidth
        {
            get;
            set;
        }

        void PushIndentation(string indent);

        string PopIndentation();

        void PushAnchorPoint();

        void PopAnchorPoint();

        /** Write the string and return how many actual chars were written.
         *  With autoindentation and wrapping, more chars than length(str)
         *  can be emitted.  No wrapping is done.
         */
        int Write(string str);

        /** Same as Write, but wrap lines using the indicated string as the
         *  wrap character (such as "\n").
         */
        int Write(string str, string wrap);

        /** Because we evaluate Template instance by invoking exec() again, we
         *  can't pass options in.  So the WRITE instruction of an applied
         *  template (such as when we wrap in between template applications
         *  like &lt;data:{v|[&lt;v&gt;]}; wrap&gt;) we need to Write the wrap string
         *  before calling exec().  We expose just like for the separator.
         *  See Interpreter.WriteObject where it checks for Template instance.
         *  If POJO, WritePlainObject passes wrap to ITemplateWriter's
         *
         *     Write(String str, String wrap)
         *
         *  method.  Can't pass to exec(). 
         */
        int WriteWrap(string wrap);

        /** Write a separator.  Same as Write() except that a \n cannot
         *  be inserted before emitting a separator.
         */
        int WriteSeparator(string str);
    }
}
