using System;
using SplashKitSDK;
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
        private Rectangle _startButtonRect;
        private Rectangle _difficultyButtonRect;
        private Rectangle _exitButtonRect;
        private Rectangle _playAgainButtonRect;
        private Rectangle _backToMenuButtonRect;
        private Rectangle[] _difficultyOptionRects;

        public UIRenderer(int windowWidth, int windowHeight)
        {
            _windowWidth = windowWidth;
            _windowHeight = windowHeight;
            _trueCenterX = _windowWidth / 2; // Đường kẻ giữa thực sự ở chính giữa màn hình
            CurrentState = GameState.MainMenu;
            SelectedDifficulty = Difficulty.Medium;
            
            _titleFont = SplashKit.FontNamed("default");
            _buttonFont = SplashKit.FontNamed("default");
            _scoreFont = SplashKit.FontNamed("default");

            InitializeButtonRects();
        }

        private void InitializeButtonRects()
        {
            float buttonWidth = 300; // Tăng từ 200 lên 300
            float buttonHeight = 60; // Tăng từ 40 lên 60

            _startButtonRect = SplashKit.RectangleFrom(_trueCenterX - buttonWidth / 2, 280, buttonWidth, buttonHeight);
            _difficultyButtonRect = SplashKit.RectangleFrom(_trueCenterX - buttonWidth / 2, 360, buttonWidth, buttonHeight);
            _exitButtonRect = SplashKit.RectangleFrom(_trueCenterX - buttonWidth / 2, 440, buttonWidth, buttonHeight);
            
            _playAgainButtonRect = SplashKit.RectangleFrom(_trueCenterX - buttonWidth / 2, 300, buttonWidth, buttonHeight);
            _backToMenuButtonRect = SplashKit.RectangleFrom(_trueCenterX - buttonWidth / 2, 380, buttonWidth, buttonHeight);

            _difficultyOptionRects = new Rectangle[3];
            for (int i = 0; i < 3; i++)
            {
                _difficultyOptionRects[i] = SplashKit.RectangleFrom(_trueCenterX - buttonWidth / 2, 220 + i * 80, buttonWidth, buttonHeight);
            }
        }

        public void Draw(Scoreboard scoreboard, PotionEffectManager potionEffectManager = null)
        {
            SplashKit.ClearScreen(Color.Black);

            switch (CurrentState)
            {
                case GameState.MainMenu:
                    DrawMainMenu();
                    break;
                case GameState.DifficultyMenu:
                    DrawDifficultyMenu();
                    break;
                case GameState.Playing:
                    DrawScore(scoreboard);
                    break;
                case GameState.GameOver:
                    DrawGameOver(scoreboard);
                    break;
            }
        }

        private void DrawMainMenu()
        {
            // Title - much larger text
            string title = "PONG";
            SplashKit.DrawText(title, Color.White, "default", 96,
                (double)(_trueCenterX - 30), 100.0);

            // Start button
            SplashKit.FillRectangle(Color.Gray, _startButtonRect);
            SplashKit.DrawRectangle(Color.White, _startButtonRect);
            DrawCenteredText("START", _buttonFont, Color.White, _startButtonRect, 24);

            // Difficulty button
            SplashKit.FillRectangle(Color.Gray, _difficultyButtonRect);
            SplashKit.DrawRectangle(Color.White, _difficultyButtonRect);
            DrawCenteredText($"DIFFICULTY: {SelectedDifficulty.ToString().ToUpper()}", 
                _buttonFont, Color.White, _difficultyButtonRect, 20);

            // Exit button
            SplashKit.FillRectangle(Color.Gray, _exitButtonRect);
            SplashKit.DrawRectangle(Color.White, _exitButtonRect);
            DrawCenteredText("EXIT", _buttonFont, Color.White, _exitButtonRect, 24);
        }

        private void DrawDifficultyMenu()
        {
            string title = "SELECT DIFFICULTY";
            SplashKit.DrawText(title, Color.White, "default", 64,
                (double)(_trueCenterX - 200), 100.0);

            string[] difficultyNames = { "EASY", "MEDIUM", "HARD" };
            
            for (int i = 0; i < 3; i++)
            {
                Color buttonColor = (Difficulty)i == SelectedDifficulty ? Color.DarkGray : Color.Gray;
                SplashKit.FillRectangle(buttonColor, _difficultyOptionRects[i]);
                SplashKit.DrawRectangle(Color.White, _difficultyOptionRects[i]);
                DrawCenteredText(difficultyNames[i], _buttonFont, Color.White, _difficultyOptionRects[i], 32);
            }
        }

        private void DrawScore(Scoreboard scoreboard)
        {
            // Vẽ điểm số trái và phải riêng biệt để căn giữa rõ ràng hơn với font size lớn hơn
            string leftScore = scoreboard.LeftScore.ToString();
            string rightScore = scoreboard.RightScore.ToString();
            
            // Căn chỉnh điểm số dựa trên đường giữa thực sự
            int centerX = _windowWidth / 2;
            
            // Vẽ điểm số trái (bên trái đường giữa) - font size 64
            SplashKit.DrawText(leftScore, Color.White, "default", 64,
                (double)((centerX / 2) - 30), 30.0);
            
            // Vẽ điểm số phải (bên phải đường giữa) - font size 64
            SplashKit.DrawText(rightScore, Color.White, "default", 64,
                (double)(centerX + (centerX / 2) - 30), 30.0);
            
            // Vẽ đường phân cách ở giữa thực sự - đến hết chiều cao với độ dày lớn hơn
            for (int y = 0; y <= _windowHeight; y += 20)
            {
                int lineEnd = Math.Min(y + 10, _windowHeight);
                SplashKit.DrawLine(Color.White, _trueCenterX - 1, y, _trueCenterX - 1, lineEnd);
                SplashKit.DrawLine(Color.White, _trueCenterX, y, _trueCenterX, lineEnd);
                SplashKit.DrawLine(Color.White, _trueCenterX + 1, y, _trueCenterX + 1, lineEnd);
            }
        }



        private void DrawGameOver(Scoreboard scoreboard)
        {
            // Semi-transparent overlay
            SplashKit.FillRectangle(Color.RGBAColor(0, 0, 0, 128), 0, 0, _windowWidth, _windowHeight);

            // Winner text - much larger
            string winnerText = Winner == 1 ? "LEFT PLAYER WINS!" : "RIGHT PLAYER WINS!";
            SplashKit.DrawText(winnerText, Color.White, "default", 48,
                (double)(_trueCenterX - 180), 150.0);

            // Final score - larger
            string finalScore = $"Final Score: {scoreboard.LeftScore} - {scoreboard.RightScore}";
            SplashKit.DrawText(finalScore, Color.White, "default", 32,
                (double)(_trueCenterX - 140), 220.0);

            // Buttons
            SplashKit.FillRectangle(Color.Gray, _playAgainButtonRect);
            SplashKit.DrawRectangle(Color.White, _playAgainButtonRect);
            DrawCenteredText("PLAY AGAIN", _buttonFont, Color.White, _playAgainButtonRect, 24);

            SplashKit.FillRectangle(Color.Gray, _backToMenuButtonRect);
            SplashKit.DrawRectangle(Color.White, _backToMenuButtonRect);
            DrawCenteredText("MAIN MENU", _buttonFont, Color.White, _backToMenuButtonRect, 24);
        }

        private void DrawCenteredText(string text, Font font, Color color, Rectangle rect, int fontSize = 18)
        {
            // Calculate approximate text width based on font size
            double textWidth = text.Length * fontSize * 0.6; // Rough estimation
            double x = rect.X + (rect.Width / 2) - (textWidth / 2);
            double y = rect.Y + (rect.Height / 2) - (fontSize / 2);
            SplashKit.DrawText(text, color, "default", fontSize, x, y);
        }

        public bool HandleMouseClick(float mouseX, float mouseY)
        {
            Point2D mousePoint = SplashKit.PointAt(mouseX, mouseY);

            switch (CurrentState)
            {
                case GameState.MainMenu:
                    if (SplashKit.PointInRectangle(mousePoint, _startButtonRect))
                        return true; // Start game
                    if (SplashKit.PointInRectangle(mousePoint, _difficultyButtonRect))
                    {
                        CurrentState = GameState.DifficultyMenu;
                        return false;
                    }
                    if (SplashKit.PointInRectangle(mousePoint, _exitButtonRect))
                        SplashKit.CloseAllWindows();
                    break;

                case GameState.DifficultyMenu:
                    for (int i = 0; i < 3; i++)
                    {
                        if (SplashKit.PointInRectangle(mousePoint, _difficultyOptionRects[i]))
                        {
                            SelectedDifficulty = (Difficulty)i;
                            CurrentState = GameState.MainMenu;
                            return false;
                        }
                    }
                    break;

                case GameState.GameOver:
                    if (SplashKit.PointInRectangle(mousePoint, _playAgainButtonRect))
                        return true; // Restart game
                    if (SplashKit.PointInRectangle(mousePoint, _backToMenuButtonRect))
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
            // SplashKit automatically manages font resources
            // No explicit disposal needed for SplashKit fonts
        }
        
    }
}
