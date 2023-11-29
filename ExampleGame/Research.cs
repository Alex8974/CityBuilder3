using ExampleGame.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpDX.Direct2D1.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExampleGame
{
    /// <summary>
    /// a class defining the amount of research the player has done
    /// </summary>
    public class Research
    {
        public bool PlanterResearch = false;

        float _scaleX = 2.0f;
        float _scaleY = 1.0f;

        public void Update(GameTime gT, KeyboardState curk, KeyboardState prevk, ref int TotalFood, ref int TotalWood)
        {
            if (curk.IsKeyDown(Keys.T) && !prevk.IsKeyDown(Keys.T) && TotalFood >= 50 && TotalWood >= 50) 
            {
                IncreaseHouseSize();
                TotalWood -= 50;
                TotalFood -= 50;
            }

            //if () PlanterResearch = true;
        }

        private void IncreaseHouseSize()
        {
            BuildingScreen.houseCapacity++;
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D boxTexture, SpriteFont font, Vector2 size)
        {
            spriteBatch.DrawString(font, "Research", new Vector2(0 * 42 + 13, 10 + 180) + size, Color.Black, 0f, new Vector2(0, 0), 0.40f, SpriteEffects.None, 0);

            spriteBatch.Draw(boxTexture, new Vector2(0 * 42 + 10, 10 + 200), new Rectangle(0 * 32, 0 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), new Vector2(_scaleX, _scaleY), SpriteEffects.None, 0);
            spriteBatch.DrawString(font, "+1 House" , new Vector2(0 * 42 + 13, 10 + 200) + size, Color.White, 0f, new Vector2(0, 0), 0.40f, SpriteEffects.None, 0);
            spriteBatch.DrawString(font, "Capacity" , new Vector2(0 * 42 + 13, 20 + 200) + size, Color.White, 0f, new Vector2(0, 0), 0.40f, SpriteEffects.None, 0);
            spriteBatch.DrawString(font, $"50 food ", new Vector2(0 * 42 + 10, 45 + 200), Color.Black, 0, new Vector2(0, 0), 0.40f, SpriteEffects.None, 0);
            spriteBatch.DrawString(font, $"50 wood ", new Vector2(0 * 42 + 10, 55 + 200), Color.Black, 0, new Vector2(0, 0), 0.40f, SpriteEffects.None, 0);
            spriteBatch.DrawString(font, $"'T'", new Vector2(0 * 42 + 80, 30 + 200), Color.Black, 0, new Vector2(0, 0), 0.40f, SpriteEffects.None, 0);
            spriteBatch.DrawString(font, $"Press", new Vector2(0 * 42 + 75, 15 + 200), Color.Black, 0, new Vector2(0, 0), 0.40f, SpriteEffects.None, 0);

            spriteBatch.Draw(boxTexture, new Vector2(0 * 42 + 10, 10 + 260), new Rectangle(0 * 32, 0 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), new Vector2(_scaleX, _scaleY), SpriteEffects.None, 0);
            spriteBatch.DrawString(font, "+1 House", new Vector2(0 * 42 + 13, 10 + 260) + size, Color.White, 0f, new Vector2(0, 0), 0.40f, SpriteEffects.None, 0);
            spriteBatch.DrawString(font, "Capacity", new Vector2(0 * 42 + 13, 20 + 260) + size, Color.White, 0f, new Vector2(0, 0), 0.40f, SpriteEffects.None, 0);
            spriteBatch.DrawString(font, $"50 food ", new Vector2(0 * 42 + 10, 45 + 260), Color.Black, 0, new Vector2(0, 0), 0.40f, SpriteEffects.None, 0);
            spriteBatch.DrawString(font, $"50 wood ", new Vector2(0 * 42 + 10, 55 + 260), Color.Black, 0, new Vector2(0, 0), 0.40f, SpriteEffects.None, 0);


        }



    }
}
