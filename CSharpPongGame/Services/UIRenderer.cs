using System;
using System.Drawing;
using System.Windows.Forms;
using PongGame.Entities;

namespace PongGame.Services
{
    public enum GameState
    {
        MainMenu,
        DifficultyMenu,
        Playing,
        GameOver
    }

    public enum Difficulty
    {
        Easy = 0,
        Medium = 1,
        Hard = 2
    }

    /// <summary>
    /// Handles UI rendering and menu interactions
    /// </summary>
    public class UIRenderer
    {
        private readonly int _windowWidth;
        private readonly int _windowHeight;
        private readonly int _trueCenterX; // Đường kẻ giữa thực sự
        private readonly Font _titleFont;
        private readonly Font _buttonFont;
        private readonly Font _scoreFont;

        public GameState CurrentState { get; set; }
        public Difficulty SelectedDifficulty { get; set; }
        public int Winner { get; set; }

        // Button rectangles for click detection
        private RectangleF _startButtonRect;
        private RectangleF _difficultyButtonRect;
        private RectangleF _exitButtonRect;
        private RectangleF _playAgainButtonRect;
        private RectangleF _backToMenuButtonRect;
        private RectangleF[] _difficultyOptionRects;

        public UIRenderer(int windowWidth, int windowHeight)
        {
            _windowWidth = windowWidth;
            _windowHeight = windowHeight;
            _trueCenterX = _windowWidth / 2; // Đường kẻ giữa thực sự ở chính giữa màn hình
            CurrentState = GameState.MainMenu;
            SelectedDifficulty = Difficulty.Medium;
            
            _titleFont = new Font("Arial", 32, FontStyle.Bold);
            _buttonFont = new Font("Arial", 16, FontStyle.Regular);
            _scoreFont = new Font("Arial", 24, FontStyle.Bold);

            InitializeButtonRects();
        }

        private void InitializeButtonRects()
        {
            float buttonWidth = 200;
            float buttonHeight = 40;

            _startButtonRect = new RectangleF(_trueCenterX - buttonWidth / 2, 300, buttonWidth, buttonHeight);
            _difficultyButtonRect = new RectangleF(_trueCenterX - buttonWidth / 2, 360, buttonWidth, buttonHeight);
            _exitButtonRect = new RectangleF(_trueCenterX - buttonWidth / 2, 420, buttonWidth, buttonHeight);
            
            _playAgainButtonRect = new RectangleF(_trueCenterX - buttonWidth / 2, 350, buttonWidth, buttonHeight);
            _backToMenuButtonRect = new RectangleF(_trueCenterX - buttonWidth / 2, 410, buttonWidth, buttonHeight);

            _difficultyOptionRects = new RectangleF[3];
            for (int i = 0; i < 3; i++)
            {
                _difficultyOptionRects[i] = new RectangleF(_trueCenterX - buttonWidth / 2, 250 + i * 60, buttonWidth, buttonHeight);
            }
        }

        public void Draw(Graphics g, Scoreboard scoreboard, PotionEffectManager potionEffectManager = null)
        {
            g.Clear(Color.Black);

            switch (CurrentState)
            {
                case GameState.MainMenu:
                    DrawMainMenu(g);
                    break;
                case GameState.DifficultyMenu:
                    DrawDifficultyMenu(g);
                    break;
                case GameState.Playing:
                    DrawScore(g, scoreboard);
                    break;
                case GameState.GameOver:
                    DrawGameOver(g, scoreboard);
                    break;
            }
        }

        private void DrawMainMenu(Graphics g)
        {
            // Title
            var titleBrush = new SolidBrush(Color.White);
            var titleSize = g.MeasureString("PONG", _titleFont);
            g.DrawString("PONG", _titleFont, titleBrush, 
                _trueCenterX - (titleSize.Width / 2), 150);

            // Buttons
            var buttonBrush = new SolidBrush(Color.Gray);
            var textBrush = new SolidBrush(Color.White);

            // Start button
            g.FillRectangle(buttonBrush, _startButtonRect);
            g.DrawRectangle(Pens.White, Rectangle.Round(_startButtonRect));
            DrawCenteredText(g, "START", _buttonFont, textBrush, _startButtonRect);

            // Difficulty button
            g.FillRectangle(buttonBrush, _difficultyButtonRect);
            g.DrawRectangle(Pens.White, Rectangle.Round(_difficultyButtonRect));
            DrawCenteredText(g, $"DIFFICULTY: {SelectedDifficulty.ToString().ToUpper()}", 
                _buttonFont, textBrush, _difficultyButtonRect);

            // Exit button
            g.FillRectangle(buttonBrush, _exitButtonRect);
            g.DrawRectangle(Pens.White, Rectangle.Round(_exitButtonRect));
            DrawCenteredText(g, "EXIT", _buttonFont, textBrush, _exitButtonRect);

            titleBrush.Dispose();
            buttonBrush.Dispose();
            textBrush.Dispose();
        }

        private void DrawDifficultyMenu(Graphics g)
        {
            var titleBrush = new SolidBrush(Color.White);
            var titleSize = g.MeasureString("SELECT DIFFICULTY", _titleFont);
            g.DrawString("SELECT DIFFICULTY", _titleFont, titleBrush,
                _trueCenterX - (titleSize.Width / 2), 150);

            var buttonBrush = new SolidBrush(Color.Gray);
            var selectedBrush = new SolidBrush(Color.DarkGray);
            var textBrush = new SolidBrush(Color.White);

            string[] difficultyNames = { "EASY", "MEDIUM", "HARD" };
            
            for (int i = 0; i < 3; i++)
            {
                var brush = (Difficulty)i == SelectedDifficulty ? selectedBrush : buttonBrush;
                g.FillRectangle(brush, _difficultyOptionRects[i]);
                g.DrawRectangle(Pens.White, Rectangle.Round(_difficultyOptionRects[i]));
                DrawCenteredText(g, difficultyNames[i], _buttonFont, textBrush, _difficultyOptionRects[i]);
            }

            titleBrush.Dispose();
            buttonBrush.Dispose();
            selectedBrush.Dispose();
            textBrush.Dispose();
        }

        private void DrawScore(Graphics g, Scoreboard scoreboard)
        {
            var textBrush = new SolidBrush(Color.White);
            
            // Vẽ điểm số trái và phải riêng biệt để căn giữa rõ ràng hơn
            string leftScore = scoreboard.LeftScore.ToString();
            string rightScore = scoreboard.RightScore.ToString();
            
            // Căn chỉnh điểm số dựa trên đường giữa thực sự
            int centerX = _windowWidth / 2;
            
            // Vẽ điểm số trái (bên trái đường giữa)
            var leftScoreSize = g.MeasureString(leftScore, _scoreFont);
            g.DrawString(leftScore, _scoreFont, textBrush, 
                (centerX / 2) - (leftScoreSize.Width / 2), 50);
            
            // Vẽ điểm số phải (bên phải đường giữa)
            var rightScoreSize = g.MeasureString(rightScore, _scoreFont);
            g.DrawString(rightScore, _scoreFont, textBrush, 
                centerX + (centerX / 2) - (rightScoreSize.Width / 2), 50);
            
            // Vẽ đường phân cách ở giữa thực sự - đến hết chiều cao
            using (Pen centerPen = new Pen(Color.White, 2))
            {
                for (int y = 0; y <= _windowHeight; y += 20)
                {
                    int lineEnd = Math.Min(y + 10, _windowHeight);
                    g.DrawLine(centerPen, _trueCenterX, y, _trueCenterX, lineEnd);
                }
            }
            
            textBrush.Dispose();
        }



        private void DrawGameOver(Graphics g, Scoreboard scoreboard)
        {
            // Semi-transparent overlay
            var overlayBrush = new SolidBrush(Color.FromArgb(128, 0, 0, 0));
            g.FillRectangle(overlayBrush, 0, 0, _windowWidth, _windowHeight);

            // Winner text
            var titleBrush = new SolidBrush(Color.White);
            string winnerText = Winner == 1 ? "LEFT PLAYER WINS!" : "RIGHT PLAYER WINS!";
            var winnerSize = g.MeasureString(winnerText, _titleFont);
            g.DrawString(winnerText, _titleFont, titleBrush,
                _trueCenterX - (winnerSize.Width / 2), 200);

            // Final score
            string finalScore = $"Final Score: {scoreboard.LeftScore} - {scoreboard.RightScore}";
            var scoreSize = g.MeasureString(finalScore, _scoreFont);
            g.DrawString(finalScore, _scoreFont, titleBrush,
                _trueCenterX - (scoreSize.Width / 2), 260);

            // Buttons
            var buttonBrush = new SolidBrush(Color.Gray);
            var textBrush = new SolidBrush(Color.White);

            g.FillRectangle(buttonBrush, _playAgainButtonRect);
            g.DrawRectangle(Pens.White, Rectangle.Round(_playAgainButtonRect));
            DrawCenteredText(g, "PLAY AGAIN", _buttonFont, textBrush, _playAgainButtonRect);

            g.FillRectangle(buttonBrush, _backToMenuButtonRect);
            g.DrawRectangle(Pens.White, Rectangle.Round(_backToMenuButtonRect));
            DrawCenteredText(g, "MAIN MENU", _buttonFont, textBrush, _backToMenuButtonRect);

            overlayBrush.Dispose();
            titleBrush.Dispose();
            buttonBrush.Dispose();
            textBrush.Dispose();
        }

        private void DrawCenteredText(Graphics g, string text, Font font, Brush brush, RectangleF rect)
        {
            var textSize = g.MeasureString(text, font);
            float x = rect.X + (rect.Width - textSize.Width) / 2;
            float y = rect.Y + (rect.Height - textSize.Height) / 2;
            g.DrawString(text, font, brush, x, y);
        }

        public bool HandleMouseClick(float mouseX, float mouseY)
        {
            var mousePoint = new PointF(mouseX, mouseY);

            switch (CurrentState)
            {
                case GameState.MainMenu:
                    if (_startButtonRect.Contains(mousePoint))
                        return true; // Start game
                    if (_difficultyButtonRect.Contains(mousePoint))
                    {
                        CurrentState = GameState.DifficultyMenu;
                        return false;
                    }
                    if (_exitButtonRect.Contains(mousePoint))
                        Application.Exit();
                    break;

                case GameState.DifficultyMenu:
                    for (int i = 0; i < 3; i++)
                    {
                        if (_difficultyOptionRects[i].Contains(mousePoint))
                        {
                            SelectedDifficulty = (Difficulty)i;
                            CurrentState = GameState.MainMenu;
                            return false;
                        }
                    }
                    break;

                case GameState.GameOver:
                    if (_playAgainButtonRect.Contains(mousePoint))
                        return true; // Restart game
                    if (_backToMenuButtonRect.Contains(mousePoint))
                    {
                        CurrentState = GameState.MainMenu;
                        return false;
                    }
                    break;
            }

            return false;
        }

        public void Dispose()
        {
            _titleFont?.Dispose();
            _buttonFont?.Dispose();
            _scoreFont?.Dispose();
        }
    }
}
