using System.Windows.Forms;
using PongGame.Entities;

namespace PongGame.Services
{
    /// <summary>
    /// Handles input events for the game
    /// </summary>
    public class InputHandler
    {
        public bool UpPressed { get; private set; }
        public bool DownPressed { get; private set; }
        public bool WPressed { get; private set; }
        public bool SPressed { get; private set; }

        /// <summary>
        /// Handle key down events
        /// </summary>
        public void HandleKeyDown(Keys key)
        {
            switch (key)
            {
                case Keys.Up:
                    UpPressed = true;
                    break;
                case Keys.Down:
                    DownPressed = true;
                    break;
                case Keys.W:
                    WPressed = true;
                    break;
                case Keys.S:
                    SPressed = true;
                    break;
            }
        }

        /// <summary>
        /// Handle key up events and reset paddle speeds
        /// </summary>
        public void HandleKeyUp(Keys key, Paddle leftPaddle, Paddle rightPaddle)
        {
            switch (key)
            {
                case Keys.Up:
                    UpPressed = false;
                    rightPaddle.ResetSpeed();
                    break;
                case Keys.Down:
                    DownPressed = false;
                    rightPaddle.ResetSpeed();
                    break;
                case Keys.W:
                    WPressed = false;
                    leftPaddle.ResetSpeed();
                    break;
                case Keys.S:
                    SPressed = false;
                    leftPaddle.ResetSpeed();
                    break;
            }
        }

        /// <summary>
        /// Update paddle movement based on currently pressed keys
        /// </summary>
        public void UpdatePaddleMovement(Paddle leftPaddle, Paddle rightPaddle)
        {
            if (WPressed)
                leftPaddle.MoveUp();
            if (SPressed)
                leftPaddle.MoveDown();
            if (UpPressed)
                rightPaddle.MoveUp();
            if (DownPressed)
                rightPaddle.MoveDown();
        }
    }
}
