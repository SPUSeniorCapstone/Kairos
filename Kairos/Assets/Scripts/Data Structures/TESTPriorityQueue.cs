using UnityEngine;

public class TESTPriorityQueue : MonoBehaviour
{
    public bool min = false;

    [Button(nameof(TestPriorityQueue))]
    public bool Button_TestPriorityQueue;
    public void TestPriorityQueue()
    {
        PriorityQueue<float> pq = new PriorityQueue<float>(PriorityQueueMode.MAX);

        if (min)
        {
            pq.Mode = PriorityQueueMode.MIN;
        }
        else
        {
            pq.Mode = PriorityQueueMode.MAX;
        }

        for (int i = 0; i < 10; i++)
        {
            pq.Enqueue(Random.Range(-10000, 10000));
        }

        float last = pq.Dequeue();

        while (pq.Count > 0)
        {
            var curr = pq.Dequeue();
            if (min)
            {
                if (curr < last)
                {
                    Debug.Log("Priority Queue Failed");
                    return;
                }
            }
            else
            {
                if (curr > last)
                {
                    Debug.Log("Priority Queue Failed");
                    return;
                }
            }

            last = curr;
        }
        Debug.Log("Priority Queue Succeeded");
    }
}
