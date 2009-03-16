/*
 * [The "BSD licence"]
 * Copyright (c) 2005-2008 Terence Parr
 * All rights reserved.
 *
 * Grammar conversion to ANTLR v3 and C#:
 * Copyright (c) 2008 Sam Harwell, Pixel Mine, Inc.
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions
 * are met:
 * 1. Redistributions of source code must retain the above copyright
 *	notice, this list of conditions and the following disclaimer.
 * 2. Redistributions in binary form must reproduce the above copyright
 *	notice, this list of conditions and the following disclaimer in the
 *	documentation and/or other materials provided with the distribution.
 * 3. The name of the author may not be used to endorse or promote products
 *	derived from this software without specific prior written permission.
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

namespace Antlr3.ST.Language
{
    using System;
    using Antlr.Runtime;

    using IToken = Antlr.Runtime.IToken;

    partial class GroupLexer
    {
        [Flags]
        public enum StateFlags
        {
            LineScanning = 1 << 0,
            InBlockComment = 1 << 1,
            InDocComment = 1 << 2,
            InTextBlock = 1 << 3,
            InAction = 15 << 4,
            InActionComment = 1 << 8,
            InBigString = 1 << 9,
            InAnonymousTemplate = 1 << 10
        }

        bool GetFlag( int bitfield, StateFlags flag )
        {
            return ( bitfield & (int)flag ) == (int)flag;
        }
        int SetFlag( int bitfield, StateFlags flag, bool value )
        {
            bitfield &= ~(int)flag;
            if ( value )
                bitfield |= (int)flag;
            return bitfield;
        }

        int ScannerState
        {
            get;
            set;
        }
        public bool InColorizer
        {
            get;
            set;
        }
        public bool InBlockComment
        {
            get
            {
                return GetFlag( ScannerState, StateFlags.InBlockComment );
            }
            set
            {
                //if ( !value )
                //    InDocComment = value;

                //if ( LineScanning && Line > _startLine )
                //    return;

                ScannerState = SetFlag( ScannerState, StateFlags.InBlockComment, value );
            }
        }
        public bool InBigString
        {
            get
            {
                return GetFlag( ScannerState, StateFlags.InBigString );
            }
            set
            {
                ScannerState = SetFlag( ScannerState, StateFlags.InBigString, value );
            }
        }
        public bool InAnonymousTemplate
        {
            get
            {
                return GetFlag( ScannerState, StateFlags.InAnonymousTemplate );
            }
            set
            {
                ScannerState = SetFlag( ScannerState, StateFlags.InAnonymousTemplate, value );
            }
        }

        public override IToken NextToken()
        {
            if ( !InColorizer )
                return base.NextToken();

            CommonToken token = (CommonToken)base.NextToken();

            if ( InBlockComment && token.Type != EOF )
            {
                if ( token.Type == CLOSE_BLOCK_COMMENT )
                    InBlockComment = false;

                token.Type = ML_COMMENT;
            }
            else if ( InBigString && token.Type != EOF )
            {
                if ( token.Type == CLOSE_BIG_STRING )
                    InBigString = false;

                token.Type = BIGSTRING;
            }
            else if ( InAnonymousTemplate && token.Type != EOF )
            {
                if ( token.Type == CLOSE_ANON_TEMPLATE )
                    InAnonymousTemplate = false;

                token.Type = ANONYMOUS_TEMPLATE;
            }

            if ( token.Type != EOF && token.StartIndex > token.StopIndex )
                throw new OperationCanceledException();

            return token;
        }
    }
}
