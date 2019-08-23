#version 430
struct HitInfo
{
	float near;
	float far;
	bool hit;
	vec3 pos;
	vec3 normal;
};

struct Voxel
{
	float x;
	float y;
	float z;
	float reflection;
	float refraction;
	float alpha;
	float r;
	float g;
	float b;

};

uniform float roll;
uniform writeonly image2D destTex;
layout (local_size_x = 16, local_size_y = 16) in;
layout(std430, binding = 1) buffer voxelBuffer
 {
    Voxel voxelData[];
 };
#define tolerance 0.00001f




vec3 getNormals(vec3 pos, vec3 boxMin, vec3 boxMax)
{
	vec3 normals;
	normals.x = float(pos.x + tolerance > boxMin.x && pos.x - tolerance < boxMin.x) - float(pos.x + tolerance > boxMax.x && pos.x - tolerance < boxMax.x);
	normals.y = float(pos.y + tolerance > boxMin.y && pos.y - tolerance < boxMin.y) - float(pos.y + tolerance > boxMax.y && pos.y - tolerance < boxMax.y);
	normals.z = float(pos.z + tolerance > boxMin.z && pos.z - tolerance < boxMin.z) - float(pos.z + tolerance > boxMax.z && pos.z - tolerance < boxMax.z);

	return normals;
}

HitInfo intersectBox(vec3 origin, vec3 dir, vec3 boxMin) 
{
	vec3 boxMax = boxMin + 1f;
	vec3 tMin = (boxMin - origin) / dir;
	vec3 tMax = (boxMax - origin) / dir;
	vec3 t1 = min(tMin, tMax);
	vec3 t2 = max(tMin, tMax);
	float tNear = max(max(t1.x, t1.y), t1.z);
	float tFar = min(min(t2.x, t2.y), t2.z);
	HitInfo hitInfo;
	hitInfo.near = tNear;
	hitInfo.far = tFar;
	hitInfo.pos = origin + (dir * tNear);
	hitInfo.hit = false;
	hitInfo.normal = getNormals(hitInfo.pos, boxMin, boxMax);
	if (tFar > 0 && tNear < tFar)
	{
		hitInfo.hit = true;
	}
	return hitInfo;
}




void main() {
	ivec2 pix = ivec2(gl_GlobalInvocationID.xy);
	ivec2 size = imageSize(destTex);
	vec4 color = vec4(0.0, 0.0, 0.0, 1.0);
	if (pix.x >= size.x || pix.y >= size.y) {
		return;
	}
	float ratio =(1.0f * size.x) / (1.0f * size.y);
	vec2 pos = vec2(pix) / vec2(size.x, size.y);
	pos -= 0.5f;
	pos.y /= ratio;
	vec3 o = vec3(0, -30, 0);
	vec3 r = vec3(pos.x, 1, pos.y);
	vec3 rR = vec3(r.x * cos(roll) - r.y * sin(roll), r.x * sin(roll) + r.y * cos(roll), r.z);
	vec3 oR = vec3(o.x * cos(roll) - o.y * sin(roll), o.x * sin(roll) + o.y * cos(roll), o.z);
	vec3 b1 =  vec3(-1  + cos(roll * 2) * 8, -5 + sin(roll * 5) * 8, -1  + sin(roll * 5) * 8);
	vec3 b2 = vec3(1 + cos(roll * 2) * 8, -3 + sin(roll * 5) * 8, 1 + sin(roll * 5) * 8);
	
	float dist = 100000000f;
	int vox = -1;
	HitInfo nearest;
	for (int i = 0; i < 200; i++)
	{
		HitInfo hit = intersectBox(oR, rR, vec3(voxelData[i].x,voxelData[i].y,voxelData[i].z));
		if (hit.hit && dist > hit.near)
		{
			dist = hit.near;
			vox = i;
			nearest = hit;
		}
	}
	if (vox >= 0)
	{
		color = vec4(voxelData[vox].r, voxelData[vox].g, voxelData[vox].b, 1f);
		if (voxelData[vox].reflection > 0.01f)
		{
			vec3 ref = reflect(rR, -nearest.normal);
			float dist2 = 100000000f;
			int vox2 = -1;
			for (int i = 0; i < 200; i++)
			{
				if (i == vox)
				{
					continue;
				}
				HitInfo reflection = intersectBox(nearest.pos, ref, vec3(voxelData[i].x,voxelData[i].y,voxelData[i].z));
				if (reflection.hit && dist2 > reflection.near)
				{
					dist2 = reflection.near;
					vox2 = i;
				}
			}
			if (vox2 >= 0)
			{
				color = mix(vec4(voxelData[vox2].r, voxelData[vox2].g, voxelData[vox2].b, 1f), color, voxelData[vox].reflection);
			}
		}
		

	}
	imageStore(destTex, pix, color);
	
};