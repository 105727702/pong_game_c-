using System;
using SplashKitSDK;
using PongGame.Entities;
using PongGame.Services;

namespace PongGame
{
    /// <summary>
    /// Main game form for the Pong game
    /// </summary>
    public class MainGameForm
    {
        private const int WINDOW_WIDTH = 1200;
        private const int WINDOW_HEIGHT = 800;
        private const int NUM_WALLS = 4;
        private const int MIN_WALL_DISTANCE = 60;

        private Ball _ball;
        private Paddle _leftPaddle;
        private Paddle _rightPaddle;
        private Scoreboard _scoreboard;
        private GameManager _gameManager;
        private InputHandler _inputHandler;
        private SoundManager _soundManager;
        private UIRenderer _uiRenderer;
        private PotionEffectManager _potionEffectManager;

        private bool _gameStarted;

        public MainGameForm()
        {
            InitializeGame();
        }

        private void InitializeGame()
        {
            // Use actual window size
            int actualWidth = WINDOW_WIDTH;
            int actualHeight = WINDOW_HEIGHT;
            
            // Initialize game entities
            _ball = new Ball(actualWidth, actualHeight);
            _leftPaddle = new Paddle(30, 250, actualHeight);
            _rightPaddle = new Paddle(actualWidth - 50, 250, actualHeight); // Đặt paddle phải sát bên phải
            _scoreboard = new Scoreboard();

            // Initialize services
            _gameManager = new GameManager(_ball, _leftPaddle, _rightPaddle, _scoreboard, actualWidth, actualHeight);
            _inputHandler = new InputHandler();
            _soundManager = new SoundManager();
            _uiRenderer = new UIRenderer(actualWidth, actualHeight);
            _potionEffectManager = new PotionEffectManager(_ball, _leftPaddle, _rightPaddle);

            // Generate initial walls (hidden)
            _gameManager.Walls = _gameManager.GenerateWalls(NUM_WALLS, MIN_WALL_DISTANCE);

            // Initialize game state
            _gameStarted = false;
            _soundManager.PlayMusic(SoundType.MenuMusic);
        }

        public void Update()
        {
            if (!_gameStarted || _gameManager.GameOver)
            {
                return; // Only handle input for menu navigation
            }

            // Update game objects
            _ball.Move();
            _inputHandler.UpdatePaddleMovement(_leftPaddle, _rightPaddle);

            // Update potion effects
            _potionEffectManager.Update();

            // Update walls based on current score for progressive difficulty
            _gameManager.UpdateWallsBasedOnScore(MIN_WALL_DISTANCE);

            // Handle collisions
            CollisionManager.HandleCollisions(_ball, _leftPaddle, _rightPaddle, 
                _gameManager.Walls, WINDOW_WIDTH, WINDOW_HEIGHT, _soundManager, _potionEffectManager);

            // Check for scoring
            if (_gameManager.CheckBallOutOfBounds(_soundManager, _potionEffectManager))
            {
                _uiRenderer.CurrentState = GameState.GameOver;
                _uiRenderer.Winner = _scoreboard.LeftScore >= 1 ? 1 : 2;
                _gameStarted = false;
            }
        }

        public void HandleInput()
        {
            // Handle mouse input
            if (SplashKit.MouseClicked(MouseButton.LeftButton))
            {
                Point2D mousePos = SplashKit.MousePosition();
                if (_uiRenderer.HandleMouseClick((float)mousePos.X, (float)mousePos.Y))
                {
                    if (_uiRenderer.CurrentState == GameState.MainMenu)
                    {
                        StartGame();
                    }
                    else if (_uiRenderer.CurrentState == GameState.GameOver)
                    {
                        RestartGame();
                    }
                }
            }

            // Handle keyboard input
            if (_gameStarted && _uiRenderer.CurrentState == GameState.Playing)
            {
                _inputHandler.HandleKeyInput(_leftPaddle, _rightPaddle);
            }
        }

        private void StartGame()
        {
            // Set ball speed based on difficulty
            switch (_uiRenderer.SelectedDifficulty)
            {
                case Difficulty.Easy:
                    _ball.SetBaseSpeed(4); // Sử dụng magnitude thực tế của vector (4,4)
                    break;
                case Difficulty.Medium:
                    _ball.SetBaseSpeed(5); // Magnitude của vector (5,5) ≈ 7.07, nhưng sẽ được normalize
                    break;
                case Difficulty.Hard:
                    _ball.SetBaseSpeed(6); // Magnitude của vector (6,6) ≈ 8.49, nhưng sẽ được normalize
                    break;
            }

            _soundManager.StopMusic();
            _gameManager.RestartGame(NUM_WALLS, MIN_WALL_DISTANCE);
            _gameManager.StartGame();
            _potionEffectManager.ClearAllEffects(); // Clear all potion effects when starting new game
            _uiRenderer.CurrentState = GameState.Playing;
            _gameStarted = true;
        }

        private void RestartGame()
        {
            _gameManager.RestartGame(NUM_WALLS, MIN_WALL_DISTANCE);
            _gameManager.StartGame();
            _potionEffectManager.ClearAllEffects(); // Clear all potion effects when restarting
            _uiRenderer.CurrentState = GameState.Playing;
            _gameStarted = true;
            _soundManager.StopMusic();
        }

        public void Render()
        {
            // Draw UI (menu, score, game over screen)
            _uiRenderer.Draw(_scoreboard, _potionEffectManager);

            // Draw game objects only when playing
            if (_gameStarted && _uiRenderer.CurrentState == GameState.Playing)
            {
                _ball.Draw();
                _leftPaddle.Draw();
                _rightPaddle.Draw();

                foreach (Wall wall in _gameManager.Walls)
                {
                    wall.Draw();
                }
            }
        }

        public void Dispose()
        {
            _soundManager?.Dispose();
            _uiRenderer?.Dispose();
        }
    }
}
