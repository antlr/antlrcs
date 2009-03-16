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
    using AntlrTool = Antlr3.AntlrTool;
    using CodeGenerator = Antlr3.Codegen.CodeGenerator;
    using CommonToken = Antlr.Runtime.CommonToken;

    [TestClass]
    public class TestMessages : BaseTest
    {

        /** Public default constructor used by TestRig */
        public TestMessages()
        {
        }

        [TestMethod]
        public void TestMessageStringificationIsConsistent() /*throws Exception*/
        {
            string action = "$other.tree = null;";
            ErrorQueue equeue = new ErrorQueue();
            ErrorManager.setErrorListener( equeue );
            Grammar g = new Grammar(
                "grammar a;\n" +
                "options { output = AST;}" +
                "otherrule\n" +
                "    : 'y' ;" +
                "rule\n" +
                "    : other=otherrule {" + action + "}\n" +
                "    ;" );
            AntlrTool antlr = newTool();
            CodeGenerator generator = new CodeGenerator( antlr, g, "Java" );
            g.setCodeGenerator( generator );
            generator.genRecognizer(); // forces load of templates
            ActionTranslator translator = new ActionTranslator( generator,
                                                                        "rule",
                                                                        new CommonToken( ANTLRParser.ACTION, action ), 1 );
            string rawTranslation =
                translator.translate();

            int expectedMsgID = ErrorManager.MSG_WRITE_TO_READONLY_ATTR;
            object expectedArg = "other";
            object expectedArg2 = "tree";
            GrammarSemanticsMessage expectedMessage =
                new GrammarSemanticsMessage( expectedMsgID, g, null, expectedArg, expectedArg2 );
            string expectedMessageString = expectedMessage.ToString();
            assertEquals( expectedMessageString, expectedMessage.ToString() );
        }
    }
}
