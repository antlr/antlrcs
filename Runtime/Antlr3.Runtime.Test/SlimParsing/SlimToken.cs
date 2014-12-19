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
    using System;
    using System.Runtime.InteropServices;

    [StructLayout( LayoutKind.Sequential, Pack = 2 )]
    public struct SlimToken
        : IToken
    {
        private short _type;
        private short _channel;
        private short _startIndex;
        private short _stopIndex;

        public SlimToken(int type)
            : this()
        {
            _type = (short)type;
        }

        #region IToken Members

        string IToken.Text
        {
            get
            {
                return string.Empty;
            }
            set
            {
            }
        }

        public int Type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = (short)value;
            }
        }

        int IToken.Line
        {
            get
            {
                return 0;
            }
            set
            {
            }
        }

        int IToken.CharPositionInLine
        {
            get
            {
                return 0;
            }
            set
            {
            }
        }

        public int Channel
        {
            get
            {
                return _channel;
            }
            set
            {
                _channel = (short)value;
            }
        }

        public int StartIndex
        {
            get
            {
                return _startIndex;
            }
            set
            {
                _startIndex = (short)value;
            }
        }

        public int StopIndex
        {
            get
            {
                return _stopIndex;
            }
            set
            {
                _stopIndex = (short)value;
            }
        }

        int IToken.TokenIndex
        {
            get
            {
                return 0;
            }
            set
            {
            }
        }

        ICharStream IToken.InputStream
        {
            get
            {
                return null;
            }
            set
            {
            }
        }

        #endregion
    }
}
