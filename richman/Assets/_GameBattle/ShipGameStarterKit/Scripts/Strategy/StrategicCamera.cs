using UnityEngine;

[AddComponentMenu("Strategy/Strategic Camera")]
public class StrategicCamera : GameCameraTarget
{
	public delegate void OnClick (int button);
	
	static public Transform viewpoint = null;
	static public bool allowInput = true;
	static public OnClick onClick = null;

	public AnimationCurve angleCurve;
	public AnimationCurve distanceCurve;

	public Vector2 sensitivity 	= new Vector2(1.0f, 1.0f);
	public Vector3 followPos 	= new Vector3(0.0f, 2.0f, 0.0f);

	Transform	mTrans;
	Camera		mCam;
	float		mAngleY		= 0.0f; // Target Y rotation
	Vector2 	mDolly		= new Vector2(0.5f, 0.5f); // X = Current, Y = target
	Vector3 	mPos;
	Vector2		mDown;
	bool		mDragging 	= false;
	bool		mMouseHeld	= false;
	float		mLerpSpeed	= 1f;

	void Start()
	{
		mTrans = transform;
		mCam = Camera.main;
		Vector3 euler = mTrans.rotation.eulerAngles;
		mAngleY = euler.y;
	}

	void LateUpdate()
	{
		if (viewpoint != null)
		{
			float factor = Mathf.Min(1.0f, Time.deltaTime * 7.0f);
			mTrans.position = Vector3.Lerp(mTrans.position, viewpoint.position, factor);
			mTrans.rotation = Quaternion.Slerp(mTrans.rotation, viewpoint.rotation, factor);
			mLerpSpeed = 0f;
		}
		else if (!mMouseHeld && !allowInput)
		{
			allowInput = true;
		}
		else
		{
			ProcessInput();
			onClick = null;

			// Lerp speed is interpolated so that the camera exits 'viewpoint focus' mode smoothly
			mLerpSpeed = Mathf.Lerp(mLerpSpeed, 1f, Mathf.Clamp01(Time.deltaTime * 5f));

			// Lerp the 'look at' position
			float factor = Mathf.Clamp01(Time.deltaTime * 20.0f * mLerpSpeed);
			mPos = Vector3.Lerp(mPos, followPos, factor);

			float angleX = angleCurve.Evaluate(mDolly.x);
			Quaternion targetRot = Quaternion.Euler(angleX, mAngleY, 0.0f);

			// Lerp the final position and rotation of the camera, offset by the dolly distance
			Vector3 offset = Vector3.forward * distanceCurve.Evaluate(mDolly.x);
			mTrans.position = Vector3.Lerp(mTrans.position, mPos - targetRot * offset, factor);
			mTrans.rotation = Quaternion.Slerp(mTrans.rotation, targetRot, factor);
		}
	}

	/// <summary>
	/// Handle all mouse input.
	/// </summary>
	
	void ProcessInput()
	{
		float factor = Mathf.Min(1.0f, Time.deltaTime * 10.0f);
		mDolly.y = Mathf.Clamp01(mDolly.y - Input.GetAxis("Mouse ScrollWheel") * sensitivity.y * 0.25f);
		mDolly.x = mDolly.x * (1.0f - factor) + mDolly.y * factor;

		// Remember when we start holding the mouse
		if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
		{
			mDown = Input.mousePosition;
			mMouseHeld = true;
			mDragging = false;
		}

		// Left mouse button should also click if it hasn't moved much
		if (Input.GetMouseButtonUp(0))
		{
			if (!mDragging)
			{
				if (onClick != null)
				{
					onClick(0);
				}
				else
				{
					RaycastHit hitInfo;

					if (Physics.Raycast(mCam.ScreenPointToRay(Input.mousePosition), out hitInfo, 300f))
					{
						int ship = LayerMask.NameToLayer("Ship");
						
						if (hitInfo.collider.gameObject.layer == ship)
						{
							DebugExt.Log("Clicked on a ship: " + hitInfo.collider.name);
						}
						else
						{
							Town town = Town.Find(hitInfo.collider.gameObject);

							if (town != null)
							{
								town.showInfo = true;
							}
						}
					}
				}
			}
		}

		// Right mouse click notification
		if (Input.GetMouseButtonUp(1))
		{
			if (!mDragging)
			{
				if (onClick != null)
				{
					onClick(1);
				}
			}
		}

		// Detect when the mouse is no longer held
		if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
		{
			if (!Input.GetMouseButton(0) && !Input.GetMouseButton(1))
			{
				mMouseHeld = false;
			}
		}

		if (mMouseHeld)
		{
			//Screen.showCursor = !mDragging;
			
			if (!mDragging)
			{
				Vector2 pos;
				pos.x = Input.mousePosition.x;
				pos.y = Input.mousePosition.y;
				
				if (Vector2.Distance(mDown, pos) > 3.0f) mDragging = true;
			}

			if (Input.GetMouseButton(0))
			{
				if (mDragging)
				{
					mAngleY += Input.GetAxis("Mouse X") * sensitivity.x * 4.5f;
					mAngleY = ToolCalculations.WrapAngle(mAngleY);
				}
			}
			else if (Input.GetMouseButton(1))
			{
				Vector3 fw = mTrans.rotation * Vector3.forward;
				fw.y = 0.0f;
				fw.Normalize();

				Vector3 rt = mTrans.rotation * Vector3.right;
				rt.y = 0.0f;
				rt.Normalize();

				factor = Mathf.Lerp(0.5f, 2.0f, mDolly.x);
				followPos += fw * Input.GetAxis("Mouse Y") * factor;
				followPos += rt * Input.GetAxis("Mouse X") * factor;
			}
		}
		else if (!Config.Instance.showWelcome)
		{
			// Mouse cursor's position on the water plane in view space
			Vector3 viewPos = mCam.WorldToViewportPoint(Game.GetMouseWaterPosition(mCam));
			Vector3 offset = Vector3.zero;

			// If the mouse is on the edge of the screen we want to move the camera in that direction
			if		(viewPos.x < 0.15f && viewPos.x > 0f) offset.x -= (1.0f - viewPos.x    / 0.15f);
			else if (viewPos.x > 0.85f && viewPos.x < 1f) offset.x += ((viewPos.x - 0.85f) / 0.15f);
			if		(viewPos.y < 0.15f && viewPos.y > 0f) offset.z -= (1.0f - viewPos.y    / 0.15f);
			else if (viewPos.y > 0.85f && viewPos.y < 1f) offset.z += ((viewPos.y - 0.85f) / 0.15f);

			// We want only the flat XZ plane direction relative to the camera's rotation
			Vector3 dir = mTrans.rotation * Vector3.forward;
			dir.y = 0f;
			dir.Normalize();
			Quaternion flatRot = Quaternion.LookRotation(dir);

			// Transform the offset by the flat rotation
			followPos += flatRot * offset * (mDolly.x * Time.deltaTime * 50f);
		}
	}
}