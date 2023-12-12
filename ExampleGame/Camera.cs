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
            backgroundWidth = width*2;
            backgroundWidth -= 160;
            backgroundHeight = height;
            backgroundHeight += 400;
            
            UpdateTransform(viewport);
        }

        public void UpdateTransform(Viewport viewport)
        {
            Transform = Matrix.CreateTranslation(-Position.X, -Position.Y, 0) *
                        Matrix.CreateScale(Scale) *
                        Matrix.CreateTranslation(viewport.Width / 2, viewport.Height / 2, 0);
        }

        public void Move(Vector2 delta)
        {
            Position += delta;

            // Calculate the screen borders in logical coordinates
            float leftBorder = viewport.Width / 2 / Scale;
            float rightBorder = backgroundWidth - viewport.Width / 2 / Scale;
            float topBorder = viewport.Height / 2 / Scale;
            float bottomBorder = backgroundHeight - viewport.Height / 2 / Scale;

            // Clamp the camera position within the borders
            Position.X = MathHelper.Clamp(Position.X, leftBorder, rightBorder);
            Position.Y = MathHelper.Clamp(Position.Y, topBorder, bottomBorder);
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
