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
    using Antlr.Runtime.Misc;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class TestFastQueue
    {
        [TestMethod]
        public void TestQueueNoRemove()
        {
            FastQueue<string> q = new FastQueue<string>();
            q.Enqueue( "a" );
            q.Enqueue( "b" );
            q.Enqueue( "c" );
            q.Enqueue( "d" );
            q.Enqueue( "e" );
            string expecting = "a b c d e";
            string found = q.ToString();
            Assert.AreEqual( expecting, found );
        }

        [TestMethod]
        public void TestQueueThenRemoveAll()
        {
            FastQueue<string> q = new FastQueue<string>();
            q.Enqueue( "a" );
            q.Enqueue( "b" );
            q.Enqueue( "c" );
            q.Enqueue( "d" );
            q.Enqueue( "e" );
            StringBuilder buf = new StringBuilder();
            while ( q.Count > 0 )
            {
                string o = q.Dequeue();
                buf.Append( o );
                if ( q.Count > 0 )
                    buf.Append( " " );
            }
            Assert.AreEqual( 0, q.Count, "queue should be empty" );
            string expecting = "a b c d e";
            string found = buf.ToString();
            Assert.AreEqual( expecting, found );
        }

        [TestMethod]
        public void TestQueueThenRemoveOneByOne()
        {
            StringBuilder buf = new StringBuilder();
            FastQueue<string> q = new FastQueue<string>();
            q.Enqueue( "a" );
            buf.Append( q.Dequeue() );
            q.Enqueue( "b" );
            buf.Append( q.Dequeue() );
            q.Enqueue( "c" );
            buf.Append( q.Dequeue() );
            q.Enqueue( "d" );
            buf.Append( q.Dequeue() );
            q.Enqueue( "e" );
            buf.Append( q.Dequeue() );
            Assert.AreEqual( 0, q.Count, "queue should be empty" );
            string expecting = "abcde";
            string found = buf.ToString();
            Assert.AreEqual( expecting, found );
        }

        // E r r o r s

        [TestMethod]
        public void TestGetFromEmptyQueue()
        {
            FastQueue<string> q = new FastQueue<string>();
            string msg = null;
            try
            {
                q.Dequeue();
            }
            catch ( ArgumentException nsee )
            {
                msg = nsee.Message;
            }
            string expecting = "queue index 0 > size 0";
            string found = msg;
            Assert.AreEqual( expecting, found );
        }

        [TestMethod]
        public void TestGetFromEmptyQueueAfterSomeAdds()
        {
            FastQueue<string> q = new FastQueue<string>();
            q.Enqueue( "a" );
            q.Enqueue( "b" );
            q.Dequeue();
            q.Dequeue();
            string msg = null;
            try
            {
                q.Dequeue();
            }
            catch ( ArgumentException nsee )
            {
                msg = nsee.Message;
            }
            string expecting = "queue index 0 > size 0";
            string found = msg;
            Assert.AreEqual( expecting, found );
        }

#if false
        [TestMethod]
        public void TestGetFromEmptyQueueAfterClear()
        {
            FastQueue<string> q = new FastQueue<string>();
            q.Enqueue( "a" );
            q.Enqueue( "b" );
            q.Clear();
            string msg = null;
            try
            {
                q.Dequeue();
            }
            catch ( ArgumentException nsee )
            {
                msg = nsee.Message;
            }
            string expecting = "queue index 0 > size 0";
            string found = msg;
            Assert.AreEqual( expecting, found );
        }
#endif
    }
}
