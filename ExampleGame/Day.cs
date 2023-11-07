using CityBuilderGame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;

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

        public Day(ContentManager c)
        {
            CurrentDay = 0;
            NightOrDay = true;
            DayNightTimer = 0;
            onePixelTexture = c.Load<Texture2D>("onePixelNight");

        }



        /// <summary>
        /// update the current game time and whether or not it is day or night
        /// </summary>
        /// <param name="gT"></param>
        /// <returns> the new total food</returns>
        public int Update(GameTime gT, int TotalFood, List<Farmer> f, List<Lumberjack> l)
        {
            DayNightTimer += gT.ElapsedGameTime.TotalSeconds;

            if(NightOrDay == true && DayNightTimer >= LENGTHOFDAY)
            {

                DayNightTimer = 0;
                NightOrDay = false;
            }
            else if(NightOrDay == false && DayNightTimer >= LENGTHOFNIGHT)
            {
                DayNightTimer = 0;
                NightOrDay = true;
                TotalFood = ChangeDay(TotalFood, f, l);
            }
            return TotalFood;
        }

        /// <summary>
        /// changes to the next day
        /// </summary>
        /// <returns> the new total food</returns>
        private int ChangeDay(int TotalFood,  List<Farmer> f, List<Lumberjack> l)
        {
            CurrentDay++;
            TotalFood -= f.Count;
            TotalFood -= l.Count;
            return TotalFood;
        }

        /// <summary>
        /// draws the information for this class
        /// </summary>
        /// <param name="gT"></param>
        /// <param name="sb"></param>
        public void Draw(GameTime gT, SpriteBatch sb, SpriteFont f, Viewport v)
        {
            sb.DrawString(f, $"Current Day:    {CurrentDay}", new Vector2(500, 10), Color.Black, 0, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
            Color nightColor = new Color(0, 0, 160, 128); // Dark blue with 50% transparency

            if (NightOrDay == false)
            {
                sb.Draw(onePixelTexture, v.Bounds, nightColor);
            }
        }


    }
}
