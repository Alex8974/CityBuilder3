﻿using ExampleGame.Enums;
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
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Reflection;
using Microsoft.Xna.Framework.Media;

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
        private TileMapGenerator generator;
        private SpriteFont font;
        private SpriteFont font2;

        private KeyboardState prevkeyboardstate;
        private KeyboardState curkeyboardstate;
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
        TutorialScreen tutorialScreen;
        Research research;

        private List<Farmer> farmers;
        private List<Lumberjack> lumberjacks;
        private List<Planter> planters;
        private List<House> housing;
        private int TotalFood = 10;
        private int TotalWood = 10;
        private int TotalPopulation = 0;
        private SpecialBuildings specialBuildings;

        Crate Moon;
        CirclingCamera MoonCamera;

        Song song;

        public CityBuilderGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            gameScreens = GameScreens.Start;
        }

        protected override void Initialize()
        {
            int logicalWidth = 1600;
            int logicalHeight = 800;
            // TODO: Add your initialization logic here
            GraphicsDevice.Viewport = new Viewport(0, 0, logicalWidth, logicalHeight);

            _graphics.PreferredBackBufferWidth = logicalWidth;
            _graphics.PreferredBackBufferHeight = logicalHeight;
            _graphics.ApplyChanges();

            farmers = new();
            lumberjacks = new();
            planters = new();
            housing = new();
            startScreen = new StartScreen();
            controlScreen = new();
            research = new();
            days = new(Content);
            specialBuildings = new();
            startScreen.Initilze(Content);
            MoonCamera = new(this, new Vector3(50, 10, 10), 1.0f);
            specialBuildings.Initilize(Content);
            base.Initialize();
        }

        public void LoadMapContent()
        {

            buildingmap = Content.Load<BasicTilemap>("map5");
            buildingScreen = new BuildingScreen(farmers, lumberjacks, Content, buildingmap, housing, research, planters, specialBuildings);
            LoadGame();
        }

        /// <summary>
        /// loads the content of the game
        /// </summary>
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            song = Content.Load<Song>("SalmonLikeTheFish");
            MediaPlayer.Play(song);
            _tilemap = Content.Load<BasicTilemap>("map4");
            tutorialScreen = new(Content, font, _graphics, _tilemap);

            
            Moon = new Crate(this, CrateType.Slats, Matrix.CreateTranslation(1, 1, 1));

            grid = new Grid(_tilemap.MapWidth, _tilemap.MapHeight, _tilemap, 0);
            camera = new Camera(GraphicsDevice.Viewport, _tilemap.MapWidth * _tilemap.TileWidth, _tilemap.MapHeight * _tilemap.TileHeight);

            font = Content.Load<SpriteFont>("File");
            font2 = Content.Load<SpriteFont>("File2");
            
        }
        
        /// <summary>
        /// loads the game from the buildingSaveFile and makes a new one if that is not found
        /// </summary>
        private void LoadGame()
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            string fileName = "buildingSaveFile.txt";

            // Use Path.Combine to create the file path
            string filePath = Path.Combine(currentDirectory, fileName);

            try
            {
                string[] lines = File.ReadAllLines(filePath);

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
                    Farmer f = new Farmer(pos, grid, Content, buildingmap, housing);
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
                    Lumberjack f = new Lumberjack(pos, grid, Content, buildingmap, housing);
                    f.home = home;
                    lumberjacks.Add(f);
                }
                #endregion

                #region[3] houseing
                bool mysteryPerson = false;
                string[] housesubline = lines[3].Split(':');
                for (int i = 0; i < Int32.Parse(housesubline[0]); i++)
                {
                    // 0 is the home 1 is the position
                    string[] subsubline = housesubline[i + 1].Split('/');
                    string[] peopleinhouse = subsubline[0].Split(',');
                    string[] posofhouse = subsubline[1].Split(',');
                    int o = Int32.Parse( peopleinhouse[0]);
                    int c = Int32.Parse(peopleinhouse[1]);
                    Vector2 pos = new Vector2(Int32.Parse(posofhouse[0]), Int32.Parse(posofhouse[1]));
                    if(o != c && mysteryPerson == false)
                    {
                        mysteryPerson = true;
                       // o--;
                    }
                    House h = new House(pos, c, o);
                    housing.Add(h);
                }
                #endregion

                #region[4] TotalFood
                TotalFood = Int32.Parse(lines[4]);
                #endregion

                #region[5] TotalWood
                TotalWood = Int32.Parse(lines[5]);
                #endregion

                #region[6] current day
                days.CurrentDay = Int32.Parse(lines[6]);
                #endregion

                #region[7] research
                BuildingScreen.houseCapacity = Int32.Parse(lines[7]);
                #endregion

                #region[8] planters
                string[] psubline = lines[8].Split(':');
                for (int i = 0; i < Int32.Parse(psubline[0]); i++)
                {
                    // 0 is the home 1 is the position
                    string[] subsubline = psubline[i + 1].Split('/');
                    string[] homesubline = subsubline[0].Split(',');
                    string[] possubline = subsubline[1].Split(',');
                    Vector2 pos = new Vector2(Int32.Parse(possubline[0]), Int32.Parse(possubline[1]));
                    Vector2 home = new Vector2(Int32.Parse(homesubline[0]), Int32.Parse(homesubline[1]));
                    Planter p = new Planter(pos, grid, Content, buildingmap, housing, true);
                    p.home = home;
                    planters.Add(p);
                }
                #endregion

                #region[9] planter Research
                research.PlanterResearch = bool.Parse(lines[9]);
                #endregion

                #region[10] avaliable buildings
                string input = lines[10];
                Dictionary<int, Wonders> result = ParseDictionaryString(input);
                specialBuildings.avaliableWonders = result;

                #endregion

                #region[11] buildable buildings
                string input2 = lines[11];
                Dictionary<int, Wonders> result2 = ParseDictionaryString(input2);
                specialBuildings.buildableWonders = result2;

                #endregion

                #region[12] built buildings
                string input3 = lines[12];
                Dictionary<KeyValuePair<int, Vector2>, Wonders> result3 = ParseDictionaryStrin(input3);
                specialBuildings.builtWonders = result3;
                #endregion
            }
            catch
            {
                MessageBox.Show("no load file found press ok to continue");
            }
        }

        static Dictionary<KeyValuePair<int, Vector2>, Wonders> ParseDictionaryStrin(string input)
        {
            Dictionary<KeyValuePair<int, Vector2>, Wonders> dictionary = new Dictionary<KeyValuePair<int, Vector2>, Wonders>();

            // Use regular expression to match key-value pairs in the input string
            string[] eachitem = input.Split("^");

            for(int i = 0; i < eachitem.Length-1; i++)
            {
                string[] eachpart = eachitem[i].Split(",");
                Enum.TryParse<Wonders>(eachpart[2].Trim('[','{','}',']'), out Wonders result);
                string hold = eachpart[1];
                Vector2 holdvector = new Vector2();
                Match match = Regex.Match(hold.Trim(']'), @"X:(\d+) Y:(\d+)");

                if (match.Success)
                {
                    // Access the captured numeric values
                    int xValue = int.Parse(match.Groups[1].Value);
                    int yValue = int.Parse(match.Groups[2].Value);

                    // Store values in an int array
                    holdvector.X = xValue;
                    holdvector.Y = yValue;

                }
                else
                {
                    Console.WriteLine("No match found");
                }

                hold = hold.Trim('[', '{', '}', ']');
                hold = hold.Trim('X', 'Y', ':');
                
                string[] holding = hold.Split(' ');
                KeyValuePair<int, Vector2> key = new KeyValuePair<int, Vector2>(Int32.Parse(eachpart[0].Trim('[', '{', '}', ']')), holdvector);
                dictionary.Add(key, result);
            }

            return dictionary;
        }
        static Dictionary<int, Wonders> ParseDictionaryString(string input)
        {
            Dictionary<int, Wonders> dictionary = new Dictionary<int, Wonders>();

            // Use regular expression to match key-value pairs in the input string
            var matches = Regex.Matches(input, @"\[(.*?)\]");

            foreach (Match match in matches)
            {
                string[] items = match.Groups[1].Value.Split(',');

                if (items.Length == 2)
                {
                    int index = int.Parse(items[0]);
                    Wonders enumValue;

                    if (Enum.TryParse<Wonders>(items[1], out enumValue))
                    {
                        dictionary.Add(index, enumValue);
                    }
                }
                // Handle additional cases if needed (e.g., nested dictionaries)
            }

            return dictionary;
        }

        /// <summary>
        /// updates the game 
        /// </summary>
        /// <param name="gameTime">the game time</param>
        protected override void Update(GameTime gameTime)
        {
           
            #region keboard / mouse
            prevkeyboardstate = curkeyboardstate;
            curkeyboardstate = Keyboard.GetState();
            prevmouseState = curmouseState;
            curmouseState = Mouse.GetState();

            //int tileX = (curmouseState.Position.X + (int)camera.Position.X - GraphicsDevice.Viewport.Width / 2) / _tilemap.TileWidth; // find the x coordinate of the clicked tile
            //int tileY = (curmouseState.Position.Y + (int)camera.Position.Y - GraphicsDevice.Viewport.Height / 2) / _tilemap.TileHeight; // find the y coordinate of the clicked tile


            //int mx = (curmouseState.Position.X + (int)camera.Position.X - GraphicsDevice.Viewport.Width / 2) / _tilemap.TileWidth;
            //int my = (curmouseState.Position.Y + (int)camera.Position.Y - GraphicsDevice.Viewport.Height / 2) / _tilemap.TileHeight;


            // Calculate the scaled mouse position
            float scaleX = (float)GraphicsDevice.PresentationParameters.BackBufferWidth / 800;
            float scaleY = (float)GraphicsDevice.PresentationParameters.BackBufferHeight / 400;

            int scaledMouseX = (int)((curmouseState.Position.X + (int)camera.Position.X - GraphicsDevice.Viewport.Width / 2) / scaleX);
            int scaledMouseY = (int)((curmouseState.Position.Y + (int)camera.Position.Y - GraphicsDevice.Viewport.Height / 2) / scaleY);

            // Calculate the tile coordinates using the scaled mouse position
            int tileX = scaledMouseX / _tilemap.TileWidth;
            int tileY = scaledMouseY / _tilemap.TileHeight;
            #endregion
            


            if (gameScreens == GameScreens.Tutorial)
            {
                gameScreens = tutorialScreen.Update(gameTime, ref camera, prevkeyboardstate, curkeyboardstate, curmouseState);
                return;
            }

            MoonCamera.Update(gameTime);

            TotalFood = days.Update(gameTime, TotalFood, farmers, lumberjacks, buildingmap);
            if(TotalFood < 0)
            {
                MessageBox.Show("You ran out of food and some of your people died");


                while(TotalFood < 0)
                {
                    if(lumberjacks.Count > 0)
                    {
                        int hold = lumberjacks.Count;
                        Lumberjack holdl = lumberjacks[hold - 1];
                        holdl.Remove(holdl, housing);
                        lumberjacks.Remove(holdl);
                        TotalFood += 2;
                    }
                    else if (planters.Count > 0)
                    {
                        int hold = planters.Count;
                        Planter holdp = planters[hold - 1];
                        holdp.Remove(holdp, housing);
                        planters.Remove(holdp);
                        TotalFood += 2;
                    }
                    else if(farmers.Count > 0)
                    {
                        int hold = farmers.Count;
                        Farmer holdp = farmers[hold - 1];
                        holdp.Remove(holdp, housing);
                        farmers.Remove(holdp);
                        TotalFood += 2;
                    }
                    else
                    {
                        MessageBox.Show("You have lost all your people \n Game Over");
                        string currentDirectory = Directory.GetCurrentDirectory();
                        string fileName = "buildingSaveFile.txt";

                        // Use Path.Combine to create the file path
                        string filePath = Path.Combine(currentDirectory, fileName);


                        if (!File.Exists(filePath))
                        {
                            File.Create(filePath).Close();
                        }

                        // Read the first three lines from the file

                        File.WriteAllText(filePath, string.Empty);

                        Exit();
                    }

                }




                
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Escape) && !prevkeyboardstate.IsKeyDown(Keys.Escape))
            {
                if(gameScreens == GameScreens.Running) CloseGame();
                farmers.Clear();
                lumberjacks.Clear();
                planters.Clear();
                housing.Clear();
                Farmer.allFarmingLocations.Clear();
                Lumberjack.allWoodChoppingLocations.Clear();
                Planter.allplantingLocations.Clear();
                days.NightOrDay = true;
                if (gameScreens != GameScreens.Start) gameScreens = GameScreens.Start;
                else Exit();
            }

            if (gameScreens == GameScreens.Controls) gameScreens = controlScreen.Update(gameTime, curkeyboardstate, prevkeyboardstate);
            else if (gameScreens == GameScreens.Start)
            {
                gameScreens = startScreen.Update(gameTime, curkeyboardstate, prevkeyboardstate, tutorialScreen, this);

            }
            else if (gameScreens == GameScreens.Running)
            {
                GameUpdate(gameTime);                
            }
            
            base.Update(gameTime);
        }

        /// <summary>
        /// when the game is closed
        /// </summary>
        protected void CloseGame()
        {
            try
            {
                string s = "";
                for (int i = 0; i < buildingmap.TileIndices.Length - 1; i++)
                {
                    s += $"{buildingmap.TileIndices[i]}, ";
                }
                WriteToFile(s);
            }
            catch
            {
                MessageBox.Show("bad save data, did not save");
            }
        }

        /// <summary>
        /// updates the current game
        /// </summary>
        /// <param name="gameTime">the game time</param>
        private void GameUpdate(GameTime gameTime)
        {
            foreach (House h in housing) h.Update(buildingmap);
            //changes to and from building and moving
            if (clickState == ClickState.Building) clickState = buildingScreen.Update(gameTime, curmouseState, buildingmap, curkeyboardstate, prevkeyboardstate, camera, _graphics.GraphicsDevice, grid, ref TotalFood, ref TotalWood, TotalPopulation);
            else if (clickState == ClickState.Move)
            {
                //sets the destination location

                if (curkeyboardstate.IsKeyDown(Keys.B) && prevkeyboardstate.IsKeyUp(Keys.B)) clickState = ClickState.Building;
                //pen.Update(gameTime, _tilemap, grid);
                foreach (Farmer f in farmers)
                {
                    if (days.NightOrDay == false) f.state = Farmer.FarmerState.ReturningHome;
                    f.Update(gameTime, buildingmap, out int hold, housing);
                    TotalFood += hold;
                }
                foreach (Lumberjack l in lumberjacks)
                {
                    if (days.NightOrDay == false) l.state = Lumberjack.LumberjackState.ReturningHome;
                    l.Update(gameTime, buildingmap, out int hold, housing);
                    TotalWood += hold;
                }
                foreach(Planter p in planters)
                {
                    if (days.NightOrDay == false) p.state = Planter.PlanterState.ReturningHome;
                    p.Update(gameTime, buildingmap, out int hold, housing);
                    //TotalFood -= hold;
                }

                if(days.NightOrDay == false)
                {
                    Planter.allplantingLocations.Clear();
                    Lumberjack.allWoodChoppingLocations.Clear();
                    Farmer.allFarmingLocations.Clear();
                }

                TotalPopulation = lumberjacks.Count + farmers.Count + planters.Count;
            }

            #region move camera
            if (curkeyboardstate.IsKeyDown(Keys.Left) || curkeyboardstate.IsKeyDown(Keys.A)) { camera.Move(new Vector2(-2, 0)); }
            if (curkeyboardstate.IsKeyDown(Keys.Right) || curkeyboardstate.IsKeyDown(Keys.D)) camera.Move(new Vector2(2, 0));
            if (curkeyboardstate.IsKeyDown(Keys.Up) || curkeyboardstate.IsKeyDown(Keys.W)) camera.Move(new Vector2(0, -2));
            if (curkeyboardstate.IsKeyDown(Keys.Down) || curkeyboardstate.IsKeyDown(Keys.S)) camera.Move(new Vector2(0, 2));
            camera.UpdateTransform(GraphicsDevice.Viewport);
            #endregion
        }

        /// <summary>
        /// writes to the save file
        /// </summary>
        /// <param name="s">the string that is being written</param>
        private void WriteToFile(string s)
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            string fileName = "buildingSaveFile.txt";

            // Use Path.Combine to create the file path
            string filePath = Path.Combine(currentDirectory, fileName);


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

                int housecount = housing.Count;
                string hs = $"{housecount}";
                foreach(House h in housing)
                {
                    hs += $":{h.Occupants},{h.Capacity}/";
                    hs += $"{h.Position.X},{h.Position.Y}";
                }
                writer.WriteLine(hs);

                writer.WriteLine(TotalFood);
                writer.WriteLine(TotalWood);
                writer.WriteLine(days.CurrentDay);
                int hold = housing.Count;
                writer.WriteLine(housing[hold-1].Capacity);

                int plantercount = planters.Count;
                string ps = $"{plantercount}";
                foreach(Planter p in planters)
                {
                    ps += $":{p.home.X},{p.home.Y}/";
                    ps += $"{p.Position.X / buildingmap.TileWidth},{p.Position.Y / buildingmap.TileHeight}";
                }
                writer.WriteLine(ps);
                writer.WriteLine(research.PlanterResearch);

                #region Wonders

                //writer.Write(specialBuildings.avaliableWonders.Count + ":");
                foreach(KeyValuePair<int, Wonders> kvp in specialBuildings.avaliableWonders)
                {
                    writer.Write(kvp.ToString());
                    //writer.Write(":");
                }
                writer.WriteLine();

                //writer.Write(specialBuildings.buildableWonders.Count + ":");
                foreach (KeyValuePair<int, Wonders> kvp in specialBuildings.buildableWonders)
                {
                    writer.Write(kvp.ToString());
                    //writer.Write(":");
                }
                writer.WriteLine();

                foreach (KeyValuePair<KeyValuePair<int, Vector2>, Wonders> kvp in specialBuildings.builtWonders)
                {
                    writer.Write(kvp.ToString());
                    writer.Write("^");
                }
                writer.WriteLine();


                #endregion
            }
        }

        /// <summary>
        /// draws the sprites
        /// </summary>
        /// <param name="gameTime">the game time</param>
        protected override void Draw(GameTime gameTime)
        {
            Matrix cameraMatrix = camera.Transform;// Your existing camera matrix
            float scaleX = (float)GraphicsDevice.PresentationParameters.BackBufferWidth / 800;
            float scaleY = (float)GraphicsDevice.PresentationParameters.BackBufferHeight / 400;

            // Apply scaling factors to the camera matrix
            Matrix scaleMatrix = Matrix.CreateScale(scaleX, scaleY, 1.0f);
            Matrix scaledCameraMatrix = Matrix.Multiply(cameraMatrix, scaleMatrix);

            if (gameScreens == GameScreens.Tutorial)
            {
                tutorialScreen.Draw(gameTime, ref camera, font, font2, scaleMatrix);
                return;
            }

            GraphicsDevice.Clear(Color.Black);

            

            //stays in the same spot when the screen moves
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, scaledCameraMatrix);
                if (gameScreens != GameScreens.Controls && gameScreens != GameScreens.Start)
                {
                    _tilemap.Draw(gameTime, _spriteBatch);
                    buildingmap.Draw(gameTime, _spriteBatch);
                }

                if(gameScreens == GameScreens.Running)
                {
                    foreach (Farmer f in farmers) f.Draw(_spriteBatch, gameTime);
                    foreach (Lumberjack l in lumberjacks) l.Draw(_spriteBatch, gameTime);
                    foreach (Planter p in planters) p.Draw(_spriteBatch, gameTime);
                    specialBuildings.Draw(gameTime, _spriteBatch);
                }
                foreach (House h in housing) h.Draw(_spriteBatch, font);
            _spriteBatch.End();


            //does not move on the screen
            _spriteBatch.Begin(transformMatrix: scaleMatrix);
            if (gameScreens == GameScreens.Start) 
            { 
                startScreen.Draw(gameTime, _spriteBatch, font);
            }
            else if (gameScreens == GameScreens.Running)
            {
                _spriteBatch.DrawString(font, $"Currently: {clickState} ", new Vector2(250, 55), Color.Black, 0, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                if (clickState == ClickState.Building) buildingScreen.Draw(gameTime, _spriteBatch, font, research);
            }
            else if (gameScreens == GameScreens.Controls) controlScreen.Draw(_spriteBatch, font);
            
            if(gameScreens == GameScreens.Running)
            {
                days.Draw(gameTime, _spriteBatch, font, GraphicsDevice.Viewport);
                _spriteBatch.DrawString(font, $"Food : {TotalFood} ", new Vector2(675, 10), Color.Black, 0, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                _spriteBatch.DrawString(font, $"Wood : {TotalWood} ", new Vector2(675, 30), Color.Black, 0, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                _spriteBatch.DrawString(font, $"Population : {TotalPopulation} ", new Vector2(650, 50), Color.Black, 0, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);

            }
            _spriteBatch.End();

            #region Draws the Moon
            Matrix transform = Matrix.CreateTranslation(700, -100, 0);
            _spriteBatch.Begin( transformMatrix: transform);
            if(days.NightOrDay == false)
            {
                Moon.Draw(MoonCamera);
            }
            _spriteBatch.End();
            #endregion

            base.Draw(gameTime);

        }
    }
}