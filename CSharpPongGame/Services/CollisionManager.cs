using System.Drawing;
using System.Collections.Generic;
using PongGame.Entities;

namespace PongGame.Services
{
    /// <summary>
    /// Module for managing collisions in the game
    /// </summary>
    public static class CollisionManager
    {
        /// <summary>
        /// Check if two rectangles collide (AABB collision)
        /// Algorithm: Compare the position and size of the two rectangles on both the X and Y axes.
        /// </summary>
        public static bool CheckCollision(RectangleF rect1, RectangleF rect2)
        {
            return rect1.X < rect2.X + rect2.Width &&
                   rect1.X + rect1.Width > rect2.X &&
                   rect1.Y < rect2.Y + rect2.Height &&
                   rect1.Y + rect1.Height > rect2.Y;
        }

        /// <summary>
        /// Resolve collision between ball and object
        /// Idea: Calculate the overlap on each axis, then push the ball away from the object in the direction of the least overlap.
        /// Then, reflect the ball's velocity based on the normal vector of the collision surface.
        /// </summary>
        public static void ResolveCollision(Ball ball, RectangleF objectBounds, Vector2D normal = null)
        {
            if (normal == null)
                normal = new Vector2D(0, 0);

            // Calculate the distance between the centers of the ball and the object
            float dx = (ball.X + ball.Size / 2f) - (objectBounds.X + objectBounds.Width / 2f);
            float dy = (ball.Y + ball.Size / 2f) - (objectBounds.Y + objectBounds.Height / 2f);

            // Calculate the overlap on each axis
            float halfBallWidth = ball.Size / 2f;
            float halfBallHeight = ball.Size / 2f;
            float halfObjectWidth = objectBounds.Width / 2f;
            float halfObjectHeight = objectBounds.Height / 2f;

            float overlapX = halfBallWidth + halfObjectWidth - System.Math.Abs(dx);
            float overlapY = halfBallHeight + halfObjectHeight - System.Math.Abs(dy);

            // Push the ball away from the object along the axis with a smaller overlap.
            if (overlapX >= overlapY) // Resolve vertical collision
            {
                if (dy < 0)
                {
                    ball.Y -= overlapY; // Reflection along the Y axis
                    normal.X = 0;
                    normal.Y = 1;
                }
                else
                {
                    ball.Y += overlapY;
                    normal.X = 0;
                    normal.Y = -1;
                }
            }
            else // Resolve horizontal collision
            {
                if (dx < 0)
                {
                    ball.X -= overlapX; // Reflection along the X axis
                    normal.X = 1;
                    normal.Y = 0;
                }
                else
                {
                    ball.X += overlapX;
                    normal.X = -1;
                    normal.Y = 0;
                }
            }
            ball.Bounce(normal);
        }

        /// <summary>
        /// Handle all collisions in the game
        /// Includes: collision with the window frame, paddle, moving wall
        /// </summary>
        public static void HandleCollisions(Ball ball, Paddle leftPaddle, Paddle rightPaddle, 
            List<Wall> walls, int windowWidth, int windowHeight, SoundManager soundManager = null, 
            PotionEffectManager potionEffectManager = null)
        {
            // Collisions with window boundaries
            if (ball.Y <= 0)
            {
                ball.Y = 0;
                ball.Bounce(new Vector2D(0, 1)); // Reflection along the Y axis
                soundManager?.PlayEffect(SoundType.WallHit);
            }
            else if (ball.Y + ball.Size >= windowHeight)
            {
                ball.Y = windowHeight - ball.Size;
                ball.Bounce(new Vector2D(0, -1));
                soundManager?.PlayEffect(SoundType.WallHit);
            }

            // Collisions with paddles
            if (CheckCollision(ball.GetBounds(), leftPaddle.GetBounds()))
            {
                ResolveCollision(ball, leftPaddle.GetBounds(), new Vector2D(1, 0));
                ball.Accelerate(leftPaddle.Speed * 0.2f, 0);
                ball.LimitSpeed(20);
                soundManager?.PlayEffect(SoundType.PaddleHit);
                
                // Apply random potion effects when ball hits paddle (20% chance)
                Random random = new Random();
                if (random.NextDouble() < 0.2) // 20% chance
                {
                    potionEffectManager?.ApplyRandomEffects(1, 3.0, soundManager); // 1 effect for 3 seconds
                }
            }
            else if (CheckCollision(ball.GetBounds(), rightPaddle.GetBounds()))
            {
                ResolveCollision(ball, rightPaddle.GetBounds(), new Vector2D(-1, 0));
                ball.Accelerate(-rightPaddle.Speed * 0.2f, 0);
                ball.LimitSpeed(20);
                soundManager?.PlayEffect(SoundType.PaddleHit);
                
                // Apply random potion effects when ball hits paddle (20% chance)
                Random random = new Random();
                if (random.NextDouble() < 0.2) // 20% chance
                {
                    potionEffectManager?.ApplyRandomEffects(1, 3.0, soundManager); // 1 effect for 3 seconds
                }
            }

            // Collisions with walls
            foreach (Wall wall in walls)
            {
                wall.Move();
                if (CheckCollision(ball.GetBounds(), wall.GetBounds()))
                {
                    ResolveCollision(ball, wall.GetBounds());
                    soundManager?.PlayEffect(SoundType.BallHitWall);
                    
                    // Apply random potion effects when ball hits wall (70% chance)
                    Random random = new Random();
                    if (random.NextDouble() < 0.7) // 70% chance
                    {
                        // Có thể áp dụng 1-2 effects ngẫu nhiên
                        int numEffects = random.Next(1, 3); // 1 hoặc 2 effects
                        potionEffectManager?.ApplyRandomEffects(numEffects, 5.0, soundManager); // effects for 5 seconds
                    }
                }
            }
        }
    }
}
