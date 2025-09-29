using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using PongGame.Entities;

namespace PongGame.Services
{
    public enum EffectType
    {
        IncreaseSpeedBall,
        EnlargePaddle,
        SlowDownBall
    }

    /// <summary>
    /// Represents a single potion effect with its type, start time, and duration
    /// </summary>
    public class Effect
    {
        public EffectType Type { get; private set; }
        public DateTime StartTime { get; private set; }
        public double Duration { get; private set; }

        public Effect(EffectType type, double duration)
        {
            Type = type;
            StartTime = DateTime.Now;
            Duration = duration;
        }

        /// <summary>
        /// Check if the effect is still active based on its duration
        /// </summary>
        public bool IsActive()
        {
            return (DateTime.Now - StartTime).TotalSeconds < Duration;
        }
    }

    /// <summary>
    /// Manages potion effects applied to the ball and paddles in the game
    /// </summary>
    public class PotionEffectManager
    {
        private readonly Ball _ball;
        private readonly Paddle _leftPaddle;
        private readonly Paddle _rightPaddle;
        private readonly List<Effect> _activeEffects;
        private readonly float _originalBallSpeed;
        private readonly int _originalPaddleHeight;
        private readonly Random _random;

        public List<Effect> ActiveEffects => _activeEffects.ToList();

        public PotionEffectManager(Ball ball, Paddle leftPaddle, Paddle rightPaddle)
        {
            _ball = ball;
            _leftPaddle = leftPaddle;
            _rightPaddle = rightPaddle;
            _activeEffects = new List<Effect>();
            _originalBallSpeed = ball.Speed;
            _originalPaddleHeight = leftPaddle.Height;
            _random = new Random();
        }

        /// <summary>
        /// Apply a random number of effects to the ball and paddles
        /// </summary>
        public void ApplyRandomEffects(int numEffects, double duration, SoundManager soundManager = null)
        {
            var availableEffects = new[] { EffectType.IncreaseSpeedBall, EffectType.EnlargePaddle, EffectType.SlowDownBall };
            bool applied = false;

            for (int i = 0; i < numEffects; i++)
            {
                var effectType = availableEffects[_random.Next(availableEffects.Length)];
                
                // Don't apply the same effect twice
                if (!_activeEffects.Any(effect => effect.Type == effectType))
                {
                    _activeEffects.Add(new Effect(effectType, duration));
                    ApplyEffect(effectType);
                    applied = true;
                }
            }

            if (applied)
            {
                soundManager?.PlayEffect(SoundType.PotionEffect);
            }
        }

        /// <summary>
        /// Update active effects and remove expired ones
        /// </summary>
        public void Update()
        {
            for (int i = _activeEffects.Count - 1; i >= 0; i--)
            {
                var effect = _activeEffects[i];
                if (!effect.IsActive())
                {
                    RemoveEffect(effect.Type);
                    _activeEffects.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Apply the effect to the ball or paddles based on the type of effect
        /// </summary>
        private void ApplyEffect(EffectType type)
        {
            switch (type)
            {
                case EffectType.IncreaseSpeedBall:
                    _ball.Speed += 5f;
                    _ball.NormalizeVelocity();
                    _ball.Color = Color.Red;
                    break;

                case EffectType.EnlargePaddle:
                    _leftPaddle.Height = 150;
                    _rightPaddle.Height = 150;
                    _leftPaddle.Y -= 25;
                    _rightPaddle.Y -= 25;
                    _leftPaddle.Color = Color.Yellow;
                    _rightPaddle.Color = Color.Green;
                    break;

                case EffectType.SlowDownBall:
                    _ball.Speed = 3f;
                    _ball.NormalizeVelocity();
                    _ball.Color = Color.Blue;
                    break;
            }
        }

        /// <summary>
        /// Remove the effect from the ball or paddles based on the type of effect
        /// </summary>
        private void RemoveEffect(EffectType type)
        {
            switch (type)
            {
                case EffectType.IncreaseSpeedBall:
                    _ball.Speed = _originalBallSpeed;
                    _ball.NormalizeVelocity();
                    _ball.Color = Color.White;
                    break;

                case EffectType.EnlargePaddle:
                    _leftPaddle.Height = _originalPaddleHeight;
                    _rightPaddle.Height = _originalPaddleHeight;
                    _leftPaddle.Y += 25;
                    _rightPaddle.Y += 25;
                    _leftPaddle.Color = Color.White;
                    _rightPaddle.Color = Color.White;
                    break;

                case EffectType.SlowDownBall:
                    _ball.Speed = _originalBallSpeed;
                    _ball.NormalizeVelocity();
                    _ball.Color = Color.White;
                    break;
            }
        }

        /// <summary>
        /// Clear all active effects
        /// </summary>
        public void ClearAllEffects()
        {
            foreach (var effect in _activeEffects)
            {
                RemoveEffect(effect.Type);
            }
            _activeEffects.Clear();
        }
    }
}
