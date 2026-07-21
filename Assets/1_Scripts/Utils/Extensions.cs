using UnityEngine;

namespace MeowTruck
{
	public static class Extensions
	{
		public static Vector2 ToVec2(this Vector3 vec3) => new Vector2(vec3.x, vec3.y);
		public static Vector3 ToVec3(this Vector2 vec2) => new Vector3(vec2.x, vec2.y, 0);
		public static Quaternion ToQuaternion(this Vector2 dir) { float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg; return Quaternion.Euler(0f, 0f, angle); }
	}
}