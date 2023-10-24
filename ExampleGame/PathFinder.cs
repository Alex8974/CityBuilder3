using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExampleGame
{
    public class Pathfinding
    {
        public static List<Node> FindPath(Node[,] grid, Node startNode, Node endNode)
        {
            List<Node> openSet = new List<Node>();
            HashSet<Node> closedSet = new HashSet<Node>();
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet[0];
                for (int i = 1; i < openSet.Count; i++)
                {
                    if (openSet[i].FCost < currentNode.FCost || (openSet[i].FCost == currentNode.FCost && openSet[i].HCost < currentNode.HCost))
                    {
                        currentNode = openSet[i];
                    }
                }

                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                if (currentNode == endNode)
                {
                    return RetracePath(startNode, endNode);
                }

                foreach (Node neighbor in GetNeighbors(grid, currentNode))
                {
                    if (!neighbor.IsWalkable || closedSet.Contains(neighbor))
                    {
                        continue;
                    }

                    int newGCost = currentNode.GCost + GetDistance(currentNode, neighbor);

                    if (newGCost < neighbor.GCost || !openSet.Contains(neighbor))
                    {
                        neighbor.GCost = newGCost;
                        neighbor.HCost = GetDistance(neighbor, endNode);
                        neighbor.Parent = currentNode;

                        if (!openSet.Contains(neighbor))
                        {
                            openSet.Add(neighbor);
                        }
                    }
                }
            }

            // No path found
            return null;
        }
        private static List<Node> RetracePath(Node startNode, Node endNode)
        {
            List<Node> path = new List<Node>();
            Node currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.Parent;
            }

            path.Reverse();
            return path;
        }

        private static List<Node> GetNeighbors(Node[,] grid, Node node)
        {
            List<Node> neighbors = new List<Node>();
            int gridSizeX = grid.GetLength(0);
            int gridSizeY = grid.GetLength(1);
            int x = node.X;
            int y = node.Y;

            if (x > 0) neighbors.Add(grid[x - 1, y]);
            if (x < gridSizeX - 1) neighbors.Add(grid[x + 1, y]);
            if (y > 0) neighbors.Add(grid[x, y - 1]);
            if (y < gridSizeY - 1) neighbors.Add(grid[x, y + 1]);

            return neighbors;
        }

        private static int GetDistance(Node nodeA, Node nodeB)
        {
            int dstX = Math.Abs(nodeA.X - nodeB.X);
            int dstY = Math.Abs(nodeA.Y - nodeB.Y);

            return dstX + dstY;
        }
    }

}
