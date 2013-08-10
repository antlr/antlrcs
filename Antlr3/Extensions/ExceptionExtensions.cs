﻿/*
 * [The "BSD licence"]
 * Copyright (c) 2005-2008 Terence Parr
 * All rights reserved.
 *
 * Conversion to C#:
 * Copyright (c) 2010 Sam Harwell, Pixel Mine, Inc.
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

namespace Antlr3.Extensions
{
    using System;
    using System.Linq;

    using BindingFlags = System.Reflection.BindingFlags;
    using MethodInfo = System.Reflection.MethodInfo;
    using StackFrame = System.Diagnostics.StackFrame;
    using StackTrace = System.Diagnostics.StackTrace;
    using TextWriter = System.IO.TextWriter;

    public static class ExceptionExtensions
    {
        private static readonly Action<Exception> _internalPreserveStackTrace = GetInternalPreserveStackTraceDelegate();

        private static Action<Exception> GetInternalPreserveStackTraceDelegate()
        {
            MethodInfo methodInfo = typeof(Exception).GetMethod("InternalPreserveStackTrace", BindingFlags.Instance | BindingFlags.NonPublic);
            if (methodInfo == null)
                return null;

            return (Action<Exception>)Delegate.CreateDelegate(typeof(Action<Exception>), methodInfo);
        }

#pragma warning disable 618
        public static bool IsCritical(this Exception e)
        {
            if (e is AccessViolationException
                || e is StackOverflowException
                || e is ExecutionEngineException
                || e is OutOfMemoryException
                || e is BadImageFormatException
                || e is AppDomainUnloadedException)
            {
                return true;
            }

            return false;
        }
#pragma warning restore 618

        public static void PreserveStackTrace(this Exception e)
        {
            if (_internalPreserveStackTrace != null)
                _internalPreserveStackTrace(e);
        }

        internal static StackFrame[] GetStackTrace(this Exception e)
        {
            StackTrace trace = new StackTrace(e, true);
            StackFrame[] frames = trace.GetFrames();
            if (frames == null)
            {
                // don't include this helper function in the trace
                frames = new StackTrace(true).GetFrames().Skip(1).ToArray();
            }
            return frames;
        }

        internal static void PrintStackTrace(this Exception e, TextWriter writer)
        {
            writer.WriteLine(e.ToString());
            string trace = e.StackTrace ?? string.Empty;
            foreach (string line in trace.Split('\n', '\r'))
            {
                if (!string.IsNullOrEmpty(line))
                    writer.WriteLine("        " + line);
            }
        }
    }
}
