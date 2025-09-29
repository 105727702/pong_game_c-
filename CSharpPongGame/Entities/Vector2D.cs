using System;

namespace PongGame.Entities
{
    /// <summary>
    /// A simple 2D vector class for representing velocity and position
    /// </summary>
    public class Vector2D
    {
        public float X { get; set; }
        public float Y { get; set; }

        /// <summary>
        /// Initialize the vector with x and y coordinates
        /// </summary>
        public Vector2D(float x = 0, float y = 0)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Add another vector to this one
        /// </summary>
        public Vector2D Add(Vector2D other)
        {
            X += other.X;
            Y += other.Y;
            return this;
        }

        /// <summary>
        /// Subtract another vector from this one
        /// </summary>
        public Vector2D Subtract(Vector2D other)
        {
            X -= other.X;
            Y -= other.Y;
            return this;
        }

        /// <summary>
        /// Multiply this vector by a scalar
        /// </summary>
        public Vector2D Multiply(float scalar)
        {
            X *= scalar;
            Y *= scalar;
            return this;
        }

        /// <summary>
        /// Calculate the magnitude (length) of the vector
        /// </summary>
        public float Magnitude => (float)Math.Sqrt(X * X + Y * Y);

        /// <summary>
        /// Normalize the vector to unit length
        /// </summary>
        public Vector2D Normalize()
        {
            float mag = Magnitude;
            if (mag > 0)
            {
                X /= mag;
                Y /= mag;
            }
            return this;
        }

        /// <summary>
        /// Limit the magnitude of the vector to a maximum value
        /// </summary>
        public Vector2D Limit(float max)
        {
            if (Magnitude > max)
            {
                return Normalize().Multiply(max);
            }
            return this;
        }

        /// <summary>
        /// Calculate the dot product with another vector
        /// </summary>
        public float DotProduct(Vector2D other)
        {
            return X * other.X + Y * other.Y;
        }

        /// <summary>
        /// Create a copy of this vector
        /// </summary>
        public Vector2D Copy()
        {
            return new Vector2D(X, Y);
        }
    }
}
