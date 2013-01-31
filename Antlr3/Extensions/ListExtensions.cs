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

using System;
using System.Collections.Generic;
using System.Linq;

namespace Antlr3.Extensions
{
    public static class ListExtensions
    {
        public static IList<T> CastList<T>( this System.Collections.IList list )
        {
            return new CastListView<T>( list );
        }

        public static IList<T> CastList<T, TSource>( this IList<TSource> list )
            where TSource : T
        {
            return new CastListView<T, TSource>( list );
        }

        public static IList<T> CastListDown<T, TSource>( this IList<TSource> list )
            where T : TSource
        {
            return new DownCastListView<T, TSource>( list );
        }

        class CastListView<T> : IList<T>
        {
            System.Collections.IList _source;

            public CastListView( System.Collections.IList source )
            {
                _source = source;
            }

            #region IList<T> Members

            public int IndexOf( T item )
            {
                return _source.IndexOf( item );
            }

            public void Insert( int index, T item )
            {
                _source.Insert( index, item );
            }

            public void RemoveAt( int index )
            {
                _source.RemoveAt( index );
            }

            public T this[int index]
            {
                get
                {
                    return (T)_source[index];
                }
                set
                {
                    _source[index] = value;
                }
            }

            #endregion

            #region ICollection<T> Members

            public void Add( T item )
            {
                _source.Add( item );
            }

            public void Clear()
            {
                _source.Clear();
            }

            public bool Contains( T item )
            {
                return _source.Contains( item );
            }

            public void CopyTo( T[] array, int arrayIndex )
            {
                _source.CopyTo( array, arrayIndex );
            }

            public int Count
            {
                get
                {
                    return _source.Count;
                }
            }

            public bool IsReadOnly
            {
                get
                {
                    return _source.IsReadOnly;
                }
            }

            public bool Remove( T item )
            {
                int index = _source.IndexOf( item );
                if ( index < 0 )
                    return false;

                _source.RemoveAt( index );
                return true;
            }

            #endregion

            #region IEnumerable<T> Members

            public IEnumerator<T> GetEnumerator()
            {
                return _source.Cast<T>().GetEnumerator();
            }

            #endregion

            #region IEnumerable Members

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return _source.GetEnumerator();
            }

            #endregion
        }

        class CastListView<T, TSource> : IList<T>
            where TSource : T
        {
            IList<TSource> _source;

            public CastListView( IList<TSource> source )
            {
                _source = source;
            }

            #region IList<T> Members

            public int IndexOf( T item )
            {
                return _source.IndexOf( (TSource)item );
            }

            public void Insert( int index, T item )
            {
                _source.Insert( index, (TSource)item );
            }

            public void RemoveAt( int index )
            {
                _source.RemoveAt( index );
            }

            public T this[int index]
            {
                get
                {
                    return (T)_source[index];
                }
                set
                {
                    _source[index] = (TSource)value;
                }
            }

            #endregion

            #region ICollection<T> Members

            public void Add( T item )
            {
                _source.Add( (TSource)item );
            }

            public void Clear()
            {
                _source.Clear();
            }

            public bool Contains( T item )
            {
                return _source.Contains( (TSource)item );
            }

            public void CopyTo( T[] array, int arrayIndex )
            {
                System.Collections.IList source = _source as System.Collections.IList;
                if ( source != null )
                {
                    source.CopyTo( array, arrayIndex );
                }
                else
                {
                    if ( array == null )
                        throw new ArgumentNullException( "array" );

                    if ( arrayIndex < 0 )
                        throw new ArgumentOutOfRangeException( "arrayIndex" );

                    if ( _source.Count + arrayIndex >= array.Length )
                        throw new ArgumentException();

                    for ( int i = 0; i < _source.Count; i++ )
                        array[arrayIndex + i] = (T)_source[i];
                }
            }

            public int Count
            {
                get
                {
                    return _source.Count;
                }
            }

            public bool IsReadOnly
            {
                get
                {
                    return _source.IsReadOnly;
                }
            }

            public bool Remove( T item )
            {
                return _source.Remove( (TSource)item );
            }

            #endregion

            #region IEnumerable<T> Members

            public IEnumerator<T> GetEnumerator()
            {
                return _source.Cast<T>().GetEnumerator();
            }

            #endregion

            #region IEnumerable Members

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return _source.GetEnumerator();
            }

            #endregion
        }

        class DownCastListView<T, TSource> : IList<T>
            where T : TSource
        {
            IList<TSource> _source;

            public DownCastListView( IList<TSource> source )
            {
                if (source == null)
                    throw new ArgumentNullException("source");

                _source = source;
            }

            #region IList<T> Members

            public int IndexOf( T item )
            {
                return _source.IndexOf( (TSource)item );
            }

            public void Insert( int index, T item )
            {
                _source.Insert( index, (TSource)item );
            }

            public void RemoveAt( int index )
            {
                _source.RemoveAt( index );
            }

            public T this[int index]
            {
                get
                {
                    return (T)_source[index];
                }
                set
                {
                    _source[index] = (TSource)value;
                }
            }

            #endregion

            #region ICollection<T> Members

            public void Add( T item )
            {
                _source.Add( (TSource)item );
            }

            public void Clear()
            {
                _source.Clear();
            }

            public bool Contains( T item )
            {
                return _source.Contains( (TSource)item );
            }

            public void CopyTo( T[] array, int arrayIndex )
            {
                System.Collections.IList source = _source as System.Collections.IList;
                if ( source != null )
                {
                    source.CopyTo( array, arrayIndex );
                }
                else
                {
                    if ( array == null )
                        throw new ArgumentNullException( "array" );

                    if ( arrayIndex < 0 )
                        throw new ArgumentOutOfRangeException( "arrayIndex" );

                    if ( _source.Count + arrayIndex >= array.Length )
                        throw new ArgumentException();

                    for ( int i = 0; i < _source.Count; i++ )
                        array[arrayIndex + i] = (T)_source[i];
                }
            }

            public int Count
            {
                get
                {
                    return _source.Count;
                }
            }

            public bool IsReadOnly
            {
                get
                {
                    return _source.IsReadOnly;
                }
            }

            public bool Remove( T item )
            {
                return _source.Remove( (TSource)item );
            }

            #endregion

            #region IEnumerable<T> Members

            public IEnumerator<T> GetEnumerator()
            {
                return _source.Cast<T>().GetEnumerator();
            }

            #endregion

            #region IEnumerable Members

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return _source.GetEnumerator();
            }

            #endregion
        }
    }
}
