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

namespace Antlr3.Misc
{
    using System.Collections.Generic;

    public class Graph<T>
    {
        protected class Node
        {
            internal T _payload;
            internal List<Node> _edges; // points at which nodes?

            public Node( T payload )
            {
                _payload = payload;
            }

            public void AddEdge( Node n )
            {
                if ( _edges == null )
                    _edges = new List<Node>();
                if ( !_edges.Contains( n ) )
                    _edges.Add( n );
            }

            public override string ToString()
            {
                return _payload.ToString();
            }
        }

        /** Map from node payload to node containing it */
        Dictionary<object, Node> nodes = new Dictionary<object, Node>();

        public void AddEdge( T a, T b )
        {
            //System.Console.Out.WriteLine( "add edge " + a + " to " + b );
            Node a_node = GetNode( a );
            Node b_node = GetNode( b );
            a_node.AddEdge( b_node );
        }

        private Node GetNode( T a )
        {
            Node existing;
            if ( nodes.TryGetValue( a, out existing ) && existing != null )
                return existing;
            Node n = new Node( a );
            nodes[a] = n;
            return n;
        }

        /** DFS-based topological sort.  A valid sort is the reverse of
         *  the post-order DFA traversal.  Amazingly simple but true.
         *  For sorting, I'm not following convention here since ANTLR
         *  needs the opposite.  Here's what I assume for sorting:
         *
         *    If there exists an edge u -> v then u depends on v and v
         *    must happen before u.
         *
         *  So if this gives nonreversed postorder traversal, I get the order
         *  I want.
         */
        public List<T> Sort()
        {
            HashSet<Node> visited = new HashSet<Node>();
            List<T> sorted = new List<T>();
            while ( visited.Count < nodes.Count )
            {
                // pick any unvisited node, n
                Node n = null;
                foreach ( var value in nodes.Values )
                {
                    n = value;
                    if ( !visited.Contains( n ) )
                        break;
                }
                DepthFirstSort( n, visited, sorted );
            }
            return sorted;
        }

        private void DepthFirstSort( Node n, HashSet<Node> visited, List<T> sorted )
        {
            if ( visited.Contains( n ) )
                return;
            visited.Add( n );
            if ( n._edges != null )
            {
                foreach ( var target in n._edges )
                {
                    DepthFirstSort( target, visited, sorted );
                }
            }
            sorted.Add( n._payload );
        }
    }
}
