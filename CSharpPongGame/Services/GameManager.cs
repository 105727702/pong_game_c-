using System;
using System.Collections.Generic;
using System.Linq;
using PongGame.Entities;

namespace PongGame.Services
{
    /// <summary>
    /// Manages the game state, including ball, paddles, scoreboard, and walls
    /// </summary>
    public class GameManager
    {
        public List<Wall> Walls { get; set; }
        public bool GameOver { get; set; }

        private readonly Ball _ball;
        private readonly Paddle _leftPaddle;
        private readonly Paddle _rightPaddle;
        private readonly Scoreboard _scoreboard;
        private readonly int _windowWidth;
        private readonly int _windowHeight;
        private readonly Random _random;

        public GameManager(Ball ball, Paddle leftPaddle, Paddle rightPaddle, Scoreboard scoreboard, 
            int windowWidth, int windowHeight)
        {
            _ball = ball;
            _leftPaddle = leftPaddle;
            _rightPaddle = rightPaddle;
            _scoreboard = scoreboard;
            _windowWidth = windowWidth;
            _windowHeight = windowHeight;
            _random = new Random();
            Walls = new List<Wall>();
            GameOver = false;
        }

        public void StartGame()
        {
            _scoreboard.Start();
        }

        /// <summary>
        /// Handle game over logic
        /// </summary>
        public void HandleGameOver(int winner, SoundManager soundManager)
        {
            GameOver = true;
            soundManager?.StopMusic();
            soundManager?.PlayMusic(SoundType.GameOverMusic);
        }

        /// <summary>
        /// Restart the game with a specified number of walls and a minimum distance between them
        /// </summary>
        public void RestartGame(int numWalls, int minDistance)
        {
            _scoreboard.Reset();
            _ball.ResetPosition();
            _leftPaddle.ResetPosition();
            _rightPaddle.ResetPosition();
            
            Walls.Clear();
            Walls = GenerateWalls(numWalls, minDistance);
            GameOver = false;
        }

        /// <summary>
        /// Generate walls with a minimum distance between them
        /// </summary>
        public List<Wall> GenerateWalls(int numWalls, int minDistance)
        {
            var newWalls = new List<Wall>();
            // Calculate speed multiplier based on number of walls
            // More walls = faster movement for increased difficulty
            float speedMultiplier = 1.0f + (numWalls - 4) * 0.3f; // Base 4 walls, +30% speed per additional wall
            
            // Căn chỉnh theo đường giữa mới ở x = 520
            // Paddle trái ở x=30 (width=20), paddle phải ở x=1010 (width=20)
            // Vùng chơi từ x=60 (sau paddle trái) đến x=980 (trước paddle phải)
            float playableStartX = 60f;
            float playableEndX = 980f; // Căn chỉnh với vùng hiển thị thực tế
            float playableWidth = playableEndX - playableStartX;
            float colWidth = playableWidth / numWalls;
            var xPositions = new List<float>();

            for (int i = 0; i < numWalls; i++)
            {
                // Phân bố walls đều trong vùng chơi thực tế
                float x = playableStartX + i * colWidth + colWidth / 2f;
                xPositions.Add(x);
            }

            foreach (float x in xPositions)
            {
                int attempts = 0;
                float y;
                
                do
                {
                    y = _random.Next(80, _windowHeight - Wall.WALL_HEIGHT - 80); // Tăng margin
                    attempts++;
                } while (newWalls.Any(w => Math.Abs(w.Y - y) < minDistance) && attempts <= 100);

                if (attempts <= 100)
                {
                    newWalls.Add(new Wall(x, y, _windowHeight, speedMultiplier));
                }
            }

            return newWalls;
        }

        /// <summary>
        /// Calculate number of walls based on current score for progressive difficulty
        /// </summary>
        public int CalculateWallCount(int totalScore, int baseWalls = 4)
        {
            // Add 1 wall for every 4 points scored (total score from both players)
            int additionalWalls = totalScore / 4;
            return Math.Min(baseWalls + additionalWalls, 6); // Max 6 walls
        }

        /// <summary>
        /// Update walls based on current score for progressive difficulty
        /// </summary>
        public void UpdateWallsBasedOnScore(int minDistance)
        {
            int totalScore = _scoreboard.LeftScore + _scoreboard.RightScore;
            int newWallCount = CalculateWallCount(totalScore);
            
            // Only regenerate if wall count should change
            if (Walls.Count != newWallCount)
            {
                Walls = GenerateWalls(newWallCount, minDistance);
            }
        }

        /// <summary>
        /// Check if the ball is out of bounds and handle scoring
        /// </summary>
        public bool CheckBallOutOfBounds(SoundManager soundManager, PotionEffectManager potionEffectManager = null)
        {
            // Left boundary - behind left paddle (x=30, width=20)
            if (_ball.X < 10) 
            {
                soundManager?.PlayEffect(SoundType.BallOut);
                _scoreboard.RightPoint();
                _ball.ResetPosition();
                
                // Reset all potion effects when ball goes out of bounds
                potionEffectManager?.ClearAllEffects();
                
                if (_scoreboard.RightScore >= 10)
                {
                    HandleGameOver(2, soundManager);
                    return true;
                }
            }
            // Right boundary - dựa trên vùng hiển thị thực tế (sau x=980)
            else if (_ball.X > 1040) 
            {
                soundManager?.PlayEffect(SoundType.BallOut);
                _scoreboard.LeftPoint();
                _ball.ResetPosition();
                
                // Reset all potion effects when ball goes out of bounds
                potionEffectManager?.ClearAllEffects();
                
                if (_scoreboard.LeftScore >= 10)
                {
                    HandleGameOver(1, soundManager);
                    return true;
                }
            }
            
            return false;
        }
    }
}

