using ExampleGame.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExampleGame.Screens
{
    public class ControlScreen
    {
        public ControlScreen()
        {

        }

        public GameScreens Update(GameTime gT, KeyboardState kbs, KeyboardState prevkbs)
        {
            if (kbs.IsKeyDown(Keys.Enter) && !prevkbs.IsKeyDown(Keys.Enter)) { return GameScreens.Start; }
            else return GameScreens.Controls;
        }

        public void Draw(SpriteBatch sb, SpriteFont f)
        {
            sb.DrawString(f, "Game Controls", new Vector2(150, 50), Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
            sb.DrawString(f, "-Press 'b' to switch between building and moving", new Vector2(20, 80), Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
            sb.DrawString(f, "-Use the 'q' and 'e' keys to toggle building options", new Vector2(40, 110), Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
            sb.DrawString(f, "-Press the letter under the person to place them" , new Vector2(40, 140), Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
            sb.DrawString(f, "at your mouse position", new Vector2(60, 170), Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
            sb.DrawString(f, "-Use the arrow keys or 'wasd' to move the screen", new Vector2(20, 200), Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
            sb.DrawString(f, "-when exiting press the 'esc' key to save your data", new Vector2(50, 230), Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);


            
        }
    }
}
