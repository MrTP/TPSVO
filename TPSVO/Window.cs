using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPSVO.Rendering;
using TPSVO.Engine;
using System.IO;

namespace TPSVO
{
    public class Window : GameWindow
    {


        private Camera camera;
        bool firstMove = true;
        Vector2 lastPos;

        public Window(int width, int height, string title) : base(width, height, GraphicsMode.Default, title)
        {
            this.CursorVisible = false;
        }

        protected override void OnLoad(EventArgs e)
        {
            Renderer.Instance.Initialize(this.Width, this.Height);
            this.VSync = VSyncMode.Adaptive;
            //The shaders have been modified to include the texture coordinates, check them out after finishing the OnLoad function.
            camera = Renderer.Instance.Camera;
            camera.Position = new Vector3(0, 0, 128f);
            camera.Pitch = 0;
            camera.Yaw = 0;
            var ver = VoxImporter.LoadModel("res/vox/Example.vox");
            List<VoxelData> model = VoxImporter.CalculateOctree(ver);
            Renderer.Instance.Voxels.AddRange(model);
            Renderer.Instance.UpdateBuffer();
            base.OnLoad(e);
        }



        protected override void OnRenderFrame(FrameEventArgs e)
        {
            Renderer.Instance.Draw();
            base.OnRenderFrame(e);
            SwapBuffers();
        }


        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            if (!Focused) // check to see if the window is focused
            {
                return;
            }


            //this.Title = this.TargetRenderFrequency + " " + Math.Round(1.0 / e.Time) + " " + camera.Pitch + " " + camera.Yaw + " " + camera.Position.ToString();
            KeyboardState input = Keyboard.GetState();

            if (input.IsKeyDown(Key.Escape))
            {
                Exit();
            }

            if (input.IsKeyDown(Key.W))
                camera.Position += camera.Front * camera.Speed * (float)e.Time; //Forward 
            if (input.IsKeyDown(Key.S))
                camera.Position -= camera.Front * camera.Speed * (float)e.Time; //Backwards
            if (input.IsKeyDown(Key.A))
                camera.Position -= camera.Right * camera.Speed * (float)e.Time; //Left
            if (input.IsKeyDown(Key.D))
                camera.Position += camera.Right * camera.Speed * (float)e.Time; //Right
            if (input.IsKeyDown(Key.Space))
                camera.Position += camera.Up * camera.Speed * (float)e.Time; //Up 
            if (input.IsKeyDown(Key.LShift))
                camera.Position -= camera.Up * camera.Speed * (float)e.Time; //Down
            if (input.IsKeyDown(Key.G))
            {
                Console.WriteLine(camera.Position);
                Console.WriteLine(camera.Pitch);
                Console.WriteLine(camera.Yaw);
                Console.WriteLine(Renderer.Instance.Details);
                Console.WriteLine();
            }


            //Get the mouse state
            MouseState mouse = Mouse.GetState();

            if (firstMove) // this bool variable is initially set to true
            {
                lastPos = new Vector2(mouse.X, mouse.Y);
                firstMove = false;
            }
            else
            {
                //Calculate the offset of the mouse position
                float deltaX = mouse.X - lastPos.X;
                float deltaY = mouse.Y - lastPos.Y;
                lastPos = new Vector2(mouse.X, mouse.Y);

                //Apply the camera pitch and yaw (we clamp the pitch in the camera class)
                camera.Yaw -= deltaX * camera.Sensitivity;
                camera.Pitch -= deltaY * camera.Sensitivity; // reversed since y-coordinates range from bottom to top
            }
            Renderer.Instance.Update();
            //Thread.Sleep(1);
            base.OnUpdateFrame(e);
        }

        //This function's main purpose is to set the mouse position back to the center of the window
        //every time the mouse has moved. So the cursor doesn't end up at the edge of the window where it cannot move
        //further out
        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            if (Focused) // check to see if the window is focused
            {
                Mouse.SetPosition(X + Width / 2f, Y + Height / 2f);
            }

            base.OnMouseMove(e);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            string[] files = Directory.GetFileSystemEntries("res/vox/");
            Random r = new Random();
            string f = files[r.Next(files.Length - 1)];

            var ver = VoxImporter.LoadModel(f);
            List<VoxelData> model = VoxImporter.CalculateOctree(ver);
            Renderer.Instance.Voxels.Clear();
            Renderer.Instance.Voxels.AddRange(model);
            Renderer.Instance.UpdateBuffer();

            base.OnMouseDown(e);
        }

        //In the mouse wheel function we manage all the zooming of the camera
        //this is simply done by changing the FOV of the camera
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                Renderer.Instance.Details++;
            }
            else
            {
                Renderer.Instance.Details--;
            }
            base.OnMouseWheel(e);
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
            //We need to update the aspect ratio once the window has been resized
            base.OnResize(e);
        }

        protected override void OnUnload(EventArgs e)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            base.OnUnload(e);
        }

    }
}
