using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExampleGame;
using System.Windows.Forms.VisualStyles;
using Microsoft.Xna.Framework.Content;
using ExampleGame.Enums;

namespace CityBuilderGame
{
    public class Farmer : Person
    {
         

        // Define farmer states
        private enum FarmerState
        {
            Idle,
            Farming,
            GoingtoFarm,
            ReturningHome
        }

        private FarmerState state = FarmerState.Idle; // Holds the current farmer task
        private Vector2 curfarmLocation; // Store the current farm location
        private Grid grid; // Reference to the grid of nodes of water
        private BasicTilemap bm; // the building map that is passed in from the constructor
        private int FarmerFrame = 0;

        private double FarmingCoolDown = 0;
        private double AnimationTmer = 0;

        public override int Health { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="pos">the farmers starting location</param>
        /// <param name="g">the grid of water so they know where not to move</param>
        /// <param name="c">the content manager</param>
        /// <param name="bm">the buildingtile map</param>
        public Farmer(Vector2 pos, Grid g, ContentManager c, BasicTilemap bm)
        {
            grid = g;
            Position = new Vector2(pos.X * 32, pos.Y * 32);
            this.bm = bm;
            FindHome();
            texture = c.Load<Texture2D>("Farmer");
            // Initialize other properties
        }

        /// <summary>
        /// finds the closest unocupied home and assigns it to the farmer
        /// </summary>
        private void FindHome()
        {
            for(int i = 0; i < bm.TileIndices.Length-1; i++)
            {
                if (bm.TileIndices[i] == (int)BuildingOptions.BlueHouse)
                {
                    bm.TileIndices[i]++;
                    int row = (i / bm.MapWidth);
                    int col = (i % bm.MapWidth);
                    home = new Vector2(col * bm.TileWidth, row * bm.TileHeight);
                    break;
                }
                else
                {
                    home = Vector2.Zero;
                }
            }
        }

        private void UpdateAnimation(GameTime gT)
        {
            AnimationTmer += gT.ElapsedGameTime.TotalSeconds;
            if(AnimationTmer > 0.30f)
            {
                switch (state)
                {
                    case FarmerState.Idle:
                        FarmerFrame = 0;
                        break;
                    case FarmerState.Farming:
                        if (FarmerFrame < 3) FarmerFrame = 3;
                        else if (FarmerFrame == 5) FarmerFrame = 3;
                        else FarmerFrame++;
                        break;
                    case FarmerState.ReturningHome:
                        if (FarmerFrame == 1) FarmerFrame = 2;
                        else if (FarmerFrame == 2) FarmerFrame = 1;
                        else FarmerFrame = 1;
                        break;
                    case FarmerState.GoingtoFarm:
                        if (FarmerFrame == 1) FarmerFrame = 2;
                        else if (FarmerFrame == 2) FarmerFrame = 1;
                        else FarmerFrame = 1;
                        break;
                }
                AnimationTmer = 0;
            }
            
        }

        /// <summary>
        /// updates the farmer
        /// </summary>
        /// <param name="gT">the game time</param>
        /// <param name="Tm"> the building map</param>
        public new void Update(GameTime gT, BasicTilemap Tm)
        {
            switch (state)
            {
                case FarmerState.Idle:
                    // Find the next farm location and update state to "Farming"
                    curfarmLocation = FindNextFarmLocation(Tm);
                    dest = curfarmLocation;
                    UpdateAnimation(gT);
                    if (Position != curfarmLocation) state = FarmerState.GoingtoFarm;
                    else state = FarmerState.Farming;
                    break;

                case FarmerState.Farming:
                    // Farm at the current location and update grid
                    FarmingCoolDown += gT.ElapsedGameTime.TotalSeconds;
                    UpdateAnimation(gT);
                    if(FarmingCoolDown > 1.0f)
                    {
                        FarmTile(curfarmLocation, Tm);
                        state = FarmerState.ReturningHome;
                        FarmingCoolDown = 0;
                    }
                    break;

                case FarmerState.ReturningHome:
                    // Move back to the home location
                    if (home != Vector2.Zero)
                    {
                        dest = home;
                        base.Update(gT, Tm, grid);
                        UpdateAnimation(gT);
                        // If the farmer reaches home, update state to "Idle"
                        if (Position == home){state = FarmerState.Idle;}
                    }
                    else{FindHome();}
                    
                    break;
                case FarmerState.GoingtoFarm:
                    UpdateAnimation(gT);
                    if (curfarmLocation != Position) base.Update(gT, Tm, grid);
                    else if(curfarmLocation == Position) { state = FarmerState.Farming; }
                    else
                    {
                        state = FarmerState.Idle;
                    }
                    break;
            }
        }

        private Vector2 FindNextFarmLocation(BasicTilemap Tm)
        {
            // Use the Pathfinding class to find the next farmable location
            // You'll need to convert the position of the farmer to grid coordinates
            // and also convert the farm locations to grid coordinates if they aren't already
            // Then, you can call the FindPath method to find a path to the nearest farm spot

            int farmerGridX = (int)(Position.X / Tm.TileWidth);
            int farmerGridY = (int)(Position.Y / Tm.TileHeight);

            // Assuming you have a list of farmable locations, convert them to grid coordinates
            // or use grid coordinates directly if they are in grid coordinates
            List<Vector2> farmableLocations = new List<Vector2>();
            
            for(int i = 0; i < Tm.TileIndices.Length-1; i++)
            {
                if(Tm.TileIndices[i] >= 24 && Tm.TileIndices[i] < 30)
                {
                    int row = ((i / Tm.MapWidth));
                    int col = ((i % Tm.MapWidth));
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


        private void FarmTile(Vector2 location, BasicTilemap bm)
        {
            int tilex = (int)location.X / 32;
            int tiley = (int)location.Y / 32;
            if(bm.TileIndices[tiley * bm.MapWidth + tilex] > 23 && bm.TileIndices[tiley * bm.MapWidth + tilex] + 1 < 31)
            {
                bm.TileIndices[tiley * bm.MapWidth + tilex] = bm.TileIndices[tiley * bm.MapWidth + tilex] + 1;
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
            s.Draw(texture, new Vector2(Position.X-2, Position.Y), new Rectangle(FarmerFrame*32,0,32,32), Color.White, 0, new Vector2(0, 0), 1.25f, SpriteEffects.None, 0);
        }
    }

}
