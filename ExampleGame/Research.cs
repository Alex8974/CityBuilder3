using ExampleGame.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExampleGame
{
    /// <summary>
    /// a class defining the amount of research the player has done
    /// </summary>
    public class Research
    {
        public bool PlanterResearch = false;
        

        public void Update(GameTime gT, KeyboardState curk, KeyboardState prevk, ref int TotalFood, ref int TotalWood)
        {
            if (curk.IsKeyDown(Keys.T) && !prevk.IsKeyDown(Keys.T) && TotalFood >= 50 && TotalWood >= 50) 
            {
                IncreaseHouseSize();
                TotalWood -= 50;
                TotalFood -= 50;
            }

            //if () PlanterResearch = true;
        }

        private void IncreaseHouseSize()
        {
            BuildingScreen.houseCapacity++;
        }



    }
}
