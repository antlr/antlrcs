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

    [TestClass]
    public class TestRewriteTemplates : BaseTest
    {
        protected bool debug = false;

        [TestMethod]
        public void TestDelete() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=template;}\n" +
                "a : ID INT -> ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                                      "a", "abc 34", debug );
            assertEquals( "", found );
        }

        [TestMethod]
        public void TestAction() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=template;}\n" +
                "a : ID INT -> {new StringTemplate($ID.text)} ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                                      "a", "abc 34", debug );
            assertEquals( "abc" + NewLine, found );
        }

        [TestMethod]
        public void TestEmbeddedLiteralConstructor() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=template;}\n" +
                "a : ID INT -> {%{$ID.text}} ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                                      "a", "abc 34", debug );
            assertEquals( "abc" + NewLine, found );
        }

        [TestMethod]
        public void TestInlineTemplate() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=template;}\n" +
                "a : ID INT -> template(x={$ID},y={$INT}) <<x:<x.text>, y:<y.text>;>> ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                                      "a", "abc 34", debug );
            assertEquals( "x:abc, y:34;" + NewLine, found );
        }

        [TestMethod]
        public void TestNamedTemplate() /*throws Exception*/ {
            // the support code adds template group in it's output Test.java
            // that defines template foo.
            string grammar =
                "grammar T;\n" +
                "options {output=template;}\n" +
                "a : ID INT -> foo(x={$ID.text},y={$INT.text}) ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                                      "a", "abc 34", debug );
            assertEquals( "abc 34" + NewLine, found );
        }

        [TestMethod]
        public void TestIndirectTemplate() /*throws Exception*/ {
            // the support code adds template group in it's output Test.java
            // that defines template foo.
            string grammar =
                "grammar T;\n" +
                "options {output=template;}\n" +
                "a : ID INT -> ({\"foo\"})(x={$ID.text},y={$INT.text}) ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                                      "a", "abc 34", debug );
            assertEquals( "abc 34" + NewLine, found );
        }

        [TestMethod]
        public void TestInlineTemplateInvokingLib() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=template;}\n" +
                "a : ID INT -> template(x={$ID.text},y={$INT.text}) \"<foo(...)>\" ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                                      "a", "abc 34", debug );
            assertEquals( "abc 34" + NewLine, found );
        }

        [TestMethod]
        public void TestPredicatedAlts() /*throws Exception*/ {
            // the support code adds template group in it's output Test.java
            // that defines template foo.
            string grammar =
                "grammar T;\n" +
                "options {output=template;}\n" +
                "a : ID INT -> {false}? foo(x={$ID.text},y={$INT.text})\n" +
                "           -> foo(x={\"hi\"}, y={$ID.text})\n" +
                "  ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                                      "a", "abc 34", debug );
            assertEquals( "hi abc" + NewLine, found );
        }

        [TestMethod]
        public void TestTemplateReturn() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=template;}\n" +
                "a : b {System.out.println($b.st);} ;\n" +
                "b : ID INT -> foo(x={$ID.text},y={$INT.text}) ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                                      "a", "abc 34", debug );
            assertEquals( "abc 34" + NewLine, found );
        }

        [TestMethod]
        public void TestReturnValueWithTemplate() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=template;}\n" +
                "a : b {System.out.println($b.i);} ;\n" +
                "b returns [int i] : ID INT {$i=8;} ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                                      "a", "abc 34", debug );
            assertEquals( "8" + NewLine, found );
        }

        [TestMethod]
        public void TestTemplateRefToDynamicAttributes() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=template;}\n" +
                "a scope {String id;} : ID {$a::id=$ID.text;} b\n" +
                "	{System.out.println($b.st.toString());}\n" +
                "   ;\n" +
                "b : INT -> foo(x={$a::id}) ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";
            string found = execParser( "T.g", grammar, "TParser", "TLexer",
                                      "a", "abc 34", debug );
            assertEquals( "abc " + NewLine, found );
        }

        // tests for rewriting templates in tree parsers

        [TestMethod]
        public void TestSingleNode() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ID ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";

            string treeGrammar =
                "tree grammar TP;\n" +
                "options {ASTLabelType=CommonTree; output=template;}\n" +
                "s : a {System.out.println($a.st);} ;\n" +
                "a : ID -> template(x={$ID.text}) <<|<x>|>> ;\n";

            string found = execTreeParser( "T.g", grammar, "TParser", "TP.g",
                                          treeGrammar, "TP", "TLexer", "a", "s", "abc" );
            assertEquals( "|abc|" + NewLine, found );
        }

        [TestMethod]
        public void TestSingleNodeRewriteMode() /*throws Exception*/ {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "a : ID ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+;\n" +
                "WS : (' '|'\\n') {$channel=HIDDEN;} ;\n";

            string treeGrammar =
                "tree grammar TP;\n" +
                "options {ASTLabelType=CommonTree; output=template; rewrite=true;}\n" +
                "s : a {System.out.println(input.getTokenStream().toString(0,0));} ;\n" +
                "a : ID -> template(x={$ID.text}) <<|<x>|>> ;\n";

            string found = execTreeParser( "T.g", grammar, "TParser", "TP.g",
                                          treeGrammar, "TP", "TLexer", "a", "s", "abc" );
            assertEquals( "|abc|" + NewLine, found );
        }

        [TestMethod]
        public void TestRewriteRuleAndRewriteModeOnSimpleElements() /*throws Exception*/ {
            ErrorQueue equeue = new ErrorQueue();
            ErrorManager.setErrorListener( equeue );
            Grammar g = new Grammar(
                "tree grammar TP;\n" +
                "options {ASTLabelType=CommonTree; output=template; rewrite=true;}\n" +
                "a: ^(A B) -> {ick}\n" +
                " | y+=INT -> {ick}\n" +
                " | x=ID -> {ick}\n" +
                " | BLORT -> {ick}\n" +
                " ;\n"
            );
            AntlrTool antlr = newTool();
            antlr.setOutputDirectory( null ); // write to /dev/null
            CodeGenerator generator = new CodeGenerator( antlr, g, "Java" );
            g.setCodeGenerator( generator );
            generator.genRecognizer();

            assertEquals( "unexpected errors: " + equeue, 0, equeue.warnings.Count );
        }

        [TestMethod]
        public void TestRewriteRuleAndRewriteModeIgnoreActionsPredicates() /*throws Exception*/ {
            ErrorQueue equeue = new ErrorQueue();
            ErrorManager.setErrorListener( equeue );
            Grammar g = new Grammar(
                "tree grammar TP;\n" +
                "options {ASTLabelType=CommonTree; output=template; rewrite=true;}\n" +
                "a: {action} {action2} x=A -> {ick}\n" +
                " | {pred1}? y+=B -> {ick}\n" +
                " | C {action} -> {ick}\n" +
                " | {pred2}?=> z+=D -> {ick}\n" +
                " | (E)=> ^(F G) -> {ick}\n" +
                " ;\n"
            );
            AntlrTool antlr = newTool();
            antlr.setOutputDirectory( null ); // write to /dev/null
            CodeGenerator generator = new CodeGenerator( antlr, g, "Java" );
            g.setCodeGenerator( generator );
            generator.genRecognizer();

            assertEquals( "unexpected errors: " + equeue, 0, equeue.warnings.Count );
        }

        [TestMethod]
        public void TestRewriteRuleAndRewriteModeNotSimple() /*throws Exception*/ {
            ErrorQueue equeue = new ErrorQueue();
            ErrorManager.setErrorListener( equeue );
            Grammar g = new Grammar(
                "tree grammar TP;\n" +
                "options {ASTLabelType=CommonTree; output=template; rewrite=true;}\n" +
                "a  : ID+ -> {ick}\n" +
                "   | INT INT -> {ick}\n" +
                "   ;\n"
            );
            AntlrTool antlr = newTool();
            antlr.setOutputDirectory( null ); // write to /dev/null
            CodeGenerator generator = new CodeGenerator( antlr, g, "Java" );
            g.setCodeGenerator( generator );
            generator.genRecognizer();

            assertEquals( "unexpected errors: " + equeue, 2, equeue.warnings.Count );
        }

        [TestMethod]
        public void TestRewriteRuleAndRewriteModeRefRule() /*throws Exception*/ {
            ErrorQueue equeue = new ErrorQueue();
            ErrorManager.setErrorListener( equeue );
            Grammar g = new Grammar(
                "tree grammar TP;\n" +
                "options {ASTLabelType=CommonTree; output=template; rewrite=true;}\n" +
                "a  : b+ -> {ick}\n" +
                "   | b b A -> {ick}\n" +
                "   ;\n" +
                "b  : B ;\n"
            );
            AntlrTool antlr = newTool();
            antlr.setOutputDirectory( null ); // write to /dev/null
            CodeGenerator generator = new CodeGenerator( antlr, g, "Java" );
            g.setCodeGenerator( generator );
            generator.genRecognizer();

            assertEquals( "unexpected errors: " + equeue, 2, equeue.warnings.Count );
        }

    }
}
