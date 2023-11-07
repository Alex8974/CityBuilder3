using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExampleGame.BiggerTileMapGenerator
{
    public class TileMapGenerator
    {
        static Random random = new Random();

        static int mapWidth = 30;
        static int mapHeight = 20;

        static int numberOfTileTypes = 5; // Number of different tile types
        static int waterTileType = 3;     // The tile type representing water

        static int[,] tileMap = new int[mapWidth, mapHeight];

        static void Main()
        {
            GenerateRandomTileMapWithWaterClusters();
            SmoothWaterBodies();
            ApplyPostGenerationRules();
            PrintTileMap();
        }

        static void GenerateRandomTileMapWithWaterClusters()
        {
            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    // Generate a random tile number, favoring water near existing water
                    double waterProbability = 0.4;
                    if (random.NextDouble() < waterProbability)
                    {
                        tileMap[x, y] = waterTileType;
                    }
                    else
                    {
                        tileMap[x, y] = random.Next(1, numberOfTileTypes);
                    }
                }
            }
        }

        static void SmoothWaterBodies()
        {
            bool[,] visited = new bool[mapWidth, mapHeight];

            // Define the directions for neighboring tiles
            int[] dx = { 1, 0, -1, 0 };
            int[] dy = { 0, 1, 0, -1 };

            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    if (tileMap[x, y] == waterTileType && !visited[x, y])
                    {
                        int waterClusterSize = FloodFill(x, y, visited);

                        // Check if the water cluster is small (you can adjust the threshold)
                        if (waterClusterSize < 10)
                        {
                            // Convert small water clusters to another tile type (e.g., land)
                            for (int yy = 0; yy < mapHeight; yy++)
                            {
                                for (int xx = 0; xx < mapWidth; xx++)
                                {
                                    if (visited[xx, yy])
                                    {
                                        tileMap[xx, yy] = random.Next(1, numberOfTileTypes); // Change to another tile type
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private static int FloodFill(int x, int y, bool[,] visited)
        {
            int[] dx = { 1, 0, -1, 0 };
            int[] dy = { 0, 1, 0, -1 };

            if (x < 0 || x >= mapWidth || y < 0 || y >= mapHeight)
                return 0;
            if (visited[x, y] || tileMap[x, y] != waterTileType)
                return 0;

            visited[x, y] = true;
            int clusterSize = 1;

            // Recursively apply flood fill to neighboring tiles
            for (int i = 0; i < 4; i++)
            {
                clusterSize += FloodFill(x + dx[i], y + dy[i], visited);
            }

            return clusterSize;
        }
    

        static void ApplyPostGenerationRules()
        {
            // Implement post-generation rules to refine the map (e.g., smooth transitions)
            // This can be done by iterating through the map and applying rules based on neighbors.
            // For example, ensure that water doesn't have abrupt edges, and other terrains blend smoothly.
        }

        static void PrintTileMap()
        {
            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    Console.Write(tileMap[x, y] + " ");
                }
                Console.WriteLine();
            }
        }
    }

}
