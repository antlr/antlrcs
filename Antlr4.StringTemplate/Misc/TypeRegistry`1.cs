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

namespace Antlr4.StringTemplate.Misc
{
    using System.Collections.Generic;
    using System.Linq;
    using Antlr4.StringTemplate.Extensions;

    using AmbiguousMatchException = System.Reflection.AmbiguousMatchException;
    using ArgumentNullException = System.ArgumentNullException;
    using Array = System.Array;
    using ICollection = System.Collections.ICollection;
    using IDictionary = System.Collections.IDictionary;
    using IDictionaryEnumerator = System.Collections.IDictionaryEnumerator;
    using IEnumerable = System.Collections.IEnumerable;
    using IEnumerator = System.Collections.IEnumerator;
    using NotSupportedException = System.NotSupportedException;
    using StringBuilder = System.Text.StringBuilder;
    using Type = System.Type;

    public class TypeRegistry<T> : IDictionary<Type, T>, IDictionary
    {
        private readonly Dictionary<Type, T> _backingStore = new Dictionary<Type, T>();
        private readonly Dictionary<Type, Type> _cache = new Dictionary<Type, Type>();

        public TypeRegistry()
        {
        }

        public TypeRegistry(IEnumerable<KeyValuePair<Type, T>> collection)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            foreach (var type in collection)
                this.Add(type.Key, type.Value);
        }

        ICollection<Type> IDictionary<Type, T>.Keys
        {
            get
            {
                return _backingStore.Keys;
            }
        }

        ICollection<T> IDictionary<Type, T>.Values
        {
            get
            {
                return _backingStore.Values;
            }
        }

        int ICollection<KeyValuePair<Type, T>>.Count
        {
            get
            {
                return _backingStore.Count;
            }
        }

        int ICollection.Count
        {
            get
            {
                return _backingStore.Count;
            }
        }

        bool ICollection<KeyValuePair<Type, T>>.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        bool IDictionary.IsFixedSize
        {
            get
            {
                return false;
            }
        }

        bool IDictionary.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        ICollection IDictionary.Keys
        {
            get
            {
                return _backingStore.Keys;
            }
        }

        ICollection IDictionary.Values
        {
            get
            {
                return _backingStore.Values;
            }
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                return false;
            }
        }

        object ICollection.SyncRoot
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        public T this[Type key]
        {
            get
            {
                T value;
                if (!TryGetValue(key, out value))
                    throw new KeyNotFoundException();

                return value;
            }

            set
            {
                _backingStore[key] = value;
                HandleAlteration(key);
            }
        }

        object IDictionary.this[object key]
        {
            get
            {
                Type t = key as Type;
                if (t == null && key != null)
                    return null;

                T value;
                if (!TryGetValue(t, out value))
                    return null;

                return value;
            }

            set
            {
                this[(Type)key] = (T)value;
            }
        }

        public void Add(Type key, T value)
        {
            _backingStore.Add(key, value);
            HandleAlteration(key);
        }

        public bool ContainsKey(Type key)
        {
            if (_cache.ContainsKey(key))
                return true;

            T value;
            return TryGetValue(key, out value);
        }

        public bool Remove(Type key)
        {
            if (_backingStore.Remove(key))
            {
                HandleAlteration(key);
                return true;
            }

            return false;
        }

        public bool TryGetValue(Type key, out T value)
        {
            if (_backingStore.TryGetValue(key, out value))
                return true;

            Type redirect;
            if (_cache.TryGetValue(key, out redirect))
            {
                if (redirect == null)
                {
                    value = default(T);
                    return false;
                }
                else
                {
                    return _backingStore.TryGetValue(redirect, out value);
                }
            }

            List<Type> candidates = _backingStore.Keys.Where(i => i.IsAssignableFrom(key)).ToList();
            if (candidates.Count == 0)
            {
                _cache[key] = null;
                value = default(T);
                return false;
            }
            else if (candidates.Count == 1)
            {
                _cache[key] = candidates[0];
                return _backingStore.TryGetValue(candidates[0], out value);
            }
            else
            {
                for (int i = 0; i < candidates.Count - 1; i++)
                {
                    if (candidates[i] == null)
                        continue;

                    for (int j = i + 1; j < candidates.Count; j++)
                    {
                        if (candidates[i].IsAssignableFrom(candidates[j]))
                        {
                            candidates[i] = null;
                            break;
                        }
                        else if (candidates[j].IsAssignableFrom(candidates[i]))
                        {
                            candidates[j] = null;
                        }
                    }
                }

                candidates.RemoveAll(i => i == null);
                if (candidates.Count != 1)
                {
                    StringBuilder builder = new StringBuilder();
                    builder.AppendFormat("The type '{0}' does not match a single item in the registry. The {1} ambiguous matches are:", key.FullName, candidates.Count);
                    foreach (var candidate in candidates)
                    {
                        builder.AppendLine();
                        builder.AppendFormat("    {0}", candidate.FullName);
                    }

                    throw new AmbiguousMatchException(builder.ToString());
                }

                _cache[key] = candidates[0];
                return _backingStore.TryGetValue(candidates[0], out value);
            }
        }

        public void Clear()
        {
            _backingStore.Clear();
            _cache.Clear();
        }

        public IEnumerator<KeyValuePair<Type, T>> GetEnumerator()
        {
            return _backingStore.GetEnumerator();
        }

        void ICollection<KeyValuePair<Type, T>>.Add(KeyValuePair<Type, T> item)
        {
            Add(item.Key, item.Value);
        }

        bool ICollection<KeyValuePair<Type, T>>.Contains(KeyValuePair<Type, T> item)
        {
            T value;
            if (_backingStore.TryGetValue(item.Key, out value))
            {
                if (EqualityComparer<T>.Default.Equals(value, item.Value))
                    return true;
            }

            return false;
        }

        void ICollection<KeyValuePair<Type, T>>.CopyTo(KeyValuePair<Type, T>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<Type, T>>)_backingStore).CopyTo(array, arrayIndex);
        }

        bool ICollection<KeyValuePair<Type, T>>.Remove(KeyValuePair<Type, T> item)
        {
            T value;
            if (_backingStore.TryGetValue(item.Key, out value))
            {
                if (EqualityComparer<T>.Default.Equals(value, item.Value))
                    return _backingStore.Remove(item.Key);
            }

            return false;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        void IDictionary.Add(object key, object value)
        {
            Add((Type)key, (T)value);
        }

        void IDictionary.Clear()
        {
            Clear();
        }

        bool IDictionary.Contains(object key)
        {
            if (key == null)
                return ContainsKey(null);

            Type t = key as Type;
            if (t == null)
                return false;

            return ContainsKey(t);
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return _backingStore.GetEnumerator();
        }

        void IDictionary.Remove(object key)
        {
            Type t = key as Type;
            if (t != null || key == null)
                Remove(t);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            ((ICollection)_backingStore).CopyTo(array, index);
        }

        private void HandleAlteration(Type type)
        {
            Type[] altered = _cache.Where(i => type.IsAssignableFrom(i.Key)).Select(i => i.Key).ToArray();
            foreach (var t in altered)
                _cache.Remove(t);
        }
    }
}
