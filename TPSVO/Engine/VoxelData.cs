using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPSVO.Engine
{
    public struct VoxelData
    {
        public float x;
        public float y;
        public float z;
        public float size;
        public int c0;
        public int c1;
        public int c2;
        public int c3;
        public int c4;
        public int c5;
        public int c6;
        public int c7;
        public bool isLeaf;
        public float reflection;
        public float refraction;
        public float alpha;
        public float r;
        public float g;
        public float b;

        public VoxelData(Vector3 pos, float size, float reflection, float refraction, float alpha, Vector3 color)
        {
            this.x = pos.X;
            this.y = pos.Y;
            this.z = pos.Z;
            this.size = size;
            this.c0 = -1;
            this.c1 = -1;
            this.c2 = -1;
            this.c3 = -1;
            this.c4 = -1;
            this.c5 = -1;
            this.c6 = -1;
            this.c7 = -1;
            this.isLeaf = false;
            this.reflection = reflection;
            this.refraction = refraction;
            this.alpha = alpha;
            this.r = color.X;
            this.g = color.Y;
            this.b = color.Z;
        }
    }
}
