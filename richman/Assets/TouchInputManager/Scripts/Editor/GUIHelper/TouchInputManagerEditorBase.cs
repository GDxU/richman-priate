using UnityEngine;
using UnityEditor;
using System.Collections;

namespace TouchInputManagerBackendEditor
{
    public class TouchInputManagerEditorBase : EditorWindow {

        //TODO clean this shit up, create a class that holds on styles/etc and pass that around rather than once for every editor
        protected Texture _touchInputManagerLogo;
        protected Texture _touchInputManagerLogoSmallFaded;
        private Texture2D _lightWithBorderBG, _lightNoBorderBG, _lightDottedBorderBG, _darkWithBorderBG, _darkNoBorderBG, _selectedBG, _selectedOldBG, _justBorderBG,_jusRightBorderBG;
        protected Texture _allLayoutsIcon, _layoutIcon;
        protected Texture _joystickIcon, _buttonIcon;
        protected Texture _settingsIcon, _moveIcon, _animationIcon, _errorIcon, _noErrorIcon;
        protected GUIStyle _styleLightWithBorderBG, _styleLightNoBorderBG, _styleLightDottedBorderBG, _styleDarkWithBorderBG, _styleDarkNoBorderBG, _styleSelectedBG, _styleOldSelectedBG, _styleSelectedBoldBG, _styleOldSelectedBoldBG, _styleJustBorderBG,_styleJustRightBorderBG;
	
        protected bool initialized = false;
        protected void OnEnable()
        {
            Init();
        }

        protected virtual void Init()
        { 
            minSize = new Vector2(500, 250);
            _touchInputManagerLogo = Resources.LoadAssetAtPath("Assets/TouchInputManager/Images/TouchInputManagerLogo.png", typeof(Texture)) as Texture;
            _touchInputManagerLogoSmallFaded = Resources.LoadAssetAtPath("Assets/TouchInputManager/Images/TouchInputManagerLogoSmallFaded.png", typeof(Texture)) as Texture;
            _lightWithBorderBG = Resources.LoadAssetAtPath("Assets/TouchInputManager/Images/lightWithBorder.png", typeof(Texture2D)) as Texture2D;
            _lightNoBorderBG = Resources.LoadAssetAtPath("Assets/TouchInputManager/Images/lightNoBorder.png", typeof(Texture2D)) as Texture2D;
            _lightDottedBorderBG = Resources.LoadAssetAtPath("Assets/TouchInputManager/Images/lightDottedBorder.png", typeof(Texture2D)) as Texture2D;
            _darkWithBorderBG = Resources.LoadAssetAtPath("Assets/TouchInputManager/Images/darkWithBorder.png", typeof(Texture2D)) as Texture2D;
            _darkNoBorderBG = Resources.LoadAssetAtPath("Assets/TouchInputManager/Images/darkNoBorder.png", typeof(Texture2D)) as Texture2D;
            _selectedBG = Resources.LoadAssetAtPath("Assets/TouchInputManager/Images/selectedNoBorder.png", typeof(Texture2D)) as Texture2D;
            _selectedOldBG = Resources.LoadAssetAtPath("Assets/TouchInputManager/Images/selectedOldNoBorder.png", typeof(Texture2D)) as Texture2D;
            _justBorderBG = Resources.LoadAssetAtPath("Assets/TouchInputManager/Images/Border.png", typeof(Texture2D)) as Texture2D;
            _jusRightBorderBG = Resources.LoadAssetAtPath("Assets/TouchInputManager/Images/BorderRight.png", typeof(Texture2D)) as Texture2D;
            _allLayoutsIcon = Resources.LoadAssetAtPath("Assets/TouchInputManager/Images/allLayoutsIcon.png", typeof(Texture)) as Texture;
            _layoutIcon = Resources.LoadAssetAtPath("Assets/TouchInputManager/Images/layoutIcon.png", typeof(Texture)) as Texture;
            _joystickIcon = Resources.LoadAssetAtPath("Assets/TouchInputManager/Images/joystickIcon.png", typeof(Texture)) as Texture;
            _buttonIcon = Resources.LoadAssetAtPath("Assets/TouchInputManager/Images/buttonIcon.png", typeof(Texture)) as Texture;
            _settingsIcon = Resources.LoadAssetAtPath("Assets/TouchInputManager/Images/settingIcon.png", typeof(Texture)) as Texture;
            _moveIcon = Resources.LoadAssetAtPath("Assets/TouchInputManager/Images/moveIcon.png", typeof(Texture)) as Texture;
            _animationIcon = Resources.LoadAssetAtPath("Assets/TouchInputManager/Images/animationIcon.png", typeof(Texture)) as Texture;
            _errorIcon = Resources.LoadAssetAtPath("Assets/TouchInputManager/Images/errorIcon.png", typeof(Texture)) as Texture;
            _noErrorIcon = Resources.LoadAssetAtPath("Assets/TouchInputManager/Images/noErrorIcon.png", typeof(Texture)) as Texture;
        }
	

        protected virtual void InitializeStyles()
        {
            if(!initialized)
                initialized = true;
            else 
                return;
		
            _styleLightWithBorderBG = new GUIStyle();
            _styleLightWithBorderBG.border = new RectOffset(0, 1, 0, 1);
            _styleLightWithBorderBG.normal.background = _lightWithBorderBG;

            _styleLightNoBorderBG = new GUIStyle();
            _styleLightNoBorderBG.normal.background = _lightNoBorderBG;

            _styleLightDottedBorderBG = new GUIStyle();
            _styleLightDottedBorderBG.border = new RectOffset(5, 11, 5, 11);
            _styleLightDottedBorderBG.normal.background = _lightDottedBorderBG;

            _styleDarkWithBorderBG = new GUIStyle();
            _styleDarkWithBorderBG.border = new RectOffset(0, 1, 0, 1);
            _styleDarkWithBorderBG.normal.background = _darkWithBorderBG;

            _styleDarkNoBorderBG = new GUIStyle();
            _styleDarkNoBorderBG.normal.background = _darkNoBorderBG; 

            _styleSelectedBG = new GUIStyle(EditorStyles.label);
            _styleSelectedBG.normal.background = _selectedBG;
            _styleSelectedBG.normal.textColor = Color.white;

            _styleOldSelectedBG = new GUIStyle(EditorStyles.label);
            _styleOldSelectedBG.normal.background = _selectedOldBG;
            _styleOldSelectedBG.normal.textColor = Color.white;

            _styleSelectedBoldBG = new GUIStyle(EditorStyles.boldLabel);
            _styleSelectedBoldBG.normal.background = _selectedBG;
            _styleSelectedBoldBG.normal.textColor = Color.white; 

            _styleOldSelectedBoldBG = new GUIStyle(EditorStyles.boldLabel);
            _styleOldSelectedBoldBG.normal.background = _lightNoBorderBG;
            _styleOldSelectedBoldBG.normal.textColor = Color.white;

            _styleJustBorderBG = new GUIStyle();
            _styleJustBorderBG.border = new RectOffset(0, 1, 0, 1);
            _styleJustBorderBG.normal.background = _justBorderBG;
		
            _styleJustRightBorderBG = new GUIStyle();
            _styleJustRightBorderBG.border = new RectOffset(0,1,0,0);
            _styleJustRightBorderBG.normal.background = _jusRightBorderBG;
        }
	
        protected void OnGUI()
        {
            if(initialized == false)
                InitializeStyles(); 
        }
	
        protected void LooseFocus()
        {
            GUI.SetNextControlName("nofucus");
            GUI.TextField(new Rect(-100, -100, 1, 1), "");
            GUI.FocusControl("nofucus");
        }
    }
}