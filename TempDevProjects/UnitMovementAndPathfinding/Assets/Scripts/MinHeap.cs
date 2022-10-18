using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro.EditorUtilities;
using Unity.VisualScripting;
using UnityEngine;

public class MinHeap<K, T> /*: IEnumerable<KeyValuePair<K, List<T>>>*/ where K : IComparable<K>
{
    public int Count { get { return itemCount; } }
    int itemCount = 0;
    public List<KeyValuePair<K,T>> values = new List<KeyValuePair<K, T>>();
    Dictionary<K, int> index = new Dictionary<K, int>();

    public T[] Values
    {
        get
        {
            T[] values = new T[itemCount];
            for (int i = 0; i < itemCount; i++)
            {
                values[i] = this.values[i].Value;
            }
            return values;
        }
    }

    public void Add(K key, T value)
    {
        if (index.ContainsKey(key))
        {
            values[index[key]] = new KeyValuePair<K, T>(key, value); 
            return;
        }
        else
        {
            index.Add(key, itemCount);
            values.Insert(itemCount, new KeyValuePair<K, T>(key, value));
            SortUp(key);
            itemCount++;
        }
    }

    public KeyValuePair<K, T> Peek()
    {
        if(itemCount > 0)
            return values[0];
        return default(KeyValuePair<K, T>);
    }

    public KeyValuePair<K,T> First()
    {
        if(itemCount > 0)
        {
            var first = values[0];
            index.Remove(first.Key);
            var insertVal = values[itemCount - 1];
            values.RemoveAt(itemCount - 1);
            itemCount--;
            if (itemCount > 1)
            {
                values[0] = insertVal;
                index[values[0].Key] = 0;
                SortDown(values[0].Key);
            }
            return first;
        }

        return default(KeyValuePair<K, T>);
    }

    public T GetValue(K key)
    {
        return values[index[key]].Value;
    }

    public bool Contains(T value)
    {
        for(int i = 0; i < itemCount; i++)
        {
            if (values[i].Value.Equals(value))
            {
                return true;
            }
        }
        return false;
    }

    public bool ContainsKey(K key)
    {
        return index.ContainsKey(key);
    }

    void SortDown(K key)
    {
        if(itemCount == 0)
        {
            return;
        }

        int c = 0;

        while (true)
        {
            if (c > itemCount)
            {
                Debug.LogError("Sort Down failed to end");
                return;
            }
            c++;

            int leftIndex = (index[key] * 2) + 1;
            int rightIndex = (index[key] * 2) + 2;

            int swapIndex = 0;

            if(leftIndex < itemCount)
            {
                swapIndex = leftIndex;
                if (rightIndex < itemCount)
                {
                    if(values[rightIndex].Key.CompareTo(values[leftIndex].Key) < 0)
                    {
                        swapIndex = rightIndex;
                    }
                }
                if (key.CompareTo(values[swapIndex].Key) > 0)
                {
                    Swap(key, values[swapIndex].Key);
                }
                else
                {
                    break;
                }
            }
            else
            {
                break;
            }



        }
    }

    void SortUp(K key)
    {
        if(itemCount == 0)
        {
            return;
        }

        int parentIndex = (index[key] - 1) / 2;

        int c = 0;
        while (key.CompareTo(values[parentIndex].Key) < 0)
        {
            if(c > itemCount)
            {
                Debug.LogError("Sort Up failed to end");
                return;
            }
            c++;

            Swap(key, values[parentIndex].Key);
            parentIndex = (index[key] - 1) / 2;
            
        }
    }

    void Swap(K A, K B)
    {
        if(A.CompareTo(B) == 0)
        {
            return;
        }

        //Debug.Log("Swapping A:" + index[A] + " " + values[index[A]] + " B:" + index[B] + " " + values[index[B]]);

        var itemA = values[index[A]];
        var itemB = values[index[B]];
        int indexA = index[A];
        int indexB = index[B];

        //Set B to A
        values[indexA] = itemB;
        index[B] = indexA;

        //Set A to B
        values[indexB] = itemA;
        index[A] = indexB;

        //Debug.Log("Swapped A:" + index[A] + " " + values[index[A]] + " B:" + index[B] + " " + values[index[B]]);
    }

    public void Clear()
    {
        values.Clear();
        index.Clear();
        itemCount = 0;
    }

    public void ClearAll()
    {
        values.Clear();
        index.Clear();
        itemCount = 0;
    }

    public bool TestMinHeap()
    {
        K n = Peek().Key;
        if (!TestMin(n))
        {
            return false;
        }

        var v = First();
        n = Peek().Key;
        if (!TestMin(n))
        {
            return false;
        }

        Add(v.Key, v.Value);
        n = Peek().Key;
        if (!TestMin(n))
        {
            return false;
        }

        for (int i = 0; i < values.Count; i++)
        {
            var val = values[i].Key;
            int left = (i * 2) + 1;
            int right = (i * 2) + 2;

            if(left < Count)
            {
                if (val.CompareTo(values[left].Key) > 0)
                {
                    Debug.Log("Parent was larger than child: index: " + i + " key: " + val);
                    return false;
                }
            }

            if (right < Count)
            {
                if (val.CompareTo(values[right].Key) > 0)
                {
                    Debug.Log("Parent was larger than child: index: " + i + " key: " + val);
                    return false;
                }
            }
        }





        Debug.Log("All Tests Passed");
        return true;

        bool TestMin(K n)
        {
            K min = values.Min((A) => A.Key);
            if (n.CompareTo(min) != 0)
            {
                Debug.LogError("MinHeap test failed - the first value was not the minimum value");
                return false;
            }
            return true;
        }
    }
}
