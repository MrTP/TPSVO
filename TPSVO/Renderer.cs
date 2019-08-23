using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using TPSVO.Engine;

namespace TPSVO
{
    public class Renderer
    {
        private int mRenderProgramId;
        private int mComputeProgramId;
        private int mHeight;
        private int mWidth;
        private static Renderer instance;
        private List<VoxelData> voxels = new List<VoxelData>();

        private Renderer()
        {

        }

        public static Renderer Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Renderer();
                }
                return instance;
            }
        }

        public void Initialize(int width, int height)
        {
            Random r = new Random();
            for (int i = 0; i < 100; i++)
            {
                Vector3 pos = new Vector3((float)r.Next(-10, 10), (float)r.Next(0, 20), (float)r.Next(-10, 10));
                Vector3 color = new Vector3((float)r.NextDouble(), (float)r.NextDouble(), (float)r.NextDouble());
                VoxelData data = new VoxelData(pos, 0, 0, 1, color);
                voxels.Add(data);
            }
            for (int i = 0; i < 100; i++)
            {
                Vector3 pos = new Vector3(-10 + i % 10, 0, -5 + i / 10);
                Vector3 color = new Vector3((float)r.NextDouble(), (float)r.NextDouble(), (float)r.NextDouble());
                VoxelData data = new VoxelData(pos, (float)r.NextDouble(), 0, 1, new Vector3(0.5f));
                voxels.Add(data);
            }
            this.mWidth = width;
            this.mHeight = height;
            int texHandle = GenerateDestTex();
            mRenderProgramId = SetupRenderProgram();
            mComputeProgramId = SetupComputeProgram();
        }

        public void Update(int frame)
        {
            GL.UseProgram(mComputeProgramId);
            GL.Uniform1(GL.GetUniformLocation(mComputeProgramId, "roll"), (float)frame * 0.005f);
            //Console.WriteLine((-20 + (frame * 0.01f) )+ "");
            GL.DispatchCompute(mWidth / 16, mHeight / 16, 1); // width * height threads in blocks of 16^2
                                                              //checkErrors("Dispatch compute shader");
        }

        public void Draw()
        {
            GL.UseProgram(mRenderProgramId);
            GL.DrawArrays(PrimitiveType.TriangleStrip, 0, 4);
            //checkErrors("Draw screen");
        }

        private int SetupRenderProgram()
        {
            int progHandle = GL.CreateProgram();
            int vp = GL.CreateShader(ShaderType.VertexShader);
            int fp = GL.CreateShader(ShaderType.FragmentShader);

            string vpSrc = File.ReadAllText("res/shaders/vertexShader.glsl");



            string fpSrc = File.ReadAllText("res/shaders/fragmentShader.glsl");

            GL.ShaderSource(vp, vpSrc);
            GL.ShaderSource(fp, fpSrc);

            GL.CompileShader(vp);
            int rvalue;
            GL.GetShader(vp, ShaderParameter.CompileStatus, out rvalue);
            if (rvalue != (int)All.True)
            {
                Console.WriteLine("Error in compiling vp");
                Console.WriteLine((All)rvalue);
                Console.WriteLine(GL.GetShaderInfoLog(vp));
            }
            GL.AttachShader(progHandle, vp);

            GL.CompileShader(fp);
            GL.GetShader(fp, ShaderParameter.CompileStatus, out rvalue);
            if (rvalue != (int)All.True)
            {
                Console.WriteLine("Error in compiling fp");
                Console.WriteLine((All)rvalue);
                Console.WriteLine(GL.GetShaderInfoLog(fp));
            }
            GL.AttachShader(progHandle, fp);

            GL.BindFragDataLocation(progHandle, 0, "color");
            GL.LinkProgram(progHandle);

            GL.GetProgram(progHandle, GetProgramParameterName.LinkStatus, out rvalue);
            if (rvalue != (int)All.True)
            {
                Console.WriteLine("Error in linking sp");
                Console.WriteLine((All)rvalue);
                Console.WriteLine(GL.GetProgramInfoLog(progHandle));
            }

            GL.UseProgram(progHandle);
            GL.Uniform1(GL.GetUniformLocation(progHandle, "srcTex"), 0);

            int vertArray;
            vertArray = GL.GenVertexArray();
            GL.BindVertexArray(vertArray);

            int posBuf;
            posBuf = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, posBuf);
            float[] data = {
                0.0f, 0.0f,
                0.0f, 1.0f,
                1.0f, 0.0f,
                1.0f, 1.0f
            };
            IntPtr dataSize = (IntPtr)(sizeof(float) * 8);

            GL.BufferData<float>(BufferTarget.ArrayBuffer, dataSize, data, BufferUsageHint.StreamDraw);
            int posPtr = GL.GetAttribLocation(progHandle, "pos");
            GL.VertexAttribPointer(posPtr, 2, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(posPtr);

            //checkErrors("Render shaders");
            return progHandle;
        }

        private int GenerateDestTex()
        {
            // We create a single float channel 512^2 texture
            int texHandle;
            texHandle = GL.GenTexture();

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, texHandle);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba16, mWidth, mHeight, 0, PixelFormat.Rgba, PixelType.Float, IntPtr.Zero);

            // Because we're also using this tex as an image (in order to write to it),
            // we bind it to an image unit as well
            GL.BindImageTexture(0, texHandle, 0, false, 0, TextureAccess.WriteOnly, SizedInternalFormat.Rgba16);
            //checkErrors("Gen texture");	
            return texHandle;
        }

        private int SetupComputeProgram()
        {
            // Creating the compute shader, and the program object containing the shader
            int progHandle = GL.CreateProgram();
            int cs = GL.CreateShader(ShaderType.ComputeShader);

            // In order to write to a texture, we have to introduce it as image2D.
            // local_size_x/y/z layout variables define the work group size.
            // gl_GlobalInvocationID is a uvec3 variable giving the global ID of the thread,
            // gl_LocalInvocationID is the local index within the work group, and
            // gl_WorkGroupID is the work group's index
            string csSrc = File.ReadAllText("res/shaders/computeShader.glsl");

            GL.ShaderSource(cs, csSrc);
            GL.CompileShader(cs);
            int rvalue;
            GL.GetShader(cs, ShaderParameter.CompileStatus, out rvalue);
            if (rvalue != (int)All.True)
            {
                Console.WriteLine(GL.GetShaderInfoLog(cs));
            }
            GL.AttachShader(progHandle, cs);

            GL.LinkProgram(progHandle);
            GL.GetProgram(progHandle, GetProgramParameterName.LinkStatus, out rvalue);
            if (rvalue != (int)All.True)
            {
                Console.WriteLine(GL.GetProgramInfoLog(progHandle));
            }

            int ssbo;
            ssbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, ssbo);
            GL.BufferData(BufferTarget.ShaderStorageBuffer, Marshal.SizeOf(typeof(VoxelData)) * voxels.Count, voxels.ToArray(), BufferUsageHint.StaticRead);
            Console.WriteLine(Marshal.SizeOf(typeof(VoxelData)));
            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 1, ssbo);
            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, 0); // unbind

            GL.UseProgram(progHandle);

            GL.Uniform1(GL.GetUniformLocation(progHandle, "destTex"), 0);



            //checkErrors("Compute shader");
            return progHandle;
        }
    }
}
