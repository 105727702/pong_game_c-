using System.Drawing;
using System.Collections.Generic;
using System;

namespace PongGame.Entities
{
    /// <summary>
    /// Wall class for the Pong game, representing a wall that moves vertically
    /// </summary>
    public class Wall
    {
        public const int WALL_WIDTH = 10;
        public const int WALL_HEIGHT = 100;

        public float X { get; set; }
        public float Y { get; set; }
        public int Width => WALL_WIDTH;
        public int Height => WALL_HEIGHT;
        public Color Color { get; set; }
        public float YSpeed { get; set; }

        private readonly int _windowHeight;
        private readonly Random _random = new Random();

        public Wall(float x, float y, int windowHeight, float speedMultiplier = 1.0f)
        {
            X = x;
            Y = y;
            Color = Color.Gray;
            // Base speed increased by number of walls
            float baseSpeed = _random.Next(1, 2) * speedMultiplier;
            YSpeed = Math.Max(baseSpeed, 2.0f); // Minimum speed of 2
            _windowHeight = windowHeight;
        }

        public void Move()
        {
            Y += YSpeed;
            if (Y <= 0 || Y + Height >= _windowHeight)
            {
                YSpeed *= -1;
            }
        }

        /// <summary>
        /// Check if the new wall position is valid (doesn't overlap with existing walls)
        /// </summary>
        public static bool IsValidPosition(float newX, float newY, List<Wall> existingWalls, int minDistance)
        {
            foreach (Wall wall in existingWalls)
            {
                if (newX < wall.X + wall.Width + minDistance &&
                    newX + WALL_WIDTH + minDistance > wall.X &&
                    newY < wall.Y + wall.Height + minDistance &&
                    newY + WALL_HEIGHT + minDistance > wall.Y)
                {
                    return false; // Overlapping or too close
                }
            }
            return true; // Valid position
        }

        /// <summary>
        /// Get the rectangle bounds of the wall for collision detection
        /// </summary>
        public RectangleF GetBounds()
        {
            return new RectangleF(X, Y, Width, Height);
        }

        /// <summary>
        /// Draw the wall
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
