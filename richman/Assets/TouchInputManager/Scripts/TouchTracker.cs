using PigeonCoopUtil;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TIMH = TouchInputManagerBackend.TouchInputManagerHelper;

namespace TouchInputManagerBackend
{
    [AddComponentMenu("")]
    public class TouchTracker : MonoBehaviour
    {

        #region PublicTweakableMembers
	
        public Rect activeRegion;
        public Vector2 centeredPositionOffset;
        public string _id;
	
        #endregion
	
        public delegate void TouchLatchUnlatch(int t);
	
        /// <summary>
        /// Quick check to see if there are any figners latched to this tracker
        /// </summary>
        public bool AnyLatchedFingers { get { return _trackedFingerIDs.Count >= 1; } }
	
        /// <summary>
        /// A quick access to the initial latched screen position 
        /// of the first tracked finger in the list of tracked fingers
        /// </summary>
        public Vector2 FirstInitialScreenPosition { 
            get 
            { 
                if(AnyLatchedFingers)
                {
                    return _trackedFingers[_trackedFingerIDs[0]].initialScreenPosition;
                }
                else
                {
                    Debug.LogError("Touch Tracker: FirstInitialScreenPosition cannot be called when no fingers are latched.");
                    return Vector2.zero;
                }
            }
        }
	
        /// <summary>
        /// A quick access to the initial latched position of the 
        /// first tracked finger in the list of tracked fingers
        /// </summary>
        public Vector2 FirstInitialPosition { 
            get 
            { 
                if(AnyLatchedFingers)
                {
                    return _trackedFingers[_trackedFingerIDs[0]].initialPosition;
                }
                else
                {
                    Debug.LogError("Touch Tracker: FirstInitialPosition cannot be called when no fingers are latched.");
                    return Vector2.zero;
                }
            }
        }
	
        /// <summary>
        /// A quick access to the screen position 
        /// of the first tracked finger in the list of tracked fingers
        /// </summary>
        public Vector2 FirstCurrentScreenPosition { 
            get 
            { 
                if(AnyLatchedFingers)
                {
                    return TIMH.ScreenSpace(_trackedFingers[_trackedFingerIDs[0]].positionHistory.First());
                }
                else
                {
                    Debug.LogError("Touch Tracker: FirstCurrentScreenPosition cannot be called when no fingers are latched.");
                    return Vector2.zero;
                }
            }
        }
	
        /// <summary>
        /// A quick access to the position of the 
        /// first tracked finger in the list of tracked fingers
        /// </summary>
        public Vector2 FirstCurrentPosition { 
            get 
            { 
                if(AnyLatchedFingers)
                {
                    return _trackedFingers[_trackedFingerIDs[0]].positionHistory.First();
                }
                else
                {
                    Debug.LogError("Touch Tracker: FirstCurrentPosition cannot be called when no fingers are latched.");
                    return Vector2.zero;
                }
            }
        }
	
	
        protected Dictionary<int, TrackedFingerInformation> _trackedFingers = new Dictionary<int, TrackedFingerInformation>();
        protected List<int> _trackedFingerIDs = new List<int>();
        protected TouchTrackerConfig config = new TouchTrackerConfig();
	
        protected TouchLatchUnlatch OnLatch, OnUnlatch;
	
        /// <summary>
        /// Sets the configuration option for 
        /// this tracker - how many finger we 
        /// should track and how many positions
        /// into the past we should track per finger
        /// </summary>
        public void SetConfig(TouchTrackerConfig ttc)
        {
            config = ttc;
        }
	
        /// <summary>
        /// Run every frame, adds the current position of any tracked
        /// fingers into a circular buffer of positions
        /// </summary>
        private void Update()
        {
            _trackedFingerIDs.ForEach(a => _trackedFingers[a].positionHistory.Add(TIMH.TouchPosition(a)));
        }

        /// <summary>
        /// Is finger t within this region
        /// </summary>
        public bool IsWithinRegion(int t)
        {
            Vector2 position = TIMH.TouchScreenSpacePosition(t);
            return activeRegion.Contains(new Vector2(position.x, position.y));
        }

        /// <summary>
        /// Whether or not this tracker can latch on another touch
        /// </summary>
        public bool CanLatchTouch()
        {
            return _trackedFingerIDs.Count < config.maxSimultaneousPoints;
        }
	
        /// <summary>
        /// Whether or not this tracker is latched to finger T
        /// </summary>
        public bool IsLatchedTo(int t)
        {
            return _trackedFingerIDs.Contains(t);
        }

        /// <summary>
        /// Called when a finger has successfully latched on
        /// </summary>
        public virtual void RegisterOnLatch(TouchLatchUnlatch d) 
        {
            OnLatch-=d;
            OnLatch+=d;
        }
	
        public virtual void UnregisterOnLatch(TouchLatchUnlatch d) 
        {
            OnLatch-=d;
        }
	
        /// <summary>
        /// Called when a finger has unlatched
        /// </summary>
        public virtual void RegisterOnUnlatch(TouchLatchUnlatch d) 
        {
            OnUnlatch-=d;
            OnUnlatch+=d;
        }
	
        public virtual void UnregisterOnUnlatch(TouchLatchUnlatch d) 
        {
            OnUnlatch-=d;
        }
	

        /// <summary>
        /// Associates this input element with finger t
        /// </summary>
        public void LatchFinger(int t)
        {
            _trackedFingers.Add(t, new TrackedFingerInformation(TIMH.TouchScreenSpacePosition(t),TIMH.TouchPosition(t),config.maxPositionHistory));
            _trackedFingers[t].positionHistory.Add(TIMH.TouchPosition(t));
            _trackedFingerIDs.Add(t);
            OnLatch(t);
        }

        /// <summary>
        /// Disassotiates this input element with finger t
        /// </summary>
        public void UnlatchFinger(int t)
        {
            OnUnlatch(t);
            _trackedFingers.Remove(t);
            _trackedFingerIDs.Remove(t);
        }

        /// <summary>
        /// Unlatched all fingers
        /// </summary>
        public void ResetTouch()
        {
            _trackedFingerIDs.ForEach(a => OnUnlatch(a));
            _trackedFingerIDs.Clear();
            _trackedFingers.Clear();

        }
    }
}

public class TouchTrackerConfig
{
    public int maxSimultaneousPoints = 1;
	public int maxPositionHistory = 1;
}

/// <summary>
/// Tracked finger information.
/// </summary>
public class TrackedFingerInformation
{
	/// <summary>
	/// The position history.
	/// </summary>
	public CircularBuffer<Vector2> positionHistory;
	
	/// <summary>
	/// The initial screen position that the finger was at when 
	/// latched
	/// </summary>
    public  Vector2 initialScreenPosition { get; private set; }
	
	/// <summary>
	/// The initial position that the finger was at when 
	/// latched
	/// </summary>
    public  Vector2 initialPosition { get; private set; }
	
	/// <summary>
	/// Initializes a new instance of the <see cref="TrackedFingerInformation"/> class.
	/// </summary>
	/// <param name='latchedScreenPos'>
	/// Latched screen position.
	/// </param>
	/// <param name='latchedPos'>
	/// Latched position.
	/// </param>
	/// <param name='maxPositionHistory'>
	/// Max position history.
	/// </param>
	public TrackedFingerInformation(Vector2 latchedScreenPos, Vector2 latchedPos, int maxPositionHistory)
	{
		positionHistory = new CircularBuffer<UnityEngine.Vector2>(maxPositionHistory);
		initialPosition = latchedPos;
		initialScreenPosition = latchedScreenPos;
	}
}

