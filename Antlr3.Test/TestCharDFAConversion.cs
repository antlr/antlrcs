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
    using Antlr.Runtime.JavaExtensions;
    using Antlr3.Tool;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using CodeGenerator = Antlr3.Codegen.CodeGenerator;
    using Console = System.Console;
    using DFA = Antlr3.Analysis.DFA;
    using DFAOptimizer = Antlr3.Analysis.DFAOptimizer;
    using IList = System.Collections.IList;

    [TestClass]
    public class TestCharDFAConversion : BaseTest
    {

        /** Public default constructor used by TestRig */
        public TestCharDFAConversion()
        {
        }

        // R A N G E S  &  S E T S

        [TestMethod]
        public void TestSimpleRangeVersusChar() /*throws Exception*/ {
            Grammar g = new Grammar(
                "lexer grammar t;\n" +
                "A : 'a'..'z' '@' | 'k' '$' ;" );
            g.createLookaheadDFAs();
            string expecting =
                ".s0-'k'->.s1\n" +
                ".s0-{'a'..'j', 'l'..'z'}->:s2=>1\n" +
                ".s1-'$'->:s3=>2\n" +
                ".s1-'@'->:s2=>1\n";
            checkDecision( g, 1, expecting, null );
        }

        [TestMethod]
        public void TestRangeWithDisjointSet() /*throws Exception*/ {
            Grammar g = new Grammar(
                "lexer grammar t;\n" +
                "A : 'a'..'z' '@'\n" +
                "  | ('k'|'9'|'p') '$'\n" +
                "  ;\n" );
            g.createLookaheadDFAs();
            // must break up a..z into {'a'..'j', 'l'..'o', 'q'..'z'}
            string expecting =
                ".s0-'9'->:s3=>2\n" +
                ".s0-{'a'..'j', 'l'..'o', 'q'..'z'}->:s2=>1\n" +
                ".s0-{'k', 'p'}->.s1\n" +
                ".s1-'$'->:s3=>2\n" +
                ".s1-'@'->:s2=>1\n";
            checkDecision( g, 1, expecting, null );
        }

        [TestMethod]
        public void TestDisjointSetCollidingWithTwoRanges() /*throws Exception*/ {
            Grammar g = new Grammar(
                "lexer grammar t;\n" +
                "A : ('a'..'z'|'0'..'9') '@'\n" +
                "  | ('k'|'9'|'p') '$'\n" +
                "  ;\n" );
            g.createLookaheadDFAs( false );
            // must break up a..z into {'a'..'j', 'l'..'o', 'q'..'z'} and 0..9
            // into 0..8
            string expecting =
                ".s0-{'0'..'8', 'a'..'j', 'l'..'o', 'q'..'z'}->:s2=>1\n" +
                ".s0-{'9', 'k', 'p'}->.s1\n" +
                ".s1-'$'->:s3=>2\n" +
                ".s1-'@'->:s2=>1\n";
            checkDecision( g, 1, expecting, null );
        }

        [TestMethod]
        public void TestDisjointSetCollidingWithTwoRangesCharsFirst() /*throws Exception*/ {
            Grammar g = new Grammar(
                "lexer grammar t;\n" +
                "A : ('k'|'9'|'p') '$'\n" +
                "  | ('a'..'z'|'0'..'9') '@'\n" +
                "  ;\n" );
            // must break up a..z into {'a'..'j', 'l'..'o', 'q'..'z'} and 0..9
            // into 0..8
            string expecting =
                ".s0-{'0'..'8', 'a'..'j', 'l'..'o', 'q'..'z'}->:s3=>2\n" +
                ".s0-{'9', 'k', 'p'}->.s1\n" +
                ".s1-'$'->:s2=>1\n" +
                ".s1-'@'->:s3=>2\n";
            checkDecision( g, 1, expecting, null );
        }

        [TestMethod]
        public void TestDisjointSetCollidingWithTwoRangesAsSeparateAlts() /*throws Exception*/ {
            Grammar g = new Grammar(
                "lexer grammar t;\n" +
                "A : 'a'..'z' '@'\n" +
                "  | 'k' '$'\n" +
                "  | '9' '$'\n" +
                "  | 'p' '$'\n" +
                "  | '0'..'9' '@'\n" +
                "  ;\n" );
            // must break up a..z into {'a'..'j', 'l'..'o', 'q'..'z'} and 0..9
            // into 0..8
            string expecting =
                ".s0-'0'..'8'->:s8=>5\n" +
                ".s0-'9'->.s6\n" +
                ".s0-'k'->.s1\n" +
                ".s0-'p'->.s4\n" +
                ".s0-{'a'..'j', 'l'..'o', 'q'..'z'}->:s2=>1\n" +
                ".s1-'$'->:s3=>2\n" +
                ".s1-'@'->:s2=>1\n" +
                ".s4-'$'->:s5=>4\n" +
                ".s4-'@'->:s2=>1\n" +
                ".s6-'$'->:s7=>3\n" +
                ".s6-'@'->:s8=>5\n";
            checkDecision( g, 1, expecting, null );
        }

        [TestMethod]
        public void TestKeywordVersusID() /*throws Exception*/ {
            Grammar g = new Grammar(
                "lexer grammar t;\n" +
                "IF : 'if' ;\n" + // choose this over ID
                "ID : ('a'..'z')+ ;\n" );
            string expecting =
                ".s0-'a'..'z'->:s2=>1\n" +
                ".s0-<EOT>->:s1=>2\n";
            checkDecision( g, 1, expecting, null );
            expecting =
                ".s0-'i'->.s1\n" +
                ".s0-{'a'..'h', 'j'..'z'}->:s4=>2\n" +
                ".s1-'f'->.s2\n" +
                ".s1-<EOT>->:s4=>2\n" +
                ".s2-'a'..'z'->:s4=>2\n" +
                ".s2-<EOT>->:s3=>1\n";
            checkDecision( g, 2, expecting, null );
        }

        [TestMethod]
        public void TestIdenticalRules() /*throws Exception*/ {
            Grammar g = new Grammar(
                "lexer grammar t;\n" +
                "A : 'a' ;\n" +
                "B : 'a' ;\n" ); // can't reach this
            string expecting =
                ".s0-'a'->.s1\n" +
                ".s1-<EOT>->:s2=>1\n";

            ErrorQueue equeue = new ErrorQueue();
            ErrorManager.setErrorListener( equeue );

            checkDecision( g, 1, expecting, new int[] { 2 } );

            assertEquals( "unexpected number of expected problems",
                        1, equeue.size() );
            Message msg = (Message)equeue.errors[0];
            assertTrue( "warning must be an unreachable alt",
                        msg is GrammarUnreachableAltsMessage );
            GrammarUnreachableAltsMessage u = (GrammarUnreachableAltsMessage)msg;
            assertEquals( "[2]", u.alts.ToElementString() );

        }

        [TestMethod]
        public void TestAdjacentNotCharLoops() /*throws Exception*/ {
            Grammar g = new Grammar(
                "lexer grammar t;\n" +
                "A : (~'r')+ ;\n" +
                "B : (~'s')+ ;\n" );
            string expecting =
                ".s0-'r'->:s3=>2\n" +
                ".s0-'s'->:s2=>1\n" +
                ".s0-{'\\u0000'..'q', 't'..'\\uFFFF'}->.s1\n" +
                ".s1-'r'->:s3=>2\n" +
                ".s1-<EOT>->:s2=>1\n" +
                ".s1-{'\\u0000'..'q', 't'..'\\uFFFF'}->.s1\n";
            checkDecision( g, 3, expecting, null );
        }

        [TestMethod]
        public void TestNonAdjacentNotCharLoops() /*throws Exception*/ {
            Grammar g = new Grammar(
                "lexer grammar t;\n" +
                "A : (~'r')+ ;\n" +
                "B : (~'t')+ ;\n" );
            string expecting =
                ".s0-'r'->:s3=>2\n" +
                ".s0-'t'->:s2=>1\n" +
                ".s0-{'\\u0000'..'q', 's', 'u'..'\\uFFFF'}->.s1\n" +
                ".s1-'r'->:s3=>2\n" +
                ".s1-<EOT>->:s2=>1\n" +
                ".s1-{'\\u0000'..'q', 's', 'u'..'\\uFFFF'}->.s1\n";
            checkDecision( g, 3, expecting, null );
        }

        [TestMethod]
        public void TestLoopsWithOptimizedOutExitBranches() /*throws Exception*/ {
            Grammar g = new Grammar(
                "lexer grammar t;\n" +
                "A : 'x'* ~'x'+ ;\n" );
            string expecting =
                ".s0-'x'->:s1=>1\n" +
                ".s0-{'\\u0000'..'w', 'y'..'\\uFFFF'}->:s2=>2\n";
            checkDecision( g, 1, expecting, null );

            // The optimizer yanks out all exit branches from EBNF blocks
            // This is ok because we've already verified there are no problems
            // with the enter/exit decision
            DFAOptimizer optimizer = new DFAOptimizer( g );
            optimizer.Optimize();
            FASerializer serializer = new FASerializer( g );
            DFA dfa = g.getLookaheadDFA( 1 );
            string result = serializer.serialize( dfa.startState );
            expecting = ".s0-'x'->:s1=>1\n";
            assertEquals( expecting, result );
        }

        // N O N G R E E D Y

        [TestMethod]
        public void TestNonGreedy() /*throws Exception*/ {
            Grammar g = new Grammar(
                "lexer grammar t;\n" +
                "CMT : '/*' ( options {greedy=false;} : . )* '*/' ;" );
            string expecting =
                ".s0-'*'->.s1\n" +
                ".s0-{'\\u0000'..')', '+'..'\\uFFFF'}->:s3=>1\n" +
                ".s1-'/'->:s2=>2\n" +
                ".s1-{'\\u0000'..'.', '0'..'\\uFFFF'}->:s3=>1\n";
            checkDecision( g, 1, expecting, null );
        }

        [TestMethod]
        public void TestNonGreedyWildcardStar() /*throws Exception*/ {
            Grammar g = new Grammar(
                "lexer grammar t;\n" +
                "SLCMT : '//' ( options {greedy=false;} : . )* '\n' ;" );
            string expecting =
                ".s0-'\\n'->:s1=>2\n" +
                ".s0-{'\\u0000'..'\\t', '\\u000B'..'\\uFFFF'}->:s2=>1\n";
            checkDecision( g, 1, expecting, null );
        }

        [TestMethod]
        public void TestNonGreedyByDefaultWildcardStar() /*throws Exception*/ {
            Grammar g = new Grammar(
                "lexer grammar t;\n" +
                "SLCMT : '//' .* '\n' ;" );
            string expecting =
                ".s0-'\\n'->:s1=>2\n" +
                ".s0-{'\\u0000'..'\\t', '\\u000B'..'\\uFFFF'}->:s2=>1\n";
            checkDecision( g, 1, expecting, null );
        }

        [TestMethod]
        public void TestNonGreedyWildcardPlus() /*throws Exception*/ {
            // same DFA as nongreedy .* but code gen checks number of
            // iterations at runtime
            Grammar g = new Grammar(
                "lexer grammar t;\n" +
                "SLCMT : '//' ( options {greedy=false;} : . )+ '\n' ;" );
            string expecting =
                ".s0-'\\n'->:s1=>2\n" +
                ".s0-{'\\u0000'..'\\t', '\\u000B'..'\\uFFFF'}->:s2=>1\n";
            checkDecision( g, 1, expecting, null );
        }

        [TestMethod]
        public void TestNonGreedyByDefaultWildcardPlus() /*throws Exception*/ {
            Grammar g = new Grammar(
                "lexer grammar t;\n" +
                "SLCMT : '//' .+ '\n' ;" );
            string expecting =
                ".s0-'\\n'->:s1=>2\n" +
                ".s0-{'\\u0000'..'\\t', '\\u000B'..'\\uFFFF'}->:s2=>1\n";
            checkDecision( g, 1, expecting, null );
        }

        [TestMethod]
        public void TestNonGreedyByDefaultWildcardPlusWithParens() /*throws Exception*/ {
            Grammar g = new Grammar(
                "lexer grammar t;\n" +
                "SLCMT : '//' (.)+ '\n' ;" );
            string expecting =
                ".s0-'\\n'->:s1=>2\n" +
                ".s0-{'\\u0000'..'\\t', '\\u000B'..'\\uFFFF'}->:s2=>1\n";
            checkDecision( g, 1, expecting, null );
        }

        [TestMethod]
        public void TestNonWildcardNonGreedy() /*throws Exception*/ {
            Grammar g = new Grammar(
                "lexer grammar t;\n" +
                "DUH : (options {greedy=false;}:'x'|'y')* 'xy' ;" );
            string expecting =
                ".s0-'x'->.s1\n" +
                ".s0-'y'->:s4=>2\n" +
                ".s1-'x'->:s3=>1\n" +
                ".s1-'y'->:s2=>3\n";
            checkDecision( g, 1, expecting, null );
        }

        [TestMethod]
        public void TestNonWildcardEOTMakesItWorkWithoutNonGreedyOption() /*throws Exception*/ {
            Grammar g = new Grammar(
                "lexer grammar t;\n" +
                "DUH : ('x'|'y')* 'xy' ;" );
            string expecting =
                ".s0-'x'->.s1\n" +
                ".s0-'y'->:s4=>1\n" +
                ".s1-'x'->:s4=>1\n" +
                ".s1-'y'->.s2\n" +
                ".s2-'x'..'y'->:s4=>1\n" +
                ".s2-<EOT>->:s3=>2\n";
            checkDecision( g, 1, expecting, null );
        }

        [TestMethod]
        public void TestAltConflictsWithLoopThenExit() /*throws Exception*/ {
            // \" predicts alt 1, but wildcard then " can predict exit also
            Grammar g = new Grammar(
                "lexer grammar t;\n" +
                "STRING : '\"' (options {greedy=false;}: '\\\\\"' | .)* '\"' ;\n"
            );
            string expecting =
                ".s0-'\"'->:s1=>3\n" +
                    ".s0-'\\\\'->.s2\n" +
                    ".s0-{'\\u0000'..'!', '#'..'[', ']'..'\\uFFFF'}->:s4=>2\n" +
                    ".s2-'\"'->:s3=>1\n" +
                    ".s2-{'\\u0000'..'!', '#'..'\\uFFFF'}->:s4=>2\n";
            checkDecision( g, 1, expecting, null );
        }

        [TestMethod]
        public void TestNonGreedyLoopThatNeverLoops() /*throws Exception*/ {
            Grammar g = new Grammar(
                "lexer grammar t;\n" +
                "DUH : (options {greedy=false;}:'x')+ ;" ); // loop never matched
            string expecting =
                ":s0=>2\n";

            ErrorQueue equeue = new ErrorQueue();
            ErrorManager.setErrorListener( equeue );

            checkDecision( g, 1, expecting, new int[] { 1 } );

            assertEquals( "unexpected number of expected problems",
                        1, equeue.size() );
            Message msg = (Message)equeue.errors[0];
            assertTrue( "warning must be an unreachable alt",
                       msg is GrammarUnreachableAltsMessage );
            GrammarUnreachableAltsMessage u = (GrammarUnreachableAltsMessage)msg;
            assertEquals( "[1]", u.alts.ToElementString() );
        }

        [TestMethod]
        public void TestRecursive() /*throws Exception*/ {
            // this is cool because the 3rd alt includes !(all other possibilities)
            Grammar g = new Grammar(
                "lexer grammar duh;\n" +
                "SUBTEMPLATE\n" +
                "        :       '{'\n" +
                "                ( SUBTEMPLATE\n" +
                "                | ESC\n" +
                "                | ~('}'|'\\\\'|'{')\n" +
                "                )*\n" +
                "                '}'\n" +
                "        ;\n" +
                "fragment\n" +
                "ESC     :       '\\\\' . ;" );
            g.createLookaheadDFAs();
            string expecting =
                ".s0-'\\\\'->:s2=>2\n" +
                ".s0-'{'->:s1=>1\n" +
                ".s0-'}'->:s4=>4\n" +
                ".s0-{'\\u0000'..'[', ']'..'z', '|', '~'..'\\uFFFF'}->:s3=>3\n";
            checkDecision( g, 1, expecting, null );
        }

        [TestMethod]
        public void TestRecursive2() /*throws Exception*/ {
            // this is also cool because it resolves \\ to be ESC alt; it's just
            // less efficient of a DFA
            Grammar g = new Grammar(
                "lexer grammar duh;\n" +
                "SUBTEMPLATE\n" +
                "        :       '{'\n" +
                "                ( SUBTEMPLATE\n" +
                "                | ESC\n" +
                "                | ~('}'|'{')\n" +
                "                )*\n" +
                "                '}'\n" +
                "        ;\n" +
                "fragment\n" +
                "ESC     :       '\\\\' . ;" );
            g.createLookaheadDFAs();
            string expecting =
                ".s0-'\\\\'->.s3\n" +
                ".s0-'{'->:s2=>1\n" +
                ".s0-'}'->:s1=>4\n" +
                ".s0-{'\\u0000'..'[', ']'..'z', '|', '~'..'\\uFFFF'}->:s5=>3\n" +
                ".s3-'\\\\'->:s8=>2\n" +
                ".s3-'{'->:s7=>2\n" +
                ".s3-'}'->.s4\n" +
                ".s3-{'\\u0000'..'[', ']'..'z', '|', '~'..'\\uFFFF'}->:s6=>2\n" +
                ".s4-'\\u0000'..'\\uFFFF'->:s6=>2\n" +
                ".s4-<EOT>->:s5=>3\n";
            checkDecision( g, 1, expecting, null );
        }

        [TestMethod]
        public void TestNotFragmentInLexer() /*throws Exception*/ {
            Grammar g = new Grammar(
                "lexer grammar T;\n" +
                "A : 'a' | ~B {;} ;\n" +
                "fragment B : 'a' ;\n" );
            g.createLookaheadDFAs();
            string expecting =
                ".s0-'a'->:s1=>1\n" +
                ".s0-{'\\u0000'..'`', 'b'..'\\uFFFF'}->:s2=>2\n";
            checkDecision( g, 1, expecting, null );
        }

        [TestMethod]
        public void TestNotSetFragmentInLexer() /*throws Exception*/ {
            Grammar g = new Grammar(
                "lexer grammar T;\n" +
                "A : B | ~B {;} ;\n" +
                "fragment B : 'a'|'b' ;\n" );
            g.createLookaheadDFAs();
            string expecting =
                ".s0-'a'..'b'->:s1=>1\n" +
                ".s0-{'\\u0000'..'`', 'c'..'\\uFFFF'}->:s2=>2\n";
            checkDecision( g, 1, expecting, null );
        }

        [TestMethod]
        public void TestNotTokenInLexer() /*throws Exception*/ {
            Grammar g = new Grammar(
                "lexer grammar T;\n" +
                "A : 'x' ('a' | ~B {;}) ;\n" +
                "B : 'a' ;\n" );
            g.createLookaheadDFAs();
            string expecting =
                ".s0-'a'->:s1=>1\n" +
                ".s0-{'\\u0000'..'`', 'b'..'\\uFFFF'}->:s2=>2\n";
            checkDecision( g, 1, expecting, null );
        }

        [TestMethod]
        public void TestNotComplicatedSetRuleInLexer() /*throws Exception*/ {
            Grammar g = new Grammar(
                "lexer grammar T;\n" +
                "A : B | ~B {;} ;\n" +
                "fragment B : 'a'|'b'|'c'..'e'|C ;\n" +
                "fragment C : 'f' ;\n" ); // has to seen from B to C
            string expecting =
                ".s0-'a'..'f'->:s1=>1\n" +
                ".s0-{'\\u0000'..'`', 'g'..'\\uFFFF'}->:s2=>2\n";
            checkDecision( g, 1, expecting, null );
        }

        [TestMethod]
        public void TestNotSetWithRuleInLexer() /*throws Exception*/ {
            Grammar g = new Grammar(
                "lexer grammar T;\n" +
                "T : ~('a' | B) | 'a';\n" +
                "fragment\n" +
                "B : 'b' ;\n" +
                "C : ~'x'{;} ;" ); // force Tokens to not collapse T|C
            string expecting =
                ".s0-'b'->:s3=>2\n" +
                ".s0-'x'->:s2=>1\n" +
                ".s0-{'\\u0000'..'a', 'c'..'w', 'y'..'\\uFFFF'}->.s1\n" +
                ".s1-<EOT>->:s2=>1\n";
            checkDecision( g, 1, expecting, null );
        }

        [TestMethod]
        public void TestSetCallsRuleWithNot() /*throws Exception*/ {
            Grammar g = new Grammar(
                "lexer grammar A;\n" +
                "T : ~'x' ;\n" +
                "S : 'x' (T | 'x') ;\n" );
            string expecting =
                ".s0-'x'->:s2=>2\n" +
                ".s0-{'\\u0000'..'w', 'y'..'\\uFFFF'}->:s1=>1\n";
            checkDecision( g, 1, expecting, null );
        }

        [TestMethod]
        public void TestSynPredInLexer() /*throws Exception*/ {
            Grammar g = new Grammar(
                "lexer grammar T;\n" +
                "LT:  '<' ' '*\n" +
                "  |  ('<' IDENT) => '<' IDENT '>'\n" + // this was causing syntax error
                "  ;\n" +
                "IDENT:    'a'+;\n" );
            // basically, Tokens rule should not do set compression test
            string expecting =
                ".s0-'<'->:s1=>1\n" +
                ".s0-'a'->:s2=>2\n";
            checkDecision( g, 4, expecting, null ); // 4 is Tokens rule
        }

        // S U P P O R T

        public void _template() /*throws Exception*/ {
            Grammar g = new Grammar(
                "grammar T;\n" +
                "a : A | B;" );
            string expecting =
                "\n";
            checkDecision( g, 1, expecting, null );
        }

        protected void checkDecision( Grammar g,
                                     int decision,
                                     string expecting,
                                     int[] expectingUnreachableAlts )
        //throws Exception
        {

            // mimic actions of org.antlr.Tool first time for grammar g
            if ( g.CodeGenerator == null )
            {
                CodeGenerator generator = new CodeGenerator( null, g, "Java" );
                g.setCodeGenerator( generator );
                g.buildNFA();
                g.createLookaheadDFAs( false );
            }

            DFA dfa = g.getLookaheadDFA( decision );
            assertNotNull( "unknown decision #" + decision, dfa );
            FASerializer serializer = new FASerializer( g );
            string result = serializer.serialize( dfa.startState );
            //System.out.print(result);
            var nonDetAlts = dfa.UnreachableAlts;
            //System.out.println("alts w/o predict state="+nonDetAlts);

            // first make sure nondeterministic alts are as expected
            if ( expectingUnreachableAlts == null )
            {
                if ( nonDetAlts != null && nonDetAlts.Count != 0 )
                {
                    Console.Error.WriteLine( "nondeterministic alts (should be empty): " + ( (IList)nonDetAlts ).ToElementString() );
                }
                assertEquals( "unreachable alts mismatch", 0, nonDetAlts != null ? nonDetAlts.Count : 0 );
            }
            else
            {
                for ( int i = 0; i < expectingUnreachableAlts.Length; i++ )
                {
                    assertTrue( "unreachable alts mismatch",
                               nonDetAlts != null ? nonDetAlts.Contains( expectingUnreachableAlts[i] ) : false );
                }
            }
            assertEquals( expecting, result );
        }

    }
}
