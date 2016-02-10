// #define DEBUG

using UnityEngine;
using System.Collections;

namespace ProCore.Decals
{
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class qd_Decal : MonoBehaviour
{
	public Texture2D texture { get { return _texture; } }
	[HideInInspector] [SerializeField] private Texture2D _texture;
	[HideInInspector] [SerializeField] private Rect _rect;

	public void SetTexture(Texture2D tex)
	{
		_texture = tex;
	}

	public void SetUVRect(Rect r)
	{
		_rect = r;

		Vector2[] uvs = new Vector2[4]
		{
			new Vector2(_rect.x + _rect.width, _rect.y),
			new Vector2(_rect.x, _rect.y),
			new Vector2(_rect.x + _rect.width, _rect.y + _rect.height),
			new Vector2(_rect.x, _rect.y + _rect.height)
		};

		GetComponent<MeshFilter>().sharedMesh.uv = uvs;
	}

	/**
	 * Freeze the scale transform.  Useful 'cause none Vector3.one scales break dynamic batching.
	 */
	public void FreezeTransform()
	{
		Vector3 scale = transform.localScale;
		Mesh m = transform.GetComponent<MeshFilter>().sharedMesh;
		Vector3[] v = m.vertices;
		for(int i = 0; i < v.Length; i++)
			v[i] = Vector3.Scale(v[i], scale);
		m.vertices = v;
		transform.localScale = Vector3.one;
	}

#if DEBUG

	void OnDrawGizmos()
	{
		Mesh m = GetComponent<MeshFilter>().sharedMesh;

		Vector3 n = transform.TransformDirection(m.normals[0]);
		Gizmos.color = new Color(n.x, n.y, n.z, 1f);

		for(int i = 0; i < m.normals.Length; i++)
		{
			Gizmos.DrawLine( 	transform.TransformPoint(m.vertices[i]),
								transform.TransformPoint(m.vertices[i]) + transform.TransformDirection(m.normals[i]) * .2f);
		}
	}
#endif
}
}