using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPSVO.Engine
{
    struct VoxelData
    {
        public float x;
        public float y;
        public float z;
        public float reflection;
        public float refraction;
        public float alpha;
        public float r;
        public float g;
        public float b;

        public VoxelData(Vector3 pos, float reflection, float refraction, float alpha, Vector3 color)
        {
            this.x = pos.X;
            this.y = pos.Y;
            this.z = pos.Z;
            this.reflection = reflection;
            this.refraction = refraction;
            this.alpha = alpha;
            this.r = color.X;
            this.g = color.Y;
            this.b = color.Z;
        }
    }
}
