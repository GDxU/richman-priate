using UnityEngine;
using System.Collections;

namespace TouchInputManagerBackend
{
    [AddComponentMenu("")]
    public class TouchButton : TouchGUIBase
    {
        private bool isDownLast = false;
        private bool isUpLast = false;
	
        public bool GetButton()
        {
            return tracker.AnyLatchedFingers;
        }

        public bool GetButtonDown()
        {
            return isDownLast;
        }

        public bool GetButtonUp()
        {
            return isUpLast;
        }
	
        public void ResetPressed()
        {
            isDownLast = false;
            isUpLast = false;
        }

        protected override void OnLatch(int t)
        {
            isDownLast = true;
        }

        protected override void OnUnlatch(int t)
        {
            isUpLast = true;
        }
	
        protected override TouchTrackerConfig GetTrackerConfig ()
        {
            return new TouchTrackerConfig{ maxPositionHistory = 1, maxSimultaneousPoints = 1 };
        }
    }
}