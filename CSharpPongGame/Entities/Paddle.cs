using System.Drawing;

namespace PongGame.Entities
{
    /// <summary>
    /// Paddle class for the Pong game, representing the player's paddle
    /// </summary>
    public class Paddle
    {
        private const float BASE_SPEED = 5f;
        private const float MAX_SPEED = 10f;
        private const float FORCE = 0.2f;

        public float X { get; set; }
        public float Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Color Color { get; set; }
        public float Speed { get; set; }
        public float StartX { get; private set; }
        public float StartY { get; private set; }

        private readonly int _windowHeight;

        public Paddle(float x, float y, int windowHeight)
        {
            X = x;
            Y = y;
            Width = 20;
            Height = 100;
            Color = Color.White;
            Speed = BASE_SPEED;
            StartX = x;
            StartY = y;
            _windowHeight = windowHeight;
        }

        public void MoveUp()
        {
            Y -= Speed;
            if (Y < 0) Y = 0; // Keep within top boundary
            IncreaseSpeed();
        }

        public void MoveDown()
        {
            Y += Speed;
            if (Y > _windowHeight - Height) Y = _windowHeight - Height; // Keep within bottom boundary
            IncreaseSpeed();
        }

        public void ResetSpeed()
        {
            Speed = BASE_SPEED;
        }

        private void IncreaseSpeed()
        {
            if (Speed < MAX_SPEED)
                Speed += FORCE;
        }

        public void ResetPosition()
        {
            X = StartX;
            Y = StartY;
        }

        /// <summary>
        /// Get the rectangle bounds of the paddle for collision detection
        /// </summary>
        public RectangleF GetBounds()
        {
            return new RectangleF(X, Y, Width, Height);
        }

        /// <summary>
        /// Draw the paddle
        /// </summary>
        public void Draw(Graphics g)
        {
            using (var brush = new SolidBrush(Color))
            {
                g.FillRectangle(brush, X, Y, Width, Height);
            }
        }
    }
}
