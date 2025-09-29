using SplashKitSDK;
using PongGame.Entities;

namespace PongGame.Services
{
    /// <summary>
    /// Handles input events for the game
    /// </summary>
    public class InputHandler
    {
        /// <summary>
        /// Handle keyboard input for paddle movement using SplashKit
        /// </summary>
        public void HandleKeyInput(Paddle leftPaddle, Paddle rightPaddle)
        {
            // Left paddle controls (W/S)
            if (SplashKit.KeyDown(KeyCode.WKey))
                leftPaddle.MoveUp();
            else if (SplashKit.KeyDown(KeyCode.SKey))
                leftPaddle.MoveDown();
            else
                leftPaddle.ResetSpeed();

            // Right paddle controls (Up/Down arrows)
            if (SplashKit.KeyDown(KeyCode.UpKey))
                rightPaddle.MoveUp();
            else if (SplashKit.KeyDown(KeyCode.DownKey))
                rightPaddle.MoveDown();
            else
                rightPaddle.ResetSpeed();
        }

        /// <summary>
        /// Update paddle movement based on currently pressed keys
        /// </summary>
        public void UpdatePaddleMovement(Paddle leftPaddle, Paddle rightPaddle)
        {
            HandleKeyInput(leftPaddle, rightPaddle);
        }
    }
}
