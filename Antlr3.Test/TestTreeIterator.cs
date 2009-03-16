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

    class TestTreeIterator
    {
        static readonly string[] tokens = new string[]
            {
                "<invalid>", "<EOR>", "<DOWN>", "<UP>", "A", "B", "C", "D", "E", "F", "G"
            };

        [TestMethod]
        public void TestNode()
        {
            ITreeAdaptor adaptor = new CommonTreeAdaptor();
            TreeWizard wiz = new TreeWizard( adaptor, tokens );
            CommonTree t = (CommonTree)wiz.Create( "A" );
            TreeIterator it = new TreeIterator( t );
            StringBuilder buf = new StringBuilder();
            bool first = true;
            while ( it.MoveNext() )
            {
                CommonTree n = (CommonTree)it.Current;

                if ( !first )
                    buf.Append( " " );

                first = false;
                buf.Append( n );
            }
            string expecting = "A EOF";
            string found = buf.ToString();
            Assert.AreEqual( expecting, found );
        }

        [TestMethod]
        public void TestFlatAB()
        {
            ITreeAdaptor adaptor = new CommonTreeAdaptor();
            TreeWizard wiz = new TreeWizard( adaptor, tokens );
            CommonTree t = (CommonTree)wiz.Create( "(nil A B)" );
            TreeIterator it = new TreeIterator( t );
            StringBuilder buf = new StringBuilder();
            bool first = true;
            while ( it.MoveNext() )
            {
                CommonTree n = (CommonTree)it.Current;

                if ( !first )
                    buf.Append( " " );

                first = false;
                buf.Append( n );
            }
            string expecting = "nil DOWN A B UP EOF";
            string found = buf.ToString();
            Assert.AreEqual( expecting, found );
        }

        [TestMethod]
        public void TestAB()
        {
            ITreeAdaptor adaptor = new CommonTreeAdaptor();
            TreeWizard wiz = new TreeWizard( adaptor, tokens );
            CommonTree t = (CommonTree)wiz.Create( "(A B)" );
            TreeIterator it = new TreeIterator( t );
            StringBuilder buf = new StringBuilder();
            bool first = true;
            while ( it.MoveNext() )
            {
                CommonTree n = (CommonTree)it.Current;

                if ( !first )
                    buf.Append( " " );

                first = false;
                buf.Append( n );
            }
            string expecting = "A DOWN B UP EOF";
            string found = buf.ToString();
            Assert.AreEqual( expecting, found );
        }

        [TestMethod]
        public void TestABC()
        {
            ITreeAdaptor adaptor = new CommonTreeAdaptor();
            TreeWizard wiz = new TreeWizard( adaptor, tokens );
            CommonTree t = (CommonTree)wiz.Create( "(A B C)" );
            TreeIterator it = new TreeIterator( t );
            StringBuilder buf = new StringBuilder();
            bool first = true;
            while ( it.MoveNext() )
            {
                CommonTree n = (CommonTree)it.Current;

                if ( !first )
                    buf.Append( " " );

                first = false;
                buf.Append( n );
            }
            string expecting = "A DOWN B C UP EOF";
            string found = buf.ToString();
            Assert.AreEqual( expecting, found );
        }

        [TestMethod]
        public void TestVerticalList()
        {
            ITreeAdaptor adaptor = new CommonTreeAdaptor();
            TreeWizard wiz = new TreeWizard( adaptor, tokens );
            CommonTree t = (CommonTree)wiz.Create( "(A (B C))" );
            TreeIterator it = new TreeIterator( t );
            StringBuilder buf = new StringBuilder();
            bool first = true;
            while ( it.MoveNext() )
            {
                CommonTree n = (CommonTree)it.Current;

                if ( !first )
                    buf.Append( " " );

                first = false;
                buf.Append( n );
            }
            string expecting = "A DOWN B DOWN C UP UP EOF";
            string found = buf.ToString();
            Assert.AreEqual( expecting, found );
        }

        [TestMethod]
        public void TestComplex()
        {
            ITreeAdaptor adaptor = new CommonTreeAdaptor();
            TreeWizard wiz = new TreeWizard( adaptor, tokens );
            CommonTree t = (CommonTree)wiz.Create( "(A (B (C D E) F) G)" );
            TreeIterator it = new TreeIterator( t );
            StringBuilder buf = new StringBuilder();
            bool first = true;
            while ( it.MoveNext() )
            {
                CommonTree n = (CommonTree)it.Current;

                if ( !first )
                    buf.Append( " " );

                first = false;
                buf.Append( n );
            }
            string expecting = "A DOWN B DOWN C DOWN D E UP F UP G UP EOF";
            string found = buf.ToString();
            Assert.AreEqual( expecting, found );
        }
    }
}
