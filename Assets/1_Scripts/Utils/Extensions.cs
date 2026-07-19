using UnityEngine;

namespace MeowTruck
{
	public static class Extensions
	{
		public static Vector2 ToVec2(this Vector3 vec3) => new Vector2(vec3.x, vec3.y);
		public static Vector3 ToVec3(this Vector2 vec2) => new Vector3(vec2.x, vec2.y, 0);
	}
}