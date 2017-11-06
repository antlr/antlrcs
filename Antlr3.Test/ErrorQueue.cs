/*
 * [The "BSD licence"]
 * Copyright (c) 2005-2008 Terence Parr
 * All rights reserved.
 *
 * Conversion to C#:
 * Copyright (c) 2008 Sam Harwell, Pixel Mine, Inc.
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

namespace AntlrUnitTests
{
    using System.Collections.Generic;
    using System.Linq;

    using IANTLRErrorListener = Antlr3.Tool.IANTLRErrorListener;
    using Message = Antlr3.Tool.Message;
    using ToolMessage = Antlr3.Tool.ToolMessage;

    public class ErrorQueue : IANTLRErrorListener
    {
        internal List<string> infos = new List<string>();
        internal List<Message> errors = new List<Message>();
        internal List<Message> warnings = new List<Message>();

        public virtual void Info( string msg )
        {
            infos.Add( msg );
        }

        public virtual void Error( Message msg )
        {
            errors.Add( msg );
        }

        public virtual void Warning( Message msg )
        {
            warnings.Add( msg );
        }

        public virtual void Error( ToolMessage msg )
        {
            errors.Add( msg );
        }

        public virtual int size()
        {
            return infos.Count + errors.Count + warnings.Count;
        }

        public override string ToString()
        {
            return "infos: " + string.Join( "\r\n  ", infos.ToArray() ) +
                "errors: " + string.Join( "\r\n   ", errors.Select( m => m.ToString() ).ToArray() ) +
                "warnings: " + string.Join( "\r\n   ", warnings.Select( m => m.ToString() ).ToArray() );
        }
    }
}
