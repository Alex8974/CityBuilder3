using CityBuilderGame;
using ExampleGame.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MessageBox = System.Windows.Forms.MessageBox;



namespace ExampleGame.Screens
{
    public class BuildingScreen
    {
        BuildingOptions buildingOptions;
        List<Farmer> farmers;
        List<Lumberjack> lumberjacks;
        List<House> housesss;
        List<Planter> planters;
        ContentManager content;
        Texture2D t;
        Texture2D boxTexture;
        Farmer ff;
        Lumberjack ll;
        public static int houseCapacity = 1;
        Research research;


        bool deleteMode = false;

        /// <summary>
        /// creates the building screen
        /// </summary>
        /// <param name="f">the list of famres</param>
        /// <param name="l">the list of lumberjacks</param>
        /// <param name="c">the content manager</param>
        public BuildingScreen(List<Farmer> f, List<Lumberjack> l, ContentManager c, BasicTilemap bm, List<House> h, Research r, List<Planter> p)
        {
            research = r;
            content = c;
            buildingOptions = (BuildingOptions) 20;
            farmers = f;
            lumberjacks = l;
            housesss = h;
            planters = p;
            t = c.Load<Texture2D>("tilemapCastleGame1.4");
            boxTexture = c.Load<Texture2D>("SelectionBox");
            ff = new Farmer(new Vector2(100, 100), c, bm, housesss);
            ll = new Lumberjack(new Vector2(100, 100),null, c, bm, housesss);
        }

        public void Initilize()
        {

        }

        public ClickState Update(GameTime gameTime, MouseState ms, BasicTilemap bm, KeyboardState kbs, KeyboardState prevkbs, Camera c, GraphicsDevice d, Grid g,ref int TotalFood,ref int TotalWood)
        {
            research.Update(gameTime, kbs, prevkbs,ref TotalFood,ref TotalWood);
            int mx = (ms.Position.X + (int)c.Position.X - d.Viewport.Width / 2) / bm.TileWidth;
            int my = (ms.Position.Y + (int)c.Position.Y - d.Viewport.Height / 2) / bm.TileHeight;

            if(mx >=0 && mx <= bm.MapWidth)
            {
                if(my >= 0 && my <= bm.MapHeight)
                {
                    if (kbs.IsKeyDown(Keys.F) && !prevkbs.IsKeyDown(Keys.F)) farmers.Add(new Farmer(new Vector2(mx, my), g, content, bm, housesss));
                    if (kbs.IsKeyDown(Keys.C) && !prevkbs.IsKeyDown(Keys.C)) lumberjacks.Add(new Lumberjack(new Vector2(mx, my), g, content, bm, housesss));
                    if (kbs.IsKeyDown(Keys.R) && !prevkbs.IsKeyDown(Keys.R) && research.PlanterResearch == true) planters.Add(new Planter(new Vector2(mx, my), g, content, bm, housesss, false));

                }
            }
            

            if (kbs.IsKeyDown(Keys.X) && !prevkbs.IsKeyDown(Keys.X)) deleteMode = !deleteMode;
            if (deleteMode == false)
            {

                #region cycles through the building options
                if (kbs.IsKeyDown(Keys.E) && prevkbs.IsKeyUp(Keys.E)) 
                { 
                    buildingOptions++;
                    if (!Enum.IsDefined(typeof(BuildingOptions), buildingOptions)) { buildingOptions--;}
                    if (buildingOptions == BuildingOptions.RedHouse || buildingOptions == BuildingOptions.ChoppedTree) buildingOptions++;
                    if (buildingOptions > (BuildingOptions)24 && buildingOptions < (BuildingOptions)30) buildingOptions = (BuildingOptions)30;
                }
                if (kbs.IsKeyDown(Keys.Q) && prevkbs.IsKeyUp(Keys.Q))
                {
                    buildingOptions--;
                    if (!Enum.IsDefined(typeof(BuildingOptions), buildingOptions)) { buildingOptions++; }
                    if (buildingOptions == BuildingOptions.RedHouse || buildingOptions == BuildingOptions.ChoppedTree) buildingOptions--;
                    if (buildingOptions > (BuildingOptions)24 && buildingOptions < (BuildingOptions)30) buildingOptions = (BuildingOptions)24;
                }
                #endregion

                #region get the mouse position and build
                try
                {
                    if (ms.LeftButton == ButtonState.Pressed && bm.TileIndices[(my * bm.MapWidth) + mx] == 0)
                    {
                        switch (buildingOptions)
                        {
                            case BuildingOptions.FenceFront:
                            case BuildingOptions.FenceBottomRightCorner:
                            case BuildingOptions.FenceUpRightSide:
                            case BuildingOptions.FenceBottomLeftCorner:
                            case BuildingOptions.FenceUpLeftSide:
                                TotalWood--;
                                if (TotalWood >= 0) bm.TileIndices[(my * bm.MapWidth) + mx] = (int)buildingOptions;
                                else TotalWood++;
                                break;
                            case BuildingOptions.BlueHouse:
                                TotalWood -= 3;
                                if (TotalWood >= 0)
                                {
                                    bm.TileIndices[(my * bm.MapWidth) + mx] = (int)buildingOptions;
                                    var h = new House(new Vector2(my, mx), houseCapacity);
                                    housesss.Add(h);
                                }

                                else TotalWood += 3;
                                break;
                            case BuildingOptions.Tree:
                                TotalFood--;
                                if (TotalFood >= 0) bm.TileIndices[(my * bm.MapWidth) + mx] = (int)buildingOptions;
                                else TotalFood++;
                                break;
                            case BuildingOptions.FullCrops:
                                TotalWood -= 2;
                                if (TotalWood >= 0) bm.TileIndices[(my * bm.MapWidth) + mx] = (int)buildingOptions;
                                else TotalWood += 2;
                                break;
                            case BuildingOptions.CropsEmpty:
                                TotalWood--;
                                if (TotalWood >= 0) bm.TileIndices[(my * bm.MapWidth) + mx] = (int)buildingOptions;
                                else TotalWood++;
                                break;
                            default:
                                MessageBox.Show("building not found");
                                break;
                        }                        
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
                        if(bm.TileIndices[(my * bm.MapWidth) + mx] == (int)BuildingOptions.BlueHouse || bm.TileIndices[(my * bm.MapWidth) + mx] == (int)BuildingOptions.RedHouse)
                        {
                            House hold = null;
                            foreach(House h in housesss)
                            {
                                if (h.Position == new Vector2(my, mx)) hold = h;
                            }
                            housesss.Remove(hold);
                        }
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
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, SpriteFont font, Research r)
        {            
            if(!deleteMode)spriteBatch.DrawString(font, $"Click to build", new Vector2(280, 100), Color.Black, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
            else spriteBatch.DrawString(font, "Click to Delete Item", new Vector2(250, 100), Color.DarkRed, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);

            #region the building options and selection

            Vector2 size = new Vector2(4,4);
            switch (buildingOptions)
            {
                case BuildingOptions.FenceFront:
                    #region front fence
                    spriteBatch.Draw(boxTexture, new Vector2(0 * 42 + 10, 10), new Rectangle(0 * 32, 1 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(t, new Vector2(0 * 42 + 13, 10), new Rectangle(4 * 32, 2 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    spriteBatch.DrawString(font, $"1 Wood ", new Vector2(10, 45), Color.Black, 0, new Vector2(0, 0), 0.40f, SpriteEffects.None, 0);
                    spriteBatch.Draw(boxTexture, new Vector2(1 * 42 + 10, 10), new Rectangle(0 * 32, 0 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(t, new Vector2(1 * 42 + 10, 10)+size, new Rectangle(0, 3 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    spriteBatch.Draw(boxTexture, new Vector2(2 * 42 + 10, 10), new Rectangle(0 * 32, 0 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(t, new Vector2(2 * 42 + 10, 10) + size, new Rectangle(1 * 32, 3 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    spriteBatch.Draw(boxTexture, new Vector2(3 * 42 + 10, 10), new Rectangle(0 * 32, 0 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(t, new Vector2(3 * 42 + 10, 10) + size, new Rectangle(2 * 32, 3 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    spriteBatch.Draw(boxTexture, new Vector2(4 * 42 + 10, 10), new Rectangle(0 * 32, 0 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(t, new Vector2(4 * 42 + 10, 10) + size, new Rectangle(3 * 32, 3 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    spriteBatch.Draw(boxTexture, new Vector2(5 * 42 + 10, 10), new Rectangle(0 * 32, 0 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(t, new Vector2(5 * 42 + 10, 10) + size, new Rectangle(4 * 32, 3 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    spriteBatch.Draw(boxTexture, new Vector2(6 * 42 + 10, 10), new Rectangle(0 * 32, 0 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(t, new Vector2(6 * 42 + 10, 10) + size, new Rectangle(1 * 32, 4 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    spriteBatch.Draw(boxTexture, new Vector2(7 * 42 + 10, 10), new Rectangle(0 * 32, 0 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(t, new Vector2(7 * 42 + 10, 10) + size, new Rectangle(3 * 32, 4 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    spriteBatch.Draw(boxTexture, new Vector2(8 * 42 + 10, 10), new Rectangle(0 * 32, 0 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(t, new Vector2(8 * 42 + 10, 10) + size, new Rectangle(4 * 32, 6 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    #endregion
                    break;
                case BuildingOptions.FenceBottomRightCorner:
                    #region bottom Right fence
                    spriteBatch.Draw(boxTexture, new Vector2(0 * 42 + 10, 10), new Rectangle(0 * 32, 0 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(t, new Vector2(0 * 42 + 13, 10) + size, new Rectangle(4 * 32, 2 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    spriteBatch.Draw(boxTexture, new Vector2(1 * 42 + 10, 10), new Rectangle(0 * 32, 1 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(t, new Vector2(1 * 42 + 10, 10) + size, new Rectangle(0, 3 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    spriteBatch.DrawString(font, $"1 Wood ", new Vector2(1*42+10, 45), Color.Black, 0, new Vector2(0, 0), 0.40f, SpriteEffects.None, 0);
                    spriteBatch.Draw(boxTexture, new Vector2(2 * 42 + 10, 10), new Rectangle(0 * 32, 0 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(t, new Vector2(2 * 42 + 10, 10) + size, new Rectangle(1 * 32, 3 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    spriteBatch.Draw(boxTexture, new Vector2(3 * 42 + 10, 10), new Rectangle(0 * 32, 0 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(t, new Vector2(3 * 42 + 10, 10) + size, new Rectangle(2 * 32, 3 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    spriteBatch.Draw(boxTexture, new Vector2(4 * 42 + 10, 10), new Rectangle(0 * 32, 0 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(t, new Vector2(4 * 42 + 10, 10) + size, new Rectangle(3 * 32, 3 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    spriteBatch.Draw(boxTexture, new Vector2(5 * 42 + 10, 10), new Rectangle(0 * 32, 0 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(t, new Vector2(5 * 42 + 10, 10) + size, new Rectangle(4 * 32, 3 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    spriteBatch.Draw(boxTexture, new Vector2(6 * 42 + 10, 10), new Rectangle(0 * 32, 0 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(t, new Vector2(6 * 42 + 10, 10) + size, new Rectangle(1 * 32, 4 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    spriteBatch.Draw(boxTexture, new Vector2(7 * 42 + 10, 10), new Rectangle(0 * 32, 0 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(t, new Vector2(7 * 42 + 10, 10) + size, new Rectangle(3 * 32, 4 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    spriteBatch.Draw(boxTexture, new Vector2(8 * 42 + 10, 10), new Rectangle(0 * 32, 0 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(t, new Vector2(8 * 42 + 10, 10) + size, new Rectangle(4 * 32, 6 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    #endregion
                    break;
                case BuildingOptions.FenceUpRightSide:
                    spriteBatch.Draw(boxTexture, new Vector2(0 * 42 + 10, 10), new Rectangle(0 * 32, 0 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(t, new Vector2(0 * 42 + 13, 10) + size, new Rectangle(4 * 32, 2 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    spriteBatch.Draw(boxTexture, new Vector2(1 * 42 + 10, 10), new Rectangle(0 * 32, 0 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(t, new Vector2(1 * 42 + 10, 10) + size, new Rectangle(0, 3 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    spriteBatch.Draw(boxTexture, new Vector2(2 * 42 + 10, 10), new Rectangle(0 * 32, 1 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(t, new Vector2(2 * 42 + 10, 10) + size, new Rectangle(1 * 32, 3 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    spriteBatch.DrawString(font, $"1 Wood ", new Vector2(2 * 42 + 10, 45), Color.Black, 0, new Vector2(0, 0), 0.40f, SpriteEffects.None, 0);
                    spriteBatch.Draw(boxTexture, new Vector2(3 * 42 + 10, 10), new Rectangle(0 * 32, 0 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(t, new Vector2(3 * 42 + 10, 10) + size, new Rectangle(2 * 32, 3 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    spriteBatch.Draw(boxTexture, new Vector2(4 * 42 + 10, 10), new Rectangle(0 * 32, 0 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(t, new Vector2(4 * 42 + 10, 10) + size, new Rectangle(3 * 32, 3 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    spriteBatch.Draw(boxTexture, new Vector2(5 * 42 + 10, 10), new Rectangle(0 * 32, 0 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(t, new Vector2(5 * 42 + 10, 10) + size, new Rectangle(4 * 32, 3 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    spriteBatch.Draw(boxTexture, new Vector2(6 * 42 + 10, 10), new Rectangle(0 * 32, 0 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(t, new Vector2(6 * 42 + 10, 10) + size, new Rectangle(1 * 32, 4 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    spriteBatch.Draw(boxTexture, new Vector2(7 * 42 + 10, 10), new Rectangle(0 * 32, 0 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(t, new Vector2(7 * 42 + 10, 10) + size, new Rectangle(3 * 32, 4 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    spriteBatch.Draw(boxTexture, new Vector2(8 * 42 + 10, 10), new Rectangle(0 * 32, 0 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(t, new Vector2(8 * 42 + 10, 10) + size, new Rectangle(4 * 32, 6 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    break;
                case BuildingOptions.FenceBottomLeftCorner:
                    spriteBatch.Draw(boxTexture, new Vector2(0 * 42 + 10, 10), new Rectangle(0 * 32, 0 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(t, new Vector2(0 * 42 + 13, 10) + size, new Rectangle(4 * 32, 2 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    spriteBatch.Draw(boxTexture, new Vector2(1 * 42 + 10, 10), new Rectangle(0 * 32, 0 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(t, new Vector2(1 * 42 + 10, 10) + size, new Rectangle(0, 3 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    spriteBatch.Draw(boxTexture, new Vector2(2 * 42 + 10, 10), new Rectangle(0 * 32, 0 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(t, new Vector2(2 * 42 + 10, 10) + size, new Rectangle(1 * 32, 3 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    spriteBatch.Draw(boxTexture, new Vector2(3 * 42 + 10, 10), new Rectangle(0 * 32, 1 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(t, new Vector2(3 * 42 + 10, 10) + size, new Rectangle(2 * 32, 3 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    spriteBatch.DrawString(font, $"1 Wood ", new Vector2(3 * 42 + 10, 45), Color.Black, 0, new Vector2(0, 0), 0.40f, SpriteEffects.None, 0);
                    spriteBatch.Draw(boxTexture, new Vector2(4 * 42 + 10, 10), new Rectangle(0 * 32, 0 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(t, new Vector2(4 * 42 + 10, 10) + size, new Rectangle(3 * 32, 3 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    spriteBatch.Draw(boxTexture, new Vector2(5 * 42 + 10, 10), new Rectangle(0 * 32, 0 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(t, new Vector2(5 * 42 + 10, 10) + size, new Rectangle(4 * 32, 3 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    spriteBatch.Draw(boxTexture, new Vector2(6 * 42 + 10, 10), new Rectangle(0 * 32, 0 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(t, new Vector2(6 * 42 + 10, 10) + size, new Rectangle(1 * 32, 4 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    spriteBatch.Draw(boxTexture, new Vector2(7 * 42 + 10, 10), new Rectangle(0 * 32, 0 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(t, new Vector2(7 * 42 + 10, 10) + size, new Rectangle(3 * 32, 4 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    spriteBatch.Draw(boxTexture, new Vector2(8 * 42 + 10, 10), new Rectangle(0 * 32, 0 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(t, new Vector2(8 * 42 + 10, 10) + size, new Rectangle(4 * 32, 6 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    break;
                case BuildingOptions.FenceUpLeftSide:
                    spriteBatch.Draw(boxTexture, new Vector2(0 * 42 + 10, 10), new Rectangle(0 * 32, 0 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(t, new Vector2(0 * 42 + 13, 10) + size, new Rectangle(4 * 32, 2 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    spriteBatch.Draw(boxTexture, new Vector2(1 * 42 + 10, 10), new Rectangle(0 * 32, 0 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(t, new Vector2(1 * 42 + 10, 10) + size, new Rectangle(0, 3 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    spriteBatch.Draw(boxTexture, new Vector2(2 * 42 + 10, 10), new Rectangle(0 * 32, 0 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(t, new Vector2(2 * 42 + 10, 10) + size, new Rectangle(1 * 32, 3 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    spriteBatch.Draw(boxTexture, new Vector2(3 * 42 + 10, 10), new Rectangle(0 * 32, 0 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(t, new Vector2(3 * 42 + 10, 10) + size, new Rectangle(2 * 32, 3 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    spriteBatch.Draw(boxTexture, new Vector2(4 * 42 + 10, 10), new Rectangle(0 * 32, 1 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(t, new Vector2(4 * 42 + 10, 10) + size, new Rectangle(3 * 32, 3 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    spriteBatch.DrawString(font, $"1 Wood ", new Vector2(4 * 42 + 10, 45), Color.Black, 0, new Vector2(0, 0), 0.40f, SpriteEffects.None, 0);
                    spriteBatch.Draw(boxTexture, new Vector2(5 * 42 + 10, 10), new Rectangle(0 * 32, 0 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(t, new Vector2(5 * 42 + 10, 10) + size, new Rectangle(4 * 32, 3 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    spriteBatch.Draw(boxTexture, new Vector2(6 * 42 + 10, 10), new Rectangle(0 * 32, 0 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(t, new Vector2(6 * 42 + 10, 10) + size, new Rectangle(1 * 32, 4 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    spriteBatch.Draw(boxTexture, new Vector2(7 * 42 + 10, 10), new Rectangle(0 * 32, 0 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(t, new Vector2(7 * 42 + 10, 10) + size, new Rectangle(3 * 32, 4 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    spriteBatch.Draw(boxTexture, new Vector2(8 * 42 + 10, 10), new Rectangle(0 * 32, 0 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(t, new Vector2(8 * 42 + 10, 10) + size, new Rectangle(4 * 32, 6 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    break;
                case BuildingOptions.BlueHouse:
                    spriteBatch.Draw(boxTexture, new Vector2(0 * 42 + 10, 10), new Rectangle(0 * 32, 0 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(t, new Vector2(0 * 42 + 13, 10) + size, new Rectangle(4 * 32, 2 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    spriteBatch.Draw(boxTexture, new Vector2(1 * 42 + 10, 10), new Rectangle(0 * 32, 0 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(t, new Vector2(1 * 42 + 10, 10) + size, new Rectangle(0, 3 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    spriteBatch.Draw(boxTexture, new Vector2(2 * 42 + 10, 10), new Rectangle(0 * 32, 0 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(t, new Vector2(2 * 42 + 10, 10) + size, new Rectangle(1 * 32, 3 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    spriteBatch.Draw(boxTexture, new Vector2(3 * 42 + 10, 10), new Rectangle(0 * 32, 0 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(t, new Vector2(3 * 42 + 10, 10) + size, new Rectangle(2 * 32, 3 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    spriteBatch.Draw(boxTexture, new Vector2(4 * 42 + 10, 10), new Rectangle(0 * 32, 0 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(t, new Vector2(4 * 42 + 10, 10) + size, new Rectangle(3 * 32, 3 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    spriteBatch.Draw(boxTexture, new Vector2(5 * 42 + 10, 10), new Rectangle(0 * 32, 1 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(t, new Vector2(5 * 42 + 10, 10) + size, new Rectangle(4 * 32, 3 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    spriteBatch.DrawString(font, $"3 Wood ", new Vector2(5 * 42 + 10, 45), Color.Black, 0, new Vector2(0, 0), 0.40f, SpriteEffects.None, 0);
                    spriteBatch.Draw(boxTexture, new Vector2(7 * 42 + 10, 10), new Rectangle(0 * 32, 0 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(t, new Vector2(7 * 42 + 10, 10) + size, new Rectangle(3 * 32, 4 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    spriteBatch.Draw(boxTexture, new Vector2(8 * 42 + 10, 10), new Rectangle(0 * 32, 0 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(t, new Vector2(8 * 42 + 10, 10) + size, new Rectangle(4 * 32, 6 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);

                    spriteBatch.Draw(boxTexture, new Vector2(6 * 42 + 10, 10), new Rectangle(0 * 32, 0 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(t, new Vector2(6 * 42 + 10, 10) + size, new Rectangle(1 * 32, 4 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);

                    break;
                case BuildingOptions.FullCrops:
                    spriteBatch.Draw(boxTexture, new Vector2(0 * 42 + 10, 10), new Rectangle(0 * 32, 0 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(t, new Vector2(0 * 42 + 13, 10) + size, new Rectangle(4 * 32, 2 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    spriteBatch.Draw(boxTexture, new Vector2(1 * 42 + 10, 10), new Rectangle(0 * 32, 0 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(t, new Vector2(1 * 42 + 10, 10) + size, new Rectangle(0, 3 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    spriteBatch.Draw(boxTexture, new Vector2(2 * 42 + 10, 10), new Rectangle(0 * 32, 0 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(t, new Vector2(2 * 42 + 10, 10) + size, new Rectangle(1 * 32, 3 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    spriteBatch.Draw(boxTexture, new Vector2(3 * 42 + 10, 10), new Rectangle(0 * 32, 0 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(t, new Vector2(3 * 42 + 10, 10) + size, new Rectangle(2 * 32, 3 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    spriteBatch.Draw(boxTexture, new Vector2(4 * 42 + 10, 10), new Rectangle(0 * 32, 0 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(t, new Vector2(4 * 42 + 10, 10) + size, new Rectangle(3 * 32, 3 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    spriteBatch.Draw(boxTexture, new Vector2(5 * 42 + 10, 10), new Rectangle(0 * 32, 0 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(t, new Vector2(5 * 42 + 10, 10) + size, new Rectangle(4 * 32, 3 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    spriteBatch.Draw(boxTexture, new Vector2(7 * 42 + 10, 10), new Rectangle(0 * 32, 1 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(t, new Vector2(7 * 42 + 10, 10) + size, new Rectangle(3 * 32, 4 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    spriteBatch.DrawString(font, $"2 Wood ", new Vector2(7 * 42 + 10, 45), Color.Black, 0, new Vector2(0, 0), 0.40f, SpriteEffects.None, 0);
                    spriteBatch.Draw(boxTexture, new Vector2(8 * 42 + 10, 10), new Rectangle(0 * 32, 0 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(t, new Vector2(8 * 42 + 10, 10) + size, new Rectangle(4 * 32, 6 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);

                    spriteBatch.Draw(boxTexture, new Vector2(6 * 42 + 10, 10), new Rectangle(0 * 32, 0 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(t, new Vector2(6 * 42 + 10, 10) + size, new Rectangle(1 * 32, 4 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);

                    break;
                case BuildingOptions.CropsEmpty:
                    spriteBatch.Draw(boxTexture, new Vector2(0 * 42 + 10, 10), new Rectangle(0 * 32, 0 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(t, new Vector2(0 * 42 + 13, 10) + size, new Rectangle(4 * 32, 2 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    spriteBatch.Draw(boxTexture, new Vector2(1 * 42 + 10, 10), new Rectangle(0 * 32, 0 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(t, new Vector2(1 * 42 + 10, 10) + size, new Rectangle(0, 3 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    spriteBatch.Draw(boxTexture, new Vector2(2 * 42 + 10, 10), new Rectangle(0 * 32, 0 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(t, new Vector2(2 * 42 + 10, 10) + size, new Rectangle(1 * 32, 3 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    spriteBatch.Draw(boxTexture, new Vector2(3 * 42 + 10, 10), new Rectangle(0 * 32, 0 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(t, new Vector2(3 * 42 + 10, 10) + size, new Rectangle(2 * 32, 3 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    spriteBatch.Draw(boxTexture, new Vector2(4 * 42 + 10, 10), new Rectangle(0 * 32, 0 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(t, new Vector2(4 * 42 + 10, 10) + size, new Rectangle(3 * 32, 3 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    spriteBatch.Draw(boxTexture, new Vector2(5 * 42 + 10, 10), new Rectangle(0 * 32, 0 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(t, new Vector2(5 * 42 + 10, 10) + size, new Rectangle(4 * 32, 3 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    spriteBatch.Draw(boxTexture, new Vector2(7 * 42 + 10, 10), new Rectangle(0 * 32, 0 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(t, new Vector2(7 * 42 + 10, 10) + size, new Rectangle(3 * 32, 4 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    spriteBatch.Draw(boxTexture, new Vector2(8 * 42 + 10, 10), new Rectangle(0 * 32, 1 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(t, new Vector2(8 * 42 + 10, 10) + size, new Rectangle(4 * 32, 6 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    spriteBatch.DrawString(font, $"1 Wood ", new Vector2(8 * 42 + 10, 45), Color.Black, 0, new Vector2(0, 0), 0.40f, SpriteEffects.None, 0);

                    spriteBatch.Draw(boxTexture, new Vector2(6 * 42 + 10, 10), new Rectangle(0 * 32, 0 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(t, new Vector2(6 * 42 + 10, 10) + size, new Rectangle(1 * 32, 4 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);

                    break;
                case BuildingOptions.Tree:
                    spriteBatch.Draw(boxTexture, new Vector2(0 * 42 + 10, 10), new Rectangle(0 * 32, 0 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(t, new Vector2(0 * 42 + 13, 10) + size, new Rectangle(4 * 32, 2 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    spriteBatch.Draw(boxTexture, new Vector2(1 * 42 + 10, 10), new Rectangle(0 * 32, 0 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(t, new Vector2(1 * 42 + 10, 10) + size, new Rectangle(0, 3 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    spriteBatch.Draw(boxTexture, new Vector2(2 * 42 + 10, 10), new Rectangle(0 * 32, 0 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(t, new Vector2(2 * 42 + 10, 10) + size, new Rectangle(1 * 32, 3 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    spriteBatch.Draw(boxTexture, new Vector2(3 * 42 + 10, 10), new Rectangle(0 * 32, 0 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(t, new Vector2(3 * 42 + 10, 10) + size, new Rectangle(2 * 32, 3 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    spriteBatch.Draw(boxTexture, new Vector2(4 * 42 + 10, 10), new Rectangle(0 * 32, 0 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(t, new Vector2(4 * 42 + 10, 10) + size, new Rectangle(3 * 32, 3 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    spriteBatch.Draw(boxTexture, new Vector2(5 * 42 + 10, 10), new Rectangle(0 * 32, 0 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(t, new Vector2(5 * 42 + 10, 10) + size, new Rectangle(4 * 32, 3 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    
                    spriteBatch.Draw(boxTexture, new Vector2(6 * 42 + 10, 10), new Rectangle(0 * 32, 1 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(t, new Vector2(6 * 42 + 10, 10) + size, new Rectangle(1 * 32, 4 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    spriteBatch.DrawString(font, $"1 Food ", new Vector2(6 * 42 + 10, 45), Color.Black, 0, new Vector2(0, 0), 0.40f, SpriteEffects.None, 0);
                    
                    spriteBatch.Draw(boxTexture, new Vector2(7 * 42 + 10, 10), new Rectangle(0 * 32, 0 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(t, new Vector2(7 * 42 + 10, 10) + size, new Rectangle(3 * 32, 4 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    spriteBatch.Draw(boxTexture, new Vector2(8 * 42 + 10, 10), new Rectangle(0 * 32, 0 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(t, new Vector2(8 * 42 + 10, 10) + size, new Rectangle(4 * 32, 6 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    break;
                default:
                    spriteBatch.Draw(boxTexture, new Vector2(0 * 42 + 10, 10), new Rectangle(0 * 32, 0 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(t, new Vector2(0 * 42 + 13, 10) + size, new Rectangle(4 * 32, 2 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    spriteBatch.Draw(boxTexture, new Vector2(1 * 42 + 10, 10), new Rectangle(0 * 32, 0 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(t, new Vector2(1 * 42 + 10, 10) + size, new Rectangle(0, 3 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    spriteBatch.Draw(boxTexture, new Vector2(2 * 42 + 10, 10), new Rectangle(0 * 32, 0 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(t, new Vector2(2 * 42 + 10, 10) + size, new Rectangle(1 * 32, 3 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    spriteBatch.Draw(boxTexture, new Vector2(3 * 42 + 10, 10), new Rectangle(0 * 32, 0 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(t, new Vector2(3 * 42 + 10, 10) + size, new Rectangle(2 * 32, 3 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    spriteBatch.Draw(boxTexture, new Vector2(4 * 42 + 10, 10), new Rectangle(0 * 32, 0 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(t, new Vector2(4 * 42 + 10, 10) + size, new Rectangle(3 * 32, 3 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    spriteBatch.Draw(boxTexture, new Vector2(5 * 42 + 10, 10), new Rectangle(0 * 32, 0 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(t, new Vector2(5 * 42 + 10, 10) + size, new Rectangle(4 * 32, 3 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    spriteBatch.Draw(boxTexture, new Vector2(6 * 42 + 10, 10), new Rectangle(0 * 32, 0 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(t, new Vector2(6 * 42 + 10, 10) + size, new Rectangle(3 * 32, 4 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    spriteBatch.Draw(boxTexture, new Vector2(7 * 42 + 10, 10), new Rectangle(0 * 32, 0 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(t, new Vector2(7 * 42 + 10, 10) + size, new Rectangle(4 * 32, 6 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                    break;
            }

            #endregion

            #region for the research options

            research.Draw(spriteBatch, boxTexture, font, size);

            #endregion


            spriteBatch.Draw(boxTexture, new Vector2(0 * 42 + 10, 32+30), new Rectangle(0 * 32, 0 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
            spriteBatch.Draw(ff.texture, new Vector2(0 * 42 + 10, 32 + 30), new Rectangle(0 * 32, 0 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
            spriteBatch.DrawString(font, "'F'", new Vector2(0 * 42 + 20, 100), Color.Black, 0, new Vector2(0, 0), 0.40f, SpriteEffects.None, 0);

            spriteBatch.Draw(boxTexture, new Vector2(1 * 42 + 10, 32 + 30), new Rectangle(0 * 32, 0 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
            spriteBatch.Draw(ll.texture, new Vector2(1 * 42 + 10, 32 + 30), new Rectangle(0 * 32, 0 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
            spriteBatch.DrawString(font, "'C'", new Vector2(1 * 42 + 20, 100), Color.Black, 0, new Vector2(0, 0), 0.40f, SpriteEffects.None, 0);

            if (r.PlanterResearch)
            {
                spriteBatch.Draw(boxTexture, new Vector2(2 * 42 + 10, 32 + 30), new Rectangle(0 * 32, 0 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                spriteBatch.Draw(ll.texture, new Vector2(2 * 42 + 10, 32 + 30), new Rectangle(0 * 32, 0 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                spriteBatch.DrawString(font, "'R'", new Vector2(2 * 42 + 20, 100), Color.Black, 0, new Vector2(0, 0), 0.40f, SpriteEffects.None, 0);
            }

            //spriteBatch.Draw(t, new Vector2(0, 0), source, Color.White, 0f, new Vector2(0, 0), SpriteEffects.None, 0);
        }
    }
}
