

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof(PathScanner))]
[RequireComponent (typeof(FollowTrackingCamera))]
[RequireComponent (typeof(diceCon))]
public class Engine01 : BaseGameEngine
{
	public static Engine01 Instance;
	// Use this for initialization
	void Start ()
	{
		Instance = this;
		StartCoroutine (setup ());
	}
	
	void Awake ()
	{
		//Instance = this;
	}
	// Update is called once per frame
	private IEnumerator setup ()
	{
		loading = true;
		yield return new WaitForEndOfFrame ();
		component_snd = GetComponent<USound> ();
		component_path_scanner = GetComponent<PathScanner> ();
		component_camera_tracker = GetComponent<FollowTrackingCamera> ();
		//component_dice = GetComponent<diceCon> ();
		component_panel_UI = panel_helper.Instance;
		//UIpanel.GetComponent<panel_helper> ();
		component_uidialog = panel_helper_dialog.Instance;
		//UIDialogPanel.GetComponent<panel_helper_dialog> ();
		yield return new WaitForEndOfFrame ();
		component_panel_UI.init ();
		Debug.Log ("there is component_uidialog.init");
		component_uidialog.init ();
		yield return new WaitForEndOfFrame ();
		if (currentplayer == null && mplayers.Count == 0) {
			Debug.LogError ("there is no setup for the current player");
		}
		turn_play = 0;
		yield return new WaitForEndOfFrame ();
		component_path_scanner.scan ();
		yield return new WaitForEndOfFrame ();
		if (mplayers.Count > 0) {
			foreach (RichChar n in mplayers) {
				component_path_scanner.rez_init_player (n); 
				yield return new WaitForEndOfFrame ();
			}
			yield return new WaitForEndOfFrame ();
			currentplayer = mplayers [turn_play];
			component_camera_tracker.autoRotate (false).ToFocus (currentplayer.character_stage.transform);
			//Debug.Log ("there is a component rendered");
			//Debug.Log (currentplayer.character_stage.transform);
			//component_camera_tracker.target = currentplayer.character_stage.transform;
		}
		yield return new WaitForEndOfFrame ();
		if (component_snd != null)
			component_snd.loadSoundInit ();
		//component_panel_UI.OnInitStart ();
		
		
		loading = false;
	}
	
	public void OnStart ()
	{
		
	}
	//the game is started from the main UI panel
	//called from the UI
	public void OnInitialStartGame ()
	{
		
		if (currentplayer.controlBy == RichChar.Brain.AI) {
			Debug.Log ("component_panel_UI. AI");
			component_panel_UI.startPanelAI ();
			cb_nexturn_trigger ();
		} else {
			Debug.Log ("component_panel_UI. HUMAN");
			component_panel_UI.StartPanelWithPlayerControl (currentplayer);
		}
	}
	
	public void Update ()
	{
		if (!loading) {
			for (int i = 0; i<mplayers.Count; i++) {
				RichChar playerinscene = mplayers [i];
				playerinscene.runStatusPosition ();
			}
		}
	}
	
	public FollowTrackingCamera getCam ()
	{
		return component_camera_tracker;
	}
	
	public PathScanner getScanner ()
	{
		return component_path_scanner;
	}
	
	public RichChar CurrentPlayer ()
	{
		//  Debug.Log(currentplayer);
		return currentplayer;
	}
	
	public panel_helper_dialog getUIdialog ()
	{
		return component_uidialog;
	}
	
	public panel_helper getPlayPanel ()
	{
		return component_panel_UI;
	}
	
	public USound getSD ()
	{
		return component_snd;
	}
	
	public void eventStepTrigger (int instanceID, int stopID, string message)
	{
		if (message.Equals ("complete_location_from_forward")) {
			
		} else if (message.Equals ("arrive_at_location_jit")) {
			//we work on this later
		}
	}
}
