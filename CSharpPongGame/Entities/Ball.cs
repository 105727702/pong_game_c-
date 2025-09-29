using System;
using System.Drawing;

namespace PongGame.Entities
{
    public class Ball
    {
        public float X { get; set; }
        public float Y { get; set; }
        public int Size { get; set; }
        public Color Color { get; set; }
        public Vector2D Velocity { get; set; }
        public float Speed { get; set; }

        private readonly int _windowWidth;
        private readonly int _windowHeight;
        private readonly Random _random = new Random();
        private float _baseSpeed;

        public Ball(int windowWidth, int windowHeight)
        {
            _windowWidth = windowWidth;
            _windowHeight = windowHeight;
            
            // Khởi tạo tại vị trí random trên đường kẻ giữa
            int trueCenterX = windowWidth / 2 + 120;
            X = trueCenterX;
            Y = _random.Next(100, windowHeight - 100);
            
            Size = 10;
            Color = Color.White;
            Velocity = new Vector2D(4, 4);
            Speed = 4;
            _baseSpeed = 4; // Lưu tốc độ cơ bản
        }

        public void Move()
        {
            X += Velocity.X;
            Y += Velocity.Y;
        }

        /// <summary>
        /// Reflect the velocity vector across the normal
        /// v' = v - 2 * (v dot n) * n
        /// </summary>
        public void Bounce(Vector2D surfaceNormal)
        {
            Vector2D incidentDirection = Velocity.Copy();
            float dotProduct = incidentDirection.DotProduct(surfaceNormal);
            Vector2D reflection = surfaceNormal.Copy().Multiply(2 * dotProduct);
            Velocity.Subtract(reflection);
        }

        /// <summary>
        /// Reset the ball's position to a random location on the center line with a random velocity
        /// </summary>
        public void ResetPosition()
        {
            // Đường kẻ giữa thực sự tại x = 520
            int trueCenterX = _windowWidth / 2 + 120;
            X = trueCenterX;
            
            // Random Y position trên đường kẻ giữa (tránh quá gần top/bottom)
            Y = _random.Next(100, _windowHeight - 100);
            
            int direction = _random.Next(0, 2) == 0 ? 1 : -1;
            Velocity = new Vector2D(4 * direction, 4 * direction);
            Speed = _baseSpeed;
        }

        /// <summary>
        /// Accelerate the ball by a given amount in x and y directions
        /// </summary>
        public void Accelerate(float ax, float ay)
        {
            Velocity.Add(new Vector2D(ax, ay));
        }

        /// <summary>
        /// Limit the speed of the ball to a maximum value
        /// </summary>
        public void LimitSpeed(float maxSpeed)
        {
            Velocity.Limit(maxSpeed);
        }

        /// <summary>
        /// Set the ball's base speed (used for difficulty settings)
        /// </summary>
        public void SetBaseSpeed(float speed)
        {
            _baseSpeed = speed;
            Speed = speed;
        }

        /// <summary>
        /// Reset speed to base speed (used when effects expire)
        /// </summary>
        public void ResetSpeed()
        {
            Speed = _baseSpeed;
        }

        /// <summary>
        /// Normalize velocity to maintain current speed
        /// </summary>
        public void NormalizeVelocity()
        {
            float magnitude = Velocity.Magnitude;
            if (magnitude > 0)
            {
                Velocity.X = (Velocity.X / magnitude) * Speed;
                Velocity.Y = (Velocity.Y / magnitude) * Speed;
            }
        }

        /// <summary>
        /// Get the rectangle bounds of the ball for collision detection
        /// </summary>
        public RectangleF GetBounds()
        {
            return new RectangleF(X, Y, Size, Size);
        }

        /// <summary>
        /// Draw the ball
        /// </summary>
        public void Draw(Graphics g)
        {
            using (var brush = new SolidBrush(Color))
            {
                g.FillRectangle(brush, X, Y, Size, Size);
            }
        }
    }
}

