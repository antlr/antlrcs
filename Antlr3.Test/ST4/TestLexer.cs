/*
 * [The "BSD licence"]
 * Copyright (c) 2005-2008 Terence Parr
 * All rights reserved.
 *
 * Conversion to C#:
 * Copyright (c) 2008-2009 Sam Harwell
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

namespace AntlrUnitTests.ST4
{
    using Antlr.Runtime;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using StringTemplate;

    using StringBuilder = System.Text.StringBuilder;

    [TestClass]
    public class TestLexer : StringTemplateTestBase
    {
        [TestMethod]
        public void TestOneExpr()
        {
            string template = "<name>";
            string expected = "[[@0,0:0='<',<LDELIM>,1:0], [@1,1:4='name',<ID>,1:1], " +
                              "[@2,5:5='>',<RDELIM>,1:5]]";
            CheckTokens(template, expected);
        }

        [TestMethod]
        public void TestOneExprSurrounded()
        {
            string template = "hi <name> mom";
            string expected = "[[@0,0:2='hi ',<TEXT>,1:0], [@1,3:3='<',<LDELIM>,1:3], " +
                              "[@2,4:7='name',<ID>,1:4], [@3,8:8='>',<RDELIM>,1:8], " +
                              "[@4,9:12=' mom',<TEXT>,1:9]]";
            CheckTokens(template, expected);
        }

        [TestMethod]
        public void TestEscDelim()
        {
            string template = "hi \\<name>";
            string expected = "[[@0,0:0='hi <name>',<TEXT>,1:0]]";
            CheckTokens(template, expected);
        }

        [TestMethod]
        public void TestEscDelimHasCorrectStartChar()
        {
            string template = "<a>\\<dog";
            string expected =
                "[[@0,0:0='<',<LDELIM>,1:0], [@1,1:1='a',<ID>,1:1], [@2,2:2='>',<RDELIM>,1:2], " +
                "[@3,3:0='<dog',<TEXT>,1:3]]";
            CheckTokens(template, expected);
        }

        [TestMethod]
        public void TestEscChar()
        {
            string template = "hi \\x";
            string expected = "[[@0,0:4='hi \\x',<TEXT>,1:0]]";
            CheckTokens(template, expected);
        }

        [TestMethod]
        public void TestString()
        {
            string template = "hi <foo(a=\">\")>";
            string expected = "[[@0,0:2='hi ',<TEXT>,1:0], [@1,3:3='<',<LDELIM>,1:3], " +
                              "[@2,4:6='foo',<ID>,1:4], [@3,7:7='(',<LPAREN>,1:7], " +
                              "[@4,8:8='a',<ID>,1:8], [@5,9:9='=',<EQUALS>,1:9], " +
                              "[@6,10:12='\">\"',<STRING>,1:10], [@7,13:13=')',<RPAREN>,1:13], " +
                              "[@8,14:14='>',<RDELIM>,1:14]]";
            CheckTokens(template, expected);
        }

        [TestMethod]
        public void TestEscInString()
        {
            string template = "hi <foo(a=\">\\\"\")>";
            string expected =
                "[[@0,0:2='hi ',<TEXT>,1:0], [@1,3:3='<',<LDELIM>,1:3], [@2,4:6='foo',<ID>,1:4], " +
                "[@3,7:7='(',<LPAREN>,1:7], [@4,8:8='a',<ID>,1:8], [@5,9:9='=',<EQUALS>,1:9], " +
                "[@6,10:0='\">\"\"',<STRING>,1:10], [@7,15:15=')',<RPAREN>,1:15], " +
                "[@8,16:16='>',<RDELIM>,1:16]]";
            CheckTokens(template, expected);
        }

        [TestMethod]
        public void TestSubtemplate()
        {
            string template = "hi <names:{n | <n>}>";
            string expected =
                "[[@0,0:2='hi ',<TEXT>,1:0], [@1,3:3='<',<LDELIM>,1:3], [@2,4:8='names',<ID>,1:4], [@3,9:9=':',<COLON>,1:9], [@4,10:10='{',<LCURLY>,1:10], [@5,11:11='n',<ID>,1:11], [@6,13:13='|',<PIPE>,1:13], [@7,15:15='<',<LDELIM>,1:15], [@8,16:16='n',<ID>,1:16], [@9,17:17='>',<RDELIM>,1:17], [@10,18:18='}',<RCURLY>,1:18], [@11,19:19='>',<RDELIM>,1:19]]";
            CheckTokens(template, expected);
        }

        [TestMethod]
        public void TestSubtemplateNoArg()
        {
            string template = "hi <names:{ <it>}>";
            string expected =
                "[[@0,0:2='hi ',<TEXT>,1:0], [@1,3:3='<',<LDELIM>,1:3], " +
                    "[@2,4:8='names',<ID>,1:4], [@3,9:9=':',<COLON>,1:9], " +
                    "[@4,10:10='{',<LCURLY>,1:10], [@5,11:11=' ',<TEXT>,1:11], " +
                    "[@6,12:12='<',<LDELIM>,1:12], [@7,13:14='it',<ID>,1:13], " +
                    "[@8,15:15='>',<RDELIM>,1:15], [@9,16:16='}',<RCURLY>,1:16], " +
                    "[@10,17:17='>',<RDELIM>,1:17]]";
            CheckTokens(template, expected);
        }

        [TestMethod]
        public void TestSubtemplateMultiArgs()
        {
            string template = "hi <names:{x,y | <x><y>}>"; // semantically bogus
            string expected =
                "[[@0,0:2='hi ',<TEXT>,1:0], [@1,3:3='<',<LDELIM>,1:3], [@2,4:8='names',<ID>,1:4], [@3,9:9=':',<COLON>,1:9], [@4,10:10='{',<LCURLY>,1:10], [@5,11:11='x',<ID>,1:11], [@6,12:12=',',<COMMA>,1:12], [@7,13:13='y',<ID>,1:13], [@8,15:15='|',<PIPE>,1:15], [@9,17:17='<',<LDELIM>,1:17], [@10,18:18='x',<ID>,1:18], [@11,19:19='>',<RDELIM>,1:19], [@12,20:20='<',<LDELIM>,1:20], [@13,21:21='y',<ID>,1:21], [@14,22:22='>',<RDELIM>,1:22], [@15,23:23='}',<RCURLY>,1:23], [@16,24:24='>',<RDELIM>,1:24]]";
            CheckTokens(template, expected);
        }

        [TestMethod]
        public void TestNestedSubtemplate()
        {
            string template = "hi <names:{n | <n:{<it>}>}>";
            string expected =
                "[[@0,0:2='hi ',<TEXT>,1:0], [@1,3:3='<',<LDELIM>,1:3], [@2,4:8='names',<ID>,1:4], [@3,9:9=':',<COLON>,1:9], [@4,10:10='{',<LCURLY>,1:10], [@5,11:11='n',<ID>,1:11], [@6,13:13='|',<PIPE>,1:13], [@7,15:15='<',<LDELIM>,1:15], [@8,16:16='n',<ID>,1:16], [@9,17:17=':',<COLON>,1:17], [@10,18:18='{',<LCURLY>,1:18], [@11,19:19='<',<LDELIM>,1:19], [@12,20:21='it',<ID>,1:20], [@13,22:22='>',<RDELIM>,1:22], [@14,23:23='}',<RCURLY>,1:23], [@15,24:24='>',<RDELIM>,1:24], [@16,25:25='}',<RCURLY>,1:25], [@17,26:26='>',<RDELIM>,1:26]]";
            CheckTokens(template, expected);
        }

        [TestMethod]
        public void TestNestedList()
        {
            string template =
                "*<[names, [\"foo\",\"bar\"]:{<it>!},phones]; separator=\", \">*";
            string expected =
                "[[@0,0:0='*',<TEXT>,1:0], [@1,1:1='<',<LDELIM>,1:1], [@2,2:2='[',<LBRACK>,1:2], " +
                "[@3,3:7='names',<ID>,1:3], [@4,8:8=',',<COMMA>,1:8], [@5,9:10=' [',<LBRACK>,1:9], " +
                "[@6,11:15='\"foo\"',<STRING>,1:11], [@7,16:16=',',<COMMA>,1:16], " +
                "[@8,17:21='\"bar\"',<STRING>,1:17], [@9,22:22=']',<RBRACK>,1:22], " +
                "[@10,23:23=':',<COLON>,1:23], [@11,24:24='{',<LCURLY>,1:24], " +
                "[@12,25:25='<',<LDELIM>,1:25], [@13,26:27='it',<ID>,1:26], " +
                "[@14,28:28='>',<RDELIM>,1:28], [@15,29:29='!',<TEXT>,1:29], " +
                "[@16,30:30='}',<RCURLY>,1:30], [@17,31:31=',',<COMMA>,1:31], " +
                "[@18,32:37='phones',<ID>,1:32], [@19,38:38=']',<RBRACK>,1:38], " +
                "[@20,39:39=';',<SEMI>,1:39], [@21,41:49='separator',<ID>,1:41], " +
                "[@22,50:50='=',<EQUALS>,1:50], [@23,51:54='\", \"',<STRING>,1:51], " +
                "[@24,55:55='>',<RDELIM>,1:55], [@25,56:56='*',<TEXT>,1:56]]";
            CheckTokens(template, expected);
        }

        [TestMethod]
        public void TestIF()
        {
            string template = "<if(!name)>works<endif>";
            string expected =
                "[[@0,0:0='<',<LDELIM>,1:0], [@1,1:2='if',<IF>,1:1], [@2,3:3='(',<LPAREN>,1:3], " +
                "[@3,4:4='!',<BANG>,1:4], [@4,5:8='name',<ID>,1:5], [@5,9:9=')',<RPAREN>,1:9], " +
                "[@6,10:10='>',<RDELIM>,1:10], [@7,11:15='works',<TEXT>,1:11], " +
                "[@8,16:16='<',<LDELIM>,1:16], [@9,17:21='endif',<ENDIF>,1:17], " +
                "[@10,22:22='>',<RDELIM>,1:22]]";
            CheckTokens(template, expected);
        }

        [TestMethod]
        public void TestIFNot()
        {
            string template = "<if(!name)>works<endif>";
            string expected =
                "[[@0,0:0='<',<LDELIM>,1:0], [@1,1:2='if',<IF>,1:1], [@2,3:3='(',<LPAREN>,1:3], " +
                "[@3,4:4='!',<BANG>,1:4], [@4,5:8='name',<ID>,1:5], [@5,9:9=')',<RPAREN>,1:9], " +
                "[@6,10:10='>',<RDELIM>,1:10], [@7,11:15='works',<TEXT>,1:11], " +
                "[@8,16:16='<',<LDELIM>,1:16], [@9,17:21='endif',<ENDIF>,1:17], " +
                "[@10,22:22='>',<RDELIM>,1:22]]";
            CheckTokens(template, expected);
        }

        [TestMethod]
        public void TestIFELSE()
        {
            string template = "<if(name)>works<else>fail<endif>";
            string expected =
                "[[@0,0:0='<',<LDELIM>,1:0], [@1,1:2='if',<IF>,1:1], [@2,3:3='(',<LPAREN>,1:3], " +
                "[@3,4:7='name',<ID>,1:4], [@4,8:8=')',<RPAREN>,1:8], [@5,9:9='>',<RDELIM>,1:9], " +
                "[@6,10:14='works',<TEXT>,1:10], [@7,15:15='<',<LDELIM>,1:15], " +
                "[@8,16:19='else',<ELSE>,1:16], [@9,20:20='>',<RDELIM>,1:20], " +
                "[@10,21:24='fail',<TEXT>,1:21], [@11,25:25='<',<LDELIM>,1:25], " +
                "[@12,26:30='endif',<ENDIF>,1:26], [@13,31:31='>',<RDELIM>,1:31]]";
            CheckTokens(template, expected);
        }

        [TestMethod]
        public void TestELSEIF()
        {
            string template = "<if(name)>fail<elseif(id)>works<else>fail<endif>";
            string expected =
                "[[@0,0:0='<',<LDELIM>,1:0], [@1,1:2='if',<IF>,1:1], [@2,3:3='(',<LPAREN>,1:3], " +
                "[@3,4:7='name',<ID>,1:4], [@4,8:8=')',<RPAREN>,1:8], [@5,9:9='>',<RDELIM>,1:9], " +
                "[@6,10:13='fail',<TEXT>,1:10], [@7,14:14='<',<LDELIM>,1:14], " +
                "[@8,15:20='elseif',<ELSEIF>,1:15], [@9,21:21='(',<LPAREN>,1:21], " +
                "[@10,22:23='id',<ID>,1:22], [@11,24:24=')',<RPAREN>,1:24], " +
                "[@12,25:25='>',<RDELIM>,1:25], [@13,26:30='works',<TEXT>,1:26], " +
                "[@14,31:31='<',<LDELIM>,1:31], [@15,32:35='else',<ELSE>,1:32], " +
                "[@16,36:36='>',<RDELIM>,1:36], [@17,37:40='fail',<TEXT>,1:37], " +
                "[@18,41:41='<',<LDELIM>,1:41], [@19,42:46='endif',<ENDIF>,1:42], " +
                "[@20,47:47='>',<RDELIM>,1:47]]";
            CheckTokens(template, expected);
        }
    }
}
