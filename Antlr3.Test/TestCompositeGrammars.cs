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

    using AntlrTool = Antlr3.AntlrTool;
    using Regex = System.Text.RegularExpressions.Regex;

    [TestClass]
    public class TestCompositeGrammars : BaseTest
    {
        protected bool debug = false;

        [TestMethod]
        public void TestWildcardStillWorks() /*throws Exception*/ {
            ErrorQueue equeue = new ErrorQueue();
            ErrorManager.SetErrorListener( equeue );
            string grammar =
                "parser grammar S;\n" +
                "a : B . C ;\n"; // not qualified ID
            Grammar g = new Grammar( grammar );
            Assert.AreEqual(0, equeue.errors.Count, "unexpected errors: " + equeue);
        }

        [TestMethod]
        public void TestDelegatorInvokesDelegateRule() /*throws Exception*/ {
            string slave =
                "parser grammar S;\n" +
                "a : B {System.out.println(\"S.a\");} ;\n";
            mkdir( tmpdir );
            writeFile( tmpdir, "S.g", slave );
            string master =
                "grammar M;\n" +
                "import S;\n" +
                "s : a ;\n" +
                "B : 'b' ;" + // defines B from inherited token space
                "WS : (' '|'\\n') {skip();} ;\n";
            string found = execParser( "M.g", master, "MParser", "MLexer",
                                      "s", "b", debug );
            Assert.AreEqual( "S.a" + NewLine, found );
        }

        [TestMethod]
        public void TestDelegatorInvokesDelegateRuleWithArgs() /*throws Exception*/ {
            // must generate something like:
            // public int a(int x) throws RecognitionException { return gS.a(x); }
            // in M.
            string slave =
                "parser grammar S;\n" +
                "a[int x] returns [int y] : B {System.out.print(\"S.a\"); $y=1000;} ;\n";
            mkdir( tmpdir );
            writeFile( tmpdir, "S.g", slave );
            string master =
                "grammar M;\n" +
                "import S;\n" +
                "s : label=a[3] {System.out.println($label.y);} ;\n" +
                "B : 'b' ;" + // defines B from inherited token space
                "WS : (' '|'\\n') {skip();} ;\n";
            string found = execParser( "M.g", master, "MParser", "MLexer",
                                      "s", "b", debug );
            Assert.AreEqual( "S.a1000" + NewLine, found );
        }

        [TestMethod]
        public void TestDelegatorInvokesDelegateRuleWithReturnStruct() /*throws Exception*/ {
            // must generate something like:
            // public int a(int x) throws RecognitionException { return gS.a(x); }
            // in M.
            string slave =
                "parser grammar S;\n" +
                "a : B {System.out.print(\"S.a\");} ;\n";
            mkdir( tmpdir );
            writeFile( tmpdir, "S.g", slave );
            string master =
                "grammar M;\n" +
                "import S;\n" +
                "s : a {System.out.println($a.text);} ;\n" +
                "B : 'b' ;" + // defines B from inherited token space
                "WS : (' '|'\\n') {skip();} ;\n";
            string found = execParser( "M.g", master, "MParser", "MLexer",
                                      "s", "b", debug );
            Assert.AreEqual( "S.ab" + NewLine, found );
        }

        [TestMethod]
        public void TestDelegatorAccessesDelegateMembers() /*throws Exception*/ {
            string slave =
                "parser grammar S;\n" +
                "@members {\n" +
                "  public void foo() {System.out.println(\"foo\");}\n" +
                "}\n" +
                "a : B ;\n";
            mkdir( tmpdir );
            writeFile( tmpdir, "S.g", slave );
            string master =
                "grammar M;\n" +		// uses no rules from the import
                "import S;\n" +
                "s : 'b' {gS.foo();} ;\n" + // gS is import pointer
                "WS : (' '|'\\n') {skip();} ;\n";
            string found = execParser( "M.g", master, "MParser", "MLexer",
                                      "s", "b", debug );
            Assert.AreEqual( "foo" + NewLine, found );
        }

        [TestMethod]
        public void TestDelegatorInvokesFirstVersionOfDelegateRule() /*throws Exception*/ {
            string slave =
                "parser grammar S;\n" +
                "a : b {System.out.println(\"S.a\");} ;\n" +
                "b : B ;\n";
            mkdir( tmpdir );
            writeFile( tmpdir, "S.g", slave );
            string slave2 =
                "parser grammar T;\n" +
                "a : B {System.out.println(\"T.a\");} ;\n"; // hidden by S.a
            writeFile( tmpdir, "T.g", slave2 );
            string master =
                "grammar M;\n" +
                "import S,T;\n" +
                "s : a ;\n" +
                "B : 'b' ;\n" +
                "WS : (' '|'\\n') {skip();} ;\n";
            string found = execParser( "M.g", master, "MParser", "MLexer",
                                      "s", "b", debug );
            Assert.AreEqual( "S.a" + NewLine, found );
        }

        [TestMethod]
        public void TestDelegatesSeeSameTokenType() /*throws Exception*/ {
            string slave =
                "parser grammar S;\n" + // A, B, C token type order
                "tokens { A; B; C; }\n" +
                "x : A {System.out.println(\"S.x\");} ;\n";
            mkdir( tmpdir );
            writeFile( tmpdir, "S.g", slave );
            string slave2 =
                "parser grammar T;\n" +
                "tokens { C; B; A; }\n" + // reverse order
                "y : A {System.out.println(\"T.y\");} ;\n";
            mkdir( tmpdir );
            writeFile( tmpdir, "T.g", slave2 );
            // The lexer will create rules to match letters a, b, c.
            // The associated token types A, B, C must have the same value
            // and all import'd parsers.  Since ANTLR regenerates all imports
            // for use with the delegator M, it can generate the same token type
            // mapping in each parser:
            // public static final int C=6;
            // public static final int EOF=-1;
            // public static final int B=5;
            // public static final int WS=7;
            // public static final int A=4;

            string master =
                "grammar M;\n" +
                "import S,T;\n" +
                "s : x y ;\n" + // matches AA, which should be "aa"
                "B : 'b' ;\n" + // another order: B, A, C
                "A : 'a' ;\n" +
                "C : 'c' ;\n" +
                "WS : (' '|'\\n') {skip();} ;\n";
            string found = execParser( "M.g", master, "MParser", "MLexer",
                                      "s", "aa", debug );
            Assert.AreEqual( "S.x" + NewLine +
                         "T.y" + NewLine, found );
        }

        [TestMethod]
        public void TestDelegatesSeeSameTokenType2() /*throws Exception*/ {
            ErrorQueue equeue = new ErrorQueue();
            ErrorManager.SetErrorListener( equeue );
            string slave =
                "parser grammar S;\n" + // A, B, C token type order
                "tokens { A; B; C; }\n" +
                "x : A {System.out.println(\"S.x\");} ;\n";
            mkdir( tmpdir );
            writeFile( tmpdir, "S.g", slave );
            string slave2 =
                "parser grammar T;\n" +
                "tokens { C; B; A; }\n" + // reverse order
                "y : A {System.out.println(\"T.y\");} ;\n";
            mkdir( tmpdir );
            writeFile( tmpdir, "T.g", slave2 );

            string master =
                "grammar M;\n" +
                "import S,T;\n" +
                "s : x y ;\n" + // matches AA, which should be "aa"
                "B : 'b' ;\n" + // another order: B, A, C
                "A : 'a' ;\n" +
                "C : 'c' ;\n" +
                "WS : (' '|'\\n') {skip();} ;\n";
            writeFile( tmpdir, "M.g", master );
            AntlrTool antlr = newTool( new string[] { "-lib", tmpdir } );
            CompositeGrammar composite = new CompositeGrammar();
            Grammar g = new Grammar( antlr, tmpdir + "/M.g", composite );
            composite.SetDelegationRoot( g );
            g.ParseAndBuildAST();
            g.composite.AssignTokenTypes();

            string expectedTokenIDToTypeMap = "[A=4, B=5, C=6, WS=7]";
            string expectedStringLiteralToTypeMap = "{}";
            string expectedTypeToTokenList = "[A, B, C, WS]";

            Assert.AreEqual( expectedTokenIDToTypeMap,
                         realElements( g.composite.TokenIDToTypeMap ).ToElementString() );
            Assert.AreEqual( expectedStringLiteralToTypeMap, g.composite.StringLiteralToTypeMap.ToElementString() );
            Assert.AreEqual( expectedTypeToTokenList,
                         realElements( g.composite.TypeToTokenList ).ToElementString() );

            Assert.AreEqual(0, equeue.errors.Count, "unexpected errors: " + equeue);
        }

        [TestMethod]
        public void TestCombinedImportsCombined() /*throws Exception*/ {
            //Assert.Inconclusive( "May be failing on just my port..." );
            // for now, we don't allow combined to import combined
            ErrorQueue equeue = new ErrorQueue();
            ErrorManager.SetErrorListener( equeue );
            string slave =
                "grammar S;\n" + // A, B, C token type order
                "tokens { A; B; C; }\n" +
                "x : 'x' INT {System.out.println(\"S.x\");} ;\n" +
                "INT : '0'..'9'+ ;\n" +
                "WS : (' '|'\\n') {skip();} ;\n";
            mkdir( tmpdir );
            writeFile( tmpdir, "S.g", slave );

            string master =
                "grammar M;\n" +
                "import S;\n" +
                "s : x INT ;\n";
            writeFile( tmpdir, "M.g", master );
            AntlrTool antlr = newTool( new string[] { "-lib", tmpdir } );
            CompositeGrammar composite = new CompositeGrammar();
            Grammar g = new Grammar( antlr, tmpdir + "/M.g", composite );
            composite.SetDelegationRoot( g );
            g.ParseAndBuildAST();
            g.composite.AssignTokenTypes();

            Assert.AreEqual(1, equeue.errors.Count, "unexpected errors: " + equeue);
            string expectedError = "error(161): " + Regex.Replace(tmpdir.ToString(), "\\-[0-9]+", "") + "/M.g:2:8: combined grammar M cannot import combined grammar S";
            Assert.AreEqual(expectedError, Regex.Replace(equeue.errors[0].ToString(), "\\-[0-9]+", ""), "unexpected errors: " + equeue);
        }

        [TestMethod]
        public void TestSameStringTwoNames() /*throws Exception*/ {
            ErrorQueue equeue = new ErrorQueue();
            ErrorManager.SetErrorListener( equeue );
            string slave =
                "parser grammar S;\n" +
                "tokens { A='a'; }\n" +
                "x : A {System.out.println(\"S.x\");} ;\n";
            mkdir( tmpdir );
            writeFile( tmpdir, "S.g", slave );
            string slave2 =
                "parser grammar T;\n" +
                "tokens { X='a'; }\n" +
                "y : X {System.out.println(\"T.y\");} ;\n";
            mkdir( tmpdir );
            writeFile( tmpdir, "T.g", slave2 );

            string master =
                "grammar M;\n" +
                "import S,T;\n" +
                "s : x y ;\n" +
                "WS : (' '|'\\n') {skip();} ;\n";
            writeFile( tmpdir, "M.g", master );
            AntlrTool antlr = newTool( new string[] { "-lib", tmpdir } );
            CompositeGrammar composite = new CompositeGrammar();
            Grammar g = new Grammar( antlr, tmpdir + "/M.g", composite );
            composite.SetDelegationRoot( g );
            g.ParseAndBuildAST();
            g.composite.AssignTokenTypes();

            string expectedTokenIDToTypeMap = "[A=4, WS=5, X=6]";
            string expectedStringLiteralToTypeMap = "{'a'=4}";
            string expectedTypeToTokenList = "[A, WS, X]";

            Assert.AreEqual( expectedTokenIDToTypeMap,
                         realElements( g.composite.TokenIDToTypeMap ).ToElementString() );
            Assert.AreEqual( expectedStringLiteralToTypeMap, g.composite.StringLiteralToTypeMap.ToElementString() );
            Assert.AreEqual( expectedTypeToTokenList,
                         realElements( g.composite.TypeToTokenList ).ToElementString() );

            object expectedArg = "X='a'";
            object expectedArg2 = "A";
            int expectedMsgID = ErrorManager.MSG_TOKEN_ALIAS_CONFLICT;
            GrammarSemanticsMessage expectedMessage =
                new GrammarSemanticsMessage( expectedMsgID, g, null, expectedArg, expectedArg2 );
            checkGrammarSemanticsError( equeue, expectedMessage );

            Assert.AreEqual(1, equeue.errors.Count, "unexpected errors: " + equeue);

            string expectedError =
                "error(158): T.g:2:10: cannot alias X='a'; string already assigned to A";
            Assert.AreEqual( expectedError, equeue.errors[0].ToString() );
        }

        [TestMethod]
        public void TestSameNameTwoStrings() /*throws Exception*/ {
            ErrorQueue equeue = new ErrorQueue();
            ErrorManager.SetErrorListener( equeue );
            string slave =
                "parser grammar S;\n" +
                "tokens { A='a'; }\n" +
                "x : A {System.out.println(\"S.x\");} ;\n";
            mkdir( tmpdir );
            writeFile( tmpdir, "S.g", slave );
            string slave2 =
                "parser grammar T;\n" +
                "tokens { A='x'; }\n" +
                "y : A {System.out.println(\"T.y\");} ;\n";
            writeFile( tmpdir, "T.g", slave2 );

            string master =
                "grammar M;\n" +
                "import S,T;\n" +
                "s : x y ;\n" +
                "WS : (' '|'\\n') {skip();} ;\n";
            writeFile( tmpdir, "M.g", master );
            AntlrTool antlr = newTool( new string[] { "-lib", tmpdir } );
            CompositeGrammar composite = new CompositeGrammar();
            Grammar g = new Grammar( antlr, tmpdir + "/M.g", composite );
            composite.SetDelegationRoot( g );
            g.ParseAndBuildAST();
            g.composite.AssignTokenTypes();

            string expectedTokenIDToTypeMap = "[A=4, T__6=6, WS=5]";
            string expectedStringLiteralToTypeMap = "{'a'=4, 'x'=6}";
            string expectedTypeToTokenList = "[A, WS, T__6]";

            Assert.AreEqual( expectedTokenIDToTypeMap,
                         realElements( g.composite.TokenIDToTypeMap ).ToElementString() );
            Assert.AreEqual( expectedStringLiteralToTypeMap, sortMapToString( g.composite.StringLiteralToTypeMap ) );
            Assert.AreEqual( expectedTypeToTokenList,
                         realElements( g.composite.TypeToTokenList ).ToElementString() );

            object expectedArg = "A='x'";
            object expectedArg2 = "'a'";
            int expectedMsgID = ErrorManager.MSG_TOKEN_ALIAS_REASSIGNMENT;
            GrammarSemanticsMessage expectedMessage =
                new GrammarSemanticsMessage( expectedMsgID, g, null, expectedArg, expectedArg2 );
            checkGrammarSemanticsError( equeue, expectedMessage );

            Assert.AreEqual(1, equeue.errors.Count, "unexpected errors: " + equeue);

            string expectedError =
                "error(159): T.g:2:10: cannot alias A='x'; token name already assigned to 'a'";
            Assert.AreEqual( expectedError, equeue.errors[0].ToString() );
        }

        [TestMethod]
        public void TestImportedTokenVocabIgnoredWithWarning() /*throws Exception*/ {
            ErrorQueue equeue = new ErrorQueue();
            ErrorManager.SetErrorListener( equeue );
            string slave =
                "parser grammar S;\n" +
                "options {tokenVocab=whatever;}\n" +
                "tokens { A='a'; }\n" +
                "x : A {System.out.println(\"S.x\");} ;\n";
            mkdir( tmpdir );
            writeFile( tmpdir, "S.g", slave );

            string master =
                "grammar M;\n" +
                "import S;\n" +
                "s : x ;\n" +
                "WS : (' '|'\\n') {skip();} ;\n";
            writeFile( tmpdir, "M.g", master );
            AntlrTool antlr = newTool( new string[] { "-lib", tmpdir } );
            CompositeGrammar composite = new CompositeGrammar();
            Grammar g = new Grammar( antlr, tmpdir + "/M.g", composite );
            composite.SetDelegationRoot( g );
            g.ParseAndBuildAST();
            g.composite.AssignTokenTypes();

            object expectedArg = "S";
            int expectedMsgID = ErrorManager.MSG_TOKEN_VOCAB_IN_DELEGATE;
            GrammarSemanticsMessage expectedMessage =
                new GrammarSemanticsMessage( expectedMsgID, g, null, expectedArg );
            checkGrammarSemanticsWarning( equeue, expectedMessage );

            Assert.AreEqual(0, equeue.errors.Count, "unexpected errors: " + equeue);
            Assert.AreEqual(1, equeue.warnings.Count, "unexpected errors: " + equeue);

            string expectedError =
                "warning(160): S.g:2:10: tokenVocab option ignored in imported grammar S";
            Assert.AreEqual( expectedError, equeue.warnings[0].ToString() );
        }

        [TestMethod]
        public void TestImportedTokenVocabWorksInRoot() /*throws Exception*/ {
            ErrorQueue equeue = new ErrorQueue();
            ErrorManager.SetErrorListener( equeue );
            string slave =
                "parser grammar S;\n" +
                "tokens { A='a'; }\n" +
                "x : A {System.out.println(\"S.x\");} ;\n";
            mkdir( tmpdir );
            writeFile( tmpdir, "S.g", slave );

            string tokens =
                "A=99\n";
            writeFile( tmpdir, "Test.tokens", tokens );

            string master =
                "grammar M;\n" +
                "options {tokenVocab=Test;}\n" +
                "import S;\n" +
                "s : x ;\n" +
                "WS : (' '|'\\n') {skip();} ;\n";
            writeFile( tmpdir, "M.g", master );
            AntlrTool antlr = newTool( new string[] { "-lib", tmpdir } );
            CompositeGrammar composite = new CompositeGrammar();
            Grammar g = new Grammar( antlr, tmpdir + "/M.g", composite );
            composite.SetDelegationRoot( g );
            g.ParseAndBuildAST();
            g.composite.AssignTokenTypes();

            string expectedTokenIDToTypeMap = "[A=99, WS=101]";
            string expectedStringLiteralToTypeMap = "{'a'=100}";
            string expectedTypeToTokenList = "[A, 'a', WS]";

            Assert.AreEqual( expectedTokenIDToTypeMap,
                         realElements( g.composite.TokenIDToTypeMap ).ToElementString() );
            Assert.AreEqual( expectedStringLiteralToTypeMap, g.composite.StringLiteralToTypeMap.ToElementString() );
            Assert.AreEqual( expectedTypeToTokenList,
                         realElements( g.composite.TypeToTokenList ).ToElementString() );

            Assert.AreEqual(0, equeue.errors.Count, "unexpected errors: " + equeue);
        }

        [TestMethod]
        public void TestSyntaxErrorsInImportsNotThrownOut() /*throws Exception*/ {
            ErrorQueue equeue = new ErrorQueue();
            ErrorManager.SetErrorListener( equeue );
            string slave =
                "parser grammar S;\n" +
                "options {toke\n";
            mkdir( tmpdir );
            writeFile( tmpdir, "S.g", slave );

            string master =
                "grammar M;\n" +
                "import S;\n" +
                "s : x ;\n" +
                "WS : (' '|'\\n') {skip();} ;\n";
            writeFile( tmpdir, "M.g", master );
            AntlrTool antlr = newTool( new string[] { "-lib", tmpdir } );
            CompositeGrammar composite = new CompositeGrammar();
            Grammar g = new Grammar( antlr, tmpdir + "/M.g", composite );
            composite.SetDelegationRoot( g );
            g.ParseAndBuildAST();
            g.composite.AssignTokenTypes();

            // whole bunch of errors from bad S.g file
            Assert.AreEqual(5, equeue.errors.Count, "unexpected errors: " + equeue);
        }

        [TestMethod]
        public void TestSyntaxErrorsInImportsNotThrownOut2() /*throws Exception*/ {
            ErrorQueue equeue = new ErrorQueue();
            ErrorManager.SetErrorListener( equeue );
            string slave =
                "parser grammar S;\n" +
                ": A {System.out.println(\"S.x\");} ;\n";
            mkdir( tmpdir );
            writeFile( tmpdir, "S.g", slave );

            string master =
                "grammar M;\n" +
                "import S;\n" +
                "s : x ;\n" +
                "WS : (' '|'\\n') {skip();} ;\n";
            writeFile( tmpdir, "M.g", master );
            AntlrTool antlr = newTool( new string[] { "-lib", tmpdir } );
            CompositeGrammar composite = new CompositeGrammar();
            Grammar g = new Grammar( antlr, tmpdir + "/M.g", composite );
            composite.SetDelegationRoot( g );
            g.ParseAndBuildAST();
            g.composite.AssignTokenTypes();

            // whole bunch of errors from bad S.g file
            Assert.AreEqual(3, equeue.errors.Count, "unexpected errors: " + equeue);
        }

        [TestMethod]
        public void TestDelegatorRuleOverridesDelegate() /*throws Exception*/ {
            string slave =
                "parser grammar S;\n" +
                "a : b {System.out.println(\"S.a\");} ;\n" +
                "b : B ;\n";
            mkdir( tmpdir );
            writeFile( tmpdir, "S.g", slave );
            string master =
                "grammar M;\n" +
                "import S;\n" +
                "b : 'b'|'c' ;\n" +
                "WS : (' '|'\\n') {skip();} ;\n";
            string found = execParser( "M.g", master, "MParser", "MLexer",
                                      "a", "c", debug );
            Assert.AreEqual( "S.a" + NewLine, found );
        }

        [TestMethod]
        public void TestDelegatorRuleOverridesLookaheadInDelegate() /*throws Exception*/ {
            string slave =
                "parser grammar JavaDecl;\n" +
                "type : 'int' ;\n" +
                "decl : type ID ';'\n" +
                "     | type ID init ';' {System.out.println(\"JavaDecl: \"+$decl.text);}\n" +
                "     ;\n" +
                "init : '=' INT ;\n";
            mkdir( tmpdir );
            writeFile( tmpdir, "JavaDecl.g", slave );
            string master =
                "grammar Java;\n" +
                "import JavaDecl;\n" +
                "prog : decl ;\n" +
                "type : 'int' | 'float' ;\n" +
                "\n" +
                "ID  : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+ ;\n" +
                "WS : (' '|'\\n') {skip();} ;\n";
            // for float to work in decl, type must be overridden
            string found = execParser( "Java.g", master, "JavaParser", "JavaLexer",
                                      "prog", "float x = 3;", debug );
            Assert.AreEqual( "JavaDecl: floatx=3;" + NewLine, found );
        }

        // LEXER INHERITANCE

        [TestMethod]
        public void TestLexerDelegatorInvokesDelegateRule() /*throws Exception*/ {
            string slave =
                "lexer grammar S;\n" +
                "A : 'a' {System.out.println(\"S.A\");} ;\n" +
                "C : 'c' ;\n";
            mkdir( tmpdir );
            writeFile( tmpdir, "S.g", slave );
            string master =
                "lexer grammar M;\n" +
                "import S;\n" +
                "B : 'b' ;\n" +
                "WS : (' '|'\\n') {skip();} ;\n";
            string found = execLexer( "M.g", master, "M", "abc", debug );
            Assert.AreEqual( "S.A"+NewLine+"abc"+NewLine, found );
        }

        [TestMethod]
        public void TestLexerDelegatorRuleOverridesDelegate() /*throws Exception*/ {
            string slave =
                "lexer grammar S;\n" +
                "A : 'a' {System.out.println(\"S.A\");} ;\n" +
                "B : 'b' {System.out.println(\"S.B\");} ;\n";
            mkdir( tmpdir );
            writeFile( tmpdir, "S.g", slave );
            string master =
                "lexer grammar M;\n" +
                "import S;\n" +
                "A : 'a' B {System.out.println(\"M.A\");} ;\n" +
                "WS : (' '|'\\n') {skip();} ;\n";
            string found = execLexer( "M.g", master, "M", "ab", debug );
            Assert.AreEqual( "S.B" + NewLine +
                         "M.A" + NewLine +
                         "ab" + NewLine, found );
        }

        [TestMethod]
        public void TestLexerDelegatorRuleOverridesDelegateLeavingNoRules() /*throws Exception*/ {
            // M.Tokens has nothing to predict tokens from S.  Should
            // not include S.Tokens alt in this case?
            string slave =
                "lexer grammar S;\n" +
                "A : 'a' {System.out.println(\"S.A\");} ;\n";
            mkdir( tmpdir );
            writeFile( tmpdir, "S.g", slave );
            string master =
                "lexer grammar M;\n" +
                "import S;\n" +
                "A : 'a' {System.out.println(\"M.A\");} ;\n" +
                "WS : (' '|'\\n') {skip();} ;\n";
            writeFile( tmpdir, "M.g", master );

            ErrorQueue equeue = new ErrorQueue();
            ErrorManager.SetErrorListener( equeue );
            AntlrTool antlr = newTool( new string[] { "-lib", tmpdir } );
            CompositeGrammar composite = new CompositeGrammar();
            Grammar g = new Grammar( antlr, tmpdir + "/M.g", composite );
            composite.SetDelegationRoot( g );
            g.ParseAndBuildAST();
            composite.AssignTokenTypes();
            composite.DefineGrammarSymbols();
            composite.CreateNFAs();
            g.CreateLookaheadDFAs( false );

            // predict only alts from M not S
            string expectingDFA =
                ".s0-'a'->.s1" + NewLine +
                ".s0-{'\\n', ' '}->:s3=>2" + NewLine +
                ".s1-<EOT>->:s2=>1" + NewLine;
            Antlr3.Analysis.DFA dfa = g.GetLookaheadDFA( 1 );
            FASerializer serializer = new FASerializer( g );
            string result = serializer.Serialize( dfa.StartState );
            Assert.AreEqual( expectingDFA, result );

            // must not be a "unreachable alt: Tokens" error
            Assert.AreEqual(0, equeue.errors.Count, "unexpected errors: " + equeue);
        }

        [TestMethod]
        public void TestInvalidImportMechanism() /*throws Exception*/ {
            // M.Tokens has nothing to predict tokens from S.  Should
            // not include S.Tokens alt in this case?
            string slave =
                "lexer grammar S;\n" +
                "A : 'a' {System.out.println(\"S.A\");} ;\n";
            mkdir( tmpdir );
            writeFile( tmpdir, "S.g", slave );
            string master =
                "tree grammar M;\n" +
                "import S;\n" +
                "a : A ;";
            writeFile( tmpdir, "M.g", master );

            ErrorQueue equeue = new ErrorQueue();
            ErrorManager.SetErrorListener( equeue );
            AntlrTool antlr = newTool( new string[] { "-lib", tmpdir } );
            CompositeGrammar composite = new CompositeGrammar();
            Grammar g = new Grammar( antlr, System.IO.Path.Combine( tmpdir, "M.g" ), composite );
            composite.SetDelegationRoot( g );
            g.ParseAndBuildAST();

            Assert.AreEqual(1, equeue.errors.Count, "unexpected errors: " + equeue);
            Assert.AreEqual(0, equeue.warnings.Count, "unexpected errors: " + equeue);

            string expectedError =
                "error(161): " + Regex.Replace(tmpdir.ToString(), "\\-[0-9]+", "") + "\\M.g:2:8: tree grammar M cannot import lexer grammar S";
            Assert.AreEqual(expectedError, Regex.Replace(equeue.errors[0].ToString(), "\\-[0-9]+", ""));
        }

        [TestMethod]
        public void TestSyntacticPredicateRulesAreNotInherited() /*throws Exception*/ {
            // if this compiles, it means that synpred1_S is defined in S.java
            // but not MParser.java.  MParser has its own synpred1_M which must
            // be separate to compile.
            string slave =
                "parser grammar S;\n" +
                "a : 'a' {System.out.println(\"S.a1\");}\n" +
                "  | 'a' {System.out.println(\"S.a2\");}\n" +
                "  ;\n" +
                "b : 'x' | 'y' {;} ;\n"; // preds generated but not need in DFA here
            mkdir( tmpdir );
            writeFile( tmpdir, "S.g", slave );
            string master =
                "grammar M;\n" +
                "options {backtrack=true;}\n" +
                "import S;\n" +
                "start : a b ;\n" +
                "nonsense : 'q' | 'q' {;} ;" + // forces def of preds here in M
                "WS : (' '|'\\n') {skip();} ;\n";
            string found = execParser( "M.g", master, "MParser", "MLexer",
                                      "start", "ax", debug );
            Assert.AreEqual( "S.a1" + NewLine, found );
        }

        [TestMethod]
        public void TestKeywordVSIDGivesNoWarning() /*throws Exception*/ {
            ErrorQueue equeue = new ErrorQueue();
            ErrorManager.SetErrorListener( equeue );
            string slave =
                "lexer grammar S;\n" +
                "A : 'abc' {System.out.println(\"S.A\");} ;\n" +
                "ID : 'a'..'z'+ ;\n";
            mkdir( tmpdir );
            writeFile( tmpdir, "S.g", slave );
            string master =
                "grammar M;\n" +
                "import S;\n" +
                "a : A {System.out.println(\"M.a\");} ;\n" +
                "WS : (' '|'\\n') {skip();} ;\n";
            string found = execParser( "M.g", master, "MParser", "MLexer",
                                      "a", "abc", debug );

            Assert.AreEqual(0, equeue.errors.Count, "unexpected errors: " + equeue);
            Assert.AreEqual(0, equeue.warnings.Count, "unexpected warnings: " + equeue);

            Assert.AreEqual( "S.A" + NewLine + "M.a" + NewLine, found );
        }

        [TestMethod]
        public void TestWarningForUndefinedToken() /*throws Exception*/ {
            ErrorQueue equeue = new ErrorQueue();
            ErrorManager.SetErrorListener( equeue );
            string slave =
                "lexer grammar S;\n" +
                "A : 'abc' {System.out.println(\"S.A\");} ;\n";
            mkdir( tmpdir );
            writeFile( tmpdir, "S.g", slave );
            string master =
                "grammar M;\n" +
                "import S;\n" +
                "a : ABC A {System.out.println(\"M.a\");} ;\n" +
                "WS : (' '|'\\n') {skip();} ;\n";
            // A is defined in S but M should still see it and not give warning.
            // only problem is ABC.

            rawGenerateAndBuildRecognizer( "M.g", master, "MParser", "MLexer", debug );

            Assert.AreEqual(0, equeue.errors.Count, "unexpected errors: " + equeue);
            Assert.AreEqual(1, equeue.warnings.Count, "unexpected warnings: " + equeue);
            string expectedError =
                "warning(105): " + Regex.Replace(tmpdir.ToString(), "\\-[0-9]+", "") + "\\M.g:3:5: no lexer rule corresponding to token: ABC";
            Assert.AreEqual(expectedError, Regex.Replace(equeue.warnings[0].ToString(), "\\-[0-9]+", ""));
        }

        /** Make sure that M can import S that imports T. */
        [TestMethod]
        public void Test3LevelImport() /*throws Exception*/ {
            ErrorQueue equeue = new ErrorQueue();
            ErrorManager.SetErrorListener( equeue );
            string slave =
                "parser grammar T;\n" +
                "a : T ;\n";
            mkdir( tmpdir );
            writeFile( tmpdir, "T.g", slave );
            string slave2 =
                "parser grammar S;\n" + // A, B, C token type order
                "import T;\n" +
                "a : S ;\n";
            mkdir( tmpdir );
            writeFile( tmpdir, "S.g", slave2 );

            string master =
                "grammar M;\n" +
                "import S;\n" +
                "a : M ;\n";
            writeFile( tmpdir, "M.g", master );
            AntlrTool antlr = newTool( new string[] { "-lib", tmpdir } );
            CompositeGrammar composite = new CompositeGrammar();
            Grammar g = new Grammar( antlr, tmpdir + "/M.g", composite );
            composite.SetDelegationRoot( g );
            g.ParseAndBuildAST();
            g.composite.AssignTokenTypes();
            g.composite.DefineGrammarSymbols();

            string expectedTokenIDToTypeMap = "[M=4, S=5, T=6]";
            string expectedStringLiteralToTypeMap = "{}";
            string expectedTypeToTokenList = "[M, S, T]";

            Assert.AreEqual( expectedTokenIDToTypeMap,
                         realElements( g.composite.TokenIDToTypeMap ).ToElementString() );
            Assert.AreEqual( expectedStringLiteralToTypeMap, g.composite.StringLiteralToTypeMap.ToElementString() );
            Assert.AreEqual( expectedTypeToTokenList,
                         realElements( g.composite.TypeToTokenList ).ToElementString() );

            Assert.AreEqual(0, equeue.errors.Count, "unexpected errors: " + equeue);

            bool ok =
                rawGenerateAndBuildRecognizer( "M.g", master, "MParser", null, false );
            bool expecting = true; // should be ok
            Assert.AreEqual( expecting, ok );
        }

        [TestMethod]
        public void TestBigTreeOfImports() /*throws Exception*/ {
            ErrorQueue equeue = new ErrorQueue();
            ErrorManager.SetErrorListener( equeue );
            string slave =
                "parser grammar T;\n" +
                "x : T ;\n";
            mkdir( tmpdir );
            writeFile( tmpdir, "T.g", slave );
            slave =
                "parser grammar S;\n" +
                "import T;\n" +
                "y : S ;\n";
            mkdir( tmpdir );
            writeFile( tmpdir, "S.g", slave );

            slave =
                "parser grammar C;\n" +
                "i : C ;\n";
            mkdir( tmpdir );
            writeFile( tmpdir, "C.g", slave );
            slave =
                "parser grammar B;\n" +
                "j : B ;\n";
            mkdir( tmpdir );
            writeFile( tmpdir, "B.g", slave );
            slave =
                "parser grammar A;\n" +
                "import B,C;\n" +
                "k : A ;\n";
            mkdir( tmpdir );
            writeFile( tmpdir, "A.g", slave );

            string master =
                "grammar M;\n" +
                "import S,A;\n" +
                "a : M ;\n";
            writeFile( tmpdir, "M.g", master );
            AntlrTool antlr = newTool( new string[] { "-lib", tmpdir } );
            CompositeGrammar composite = new CompositeGrammar();
            Grammar g = new Grammar( antlr, tmpdir + "/M.g", composite );
            composite.SetDelegationRoot( g );
            g.ParseAndBuildAST();
            g.composite.AssignTokenTypes();
            g.composite.DefineGrammarSymbols();

            string expectedTokenIDToTypeMap = "[A=4, B=5, C=6, M=7, S=8, T=9]";
            string expectedStringLiteralToTypeMap = "{}";
            string expectedTypeToTokenList = "[A, B, C, M, S, T]";

            Assert.AreEqual( expectedTokenIDToTypeMap,
                         realElements( g.composite.TokenIDToTypeMap ).ToElementString() );
            Assert.AreEqual( expectedStringLiteralToTypeMap, g.composite.StringLiteralToTypeMap.ToElementString() );
            Assert.AreEqual( expectedTypeToTokenList,
                         realElements( g.composite.TypeToTokenList ).ToElementString() );

            Assert.AreEqual(0, equeue.errors.Count, "unexpected errors: " + equeue);

            bool ok =
                rawGenerateAndBuildRecognizer( "M.g", master, "MParser", null, false );
            bool expecting = true; // should be ok
            Assert.AreEqual( expecting, ok );
        }

        [TestMethod]
        public void TestRulesVisibleThroughMultilevelImport() /*throws Exception*/ {
            ErrorQueue equeue = new ErrorQueue();
            ErrorManager.SetErrorListener( equeue );
            string slave =
                "parser grammar T;\n" +
                "x : T ;\n";
            mkdir( tmpdir );
            writeFile( tmpdir, "T.g", slave );
            string slave2 =
                "parser grammar S;\n" + // A, B, C token type order
                "import T;\n" +
                "a : S ;\n";
            mkdir( tmpdir );
            writeFile( tmpdir, "S.g", slave2 );

            string master =
                "grammar M;\n" +
                "import S;\n" +
                "a : M x ;\n"; // x MUST BE VISIBLE TO M
            writeFile( tmpdir, "M.g", master );
            AntlrTool antlr = newTool( new string[] { "-lib", tmpdir } );
            CompositeGrammar composite = new CompositeGrammar();
            Grammar g = new Grammar( antlr, tmpdir + "/M.g", composite );
            composite.SetDelegationRoot( g );
            g.ParseAndBuildAST();
            g.composite.AssignTokenTypes();
            g.composite.DefineGrammarSymbols();

            string expectedTokenIDToTypeMap = "[M=4, S=5, T=6]";
            string expectedStringLiteralToTypeMap = "{}";
            string expectedTypeToTokenList = "[M, S, T]";

            Assert.AreEqual( expectedTokenIDToTypeMap,
                         realElements( g.composite.TokenIDToTypeMap ).ToElementString() );
            Assert.AreEqual( expectedStringLiteralToTypeMap, g.composite.StringLiteralToTypeMap.ToElementString() );
            Assert.AreEqual( expectedTypeToTokenList,
                         realElements( g.composite.TypeToTokenList ).ToElementString() );

            Assert.AreEqual(0, equeue.errors.Count, "unexpected errors: " + equeue);
        }

        [TestMethod]
        public void TestNestedComposite()
        {
            // Wasn't compiling. http://www.antlr.org/jira/browse/ANTLR-438
            ErrorQueue equeue = new ErrorQueue();
            ErrorManager.SetErrorListener(equeue);
            string gstr =
                "lexer grammar L;\n" +
                "T1: '1';\n" +
                "T2: '2';\n" +
                "T3: '3';\n" +
                "T4: '4';\n";
            mkdir(tmpdir);
            writeFile(tmpdir, "L.g", gstr);
            gstr =
                "parser grammar G1;\n" +
                "s: a | b;\n" +
                "a: T1;\n" +
                "b: T2;\n";
            mkdir(tmpdir);
            writeFile(tmpdir, "G1.g", gstr);

            gstr =
                "parser grammar G2;\n" +
                "import G1;\n" +
                "a: T3;\n";
            mkdir(tmpdir);
            writeFile(tmpdir, "G2.g", gstr);
            string G3str =
                "grammar G3;\n" +
                "import G2;\n" +
                "b: T4;\n";
            mkdir(tmpdir);
            writeFile(tmpdir, "G3.g", G3str);

            AntlrTool antlr = newTool(new string[] { "-lib", tmpdir });
            CompositeGrammar composite = new CompositeGrammar();
            Grammar g = new Grammar(antlr, tmpdir + "/G3.g", composite);
            composite.SetDelegationRoot(g);
            g.ParseAndBuildAST();
            g.composite.AssignTokenTypes();
            g.composite.DefineGrammarSymbols();

            string expectedTokenIDToTypeMap = "[T1=4, T2=5, T3=6, T4=7]";
            string expectedStringLiteralToTypeMap = "{}";
            string expectedTypeToTokenList = "[T1, T2, T3, T4]";

            Assert.AreEqual(expectedTokenIDToTypeMap, realElements(g.composite.TokenIDToTypeMap).ToElementString());
            Assert.AreEqual(expectedStringLiteralToTypeMap, g.composite.StringLiteralToTypeMap.ToElementString());
            Assert.AreEqual(expectedTypeToTokenList, realElements(g.composite.TypeToTokenList).ToElementString());

            Assert.AreEqual(0, equeue.errors.Count, "unexpected errors: " + equeue);

            bool ok =
                rawGenerateAndBuildRecognizer("G3.g", G3str, "G3Parser", null, false);
            bool expecting = true; // should be ok
            Assert.AreEqual(expecting, ok);
        }

        [TestMethod]
        public void TestHeadersPropogatedCorrectlyToImportedGrammars()
        {
            string slave =
                "parser grammar S;\n" +
                "a : B {System.out.print(\"S.a\");} ;\n";
            mkdir(tmpdir);
            writeFile(tmpdir, "S.g", slave);
            string master =
                "grammar M;\n" +
                "import S;\n" +
                "@header{package mypackage;}\n" +
                "@lexer::header{package mypackage;}\n" +
                "s : a ;\n" +
                "B : 'b' ;" + // defines B from inherited token space
                "WS : (' '|'\\n') {skip();} ;\n";
            bool ok = antlr("M.g", "M.g", master, debug);
            bool expecting = true; // should be ok
            Assert.AreEqual(expecting, ok);
        }
    }
}
