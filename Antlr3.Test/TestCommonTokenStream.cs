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
    using Antlr3.Tool;
    using Antlr.Runtime;

    /** This actually tests new (12/4/09) buffered but on-demand fetching stream */
    [TestClass]
    public class TestCommonTokenStream : BaseTest
    {
        [TestMethod]
        public void TestFirstToken()
        {
            Grammar g = new Grammar(
                "lexer grammar t;\n" +
                "ID : 'a'..'z'+;\n" +
                "INT : '0'..'9'+;\n" +
                "SEMI : ';';\n" +
                "ASSIGN : '=';\n" +
                "PLUS : '+';\n" +
                "MULT : '*';\n" +
                "WS : ' '+;\n");
            // Tokens: 012345678901234567
            // Input:  x = 3 * 0 + 2 * 0;
            ICharStream input = new ANTLRStringStream("x = 3 * 0 + 2 * 0;");
            Interpreter lexEngine = new Interpreter(g, input);
            BufferedTokenStream tokens = new BufferedTokenStream(lexEngine);

            string result = tokens.LT(1).Text;
            string expecting = "x";
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void Test2ndToken()
        {
            Grammar g = new Grammar(
                "lexer grammar t;\n" +
                "ID : 'a'..'z'+;\n" +
                "INT : '0'..'9'+;\n" +
                "SEMI : ';';\n" +
                "ASSIGN : '=';\n" +
                "PLUS : '+';\n" +
                "MULT : '*';\n" +
                "WS : ' '+;\n");
            // Tokens: 012345678901234567
            // Input:  x = 3 * 0 + 2 * 0;
            ICharStream input = new ANTLRStringStream("x = 3 * 0 + 2 * 0;");
            Interpreter lexEngine = new Interpreter(g, input);
            BufferedTokenStream tokens = new BufferedTokenStream(lexEngine);

            string result = tokens.LT(2).Text;
            string expecting = " ";
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestCompleteBuffer()
        {
            Grammar g = new Grammar(
                "lexer grammar t;\n" +
                "ID : 'a'..'z'+;\n" +
                "INT : '0'..'9'+;\n" +
                "SEMI : ';';\n" +
                "ASSIGN : '=';\n" +
                "PLUS : '+';\n" +
                "MULT : '*';\n" +
                "WS : ' '+;\n");
            // Tokens: 012345678901234567
            // Input:  x = 3 * 0 + 2 * 0;
            ICharStream input = new ANTLRStringStream("x = 3 * 0 + 2 * 0;");
            Interpreter lexEngine = new Interpreter(g, input);
            BufferedTokenStream tokens = new BufferedTokenStream(lexEngine);

            int i = 1;
            IToken t = tokens.LT(i);
            while (t.Type != CharStreamConstants.EndOfFile)
            {
                i++;
                t = tokens.LT(i);
            }
            tokens.LT(i++); // push it past end
            tokens.LT(i++);

            string result = tokens.ToString();
            string expecting = "x = 3 * 0 + 2 * 0;";
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestCompleteBufferAfterConsuming()
        {
            Grammar g = new Grammar(
                "lexer grammar t;\n" +
                "ID : 'a'..'z'+;\n" +
                "INT : '0'..'9'+;\n" +
                "SEMI : ';';\n" +
                "ASSIGN : '=';\n" +
                "PLUS : '+';\n" +
                "MULT : '*';\n" +
                "WS : ' '+;\n");
            // Tokens: 012345678901234567
            // Input:  x = 3 * 0 + 2 * 0;
            ICharStream input = new ANTLRStringStream("x = 3 * 0 + 2 * 0;");
            Interpreter lexEngine = new Interpreter(g, input);
            BufferedTokenStream tokens = new BufferedTokenStream(lexEngine);

            IToken t = tokens.LT(1);
            while (t.Type != CharStreamConstants.EndOfFile)
            {
                tokens.Consume();
                t = tokens.LT(1);
            }
            tokens.Consume();
            tokens.LT(1); // push it past end
            tokens.Consume();
            tokens.LT(1);

            string result = tokens.ToString();
            string expecting = "x = 3 * 0 + 2 * 0;";
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestLookback()
        {
            Grammar g = new Grammar(
                "lexer grammar t;\n" +
                "ID : 'a'..'z'+;\n" +
                "INT : '0'..'9'+;\n" +
                "SEMI : ';';\n" +
                "ASSIGN : '=';\n" +
                "PLUS : '+';\n" +
                "MULT : '*';\n" +
                "WS : ' '+;\n");
            // Tokens: 012345678901234567
            // Input:  x = 3 * 0 + 2 * 0;
            ICharStream input = new ANTLRStringStream("x = 3 * 0 + 2 * 0;");
            Interpreter lexEngine = new Interpreter(g, input);
            BufferedTokenStream tokens = new BufferedTokenStream(lexEngine);

            tokens.Consume(); // get x into buffer
            IToken t = tokens.LT(-1);
            Assert.AreEqual("x", t.Text);

            tokens.Consume();
            tokens.Consume(); // consume '='
            t = tokens.LT(-3);
            Assert.AreEqual("x", t.Text);
            t = tokens.LT(-2);
            Assert.AreEqual(" ", t.Text);
            t = tokens.LT(-1);
            Assert.AreEqual("=", t.Text);
        }

        private class TestOffChannelTokenSource : ITokenSource
        {
            int i = 0;
            IToken[] tokens =
            {
                new CommonToken(1," "),
                new CommonToken(1,"x"),
                new CommonToken(1," "),
                new CommonToken(1,"="),
                new CommonToken(1,"34"),
                new CommonToken(1," "),
                new CommonToken(1," "),
                new CommonToken(1,";"),
                new CommonToken(1,"\n"),
                new CommonToken(CharStreamConstants.EndOfFile,"")
            };

            public TestOffChannelTokenSource()
            {
                tokens[0].Channel = Lexer.Hidden;
                tokens[2].Channel = Lexer.Hidden;
                tokens[5].Channel = Lexer.Hidden;
                tokens[6].Channel = Lexer.Hidden;
                tokens[8].Channel = Lexer.Hidden;
            }

            public string SourceName
            {
                get
                {
                    return "test";
                }
            }

            public string[] TokenNames
            {
                get
                {
                    throw new System.NotImplementedException();
                }
            }

            public IToken NextToken()
            {
                return tokens[i++];
            }
        }

        [TestMethod]
        public void TestOffChannel()
        {
            ITokenSource lexer = // simulate input " x =34  ;\n"
                new TestOffChannelTokenSource();

            CommonTokenStream tokens = new CommonTokenStream(lexer);

            Assert.AreEqual("x", tokens.LT(1).Text); // must skip first off channel token
            tokens.Consume();
            Assert.AreEqual("=", tokens.LT(1).Text);
            Assert.AreEqual("x", tokens.LT(-1).Text);

            tokens.Consume();
            Assert.AreEqual("34", tokens.LT(1).Text);
            Assert.AreEqual("=", tokens.LT(-1).Text);

            tokens.Consume();
            Assert.AreEqual(";", tokens.LT(1).Text);
            Assert.AreEqual("34", tokens.LT(-1).Text);

            tokens.Consume();
            Assert.AreEqual(CharStreamConstants.EndOfFile, tokens.LA(1));
            Assert.AreEqual(";", tokens.LT(-1).Text);

            Assert.AreEqual("34", tokens.LT(-2).Text);
            Assert.AreEqual("=", tokens.LT(-3).Text);
            Assert.AreEqual("x", tokens.LT(-4).Text);
        }
    }
}
