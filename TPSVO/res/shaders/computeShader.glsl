#version 430

struct HitInfo
{
	float near;
	float far;
	bool hit;
	vec3 pos;
	vec3 color;
	int voxel;
};

struct StackedVoxel
{
	int depth;
	int voxel;
	float dist;
};

struct Voxel
{
	float x;
	float y;
	float z;
	float size;
	int c0;
	int c1;
	int c2;
	int c3;
	int c4;
	int c5;
	int c6;
	int c7;
	bool isLeaf;
	float reflection;
	float refraction;
	float alpha;
	float r;
	float g;
	float b;
};

uniform int voxels;
uniform vec3 position;
uniform float pitch;
uniform float yaw;
uniform int details;
uniform writeonly image2D destTex;
layout (local_size_x = 16, local_size_y = 16) in;
layout(std430, binding = 1) buffer voxelBuffer
 {
    Voxel voxelData[];
 };
#define tolerance 0.000001f
StackedVoxel stack[35];
int stackPos = 0;


vec3 getNormals(vec3 pos, vec3 center)
{
	vec3 dist = center - pos;
	float maxDist = max(max(abs(dist.x), abs(dist.y)), abs(dist.z));
	if (maxDist > abs(dist.x))
	{
		dist.x = 0;
	}
	if (maxDist > abs(dist.y))
	{
		dist.y = 0;
	}
	if (maxDist > abs(dist.z))
	{
		dist.z = 0;
	}
	return normalize(dist);
}

HitInfo GetHitInfo(vec3 origin, vec3 dir, int index) 
{
	Voxel box = voxelData[index];
	vec3 pos = vec3(box.x, box.y, box.z);
	vec3 boxMax = pos + box.size;
	vec3 tMin = (pos - origin) / dir;
	vec3 tMax = (boxMax - origin) / dir;
	vec3 t1 = min(tMin, tMax);
	vec3 t2 = max(tMin, tMax);
	float tNear = max(max(t1.x, t1.y), t1.z);
	float tFar = min(min(t2.x, t2.y), t2.z);
	HitInfo hitInfo;
	hitInfo.near = tNear;
	hitInfo.far = tFar;
	hitInfo.voxel = index;
	hitInfo.pos = origin + (dir * tNear);
	hitInfo.hit = false;
	if (tFar > 0 && tNear < tFar)
	{
		hitInfo.hit = true;
	}
	return hitInfo;
}

mat3 rotateY(float rad) {
    float c = cos(rad);
    float s = sin(rad);
    return mat3(
        c, 0.0, s,
        0.0, 1.0, 0.0,
        -s, 0.0, c
    );
}
    
mat3 rotateZ(float rad) {
    float c = cos(rad);
    float s = sin(rad);
    return mat3(
        c, -s, 0.0,
        s, c, 0.0,
        0.0, 0.0, 1.0
    );
}

StackedVoxel getStackedVoxel(int depth, HitInfo info)
{
	StackedVoxel stacked;
	stacked.depth = depth;
	stacked.voxel = info.voxel;
	stacked.dist = info.near;
	return stacked;
}

void SortStack(int start)
{
	for (int i = 0; i < stackPos - start; i++)
	{
		for (int j = start; j < stackPos - i; j++)
		{
			if (stack[j].dist <= stack[j + 1].dist)
			{
				StackedVoxel s1 = stack[j];
				stack[j] = stack[j + 1];
				stack[j + 1] = s1;
			}
		}	
	}
}

#define maxDepth 12;
HitInfo Trace(vec3 origin, vec3 dir, int root)
{
	int depth = 0;
	int currentIndex = 0;
	vec3 color = vec3(0,0,0);
	float colorIndex = 0f;
	stackPos = 0;
	HitInfo hitInfo;
	hitInfo.hit = false;
	
	while (true)
	{
		Voxel voxel = voxelData[currentIndex];
		hitInfo = GetHitInfo(origin, dir, currentIndex);
		if ((voxel.size <= 1 || voxel.isLeaf) && hitInfo.hit)
		{
			return hitInfo;
		}
		if (depth > 20)
		{
			return hitInfo;
		}
		
		if (hitInfo.hit)
		{
			depth++;
			int startPos = stackPos;
			if (stackPos > 25)
			{
				return hitInfo;
			}
			if (voxel.c0 >= 0)
			{
				HitInfo info = GetHitInfo(origin, dir, voxel.c0);
				if (info.hit)
				{
					stackPos++;
					stack[stackPos] = getStackedVoxel(depth, info);
				}
			}
			if (voxel.c1 >= 0)
			{
				HitInfo info = GetHitInfo(origin, dir, voxel.c1);
				if (info.hit)
				{
					stackPos++;
					stack[stackPos] = getStackedVoxel(depth, info);
				}
			}
			if (voxel.c2 >= 0)
			{
				HitInfo info = GetHitInfo(origin, dir, voxel.c2);
				if (info.hit)
				{
					stackPos++;
					stack[stackPos] = getStackedVoxel(depth, info);
				}
			}
			if (voxel.c3 >= 0)
			{
				HitInfo info = GetHitInfo(origin, dir, voxel.c3);
				if (info.hit)
				{
					stackPos++;
					stack[stackPos] = getStackedVoxel(depth, info);
				}
			}
			if (voxel.c4 >= 0)
			{
				HitInfo info = GetHitInfo(origin, dir, voxel.c4);
				if (info.hit)
				{
					stackPos++;
					stack[stackPos] = getStackedVoxel(depth, info);
				}
			}
			if (voxel.c5 >= 0)
			{
				HitInfo info = GetHitInfo(origin, dir, voxel.c5);
				if (info.hit)
				{
					stackPos++;
					stack[stackPos] = getStackedVoxel(depth, info);
				}
			}
			if (voxel.c6 >= 0)
			{
				HitInfo info = GetHitInfo(origin, dir, voxel.c6);
				if (info.hit)
				{
					stackPos++;
					stack[stackPos] = getStackedVoxel(depth, info);
				}
			}
			if (voxel.c7 >= 0)
			{
				HitInfo info = GetHitInfo(origin, dir, voxel.c7);
				if (info.hit)
				{
					stackPos++;
					stack[stackPos] = getStackedVoxel(depth, info);
				}
			}
			SortStack(startPos);
		}
		else
		{
			if (depth <= 0)
			{
				hitInfo.hit = false;
				return hitInfo;
			}
		}
		if (stackPos > 0)
		{
			StackedVoxel current = stack[stackPos];
			stackPos--;
			depth = current.depth;
			currentIndex = current.voxel;
		}
		else
		{
			hitInfo.hit = false;
			return hitInfo;
		}

	}
	hitInfo.hit = false;
	return hitInfo;

}




void main() {
	ivec2 pix = ivec2(gl_GlobalInvocationID.xy);
	vec4 color = vec4(0.0, 0.0, 0.0, 1.0);
	if (voxels > 0)
	{
		ivec2 size = imageSize(destTex);
		if (pix.x >= size.x || pix.y >= size.y) {
			return;
		}
		float ratio =(1.0f * size.x) / (1.0f * size.y);
		vec2 pos = vec2(pix) / vec2(size.x, size.y);
		pos -= 0.5f;
		pos.y /= ratio;
		vec3 rayDir = vec3(1, pos.x, pos.y);
		rayDir = rotateY(pitch) * rayDir;
		rayDir = rotateZ(yaw) * rayDir;

		HitInfo hit = Trace(position, rayDir, 0);
		if (hit.hit)
		{
			Voxel v = voxelData[hit.voxel];
			color = vec4(v.r, v.g, v.b, 1f);	
			float refVal = max(0f,v.b - v.r);
			vec3 center = vec3(v.x, v.y, v.z) + v.size / 2f;
			vec3 normals = getNormals(hit.pos, center);
			if (v.r == 0.3203125f)
			{
				vec3 ref = reflect(rayDir, normals);
				HitInfo mirror = Trace(hit.pos + ref, ref, 0);
				if (mirror.hit)
				{
					Voxel v2 = voxelData[mirror.voxel];
					color = mix(color, mix(vec4(0.5f), vec4(v2.r, v2.g, v2.b, 1f), 0.8f), refVal);
				}
			}
			if (v.r != 0.3203125f)
			{
				vec3 light = vec3(128f);
				light.x = 128f * sin(0.02f * details);
				vec3 ref = normalize(light - hit.pos);
				HitInfo mirror = Trace(hit.pos + ref, ref, 0);
				if (mirror.hit)
				{
					v = voxelData[mirror.voxel];
					color = mix(color, vec4(0.5f), 0.3f);
				}
			}
		}
		else
		{
			color = vec4(0.5f,0.5f,0.5f, 1f);
		}
		
	}
	
	imageStore(destTex, pix, color);
	
};