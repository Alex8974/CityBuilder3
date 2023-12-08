using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;

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
        private Dictionary<int, Wonders> avaliableWonders;
        private Dictionary<int, Wonders> buildableWonders;
        private Dictionary<KeyValuePair<int, Vector2>, Wonders> builtWonders;

        int Popcap = 25;
        int lastpop = 0;

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
            if (curkbs.IsKeyDown(Keys.M) && !prevkbs.IsKeyDown(Keys.M))
            {
                PickRandomWonder();
            }
            if(curkbs.IsKeyDown(Keys.N) && !prevkbs.IsKeyDown(Keys.N))
            {
                int hold = buildableWonders.First().Key;
                WonderBuild(hold, mousepos);
            }
        }

        /// <summary>
        /// picks a random Wonder to moves it to the buildable list
        /// </summary>
        private void PickRandomWonder()
        {
            Random r = new Random();
            int randomSpot = r.Next(0, avaliableWonders.Count) - 1;
            buildableWonders.Add(randomSpot, avaliableWonders[randomSpot]);
            avaliableWonders.Remove(randomSpot);
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

    }
}
