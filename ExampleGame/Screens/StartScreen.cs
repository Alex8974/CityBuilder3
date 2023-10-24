using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExampleGame.Enums;

namespace ExampleGame.Screens
{
    public class StartScreen
    {

        public void Initilze(ContentManager c)
        {

        }

        public GameScreens Update(GameTime gT, KeyboardState keyboardState)
        {
            if (keyboardState.IsKeyDown(Keys.Enter)) return GameScreens.Running;
            else if (keyboardState.IsKeyDown(Keys.C)) return GameScreens.Controls;
            else return GameScreens.Start;
        }

        public void Draw(GameTime gameTime, SpriteBatch sb, SpriteFont f)
        {
            sb.DrawString(f, "City Builder", new Vector2(250, 150), Color.Black, 0, new Vector2(0, 0), 2f, SpriteEffects.None, 0);
            sb.DrawString(f, "Press 'Enter' to start", new Vector2(300, 200), Color.Black, 0, new Vector2(0, 0), 2f, SpriteEffects.None, 0);
            sb.DrawString(f, "Press 'C' to view controls", new Vector2(280, 220), Color.Black, 0, new Vector2(0, 0), 2f, SpriteEffects.None, 0);
        }
    }
}
