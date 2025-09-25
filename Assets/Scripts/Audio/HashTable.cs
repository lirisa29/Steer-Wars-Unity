using System;
using System.Collections.Generic;

public class HashTable<TK, TV> where TK : IComparable
{
    public List<KeyValuePair<TK, TV>>[] buckets;
    const int defaultCapacity = 20;
    public int size = 0;

    public HashTable()
    {
        buckets = new List<KeyValuePair<TK, TV>>[defaultCapacity];
    }
    public HashTable(int capacity)
    {
        buckets = new List<KeyValuePair<TK, TV>>[capacity];
    }

    public TV Get(TK key)
    {
        int idex = GetIndex(key);
        if (buckets[idex] == null)
            throw new ArgumentException("Key does not exist.");
        int chainIdx = GetChainIndex(idex, key);
        if (chainIdx < 0)
            throw new ArgumentException("Key does not exist.");
        return buckets[idex][chainIdx].Value;
    }

    public TV this[TK key]
    {
        get { return Get(key); }
        set { Add(key, value); }
    }

    public bool ContainsKey(TK key)
    {
        int idex = GetIndex(key);
        if (buckets[idex] != null)
            return GetChainIndex(idex, key) >= 0;
        return false;
    }

    public int GetIndex(TK key)
    {
        return Math.Abs(key.GetHashCode()) % buckets.Length;
    }

    public int GetChainIndex(int idex, TK key)
    {
        if (buckets[idex] == null)
            return -1;
        int chainIdex = 0;
        while (chainIdex < buckets[idex].Count)
        {
            if (buckets[idex][chainIdex].Key.CompareTo(key) == 0)
                return chainIdex;
            chainIdex++;
        }
        return -1;
    }

    public void EnsureCapacity()
    {
        if (size / (float)buckets.Length < defaultCapacity)
            return;
        List<KeyValuePair<TK, TV>>[] oldBuckets = buckets;
        buckets = new List<KeyValuePair<TK, TV>>[oldBuckets.Length * 2];
        foreach (var chain in oldBuckets)
        {
            if (chain == null)
                continue;
            foreach (var pair in chain)
            {
                int idex = GetIndex(pair.Key);
                if (buckets[idex] == null)
                    buckets[idex] = new List<KeyValuePair<TK, TV>>();
                buckets[idex].Add(new KeyValuePair<TK, TV>(pair.Key, pair.Value));
            }
        }
    }

    public void Add(TK key, TV value)
    {
        int idex = GetIndex(key);
        if (buckets[idex] == null)
            buckets[idex] = new List<KeyValuePair<TK, TV>>();
        buckets[idex].Add(new KeyValuePair<TK, TV>(key, value));
        size++;
        EnsureCapacity();
    }

    public bool Remove(TK key)
    {
        int idex = GetIndex(key);
        if (buckets[idex] == null)
            return false;
        int chainIdex = GetChainIndex(idex, key);
        if (chainIdex >= 0)
        {
            buckets[idex].RemoveAt(chainIdex);
            size--;
        }
        return chainIdex >= 0;
    }

    public override string ToString()
    {
        string result = "";
        for (int i = 0; i < buckets.Length; i++)
        {
            result += i + ": ";
            if (buckets[i] != null)
            {
                foreach (var pair in buckets[i])
                {
                    result += pair.Key + " ==> " + pair.Value + ", ";
                }
            }
            result += "\n";
        }
        return result;
    }
}
