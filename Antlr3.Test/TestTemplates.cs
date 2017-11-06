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
    using Antlr3.Grammars;
    using Antlr3.Tool;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using ActionTranslator = Antlr3.Grammars.ActionTranslator;
    using AngleBracketTemplateLexer = Antlr3.ST.Language.AngleBracketTemplateLexer;
    using AntlrTool = Antlr3.AntlrTool;
    using CodeGenerator = Antlr3.Codegen.CodeGenerator;
    using CommonToken = Antlr.Runtime.CommonToken;
    using StringTemplate = Antlr4.StringTemplate.Template;
    using StringTemplateGroup = Antlr4.StringTemplate.TemplateGroup;

    /** Test templates in actions; %... shorthands */
    [TestClass]
    public class TestTemplates : BaseTest
    {
        private readonly string LINE_SEP = NewLine;

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestTemplateConstructor() /*throws Exception*/ {
            string action = "x = %foo(name={$ID.text});";
            string expecting = "x = templateLib.getInstanceOf(\"foo\"," +
                "new STAttrMap().put(\"name\", (ID1!=null?ID1.getText():null)));";

            ErrorQueue equeue = new ErrorQueue();
            ErrorManager.SetErrorListener( equeue );
            Grammar g = new Grammar(
                "grammar t;\n" +
                "options {\n" +
                "    output=template;\n" +
                "}\n" +
                "\n" +
                "a : ID {" + action + "}\n" +
                "  ;\n" +
                "\n" +
                "ID : 'a';\n" );
            AntlrTool antlr = newTool();
            CodeGenerator generator = new CodeGenerator( antlr, g, "Java" );
            g.CodeGenerator = generator;
            generator.GenRecognizer(); // forces load of templates
            ActionTranslator translator =
                new ActionTranslator( generator,
                                            "a",
                                            new CommonToken( ANTLRParser.ACTION, action ), 1 );
            string rawTranslation =
                translator.Translate();
            StringTemplateGroup templates =
                new StringTemplateGroup();
            StringTemplate actionST = new StringTemplate( templates, rawTranslation );
            string found = actionST.Render();

            assertNoErrors( equeue );

            Assert.AreEqual( expecting, found );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestTemplateConstructorNoArgs() /*throws Exception*/ {
            string action = "x = %foo();";
            string expecting = "x = templateLib.getInstanceOf(\"foo\");";

            ErrorQueue equeue = new ErrorQueue();
            ErrorManager.SetErrorListener( equeue );
            Grammar g = new Grammar(
                "grammar t;\n" +
                "options {\n" +
                "    output=template;\n" +
                "}\n" +
                "\n" +
                "a : ID {" + action + "}\n" +
                "  ;\n" +
                "\n" +
                "ID : 'a';\n" );
            AntlrTool antlr = newTool();
            CodeGenerator generator = new CodeGenerator( antlr, g, "Java" );
            g.CodeGenerator = generator;
            generator.GenRecognizer(); // forces load of templates
            ActionTranslator translator =
                new ActionTranslator( generator,
                                            "a",
                                            new CommonToken( ANTLRParser.ACTION, action ), 1 );
            string rawTranslation =
                translator.Translate();
            StringTemplateGroup templates =
                new StringTemplateGroup();
            StringTemplate actionST = new StringTemplate( templates, rawTranslation );
            string found = actionST.Render();

            assertNoErrors( equeue );

            Assert.AreEqual( expecting, found );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestIndirectTemplateConstructor() /*throws Exception*/ {
            string action = "x = %({\"foo\"})(name={$ID.text});";
            string expecting = "x = templateLib.getInstanceOf(\"foo\"," +
                "new STAttrMap().put(\"name\", (ID1!=null?ID1.getText():null)));";

            ErrorQueue equeue = new ErrorQueue();
            ErrorManager.SetErrorListener( equeue );
            Grammar g = new Grammar(
                "grammar t;\n" +
                "options {\n" +
                "    output=template;\n" +
                "}\n" +
                "\n" +
                "a : ID {" + action + "}\n" +
                "  ;\n" +
                "\n" +
                "ID : 'a';\n" );
            AntlrTool antlr = newTool();
            CodeGenerator generator = new CodeGenerator( antlr, g, "Java" );
            g.CodeGenerator = generator;
            generator.GenRecognizer(); // forces load of templates
            ActionTranslator translator =
                new ActionTranslator( generator,
                                            "a",
                                            new CommonToken( ANTLRParser.ACTION, action ), 1 );
            string rawTranslation =
                translator.Translate();
            StringTemplateGroup templates =
                new StringTemplateGroup();
            StringTemplate actionST = new StringTemplate( templates, rawTranslation );
            string found = actionST.Render();

            assertNoErrors( equeue );

            Assert.AreEqual( expecting, found );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestStringConstructor() /*throws Exception*/ {
            string action = "x = %{$ID.text};";
            string expecting = "x = new StringTemplate(templateLib,(ID1!=null?ID1.getText():null));";

            ErrorQueue equeue = new ErrorQueue();
            ErrorManager.SetErrorListener( equeue );
            Grammar g = new Grammar(
                "grammar t;\n" +
                "options {\n" +
                "    output=template;\n" +
                "}\n" +
                "\n" +
                "a : ID {" + action + "}\n" +
                "  ;\n" +
                "\n" +
                "ID : 'a';\n" );
            AntlrTool antlr = newTool();
            CodeGenerator generator = new CodeGenerator( antlr, g, "Java" );
            g.CodeGenerator = generator;
            generator.GenRecognizer(); // forces load of templates
            ActionTranslator translator = new ActionTranslator( generator,
                                                                         "a",
                                                                         new CommonToken( ANTLRParser.ACTION, action ), 1 );
            string rawTranslation =
                translator.Translate();
            StringTemplateGroup templates =
                new StringTemplateGroup();
            StringTemplate actionST = new StringTemplate( templates, rawTranslation );
            string found = actionST.Render();

            assertNoErrors( equeue );

            Assert.AreEqual( expecting, found );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestSetAttr() /*throws Exception*/ {
            string action = "%x.y = z;";
            string expecting = "(x).setAttribute(\"y\", z);";

            ErrorQueue equeue = new ErrorQueue();
            ErrorManager.SetErrorListener( equeue );
            Grammar g = new Grammar(
                "grammar t;\n" +
                "options {\n" +
                "    output=template;\n" +
                "}\n" +
                "\n" +
                "a : ID {" + action + "}\n" +
                "  ;\n" +
                "\n" +
                "ID : 'a';\n" );
            AntlrTool antlr = newTool();
            CodeGenerator generator = new CodeGenerator( antlr, g, "Java" );
            g.CodeGenerator = generator;
            generator.GenRecognizer(); // forces load of templates
            ActionTranslator translator =
                new ActionTranslator( generator,
                                            "a",
                                            new CommonToken( ANTLRParser.ACTION, action ), 1 );
            string rawTranslation =
                translator.Translate();
            StringTemplateGroup templates =
                new StringTemplateGroup();
            StringTemplate actionST = new StringTemplate( templates, rawTranslation );
            string found = actionST.Render();

            assertNoErrors( equeue );

            Assert.AreEqual( expecting, found );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestSetAttrOfExpr() /*throws Exception*/ {
            string action = "%{foo($ID.text).getST()}.y = z;";
            string expecting = "(foo((ID1!=null?ID1.getText():null)).getST()).setAttribute(\"y\", z);";

            ErrorQueue equeue = new ErrorQueue();
            ErrorManager.SetErrorListener( equeue );
            Grammar g = new Grammar(
                "grammar t;\n" +
                "options {\n" +
                "    output=template;\n" +
                "}\n" +
                "\n" +
                "a : ID {" + action + "}\n" +
                "  ;\n" +
                "\n" +
                "ID : 'a';\n" );
            AntlrTool antlr = newTool();
            CodeGenerator generator = new CodeGenerator( antlr, g, "Java" );
            g.CodeGenerator = generator;
            generator.GenRecognizer(); // forces load of templates
            ActionTranslator translator = new ActionTranslator( generator,
                                                                         "a",
                                                                         new CommonToken( ANTLRParser.ACTION, action ), 1 );
            string rawTranslation =
                translator.Translate();
            StringTemplateGroup templates =
                new StringTemplateGroup();
            StringTemplate actionST = new StringTemplate( templates, rawTranslation );
            string found = actionST.Render();

            assertNoErrors( equeue );

            Assert.AreEqual( expecting, found );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestSetAttrOfExprInMembers() /*throws Exception*/ {
            ErrorQueue equeue = new ErrorQueue();
            ErrorManager.SetErrorListener( equeue );
            Grammar g = new Grammar(
                "grammar t;\n" +
                "options {\n" +
                "    output=template;\n" +
                "}\n" +
                "@members {\n" +
                "%code.instr = o;" + // must not get null ptr!
                "}\n" +
                "a : ID\n" +
                "  ;\n" +
                "\n" +
                "ID : 'a';\n" );
            AntlrTool antlr = newTool();
            CodeGenerator generator = new CodeGenerator( antlr, g, "Java" );
            g.CodeGenerator = generator;
            generator.GenRecognizer(); // forces load of templates

            assertNoErrors( equeue );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestCannotHaveSpaceBeforeDot() /*throws Exception*/ {
            string action = "%x .y = z;";
            //String expecting = null;

            ErrorQueue equeue = new ErrorQueue();
            ErrorManager.SetErrorListener( equeue );
            Grammar g = new Grammar(
                "grammar t;\n" +
                "options {\n" +
                "    output=template;\n" +
                "}\n" +
                "\n" +
                "a : ID {" + action + "}\n" +
                "  ;\n" +
                "\n" +
                "ID : 'a';\n" );
            AntlrTool antlr = newTool();
            CodeGenerator generator = new CodeGenerator( antlr, g, "Java" );
            g.CodeGenerator = generator;
            generator.GenRecognizer(); // forces load of templates

            int expectedMsgID = ErrorManager.MSG_INVALID_TEMPLATE_ACTION;
            object expectedArg = "%x";
            GrammarSemanticsMessage expectedMessage =
                new GrammarSemanticsMessage( expectedMsgID, g, null, expectedArg );
            checkError( equeue, expectedMessage );
        }

        [TestMethod][TestCategory(TestCategories.Antlr3)]
        public void TestCannotHaveSpaceAfterDot() /*throws Exception*/ {
            string action = "%x. y = z;";
            //String expecting = null;

            ErrorQueue equeue = new ErrorQueue();
            ErrorManager.SetErrorListener( equeue );
            Grammar g = new Grammar(
                "grammar t;\n" +
                "options {\n" +
                "    output=template;\n" +
                "}\n" +
                "\n" +
                "a : ID {" + action + "}\n" +
                "  ;\n" +
                "\n" +
                "ID : 'a';\n" );
            AntlrTool antlr = newTool();
            CodeGenerator generator = new CodeGenerator( antlr, g, "Java" );
            g.CodeGenerator = generator;
            generator.GenRecognizer(); // forces load of templates

            int expectedMsgID = ErrorManager.MSG_INVALID_TEMPLATE_ACTION;
            object expectedArg = "%x.";
            GrammarSemanticsMessage expectedMessage =
                new GrammarSemanticsMessage( expectedMsgID, g, null, expectedArg );
            checkError( equeue, expectedMessage );
        }

        protected void checkError( ErrorQueue equeue,
                                  GrammarSemanticsMessage expectedMessage )
        //throws Exception
        {
            /*
            System.out.println(equeue.infos);
            System.out.println(equeue.warnings);
            System.out.println(equeue.errors);
            */
            Message foundMsg = null;
            for ( int i = 0; i < equeue.errors.Count; i++ )
            {
                Message m = (Message)equeue.errors[i];
                if ( m.msgID == expectedMessage.msgID )
                {
                    foundMsg = m;
                }
            }
            Assert.IsTrue(equeue.errors.Count > 0, "no error; " + expectedMessage.msgID + " expected");
            Assert.IsTrue(equeue.errors.Count <= 1, "too many errors; " + equeue.errors);
            Assert.IsTrue(foundMsg != null, "couldn't find expected error: " + expectedMessage.msgID);
            Assert.IsTrue(foundMsg is GrammarSemanticsMessage, "error is not a GrammarSemanticsMessage");
            Assert.AreEqual( expectedMessage.arg, foundMsg.arg );
            Assert.AreEqual( expectedMessage.arg2, foundMsg.arg2 );
        }

        // S U P P O R T
        private void assertNoErrors( ErrorQueue equeue )
        {
            Assert.AreEqual(equeue.errors.Count, 0, "unexpected errors: " + equeue);
        }
    }
}
