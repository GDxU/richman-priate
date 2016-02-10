using UnityEngine;

static public class Game
{
	/// <summary>
	/// Gets the position of the mouse on the water plane.
	/// </summary>

	static public Vector3 GetMouseWaterPosition ()
	{
		return GetMouseWaterPosition(Camera.main);
	}

	/// <summary>
	/// Gets the position of the mouse on the water plane.
	/// </summary>

	static public Vector3 GetMouseWaterPosition (Camera cam)
	{
		// Since the water plane is always at (0, 0, 0) and points straight up, distance
		// to plane calculation has been greatly simplified.
		Ray ray = cam.ScreenPointToRay(Input.mousePosition);
		return ray.origin - ray.direction * (ray.origin.y / ray.direction.y);
	}
}