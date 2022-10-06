using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathFinder
{
    List<Vector2> restricted = new List<Vector2>();

    class Node
    {
        Vector2 position;

        /// <summary>
        /// Distance from starting node
        /// </summary>
        public float g;

        /// <summary>
        /// Distance from end node
        /// </summary>
        public float h;

        public float cost
        {
            get
            {
                return g + h;
            }
        }

        public Node(Vector2 pos, Vector2 start, Vector2 end)
        {
            position = pos;
            g = Vector2.Distance(pos, start);
            h = Vector2.Distance(pos, end);
        }

        public Node(Vector2 pos, float gCost, float hCost)
        {
            position = pos;
            this.g = gCost;
            this.h = hCost;
        }
    }
    static void FindPath(Vector2 start, Vector2 end)
    {
        Vector2[] ComparePositions =
            {
                
            };

        List<Node> open = new List<Node>();
        List<Node> closed = new List<Node>();

        float h = Vector2.Distance(start, end);
        float g = 0;

        

        Node current = new Node(start, start, end);
        
        while (h > 0)
        {
            
        }
    }
}
