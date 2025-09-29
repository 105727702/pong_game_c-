using System;
using System.Drawing;
using System.Windows.Forms;
using PongGame.Entities;
using PongGame.Services;

namespace PongGame
{
    /// <summary>
    /// Main game form for the Pong game
    /// </summary>
    public partial class MainGameForm : Form
    {
        private const int WINDOW_WIDTH = 800;
        private const int WINDOW_HEIGHT = 600;
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

        private System.Windows.Forms.Timer _gameTimer;
        private bool _gameStarted;

        public MainGameForm()
        {
            InitializeComponent();
            InitializeGame();
        }

        private void InitializeComponent()
        {
            SuspendLayout();

            // MainGameForm
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(WINDOW_WIDTH, WINDOW_HEIGHT);
            Text = "Pong Game";
            BackColor = Color.Black;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            KeyPreview = true;
            DoubleBuffered = true;

            ResumeLayout(false);
        }

        private void InitializeGame()
        {
            // Use actual client size instead of constants to avoid gaps
            int actualWidth = this.ClientSize.Width;
            int actualHeight = this.ClientSize.Height;
            
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

            // Setup game timer
            _gameTimer = new System.Windows.Forms.Timer();
            _gameTimer.Interval = 16; // ~60 FPS
            _gameTimer.Tick += GameTimer_Tick;
            _gameTimer.Start();

            // Event handlers
            KeyDown += MainGameForm_KeyDown;
            KeyUp += MainGameForm_KeyUp;
            MouseClick += MainGameForm_MouseClick;
            Paint += MainGameForm_Paint;
            FormClosing += MainGameForm_FormClosing;
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            if (!_gameStarted || _gameManager.GameOver)
            {
                Invalidate(); // Only redraw UI
                return;
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
                _gameManager.Walls, this.ClientSize.Width, this.ClientSize.Height, _soundManager, _potionEffectManager);

            // Check for scoring
            if (_gameManager.CheckBallOutOfBounds(_soundManager, _potionEffectManager))
            {
                _uiRenderer.CurrentState = GameState.GameOver;
                _uiRenderer.Winner = _scoreboard.LeftScore >= 10 ? 1 : 2;
                _gameStarted = false;
            }

            Invalidate(); // Trigger repaint
        }

        private void MainGameForm_KeyDown(object sender, KeyEventArgs e)
        {
            _inputHandler.HandleKeyDown(e.KeyCode);
        }

        private void MainGameForm_KeyUp(object sender, KeyEventArgs e)
        {
            _inputHandler.HandleKeyUp(e.KeyCode, _leftPaddle, _rightPaddle);
        }

        private void MainGameForm_MouseClick(object sender, MouseEventArgs e)
        {
            if (_uiRenderer.HandleMouseClick(e.X, e.Y))
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

        private void StartGame()
        {
            // Set ball speed based on difficulty
            switch (_uiRenderer.SelectedDifficulty)
            {
                case Difficulty.Easy:
                    _ball.Speed = 6;
                    _ball.Velocity = new Vector2D(8, 8);
                    break;
                case Difficulty.Medium:
                    _ball.Speed = 8;
                    _ball.Velocity = new Vector2D(10, 10);
                    break;
                case Difficulty.Hard:
                    _ball.Speed = 10;
                    _ball.Velocity = new Vector2D(12, 12);
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

        private void MainGameForm_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Draw UI (menu, score, game over screen)
            _uiRenderer.Draw(g, _scoreboard, _potionEffectManager);

            // Draw game objects only when playing
            if (_gameStarted && _uiRenderer.CurrentState == GameState.Playing)
            {
                _ball.Draw(g);
                _leftPaddle.Draw(g);
                _rightPaddle.Draw(g);

                foreach (Wall wall in _gameManager.Walls)
                {
                    wall.Draw(g);
                }
            }
        }

        private void MainGameForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _gameTimer?.Stop();
            _gameTimer?.Dispose();
            _soundManager?.Dispose();
            _uiRenderer?.Dispose();
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // Fill entire client area with black to eliminate any gaps
            using (var brush = new SolidBrush(Color.Black))
            {
                e.Graphics.FillRectangle(brush, this.ClientRectangle);
            }
        }
    }
}
