using System;
using SplashKitSDK;
using System.Collections.Generic;
using System.IO;

namespace PongGame.Services
{
    public enum SoundType
    {
        WallHit,
        PaddleHit,
        BallHitWall,
        BallOut,
        MenuMusic,
        GameOverMusic,
        PotionEffect
    }

    /// <summary>
    /// Manages sound effects and background music for the game
    /// </summary>
    public class SoundManager
    {
        private readonly Dictionary<SoundType, SoundEffect> _soundEffects;
        private Music _currentMusic;

        public SoundManager()
        {
            _soundEffects = new Dictionary<SoundType, SoundEffect>();
            InitializeSounds();
        }

        private void InitializeSounds()
        {
            try
            {
                // Try to load custom sound files if they exist, otherwise use default SplashKit sounds
                LoadCustomSounds();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing sounds: {ex.Message}");
            }
        }

        private void LoadCustomSounds()
        {
            string soundDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sounds");
            
            if (Directory.Exists(soundDir))
            {
                TryLoadSound(SoundType.WallHit, Path.Combine(soundDir, "wall_hit.wav"));
                TryLoadSound(SoundType.PaddleHit, Path.Combine(soundDir, "paddle_hit.wav"));
                TryLoadSound(SoundType.BallHitWall, Path.Combine(soundDir, "ball_hit_wall.wav"));
                TryLoadSound(SoundType.BallOut, Path.Combine(soundDir, "ball_out.wav"));
                TryLoadSound(SoundType.MenuMusic, Path.Combine(soundDir, "menu_music.wav"));
                TryLoadSound(SoundType.GameOverMusic, Path.Combine(soundDir, "game_over.wav"));
            }
        }

        private void TryLoadSound(SoundType type, string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    _soundEffects[type] = SplashKit.LoadSoundEffect(type.ToString(), filePath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not load sound {type}: {ex.Message}");
            }
        }

        public void PlayEffect(SoundType type)
        {
            try
            {
                if (_soundEffects.ContainsKey(type))
                {
                    SplashKit.PlaySoundEffect(_soundEffects[type]);
                }
                // SplashKit doesn't have system beep equivalent, so we skip fallback
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error playing sound effect {type}: {ex.Message}");
            }
        }

        public void PlayMusic(SoundType type)
        {
            try
            {
                StopMusic();
                if (_soundEffects.ContainsKey(type))
                {
                    // For music, we'll treat it as a looping sound effect
                    SplashKit.PlaySoundEffect(_soundEffects[type]);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error playing music {type}: {ex.Message}");
            }
        }

        public void StopMusic()
        {
            try
            {
                if (_currentMusic != null)
                {
                    SplashKit.StopMusic();
                    _currentMusic = null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error stopping music: {ex.Message}");
            }
        }

        public void Dispose()
        {
            try
            {
                StopMusic();
                // SplashKit manages sound resources automatically
                _soundEffects.Clear();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error disposing sound manager: {ex.Message}");
            }
        }
    }
}
