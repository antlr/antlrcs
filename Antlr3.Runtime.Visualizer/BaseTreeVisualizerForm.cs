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
    using EventArgs = System.EventArgs;
    using Form = System.Windows.Forms.Form;
    using ITokenStream = Antlr.Runtime.ITokenStream;
    using ITreeAdaptor = Antlr.Runtime.Tree.ITreeAdaptor;

    public partial class BaseTreeVisualizerForm : Form
    {
        public BaseTreeVisualizerForm(ITreeAdaptor adaptor, object tree)
            : this(adaptor, tree, null, null)
        {
        }

        public BaseTreeVisualizerForm(ITreeAdaptor adaptor, object tree, ITokenStream tokenStream)
            : this(adaptor, tree, tokenStream, null)
        {
        }

        public BaseTreeVisualizerForm(ITreeAdaptor adaptor, object tree, string sourceText)
            : this(adaptor, tree, null, sourceText)
        {
        }

        public BaseTreeVisualizerForm(ITreeAdaptor adaptor, object tree, ITokenStream tokenStream, string sourceText)
        {
            if (adaptor == null)
                throw new ArgumentNullException("adaptor");
            if (tree == null)
                throw new ArgumentNullException("tree");

            InitializeComponent();

            TreeVisualizerViewModel viewModel = new TreeVisualizerViewModel(adaptor, tree, tokenStream, sourceText);
            ((BaseTreeVisualizerViewControl)elementHost1.Child).ViewModel = viewModel;
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
