/*
 * [The "BSD licence"]
 * Copyright (c) 2003-2008 Terence Parr
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

using System;
using System.Collections.Generic;

using ITree = Antlr.Runtime.Tree.ITree;

namespace Antlr.Runtime.JavaExtensions
{
    public static class AntlrJavaExtensions
    {
        /** Walk the tree looking for all subtrees.  Return
         *  an enumeration that lets the caller walk the list
         *  of subtree roots found herein.
         */
        public static IEnumerable<ITree> findAllPartial( this ITree tree, ITree node )
        {
            return DoWorkForFindAll( tree, node, true );
        }

        #region Helper functions

        private static bool equals( this ITree tree, ITree target )
        {
            if ( tree == null || target == null )
                return false;

            string treeText = tree.Text;
            string targetText = target.Text;

            if ( ( treeText == null ) != ( targetText == null ) )
                return false;

            if ( treeText == null )
                return tree.Type == target.Type;

            return ( treeText == targetText ) && ( tree.Type == target.Type );
        }

        private static bool equalsTree( this ITree tree, ITree target )
        {
            // if either tree is nil, then they are equal only if both are nil
            bool treeNil = ( ( tree == null ) || ( tree.IsNil && tree.ChildCount == 0 ) );
            bool targetNil = ( ( target == null ) || ( target.IsNil && target.ChildCount == 0 ) );
            if ( treeNil || targetNil )
                return ( treeNil == targetNil );

            if ( tree.ChildCount != target.ChildCount )
                return false;

            // check roots first
            if ( !tree.equals( target ) )
                return false;

            // if roots match, do a full list match on children.
            // we know tree and target have the same ChildCount.
            int childCount = tree.ChildCount;
            for ( int i = 0; i < childCount; i++ )
            {
                if ( !tree.GetChild( i ).equalsTree( target.GetChild( i ) ) )
                {
                    return false;
                }
            }

            return true;
        }

        private static bool equalsTreePartial( this ITree tree, ITree target )
        {
            // an empty tree is always a subset of any tree
            if ( target == null || target.IsNil )
                return true;

            if ( target.ChildCount > tree.ChildCount )
                return false;

            // check roots first
            if ( !tree.equals( target ) )
                return false;

            // if roots match, do a full list partial test on children
            int childCount = Math.Min( tree.ChildCount, target.ChildCount );
            for ( int i = 0; i < childCount; i++ )
            {
                if ( !tree.GetChild( i ).equalsTreePartial( target.GetChild( i ) ) )
                {
                    return false;
                }
            }

            return true;
        }

        private static IEnumerable<ITree> DoWorkForFindAll( ITree nodeToSearch, ITree target, bool partialMatch )
        {
            if ( ( partialMatch && nodeToSearch.equalsTreePartial( target ) )
                || ( !partialMatch && nodeToSearch.equalsTree( target ) ) )
            {
                yield return nodeToSearch;
            }

            for ( int i = 0; i < nodeToSearch.ChildCount; i++ )
            {
                ITree child = nodeToSearch.GetChild( i );
                if ( child != null )
                {
                    foreach ( ITree tree in DoWorkForFindAll( child, target, partialMatch ) )
                        yield return tree;
                }
            }
        }

        #endregion
    }
}
