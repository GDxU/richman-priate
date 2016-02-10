using UnityEngine;
using System.Collections;

namespace TouchInputManagerBackend
{
    [AddComponentMenu("")]
    public class TouchJoystick : TouchGUIBase
    {
        public bool disableX, disableY;

        protected override void PositionTop()
        {
            if (tracker.AnyLatchedFingers)
            {
				if(topPart.texture != null)
				{
					Vector2 textureSize = basePart.texture == null ? new Vector2(topPart.texture.width,topPart.texture.height) : new Vector2(basePart.texture.width,basePart.texture.height);

	                Vector2 touchPos =  tracker.FirstCurrentScreenPosition;
	                Vector2 firstInitScreenPos = tracker.FirstInitialScreenPosition;
	                float scaleX = Mathf.Clamp(Mathf.Abs(touchPos.x - firstInitScreenPos.x), 0, basePart.transform.localScale.x / 2);
					float scaleY = Mathf.Clamp(Mathf.Abs(touchPos.y - firstInitScreenPos.y), 0, basePart.transform.localScale.y / 2 / ((float)textureSize.x / (float)textureSize.y) / ((float)Screen.height / (float)Screen.width));
	                topPart.transform.position = new Vector3(firstInitScreenPos.x + (((GetInputNormalized().x * scaleX))), firstInitScreenPos.y + (((GetInputNormalized().y * scaleY))),topPart.transform.position.z);
				}
			}
            else
            {
                topPart.transform.position = new Vector3(basePart.transform.position.x, basePart.transform.position.y,topPart.transform.position.z);
            }
        }
	
        public Vector2 GetInputNormalized()
        {
            if(tracker.AnyLatchedFingers)
            {
                Vector2 input = (tracker.FirstCurrentPosition - tracker.FirstInitialPosition);
                input = new Vector2(disableX ? 0 : input.x, disableY ? 0 : input.y);
                return input.normalized;
            }
            else
                return Vector2.zero;
        }
	
        public Vector2 GetInput()
        {
            if (tracker.AnyLatchedFingers)
            {
                Vector2 normalizedSize = GetInputNormalized();
                Vector2 touchPos = tracker.FirstCurrentPosition;
                float scaleX = Mathf.Clamp(Mathf.Abs(touchPos.x - tracker.FirstInitialPosition.x), 0, _baseScreenSize.x) / _baseScreenSize.x;
                float scaleY = Mathf.Clamp(Mathf.Abs(touchPos.y - tracker.FirstInitialPosition.y), 0, _baseScreenSize.y) / _baseScreenSize.y;
                return new Vector2(normalizedSize.x * scaleX, normalizedSize.y * scaleY);
            }
            else
                return Vector2.zero;
        }
	
        protected override TouchTrackerConfig GetTrackerConfig ()
        {
            return new TouchTrackerConfig{ maxPositionHistory = 1, maxSimultaneousPoints = 1 };
        }
    }
}