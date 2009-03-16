/*
 * [The "BSD licence"]
 * Copyright (c) 2005-2008 Terence Parr
 * All rights reserved.
 *
 * Conversion to C#:
 * Copyright (c) 2008-2009 Sam Harwell, Pixel Mine, Inc.
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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Antlr.Runtime.Tree;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Antlr.Runtime;

    [TestClass]
    public class TestBufferedTreeNodeStream : TestTreeNodeStream
    {
        // inherits tests; these methods make it use a new buffer

        public override ITreeNodeStream newStream( object t )
        {
            return new BufferedTreeNodeStream( t );
        }

        public override string ToTokenTypeString( ITreeNodeStream stream )
        {
            return ( (BufferedTreeNodeStream)stream ).ToTokenTypeString();
        }

        [TestMethod]
        public void TestSeek()
        {
            // ^(101 ^(102 103 ^(106 107) ) 104 105)
            // stream has 7 real + 6 nav nodes
            // Sequence of types: 101 DN 102 DN 103 106 DN 107 UP UP 104 105 UP EOF
            ITree r0 = new CommonTree( new CommonToken( 101 ) );
            ITree r1 = new CommonTree( new CommonToken( 102 ) );
            r0.AddChild( r1 );
            r1.AddChild( new CommonTree( new CommonToken( 103 ) ) );
            ITree r2 = new CommonTree( new CommonToken( 106 ) );
            r2.AddChild( new CommonTree( new CommonToken( 107 ) ) );
            r1.AddChild( r2 );
            r0.AddChild( new CommonTree( new CommonToken( 104 ) ) );
            r0.AddChild( new CommonTree( new CommonToken( 105 ) ) );

            ITreeNodeStream stream = newStream( r0 );
            stream.Consume(); // consume 101
            stream.Consume(); // consume DN
            stream.Consume(); // consume 102
            stream.Seek( 7 );   // seek to 107
            assertEquals( 107, ( (ITree)stream.LT( 1 ) ).Type );
            stream.Consume(); // consume 107
            stream.Consume(); // consume UP
            stream.Consume(); // consume UP
            assertEquals( 104, ( (ITree)stream.LT( 1 ) ).Type );
        }
    }
}
