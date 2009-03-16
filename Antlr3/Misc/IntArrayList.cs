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
    using Antlr.Runtime.JavaExtensions;

    using Array = System.Array;
    using ICloneable = System.ICloneable;
    using StringBuilder = System.Text.StringBuilder;

    /** An ArrayList based upon int members.  Not quite a real implementation of a
     *  modifiable list as I don't do, for example, add(index,element).
     *  TODO: unused?
     */
    public class IntArrayList : List<object>, ICloneable
    {
        private const int DEFAULT_CAPACITY = 10;
        protected int n = 0;
        protected int[] elements = null;

        public IntArrayList()
            : this( DEFAULT_CAPACITY )
        {
        }

        public IntArrayList( int initialCapacity )
        {
            elements = new int[initialCapacity];
        }

        IntArrayList( int[] elements, int n )
        {
            this.elements = elements;
            this.n = n;
        }

        /** Set the ith element.  Like ArrayList, this does NOT affect size. */
        public virtual int set( int i, int newValue )
        {
            if ( i >= n )
            {
                setSize( i ); // unlike definition of set in ArrayList, set size
            }
            int v = elements[i];
            elements[i] = newValue;
            return v;
        }

        public virtual bool add( int o )
        {
            if ( n >= elements.Length )
            {
                grow();
            }
            elements[n] = o;
            n++;
            return true;
        }

        public virtual void setSize( int newSize )
        {
            if ( newSize >= elements.Length )
            {
                ensureCapacity( newSize );
            }
            n = newSize;
        }

        protected virtual void grow()
        {
            ensureCapacity( ( elements.Length * 3 ) / 2 + 1 );
        }

        public virtual bool contains( int v )
        {
            for ( int i = 0; i < n; i++ )
            {
                int element = elements[i];
                if ( element == v )
                {
                    return true;
                }
            }
            return false;
        }

        public virtual void ensureCapacity( int newCapacity )
        {
            int oldCapacity = elements.Length;
            if ( n >= oldCapacity )
            {
                int[] oldData = elements;
                elements = new int[newCapacity];
                Array.Copy( oldData, elements, n );
            }
        }

        public virtual object get( int i )
        {
            return element( i );
        }

        public virtual int element( int i )
        {
            return elements[i];
        }

        public virtual int[] getElements()
        {
            int[] a = new int[n];
            Array.Copy( elements, a, n );
            return a;
        }

        public virtual int size()
        {
            return n;
        }

        public virtual int capacity()
        {
            return elements.Length;
        }

        public override bool Equals( object o )
        {
            if ( o == null )
            {
                return false;
            }
            IntArrayList other = (IntArrayList)o;
            if ( this.size() != other.size() )
            {
                return false;
            }
            for ( int i = 0; i < n; i++ )
            {
                if ( elements[i] != other.elements[i] )
                {
                    return false;
                }
            }
            return true;
        }

        public override int GetHashCode()
        {
            return ObjectExtensions.ShiftPrimeXOR( elements.GetHashCode(), n );
        }

        public virtual object Clone()
        {
            return new IntArrayList( (int[])elements.Clone(), n );
        }

        public override string ToString()
        {
            StringBuilder buf = new StringBuilder();
            for ( int i = 0; i < n; i++ )
            {
                if ( i > 0 )
                {
                    buf.Append( ", " );
                }
                buf.Append( elements[i] );
            }
            return buf.ToString();
        }
    }
}
