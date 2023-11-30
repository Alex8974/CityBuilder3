using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CityBuilderGame.Farmer;
using static CityBuilderGame.Lumberjack;

namespace ExampleGame
{
    public class Planter : Person
    {
        public enum PlanterState
        {
            Idle,
            Planting,
            GoingToPlant,
            ReturningHome
        }

        public PlanterState state = PlanterState.Idle; // Holds the current farmer task
        private Vector2 PlantingLocation; // Store the current farm location
        private Grid grid; // Reference to the grid of nodes of water
        private BasicTilemap bm; // the building map that is passed in from the constructor

        private double FarmingCoolDown = 0; // the time since last farm from the farmer
        int ResourcesHeld = 0;
        public static List<Vector2> allplantingLocations = new List<Vector2>();

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="pos">the farmers starting location</param>
        /// <param name="g">the grid of water so they know where not to move</param>
        /// <param name="c">the content manager</param>
        /// <param name="bm">the buildingtile map</param>
        public Planter(Vector2 pos, Grid g, ContentManager c, BasicTilemap bm, List<House> h)
        {
            grid = g;
            Position = new Vector2(pos.X * 32, pos.Y * 32);
            this.bm = bm;
            FindHome(h, bm);
            texture = c.Load<Texture2D>("Farmer");
            DrawPosition = Position;
            // Initialize other properties
        }

        public Planter(Vector2 pos, ContentManager c, BasicTilemap bm, List<House> h)
        {
            grid = null;
            Position = new Vector2(pos.X * 32, pos.Y * 32);
            this.bm = bm;
            FindHome(h, bm);
            texture = c.Load<Texture2D>("Farmer");
            DrawPosition = Position;
            // Initialize other properties
        }

        /// <summary>
        /// updates the animation for the farmer
        /// </summary>
        /// <param name="gT">the game time</param>
        private void UpdateAnimation(GameTime gT)
        {
            animationTimer += gT.ElapsedGameTime.TotalSeconds;
            if (animationTimer > 0.30f)
            {
                switch (state)
                {
                    case PlanterState.Idle:
                        animationFrame = 0;
                        break;
                    case PlanterState.Planting:
                        if (animationFrame < 3) animationFrame = 3;
                        else if (animationFrame == 5) animationFrame = 3;
                        else animationFrame++;
                        break;
                    case PlanterState.ReturningHome:
                        if (animationFrame == 1) animationFrame = 2;
                        else if (animationFrame == 2) animationFrame = 1;
                        else animationFrame = 1;
                        break;
                    case PlanterState.GoingToPlant:
                        if (animationFrame == 1) animationFrame = 2;
                        else if (animationFrame == 2) animationFrame = 1;
                        else animationFrame = 1;
                        break;
                }
                animationTimer = 0;
            }
        }

        /// <summary>
        /// updates the farmer
        /// </summary>
        /// <param name="gT">the game time</param>
        /// <param name="Tm"> the building map</param>
        public new void Update(GameTime gT, BasicTilemap Tm, out int NewResources, List<House> h)
        {
            NewResources = 0;
            switch (state)
            {
                case PlanterState.Idle:
                    // Find the next farm location and update state to "Farming"
                    PlantingLocation = FindNextPlantLocation(Tm);
                    allplantingLocations.Add(PlantingLocation);
                    dest = PlantingLocation;
                    if (home == PlantingLocation) PlantingLocation = Vector2.Zero;
                    UpdateAnimation(gT);
                    if (Position != PlantingLocation) state = PlanterState.GoingToPlant;
                    else if (PlantingLocation == Position) state = PlanterState.Planting;
                    break;

                case PlanterState.Planting:
                    // Farm at the current location and update grid
                    FarmingCoolDown += gT.ElapsedGameTime.TotalSeconds;
                    UpdateAnimation(gT);
                    if (FarmingCoolDown > 1.0f)
                    {
                        PlantTile(PlantingLocation, Tm);
                        state = PlanterState.ReturningHome;
                        FarmingCoolDown = 0;
                        ResourcesHeld = 1;
                        allplantingLocations.Remove(PlantingLocation);
                    }
                    break;

                case PlanterState.ReturningHome:
                    // Move back to the home location

                    if (home != Vector2.Zero)
                    {
                        dest = home;
                        base.Update(gT, Tm, grid);
                        UpdateAnimation(gT);
                        // If the farmer reaches home, update state to "Idle"
                        if (Position == home)
                        {
                            state = PlanterState.Idle;
                            if (CheckHome(bm))
                            {
                                NewResources += ResourcesHeld;
                                ResourcesHeld = 0;
                            }
                            else
                            {
                                home = Vector2.Zero;
                                FindHome(h, bm);
                            }
                        }
                    }
                    else { FindHome(h, bm); }

                    break;
                case PlanterState.GoingToPlant:
                    UpdateAnimation(gT);
                    if (PlantingLocation == new Vector2(0, 0))
                    {
                        state = PlanterState.Idle;
                    }
                    if (PlantingLocation != Position) base.Update(gT, Tm, grid);
                    else if (PlantingLocation == Position) { state = PlanterState.Planting; }
                    else
                    { state = PlanterState.Idle; }
                    break;

            }
        }

        /// <summary>
        /// finds the closest farm to the farmer
        /// </summary>
        /// <param name="Tm">the building tile map</param>
        /// <returns>returns the position in a vector2</returns>
        private Vector2 FindNextPlantLocation(BasicTilemap Tm)
        {
            // Use the Pathfinding class to find the next farmable location
            // You'll need to convert the position of the farmer to grid coordinates
            // and also convert the farm locations to grid coordinates if they aren't already
            // Then, you can call the FindPath method to find a path to the nearest farm spot

            int farmerGridX = (int)(Position.X / Tm.TileWidth);
            int farmerGridY = (int)(Position.Y / Tm.TileHeight);

            if (farmerGridX > 100) farmerGridX = farmerGridX / Tm.TileWidth;
            if (farmerGridY > 100) farmerGridY = farmerGridY / Tm.TileWidth;
            // Assuming you have a list of farmable locations, convert them to grid coordinates
            // or use grid coordinates directly if they are in grid coordinates
            List<Vector2> farmableLocations = new List<Vector2>();

            for (int i = 0; i < Tm.TileIndices.Length - 1; i++)
            {
                if (Tm.TileIndices[i] > 24 && Tm.TileIndices[i] <= 30 || Tm.TileIndices[i] == 23)
                {
                    int row = ((i / Tm.MapWidth));
                    int col = ((i % Tm.MapWidth));

                    if (allFarmingLocations.Contains(new Vector2(col * bm.TileHeight, row * bm.TileWidth)) ||
                        allplantingLocations.Contains(new Vector2(col * bm.TileHeight, row * bm.TileWidth)) ||
                        allWoodChoppingLocations.Contains(new Vector2(col * bm.TileHeight, row * bm.TileWidth))
                        )
                    {
                        continue; // Skip this location
                    }

                    farmableLocations.Add(new Vector2(col, row));
                }
            }
            Node startNode = grid.Nodes[farmerGridX, farmerGridY];

            Node nearestFarmNode = null;
            int shortestPathLength = int.MaxValue;

            foreach (var farmLocation in farmableLocations)
            {
                int farmGridX = (int)(farmLocation.X);
                int farmGridY = (int)(farmLocation.Y);
                Node endNode = grid.Nodes[farmGridX, farmGridY];


                List<Node> path = Pathfinding.FindPath(grid.Nodes, startNode, endNode);

                if (path != null && path.Count < shortestPathLength)
                {
                    nearestFarmNode = endNode;
                    shortestPathLength = path.Count;
                }
            }

            if (nearestFarmNode != null)
            {
                return new Vector2(nearestFarmNode.X * Tm.TileWidth, nearestFarmNode.Y * Tm.TileHeight);
            }
            else
            {
                // Handle the case where no farmable location is reachable
                return Position; // Stay at the current position
            }
        }

        /// <summary>
        /// farms the tile that the farmer is on
        /// </summary>
        /// <param name="location">the location of the tile</param>
        /// <param name="bm">the building map</param>
        private void PlantTile(Vector2 location, BasicTilemap bm)
        {
            int tilex = (int)location.X / 32;
            int tiley = (int)location.Y / 32;
            if ((24 < bm.TileIndices[tiley * bm.MapWidth + tilex] && bm.TileIndices[tiley * bm.MapWidth + tilex] <= 30) || bm.TileIndices[tiley * bm.MapWidth + tilex] == 23)
            {
                bm.TileIndices[tiley * bm.MapWidth + tilex]--;
            }
        }

        public override void Building()
        {
            throw new NotImplementedException();
        }

        public override void CheckForBuild()
        {
            throw new NotImplementedException();
        }

        public override void Draw(SpriteBatch s, GameTime gT)
        {
            s.Draw(texture, new Vector2(DrawPosition.X - 2, DrawPosition.Y), new Rectangle(animationFrame * size, 0, size, size), Color.White, 0, new Vector2(0, 0), 1.25f, SpriteEffects.None, 0);
        }
    }
}
