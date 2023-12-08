using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
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
        private Dictionary<int, Wonders> builtWonders;

        int Popcap = 25;
        int lastpop = 0;

        public SpecialBuildings()
        {

        }

        public void Initilize(ContentManager c)
        {
            avaliableWonders.Add(1, Wonders.TajMahal);
            avaliableWonders.Add(2, Wonders.GreatWallOfChina);
            avaliableWonders.Add(3, Wonders.MachuPichu);
            avaliableWonders.Add(4, Wonders.Colosseum);
            avaliableWonders.Add(5, Wonders.ChristTheRedeemer);
            avaliableWonders.Add(6, Wonders.Petra);
            avaliableWonders.Add(7, Wonders.ChichenItza);
            avaliableWonders.Add(8, Wonders.SydneyOperaHouse);
            avaliableWonders.Add(9, Wonders.AcropolisOfAthens);
            avaliableWonders.Add(10, Wonders.PyramidsOfGiza);
            avaliableWonders.Add(11, Wonders.StBasilsCathedral);
            avaliableWonders.Add(12, Wonders.HagiaSophia);

            BuildingTexture = c.Load<Texture2D>("thefilename");
        }

        public void Update(GameTime gameTime, int population)
        {

        }

        /// <summary>
        /// picks a random Wonder to moves it to the buildable list
        /// </summary>
        private void PickRandomWonder()
        {
            Random r = new Random();
            int randomSpot = r.Next(1, avaliableWonders.Count) - 1;
            buildableWonders.Add(randomSpot, avaliableWonders[randomSpot]);
            avaliableWonders.Remove(randomSpot);
        }

        private void WonderBuild(int keyofWonderBuilt)
        {

            builtWonders.Add(keyofWonderBuilt, buildableWonders[keyofWonderBuilt]);
        }

        public void Draw(GameTime gT, SpriteBatch sb)
        {

        }

    }
}
