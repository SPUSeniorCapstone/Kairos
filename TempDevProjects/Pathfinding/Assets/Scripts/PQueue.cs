using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Priority Queue
/// </summary>
/// <typeparam name="t">type</typeparam>
public class PQueue<t>
{
    List<t> list = new List<t>();
    Comparison<t> comparison;

    /// <summary>
    /// Creates a new Priority Queue
    /// </summary>
    /// <param name="_compare">Compare(A, B)</param>
    public PQueue(Comparison<t> _compare)
    {
        comparison = _compare;
    }

    public void Enqueue(t Item)
    {
        for(int i = 0; i < list.Count; i++)
        {
            if(comparison(Item, list[i]) >= 0)
            {
                list.Prepend(Item);
            }
            else if(i == list.Count - 1)
            {
                list.Append(Item);
            }
        }
    }

    public t Dequeue()
    {
        if(Empty())
        {
            return default(t);
        }

        t item = list[0];
        list.RemoveAt(0);
        return item;
    }

    public bool Empty()
    {
        if(list.Count == 0) return false;
        return true;
    }

}
