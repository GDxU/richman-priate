using UnityEngine;

[AddComponentMenu("Strategy/Arrow Projector")]
public class ArrowProjector : MonoBehaviour
{
	public Color color = Color.white;
	public Vector3 originPos;
	public Vector3 targetPos;
	
	private Projector mProj = null;
	
	void OnEnable()
	{
		originPos = transform.position;
		originPos.y = 0.0f;
		mProj = GetComponent<Projector>();
		if (mProj != null) mProj.enabled = true;
	}
	
	void OnDisable()
	{
		if (mProj != null) mProj.enabled = false;
	}
	
	void LateUpdate()
	{
		if (mProj != null && mProj.material != null)
		{
			float factor = Mathf.Min(1.0f, Time.deltaTime * 10.0f);
			mProj.material.color = Color.Lerp(mProj.material.color, color, factor);
			if (mProj.material.color.a > 0.001f) UpdatePosition();
		}
	}
	
	void UpdatePosition()
	{
		// Position the projector directly between the origin and the target position
		Vector3 dir = targetPos - originPos;
		transform.position = originPos + dir * 0.5f;
		float mag = dir.magnitude;
		
		if (mag > 0.0f)
		{
			// Calculate for angle. Basic trigonometry: atan(opposite / adjacent)
			float angle = Mathf.Atan(2.0f * mProj.orthographicSize / mag) * Mathf.Rad2Deg;
			Vector3 euler = Quaternion.LookRotation(dir).eulerAngles;
	
			// The projector needs to point straight down, adding 90 degrees
			euler.x = 180.0f - angle;
			transform.rotation = Quaternion.Euler(euler);
		}
	}
}