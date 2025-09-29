using System;
using SplashKitSDK;

namespace PongGame
{
    internal static class Program
    {
        static void Main()
        {            
            try
            {
                // Initialize SplashKit window
                SplashKit.OpenWindow("Pong Game", 1200, 800);
                
                // Create game instance
                var game = new MainGameForm();
                
                // Main game loop
                while (!SplashKit.WindowCloseRequested("Pong Game"))
                {
                    SplashKit.ProcessEvents();
                    game.HandleInput();
                    game.Update();
                    game.Render();
                    SplashKit.RefreshScreen(60);
                }
                
                // Cleanup
                game.Dispose();
                SplashKit.CloseAllWindows();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }
}
