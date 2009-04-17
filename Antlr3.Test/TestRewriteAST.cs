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

    using AntlrTool = Antlr3.AntlrTool;
    using CodeGenerator = Antlr3.Codegen.CodeGenerator;
    using ErrorManager = Antlr3.Tool.ErrorManager;
    using Grammar = Antlr3.Tool.Grammar;
    using GrammarSemanticsMessage = Antlr3.Tool.GrammarSemanticsMessage;

    [TestClass]
    public class TestRewriteAST : BaseTest
    {
        protected bool debug = false;

        [TestMethod]
        public void TestDelete() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ID INT -> ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                        "a", "abc 34", debug );
            assertEquals( "", found );
        }

        [TestMethod]
        public void TestSingleToken() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ID -> ID;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                        "a", "abc", debug );
            assertEquals( "abc" + NewLine, found );
        }

        [TestMethod]
        public void TestSingleTokenToNewNode() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ID -> ID[\"x\"];\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                        "a", "abc", debug );
            assertEquals( "x" + NewLine, found );
        }

        [TestMethod]
        public void TestSingleTokenToNewNodeRoot() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ID -> ^(ID[\"x\"] INT);\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                        "a", "abc", debug );
            assertEquals( "(x INT)" + NewLine, found );
        }

        [TestMethod]
        public void TestSingleTokenToNewNode2() /*throws Exception*/ {
            // Allow creation of new nodes w/o args.
            string grammar =
                "grammar TT;\n" +
                "options {output=AST;}\n" +
                "a : ID -> ID[ ];\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "TT.g", grammar, "TTParser", "TTLexer",
                        "a", "abc", debug );
            assertEquals( "ID" + NewLine, found );
        }

        [TestMethod]
        public void TestSingleCharLiteral() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : 'c' -> 'c';\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                        "a", "c", debug );
            assertEquals( "c" + NewLine, found );
        }

        [TestMethod]
        public void TestSingleStringLiteral() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : 'ick' -> 'ick';\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                        "a", "ick", debug );
            assertEquals( "ick" + NewLine, found );
        }

        [TestMethod]
        public void TestSingleRule() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : b -> b;\n" +
                "b : ID ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                        "a", "abc", debug );
            assertEquals( "abc" + NewLine, found );
        }

        [TestMethod]
        public void TestReorderTokens() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ID INT -> INT ID;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                        "a", "abc 34", debug );
            assertEquals( "34 abc" + NewLine, found );
        }

        [TestMethod]
        public void TestReorderTokenAndRule() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : b INT -> INT b;\n" +
                "b : ID ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                        "a", "abc 34", debug );
            assertEquals( "34 abc" + NewLine, found );
        }

        [TestMethod]
        public void TestTokenTree() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ID INT -> ^(INT ID);\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                        "a", "abc 34", debug );
            assertEquals( "(34 abc)" + NewLine, found );
        }

        [TestMethod]
        public void TestTokenTreeAfterOtherStuff() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : 'void' ID INT -> 'void' ^(INT ID);\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                        "a", "void abc 34", debug );
            assertEquals( "void (34 abc)" + NewLine, found );
        }

        [TestMethod]
        public void TestNestedTokenTreeWithOuterLoop() /*throws Exception*/ {
            // verify that ID and INT both iterate over outer index variable
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "tokens {DUH;}\n" +
                "a : ID INT ID INT -> ^( DUH ID ^( DUH INT) )+ ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                        "a", "a 1 b 2", debug );
            assertEquals( "(DUH a (DUH 1)) (DUH b (DUH 2))" + NewLine, found );
        }

        [TestMethod]
        public void TestOptionalSingleToken() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ID -> ID? ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                        "a", "abc", debug );
            assertEquals( "abc" + NewLine, found );
        }

        [TestMethod]
        public void TestClosureSingleToken() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ID ID -> ID* ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                        "a", "a b", debug );
            assertEquals( "a b" + NewLine, found );
        }

        [TestMethod]
        public void TestPositiveClosureSingleToken() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ID ID -> ID+ ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                        "a", "a b", debug );
            assertEquals( "a b" + NewLine, found );
        }

        [TestMethod]
        public void TestOptionalSingleRule() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : b -> b?;\n" +
                "b : ID ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                        "a", "abc", debug );
            assertEquals( "abc" + NewLine, found );
        }

        [TestMethod]
        public void TestClosureSingleRule() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : b b -> b*;\n" +
                "b : ID ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                        "a", "a b", debug );
            assertEquals( "a b" + NewLine, found );
        }

        [TestMethod]
        public void TestClosureOfLabel() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : x+=b x+=b -> $x*;\n" +
                "b : ID ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                        "a", "a b", debug );
            assertEquals( "a b" + NewLine, found );
        }

        [TestMethod]
        public void TestOptionalLabelNoListLabel() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : (x=ID)? -> $x?;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                        "a", "a", debug );
            assertEquals( "a" + NewLine, found );
        }

        [TestMethod]
        public void TestPositiveClosureSingleRule() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : b b -> b+;\n" +
                "b : ID ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                        "a", "a b", debug );
            assertEquals( "a b" + NewLine, found );
        }

        [TestMethod]
        public void TestSinglePredicateT() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ID -> {true}? ID -> ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                        "a", "abc", debug );
            assertEquals( "abc" + NewLine, found );
        }

        [TestMethod]
        public void TestSinglePredicateF() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ID -> {false}? ID -> ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                        "a", "abc", debug );
            assertEquals( "", found );
        }

        [TestMethod]
        public void TestMultiplePredicate() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ID INT -> {false}? ID\n" +
                "           -> {true}? INT\n" +
                "           -> \n" +
                "  ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                        "a", "a 2", debug );
            assertEquals( "2" + NewLine, found );
        }

        [TestMethod]
        public void TestMultiplePredicateTrees() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ID INT -> {false}? ^(ID INT)\n" +
                "           -> {true}? ^(INT ID)\n" +
                "           -> ID\n" +
                "  ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                        "a", "a 2", debug );
            assertEquals( "(2 a)" + NewLine, found );
        }

        [TestMethod]
        public void TestSimpleTree() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : op INT -> ^(op INT);\n" +
                "op : '+'|'-' ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                        "a", "-34", debug );
            assertEquals( "(- 34)" + NewLine, found );
        }

        [TestMethod]
        public void TestSimpleTree2() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : op INT -> ^(INT op);\n" +
                "op : '+'|'-' ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                        "a", "+ 34", debug );
            assertEquals( "(34 +)" + NewLine, found );
        }


        [TestMethod]
        public void TestNestedTrees() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : 'var' (ID ':' type ';')+ -> ^('var' ^(':' ID type)+) ;\n" +
                "type : 'int' | 'float' ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                        "a", "var a:int; b:float;", debug );
            assertEquals( "(var (: a int) (: b float))" + NewLine, found );
        }

        [TestMethod]
        public void TestImaginaryTokenCopy() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "tokens {VAR;}\n" +
                "a : ID (',' ID)*-> ^(VAR ID)+ ;\n" +
                "type : 'int' | 'float' ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                        "a", "a,b,c", debug );
            assertEquals( "(VAR a) (VAR b) (VAR c)" + NewLine, found );
        }

        [TestMethod]
        public void TestTokenUnreferencedOnLeftButDefined() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "tokens {VAR;}\n" +
                "a : b -> ID ;\n" +
                "b : ID ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                        "a", "a", debug );
            assertEquals( "ID" + NewLine, found );
        }

        [TestMethod]
        public void TestImaginaryTokenCopySetText() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "tokens {VAR;}\n" +
                "a : ID (',' ID)*-> ^(VAR[\"var\"] ID)+ ;\n" +
                "type : 'int' | 'float' ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                        "a", "a,b,c", debug );
            assertEquals( "(var a) (var b) (var c)" + NewLine, found );
        }

        [TestMethod]
        public void TestImaginaryTokenNoCopyFromToken() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "tokens {BLOCK;}\n" +
                "a : lc='{' ID+ '}' -> ^(BLOCK[$lc] ID+) ;\n" +
                "type : 'int' | 'float' ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                        "a", "{a b c}", debug );
            assertEquals( "({ a b c)" + NewLine, found );
        }

        [TestMethod]
        public void TestImaginaryTokenNoCopyFromTokenSetText() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "tokens {BLOCK;}\n" +
                "a : lc='{' ID+ '}' -> ^(BLOCK[$lc,\"block\"] ID+) ;\n" +
                "type : 'int' | 'float' ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                        "a", "{a b c}", debug );
            assertEquals( "(block a b c)" + NewLine, found );
        }

        [TestMethod]
        public void TestMixedRewriteAndAutoAST() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "tokens {BLOCK;}\n" +
                "a : b b^ ;\n" + // 2nd b matches only an INT; can make it root
                "b : ID INT -> INT ID\n" +
                "  | INT\n" +
                "  ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                        "a", "a 1 2", debug );
            assertEquals( "(2 1 a)" + NewLine, found );
        }

        [TestMethod]
        public void TestSubruleWithRewrite() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "tokens {BLOCK;}\n" +
                "a : b b ;\n" +
                "b : (ID INT -> INT ID | INT INT -> INT+ )\n" +
                "  ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                        "a", "a 1 2 3", debug );
            assertEquals( "1 a 2 3" + NewLine, found );
        }

        [TestMethod]
        public void TestSubruleWithRewrite2() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "tokens {TYPE;}\n" +
                "a : b b ;\n" +
                "b : 'int'\n" +
                "    ( ID -> ^(TYPE 'int' ID)\n" +
                "    | ID '=' INT -> ^(TYPE 'int' ID INT)\n" +
                "    )\n" +
                "    ';'\n" +
                "  ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                        "a", "int a; int b=3;", debug );
            assertEquals( "(TYPE int a) (TYPE int b 3)" + NewLine, found );
        }

        [TestMethod]
        public void TestNestedRewriteShutsOffAutoAST() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "tokens {BLOCK;}\n" +
                "a : b b ;\n" +
                "b : ID ( ID (last=ID -> $last)+ ) ';'\n" + // get last ID
                "  | INT\n" + // should still get auto AST construction
                "  ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                        "a", "a b c d; 42", debug );
            assertEquals( "d 42" + NewLine, found );
        }

        [TestMethod]
        public void TestRewriteActions() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : atom -> ^({adaptor.create(INT,\"9\")} atom) ;\n" +
                "atom : INT ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                        "a", "3", debug );
            assertEquals( "(9 3)" + NewLine, found );
        }

        [TestMethod]
        public void TestRewriteActions2() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : atom -> {adaptor.create(INT,\"9\")} atom ;\n" +
                "atom : INT ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                        "a", "3", debug );
            assertEquals( "9 3" + NewLine, found );
        }

        [TestMethod]
        public void TestRefToOldValue() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "tokens {BLOCK;}\n" +
                "a : (atom -> atom) (op='+' r=atom -> ^($op $a $r) )* ;\n" +
                "atom : INT ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                        "a", "3+4+5", debug );
            assertEquals( "(+ (+ 3 4) 5)" + NewLine, found );
        }

        [TestMethod]
        public void TestCopySemanticsForRules() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "tokens {BLOCK;}\n" +
                "a : atom -> ^(atom atom) ;\n" + // NOT CYCLE! (dup atom)
                "atom : INT ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                        "a", "3", debug );
            assertEquals( "(3 3)" + NewLine, found );
        }

        [TestMethod]
        public void TestCopySemanticsForRules2() /*throws Exception*/ {
            // copy type as a root for each invocation of (...)+ in rewrite
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : type ID (',' ID)* ';' -> ^(type ID)+ ;\n" +
                "type : 'int' ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                        "a", "int a,b,c;", debug );
            assertEquals( "(int a) (int b) (int c)" + NewLine, found );
        }

        [TestMethod]
        public void TestCopySemanticsForRules3() /*throws Exception*/ {
            // copy type *and* modifier even though it's optional
            // for each invocation of (...)+ in rewrite
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : modifier? type ID (',' ID)* ';' -> ^(type modifier? ID)+ ;\n" +
                "type : 'int' ;\n" +
                "modifier : 'public' ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                        "a", "public int a,b,c;", debug );
            assertEquals( "(int public a) (int public b) (int public c)" + NewLine, found );
        }

        [TestMethod]
        public void TestCopySemanticsForRules3Double() /*throws Exception*/ {
            // copy type *and* modifier even though it's optional
            // for each invocation of (...)+ in rewrite
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : modifier? type ID (',' ID)* ';' -> ^(type modifier? ID)+ ^(type modifier? ID)+ ;\n" +
                "type : 'int' ;\n" +
                "modifier : 'public' ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                        "a", "public int a,b,c;", debug );
            assertEquals( "(int public a) (int public b) (int public c) (int public a) (int public b) (int public c)" + NewLine, found );
        }

        [TestMethod]
        public void TestCopySemanticsForRules4() /*throws Exception*/ {
            // copy type *and* modifier even though it's optional
            // for each invocation of (...)+ in rewrite
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "tokens {MOD;}\n" +
                "a : modifier? type ID (',' ID)* ';' -> ^(type ^(MOD modifier)? ID)+ ;\n" +
                "type : 'int' ;\n" +
                "modifier : 'public' ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                        "a", "public int a,b,c;", debug );
            assertEquals( "(int (MOD public) a) (int (MOD public) b) (int (MOD public) c)" + NewLine, found );
        }

        [TestMethod]
        public void TestCopySemanticsLists() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "tokens {MOD;}\n" +
                "a : ID (',' ID)* ';' -> ID+ ID+ ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                        "a", "a,b,c;", debug );
            assertEquals( "a b c a b c" + NewLine, found );
        }

        [TestMethod]
        public void TestCopyRuleLabel() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "tokens {BLOCK;}\n" +
                "a : x=b -> $x $x;\n" +
                "b : ID ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                        "a", "a", debug );
            assertEquals( "a a" + NewLine, found );
        }

        [TestMethod]
        public void TestCopyRuleLabel2() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "tokens {BLOCK;}\n" +
                "a : x=b -> ^($x $x);\n" +
                "b : ID ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                        "a", "a", debug );
            assertEquals( "(a a)" + NewLine, found );
        }

        [TestMethod]
        public void TestQueueingOfTokens() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : 'int' ID (',' ID)* ';' -> ^('int' ID+) ;\n" +
                "op : '+'|'-' ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                        "a", "int a,b,c;", debug );
            assertEquals( "(int a b c)" + NewLine, found );
        }

        [TestMethod]
        public void TestCopyOfTokens() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : 'int' ID ';' -> 'int' ID 'int' ID ;\n" +
                "op : '+'|'-' ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                        "a", "int a;", debug );
            assertEquals( "int a int a" + NewLine, found );
        }

        [TestMethod]
        public void TestTokenCopyInLoop() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : 'int' ID (',' ID)* ';' -> ^('int' ID)+ ;\n" +
                "op : '+'|'-' ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                        "a", "int a,b,c;", debug );
            assertEquals( "(int a) (int b) (int c)" + NewLine, found );
        }

        [TestMethod]
        public void TestTokenCopyInLoopAgainstTwoOthers() /*throws Exception*/ {
            // must smear 'int' copies across as root of multiple trees
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : 'int' ID ':' INT (',' ID ':' INT)* ';' -> ^('int' ID INT)+ ;\n" +
                "op : '+'|'-' ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                        "a", "int a:1,b:2,c:3;", debug );
            assertEquals( "(int a 1) (int b 2) (int c 3)" + NewLine, found );
        }

        [TestMethod]
        public void TestListRefdOneAtATime() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ID+ -> ID ID ID ;\n" + // works if 3 input IDs
                "op : '+'|'-' ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                        "a", "a b c", debug );
            assertEquals( "a b c" + NewLine, found );
        }

        [TestMethod]
        public void TestSplitListWithLabels() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "tokens {VAR;}\n" +
                "a : first=ID others+=ID* -> $first VAR $others+ ;\n" +
                "op : '+'|'-' ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                        "a", "a b c", debug );
            assertEquals( "a VAR b c" + NewLine, found );
        }

        [TestMethod]
        public void TestComplicatedMelange() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "tokens {BLOCK;}\n" +
                "a : A A b=B B b=B c+=C C c+=C D {String s=$D.text;} -> A+ B+ C+ D ;\n" +
                "type : 'int' | 'float' ;\n" +
                "A : 'a' ;\n" +
                "B : 'b' ;\n" +
                "C : 'c' ;\n" +
                "D : 'd' ;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                        "a", "a a b b b c c c d", debug );
            assertEquals( "a a b b b c c c d" + NewLine, found );
        }

        [TestMethod]
        public void TestRuleLabel() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "tokens {BLOCK;}\n" +
                "a : x=b -> $x;\n" +
                "b : ID ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                        "a", "a", debug );
            assertEquals( "a" + NewLine, found );
        }

        [TestMethod]
        public void TestAmbiguousRule() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ID a -> a | INT ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT: '0'..'9'+ ;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                        "a", "abc 34", debug );
            assertEquals( "34" + NewLine, found );
        }

        [TestMethod]
        public void TestWeirdRuleRef() /*throws Exception*/ {
            ErrorQueue equeue = new ErrorQueue();
            ErrorManager.SetErrorListener( equeue );
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ID a -> $a | INT ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT: '0'..'9'+ ;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";

            Grammar g = new Grammar( grammar );
            AntlrTool antlr = newTool();
            antlr.SetOutputDirectory( null ); // write to /dev/null
            CodeGenerator generator = new CodeGenerator( antlr, g, "Java" );
            g.CodeGenerator = generator;
            generator.GenRecognizer();

            // $a is ambig; is it previous root or ref to a ref in alt?
            assertEquals( "unexpected errors: " + equeue, 1, equeue.errors.Count );
        }

        [TestMethod]
        public void TestRuleListLabel() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "tokens {BLOCK;}\n" +
                "a : x+=b x+=b -> $x+;\n" +
                "b : ID ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                        "a", "a b", debug );
            assertEquals( "a b" + NewLine, found );
        }

        [TestMethod]
        public void TestRuleListLabel2() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "tokens {BLOCK;}\n" +
                "a : x+=b x+=b -> $x $x*;\n" +
                "b : ID ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                        "a", "a b", debug );
            assertEquals( "a b" + NewLine, found );
        }

        [TestMethod]
        public void TestOptional() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "tokens {BLOCK;}\n" +
                "a : x=b (y=b)? -> $x $y?;\n" +
                "b : ID ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                        "a", "a", debug );
            assertEquals( "a" + NewLine, found );
        }

        [TestMethod]
        public void TestOptional2() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "tokens {BLOCK;}\n" +
                "a : x=ID (y=b)? -> $x $y?;\n" +
                "b : ID ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                        "a", "a b", debug );
            assertEquals( "a b" + NewLine, found );
        }

        [TestMethod]
        public void TestOptional3() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "tokens {BLOCK;}\n" +
                "a : x=ID (y=b)? -> ($x $y)?;\n" +
                "b : ID ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                        "a", "a b", debug );
            assertEquals( "a b" + NewLine, found );
        }

        [TestMethod]
        public void TestOptional4() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "tokens {BLOCK;}\n" +
                "a : x+=ID (y=b)? -> ($x $y)?;\n" +
                "b : ID ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                        "a", "a b", debug );
            assertEquals( "a b" + NewLine, found );
        }

        [TestMethod]
        public void TestOptional5() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "tokens {BLOCK;}\n" +
                "a : ID -> ID? ;\n" + // match an ID to optional ID
                "b : ID ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                        "a", "a", debug );
            assertEquals( "a" + NewLine, found );
        }

        [TestMethod]
        public void TestArbitraryExprType() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "tokens {BLOCK;}\n" +
                "a : x+=b x+=b -> {new CommonTree()};\n" +
                "b : ID ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                        "a", "a b", debug );
            assertEquals( "", found );
        }

        [TestMethod]
        public void TestSet() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options { output = AST; } \n" +
                "a: (INT|ID)+ -> INT+ ID+ ;\n" +
                "INT: '0'..'9'+;\n" +
                "ID : 'a'..'z'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                        "a", "2 a 34 de", debug );
            assertEquals( "2 34 a de" + NewLine, found );
        }

        [TestMethod]
        public void TestSet2() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options { output = AST; } \n" +
                "a: (INT|ID) -> INT? ID? ;\n" +
                "INT: '0'..'9'+;\n" +
                "ID : 'a'..'z'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                        "a", "2", debug );
            assertEquals( "2" + NewLine, found );
        }

        [TestMethod]
        public void TestSetWithLabel() /*throws Exception*/ {
            // FAILS. The should probably generate a warning from antlr
            // See http://www.antlr.org:8888/browse/ANTLR-162
            string grammar =
                "grammar T;\n" +
                "options { output = AST; } \n" +
                "a : x=(INT|ID) -> $x ;\n" +
                "INT: '0'..'9'+;\n" +
                "ID : 'a'..'z'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                        "a", "2", debug );
            Assert.Inconclusive( "Known to fail." );
            assertEquals( "2" + NewLine, found );
        }

        [TestMethod]
        public void TestRewriteAction() /*throws Exception*/ {
            string grammar =
                "grammar T; \n" +
                "options { output = AST; }\n" +
                "tokens { FLOAT; }\n" +
                "r\n" +
                "    : INT -> {new CommonTree(new CommonToken(FLOAT,$INT.text+\".0\"))} \n" +
                "    ; \n" +
                "INT : '0'..'9'+; \n" +
                "WS: (' ' | '\\n' | '\\t')+ {$channel = HIDDEN;}; \n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                        "r", "25", debug );
            assertEquals( "25.0" + NewLine, found );
        }

        [TestMethod]
        public void TestOptionalSubruleWithoutRealElements() /*throws Exception*/ {
            // copy type *and* modifier even though it's optional
            // for each invocation of (...)+ in rewrite
            string grammar =
                "grammar T;\n" +
                "options {output=AST;} \n" +
                "tokens {PARMS;} \n" +
                "\n" +
                "modulo \n" +
                " : 'modulo' ID ('(' parms+ ')')? -> ^('modulo' ID ^(PARMS parms+)?) \n" +
                " ; \n" +
                "parms : '#'|ID; \n" +
                "ID : ('a'..'z' | 'A'..'Z')+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                        "modulo", "modulo abc (x y #)", debug );
            assertEquals( "(modulo abc (PARMS x y #))" + NewLine, found );
        }

        // C A R D I N A L I T Y  I S S U E S

        [TestMethod]
        public void TestCardinality() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "tokens {BLOCK;}\n" +
                "a : ID ID INT INT INT -> (ID INT)+;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+; \n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            execParser( "T.g", grammar, "TParser", "TLexer",
                        "a", "a b 3 4 5", debug );
            string expecting =
                "org.antlr.runtime.tree.RewriteCardinalityException: token ID";
            string found = getFirstLineOfException();
            assertEquals( expecting, found );
        }

        [TestMethod]
        public void TestCardinality2() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ID+ -> ID ID ID ;\n" + // only 2 input IDs
                "op : '+'|'-' ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            execParser( "T.g", grammar, "TParser", "TLexer",
                       "a", "a b", debug );
            string expecting =
                "org.antlr.runtime.tree.RewriteCardinalityException: token ID";
            string found = getFirstLineOfException();
            assertEquals( expecting, found );
        }

        [TestMethod]
        public void TestCardinality3() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ID? INT -> ID INT ;\n" +
                "op : '+'|'-' ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            execParser( "T.g", grammar, "TParser", "TLexer",
                       "a", "3", debug );
            string expecting =
                "org.antlr.runtime.tree.RewriteEmptyStreamException: token ID";
            string found = getFirstLineOfException();
            assertEquals( expecting, found );
        }

        [TestMethod]
        public void TestLoopCardinality() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ID? INT -> ID+ INT ;\n" +
                "op : '+'|'-' ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            execParser( "T.g", grammar, "TParser", "TLexer",
                       "a", "3", debug );
            string expecting =
                "org.antlr.runtime.tree.RewriteEarlyExitException";
            string found = getFirstLineOfException();
            assertEquals( expecting, found );
        }

        [TestMethod]
        public void TestWildcard() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ID c=. -> $c;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                        "a", "abc 34", debug );
            assertEquals( "34" + NewLine, found );
        }

        // E R R O R S

        [TestMethod]
        public void TestUnknownRule() /*throws Exception*/ {
            ErrorQueue equeue = new ErrorQueue();
            ErrorManager.SetErrorListener( equeue );

            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : INT -> ugh ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";

            Grammar g = new Grammar( grammar );
            AntlrTool antlr = newTool();
            antlr.SetOutputDirectory( null ); // write to /dev/null
            CodeGenerator generator = new CodeGenerator( antlr, g, "Java" );
            g.CodeGenerator = generator;
            generator.GenRecognizer();

            int expectedMsgID = ErrorManager.MSG_UNDEFINED_RULE_REF;
            object expectedArg = "ugh";
            object expectedArg2 = null;
            GrammarSemanticsMessage expectedMessage =
                new GrammarSemanticsMessage( expectedMsgID, g, null, expectedArg, expectedArg2 );

            checkError( equeue, expectedMessage );
        }

        [TestMethod]
        public void TestKnownRuleButNotInLHS() /*throws Exception*/ {
            ErrorQueue equeue = new ErrorQueue();
            ErrorManager.SetErrorListener( equeue );

            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : INT -> b ;\n" +
                "b : 'b' ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";

            Grammar g = new Grammar( grammar );
            AntlrTool antlr = newTool();
            antlr.SetOutputDirectory( null ); // write to /dev/null
            CodeGenerator generator = new CodeGenerator( antlr, g, "Java" );
            g.CodeGenerator = generator;
            generator.GenRecognizer();

            int expectedMsgID = ErrorManager.MSG_REWRITE_ELEMENT_NOT_PRESENT_ON_LHS;
            object expectedArg = "b";
            object expectedArg2 = null;
            GrammarSemanticsMessage expectedMessage =
                new GrammarSemanticsMessage( expectedMsgID, g, null, expectedArg, expectedArg2 );

            checkError( equeue, expectedMessage );
        }

        [TestMethod]
        public void TestUnknownToken() /*throws Exception*/ {
            ErrorQueue equeue = new ErrorQueue();
            ErrorManager.SetErrorListener( equeue );

            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : INT -> ICK ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";

            Grammar g = new Grammar( grammar );
            AntlrTool antlr = newTool();
            antlr.SetOutputDirectory( null ); // write to /dev/null
            CodeGenerator generator = new CodeGenerator( antlr, g, "Java" );
            g.CodeGenerator = generator;
            generator.GenRecognizer();

            int expectedMsgID = ErrorManager.MSG_UNDEFINED_TOKEN_REF_IN_REWRITE;
            object expectedArg = "ICK";
            object expectedArg2 = null;
            GrammarSemanticsMessage expectedMessage =
                new GrammarSemanticsMessage( expectedMsgID, g, null, expectedArg, expectedArg2 );

            checkError( equeue, expectedMessage );
        }

        [TestMethod]
        public void TestUnknownLabel() /*throws Exception*/ {
            ErrorQueue equeue = new ErrorQueue();
            ErrorManager.SetErrorListener( equeue );

            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : INT -> $foo ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";

            Grammar g = new Grammar( grammar );
            AntlrTool antlr = newTool();
            antlr.SetOutputDirectory( null ); // write to /dev/null
            CodeGenerator generator = new CodeGenerator( antlr, g, "Java" );
            g.CodeGenerator = generator;
            generator.GenRecognizer();

            int expectedMsgID = ErrorManager.MSG_UNDEFINED_LABEL_REF_IN_REWRITE;
            object expectedArg = "foo";
            object expectedArg2 = null;
            GrammarSemanticsMessage expectedMessage =
                new GrammarSemanticsMessage( expectedMsgID, g, null, expectedArg, expectedArg2 );

            checkError( equeue, expectedMessage );
        }

        [TestMethod]
        public void TestUnknownCharLiteralToken() /*throws Exception*/ {
            ErrorQueue equeue = new ErrorQueue();
            ErrorManager.SetErrorListener( equeue );

            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : INT -> 'a' ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";

            Grammar g = new Grammar( grammar );
            AntlrTool antlr = newTool();
            antlr.SetOutputDirectory( null ); // write to /dev/null
            CodeGenerator generator = new CodeGenerator( antlr, g, "Java" );
            g.CodeGenerator = generator;
            generator.GenRecognizer();

            int expectedMsgID = ErrorManager.MSG_UNDEFINED_TOKEN_REF_IN_REWRITE;
            object expectedArg = "'a'";
            object expectedArg2 = null;
            GrammarSemanticsMessage expectedMessage =
                new GrammarSemanticsMessage( expectedMsgID, g, null, expectedArg, expectedArg2 );

            checkError( equeue, expectedMessage );
        }

        [TestMethod]
        public void TestUnknownStringLiteralToken() /*throws Exception*/ {
            ErrorQueue equeue = new ErrorQueue();
            ErrorManager.SetErrorListener( equeue );

            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : INT -> 'foo' ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";

            Grammar g = new Grammar( grammar );
            AntlrTool antlr = newTool();
            antlr.SetOutputDirectory( null ); // write to /dev/null
            CodeGenerator generator = new CodeGenerator( antlr, g, "Java" );
            g.CodeGenerator = generator;
            generator.GenRecognizer();

            int expectedMsgID = ErrorManager.MSG_UNDEFINED_TOKEN_REF_IN_REWRITE;
            object expectedArg = "'foo'";
            object expectedArg2 = null;
            GrammarSemanticsMessage expectedMessage =
                new GrammarSemanticsMessage( expectedMsgID, g, null, expectedArg, expectedArg2 );

            checkError( equeue, expectedMessage );
        }

        [TestMethod]
        public void TestExtraTokenInSimpleDecl() /*throws Exception*/ {
            string grammar =
                "grammar foo;\n" +
                "options {output=AST;}\n" +
                "tokens {EXPR;}\n" +
                "decl : type ID '=' INT ';' -> ^(EXPR type ID INT) ;\n" +
                "type : 'int' | 'float' ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "foo.g", grammar, "fooParser", "fooLexer",
                                      "decl", "int 34 x=1;", debug );
            assertEquals( "line 1:4 extraneous input '34' expecting ID" + NewLine, this.stderrDuringParse );
            assertEquals( "(EXPR int x 1)" + NewLine, found ); // tree gets correct x and 1 tokens
        }

        [TestMethod]
        public void TestMissingIDInSimpleDecl() /*throws Exception*/ {
            string grammar =
                "grammar foo;\n" +
                "options {output=AST;}\n" +
                "tokens {EXPR;}\n" +
                "decl : type ID '=' INT ';' -> ^(EXPR type ID INT) ;\n" +
                "type : 'int' | 'float' ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "foo.g", grammar, "fooParser", "fooLexer",
                                      "decl", "int =1;", debug );
            assertEquals( "line 1:4 missing ID at '='" + NewLine, this.stderrDuringParse );
            assertEquals( "(EXPR int <missing ID> 1)" + NewLine, found ); // tree gets invented ID token
        }

        [TestMethod]
        public void TestMissingSetInSimpleDecl() /*throws Exception*/ {
            string grammar =
                "grammar foo;\n" +
                "options {output=AST;}\n" +
                "tokens {EXPR;}\n" +
                "decl : type ID '=' INT ';' -> ^(EXPR type ID INT) ;\n" +
                "type : 'int' | 'float' ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "foo.g", grammar, "fooParser", "fooLexer",
                                      "decl", "x=1;", debug );
            assertEquals( "line 1:0 mismatched input 'x' expecting set null" + NewLine, this.stderrDuringParse );
            assertEquals( "(EXPR <error: x> x 1)" + NewLine, found ); // tree gets invented ID token
        }

        [TestMethod]
        public void TestMissingTokenGivesErrorNode() /*throws Exception*/ {
            string grammar =
                "grammar foo;\n" +
                "options {output=AST;}\n" +
                "a : ID INT -> ID INT ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "foo.g", grammar, "fooParser", "fooLexer",
                                      "a", "abc", debug );
            assertEquals( "line 0:-1 missing INT at '<EOF>'" + NewLine, this.stderrDuringParse );
            // doesn't do in-line recovery for sets (yet?)
            assertEquals( "abc <missing INT>" + NewLine, found );
        }

        [TestMethod]
        public void TestExtraTokenGivesErrorNode() /*throws Exception*/ {
            string grammar =
                "grammar foo;\n" +
                "options {output=AST;}\n" +
                "a : b c -> b c;\n" +
                "b : ID -> ID ;\n" +
                "c : INT -> INT ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "foo.g", grammar, "fooParser", "fooLexer",
                                      "a", "abc ick 34", debug );
            assertEquals( "line 1:4 extraneous input 'ick' expecting INT" + NewLine, this.stderrDuringParse );
            assertEquals( "abc 34" + NewLine, found );
        }

        [TestMethod]
        public void TestMissingFirstTokenGivesErrorNode() /*throws Exception*/ {
            string grammar =
                "grammar foo;\n" +
                "options {output=AST;}\n" +
                "a : ID INT -> ID INT ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "foo.g", grammar, "fooParser", "fooLexer",
                                      "a", "34", debug );
            assertEquals( "line 1:0 missing ID at '34'" + NewLine, this.stderrDuringParse );
            assertEquals( "<missing ID> 34" + NewLine, found );
        }

        [TestMethod]
        public void TestMissingFirstTokenGivesErrorNode2() /*throws Exception*/ {
            string grammar =
                "grammar foo;\n" +
                "options {output=AST;}\n" +
                "a : b c -> b c;\n" +
                "b : ID -> ID ;\n" +
                "c : INT -> INT ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "foo.g", grammar, "fooParser", "fooLexer",
                                      "a", "34", debug );
            // finds an error at the first token, 34, and re-syncs.
            // re-synchronizing does not consume a token because 34 follows
            // ref to rule b (start of c). It then matches 34 in c.
            assertEquals( "line 1:0 missing ID at '34'" + NewLine, this.stderrDuringParse );
            assertEquals( "<missing ID> 34" + NewLine, found );
        }

        [TestMethod]
        public void TestNoViableAltGivesErrorNode() /*throws Exception*/ {
            string grammar =
                "grammar foo;\n" +
                "options {output=AST;}\n" +
                "a : b -> b | c -> c;\n" +
                "b : ID -> ID ;\n" +
                "c : INT -> INT ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "S : '*' ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "foo.g", grammar, "fooParser", "fooLexer",
                                      "a", "*", debug );
            // finds an error at the first token, 34, and re-syncs.
            // re-synchronizing does not consume a token because 34 follows
            // ref to rule b (start of c). It then matches 34 in c.
            assertEquals( "line 1:0 no viable alternative at input '*'" + NewLine, this.stderrDuringParse );
            assertEquals( "<unexpected: [@0,0:0='*',<6>,1:0], resync=*>" + NewLine, found );
        }

    }
}
