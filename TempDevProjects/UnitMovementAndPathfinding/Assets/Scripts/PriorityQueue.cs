using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


/// <summary>
/// Creates a Priority Queue with key K and value T. T is stored as an array so as to allow multiple entries with the same key. 
/// If entries have the same key, its first in first out.
/// </summary>
/// <typeparam name="K"></typeparam>
/// <typeparam name="T"></typeparam>
public class PriorityQueue<K, T> /*: IEnumerable<KeyValuePair<K, List<T>>>*/ where K : IComparable<K>
{
    public MinHeap<K, List<T>> list;

    public PriorityQueue()
    {
        list = new MinHeap<K, List<T>>();
    }

    public T Peek()
    {
        var item = list.Peek();
        var ret = item.Value[0];
        return ret;
    }

    /// <summary>
    /// If multiple items with the same key are added, Dequeue will return them in order of submission
    /// </summary>
    /// <returns></returns>
    public T Dequeue()
    {
        if(list.Count == 0)
        {
            return default(T);
        }

        var item = list.First();

        if(item.Value == null || item.Value.Count == 0)
        {
            return default(T);
        }

        var ret = item.Value[0];
        item.Value.RemoveAt(0);
        if (item.Value.Count > 0)
        {
            list.Add(item.Key, item.Value);
        }
        return ret;
    }

    public void Enqueue(K key, T value)
    {
        if (list.ContainsKey(key))
        {
            var l = list.GetValue(key);
            l.Add(value);
            list.Add(key, l);
        }
        else
        {
            List<T> l = new List<T>();
            l.Add(value);
            list.Add(key, l);
        }
    }

    public bool ContainsKey(K key)
    {
        return list.ContainsKey(key);
    }

    public bool Contains(T value)
    {
        foreach (var val in list.Values)
        {
            if (val.Contains(value))
            {
                return true;
            }
        }
        return false;
    }

    public void Clear()
    {
        list.Clear();
    }

    public List<T> Values
    {
        get
        {
            List<T> val = new List<T>();
            foreach (var item in list.Values)
            {
                foreach (var i in item)
                {
                    val.Add(i);
                }
            }
            return val;
        }
    }

    public void RunTest()
    {
        list.TestMinHeap();
    }
}
