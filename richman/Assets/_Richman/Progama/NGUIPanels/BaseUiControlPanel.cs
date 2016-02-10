using UnityEngine;
using System.Collections;
[RequireComponent(typeof(UILabel))]
[RequireComponent(typeof(TweenAlpha))]
public class BaseUiControlPanel : MonoBehaviour {
	public AudioClip button;
	public GameObject control_panel, waiting_panel, property_list;
	protected bool control_panel_play_forward;
	protected gameEngine interface_engine;
	protected UILabel nametag, money;
	protected TweenAlpha control_panel_alpha, waiting_panel_alpha;
	protected float alphaAnimationTime = 0.5f;
	protected float alphaAnimationTimeDelay = 0.89f;
	protected UIPlaySound[] soundComponent;
	protected DIR panel_transition;
	public enum DIR
	{
		FORWARD,
		BACKWARD
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	private void setSoundClipOnButton (float volumne, string tag, AudioClip clip)
	{
		foreach (UIPlaySound uisnd in soundComponent) {
			if (uisnd.gameObject.tag.Equals (tag)) {
				uisnd.audioClip = clip;
				uisnd.volume = volumne;
			}
		}
	}
	
	public void init ()
	{
		soundComponent = GetComponentsInChildren<UIPlaySound> ();
		setSoundClipOnButton (0.5f, "uibutton", button);
		interface_engine = gameEngine.Instance;
		UILabel[] lbl = control_panel.GetComponentsInChildren<UILabel> ();
		foreach (UILabel r in lbl) {
			if (r.gameObject.name.Equals ("NameTag")) {
				nametag = r;
			}
			if (r.gameObject.name.Equals ("money")) {
				money = r;
			}
			//Debug.Log ("found the name: " + r.gameObject.name);
		}
		waiting_panel.SetActive (true);
		control_panel.SetActive (true);
		control_panel_alpha = control_panel.GetComponent<TweenAlpha> ();
		waiting_panel_alpha = waiting_panel.GetComponent<TweenAlpha> ();
		waiting_panel.SetActive (false);
		control_panel.SetActive (false);
	}

}
