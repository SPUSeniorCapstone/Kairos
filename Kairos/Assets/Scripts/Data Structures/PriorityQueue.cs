using System;
using System.Collections.Generic;

public enum PriorityQueueMode
{
    MIN,
    MAX
}

public class PriorityQueue<T> where T : IComparable<T>
{
    /// <summary>
    /// Changing the mode of a non-empty Priority Queue is an O(n) operation 
    /// </summary>
    public PriorityQueueMode Mode
    {
        get { return mode; }
        set
        {
            mode = value;
            if (elements.Count > 0)
            {
                var e = elements.ToArray();
                elements.Clear();
                foreach (var element in e)
                {
                    Enqueue(element);
                }
            }
        }
    }
    private PriorityQueueMode mode = PriorityQueueMode.MAX;

    public List<T> Elements
    {
        get { return elements; }
    }
    private List<T> elements = new List<T>();

    private HashSet<T> container = new HashSet<T>();

    public PriorityQueue(PriorityQueueMode mode)
    {
        this.mode = mode;
    }


    /// <summary>
    /// Adds an item to the Queue; Time Complexity: O(nlog(n))
    /// </summary>
    public void Enqueue(T item)
    {
        elements.Add(item);
        container.Add(item);
        int i = elements.Count - 1;
        while (i > 0)
        {
            int j = (i - 1) / 2;
            if (mode == PriorityQueueMode.MAX && elements[i].CompareTo(elements[j]) <= 0)
            {
                break;
            }
            else if (mode == PriorityQueueMode.MIN && elements[i].CompareTo(elements[j]) >= 0)
            {
                break;
            }
            T tmp = elements[i];
            elements[i] = elements[j];
            elements[j] = tmp;
            i = j;
        }
    }

    /// <summary>
    /// Dequeues the MIN/MAX value of the Queue; Time Complexity: O(nlog(n))
    /// </summary>
    public T Dequeue()
    {
        T ret = elements[0];
        elements[0] = elements[elements.Count - 1];
        elements.RemoveAt(elements.Count - 1);
        container.Remove(ret);

        int i = 0;
        while (true)
        {
            int j = i * 2 + 1;
            if (j >= elements.Count)
            {
                break;
            }
            int rc = j + 1;
            
            if(mode == PriorityQueueMode.MIN)
            {
                if (rc < elements.Count && elements[rc].CompareTo(elements[j]) < 0)
                {
                    j = rc;
                }
                if (elements[i].CompareTo(elements[j]) <= 0)
                {
                    break;
                }
            }
            else
            {
                if (rc < elements.Count && elements[rc].CompareTo(elements[j]) > 0)
                {
                    j = rc;
                }
                if (elements[i].CompareTo(elements[j]) >= 0)
                {
                    break;
                }
            }

            T tmp = elements[i];
            elements[i] = elements[j];
            elements[j] = tmp;
            i = j;
        }
        return ret;
    }

    /// <summary>
    /// Returns, but does not remove, the MIN/MAX value of the Queue; Time Complexity: O(1)
    /// </summary>
    /// <returns></returns>
    public T Peek()
    {
        return elements[0];
    }

    public int Count
    {
        get
        {
            return elements.Count;
        }
    }

    public bool Contains(T item)
    {
        return container.Contains(item);
    }
}
