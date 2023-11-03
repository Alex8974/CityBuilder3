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
using SharpDX.DirectWrite;

namespace CityBuilderGame
{
    public class Lumberjack : Person
    {
         

        // Define lumberjack states
        public enum LumberjackState
        {
            Idle,
            ChoppingWood,
            GoingToTree,
            ReturningHome
        }

        public LumberjackState state = LumberjackState.Idle; // Holds the current farmer task
        private Vector2 curTreeLocation; // Store the current tree location
        private Grid grid; // Reference to the grid of nodes of water
        private BasicTilemap bm; // the building map that is passed in from the constructor

        private double ChoppingCoolDown = 0; // the time since last chop from the lumberjack
        int ResourcesHeld = 0;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="pos">the farmers starting location</param>
        /// <param name="g">the grid of water so they know where not to move</param>
        /// <param name="c">the content manager</param>
        /// <param name="bm">the buildingtile map</param>
        public Lumberjack(Vector2 pos, Grid g, ContentManager c, BasicTilemap bm)
        {
            grid = g;
            Position = new Vector2(pos.X * 32, pos.Y * 32);
            this.bm = bm;
            FindHome();
            texture = c.Load<Texture2D>("WoodChopper");
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

        /// <summary>
        /// updates the animation for the farmer
        /// </summary>
        /// <param name="gT">the game time</param>
        private void UpdateAnimation(GameTime gT)
        {
            animationTimer += gT.ElapsedGameTime.TotalSeconds;
            if(animationTimer > 0.30f)
            {
                switch (state)
                {
                    case LumberjackState.Idle:
                        //animationFrame = 0;
                        break;
                    case LumberjackState.ChoppingWood:
                        if (animationFrame < 3) animationFrame = 3;
                        else if (animationFrame == 5) animationFrame = 3;
                        else animationFrame++;
                        break;
                    case LumberjackState.ReturningHome:
                        if (animationFrame == 1) animationFrame = 2;
                        else if (animationFrame == 2) animationFrame = 1;
                        else animationFrame = 1;
                        break;
                    case LumberjackState.GoingToTree:
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
        public new void Update(GameTime gT, BasicTilemap Tm, out int NewResources)
        {
            NewResources = 0;
            switch (state)
            {
                case LumberjackState.Idle:
                    // Find the next farm location and update state to "Farming"
                    curTreeLocation = FindNextTreeLocation(Tm);
                    dest = curTreeLocation;
                    if (home == curTreeLocation) curTreeLocation = Vector2.Zero;
                    UpdateAnimation(gT);
                    if (Position != curTreeLocation) state = LumberjackState.GoingToTree;
                    else if (curTreeLocation == Position)state = LumberjackState.ChoppingWood;
                    break;

                case LumberjackState.ChoppingWood:
                    // Farm at the current location and update grid
                    ChoppingCoolDown += gT.ElapsedGameTime.TotalSeconds;
                    UpdateAnimation(gT);
                    if(ChoppingCoolDown > 3.0f)
                    {
                        ChopTile(curTreeLocation, Tm);
                        state = LumberjackState.ReturningHome;
                        ChoppingCoolDown = 0;
                        ResourcesHeld = 3;
                    }
                    break;

                case LumberjackState.ReturningHome:
                    // Move back to the home location
                    if (home != Vector2.Zero)
                    {
                        dest = home;
                        base.Update(gT, Tm, grid);
                        UpdateAnimation(gT);
                        // If the farmer reaches home, update state to "Idle"
                        if (Position == home)
                        {
                            state = LumberjackState.Idle;
                            if (CheckHome())
                            {
                                NewResources += ResourcesHeld;
                                ResourcesHeld = 0;
                            }
                            else
                            {
                                home = Vector2.Zero;
                                FindHome();
                            }
                        }
                    }
                    else{FindHome();}
                    
                    break;
                case LumberjackState.GoingToTree:
                    UpdateAnimation(gT);
                    if (curTreeLocation != Position) base.Update(gT, Tm, grid);
                    else if(curTreeLocation == Position) { state = LumberjackState.ChoppingWood; }
                    else
                    {
                        state = LumberjackState.Idle;
                    }
                    break;
            
            }
        }

        private bool CheckHome()
        {
            int holdx = (int)home.X / 32;
            int holdy = (int)home.Y / 32;

            if (bm.TileIndices[(holdy*bm.MapWidth) +holdx] == 21) return true;
            else return false;
        }

        /// <summary>
        /// finds the closest farm to the farmer
        /// </summary>
        /// <param name="Tm">the building tile map</param>
        /// <returns>returns the position in a vector2</returns>
        private Vector2 FindNextTreeLocation(BasicTilemap Tm)
        {
            // Use the Pathfinding class to find the next farmable location
            // You'll need to convert the position of the farmer to grid coordinates
            // and also convert the farm locations to grid coordinates if they aren't already
            // Then, you can call the FindPath method to find a path to the nearest farm spot

            int farmerGridX = (int)(Position.X / Tm.TileWidth);
            int farmerGridY = (int)(Position.Y / Tm.TileHeight);

            // Assuming you have a list of farmable locations, convert them to grid coordinates
            // or use grid coordinates directly if they are in grid coordinates
            List<Vector2> trees = new List<Vector2>();
            
            for(int i = 0; i < Tm.TileIndices.Length-1; i++)
            {
                if(Tm.TileIndices[i] == 22)
                {
                    int row = ((i / Tm.MapWidth));
                    int col = ((i % Tm.MapWidth));
                    trees.Add(new Vector2(col, row));
                }
            }


            Node startNode = grid.Nodes[farmerGridX, farmerGridY];
            Node nearestTreeNode = null;
            int shortestPathLength = int.MaxValue;

            foreach (var treeLocation in trees)
            {
                int treeGridX = (int)(treeLocation.X);
                int treeGridY = (int)(treeLocation.Y);
                Node endNode = grid.Nodes[treeGridX, treeGridY];
                

                List<Node> path = Pathfinding.FindPath(grid.Nodes, startNode, endNode);

                if (path != null && path.Count < shortestPathLength)
                {
                    nearestTreeNode = endNode;
                    shortestPathLength = path.Count;
                }
            }

            if (nearestTreeNode != null)
            {
                return new Vector2(nearestTreeNode.X * Tm.TileWidth, nearestTreeNode.Y * Tm.TileHeight);
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
        private void ChopTile(Vector2 location, BasicTilemap bm)
        {
            int tilex = (int)location.X / 32;
            int tiley = (int)location.Y / 32;
            if(bm.TileIndices[tiley * bm.MapWidth + tilex] == 22)
            {
                bm.TileIndices[tiley * bm.MapWidth + tilex] = 23;
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

        /// <summary>
        /// draws the farmer
        /// </summary>
        /// <param name="s">spritebatch</param>
        /// <param name="gT">game time</param>
        public override void Draw(SpriteBatch s, GameTime gT)
        {
            s.Draw(texture, new Vector2(Position.X-2, Position.Y), new Rectangle(animationFrame*size,0,size,size), Color.White, 0, new Vector2(0, 0), 1.25f, SpriteEffects.None, 0);
        }
    }

}
