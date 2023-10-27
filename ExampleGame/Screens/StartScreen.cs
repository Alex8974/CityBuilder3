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
        private SelectedIndex selectedIndex = 0;
        public void Initilze(ContentManager c)
        {

        }

        public GameScreens Update(GameTime gT, KeyboardState keyboardState, KeyboardState prevkbs)
        {
            if (keyboardState.IsKeyDown(Keys.Up) && !prevkbs.IsKeyDown(Keys.Up))
            {
                selectedIndex++;
                if (!Enum.IsDefined(typeof(SelectedIndex), selectedIndex)) { selectedIndex = 0; }

            }
            else if (keyboardState.IsKeyDown(Keys.Down) && !prevkbs.IsKeyDown(Keys.Down))
            {
                selectedIndex--;
                if (!Enum.IsDefined(typeof(SelectedIndex), selectedIndex)) { selectedIndex = (SelectedIndex)1; }
            }
            if (keyboardState.IsKeyDown(Keys.Enter))
            {
                switch (selectedIndex)
                {
                    case SelectedIndex.StartGame:
                        return GameScreens.Running;
                    case SelectedIndex.Controls:
                        return GameScreens.Controls;
                    default:
                        return GameScreens.Start;
                }
            }
            return GameScreens.Start;
        }

        public void Draw(GameTime gameTime, SpriteBatch sb, SpriteFont f)
        {
            switch (selectedIndex)
            {
                case SelectedIndex.StartGame:
                    sb.DrawString(f, "City Builder", new Vector2(320, 120), Color.Black, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
                    sb.DrawString(f, "Bengin Game", new Vector2(280, 200), Color.Yellow, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
                    sb.DrawString(f, "View Controlls", new Vector2(250, 230), Color.Black, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    break;
                case SelectedIndex.Controls:
                    sb.DrawString(f, "City Builder", new Vector2(320, 120), Color.Black, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
                    sb.DrawString(f, "Bengin Game", new Vector2(280, 200), Color.Black, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
                    sb.DrawString(f, "View Controlls", new Vector2(250, 230), Color.Yellow, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    break;
                default:
                    sb.DrawString(f, "City Builder", new Vector2(320, 120), Color.Black, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
                    sb.DrawString(f, "Bengin Game", new Vector2(280, 200), Color.Black, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
                    sb.DrawString(f, "View Controlls", new Vector2(250, 230), Color.Black, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    break;
            }

            
        }
    }

    enum SelectedIndex
    {
        StartGame, 
        Controls
    }
}
