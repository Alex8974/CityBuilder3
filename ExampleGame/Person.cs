//using CollisionExample.Collisions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.MediaFoundation.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExampleGame
{
    public abstract class Person
    {
        List<Node> path;
        int currentNodeIndex = 0;

        public Vector2 home { get; set; }

        /// <summary>
        /// the time between each animaitons
        /// </summary>
        protected const float ANIMATION_TIMER = 0.1f;

        /// <summary>
        /// the size of the sprite
        /// </summary>
        protected int size = 32;

        protected const double WALKCOOLDOWN = 0.5f;
        protected double walktimer = 0;

        /// <summary>
        /// the frame the animaiton is on col
        /// </summary>
        protected int animationFrame = 0;

        /// <summary>
        /// the current time between the animations
        /// </summary>
        protected double animationTimer;

        /// <summary>
        /// the texture for the sprite
        /// </summary>
        public Texture2D texture;

        /// <summary>
        /// if the person is building
        /// </summary>
        protected bool building;

        /// <summary>
        /// 1 for the left other number for the right
        /// </summary>
        public int team { get; set; }

        /// <summary>
        /// the health of the person
        /// </summary>
        public int Health { get; set; } = 8;

        /// <summary>
        /// their movement spped
        /// </summary>
        public float Speed { get; set; }

        /// <summary>
        /// the position in the top left corner
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// the position to draw when moving
        /// </summary>
        public Vector2 DrawPosition { get; set; }

        /// <summary>
        /// the destination
        /// </summary>
        public Vector2 dest { get; set; }

        /// <summary>
        /// moves the person 
        /// </summary>
        /// <param name="gT">the current game time</param>
        public void Move(GameTime gT, BasicTilemap Tm)
        {


            if (Position != dest)
            {
                // Calculate the direction vector from the current position to the destination
                Vector2 direction = Vector2.Normalize(dest - Position);

                Speed = 32.0f;
                // Update the character's position based on its speed
                if (Math.Abs(direction.X) > Math.Abs(direction.Y))
                {
                    if (direction.X > 0) direction.X = 1;
                    else direction.X = -1;
                    direction.Y = 0;
                }
                else
                {
                    if (direction.Y > 0) direction.Y = 1;
                    else direction.Y = -1;
                    direction.X = 0;
                }

                // Update the character's position based on its speed
                Position += direction * Speed;
                DrawPosition = Position;

                // Handle collision detection and other logic as needed.
            }

        }
        public void Move(GameTime gT, int[] des, BasicTilemap Tm)
        {

            Vector2 vdes = new Vector2(des[0] * Tm.TileWidth, des[1] * Tm.TileHeight);
            // Calculate the direction vector from the current position to the destination
            Vector2 direction = Vector2.Normalize(vdes - Position);

            Speed = 32.0f;
            // Update the character's position based on its speed
            if (Math.Abs(direction.X) > Math.Abs(direction.Y))
            {
                if (direction.X > 0) direction.X = 1;
                else direction.X = -1;
                direction.Y = 0;
            }
            else
            {
                if (direction.Y > 0) direction.Y = 1;
                else direction.Y = -1;
                direction.X = 0;
            }

            // Update the character's position based on its speed
            Position += direction * Speed;
            DrawPosition = Position;
            // Handle collision detection and other logic as needed.
        }
        public void Move(GameTime gT, Vector2 des, BasicTilemap Tm)
        {

            Vector2 vdes = des;
            // Calculate the direction vector from the current position to the destination
            Vector2 direction = Vector2.Normalize(vdes - Position);

            Speed = 32.0f;
            // Update the character's position based on its speed
            if (Math.Abs(direction.X) > Math.Abs(direction.Y))
            {
                if (direction.X > 0) direction.X = 1;
                else direction.X = -1;
                direction.Y = 0;
            }
            else
            {
                if (direction.Y > 0) direction.Y = 1;
                else direction.Y = -1;
                direction.X = 0;
            }

            // Update the character's position based on its speed
            Position += direction * Speed;
            DrawPosition = Position;
            // Handle collision detection and other logic as needed.
        }

        /// <summary>
        /// makes the person attack 
        /// </summary>
        /// <param name="other">the person beign attacked</param>
        public abstract void Building();


        public abstract void CheckForBuild();

        /// <summary>
        /// updates the person 
        /// </summary>
        /// <param name="gT">the game time</param>
        public void Update(GameTime gT, BasicTilemap Tm, Grid g)
        {
            walktimer += gT.ElapsedGameTime.TotalSeconds;
            if (dest != Position)
            {
                if (path != null && currentNodeIndex < path.Count)
                {
                    // the grid cordinates that you want to move to
                    Node targetNode = path[currentNodeIndex];
                    // if they should move positions
                    if(walktimer > WALKCOOLDOWN)
                    {
                        if (this.Position == new Vector2(targetNode.X * Tm.TileWidth, targetNode.Y * Tm.TileHeight))
                        {
                            // Move to the next node in the path
                            currentNodeIndex++;
                        }
                        else
                        {
                            Move(gT, new int[] { targetNode.X, targetNode.Y }, Tm);
                            currentNodeIndex++;
                            walktimer = 0;
                        }
                    }
                    else if(walktimer < WALKCOOLDOWN && path != null)
                    {

                        Node nextnode = path[currentNodeIndex];
                        float percentage = (float)(walktimer / WALKCOOLDOWN);
                        DrawPosition = new Vector2(
                            MathHelper.Lerp(Position.X, nextnode.X * 32, (float)percentage),
                            MathHelper.Lerp(Position.Y, nextnode.Y * 32, (float)percentage)
                        );

                    }
                    // Check if the person has reached the target node
                   
                }
                else if (path != null && currentNodeIndex >= path.Count)
                {
                    currentNodeIndex = 0;
                    path = null;
                }
                else
                {
                    try
                    {
                        path = Pathfinding.FindPath(g.Nodes, g.Nodes[(int)Position.X / Tm.TileWidth, (int)Position.Y / Tm.TileHeight], g.Nodes[(int)dest.X / Tm.TileWidth, (int)dest.Y / Tm.TileHeight]);
                    }
                    catch
                    {
                        //MessageBox.Show("bad click");
                    }
                }
            }
        }

        /// <summary>
        /// finds the closest unocupied home and assigns it to the farmer
        /// </summary>
        protected void FindHome(List<House> h, BasicTilemap bm)
        {
            foreach (House house in h)
            {
                if (house.Occupants < house.Capacity)
                {
                    house.Occupants++;
                    if (house.Occupants == house.Capacity)
                    {
                        bm.TileIndices[(int)house.Position.X * bm.MapWidth + (int)house.Position.Y]++;
                    }
                    home = new Vector2(house.Position.Y * bm.TileWidth, house.Position.X * bm.TileHeight);
                    break;

                }
            }

            //for(int i = 0; i < bm.TileIndices.Length-1; i++)
            //{
            //    if (bm.TileIndices[i] == (int)BuildingOptions.BlueHouse)
            //    {
            //        bm.TileIndices[i]++;
            //        int row = (i / bm.MapWidth);
            //        int col = (i % bm.MapWidth);
            //        home = new Vector2(col * bm.TileWidth, row * bm.TileHeight);
            //        break;
            //    }
            //    else
            //    {
            //        home = Vector2.Zero;
            //    }
            //}
        }

        /// <summary>
        /// checks to see if the home still exists
        /// </summary>
        /// <param name="bm">the building map checking on </param>
        /// <returns>true if it still exists false if not</returns>
        protected bool CheckHome(BasicTilemap bm)
        {
            int holdx = (int)home.X / 32;
            int holdy = (int)home.Y / 32;

            if (bm.TileIndices[(holdy * bm.MapWidth) + holdx] == 21 || bm.TileIndices[(holdy * bm.MapWidth) + holdx] == 20) return true;
            else return false;
        }

        /// <summary>
        /// draws the person
        /// </summary>
        /// <param name="s">the spritebatch</param>
        /// <param name="gT">the game time</param>
        public abstract void Draw(SpriteBatch s, GameTime gT);

    }
}
