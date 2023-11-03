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

namespace ExampleGame
{
    /// <summary>
    /// This game demonstrates the use of a Tilemap loaded through 
    /// the content pipeline with a custom importer and processor
    /// </summary>
    public class CityBuilderGame : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private BasicTilemap _tilemap;
        private BasicTilemap buildingmap;
        private SpriteFont font;

        private KeyboardState prevkeyboardstate;
        private KeyboardState curkeyboardstate;
        private penguin pen;
        private MouseState curmouseState;
        private MouseState prevmouseState;
        Grid grid;
        ClickState clickState;
        GameScreens gameScreens;
        Day days;

        private Camera camera;
        BuildingScreen buildingScreen;
        StartScreen startScreen;
        ControlScreen controlScreen;

        private List<Farmer> farmers;
        private List<Lumberjack> lumberjacks;
        private int TotalFood = 10;
        private int TotalWood = 10;

        public CityBuilderGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            gameScreens = GameScreens.Start;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            pen = new penguin();
            farmers = new();
            lumberjacks = new();
            buildingScreen = new BuildingScreen(farmers, lumberjacks, Content);
            startScreen = new StartScreen();
            controlScreen = new();
            days = new(Content);
            startScreen.Initilze(Content);
            base.Initialize();
        }

        /// <summary>
        /// loads the content of the game
        /// </summary>
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _tilemap = Content.Load<BasicTilemap>("map4");
            buildingmap = Content.Load<BasicTilemap>("map5");
            pen.Position = new Vector2(6 * _tilemap.TileWidth, 6 * _tilemap.TileHeight);
            
            // TODO: use this.Content to load your game content here
            //_tilemap.LoadContent(Content);
            //buildingmap.LoadContent(Content);
            grid = new Grid(_tilemap.MapWidth, _tilemap.MapHeight, _tilemap, 0);
            camera = new Camera(GraphicsDevice.Viewport, _tilemap.MapWidth * _tilemap.TileWidth, _tilemap.MapHeight * _tilemap.TileHeight);

            font = Content.Load<SpriteFont>("File");
            LoadGame();

            pen.texture = Content.Load<Texture2D>("Penguin64pxT50pxW");
        }
        
        /// <summary>
        /// loads the game from the buildingSaveFile and makes a new one if that is not found
        /// </summary>
        private void LoadGame()
        {
            string filepath = "..\\..\\..\\buildingSaveFile.txt";

            try
            {
                string[] lines = File.ReadAllLines(filepath);

                string boxes = lines[0];
                string[] tiles = boxes.Split(',');

                #region[0] the tilemap
                for (int i = 0; i < buildingmap.TileIndices.Length -1; i++)
                {
                    if(int.TryParse(tiles[i], out int tileIndex))
                    {
                        buildingmap.TileIndices[i] = tileIndex;
                    }
                    else
                    {
                        buildingmap.TileIndices[i] = 0;
                    }
                }
                #endregion
                
                #region[1] the farmers
                string[] subline = lines[1].Split(':');
                for(int i = 0; i < Int32.Parse(subline[0]); i++)
                {
                    // 0 is the home 1 is the position
                    string[] subsubline = subline[i+1].Split('/');
                    string[] homesubline = subsubline[0].Split(',');
                    string[] possubline = subsubline[1].Split(',');
                    Vector2 pos = new Vector2(Int32.Parse(possubline[0]), Int32.Parse(possubline[1]));
                    Vector2 home = new Vector2(Int32.Parse(homesubline[0]), Int32.Parse(homesubline[1]));
                    Farmer f = new Farmer(pos, grid, Content, buildingmap);
                    f.home = home;
                    farmers.Add(f);
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
                    Lumberjack f = new Lumberjack(pos, grid, Content, buildingmap);
                    f.home = home;
                    lumberjacks.Add(f);
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
        protected override void Update(GameTime gameTime)
        {
            TotalFood = days.Update(gameTime, TotalFood, farmers, lumberjacks);
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                CloseGame();
                Exit();
            }

            #region keboard / mouse
            prevkeyboardstate = curkeyboardstate;
            curkeyboardstate = Keyboard.GetState();
            prevmouseState = curmouseState;
            curmouseState = Mouse.GetState();

            int tileX = (curmouseState.Position.X + (int)camera.Position.X - GraphicsDevice.Viewport.Width / 2) / _tilemap.TileWidth; // find the x coordinate of the clicked tile
            int tileY = (curmouseState.Position.Y + (int)camera.Position.Y - GraphicsDevice.Viewport.Height / 2) / _tilemap.TileHeight; // find the y coordinate of the clicked tile
            #endregion
            if (gameScreens == GameScreens.Controls) gameScreens = controlScreen.Update(gameTime, curkeyboardstate, prevkeyboardstate);
            else if (gameScreens == GameScreens.Start) gameScreens = startScreen.Update(gameTime, curkeyboardstate, prevkeyboardstate);
            else if (gameScreens == GameScreens.Running)
            {


                //changes to and from building and moving
                if (clickState == ClickState.Building) clickState = buildingScreen.Update(gameTime, curmouseState, buildingmap, curkeyboardstate, prevkeyboardstate, camera, _graphics.GraphicsDevice, grid,ref TotalFood,ref TotalWood);
                else if (clickState == ClickState.Move)
                {
                    //sets the destination location
                    if (curmouseState.LeftButton == ButtonState.Pressed) { pen.dest = new Vector2(tileX * _tilemap.TileWidth, tileY * _tilemap.TileHeight); }

                    if (curkeyboardstate.IsKeyDown(Keys.B) && prevkeyboardstate.IsKeyUp(Keys.B)) clickState = ClickState.Building;
                    //pen.Update(gameTime, _tilemap, grid);
                    foreach (Farmer f in farmers)
                    {
                        if (days.NightOrDay == false) f.state = Farmer.FarmerState.ReturningHome;
                        f.Update(gameTime, buildingmap, out int hold);
                        TotalFood += hold;
                    }
                    foreach(Lumberjack l in lumberjacks)
                    {
                        if (days.NightOrDay == false) l.state = Lumberjack.LumberjackState.ReturningHome;
                        l.Update(gameTime, buildingmap, out int hold);
                        TotalWood += hold;
                    }
                }

                #region move arrowkeys
                if (curkeyboardstate.IsKeyDown(Keys.A) && !prevkeyboardstate.IsKeyDown(Keys.A))
                {
                    // Calculate the destination based on current position and the desired direction
                    pen.dest = new Vector2(pen.Position.X - _tilemap.TileWidth, pen.Position.Y);
                    pen.Move(gameTime, _tilemap);
                }

                if (curkeyboardstate.IsKeyDown(Keys.D) && !prevkeyboardstate.IsKeyDown(Keys.D))
                {
                    pen.dest = new Vector2(pen.Position.X + _tilemap.TileWidth, pen.Position.Y);
                    pen.Move(gameTime, _tilemap);
                }

                if (curkeyboardstate.IsKeyDown(Keys.W) && !prevkeyboardstate.IsKeyDown(Keys.W))
                {
                    pen.dest = new Vector2(pen.Position.X, pen.Position.Y - _tilemap.TileHeight);
                    pen.Move(gameTime, _tilemap);
                }

                if (curkeyboardstate.IsKeyDown(Keys.S) && !prevkeyboardstate.IsKeyDown(Keys.S))
                {
                    pen.dest = new Vector2(pen.Position.X, pen.Position.Y + _tilemap.TileHeight);
                    pen.Move(gameTime, _tilemap);
                }
                #endregion

                #region move camera
                if (curkeyboardstate.IsKeyDown(Keys.Left)) { camera.Move(new Vector2(-2, 0)); }
                if (curkeyboardstate.IsKeyDown(Keys.Right)) camera.Move(new Vector2(2, 0));
                if (curkeyboardstate.IsKeyDown(Keys.Up)) camera.Move(new Vector2(0, -2));
                if (curkeyboardstate.IsKeyDown(Keys.Down)) camera.Move(new Vector2(0, 2));
                camera.UpdateTransform(GraphicsDevice.Viewport);
                #endregion
            }
            
            base.Update(gameTime);
        }

        /// <summary>
        /// when the game is closed
        /// </summary>
        protected void CloseGame()
        {
            string s = "";
            for(int i = 0; i < buildingmap.TileIndices.Length-1; i++)
            {
                s += $"{buildingmap.TileIndices[i]}, ";
            }
            WriteToFile(s);
        }

        /// <summary>
        /// writes to the save file
        /// </summary>
        /// <param name="s">the string that is being written</param>
        private void WriteToFile(string s)
        {
            string filePath = "..\\..\\..\\buildingSaveFile.txt";

            if (!File.Exists(filePath))
            {
                File.Create(filePath).Close();
            }

            // Read the first three lines from the file

            File.WriteAllLines(filePath, new string[0]);

            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                writer.WriteLine(s);
                int farmerCount = farmers.Count;
                string fs = $"{farmerCount}";
                foreach(Farmer f in farmers)
                {
                    fs += $":{f.home.X},{f.home.Y}/";
                    fs += $"{f.Position.X / buildingmap.TileWidth},{f.Position.Y / buildingmap.TileHeight}";
                }
                writer.WriteLine(fs);

                int lumberCount = lumberjacks.Count;
                string ls = $"{lumberCount}";
                foreach (Lumberjack l in lumberjacks)
                {
                    ls += $":{l.home.X},{l.home.Y}/";
                    ls += $"{l.Position.X / buildingmap.TileWidth},{l.Position.Y / buildingmap.TileHeight}";
                }
                writer.WriteLine(ls);
                writer.WriteLine(TotalFood);
                writer.WriteLine(TotalWood);
                writer.WriteLine(days.CurrentDay);
            }
        }

        /// <summary>
        /// draws the sprites
        /// </summary>
        /// <param name="gameTime">the game time</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, camera.Transform);
            if (gameScreens != GameScreens.Controls)
            {
                _tilemap.Draw(gameTime, _spriteBatch);
                _spriteBatch.Draw(pen.texture, pen.Position, null, Color.White, 0, new Vector2(0, 0), 0.5f, SpriteEffects.None, 0);
                buildingmap.Draw(gameTime, _spriteBatch);
            }

            foreach (Farmer f in farmers) f.Draw(_spriteBatch, gameTime);
            foreach (Lumberjack l in lumberjacks) l.Draw(_spriteBatch, gameTime);
            _spriteBatch.End();



            _spriteBatch.Begin();
            if (gameScreens == GameScreens.Start) { startScreen.Draw(gameTime, _spriteBatch, font); }
            else if (gameScreens == GameScreens.Running)
            {
                _spriteBatch.DrawString(font, $"Currently: {clickState} ", new Vector2(250, 55), Color.Black, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                if (clickState == ClickState.Building) buildingScreen.Draw(gameTime, _spriteBatch, font);
            }
            else if (gameScreens == GameScreens.Controls) controlScreen.Draw(_spriteBatch, font);
            
            if(gameScreens == GameScreens.Running)
            {
                days.Draw(gameTime, _spriteBatch, font, GraphicsDevice.Viewport);
                _spriteBatch.DrawString(font, $"Food : {TotalFood} ", new Vector2(700, 10), Color.Black, 0, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                _spriteBatch.DrawString(font, $"Wood : {TotalWood} ", new Vector2(700, 30), Color.Black, 0, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
            }
            _spriteBatch.End();

            base.Draw(gameTime);

        }
    }
}