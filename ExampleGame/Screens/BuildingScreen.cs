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
    public class BuildingScreen
    {
        BuildingOptions buildingOptions;

        bool deleteMode = false;
        public BuildingScreen()
        {
            buildingOptions =(BuildingOptions) 20;
        }

        public void Initilize()
        {

        }

        public ClickState Update(GameTime gameTime, MouseState ms, BasicTilemap bm, KeyboardState kbs, KeyboardState prevkbs)
        {
            int mx = (int)ms.Position.X / 32;
            int my = (int)ms.Position.Y / 32;

            if (kbs.IsKeyDown(Keys.X) && !prevkbs.IsKeyDown(Keys.X)) deleteMode = !deleteMode;
            if (deleteMode == false)
            {
                if (kbs.IsKeyDown(Keys.E) && prevkbs.IsKeyUp(Keys.E)) buildingOptions++;
                if (kbs.IsKeyDown(Keys.Q) && prevkbs.IsKeyUp(Keys.Q)) buildingOptions--;

                #region get the mouse position and build

                
                try
                {
                    if (ms.LeftButton == ButtonState.Pressed && bm.TileIndices[(my * bm.MapWidth) + mx] == 0)
                    {
                        bm.TileIndices[(my * bm.MapWidth) + mx] = (int)buildingOptions;
                    }
                }
                catch
                {
                    System.Windows.Forms.MessageBox.Show("Bad Click");
                }
                #endregion
            }
            else if (deleteMode == true)
            {
                #region Delete Item
                try
                {
                    if (ms.LeftButton == ButtonState.Pressed)
                    {
                        bm.TileIndices[(my * bm.MapWidth) + mx] = 0;
                    }
                }
                catch
                {
                }
                #endregion
            }
            if (kbs.IsKeyDown(Keys.B) && !prevkbs.IsKeyDown(Keys.B)) return ClickState.Move;
            else return ClickState.Building;



        }
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, SpriteFont font)
        {
            if(!deleteMode)spriteBatch.DrawString(font, $"Click to build : {buildingOptions} ", new Vector2(280, 100), Color.Black, 0, new Vector2(0, 0), 2f, SpriteEffects.None, 0);
            else spriteBatch.DrawString(font, "Click to Delete Item", new Vector2(280, 100), Color.DarkRed, 0, new Vector2(0, 0), 2f, SpriteEffects.None, 0);
        }
    }
}
