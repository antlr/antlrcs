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

namespace Antlr3.Misc
{
    using System.Collections.Generic;
    using System.Linq;

    using ArgumentException = System.ArgumentException;
    using Grammar = Antlr3.Tool.Grammar;
    using Label = Antlr3.Analysis.Label;
    using Math = System.Math;
    using NotImplementedException = System.NotImplementedException;
    using StringBuilder = System.Text.StringBuilder;

    /** A set of integers that relies on ranges being common to do
     *  "run-length-encoded" like compression (if you view an IntSet like
     *  a BitSet with runs of 0s and 1s).  Only ranges are recorded so that
     *  a few ints up near value 1000 don't cause massive bitsets, just two
     *  integer intervals.
     *
     *  element values may be negative.  Useful for sets of EPSILON and EOF.
     *
     *  0..9 char range is index pair ['\u0030','\u0039'].
     *  Multiple ranges are encoded with multiple index pairs.  Isolated
     *  elements are encoded with an index pair where both intervals are the same.
     *
     *  The ranges are ordered and disjoint so that 2..6 appears before 101..103.
     */
    public class IntervalSet : IIntSet
    {
        public static readonly Interval CompleteInterval = Interval.FromBounds(0, Label.MAX_CHAR_VALUE);
        public static readonly IntervalSet COMPLETE_SET = IntervalSet.Of(CompleteInterval);

        /** The list of sorted, disjoint intervals. */
        private IList<Interval> intervals;

        /** Create a set with no elements */
        public IntervalSet()
        {
            //intervals = new ArrayList<Interval>( 2 ); // most sets are 1 or 2 elements
            intervals = new List<Interval>( 2 ); // most sets are 1 or 2 elements
        }

        public IntervalSet( IList<Interval> intervals )
        {
            this.intervals = intervals;
        }

        #region Properties
        public int Count
        {
            get
            {
                return intervals.Sum( interval => interval.b - interval.a + 1 );
            }
        }

        public ICollection<Interval> Intervals
        {
            get
            {
                return intervals;
            }
        }

        public int MaxElement
        {
            get
            {
                return GetMaxElement();
            }
        }
        public int MinElement
        {
            get
            {
                return GetMinElement();
            }
        }
        public int SingleElement
        {
            get
            {
                return GetSingleElement();
            }
        }
        #endregion

        /** Create a set with a single element, el. */
        public static IntervalSet Of( int a )
        {
            return Of(Interval.FromBounds(a, a));
        }

        /** Create a set with all ints within range [a..b] (inclusive) */
        public static IntervalSet Of( int a, int b )
        {
            return Of(Interval.FromBounds(a, b));
        }

        public static IntervalSet Of(Interval interval)
        {
            IntervalSet s = new IntervalSet(new List<Interval> { interval });
            return s;
        }

        /** Add a single element to the set.  An isolated element is stored
         *  as a range el..el.
         */
        public virtual void Add( int el )
        {
            Add( el, el );
        }

        /** Add interval; i.e., add all integers from a to b to set.
         *  If b<a, do nothing.
         *  Keep list in sorted order (by left range value).
         *  If overlap, combine ranges.  For example,
         *  If this is {1..5, 10..20}, adding 6..7 yields
         *  {1..5, 6..7, 10..20}.  Adding 4..8 yields {1..8, 10..20}.
         */
        public virtual void Add( int a, int b )
        {
            Add( Interval.FromBounds( a, b ) );
        }

        // copy on write so we can cache a..a intervals and sets of that
        protected virtual void Add( Interval addition )
        {
            //JSystem.@out.println("add "+addition+" to "+intervals.toString());
            if ( addition.b < addition.a )
            {
                return;
            }
            // find position in list
            // Use iterators as we modify list in place
            //for ( ListIterator iter = intervals.listIterator(); iter.hasNext(); )
            for ( int i = 0; i < intervals.Count; i++ )
            {
                //Interval r = (Interval)iter.next();
                Interval r = intervals[i];
                if ( addition.Equals( r ) )
                {
                    return;
                }
                if ( addition.Adjacent( r ) || !addition.Disjoint( r ) )
                {
                    // next to each other, make a single larger interval
                    Interval bigger = addition.Union( r );
                    intervals[i] = bigger;
                    // make sure we didn't just create an interval that
                    // should be merged with next interval in list
                    if ( i < intervals.Count - 1 )
                    {
                        i++;
                        Interval next = intervals[i];
                        //Interval next = (Interval)iter.next();
                        if ( bigger.Adjacent( next ) || !bigger.Disjoint( next ) )
                        {
                            // if we bump up against or overlap next, merge
                            //iter.remove();   // remove this one
                            intervals.RemoveAt( i );
                            //iter.previous(); // move backwards to what we just set
                            i--;
                            //iter.set( bigger.union( next ) ); // set to 3 merged ones
                            intervals[i] = bigger.Union( next );
                        }
                    }
                    return;
                }
                if ( addition.StartsBeforeDisjoint( r ) )
                {
                    // insert before r
                    //iter.previous();
                    //iter.add( addition );
                    intervals.Insert( i, addition );
                    return;
                }
                // if disjoint and after r, a future iteration will handle it
            }
            // ok, must be after last interval (and disjoint from last interval)
            // just add it
            intervals.Add( addition );
        }

#if false
        protected virtual void Add( Interval addition )
        {
            //JSystem.@out.println("add "+addition+" to "+intervals.toString());
            if ( addition.b < addition.a )
            {
                return;
            }
            // find position in list
            //for (ListIterator iter = intervals.listIterator(); iter.hasNext();) {
            int n = intervals.Count;
            for ( int i = 0; i < n; i++ )
            {
                Interval r = (Interval)intervals[i];
                if ( addition.Equals( r ) )
                {
                    return;
                }
                if ( addition.adjacent( r ) || !addition.disjoint( r ) )
                {
                    // next to each other, make a single larger interval
                    Interval bigger = addition.union( r );
                    intervals[i] = bigger;
                    // make sure we didn't just create an interval that
                    // should be merged with next interval in list
                    if ( ( i + 1 ) < n )
                    {
                        i++;
                        Interval next = (Interval)intervals[i];
                        if ( bigger.adjacent( next ) || !bigger.disjoint( next ) )
                        {
                            // if we bump up against or overlap next, merge
                            intervals.RemoveAt( i ); // remove next one
                            i--;
                            intervals[i] = bigger.union( next ); // set to 3 merged ones
                        }
                    }
                    return;
                }
                if ( addition.startsBeforeDisjoint( r ) )
                {
                    // insert before r
                    intervals.Insert( i, addition );
                    return;
                }
                // if disjoint and after r, a future iteration will handle it
            }
            // ok, must be after last interval (and disjoint from last interval)
            // just add it
            intervals.Add( addition );
        }
#endif

        public virtual void AddAll( IIntSet set )
        {
            if ( set == null )
            {
                return;
            }
            if ( !( set is IntervalSet ) )
            {
                throw new ArgumentException( "can't add non IntSet (" +
                                                   set.GetType().Name +
                                                   ") to IntervalSet" );
            }
            IntervalSet other = (IntervalSet)set;
            // walk set and add each interval
            int n = other.intervals.Count;
            for ( int i = 0; i < n; i++ )
            {
                Interval I = (Interval)other.intervals[i];
                this.Add( I.a, I.b );
            }
        }

        public virtual IIntSet Complement( int minElement, int maxElement )
        {
            return this.Complement( Interval.FromBounds( minElement, maxElement ) );
        }

        /** Given the set of possible values (rather than, say UNICODE or MAXINT),
         *  return a new set containing all elements in vocabulary, but not in
         *  this.  The computation is (vocabulary - this).
         *
         *  'this' is assumed to be either a subset or equal to vocabulary.
         */
        public virtual IIntSet Complement( Interval vocabulary )
        {
            if (vocabulary.b < MinElement || vocabulary.a > MaxElement)
            {
                // nothing in common with this set
                return null;
            }

            int n = intervals.Count;
            if ( n == 0 )
            {
                return IntervalSet.Of(vocabulary);
            }

            IntervalSet compl = new IntervalSet();

            Interval first = intervals[0];
            // add a range from 0 to first.a constrained to vocab
            if ( first.a > vocabulary.a )
            {
                compl.Intervals.Add(Interval.FromBounds(vocabulary.a, first.a - 1));
            }

            for ( int i = 1; i < n; i++ )
            {
                if (intervals[i - 1].b >= vocabulary.b)
                    break;

                if (intervals[i].a <= vocabulary.a)
                    continue;

                if (intervals[i - 1].b == intervals[i].a - 1)
                    continue;

                compl.Intervals.Add(Interval.FromBounds(Math.Max(vocabulary.a, intervals[i - 1].b + 1), Math.Min(vocabulary.b, intervals[i].a - 1)));

                //// from 2nd interval .. nth
                //Interval previous = intervals[i - 1];
                //Interval current = intervals[i];
                //IntervalSet s = IntervalSet.Of( previous.b + 1, current.a - 1 );
                //IntervalSet a = (IntervalSet)s.And( vocabularyIS );
                //compl.AddAll( a );
            }

            Interval last = intervals[n - 1];
            // add a range from last.b to maxElement constrained to vocab
            if ( last.b < vocabulary.b )
            {
                compl.Intervals.Add(Interval.FromBounds(last.b + 1, vocabulary.b));
                //IntervalSet s = IntervalSet.Of( last.b + 1, maxElement );
                //IntervalSet a = (IntervalSet)s.And( vocabularyIS );
                //compl.AddAll( a );
            }

            return compl;
        }

        /** Compute this-other via this&~other.
         *  Return a new set containing all elements in this but not in other.
         *  other is assumed to be a subset of this;
         *  anything that is in other but not in this will be ignored.
         */
        public virtual IIntSet Subtract( IIntSet other )
        {
            // assume the whole unicode range here for the complement
            // because it doesn't matter.  Anything beyond the max of this' set
            // will be ignored since we are doing this & ~other.  The intersection
            // will be empty.  The only problem would be when this' set max value
            // goes beyond MAX_CHAR_VALUE, but hopefully the constant MAX_CHAR_VALUE
            // will prevent this.
            return this.And( ( (IntervalSet)other ).Complement( CompleteInterval ) );
        }

#if false
        /** return a new set containing all elements in this but not in other.
         *  Intervals may have to be broken up when ranges in this overlap
         *  with ranges in other.  other is assumed to be a subset of this;
         *  anything that is in other but not in this will be ignored.
         *
         *  Keep around, but 10-20-2005, I decided to make complement work w/o
         *  subtract and so then subtract can simply be a&~b
         */
        public IIntSet Subtract( IIntSet other )
        {
            if ( other == null || !( other is IntervalSet ) )
            {
                return null; // nothing in common with null set
            }

            IntervalSet diff = new IntervalSet();

            // iterate down both interval lists
            var thisIter = this.intervals.GetEnumerator();
            var otherIter = ( (IntervalSet)other ).intervals.GetEnumerator();
            Interval mine = null;
            Interval theirs = null;
            if ( thisIter.MoveNext() )
            {
                mine = (Interval)thisIter.Current;
            }
            if ( otherIter.MoveNext() )
            {
                theirs = (Interval)otherIter.Current;
            }
            while ( mine != null )
            {
                //JSystem.@out.println("mine="+mine+", theirs="+theirs);
                // CASE 1: nothing in theirs removes a chunk from mine
                if ( theirs == null || mine.disjoint( theirs ) )
                {
                    // SUBCASE 1a: finished traversing theirs; keep adding mine now
                    if ( theirs == null )
                    {
                        // add everything in mine to difference since theirs done
                        diff.add( mine );
                        mine = null;
                        if ( thisIter.MoveNext() )
                        {
                            mine = (Interval)thisIter.Current;
                        }
                    }
                    else
                    {
                        // SUBCASE 1b: mine is completely to the left of theirs
                        // so we can add to difference; move mine, but not theirs
                        if ( mine.startsBeforeDisjoint( theirs ) )
                        {
                            diff.add( mine );
                            mine = null;
                            if ( thisIter.MoveNext() )
                            {
                                mine = (Interval)thisIter.Current;
                            }
                        }
                        // SUBCASE 1c: theirs is completely to the left of mine
                        else
                        {
                            // keep looking in theirs
                            theirs = null;
                            if ( otherIter.MoveNext() )
                            {
                                theirs = (Interval)otherIter.Current;
                            }
                        }
                    }
                }
                else
                {
                    // CASE 2: theirs breaks mine into two chunks
                    if ( mine.properlyContains( theirs ) )
                    {
                        // must add two intervals: stuff to left and stuff to right
                        diff.add( mine.a, theirs.a - 1 );
                        // don't actually add stuff to right yet as next 'theirs'
                        // might overlap with it
                        // The stuff to the right might overlap with next "theirs".
                        // so it is considered next
                        Interval right = new Interval( theirs.b + 1, mine.b );
                        mine = right;
                        // move theirs forward
                        theirs = null;
                        if ( otherIter.MoveNext() )
                        {
                            theirs = (Interval)otherIter.Current;
                        }
                    }

                    // CASE 3: theirs covers mine; nothing to add to diff
                    else if ( theirs.properlyContains( mine ) )
                    {
                        // nothing to add, theirs forces removal totally of mine
                        // just move mine looking for an overlapping interval
                        mine = null;
                        if ( thisIter.MoveNext() )
                        {
                            mine = (Interval)thisIter.Current;
                        }
                    }

                    // CASE 4: non proper overlap
                    else
                    {
                        // overlap, but not properly contained
                        diff.add( mine.differenceNotProperlyContained( theirs ) );
                        // update iterators
                        bool moveTheirs = true;
                        if ( mine.startsBeforeNonDisjoint( theirs ) ||
                             theirs.b > mine.b )
                        {
                            // uh oh, right of theirs extends past right of mine
                            // therefore could overlap with next of mine so don't
                            // move theirs iterator yet
                            moveTheirs = false;
                        }
                        // always move mine
                        mine = null;
                        if ( thisIter.MoveNext() )
                        {
                            mine = (Interval)thisIter.Current;
                        }
                        if ( moveTheirs )
                        {
                            theirs = null;
                            if ( otherIter.MoveNext() )
                            {
                                theirs = (Interval)otherIter.Current;
                            }
                        }
                    }
                }
            }
            return diff;
        }
#endif

        /** TODO: implement this! */
        public IIntSet Or( IIntSet a )
        {
            IntervalSet o = new IntervalSet();
            o.AddAll( this );
            o.AddAll( a );
            //throw new NoSuchMethodError();
            return o;
        }

        /** Return a new set with the intersection of this set with other.  Because
         *  the intervals are sorted, we can use an iterator for each list and
         *  just walk them together.  This is roughly O(min(n,m)) for interval
         *  list lengths n and m.
         */
        public IIntSet And( IIntSet other )
        {
            if ( other == null )
            { //|| !(other instanceof IntervalSet) ) {
                return null; // nothing in common with null set
            }

            var myIntervals = this.intervals;
            var theirIntervals = ( (IntervalSet)other ).intervals;
            IntervalSet intersection = new IntervalSet();
            int mySize = myIntervals.Count;
            int theirSize = theirIntervals.Count;
            int i = 0;
            int j = 0;
            // iterate down both interval lists looking for nondisjoint intervals
            while ( i < mySize && j < theirSize )
            {
                Interval mine = myIntervals[i];
                Interval theirs = theirIntervals[j];
                //JSystem.@out.println("mine="+mine+" and theirs="+theirs);
                if ( mine.StartsBeforeDisjoint( theirs ) )
                {
                    // move this iterator looking for interval that might overlap
                    i++;
                }
                else if ( theirs.StartsBeforeDisjoint( mine ) )
                {
                    // move other iterator looking for interval that might overlap
                    j++;
                }
                else if ( mine.ProperlyContains( theirs ) )
                {
                    // overlap, add intersection, get next theirs
                    intersection.Intervals.Add( theirs );
                    j++;
                }
                else if ( theirs.ProperlyContains( mine ) )
                {
                    // overlap, add intersection, get next mine
                    intersection.Intervals.Add( mine );
                    i++;
                }
                else if ( !mine.Disjoint( theirs ) )
                {
                    // overlap, add intersection
                    intersection.Add( mine.Intersection( theirs ) );
                    // Move the iterator of lower range [a..b], but not
                    // the upper range as it may contain elements that will collide
                    // with the next iterator. So, if mine=[0..115] and
                    // theirs=[115..200], then intersection is 115 and move mine
                    // but not theirs as theirs may collide with the next range
                    // in thisIter.
                    // move both iterators to next ranges
                    if ( mine.StartsAfterNonDisjoint( theirs ) )
                    {
                        j++;
                    }
                    else if ( theirs.StartsAfterNonDisjoint( mine ) )
                    {
                        i++;
                    }
                }
            }

            return intersection;
        }

        /** Is el in any range of this set? */
        public virtual bool Contains( int el )
        {
            int n = intervals.Count;
            for ( int i = 0; i < n; i++ )
            {
                Interval I = (Interval)intervals[i];
                int a = I.a;
                int b = I.b;
                if ( el < a )
                {
                    break; // list is sorted and el is before this interval; not here
                }
                if ( el >= a && el <= b )
                {
                    return true; // found in this interval
                }
            }
            return false;
            /*
                    for (ListIterator iter = intervals.listIterator(); iter.hasNext();) {
                        Interval I = (Interval) iter.next();
                        if ( el<I.a ) {
                            break; // list is sorted and el is before this interval; not here
                        }
                        if ( el>=I.a && el<=I.b ) {
                            return true; // found in this interval
                        }
                    }
                    return false;
                    */
        }

        /** return true if this set has no members */
        public virtual bool IsNil
        {
            get
            {
                return intervals == null || intervals.Count == 0;
            }
        }

        /** If this set is a single integer, return it otherwise Label.INVALID */
        public virtual int GetSingleElement()
        {
            if ( intervals != null && intervals.Count == 1 )
            {
                Interval I = (Interval)intervals[0];
                if ( I.a == I.b )
                {
                    return I.a;
                }
            }
            return Label.INVALID;
        }

        public virtual int GetMaxElement()
        {
            if ( IsNil )
            {
                return Label.INVALID;
            }
            Interval last = (Interval)intervals[intervals.Count - 1];
            return last.b;
        }

        /** Return minimum element >= 0 */
        public virtual int GetMinElement()
        {
            if ( IsNil )
            {
                return Label.INVALID;
            }
            int n = intervals.Count;
            for ( int i = 0; i < n; i++ )
            {
                Interval I = (Interval)intervals[i];
                int a = I.a;
                int b = I.b;
                for ( int v = a; v <= b; v++ )
                {
                    if ( v >= 0 )
                        return v;
                }
            }
            return Label.INVALID;
        }

        /** Are two IntervalSets equal?  Because all intervals are sorted
         *  and disjoint, equals is a simple linear walk over both lists
         *  to make sure they are the same.  Interval.equals() is used
         *  by the List.equals() method to check the ranges.
         */
        public override bool Equals( object obj )
        {
            IntervalSet other = obj as IntervalSet;
            if (other == null)
                return false;

            return intervals.SequenceEqual( other.intervals );
        }

        public override int GetHashCode()
        {
            return intervals.GetHashCode();
        }

        public override string ToString()
        {
            return ToString( null );
        }

        public virtual string ToString( Grammar g )
        {
            if ( this.intervals == null || this.intervals.Count == 0 )
            {
                return "{}";
            }

            StringBuilder buf = new StringBuilder();
            if ( this.intervals.Count > 1 )
            {
                buf.Append( "{" );
            }
            foreach ( Interval I in intervals )
            {
                // element separation
                if ( buf.Length > 1 )
                    buf.Append( ", " );

                int a = I.a;
                int b = I.b;
                if ( a == b )
                {
                    if ( g != null )
                    {
                        buf.Append( g.GetTokenDisplayName( a ) );
                    }
                    else
                    {
                        buf.Append( a );
                    }
                }
                else
                {
                    if ( g != null )
                    {
                        buf.Append( g.GetTokenDisplayName( a ) + ".." + g.GetTokenDisplayName( b ) );
                    }
                    else
                    {
                        buf.Append( a + ".." + b );
                    }
                }
            }
            if ( this.intervals.Count > 1 )
            {
                buf.Append( "}" );
            }
            return buf.ToString();
        }

        public List<int> ToList()
        {
            int count = ( (IIntSet)this ).Count;
            List<int> list = new List<int>( count );

            foreach ( Interval interval in intervals )
            {
                for ( int i = interval.a; i <= interval.b; i++ )
                    list.Add( i );
            }

            return list;
        }

        /** Get the ith element of ordered set.  Used only by RandomPhrase so
         *  don't bother to implement if you're not doing that for a new
         *  ANTLR code gen target.
         */
        public virtual int Get( int i )
        {
            int n = intervals.Count;
            int index = 0;
            for ( int j = 0; j < n; j++ )
            {
                Interval I = (Interval)intervals[j];
                int a = I.a;
                int b = I.b;
                for ( int v = a; v <= b; v++ )
                {
                    if ( index == i )
                    {
                        return v;
                    }
                    index++;
                }
            }
            return -1;
        }

        public int[] ToArray()
        {
            int[] values = new int[Count];
            int n = intervals.Count;
            int j = 0;
            for ( int i = 0; i < n; i++ )
            {
                Interval I = (Interval)intervals[i];
                int a = I.a;
                int b = I.b;
                for ( int v = a; v <= b; v++ )
                {
                    values[j] = v;
                    j++;
                }
            }
            return values;
        }

        public Antlr.Runtime.BitSet ToRuntimeBitSet()
        {
            Antlr.Runtime.BitSet s =
                new Antlr.Runtime.BitSet( GetMaxElement() + 1 );
            int n = intervals.Count;
            for ( int i = 0; i < n; i++ )
            {
                Interval I = (Interval)intervals[i];
                int a = I.a;
                int b = I.b;
                for ( int v = a; v <= b; v++ )
                {
                    s.Add( v );
                }
            }
            return s;
        }

        public virtual void Remove( int el )
        {
            throw new NotImplementedException();
        }

        /*
        protected void finalize()
        {
            super.finalize();
            JSystem.@out.println("size "+intervals.size()+" "+size());
        }
        */

        #region ICollection<int> Members

        void ICollection<int>.Add( int item )
        {
            Add( item );
        }

        void ICollection<int>.Clear()
        {
            intervals.Clear();
        }

        bool ICollection<int>.Contains( int item )
        {
            return Contains( item );
        }

        void ICollection<int>.CopyTo( int[] array, int arrayIndex )
        {
            throw new NotImplementedException();
        }

        bool ICollection<int>.IsReadOnly
        {
            get
            {
                return intervals.IsReadOnly;
            }
        }

        bool ICollection<int>.Remove( int item )
        {
            Remove( item );
            return true;
        }

        #endregion

        #region IEnumerable<int> Members

        public IEnumerator<int> GetEnumerator()
        {
            return intervals.SelectMany( interval => Enumerable.Range( interval.a, interval.b - interval.a + 1 ) ).GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
