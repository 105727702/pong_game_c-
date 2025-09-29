using System;
using System.Media;
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
        private readonly Dictionary<SoundType, SoundPlayer> _soundPlayers;
        private SoundPlayer _currentMusic;

        public SoundManager()
        {
            _soundPlayers = new Dictionary<SoundType, SoundPlayer>();
            InitializeSounds();
        }

        private void InitializeSounds()
        {
            try
            {
                // Initialize built-in system sounds as fallback
                _soundPlayers[SoundType.WallHit] = new SoundPlayer();
                _soundPlayers[SoundType.PaddleHit] = new SoundPlayer();
                _soundPlayers[SoundType.BallHitWall] = new SoundPlayer();
                _soundPlayers[SoundType.BallOut] = new SoundPlayer();
                _soundPlayers[SoundType.MenuMusic] = new SoundPlayer();
                _soundPlayers[SoundType.GameOverMusic] = new SoundPlayer();

                // Try to load custom sound files if they exist
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
                    _soundPlayers[type] = new SoundPlayer(filePath);
                    _soundPlayers[type].Load();
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
                if (_soundPlayers.ContainsKey(type))
                {
                    _soundPlayers[type].Play();
                }
                else
                {
                    // Fallback to system beep for sound effects
                    SystemSounds.Beep.Play();
                }
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
                if (_soundPlayers.ContainsKey(type))
                {
                    _currentMusic = _soundPlayers[type];
                    _currentMusic.PlayLooping();
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
                _currentMusic?.Stop();
                _currentMusic = null;
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
                foreach (var player in _soundPlayers.Values)
                {
                    player?.Dispose();
                }
                _soundPlayers.Clear();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error disposing sound manager: {ex.Message}");
            }
        }
    }
}
