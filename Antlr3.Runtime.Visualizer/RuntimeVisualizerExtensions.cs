/*
 * [The "BSD license"]
 * Copyright (c) 2011 Terence Parr
 * All rights reserved.
 *
 * Conversion to C#:
 * Copyright (c) 2011 Sam Harwell, Tunnel Vision Laboratories, LLC
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

namespace Antlr3.Runtime.Visualizer
{
    using ArgumentNullException = System.ArgumentNullException;
    using CommonTreeAdaptor = Antlr.Runtime.Tree.CommonTreeAdaptor;
    using ITokenStream = Antlr.Runtime.ITokenStream;
    using ITree = Antlr.Runtime.Tree.ITree;
    using ITreeAdaptor = Antlr.Runtime.Tree.ITreeAdaptor;
    using IWin32Window = System.Windows.Forms.IWin32Window;

    public static class RuntimeVisualizerExtensions
    {
        public static void Visualize(this ITree tree)
        {
            Visualize(new CommonTreeAdaptor(), tree, default(ITokenStream));
        }

        public static void Visualize(this ITree tree, ITokenStream tokenStream)
        {
            Visualize(new CommonTreeAdaptor(), tree, tokenStream);
        }

        public static void Visualize(this ITree tree, IWin32Window owner)
        {
            Visualize(new CommonTreeAdaptor(), tree, null, owner);
        }

        public static void Visualize(this ITree tree, ITokenStream tokenStream, IWin32Window owner)
        {
            Visualize(new CommonTreeAdaptor(), tree, tokenStream, owner);
        }

        public static void Visualize(this ITreeAdaptor treeAdaptor, object tree)
        {
            Visualize(treeAdaptor, tree, default(ITokenStream));
        }

        public static void Visualize(this ITreeAdaptor treeAdaptor, object tree, ITokenStream tokenStream)
        {
            if (treeAdaptor == null)
                throw new ArgumentNullException("treeAdaptor");
            if (tree == null)
                throw new ArgumentNullException("tree");

            BaseTreeVisualizerForm visualizer = new BaseTreeVisualizerForm(treeAdaptor, tree, tokenStream);
            visualizer.ShowDialog();
        }

        public static void Visualize(this ITreeAdaptor treeAdaptor, object tree, IWin32Window owner)
        {
            Visualize(treeAdaptor, tree, null, owner);
        }

        public static void Visualize(this ITreeAdaptor treeAdaptor, object tree, ITokenStream tokenStream, IWin32Window owner)
        {
            if (treeAdaptor == null)
                throw new ArgumentNullException("treeAdaptor");
            if (tree == null)
                throw new ArgumentNullException("tree");

            BaseTreeVisualizerForm visualizer = new BaseTreeVisualizerForm(treeAdaptor, tree, tokenStream);
            visualizer.ShowDialog(owner);
        }

        public static void Visualize(this ITokenStream tokenStream)
        {
            if (tokenStream == null)
                throw new ArgumentNullException("tokenStream");

            TokenStreamVisualizerForm visualizer = new TokenStreamVisualizerForm(tokenStream);
            visualizer.ShowDialog();
        }

        public static void Visualize(this ITokenStream tokenStream, IWin32Window owner)
        {
            if (tokenStream == null)
                throw new ArgumentNullException("tokenStream");

            TokenStreamVisualizerForm visualizer = new TokenStreamVisualizerForm(tokenStream);
            visualizer.ShowDialog(owner);
        }
    }
}
