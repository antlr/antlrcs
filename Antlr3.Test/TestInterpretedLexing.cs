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
    using Antlr.Runtime;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Grammar = Antlr3.Tool.Grammar;
    using Interpreter = Antlr3.Tool.Interpreter;

    [TestClass]
    public class TestInterpretedLexing : BaseTest
    {

        /*
        static class Tracer implements ANTLRDebugInterface {
            Grammar g;
            public DebugActions(Grammar g) {
                this.g = g;
            }
            public void enterRule(String ruleName) {
                System.out.println("enterRule("+ruleName+")");
            }

            public void exitRule(String ruleName) {
                System.out.println("exitRule("+ruleName+")");
            }

            public void matchElement(int type) {
                System.out.println("matchElement("+g.getTokenName(type)+")");
            }

            public void mismatchedElement(MismatchedTokenException e) {
                System.out.println(e);
                e.printStackTrace(System.out);
            }

            public void mismatchedSet(MismatchedSetException e) {
                System.out.println(e);
                e.printStackTrace(System.out);
            }

            public void noViableAlt(NoViableAltException e) {
                System.out.println(e);
                e.printStackTrace(System.out);
            }
        }
        */

        /** Public default constructor used by TestRig */
        public TestInterpretedLexing()
        {
        }

        [TestMethod]
        public void TestSimpleAltCharTest() /*throws Exception*/ {
            Grammar g = new Grammar(
                    "lexer grammar t;\n" +
                    "A : 'a' | 'b' | 'c';" );
            int Atype = g.GetTokenType( "A" );
            Interpreter engine = new Interpreter( g, new ANTLRStringStream( "a" ) );
            engine = new Interpreter( g, new ANTLRStringStream( "b" ) );
            IToken result = engine.Scan( "A" );
            assertEquals( result.Type, Atype );
            engine = new Interpreter( g, new ANTLRStringStream( "c" ) );
            result = engine.Scan( "A" );
            assertEquals( result.Type, Atype );
        }

        [TestMethod]
        public void TestSingleRuleRef() /*throws Exception*/ {
            Grammar g = new Grammar(
                    "lexer grammar t;\n" +
                    "A : 'a' B 'c' ;\n" +
                    "B : 'b' ;\n" );
            int Atype = g.GetTokenType( "A" );
            Interpreter engine = new Interpreter( g, new ANTLRStringStream( "abc" ) ); // should ignore the x
            IToken result = engine.Scan( "A" );
            assertEquals( result.Type, Atype );
        }

        [TestMethod]
        public void TestSimpleLoop() /*throws Exception*/ {
            Grammar g = new Grammar(
                    "lexer grammar t;\n" +
                    "INT : (DIGIT)+ ;\n" +
                    "fragment DIGIT : '0'..'9';\n" );
            int INTtype = g.GetTokenType( "INT" );
            Interpreter engine = new Interpreter( g, new ANTLRStringStream( "12x" ) ); // should ignore the x
            IToken result = engine.Scan( "INT" );
            assertEquals( result.Type, INTtype );
            engine = new Interpreter( g, new ANTLRStringStream( "1234" ) );
            result = engine.Scan( "INT" );
            assertEquals( result.Type, INTtype );
        }

        [TestMethod]
        public void TestMultAltLoop() /*throws Exception*/ {
            Grammar g = new Grammar(
                    "lexer grammar t;\n" +
                    "A : ('0'..'9'|'a'|'b')+ ;\n" );
            int Atype = g.GetTokenType( "A" );
            Interpreter engine = new Interpreter( g, new ANTLRStringStream( "a" ) );
            IToken result = engine.Scan( "A" );
            engine = new Interpreter( g, new ANTLRStringStream( "a" ) );
            result = engine.Scan( "A" );
            assertEquals( result.Type, Atype );
            engine = new Interpreter( g, new ANTLRStringStream( "1234" ) );
            result = engine.Scan( "A" );
            assertEquals( result.Type, Atype );
            engine = new Interpreter( g, new ANTLRStringStream( "aaa" ) );
            result = engine.Scan( "A" );
            assertEquals( result.Type, Atype );
            engine = new Interpreter( g, new ANTLRStringStream( "aaaa9" ) );
            result = engine.Scan( "A" );
            assertEquals( result.Type, Atype );
            engine = new Interpreter( g, new ANTLRStringStream( "b" ) );
            result = engine.Scan( "A" );
            assertEquals( result.Type, Atype );
            engine = new Interpreter( g, new ANTLRStringStream( "baa" ) );
            result = engine.Scan( "A" );
            assertEquals( result.Type, Atype );
        }

        [TestMethod]
        public void TestSimpleLoops() /*throws Exception*/ {
            Grammar g = new Grammar(
                    "lexer grammar t;\n" +
                    "A : ('0'..'9')+ '.' ('0'..'9')* | ('0'..'9')+ ;\n" );
            int Atype = g.GetTokenType( "A" );
            ICharStream input = new ANTLRStringStream( "1234.5" );
            Interpreter engine = new Interpreter( g, input );
            IToken result = engine.Scan( "A" );
            Assert.AreEqual( Atype, result.Type );
        }

        [TestMethod]
        public void TestTokensRules() /*throws Exception*/ {
            Grammar pg = new Grammar(
                "parser grammar p;\n" +
                "a : (INT|FLOAT|WS)+;\n" );
            Grammar g = new Grammar();
            g.ImportTokenVocabulary( pg );
            g.FileName = "<string>";
            g.SetGrammarContent(
                "lexer grammar t;\n" +
                "INT : (DIGIT)+ ;\n" +
                "FLOAT : (DIGIT)+ '.' (DIGIT)* ;\n" +
                "fragment DIGIT : '0'..'9';\n" +
                "WS : (' ')+ {channel=99;};\n" );
            ICharStream input = new ANTLRStringStream( "123 139.52" );
            Interpreter lexEngine = new Interpreter( g, input );

            CommonTokenStream tokens = new CommonTokenStream( lexEngine );
            tokens.LT(5); // make sure it grabs all tokens
            string result = tokens.ToString();
            //System.out.println(result);
            string expecting = "123 139.52";
            Assert.AreEqual( expecting, result );
        }

    }

}
