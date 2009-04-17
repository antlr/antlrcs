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
    using Antlr.Runtime.JavaExtensions;

    using Graph = Antlr3.Misc.Graph<string>;

    /** Test topo sort in GraphNode. */
    [TestClass]
    public class TestTopologicalSort : BaseTest
    {
        [TestMethod]
        public virtual void TestFairlyLargeGraph()
        {
            Graph g = new Graph();
            g.AddEdge( "C", "F" );
            g.AddEdge( "C", "G" );
            g.AddEdge( "C", "A" );
            g.AddEdge( "C", "B" );
            g.AddEdge( "A", "D" );
            g.AddEdge( "A", "E" );
            g.AddEdge( "B", "E" );
            g.AddEdge( "D", "E" );
            g.AddEdge( "D", "F" );
            g.AddEdge( "F", "H" );
            g.AddEdge( "E", "F" );
            g.AddEdge( "G", "A" );

            string expecting = "[H, F, E, D, A, G, B, C]";
            var nodes = g.Sort();
            string result = nodes.ToElementString();
            assertEquals( expecting, result );
        }

        [TestMethod]
        public virtual void TestCyclicGraph()
        {
            Graph g = new Graph();
            g.AddEdge( "A", "B" );
            g.AddEdge( "B", "C" );
            g.AddEdge( "C", "A" );
            g.AddEdge( "C", "D" );

            string expecting = "[D, C, B, A]";
            var nodes = g.Sort();
            string result = nodes.ToElementString();
            assertEquals( expecting, result );
        }

        [TestMethod]
        public virtual void TestRepeatedEdges()
        {
            Graph g = new Graph();
            g.AddEdge( "A", "B" );
            g.AddEdge( "B", "C" );
            g.AddEdge( "A", "B" ); // dup
            g.AddEdge( "C", "D" );

            string expecting = "[D, C, B, A]";
            var nodes = g.Sort();
            string result = nodes.ToElementString();
            assertEquals( expecting, result );
        }

        [TestMethod]
        public virtual void TestSimpleTokenDependence()
        {
            Graph g = new Graph();
            g.AddEdge( "Java.g", "MyJava.tokens" ); // Java feeds off manual token file
            g.AddEdge( "Java.tokens", "Java.g" );
            g.AddEdge( "Def.g", "Java.tokens" );    // walkers feed off generated tokens
            g.AddEdge( "Ref.g", "Java.tokens" );

            string expecting = "[MyJava.tokens, Java.g, Java.tokens, Def.g, Ref.g]";
            var nodes = g.Sort();
            string result = nodes.ToElementString();
            assertEquals( expecting, result );
        }

        [TestMethod]
        public virtual void TestParserLexerCombo()
        {
            Graph g = new Graph();
            g.AddEdge( "JavaLexer.tokens", "JavaLexer.g" );
            g.AddEdge( "JavaParser.g", "JavaLexer.tokens" );
            g.AddEdge( "Def.g", "JavaLexer.tokens" );
            g.AddEdge( "Ref.g", "JavaLexer.tokens" );

            string expecting = "[JavaLexer.g, JavaLexer.tokens, JavaParser.g, Def.g, Ref.g]";
            var nodes = g.Sort();
            string result = nodes.ToElementString();
            assertEquals( expecting, result );
        }
    }
}
