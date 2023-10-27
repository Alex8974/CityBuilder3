using CityBuilderGame;
using ExampleGame.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
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
        List<Farmer> farmers;
        ContentManager content;

        bool deleteMode = false;
        public BuildingScreen(List<Farmer> f, ContentManager c)
        {
            content = c;
            buildingOptions = (BuildingOptions) 20;
            farmers = f;
        }

        public void Initilize()
        {

        }

        public ClickState Update(GameTime gameTime, MouseState ms, BasicTilemap bm, KeyboardState kbs, KeyboardState prevkbs, Camera c, GraphicsDevice d, Grid g)
        {
            int mx = (ms.Position.X + (int)c.Position.X - d.Viewport.Width / 2) / bm.TileWidth;
            int my = (ms.Position.Y + (int)c.Position.Y - d.Viewport.Height / 2) / bm.TileHeight;

            if (kbs.IsKeyDown(Keys.P) && !prevkbs.IsKeyDown(Keys.P)) farmers.Add(new Farmer(new Vector2(mx, my), g, content, bm));

            if (kbs.IsKeyDown(Keys.X) && !prevkbs.IsKeyDown(Keys.X)) deleteMode = !deleteMode;
            if (deleteMode == false)
            {

                #region cycles through the building options
                if (kbs.IsKeyDown(Keys.E) && prevkbs.IsKeyUp(Keys.E)) 
                { 
                    buildingOptions++;
                    if (!Enum.IsDefined(typeof(BuildingOptions), buildingOptions)) { buildingOptions--;}
                    if (buildingOptions == BuildingOptions.RedHouse) buildingOptions++;
                    if (buildingOptions > (BuildingOptions)24 && buildingOptions < (BuildingOptions)30) buildingOptions = (BuildingOptions)30;
                }
                if (kbs.IsKeyDown(Keys.Q) && prevkbs.IsKeyUp(Keys.Q))
                {
                    buildingOptions--;
                    if (!Enum.IsDefined(typeof(BuildingOptions), buildingOptions)) { buildingOptions++; }
                    if (buildingOptions == BuildingOptions.RedHouse) buildingOptions--;
                    if (buildingOptions > (BuildingOptions)24 && buildingOptions < (BuildingOptions)30) buildingOptions = (BuildingOptions)24;
                }
                #endregion

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
                    //System.Windows.Forms.MessageBox.Show("Bad Click");
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
            if(!deleteMode)spriteBatch.DrawString(font, $"Click to build : {buildingOptions} ", new Vector2(280, 100), Color.Black, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
            else spriteBatch.DrawString(font, "Click to Delete Item", new Vector2(250, 100), Color.DarkRed, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
        }
    }
}
