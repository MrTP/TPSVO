using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;

namespace TPSVO
{
    class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            int width = 1280;
            int height = 720;
            using (var game = new GameWindow(width, height))
            {
                GameHandler handler = new GameHandler(game.Width, game.Height);
                game.Load += (sender, e) =>
                {
                    // setup settings, load textures, sounds
                    handler.Initialize();
                    game.VSync = VSyncMode.On;

                };

                game.Unload += (sender, e) =>
                {
                };

                game.KeyDown += (object sender, KeyboardKeyEventArgs e) =>
                {
                    if (e.Key == Key.Space)
                    {
                        game.Exit();
                    }
                };

                int i = 0;
                game.UpdateFrame += (sender, e) =>
                {
                    // add game logic, input handling

                    // update shader uniforms

                    // update shader mesh

                    handler.Update(i);
                    ++i;
                };


                game.RenderFrame += (sender, e) =>
                {
                    GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                    handler.Draw();

                    game.SwapBuffers();
                };

                game.Resize += (sender, e) =>
                {
                    GL.Viewport(0, 0, game.Width, game.Height);
                };

                game.Run(60.0);
            }
        }
    }
}
