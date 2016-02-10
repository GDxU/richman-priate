//#define TRIANGLE_STRIP

using UnityEngine;
using System.Collections.Generic;

[AddComponentMenu("Game/Improved Trail")]
public class ImprovedTrail : MonoBehaviour
{
	// Material used by the trail renderer
	public Material material;

	// Allowed length of each segment before a new point gets created
	public float maxSegmentLength = 1f;

	// Allowed angle difference between segments before a new point gets created
	public float maxSegmentAngleVariance = 10f;

	// Maximum allowed change in alpha before a new point gets created
	public float maxSegmentAlphaVariance = 0.1f;

	// Desired lifetime of each emitted point
	public float segmentLifetime = 1f;

	// Alpha applied to newly emitted points
	public float alpha = 1f;

	// Velocity applied to each point in units per second
	public Vector3 localVelocity = Vector3.zero;

	// Velocity falloff -- the higher it is the quicker the velocity fades away
	public float velocityFalloffPower = 1f;

	// Alpha based on time
	public AnimationCurve alphaCurve;

	// Curve used to adjust the size of the trail with time
	public AnimationCurve sizeCurve;

	// Colors used for interpolation
	public Color[] colors = null;

	/// <summary>
	/// Normal defining a plane on which all points will be placed.
	/// If left as zero, Camera's plane will be used instead.
	/// For 3D trails, leave this at its default value.
	/// For 2D trails, specify the normal (for example Vector3.up for a flat XZ plane). 
	/// </summary>

	public Vector3 planeNormal = Vector3.zero;

	// Destroy the game object when no longer visible
	[HideInInspector] public bool destroyWhenInvisible = false;

	GameObject mChild;
	Transform mTrans;
	Transform mCamera;
	Mesh mMesh;
	MeshFilter mFilter;
	MeshRenderer mRen;

	class Point
	{
		public Vector3 pos;
		public Vector3 dir;
		public Vector3 cross;
		public Vector3 vel;
		public Color col;
		public float life;
		public float alpha;
		public float creationTime;
	}

	List<Point> mPoints = new List<Point>();
	List<Point> mUnused = new List<Point>();

	/// <summary>
	/// Helper function that returns the appropriate color for the specified 0-1 range life.
	/// </summary>

	Color GetColor (float life)
	{
		if (colors == null || colors.Length == 0) return Color.white;

		life = Mathf.Clamp01(life);
		life = 1.0f - life;
		life *= (colors.Length - 1);

		int index = Mathf.FloorToInt(life);
		life -= index;

		if (index + 1 < colors.Length)
		{
			return Color.Lerp(colors[index], colors[index + 1], life);
		}
		return colors[index];
	}

	/// <summary>
	/// Add a new point to the list.
	/// </summary>

	Point Add ()
	{
		if (mUnused.Count > 0)
		{
			int last = mUnused.Count - 1;
			Point p = mUnused[last];
			p.creationTime = Time.time;
			p.vel = localVelocity;
			p.life = 1f;
			mUnused.RemoveAt(last);
			mPoints.Add(p);
			return p;
		}

		Point newPoint = new Point();
		newPoint.creationTime = Time.time;
		newPoint.vel = localVelocity;
		newPoint.life = 1f;
		mPoints.Add(newPoint);
		return newPoint;
	}

	/// <summary>
	/// Discard the specified point, placing it into the unused list.
	/// </summary>

	void Discard (Point p) { if (mPoints.Remove(p)) { mUnused.Add(p); } }

	/// <summary>
	/// Create the first 2 points, add a mesh renderer and a mesh filter.
	/// </summary>

	void Start ()
	{
		mTrans = transform;
		mChild = new GameObject(GetType() + " for " + name);
		mCamera = Camera.main.transform;
		mFilter = mChild.AddComponent<MeshFilter>();
		mRen = mChild.AddComponent<MeshRenderer>();
		mMesh = new Mesh();

		// It's always a good idea to name dynamically created meshes
		mMesh.name = gameObject.name + " (" + GetType() + ")";
		mFilter.mesh = mMesh;
		mRen.material = material;

		// Add the first 2 points forming a line
		{
			Point p = Add();
			p.pos = mTrans.position;
			p.dir = Vector3.forward;
			p.cross = Vector3.right;
			p.alpha = 0f;

			p = Add();
			p.pos = mTrans.position;
			p.dir = Vector3.forward;
			p.cross = Vector3.right;
			p.alpha = 0f;
		}
	}

	/// <summary>
	/// Enable/disable the trail when this script gets the event.
	/// </summary>

	void OnEnable ()  { if (mChild != null) mChild.active = true; }
	void OnDisable () { if (mChild != null) mChild.active = false; }

	/// <summary>
	/// Release all resources we created.
	/// </summary>

	void OnDestroy ()
	{
		Destroy(mRen);
		Destroy(mFilter);
		Destroy(mChild);
		Destroy(mMesh);
	}

	/// <summary>
	/// Emit or cull points, update the mesh.
	/// </summary>

	void LateUpdate ()
	{
		// Update the last point's position
		{
			Point prev = mPoints[mPoints.Count - 2];
			Point curr = mPoints[mPoints.Count - 1];

			// Calculate the new position and direction
			Vector3 newPos = mTrans.position;
			Vector3 newDir = newPos - prev.pos;

			// Calculate the distance traveled since the last point creation
			float distance = newDir.magnitude;

			if (distance > 0.001f)
			{
				// We will want to create a new point if the magnitude exceeded the max length
				bool create = (distance > maxSegmentLength);

				// Normalize the direction
				newDir *= (1.0f / distance);

				if (mPoints.Count == 2)
				{
					// We only have 2 points
					prev.dir = newDir;
					prev.alpha = 0f;
					create = true;
				}
				else if (!create && distance > maxSegmentLength * 0.5f)
				{
					// Check to see if the angle has changed significantly enough
					float angle = Vector3.Angle(newDir, prev.dir);
					create = (angle > maxSegmentAngleVariance);

					if (!create)
					{
						float alphaChange = Mathf.Abs(alpha - prev.alpha);
						create = (alphaChange > maxSegmentAlphaVariance);
					}
				}

				// Update the last point
				curr.pos = newPos;
				curr.dir = newDir;
				curr.alpha = alpha;
				curr.creationTime = Time.time;
				curr.life = 1f;

				if (create)
				{
					// Add a new point
					Point newPoint = Add();
					newPoint.pos = newPos;
					newPoint.dir = newDir;
					newPoint.alpha = alpha;
				}
			}
		}

		if (mPoints.Count > 2)
		{
			// How much each emitted point decays this update
			float decay = (segmentLifetime > 0f) ? Time.deltaTime / segmentLifetime : 1f;

			// Reduce the life and alpha of all points
			for (int i = 0, imax = mPoints.Count - 1; i < imax; ++i)
			{
				Point p = mPoints[i];
				p.life -= decay;
				p.alpha -= decay;
				if (p.life < 0f || p.alpha < 0f) p.alpha = 0f;
			}

			// Eliminate expired points
			while (mPoints.Count > 2)
			{
				Point curr = mPoints[0];
				Point next = mPoints[1];
				if (curr.alpha == 0f && next.alpha == 0f) Discard(curr);
				else break;
			}
		}

		// Since the last point is always temporary, we need at least 3 points in order to draw anything
		if (mPoints.Count > 2)
		{
			bool dynamicNormal = (planeNormal.x + planeNormal.y + planeNormal.z != 1f);
			Vector3 camPos = mCamera.position;

			// Recalculate the cross products
			for (int i = 0; i < mPoints.Count; ++i)
			{
				Point p = mPoints[i];
				p.col = GetColor(p.life);

				// All points except the last should be affected by velocity
				if (i + 1 < mPoints.Count)
				{
					if (velocityFalloffPower != 1f && (p.vel.sqrMagnitude != 0f))
					{
						float factor = 1.0f - (Time.time - p.creationTime) / segmentLifetime;
						factor = Mathf.Pow(factor, velocityFalloffPower);
						factor *= Time.deltaTime;
						p.pos += mTrans.TransformDirection(p.vel) * factor;
					}
					else
					{
						p.pos += mTrans.TransformDirection(p.vel) * Time.deltaTime;
					}
				}

				if (dynamicNormal)
				{
					// Dynamic normal vector -- use the camera
					Vector3 eyeDir = (camPos - p.pos).normalized;
					p.cross = Vector3.Cross(p.dir, eyeDir).normalized;
				}
				else
				{
					// Fixed normal vector -- use the specified value
					p.cross = Vector3.Cross(p.dir, planeNormal).normalized;
				}
			}
			
			// Update the mesh
			UpdateMesh();
		}
		else
		{
			mMesh.Clear();
			if (destroyWhenInvisible) Destroy(gameObject);
		}
	}

	/// <summary>
	/// Update the mesh, creating triangles based on our points and the camera's orientation.
	/// </summary>

	void UpdateMesh ()
	{
		// It takes 2 vertices per point in order to draw it
		int pointCount = mPoints.Count;
		int vertexCount = pointCount << 2;

#if TRIANGLE_STRIP
		int indexCount = vertexCount;
#else
		int indexCount = (pointCount - 1) * 6;
#endif

		Vector3[] vertices = new Vector3[vertexCount];
		Vector2[] uvs = new Vector2[vertexCount];
		Color[] cols = new Color[vertexCount];
		int[] indices = new int[indexCount];

		for (int i = 0; i < pointCount; ++i)
		{
			Point p = mPoints[i];

			// Calculate the combined cross product
			Vector3 cross = (i + 1 < pointCount) ? (p.cross + mPoints[i + 1].cross) * 0.5f : p.cross;

			float time = 1.0f - p.life;
			cross *= 0.5f * sizeCurve.Evaluate(time);

			int i0 = i << 1;
			int i1 = i0 + 1;

			vertices[i0] = p.pos + cross;
			vertices[i1] = p.pos - cross;

			float y = 1.0f - p.life;
			uvs[i0] = new Vector2(0f, y);
			uvs[i1] = new Vector2(1f, y);

			Color c = p.col;
			c.a *= p.alpha * alphaCurve.Evaluate(time);
			cols[i0] = c;
			cols[i1] = c;

#if TRIANGLE_STRIP
			indices[i0] = i0;
			indices[i1] = i1;
#endif
		}

#if !TRIANGLE_STRIP
		// Calculate the indices
		for (int i = 0, index = 0; index < indexCount; i += 2)
		{
			indices[index++] = i;
			indices[index++] = i + 2;
			indices[index++] = i + 1;

			indices[index++] = i + 1;
			indices[index++] = i + 2;
			indices[index++] = i + 3;
		}
#endif
		// Update the mesh
		mMesh.Clear();
		mMesh.vertices = vertices;
		mMesh.uv = uvs;
		mMesh.colors = cols;

		// NOTE: Triangle strip approach seems to connect the last triangle with the first. Unity bug?
#if TRIANGLE_STRIP
		mMesh.SetTriangleStrip(indices, 0);
#else
		mMesh.triangles = indices;
#endif
		mMesh.RecalculateBounds();

		// Cleanup so Unity crashes less
		vertices = null;
		uvs		 = null;
		cols	 = null;
		indices  = null;
	}
}