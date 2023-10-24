using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExampleGame
{
    public class Node
    {
        public int GCost { get; set; } // Cost from start
        public int HCost { get; set; } // Heuristic cost to end
        public int FCost => GCost + HCost;
        public Node Parent { get; set; }
        public int X { get; }
        public int Y { get; }
        public bool IsWalkable { get; set; }

        public Node(int x, int y, bool isWalkable)
        {
            X = x;
            Y = y;
            IsWalkable = isWalkable;
        }
    }

    public class Grid
    {
        public Node[,] Nodes { get; }
        public int Width { get; }
        public int Height { get; }

        public Grid(int width, int height, BasicTilemap Tm)
        {
            Width = width;
            Height = height;
            Nodes = new Node[width, height];

            // Initialize the grid with nodes
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Nodes[x, y] = new Node(x, y, isWalkable: true); // You can set IsWalkable as needed
                    int tileValue = Tm.TileIndices[y * width + x];
                    if (tileValue == 3 || tileValue == 11 || tileValue == 12 || tileValue == 13 || tileValue == 14) Nodes[x, y].IsWalkable = false;
                }
            }
        }
    }

}
