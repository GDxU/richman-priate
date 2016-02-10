//============================================================================================================
// Weili Zhi Copy right reserved.
//============================================================================================================
/// Action Base
//============================================================================================================
// Created on 18/7/2012 9:06:52 AM by Weili Zhi
//============================================================================================================
using UnityEngine;
using System;

public class SteerWheelController : AIControllerBase
{
	public GameObject PowerTarget = null;
	public float MaxAngle = 720.0f;
	public float MinAngle = -720.0f;
	public float MaxRotationSpeed = 1500f;
	public float SlowAcc = 500f;
	public float RewindAcc = 500f;
	public bool AllowRewind = true;
	public bool SupportMouseControl = true;
	public bool SupportKeyboardControl = true;
	
	private Vector3 mCenter = Vector3.zero;
	private bool mHolding = false;
	private float mCurrentSpeed = 0.0f;
	private DragMessage mLastMessage = null;
	private bool mLeavingSteerPositive = false;
	private float mRewindAngle = 0f;
	private float mRewindTime = 0f;
	private float mHalfwayRewindTime = 0f;
	private float mTotalAngle = 0.0f;
	private MouseDrag mMouseDrag = null;
	private KeyBoardController mKeyboard = null;
    
	void Awake()
	{
		if( SupportMouseControl ) {
			mMouseDrag = gameObject.AddComponent<MouseDrag>();
			Rect dragArea = new Rect(0f, 0f, 200f, 150f);
			mMouseDrag.DragArea = dragArea;
			mMouseDrag.MessageTarget = gameObject;
		}
		if( SupportKeyboardControl ) {
			mKeyboard = gameObject.AddComponent<KeyBoardController>();
		}	
	}
	
	void Start()
	{
		Camera parentCamera = transform.parent.GetComponent<Camera>();
		if( parentCamera == null ) {
			DebugExt.Log("SteerWheelController needs to work with a game object under a camera");
			ControllerEnabled = false;
		}
		mCenter = transform.localPosition;
	}
	
	public void DragStart()
	{
		if( !ControllerEnabled ) return;
		mHolding = true;
		StopRewind();
	}
	
	public void DragMove(DragMessage msg)
    {
		if( !ControllerEnabled ) return;
		float turnAngle = - msg.DeltaPosition.x;
		if( msg.StartPosition.x < - mCenter.x ) {
			turnAngle -= msg.DeltaPosition.y;
		}
		else turnAngle += msg.DeltaPosition.y;
		
		SetWheelRotationAngle( mTotalAngle + turnAngle );
		if( mTotalAngle > MaxAngle ) { 
			turnAngle -= (mTotalAngle - MaxAngle);
            SetWheelRotationAngle(MaxAngle);
			mCurrentSpeed = 0.0f;
		}
		else if( mTotalAngle < MinAngle ) {
			turnAngle -= (mTotalAngle - MinAngle);
            SetWheelRotationAngle(MinAngle);
			mCurrentSpeed = 0.0f;
		}
		else {
			mCurrentSpeed = turnAngle / msg.DeltaTime;
			if( mCurrentSpeed > MaxRotationSpeed ) mCurrentSpeed = MaxRotationSpeed;
		}
		transform.rotation = transform.rotation * Quaternion.Euler( 0, 0, turnAngle );
		mLastMessage = msg;
    }
	
    public void DragEnd()
    {
		if( !ControllerEnabled ) return;
        mHolding = false;
		mLeavingSteerPositive = mCurrentSpeed > 0? true : false;
    }
	
	void KeyBoardDrive(float x)
	{
		if( mLastMessage == null ) {
			mLastMessage = new DragMessage();
			mLastMessage.StartPosition = Vector2.zero;
		}
		
		Vector2 endPosition = mLastMessage.StartPosition;
		endPosition.x -= x * MaxAngle;
		mLastMessage.EndPosition = endPosition;
		mLastMessage.DeltaTime = Time.deltaTime;
		mLastMessage.DeltaPosition = mLastMessage.StartPosition - mLastMessage.EndPosition;
		DragMove(mLastMessage);
	}
	
	protected override void OnFrameUpdate()
    {
		if( !mHolding ) {
			if((mLeavingSteerPositive && mCurrentSpeed > 0f ) ||
				(!mLeavingSteerPositive && mCurrentSpeed < 0f )) {
				float slowDown = SlowAcc * Time.deltaTime;
				Vector2 endPosition = mLastMessage.StartPosition;
				endPosition.x += mCurrentSpeed * Time.deltaTime;;
				mLastMessage.EndPosition = endPosition;
				mLastMessage.DeltaTime = Time.deltaTime;
				mLastMessage.DeltaPosition = mLastMessage.StartPosition - mLastMessage.EndPosition;
				DragMove(mLastMessage);
				if( mCurrentSpeed > 0 )
					mCurrentSpeed -= slowDown;
				else
					mCurrentSpeed += slowDown;
				if( Math.Abs(mCurrentSpeed) <= slowDown) mCurrentSpeed = 0.0f;
			}
			else if( AllowRewind && mTotalAngle != 0f ){ // Speed is 0, so now rewind back to 0.
				if( mRewindAngle == 0f ) { // Start to rewind.
					mRewindTime = 0f;
					mRewindAngle = mTotalAngle;
					RewindAcc = Mathf.Abs(RewindAcc);
					mHalfwayRewindTime = (float) Mathf.Abs(Mathf.Sqrt( Mathf.Abs(mTotalAngle / RewindAcc)));
					if( mTotalAngle < 0 ) RewindAcc = -RewindAcc;
					
				}
				else { // Keep going.
					mRewindTime += Time.deltaTime;
					if( mRewindTime < mHalfwayRewindTime ) {
                        SetWheelRotationAngle((mRewindAngle - RewindAcc * mRewindTime * mRewindTime / 2) % 360f);
						transform.rotation = transform.parent.rotation * Quaternion.Euler( 0f, 0f, mTotalAngle );
					}
					else if( mRewindTime < mHalfwayRewindTime * 2 ){
						float restTime = mHalfwayRewindTime * 2 - mRewindTime;
                        SetWheelRotationAngle((RewindAcc * restTime * restTime / 2) % 360f);
						transform.rotation = transform.parent.rotation * Quaternion.Euler( 0f, 0f, mTotalAngle );
					}
					else {
                        SetWheelRotationAngle(0);
						transform.rotation = transform.parent.rotation * Quaternion.Euler( 0f, 0f, 0f );
						StopRewind();
					}
				}
			}
		}
    }
	
	private void StopRewind()
	{
		mRewindTime = 0;
		mHalfwayRewindTime = 0;
		mRewindAngle = 0;
	}

    private void SetWheelRotationAngle(float angle)
    {
		if( mTotalAngle == angle && angle == 0 ) return;
		mTotalAngle = angle;
		if( PowerTarget != null ) {
			GlobalMethods.SendMessage(PowerTarget, "SetSteerPower", mTotalAngle / MaxAngle);
		}
	}
	
	public override void DisableController()
	{
		base.DisableController();
		if( mMouseDrag != null ) mMouseDrag.enabled = false;
		if( mKeyboard != null ) mKeyboard.enabled = false;
	}
	
	public override void EnableController()
	{
		base.EnableController();
		if( mMouseDrag != null ) mMouseDrag.enabled = true;
		if( mKeyboard != null ) mKeyboard.enabled = true;
	}
	
	public void SetForceTarget(GameObject target)
	{
		PowerTarget = target;
	}
}