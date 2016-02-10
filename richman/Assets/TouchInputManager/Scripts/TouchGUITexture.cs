using UnityEngine;
using System.Collections;

namespace TouchInputManagerBackend
{
    [AddComponentMenu("")]
    public class TouchGUITexture : MonoBehaviour
    {

        //TODO layer literally does nothing
        public int layer = 0;

        public Texture texture;
        public Color color;
	
        public void Draw ()
        {
            if (enabled == false || texture == null)
                return;

            GUI.color = color;
            Vector2 basePosNonScreenSpace = new Vector2(transform.position.x * Screen.width, (1 - transform.position.y) * Screen.height);
            float baseWidthPixels = transform.localScale.x * Screen.width;
            float baseHeightPixels = baseWidthPixels / ((float)texture.width / (float)texture.height);

            Rect baseDrawArea = new Rect(basePosNonScreenSpace.x - (baseWidthPixels / 2), basePosNonScreenSpace.y - (baseHeightPixels / 2), baseWidthPixels, baseHeightPixels);
            GUI.DrawTexture(baseDrawArea, texture);

            GUI.color = Color.white;
        }
    }
}