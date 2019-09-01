struct Plane
{
	vec3 normal;
	float dist;
};

struct AABB
{
	vec3 pos;
	float size;
};

float RayDistance(Plane plane, vec3 origin, vec3 direction)
{
	float dotProduct = dot(plane.normal, direction);
	return (plane.dist - dot(plane.normal, origin)) / dotProduct;
}

bool Contains(AABB box, vec3 point)
{
	return (point.x > box.pos.x && point.y > box.pos.y && point.z > box.pos.z && point.x < box.pos.x + box.size && point.y < box.pos.y + box.size && point.z < box.pos.z + box.size);
}