/*
 * [The "BSD licence"]
 * Copyright (c) 2003-2008 Terence Parr
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

namespace Antlr3.ST
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using StringBuilder = System.Text.StringBuilder;
    using TextWriter = System.IO.TextWriter;

    /** <summary>
     *  Essentially a char filter that knows how to auto-indent output
     *  by maintaining a stack of indent levels.  I set a flag upon newline
     *  and then next nonwhitespace char resets flag and spits out indention.
     *  The indent stack is a stack of strings so we can repeat original indent
     *  not just the same number of columns (don't have to worry about tabs vs
     *  spaces then).
     *  </summary>
     *
     *  <remarks>
     *  Anchors are char positions (tabs won't work) that indicate where all
     *  future wraps should justify to.  The wrap position is actually the
     *  larger of either the last anchor or the indentation level.
     *
     *  This is a filter on a Writer.
     *
     *  \n is the proper way to say newline for options and templates.
     *  Templates can mix them but use \n for sure and options like
     *  wrap="\n". ST will generate the right thing. Override the default (locale)
     *  newline by passing in a string to the constructor.
     *  </remarks>
     */
    public class AutoIndentWriter : IStringTemplateWriter
    {
        const int NO_WRAP = -1;

        /** <summary>
         *  stack of indents; use List as it's much faster than Stack. Grows
         *  from 0..n-1.  List&lt;String&gt;
         *  </summary>
         */
        List<string> _indents = new List<string>();

        /** <summary>
         *  Stack of integer anchors (char positions in line); avoid Integer
         *  creation overhead.
         *  </summary>
         */
        int[] _anchors = new int[10];
        int _anchors_sp = -1;

        /** <summary>\n or \r\n?</summary> */
        string _newline;

        TextWriter _writer = null;
        bool _atStartOfLine = true;

        /** <summary>
         *  Track char position in the line (later we can think about tabs).
         *  Indexed from 0.  We want to keep charPosition &lt;= lineWidth.
         *  This is the position we are *about* to write not the position
         *  last written to.
         *  </summary>
         */
        int _charPosition = 0;
        int _lineWidth = NO_WRAP;

        public AutoIndentWriter( TextWriter writer, string newline )
        {
            _writer = writer;
            _indents.Add( string.Empty ); // start with no indent
            _newline = newline;
        }

        public AutoIndentWriter( TextWriter writer )
            : this( writer, Environment.NewLine )
        {
        }

        public TextWriter Writer
        {
            get
            {
                return _writer;
            }
        }

        public virtual void setLineWidth( int lineWidth )
        {
            _lineWidth = lineWidth;
        }

        /** <summary>
         *  Push even blank (null) indents as they are like scopes; must
         *  be able to pop them back off stack.
         *  </summary>
         *
         *  <remarks>
         *  To deal with combined anchors and indentation, force indents to
         *  include any current anchor point.  If current anchor is beyond
         *  current indent width, add the difference to the indent to be added.
         *
         *  This prevents a check later to deal with anchors when starting new line.
         *  </remarks>
         */
        public virtual void pushIndentation( string indent )
        {
            int lastAnchor = 0;
            int indentWidth = getIndentationWidth();
            // If current anchor is beyond current indent width, add in difference
            if ( _anchors_sp >= 0 && _anchors[_anchors_sp] > indentWidth )
            {
                lastAnchor = _anchors[_anchors_sp];
                StringBuilder buf = getIndentString( lastAnchor - indentWidth );
                if ( indent != null )
                    buf.Append( indent ); // don't add if null
                _indents.Add( buf.ToString() );
                return;
            }
            _indents.Add( indent ?? string.Empty );
        }

        /// <exception cref="System.ArgumentOutOfRangeException" />
        public virtual string popIndentation()
        {
            string value = _indents[_indents.Count - 1];
            _indents.RemoveAt( _indents.Count - 1 );
            return value;
        }

        /// <exception cref="System.ArgumentNullException" />
        /// <exception cref="System.RankException" />
        /// <exception cref="System.ArrayTypeMismatchException" />
        /// <exception cref="System.InvalidCastException" />
        /// <exception cref="System.ArgumentOutOfRangeException" />
        /// <exception cref="System.ArgumentException" />
        public virtual void pushAnchorPoint()
        {
            if ( ( _anchors_sp + 1 ) >= _anchors.Length )
            {
                int[] a = new int[_anchors.Length * 2];
                Array.Copy( _anchors, 0, a, 0, _anchors.Length );
                _anchors = a;
            }
            _anchors_sp++;
            _anchors[_anchors_sp] = _charPosition;
        }

        public virtual void popAnchorPoint()
        {
            _anchors_sp--;
        }

        /// <exception cref="System.ArgumentNullException" />
        /// <exception cref="System.OverflowException" />
        public virtual int getIndentationWidth()
        {
            return _indents.Sum( s => s.Length );
        }

        /// <summary>Write out a string literal or attribute expression or expression element.</summary>
        /// <exception cref="System.ObjectDisposedException" />
        /// <exception cref="System.IO.IOException" />
        public virtual int write( string str )
        {
            int n = 0;
            for ( int i = 0; i < str.Length; i++ )
            {
                char c = str[i];
                // found \n or \r\n newline?
                if ( c == '\r' || c == '\n' )
                {
                    _atStartOfLine = true;
                    n += _newline.Length; // wrote n more char
                    _writer.Write( _newline );
                    _charPosition = 0; 
                    // skip an extra char upon \r\n
                    if ( ( c == '\r' && ( i + 1 ) < str.Length && str[i+1] == '\n' ) )
                    {
                        i++; // loop iteration i++ takes care of skipping 2nd char
                    }
                    continue;
                }
                // normal character
                // check to see if we are at the start of a line; need indent if so
                if ( _atStartOfLine )
                {
                    n += indent();
                    _atStartOfLine = false;
                }
                n++;
                _writer.Write( c );
                _charPosition++;
            }
            return n;
        }

        /// <exception cref="System.ObjectDisposedException" />
        /// <exception cref="System.IO.IOException" />
        public virtual int writeSeparator( string str )
        {
            return write( str );
        }

        /** <summary>Write out a string literal or attribute expression or expression element.</summary>
         *
         *  <remarks>
         *  If doing line wrap, then check wrap before emitting this str.  If
         *  at or beyond desired line width then emit a \n and any indentation
         *  before spitting out this str.
         *  </remarks>
         *
         *  <exception cref="System.ArgumentNullException" />
         *  <exception cref="System.ObjectDisposedException" />
         *  <exception cref="System.OverflowException" />
         *  <exception cref="System.IO.IOException" />
         */
        public virtual int write( string str, string wrap )
        {
            int n = writeWrapSeparator( wrap );
            return n + write( str );
        }

        /// <exception cref="System.ArgumentNullException" />
        /// <exception cref="System.ObjectDisposedException" />
        /// <exception cref="System.OverflowException" />
        /// <exception cref="System.IO.IOException" />
        public virtual int writeWrapSeparator( string wrap )
        {
            int n = 0;
            // if want wrap and not already at start of line (last char was \n)
            // and we have hit or exceeded the threshold
            if ( _lineWidth != NO_WRAP && wrap != null && !_atStartOfLine &&
                 _charPosition >= _lineWidth )
            {
                // ok to wrap
                // Walk wrap string and look for A\nB.  Spit out A\n
                // then spit indent or anchor, whichever is larger
                // then spit out B.
                for ( int i = 0; i < wrap.Length; i++ )
                {
                    char c = wrap[i];
                    if ( c == '\r' || c == '\n' )
                    {
                        n += _newline.Length;
                        _writer.Write( _newline );
                        _charPosition = 0;
                        n += indent();

                        // handle \r\n in the wrap string
                        if ( (c == '\r') && (i < wrap.Length - 1) && (wrap[i + 1] == '\n') )
                            i++;

                        // continue writing any chars out
                    }
                    else
                    {  // write A or B part
                        n++;
                        _writer.Write( c );
                        _charPosition++;
                    }
                }
            }
            return n;
        }

        /// <exception cref="System.ObjectDisposedException" />
        /// <exception cref="System.IO.IOException" />
        public virtual int indent()
        {
            int n = 0;
            for ( int i = 0; i < _indents.Count; i++ )
            {
                string ind = _indents[i];
                if ( ind != null )
                {
                    n += ind.Length;
                    _writer.Write( ind );
                }
            }
            _charPosition += n;
            return n;
        }

        /// <exception cref="System.ArgumentException" />
        /// <exception cref="System.ObjectDisposedException" />
        /// <exception cref="System.IO.IOException" />
        public virtual int indent( int spaces )
        {
            if ( spaces < 0 )
                throw new ArgumentException( "spaces cannot be negative", "spaces" );

            _writer.Write( new string( ' ', spaces ) );
            _charPosition += spaces;
            return spaces;
        }

        protected virtual StringBuilder getIndentString( int spaces )
        {
            return new StringBuilder( new string( ' ', spaces ) );
        }
    }
}
