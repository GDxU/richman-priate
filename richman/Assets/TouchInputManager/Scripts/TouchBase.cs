using UnityEngine;
using System.Collections;

namespace TouchInputManagerBackend
{
    public abstract class TouchBase : MonoBehaviour
    {
        [HideInInspector]
        public TouchTracker tracker;	
	
        public void Initialize()
        {
            tracker.SetConfig(GetTrackerConfig());
            tracker.RegisterOnLatch(OnLatch);
            tracker.RegisterOnUnlatch(OnUnlatch);
        }
	
        /// <summary>
        /// Called when a finger T is latched to the tracker
        /// </summary>
        protected virtual void OnLatch(int t){}
	
        /// <summary>
        /// Called when a finger T is unlatched to the tracker
        /// </summary>
        protected virtual void OnUnlatch(int t){}
		
        /// <summary>
        /// Gets the tracker config. Return a 
        /// TouchTrackerConfig with the configuration
        /// your touch object needs
        /// </summary>
        protected abstract TouchTrackerConfig GetTrackerConfig();
	
    }
}