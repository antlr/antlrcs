/*
 * [The "BSD licence"]
 * Copyright (c) 2005-2008 Terence Parr
 * All rights reserved.
 *
 * Conversion to C#:
 * Copyright (c) 2008-2009 Sam Harwell, Pixel Mine, Inc.
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

namespace Antlr3.Runtime.Test
{
    using System;
    using Antlr.Runtime;
    using Antlr.Runtime.Tree;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class TestDotTreeGenerator
    {
        [TestMethod]
        public void TestCreateDot()
        {
            ANTLRStringStream input = new ANTLRStringStream( "3 * x" );
            SimpleExpressionLexer lexer = new SimpleExpressionLexer( input );
            CommonTokenStream tokens = new CommonTokenStream( lexer );
            SimpleExpressionParser parser = new SimpleExpressionParser( tokens );
            var result = parser.expression();

            var tree = result.Tree;
            var adaptor = parser.TreeAdaptor;

            DotTreeGenerator gen = new DotTreeGenerator();
            string output = gen.ToDot( tree, adaptor );
            string newline = Environment.NewLine;
            string expected =
                @"digraph {" + newline
                + newline
                + @"	ordering=out;" + newline
                + @"	ranksep=.4;" + newline
                + @"	bgcolor=""lightgrey""; node [shape=box, fixedsize=false, fontsize=12, fontname=""Helvetica-bold"", fontcolor=""blue""" + newline
                + @"		width=.25, height=.25, color=""black"", fillcolor=""white"", style=""filled, solid, bold""];" + newline
                + @"	edge [arrowsize=.5, color=""black"", style=""bold""]" + newline
                + newline
                + @"  n0 [label=""""];" + newline
                + @"  n1 [label=""*""];" + newline
                + @"  n1 [label=""*""];" + newline
                + @"  n2 [label=""3""];" + newline
                + @"  n3 [label=""x""];" + newline
                + @"  n4 [label=""""];" + newline
                + newline
                + @"  n0 -> n1 // """" -> ""*""" + newline
                + @"  n1 -> n2 // ""*"" -> ""3""" + newline
                + @"  n1 -> n3 // ""*"" -> ""x""" + newline
                + @"  n0 -> n4 // """" -> """"" + newline
                + newline
                + @"}" + newline
                + @"";

            Assert.AreEqual( expected, output );
        }
    }
}
