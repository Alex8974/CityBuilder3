using ExampleGame.Enums;
using ExampleGame.Screens;
using ExampleGame.Enums;
using ExampleGame.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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

        private Camera camera;
        BuildingScreen buildingScreen;
        StartScreen startScreen;


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
            buildingScreen = new BuildingScreen();
            startScreen = new StartScreen();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _tilemap = Content.Load<BasicTilemap>("map4");
            buildingmap = Content.Load<BasicTilemap>("map5");
            pen.Position = new Vector2(6 * _tilemap.TileWidth, 6 * _tilemap.TileHeight);

            // TODO: use this.Content to load your game content here
            //_tilemap.LoadContent(Content);
            //buildingmap.LoadContent(Content);
            grid = new Grid(_tilemap.MapWidth, _tilemap.MapHeight, _tilemap);
            camera = new Camera(GraphicsDevice.Viewport, _tilemap.MapWidth * _tilemap.TileWidth, _tilemap.TileHeight * _tilemap.TileHeight);

            font = Content.Load<SpriteFont>("File");
            pen.texture = Content.Load<Texture2D>("Penguin64pxT50pxW");
        }

        protected override void Update(GameTime gameTime)
        {

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();

            #region keboard / mouse
            prevkeyboardstate = curkeyboardstate;
            curkeyboardstate = Keyboard.GetState();
            prevmouseState = curmouseState;
            curmouseState = Mouse.GetState();

            int tileX = (curmouseState.Position.X + (int)camera.Position.X - GraphicsDevice.Viewport.Width / 2) / _tilemap.TileWidth; // find the x coordinate of the clicked tile
            int tileY = (curmouseState.Position.Y + (int)camera.Position.Y - GraphicsDevice.Viewport.Height / 2) / _tilemap.TileHeight; // find the y coordinate of the clicked tile
            #endregion

            if (gameScreens == GameScreens.Start) gameScreens = startScreen.Update(gameTime, curkeyboardstate);
            else if (gameScreens == GameScreens.Running)
            {


                //changes to and from building and moving
                if (clickState == ClickState.Building) clickState = buildingScreen.Update(gameTime, curmouseState, buildingmap, curkeyboardstate, prevkeyboardstate);
                else if (clickState == ClickState.Move)
                {
                    //sets the destination location
                    if (curmouseState.LeftButton == ButtonState.Pressed) { pen.dest = new Vector2(tileX * _tilemap.TileWidth, tileY * _tilemap.TileHeight); }

                    if (curkeyboardstate.IsKeyDown(Keys.B) && prevkeyboardstate.IsKeyUp(Keys.B)) clickState = ClickState.Building;
                    pen.Update(gameTime, _tilemap, grid);

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

            }

            #region move camera
            if (curkeyboardstate.IsKeyDown(Keys.Left)) { camera.Move(new Vector2(-2, 0)); }
            if (curkeyboardstate.IsKeyDown(Keys.Right)) camera.Move(new Vector2(2, 0));
            if (curkeyboardstate.IsKeyDown(Keys.Up)) camera.Move(new Vector2(0, -2));
            if (curkeyboardstate.IsKeyDown(Keys.Down)) camera.Move(new Vector2(0, 2));
            camera.UpdateTransform(GraphicsDevice.Viewport);
            #endregion


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, camera.Transform);
            _tilemap.Draw(gameTime, _spriteBatch);
            _spriteBatch.Draw(pen.texture, pen.Position, null, Color.White, 0, new Vector2(0, 0), 0.5f, SpriteEffects.None, 0);
            buildingmap.Draw(gameTime, _spriteBatch);

            _spriteBatch.End();
            base.Draw(gameTime);

            _spriteBatch.Begin();
            if (gameScreens == GameScreens.Start) { startScreen.Draw(gameTime, _spriteBatch, font); }
            else if (gameScreens == GameScreens.Running)
            {
                _spriteBatch.DrawString(font, $"Current action state: {clickState} ", new Vector2(250, 50), Color.Black, 0, new Vector2(0, 0), 1.5f, SpriteEffects.None, 0);
                if (clickState == ClickState.Building) buildingScreen.Draw(gameTime, _spriteBatch, font);
            }

            _spriteBatch.End();
        }
    }
}