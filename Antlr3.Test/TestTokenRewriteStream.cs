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
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using ANTLRStringStream = Antlr.Runtime.ANTLRStringStream;
    using ArgumentException = System.ArgumentException;
    using Grammar = Antlr3.Tool.Grammar;
    using ICharStream = Antlr.Runtime.ICharStream;
    using Interpreter = Antlr3.Tool.Interpreter;
    using TokenRewriteStream = Antlr.Runtime.TokenRewriteStream;

    [TestClass]
    public class TestTokenRewriteStream : BaseTest
    {
        [TestMethod]
        public void TestInsertBeforeIndex0() /*throws Exception*/ {
            Grammar g = new Grammar(
                "lexer grammar t;\n" +
                "A : 'a';\n" +
                "B : 'b';\n" +
                "C : 'c';\n" );
            ICharStream input = new ANTLRStringStream( "abc" );
            Interpreter lexEngine = new Interpreter( g, input );
            TokenRewriteStream tokens = new TokenRewriteStream( lexEngine );
            tokens.InsertBefore( 0, "0" );
            string result = tokens.ToString();
            string expecting = "0abc";
            assertEquals( expecting, result );
        }

        [TestMethod]
        public void TestInsertAfterLastIndex() /*throws Exception*/ {
            Grammar g = new Grammar(
                "lexer grammar t;\n" +
                "A : 'a';\n" +
                "B : 'b';\n" +
                "C : 'c';\n" );
            ICharStream input = new ANTLRStringStream( "abc" );
            Interpreter lexEngine = new Interpreter( g, input );
            TokenRewriteStream tokens = new TokenRewriteStream( lexEngine );
            tokens.InsertAfter( 2, "x" );
            string result = tokens.ToString();
            string expecting = "abcx";
            assertEquals( expecting, result );
        }

        [TestMethod]
        public void Test2InsertBeforeAfterMiddleIndex() /*throws Exception*/ {
            Grammar g = new Grammar(
                "lexer grammar t;\n" +
                "A : 'a';\n" +
                "B : 'b';\n" +
                "C : 'c';\n" );
            ICharStream input = new ANTLRStringStream( "abc" );
            Interpreter lexEngine = new Interpreter( g, input );
            TokenRewriteStream tokens = new TokenRewriteStream( lexEngine );
            tokens.Fill();
            tokens.InsertBefore( 1, "x" );
            tokens.InsertAfter( 1, "x" );
            string result = tokens.ToString();
            string expecting = "axbxc";
            assertEquals( expecting, result );
        }

        [TestMethod]
        public void TestReplaceIndex0() /*throws Exception*/ {
            Grammar g = new Grammar(
                "lexer grammar t;\n" +
                "A : 'a';\n" +
                "B : 'b';\n" +
                "C : 'c';\n" );
            ICharStream input = new ANTLRStringStream( "abc" );
            Interpreter lexEngine = new Interpreter( g, input );
            TokenRewriteStream tokens = new TokenRewriteStream( lexEngine );
            tokens.Fill();
            tokens.Replace( 0, "x" );
            string result = tokens.ToString();
            string expecting = "xbc";
            assertEquals( expecting, result );
        }

        [TestMethod]
        public void TestReplaceLastIndex() /*throws Exception*/ {
            Grammar g = new Grammar(
                "lexer grammar t;\n" +
                "A : 'a';\n" +
                "B : 'b';\n" +
                "C : 'c';\n" );
            ICharStream input = new ANTLRStringStream( "abc" );
            Interpreter lexEngine = new Interpreter( g, input );
            TokenRewriteStream tokens = new TokenRewriteStream( lexEngine );
            tokens.Fill();
            tokens.Replace( 2, "x" );
            string result = tokens.ToString();
            string expecting = "abx";
            assertEquals( expecting, result );
        }

        [TestMethod]
        public void TestReplaceMiddleIndex() /*throws Exception*/ {
            Grammar g = new Grammar(
                "lexer grammar t;\n" +
                "A : 'a';\n" +
                "B : 'b';\n" +
                "C : 'c';\n" );
            ICharStream input = new ANTLRStringStream( "abc" );
            Interpreter lexEngine = new Interpreter( g, input );
            TokenRewriteStream tokens = new TokenRewriteStream( lexEngine );
            tokens.Fill();
            tokens.Replace( 1, "x" );
            string result = tokens.ToString();
            string expecting = "axc";
            assertEquals( expecting, result );
        }

        [TestMethod]
        public void TestToStringStartStop() /*throws Exception*/ {
            Grammar g = new Grammar(
                "lexer grammar t;\n" +
                "ID : 'a'..'z'+;\n" +
                "INT : '0'..'9'+;\n" +
                "SEMI : ';';\n" +
                "MUL : '*';\n" +
                "ASSIGN : '=';\n" +
                "WS : ' '+;\n" );
            // Tokens: 0123456789
            // Input:  x = 3 * 0;
            ICharStream input = new ANTLRStringStream( "x = 3 * 0;" );
            Interpreter lexEngine = new Interpreter( g, input );
            TokenRewriteStream tokens = new TokenRewriteStream( lexEngine );
            tokens.Fill();
            tokens.Replace( 4, 8, "0" ); // replace 3 * 0 with 0

            string result = tokens.ToOriginalString();
            string expecting = "x = 3 * 0;";
            assertEquals( expecting, result );

            result = tokens.ToString();
            expecting = "x = 0;";
            assertEquals( expecting, result );

            result = tokens.ToString( 0, 9 );
            expecting = "x = 0;";
            assertEquals( expecting, result );

            result = tokens.ToString( 4, 8 );
            expecting = "0";
            assertEquals( expecting, result );
        }

        [TestMethod]
        public void TestToStringStartStop2() /*throws Exception*/ {
            Grammar g = new Grammar(
                "lexer grammar t;\n" +
                "ID : 'a'..'z'+;\n" +
                "INT : '0'..'9'+;\n" +
                "SEMI : ';';\n" +
                "ASSIGN : '=';\n" +
                "PLUS : '+';\n" +
                "MULT : '*';\n" +
                "WS : ' '+;\n" );
            // Tokens: 012345678901234567
            // Input:  x = 3 * 0 + 2 * 0;
            ICharStream input = new ANTLRStringStream( "x = 3 * 0 + 2 * 0;" );
            Interpreter lexEngine = new Interpreter( g, input );
            TokenRewriteStream tokens = new TokenRewriteStream( lexEngine );
            tokens.Fill();

            string result = tokens.ToOriginalString();
            string expecting = "x = 3 * 0 + 2 * 0;";
            assertEquals( expecting, result );

            tokens.Replace( 4, 8, "0" ); // replace 3 * 0 with 0
            result = tokens.ToString();
            expecting = "x = 0 + 2 * 0;";
            assertEquals( expecting, result );

            result = tokens.ToString( 0, 17 );
            expecting = "x = 0 + 2 * 0;";
            assertEquals( expecting, result );

            result = tokens.ToString( 4, 8 );
            expecting = "0";
            assertEquals( expecting, result );

            result = tokens.ToString( 0, 8 );
            expecting = "x = 0";
            assertEquals( expecting, result );

            result = tokens.ToString( 12, 16 );
            expecting = "2 * 0";
            assertEquals( expecting, result );

            tokens.InsertAfter( 17, "// comment" );
            result = tokens.ToString( 12, 18 );
            expecting = "2 * 0;// comment";
            assertEquals( expecting, result );

            result = tokens.ToString( 0, 8 ); // try again after insert at end
            expecting = "x = 0";
            assertEquals( expecting, result );
        }


        [TestMethod]
        public void Test2ReplaceMiddleIndex() /*throws Exception*/ {
            Grammar g = new Grammar(
                "lexer grammar t;\n" +
                "A : 'a';\n" +
                "B : 'b';\n" +
                "C : 'c';\n" );
            ICharStream input = new ANTLRStringStream( "abc" );
            Interpreter lexEngine = new Interpreter( g, input );
            TokenRewriteStream tokens = new TokenRewriteStream( lexEngine );
            tokens.Fill();
            tokens.Replace( 1, "x" );
            tokens.Replace( 1, "y" );
            string result = tokens.ToString();
            string expecting = "ayc";
            assertEquals( expecting, result );
        }

        [TestMethod]
        public void Test2ReplaceMiddleIndex1InsertBefore() /*throws Exception*/ {
            Grammar g = new Grammar(
                "lexer grammar t;\n" +
                "A : 'a';\n" +
                "B : 'b';\n" +
                "C : 'c';\n" );
            ICharStream input = new ANTLRStringStream( "abc" );
            Interpreter lexEngine = new Interpreter( g, input );
            TokenRewriteStream tokens = new TokenRewriteStream( lexEngine );
            tokens.Fill();
            tokens.InsertBefore( 0, "_" );
            tokens.Replace( 1, "x" );
            tokens.Replace( 1, "y" );
            string result = tokens.ToString();
            string expecting = "_ayc";
            assertEquals( expecting, result );
        }

        [TestMethod]
        public void TestReplaceThenDeleteMiddleIndex() /*throws Exception*/ {
            Grammar g = new Grammar(
                "lexer grammar t;\n" +
                "A : 'a';\n" +
                "B : 'b';\n" +
                "C : 'c';\n" );
            ICharStream input = new ANTLRStringStream( "abc" );
            Interpreter lexEngine = new Interpreter( g, input );
            TokenRewriteStream tokens = new TokenRewriteStream( lexEngine );
            tokens.Fill();
            tokens.Replace( 1, "x" );
            tokens.Delete( 1 );
            string result = tokens.ToString();
            string expecting = "ac";
            assertEquals( expecting, result );
        }

        [TestMethod]
        public void TestInsertInPriorReplace() /*throws Exception*/ {
            Grammar g = new Grammar(
                "lexer grammar t;\n" +
                "A : 'a';\n" +
                "B : 'b';\n" +
                "C : 'c';\n" );
            ICharStream input = new ANTLRStringStream( "abc" );
            Interpreter lexEngine = new Interpreter( g, input );
            TokenRewriteStream tokens = new TokenRewriteStream( lexEngine );
            tokens.Fill();
            tokens.Replace( 0, 2, "x" );
            tokens.InsertBefore( 1, "0" );
            Exception exc = null;
            try
            {
                tokens.ToString();
            }
            catch ( ArgumentException iae )
            {
                exc = iae;
            }
            string expecting = "insert op <InsertBeforeOp@[@1,1:1='b',<5>,1:1]:\"0\"> within boundaries of previous <ReplaceOp@[@0,0:0='a',<4>,1:0]..[@2,2:2='c',<6>,1:2]:\"x\">";
            assertNotNull( exc );
            assertEquals( expecting, exc.Message );
        }

        [TestMethod]
        public void TestInsertThenReplaceSameIndex() /*throws Exception*/ {
            Grammar g = new Grammar(
                "lexer grammar t;\n" +
                "A : 'a';\n" +
                "B : 'b';\n" +
                "C : 'c';\n" );
            ICharStream input = new ANTLRStringStream( "abc" );
            Interpreter lexEngine = new Interpreter( g, input );
            TokenRewriteStream tokens = new TokenRewriteStream( lexEngine );
            tokens.Fill();
            tokens.InsertBefore( 0, "0" );
            tokens.Replace( 0, "x" ); // supercedes insert at 0
            string result = tokens.ToString();
            string expecting = "0xbc";
            assertEquals( expecting, result );
        }

        [TestMethod]
        public void Test2InsertMiddleIndex() /*throws Exception*/ {
            Grammar g = new Grammar(
                "lexer grammar t;\n" +
                "A : 'a';\n" +
                "B : 'b';\n" +
                "C : 'c';\n" );
            ICharStream input = new ANTLRStringStream( "abc" );
            Interpreter lexEngine = new Interpreter( g, input );
            TokenRewriteStream tokens = new TokenRewriteStream( lexEngine );
            tokens.Fill();
            tokens.InsertBefore( 1, "x" );
            tokens.InsertBefore( 1, "y" );
            string result = tokens.ToString();
            string expecting = "ayxbc";
            assertEquals( expecting, result );
        }

        [TestMethod]
        public void Test2InsertThenReplaceIndex0() /*throws Exception*/ {
            Grammar g = new Grammar(
                "lexer grammar t;\n" +
                "A : 'a';\n" +
                "B : 'b';\n" +
                "C : 'c';\n" );
            ICharStream input = new ANTLRStringStream( "abc" );
            Interpreter lexEngine = new Interpreter( g, input );
            TokenRewriteStream tokens = new TokenRewriteStream( lexEngine );
            tokens.Fill();
            tokens.InsertBefore( 0, "x" );
            tokens.InsertBefore( 0, "y" );
            tokens.Replace( 0, "z" );
            string result = tokens.ToString();
            string expecting = "yxzbc";
            assertEquals( expecting, result );
        }

        [TestMethod]
        public void TestReplaceThenInsertBeforeLastIndex() /*throws Exception*/ {
            Grammar g = new Grammar(
                "lexer grammar t;\n" +
                "A : 'a';\n" +
                "B : 'b';\n" +
                "C : 'c';\n" );
            ICharStream input = new ANTLRStringStream( "abc" );
            Interpreter lexEngine = new Interpreter( g, input );
            TokenRewriteStream tokens = new TokenRewriteStream( lexEngine );
            tokens.Fill();
            tokens.Replace( 2, "x" );
            tokens.InsertBefore( 2, "y" );
            string result = tokens.ToString();
            string expecting = "abyx";
            assertEquals( expecting, result );
        }

        [TestMethod]
        public void TestInsertThenReplaceLastIndex() /*throws Exception*/ {
            Grammar g = new Grammar(
                "lexer grammar t;\n" +
                "A : 'a';\n" +
                "B : 'b';\n" +
                "C : 'c';\n" );
            ICharStream input = new ANTLRStringStream( "abc" );
            Interpreter lexEngine = new Interpreter( g, input );
            TokenRewriteStream tokens = new TokenRewriteStream( lexEngine );
            tokens.Fill();
            tokens.InsertBefore( 2, "y" );
            tokens.Replace( 2, "x" );
            string result = tokens.ToString();
            string expecting = "abyx";
            assertEquals( expecting, result );
        }

        [TestMethod]
        public void TestReplaceThenInsertAfterLastIndex() /*throws Exception*/ {
            Grammar g = new Grammar(
                "lexer grammar t;\n" +
                "A : 'a';\n" +
                "B : 'b';\n" +
                "C : 'c';\n" );
            ICharStream input = new ANTLRStringStream( "abc" );
            Interpreter lexEngine = new Interpreter( g, input );
            TokenRewriteStream tokens = new TokenRewriteStream( lexEngine );
            tokens.Fill();
            tokens.Replace( 2, "x" );
            tokens.InsertAfter( 2, "y" );
            string result = tokens.ToString();
            string expecting = "abxy";
            assertEquals( expecting, result );
        }

        [TestMethod]
        public void TestReplaceRangeThenInsertAtLeftEdge() /*throws Exception*/ {
            Grammar g = new Grammar(
                "lexer grammar t;\n" +
                "A : 'a';\n" +
                "B : 'b';\n" +
                "C : 'c';\n" );
            ICharStream input = new ANTLRStringStream( "abcccba" );
            Interpreter lexEngine = new Interpreter( g, input );
            TokenRewriteStream tokens = new TokenRewriteStream( lexEngine );
            tokens.Fill();
            tokens.Replace( 2, 4, "x" );
            tokens.InsertBefore( 2, "y" );
            string result = tokens.ToString();
            string expecting = "abyxba";
            assertEquals( expecting, result );
        }

        [TestMethod]
        public void TestReplaceRangeThenInsertAtRightEdge() /*throws Exception*/ {
            Grammar g = new Grammar(
                "lexer grammar t;\n" +
                "A : 'a';\n" +
                "B : 'b';\n" +
                "C : 'c';\n" );
            ICharStream input = new ANTLRStringStream( "abcccba" );
            Interpreter lexEngine = new Interpreter( g, input );
            TokenRewriteStream tokens = new TokenRewriteStream( lexEngine );
            tokens.Fill();
            tokens.Replace( 2, 4, "x" );
            tokens.InsertBefore( 4, "y" ); // no effect; within range of a replace
            Exception exc = null;
            try
            {
                tokens.ToString();
            }
            catch ( ArgumentException iae )
            {
                exc = iae;
            }
            string expecting = "insert op <InsertBeforeOp@[@4,4:4='c',<6>,1:4]:\"y\"> within boundaries of previous <ReplaceOp@[@2,2:2='c',<6>,1:2]..[@4,4:4='c',<6>,1:4]:\"x\">";
            assertNotNull( exc );
            assertEquals( expecting, exc.Message );
        }

        [TestMethod]
        public void TestReplaceRangeThenInsertAfterRightEdge() /*throws Exception*/ {
            Grammar g = new Grammar(
                "lexer grammar t;\n" +
                "A : 'a';\n" +
                "B : 'b';\n" +
                "C : 'c';\n" );
            ICharStream input = new ANTLRStringStream( "abcccba" );
            Interpreter lexEngine = new Interpreter( g, input );
            TokenRewriteStream tokens = new TokenRewriteStream( lexEngine );
            tokens.Fill();
            tokens.Replace( 2, 4, "x" );
            tokens.InsertAfter( 4, "y" );
            string result = tokens.ToString();
            string expecting = "abxyba";
            assertEquals( expecting, result );
        }

        [TestMethod]
        public void TestReplaceAll() /*throws Exception*/ {
            Grammar g = new Grammar(
                "lexer grammar t;\n" +
                "A : 'a';\n" +
                "B : 'b';\n" +
                "C : 'c';\n" );
            ICharStream input = new ANTLRStringStream( "abcccba" );
            Interpreter lexEngine = new Interpreter( g, input );
            TokenRewriteStream tokens = new TokenRewriteStream( lexEngine );
            tokens.Fill();
            tokens.Replace( 0, 6, "x" );
            string result = tokens.ToString();
            string expecting = "x";
            assertEquals( expecting, result );
        }

        [TestMethod]
        public void TestReplaceSubsetThenFetch() /*throws Exception*/ {
            Grammar g = new Grammar(
                "lexer grammar t;\n" +
                "A : 'a';\n" +
                "B : 'b';\n" +
                "C : 'c';\n" );
            ICharStream input = new ANTLRStringStream( "abcccba" );
            Interpreter lexEngine = new Interpreter( g, input );
            TokenRewriteStream tokens = new TokenRewriteStream( lexEngine );
            tokens.Fill();
            tokens.Replace( 2, 4, "xyz" );
            string result = tokens.ToString( 0, 6 );
            string expecting = "abxyzba";
            assertEquals( expecting, result );
        }

        [TestMethod]
        public void TestReplaceThenReplaceSuperset() /*throws Exception*/ {
            Grammar g = new Grammar(
                "lexer grammar t;\n" +
                "A : 'a';\n" +
                "B : 'b';\n" +
                "C : 'c';\n" );
            ICharStream input = new ANTLRStringStream( "abcccba" );
            Interpreter lexEngine = new Interpreter( g, input );
            TokenRewriteStream tokens = new TokenRewriteStream( lexEngine );
            tokens.Fill();
            tokens.Replace( 2, 4, "xyz" );
            tokens.Replace( 3, 5, "foo" ); // overlaps, error
            Exception exc = null;
            try
            {
                tokens.ToString();
            }
            catch ( ArgumentException iae )
            {
                exc = iae;
            }
            string expecting = "replace op boundaries of <ReplaceOp@[@3,3:3='c',<6>,1:3]..[@5,5:5='b',<5>,1:5]:\"foo\"> overlap with previous <ReplaceOp@[@2,2:2='c',<6>,1:2]..[@4,4:4='c',<6>,1:4]:\"xyz\">";
            assertNotNull( exc );
            assertEquals( expecting, exc.Message );
        }

        [TestMethod]
        public void TestReplaceThenReplaceLowerIndexedSuperset() /*throws Exception*/ {
            Grammar g = new Grammar(
                "lexer grammar t;\n" +
                "A : 'a';\n" +
                "B : 'b';\n" +
                "C : 'c';\n" );
            ICharStream input = new ANTLRStringStream( "abcccba" );
            Interpreter lexEngine = new Interpreter( g, input );
            TokenRewriteStream tokens = new TokenRewriteStream( lexEngine );
            tokens.Fill();
            tokens.Replace( 2, 4, "xyz" );
            tokens.Replace( 1, 3, "foo" ); // overlap, error
            Exception exc = null;
            try
            {
                tokens.ToString();
            }
            catch ( ArgumentException iae )
            {
                exc = iae;
            }
            string expecting = "replace op boundaries of <ReplaceOp@[@1,1:1='b',<5>,1:1]..[@3,3:3='c',<6>,1:3]:\"foo\"> overlap with previous <ReplaceOp@[@2,2:2='c',<6>,1:2]..[@4,4:4='c',<6>,1:4]:\"xyz\">";
            assertNotNull( exc );
            assertEquals( expecting, exc.Message );
        }

        [TestMethod]
        public void TestReplaceSingleMiddleThenOverlappingSuperset() /*throws Exception*/ {
            Grammar g = new Grammar(
                "lexer grammar t;\n" +
                "A : 'a';\n" +
                "B : 'b';\n" +
                "C : 'c';\n" );
            ICharStream input = new ANTLRStringStream( "abcba" );
            Interpreter lexEngine = new Interpreter( g, input );
            TokenRewriteStream tokens = new TokenRewriteStream( lexEngine );
            tokens.Fill();
            tokens.Replace( 2, 2, "xyz" );
            tokens.Replace( 0, 3, "foo" );
            string result = tokens.ToString();
            string expecting = "fooa";
            assertEquals( expecting, result );
        }

        // June 2, 2008 I rewrote core of rewrite engine; just adding lots more tests here

        [TestMethod]
        public void TestCombineInserts() /*throws Exception*/ {
            Grammar g = new Grammar(
                "lexer grammar t;\n" +
                "A : 'a';\n" +
                "B : 'b';\n" +
                "C : 'c';\n" );
            ICharStream input = new ANTLRStringStream( "abc" );
            Interpreter lexEngine = new Interpreter( g, input );
            TokenRewriteStream tokens = new TokenRewriteStream( lexEngine );
            tokens.Fill();
            tokens.InsertBefore( 0, "x" );
            tokens.InsertBefore( 0, "y" );
            string result = tokens.ToString();
            string expecting = "yxabc";
            assertEquals( expecting, result );
        }

        [TestMethod]
        public void TestCombine3Inserts() /*throws Exception*/ {
            Grammar g = new Grammar(
                "lexer grammar t;\n" +
                "A : 'a';\n" +
                "B : 'b';\n" +
                "C : 'c';\n" );
            ICharStream input = new ANTLRStringStream( "abc" );
            Interpreter lexEngine = new Interpreter( g, input );
            TokenRewriteStream tokens = new TokenRewriteStream( lexEngine );
            tokens.Fill();
            tokens.InsertBefore( 1, "x" );
            tokens.InsertBefore( 0, "y" );
            tokens.InsertBefore( 1, "z" );
            string result = tokens.ToString();
            string expecting = "yazxbc";
            assertEquals( expecting, result );
        }

        [TestMethod]
        public void TestCombineInsertOnLeftWithReplace() /*throws Exception*/ {
            Grammar g = new Grammar(
                "lexer grammar t;\n" +
                "A : 'a';\n" +
                "B : 'b';\n" +
                "C : 'c';\n" );
            ICharStream input = new ANTLRStringStream( "abc" );
            Interpreter lexEngine = new Interpreter( g, input );
            TokenRewriteStream tokens = new TokenRewriteStream( lexEngine );
            tokens.Fill();
            tokens.Replace( 0, 2, "foo" );
            tokens.InsertBefore( 0, "z" ); // combine with left edge of rewrite
            string result = tokens.ToString();
            string expecting = "zfoo";
            assertEquals( expecting, result );
        }

        [TestMethod]
        public void TestCombineInsertOnLeftWithDelete() /*throws Exception*/ {
            Grammar g = new Grammar(
                "lexer grammar t;\n" +
                "A : 'a';\n" +
                "B : 'b';\n" +
                "C : 'c';\n" );
            ICharStream input = new ANTLRStringStream( "abc" );
            Interpreter lexEngine = new Interpreter( g, input );
            TokenRewriteStream tokens = new TokenRewriteStream( lexEngine );
            tokens.Fill();
            tokens.Delete( 0, 2 );
            tokens.InsertBefore( 0, "z" ); // combine with left edge of rewrite
            string result = tokens.ToString();
            string expecting = "z"; // make sure combo is not znull
            assertEquals( expecting, result );
        }

        [TestMethod]
        public void TestDisjointInserts() /*throws Exception*/ {
            Grammar g = new Grammar(
                "lexer grammar t;\n" +
                "A : 'a';\n" +
                "B : 'b';\n" +
                "C : 'c';\n" );
            ICharStream input = new ANTLRStringStream( "abc" );
            Interpreter lexEngine = new Interpreter( g, input );
            TokenRewriteStream tokens = new TokenRewriteStream( lexEngine );
            tokens.Fill();
            tokens.InsertBefore( 1, "x" );
            tokens.InsertBefore( 2, "y" );
            tokens.InsertBefore( 0, "z" );
            string result = tokens.ToString();
            string expecting = "zaxbyc";
            assertEquals( expecting, result );
        }

        [TestMethod]
        public void TestOverlappingReplace() /*throws Exception*/ {
            Grammar g = new Grammar(
                "lexer grammar t;\n" +
                "A : 'a';\n" +
                "B : 'b';\n" +
                "C : 'c';\n" );
            ICharStream input = new ANTLRStringStream( "abcc" );
            Interpreter lexEngine = new Interpreter( g, input );
            TokenRewriteStream tokens = new TokenRewriteStream( lexEngine );
            tokens.Fill();
            tokens.Replace( 1, 2, "foo" );
            tokens.Replace( 0, 3, "bar" ); // wipes prior nested replace
            string result = tokens.ToString();
            string expecting = "bar";
            assertEquals( expecting, result );
        }

        [TestMethod]
        public void TestOverlappingReplace2() /*throws Exception*/ {
            Grammar g = new Grammar(
                "lexer grammar t;\n" +
                "A : 'a';\n" +
                "B : 'b';\n" +
                "C : 'c';\n" );
            ICharStream input = new ANTLRStringStream( "abcc" );
            Interpreter lexEngine = new Interpreter( g, input );
            TokenRewriteStream tokens = new TokenRewriteStream( lexEngine );
            tokens.Fill();
            tokens.Replace( 0, 3, "bar" );
            tokens.Replace( 1, 2, "foo" ); // cannot split earlier replace
            Exception exc = null;
            try
            {
                tokens.ToString();
            }
            catch ( ArgumentException iae )
            {
                exc = iae;
            }
            string expecting = "replace op boundaries of <ReplaceOp@[@1,1:1='b',<5>,1:1]..[@2,2:2='c',<6>,1:2]:\"foo\"> overlap with previous <ReplaceOp@[@0,0:0='a',<4>,1:0]..[@3,3:3='c',<6>,1:3]:\"bar\">";
            assertNotNull( exc );
            assertEquals( expecting, exc.Message );
        }

        [TestMethod]
        public void TestOverlappingReplace3() /*throws Exception*/ {
            Grammar g = new Grammar(
                "lexer grammar t;\n" +
                "A : 'a';\n" +
                "B : 'b';\n" +
                "C : 'c';\n" );
            ICharStream input = new ANTLRStringStream( "abcc" );
            Interpreter lexEngine = new Interpreter( g, input );
            TokenRewriteStream tokens = new TokenRewriteStream( lexEngine );
            tokens.Fill();
            tokens.Replace( 1, 2, "foo" );
            tokens.Replace( 0, 2, "bar" ); // wipes prior nested replace
            string result = tokens.ToString();
            string expecting = "barc";
            assertEquals( expecting, result );
        }

        [TestMethod]
        public void TestOverlappingReplace4() /*throws Exception*/ {
            Grammar g = new Grammar(
                "lexer grammar t;\n" +
                "A : 'a';\n" +
                "B : 'b';\n" +
                "C : 'c';\n" );
            ICharStream input = new ANTLRStringStream( "abcc" );
            Interpreter lexEngine = new Interpreter( g, input );
            TokenRewriteStream tokens = new TokenRewriteStream( lexEngine );
            tokens.Fill();
            tokens.Replace( 1, 2, "foo" );
            tokens.Replace( 1, 3, "bar" ); // wipes prior nested replace
            string result = tokens.ToString();
            string expecting = "abar";
            assertEquals( expecting, result );
        }

        [TestMethod]
        public void TestDropIdenticalReplace() /*throws Exception*/ {
            Grammar g = new Grammar(
                "lexer grammar t;\n" +
                "A : 'a';\n" +
                "B : 'b';\n" +
                "C : 'c';\n" );
            ICharStream input = new ANTLRStringStream( "abcc" );
            Interpreter lexEngine = new Interpreter( g, input );
            TokenRewriteStream tokens = new TokenRewriteStream( lexEngine );
            tokens.Fill();
            tokens.Replace( 1, 2, "foo" );
            tokens.Replace( 1, 2, "foo" ); // drop previous, identical
            string result = tokens.ToString();
            string expecting = "afooc";
            assertEquals( expecting, result );
        }

        [TestMethod]
        public void TestDropPrevCoveredInsert() /*throws Exception*/ {
            Grammar g = new Grammar(
                "lexer grammar t;\n" +
                "A : 'a';\n" +
                "B : 'b';\n" +
                "C : 'c';\n" );
            ICharStream input = new ANTLRStringStream( "abc" );
            Interpreter lexEngine = new Interpreter( g, input );
            TokenRewriteStream tokens = new TokenRewriteStream( lexEngine );
            tokens.Fill();
            tokens.InsertBefore( 1, "foo" );
            tokens.Replace( 1, 2, "foo" ); // kill prev insert
            string result = tokens.ToString();
            string expecting = "afoofoo";
            assertEquals( expecting, result );
        }

        [TestMethod]
        public void TestLeaveAloneDisjointInsert() /*throws Exception*/ {
            Grammar g = new Grammar(
                "lexer grammar t;\n" +
                "A : 'a';\n" +
                "B : 'b';\n" +
                "C : 'c';\n" );
            ICharStream input = new ANTLRStringStream( "abcc" );
            Interpreter lexEngine = new Interpreter( g, input );
            TokenRewriteStream tokens = new TokenRewriteStream( lexEngine );
            tokens.Fill();
            tokens.InsertBefore( 1, "x" );
            tokens.Replace( 2, 3, "foo" );
            string result = tokens.ToString();
            string expecting = "axbfoo";
            assertEquals( expecting, result );
        }

        [TestMethod]
        public void TestLeaveAloneDisjointInsert2() /*throws Exception*/ {
            Grammar g = new Grammar(
                "lexer grammar t;\n" +
                "A : 'a';\n" +
                "B : 'b';\n" +
                "C : 'c';\n" );
            ICharStream input = new ANTLRStringStream( "abcc" );
            Interpreter lexEngine = new Interpreter( g, input );
            TokenRewriteStream tokens = new TokenRewriteStream( lexEngine );
            tokens.Fill();
            tokens.Replace( 2, 3, "foo" );
            tokens.InsertBefore( 1, "x" );
            string result = tokens.ToString();
            string expecting = "axbfoo";
            assertEquals( expecting, result );
        }

        [TestMethod]
        public void TestInsertBeforeTokenThenDeleteThatToken()
        {
            Grammar g = new Grammar(
                "lexer grammar t;\n" +
                "A : 'a';\n" +
                "B : 'b';\n" +
                "C : 'c';\n");
            ICharStream input = new ANTLRStringStream("abc");
            Interpreter lexEngine = new Interpreter(g, input);
            TokenRewriteStream tokens = new TokenRewriteStream(lexEngine);
            tokens.Fill();
            tokens.InsertBefore(2, "y");
            tokens.Delete(2);
            string result = tokens.ToString();
            string expecting = "aby";
            assertEquals(expecting, result);
        }
    }
}
