using UnityEngine;
using System.Collections.Generic;

public class SplineQ
{
	// Class rather than struct so it's passed by reference rather than by value
	public class CtrlPoint
	{
		public float		mTime;
		public Quaternion	mVal;
		public float		mTan;		// Hermite control tangent, calculated
		public Quaternion	mCtrl;		// Hermite control point, calculated
	};

	// Sampling type
	public enum SampleType
	{
		Floor,
		Linear,
		Spline,
	};

	// Whether the spline tangents have been calculated
	private bool mIsSmooth = false;

	// Array holding the spline control points
	private List<CtrlPoint> mCp = new List<CtrlPoint>();

	// Cached values
	private int mLastIndex = 0;
	private float mLastSample = 0.0f;

	// Simplified internal access to control points
	private CtrlPoint	front	{ get { return mCp[0]; } }
	private CtrlPoint	back	{ get { return mCp[mCp.Count - 1]; } }
	public Quaternion	start	{ get { return front.mVal; } }
	public Quaternion	end		{ get { return back.mVal; } }
	public bool			isValid { get { return mCp.Count > 0; } }

	/// <summary>
	/// Read access to the control points
	/// </summary>

	public List<CtrlPoint> list { get { return mCp; } }

	/// <summary>
	/// Starting time for spline interpolation
	/// </summary>
 
	public float startTime
	{
		get
		{
			if (mCp.Count == 0) return 0.0f;
			return front.mTime;
		}
	}

	/// <summary>
	/// Time when the spline interpolation ends
	/// </summary>
 
	public float endTime
	{
		get
		{
			if (mCp.Count == 0) return 0.0f;
			return back.mTime;
		}
	}

	/// <summary>
	/// Length of the interpolation
	/// </summary>
 
	public float length
	{
		get
		{
			if (mCp.Count == 0) return 0.0f;
			return endTime - startTime;
		}
	}

	/// <summary>
	/// Clear all spline points
	/// </summary>
 
	public void Clear()
	{
		mCp.Clear();
		mIsSmooth = false;
	}

	/// <summary>
	/// Add a new key to the spline at the specified time
	/// </summary>
 
	public void AddKey(float time, Quaternion rot)
	{
		CtrlPoint ctrl = new CtrlPoint();
		ctrl.mTime = time;
		ctrl.mVal  = rot;
		ctrl.mTan  = 0.0f;
		ctrl.mCtrl = rot;
		mIsSmooth  = false;

		int index = 0;

		// Find the index where the control point should be added
		for (int i = mCp.Count; i > 0; )
		{
			CtrlPoint point = mCp[--i];

			if (point.mTime < time)
			{
				index = i + 1;
				break;
			}
		}

		// Ensure that the rotation always takes the shortest path
		if (index > 0 && mCp.Count > 0)
		{
			CtrlPoint prev = mCp[index - 1];

			if (Quaternion.Dot(prev.mVal, ctrl.mVal) < 0.0f)
			{
				ctrl.mVal = new Quaternion(-ctrl.mVal.x, -ctrl.mVal.y, -ctrl.mVal.z, -ctrl.mVal.w);
				ctrl.mCtrl = ctrl.mVal;
			}
		}

		// Add this control point
		if (index == mCp.Count)
		{
			mCp.Add(ctrl);
		}
		else
		{
			mLastIndex = 0;
			mLastSample = 0.0f;
			mCp.Insert(index, ctrl);
		}
	}

	/// <summary>
	/// Calculate the spline tangents
	/// </summary>
 
	private void Smoothen()
	{
		if (mCp.Count > 2)
		{
			for (int current = 1, end = mCp.Count - 1; current < end; ++current)
			{
				CtrlPoint ctrlPast		= mCp[current - 1];
				CtrlPoint ctrlCurrent	= mCp[current];
				CtrlPoint ctrlFuture	= mCp[current + 1];

				float duration0 = ctrlCurrent.mTime - ctrlPast.mTime;
				float duration1 = ctrlFuture.mTime - ctrlCurrent.mTime;

				float dot0 = Quaternion.Dot(ctrlCurrent.mVal, ctrlPast.mVal);
				float dot1 = Quaternion.Dot(ctrlCurrent.mVal, ctrlFuture.mVal);

				ctrlCurrent.mTan  = Interpolation.GetHermiteTangent( dot0, dot1, duration0, duration1 );
				ctrlCurrent.mCtrl = Interpolation.GetSquadControlRotation( ctrlPast.mVal, ctrlCurrent.mVal, ctrlFuture.mVal );
			}
		}
		mIsSmooth = true;
	}

	/// <summary>
	/// Convenience function
	/// </summary>

	public Quaternion Sample(float time, bool smooth)
	{
		return Sample(time, smooth ? SampleType.Spline : SampleType.Linear);
	}

	/// <summary>
	/// Sample the spline at the specified time
	/// </summary>

	public Quaternion Sample(float time, SampleType type)
	{
		if (mCp.Count == 0) return Quaternion.identity;

		if (time > front.mTime)
		{
			if (time < back.mTime)
			{
				if (!mIsSmooth) Smoothen();

				// Start at the last sampled keyframe if we're sampling forward
				int i = (mLastSample > time) ? 0 : mLastIndex;

				// Cache the current sampling time for the next execution
				mLastSample = time;
				mLastIndex = mCp.Count - 1;

				// Run through all contrl points until the proper time is encountered
				while (i < mLastIndex)
				{
					CtrlPoint current = mCp[i];
					CtrlPoint next = mCp[++i];

					if (time < next.mTime)
					{
						// Remember the current location
						mLastIndex = i - 1;

						if (type == SampleType.Floor) return current.mVal;

						float duration = next.mTime - current.mTime;
						float factor = (time - current.mTime) / duration;

						if (type == SampleType.Spline)
						{
							factor = Interpolation.Hermite(current.mTan, next.mTan, factor, duration);

							return Interpolation.Squad(	current.mVal, next.mVal,
														current.mCtrl, next.mCtrl,
														factor );
						}
						return Quaternion.Slerp(current.mVal, next.mVal, factor);
					}
				}
			}
			return back.mVal;
		}
		return front.mVal;
	}
}