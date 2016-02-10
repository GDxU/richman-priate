using UnityEngine;
using System.Collections;

public class panel_helper : BaseUiControlPanel
{
		public static panel_helper Instance;

		void Awake ()
		{
				Instance = this;
		}
		/**
         * start game from the loading of the level
         */
		public void OnInitStart ()
		{
				control_panel.SetActive (true);
				waiting_panel_alpha = TweenAlpha.Begin (waiting_panel, alphaAnimationTime * 2f, 1f);
				waiting_panel_alpha.PlayForward ();
		}
    
		// display the waiting span
		// animate the waiting panel
		public void close_user_control_panel ()
		{
				if (gameEngine.Instance.CurrentPlayer ().controlBy == RichChar.Brain.HUMAN) {
						control_panel.SetActive (false);

				}

				if (gameEngine.Instance.CurrentPlayer ().controlBy == RichChar.Brain.AI) {

				}

				waiting_panel.SetActive (true);
				waiting_panel_alpha = TweenAlpha.Begin (waiting_panel, alphaAnimationTime * 2f, 1f);
				waiting_panel_alpha.delay = alphaAnimationTimeDelay;
				waiting_panel_alpha.PlayForward ();
				panel_transition = DIR.FORWARD;
		}

		// close the current waiting span
		// animate the waiting panel
		public void closeWaiting ()
		{ 
				waiting_panel_alpha = TweenAlpha.Begin (waiting_panel, alphaAnimationTime, 0f);
				waiting_panel_alpha.delay = 0f;
				waiting_panel_alpha.PlayForward ();
		}
		//called from the componenet
		public  void OnFinishedControlPanel ()
		{
				//the panel is close and off
				if (panel_transition == DIR.FORWARD) {
						control_panel.SetActive (false);
						waiting_panel.SetActive (true);
						waiting_panel_alpha.PlayForward ();
                        
				}

				if (panel_transition == DIR.BACKWARD) {
                    
				}
		}
		//called from the componenet
//      public void OnFinishedWaitingPanel ()
//      {
//              control_panel.SetActive (true);
//              waiting_panel.SetActive (false);
//              control_panel_alpha.PlayReverse ();
//              play_dice = false;
//      }
               
		public void StartPanelWithPlayerControl (RichChar plr)
		{
				try {
						if (plr == null)
								throw new UnityException ("null pointer for RichChar plr");
						control_panel.SetActive (true);
						waiting_panel.SetActive (false);
						nametag.text = plr.playername;
						money.text = plr.moneyinhand.ToString ();
						panel_transition = DIR.BACKWARD;
						control_panel_alpha.PlayReverse ();
				} catch (UnityException e) {
						Debug.LogError (e.ToString ());
				}
		}

		public void startPanelAI ()
		{
				control_panel.SetActive (false);
				waiting_panel_alpha = TweenAlpha.Begin (waiting_panel, alphaAnimationTime * 2f, 1f);
				waiting_panel_alpha.PlayForward ();
		}
		//called from the UI
		public void OnPressDice ()
		{
				//trigger dice effect
				//done dice effect
				//current player make a move now
				gameEngine.Instance.GetComponent<diceCon> ().throw_dice ();
				close_user_control_panel ();
		}

		public void OnPressList ()
		{
				UIPropertyList prolistcomponent = gameObject.GetComponent<UIPropertyList> ();
				property_list.SetActive (true);
				//prolistcomponent.setPerson (gameEngine.Instance.CurrentPlayer ()).RenderPropertyList ();
				prolistcomponent.RenderPropertyList (gameEngine.Instance.CurrentPlayer ());
				control_panel.SetActive (false);
		}

		public void OnPressListBack ()
		{
				UIPropertyList prolistcomponent = gameObject.GetComponent<UIPropertyList> ();
				prolistcomponent.UIInActive (property_list,control_panel);
				//property_list.SetActive (false);
				//control_panel.SetActive (true);
		}

} 
