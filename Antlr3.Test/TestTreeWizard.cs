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
    using System.Collections.Generic;
    using Antlr.Runtime.JavaExtensions;
    using Antlr.Runtime.Tree;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using IList = System.Collections.IList;

    [TestClass]
    public class TestTreeWizard : BaseTest
    {
        protected static readonly string[] tokens =
            new string[] { "", "", "", "", "", "A", "B", "C", "D", "E", "ID", "VAR" };
        protected static readonly ITreeAdaptor adaptor = new CommonTreeAdaptor();

        [TestMethod]
        public void TestSingleNode() /*throws Exception*/ {
            TreeWizard wiz = new TreeWizard( adaptor, tokens );
            CommonTree t = (CommonTree)wiz.Create( "ID" );
            string found = t.ToStringTree();
            string expecting = "ID";
            assertEquals( expecting, found );
        }

        [TestMethod]
        public void TestSingleNodeWithArg() /*throws Exception*/ {
            TreeWizard wiz = new TreeWizard( adaptor, tokens );
            CommonTree t = (CommonTree)wiz.Create( "ID[foo]" );
            string found = t.ToStringTree();
            string expecting = "foo";
            assertEquals( expecting, found );
        }

        [TestMethod]
        public void TestSingleNodeTree() /*throws Exception*/ {
            TreeWizard wiz = new TreeWizard( adaptor, tokens );
            CommonTree t = (CommonTree)wiz.Create( "(A)" );
            string found = t.ToStringTree();
            string expecting = "A";
            assertEquals( expecting, found );
        }

        [TestMethod]
        public void TestSingleLevelTree() /*throws Exception*/ {
            TreeWizard wiz = new TreeWizard( adaptor, tokens );
            CommonTree t = (CommonTree)wiz.Create( "(A B C D)" );
            string found = t.ToStringTree();
            string expecting = "(A B C D)";
            assertEquals( expecting, found );
        }

        [TestMethod]
        public void TestListTree() /*throws Exception*/ {
            TreeWizard wiz = new TreeWizard( adaptor, tokens );
            CommonTree t = (CommonTree)wiz.Create( "(nil A B C)" );
            string found = t.ToStringTree();
            string expecting = "A B C";
            assertEquals( expecting, found );
        }

        [TestMethod]
        public void TestInvalidListTree() /*throws Exception*/ {
            TreeWizard wiz = new TreeWizard( adaptor, tokens );
            CommonTree t = (CommonTree)wiz.Create( "A B C" );
            assertTrue( t == null );
        }

        [TestMethod]
        public void TestDoubleLevelTree() /*throws Exception*/ {
            TreeWizard wiz = new TreeWizard( adaptor, tokens );
            CommonTree t = (CommonTree)wiz.Create( "(A (B C) (B D) E)" );
            string found = t.ToStringTree();
            string expecting = "(A (B C) (B D) E)";
            assertEquals( expecting, found );
        }

        [TestMethod]
        public void TestSingleNodeIndex() /*throws Exception*/ {
            TreeWizard wiz = new TreeWizard( adaptor, tokens );
            CommonTree t = (CommonTree)wiz.Create( "ID" );
            var m = wiz.Index( t );
            string found = m.ToElementString();
            string expecting = "{10=[ID]}";
            assertEquals( expecting, found );
        }

        [TestMethod]
        public void TestNoRepeatsIndex() /*throws Exception*/ {
            TreeWizard wiz = new TreeWizard( adaptor, tokens );
            CommonTree t = (CommonTree)wiz.Create( "(A B C D)" );
            var m = wiz.Index( t );
            string found = sortMapToString( m );
            string expecting = "{5=[A], 6=[B], 7=[C], 8=[D]}";
            assertEquals( expecting, found );
        }

        [TestMethod]
        public void TestRepeatsIndex() /*throws Exception*/ {
            TreeWizard wiz = new TreeWizard( adaptor, tokens );
            CommonTree t = (CommonTree)wiz.Create( "(A B (A C B) B D D)" );
            var m = wiz.Index( t );
            string found = sortMapToString( m );
            string expecting = "{5=[A, A], 6=[B, B, B], 7=[C], 8=[D, D]}";
            assertEquals( expecting, found );
        }

        class testNoRepeatsVisit_TreeWizard_Visitor : TreeWizard.Visitor
        {
            IList _elements;

            public testNoRepeatsVisit_TreeWizard_Visitor( IList elements )
            {
                _elements = elements;
            }

            public override void Visit( object t )
            {
                _elements.Add( t );
            }
        }

        [TestMethod]
        public void TestNoRepeatsVisit() /*throws Exception*/ {
            TreeWizard wiz = new TreeWizard( adaptor, tokens );
            CommonTree t = (CommonTree)wiz.Create( "(A B C D)" );
            IList elements = new List<object>();
            wiz.Visit( t, wiz.GetTokenType( "B" ), new testNoRepeatsVisit_TreeWizard_Visitor( elements ) );
            string found = elements.ToElementString();
            string expecting = "[B]";
            assertEquals( expecting, found );
        }

        [TestMethod]
        public void TestNoRepeatsVisit2() /*throws Exception*/ {
            TreeWizard wiz = new TreeWizard( adaptor, tokens );
            CommonTree t = (CommonTree)wiz.Create( "(A B (A C B) B D D)" );
            IList elements = new List<object>();
            wiz.Visit( t, wiz.GetTokenType( "C" ),
                          new testNoRepeatsVisit_TreeWizard_Visitor( elements ) );
            string found = elements.ToElementString();
            string expecting = "[C]";
            assertEquals( expecting, found );
        }

        [TestMethod]
        public void TestRepeatsVisit() /*throws Exception*/ {
            TreeWizard wiz = new TreeWizard( adaptor, tokens );
            CommonTree t = (CommonTree)wiz.Create( "(A B (A C B) B D D)" );
            IList elements = new List<object>();
            wiz.Visit( t, wiz.GetTokenType( "B" ),
                           new testNoRepeatsVisit_TreeWizard_Visitor( elements ) );
            string found = elements.ToElementString();
            string expecting = "[B, B, B]";
            assertEquals( expecting, found );
        }

        [TestMethod]
        public void TestRepeatsVisit2() /*throws Exception*/ {
            TreeWizard wiz = new TreeWizard( adaptor, tokens );
            CommonTree t = (CommonTree)wiz.Create( "(A B (A C B) B D D)" );
            IList elements = new List<object>();
            wiz.Visit( t, wiz.GetTokenType( "A" ),
                           new testNoRepeatsVisit_TreeWizard_Visitor( elements ) );
            string found = elements.ToElementString();
            string expecting = "[A, A]";
            assertEquals( expecting, found );
        }

        class testRepeatsVisitWithContext_TreeWizard_Visitor : TreeWizard.Visitor
        {
            IList _elements;

            public testRepeatsVisitWithContext_TreeWizard_Visitor( IList elements )
            {
                _elements = elements;
            }

            public override void Visit( object t, object parent, int childIndex, IDictionary<string, object> labels )
            {
                _elements.Add( adaptor.GetText( t ) + "@" +
                             ( parent != null ? adaptor.GetText( parent ) : "nil" ) +
                             "[" + childIndex + "]" );
            }

            public override void Visit( object t )
            {
            }
        }

        [TestMethod]
        public void TestRepeatsVisitWithContext() /*throws Exception*/ {
            TreeWizard wiz = new TreeWizard( adaptor, tokens );
            CommonTree t = (CommonTree)wiz.Create( "(A B (A C B) B D D)" );
            IList elements = new List<object>();
            wiz.Visit( t, wiz.GetTokenType( "B" ),
               new testRepeatsVisitWithContext_TreeWizard_Visitor( elements ) );
            string found = elements.ToElementString();
            string expecting = "[B@A[0], B@A[1], B@A[2]]";
            assertEquals( expecting, found );
        }

        [TestMethod]
        public void TestRepeatsVisitWithNullParentAndContext() /*throws Exception*/ {
            TreeWizard wiz = new TreeWizard( adaptor, tokens );
            CommonTree t = (CommonTree)wiz.Create( "(A B (A C B) B D D)" );
            IList elements = new List<object>();
            wiz.Visit( t, wiz.GetTokenType( "A" ),
               new testRepeatsVisitWithContext_TreeWizard_Visitor( elements ) );
            string found = elements.ToElementString();
            string expecting = "[A@nil[0], A@A[1]]";
            assertEquals( expecting, found );
        }

        [TestMethod]
        public void TestVisitPattern() /*throws Exception*/ {
            TreeWizard wiz = new TreeWizard( adaptor, tokens );
            CommonTree t = (CommonTree)wiz.Create( "(A B C (A B) D)" );
            IList elements = new List<object>();
            wiz.Visit( t, "(A B)",
                           new testNoRepeatsVisit_TreeWizard_Visitor( elements ) );
            string found = elements.ToElementString();
            string expecting = "[A]"; // shouldn't match overall root, just (A B)
            assertEquals( expecting, found );
        }

        [TestMethod]
        public void TestVisitPatternMultiple() /*throws Exception*/ {
            TreeWizard wiz = new TreeWizard( adaptor, tokens );
            CommonTree t = (CommonTree)wiz.Create( "(A B C (A B) (D (A B)))" );
            IList elements = new List<object>();
            wiz.Visit( t, "(A B)",
                          new testRepeatsVisitWithContext_TreeWizard_Visitor( elements ) );
            string found = elements.ToElementString();
            string expecting = "[A@A[2], A@D[0]]"; // shouldn't match overall root, just (A B)
            assertEquals( expecting, found );
        }

        class testVisitPatternMultipleWithLabels_TreeWizard_Visitor : TreeWizard.IContextVisitor
        {
            IList _elements;

            public testVisitPatternMultipleWithLabels_TreeWizard_Visitor( IList elements )
            {
                _elements = elements;
            }

            public virtual void Visit( object t, object parent, int childIndex, IDictionary<string, object> labels )
            {
                _elements.Add( adaptor.GetText( t ) + "@" +
                             ( parent != null ? adaptor.GetText( parent ) : "nil" ) +
                             "[" + childIndex + "]" + labels.get( "a" ) + "&" + labels.get( "b" ) );
            }

        }

        [TestMethod]
        public void TestVisitPatternMultipleWithLabels() /*throws Exception*/ {
            TreeWizard wiz = new TreeWizard( adaptor, tokens );
            CommonTree t = (CommonTree)wiz.Create( "(A B C (A[foo] B[bar]) (D (A[big] B[dog])))" );
            IList elements = new List<object>();
            wiz.Visit( t, "(%a:A %b:B)",
                          new testVisitPatternMultipleWithLabels_TreeWizard_Visitor( elements ) );
            string found = elements.ToElementString();
            string expecting = "[foo@A[2]foo&bar, big@D[0]big&dog]";
            assertEquals( expecting, found );
        }

        [TestMethod]
        public void TestParse() /*throws Exception*/ {
            TreeWizard wiz = new TreeWizard( adaptor, tokens );
            CommonTree t = (CommonTree)wiz.Create( "(A B C)" );
            bool valid = wiz.Parse( t, "(A B C)" );
            assertTrue( valid );
        }

        [TestMethod]
        public void TestParseSingleNode() /*throws Exception*/ {
            TreeWizard wiz = new TreeWizard( adaptor, tokens );
            CommonTree t = (CommonTree)wiz.Create( "A" );
            bool valid = wiz.Parse( t, "A" );
            assertTrue( valid );
        }

        [TestMethod]
        public void TestParseFlatTree() /*throws Exception*/ {
            TreeWizard wiz = new TreeWizard( adaptor, tokens );
            CommonTree t = (CommonTree)wiz.Create( "(nil A B C)" );
            bool valid = wiz.Parse( t, "(nil A B C)" );
            assertTrue( valid );
        }

        [TestMethod]
        public void TestWildcard() /*throws Exception*/ {
            TreeWizard wiz = new TreeWizard( adaptor, tokens );
            CommonTree t = (CommonTree)wiz.Create( "(A B C)" );
            bool valid = wiz.Parse( t, "(A . .)" );
            assertTrue( valid );
        }

        [TestMethod]
        public void TestParseWithText() /*throws Exception*/ {
            TreeWizard wiz = new TreeWizard( adaptor, tokens );
            CommonTree t = (CommonTree)wiz.Create( "(A B[foo] C[bar])" );
            // C pattern has no text arg so despite [bar] in t, no need
            // to match text--check structure only.
            bool valid = wiz.Parse( t, "(A B[foo] C)" );
            assertTrue( valid );
        }

        [TestMethod]
        public void TestParseWithTextFails() /*throws Exception*/ {
            TreeWizard wiz = new TreeWizard( adaptor, tokens );
            CommonTree t = (CommonTree)wiz.Create( "(A B C)" );
            bool valid = wiz.Parse( t, "(A[foo] B C)" );
            assertTrue( !valid ); // fails
        }

        [TestMethod]
        public void TestParseLabels() /*throws Exception*/ {
            TreeWizard wiz = new TreeWizard( adaptor, tokens );
            CommonTree t = (CommonTree)wiz.Create( "(A B C)" );
            IDictionary<string, object> labels = new Dictionary<string, object>();
            bool valid = wiz.Parse( t, "(%a:A %b:B %c:C)", labels );
            assertTrue( valid );
            assertEquals( "A", labels.get( "a" ).ToString() );
            assertEquals( "B", labels.get( "b" ).ToString() );
            assertEquals( "C", labels.get( "c" ).ToString() );
        }

        [TestMethod]
        public void TestParseWithWildcardLabels() /*throws Exception*/ {
            TreeWizard wiz = new TreeWizard( adaptor, tokens );
            CommonTree t = (CommonTree)wiz.Create( "(A B C)" );
            var labels = new Dictionary<string, object>();
            bool valid = wiz.Parse( t, "(A %b:. %c:.)", labels );
            assertTrue( valid );
            assertEquals( "B", labels.get( "b" ).ToString() );
            assertEquals( "C", labels.get( "c" ).ToString() );
        }

        [TestMethod]
        public void TestParseLabelsAndTestText() /*throws Exception*/ {
            TreeWizard wiz = new TreeWizard( adaptor, tokens );
            CommonTree t = (CommonTree)wiz.Create( "(A B[foo] C)" );
            var labels = new Dictionary<string, object>();
            bool valid = wiz.Parse( t, "(%a:A %b:B[foo] %c:C)", labels );
            assertTrue( valid );
            assertEquals( "A", labels.get( "a" ).ToString() );
            assertEquals( "foo", labels.get( "b" ).ToString() );
            assertEquals( "C", labels.get( "c" ).ToString() );
        }

        [TestMethod]
        public void TestParseLabelsInNestedTree() /*throws Exception*/ {
            TreeWizard wiz = new TreeWizard( adaptor, tokens );
            CommonTree t = (CommonTree)wiz.Create( "(A (B C) (D E))" );
            var labels = new Dictionary<string, object>();
            bool valid = wiz.Parse( t, "(%a:A (%b:B %c:C) (%d:D %e:E) )", labels );
            assertTrue( valid );
            assertEquals( "A", labels.get( "a" ).ToString() );
            assertEquals( "B", labels.get( "b" ).ToString() );
            assertEquals( "C", labels.get( "c" ).ToString() );
            assertEquals( "D", labels.get( "d" ).ToString() );
            assertEquals( "E", labels.get( "e" ).ToString() );
        }

        [TestMethod]
        public void TestEquals() /*throws Exception*/ {
            TreeWizard wiz = new TreeWizard( adaptor, tokens );
            CommonTree t1 = (CommonTree)wiz.Create( "(A B C)" );
            CommonTree t2 = (CommonTree)wiz.Create( "(A B C)" );
            bool same = TreeWizard.Equals( t1, t2, adaptor );
            assertTrue( same );
        }

        [TestMethod]
        public void TestEqualsWithText() /*throws Exception*/ {
            TreeWizard wiz = new TreeWizard( adaptor, tokens );
            CommonTree t1 = (CommonTree)wiz.Create( "(A B[foo] C)" );
            CommonTree t2 = (CommonTree)wiz.Create( "(A B[foo] C)" );
            bool same = TreeWizard.Equals( t1, t2, adaptor );
            assertTrue( same );
        }

        [TestMethod]
        public void TestEqualsWithMismatchedText() /*throws Exception*/ {
            TreeWizard wiz = new TreeWizard( adaptor, tokens );
            CommonTree t1 = (CommonTree)wiz.Create( "(A B[foo] C)" );
            CommonTree t2 = (CommonTree)wiz.Create( "(A B C)" );
            bool same = TreeWizard.Equals( t1, t2, adaptor );
            assertTrue( !same );
        }

        [TestMethod]
        public void TestFindPattern() /*throws Exception*/ {
            TreeWizard wiz = new TreeWizard( adaptor, tokens );
            CommonTree t = (CommonTree)wiz.Create( "(A B C (A[foo] B[bar]) (D (A[big] B[dog])))" );
            IList subtrees = wiz.Find( t, "(A B)" );
            IList elements = subtrees;
            string found = elements.ToElementString();
            string expecting = "[foo, big]";
            assertEquals( expecting, found );
        }

    }
}
