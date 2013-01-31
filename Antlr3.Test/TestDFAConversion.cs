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
    using System.Collections.Generic;
    using System.Linq;
    using Antlr.Runtime.JavaExtensions;
    using Antlr3.Tool;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using AntlrTool = Antlr3.AntlrTool;
    using BitSet = Antlr3.Misc.BitSet;
    using CodeGenerator = Antlr3.Codegen.CodeGenerator;
    using Console = System.Console;
    using DecisionProbe = Antlr3.Analysis.DecisionProbe;
    using DFA = Antlr3.Analysis.DFA;
    using ICollection = System.Collections.ICollection;
    using IList = System.Collections.IList;
    using StringBuilder = System.Text.StringBuilder;

    [TestClass]
    public class TestDFAConversion : BaseTest
    {

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestA() /*throws Exception*/ {
            Grammar g = new Grammar(
                "parser grammar t;\n" +
                "a : A C | B;" );
            string expecting =
                ".s0-A->:s1=>1" + NewLine +
                ".s0-B->:s2=>2" + NewLine;
            checkDecision( g, 1, expecting, null, null, null, null, 0 );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestAB_or_AC() /*throws Exception*/ {
            Grammar g = new Grammar(
                "parser grammar t;\n" +
                "a : A B | A C;" );
            string expecting =
                ".s0-A->.s1" + NewLine +
                ".s1-B->:s2=>1" + NewLine +
                ".s1-C->:s3=>2" + NewLine;
            checkDecision( g, 1, expecting, null, null, null, null, 0 );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestAB_or_AC_k2() /*throws Exception*/ {
            Grammar g = new Grammar(
                "parser grammar t;\n" +
                "options {k=2;}\n" +
                "a : A B | A C;" );
            string expecting =
                ".s0-A->.s1" + NewLine +
                ".s1-B->:s2=>1" + NewLine +
                ".s1-C->:s3=>2" + NewLine;
            checkDecision( g, 1, expecting, null, null, null, null, 0 );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestAB_or_AC_k1() /*throws Exception*/ {
            Grammar g = new Grammar(
                "parser grammar t;\n" +
                "options {k=1;}\n" +
                "a : A B | A C;" );
            string expecting =
                ".s0-A->:s1=>1" + NewLine;
            int[] unreachableAlts = new int[] { 2 };
            int[] nonDetAlts = new int[] { 1, 2 };
            string ambigInput = "A";
            int[] danglingAlts = new int[] { 2 };
            int numWarnings = 2; // ambig upon A
            checkDecision( g, 1, expecting, unreachableAlts,
                          nonDetAlts, ambigInput, danglingAlts, numWarnings );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestselfRecurseNonDet() /*throws Exception*/ {
            Grammar g = new Grammar(
                "parser grammar t;\n" +
                "s : a ;\n" +
                "a : A a X | A a Y;" );
            IList<int> altsWithRecursion = new int[] { 1, 2 };
            assertNonLLStar( g, altsWithRecursion );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestRecursionOverflow() /*throws Exception*/ {
            Grammar g = new Grammar(
                "parser grammar t;\n" +
                "s : a Y | A A A A A X ;\n" + // force recursion past m=4
                "a : A a | Q;" );
            IList expectedTargetRules = new List<object>( new object[] { "a" } );
            int expectedAlt = 1;
            assertRecursionOverflow( g, expectedTargetRules, expectedAlt );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestRecursionOverflow2() /*throws Exception*/ {
            Grammar g = new Grammar(
                "parser grammar t;\n" +
                "s : a Y | A+ X ;\n" + // force recursion past m=4
                "a : A a | Q;" );
            IList expectedTargetRules = new List<object>( new object[] { "a" } );
            int expectedAlt = 1;
            assertRecursionOverflow( g, expectedTargetRules, expectedAlt );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestRecursionOverflowWithPredOk() /*throws Exception*/ {
            // overflows with k=*, but resolves with pred
            // no warnings/errors
            Grammar g = new Grammar(
                "parser grammar t;\n" +
                "s : (a Y)=> a Y | A A A A A X ;\n" + // force recursion past m=4
                "a : A a | Q;" );
            string expecting =
                ".s0-A->.s1" + NewLine +
                ".s0-Q&&{synpred1_t}?->:s11=>1" + NewLine +
                ".s1-A->.s2" + NewLine +
                ".s1-Q&&{synpred1_t}?->:s10=>1" + NewLine +
                ".s2-A->.s3" + NewLine +
                ".s2-Q&&{synpred1_t}?->:s9=>1" + NewLine +
                ".s3-A->.s4" + NewLine +
                ".s3-Q&&{synpred1_t}?->:s8=>1" + NewLine +
                ".s4-A->.s5" + NewLine +
                ".s4-Q&&{synpred1_t}?->:s6=>1" + NewLine +
                ".s5-{synpred1_t}?->:s6=>1" + NewLine +
                ".s5-{true}?->:s7=>2" + NewLine;
            int[] unreachableAlts = null;
            int[] nonDetAlts = null;
            string ambigInput = null;
            int[] danglingAlts = null;
            int numWarnings = 0;
            checkDecision( g, 1, expecting, unreachableAlts,
                          nonDetAlts, ambigInput, danglingAlts, numWarnings );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestRecursionOverflowWithPredOk2() /*throws Exception*/ {
            // must predict Z w/o predicate
            Grammar g = new Grammar(
                "parser grammar t;\n" +
                "s : (a Y)=> a Y | A A A A A X | Z;\n" + // force recursion past m=4
                "a : A a | Q;" );
            string expecting =
                ".s0-A->.s1" + NewLine +
                ".s0-Q&&{synpred1_t}?->:s11=>1" + NewLine +
                ".s0-Z->:s12=>3" + NewLine +
                ".s1-A->.s2" + NewLine +
                ".s1-Q&&{synpred1_t}?->:s10=>1" + NewLine +
                ".s2-A->.s3" + NewLine +
                ".s2-Q&&{synpred1_t}?->:s9=>1" + NewLine +
                ".s3-A->.s4" + NewLine +
                ".s3-Q&&{synpred1_t}?->:s8=>1" + NewLine +
                ".s4-A->.s5" + NewLine +
                ".s4-Q&&{synpred1_t}?->:s6=>1" + NewLine +
                ".s5-{synpred1_t}?->:s6=>1" + NewLine +
                ".s5-{true}?->:s7=>2" + NewLine;
            int[] unreachableAlts = null;
            int[] nonDetAlts = null;
            string ambigInput = null;
            int[] danglingAlts = null;
            int numWarnings = 0;
            checkDecision( g, 1, expecting, unreachableAlts,
                          nonDetAlts, ambigInput, danglingAlts, numWarnings );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestCannotSeePastRecursion() /*throws Exception*/ {
            Grammar g = new Grammar(
                "parser grammar t;\n" +
                "x   : y X\n" +
                "    | y Y\n" +
                "    ;\n" +
                "y   : L y R\n" +
                "    | B\n" +
                "    ;" );
            IList<int> altsWithRecursion = new int[] { 1, 2 };
            assertNonLLStar( g, altsWithRecursion );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestSynPredResolvesRecursion() /*throws Exception*/ {
            Grammar g = new Grammar(
                "parser grammar t;\n" +
                "x   : (y X)=> y X\n" +
                "    | y Y\n" +
                "    ;\n" +
                "y   : L y R\n" +
                "    | B\n" +
                "    ;" );
            string expecting =
                ".s0-B->.s4" + NewLine +
                ".s0-L->.s1" + NewLine +
                ".s1-{synpred1_t}?->:s2=>1" + NewLine +
                ".s1-{true}?->:s3=>2" + NewLine +
                ".s4-{synpred1_t}?->:s2=>1" + NewLine +
                ".s4-{true}?->:s3=>2" + NewLine;
            int[] unreachableAlts = null;
            int[] nonDetAlts = null;
            string ambigInput = null;
            int[] danglingAlts = null;
            int numWarnings = 0;
            checkDecision( g, 1, expecting, unreachableAlts,
                          nonDetAlts, ambigInput, danglingAlts, numWarnings );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestSynPredMissingInMiddle()
        {
            Grammar g = new Grammar(
                "parser grammar t;\n" +
                "x   : (A)=> X\n" +
                "    | X\n" +  // assume missing synpred is true also
                "	 | (C)=> X" +
                "    ;\n");
            string expecting =
                ".s0-X->.s1" + NewLine +
                ".s1-{synpred1_t}?->:s2=>1" + NewLine +
                ".s1-{synpred2_t}?->:s4=>3" + NewLine +
                ".s1-{true}?->:s3=>2" + NewLine;
            int[] unreachableAlts = null;
            int[] nonDetAlts = null;
            string ambigInput = null;
            int[] danglingAlts = null;
            int numWarnings = 0;
            checkDecision(g, 1, expecting, unreachableAlts,
                          nonDetAlts, ambigInput, danglingAlts, numWarnings);
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestAutoBacktrackAndPredMissingInMiddle()
        {
            Grammar g = new Grammar(
                "parser grammar t;\n" +
                "options {backtrack=true;}\n" +
                "x   : (A)=> X\n" +
                "    | X\n" +  // assume missing synpred is true also
                "	 | (C)=> X" +
                "    ;\n");
            string expecting =
                ".s0-X->.s1" + NewLine +
                ".s1-{synpred1_t}?->:s2=>1" + NewLine +  // gen code should have this as (A)=>
                ".s1-{synpred2_t}?->:s3=>2" + NewLine + // gen code should have this as (X)=>
                ".s1-{synpred3_t}?->:s4=>3" + NewLine; // gen code should have this as (C)=>
            int[] unreachableAlts = null;
            int[] nonDetAlts = null;
            string ambigInput = null;
            int[] danglingAlts = null;
            int numWarnings = 0;
            checkDecision(g, 1, expecting, unreachableAlts,
                          nonDetAlts, ambigInput, danglingAlts, numWarnings);
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestSemPredResolvesRecursion() /*throws Exception*/ {
            Grammar g = new Grammar(
                "parser grammar t;\n" +
                "x   : {p}? y X\n" +
                "    | y Y\n" +
                "    ;\n" +
                "y   : L y R\n" +
                "    | B\n" +
                "    ;" );
            string expecting =
                ".s0-B->.s4" + NewLine +
                ".s0-L->.s1" + NewLine +
                ".s1-{p}?->:s2=>1" + NewLine +
                ".s1-{true}?->:s3=>2" + NewLine +
                ".s4-{p}?->:s2=>1" + NewLine +
                ".s4-{true}?->:s3=>2" + NewLine;
            int[] unreachableAlts = null;
            int[] nonDetAlts = null;
            string ambigInput = null;
            int[] danglingAlts = null;
            int numWarnings = 0;
            checkDecision( g, 1, expecting, unreachableAlts,
                          nonDetAlts, ambigInput, danglingAlts, numWarnings );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestSemPredResolvesRecursion2() /*throws Exception*/ {
            Grammar g = new Grammar(
                "parser grammar t;\n" +
                "x\n" +
                "options {k=1;}\n" +
                "   : {p}? y X\n" +
                "    | y Y\n" +
                "    ;\n" +
                "y   : L y R\n" +
                "    | B\n" +
                "    ;" );
            string expecting =
                ".s0-B->.s4" + NewLine +
                ".s0-L->.s1" + NewLine +
                ".s1-{p}?->:s2=>1" + NewLine +
                ".s1-{true}?->:s3=>2" + NewLine +
                ".s4-{p}?->:s2=>1" + NewLine +
                ".s4-{true}?->:s3=>2" + NewLine;
            int[] unreachableAlts = null;
            int[] nonDetAlts = null;
            string ambigInput = null;
            int[] danglingAlts = null;
            int numWarnings = 0;
            checkDecision( g, 1, expecting, unreachableAlts,
                          nonDetAlts, ambigInput, danglingAlts, numWarnings );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestSemPredResolvesRecursion3() /*throws Exception*/ {
            Grammar g = new Grammar(
                "parser grammar t;\n" +
                "x\n" +
                "options {k=2;}\n" + // just makes bigger DFA
                "   : {p}? y X\n" +
                "    | y Y\n" +
                "    ;\n" +
                "y   : L y R\n" +
                "    | B\n" +
                "    ;" );
            string expecting =
                ".s0-B->.s6" + NewLine +
                ".s0-L->.s1" + NewLine +
                ".s1-B->.s5" + NewLine +
                ".s1-L->.s2" + NewLine +
                ".s2-{p}?->:s3=>1" + NewLine +
                ".s2-{true}?->:s4=>2" + NewLine +
                ".s5-{p}?->:s3=>1" + NewLine +
                ".s5-{true}?->:s4=>2" + NewLine +
                ".s6-X->:s3=>1" + NewLine +
                ".s6-Y->:s4=>2" + NewLine;
            int[] unreachableAlts = null;
            int[] nonDetAlts = null;
            string ambigInput = null;
            int[] danglingAlts = null;
            int numWarnings = 0;
            checkDecision( g, 1, expecting, unreachableAlts,
                          nonDetAlts, ambigInput, danglingAlts, numWarnings );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestSynPredResolvesRecursion2() /*throws Exception*/ {
            // k=* fails and it retries/succeeds with k=1 silently
            // because of predicate
            Grammar g = new Grammar(
                "parser grammar t;\n" +
                "statement\n" +
                "    :     (reference ASSIGN)=> reference ASSIGN expr\n" +
                "    |     expr\n" +
                "    ;\n" +
                "expr:     reference\n" +
                "    |     INT\n" +
                "    |     FLOAT\n" +
                "    ;\n" +
                "reference\n" +
                "    :     ID L argument_list R\n" +
                "    ;\n" +
                "argument_list\n" +
                "    :     expr COMMA expr\n" +
                "    ;" );
            string expecting =
                ".s0-ID->.s1" + NewLine +
                ".s0-{FLOAT, INT}->:s3=>2" + NewLine +
                ".s1-{synpred1_t}?->:s2=>1" + NewLine +
                ".s1-{true}?->:s3=>2" + NewLine;
            int[] unreachableAlts = null;
            int[] nonDetAlts = null;
            string ambigInput = null;
            int[] danglingAlts = null;
            int numWarnings = 0;
            checkDecision( g, 1, expecting, unreachableAlts,
                          nonDetAlts, ambigInput, danglingAlts, numWarnings );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestSynPredResolvesRecursion3() /*throws Exception*/ {
            // No errors with k=1; don't try k=* first
            Grammar g = new Grammar(
                "parser grammar t;\n" +
                "statement\n" +
                "options {k=1;}\n" +
                "    :     (reference ASSIGN)=> reference ASSIGN expr\n" +
                "    |     expr\n" +
                "    ;\n" +
                "expr:     reference\n" +
                "    |     INT\n" +
                "    |     FLOAT\n" +
                "    ;\n" +
                "reference\n" +
                "    :     ID L argument_list R\n" +
                "    ;\n" +
                "argument_list\n" +
                "    :     expr COMMA expr\n" +
                "    ;" );
            string expecting =
                ".s0-ID->.s1" + NewLine +
                ".s0-{FLOAT, INT}->:s3=>2" + NewLine +
                ".s1-{synpred1_t}?->:s2=>1" + NewLine +
                ".s1-{true}?->:s3=>2" + NewLine;
            int[] unreachableAlts = null;
            int[] nonDetAlts = null;
            string ambigInput = null;
            int[] danglingAlts = null;
            int numWarnings = 0;
            checkDecision( g, 1, expecting, unreachableAlts,
                          nonDetAlts, ambigInput, danglingAlts, numWarnings );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestSynPredResolvesRecursion4() /*throws Exception*/ {
            // No errors with k=2; don't try k=* first
            // Should be ok like k=1 'except bigger DFA
            Grammar g = new Grammar(
                "parser grammar t;\n" +
                "statement\n" +
                "options {k=2;}\n" +
                "    :     (reference ASSIGN)=> reference ASSIGN expr\n" +
                "    |     expr\n" +
                "    ;\n" +
                "expr:     reference\n" +
                "    |     INT\n" +
                "    |     FLOAT\n" +
                "    ;\n" +
                "reference\n" +
                "    :     ID L argument_list R\n" +
                "    ;\n" +
                "argument_list\n" +
                "    :     expr COMMA expr\n" +
                "    ;" );
            string expecting =
                ".s0-ID->.s1" + NewLine +
                ".s0-{FLOAT, INT}->:s4=>2" + NewLine +
                ".s1-L->.s2" + NewLine +
                ".s2-{synpred1_t}?->:s3=>1" + NewLine +
                ".s2-{true}?->:s4=>2" + NewLine;
            int[] unreachableAlts = null;
            int[] nonDetAlts = null;
            string ambigInput = null;
            int[] danglingAlts = null;
            int numWarnings = 0;
            checkDecision( g, 1, expecting, unreachableAlts,
                          nonDetAlts, ambigInput, danglingAlts, numWarnings );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestSynPredResolvesRecursionInLexer() /*throws Exception*/ {
            Grammar g = new Grammar(
                "lexer grammar t;\n" +
                "A :     (B ';')=> B ';'\n" +
                "  |     B '.'\n" +
                "  ;\n" +
                "fragment\n" +
                "B :     '(' B ')'\n" +
                "  |     'x'\n" +
                "  ;\n" );
            string expecting =
                ".s0-'('->.s1" + NewLine +
                ".s0-'x'->.s4" + NewLine +
                ".s1-{synpred1_t}?->:s2=>1" + NewLine +
                ".s1-{true}?->:s3=>2" + NewLine +
                ".s4-{synpred1_t}?->:s2=>1" + NewLine +
                ".s4-{true}?->:s3=>2" + NewLine;
            int[] unreachableAlts = null;
            int[] nonDetAlts = null;
            string ambigInput = null;
            int[] danglingAlts = null;
            int numWarnings = 0;
            checkDecision( g, 1, expecting, unreachableAlts,
                          nonDetAlts, ambigInput, danglingAlts, numWarnings );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestAutoBacktrackResolvesRecursionInLexer() /*throws Exception*/ {
            Grammar g = new Grammar(
                "lexer grammar t;\n" +
                "options {backtrack=true;}\n" +
                "A :     B ';'\n" +
                "  |     B '.'\n" +
                "  ;\n" +
                "fragment\n" +
                "B :     '(' B ')'\n" +
                "  |     'x'\n" +
                "  ;\n" );
            string expecting =
                ".s0-'('->.s1" + NewLine +
                ".s0-'x'->.s4" + NewLine +
                ".s1-{synpred1_t}?->:s2=>1" + NewLine +
                ".s1-{true}?->:s3=>2" + NewLine +
                ".s4-{synpred1_t}?->:s2=>1" + NewLine +
                ".s4-{true}?->:s3=>2" + NewLine;
            int[] unreachableAlts = null;
            int[] nonDetAlts = null;
            string ambigInput = null;
            int[] danglingAlts = null;
            int numWarnings = 0;
            checkDecision( g, 1, expecting, unreachableAlts,
                          nonDetAlts, ambigInput, danglingAlts, numWarnings );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestAutoBacktrackResolvesRecursion() /*throws Exception*/ {
            Grammar g = new Grammar(
                "parser grammar t;\n" +
                "options {backtrack=true;}\n" +
                "x   : y X\n" +
                "    | y Y\n" +
                "    ;\n" +
                "y   : L y R\n" +
                "    | B\n" +
                "    ;" );
            string expecting =
                ".s0-B->.s4" + NewLine +
                ".s0-L->.s1" + NewLine +
                ".s1-{synpred1_t}?->:s2=>1" + NewLine +
                ".s1-{true}?->:s3=>2" + NewLine +
                ".s4-{synpred1_t}?->:s2=>1" + NewLine +
                ".s4-{true}?->:s3=>2" + NewLine;
            int[] unreachableAlts = null;
            int[] nonDetAlts = null;
            string ambigInput = null;
            int[] danglingAlts = null;
            int numWarnings = 0;
            checkDecision( g, 1, expecting, unreachableAlts,
                          nonDetAlts, ambigInput, danglingAlts, numWarnings );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestselfRecurseNonDet2() /*throws Exception*/ {
            Grammar g = new Grammar(
                "parser grammar t;\n" +
                "s : a ;\n" +
                "a : P a P | P;" );
            // nondeterministic from left edge
            string expecting =
                ".s0-P->.s1" + NewLine +
                ".s1-EOF->:s3=>2" + NewLine +
                ".s1-P->:s2=>1" + NewLine;
            int[] unreachableAlts = null;
            int[] nonDetAlts = new int[] { 1, 2 };
            string ambigInput = "P P";
            int[] danglingAlts = null;
            int numWarnings = 1;
            checkDecision( g, 1, expecting, unreachableAlts,
                          nonDetAlts, ambigInput, danglingAlts, numWarnings );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestIndirectRecursionLoop() /*throws Exception*/ {
            Grammar g = new Grammar(
                "parser grammar t;\n" +
                "s : a ;\n" +
                "a : b X ;\n" +
                "b : a B ;\n" );

            DecisionProbe.verbose = true; // make sure we get all error info
            ErrorQueue equeue = new ErrorQueue();
            ErrorManager.SetErrorListener( equeue );

            HashSet<Rule> leftRecursive = g.GetLeftRecursiveRules();
            //Set expectedRules =
            //    new HashSet() {{add("a"); add("b");}};
            var expectedRules = new HashSet<string>();
            expectedRules.Add( "a" );
            expectedRules.Add( "b" );

            Assert.IsTrue( expectedRules.SequenceEqual( ruleNames( leftRecursive ) ) );

            Assert.AreEqual(1, equeue.errors.Count);
            Message msg = equeue.errors[0];
            Assert.IsTrue(msg is LeftRecursionCyclesMessage, "expecting left recursion cycles; found " + msg.GetType().Name);
            LeftRecursionCyclesMessage cyclesMsg = (LeftRecursionCyclesMessage)msg;

            // cycle of [a, b]
            ICollection result = cyclesMsg.cycles;
            var expecting = new HashSet<string>(); //{{add("a"); add("b");}};
            expecting.Add( "a" );
            expecting.Add( "b" );
            Assert.IsTrue( expecting.SequenceEqual( ruleNames2( result ) ) );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestIndirectRecursionLoop2() /*throws Exception*/ {
            Grammar g = new Grammar(
                "parser grammar t;\n" +
                "s : a ;\n" +
                "a : i b X ;\n" + // should see through i
                "b : a B ;\n" +
                "i : ;\n" );

            DecisionProbe.verbose = true; // make sure we get all error info
            ErrorQueue equeue = new ErrorQueue();
            ErrorManager.SetErrorListener( equeue );

            var leftRecursive = g.GetLeftRecursiveRules();
            //Set expectedRules =
            //    new HashSet() {{add("a"); add("b");}};
            var expectedRules = new HashSet<string>();
            expectedRules.Add( "a" );
            expectedRules.Add( "b" );
            Assert.IsTrue( expectedRules.SequenceEqual( ruleNames( leftRecursive ) ) );

            Assert.AreEqual(1, equeue.errors.Count);
            Message msg = equeue.errors[0];
            Assert.IsTrue(msg is LeftRecursionCyclesMessage, "expecting left recursion cycles; found " + msg.GetType().Name);
            LeftRecursionCyclesMessage cyclesMsg = (LeftRecursionCyclesMessage)msg;

            // cycle of [a, b]
            ICollection result = cyclesMsg.cycles;
            //Set expecting = new HashSet() {{add("a"); add("b");}};
            var expecting = new HashSet<string>();
            expecting.Add( "a" );
            expecting.Add( "b" );

            Assert.IsTrue( expecting.SequenceEqual( ruleNames2( result ) ) );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestIndirectRecursionLoop3() /*throws Exception*/ {
            Grammar g = new Grammar(
                "parser grammar t;\n" +
                "s : a ;\n" +
                "a : i b X ;\n" + // should see through i
                "b : a B ;\n" +
                "i : ;\n" +
                "d : e ;\n" +
                "e : d ;\n" );

            DecisionProbe.verbose = true; // make sure we get all error info
            ErrorQueue equeue = new ErrorQueue();
            ErrorManager.SetErrorListener( equeue );

            var leftRecursive = g.GetLeftRecursiveRules();
            var expectedRules = new HashSet<string>() { "a", "b", "d", "e" };

            Assert.IsTrue( expectedRules.SequenceEqual( ruleNames( leftRecursive ) ) );

            Assert.AreEqual(1, equeue.errors.Count);
            Message msg = equeue.errors[0];
            Assert.IsTrue(msg is LeftRecursionCyclesMessage, "expecting left recursion cycles; found " + msg.GetType().Name);
            LeftRecursionCyclesMessage cyclesMsg = (LeftRecursionCyclesMessage)msg;

            // cycle of [a, b]
            ICollection result = cyclesMsg.cycles;
            var expecting = new HashSet<string>() { "a", "b", "d", "e" };

            Assert.IsTrue( expecting.SequenceEqual( ruleNames2( result ) ) );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestifThenElse() /*throws Exception*/ {
            Grammar g = new Grammar(
                "parser grammar t;\n" +
                "s : IF s (E s)? | B;\n" +
                "slist: s SEMI ;" );
            string expecting =
                ".s0-E->:s1=>1" + NewLine +
                ".s0-SEMI->:s2=>2" + NewLine;
            int[] unreachableAlts = null;
            int[] nonDetAlts = new int[] { 1, 2 };
            string ambigInput = "E";
            int[] danglingAlts = null;
            int numWarnings = 1;
            checkDecision( g, 1, expecting, unreachableAlts,
                          nonDetAlts, ambigInput, danglingAlts, numWarnings );
            expecting =
                ".s0-B->:s2=>2" + NewLine +
                ".s0-IF->:s1=>1" + NewLine;
            checkDecision( g, 2, expecting, null, null, null, null, 0 );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestifThenElseChecksStackSuffixConflict() /*throws Exception*/ {
            // if you don't check stack soon enough, this finds E B not just E
            // as ambig input
            Grammar g = new Grammar(
                "parser grammar t;\n" +
                "slist: s SEMI ;\n" +
                "s : IF s el | B;\n" +
                "el: (E s)? ;\n" );
            string expecting =
                ".s0-E->:s1=>1" + NewLine +
                ".s0-SEMI->:s2=>2" + NewLine;
            int[] unreachableAlts = null;
            int[] nonDetAlts = new int[] { 1, 2 };
            string ambigInput = "E";
            int[] danglingAlts = null;
            int numWarnings = 1;
            checkDecision( g, 2, expecting, unreachableAlts,
                          nonDetAlts, ambigInput, danglingAlts, numWarnings );
            expecting =
                ".s0-B->:s2=>2" + NewLine +
                ".s0-IF->:s1=>1" + NewLine;
            checkDecision( g, 1, expecting, null, null, null, null, 0 );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestInvokeRule() /*throws Exception*/ {
            Grammar g = new Grammar(
                "parser grammar t;\n" +
                "a : b A\n" +
                "  | b B\n" +
                "  | C\n" +
                "  ;\n" +
                "b : X\n" +
                "  ;\n" );
            string expecting =
                ".s0-C->:s4=>3" + NewLine +
                ".s0-X->.s1" + NewLine +
                ".s1-A->:s2=>1" + NewLine +
                ".s1-B->:s3=>2" + NewLine;
            checkDecision( g, 1, expecting, null, null, null, null, 0 );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestDoubleInvokeRuleLeftEdge() /*throws Exception*/ {
            Grammar g = new Grammar(
                "parser grammar t;\n" +
                "a : b X\n" +
                "  | b Y\n" +
                "  ;\n" +
                "b : c B\n" +
                "  | c\n" +
                "  ;\n" +
                "c : C ;\n" );
            string expecting =
                ".s0-C->.s1" + NewLine +
                ".s1-B->.s2" + NewLine +
                ".s1-X->:s3=>1" + NewLine +
                ".s1-Y->:s4=>2" + NewLine +
                ".s2-X->:s3=>1" + NewLine +
                ".s2-Y->:s4=>2" + NewLine;
            checkDecision( g, 1, expecting, null, null, null, null, 0 );
            expecting =
                ".s0-C->.s1" + NewLine +
                ".s1-B->:s2=>1" + NewLine +
                ".s1-X..Y->:s3=>2" + NewLine;
            checkDecision( g, 2, expecting, null, null, null, null, 0 );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestimmediateTailRecursion() /*throws Exception*/ {
            Grammar g = new Grammar(
                "parser grammar t;\n" +
                "s : a ;\n" +
                "a : A a | A B;" );
            string expecting =
                ".s0-A->.s1" + NewLine +
                ".s1-A->:s3=>1" + NewLine +
                ".s1-B->:s2=>2" + NewLine;
            checkDecision( g, 1, expecting, null, null, null, null, 0 );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestAStar_immediateTailRecursion() /*throws Exception*/ {
            Grammar g = new Grammar(
                "parser grammar t;\n" +
                "s : a ;\n" +
                "a : A a | ;" );
            string expecting =
                ".s0-A->:s1=>1" + NewLine +
                ".s0-EOF->:s2=>2" + NewLine;
            int[] unreachableAlts = null; // without
            int[] nonDetAlts = null;
            string ambigInput = null;
            int[] danglingAlts = null;
            int numWarnings = 0;
            checkDecision( g, 1, expecting, unreachableAlts,
                          nonDetAlts, ambigInput, danglingAlts, numWarnings );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestNoStartRule() /*throws Exception*/ {
            ErrorQueue equeue = new ErrorQueue();
            ErrorManager.SetErrorListener( equeue );
            Grammar g = new Grammar(
                "parser grammar t;\n" +
                "a : A a | X;" ); // single rule 'a' refers to itself; no start rule

            AntlrTool antlr = newTool();
            antlr.SetOutputDirectory( null ); // write to /dev/null
            CodeGenerator generator = new CodeGenerator( antlr, g, "Java" );
            g.CodeGenerator = generator;
            generator.GenRecognizer();

            Message msg = (Message)equeue.warnings[0];
            Assert.IsTrue(msg is GrammarSemanticsMessage, "expecting no start rules; found " + msg.GetType().Name);
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestAStar_immediateTailRecursion2() /*throws Exception*/ {
            Grammar g = new Grammar(
                "parser grammar t;\n" +
                "s : a ;\n" +
                "a : A a | A ;" );
            string expecting =
                ".s0-A->.s1" + NewLine +
                ".s1-A->:s2=>1" + NewLine +
                ".s1-EOF->:s3=>2" + NewLine;
            int[] unreachableAlts = null;
            int[] nonDetAlts = null;
            string ambigInput = null;
            int[] danglingAlts = null;
            int numWarnings = 0;
            checkDecision( g, 1, expecting, unreachableAlts,
                          nonDetAlts, ambigInput, danglingAlts, numWarnings );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestimmediateLeftRecursion() /*throws Exception*/ {
            Grammar g = new Grammar(
                "parser grammar t;\n" +
                "s : a ;\n" +
                "a : a A | B;" );
            var leftRecursive = g.GetLeftRecursiveRules();
            //Set expectedRules = new HashSet() {{add("a");}};
            var expectedRules = new HashSet<string>();
            expectedRules.Add( "a" );
            Assert.IsTrue( expectedRules.SequenceEqual( ruleNames( leftRecursive ) ) );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestIndirectLeftRecursion() /*throws Exception*/ {
            Grammar g = new Grammar(
                "parser grammar t;\n" +
                "s : a ;\n" +
                "a : b | A ;\n" +
                "b : c ;\n" +
                "c : a | C ;\n" );
            var leftRecursive = g.GetLeftRecursiveRules();
            //Set expectedRules = new HashSet() {{add("a"); add("b"); add("c");}};
            var expectedRules = new HashSet<string>();
            expectedRules.Add( "a" );
            expectedRules.Add( "b" );
            expectedRules.Add( "c" );
            Assert.IsTrue( expectedRules.SequenceEqual( ruleNames( leftRecursive ) ) );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestLeftRecursionInMultipleCycles() /*throws Exception*/ {
            Grammar g = new Grammar(
                "parser grammar t;\n" +
                    "s : a x ;\n" +
                    "a : b | A ;\n" +
                    "b : c ;\n" +
                    "c : a | C ;\n" +
                    "x : y | X ;\n" +
                    "y : x ;\n" );
            var leftRecursive = g.GetLeftRecursiveRules();
            //Set expectedRules =
            //    new HashSet() {{add("a"); add("b"); add("c"); add("x"); add("y");}};
            var expectedRules = new HashSet<string>();
            expectedRules.Add( "a" );
            expectedRules.Add( "b" );
            expectedRules.Add( "c" );
            expectedRules.Add( "x" );
            expectedRules.Add( "y" );
            Assert.IsTrue( expectedRules.SequenceEqual( ruleNames( leftRecursive ) ) );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestCycleInsideRuleDoesNotForceInfiniteRecursion() /*throws Exception*/ {
            Grammar g = new Grammar(
                "parser grammar t;\n" +
                "s : a ;\n" +
                "a : (A|)+ B;\n" );
            // before I added a visitedStates thing, it was possible to loop
            // forever inside of a rule if there was an epsilon loop.
            var leftRecursive = g.GetLeftRecursiveRules();
            var expectedRules = new HashSet<Rule>();
            Assert.IsTrue( expectedRules.SequenceEqual( leftRecursive ) );
        }

        // L O O P S

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestAStar() /*throws Exception*/ {
            Grammar g = new Grammar(
                "parser grammar t;\n" +
                "a : ( A )* ;" );
            string expecting =
                ".s0-A->:s1=>1" + NewLine +
                ".s0-EOF->:s2=>2" + NewLine;
            checkDecision( g, 1, expecting, null, null, null, null, 0 );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestAorBorCStar() /*throws Exception*/ {
            Grammar g = new Grammar(
                "parser grammar t;\n" +
                "a : ( A | B | C )* ;" );
            string expecting =
                ".s0-A..C->:s1=>1" + NewLine +
                ".s0-EOF->:s2=>2" + NewLine;
            checkDecision( g, 1, expecting, null, null, null, null, 0 );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestAPlus() /*throws Exception*/ {
            Grammar g = new Grammar(
                "parser grammar t;\n" +
                "a : ( A )+ ;" );
            string expecting =
                ".s0-A->:s1=>1" + NewLine +
                ".s0-EOF->:s2=>2" + NewLine;
            checkDecision( g, 1, expecting, null, null, null, null, 0 ); // loopback decision
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestAPlusNonGreedyWhenDeterministic() /*throws Exception*/ {
            Grammar g = new Grammar(
                "parser grammar t;\n" +
                "a : (options {greedy=false;}:A)+ ;\n" );
            // should look the same as A+ since no ambiguity
            string expecting =
                ".s0-A->:s1=>1" + NewLine +
                ".s0-EOF->:s2=>2" + NewLine;
            checkDecision( g, 1, expecting, null, null, null, null, 0 );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestAPlusNonGreedyWhenNonDeterministic() /*throws Exception*/ {
            Grammar g = new Grammar(
                "parser grammar t;\n" +
                "a : (options {greedy=false;}:A)+ A+ ;\n" );
            // should look the same as A+ since no ambiguity
            string expecting =
                ".s0-A->:s1=>2" + NewLine; // always chooses to exit
            int[] unreachableAlts = new int[] { 1 };
            int[] nonDetAlts = new int[] { 1, 2 };
            string ambigInput = "A";
            int[] danglingAlts = null;
            int numWarnings = 2;
            checkDecision( g, 1, expecting, unreachableAlts,
                          nonDetAlts, ambigInput, danglingAlts, numWarnings );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestAPlusGreedyWhenNonDeterministic() /*throws Exception*/ {
            Grammar g = new Grammar(
                "parser grammar t;\n" +
                "a : (options {greedy=true;}:A)+ A+ ;\n" );
            // should look the same as A+ since no ambiguity
            string expecting =
                ".s0-A->:s1=>1" + NewLine; // always chooses to enter loop upon A
            // turns off 1 of warnings. A can never exit loop now
            int[] unreachableAlts = new int[] { 2 };
            int[] nonDetAlts = null;
            string ambigInput = null;
            int[] danglingAlts = null;
            int numWarnings = 1;
            checkDecision(g, 1, expecting, unreachableAlts,
                          nonDetAlts, ambigInput, danglingAlts, numWarnings );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestAorBorCPlus() /*throws Exception*/ {
            Grammar g = new Grammar(
                "parser grammar t;\n" +
                "a : ( A | B | C )+ ;" );
            string expecting =
                ".s0-A..C->:s1=>1" + NewLine +
                ".s0-EOF->:s2=>2" + NewLine;
            checkDecision( g, 1, expecting, null, null, null, null, 0 );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestAOptional() /*throws Exception*/ {
            Grammar g = new Grammar(
                "parser grammar t;\n" +
                "a : ( A )? B ;" );
            string expecting =
                ".s0-A->:s1=>1" + NewLine +
                ".s0-B->:s2=>2" + NewLine;
            checkDecision( g, 1, expecting, null, null, null, null, 0 ); // loopback decision
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestAorBorCOptional() /*throws Exception*/ {
            Grammar g = new Grammar(
                "parser grammar t;\n" +
                "a : ( A | B | C )? Z ;" );
            string expecting =
                ".s0-A..C->:s1=>1" + NewLine +
                ".s0-Z->:s2=>2" + NewLine;
            checkDecision( g, 1, expecting, null, null, null, null, 0 ); // loopback decision
        }

        // A R B I T R A R Y  L O O K A H E A D

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestAStarBOrAStarC() /*throws Exception*/ {
            Grammar g = new Grammar(
                "parser grammar t;\n" +
                "a : (A)* B | (A)* C;" );
            string expecting =
                ".s0-A->:s1=>1" + NewLine +
                ".s0-B->:s2=>2" + NewLine;
            checkDecision( g, 1, expecting, null, null, null, null, 0 ); // loopback
            expecting =
                ".s0-A->:s1=>1" + NewLine +
                ".s0-C->:s2=>2" + NewLine;
            checkDecision( g, 2, expecting, null, null, null, null, 0 ); // loopback
            expecting =
                ".s0-A->.s1" + NewLine +
                ".s0-B->:s2=>1" + NewLine +
                ".s0-C->:s3=>2" + NewLine +
                ".s1-A->.s1" + NewLine +
                ".s1-B->:s2=>1" + NewLine +
                ".s1-C->:s3=>2" + NewLine;
            checkDecision( g, 3, expecting, null, null, null, null, 0 ); // rule block
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestAStarBOrAPlusC() /*throws Exception*/ {
            Grammar g = new Grammar(
                "parser grammar t;\n" +
                "a : (A)* B | (A)+ C;" );
            string expecting =
                ".s0-A->:s1=>1" + NewLine +
                ".s0-B->:s2=>2" + NewLine;
            checkDecision( g, 1, expecting, null, null, null, null, 0 ); // loopback
            expecting =
                ".s0-A->:s1=>1" + NewLine +
                ".s0-C->:s2=>2" + NewLine;
            checkDecision( g, 2, expecting, null, null, null, null, 0 ); // loopback
            expecting =
                ".s0-A->.s1" + NewLine +
                ".s0-B->:s2=>1" + NewLine +
                ".s1-A->.s1" + NewLine +
                ".s1-B->:s2=>1" + NewLine +
                ".s1-C->:s3=>2" + NewLine;
            checkDecision( g, 3, expecting, null, null, null, null, 0 ); // rule block
        }


        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestAOrBPlusOrAPlus() /*throws Exception*/ {
            Grammar g = new Grammar(
                "parser grammar t;\n" +
                "a : (A|B)* X | (A)+ Y;" );
            string expecting =
                ".s0-A..B->:s1=>1" + NewLine +
                ".s0-X->:s2=>2" + NewLine;
            checkDecision( g, 1, expecting, null, null, null, null, 0 ); // loopback (A|B)*
            expecting =
                ".s0-A->:s1=>1" + NewLine +
                ".s0-Y->:s2=>2" + NewLine;
            checkDecision( g, 2, expecting, null, null, null, null, 0 ); // loopback (A)+
            expecting =
                ".s0-A->.s1" + NewLine +
                ".s0-B..X->:s2=>1" + NewLine +
                ".s1-A->.s1" + NewLine +
                ".s1-B..X->:s2=>1" + NewLine +
                ".s1-Y->:s3=>2" + NewLine;
            checkDecision( g, 3, expecting, null, null, null, null, 0 ); // rule
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestLoopbackAndExit() /*throws Exception*/ {
            Grammar g = new Grammar(
                "parser grammar t;\n" +
                "a : (A|B)+ B;" );
            string expecting =
                ".s0-A->:s3=>1" + NewLine +
                ".s0-B->.s1" + NewLine +
                ".s1-A..B->:s3=>1" + NewLine +
                ".s1-EOF->:s2=>2" + NewLine; // sees A|B as a set
            checkDecision( g, 1, expecting, null, null, null, null, 0 );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestOptionalAltAndBypass() /*throws Exception*/ {
            Grammar g = new Grammar(
                "parser grammar t;\n" +
                "a : (A|B)? B;" );
            string expecting =
                ".s0-A->:s2=>1" + NewLine +
                ".s0-B->.s1" + NewLine +
                ".s1-B->:s2=>1" + NewLine +
                ".s1-EOF->:s3=>2" + NewLine;
            checkDecision( g, 1, expecting, null, null, null, null, 0 );
        }

        // R E S O L V E  S Y N  C O N F L I C T S

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestResolveLL1ByChoosingFirst() /*throws Exception*/ {
            Grammar g = new Grammar(
                "parser grammar t;\n" +
                "a : A C | A C;" );
            string expecting =
                ".s0-A->.s1" + NewLine +
                ".s1-C->:s2=>1" + NewLine;
            int[] unreachableAlts = new int[] { 2 };
            int[] nonDetAlts = new int[] { 1, 2 };
            string ambigInput = "A C";
            int[] danglingAlts = null;
            int numWarnings = 2;
            checkDecision( g, 1, expecting, unreachableAlts,
                          nonDetAlts, ambigInput, danglingAlts, numWarnings );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestResolveLL2ByChoosingFirst() /*throws Exception*/ {
            Grammar g = new Grammar(
                "parser grammar t;\n" +
                "a : A B | A B;" );
            string expecting =
                ".s0-A->.s1" + NewLine +
                ".s1-B->:s2=>1" + NewLine;
            int[] unreachableAlts = new int[] { 2 };
            int[] nonDetAlts = new int[] { 1, 2 };
            string ambigInput = "A B";
            int[] danglingAlts = null;
            int numWarnings = 2;
            checkDecision( g, 1, expecting, unreachableAlts,
                          nonDetAlts, ambigInput, danglingAlts, numWarnings );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestResolveLL2MixAlt() /*throws Exception*/ {
            Grammar g = new Grammar(
                "parser grammar t;\n" +
                "a : A B | A C | A B | Z;" );
            string expecting =
                ".s0-A->.s1" + NewLine +
                ".s0-Z->:s4=>4" + NewLine +
                ".s1-B->:s2=>1" + NewLine +
                ".s1-C->:s3=>2" + NewLine;
            int[] unreachableAlts = new int[] { 3 };
            int[] nonDetAlts = new int[] { 1, 3 };
            string ambigInput = "A B";
            int[] danglingAlts = null;
            int numWarnings = 2;
            checkDecision( g, 1, expecting, unreachableAlts,
                          nonDetAlts, ambigInput, danglingAlts, numWarnings );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestIndirectIFThenElseStyleAmbig() /*throws Exception*/ {
            //Assert.Inconclusive( "May be failing on just my port..." );
            // the (c)+ loopback is ambig because it could match "CASE"
            // by entering the loop or by falling out and ignoring (s)*
            // back falling back into (cg)* loop which stats over and
            // calls cg again.  Either choice allows it to get back to
            // the same node.  The software catches it as:
            // "avoid infinite closure computation emanating from alt 1
            // of ():27|2|[8 $]" where state 27 is the first alt of (c)+
            // and 8 is the first alt of the (cg)* loop.
            Grammar g = new Grammar(
                "parser grammar t;\n" +
                "s : stat ;\n" +
                "stat : LCURLY ( cg )* RCURLY | E SEMI  ;\n" +
                "cg : (c)+ (stat)* ;\n" +
                "c : CASE E ;\n" );
            string expecting =
                ".s0-CASE->:s2=>1" + NewLine +
                ".s0-E..RCURLY->:s1=>2" + NewLine;
            int[] unreachableAlts = null;
            int[] nonDetAlts = new int[] { 1, 2 };
            string ambigInput = "CASE";
            int[] danglingAlts = null;
            int numWarnings = 1;
            checkDecision( g, 3, expecting, unreachableAlts,
                          nonDetAlts, ambigInput, danglingAlts, numWarnings );
        }

        // S E T S

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestComplement() /*throws Exception*/ {
            Grammar g = new Grammar(
                "parser grammar t;\n" +
                "a : ~(A | B | C) | C {;} ;\n" +
                "b : X Y Z ;" );
            string expecting =
                ".s0-C->:s2=>2" + NewLine +
                ".s0-X..Z->:s1=>1" + NewLine;
            checkDecision( g, 1, expecting, null, null, null, null, 0 );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestComplementToken() /*throws Exception*/ {
            Grammar g = new Grammar(
                "parser grammar t;\n" +
                "a : ~C | C {;} ;\n" +
                "b : X Y Z ;" );
            string expecting =
                ".s0-C->:s2=>2" + NewLine +
                ".s0-X..Z->:s1=>1" + NewLine;
            checkDecision( g, 1, expecting, null, null, null, null, 0 );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestComplementChar() /*throws Exception*/ {
            Grammar g = new Grammar(
                "lexer grammar t;\n" +
                "A : ~'x' | 'x' {;} ;\n" );
            string expecting =
                ".s0-'x'->:s2=>2" + NewLine +
                ".s0-{'\\u0000'..'w', 'y'..'\\uFFFF'}->:s1=>1" + NewLine;
            checkDecision( g, 1, expecting, null, null, null, null, 0 );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestComplementCharSet() /*throws Exception*/ {
            Grammar g = new Grammar(
                "lexer grammar t;\n" +
                "A : ~(' '|'\t'|'x'|'y') | 'x';\n" + // collapse into single set
                "B : 'y' ;" );
            string expecting =
                ".s0-'y'->:s2=>2" + NewLine +
                ".s0-{'\\u0000'..'\\b', '\\n'..'\\u001F', '!'..'x', 'z'..'\\uFFFF'}->:s1=>1" + NewLine;
            checkDecision( g, 1, expecting, null, null, null, null, 0 );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestNoSetCollapseWithActions() /*throws Exception*/ {
            Grammar g = new Grammar(
                "parser grammar t;\n" +
                "a : (A | B {foo}) | C;" );
            string expecting =
                ".s0-A->:s1=>1" + NewLine +
                ".s0-B->:s2=>2" + NewLine;
            checkDecision( g, 1, expecting, null, null, null, null, 0 );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestRuleAltsSetCollapse() /*throws Exception*/ {
            Grammar g = new Grammar(
                "parser grammar t;\n" +
                "a : A | B | C ;"
            );
            string expecting = // still looks like block
                "(grammar t (rule a ARG RET scope (BLOCK (ALT A <end-of-alt>) (ALT B <end-of-alt>) (ALT C <end-of-alt>) <end-of-block>) <end-of-rule>))";
            Assert.AreEqual( expecting, g.Tree.ToStringTree() );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestTokensRuleAltsDoNotCollapse() /*throws Exception*/ {
            Grammar g = new Grammar(
                "lexer grammar t;\n" +
                "A : 'a';" +
                "B : 'b';\n"
            );
            string expecting =
                ".s0-'a'->:s1=>1" + NewLine +
                ".s0-'b'->:s2=>2" + NewLine;
            checkDecision( g, 1, expecting, null, null, null, null, 0 );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestMultipleSequenceCollision() /*throws Exception*/ {
            Grammar g = new Grammar(
                "parser grammar t;\n" +
                "a : (A{;}|B)\n" +
                "  | (A{;}|B)\n" +
                "  | A\n" +
                "  ;" );
            string expecting =
                ".s0-A->:s1=>1" + NewLine +
                ".s0-B->:s2=>1" + NewLine; // not optimized because states are nondet
            int[] unreachableAlts = new int[] { 2, 3 };
            int[] nonDetAlts = new int[] { 1, 2, 3 };
            string ambigInput = "A";
            int[] danglingAlts = null;
            int numWarnings = 3;
            checkDecision( g, 3, expecting, unreachableAlts,
                          nonDetAlts, ambigInput, danglingAlts, numWarnings );
            /* There are 2 nondet errors, but the checkDecision only checks first one :(
            The "B" conflicting input is not checked except by virtue of the
            result DFA.
    <string>:2:5: Decision can match input such as "A" using multiple alternatives:
    alt 1 via NFA path 7,2,3
    alt 2 via NFA path 14,9,10
    alt 3 via NFA path 16,17
    As a result, alternative(s) 2,3 were disabled for that input,
    <string>:2:5: Decision can match input such as "B" using multiple alternatives:
    alt 1 via NFA path 7,8,4,5
    alt 2 via NFA path 14,15,11,12
    As a result, alternative(s) 2 were disabled for that input
    <string>:2:5: The following alternatives are unreachable: 2,3
    */
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestMultipleAltsSameSequenceCollision() /*throws Exception*/ {
            Grammar g = new Grammar(
                "parser grammar t;\n" +
                "a : type ID \n" +
                "  | type ID\n" +
                "  | type ID\n" +
                "  | type ID\n" +
                "  ;\n" +
                "\n" +
                "type : I | F;" );
            // nondeterministic from left edge; no stop state
            string expecting =
                ".s0-F..I->.s1" + NewLine +
                ".s1-ID->:s2=>1" + NewLine;
            int[] unreachableAlts = new int[] { 2, 3, 4 };
            int[] nonDetAlts = new int[] { 1, 2, 3, 4 };
            string ambigInput = "F..I ID";
            int[] danglingAlts = null;
            int numWarnings = 2;
            checkDecision( g, 1, expecting, unreachableAlts,
                          nonDetAlts, ambigInput, danglingAlts, numWarnings );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestFollowReturnsToLoopReenteringSameRule() /*throws Exception*/ {
            //Assert.Inconclusive( "May be failing on just my port..." );
            // D07 can be matched in the (...)? or fall out of esc back into (..)*
            // loop in sl.  Note that D07 is matched by ~(R|SLASH).  No good
            // way to write that grammar I guess
            Grammar g = new Grammar(
                "parser grammar t;\n" +
                "sl : L ( esc | ~(R|SLASH) )* R ;\n" +
                "\n" +
                "esc : SLASH ( N | D03 (D07)? ) ;" );
            string expecting =
                ".s0-D03..N->:s2=>2" + NewLine +
                ".s0-R->:s3=>3" + NewLine +
                ".s0-SLASH->:s1=>1" + NewLine;
            int[] unreachableAlts = null;
            int[] nonDetAlts = new int[] { 1, 2 };
            string ambigInput = "D07";
            int[] danglingAlts = null;
            int numWarnings = 1;
            checkDecision( g, 1, expecting, unreachableAlts,
                          nonDetAlts, ambigInput, danglingAlts, numWarnings );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestTokenCallsAnotherOnLeftEdge() /*throws Exception*/ {
            Grammar g = new Grammar(
                "lexer grammar t;\n" +
                "F   :   I '.'\n" +
                "    ;\n" +
                "I   :   '0'\n" +
                "    ;\n"
            );
            string expecting =
                ".s0-'0'->.s1" + NewLine +
                ".s1-'.'->:s3=>1" + NewLine +
                ".s1-<EOT>->:s2=>2" + NewLine;
            checkDecision( g, 1, expecting, null, null, null, null, 0 );
        }


        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestSelfRecursionAmbigAlts() /*throws Exception*/ {
            // ambiguous grammar for "L ID R" (alts 1,2 of a)
            Grammar g = new Grammar(
                "parser grammar t;\n" +
                "s : a;\n" +
                "a   :   L ID R\n" +
                "    |   L a R\n" + // disabled for L ID R
                "    |   b\n" +
                "    ;\n" +
                "\n" +
                "b   :   ID\n" +
                "    ;\n" );
            string expecting =
                ".s0-ID->:s5=>3" + NewLine +
                ".s0-L->.s1" + NewLine +
                ".s1-ID->.s2" + NewLine +
                ".s1-L->:s4=>2" + NewLine +
                ".s2-R->:s3=>1" + NewLine;
            int[] unreachableAlts = null;
            int[] nonDetAlts = new int[] { 1, 2 };
            string ambigInput = "L ID R";
            int[] danglingAlts = null;
            int numWarnings = 1;
            checkDecision( g, 1, expecting, unreachableAlts,
                          nonDetAlts, ambigInput, danglingAlts, numWarnings );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestIndirectRecursionAmbigAlts() /*throws Exception*/ {
            // ambiguous grammar for "L ID R" (alts 1,2 of a)
            // This was derived from the java grammar 12/4/2004 when it
            // was not handling a unaryExpression properly.  I traced it
            // to incorrect closure-busy condition.  It thought that the trace
            // of a->b->a->b again for "L ID" was an infinite loop, but actually
            // the repeat call to b only happens *after* an L has been matched.
            // I added a check to see what the initial stack looks like and it
            // seems to work now.
            Grammar g = new Grammar(
                "parser grammar t;\n" +
                "s   :   a ;\n" +
                "a   :   L ID R\n" +
                "    |   b\n" +
                "    ;\n" +
                "\n" +
                "b   :   ID\n" +
                "    |   L a R\n" +
                "    ;" );
            string expecting =
                ".s0-ID->:s4=>2" + NewLine +
                ".s0-L->.s1" + NewLine +
                ".s1-ID->.s2" + NewLine +
                ".s1-L->:s4=>2" + NewLine +
                ".s2-R->:s3=>1" + NewLine;
            int[] unreachableAlts = null;
            int[] nonDetAlts = new int[] { 1, 2 };
            string ambigInput = "L ID R";
            int[] danglingAlts = null;
            int numWarnings = 1;
            checkDecision( g, 1, expecting, unreachableAlts,
                          nonDetAlts, ambigInput, danglingAlts, numWarnings );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestTailRecursionInvokedFromArbitraryLookaheadDecision() /*throws Exception*/ {
            Grammar g = new Grammar(
                "parser grammar t;\n" +
                "a : b X\n" +
                "  | b Y\n" +
                "  ;\n" +
                "\n" +
                "b : A\n" +
                "  | A b\n" +
                "  ;\n" );
            var altsWithRecursion = new int[] { 1, 2 };
            assertNonLLStar( g, altsWithRecursion );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestWildcardStarK1AndNonGreedyByDefaultInParser() /*throws Exception*/ {
            // no error because .* assumes it should finish when it sees R
            Grammar g = new Grammar(
                "parser grammar t;\n" +
                "s : A block EOF ;\n" +
                "block : L .* R ;" );
            string expecting =
                ".s0-A..L->:s2=>1" + NewLine +
                ".s0-R->:s1=>2" + NewLine;
            int[] unreachableAlts = null;
            int[] nonDetAlts = null;
            string ambigInput = null;
            int[] danglingAlts = null;
            int numWarnings = 0;
            checkDecision( g, 1, expecting, unreachableAlts,
                          nonDetAlts, ambigInput, danglingAlts, numWarnings );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestWildcardPlusK1AndNonGreedyByDefaultInParser() /*throws Exception*/ {
            Grammar g = new Grammar(
                "parser grammar t;\n" +
                "s : A block EOF ;\n" +
                "block : L .+ R ;" );
            string expecting =
                ".s0-A..L->:s2=>1" + NewLine +
                ".s0-R->:s1=>2" + NewLine;
            int[] unreachableAlts = null;
            int[] nonDetAlts = null;
            string ambigInput = null;
            int[] danglingAlts = null;
            int numWarnings = 0;
            checkDecision( g, 1, expecting, unreachableAlts,
                          nonDetAlts, ambigInput, danglingAlts, numWarnings );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestGatedSynPred() /*throws Exception*/ {
            Grammar g = new Grammar(
                "parser grammar t;\n" +
                "x   : (X)=> X\n" +
                "    | Y\n" +
                "    ;\n" );
            string expecting =
                ".s0-X&&{synpred1_t}?->:s1=>1" + NewLine + // does not hoist; it gates edges
                ".s0-Y->:s2=>2" + NewLine;
            int[] unreachableAlts = null;
            int[] nonDetAlts = null;
            string ambigInput = null;
            int[] danglingAlts = null;
            int numWarnings = 0;
            checkDecision( g, 1, expecting, unreachableAlts,
                          nonDetAlts, ambigInput, danglingAlts, numWarnings );

            HashSet<string> preds = g.synPredNamesUsedInDFA;
            HashSet<string> expectedPreds = new HashSet<string>(); //{{add("synpred1_t");}};
            expectedPreds.Add( "synpred1_t" );
            Assert.IsTrue(expectedPreds.SequenceEqual(preds), "predicate names not recorded properly in grammar");
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestHoistedGatedSynPred()
        {
            Grammar g = new Grammar(
                "parser grammar t;\n" +
                "x   : (X)=> X\n" +
                "    | X\n" +
                "    ;\n");
            string expecting =
                ".s0-X->.s1" + NewLine +
                ".s1-{synpred1_t}?->:s2=>1" + NewLine + // hoists into decision
            ".s1-{true}?->:s3=>2" + NewLine;
            int[] unreachableAlts = null;
            int[] nonDetAlts = null;
            string ambigInput = null;
            int[] danglingAlts = null;
            int numWarnings = 0;
            checkDecision(g, 1, expecting, unreachableAlts,
                          nonDetAlts, ambigInput, danglingAlts, numWarnings);

            HashSet<string> preds = g.synPredNamesUsedInDFA;
            HashSet<string> expectedPreds = new HashSet<string>() { "synpred1_t" };
            Assert.IsTrue(expectedPreds.SetEquals(preds), "predicate names not recorded properly in grammar");
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestHoistedGatedSynPred2()
        {
            Grammar g = new Grammar(
                "parser grammar t;\n" +
                "x   : (X)=> (X|Y)\n" +
                "    | X\n" +
                "    ;\n");
            string expecting =
                ".s0-X->.s1" + NewLine +
                ".s0-Y&&{synpred1_t}?->:s2=>1" + NewLine +
                ".s1-{synpred1_t}?->:s2=>1" + NewLine +
                    ".s1-{true}?->:s3=>2" + NewLine;
            int[] unreachableAlts = null;
            int[] nonDetAlts = null;
            string ambigInput = null;
            int[] danglingAlts = null;
            int numWarnings = 0;
            checkDecision(g, 1, expecting, unreachableAlts,
                          nonDetAlts, ambigInput, danglingAlts, numWarnings);

            HashSet<string> preds = g.synPredNamesUsedInDFA;
            HashSet<string> expectedPreds = new HashSet<string>(); //{{add("synpred1_t");}};
            expectedPreds.Add("synpred1_t");
            Assert.IsTrue(expectedPreds.SequenceEqual(preds), "predicate names not recorded properly in grammar");
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestGreedyGetsNoErrorForAmbig()
        {
            Grammar g = new Grammar(
                "parser grammar t;\n" +
                "s : IF s (options {greedy=true;} : E s)? | B;\n" +
                "slist: s SEMI ;");
            string expecting =
                ".s0-E->:s1=>1" + NewLine +
                ".s0-SEMI->:s2=>2" + NewLine;
            int[] unreachableAlts = null;
            int[] nonDetAlts = null;
            string ambigInput = null;
            int[] danglingAlts = null;
            int numWarnings = 0;
            checkDecision(g, 1, expecting, unreachableAlts,
                          nonDetAlts, ambigInput, danglingAlts, numWarnings);
            expecting =
                ".s0-B->:s2=>2" + NewLine +
                ".s0-IF->:s1=>1" + NewLine;
            checkDecision(g, 2, expecting, null, null, null, null, 0);
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestGreedyNonLLStarStillGetsError()
        {
            Grammar g = new Grammar(
                "parser grammar t;\n" +
                "x   : ( options {greedy=true;}\n" +
                "	   : y X\n" +
                "      | y Y\n" +
                "	   )\n" +
                "    ;\n" +
                "y   : L y R\n" +
                "    | B\n" +
                "    ;");
            IList<int> altsWithRecursion = new int[] { 1, 2 };
            assertNonLLStar(g, altsWithRecursion);
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestGreedyRecOverflowStillGetsError()
        {
            Grammar g = new Grammar(
                "parser grammar t;\n" +
                "s : (options {greedy=true;} : a Y | A A A A A X) ;\n" + // force recursion past m=4
                "a : A a | Q;");
            IList expectedTargetRules = new string[] { "a" };
            int expectedAlt = 1;
            assertRecursionOverflow(g, expectedTargetRules, expectedAlt);
        }

        // Check state table creation

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestCyclicTableCreation() /*throws Exception*/ {
            Grammar g = new Grammar(
                "parser grammar t;\n" +
                "a : A+ X | A+ Y ;" );
            string expecting =
                ".s0-A->:s1=>1" + NewLine +
                ".s0-X->:s2=>2" + NewLine;
            checkDecision( g, 1, expecting, null, null, null, null, 0 );
        }


        // S U P P O R T

        public void _template() /*throws Exception*/ {
            Grammar g = new Grammar(
                "parser grammar t;\n" +
                "a : A | B;" );
            string expecting =
                "\n";
            checkDecision( g, 1, expecting, null, null, null, null, 0 );
        }

        protected void assertNonLLStar( Grammar g, IList<int> expectedBadAlts )
        {
            DecisionProbe.verbose = true; // make sure we get all error info
            ErrorQueue equeue = new ErrorQueue();
            ErrorManager.SetErrorListener( equeue );

            // mimic actions of org.antlr.Tool first time for grammar g
            if ( g.NumberOfDecisions == 0 )
            {
                g.BuildNFA();
                g.CreateLookaheadDFAs( false );
            }
            NonRegularDecisionMessage msg = getNonRegularDecisionMessage( equeue.errors );
            Assert.IsNotNull(msg, "expected fatal non-LL(*) msg");
            List<int> alts = new List<int>(msg.altsWithRecursion);
            alts.Sort();
            //Collections.sort( alts );
            //Assert.AreEqual( expectedBadAlts, alts );
            Assert.IsTrue( expectedBadAlts.SequenceEqual( alts ) );
        }

        protected void assertRecursionOverflow( Grammar g,
                                               IList expectedTargetRules,
                                               int expectedAlt )
        {
            DecisionProbe.verbose = true; // make sure we get all error info
            ErrorQueue equeue = new ErrorQueue();
            ErrorManager.SetErrorListener( equeue );

            // mimic actions of org.antlr.Tool first time for grammar g
            if ( g.NumberOfDecisions == 0 )
            {
                g.BuildNFA();
                g.CreateLookaheadDFAs( false );
            }
            RecursionOverflowMessage msg = getRecursionOverflowMessage( equeue.errors );
            Assert.IsNotNull(msg, "missing expected recursion overflow msg" + msg);
            Assert.AreEqual(expectedTargetRules.ToElementString(), msg.targetRules.ToList().ToElementString(), "target rules mismatch");
            Assert.AreEqual(expectedAlt, msg.alt, "mismatched alt");
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestWildcardInTreeGrammar() /*throws Exception*/
        {
            Grammar g = new Grammar(
                "tree grammar t;\n" +
                "a : A B | A . ;\n" );
            string expecting =
                ".s0-A->.s1" + NewLine +
                ".s1-A->:s3=>2" + NewLine +
                ".s1-B->:s2=>1" + NewLine;
            int[] unreachableAlts = null;
            int[] nonDetAlts = new int[] { 1, 2 };
            string ambigInput = null;
            int[] danglingAlts = null;
            int numWarnings = 1;
            checkDecision( g, 1, expecting, unreachableAlts,
                          nonDetAlts, ambigInput, danglingAlts, numWarnings );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestWildcardInTreeGrammar2() /*throws Exception*/
        {
            Grammar g = new Grammar(
                "tree grammar t;\n" +
                "a : ^(A X Y) | ^(A . .) ;\n" );
            string expecting =
                ".s0-A->.s1" + NewLine +
                ".s1-DOWN->.s2" + NewLine +
                ".s2-X->.s3" + NewLine +
                ".s2-{A, Y}->:s6=>2" + NewLine +
                ".s3-Y->.s4" + NewLine +
                ".s3-{DOWN, A..X}->:s6=>2" + NewLine +
                ".s4-DOWN->:s6=>2" + NewLine +
                ".s4-UP->:s5=>1" + NewLine;
            int[] unreachableAlts = null;
            int[] nonDetAlts = new int[] { 1, 2 };
            string ambigInput = null;
            int[] danglingAlts = null;
            int numWarnings = 1;
            checkDecision( g, 1, expecting, unreachableAlts,
                          nonDetAlts, ambigInput, danglingAlts, numWarnings );
        }

        protected void checkDecision( Grammar g,
                                     int decision,
                                     string expecting,
                                     int[] expectingUnreachableAlts,
                                     int[] expectingNonDetAlts,
                                     string expectingAmbigInput,
                                     int[] expectingDanglingAlts,
                                     int expectingNumWarnings )
        //throws Exception
        {
            DecisionProbe.verbose = true; // make sure we get all error info
            ErrorQueue equeue = new ErrorQueue();
            ErrorManager.SetErrorListener( equeue );

            // mimic actions of org.antlr.Tool first time for grammar g
            if ( g.NumberOfDecisions == 0 )
            {
                g.BuildNFA();
                g.CreateLookaheadDFAs( false );
            }
            CodeGenerator generator = new CodeGenerator( newTool(), g, "Java" );
            g.CodeGenerator = generator;

            if ( equeue.size() != expectingNumWarnings )
            {
                Console.Error.WriteLine( "Warnings issued: " + equeue );
            }

            Assert.AreEqual(expectingNumWarnings, equeue.size(), "unexpected number of expected problems");

            DFA dfa = g.GetLookaheadDFA( decision );
            Assert.IsNotNull( dfa, "no DFA for decision " + decision );
            FASerializer serializer = new FASerializer( g );
            string result = serializer.Serialize( dfa.StartState );

            var unreachableAlts = dfa.UnreachableAlts;

            // make sure unreachable alts are as expected
            if ( expectingUnreachableAlts != null )
            {
                BitSet s = new BitSet();
                s.AddAll( expectingUnreachableAlts );
                BitSet s2 = new BitSet();
                s2.AddAll( unreachableAlts );
                Assert.AreEqual(s, s2, "unreachable alts mismatch");
            }
            else
            {
                Assert.AreEqual(0, unreachableAlts != null ? unreachableAlts.Count : 0, "number of unreachable alts");
            }

            // check conflicting input
            if ( expectingAmbigInput != null )
            {
                // first, find nondet message
                Message msg = (Message)equeue.warnings[0];
                Assert.IsTrue(msg is GrammarNonDeterminismMessage, "expecting nondeterminism; found " + msg.GetType().Name);
                GrammarNonDeterminismMessage nondetMsg =
                    getNonDeterminismMessage( equeue.warnings );
                var labels =
                    nondetMsg.probe.GetSampleNonDeterministicInputSequence( nondetMsg.problemState );
                string input = nondetMsg.probe.GetInputSequenceDisplay( labels );
                Assert.AreEqual( expectingAmbigInput, input );
            }

            // check nondet alts
            if ( expectingNonDetAlts != null )
            {
                RecursionOverflowMessage recMsg = null;
                GrammarNonDeterminismMessage nondetMsg =
                    getNonDeterminismMessage( equeue.warnings );
                IList<int> nonDetAlts = null;
                if ( nondetMsg != null )
                {
                    nonDetAlts =
                        nondetMsg.probe.GetNonDeterministicAltsForState( nondetMsg.problemState );
                }
                else
                {
                    recMsg = getRecursionOverflowMessage( equeue.warnings );
                    if ( recMsg != null )
                    {
                        //nonDetAlts = new ArrayList(recMsg.alts);
                    }
                }
                // compare nonDetAlts with expectingNonDetAlts
                BitSet s = new BitSet();
                s.AddAll( expectingNonDetAlts );
                BitSet s2 = new BitSet();
                s2.AddAll( nonDetAlts );
                Assert.AreEqual(s, s2, "nondet alts mismatch");
                Assert.IsTrue(nondetMsg != null || recMsg != null, "found no nondet alts; expecting: " + str(expectingNonDetAlts));
            }
            else
            {
                // not expecting any nondet alts, make sure there are none
                GrammarNonDeterminismMessage nondetMsg =
                    getNonDeterminismMessage( equeue.warnings );
                Assert.IsNull(nondetMsg, "found nondet alts, but expecting none");
            }

            Assert.AreEqual( expecting, result );
        }

        protected GrammarNonDeterminismMessage getNonDeterminismMessage( IList warnings )
        {
            for ( int i = 0; i < warnings.Count; i++ )
            {
                Message m = (Message)warnings[i];
                if ( m is GrammarNonDeterminismMessage )
                {
                    return (GrammarNonDeterminismMessage)m;
                }
            }
            return null;
        }

        protected NonRegularDecisionMessage getNonRegularDecisionMessage( IList errors )
        {
            for ( int i = 0; i < errors.Count; i++ )
            {
                Message m = (Message)errors[i];
                if ( m is NonRegularDecisionMessage )
                {
                    return (NonRegularDecisionMessage)m;
                }
            }
            return null;
        }

        protected RecursionOverflowMessage getRecursionOverflowMessage( IList warnings )
        {
            for ( int i = 0; i < warnings.Count; i++ )
            {
                Message m = (Message)warnings[i];
                if ( m is RecursionOverflowMessage )
                {
                    return (RecursionOverflowMessage)m;
                }
            }
            return null;
        }

        protected LeftRecursionCyclesMessage getLeftRecursionCyclesMessage( IList warnings )
        {
            for ( int i = 0; i < warnings.Count; i++ )
            {
                Message m = (Message)warnings[i];
                if ( m is LeftRecursionCyclesMessage )
                {
                    return (LeftRecursionCyclesMessage)m;
                }
            }
            return null;
        }

        protected GrammarDanglingStateMessage getDanglingStateMessage( IList warnings )
        {
            for ( int i = 0; i < warnings.Count; i++ )
            {
                Message m = (Message)warnings[i];
                if ( m is GrammarDanglingStateMessage )
                {
                    return (GrammarDanglingStateMessage)m;
                }
            }
            return null;
        }

        protected string str( int[] elements )
        {
            StringBuilder buf = new StringBuilder();
            for ( int i = 0; i < elements.Length; i++ )
            {
                if ( i > 0 )
                {
                    buf.Append( ", " );
                }
                int element = elements[i];
                buf.Append( element );
            }
            return buf.ToString();
        }

        protected HashSet<string> ruleNames( System.Collections.IEnumerable rules )
        {
            HashSet<string> x = new HashSet<string>();
            foreach ( Rule r in rules )
            {
                x.Add( r.Name );
            }
            return x;
        }

        protected HashSet<string> ruleNames2( System.Collections.IEnumerable rules )
        {
            HashSet<string> x = new HashSet<string>();
            foreach ( System.Collections.IEnumerable s in rules )
            {
                x.addAll( ruleNames( s ) );
            }
            return x;
        }
    }
}
