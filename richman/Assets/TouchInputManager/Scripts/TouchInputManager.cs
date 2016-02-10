using TouchInputManagerBackend;
using UnityEngine;

public class TouchInputManager
{
    #region singleton nonsense (r_Instance)

    private static TouchInputManagerBehaviour _Instance;

    private static TouchInputManagerBehaviour r_Instance
    {
        get
        {
            if (_Instance == null)
            {
                _Instance = Object.FindObjectOfType(typeof(TouchInputManagerBehaviour)) as TouchInputManagerBehaviour;
                if (_Instance == null)
                {
                    _Instance = (((Object.Instantiate(Resources.Load("TouchInputManager/TouchInputManager"))) ?? (new GameObject()).AddComponent<TouchInputManagerBehaviour>().gameObject) as GameObject).GetComponent<TouchInputManagerBehaviour>();
                    _Instance.Load();
                }

                RenderLayoutAll(false);
                PassInputToLayoutAll(false);
            }
            return _Instance;
        }
    }
    #endregion

    /// <summary>
    /// Sets whether or not we should render all layouts on screen.
    /// </summary>
    /// <param name="render">Render/Dont render</param>
    static public void RenderLayoutAll(bool render)
    {
        r_Instance.RenderLayoutAll(render);

    }

    /// <summary>
    /// Sets whether or not all layouts should receive and react to touch input.
    /// </summary>
    /// <param name="passInput">Pass input/Dont pass input</param>
    static public void PassInputToLayoutAll(bool passInput)
    {
        r_Instance.PassInputToLayoutAll(passInput);
    }

    /// <summary>
    /// Sets whether or not layout identified by ID should be rendered.
    /// </summary>
    /// <param name="ID">ID of the layout (specified in the editor)</param>
    /// <param name="render">Render/Dont render</param>
    static public void RenderLayout(LayoutID ID, bool render)
    {
        r_Instance.RenderLayout(ID, render);
    }

    /// <summary>
    /// Sets whether or not layout identified by ID should receive and react to touch input.
    /// </summary>
    /// <param name="ID">ID of the layout (specified in the editor)</param>
    /// <param name="passInput">Render/Dont render</param>
    static public void PassInputToLayout(LayoutID ID, bool passInput)
    {
        r_Instance.PassInputToLayout(ID, passInput);
    }

    /// <summary>
    /// Changes the current animation preset an input device identified by inputID to a preset identified by presetID.
    /// </summary>
    /// <param name="presetID">ID of the preset (specified in the editor)</param>
    /// <param name="inputID">ID of the input device (specified in the editor)</param>
    /// <param name="layoutID">ID of the layout (specified in the editor)</param>
    static public void ChangeAnimationPreset(string presetID, InputID inputID, LayoutID layoutID)
    {
        r_Instance.ChangeAnimationPreset(presetID, inputID, layoutID);
    }

    /// <summary>
    /// Returns the current animation preset ID of an input device identified by inputID in a layout identified by layoutID.
    /// </summary>
    /// <param name="inputID">ID of the input device (specified in the editor)</param>
    /// <param name="layoutID">ID of the layout (specified in the editor)</param>
	static public string GetCurrentAnimationPreset(InputID inputID, LayoutID layoutID)
    {
        return r_Instance.GetCurrentAnimationPreset(inputID, layoutID);
    }

    /// <summary>
    /// Returns the current unnormalized value of the virtual joystick identified by joystickID in the layout identified by layoutID.
    /// </summary>
    /// <param name="joystickID">ID of the joystick (specified in the editor)</param>
    /// <param name="layoutID">ID of the layout in which it resides (specified in the editor)</param>
    static public Vector2 GetJoystick(InputID joystickID, LayoutID layoutID)
    {
        return r_Instance.GetJoystick(joystickID, layoutID, false);
    }

    /// <summary>
    /// Returns the current value of the virtual joystick identified by joystickID in the layout identified by layoutID, normalizing the returned value if asked to.
    /// </summary>
    /// <param name="joystickID">ID of the joystick (specified in the editor)</param>
    /// <param name="layoutID">ID of the layout in which it resides (specified in the editor)</param>
    /// <param name="normalized">Whether or not we should normalize the value</param>
    static public Vector2 GetJoystick(InputID joystickID, LayoutID layoutID, bool normalized)
    {
        return r_Instance.GetJoystick(joystickID, layoutID, normalized);

    }

    /// <summary>
    /// Returns true for as long as the user pressed down the virtual button identified by buttonID in the layout identified by layoutID.
    /// </summary>
    /// <param name="buttonID">ID of the button (specified in the editor)</param>
    /// <param name="layoutID">ID of the layout in which it resides (specified in the editor)</param>
    static public bool GetButton(InputID buttonID, LayoutID layoutID)
    {
        return r_Instance.GetButton(buttonID, layoutID);

    }

    /// <summary>
    /// Returns true during the frame the user pressed down the virtual button identified by buttonID in the layout identified by layoutID.
    /// </summary>
    /// <param name="buttonID">ID of the button (specified in the editor)</param>
    /// <param name="layoutID">ID of the layout in which it resides (specified in the editor)</param>
    static public bool GetButtonDown(InputID buttonID, LayoutID layoutID)
    {
        return r_Instance.GetButtonDown(buttonID, layoutID);

    }

    /// <summary>
    /// Returns true during the frame the user releases the virtual button identified by buttonID in the layout identified by layoutID.
    /// </summary>
    /// <param name="buttonID">ID of the button (specified in the editor)</param>
    /// <param name="layoutID">ID of the layout in which it resides (specified in the editor)</param>
    static public bool GetButtonUp(InputID buttonID, LayoutID layoutID)
    {
        return r_Instance.GetButtonUp(buttonID, layoutID);
    }
}
/*
public class MonoBehaviour
{
    /// <summary>
    /// OnTouchEnter is called once when the user touches the Collider on the screen through a Zone. 
    /// </summary>
    /// <remarks> 
    /// You need to define these functions inside your MonoBehaviours, just as you do with MonoBehaviour.OnCollisionEnter(Collision), etc.
    /// The touch must fall within the region specified for the Zone in its Layout.
    /// Make sure the Layout that the Zone resides in is set to receive input.
    /// Make sure that the zone is set to interact with the same layers as the colliders you are interested in.
    /// </remarks> 
    /// <param name="touchPosition">The current screen position of the touch.</param>
    public void OnTouchEnter(Vector2 touchPosition)
    {
    }

    /// <summary>
    /// OnTouch is called every frame where the user is touching the Collider on the screen through a Zone. 
    /// </summary>
    /// <param name="touchPosition">The current screen position of the touch.</param>
    /// <seealso cref="MonoBehaviour.OnTouchEnter(Vector2)">
    /// Make sure you read the remarks on this function, as they apply here as well. </seealso> 
    public void OnTouch(Vector2 touchPosition)
    {
    }

    /// <summary>
    /// OnTouchExit is called once when the user released the Collider they are touching. 
    /// </summary>
    /// <param name="touchPosition">The current screen position of the touch.</param>
    /// <seealso cref="MonoBehaviour.OnTouchEnter(Vector2)">
    /// Make sure you read the remarks on this function, as they apply here as well. </seealso> 
    public void OnTouchExit(Vector2 touchPosition)
    {
    }
}*/