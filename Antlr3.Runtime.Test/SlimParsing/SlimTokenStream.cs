/*
 * [The "BSD license"]
 * Copyright (c) 2011 Terence Parr
 * All rights reserved.
 *
 * Conversion to C#:
 * Copyright (c) 2011 Sam Harwell, Pixel Mine, Inc.
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

namespace Antlr.Runtime
{
    using System.Collections.Generic;
    using System;
    using System.Text;

    /** <summary>
     *  A pretty quick CharStream that pulls all data from an array
     *  directly.  Every method call counts in the lexer.  Java's
     *  strings aren't very good so I'm avoiding.
     *  </summary>
     */
    [System.Serializable]
    public class FastTokenStream
        : ITokenStream<SlimToken>
    {
        [System.NonSerialized]
        ITokenSource<SlimToken> _tokenSource;

        /** <summary>
         *  Record every single token pulled from the source so we can reproduce
         *  chunks of it later.
         *  </summary>
         */
        protected List<SlimToken> tokens;

        /** <summary>Skip tokens on any channel but this one; this is how we skip whitespace...</summary> */
        protected int channel = TokenChannels.Default;

        /** <summary>Track the last mark() call result value for use in rewind().</summary> */
        protected int lastMarker;

        /** <summary>
         *  The index into the tokens list of the current token (next token
         *  to consume).  p==-1 indicates that the tokens list is empty
         *  </summary>
         */
        protected int p = -1;

        public FastTokenStream()
        {
            tokens = new List<SlimToken>( 500 );
        }

        public FastTokenStream( ITokenSource<SlimToken> tokenSource )
            : this()
        {
            this._tokenSource = tokenSource;
        }

        public FastTokenStream( ITokenSource<SlimToken> tokenSource, int channel )
            : this( tokenSource )
        {
            this.channel = channel;
        }

        public int Index
        {
            get
            {
                return p;
            }
        }

        /** <summary>Reset this token stream by setting its token source.</summary> */
        public void SetTokenSource( ITokenSource<SlimToken> tokenSource )
        {
            this._tokenSource = tokenSource;
            tokens.Clear();
            p = -1;
            channel = TokenChannels.Default;
        }

        /** <summary>
         *  Load all tokens from the token source and put in tokens.
         *  This is done upon first LT request because you might want to
         *  set some token type / channel overrides before filling buffer.
         *  </summary>
         */
        public void FillBuffer()
        {
            // fast return if the buffer is already full
            if ( p != -1 )
                return;

            int index = 0;
            SlimToken t = _tokenSource.NextToken();
            while ( t.Type != CharStreamConstants.EndOfFile )
            {
                //t.TokenIndex = index;
                tokens.Add( t );
                index++;

                t = _tokenSource.NextToken();
            }
            // leave p pointing at first token on channel
            p = 0;
            p = SkipOffTokenChannels( p );
        }

        /** <summary>
         *  Move the input pointer to the next incoming token.  The stream
         *  must become active with LT(1) available.  consume() simply
         *  moves the input pointer so that LT(1) points at the next
         *  input symbol. Consume at least one token.
         *  </summary>
         *
         *  <remarks>
         *  Walk past any token not on the channel the parser is listening to.
         *  </remarks>
         */
        public void Consume()
        {
            if ( p < tokens.Count )
            {
                p++;
                p = SkipOffTokenChannels( p ); // leave p on valid token
            }
        }

        /** <summary>Given a starting index, return the index of the first on-channel token.</summary> */
        protected int SkipOffTokenChannels( int i )
        {
            int n = tokens.Count;
            while ( i < n && tokens[i].Channel != channel )
            {
                i++;
            }
            return i;
        }

        protected int SkipOffTokenChannelsReverse( int i )
        {
            while ( i >= 0 && tokens[i].Channel != channel )
            {
                i--;
            }
            return i;
        }

        public IList<SlimToken> GetTokens()
        {
            if ( p == -1 )
            {
                FillBuffer();
            }
            return tokens;
        }

        public IList<SlimToken> GetTokens( int start, int stop )
        {
            return GetTokens( start, stop, (BitSet)null );
        }

        /** <summary>
         *  Given a start and stop index, return a List of all tokens in
         *  the token type BitSet.  Return null if no tokens were found.  This
         *  method looks at both on and off channel tokens.
         *  </summary>
         */
        public IList<SlimToken> GetTokens( int start, int stop, BitSet types )
        {
            if ( p == -1 )
            {
                FillBuffer();
            }
            if ( stop >= tokens.Count )
            {
                stop = tokens.Count - 1;
            }
            if ( start < 0 )
            {
                start = 0;
            }
            if ( start > stop )
            {
                return null;
            }

            // list = tokens[start:stop]:{Token t, t.getType() in types}
            List<SlimToken> filteredTokens = new List<SlimToken>();
            for ( int i = start; i <= stop; i++ )
            {
                SlimToken t = tokens[i];
                if ( types == null || types.Member( t.Type ) )
                {
                    filteredTokens.Add( t );
                }
            }
            if ( filteredTokens.Count == 0 )
            {
                filteredTokens = null;
            }
            return filteredTokens;
        }

        public IList<SlimToken> GetTokens( int start, int stop, IList<int> types )
        {
            return GetTokens( start, stop, new BitSet( types ) );
        }

        public IList<SlimToken> GetTokens( int start, int stop, int ttype )
        {
            return GetTokens( start, stop, BitSet.Of( ttype ) );
        }

        /** <summary>
         *  Get the ith token from the current position 1..n where k=1 is the
         *  first symbol of lookahead.
         *  </summary>
         */
        public SlimToken LT( int k )
        {
            if ( p == -1 )
            {
                FillBuffer();
            }
            if ( k == 0 )
            {
                return default( SlimToken );
            }
            if ( k < 0 )
            {
                return LB( -k );
            }
            //System.out.print("LT(p="+p+","+k+")=");
            if ( ( p + k - 1 ) >= tokens.Count )
            {
                return new SlimToken(TokenTypes.EndOfFile);
            }
            //System.out.println(tokens.get(p+k-1));
            int i = p;
            int n = 1;
            // find k good tokens
            while ( n < k )
            {
                // skip off-channel tokens
                i = SkipOffTokenChannels( i + 1 ); // leave p on valid token
                n++;
            }
            if ( i >= tokens.Count )
            {
                return new SlimToken(TokenTypes.EndOfFile);
            }
            return tokens[i];
        }

        /** <summary>Look backwards k tokens on-channel tokens</summary> */
        protected SlimToken LB( int k )
        {
            //System.out.print("LB(p="+p+","+k+") ");
            if ( p == -1 )
            {
                FillBuffer();
            }
            if ( k == 0 )
            {
                return default( SlimToken );
            }
            if ( ( p - k ) < 0 )
            {
                return default( SlimToken );
            }

            int i = p;
            int n = 1;
            // find k good tokens looking backwards
            while ( n <= k )
            {
                // skip off-channel tokens
                i = SkipOffTokenChannelsReverse( i - 1 ); // leave p on valid token
                n++;
            }
            if ( i < 0 )
            {
                return default( SlimToken );
            }
            return tokens[i];
        }

        /** <summary>
         *  Return absolute token i; ignore which channel the tokens are on;
         *  that is, count all tokens not just on-channel tokens.
         *  </summary>
         */
        public SlimToken Get( int i )
        {
            return tokens[i];
        }

        public int LA( int i )
        {
            return LT( i ).Type;
        }

        public int Mark()
        {
            if ( p == -1 )
            {
                FillBuffer();
            }
            lastMarker = Index;
            return lastMarker;
        }

        public void Release( int marker )
        {
            // no resources to release
        }

        public int Count
        {
            get
            {
                return tokens.Count;
            }
        }

        public void Rewind( int marker )
        {
            Seek( marker );
        }

        public void Rewind()
        {
            Seek( lastMarker );
        }

        public void Reset()
        {
            p = 0;
            lastMarker = 0;
        }

        public void Seek( int index )
        {
            p = index;
        }

        public ITokenSource<SlimToken> TokenSource
        {
            get
            {
                return _tokenSource;
            }
        }

        public string SourceName
        {
            get
            {
                return TokenSource.SourceName;
            }
        }

        public override string ToString()
        {
            if ( p == -1 )
            {
                throw new InvalidOperationException( "Buffer is not yet filled." );
            }
            return ToString( 0, tokens.Count - 1 );
        }

        public virtual string ToString( int start, int stop )
        {
            if ( start < 0 || stop < 0 )
            {
                return null;
            }
            if ( p == -1 )
            {
                throw new InvalidOperationException( "Buffer is not yet filled." );
            }
            if ( stop >= tokens.Count )
            {
                stop = tokens.Count - 1;
            }
            StringBuilder buf = new StringBuilder();
            for ( int i = start; i <= stop; i++ )
            {
                SlimToken t = tokens[i];
                SlimLexer lexer = _tokenSource as SlimLexer;
                if ( lexer != null )
                {
                    SlimStringStream input = lexer.CharStream as SlimStringStream;
                    if ( input != null )
                    {
                        string text = input.Substring( t.StartIndex, t.StopIndex - t.StartIndex + 1 );
                        buf.Append( text );
                    }
                }
            }
            return buf.ToString();
        }

        public virtual string ToString( IToken start, IToken stop )
        {
            if ( start != null && stop != null )
            {
                return ToString( start.TokenIndex, stop.TokenIndex );
            }
            return null;
        }
    }
}
