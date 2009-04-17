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

    using Grammar = Antlr3.Tool.Grammar;

    [TestClass]
    public class TestASTConstruction : BaseTest
    {

        /** Public default constructor used by TestRig */
        public TestASTConstruction()
        {
        }

        [TestMethod]
        public void TestA() /*throws Exception*/ {
            Grammar g = new Grammar(
                    "parser grammar P;\n" +
                    "a : A;" );
            string expecting =
                "(rule a ARG RET scope (BLOCK (ALT A <end-of-alt>) <end-of-block>) <end-of-rule>)";
            string found = g.GetRule( "a" ).tree.ToStringTree();
            assertEquals( expecting, found );
        }

        [TestMethod]
        public void TestNakeRulePlusInLexer() /*throws Exception*/ {
            Grammar g = new Grammar(
                    "lexer grammar P;\n" +
                    "A : B+;\n" +
                    "B : 'a';" );
            string expecting =
                "(rule A ARG RET scope (BLOCK (ALT (+ (BLOCK (ALT B <end-of-alt>) <end-of-block>)) <end-of-alt>) <end-of-block>) <end-of-rule>)";
            string found = g.GetRule( "A" ).tree.ToStringTree();
            assertEquals( expecting, found );
        }

        [TestMethod]
        public void TestRulePlus() /*throws Exception*/ {
            Grammar g = new Grammar(
                    "parser grammar P;\n" +
                    "a : (b)+;\n" +
                    "b : B;" );
            string expecting =
                "(rule a ARG RET scope (BLOCK (ALT (+ (BLOCK (ALT b <end-of-alt>) <end-of-block>)) <end-of-alt>) <end-of-block>) <end-of-rule>)";
            string found = g.GetRule( "a" ).tree.ToStringTree();
            assertEquals( expecting, found );
        }

        [TestMethod]
        public void TestNakedRulePlus() /*throws Exception*/ {
            Grammar g = new Grammar(
                    "parser grammar P;\n" +
                    "a : b+;\n" +
                    "b : B;" );
            string expecting =
                "(rule a ARG RET scope (BLOCK (ALT (+ (BLOCK (ALT b <end-of-alt>) <end-of-block>)) <end-of-alt>) <end-of-block>) <end-of-rule>)";
            string found = g.GetRule( "a" ).tree.ToStringTree();
            assertEquals( expecting, found );
        }

        [TestMethod]
        public void TestRuleOptional() /*throws Exception*/ {
            Grammar g = new Grammar(
                    "parser grammar P;\n" +
                    "a : (b)?;\n" +
                    "b : B;" );
            string expecting =
                "(rule a ARG RET scope (BLOCK (ALT (? (BLOCK (ALT b <end-of-alt>) <end-of-block>)) <end-of-alt>) <end-of-block>) <end-of-rule>)";
            string found = g.GetRule( "a" ).tree.ToStringTree();
            assertEquals( expecting, found );
        }

        [TestMethod]
        public void TestNakedRuleOptional() /*throws Exception*/ {
            Grammar g = new Grammar(
                    "parser grammar P;\n" +
                    "a : b?;\n" +
                    "b : B;" );
            string expecting =
                "(rule a ARG RET scope (BLOCK (ALT (? (BLOCK (ALT b <end-of-alt>) <end-of-block>)) <end-of-alt>) <end-of-block>) <end-of-rule>)";
            string found = g.GetRule( "a" ).tree.ToStringTree();
            assertEquals( expecting, found );
        }

        [TestMethod]
        public void TestRuleStar() /*throws Exception*/ {
            Grammar g = new Grammar(
                    "parser grammar P;\n" +
                    "a : (b)*;\n" +
                    "b : B;" );
            string expecting =
                "(rule a ARG RET scope (BLOCK (ALT (* (BLOCK (ALT b <end-of-alt>) <end-of-block>)) <end-of-alt>) <end-of-block>) <end-of-rule>)";
            string found = g.GetRule( "a" ).tree.ToStringTree();
            assertEquals( expecting, found );
        }

        [TestMethod]
        public void TestNakedRuleStar() /*throws Exception*/ {
            Grammar g = new Grammar(
                    "parser grammar P;\n" +
                    "a : b*;\n" +
                    "b : B;" );
            string expecting =
                "(rule a ARG RET scope (BLOCK (ALT (* (BLOCK (ALT b <end-of-alt>) <end-of-block>)) <end-of-alt>) <end-of-block>) <end-of-rule>)";
            string found = g.GetRule( "a" ).tree.ToStringTree();
            assertEquals( expecting, found );
        }

        [TestMethod]
        public void TestCharStar() /*throws Exception*/ {
            Grammar g = new Grammar(
                    "grammar P;\n" +
                    "a : 'a'*;" );
            string expecting =
                "(rule a ARG RET scope (BLOCK (ALT (* (BLOCK (ALT 'a' <end-of-alt>) <end-of-block>)) <end-of-alt>) <end-of-block>) <end-of-rule>)";
            string found = g.GetRule( "a" ).tree.ToStringTree();
            assertEquals( expecting, found );
        }

        [TestMethod]
        public void TestCharStarInLexer() /*throws Exception*/ {
            Grammar g = new Grammar(
                    "lexer grammar P;\n" +
                    "B : 'b'*;" );
            string expecting =
                "(rule B ARG RET scope (BLOCK (ALT (* (BLOCK (ALT 'b' <end-of-alt>) <end-of-block>)) <end-of-alt>) <end-of-block>) <end-of-rule>)";
            string found = g.GetRule( "B" ).tree.ToStringTree();
            assertEquals( expecting, found );
        }

        [TestMethod]
        public void TestStringStar() /*throws Exception*/ {
            Grammar g = new Grammar(
                    "grammar P;\n" +
                    "a : 'while'*;" );
            string expecting =
                "(rule a ARG RET scope (BLOCK (ALT (* (BLOCK (ALT 'while' <end-of-alt>) <end-of-block>)) <end-of-alt>) <end-of-block>) <end-of-rule>)";
            string found = g.GetRule( "a" ).tree.ToStringTree();
            assertEquals( expecting, found );
        }

        [TestMethod]
        public void TestStringStarInLexer() /*throws Exception*/ {
            Grammar g = new Grammar(
                    "lexer grammar P;\n" +
                    "B : 'while'*;" );
            string expecting =
                "(rule B ARG RET scope (BLOCK (ALT (* (BLOCK (ALT 'while' <end-of-alt>) <end-of-block>)) <end-of-alt>) <end-of-block>) <end-of-rule>)";
            string found = g.GetRule( "B" ).tree.ToStringTree();
            assertEquals( expecting, found );
        }

        [TestMethod]
        public void TestCharPlus() /*throws Exception*/ {
            Grammar g = new Grammar(
                    "grammar P;\n" +
                    "a : 'a'+;" );
            string expecting =
                "(rule a ARG RET scope (BLOCK (ALT (+ (BLOCK (ALT 'a' <end-of-alt>) <end-of-block>)) <end-of-alt>) <end-of-block>) <end-of-rule>)";
            string found = g.GetRule( "a" ).tree.ToStringTree();
            assertEquals( expecting, found );
        }

        [TestMethod]
        public void TestCharPlusInLexer() /*throws Exception*/ {
            Grammar g = new Grammar(
                    "lexer grammar P;\n" +
                    "B : 'b'+;" );
            string expecting =
                "(rule B ARG RET scope (BLOCK (ALT (+ (BLOCK (ALT 'b' <end-of-alt>) <end-of-block>)) <end-of-alt>) <end-of-block>) <end-of-rule>)";
            string found = g.GetRule( "B" ).tree.ToStringTree();
            assertEquals( expecting, found );
        }

        [TestMethod]
        public void TestCharOptional() /*throws Exception*/ {
            Grammar g = new Grammar(
                    "grammar P;\n" +
                    "a : 'a'?;" );
            string expecting =
                "(rule a ARG RET scope (BLOCK (ALT (? (BLOCK (ALT 'a' <end-of-alt>) <end-of-block>)) <end-of-alt>) <end-of-block>) <end-of-rule>)";
            string found = g.GetRule( "a" ).tree.ToStringTree();
            assertEquals( expecting, found );
        }

        [TestMethod]
        public void TestCharOptionalInLexer() /*throws Exception*/ {
            Grammar g = new Grammar(
                    "lexer grammar P;\n" +
                    "B : 'b'?;" );
            string expecting =
                "(rule B ARG RET scope (BLOCK (ALT (? (BLOCK (ALT 'b' <end-of-alt>) <end-of-block>)) <end-of-alt>) <end-of-block>) <end-of-rule>)";
            string found = g.GetRule( "B" ).tree.ToStringTree();
            assertEquals( expecting, found );
        }

        [TestMethod]
        public void TestCharRangePlus() /*throws Exception*/ {
            Grammar g = new Grammar(
                    "lexer grammar P;\n" +
                    "ID : 'a'..'z'+;" );
            string expecting =
                "(rule ID ARG RET scope (BLOCK (ALT (+ (BLOCK (ALT (.. 'a' 'z') <end-of-alt>) <end-of-block>)) <end-of-alt>) <end-of-block>) <end-of-rule>)";
            string found = g.GetRule( "ID" ).tree.ToStringTree();
            assertEquals( expecting, found );
        }

        [TestMethod]
        public void TestLabel() /*throws Exception*/ {
            Grammar g = new Grammar(
                    "grammar P;\n" +
                    "a : x=ID;" );
            string expecting =
                "(rule a ARG RET scope (BLOCK (ALT (= x ID) <end-of-alt>) <end-of-block>) <end-of-rule>)";
            string found = g.GetRule( "a" ).tree.ToStringTree();
            assertEquals( expecting, found );
        }

        [TestMethod]
        public void TestLabelOfOptional() /*throws Exception*/ {
            Grammar g = new Grammar(
                    "grammar P;\n" +
                    "a : x=ID?;" );
            string expecting =
                "(rule a ARG RET scope (BLOCK (ALT (? (BLOCK (ALT (= x ID) <end-of-alt>) <end-of-block>)) <end-of-alt>) <end-of-block>) <end-of-rule>)";
            string found = g.GetRule( "a" ).tree.ToStringTree();
            assertEquals( expecting, found );
        }

        [TestMethod]
        public void TestLabelOfClosure() /*throws Exception*/ {
            Grammar g = new Grammar(
                    "grammar P;\n" +
                    "a : x=ID*;" );
            string expecting =
                "(rule a ARG RET scope (BLOCK (ALT (* (BLOCK (ALT (= x ID) <end-of-alt>) <end-of-block>)) <end-of-alt>) <end-of-block>) <end-of-rule>)";
            string found = g.GetRule( "a" ).tree.ToStringTree();
            assertEquals( expecting, found );
        }

        [TestMethod]
        public void TestRuleLabel() /*throws Exception*/ {
            Grammar g = new Grammar(
                    "grammar P;\n" +
                    "a : x=b;\n" +
                    "b : ID;\n" );
            string expecting =
                "(rule a ARG RET scope (BLOCK (ALT (= x b) <end-of-alt>) <end-of-block>) <end-of-rule>)";
            string found = g.GetRule( "a" ).tree.ToStringTree();
            assertEquals( expecting, found );
        }

        [TestMethod]
        public void TestSetLabel() /*throws Exception*/ {
            Grammar g = new Grammar(
                    "grammar P;\n" +
                    "a : x=(A|B);\n" );
            string expecting =
                "(rule a ARG RET scope (BLOCK (ALT (= x (BLOCK (ALT A <end-of-alt>) (ALT B <end-of-alt>) <end-of-block>)) <end-of-alt>) <end-of-block>) <end-of-rule>)";
            string found = g.GetRule( "a" ).tree.ToStringTree();
            assertEquals( expecting, found );
        }

        [TestMethod]
        public void TestNotSetLabel() /*throws Exception*/ {
            Grammar g = new Grammar(
                    "grammar P;\n" +
                    "a : x=~(A|B);\n" );
            string expecting =
                "(rule a ARG RET scope (BLOCK (ALT (= x (~ (BLOCK (ALT A <end-of-alt>) (ALT B <end-of-alt>) <end-of-block>))) <end-of-alt>) <end-of-block>) <end-of-rule>)";
            string found = g.GetRule( "a" ).tree.ToStringTree();
            assertEquals( expecting, found );
        }

        [TestMethod]
        public void TestNotSetListLabel() /*throws Exception*/ {
            Grammar g = new Grammar(
                    "grammar P;\n" +
                    "a : x+=~(A|B);\n" );
            string expecting =
                "(rule a ARG RET scope (BLOCK (ALT (+= x (~ (BLOCK (ALT A <end-of-alt>) (ALT B <end-of-alt>) <end-of-block>))) <end-of-alt>) <end-of-block>) <end-of-rule>)";
            string found = g.GetRule( "a" ).tree.ToStringTree();
            assertEquals( expecting, found );
        }

        [TestMethod]
        public void TestNotSetListLabelInLoop() /*throws Exception*/ {
            Grammar g = new Grammar(
                    "grammar P;\n" +
                    "a : x+=~(A|B)+;\n" );
            string expecting =
                "(rule a ARG RET scope (BLOCK (ALT (+ (BLOCK (ALT (+= x (~ (BLOCK (ALT A <end-of-alt>) (ALT B <end-of-alt>) <end-of-block>))) <end-of-alt>) <end-of-block>)) <end-of-alt>) <end-of-block>) <end-of-rule>)";
            string found = g.GetRule( "a" ).tree.ToStringTree();
            assertEquals( expecting, found );
        }

        [TestMethod]
        public void TestRuleLabelOfPositiveClosure() /*throws Exception*/ {
            Grammar g = new Grammar(
                    "grammar P;\n" +
                    "a : x=b+;\n" +
                    "b : ID;\n" );
            string expecting =
                "(rule a ARG RET scope (BLOCK (ALT (+ (BLOCK (ALT (= x b) <end-of-alt>) <end-of-block>)) <end-of-alt>) <end-of-block>) <end-of-rule>)";
            string found = g.GetRule( "a" ).tree.ToStringTree();
            assertEquals( expecting, found );
        }

        [TestMethod]
        public void TestListLabelOfClosure() /*throws Exception*/ {
            Grammar g = new Grammar(
                    "grammar P;\n" +
                    "a : x+=ID*;" );
            string expecting =
                "(rule a ARG RET scope (BLOCK (ALT (* (BLOCK (ALT (+= x ID) <end-of-alt>) <end-of-block>)) <end-of-alt>) <end-of-block>) <end-of-rule>)";
            string found = g.GetRule( "a" ).tree.ToStringTree();
            assertEquals( expecting, found );
        }

        [TestMethod]
        public void TestListLabelOfClosure2() /*throws Exception*/ {
            Grammar g = new Grammar(
                    "grammar P;\n" +
                    "a : x+='int'*;" );
            string expecting =
                "(rule a ARG RET scope (BLOCK (ALT (* (BLOCK (ALT (+= x 'int') <end-of-alt>) <end-of-block>)) <end-of-alt>) <end-of-block>) <end-of-rule>)";
            string found = g.GetRule( "a" ).tree.ToStringTree();
            assertEquals( expecting, found );
        }

        [TestMethod]
        public void TestRuleListLabelOfPositiveClosure() /*throws Exception*/ {
            Grammar g = new Grammar(
                    "grammar P;\n" +
                    "options {output=AST;}\n" +
                    "a : x+=b+;\n" +
                    "b : ID;\n" );
            string expecting =
                "(rule a ARG RET scope (BLOCK (ALT (+ (BLOCK (ALT (+= x b) <end-of-alt>) <end-of-block>)) <end-of-alt>) <end-of-block>) <end-of-rule>)";
            string found = g.GetRule( "a" ).tree.ToStringTree();
            assertEquals( expecting, found );
        }

        [TestMethod]
        public void TestRootTokenInStarLoop() /*throws Exception*/ {
            Grammar g = new Grammar(
                    "grammar Expr;\n" +
                    "options { backtrack=true; }\n" +
                    "a : ('*'^)* ;\n" );  // bug: the synpred had nothing in it
            string expecting =
                "(rule synpred1_Expr ARG RET scope (BLOCK (ALT '*' <end-of-alt>) <end-of-block>) <end-of-rule>)";
            string found = g.GetRule( "synpred1_Expr" ).tree.ToStringTree();
            assertEquals( expecting, found );
        }

        [TestMethod]
        public void TestActionInStarLoop() /*throws Exception*/ {
            Grammar g = new Grammar(
                    "grammar Expr;\n" +
                    "options { backtrack=true; }\n" +
                    "a : ({blort} 'x')* ;\n" );  // bug: the synpred had nothing in it
            string expecting =
                "(rule synpred1_Expr ARG RET scope (BLOCK (ALT blort 'x' <end-of-alt>) <end-of-block>) <end-of-rule>)";
            string found = g.GetRule( "synpred1_Expr" ).tree.ToStringTree();
            assertEquals( expecting, found );
        }

    }
}
