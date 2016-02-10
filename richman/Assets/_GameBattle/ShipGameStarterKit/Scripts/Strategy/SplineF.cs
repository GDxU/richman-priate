using UnityEngine;
using System.Collections.Generic;

public class SplineF
{
	// Class rather than struct so it's passed by reference rather than by value
	public class CtrlPoint
	{
		public float mTime;
		public float mVal;
		public float mTan;
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
	public float		start	{ get { return front.mVal; } }
	public float		end		{ get { return back.mVal; } }
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
 
	public void AddKey (float time, float pos)
	{
		CtrlPoint ctrl = new CtrlPoint();
		ctrl.mTime = time;
		ctrl.mVal = pos;
		ctrl.mTan = 0.0f;
		mIsSmooth = false;

		int index = 0;

		for (int i = mCp.Count; i > 0; )
		{
			CtrlPoint point = mCp[--i];

			if (point.mTime < time)
			{
				index = i + 1;
				break;
			}
		}

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

				ctrlCurrent.mTan = Interpolation.GetHermiteTangent(
					ctrlCurrent.mVal	- ctrlPast.mVal,
					ctrlFuture.mVal		- ctrlCurrent.mVal,
					ctrlCurrent.mTime	- ctrlPast.mTime,
					ctrlFuture.mTime	- ctrlCurrent.mTime);
			}
		}
		mIsSmooth = true;
	}

	/// <summary>
	/// Convenience function
	/// </summary>

	public float Sample(float time, bool smooth)
	{
		return Sample(time, smooth ? SampleType.Spline : SampleType.Linear);
	}

	/// <summary>
	/// Sample the spline at the specified time
	/// </summary>

	public float Sample (float time, SampleType type)
	{
		if (mCp.Count == 0) return 0.0f;

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

				// Run through all control points until the proper time is encountered
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
							return Interpolation.Hermite(
								current.mVal, next.mVal,
								current.mTan, next.mTan,
								factor, duration);
						}
						return Interpolation.Linear(current.mVal, next.mVal, factor);
					}
				}
			}
			return back.mVal;
		}
		return front.mVal;
	}
}