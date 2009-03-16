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
    using Antlr.Runtime;
    using Antlr.Runtime.Tree;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using CommonToken = Antlr.Runtime.CommonToken;
    using IToken = Antlr.Runtime.IToken;
    using StringBuilder = System.Text.StringBuilder;

    /** Test the tree node stream. */
    [TestClass]
    public class TestTreeNodeStream : BaseTest
    {

        /** Build new stream; let's us override to test other streams. */
        public virtual ITreeNodeStream newStream( object t )
        {
            return new CommonTreeNodeStream( t );
        }

        public virtual string ToTokenTypeString( ITreeNodeStream stream )
        {
            return ( (CommonTreeNodeStream)stream ).ToTokenTypeString();
        }

        [TestMethod]
        public void TestSingleNode()
        {
            ITree t = new CommonTree( new CommonToken( 101 ) );

            ITreeNodeStream stream = newStream( t );
            string expecting = " 101";
            string found = ToNodesOnlyString( stream );
            assertEquals( expecting, found );

            expecting = " 101";
            found = ToTokenTypeString( stream );
            assertEquals( expecting, found );
        }

        [TestMethod]
        public void Test4Nodes() /*throws Exception*/ {
            // ^(101 ^(102 103) 104)
            ITree t = new CommonTree( new CommonToken( 101 ) );
            t.AddChild( new CommonTree( new CommonToken( 102 ) ) );
            t.GetChild( 0 ).AddChild( new CommonTree( new CommonToken( 103 ) ) );
            t.AddChild( new CommonTree( new CommonToken( 104 ) ) );

            ITreeNodeStream stream = newStream( t );
            string expecting = " 101 102 103 104";
            string found = ToNodesOnlyString( stream );
            assertEquals( expecting, found );

            expecting = " 101 2 102 2 103 3 104 3";
            found = ToTokenTypeString( stream );
            assertEquals( expecting, found );
        }

        [TestMethod]
        public void TestList() /*throws Exception*/ {
            ITree root = new CommonTree( (IToken)null );

            ITree t = new CommonTree( new CommonToken( 101 ) );
            t.AddChild( new CommonTree( new CommonToken( 102 ) ) );
            t.GetChild( 0 ).AddChild( new CommonTree( new CommonToken( 103 ) ) );
            t.AddChild( new CommonTree( new CommonToken( 104 ) ) );

            ITree u = new CommonTree( new CommonToken( 105 ) );

            root.AddChild( t );
            root.AddChild( u );

            ITreeNodeStream stream = newStream( root );
            string expecting = " 101 102 103 104 105";
            string found = ToNodesOnlyString( stream );
            assertEquals( expecting, found );

            expecting = " 101 2 102 2 103 3 104 3 105";
            found = ToTokenTypeString( stream );
            assertEquals( expecting, found );
        }

        [TestMethod]
        public void TestFlatList() /*throws Exception*/ {
            ITree root = new CommonTree( (IToken)null );

            root.AddChild( new CommonTree( new CommonToken( 101 ) ) );
            root.AddChild( new CommonTree( new CommonToken( 102 ) ) );
            root.AddChild( new CommonTree( new CommonToken( 103 ) ) );

            ITreeNodeStream stream = newStream( root );
            string expecting = " 101 102 103";
            string found = ToNodesOnlyString( stream );
            assertEquals( expecting, found );

            expecting = " 101 102 103";
            found = ToTokenTypeString( stream );
            assertEquals( expecting, found );
        }

        [TestMethod]
        public void TestListWithOneNode() /*throws Exception*/ {
            ITree root = new CommonTree( (IToken)null );

            root.AddChild( new CommonTree( new CommonToken( 101 ) ) );

            ITreeNodeStream stream = newStream( root );
            string expecting = " 101";
            string found = ToNodesOnlyString( stream );
            assertEquals( expecting, found );

            expecting = " 101";
            found = ToTokenTypeString( stream );
            assertEquals( expecting, found );
        }

        [TestMethod]
        public void TestAoverB() /*throws Exception*/ {
            ITree t = new CommonTree( new CommonToken( 101 ) );
            t.AddChild( new CommonTree( new CommonToken( 102 ) ) );

            ITreeNodeStream stream = newStream( t );
            string expecting = " 101 102";
            string found = ToNodesOnlyString( stream );
            assertEquals( expecting, found );

            expecting = " 101 2 102 3";
            found = ToTokenTypeString( stream );
            assertEquals( expecting, found );
        }

        [TestMethod]
        public void TestLT() /*throws Exception*/ {
            // ^(101 ^(102 103) 104)
            ITree t = new CommonTree( new CommonToken( 101 ) );
            t.AddChild( new CommonTree( new CommonToken( 102 ) ) );
            t.GetChild( 0 ).AddChild( new CommonTree( new CommonToken( 103 ) ) );
            t.AddChild( new CommonTree( new CommonToken( 104 ) ) );

            ITreeNodeStream stream = newStream( t );
            assertEquals( 101, ( (ITree)stream.LT( 1 ) ).Type );
            assertEquals( TokenConstants.DOWN, ( (ITree)stream.LT( 2 ) ).Type );
            assertEquals( 102, ( (ITree)stream.LT( 3 ) ).Type );
            assertEquals( TokenConstants.DOWN, ( (ITree)stream.LT( 4 ) ).Type );
            assertEquals( 103, ( (ITree)stream.LT( 5 ) ).Type );
            assertEquals( TokenConstants.UP, ( (ITree)stream.LT( 6 ) ).Type );
            assertEquals( 104, ( (ITree)stream.LT( 7 ) ).Type );
            assertEquals( TokenConstants.UP, ( (ITree)stream.LT( 8 ) ).Type );
            assertEquals( TokenConstants.EOF, ( (ITree)stream.LT( 9 ) ).Type );
            // check way ahead
            assertEquals( TokenConstants.EOF, ( (ITree)stream.LT( 100 ) ).Type );
        }

        [TestMethod]
        public void TestMarkRewindEntire() /*throws Exception*/ {
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
            int m = stream.Mark(); // MARK
            for ( int k = 1; k <= 13; k++ )
            { // consume til end
                stream.LT( 1 );
                stream.Consume();
            }
            assertEquals( TokenConstants.EOF, ( (ITree)stream.LT( 1 ) ).Type );
            assertEquals( TokenConstants.UP, ( (ITree)stream.LT( -1 ) ).Type );
            stream.Rewind( m );      // REWIND

            // consume til end again :)
            for ( int k = 1; k <= 13; k++ )
            { // consume til end
                stream.LT( 1 );
                stream.Consume();
            }
            assertEquals( TokenConstants.EOF, ( (ITree)stream.LT( 1 ) ).Type );
            assertEquals( TokenConstants.UP, ( (ITree)stream.LT( -1 ) ).Type );
        }

        [TestMethod]
        public void TestMarkRewindInMiddle() /*throws Exception*/ {
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
            for ( int k = 1; k <= 7; k++ )
            { // consume til middle
                //System.out.println(((Tree)stream.LT(1)).getType());
                stream.Consume();
            }
            assertEquals( 107, ( (ITree)stream.LT( 1 ) ).Type );
            stream.Mark(); // MARK
            stream.Consume(); // consume 107
            stream.Consume(); // consume UP
            stream.Consume(); // consume UP
            stream.Consume(); // consume 104
            stream.Rewind();      // REWIND
            stream.Mark(); // keep saving nodes though

            assertEquals( 107, ( (ITree)stream.LT( 1 ) ).Type );
            stream.Consume();
            assertEquals( TokenConstants.UP, ( (ITree)stream.LT( 1 ) ).Type );
            stream.Consume();
            assertEquals( TokenConstants.UP, ( (ITree)stream.LT( 1 ) ).Type );
            stream.Consume();
            assertEquals( 104, ( (ITree)stream.LT( 1 ) ).Type );
            stream.Consume();
            // now we're past rewind position
            assertEquals( 105, ( (ITree)stream.LT( 1 ) ).Type );
            stream.Consume();
            assertEquals( TokenConstants.UP, ( (ITree)stream.LT( 1 ) ).Type );
            stream.Consume();
            assertEquals( TokenConstants.EOF, ( (ITree)stream.LT( 1 ) ).Type );
            assertEquals( TokenConstants.UP, ( (ITree)stream.LT( -1 ) ).Type );
        }

        [TestMethod]
        public void TestMarkRewindNested() /*throws Exception*/ {
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
            int m = stream.Mark(); // MARK at start
            stream.Consume(); // consume 101
            stream.Consume(); // consume DN
            int m2 = stream.Mark(); // MARK on 102
            stream.Consume(); // consume 102
            stream.Consume(); // consume DN
            stream.Consume(); // consume 103
            stream.Consume(); // consume 106
            stream.Rewind( m2 );      // REWIND to 102
            assertEquals( 102, ( (ITree)stream.LT( 1 ) ).Type );
            stream.Consume();
            assertEquals( TokenConstants.DOWN, ( (ITree)stream.LT( 1 ) ).Type );
            stream.Consume();
            // stop at 103 and rewind to start
            stream.Rewind( m ); // REWIND to 101
            assertEquals( 101, ( (ITree)stream.LT( 1 ) ).Type );
            stream.Consume();
            assertEquals( TokenConstants.DOWN, ( (ITree)stream.LT( 1 ) ).Type );
            stream.Consume();
            assertEquals( 102, ( (ITree)stream.LT( 1 ) ).Type );
            stream.Consume();
            assertEquals( TokenConstants.DOWN, ( (ITree)stream.LT( 1 ) ).Type );
        }

        [TestMethod]
        public void TestSeekFromStart() /*throws Exception*/ {
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
            stream.Seek( 7 );   // seek to 107
            assertEquals( 107, ( (ITree)stream.LT( 1 ) ).Type );
            stream.Consume(); // consume 107
            stream.Consume(); // consume UP
            stream.Consume(); // consume UP
            assertEquals( 104, ( (ITree)stream.LT( 1 ) ).Type );
        }

        public string ToNodesOnlyString( ITreeNodeStream nodes )
        {
            ITreeAdaptor adaptor = nodes.TreeAdaptor;
            StringBuilder buf = new StringBuilder();
            object o = nodes.LT( 1 );
            int type = adaptor.GetType( o );
            while ( o != null && type != TokenConstants.EOF )
            {
                if ( !( type == TokenConstants.DOWN || type == TokenConstants.UP ) )
                {
                    buf.Append( " " );
                    buf.Append( type );
                }
                nodes.Consume();
                o = nodes.LT( 1 );
                type = adaptor.GetType( o );
            }
            return buf.ToString();
        }
    }
}
