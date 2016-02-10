using UnityEngine;
using System.Collections;

namespace TouchInputManagerBackend
{
    [AddComponentMenu("")]
    public class TouchZone : TouchBase
    {
        public LayerMask layersToTouch = -1;
    	
        private GameObject pressedOn;

        protected override void OnLatch(int t)
        {
            Ray ray = Camera.main.ScreenPointToRay(tracker.FirstCurrentPosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
                pressedOn = hit.collider.gameObject;

            if (pressedOn)
				pressedOn.SendMessage ("OnTouchEnter", tracker.FirstCurrentPosition, SendMessageOptions.DontRequireReceiver);
			else
				tracker.UnlatchFinger (t);
        }

        protected void Update()
        {
            if(pressedOn)
                pressedOn.SendMessage("OnTouch", tracker.FirstCurrentPosition, SendMessageOptions.DontRequireReceiver);
        }

        protected override void OnUnlatch(int t)
        {
            if (pressedOn)
                pressedOn.SendMessage("OnTouchExit", tracker.FirstCurrentPosition, SendMessageOptions.DontRequireReceiver);

            pressedOn = null;

        }
	
        protected override TouchTrackerConfig GetTrackerConfig ()
        {
            return new TouchTrackerConfig{ maxPositionHistory = 1, maxSimultaneousPoints = 1 };
        }

    }
}