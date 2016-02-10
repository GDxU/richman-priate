using UnityEngine;
using System.Collections.Generic;

public class SplineV
{
	// Class rather than struct so it's passed by reference rather than by value
	public class CtrlPoint
	{
		public float mTime;
		public Vector3 mVal;
		public Vector3 mTan;
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
	public Vector3		start	{ get { return front.mVal; } }
	public Vector3		end		{ get { return back.mVal; } }
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
 
	public void AddKey (float time, Vector3 pos)
	{
		CtrlPoint ctrl = new CtrlPoint();
		ctrl.mTime = time;
		ctrl.mVal = pos;
		ctrl.mTan = Vector3.zero;
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

	public Vector3 Sample(float time, bool smooth)
	{
		return Sample(time, smooth ? SampleType.Spline : SampleType.Linear);
	}

	/// <summary>
	/// Sample the spline at the specified time
	/// </summary>

	public Vector3 Sample (float time, SampleType type)
	{
		if (mCp.Count == 0) return Vector3.zero;

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
	
	/// <summary>
	/// Gets the magnitude of the spline.
	/// </summary>
	
	public float GetMagnitude()
	{
		float mag = 0f;
		
		if (mCp.Count > 1)
		{
			for (int i = 1, imax = mCp.Count; i < imax; ++i)
			{
				mag += (mCp[i].mVal - mCp[i-1].mVal).magnitude;
			}
		}
		return mag;
	}
	
	/// <summary>
	/// Normalize the specified spline.
	/// </summary>
	
	public static SplineV Normalize (SplineV initial, int subdivisions)
	{
		float key = 0.0f;
		float start = initial.startTime;
		float length = initial.endTime - start;
		
		// Add the first key
		SplineV spline = new SplineV();
		spline.AddKey(key, initial.start);
		
		// Add remaining values using the traveled distance as keys
		for (int i = 1; i < subdivisions; ++i)
		{
			float f0 = (float)(i - 1) / subdivisions;
			float f1 = (float)(i    ) / subdivisions;
			
			Vector3 v0 = initial.Sample(start + f0 * length, SplineV.SampleType.Spline);
			Vector3 v1 = initial.Sample(start + f1 * length, SplineV.SampleType.Spline);
			
			key += (v1 - v0).magnitude;
			spline.AddKey(key, v1);
		}
		return spline;
	}
}