using System.Collections.Generic;

namespace Engine
{
    public static class DictionaryExtensions
    {
        public  static void Add<T1, T2>(this Dictionary<T1, T2> dictionary, KeyValuePair<T1, T2> pair)
        {
            dictionary.Add(pair.Key, pair.Value);
        }

        public static void AddOrInit<T>(this Dictionary<T, int> dictionary, T key, int value)
        {
            if (dictionary.ContainsKey(key))
                dictionary[key] += value;
            else
                dictionary[key] = value;
        }
    }
}
