/*
 * [The "BSD licence"]
 * Copyright (c) 2005-2008 Terence Parr
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

namespace Antlr3.Analysis
{
    using Grammar = Antlr3.Tool.Grammar;
    using IIntSet = Antlr3.Misc.IIntSet;
    using IntervalSet = Antlr3.Misc.IntervalSet;

    /** An LL(1) lookahead set; contains a set of token types and a "hasEOF"
     *  condition when the set contains EOF.  Since EOF is -1 everywhere and -1
     *  cannot be stored in my BitSet, I set a condition here.  There may be other
     *  reasons in the future to abstract a LookaheadSet over a raw BitSet.
     */
    public class LookaheadSet
    {
        public IntervalSet tokenTypeSet;

        public LookaheadSet()
        {
            tokenTypeSet = new IntervalSet();
        }

        public LookaheadSet( IIntSet s )
            : this()
        {
            tokenTypeSet.addAll( s );
        }

        public LookaheadSet( int atom )
        {
            tokenTypeSet = IntervalSet.of( atom );
        }

        public LookaheadSet( LookaheadSet other )
            : this()
        {
            this.tokenTypeSet.addAll( other.tokenTypeSet );
        }

        #region Properties
        public bool IsNil
        {
            get
            {
                return tokenTypeSet.IsNil;
            }
        }
        #endregion

        public virtual void OrInPlace( LookaheadSet other )
        {
            this.tokenTypeSet.addAll( other.tokenTypeSet );
        }

        public virtual LookaheadSet Or( LookaheadSet other )
        {
            return new LookaheadSet( tokenTypeSet.or( other.tokenTypeSet ) );
        }

        public virtual LookaheadSet Subtract( LookaheadSet other )
        {
            return new LookaheadSet( this.tokenTypeSet.subtract( other.tokenTypeSet ) );
        }

        public virtual bool Member( int a )
        {
            return tokenTypeSet.member( a );
        }

        public virtual LookaheadSet Intersection( LookaheadSet s )
        {
            IIntSet i = this.tokenTypeSet.and( s.tokenTypeSet );
            LookaheadSet intersection = new LookaheadSet( i );
            return intersection;
        }

        public virtual void Remove( int a )
        {
            tokenTypeSet = (IntervalSet)tokenTypeSet.subtract( IntervalSet.of( a ) );
        }

        public override int GetHashCode()
        {
            return tokenTypeSet.GetHashCode();
        }

        public override bool Equals( object other )
        {
            return tokenTypeSet.Equals( ( (LookaheadSet)other ).tokenTypeSet );
        }

        public virtual string ToString( Grammar g )
        {
            if ( tokenTypeSet == null )
            {
                return "";
            }
            string r = tokenTypeSet.ToString( g );
            return r;
        }

        public override string ToString()
        {
            return ToString( null );
        }
    }
}
