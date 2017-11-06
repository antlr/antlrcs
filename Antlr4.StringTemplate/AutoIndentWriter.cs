/*
 * [The "BSD licence"]
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

namespace Antlr4.StringTemplate
{
    using System.Collections.Generic;
    using Antlr.Runtime.Misc;

    using Environment = System.Environment;
    using TextWriter = System.IO.TextWriter;

    /** Essentially a char filter that knows how to auto-indent output
     *  by maintaining a stack of indent levels.
     *
     *  The indent stack is a stack of strings so we can repeat original indent
     *  not just the same number of columns (don't have to worry about tabs vs
     *  spaces then).
     *
     *  Anchors are char positions (tabs won't work) that indicate where all
     *  future wraps should justify to.  The wrap position is actually the
     *  larger of either the last anchor or the indentation level.
     *
     *  This is a filter on a Writer.
     *
     *  \n is the proper way to say newline for options and templates.
     *  Templates can mix \r\n and \n them but use \n for sure in options like
     *  wrap="\n". Template will generate the right thing. Override the default (locale)
     *  newline by passing in a string to the constructor.
     */
    public class AutoIndentWriter : ITemplateWriter
    {
        public const int NoWrap = -1;

        /// <summary>
        /// Stack of indents
        /// </summary>
        private readonly ListStack<string> _indents = new ListStack<string>();

        /// <summary>
        /// Stack of integer anchors (char positions in line)
        /// </summary>
        private readonly Stack<int> _anchors = new Stack<int>();

        /// <summary>
        /// The newline character used for this writer
        /// </summary>
        private readonly string _newline;

        /// <summary>
        /// The underlying output stream
        /// </summary>
        private TextWriter _writer = null;

        /** Track char position in the line (later we can think about tabs).
         *  Indexed from 0.  We want to keep charPosition &lt;= lineWidth.
         *  This is the position we are *about* to Write not the position
         *  last written to.
         */
        private int _charPosition = 0;

        /// <summary>
        /// The absolute char index into the output of the next char to be written.
        /// </summary>
        private int _charIndex = 0;

        private int _lineWidth = NoWrap;

        public AutoIndentWriter(TextWriter writer)
            : this(writer, Environment.NewLine)
        {
        }

        public AutoIndentWriter(TextWriter writer, string newline)
        {
            this._writer = writer;
            _indents.Push(null); // s oftart with no indent
            this._newline = newline;
        }

        public int Index
        {
            get
            {
                return _charIndex;
            }

            protected set
            {
                _charIndex = value;
            }
        }

        public int LineWidth
        {
            get
            {
                return _lineWidth;
            }

            set
            {
                _lineWidth = value;
            }
        }

        protected bool AtStartOfLine
        {
            get
            {
                return _charPosition == 0;
            }
        }

        protected TextWriter Writer
        {
            get
            {
                return _writer;
            }

            set
            {
                _writer = value;
            }
        }

        public virtual void PushIndentation(string indent)
        {
            _indents.Push(indent);
        }

        public virtual string PopIndentation()
        {
            return _indents.Pop();
        }

        public virtual void PushAnchorPoint()
        {
            _anchors.Push(_charPosition);
        }

        public virtual void PopAnchorPoint()
        {
            _anchors.Pop();
        }

        /** Write out a string literal or attribute expression or expression element.*/
        public virtual int Write(string value)
        {
            int n = 0;
            int valueLength = value.Length;
            int newlineLength = _newline.Length;
            for (int i = 0; i < valueLength; i++)
            {
                char c = value[i];
                // found \n or \r\n newline?
                if (c == '\r')
                    continue;

                if (c == '\n')
                {
                    Writer.Write(_newline);
                    _charPosition = 0;
                    n += newlineLength;
                    Index += newlineLength;
                    continue;
                }

                // normal character
                // check to see if we are at the start of a line; need indent if so
                if (AtStartOfLine)
                {
                    n += Indent();
                }

                n++;
                Writer.Write(c);
                _charPosition++;
                Index++;
            }
            return n;
        }

        public virtual int WriteSeparator(string value)
        {
            return Write(value);
        }

        /** Write out a string literal or attribute expression or expression element.
         *
         *  If doing line wrap, then check wrap before emitting this str.  If
         *  at or beyond desired line width then emit a \n and any indentation
         *  before spitting out this str.
         */
        public virtual int Write(string value, string wrap)
        {
            int n = WriteWrap(wrap);
            return n + Write(value);
        }

        public virtual int WriteWrap(string wrap)
        {
            int n = 0;
            // if want wrap and not already at start of line (last char was \n)
            // and we have hit or exceeded the threshold
            if (LineWidth != NoWrap && wrap != null && !AtStartOfLine && _charPosition >= LineWidth)
            {
                // ok to wrap
                // Walk wrap string and look for A\nB.  Spit out A\n
                // then spit indent or anchor, whichever is larger
                // then spit out B.
                for (int i = 0; i < wrap.Length; i++)
                {
                    char c = wrap[i];
                    if (c == '\r')
                        continue;

                    if (c == '\n')
                    {
                        Writer.Write(_newline);
                        n += _newline.Length;
                        _charPosition = 0;
                        Index += _newline.Length;
                        n += Indent();
                        // continue writing any chars out
                    }
                    else
                    {
                        // Write A or B part
                        n++;
                        Writer.Write(c);
                        _charPosition++;
                        Index++;
                    }
                }
            }
            return n;
        }

        protected virtual int Indent()
        {
            int n = 0;
            foreach (string ind in _indents)
            {
                if (ind != null)
                {
                    n += ind.Length;
                    Writer.Write(ind);
                }
            }

            // If current anchor is beyond current indent width, indent to anchor
            // *after* doing indents (might tabs in there or whatever)
            int indentWidth = n;
            if (_anchors.Count > 0 && _anchors.Peek() > indentWidth)
            {
                int remainder = _anchors.Peek() - indentWidth;
                Writer.Write(new string(' ', remainder));
                n += remainder;
            }

            _charPosition += n;
            Index += n;
            return n;
        }
    }
}
