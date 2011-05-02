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

namespace Antlr3.Misc
{
    using System.Collections.Generic;

    using ArgumentException = System.ArgumentException;
    using ArgumentNullException = System.ArgumentNullException;

    /** A HashMap that remembers the order that the elements were added.
     *  You can alter the ith element with this[i]=value too :)  Unique list.
     *  I need the replace/set-element-i functionality so I'm subclassing
     *  OrderedHashSet.
     */
    public class OrderedHashSet<T> : ICollection<T>
    {
        /** Track the elements as they are added to the set */
        private readonly List<T> _elements = new List<T>();

        private readonly HashSet<T> _elementSet = new HashSet<T>();

        public T this[int i]
        {
            get
            {
                return _elements[i];
            }

            /** Replace an existing value with a new value; updates the element
             *  list and the hash table, but not the key as that has not changed.
             */
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                T oldElement = _elements[i];
                if (_elementSet.Contains(value) && !_elementSet.Comparer.Equals(oldElement, value))
                    throw new ArgumentException("The value already exists in this set.", "value");

                _elements[i] = value; // update list
                _elementSet.Remove( oldElement );
                _elementSet.Add( value );
            }
        }

        /** Add a value to list; keep in hashtable for consistency also;
         *  Key is object itself.  Good for say asking if a certain string is in
         *  a list of strings.
         */
        public bool Add( T value )
        {
            if ( _elementSet.Add( value ) )
            {
                _elements.Add( value );
                return true;
            }

            return false;
        }

        public bool Remove( T o )
        {
            if (_elementSet.Remove(o))
            {
                _elements.Remove(o);
                return true;
            }

            return false;
        }

        public void Clear()
        {
            _elements.Clear();
            _elementSet.Clear();
        }

        /** Return the List holding list of table elements.  Note that you are
         *  NOT getting a copy so don't write to the list.
         */
        public IList<T> GetElements()
        {
            return _elements;
        }

        public override string ToString()
        {
            return _elements.ToString();
        }

        #region ICollection<T> Members

        void ICollection<T>.Add( T item )
        {
            this.Add( item );
        }

        public bool Contains( T item )
        {
            return _elementSet.Contains( item );
        }

        public void CopyTo( T[] array, int arrayIndex )
        {
            _elements.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get
            {
                return _elements.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        #endregion

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            return _elements.GetEnumerator();
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
