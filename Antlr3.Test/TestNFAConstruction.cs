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

    using FASerializer = Antlr3.Tool.FASerializer;
    using Grammar = Antlr3.Tool.Grammar;
    using State = Antlr3.Analysis.State;

    [TestClass]
    public class TestNFAConstruction : BaseTest
    {

        /** Public default constructor used by TestRig */
        public TestNFAConstruction()
        {
        }

        [TestMethod]
        public void TestA() /*throws Exception*/ {
            Grammar g = new Grammar(
                "parser grammar P;\n" +
                "a : A;" );
            string expecting =
                ".s0->.s1" + NewLine +
                ".s1->.s2" + NewLine +
                ".s2-A->.s3" + NewLine +
                ".s3->:s4" + NewLine +
                ":s4-EOF->.s5" + NewLine;
            checkRule( g, "a", expecting );
        }

        [TestMethod]
        public void TestAB() /*throws Exception*/ {
            Grammar g = new Grammar(
                "parser grammar P;\n" +
                "a : A B ;" );
            string expecting =
                ".s0->.s1" + NewLine +
                ".s1->.s2" + NewLine +
                ".s2-A->.s3" + NewLine +
                ".s3-B->.s4" + NewLine +
                ".s4->:s5" + NewLine +
                ":s5-EOF->.s6" + NewLine;
            checkRule( g, "a", expecting );
        }

        [TestMethod]
        public void TestAorB() /*throws Exception*/ {
            Grammar g = new Grammar(
                "parser grammar P;\n" +
                "a : A | B {;} ;" );
            /* expecting (0)--Ep-->(1)--Ep-->(2)--A-->(3)--Ep-->(4)--Ep-->(5,end)
                                            |                            ^
                                           (6)--Ep-->(7)--B-->(8)--------|
                     */
            string expecting =
                ".s0->.s1" + NewLine +
                ".s1->.s2" + NewLine +
                ".s1->.s7" + NewLine +
                ".s10->.s4" + NewLine +
                ".s2-A->.s3" + NewLine +
                ".s3->.s4" + NewLine +
                ".s4->:s5" + NewLine +
                ".s7->.s8" + NewLine +
                ".s8-B->.s9" + NewLine +
                ".s9-{}->.s10" + NewLine +
                ":s5-EOF->.s6" + NewLine;
            checkRule( g, "a", expecting );
        }

        [TestMethod]
        public void TestRangeOrRange() /*throws Exception*/ {
            Grammar g = new Grammar(
                "lexer grammar P;\n" +
                "A : ('a'..'c' 'h' | 'q' 'j'..'l') ;"
            );
            string expecting =
                ".s0->.s1" + NewLine +
                ".s1->.s2" + NewLine +
                ".s10-'q'->.s11" + NewLine +
                ".s11-'j'..'l'->.s12" + NewLine +
                ".s12->.s6" + NewLine +
                ".s2->.s3" + NewLine +
                ".s2->.s9" + NewLine +
                ".s3-'a'..'c'->.s4" + NewLine +
                ".s4-'h'->.s5" + NewLine +
                ".s5->.s6" + NewLine +
                ".s6->:s7" + NewLine +
                ".s9->.s10" + NewLine +
                ":s7-<EOT>->.s8" + NewLine;
            checkRule( g, "A", expecting );
        }

        [TestMethod]
        public void TestRange() /*throws Exception*/ {
            Grammar g = new Grammar(
                "lexer grammar P;\n" +
                "A : 'a'..'c' ;"
            );
            string expecting =
                ".s0->.s1" + NewLine +
                ".s1->.s2" + NewLine +
                ".s2-'a'..'c'->.s3" + NewLine +
                ".s3->:s4" + NewLine +
                ":s4-<EOT>->.s5" + NewLine;
            checkRule( g, "A", expecting );
        }

        [TestMethod]
        public void TestCharSetInParser() /*throws Exception*/ {
            Grammar g = new Grammar(
                "grammar P;\n" +
                "a : A|'b' ;"
            );
            string expecting =
                ".s0->.s1" + NewLine +
                ".s1->.s2" + NewLine +
                ".s2-A..'b'->.s3" + NewLine +
                ".s3->:s4" + NewLine +
                ":s4-EOF->.s5" + NewLine;
            checkRule( g, "a", expecting );
        }

        [TestMethod]
        public void TestABorCD() /*throws Exception*/ {
            Grammar g = new Grammar(
                "parser grammar P;\n" +
                "a : A B | C D;" );
            string expecting =
                ".s0->.s1" + NewLine +
                ".s1->.s2" + NewLine +
                ".s1->.s8" + NewLine +
                ".s10-D->.s11" + NewLine +
                ".s11->.s5" + NewLine +
                ".s2-A->.s3" + NewLine +
                ".s3-B->.s4" + NewLine +
                ".s4->.s5" + NewLine +
                ".s5->:s6" + NewLine +
                ".s8->.s9" + NewLine +
                ".s9-C->.s10" + NewLine +
                ":s6-EOF->.s7" + NewLine;
            checkRule( g, "a", expecting );
        }

        [TestMethod]
        public void TestbA() /*throws Exception*/ {
            Grammar g = new Grammar(
                "parser grammar P;\n" +
                "a : b A ;\n" +
                "b : B ;" );
            string expecting =
                ".s0->.s1" + NewLine +
                ".s1->.s2" + NewLine +
                ".s2->.s3" + NewLine +
                ".s3->.s4" + NewLine +
                ".s4->.s5" + NewLine +
                ".s5-B->.s6" + NewLine +
                ".s6->:s7" + NewLine +
                ".s8-A->.s9" + NewLine +
                ".s9->:s10" + NewLine +
                ":s10-EOF->.s11" + NewLine +
                ":s7->.s8" + NewLine;
            checkRule( g, "a", expecting );
        }

        [TestMethod]
        public void TestbA_bC() /*throws Exception*/ {
            Grammar g = new Grammar(
                "parser grammar P;\n" +
                "a : b A ;\n" +
                "b : B ;\n" +
                "c : b C;" );
            string expecting =
                ".s0->.s1" + NewLine +
                ".s1->.s2" + NewLine +
                ".s12->.s13" + NewLine +
                ".s13-C->.s14" + NewLine +
                ".s14->:s15" + NewLine +
                ".s2->.s3" + NewLine +
                ".s3->.s4" + NewLine +
                ".s4->.s5" + NewLine +
                ".s5-B->.s6" + NewLine +
                ".s6->:s7" + NewLine +
                ".s8-A->.s9" + NewLine +
                ".s9->:s10" + NewLine +
                ":s10-EOF->.s11" + NewLine +
                ":s15-EOF->.s16" + NewLine +
                ":s7->.s12" + NewLine +
                ":s7->.s8" + NewLine;
            checkRule( g, "a", expecting );
        }

        [TestMethod]
        public void TestAorEpsilon() /*throws Exception*/ {
            Grammar g = new Grammar(
                "parser grammar P;\n" +
                "a : A | ;" );
            /* expecting (0)--Ep-->(1)--Ep-->(2)--A-->(3)--Ep-->(4)--Ep-->(5,end)
                                            |                            ^
                                           (6)--Ep-->(7)--Ep-->(8)-------|
                     */
            string expecting =
                ".s0->.s1" + NewLine +
                ".s1->.s2" + NewLine +
                ".s1->.s7" + NewLine +
                ".s2-A->.s3" + NewLine +
                ".s3->.s4" + NewLine +
                ".s4->:s5" + NewLine +
                ".s7->.s8" + NewLine +
                ".s8->.s9" + NewLine +
                ".s9->.s4" + NewLine +
                ":s5-EOF->.s6" + NewLine;
            checkRule( g, "a", expecting );
        }

        [TestMethod]
        public void TestAOptional() /*throws Exception*/ {
            Grammar g = new Grammar(
                "parser grammar P;\n" +
                "a : (A)?;" );
            string expecting =
                ".s0->.s1" + NewLine +
                ".s1->.s2" + NewLine +
                ".s2->.s3" + NewLine +
                ".s2->.s8" + NewLine +
                ".s3-A->.s4" + NewLine +
                ".s4->.s5" + NewLine +
                ".s5->:s6" + NewLine +
                ".s8->.s5" + NewLine +
                ":s6-EOF->.s7" + NewLine;
            checkRule( g, "a", expecting );
        }

        [TestMethod]
        public void TestNakedAoptional() /*throws Exception*/ {
            Grammar g = new Grammar(
                "parser grammar P;\n" +
                "a : A?;" );
            string expecting =
                ".s0->.s1" + NewLine +
                ".s1->.s2" + NewLine +
                ".s2->.s3" + NewLine +
                ".s2->.s8" + NewLine +
                ".s3-A->.s4" + NewLine +
                ".s4->.s5" + NewLine +
                ".s5->:s6" + NewLine +
                ".s8->.s5" + NewLine +
                ":s6-EOF->.s7" + NewLine;
            checkRule( g, "a", expecting );
        }

        [TestMethod]
        public void TestAorBthenC() /*throws Exception*/ {
            Grammar g = new Grammar(
                "parser grammar P;\n" +
                "a : (A | B) C;" );
            /* expecting

                    (0)--Ep-->(1)--Ep-->(2)--A-->(3)--Ep-->(4)--Ep-->(5)--C-->(6)--Ep-->(7,end)
                               |                            ^
                              (8)--Ep-->(9)--B-->(10)-------|
                     */
        }

        [TestMethod]
        public void TestAplus() /*throws Exception*/ {
            Grammar g = new Grammar(
                "parser grammar P;\n" +
                "a : (A)+;" );
            string expecting =
                ".s0->.s1" + NewLine +
                ".s1->.s2" + NewLine +
                ".s2->.s3" + NewLine +
                ".s3->.s4" + NewLine +
                ".s4-A->.s5" + NewLine +
                ".s5->.s3" + NewLine +
                ".s5->.s6" + NewLine +
                ".s6->:s7" + NewLine +
                ":s7-EOF->.s8" + NewLine;
            checkRule( g, "a", expecting );
        }

        [TestMethod]
        public void TestNakedAplus() /*throws Exception*/ {
            Grammar g = new Grammar(
                "parser grammar P;\n" +
                "a : A+;" );
            string expecting =
                ".s0->.s1" + NewLine +
                ".s1->.s2" + NewLine +
                ".s2->.s3" + NewLine +
                ".s3->.s4" + NewLine +
                ".s4-A->.s5" + NewLine +
                ".s5->.s3" + NewLine +
                ".s5->.s6" + NewLine +
                ".s6->:s7" + NewLine +
                ":s7-EOF->.s8" + NewLine;
            checkRule( g, "a", expecting );
        }

        [TestMethod]
        public void TestAplusNonGreedy() /*throws Exception*/ {
            Grammar g = new Grammar(
                "lexer grammar t;\n" +
                "A : (options {greedy=false;}:'0'..'9')+ ;\n" );
            string expecting =
                ".s0->.s1" + NewLine +
                ".s1->.s2" + NewLine +
                ".s2->.s3" + NewLine +
                ".s3->.s4" + NewLine +
                ".s4-'0'..'9'->.s5" + NewLine +
                ".s5->.s3" + NewLine +
                ".s5->.s6" + NewLine +
                ".s6->:s7" + NewLine +
                ":s7-<EOT>->.s8" + NewLine;
            checkRule( g, "A", expecting );
        }

        [TestMethod]
        public void TestAorBplus() /*throws Exception*/ {
            Grammar g = new Grammar(
                "parser grammar P;\n" +
                "a : (A | B{action})+ ;" );
            string expecting =
                ".s0->.s1" + NewLine +
                ".s1->.s2" + NewLine +
                ".s10->.s11" + NewLine +
                ".s11-B->.s12" + NewLine +
                ".s12-{}->.s13" + NewLine +
                ".s13->.s6" + NewLine +
                ".s2->.s3" + NewLine +
                ".s3->.s10" + NewLine +
                ".s3->.s4" + NewLine +
                ".s4-A->.s5" + NewLine +
                ".s5->.s6" + NewLine +
                ".s6->.s3" + NewLine +
                ".s6->.s7" + NewLine +
                ".s7->:s8" + NewLine +
                ":s8-EOF->.s9" + NewLine;
            checkRule( g, "a", expecting );
        }

        [TestMethod]
        public void TestAorBorEmptyPlus() /*throws Exception*/ {
            Grammar g = new Grammar(
                "parser grammar P;\n" +
                "a : (A | B | )+ ;" );
            string expecting =
                ".s0->.s1" + NewLine +
                ".s1->.s2" + NewLine +
                ".s10->.s11" + NewLine +
                ".s10->.s13" + NewLine +
                ".s11-B->.s12" + NewLine +
                ".s12->.s6" + NewLine +
                ".s13->.s14" + NewLine +
                ".s14->.s15" + NewLine +
                ".s15->.s6" + NewLine +
                ".s2->.s3" + NewLine +
                ".s3->.s10" + NewLine +
                ".s3->.s4" + NewLine +
                ".s4-A->.s5" + NewLine +
                ".s5->.s6" + NewLine +
                ".s6->.s3" + NewLine +
                ".s6->.s7" + NewLine +
                ".s7->:s8" + NewLine +
                ":s8-EOF->.s9" + NewLine;
            checkRule( g, "a", expecting );
        }

        [TestMethod]
        public void TestAStar() /*throws Exception*/ {
            Grammar g = new Grammar(
                "parser grammar P;\n" +
                "a : (A)*;" );
            string expecting =
                ".s0->.s1" + NewLine +
                ".s1->.s2" + NewLine +
                ".s2->.s3" + NewLine +
                ".s2->.s9" + NewLine +
                ".s3->.s4" + NewLine +
                ".s4-A->.s5" + NewLine +
                ".s5->.s3" + NewLine +
                ".s5->.s6" + NewLine +
                ".s6->:s7" + NewLine +
                ".s9->.s6" + NewLine +
                ":s7-EOF->.s8" + NewLine;
            checkRule( g, "a", expecting );
        }

        [TestMethod]
        public void TestNestedAstar() /*throws Exception*/ {
            Grammar g = new Grammar(
                "parser grammar P;\n" +
                "a : (A*)*;" );
            string expecting =
                ".s0->.s1" + NewLine +
                ".s1->.s2" + NewLine +
                ".s10->:s11" + NewLine +
                ".s13->.s8" + NewLine +
                ".s14->.s10" + NewLine +
                ".s2->.s14" + NewLine +
                ".s2->.s3" + NewLine +
                ".s3->.s4" + NewLine +
                ".s4->.s13" + NewLine +
                ".s4->.s5" + NewLine +
                ".s5->.s6" + NewLine +
                ".s6-A->.s7" + NewLine +
                ".s7->.s5" + NewLine +
                ".s7->.s8" + NewLine +
                ".s8->.s9" + NewLine +
                ".s9->.s10" + NewLine +
                ".s9->.s3" + NewLine +
                ":s11-EOF->.s12" + NewLine;
            checkRule( g, "a", expecting );
        }

        [TestMethod]
        public void TestPlusNestedInStar() /*throws Exception*/ {
            Grammar g = new Grammar(
                "parser grammar P;\n" +
                "a : (A+)*;" );
            string expecting =
                ".s0->.s1" + NewLine +
                ".s1->.s2" + NewLine +
                ".s10->:s11" + NewLine +
                ".s13->.s10" + NewLine +
                ".s2->.s13" + NewLine +
                ".s2->.s3" + NewLine +
                ".s3->.s4" + NewLine +
                ".s4->.s5" + NewLine +
                ".s5->.s6" + NewLine +
                ".s6-A->.s7" + NewLine +
                ".s7->.s5" + NewLine +
                ".s7->.s8" + NewLine +
                ".s8->.s9" + NewLine +
                ".s9->.s10" + NewLine +
                ".s9->.s3" + NewLine +
                ":s11-EOF->.s12" + NewLine;
            checkRule( g, "a", expecting );
        }

        [TestMethod]
        public void TestStarNestedInPlus() /*throws Exception*/ {
            Grammar g = new Grammar(
                "parser grammar P;\n" +
                "a : (A*)+;" );
            string expecting =
                ".s0->.s1" + NewLine +
                ".s1->.s2" + NewLine +
                ".s10->:s11" + NewLine +
                ".s13->.s8" + NewLine +
                ".s2->.s3" + NewLine +
                ".s3->.s4" + NewLine +
                ".s4->.s13" + NewLine +
                ".s4->.s5" + NewLine +
                ".s5->.s6" + NewLine +
                ".s6-A->.s7" + NewLine +
                ".s7->.s5" + NewLine +
                ".s7->.s8" + NewLine +
                ".s8->.s9" + NewLine +
                ".s9->.s10" + NewLine +
                ".s9->.s3" + NewLine +
                ":s11-EOF->.s12" + NewLine;
            checkRule( g, "a", expecting );
        }

        [TestMethod]
        public void TestNakedAstar() /*throws Exception*/ {
            Grammar g = new Grammar(
                "parser grammar P;\n" +
                "a : A*;" );
            string expecting =
                ".s0->.s1" + NewLine +
                ".s1->.s2" + NewLine +
                ".s2->.s3" + NewLine +
                ".s2->.s9" + NewLine +
                ".s3->.s4" + NewLine +
                ".s4-A->.s5" + NewLine +
                ".s5->.s3" + NewLine +
                ".s5->.s6" + NewLine +
                ".s6->:s7" + NewLine +
                ".s9->.s6" + NewLine +
                ":s7-EOF->.s8" + NewLine;
            checkRule( g, "a", expecting );
        }

        [TestMethod]
        public void TestAorBstar() /*throws Exception*/ {
            Grammar g = new Grammar(
                "parser grammar P;\n" +
                "a : (A | B{action})* ;" );
            string expecting =
                ".s0->.s1" + NewLine +
                ".s1->.s2" + NewLine +
                ".s10->.s11" + NewLine +
                ".s11-B->.s12" + NewLine +
                ".s12-{}->.s13" + NewLine +
                ".s13->.s6" + NewLine +
                ".s14->.s7" + NewLine +
                ".s2->.s14" + NewLine +
                ".s2->.s3" + NewLine +
                ".s3->.s10" + NewLine +
                ".s3->.s4" + NewLine +
                ".s4-A->.s5" + NewLine +
                ".s5->.s6" + NewLine +
                ".s6->.s3" + NewLine +
                ".s6->.s7" + NewLine +
                ".s7->:s8" + NewLine +
                ":s8-EOF->.s9" + NewLine;
            checkRule( g, "a", expecting );
        }

        [TestMethod]
        public void TestAorBOptionalSubrule() /*throws Exception*/ {
            Grammar g = new Grammar(
                "parser grammar P;\n" +
                "a : ( A | B )? ;" );
            string expecting =
                ".s0->.s1" + NewLine +
                ".s1->.s2" + NewLine +
                ".s2->.s3" + NewLine +
                ".s2->.s8" + NewLine +
                ".s3-A..B->.s4" + NewLine +
                ".s4->.s5" + NewLine +
                ".s5->:s6" + NewLine +
                ".s8->.s5" + NewLine +
                ":s6-EOF->.s7" + NewLine;
            checkRule( g, "a", expecting );
        }

        [TestMethod]
        public void TestPredicatedAorB() /*throws Exception*/ {
            Grammar g = new Grammar(
                "parser grammar P;\n" +
                "a : {p1}? A | {p2}? B ;" );
            string expecting =
                ".s0->.s1" + NewLine +
                ".s1->.s2" + NewLine +
                ".s1->.s8" + NewLine +
                ".s10-B->.s11" + NewLine +
                ".s11->.s5" + NewLine +
                ".s2-{p1}?->.s3" + NewLine +
                ".s3-A->.s4" + NewLine +
                ".s4->.s5" + NewLine +
                ".s5->:s6" + NewLine +
                ".s8->.s9" + NewLine +
                ".s9-{p2}?->.s10" + NewLine +
                ":s6-EOF->.s7" + NewLine;
            checkRule( g, "a", expecting );
        }

        [TestMethod]
        public void TestMultiplePredicates() /*throws Exception*/ {
            Grammar g = new Grammar(
                "parser grammar P;\n" +
                "a : {p1}? {p1a}? A | {p2}? B | {p3} b;\n" +
                "b : {p4}? B ;" );
            string expecting =
                ".s0->.s1" + NewLine +
                ".s1->.s2" + NewLine +
                ".s1->.s9" + NewLine +
                ".s10-{p2}?->.s11" + NewLine +
                ".s11-B->.s12" + NewLine +
                ".s12->.s6" + NewLine +
                ".s13->.s14" + NewLine +
                ".s14-{}->.s15" + NewLine +
                ".s15->.s16" + NewLine +
                ".s16->.s17" + NewLine +
                ".s17->.s18" + NewLine +
                ".s18-{p4}?->.s19" + NewLine +
                ".s19-B->.s20" + NewLine +
                ".s2-{p1}?->.s3" + NewLine +
                ".s20->:s21" + NewLine +
                ".s22->.s6" + NewLine +
                ".s3-{p1a}?->.s4" + NewLine +
                ".s4-A->.s5" + NewLine +
                ".s5->.s6" + NewLine +
                ".s6->:s7" + NewLine +
                ".s9->.s10" + NewLine +
                ".s9->.s13" + NewLine +
                ":s21->.s22" + NewLine +
                ":s7-EOF->.s8" + NewLine;
            checkRule( g, "a", expecting );
        }

        [TestMethod]
        public void TestSets() /*throws Exception*/ {
            Grammar g = new Grammar(
                "parser grammar P;\n" +
                "a : ( A | B )+ ;\n" +
                "b : ( A | B{;} )+ ;\n" +
                "c : (A|B) (A|B) ;\n" +
                "d : ( A | B )* ;\n" +
                "e : ( A | B )? ;" );
            string expecting =
                ".s0->.s1" + NewLine +
                ".s1->.s2" + NewLine +
                ".s2->.s3" + NewLine +
                ".s3->.s4" + NewLine +
                ".s4-A..B->.s5" + NewLine +
                ".s5->.s3" + NewLine +
                ".s5->.s6" + NewLine +
                ".s6->:s7" + NewLine +
                ":s7-EOF->.s8" + NewLine;
            checkRule( g, "a", expecting );
            expecting =
                ".s0->.s1" + NewLine +
                ".s1->.s2" + NewLine +
                ".s10->.s11" + NewLine +
                ".s11-B->.s12" + NewLine +
                ".s12-{}->.s13" + NewLine +
                ".s13->.s6" + NewLine +
                ".s2->.s3" + NewLine +
                ".s3->.s10" + NewLine +
                ".s3->.s4" + NewLine +
                ".s4-A->.s5" + NewLine +
                ".s5->.s6" + NewLine +
                ".s6->.s3" + NewLine +
                ".s6->.s7" + NewLine +
                ".s7->:s8" + NewLine +
                ":s8-EOF->.s9" + NewLine;
            checkRule( g, "b", expecting );
            expecting =
                ".s0->.s1" + NewLine +
                ".s1->.s2" + NewLine +
                ".s2-A..B->.s3" + NewLine +
                ".s3-A..B->.s4" + NewLine +
                ".s4->:s5" + NewLine +
                ":s5-EOF->.s6" + NewLine;
            checkRule( g, "c", expecting );
            expecting =
                ".s0->.s1" + NewLine +
                ".s1->.s2" + NewLine +
                ".s2->.s3" + NewLine +
                ".s2->.s9" + NewLine +
                ".s3->.s4" + NewLine +
                ".s4-A..B->.s5" + NewLine +
                ".s5->.s3" + NewLine +
                ".s5->.s6" + NewLine +
                ".s6->:s7" + NewLine +
                ".s9->.s6" + NewLine +
                ":s7-EOF->.s8" + NewLine;
            checkRule( g, "d", expecting );
            expecting =
                ".s0->.s1" + NewLine +
                ".s1->.s2" + NewLine +
                ".s2->.s3" + NewLine +
                ".s2->.s8" + NewLine +
                ".s3-A..B->.s4" + NewLine +
                ".s4->.s5" + NewLine +
                ".s5->:s6" + NewLine +
                ".s8->.s5" + NewLine +
                ":s6-EOF->.s7" + NewLine;
            checkRule( g, "e", expecting );
        }

        [TestMethod]
        public void TestNotSet() /*throws Exception*/ {
            Grammar g = new Grammar(
                "parser grammar P;\n" +
                "tokens { A; B; C; }\n" +
                "a : ~A ;\n" );
            string expecting =
                ".s0->.s1" + NewLine +
                ".s1->.s2" + NewLine +
                ".s2-B..C->.s3" + NewLine +
                ".s3->:s4" + NewLine +
                ":s4-EOF->.s5" + NewLine;
            checkRule( g, "a", expecting );

            string expectingGrammarStr =
                "1:8: parser grammar P;\n" +
                "a : ~ A ;";
            Assert.AreEqual( expectingGrammarStr, g.ToString() );
        }

        [TestMethod]
        public void TestNotSingletonBlockSet() /*throws Exception*/ {
            Grammar g = new Grammar(
                "parser grammar P;\n" +
                "tokens { A; B; C; }\n" +
                "a : ~(A) ;\n" );
            string expecting =
                ".s0->.s1" + NewLine +
                ".s1->.s2" + NewLine +
                ".s2-B..C->.s3" + NewLine +
                ".s3->:s4" + NewLine +
                ":s4-EOF->.s5" + NewLine;
            checkRule( g, "a", expecting );

            string expectingGrammarStr =
                "1:8: parser grammar P;\n" +
                "a : ~ ( A ) ;";
            Assert.AreEqual( expectingGrammarStr, g.ToString() );
        }

        [TestMethod]
        public void TestNotCharSet() /*throws Exception*/ {
            Grammar g = new Grammar(
                "lexer grammar P;\n" +
                "A : ~'3' ;\n" );
            string expecting =
                ".s0->.s1" + NewLine +
                ".s1->.s2" + NewLine +
                ".s2-{'\\u0000'..'2', '4'..'\\uFFFF'}->.s3" + NewLine +
                ".s3->:s4" + NewLine +
                ":s4-<EOT>->.s5" + NewLine;
            checkRule( g, "A", expecting );

            string expectingGrammarStr =
                "1:7: lexer grammar P;\n" +
                "A : ~ '3' ;\n" +
                "Tokens : A ;";
            Assert.AreEqual( expectingGrammarStr, g.ToString() );
        }

        [TestMethod]
        public void TestNotBlockSet() /*throws Exception*/ {
            Grammar g = new Grammar(
                "lexer grammar P;\n" +
                "A : ~('3'|'b') ;\n" );
            string expecting =
                ".s0->.s1" + NewLine +
                ".s1->.s2" + NewLine +
                ".s2-{'\\u0000'..'2', '4'..'a', 'c'..'\\uFFFF'}->.s3" + NewLine +
                ".s3->:s4" + NewLine +
                ":s4-<EOT>->.s5" + NewLine;
            checkRule( g, "A", expecting );

            string expectingGrammarStr =
                "1:7: lexer grammar P;\n" +
                "A : ~ ( '3' | 'b' ) ;\n" +
                "Tokens : A ;";
            Assert.AreEqual( expectingGrammarStr, g.ToString() );
        }

        [TestMethod]
        public void TestNotSetLoop() /*throws Exception*/ {
            Grammar g = new Grammar(
                "lexer grammar P;\n" +
                "A : ~('3')* ;\n" );
            string expecting =
                ".s0->.s1" + NewLine +
                ".s1->.s2" + NewLine +
                ".s2->.s3" + NewLine +
                ".s2->.s9" + NewLine +
                ".s3->.s4" + NewLine +
                ".s4-{'\\u0000'..'2', '4'..'\\uFFFF'}->.s5" + NewLine +
                ".s5->.s3" + NewLine +
                ".s5->.s6" + NewLine +
                ".s6->:s7" + NewLine +
                ".s9->.s6" + NewLine +
                ":s7-<EOT>->.s8" + NewLine;
            checkRule( g, "A", expecting );

            string expectingGrammarStr =
                "1:7: lexer grammar P;\n" +
                "A : (~ ( '3' ) )* ;\n" +
                "Tokens : A ;";
            Assert.AreEqual( expectingGrammarStr, g.ToString() );
        }

        [TestMethod]
        public void TestNotBlockSetLoop() /*throws Exception*/ {
            Grammar g = new Grammar(
                "lexer grammar P;\n" +
                "A : ~('3'|'b')* ;\n" );
            string expecting =
                ".s0->.s1" + NewLine +
                ".s1->.s2" + NewLine +
                ".s2->.s3" + NewLine +
                ".s2->.s9" + NewLine +
                ".s3->.s4" + NewLine +
                ".s4-{'\\u0000'..'2', '4'..'a', 'c'..'\\uFFFF'}->.s5" + NewLine +
                ".s5->.s3" + NewLine +
                ".s5->.s6" + NewLine +
                ".s6->:s7" + NewLine +
                ".s9->.s6" + NewLine +
                ":s7-<EOT>->.s8" + NewLine;
            checkRule( g, "A", expecting );

            string expectingGrammarStr =
                "1:7: lexer grammar P;\n" +
                "A : (~ ( '3' | 'b' ) )* ;\n" +
                "Tokens : A ;";
            Assert.AreEqual( expectingGrammarStr, g.ToString() );
        }

        [TestMethod]
        public void TestSetsInCombinedGrammarSentToLexer() /*throws Exception*/ {
            // not sure this belongs in this test suite, but whatever.
            Grammar g = new Grammar(
                "grammar t;\n" +
                "A : '{' ~('}')* '}';\n" );
            string result = g.GetLexerGrammar();
            string expecting =
                "lexer grammar t;" + NewLine +
                "" + NewLine +
                "// $ANTLR src \"<string>\" 2" + NewLine +
                "A : '{' ~('}')* '}';";
            Assert.AreEqual( expecting, result );
        }

        [TestMethod]
        public void TestLabeledNotSet() /*throws Exception*/ {
            Grammar g = new Grammar(
                "parser grammar P;\n" +
                "tokens { A; B; C; }\n" +
                "a : t=~A ;\n" );
            string expecting =
                ".s0->.s1" + NewLine +
                ".s1->.s2" + NewLine +
                ".s2-B..C->.s3" + NewLine +
                ".s3->:s4" + NewLine +
                ":s4-EOF->.s5" + NewLine;
            checkRule( g, "a", expecting );

            string expectingGrammarStr =
                "1:8: parser grammar P;\n" +
                "a : t=~ A ;";
            Assert.AreEqual( expectingGrammarStr, g.ToString() );
        }

        [TestMethod]
        public void TestLabeledNotCharSet() /*throws Exception*/ {
            Grammar g = new Grammar(
                "lexer grammar P;\n" +
                "A : t=~'3' ;\n" );
            string expecting =
                ".s0->.s1" + NewLine +
                ".s1->.s2" + NewLine +
                ".s2-{'\\u0000'..'2', '4'..'\\uFFFF'}->.s3" + NewLine +
                ".s3->:s4" + NewLine +
                ":s4-<EOT>->.s5" + NewLine;
            checkRule( g, "A", expecting );

            string expectingGrammarStr =
                "1:7: lexer grammar P;\n" +
                "A : t=~ '3' ;\n" +
                "Tokens : A ;";
            Assert.AreEqual( expectingGrammarStr, g.ToString() );
        }

        [TestMethod]
        public void TestLabeledNotBlockSet() /*throws Exception*/ {
            Grammar g = new Grammar(
                "lexer grammar P;\n" +
                "A : t=~('3'|'b') ;\n" );
            string expecting =
                ".s0->.s1" + NewLine +
                ".s1->.s2" + NewLine +
                ".s2-{'\\u0000'..'2', '4'..'a', 'c'..'\\uFFFF'}->.s3" + NewLine +
                ".s3->:s4" + NewLine +
                ":s4-<EOT>->.s5" + NewLine;
            checkRule( g, "A", expecting );

            string expectingGrammarStr =
                "1:7: lexer grammar P;\n" +
                "A : t=~ ( '3' | 'b' ) ;\n" +
                "Tokens : A ;";
            Assert.AreEqual( expectingGrammarStr, g.ToString() );
        }

        [TestMethod]
        public void TestEscapedCharLiteral() /*throws Exception*/ {
            Grammar g = new Grammar(
                "grammar P;\n" +
                "a : '\\n';" );
            string expecting =
                ".s0->.s1" + NewLine +
                ".s1->.s2" + NewLine +
                ".s2-'\\n'->.s3" + NewLine +
                ".s3->:s4" + NewLine +
                ":s4-EOF->.s5" + NewLine;
            checkRule( g, "a", expecting );
        }

        [TestMethod]
        public void TestEscapedStringLiteral() /*throws Exception*/ {
            Grammar g = new Grammar(
                "grammar P;\n" +
                "a : 'a\\nb\\u0030c\\'';" );
            string expecting =
                ".s0->.s1" + NewLine +
                ".s1->.s2" + NewLine +
                ".s2-'a\\nb\\u0030c\\''->.s3" + NewLine +
                ".s3->:s4" + NewLine +
                ":s4-EOF->.s5" + NewLine;
            checkRule( g, "a", expecting );
        }

        // AUTO BACKTRACKING STUFF

        [TestMethod]
        public void TestAutoBacktracking_RuleBlock() /*throws Exception*/ {
            Grammar g = new Grammar(
                "grammar t;\n" +
                "options {backtrack=true;}\n" +
                "a : 'a'{;}|'b';"
            );
            string expecting =
                ".s0->.s1" + NewLine +
                ".s1->.s2" + NewLine +
                ".s1->.s9" + NewLine +
                ".s10-'b'->.s11" + NewLine +
                ".s11->.s6" + NewLine +
                ".s2-{synpred1_t}?->.s3" + NewLine +
                ".s3-'a'->.s4" + NewLine +
                ".s4-{}->.s5" + NewLine +
                ".s5->.s6" + NewLine +
                ".s6->:s7" + NewLine +
                ".s9->.s10" + NewLine +
                ":s7-EOF->.s8" + NewLine;
            checkRule( g, "a", expecting );
        }

        [TestMethod]
        public void TestAutoBacktracking_RuleSetBlock() /*throws Exception*/ {
            Grammar g = new Grammar(
                "grammar t;\n" +
                "options {backtrack=true;}\n" +
                "a : 'a'|'b';"
            );
            string expecting =
                ".s0->.s1" + NewLine +
                ".s1->.s2" + NewLine +
                ".s2-'a'..'b'->.s3" + NewLine +
                ".s3->:s4" + NewLine +
                ":s4-EOF->.s5" + NewLine;
            checkRule( g, "a", expecting );
        }

        [TestMethod]
        public void TestAutoBacktracking_SimpleBlock() /*throws Exception*/ {
            Grammar g = new Grammar(
                "grammar t;\n" +
                "options {backtrack=true;}\n" +
                "a : ('a'{;}|'b') ;"
            );
            string expecting =
                ".s0->.s1" + NewLine +
                ".s1->.s2" + NewLine +
                ".s10->.s11" + NewLine +
                ".s11-'b'->.s12" + NewLine +
                ".s12->.s7" + NewLine +
                ".s2->.s10" + NewLine +
                ".s2->.s3" + NewLine +
                ".s3-{synpred1_t}?->.s4" + NewLine +
                ".s4-'a'->.s5" + NewLine +
                ".s5-{}->.s6" + NewLine +
                ".s6->.s7" + NewLine +
                ".s7->:s8" + NewLine +
                ":s8-EOF->.s9" + NewLine;
            checkRule( g, "a", expecting );
        }

        [TestMethod]
        public void TestAutoBacktracking_SetBlock() /*throws Exception*/ {
            Grammar g = new Grammar(
                "grammar t;\n" +
                "options {backtrack=true;}\n" +
                "a : ('a'|'b') ;"
            );
            string expecting =
                ".s0->.s1" + NewLine +
                ".s1->.s2" + NewLine +
                ".s2-'a'..'b'->.s3" + NewLine +
                ".s3->:s4" + NewLine +
                ":s4-EOF->.s5" + NewLine;
            checkRule( g, "a", expecting );
        }

        [TestMethod]
        public void TestAutoBacktracking_StarBlock() /*throws Exception*/ {
            Grammar g = new Grammar(
                "grammar t;\n" +
                "options {backtrack=true;}\n" +
                "a : ('a'{;}|'b')* ;"
            );
            string expecting =
                ".s0->.s1" + NewLine +
                ".s1->.s2" + NewLine +
                ".s12->.s13" + NewLine +
                ".s13-{synpred2_t}?->.s14" + NewLine +
                ".s14-'b'->.s15" + NewLine +
                ".s15->.s8" + NewLine +
                ".s16->.s9" + NewLine +
                ".s2->.s16" + NewLine +
                ".s2->.s3" + NewLine +
                ".s3->.s12" + NewLine +
                ".s3->.s4" + NewLine +
                ".s4-{synpred1_t}?->.s5" + NewLine +
                ".s5-'a'->.s6" + NewLine +
                ".s6-{}->.s7" + NewLine +
                ".s7->.s8" + NewLine +
                ".s8->.s3" + NewLine +
                ".s8->.s9" + NewLine +
                ".s9->:s10" + NewLine +
                ":s10-EOF->.s11" + NewLine;
            checkRule( g, "a", expecting );
        }

        [TestMethod]
        public void TestAutoBacktracking_StarSetBlock_IgnoresPreds() /*throws Exception*/ {
            Grammar g = new Grammar(
                "grammar t;\n" +
                "options {backtrack=true;}\n" +
                "a : ('a'|'b')* ;"
            );
            string expecting =
                ".s0->.s1" + NewLine +
                ".s1->.s2" + NewLine +
                ".s2->.s3" + NewLine +
                ".s2->.s9" + NewLine +
                ".s3->.s4" + NewLine +
                ".s4-'a'..'b'->.s5" + NewLine +
                ".s5->.s3" + NewLine +
                ".s5->.s6" + NewLine +
                ".s6->:s7" + NewLine +
                ".s9->.s6" + NewLine +
                ":s7-EOF->.s8" + NewLine;
            checkRule( g, "a", expecting );
        }

        [TestMethod]
        public void TestAutoBacktracking_StarSetBlock() /*throws Exception*/ {
            Grammar g = new Grammar(
                "grammar t;\n" +
                "options {backtrack=true;}\n" +
                "a : ('a'|'b'{;})* ;"
            );
            string expecting =
                ".s0->.s1" + NewLine +
                ".s1->.s2" + NewLine +
                ".s11->.s12" + NewLine +
                ".s12-{synpred2_t}?->.s13" + NewLine +
                ".s13-'b'->.s14" + NewLine +
                ".s14-{}->.s15" + NewLine +
                ".s15->.s7" + NewLine +
                ".s16->.s8" + NewLine +
                ".s2->.s16" + NewLine +
                ".s2->.s3" + NewLine +
                ".s3->.s11" + NewLine +
                ".s3->.s4" + NewLine +
                ".s4-{synpred1_t}?->.s5" + NewLine +
                ".s5-'a'->.s6" + NewLine +
                ".s6->.s7" + NewLine +
                ".s7->.s3" + NewLine +
                ".s7->.s8" + NewLine +
                ".s8->:s9" + NewLine +
                ":s9-EOF->.s10" + NewLine;
            checkRule( g, "a", expecting );
        }

        [TestMethod]
        public void TestAutoBacktracking_StarBlock1Alt() /*throws Exception*/ {
            Grammar g = new Grammar(
                "grammar t;\n" +
                "options {backtrack=true;}\n" +
                "a : ('a')* ;"
            );
            string expecting =
                ".s0->.s1" + NewLine +
                ".s1->.s2" + NewLine +
                ".s10->.s7" + NewLine +
                ".s2->.s10" + NewLine +
                ".s2->.s3" + NewLine +
                ".s3->.s4" + NewLine +
                ".s4-{synpred1_t}?->.s5" + NewLine +
                ".s5-'a'->.s6" + NewLine +
                ".s6->.s3" + NewLine +
                ".s6->.s7" + NewLine +
                ".s7->:s8" + NewLine +
                ":s8-EOF->.s9" + NewLine;
            checkRule( g, "a", expecting );
        }

        [TestMethod]
        public void TestAutoBacktracking_PlusBlock() /*throws Exception*/ {
            Grammar g = new Grammar(
                "grammar t;\n" +
                "options {backtrack=true;}\n" +
                "a : ('a'{;}|'b')+ ;"
            );
            string expecting =
                ".s0->.s1" + NewLine +
                ".s1->.s2" + NewLine +
                ".s12->.s13" + NewLine +
                ".s13-{synpred2_t}?->.s14" + NewLine +
                ".s14-'b'->.s15" + NewLine +
                ".s15->.s8" + NewLine +
                ".s2->.s3" + NewLine +
                ".s3->.s12" + NewLine +
                ".s3->.s4" + NewLine +
                ".s4-{synpred1_t}?->.s5" + NewLine +
                ".s5-'a'->.s6" + NewLine +
                ".s6-{}->.s7" + NewLine +
                ".s7->.s8" + NewLine +
                ".s8->.s3" + NewLine +
                ".s8->.s9" + NewLine +
                ".s9->:s10" + NewLine +
                ":s10-EOF->.s11" + NewLine;
            checkRule( g, "a", expecting );
        }

        [TestMethod]
        public void TestAutoBacktracking_PlusSetBlock() /*throws Exception*/ {
            Grammar g = new Grammar(
                "grammar t;\n" +
                "options {backtrack=true;}\n" +
                "a : ('a'|'b'{;})+ ;"
            );
            string expecting =
                ".s0->.s1" + NewLine +
                ".s1->.s2" + NewLine +
                ".s11->.s12" + NewLine +
                ".s12-{synpred2_t}?->.s13" + NewLine +
                ".s13-'b'->.s14" + NewLine +
                ".s14-{}->.s15" + NewLine +
                ".s15->.s7" + NewLine +
                ".s2->.s3" + NewLine +
                ".s3->.s11" + NewLine +
                ".s3->.s4" + NewLine +
                ".s4-{synpred1_t}?->.s5" + NewLine +
                ".s5-'a'->.s6" + NewLine +
                ".s6->.s7" + NewLine +
                ".s7->.s3" + NewLine +
                ".s7->.s8" + NewLine +
                ".s8->:s9" + NewLine +
                ":s9-EOF->.s10" + NewLine;
            checkRule( g, "a", expecting );
        }

        [TestMethod]
        public void TestAutoBacktracking_PlusBlock1Alt() /*throws Exception*/ {
            Grammar g = new Grammar(
                "grammar t;\n" +
                "options {backtrack=true;}\n" +
                "a : ('a')+ ;"
            );
            string expecting =
                ".s0->.s1" + NewLine +
                ".s1->.s2" + NewLine +
                ".s2->.s3" + NewLine +
                ".s3->.s4" + NewLine +
                ".s4-{synpred1_t}?->.s5" + NewLine +
                ".s5-'a'->.s6" + NewLine +
                ".s6->.s3" + NewLine +
                ".s6->.s7" + NewLine +
                ".s7->:s8" + NewLine +
                ":s8-EOF->.s9" + NewLine;
            checkRule( g, "a", expecting );
        }

        [TestMethod]
        public void TestAutoBacktracking_OptionalBlock2Alts() /*throws Exception*/ {
            Grammar g = new Grammar(
                "grammar t;\n" +
                "options {backtrack=true;}\n" +
                "a : ('a'{;}|'b')?;"
            );
            string expecting =
                ".s0->.s1" + NewLine +
                ".s1->.s2" + NewLine +
                ".s10->.s11" + NewLine +
                ".s10->.s14" + NewLine +
                ".s11-{synpred2_t}?->.s12" + NewLine +
                ".s12-'b'->.s13" + NewLine +
                ".s13->.s7" + NewLine +
                ".s14->.s7" + NewLine +
                ".s2->.s10" + NewLine +
                ".s2->.s3" + NewLine +
                ".s3-{synpred1_t}?->.s4" + NewLine +
                ".s4-'a'->.s5" + NewLine +
                ".s5-{}->.s6" + NewLine +
                ".s6->.s7" + NewLine +
                ".s7->:s8" + NewLine +
                ":s8-EOF->.s9" + NewLine;
            checkRule( g, "a", expecting );
        }

        [TestMethod]
        public void TestAutoBacktracking_OptionalBlock1Alt() /*throws Exception*/ {
            Grammar g = new Grammar(
                "grammar t;\n" +
                "options {backtrack=true;}\n" +
                "a : ('a')?;"
            );
            string expecting =
                ".s0->.s1" + NewLine +
                ".s1->.s2" + NewLine +
                ".s2->.s3" + NewLine +
                ".s2->.s9" + NewLine +
                ".s3-{synpred1_t}?->.s4" + NewLine +
                ".s4-'a'->.s5" + NewLine +
                ".s5->.s6" + NewLine +
                ".s6->:s7" + NewLine +
                ".s9->.s6" + NewLine +
                ":s7-EOF->.s8" + NewLine;
            checkRule( g, "a", expecting );
        }

        [TestMethod]
        public void TestAutoBacktracking_ExistingPred() /*throws Exception*/ {
            Grammar g = new Grammar(
                "grammar t;\n" +
                "options {backtrack=true;}\n" +
                "a : ('a')=> 'a' | 'b';"
            );
            string expecting =
                ".s0->.s1" + NewLine +
                ".s1->.s2" + NewLine +
                ".s1->.s8" + NewLine +
                ".s10->.s5" + NewLine +
                ".s2-{synpred1_t}?->.s3" + NewLine +
                ".s3-'a'->.s4" + NewLine +
                ".s4->.s5" + NewLine +
                ".s5->:s6" + NewLine +
                ".s8->.s9" + NewLine +
                ".s9-'b'->.s10" + NewLine +
                ":s6-EOF->.s7" + NewLine;
            checkRule( g, "a", expecting );
        }

        private void checkRule( Grammar g, string rule, string expecting )
        {
            g.BuildNFA();
            State startState = g.GetRuleStartState( rule );
            FASerializer serializer = new FASerializer( g );
            string result = serializer.Serialize( startState );

            //System.out.print(result);
            Assert.AreEqual( expecting, result );
        }

    }
}
