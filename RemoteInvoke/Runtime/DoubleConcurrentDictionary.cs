using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable
namespace RemoteInvoke.Runtime
{
    public class DoubleConcurrentDictionary<TKey, TValue> : ConcurrentDictionary<TKey, TValue> where TKey : notnull where TValue : notnull
    {
        private readonly ConcurrentDictionary<TValue, TKey> reverseMappedDictionary = new();

        public new TValue this[TKey key]
        {
            get => base[key];

            set
            {
                base[key] = value;
                reverseMappedDictionary[value] = key;
            }
        }

        public new TValue AddOrUpdate<TArg>(TKey key, Func<TKey, TArg, TValue> addValueFactory, Func<TKey, TValue, TArg, TValue> updateValueFactory, TArg factoryArgument)
        {
            TValue value = base.AddOrUpdate<TArg>(key, addValueFactory, updateValueFactory, factoryArgument);

            AddOrUpdateReverse(value, key);

            return value;
        }

        public TKey AddOrUpdate<TArg>(TValue key, Func<TValue, TArg, TKey> addValueFactory, Func<TValue, TKey, TArg, TKey> updateValueFactory, TArg factoryArgument)
        {
            TKey value = reverseMappedDictionary.AddOrUpdate<TArg>(key, addValueFactory, updateValueFactory, factoryArgument);

            AddOrUpdateBase(value, key);

            return value;
        }

        public new TValue AddOrUpdate(TKey key, TValue addValue, Func<TKey, TValue, TValue> updateValueFactory)
        {
            TValue value = base.AddOrUpdate(key, addValue, updateValueFactory);

            AddOrUpdateReverse(value, key);

            return value;
        }

        public TKey AddOrUpdate(TValue key, TKey addValue, Func<TValue, TKey, TKey> updateValueFactory)
        {
            TKey value = reverseMappedDictionary.AddOrUpdate(key, addValue, updateValueFactory);

            AddOrUpdateBase(value, key);

            return value;
        }

        public new TValue AddOrUpdate(TKey key, Func<TKey, TValue> addValueFactory, Func<TKey, TValue, TValue> updateValueFactory)
        {
            TValue value = base.AddOrUpdate(key, addValueFactory, updateValueFactory);

            AddOrUpdateReverse(value, key);

            return value;
        }

        public TKey AddOrUpdate(TValue key, Func<TValue, TKey> addValueFactory, Func<TValue, TKey, TKey> updateValueFactory)
        {
            TKey value = reverseMappedDictionary.AddOrUpdate(key, addValueFactory, updateValueFactory);

            AddOrUpdateBase(value, key);

            return value;
        }

        public new void Clear()
        {
            base.Clear();
            reverseMappedDictionary.Clear();
        }

        public bool ContainsKey(TValue key) => reverseMappedDictionary.ContainsKey(key);

        public new IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => base.GetEnumerator();

        public new TValue GetOrAdd(TKey key, TValue value)
        {
            TValue result = base.GetOrAdd(key, value);

            AddOrUpdateReverse(result, key);

            return result;
        }

        public TKey GetOrAdd(TValue key, TKey value)
        {
            TKey result = reverseMappedDictionary.GetOrAdd(key, value);

            AddOrUpdateBase(result, key);

            return result;
        }

        public new TValue GetOrAdd<TArg>(TKey key, Func<TKey, TArg, TValue> valueFactory, TArg factoryArgument)
        {
            TValue result = base.GetOrAdd(key, valueFactory, factoryArgument);

            AddOrUpdateReverse(result, key);

            return result;
        }

        public TKey GetOrAdd<TArg>(TValue key, Func<TValue, TArg, TKey> valueFactory, TArg factoryArgument)
        {
            TKey result = reverseMappedDictionary.GetOrAdd(key, valueFactory, factoryArgument);

            AddOrUpdateBase(result, key);

            return result;
        }

        public new TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory)
        {
            TValue result = base.GetOrAdd(key, valueFactory);

            AddOrUpdateReverse(result, key);

            return result;
        }

        public TKey GetOrAdd(TValue key, Func<TValue, TKey> valueFactory)
        {
            TKey result = reverseMappedDictionary.GetOrAdd(key, valueFactory);

            AddOrUpdateBase(result, key);

            return result;
        }

        public new bool TryAdd(TKey key, TValue value)
        {
            bool pass = base.TryAdd(key, value);

            if (pass)
            {
                AddOrUpdateReverse(value, key);
            }

            return pass;
        }

        public bool TryAdd(TValue key, TKey value)
        {
            bool pass = reverseMappedDictionary.TryAdd(key, value);

            if (pass)
            {
                AddOrUpdateBase(value, key);
            }

            return pass;
        }

        public bool TryGetValue(TValue key, [MaybeNullWhen(false)] out TKey value) => reverseMappedDictionary.TryGetValue(key, out value);

        public new bool TryRemove(KeyValuePair<TKey, TValue> item)
        {
            if (base.TryRemove(item))
            {
                reverseMappedDictionary.TryRemove(new KeyValuePair<TValue, TKey>(item.Value, item.Key));
                return true;
            }
            return false;
        }

        public bool TryRemove(KeyValuePair<TValue, TKey> item)
        {
            if (reverseMappedDictionary.TryRemove(item))
            {
                base.TryRemove(new KeyValuePair<TKey, TValue>(item.Value, item.Key));
                return true;
            }
            return false;
        }

        public new bool TryRemove(TKey key, [MaybeNullWhen(false)] out TValue value)
        {
            if (base.TryRemove(key, out value))
            {
                reverseMappedDictionary.Remove(value, out _);
                return true;
            }
            return false;
        }

        public bool TryRemove(TValue key, [MaybeNullWhen(false)] out TKey value)
        {
            if (reverseMappedDictionary.TryRemove(key, out value))
            {
                base.TryRemove(value, out _);
                return true;
            }
            return false;
        }

        public new bool TryUpdate(TKey key, TValue newValue, TValue comparisonValue)
        {
            if (base.TryUpdate(key, newValue, comparisonValue))
            {
                AddOrUpdateReverse(newValue, key);
                return true;
            }
            return false;
        }

        public bool TryUpdate(TValue key, TKey newValue, TKey comparisonValue)
        {
            if (reverseMappedDictionary.TryUpdate(key, newValue, comparisonValue))
            {
                AddOrUpdateBase(newValue, key);
                return true;
            }
            return false;
        }

        private bool AddOrUpdateBase(TKey key, TValue value)
        {
            if (base.ContainsKey(key))
            {
                base[key] = value;

                return true;
            }
            else
            {
                return base.TryAdd(key, value);
            }
        }

        private bool AddOrUpdateReverse(TValue key, TKey value)
        {
            if (reverseMappedDictionary.ContainsKey(key))
            {
                reverseMappedDictionary[key] = value;

                return true;
            }
            else
            {
                return reverseMappedDictionary.TryAdd(key, value);
            }
        }
    }
}
