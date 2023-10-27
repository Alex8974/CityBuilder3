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

        protected Vector2 home;

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
            if (walktimer > WALKCOOLDOWN && dest != Position)
            {
                if (path != null && currentNodeIndex < path.Count)
                {
                    Node targetNode = path[currentNodeIndex];

                    // Check if the penguin has reached the target node
                    if (this.Position == new Vector2(targetNode.X*Tm.TileWidth, targetNode.Y*Tm.TileHeight))
                    {
                        // Move to the next node in the path
                        currentNodeIndex++;
                    }
                    else
                    {
                        // Use your custom Move method to move the penguin
                        Move(gT, new int[] { targetNode.X, targetNode.Y }, Tm);
                        walktimer = 0;
                    }
                }
                else if( path != null && currentNodeIndex >= path.Count)
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
        /// draws the person
        /// </summary>
        /// <param name="s">the spritebatch</param>
        /// <param name="gT">the game time</param>
        public abstract void Draw(SpriteBatch s, GameTime gT);

    }
}
