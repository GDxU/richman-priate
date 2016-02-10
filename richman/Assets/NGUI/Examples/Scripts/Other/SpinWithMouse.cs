using UnityEngine;

[AddComponentMenu("NGUI/Examples/Spin With Mouse")]
public class SpinWithMouse : MonoBehaviour
{
		public Transform target;
		public float speed = 1f;
		public Vector3 direction = Vector3.up;
		Transform mTrans;
		private Vector2 d;

		void Start ()
		{
				mTrans = transform;
		}

		void OnDrag (Vector2 delta)
		{
				d = delta;
				UICamera.currentTouch.clickNotification = UICamera.ClickNotification.None;
				Vector3 on = direction * -0.5f * delta.x * delta.y * speed;
				if (target != null) {
						target.localRotation = Quaternion.Euler (on) * target.localRotation;
				} else {
						mTrans.localRotation = Quaternion.Euler (on) * mTrans.localRotation;
				}
		}
}