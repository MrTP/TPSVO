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
            Window window = new Window(1280, 720, "TPSVO");
            window.Run(60, 60);
        }
    }
}
