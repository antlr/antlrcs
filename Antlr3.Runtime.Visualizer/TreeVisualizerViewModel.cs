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
    using System;
    using Antlr.Runtime;
    using Antlr.Runtime.Tree;

    public class TreeVisualizerViewModel
    {
        private readonly ITreeAdaptor _adaptor;
        private readonly object _tree;
        private readonly TreeViewModel _treeViewModel;
        private readonly ITokenStream _tokenStream;
        private readonly string _sourceText;

        public TreeVisualizerViewModel(ITreeAdaptor adaptor, object tree, ITokenStream tokenStream, string sourceText)
        {
            if (adaptor == null)
                throw new ArgumentNullException("adaptor");
            if (tree == null)
                throw new ArgumentNullException("tree");

            _adaptor = adaptor;
            _tree = tree;
            _tokenStream = tokenStream;
            _sourceText = sourceText;

            object root = adaptor.Nil();
            adaptor.AddChild(root, tree);
            _treeViewModel = new TreeViewModel(_adaptor, root);

        }

        public ITreeAdaptor Adaptor
        {
            get
            {
                return _adaptor;
            }
        }

        public object Tree
        {
            get
            {
                return _tree;
            }
        }

        public TreeViewModel TreeViewModel
        {
            get
            {
                return _treeViewModel;
            }
        }

        public ITokenStream TokenStream
        {
            get
            {
                return _tokenStream;
            }
        }

        public string SourceText
        {
            get
            {
                if (_sourceText != null)
                    return _sourceText;

                ITokenStream tokenStream = TokenStream;
                if (tokenStream == null)
                    return null;

                Lexer lexer = tokenStream.TokenSource as Lexer;
                if (lexer != null)
                    return lexer.CharStream.Substring(0, lexer.CharStream.Count - 1);

                return _tokenStream.ToString(0, _tokenStream.Count - 1);
            }
        }

        public IToken GetToken(int index)
        {
            ITokenStream tokenStream = TokenStream;
            if (tokenStream == null)
                return null;

            if (index < 0 || index >= tokenStream.Count)
                return null;

            return _tokenStream.Get(index);
        }
    }
}
