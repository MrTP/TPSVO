using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPSVO.Engine;

namespace TPSVO.Engine
{
    public static class VoxImporter
    {
        private static Vector3 negative = new Vector3(-1, -1, -1);
        private static Vector3 transparency = new Vector3(255, 0, 255);

        #region COLORTABLE
        private static uint[] default_palette = new uint[]{
    0x00000000, 0xffffffff, 0xffccffff, 0xff99ffff, 0xff66ffff, 0xff33ffff, 0xff00ffff, 0xffffccff, 0xffccccff, 0xff99ccff, 0xff66ccff, 0xff33ccff, 0xff00ccff, 0xffff99ff, 0xffcc99ff, 0xff9999ff,
    0xff6699ff, 0xff3399ff, 0xff0099ff, 0xffff66ff, 0xffcc66ff, 0xff9966ff, 0xff6666ff, 0xff3366ff, 0xff0066ff, 0xffff33ff, 0xffcc33ff, 0xff9933ff, 0xff6633ff, 0xff3333ff, 0xff0033ff, 0xffff00ff,
    0xffcc00ff, 0xff9900ff, 0xff6600ff, 0xff3300ff, 0xff0000ff, 0xffffffcc, 0xffccffcc, 0xff99ffcc, 0xff66ffcc, 0xff33ffcc, 0xff00ffcc, 0xffffcccc, 0xffcccccc, 0xff99cccc, 0xff66cccc, 0xff33cccc,
    0xff00cccc, 0xffff99cc, 0xffcc99cc, 0xff9999cc, 0xff6699cc, 0xff3399cc, 0xff0099cc, 0xffff66cc, 0xffcc66cc, 0xff9966cc, 0xff6666cc, 0xff3366cc, 0xff0066cc, 0xffff33cc, 0xffcc33cc, 0xff9933cc,
    0xff6633cc, 0xff3333cc, 0xff0033cc, 0xffff00cc, 0xffcc00cc, 0xff9900cc, 0xff6600cc, 0xff3300cc, 0xff0000cc, 0xffffff99, 0xffccff99, 0xff99ff99, 0xff66ff99, 0xff33ff99, 0xff00ff99, 0xffffcc99,
    0xffcccc99, 0xff99cc99, 0xff66cc99, 0xff33cc99, 0xff00cc99, 0xffff9999, 0xffcc9999, 0xff999999, 0xff669999, 0xff339999, 0xff009999, 0xffff6699, 0xffcc6699, 0xff996699, 0xff666699, 0xff336699,
    0xff006699, 0xffff3399, 0xffcc3399, 0xff993399, 0xff663399, 0xff333399, 0xff003399, 0xffff0099, 0xffcc0099, 0xff990099, 0xff660099, 0xff330099, 0xff000099, 0xffffff66, 0xffccff66, 0xff99ff66,
    0xff66ff66, 0xff33ff66, 0xff00ff66, 0xffffcc66, 0xffcccc66, 0xff99cc66, 0xff66cc66, 0xff33cc66, 0xff00cc66, 0xffff9966, 0xffcc9966, 0xff999966, 0xff669966, 0xff339966, 0xff009966, 0xffff6666,
    0xffcc6666, 0xff996666, 0xff666666, 0xff336666, 0xff006666, 0xffff3366, 0xffcc3366, 0xff993366, 0xff663366, 0xff333366, 0xff003366, 0xffff0066, 0xffcc0066, 0xff990066, 0xff660066, 0xff330066,
    0xff000066, 0xffffff33, 0xffccff33, 0xff99ff33, 0xff66ff33, 0xff33ff33, 0xff00ff33, 0xffffcc33, 0xffcccc33, 0xff99cc33, 0xff66cc33, 0xff33cc33, 0xff00cc33, 0xffff9933, 0xffcc9933, 0xff999933,
    0xff669933, 0xff339933, 0xff009933, 0xffff6633, 0xffcc6633, 0xff996633, 0xff666633, 0xff336633, 0xff006633, 0xffff3333, 0xffcc3333, 0xff993333, 0xff663333, 0xff333333, 0xff003333, 0xffff0033,
    0xffcc0033, 0xff990033, 0xff660033, 0xff330033, 0xff000033, 0xffffff00, 0xffccff00, 0xff99ff00, 0xff66ff00, 0xff33ff00, 0xff00ff00, 0xffffcc00, 0xffcccc00, 0xff99cc00, 0xff66cc00, 0xff33cc00,
    0xff00cc00, 0xffff9900, 0xffcc9900, 0xff999900, 0xff669900, 0xff339900, 0xff009900, 0xffff6600, 0xffcc6600, 0xff996600, 0xff666600, 0xff336600, 0xff006600, 0xffff3300, 0xffcc3300, 0xff993300,
    0xff663300, 0xff333300, 0xff003300, 0xffff0000, 0xffcc0000, 0xff990000, 0xff660000, 0xff330000, 0xff0000ee, 0xff0000dd, 0xff0000bb, 0xff0000aa, 0xff000088, 0xff000077, 0xff000055, 0xff000044,
    0xff000022, 0xff000011, 0xff00ee00, 0xff00dd00, 0xff00bb00, 0xff00aa00, 0xff008800, 0xff007700, 0xff005500, 0xff004400, 0xff002200, 0xff001100, 0xffee0000, 0xffdd0000, 0xffbb0000, 0xffaa0000,
    0xff880000, 0xff770000, 0xff550000, 0xff440000, 0xff220000, 0xff110000, 0xffeeeeee, 0xffdddddd, 0xffbbbbbb, 0xffaaaaaa, 0xff888888, 0xff777777, 0xff555555, 0xff444444, 0xff222222, 0xff111111
};
        #endregion

        private struct VoxelInfo
        {
            public byte x;
            public byte y;
            public byte z;
            public byte color;

            public VoxelInfo(BinaryReader stream)
            {
                x = (byte)(stream.ReadByte());
                y = (byte)(stream.ReadByte());
                z = (byte)(stream.ReadByte());
                color = stream.ReadByte();
            }
        }

        public static bool isSolid(Vector3[,,] voxels, int x, int y, int z)
        {
            if (x < 0 || x >= voxels.GetLength(0) || y < 0 || y >= voxels.GetLength(1) || z < 0 || z >= voxels.GetLength(2))
                return false;

            return voxels[x, y, z] != negative;

        }

        /// <summary>
        /// Load a MagicaVoxel .vox format file into the custom ushort[] structure that we use for voxel chunks.
        /// </summary>
        /// <param name="stream">An open BinaryReader stream that is the .vox file.</param>
        /// <param name="overrideColors">Optional color lookup table for converting RGB values into my internal engine color format.</param>
        /// <returns>The voxel chunk data for the MagicaVoxel .vox file.</returns>
        public static Vector3[,,] LoadModel(string filename)
        {
            FileStream fs = File.Open(filename, FileMode.Open);
            BinaryReader stream = new BinaryReader(fs);
            VoxelInfo[] voxelData = null;
            List<VoxelInfo[]> frames = new List<VoxelInfo[]>();
            List<Vector3> sizes = new List<Vector3>();
            string magic = new string(stream.ReadChars(4));
            int version = stream.ReadInt32();
            Dictionary<int, Vector3> colorTable = new Dictionary<int, Vector3>();
            Vector3[,,] voxels = null;
            // a MagicaVoxel .vox file starts with a 'magic' 4 character 'VOX ' identifier
            if (magic == "VOX ")
            {
                int sizex = 0, sizey = 0, sizez = 0;
                //bool subsample = false;

                while (stream.BaseStream.Position < stream.BaseStream.Length)
                {
                    // each chunk has an ID, size and child chunks
                    char[] chunkId = stream.ReadChars(4);
                    int chunkSize = stream.ReadInt32();
                    int childChunks = stream.ReadInt32();
                    string chunkName = new string(chunkId);

                    // there are only 2 chunks we only care about, and they are SIZE and XYZI
                    if (chunkName == "SIZE")
                    {
                        if (voxelData != null)
                        {
                            frames.Add(voxelData);
                            sizes.Add(new Vector3(sizex, sizez, sizey));
                            voxelData = null;
                            voxels = null;
                        }
                        sizex = stream.ReadInt32();
                        sizey = stream.ReadInt32();
                        sizez = stream.ReadInt32();

                        stream.ReadBytes(chunkSize - 4 * 3);
                    }
                    else if (chunkName == "XYZI")
                    {
                        // XYZI contains n voxels
                        int numVoxels = stream.ReadInt32();


                        // each voxel has x, y, z and color index values
                        voxelData = new VoxelInfo[numVoxels];
                        for (int i = 0; i < voxelData.Length; i++)
                            voxelData[i] = new VoxelInfo(stream);
                    }
                    else if (chunkName == "RGBA")
                    {
                        colorTable.Clear();
                        for (int i = 0; i < 256; i++)
                        {
                            byte r = stream.ReadByte();
                            byte g = stream.ReadByte();
                            byte b = stream.ReadByte();
                            byte a = stream.ReadByte();
                            if (!colorTable.ContainsKey(i + 1))
                            {
                                colorTable.Add(i + 1, new Vector3((float)(r) / 256.0f, (float)(g) / 256.0f, (float)(b) / 256.0f));
                            }
                        }
                    }
                    else
                    {
                        byte[] chunk = stream.ReadBytes(chunkSize);
                        //Console.WriteLine(System.Text.Encoding.Default.GetString(chunk));
                    }
                    // read any excess bytes
                }
                frames.Add(voxelData);
                sizes.Add(new Vector3(sizex, sizez, sizey));
                for (int q = 0; q < frames.Count; q++)
                {
                    voxelData = frames[q];
                    if (colorTable.Count == 0)
                    {
                        for (int i = 0; i < default_palette.Length; i++)
                        {
                            float color = (float)(default_palette[i] % (256 * 256 * 256));

                            colorTable[i + 1] = new Vector3((float)(color % 256) / 256.0f, (float)(Math.Floor(color / 256) % 256) / 256.0f, (float)(Math.Floor(color / 256 / 256) % (256 * 256)) / 256.0f);
                        }
                    }
                    voxels = new Vector3[(int)sizes[q].X, (int)sizes[q].Z, (int)sizes[q].Y];
                    for (int x = 0; x < (int)sizes[q].X; x++)
                    {
                        for (int y = 0; y < (int)sizes[q].Y; y++)
                        {
                            for (int z = 0; z < (int)sizes[q].Z; z++)
                            {
                                voxels[x, z, y] = negative;
                            }
                        }
                    }
                    for (int i = 0; i < voxelData.Length; i++)
                    {
                        if (voxelData[i].x >= 0 && voxelData[i].x < sizes[q].X && voxelData[i].y >= 0 && voxelData[i].y < sizes[q].Z && voxelData[i].z >= 0 && voxelData[i].z < sizes[q].Y)
                        {
                            voxels[voxelData[i].x, voxelData[i].y, voxelData[i].z] = colorTable[voxelData[i].color];
                        }
                    }
                    fs.Close();
                    stream.Close();
                    return voxels;

                }
            }
            fs.Close();
            stream.Close();

            return null;
        }

        public static List<VoxelData> CalculateOctree(Vector3[,,] voxels)
        {
            List<VoxelData> voxelData = new List<VoxelData>();
            int size = voxels.GetLength(0);
            size = NextPowerOf2(size);
            
            VoxelData root = new VoxelData(new Vector3(0, 0, 0), size, 0, 0, 1, new Vector3(1, 0.5f, 1));
            voxelData.Add(root);
            SplitOctree(voxels, voxelData, 0);
            return voxelData;
        }

        private static void SplitOctree(Vector3[,,] voxels, List<VoxelData> voxelData, int parentIndex)
        {
            VoxelData parent = voxelData[parentIndex];
            if (parent.size <= 1)
            {
                parent.isLeaf = true;
                return;
            }
            float size = parent.size / 2f;
            float x1 = parent.x;
            float y1 = parent.y;
            float z1 = parent.z;
            float x2 = parent.x + size;
            float y2 = parent.y + size;
            float z2 = parent.z + size;
            parent.c0 = AddVoxelData(voxels, voxelData, x1, y1, z1, size);
            parent.c1 = AddVoxelData(voxels, voxelData, x1 + size, y1, z1, size);
            parent.c2 = AddVoxelData(voxels, voxelData, x1 + size, y1 + size, z1, size);
            parent.c3 = AddVoxelData(voxels, voxelData, x1, y1 + size, z1, size);

            parent.c4 = AddVoxelData(voxels, voxelData, x1, y1, z1 + size, size);
            parent.c5 = AddVoxelData(voxels, voxelData, x1 + size, y1, z1 + size, size);
            parent.c6 = AddVoxelData(voxels, voxelData, x1 + size, y1 + size, z1 + size, size);
            parent.c7 = AddVoxelData(voxels, voxelData, x1, y1 + size, z1 + size, size);
            voxelData[parentIndex] = parent;
        }

        private static int AddVoxelData(Vector3[,,] voxels, List<VoxelData> voxelData, float x, float y, float z, float size)
        {

            Vector3 pos = new Vector3(x, y, z);
            int index = -1;
            Vector3 color = GetVoxelColor(voxels, pos, size);
            if (color.X < 0)
            {
                return -1;
            }
            VoxelData data = new VoxelData(pos, size, 0, 0, 1, color);
            if (data.size >= 0)
            {
                index = voxelData.Count;
                voxelData.Add(data);
                SplitOctree(voxels, voxelData, voxelData.Count - 1);
            }
            return index;
        }

        private static int NextPowerOf2(int n)
        {
            int count = 0;

            // First n in the below  
            // condition is for the 
            // case where n is 0 
            if (n > 0 && (n & (n - 1)) == 0)
                return n;

            while (n != 0)
            {
                n >>= 1;
                count += 1;
            }

            return 1 << count;
        }

        private static Vector3 GetVoxelColor(Vector3[,,] voxels, Vector3 pos, float size)
        {
            Vector3 color = new Vector3(-1, -1, -1);
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    for (int z = 0; z < size; z++)
                    {
                        if (isSolid(voxels, (int)(pos.X + x), (int)(pos.Y + y), (int)(pos.Z + z)))
                        {
                            color = voxels[(int)(pos.X + x), (int)(pos.Y + y), (int)(pos.Z + z)];
                            return color;
                        }
                    }
                }
            }
            return color;
        }

    }
}
