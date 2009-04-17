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

    using Grammar = Antlr3.Tool.Grammar;

    /** A generic set of ints that has an efficient implementation, BitSet,
     *  which is a compressed bitset and is useful for ints that
     *  are small, for example less than 500 or so, and w/o many ranges.  For
     *  ranges with large values like unicode char sets, this is not very efficient.
     *  Consider using IntervalSet.  Not all methods in IntervalSet are implemented.
     *
     *  @see org.antlr.misc.BitSet
     *  @see org.antlr.misc.IntervalSet
     */
    public interface IIntSet : ICollection<int>
    {
        /** Add all elements from incoming set to this set.  Can limit
         *  to set of its own type.
         */
        void AddAll( IIntSet set );

        /** Return the intersection of this set with the argument, creating
         *  a new set.
         */
        IIntSet And( IIntSet a );

        IIntSet Complement( IIntSet elements );

        IIntSet Or( IIntSet a );

        IIntSet Subtract( IIntSet a );

        bool IsNil
        {
            get;
        }

        bool Equals( object obj );

        int GetSingleElement();

        List<int> ToList();

        string ToString();

        string ToString( Grammar g );
    }
}
