namespace AntlrUnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Antlr3.ST;

    public class JavaRuntimeTestHarness : RuntimeTestHarness
    {
        public override string TestFileName
        {
            get
            {
                return "Test.java";
            }
        }

        public override StringTemplate GetParserCreationTemplate()
        {
            StringTemplate createParserST =
                new StringTemplate(
                    "        Profiler2 profiler = new Profiler2();\n" +
                    "        $parserName$ parser = new $parserName$(tokens,profiler);\n" +
                    "        profiler.setParser(parser);\n"
                    );

            if ( !Debug )
            {
                createParserST =
                    new StringTemplate(
                        "        $parserName$ parser = new $parserName$(tokens);\n"
                        );
            }

            return createParserST;
        }

        public override StringTemplate GetLexerTestFileTemplate()
        {
            StringTemplate outputFileST = new StringTemplate(
                "import org.antlr.runtime.*;\n" +
                "import org.antlr.runtime.tree.*;\n" +
                "import org.antlr.runtime.debug.*;\n" +
                "\n" +
                "class Profiler2 extends Profiler {\n" +
                "    public void terminate() { ; }\n" +
                "}\n" +
                "public class Test {\n" +
                "    public static void main(String[] args) throws Exception {\n" +
                "        CharStream input = new ANTLRFileStream(args[0]);\n" +
                "        $lexerName$ lex = new $lexerName$(input);\n" +
                "        CommonTokenStream tokens = new CommonTokenStream(lex);\n" +
                "        System.out.println(tokens);\n" +
                "    }\n" +
                "}"
                );

            return outputFileST;
        }
        public override StringTemplate GetCombinedTestFileTemplate()
        {
            StringTemplate outputFileST = new StringTemplate(
                "import org.antlr.runtime.*;\n" +
                "import org.antlr.runtime.tree.*;\n" +
                "import org.antlr.runtime.debug.*;\n" +
                "\n" +
                "class Profiler2 extends Profiler {\n" +
                "    public void terminate() { ; }\n" +
                "}\n" +
                "public class Test {\n" +
                "    public static void main(String[] args) throws Exception {\n" +
                "        CharStream input = new ANTLRFileStream(args[0]);\n" +
                "        $lexerName$ lex = new $lexerName$(input);\n" +
                "        CommonTokenStream tokens = new CommonTokenStream(lex);\n" +
                "        $createParser$\n" +
                "        parser.$parserStartRuleName$();\n" +
                "    }\n" +
                "}"
                );

            return outputFileST;
        }
        public override StringTemplate GetTreeTestFileTemplate()
        {
            StringTemplate outputFileST = new StringTemplate(
                "import org.antlr.runtime.*;\n" +
                "import org.antlr.runtime.tree.*;\n" +
                "import org.antlr.runtime.debug.*;\n" +
                "\n" +
                "class Profiler2 extends Profiler {\n" +
                "    public void terminate() { ; }\n" +
                "}\n" +
                "public class Test {\n" +
                "    public static void main(String[] args) throws Exception {\n" +
                "        CharStream input = new ANTLRFileStream(args[0]);\n" +
                "        $lexerName$ lex = new $lexerName$(input);\n" +
                "        TokenRewriteStream tokens = new TokenRewriteStream(lex);\n" +
                "        $createParser$\n" +
                "        $parserName$.$parserStartRuleName$_return r = parser.$parserStartRuleName$();\n" +
                "        $if(!treeParserStartRuleName)$\n" +
                "        if ( r.tree!=null ) {\n" +
                "            System.out.println(((Tree)r.tree).toStringTree());\n" +
                "            ((CommonTree)r.tree).sanityCheckParentAndChildIndexes();\n" +
                "		 }\n" +
                "        $else$\n" +
                "        CommonTreeNodeStream nodes = new CommonTreeNodeStream((Tree)r.tree);\n" +
                "        nodes.setTokenStream(tokens);\n" +
                "        $treeParserName$ walker = new $treeParserName$(nodes);\n" +
                "        walker.$treeParserStartRuleName$();\n" +
                "        $endif$\n" +
                "    }\n" +
                "}"
                );

            return outputFileST;
        }
        public override StringTemplate GetTreeAndTreeTestFileTemplate()
        {
            StringTemplate outputFileST = new StringTemplate(
                "import org.antlr.runtime.*;\n" +
                "import org.antlr.runtime.tree.*;\n" +
                "import org.antlr.runtime.debug.*;\n" +
                "\n" +
                "class Profiler2 extends Profiler {\n" +
                "    public void terminate() { ; }\n" +
                "}\n" +
                "public class Test {\n" +
                "    public static void main(String[] args) throws Exception {\n" +
                "        CharStream input = new ANTLRFileStream(args[0]);\n" +
                "        $lexerName$ lex = new $lexerName$(input);\n" +
                "        TokenRewriteStream tokens = new TokenRewriteStream(lex);\n" +
                "        $createParser$\n" +
                "        $parserName$.$parserStartRuleName$_return r = parser.$parserStartRuleName$();\n" +
                "        ((CommonTree)r.tree).sanityCheckParentAndChildIndexes();\n" +
                "        CommonTreeNodeStream nodes = new CommonTreeNodeStream((Tree)r.tree);\n" +
                "        nodes.setTokenStream(tokens);\n" +
                "        $treeParserName$ walker = new $treeParserName$(nodes);\n" +
                "        $treeParserName$.$treeParserStartRuleName$_return r2 = walker.$treeParserStartRuleName$();\n" +
                "		 CommonTree rt = ((CommonTree)r2.tree);\n" +
                "		 if ( rt!=null ) System.out.println(((CommonTree)r2.tree).toStringTree());\n" +
                "    }\n" +
                "}"
                );

            return outputFileST;
        }
        public override StringTemplate GetTemplateTestFileTemplate()
        {
            StringTemplate outputFileST = new StringTemplate(
                "import org.antlr.runtime.*;\n" +
                "import org.antlr.stringtemplate.*;\n" +
                "import org.antlr.stringtemplate.language.*;\n" +
                "import org.antlr.runtime.debug.*;\n" +
                "import java.io.*;\n" +
                "\n" +
                "class Profiler2 extends Profiler {\n" +
                "    public void terminate() { ; }\n" +
                "}\n" +
                "public class Test {\n" +
                "    static String templates =\n" +
                "    		\"group test;\"+" +
                "    		\"foo(x,y) ::= \\\"<x> <y>\\\"\";\n" +
                "    static StringTemplateGroup group =" +
                "    		new StringTemplateGroup(new StringReader(templates)," +
                "					AngleBracketTemplateLexer.class);" +
                "    public static void main(String[] args) throws Exception {\n" +
                "        CharStream input = new ANTLRFileStream(args[0]);\n" +
                "        $lexerName$ lex = new $lexerName$(input);\n" +
                "        CommonTokenStream tokens = new CommonTokenStream(lex);\n" +
                "        $createParser$\n" +
                "		 parser.setTemplateLib(group);\n" +
                "        $parserName$.$parserStartRuleName$_return r = parser.$parserStartRuleName$();\n" +
                "        if ( r.st!=null )\n" +
                "            System.out.print(r.st.toString());\n" +
                "	 	 else\n" +
                "            System.out.print(\"\");\n" +
                "    }\n" +
                "}"
                );

            return outputFileST;
        }
    }
}
