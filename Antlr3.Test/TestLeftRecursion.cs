/*
 * [The "BSD licence"]
 * Copyright (c) 2011 Terence Parr
 * All rights reserved.
 *
 * Conversion to C#:
 * Copyright (c) 2011 Sam Harwell, Pixel Mine, Inc.
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
    using Console = System.Console;

    [TestClass]
    public class TestLeftRecursion : BaseTest
    {
        protected bool debug = false;

        [TestMethod]
        public void TestSimple()
        {
            string grammar =
                "grammar T;\n" +
                "s : a {System.out.println($a.text);} ;\n" +
                "a : a ID\n" +
                "  | ID" +
                "  ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "WS : (' '|'\\n') {skip();} ;\n";
            string found = execParser("T.g", grammar, "TParser", "TLexer",
                                      "s", "a b c", debug);
            string expecting = "abc\n";
            Assert.AreEqual(expecting, found);
        }

        [TestMethod]
        public void TestTernaryExpr()
        {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "e : e '*'^ e" +
                "  | e '+'^ e" +
                "  | e '?'<assoc=right>^ e ':'! e" +
                "  | e '='<assoc=right>^ e" +
                "  | ID" +
                "  ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "WS : (' '|'\\n') {skip();} ;\n";

            string[] tests = {
			"a",			"a",
			"a+b",			"(+ a b)",
			"a*b",			"(* a b)",
			"a?b:c",		"(? a b c)",
			"a=b=c",		"(= a (= b c))",
			"a?b+c:d",		"(? a (+ b c) d)",
			"a?b=c:d",		"(? a (= b c) d)",
			"a? b?c:d : e",	"(? a (? b c d) e)",
			"a?b: c?d:e",	"(? a b (? c d e))",
		};

            RunTests(grammar, tests, "e");
        }

        [TestMethod]
        public void TestDeclarationsUsingASTOperators()
        {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "declarator\n" +
                "        : declarator '['^ e ']'!\n" +
                "        | declarator '['^ ']'!\n" +
                "        | declarator '('^ ')'!\n" +
                "        | '*'^ declarator\n" + // binds less tight than suffixes
                "        | '('! declarator ')'!\n" +
                "        | ID\n" +
                "        ;\n" +
                "e : INT ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+ ;\n" +
                "WS : (' '|'\\n') {skip();} ;\n";

            string[] tests = {
			"a",		"a",
			"*a",		"(* a)",
			"**a",		"(* (* a))",
			"a[3]",		"([ a 3)",
			"b[]",		"([ b)",
			"(a)",		"a",
			"a[]()",	"(( ([ a))",
			"a[][]",	"([ ([ a))",
			"*a[]",		"(* ([ a))",
			"(*a)[]",	"([ (* a))",
		};

            RunTests(grammar, tests, "declarator");
        }

        [TestMethod]
        public void TestDeclarationsUsingRewriteOperators()
        {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "declarator\n" +
                "        : declarator '[' e ']' -> ^('[' declarator e)\n" +
                "        | declarator '[' ']' -> ^('[' declarator)\n" +
                "        | declarator '(' ')' -> ^('(' declarator)\n" +
                "        | '*' declarator -> ^('*' declarator) \n" + // binds less tight than suffixes
                "        | '(' declarator ')' -> declarator\n" +
                "        | ID -> ID\n" +
                "        ;\n" +
                "e : INT ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+ ;\n" +
                "WS : (' '|'\\n') {skip();} ;\n";

            string[] tests = {
			"a",		"a",
			"*a",		"(* a)",
			"**a",		"(* (* a))",
			"a[3]",		"([ a 3)",
			"b[]",		"([ b)",
			"(a)",		"a",
			"a[]()",	"(( ([ a))",
			"a[][]",	"([ ([ a))",
			"*a[]",		"(* ([ a))",
			"(*a)[]",	"([ (* a))",
		};

            RunTests(grammar, tests, "declarator");
        }

        [TestMethod]
        public void TestExpressionsUsingASTOperators()
        {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "e : e '.'^ ID\n" +
                "  | e '.'^ 'this'\n" +
                "  | '-'^ e\n" +
                "  | e '*'^ e\n" +
                "  | e ('+'^|'-'^) e\n" +
                "  | INT\n" +
                "  | ID\n" +
                "  ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+ ;\n" +
                "WS : (' '|'\\n') {skip();} ;\n";

            string[] tests = {
			"a",		"a",
			"1",		"1",
			"a+1",		"(+ a 1)",
			"a*1",		"(* a 1)",
			"a.b",		"(. a b)",
			"a.this",	"(. a this)",
			"a-b+c",	"(+ (- a b) c)",
			"a+b*c",	"(+ a (* b c))",
			"a.b+1",	"(+ (. a b) 1)",
			"-a",		"(- a)",
			"-a+b",		"(+ (- a) b)",
			"-a.b",		"(- (. a b))",
		};

            RunTests(grammar, tests, "e");
        }

        [TestMethod]
        public void TestExpressionsUsingRewriteOperators()
        {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "e : e '.' ID 				-> ^('.' e ID)\n" +
                "  | e '.' 'this' 			-> ^('.' e 'this')\n" +
                "  | '-' e 					-> ^('-' e)\n" +
                "  | e '*' b=e 				-> ^('*' e $b)\n" +
                "  | e (op='+'|op='-') b=e	-> ^($op e $b)\n" +
                "  | INT 					-> INT\n" +
                "  | ID 					-> ID\n" +
                "  ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+ ;\n" +
                "WS : (' '|'\\n') {skip();} ;\n";

            string[] tests =
                {
                    "a",		"a",
                    "1",		"1",
                    "a+1",		"(+ a 1)",
                    "a*1",		"(* a 1)",
                    "a.b",		"(. a b)",
                    "a.this",	"(. a this)",
                    "a+b*c",	"(+ a (* b c))",
                    "a.b+1",	"(+ (. a b) 1)",
                    "-a",		"(- a)",
                    "-a+b",		"(+ (- a) b)",
                    "-a.b",		"(- (. a b))",
                };

            RunTests(grammar, tests, "e");
        }

        [TestMethod]
        public void TestExpressionAssociativity()
        {
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "e\n" +
                "  : e '.'^ ID\n" +
                "  | '-'^ e\n" +
                "  | e '^'<assoc=right>^ e\n" +
                "  | e '*'^ e\n" +
                "  | e ('+'^|'-'^) e\n" +
                "  | e ('='<assoc=right>^ |'+='<assoc=right>^) e\n" +
                "  | INT\n" +
                "  | ID\n" +
                "  ;\n" +
                "ID : 'a'..'z'+ ;\n" +
                "INT : '0'..'9'+ ;\n" +
                "WS : (' '|'\\n') {skip();} ;\n";

            string[] tests =
                {
                    "a",		"a",
                    "1",		"1",
                    "a+1",		"(+ a 1)",
                    "a*1",		"(* a 1)",
                    "a.b",		"(. a b)",
                    "a-b+c",	"(+ (- a b) c)",

                    "a+b*c",	"(+ a (* b c))",
                    "a.b+1",	"(+ (. a b) 1)",
                    "-a",		"(- a)",
                    "-a+b",		"(+ (- a) b)",
                    "-a.b",		"(- (. a b))",
                    "a^b^c",	"(^ a (^ b c))",
                    "a=b=c",	"(= a (= b c))",
                    "a=b=c+d.e","(= a (= b (+ c (. d e))))",
                };

            RunTests(grammar, tests, "e");
        }

        [TestMethod]
        public void TestJavaExpressions()
        {
            // Generates about 7k in bytecodes for generated e_ rule;
            // Well within the 64k method limit. e_primary compiles
            // to about 2k in bytecodes.
            // this is simplified from real java
            string grammar =
                "grammar T;\n" +
                "options {output=AST;}\n" +
                "expressionList\n" +
                "    :   e (','! e)*\n" +
                "    ;\n" +
                "e   :   '('! e ')'!\n" +
                "    |   'this' \n" +
                "    |   'super'\n" +
                "    |   INT\n" +
                "    |   ID\n" +
                "    |   type '.'^ 'class'\n" +
                "    |   e '.'^ ID\n" +
                "    |   e '.'^ 'this'\n" +
                "    |   e '.'^ 'super' '('^ expressionList? ')'!\n" +
                "    |   e '.'^ 'new'^ ID '('! expressionList? ')'!\n" +
                "	 |	 'new'^ type ( '(' expressionList? ')'! | (options {k=1;}:'[' e ']'!)+)\n" + // ugly; simplified
                "    |   e '['^ e ']'!\n" +
                "    |   '('^ type ')'! e\n" +
                "    |   e ('++'^ | '--'^)\n" +
                "    |   e '('^ expressionList? ')'!\n" +
                "    |   ('+'^|'-'^|'++'^|'--'^) e\n" +
                "    |   ('~'^|'!'^) e\n" +
                "    |   e ('*'^|'/'^|'%'^) e\n" +
                "    |   e ('+'^|'-'^) e\n" +
                "    |   e ('<'^ '<' | '>'^ '>' '>' | '>'^ '>') e\n" +
                "    |   e ('<='^ | '>='^ | '>'^ | '<'^) e\n" +
                "    |   e 'instanceof'^ e\n" +
                "    |   e ('=='^ | '!='^) e\n" +
                "    |   e '&'^ e\n" +
                "    |   e '^'<assoc=right>^ e\n" +
                "    |   e '|'^ e\n" +
                "    |   e '&&'^ e\n" +
                "    |   e '||'^ e\n" +
                "    |   e '?' e ':' e\n" +
                "    |   e ('='<assoc=right>^\n" +
                "          |'+='<assoc=right>^\n" +
                "          |'-='<assoc=right>^\n" +
                "          |'*='<assoc=right>^\n" +
                "          |'/='<assoc=right>^\n" +
                "          |'&='<assoc=right>^\n" +
                "          |'|='<assoc=right>^\n" +
                "          |'^='<assoc=right>^\n" +
                "          |'>>='<assoc=right>^\n" +
                "          |'>>>='<assoc=right>^\n" +
                "          |'<<='<assoc=right>^\n" +
                "          |'%='<assoc=right>^) e\n" +
                "    ;\n" +
                "type: ID \n" +
                "    | ID '['^ ']'!\n" +
                "    | 'int'\n" +
                "	 | 'int' '['^ ']'! \n" +
                "    ;\n" +
                "ID : ('a'..'z'|'A'..'Z'|'_'|'$')+;\n" +
                "INT : '0'..'9'+ ;\n" +
                "WS : (' '|'\\n') {skip();} ;\n";

            string[] tests =
                {
                    "a",		"a",
                    "1",		"1",
                    "a+1",		"(+ a 1)",
                    "a*1",		"(* a 1)",
                    "a.b",		"(. a b)",
                    "a-b+c",	"(+ (- a b) c)",

                    "a+b*c",	"(+ a (* b c))",
                    "a.b+1",	"(+ (. a b) 1)",
                    "-a",		"(- a)",
                    "-a+b",		"(+ (- a) b)",
                    "-a.b",		"(- (. a b))",
                    "a^b^c",	"(^ a (^ b c))",
                    "a=b=c",	"(= a (= b c))",
                    "a=b=c+d.e","(= a (= b (+ c (. d e))))",
                    "a|b&c",	"(| a (& b c))",
                    "(a|b)&c",	"(& (| a b) c)",
                    "a > b",	"(> a b)",
                    "a >> b",	"(> a b)",  // text is from one token
                    "a < b",	"(< a b)",

                    "(T)x",							"(( T x)",
                    "new A().b",					"(. (new A () b)",
                    "(T)t.f()",						"(( (( T (. t f)))",
                    "a.f(x)==T.c",					"(== (( (. a f) x) (. T c))",
                    "a.f().g(x,1)",					"(( (. (( (. a f)) g) x 1)",
                    "new T[((n-1) * x) + 1]",		"(new T [ (+ (* (- n 1) x) 1))",
                };

            RunTests(grammar, tests, "e");
        }

        public void RunTests(string grammar, string[] tests, string startRule)
        {
            rawGenerateAndBuildRecognizer("T.g", grammar, "TParser", "TLexer", debug);
            WriteRecognizerAndCompile("TParser",
                                             null,
                                             "TLexer",
                                             startRule,
                                             null,
                                             true,
                                             false,
                                             false,
                                             debug);

            for (int i = 0; i < tests.Length; i += 2)
            {
                string test = tests[i];
                string expecting = tests[i + 1] + "\n";
                writeFile(tmpdir, "input", test);
                string found = execRecognizer();
                Console.Write(test + " -> " + found);
                Assert.AreEqual(expecting, found);
            }
        }
    }
}
