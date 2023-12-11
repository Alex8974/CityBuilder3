using ExampleGame.Enums;
using ExampleGame.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Linq;
using SharpDX.DirectWrite;
using System.IO;
using System;
using System.Drawing.Text;
using System.Windows.Forms;
using MessageBox = System.Windows.Forms.MessageBox;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;
using Keys = Microsoft.Xna.Framework.Input.Keys;
using CityBuilderGame;
using System.Collections.Generic;
using System.Xml.Schema;
using System.Diagnostics.Eventing.Reader;
using LightingAndCamerasExample;
using Basic3DExample;
using SharpDX.Direct2D1.Effects;
using ExampleGame.BiggerTileMapGenerator;
using Microsoft.Xna.Framework.Content;

namespace ExampleGame
{
    /// <summary>
    /// This game demonstrates the use of a Tilemap loaded through 
    /// the content pipeline with a custom importer and processor
    /// </summary>
    public class TutorialScreen
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private BasicTilemap _tilemap;
        private BasicTilemap buildingmapt;
        private TileMapGenerator generator;

        Grid grid;
        ClickState clickState;
        GameScreens gameScreens;
        BuildingScreen buildingScreen;
        Day days;

        private List<Farmer> farmers2;
        private List<Lumberjack> lumberjacks2;
        private List<Planter> planters;
        private List<House> houses;
        private int TotalFood = 10;
        private int TotalWood = 10;
        private Research research;


        ContentManager content;

        Texture2D pixel;


        public TutorialScreen(ContentManager c, SpriteFont font, GraphicsDeviceManager g, BasicTilemap land)
        {
            content = c;
            _graphics = g;
            _tilemap = land;
            gameScreens = GameScreens.Running;
            research = new Research();
        }

        public void Initialize()
        {
            // TODO: Add your initialization logic here
            _graphics.GraphicsDevice.Viewport = new Viewport(0, 0, 1600, 800);

            _graphics.PreferredBackBufferWidth = 1600;
            _graphics.PreferredBackBufferHeight = 800;
            _graphics.ApplyChanges();
            houses = new();
            farmers2 = new List<Farmer>();
            lumberjacks2 = new List<Lumberjack>();
            planters = new();
            days = new(content);
            buildingmapt = new();
        }

        /// <summary>
        /// loads the content of the game
        /// </summary>
        public void LoadContent(List<House> h)
        {
            _spriteBatch = new SpriteBatch(_graphics.GraphicsDevice);

            buildingmapt = content.Load<BasicTilemap>("map5t");

            grid = new Grid(_tilemap.MapWidth, _tilemap.MapHeight, _tilemap, 0);
            LoadGame();
            buildingScreen = new BuildingScreen(farmers2, lumberjacks2, content, buildingmapt, h, research, planters);

            pixel = new Texture2D(_graphics.GraphicsDevice, 1, 1);
            Color[] data = new Color[1];
            data[0] = Color.LightGray; // Set the color you want
            pixel.SetData(data);
        }

        /// <summary>
        /// loads the game from the buildingSaveFile and makes a new one if that is not found
        /// </summary>
        private void LoadGame()
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            string fileName = "TutorialSave.txt";

            string filePath = Path.Combine(currentDirectory, fileName);
            try
            {
                string[] lines = File.ReadAllLines(filePath);

                string boxes = lines[0];
                string[] tiles = boxes.Split(',');

                #region[0] the tilemap
                for (int i = 0; i < buildingmapt.TileIndices.Length - 1; i++)
                {
                    if (int.TryParse(tiles[i], out int tileIndex))
                    {
                        buildingmapt.TileIndices[i] = tileIndex;
                    }
                    else
                    {
                        buildingmapt.TileIndices[i] = 0;
                    }
                }
                #endregion

                #region[1] the farmers
                string[] subline = lines[1].Split(':');
                for (int i = 0; i < Int32.Parse(subline[0]); i++)
                {
                    // 0 is the home 1 is the position
                    string[] subsubline = subline[i + 1].Split('/');
                    string[] homesubline = subsubline[0].Split(',');
                    string[] possubline = subsubline[1].Split(',');
                    Vector2 pos = new Vector2(Int32.Parse(possubline[0]), Int32.Parse(possubline[1]));
                    Vector2 home = new Vector2(Int32.Parse(homesubline[0]), Int32.Parse(homesubline[1]));
                    Farmer f = new Farmer(pos, grid, content, buildingmapt, houses);
                    f.home = home;
                    farmers2.Add(f);
                }
                #endregion

                #region[2] the wood choppers
                string[] ssubline = lines[2].Split(':');
                for (int i = 0; i < Int32.Parse(ssubline[0]); i++)
                {
                    // 0 is the home 1 is the position
                    string[] subsubline = ssubline[i + 1].Split('/');
                    string[] homesubline = subsubline[0].Split(',');
                    string[] possubline = subsubline[1].Split(',');
                    Vector2 pos = new Vector2(Int32.Parse(possubline[0]), Int32.Parse(possubline[1]));
                    Vector2 home = new Vector2(Int32.Parse(homesubline[0]), Int32.Parse(homesubline[1]));
                    Lumberjack f = new Lumberjack(pos, grid, content, buildingmapt, houses);
                    f.home = home;
                    lumberjacks2.Add(f);
                }
                #endregion

                #region[3] TotalFood
                TotalFood = Int32.Parse(lines[3]);
                #endregion

                #region[4] TotalWood
                TotalWood = Int32.Parse(lines[4]);
                #endregion

                #region[5] current day
                days.CurrentDay = Int32.Parse(lines[5]);
                #endregion
            }
            catch
            {
                MessageBox.Show("no load file found press ok to continue");
            }
        }

        /// <summary>
        /// updates the game 
        /// </summary>
        /// <param name="gameTime">the game time</param>
        public GameScreens Update(GameTime gameTime, ref Camera camera, KeyboardState prevkeyboardstate, KeyboardState curkeyboardstate, MouseState curmouseState)
        {
            TotalFood = days.Update(gameTime, TotalFood, farmers2, lumberjacks2, buildingmapt);
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                return GameScreens.Start;
            }

            //int tileX = (curmouseState.Position.X + (int)camera.Position.X - _graphics.GraphicsDevice.Viewport.Width / 2) / _tilemap.TileWidth; // find the x coordinate of the clicked tile
            //int tileY = (curmouseState.Position.Y + (int)camera.Position.Y - _graphics.GraphicsDevice.Viewport.Height / 2) / _tilemap.TileHeight; // find the y coordinate of the clicked tile


            if (gameScreens == GameScreens.Running)
            {
                if (clickState == ClickState.Building)
                { 
                    clickState = buildingScreen.Update(gameTime, curmouseState, buildingmapt, curkeyboardstate, prevkeyboardstate, camera, _graphics.GraphicsDevice, grid, ref TotalFood, ref TotalWood); 
                }

                //changes to and from building and moving
                else if (clickState == ClickState.Move)
                {
                    //sets the destination location
                    if (curkeyboardstate.IsKeyDown(Keys.B) && prevkeyboardstate.IsKeyUp(Keys.B)) clickState = ClickState.Building;

                    foreach (Farmer f in farmers2)
                    {
                        if (days.NightOrDay == false) f.state = Farmer.FarmerState.ReturningHome;
                        f.Update(gameTime, buildingmapt, out int hold, houses);
                        TotalFood += hold;
                    }
                    foreach (Lumberjack l in lumberjacks2)
                    {
                        if (days.NightOrDay == false) l.state = Lumberjack.LumberjackState.ReturningHome;
                        l.Update(gameTime, buildingmapt, out int hold, houses);
                        TotalWood += hold;
                    }
                    foreach(Planter p in planters)
                    {
                        if (days.NightOrDay == false) p.state = Planter.PlanterState.ReturningHome;
                        p.Update(gameTime, buildingmapt, grid);
                    }
                }

                #region move camera
                if (curkeyboardstate.IsKeyDown(Keys.Left) || curkeyboardstate.IsKeyDown(Keys.A)) { camera.Move(new Vector2(-2, 0)); }
                if (curkeyboardstate.IsKeyDown(Keys.Right) || curkeyboardstate.IsKeyDown(Keys.D)) camera.Move(new Vector2(2, 0));
                if (curkeyboardstate.IsKeyDown(Keys.Up) || curkeyboardstate.IsKeyDown(Keys.W)) camera.Move(new Vector2(0, -2));
                if (curkeyboardstate.IsKeyDown(Keys.Down) || curkeyboardstate.IsKeyDown(Keys.S)) camera.Move(new Vector2(0, 2));
                camera.UpdateTransform(_graphics.GraphicsDevice.Viewport);
                #endregion
                
            }
            return GameScreens.Tutorial;
        }



        /// <summary>
        /// draws the sprites
        /// </summary>
        /// <param name="gameTime">the game time</param>
        public void Draw(GameTime gameTime, ref Camera camera, SpriteFont font, SpriteFont small, Matrix scaleMatrix)
        {
            _graphics.GraphicsDevice.Clear(Color.Black);


            Matrix scaledCameraMatrix = Matrix.Multiply(camera.Transform , scaleMatrix);

            // TODO: Add your drawing code here
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, scaledCameraMatrix);

            if (gameScreens != GameScreens.Controls)
            {
                _tilemap.Draw(gameTime, _spriteBatch);
                buildingmapt.Draw(gameTime, _spriteBatch);
            }

            foreach (Farmer f in farmers2) f.Draw(_spriteBatch, gameTime);
            foreach (Lumberjack l in lumberjacks2) l.Draw(_spriteBatch, gameTime);

            int x = 200;
            int y = 290;
            _spriteBatch.Draw(pixel, new Rectangle(x, y, 225, 24), Color.White);
            _spriteBatch.DrawString(small, "The people need a house to work", new Vector2(x+3, y+3), Color.Black, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);

            _spriteBatch.Draw(pixel, new Rectangle(x-15, y+35, 328, 24), Color.White);
            _spriteBatch.DrawString(small, "Press 'S' or the down arrow to move the screen", new Vector2(x + 3-15, y + 3+35), Color.Black, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);


            x = 200;
            y = 500;
            _spriteBatch.Draw(pixel, new Rectangle(x, y, 210, 24), Color.White);
            _spriteBatch.DrawString(small, "Press B to enter Building Mode", new Vector2(x + 3, y + 3), Color.Black, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);

            _spriteBatch.Draw(pixel, new Rectangle(x-20, y + 35, 260, 24), Color.White);
            _spriteBatch.DrawString(small, "Now Try building a house and 2 farms", new Vector2(x-20 + 3, y + 3+35), Color.Black, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);

            _spriteBatch.Draw(pixel, new Rectangle(x - 20, y + 70, 260, 24), Color.White);
            _spriteBatch.DrawString(small, "Use the 'Q' and 'E' to swtich building options", new Vector2(x - 20 + 3, y + 3 + 70), Color.Black, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);


            x = 225;
            y = 650;
            _spriteBatch.Draw(pixel, new Rectangle(x, y, 150, 24), Color.White);
            _spriteBatch.DrawString(small, "Now Place a farmer", new Vector2(x + 3, y + 3), Color.Black, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
            _spriteBatch.End();



            _spriteBatch.Begin(transformMatrix: scaleMatrix);

            if (gameScreens == GameScreens.Running)
            {
                _spriteBatch.DrawString(font, $"Currently: {clickState} ", new Vector2(250, 55), Color.Black, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                if (clickState == ClickState.Building) buildingScreen.Draw(gameTime, _spriteBatch, font, research);

            }

            if (gameScreens == GameScreens.Running)
            {
                days.Draw(gameTime, _spriteBatch, font, _graphics.GraphicsDevice.Viewport);
                _spriteBatch.DrawString(font, $"Food : {TotalFood} ", new Vector2(700, 10), Color.Black, 0, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                _spriteBatch.DrawString(font, $"Wood : {TotalWood} ", new Vector2(700, 30), Color.Black, 0, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
            }
            _spriteBatch.End();

        }
    }
}