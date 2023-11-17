﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExampleGame
{
    public class House
    {
        public int Occupants;
        public int Capacity;
        public bool Full;
        public Vector2 Position;

        /// <summary>
        /// the house constructor
        /// </summary>
        /// <param name="pos">the position of the house in small number</param>
        /// <param name="c">the capacity of the house</param>
        public House(Vector2 pos, int c)
        {
            Capacity = c;
            Full = false;
            Position = pos;
        }


        public void Draw(SpriteBatch sb, SpriteFont f)
        {
            if(Occupants != Capacity)
            {
                sb.DrawString(f, $"{Occupants}/{Capacity}", new Vector2(Position.Y * 32, Position.X * 32), Color.Black, 0, new Vector2(0, 0), 0.5f, SpriteEffects.None, 0);

            }
        }
    }
}
