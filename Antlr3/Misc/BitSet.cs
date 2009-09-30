/*
 * [The "BSD licence"]
 * Copyright (c) 2005-2008 Terence Parr
 * All rights reserved.
 *
 * Conversion to C#:
 * Copyright (c) 2008-2009 Sam Harwell, Pixel Mine, Inc.
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

namespace Antlr3.Misc
{
    using System.Collections.Generic;
    using System.Linq;
    using Antlr.Runtime.JavaExtensions;

    using ArgumentException = System.ArgumentException;
    using Array = System.Array;
    using CLSCompliant = System.CLSCompliantAttribute;
    using Grammar = Antlr3.Tool.Grammar;
    using ICloneable = System.ICloneable;
    using IDictionary = System.Collections.IDictionary;
    using IEnumerable = System.Collections.IEnumerable;
    using IList = System.Collections.IList;
    using Label = Antlr3.Analysis.Label;
    using Math = System.Math;
    using NotImplementedException = System.NotImplementedException;
    using Obsolete = System.ObsoleteAttribute;
    using StringBuilder = System.Text.StringBuilder;

    /**A BitSet to replace java.util.BitSet.
     *
     * Primary differences are that most set operators return new sets
     * as opposed to oring and anding "in place".  Further, a number of
     * operations were added.  I cannot contain a BitSet because there
     * is no way to access the internal bits (which I need for speed)
     * and, because it is final, I cannot subclass to add functionality.
     * Consider defining set degree.  Without access to the bits, I must
     * call a method n times to test the ith bit...ack!
     *
     * Also seems like or() from util is wrong when size of incoming set is bigger
     * than this.bits.length.
     *
     * @author Terence Parr
     */
    public class BitSet : IIntSet, ICloneable, ICollection<int>
    {
        const int Bits = 64;    // number of bits / long
        const int LogBits = 6; // 2^6 == 64

        /* We will often need to do a mod operator (i mod nbits).  Its
         * turns out that, for powers of two, this mod operation is
         * same as (i & (nbits-1)).  Since mod is slow, we use a
         * precomputed mod mask to do the mod instead.
         */
        const int ModMask = Bits - 1;

        /** The actual data bits */
        ulong[] _bits;

        /** Construct a bitset of size one word (64 bits) */
        public BitSet()
            : this( Bits )
        {
        }

        /** Construction from a static array of longs */
        [CLSCompliant(false)]
        public BitSet(ulong[] bits)
        {
            _bits = bits;
        }

        /** Construct a bitset given the size
         * @param nbits The size of the bitset in bits
         */
        public BitSet( int nbits )
        {
            _bits = new ulong[( ( nbits - 1 ) >> LogBits ) + 1];
        }

        #region Properties
        public bool IsNil
        {
            get
            {
                for ( int i = _bits.Length - 1; i >= 0; i-- )
                {
                    if ( _bits[i] != 0 )
                        return false;
                }
                return true;
            }
        }
        /** return how much space is being used by the bits array not
         *  how many actually have member bits on.
         */
        public int LengthInLongWords
        {
            get
            {
                return _bits.Length;
            }
        }
        public int NumBits
        {
            get
            {
                // num words * bits per word
                return _bits.Length << LogBits;
            }
        }
        public int Count
        {
            get
            {
                int deg = 0;
                for ( int i = _bits.Length - 1; i >= 0; i-- )
                {
                    ulong word = _bits[i];
                    if ( word != 0L )
                    {
                        for ( int bit = Bits - 1; bit >= 0; bit-- )
                        {
                            if ( ( word & ( 1UL << bit ) ) != 0 )
                            {
                                deg++;
                            }
                        }
                    }
                }
                return deg;
            }
        }
        #endregion

        /** or this element into this set (grow as necessary to accommodate) */
        public virtual void Add( int el )
        {
            //System.Console.Out.WriteLine( "add(" + el + ")" );
            int n = WordNumber( el );
            //System.Console.Out.WriteLine( "word number is " + n );
            //System.Console.Out.WriteLine( "bits.length " + _bits.Length );
            if ( n >= _bits.Length )
            {
                GrowToInclude( el );
            }
            _bits[n] |= BitMask( el );
        }

        public virtual void AddAll( IIntSet set )
        {
            if ( set is BitSet )
            {
                this.OrInPlace( (BitSet)set );
            }
            else if ( set is IntervalSet )
            {
                IntervalSet other = (IntervalSet)set;
                // walk set and add each interval
                foreach ( Interval I in other.intervals )
                {
                    this.OrInPlace( BitSet.Range( I.a, I.b ) );
                }
            }
            else
            {
                throw new ArgumentException( "can't add " +
                                                   set.GetType().Name +
                                                   " to BitSet" );
            }
        }

        public virtual void AddAll( int[] elements )
        {
            if ( elements == null )
            {
                return;
            }
            for ( int i = 0; i < elements.Length; i++ )
            {
                int e = elements[i];
                Add( e );
            }
        }

        public virtual void AddAll( IEnumerable elements )
        {
            if ( elements == null )
            {
                return;
            }
            foreach ( object o in elements )
            {
                if ( !( o is int ) )
                {
                    throw new ArgumentException();
                }
                int eI = (int)o;
                Add( eI );
            }
        }

        public virtual IIntSet And( IIntSet a )
        {
            BitSet s = (BitSet)this.Clone();
            s.AndInPlace( (BitSet)a );
            return s;
        }

        public virtual void AndInPlace( BitSet a )
        {
            int min = Math.Min( _bits.Length, a._bits.Length );
            for ( int i = min - 1; i >= 0; i-- )
            {
                _bits[i] &= a._bits[i];
            }
            // clear all bits in this not present in a (if this bigger than a).
            for ( int i = min; i < _bits.Length; i++ )
            {
                _bits[i] = 0;
            }
        }

        private static ulong BitMask( int bitNumber )
        {
            int bitPosition = bitNumber & ModMask; // bitNumber mod BITS
            return 1UL << bitPosition;
        }

        public virtual void Clear()
        {
            for ( int i = _bits.Length - 1; i >= 0; i-- )
            {
                _bits[i] = 0;
            }
        }

        public virtual void Clear( int el )
        {
            int n = WordNumber( el );
            if ( n >= _bits.Length )
            {	// grow as necessary to accommodate
                GrowToInclude( el );
            }
            _bits[n] &= ~BitMask( el );
        }

        public virtual object Clone()
        {
            return new BitSet( (ulong[])_bits.Clone() );
        }

        public override int GetHashCode()
        {
            throw new System.NotImplementedException();
        }

        public override bool Equals( object other )
        {
            if ( other == null || !( other is BitSet ) )
            {
                return false;
            }

            BitSet otherSet = (BitSet)other;

            int n = Math.Min( this._bits.Length, otherSet._bits.Length );

            // for any bits in common, compare
            for ( int i = 0; i < n; i++ )
            {
                if ( this._bits[i] != otherSet._bits[i] )
                {
                    return false;
                }
            }

            // make sure any extra bits are off

            if ( this._bits.Length > n )
            {
                for ( int i = n + 1; i < this._bits.Length; i++ )
                {
                    if ( this._bits[i] != 0 )
                    {
                        return false;
                    }
                }
            }
            else if ( otherSet._bits.Length > n )
            {
                for ( int i = n + 1; i < otherSet._bits.Length; i++ )
                {
                    if ( otherSet._bits[i] != 0 )
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /**
         * Grows the set to a larger number of bits.
         * @param bit element that must fit in set
         */
        public virtual void GrowToInclude( int bit )
        {
            int newSize = Math.Max( _bits.Length << 1, NumWordsToHold( bit ) );
            ulong[] newbits = new ulong[newSize];
            Array.Copy( _bits, newbits, _bits.Length );
            _bits = newbits;
        }

        public virtual bool Contains( int el )
        {
            int n = WordNumber( el );
            if ( n >= _bits.Length )
                return false;
            return ( _bits[n] & BitMask( el ) ) != 0;
        }

        /** Get the first element you find and return it.  Return Label.INVALID
         *  otherwise.
         */
        public virtual int GetSingleElement()
        {
            for ( int i = 0; i < ( _bits.Length << LogBits ); i++ )
            {
                if ( Contains( i ) )
                {
                    return i;
                }
            }
            return Label.INVALID;
        }

        public virtual IIntSet Complement()
        {
            BitSet s = (BitSet)this.Clone();
            s.NotInPlace();
            return s;
        }

        public virtual IIntSet Complement( IIntSet set )
        {
            if ( set == null )
            {
                return this.Complement();
            }
            return set.Subtract( this );
        }

        public virtual void NotInPlace()
        {
            for ( int i = _bits.Length - 1; i >= 0; i-- )
            {
                _bits[i] = ~_bits[i];
            }
        }

        /** complement bits in the range 0..maxBit. */
        public virtual void NotInPlace( int maxBit )
        {
            NotInPlace( 0, maxBit );
        }

        /** complement bits in the range minBit..maxBit.*/
        public virtual void NotInPlace( int minBit, int maxBit )
        {
            // make sure that we have room for maxBit
            GrowToInclude( maxBit );
            for ( int i = minBit; i <= maxBit; i++ )
            {
                int n = WordNumber( i );
                _bits[n] ^= BitMask( i );
            }
        }

        private int NumWordsToHold( int el )
        {
            return ( el >> LogBits ) + 1;
        }

        public static BitSet Of( int el )
        {
            BitSet s = new BitSet( el + 1 );
            s.Add( el );
            return s;
        }

        public static BitSet Of<T>( T elements )
            where T : IEnumerable<int>
        {
            BitSet s = new BitSet();
            foreach ( int i in elements )
            {
                s.Add( i );
            }
            return s;
        }

        public static BitSet Of( IntervalSet set )
        {
            return Of( (IIntSet)set );
        }
        public static BitSet Of( IIntSet set )
        {
            if ( set == null )
            {
                return null;
            }

            if ( set is BitSet )
            {
                return (BitSet)set;
            }
            if ( set is IntervalSet )
            {
                BitSet s = new BitSet();
                s.AddAll( set );
                return s;
            }
            throw new ArgumentException( "can't create BitSet from " + set.GetType().Name );
        }

        public static BitSet Of( IDictionary elements )
        {
            return BitSet.Of( elements.Keys.Cast<int>() );
        }

        public static BitSet Of<TKey, TValue>( System.Collections.Generic.IDictionary<TKey, TValue> elements )
        {
            return BitSet.Of( elements.Keys.Cast<int>() );
        }

        public static BitSet Range( int a, int b )
        {
            BitSet s = new BitSet( b + 1 );
            for ( int i = a; i <= b; i++ )
            {
                int n = WordNumber( i );
                s._bits[n] |= BitMask( i );
            }
            return s;
        }

        /** return this | a in a new set */
        public virtual IIntSet Or( IIntSet a )
        {
            if ( a == null )
            {
                return this;
            }
            BitSet s = (BitSet)this.Clone();
            s.OrInPlace( (BitSet)a );
            return s;
        }

        public virtual void OrInPlace( BitSet a )
        {
            if ( a == null )
            {
                return;
            }
            // If this is smaller than a, grow this first
            if ( a._bits.Length > _bits.Length )
            {
                SetSize( a._bits.Length );
            }
            int min = Math.Min( _bits.Length, a._bits.Length );
            for ( int i = min - 1; i >= 0; i-- )
            {
                _bits[i] |= a._bits[i];
            }
        }

        // remove this element from this set
        public virtual void Remove( int el )
        {
            int n = WordNumber( el );
            if ( n >= _bits.Length )
            {
                GrowToInclude( el );
            }
            _bits[n] &= ~BitMask( el );
        }

        /**
         * Sets the size of a set.
         * @param nwords how many words the new set should be
         */
        private void SetSize( int nwords )
        {
            ulong[] newbits = new ulong[nwords];
            int n = Math.Min( nwords, _bits.Length );
            Array.Copy( _bits, newbits, n );
            _bits = newbits;
        }

        /**Is this contained within a? */
        public virtual bool Subset( BitSet a )
        {
            if ( a == null )
                return false;
            return this.And( a ).Equals( this );
        }

        /**Subtract the elements of 'a' from 'this' in-place.
         * Basically, just turn off all bits of 'this' that are in 'a'.
         */
        public virtual void SubtractInPlace( BitSet a )
        {
            if ( a == null )
                return;
            // for all words of 'a', turn off corresponding bits of 'this'
            for ( int i = 0; i < _bits.Length && i < a._bits.Length; i++ )
            {
                _bits[i] &= ~a._bits[i];
            }
        }

        public virtual IIntSet Subtract( IIntSet a )
        {
            if ( a == null || !( a is BitSet ) )
                return null;

            BitSet s = (BitSet)this.Clone();
            s.SubtractInPlace( (BitSet)a );
            return s;
        }

        public virtual List<int> ToList()
        {
            return new List<int>( ToArray() );
        }

        public virtual int[] ToArray()
        {
            int[] elems = new int[Count];
            int en = 0;
            for ( int i = 0; i < ( _bits.Length << LogBits ); i++ )
            {
                if ( Contains( i ) )
                {
                    elems[en++] = i;
                }
            }
            return elems;
        }

        [CLSCompliant(false)]
        public virtual ulong[] ToPackedArray()
        {
            return _bits;
        }

        public override string ToString()
        {
            return ToString( null );
        }

        /** Transform a bit set into a string by formatting each element as an integer
         * separator The string to put in between elements
         * @return A commma-separated list of values
         */
        public virtual string ToString( Grammar g )
        {
            StringBuilder buf = new StringBuilder();
            string separator = ",";
            bool havePrintedAnElement = false;
            buf.Append( '{' );

            for ( int i = 0; i < ( _bits.Length << LogBits ); i++ )
            {
                if ( Contains( i ) )
                {
                    if ( i > 0 && havePrintedAnElement )
                    {
                        buf.Append( separator );
                    }
                    if ( g != null )
                    {
                        buf.Append( g.GetTokenDisplayName( i ) );
                    }
                    else
                    {
                        buf.Append( i );
                    }
                    havePrintedAnElement = true;
                }
            }
            buf.Append( '}' );
            return buf.ToString();
        }

        /**Create a string representation where instead of integer elements, the
         * ith element of vocabulary is displayed instead.  Vocabulary is a Vector
         * of Strings.
         * separator The string to put in between elements
         * @return A commma-separated list of character constants.
         */
        public virtual string ToString( string separator, IList vocabulary )
        {
            if ( vocabulary == null )
            {
                return ToString( null );
            }
            string str = "";
            for ( int i = 0; i < ( _bits.Length << LogBits ); i++ )
            {
                if ( Contains( i ) )
                {
                    if ( str.Length > 0 )
                    {
                        str += separator;
                    }
                    if ( i >= vocabulary.Count )
                    {
                        str += "'" + (char)i + "'";
                    }
                    else if ( vocabulary[i] == null )
                    {
                        str += "'" + (char)i + "'";
                    }
                    else
                    {
                        str += (string)vocabulary[i];
                    }
                }
            }
            return str;
        }

        /**
         * Dump a comma-separated list of the words making up the bit set.
         * Split each 64 bit number into two more manageable 32 bit numbers.
         * This generates a comma-separated list of C++-like unsigned long constants.
         */
        public virtual string ToStringOfHalfWords()
        {
            StringBuilder s = new StringBuilder();
            for ( int i = 0; i < _bits.Length; i++ )
            {
                if ( i != 0 )
                    s.Append( ", " );
                ulong tmp = _bits[i];
                tmp &= 0xFFFFFFFFL;
                s.Append( tmp );
                s.Append( "UL" );
                s.Append( ", " );
                tmp = _bits[i] >> 32;
                tmp &= 0xFFFFFFFFL;
                s.Append( tmp );
                s.Append( "UL" );
            }
            return s.ToString();
        }

        /**
         * Dump a comma-separated list of the words making up the bit set.
         * This generates a comma-separated list of Java-like long int constants.
         */
        public virtual string ToStringOfWords()
        {
            StringBuilder s = new StringBuilder();
            for ( int i = 0; i < _bits.Length; i++ )
            {
                if ( i != 0 )
                    s.Append( ", " );
                s.Append( _bits[i] );
                s.Append( "L" );
            }
            return s.ToString();
        }

        public virtual string ToStringWithRanges()
        {
            return ToString();
        }

        private static int WordNumber( int bit )
        {
            return bit >> LogBits; // bit / BITS
        }

        #region ICollection<int> Members

        void ICollection<int>.CopyTo( int[] array, int arrayIndex )
        {
            throw new System.NotImplementedException();
        }

        bool ICollection<int>.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        bool ICollection<int>.Remove( int item )
        {
            bool removed = Contains( item );
            Remove( item );
            return removed;
        }

        #endregion

        #region IEnumerable<int> Members

        public IEnumerator<int> GetEnumerator()
        {
            return ToArray().Cast<int>().GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
