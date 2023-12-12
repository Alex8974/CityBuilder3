using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpDX.Direct2D1.Effects;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using MessageBox = System.Windows.Forms.MessageBox;


namespace ExampleGame
{
    public enum Wonders
    {
        TajMahal,
        GreatWallOfChina,
        MachuPichu,
        Colosseum,
        ChristTheRedeemer,
        Petra,
        ChichenItza,
        SydneyOperaHouse,
        AcropolisOfAthens,
        PyramidsOfGiza,
        StBasilsCathedral,
        HagiaSophia
}

    public class SpecialBuildings
    {

        public Texture2D BuildingTexture;
        public Dictionary<int, Wonders> avaliableWonders; // the first section 
        public Dictionary<int, Wonders> buildableWonders; // can build but not built
        public Dictionary<KeyValuePair<int, Vector2>, Wonders> builtWonders; // built wonders

        int Popcap = 25;
        int lastpop = 0;

        float _scaleX = 2.0f;
        float _scaleY = 1.0f;

        public SpecialBuildings()
        {
            avaliableWonders = new();
            buildableWonders = new();
            builtWonders = new();
        }

        public void Initilize(ContentManager c)
        {
            avaliableWonders.Add(0, Wonders.TajMahal);
            avaliableWonders.Add(1, Wonders.GreatWallOfChina);
            avaliableWonders.Add(2, Wonders.MachuPichu);
            avaliableWonders.Add(3, Wonders.Colosseum);
            avaliableWonders.Add(4, Wonders.ChristTheRedeemer);
            avaliableWonders.Add(5, Wonders.Petra);
            avaliableWonders.Add(6, Wonders.ChichenItza);
            avaliableWonders.Add(7, Wonders.SydneyOperaHouse);
            avaliableWonders.Add(8, Wonders.AcropolisOfAthens);
            avaliableWonders.Add(9, Wonders.PyramidsOfGiza);
            avaliableWonders.Add(10, Wonders.StBasilsCathedral);
            avaliableWonders.Add(11, Wonders.HagiaSophia);

            BuildingTexture = c.Load<Texture2D>("AllTheWonders");
        }

        public void Update(GameTime gameTime, int population, KeyboardState curkbs, KeyboardState prevkbs, Vector2 mousepos)
        {
            if (population >= Popcap * (builtWonders.Count + buildableWonders.Count+1))
            {
                PickRandomWonder();

            }
            if(curkbs.IsKeyDown(Keys.V) && !prevkbs.IsKeyDown(Keys.V))
            {
                if(buildableWonders.Count <= 0)
                {
                    MessageBox.Show("You have no wonders ready");
                }
                else
                {
                    int hold = buildableWonders.First().Key;
                    WonderBuild(hold, mousepos);
                }
                
            }
        }

        /// <summary>
        /// picks a random Wonder to moves it to the buildable list
        /// </summary>
        private void PickRandomWonder()
        {
            Random r = new Random();

            if(avaliableWonders.Count > 0)
            {
                List<int> keys = new List<int>(avaliableWonders.Keys);

                int randomIndex = r.Next(keys.Count);
                int randomKey = keys[randomIndex];

                buildableWonders.Add(randomKey, avaliableWonders[randomKey]);

                avaliableWonders.Remove(randomKey);
            }
        }

        private void WonderBuild(int keyofWonderBuilt, Vector2 pos)
        {
            KeyValuePair<int, Vector2> hold = new KeyValuePair<int, Vector2>(keyofWonderBuilt, pos);
            builtWonders.Add( hold, buildableWonders[keyofWonderBuilt]);
            buildableWonders.Remove(keyofWonderBuilt);
        }

        public void Draw(GameTime gT, SpriteBatch sb)
        {
            foreach(var kvp in builtWonders)
            {
                sb.Draw(BuildingTexture, new Vector2(kvp.Key.Value.X*32, kvp.Key.Value.Y*32), new Rectangle(kvp.Key.Key * 64, 0, 64, 64), Color.White, 0, new Vector2(0, 0), 0.5f, SpriteEffects.None, 0);
            }
        }
        public void DrawBuildingMenu(GameTime gT, SpriteBatch spriteBatch, SpriteFont font, Texture2D boxTexture, Vector2 size)
        {
            if (buildableWonders.Count > 0)
            {
                spriteBatch.DrawString(font, "Famous Buildings", new Vector2(700, 190), Color.Black, 0f, new Vector2(0, 0), 0.40f, SpriteEffects.None, 0);
                spriteBatch.DrawString(font, $"' V '", new Vector2(700 + 50, 220), Color.Black, 0, new Vector2(0, 0), 0.40f, SpriteEffects.None, 0);
                spriteBatch.DrawString(font, $"Press", new Vector2(700 + 45, 210), Color.Black, 0, new Vector2(0, 0), 0.40f, SpriteEffects.None, 0);
                spriteBatch.DrawString(font, $"To Build", new Vector2(700 + 45, 230), Color.Black, 0, new Vector2(0, 0), 0.40f, SpriteEffects.None, 0);
            }
            int i = 0;
            foreach(KeyValuePair<int, Wonders> kvp in buildableWonders)
            {
                spriteBatch.Draw(boxTexture, new Vector2(700, 10 + 200 + (30*i)), new Rectangle(0 * 32, 0 * 32, 32, 32), Color.White, 0f, new Vector2(0, 0), new Vector2(1.25f, _scaleY), SpriteEffects.None, 0);
                spriteBatch.Draw(BuildingTexture,new Vector2(705,210 + (30*i)), new Rectangle(kvp.Key * 64, 0, 64, 64), Color.White, 0f, new Vector2(0, 0), 0.5f, SpriteEffects.None, 0);
                i++;
            }
     
            

            
        }

    }
}
