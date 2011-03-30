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
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using ANTLRStringStream = Antlr.Runtime.ANTLRStringStream;
    using DFA = Antlr3.Analysis.DFA;
    using Grammar = Antlr3.Tool.Grammar;
    using NFA = Antlr3.Analysis.NFA;

    [TestClass]
    public class TestDFAMatching : BaseTest
    {

        /** Public default constructor used by TestRig */
        public TestDFAMatching()
        {
        }

        [TestMethod]
        public void TestSimpleAltCharTest() /*throws Exception*/ {
            Grammar g = new Grammar(
                    "lexer grammar t;\n" +
                    "A : {;}'a' | 'b' | 'c';" );
            g.BuildNFA();
            g.CreateLookaheadDFAs( false );
            DFA dfa = g.GetLookaheadDFA( 1 );
            checkPrediction( dfa, "a", 1 );
            checkPrediction( dfa, "b", 2 );
            checkPrediction( dfa, "c", 3 );
            checkPrediction( dfa, "d", NFA.INVALID_ALT_NUMBER );
        }

        [TestMethod]
        public void TestSets() /*throws Exception*/ {
            Grammar g = new Grammar(
                    "lexer grammar t;\n" +
                    "A : {;}'a'..'z' | ';' | '0'..'9' ;" );
            g.BuildNFA();
            g.CreateLookaheadDFAs( false );
            DFA dfa = g.GetLookaheadDFA( 1 );
            checkPrediction( dfa, "a", 1 );
            checkPrediction( dfa, "q", 1 );
            checkPrediction( dfa, "z", 1 );
            checkPrediction( dfa, ";", 2 );
            checkPrediction( dfa, "9", 3 );
        }

        [TestMethod]
        public void TestFiniteCommonLeftPrefixes() /*throws Exception*/ {
            Grammar g = new Grammar(
                    "lexer grammar t;\n" +
                    "A : 'a' 'b' | 'a' 'c' | 'd' 'e' ;" );
            g.BuildNFA();
            g.CreateLookaheadDFAs( false );
            DFA dfa = g.GetLookaheadDFA( 1 );
            checkPrediction( dfa, "ab", 1 );
            checkPrediction( dfa, "ac", 2 );
            checkPrediction( dfa, "de", 3 );
            checkPrediction( dfa, "q", NFA.INVALID_ALT_NUMBER );
        }

        [TestMethod]
        public void TestSimpleLoops() /*throws Exception*/ {
            Grammar g = new Grammar(
                    "lexer grammar t;\n" +
                    "A : (DIGIT)+ '.' DIGIT | (DIGIT)+ ;\n" +
                    "fragment DIGIT : '0'..'9' ;\n" );
            g.BuildNFA();
            g.CreateLookaheadDFAs( false );
            DFA dfa = g.GetLookaheadDFA( 3 );
            checkPrediction( dfa, "32", 2 );
            checkPrediction( dfa, "999.2", 1 );
            checkPrediction( dfa, ".2", NFA.INVALID_ALT_NUMBER );
        }

        protected void checkPrediction( DFA dfa, string input, int expected )
        //throws Exception
        {
            ANTLRStringStream stream = new ANTLRStringStream( input );
            Assert.AreEqual( dfa.Predict( stream ), expected );
        }

    }
}
