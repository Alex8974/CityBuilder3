using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct2D1.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Taskbar;

namespace ExampleGame
{
    public class Camera
    {
        public Matrix Transform { get; private set; }

        /// <summary>
        /// the middle of the screen
        /// </summary>
        public Vector2 Position;
        public float Scale { get; private set; }

        private int backgroundWidth;
        private int backgroundHeight;

        private float minScale = 1.0f;
        private float maxScale = 2.0f;

        Viewport viewport;

        public Camera(Viewport viewport, int width, int height)
        {
            this.viewport = viewport;
            Scale = 1.0f;
            Position = new Vector2(viewport.Width /2, viewport.Height/2);
            backgroundWidth = width;
            backgroundHeight = height;
            UpdateTransform(viewport);
        }

        public void UpdateTransform(Viewport viewport)
        {
            Transform = Matrix.CreateTranslation(-Position.X, -Position.Y, 0) *
                        Matrix.CreateScale(Scale) *
                        Matrix.CreateTranslation(viewport.Width / 2, viewport.Height / 2, 0);
        }

        /// <summary>
        /// moves the camera screen
        /// </summary>
        /// <param name="delta">how you want the camera to move</param>
        public void Move(Vector2 delta)
        {

            Position += delta;
            float SideofScreen;
            float topbottomScreen;
            if (Position.X <= viewport.Width / 2) SideofScreen = Position.X - (viewport.Width / 2/ Scale);
            else SideofScreen = Position.X + (viewport.Width / 2 / Scale);

            if (Position.Y <= viewport.Height / 2) topbottomScreen = Position.Y - (viewport.Height / 2 / Scale);
            else topbottomScreen = Position.Y + (viewport.Height / 2 / Scale);

            float holdx = MathHelper.Clamp(SideofScreen, 0, backgroundWidth);
            float holdy = MathHelper.Clamp(topbottomScreen, 0, backgroundHeight);


            if (Position.X <= viewport.Width / 2)
            {
                if (Position.Y <= viewport.Height / 2) Position = new Vector2(holdx + (viewport.Width / 2 / Scale), holdy + (viewport.Height / 2 / Scale));
                else Position = new Vector2(holdx + (viewport.Width / 2 / Scale), holdy - (viewport.Height / 2 / Scale));
            }
            else
            {
                if (Position.Y <= viewport.Height / 2) Position = new Vector2(holdx - (viewport.Width / 2 / Scale), holdy + (viewport.Height / 2 / Scale));
                else Position = new Vector2(holdx - (viewport.Width / 2 / Scale), holdy - (viewport.Height / 2 / Scale));
            }
                
        }

        public void Center(Viewport viewport)
        {
            Position = new Vector2(viewport.Width / 2, viewport.Height / 2);
        }

        public void ZoomIn(float factor)
        {
            Scale *= factor;
            Scale = Math.Min(Scale, maxScale);
        }

        public void ZoomOut(float factor)
        {
            Scale /= 1.1f; // Adjust the zoom factor as needed
            Scale = Math.Max(Scale, minScale); // Ensure a minimum scale
        }
    }

}
