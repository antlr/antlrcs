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

namespace Antlr3.Tool
{
    using Antlr.Runtime.JavaExtensions;

    using Exception = System.Exception;
    using StringTemplate = Antlr3.ST.StringTemplate;

    /** A generic message from the tool such as "file not found" type errors; there
     *  is no reason to create a special object for each error unlike the grammar
     *  errors, which may be rather complex.
     *
     *  Sometimes you need to pass in a filename or something to say it is "bad".
     *  Allow a generic object to be passed in and the string template can deal
     *  with just printing it or pulling a property out of it.
     *
     *  TODO what to do with exceptions?  Want stack trace for internal errors?
     */
    public class ToolMessage : Message
    {

        public ToolMessage( int msgID )
            : base( msgID, null, null )
        {
        }
        public ToolMessage( int msgID, object arg )
            : base( msgID, arg, null )
        {
        }
        public ToolMessage( int msgID, Exception e )
            : base( msgID )
        {
            this.e = e;
        }
        public ToolMessage( int msgID, object arg, object arg2 )
            : base( msgID, arg, arg2 )
        {
        }
        public ToolMessage( int msgID, object arg, Exception e )
            : base( msgID, arg, null )
        {
            this.e = e;
        }
        public override string ToString()
        {
            StringTemplate st = GetMessageTemplate();
            if ( arg != null )
            {
                st.SetAttribute( "arg", arg );
            }
            if ( arg2 != null )
            {
                st.SetAttribute( "arg2", arg2 );
            }
            if ( e != null )
            {
                st.SetAttribute( "exception", e );
                st.SetAttribute( "stackTrace", e.getStackTrace() );
            }
            return base.ToString( st );
        }
    }
}
