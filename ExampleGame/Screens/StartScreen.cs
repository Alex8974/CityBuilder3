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
        private Texture2D backgoundtexture;
        public void Initilze(ContentManager c)
        {
            backgoundtexture = c.Load<Texture2D>("city");
        }

        public GameScreens Update(GameTime gT, KeyboardState keyboardState, KeyboardState prevkbs, TutorialScreen t, CityBuilderGame c)
        {
            if (keyboardState.IsKeyDown(Keys.Up) && !prevkbs.IsKeyDown(Keys.Up))
            {
                selectedIndex--;
                if (!Enum.IsDefined(typeof(SelectedIndex), selectedIndex)) { selectedIndex = 0; }

            }
            else if (keyboardState.IsKeyDown(Keys.Down) && !prevkbs.IsKeyDown(Keys.Down))
            {
                selectedIndex++;
                if (!Enum.IsDefined(typeof(SelectedIndex), selectedIndex)) { selectedIndex = (SelectedIndex)2; }
            }
            if (keyboardState.IsKeyDown(Keys.Enter) && !prevkbs.IsKeyDown(Keys.Enter))
            {
                switch (selectedIndex)
                {
                    case SelectedIndex.StartGame:
                        c.LoadMapContent();
                        return GameScreens.Running;
                    case SelectedIndex.Controls:
                        return GameScreens.Controls;
                    case SelectedIndex.Tutorial:
                        t.Initialize();
                        t.LoadContent(new List<House>());
                        return GameScreens.Tutorial;
                    default:
                        return GameScreens.Start;
                }
            }
            return GameScreens.Start;
        }

        public void Draw(GameTime gameTime, SpriteBatch sb, SpriteFont f)
        {
            sb.Draw(backgoundtexture, new Vector2(0, 0), null, Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);

            switch (selectedIndex)
            {

                case SelectedIndex.StartGame:
                    sb.DrawString(f, "City Builder", new Vector2(230, 120), Color.Black, 0, new Vector2(0, 0), 2f, SpriteEffects.None, 0);
                    sb.DrawString(f, "Bengin Game", new Vector2(280, 200), Color.Yellow, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
                    sb.DrawString(f, "View Controlls", new Vector2(260, 230), Color.Black, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    sb.DrawString(f, "Tutorial", new Vector2(300, 260), Color.Black, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    break;
                case SelectedIndex.Controls:
                    sb.DrawString(f, "City Builder", new Vector2(230, 120), Color.Black, 0, new Vector2(0, 0), 2f, SpriteEffects.None, 0);
                    sb.DrawString(f, "Bengin Game", new Vector2(280, 200), Color.Black, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
                    sb.DrawString(f, "View Controlls", new Vector2(260, 230), Color.Yellow, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    sb.DrawString(f, "Tutorial", new Vector2(300, 260), Color.Black, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    break;
                case SelectedIndex.Tutorial:
                    sb.DrawString(f, "City Builder", new Vector2(230, 120), Color.Black, 0, new Vector2(0, 0), 2f, SpriteEffects.None, 0);
                    sb.DrawString(f, "Bengin Game", new Vector2(280, 200), Color.Black, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
                    sb.DrawString(f, "View Controlls", new Vector2(260, 230), Color.Black, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    sb.DrawString(f, "Tutorial", new Vector2(300, 260), Color.Yellow, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    break;
                default:
                    sb.DrawString(f, "City Builder", new Vector2(230, 120), Color.Black, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
                    sb.DrawString(f, "Bengin Game", new Vector2(280, 200), Color.Black, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
                    sb.DrawString(f, "View Controlls", new Vector2(260, 230), Color.Black, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    sb.DrawString(f, "Tutorial", new Vector2(300, 230), Color.Black, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    break;
            }

            
        }
    }

    enum SelectedIndex
    {
        StartGame, 
        Controls,
        Tutorial
    }
}
