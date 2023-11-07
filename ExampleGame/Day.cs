using CityBuilderGame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using System.ComponentModel;
using SharpDX.DirectWrite;
using SharpDX.Direct2D1.Effects;
using ExampleGame.Enums;

namespace ExampleGame
{
    public class Day
    {
        public int CurrentDay;

        /// <summary>
        /// if true it is day, if false it is night
        /// </summary>
        public bool NightOrDay;

        /// <summary>
        /// how long the day lasts
        /// </summary>
        public const double LENGTHOFDAY = 45.0f;

        /// <summary>
        /// how long the night lasts
        /// </summary>
        public const double LENGTHOFNIGHT = 15.0f;

        /// <summary>
        /// how long the current night or day has progressed
        /// </summary>
        public double DayNightTimer;

        Texture2D onePixelTexture;
        Texture2D circle;
        double rotation;
        int WRegrowOddsMax = 15;
        int WRegrowOddsLessThanToSucced = 2;
        int TRegrowOddsMax = 30;
        int TRegrowOddsLessThanToSucced = 2;

        public Day(ContentManager c)
        {
            CurrentDay = 0;
            NightOrDay = true;
            DayNightTimer = 0;
            onePixelTexture = c.Load<Texture2D>("onePixelNight");
            circle = c.Load<Texture2D>("DayNightCircle");
            rotation = 0;

        }



        /// <summary>
        /// update the current game time and whether or not it is day or night
        /// </summary>
        /// <param name="gT"></param>
        /// <returns> the new total food</returns>
        public int Update(GameTime gT, int TotalFood, List<Farmer> f, List<Lumberjack> l, BasicTilemap bm)
        {
            DayNightTimer += gT.ElapsedGameTime.TotalSeconds;
            if(NightOrDay == true && DayNightTimer >= LENGTHOFDAY)
            {
                Farmer.allFarmingLocations.Clear();
                DayNightTimer = 0;
                NightOrDay = false;
            }
            else if(NightOrDay == false && DayNightTimer >= LENGTHOFNIGHT)
            {
                DayNightTimer = 0;
                NightOrDay = true;
                TotalFood = ChangeDay(TotalFood, f, l);
                GrowOnNewDay(bm);
            }
            return TotalFood;
        }

        private void GrowOnNewDay(BasicTilemap bm)
        {
            //for crops
            for(int i =0; i < bm.MapHeight; i++)
            {
                for(int j = 0; j < bm.MapWidth; j++)
                {
                    if (bm.TileIndices[(i*bm.MapWidth) + j] <= (int)BuildingOptions.CropsEmpty && bm.TileIndices[(i * bm.MapWidth) + j] >= (int)BuildingOptions.Crops80)
                    {
                        Random r = new Random();
                        
                        if (r.Next(WRegrowOddsMax) < WRegrowOddsLessThanToSucced) bm.TileIndices[(i * bm.MapWidth) + j] -= 2;
                    }
                }
            }
            //for trees
            for (int i = 0; i < bm.MapHeight; i++)
            {
                for (int j = 0; j < bm.MapWidth; j++)
                {
                    if (bm.TileIndices[(i * bm.MapWidth) + j] == (int)BuildingOptions.ChoppedTree)
                    {
                        Random r = new Random();

                        if (r.Next(TRegrowOddsMax) < TRegrowOddsLessThanToSucced) bm.TileIndices[(i * bm.MapWidth) + j]--;
                    }
                }
            }
        }

        /// <summary>
        /// changes to the next day
        /// </summary>
        /// <returns> the new total food</returns>
        private int ChangeDay(int TotalFood,  List<Farmer> f, List<Lumberjack> l)
        {
            CurrentDay++;
            TotalFood -= f.Count*2;
            TotalFood -= l.Count*2;
            rotation = 0;
            return TotalFood;
        }

        /// <summary>
        /// draws the information for this class
        /// </summary>
        /// <param name="gT"></param>
        /// <param name="sb"></param>
        public void Draw(GameTime gT, SpriteBatch sb, SpriteFont f, Viewport v)
        {

            rotation += gT.ElapsedGameTime.TotalSeconds;
            float calcR = (float)(rotation / (LENGTHOFDAY + LENGTHOFNIGHT));
            float RcalcR = (float) calcR * (float)6.25;
            sb.Draw(circle, new Vector2(645,21), null, Color.White, -RcalcR , new Vector2(16, 16), 1f, SpriteEffects.None, 0);
            sb.DrawString(f, $"Current Day:    {CurrentDay}", new Vector2(500, 10), Color.Black, 0, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
            Color nightColor = new Color(0, 0, 160, 128); // Dark blue with 50% transparency

            if (NightOrDay == false)
            {
                sb.Draw(onePixelTexture, v.Bounds, nightColor);
            }
        }


    }
}
