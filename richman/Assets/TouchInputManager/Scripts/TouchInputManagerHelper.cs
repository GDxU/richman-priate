using UnityEngine;
using System.Collections;


#if (UNITY_IPHONE || UNITY_ANDROID) && !UNITY_EDITOR
using TouchEditor = UnityEngine.Touch;
#endif

namespace TouchInputManagerBackend
{
    public static class TouchInputManagerHelper
    {
        /// <summary>
        /// Convert a Vec2 to screen coordinates
        /// </summary>
        public static Vector2 ScreenSpace(Vector2 vec)
        {
            return new Vector2(vec.x / Screen.width, vec.y / Screen.height);
        }

        /// <summary>
        /// Convert a Vec2 to pixel coordinates
        /// </summary>
        public static Vector2 UndoScreenSpace(Vector2 vec)
        {
            return new Vector2(vec.x * Screen.width, vec.y * Screen.height);
        }


        /// <summary>
        /// Finger t's position
        /// </summary>
        public static Vector2 TouchPosition(int t)
        {
            return FindTouch(t).position;
        }
	
        /// <summary>
        /// Finger t's screen space position
        /// </summary>
        public static Vector2 TouchScreenSpacePosition(int t)
        {
            return ScreenSpace(FindTouch(t).position);
        }

        /// <summary>
        /// Tries to find and return a touch from Unity Input
        /// </summary>
        public static TouchEditor FindTouch(int fingerID)
        {
#if (!UNITY_IPHONE && !UNITY_ANDROID) || UNITY_EDITOR
            return new TouchEditor { position = Input.mousePosition };
#else
            foreach (Touch t in Input.touches)
	{
		if(t.fingerId == fingerID)
			return t;
	}
	return new Touch();
#endif
        }

    }
}
