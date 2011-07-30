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
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using Antlr.Runtime.Tree;

    public class TreeViewModel
    {
        private readonly ITreeAdaptor _adaptor;
        private readonly object _tree;
        private ReadOnlyCollection<TreeViewModel> _children;

        public TreeViewModel(ITreeAdaptor adaptor, object tree)
        {
            if (adaptor == null)
                throw new ArgumentNullException("adaptor");
            if (tree == null)
                throw new ArgumentNullException("tree");

            _adaptor = adaptor;
            _tree = tree;
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

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public ReadOnlyCollection<TreeViewModel> Children
        {
            get
            {
                if (_children == null)
                {
                    List<TreeViewModel> children = new List<TreeViewModel>();
                    int count = _adaptor.GetChildCount(_tree);
                    for (int i = 0; i < count; i++)
                    {
                        object child = _adaptor.GetChild(_tree, i);
                        if (child == null)
                            continue;

                        children.Add(new TreeViewModel(_adaptor, child));
                    }

                    _children = children.AsReadOnly();
                }

                return _children;
            }
        }

        public string Text
        {
            get
            {
                return _adaptor.GetText(_tree);
            }
        }
    }
}
