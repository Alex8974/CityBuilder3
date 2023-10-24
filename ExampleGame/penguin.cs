using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExampleGame
{
    internal class penguin : Person
    {
        public override int Health { get; set; } = 8;

        public override void Building()
        {
            throw new NotImplementedException();
        }

        public override void CheckForBuild()
        {
            throw new NotImplementedException();
        }

        public override void Draw(SpriteBatch s, GameTime gT)
        {
            throw new NotImplementedException();
        }
    }
}
