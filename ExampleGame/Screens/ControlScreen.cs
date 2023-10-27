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
            if (kbs.IsKeyDown(Keys.Enter) && prevkbs.IsKeyDown(Keys.Enter)) { return GameScreens.Start; }
            else return GameScreens.Controls;
        }

        public void Draw(SpriteBatch sb, SpriteFont f)
        {
            sb.DrawString(f, "Game Controls", new Vector2(150, 50), Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
            sb.DrawString(f, "Press 'b' to switch between building and moving", new Vector2(150, 80), Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
            sb.DrawString(f, "Use the 'Q' and 'E' keys to toggle building options", new Vector2(150, 110), Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
            sb.DrawString(f, "While in building mode press 'p' to place farmer", new Vector2(150, 140), Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
            sb.DrawString(f, "Use the arrow keys to move the screen", new Vector2(150, 170), Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
            sb.DrawString(f, "", new Vector2(150, 200), Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
            sb.DrawString(f, "", new Vector2(150, 230), Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);

        }
    }
}
