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
    using System.Text;

    [TestClass]
    public class TestFastLexer
    {
        [TestMethod]
        public void TestBasicFastLexer()
        {
            string inputElement = "x-2356*Abte+32+eno/6623+y";
            StringBuilder builder = new StringBuilder( inputElement, 1000000 * inputElement.Length );
            for ( int i = 0; i < 999999; i++ )
                builder.Append( inputElement );

            string text = builder.ToString();
            int iterations = 5;
            // warmup
            IterateFast( text, iterations );
            Iterate( text, iterations );

            var time = Iterate( text, iterations );
            var timeFast = IterateFast( text, iterations );

            Console.WriteLine( "Elapsed time (norm): {0} seconds.", time.TotalSeconds );
            Console.WriteLine( "Elapsed time (fast): {0} seconds.", timeFast.TotalSeconds );
        }

        TimeSpan Iterate( string text, int count )
        {
            DateTime start = DateTime.Now;

            for ( int i = 0; i < count; i++ )
            {
                SimpleExpressionLexer lexer = new SimpleExpressionLexer( new ANTLRStringStream( text ) );
                CommonTokenStream tokens = new CommonTokenStream( lexer );
                tokens.Fill();
            }

            return DateTime.Now - start;
        }

        TimeSpan IterateFast( string text, int count )
        {
            DateTime start = DateTime.Now;

            for ( int i = 0; i < count; i++ )
            {
                FastSimpleExpressionLexer lexer = new FastSimpleExpressionLexer( new SlimStringStream( text ) );
                FastTokenStream tokens = new FastTokenStream( lexer );
                //FastTokenStream<SlimToken> tokens = new FastTokenStream<SlimToken>( lexer );
                tokens.FillBuffer();
            }

            return DateTime.Now - start;
        }
    }
}
