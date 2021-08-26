using System.Collections.Generic;
using Domain.StageObj;
using UnityEngine;

namespace Domain
{
    public class Node
    {
        public int X;
        public int Y;
        public int F;
        public int G;
        public int H;
        public int Height = 0;
        public List<Node> Neighbors;
        public Node previous = null;

        public Vector3 position => new Vector3(X, Y, 0);

        public bool hasSpy => spy != null;
        public bool hasItem => item != null;

        public Spy spy;
        public Item item;

        public Node(int x, int y, int height)
        {
            X = x;
            Y = y;
            F = 0;
            G = 0;
            H = 0;
            Neighbors = new List<Node>();
            Height = height;
        }

        public void AddNeighbors(Node[,] grid, int x, int y)
        {
            if (x < grid.GetUpperBound(0))
                Neighbors.Add(grid[x + 1, y]);
            if (x > 0)
                Neighbors.Add(grid[x - 1, y]);
            if (y < grid.GetUpperBound(1))
                Neighbors.Add(grid[x, y + 1]);
            if (y > 0)
                Neighbors.Add(grid[x, y - 1]);
        }
    }
}