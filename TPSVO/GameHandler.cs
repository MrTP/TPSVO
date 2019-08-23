using System;
using System.Collections.Generic;
using System.Text;

namespace TPSVO
{
    public class GameHandler
    {
        private int width;
        private int height;
        private Renderer renderer;

        public GameHandler(int width, int height)
        {
            this.width = width;
            this.height = height;
            this.renderer = Renderer.Instance;
        }

        internal void Update(int v)
        {
            this.renderer.Update(v);
        }

        internal void Initialize()
        {
            this.renderer.Initialize(width, height);
        }

        internal void Draw()
        {
            this.renderer.Draw();
        }
    }
}
