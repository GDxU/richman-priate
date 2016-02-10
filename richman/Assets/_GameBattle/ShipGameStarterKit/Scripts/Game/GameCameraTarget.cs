using UnityEngine;

[AddComponentMenu("Game/Camera Target")]
public class GameCameraTarget : MonoBehaviour
{
	public void Activate (bool val)
	{
		if (val) GameCamera.AddTarget(transform);
		else GameCamera.RemoveTarget(transform);
	}

	void OnEnable  () { Activate(true);  }
	void OnDisable () { Activate(false); }
}