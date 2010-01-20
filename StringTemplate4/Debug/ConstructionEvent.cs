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
    using System.Diagnostics;

    public class ConstructionEvent
    {
        private readonly StackTrace stack;

        public ConstructionEvent()
        {
            this.stack = new StackTrace(true);
        }

        public string FileName
        {
            get
            {
                var entry = TemplateEntryPoint;
                if (entry == null)
                    return string.Empty;

                return entry.GetFileName();
            }
        }

        public int Line
        {
            get
            {
                var entry = TemplateEntryPoint;
                if (entry == null)
                    return 0;

                return entry.GetFileLineNumber();
            }
        }

        public StackFrame TemplateEntryPoint
        {
            get
            {
                var frames = stack.GetFrames();
                if (frames == null || frames.Length == 0)
                    return null;

                foreach (var frame in frames)
                {
                    var method = frame.GetMethod();
                    if (method == null)
                        continue;

                    if (method.Name == "Main")
                        return frame;

                    var ns = method.DeclaringType.Namespace;
                    if (ns == null)
                        return frame;

                    if (!ns.StartsWith("StringTemplate"))
                        return frame;
                }

                return frames[0];
            }
        }
    }
}
