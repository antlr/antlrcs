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
    using Antlr.Runtime.Tree;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using ArgumentException = System.ArgumentException;
    using CommonToken = Antlr.Runtime.CommonToken;
    using IToken = Antlr.Runtime.IToken;

    [TestClass]
    public class TestTrees : BaseTest
    {
        ITreeAdaptor adaptor = new CommonTreeAdaptor();
        protected bool debug = false;

        /*static*/
        class V : CommonTree
        {
            public int x;
            public V( IToken t )
            {
                this.Token = t;
            }
            public V( int ttype, int x )
            {
                this.x = x;
                Token = new CommonToken( ttype );
            }
            public V( int ttype, IToken t, int x )
            {
                Token = t;
                this.x = x;
            }
            public override string ToString()
            {
                return ( Token != null ? Token.Text : "" ) + "<V>";
            }
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestSingleNode() /*throws Exception*/ {
            CommonTree t = new CommonTree( new CommonToken( 101 ) );
            Assert.IsNull( t.Parent );
            Assert.AreEqual( -1, t.ChildIndex );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestTwoChildrenOfNilRoot() /*throws Exception*/ {
            CommonTree root_0 = (CommonTree)adaptor.Nil();
            CommonTree t = new V( 101, 2 );
            CommonTree u = new V( new CommonToken( 102, "102" ) );
            adaptor.AddChild( root_0, t );
            adaptor.AddChild( root_0, u );
            Assert.IsNull( root_0.Parent );
            Assert.AreEqual( -1, root_0.ChildIndex );
            Assert.AreEqual( 0, t.ChildIndex );
            Assert.AreEqual( 1, u.ChildIndex );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void Test4Nodes() /*throws Exception*/ {
            // ^(101 ^(102 103) 104)
            CommonTree r0 = new CommonTree( new CommonToken( 101 ) );
            r0.AddChild( new CommonTree( new CommonToken( 102 ) ) );
            r0.GetChild( 0 ).AddChild( new CommonTree( new CommonToken( 103 ) ) );
            r0.AddChild( new CommonTree( new CommonToken( 104 ) ) );

            Assert.IsNull( r0.Parent );
            Assert.AreEqual( -1, r0.ChildIndex );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestList() /*throws Exception*/ {
            // ^(nil 101 102 103)
            CommonTree r0 = new CommonTree( (IToken)null );
            CommonTree c0, c1, c2;
            r0.AddChild( c0 = new CommonTree( new CommonToken( 101 ) ) );
            r0.AddChild( c1 = new CommonTree( new CommonToken( 102 ) ) );
            r0.AddChild( c2 = new CommonTree( new CommonToken( 103 ) ) );

            Assert.IsNull( r0.Parent );
            Assert.AreEqual( -1, r0.ChildIndex );
            Assert.AreEqual( r0, c0.Parent );
            Assert.AreEqual( 0, c0.ChildIndex );
            Assert.AreEqual( r0, c1.Parent );
            Assert.AreEqual( 1, c1.ChildIndex );
            Assert.AreEqual( r0, c2.Parent );
            Assert.AreEqual( 2, c2.ChildIndex );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestList2() /*throws Exception*/ {
            // Add child ^(nil 101 102 103) to root 5
            // should pull 101 102 103 directly to become 5's child list
            CommonTree root = new CommonTree( new CommonToken( 5 ) );

            // child tree
            CommonTree r0 = new CommonTree( (IToken)null );
            CommonTree c0, c1, c2;
            r0.AddChild( c0 = new CommonTree( new CommonToken( 101 ) ) );
            r0.AddChild( c1 = new CommonTree( new CommonToken( 102 ) ) );
            r0.AddChild( c2 = new CommonTree( new CommonToken( 103 ) ) );

            root.AddChild( r0 );

            Assert.IsNull( root.Parent );
            Assert.AreEqual( -1, root.ChildIndex );
            // check children of root all point at root
            Assert.AreEqual( root, c0.Parent );
            Assert.AreEqual( 0, c0.ChildIndex );
            Assert.AreEqual( root, c0.Parent );
            Assert.AreEqual( 1, c1.ChildIndex );
            Assert.AreEqual( root, c0.Parent );
            Assert.AreEqual( 2, c2.ChildIndex );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestAddListToExistChildren() /*throws Exception*/ {
            // Add child ^(nil 101 102 103) to root ^(5 6)
            // should add 101 102 103 to end of 5's child list
            CommonTree root = new CommonTree( new CommonToken( 5 ) );
            root.AddChild( new CommonTree( new CommonToken( 6 ) ) );

            // child tree
            CommonTree r0 = new CommonTree( (IToken)null );
            CommonTree c0, c1, c2;
            r0.AddChild( c0 = new CommonTree( new CommonToken( 101 ) ) );
            r0.AddChild( c1 = new CommonTree( new CommonToken( 102 ) ) );
            r0.AddChild( c2 = new CommonTree( new CommonToken( 103 ) ) );

            root.AddChild( r0 );

            Assert.IsNull( root.Parent );
            Assert.AreEqual( -1, root.ChildIndex );
            // check children of root all point at root
            Assert.AreEqual( root, c0.Parent );
            Assert.AreEqual( 1, c0.ChildIndex );
            Assert.AreEqual( root, c0.Parent );
            Assert.AreEqual( 2, c1.ChildIndex );
            Assert.AreEqual( root, c0.Parent );
            Assert.AreEqual( 3, c2.ChildIndex );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestDupTree() /*throws Exception*/ {
            // ^(101 ^(102 103 ^(106 107) ) 104 105)
            CommonTree r0 = new CommonTree( new CommonToken( 101 ) );
            CommonTree r1 = new CommonTree( new CommonToken( 102 ) );
            r0.AddChild( r1 );
            r1.AddChild( new CommonTree( new CommonToken( 103 ) ) );
            ITree r2 = new CommonTree( new CommonToken( 106 ) );
            r2.AddChild( new CommonTree( new CommonToken( 107 ) ) );
            r1.AddChild( r2 );
            r0.AddChild( new CommonTree( new CommonToken( 104 ) ) );
            r0.AddChild( new CommonTree( new CommonToken( 105 ) ) );

            CommonTree dup = (CommonTree)( new CommonTreeAdaptor() ).DupTree( r0 );

            Assert.IsNull( dup.Parent );
            Assert.AreEqual( -1, dup.ChildIndex );
            dup.SanityCheckParentAndChildIndexes();
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestBecomeRoot() /*throws Exception*/ {
            // 5 becomes new root of ^(nil 101 102 103)
            CommonTree newRoot = new CommonTree( new CommonToken( 5 ) );

            CommonTree oldRoot = new CommonTree( (IToken)null );
            oldRoot.AddChild( new CommonTree( new CommonToken( 101 ) ) );
            oldRoot.AddChild( new CommonTree( new CommonToken( 102 ) ) );
            oldRoot.AddChild( new CommonTree( new CommonToken( 103 ) ) );

            ITreeAdaptor adaptor = new CommonTreeAdaptor();
            adaptor.BecomeRoot( newRoot, oldRoot );
            newRoot.SanityCheckParentAndChildIndexes();
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestBecomeRoot2() /*throws Exception*/ {
            // 5 becomes new root of ^(101 102 103)
            CommonTree newRoot = new CommonTree( new CommonToken( 5 ) );

            CommonTree oldRoot = new CommonTree( new CommonToken( 101 ) );
            oldRoot.AddChild( new CommonTree( new CommonToken( 102 ) ) );
            oldRoot.AddChild( new CommonTree( new CommonToken( 103 ) ) );

            ITreeAdaptor adaptor = new CommonTreeAdaptor();
            adaptor.BecomeRoot( newRoot, oldRoot );
            newRoot.SanityCheckParentAndChildIndexes();
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestBecomeRoot3() /*throws Exception*/ {
            // ^(nil 5) becomes new root of ^(nil 101 102 103)
            CommonTree newRoot = new CommonTree( (IToken)null );
            newRoot.AddChild( new CommonTree( new CommonToken( 5 ) ) );

            CommonTree oldRoot = new CommonTree( (IToken)null );
            oldRoot.AddChild( new CommonTree( new CommonToken( 101 ) ) );
            oldRoot.AddChild( new CommonTree( new CommonToken( 102 ) ) );
            oldRoot.AddChild( new CommonTree( new CommonToken( 103 ) ) );

            ITreeAdaptor adaptor = new CommonTreeAdaptor();
            adaptor.BecomeRoot( newRoot, oldRoot );
            newRoot.SanityCheckParentAndChildIndexes();
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestBecomeRoot5() /*throws Exception*/ {
            // ^(nil 5) becomes new root of ^(101 102 103)
            CommonTree newRoot = new CommonTree( (IToken)null );
            newRoot.AddChild( new CommonTree( new CommonToken( 5 ) ) );

            CommonTree oldRoot = new CommonTree( new CommonToken( 101 ) );
            oldRoot.AddChild( new CommonTree( new CommonToken( 102 ) ) );
            oldRoot.AddChild( new CommonTree( new CommonToken( 103 ) ) );

            ITreeAdaptor adaptor = new CommonTreeAdaptor();
            adaptor.BecomeRoot( newRoot, oldRoot );
            newRoot.SanityCheckParentAndChildIndexes();
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestBecomeRoot6() /*throws Exception*/ {
            // emulates construction of ^(5 6)
            CommonTree root_0 = (CommonTree)adaptor.Nil();
            CommonTree root_1 = (CommonTree)adaptor.Nil();
            root_1 = (CommonTree)adaptor.BecomeRoot( new CommonTree( new CommonToken( 5 ) ), root_1 );

            adaptor.AddChild( root_1, new CommonTree( new CommonToken( 6 ) ) );

            adaptor.AddChild( root_0, root_1 );

            root_0.SanityCheckParentAndChildIndexes();
        }

        // Test replaceChildren

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        [ExpectedException(typeof(ArgumentException))]
        public void TestReplaceWithNoChildren() /*throws Exception*/ {
            CommonTree t = new CommonTree( new CommonToken( 101 ) );
            CommonTree newChild = new CommonTree( new CommonToken( 5 ) );
            t.ReplaceChildren( 0, 0, newChild );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestReplaceWithOneChildren() /*throws Exception*/ {
            // assume token type 99 and use text
            CommonTree t = new CommonTree( new CommonToken( 99, "a" ) );
            CommonTree c0 = new CommonTree( new CommonToken( 99, "b" ) );
            t.AddChild( c0 );

            CommonTree newChild = new CommonTree( new CommonToken( 99, "c" ) );
            t.ReplaceChildren( 0, 0, newChild );
            string expecting = "(a c)";
            Assert.AreEqual( expecting, t.ToStringTree() );
            t.SanityCheckParentAndChildIndexes();
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestReplaceInMiddle() /*throws Exception*/ {
            CommonTree t = new CommonTree( new CommonToken( 99, "a" ) );
            t.AddChild( new CommonTree( new CommonToken( 99, "b" ) ) );
            t.AddChild( new CommonTree( new CommonToken( 99, "c" ) ) ); // index 1
            t.AddChild( new CommonTree( new CommonToken( 99, "d" ) ) );

            CommonTree newChild = new CommonTree( new CommonToken( 99, "x" ) );
            t.ReplaceChildren( 1, 1, newChild );
            string expecting = "(a b x d)";
            Assert.AreEqual( expecting, t.ToStringTree() );
            t.SanityCheckParentAndChildIndexes();
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestReplaceAtLeft() /*throws Exception*/ {
            CommonTree t = new CommonTree( new CommonToken( 99, "a" ) );
            t.AddChild( new CommonTree( new CommonToken( 99, "b" ) ) ); // index 0
            t.AddChild( new CommonTree( new CommonToken( 99, "c" ) ) );
            t.AddChild( new CommonTree( new CommonToken( 99, "d" ) ) );

            CommonTree newChild = new CommonTree( new CommonToken( 99, "x" ) );
            t.ReplaceChildren( 0, 0, newChild );
            string expecting = "(a x c d)";
            Assert.AreEqual( expecting, t.ToStringTree() );
            t.SanityCheckParentAndChildIndexes();
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestReplaceAtRight() /*throws Exception*/ {
            CommonTree t = new CommonTree( new CommonToken( 99, "a" ) );
            t.AddChild( new CommonTree( new CommonToken( 99, "b" ) ) );
            t.AddChild( new CommonTree( new CommonToken( 99, "c" ) ) );
            t.AddChild( new CommonTree( new CommonToken( 99, "d" ) ) ); // index 2

            CommonTree newChild = new CommonTree( new CommonToken( 99, "x" ) );
            t.ReplaceChildren( 2, 2, newChild );
            string expecting = "(a b c x)";
            Assert.AreEqual( expecting, t.ToStringTree() );
            t.SanityCheckParentAndChildIndexes();
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestReplaceOneWithTwoAtLeft() /*throws Exception*/ {
            CommonTree t = new CommonTree( new CommonToken( 99, "a" ) );
            t.AddChild( new CommonTree( new CommonToken( 99, "b" ) ) );
            t.AddChild( new CommonTree( new CommonToken( 99, "c" ) ) );
            t.AddChild( new CommonTree( new CommonToken( 99, "d" ) ) );

            CommonTree newChildren = (CommonTree)adaptor.Nil();
            newChildren.AddChild( new CommonTree( new CommonToken( 99, "x" ) ) );
            newChildren.AddChild( new CommonTree( new CommonToken( 99, "y" ) ) );

            t.ReplaceChildren( 0, 0, newChildren );
            string expecting = "(a x y c d)";
            Assert.AreEqual( expecting, t.ToStringTree() );
            t.SanityCheckParentAndChildIndexes();
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestReplaceOneWithTwoAtRight() /*throws Exception*/ {
            CommonTree t = new CommonTree( new CommonToken( 99, "a" ) );
            t.AddChild( new CommonTree( new CommonToken( 99, "b" ) ) );
            t.AddChild( new CommonTree( new CommonToken( 99, "c" ) ) );
            t.AddChild( new CommonTree( new CommonToken( 99, "d" ) ) );

            CommonTree newChildren = (CommonTree)adaptor.Nil();
            newChildren.AddChild( new CommonTree( new CommonToken( 99, "x" ) ) );
            newChildren.AddChild( new CommonTree( new CommonToken( 99, "y" ) ) );

            t.ReplaceChildren( 2, 2, newChildren );
            string expecting = "(a b c x y)";
            Assert.AreEqual( expecting, t.ToStringTree() );
            t.SanityCheckParentAndChildIndexes();
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestReplaceOneWithTwoInMiddle() /*throws Exception*/ {
            CommonTree t = new CommonTree( new CommonToken( 99, "a" ) );
            t.AddChild( new CommonTree( new CommonToken( 99, "b" ) ) );
            t.AddChild( new CommonTree( new CommonToken( 99, "c" ) ) );
            t.AddChild( new CommonTree( new CommonToken( 99, "d" ) ) );

            CommonTree newChildren = (CommonTree)adaptor.Nil();
            newChildren.AddChild( new CommonTree( new CommonToken( 99, "x" ) ) );
            newChildren.AddChild( new CommonTree( new CommonToken( 99, "y" ) ) );

            t.ReplaceChildren( 1, 1, newChildren );
            string expecting = "(a b x y d)";
            Assert.AreEqual( expecting, t.ToStringTree() );
            t.SanityCheckParentAndChildIndexes();
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestReplaceTwoWithOneAtLeft() /*throws Exception*/ {
            CommonTree t = new CommonTree( new CommonToken( 99, "a" ) );
            t.AddChild( new CommonTree( new CommonToken( 99, "b" ) ) );
            t.AddChild( new CommonTree( new CommonToken( 99, "c" ) ) );
            t.AddChild( new CommonTree( new CommonToken( 99, "d" ) ) );

            CommonTree newChild = new CommonTree( new CommonToken( 99, "x" ) );

            t.ReplaceChildren( 0, 1, newChild );
            string expecting = "(a x d)";
            Assert.AreEqual( expecting, t.ToStringTree() );
            t.SanityCheckParentAndChildIndexes();
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestReplaceTwoWithOneAtRight() /*throws Exception*/ {
            CommonTree t = new CommonTree( new CommonToken( 99, "a" ) );
            t.AddChild( new CommonTree( new CommonToken( 99, "b" ) ) );
            t.AddChild( new CommonTree( new CommonToken( 99, "c" ) ) );
            t.AddChild( new CommonTree( new CommonToken( 99, "d" ) ) );

            CommonTree newChild = new CommonTree( new CommonToken( 99, "x" ) );

            t.ReplaceChildren( 1, 2, newChild );
            string expecting = "(a b x)";
            Assert.AreEqual( expecting, t.ToStringTree() );
            t.SanityCheckParentAndChildIndexes();
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestReplaceAllWithOne() /*throws Exception*/ {
            CommonTree t = new CommonTree( new CommonToken( 99, "a" ) );
            t.AddChild( new CommonTree( new CommonToken( 99, "b" ) ) );
            t.AddChild( new CommonTree( new CommonToken( 99, "c" ) ) );
            t.AddChild( new CommonTree( new CommonToken( 99, "d" ) ) );

            CommonTree newChild = new CommonTree( new CommonToken( 99, "x" ) );

            t.ReplaceChildren( 0, 2, newChild );
            string expecting = "(a x)";
            Assert.AreEqual( expecting, t.ToStringTree() );
            t.SanityCheckParentAndChildIndexes();
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestReplaceAllWithTwo() /*throws Exception*/ {
            CommonTree t = new CommonTree( new CommonToken( 99, "a" ) );
            t.AddChild( new CommonTree( new CommonToken( 99, "b" ) ) );
            t.AddChild( new CommonTree( new CommonToken( 99, "c" ) ) );
            t.AddChild( new CommonTree( new CommonToken( 99, "d" ) ) );

            CommonTree newChildren = (CommonTree)adaptor.Nil();
            newChildren.AddChild( new CommonTree( new CommonToken( 99, "x" ) ) );
            newChildren.AddChild( new CommonTree( new CommonToken( 99, "y" ) ) );

            t.ReplaceChildren( 0, 2, newChildren );
            string expecting = "(a x y)";
            Assert.AreEqual( expecting, t.ToStringTree() );
            t.SanityCheckParentAndChildIndexes();
        }
    }
}
