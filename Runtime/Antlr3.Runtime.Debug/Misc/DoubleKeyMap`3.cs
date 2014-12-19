namespace Antlr.Runtime.Debug.Misc
{
    using System.Collections.Generic;

    public class DoubleKeyMap<TKey1, TKey2, TValue>
    {
        internal IDictionary<TKey1, IDictionary<TKey2, TValue>> data = new Dictionary<TKey1, IDictionary<TKey2, TValue>>();

        public virtual TValue Put(TKey1 k1, TKey2 k2, TValue v)
        {
            IDictionary<TKey2, TValue> data2;
            data.TryGetValue(k1, out data2);
            TValue prev = default(TValue);
            if (data2 == null)
            {
                data2 = new Dictionary<TKey2, TValue>();
                data[k1]=data2;
            }
            else
            {
                data2.TryGetValue(k2, out prev);
            }
            data2[k2]= v;
            return prev;
        }

        public virtual TValue Get(TKey1 k1, TKey2 k2)
        {
            IDictionary<TKey2, TValue> data2;
            data.TryGetValue(k1, out data2);
            if (data2 == null)
                return default(TValue);

            TValue value;
            data2.TryGetValue(k2, out value);
            return value;
        }

        public virtual IDictionary<TKey2, TValue> Get(TKey1 k1)
        {
            IDictionary<TKey2, TValue> value;
            data.TryGetValue(k1, out value);
            return value;
        }

        /** Get all values associated with primary key */
        public virtual ICollection<TValue> Values(TKey1 k1)
        {
            IDictionary<TKey2, TValue> data2;
            data.TryGetValue(k1, out data2);
            if (data2 == null)
                return null;

            return data2.Values;
        }

        /** get all primary keys */
        public virtual ICollection<TKey1> KeySet()
        {
            return data.Keys;
        }

        /** get all secondary keys associated with a primary key */
        public virtual ICollection<TKey2> KeySet(TKey1 k1)
        {
            IDictionary<TKey2, TValue> data2;
            data.TryGetValue(k1, out data2);
            if (data2 == null)
                return null;

            return data2.Keys;
        }

        public virtual ICollection<TValue> Values()
        {
            Dictionary<TValue, bool> s = new Dictionary<TValue, bool>();
            foreach (IDictionary<TKey2, TValue> k2 in data.Values)
            {
                foreach (TValue v in k2.Values)
                    s[v] = true;
            }

            return s.Keys;
        }
    }
}
