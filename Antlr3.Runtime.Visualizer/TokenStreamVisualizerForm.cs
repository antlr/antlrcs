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
    using System.Linq;
    using Antlr.Runtime;

    using Color = System.Drawing.Color;
    using Form = System.Windows.Forms.Form;

    public partial class TokenStreamVisualizerForm : Form
    {
        private readonly ITokenStream _tokenStream;
        private readonly IToken[] _tokens;
        private readonly string[] _tokenNames;

        public TokenStreamVisualizerForm( ITokenStream tokenStream )
        {
            if (tokenStream == null)
                throw new ArgumentNullException("tokenStream");

            InitializeComponent();

            List<IToken> tokens = new List<IToken>();

            int marker = tokenStream.Mark();
            int currentPosition = tokenStream.Index;
            try
            {
                tokenStream.Seek(0);
                while (tokenStream.LA(1) != CharStreamConstants.EndOfFile)
                    tokenStream.Consume();

                for (int i = 0; i < tokenStream.Count; i++)
                    tokens.Add(tokenStream.Get(i));
            }
            finally
            {
                tokenStream.Rewind(marker);
            }

            this._tokenStream = tokenStream;
            this._tokens = tokens.ToArray();

            if (tokenStream.TokenSource != null)
                this._tokenNames = tokenStream.TokenSource.TokenNames;

            this._tokenNames = this._tokenNames ?? new string[0];

            UpdateTokenTypes();
            UpdateHighlighting();

            listBox1.BackColor = Color.Wheat;
        }

        private IToken[] Tokens
        {
            get
            {
                return _tokens;
            }
        }

        private string[] TokenNames
        {
            get
            {
                return _tokenNames;
            }
        }

        private void UpdateHighlighting()
        {
            Dictionary<string, string> selected = listBox1.SelectedItems.Cast<string>().ToDictionary( i => i );

            int selectionStart = richTextBox1.SelectionStart;
            int selectionLength = richTextBox1.SelectionLength;
            bool modify = ( richTextBox1.TextLength == Tokens.Select(GetTokenText).Sum( text => text.Replace( "\r\n", "\n" ).Length ) );

            if ( modify )
            {
                richTextBox1.SelectAll();
                richTextBox1.SelectionBackColor = Color.White;
            }
            else
            {
                richTextBox1.Clear();
            }

            int index = 0;
            foreach ( IToken token in Tokens )
            {
                string text = GetTokenText(token).Replace( "\r\n", "\n" );

                if ( modify )
                    richTextBox1.Select( index, text.Length );

                if ( selected.ContainsKey( GetTokenName( token.Type ) ) )
                    richTextBox1.SelectionBackColor = Color.Yellow;
                else
                    richTextBox1.SelectionBackColor = Color.White;

                if ( !modify )
                    richTextBox1.AppendText( text );

                index += text.Length;
            }

            richTextBox1.Select( selectionStart, selectionLength );
        }

        private string GetTokenText(IToken token)
        {
            if (token.InputStream != null && token.StartIndex >= 0 && token.StopIndex < token.InputStream.Count)
                return token.InputStream.Substring(token.StartIndex, token.StopIndex - token.StartIndex + 1);

            return token.Text ?? string.Empty;
        }

        private void UpdateTokenTypes()
        {
            listBox1.Items.Clear();
            listBox1.Items.AddRange( Tokens.Select( token => token.Type ).Distinct().OrderBy( i => i ).Select( i => (object)GetTokenName( i ) ).ToArray() );
        }

        private string GetTokenName(int i)
        {
            if ( i >= 0 && TokenNames != null && i < TokenNames.Length )
                return TokenNames[i];

            return i.ToString();
        }

        private void listBox1_SelectedIndexChanged( object sender, EventArgs e )
        {
            UpdateHighlighting();
        }

        private void closeToolStripMenuItem_Click( object sender, EventArgs e )
        {
            Close();
        }
    }
}
