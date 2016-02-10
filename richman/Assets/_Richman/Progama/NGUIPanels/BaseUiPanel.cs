using UnityEngine;
using System.Collections;
using System;

public class BaseUiPanel : MonoBehaviour
{

		public GameObject TEXTURE, ACTION, CANCEL, DIALOG, CLOSEBTN, BG, SLIDER;
		protected float  scaleAnimationTime = 1.0f;
		public float transitionDialogWaitingTime = 0.7f;
		public AnimationCurve curve1, curve2;
		protected UIPlaySound[] soundComponent;
		public AudioClip button;
		public Color32[] dialog_colors;
		protected UILabel questionTextView;
		protected RichChar focus_player;
		protected DialogDescriptor DDescr;
		protected BankDialogDescriptor BDescr;
		//private static panel_helper_dialog Instance;
		public Action mSuccessEvent;
		public Action<Action> mTransaction;
		public object onCompleteParam;
		public  Action mComplete;
		protected TweenScale tweenScale;
		protected TweenAlpha tweenaph;
		protected  talk_type talk;
		protected gameEngine ge;
		public enum DIR
		{
				FORWARD,
				BACKWARD
		}
	
		public enum talk_type
		{
				MONEY, //WITH MONEY
				DECISION, //NO MONEY
				SUCCESS,
				FAILURE,
				BANK_SERVICE,
				NA
		}
		
		protected void setallgo (bool b)
		{
				TEXTURE.SetActive (b);
				ACTION.SetActive (b);
				CANCEL.SetActive (b);
				DIALOG.SetActive (b);
				CLOSEBTN.SetActive (b);
				SLIDER.SetActive (b);
				BG.SetActive (b);
		}

		protected void setSoundClipOnButton (float volumne, string tag, AudioClip clip)
		{
				foreach (UIPlaySound uisnd in soundComponent) {
						if (uisnd.gameObject.tag.Equals (tag)) {
								uisnd.audioClip = clip;
								uisnd.volume = volumne;
						}
				}
		}
		/// <summary>
		/// Playbackward this instance.
		/// close the panel
		/// </summary>
		protected void playbackward ()
		{
				tweenaph = TweenAlpha.Begin (TEXTURE, scaleAnimationTime * 0.8f, 0f);
				tweenaph.from = 1.0f;
				tweenaph.SetStartToCurrentValue ();
				tweenaph.to = 0.0f;
				tweenaph.animationCurve = curve1;
				DIALOG.SetActive (true);
				tweenScale = TweenScale.Begin (DIALOG, scaleAnimationTime * 1.1f, new Vector3 (1f, 0f, 1f));
				EventDelegate.Add (tweenScale.onFinished, onAniDoneBackward);
				tweenaph.SetStartToCurrentValue ();
				questionTextView.text = "";
				all_buttons (false);
		
		}
	
		protected void activate_forward ()
		{
				DIALOG.SetActive (true);
				TEXTURE.SetActive (true);
		}
	
		protected void deactivate_buttons ()
		{
				CANCEL.SetActive (false);
				ACTION.SetActive (false);
		}

		protected void all_buttons (bool b)
		{
				CANCEL.SetActive (b);
				ACTION.SetActive (b);
				CLOSEBTN.SetActive (b);
		}

		protected void two_buttons (bool b)
		{
				CANCEL.SetActive (b);
				ACTION.SetActive (b);
		}

		protected void show_close_btn ()
		{
				CLOSEBTN.SetActive (true);
		}
	
		protected void hide_close_btn ()
		{
				CLOSEBTN.SetActive (false);
		}
		
		/// <summary>
		/// Playforward this instance.
		/// open the panel and display the dialog in here
		/// </summary>
		protected void playforward ()
		{
				tweenaph = TweenAlpha.Begin (TEXTURE, scaleAnimationTime * 2f, 1f);
				tweenaph.from = 0f;
				tweenaph.SetStartToCurrentValue ();
				tweenaph.to = 1.0f;
				tweenaph.animationCurve = curve1;

				tweenScale = TweenScale.Begin (DIALOG, scaleAnimationTime, new Vector3 (1f, 1f, 1f));
				tweenScale.from = new Vector3 (1f, 0f, 1f);
				tweenScale.to = new Vector3 (1f, 1f, 1f);
				EventDelegate.Add (tweenScale.onFinished, onAniDoneForward);
				
		}

		protected GameObject firstChild (GameObject g)
		{
				Transform [] t = g.GetComponentsInChildren<Transform> ();
				return	t [0].gameObject;
		}
		/// <summary>
		/// Ons the ani done forward.
		/// </summary>
		protected void onAniDoneForward ()
		{

		}
		/// <summary>
		/// Ons the ani done backward.
		/// </summary>
		protected void onAniDoneBackward ()
		{

		}
		/// <summary>
		/// Init this instance.
		/// </summary>
		protected void init ()
		{
				//Debug.Log ("base init in here");
		}
		/// <summary>
		/// Sets the active girl to be one of them.
		/// </summary>
		/// <param name="i">The index.</param>
		protected void setActiveGirl (int i)
		{
				string child = "girl" + i;
				if (!TEXTURE.activeSelf)
						Debug.Log ("texture is not active");
				foreach (Transform girl in TEXTURE.transform) {
						//Debug.Log ("girl:" + girl.gameObject.name);
						if (girl.gameObject.name.Equals (child)) {
								girl.gameObject.SetActive (true);
								//Debug.Log ("girl is active");
						} else {
								girl.gameObject.SetActive (false);
						}
				}
		}

		protected IEnumerator textOrder (string t1, string t2, Action cbTextOrder)
		{
				this.two_buttons (false);
				this.questionTextView.text = t1;
				yield return new WaitForSeconds (transitionDialogWaitingTime);
				this.questionTextView.text = t2;
				yield return new WaitForSeconds (transitionDialogWaitingTime);
				this.two_buttons (true);
				if (cbTextOrder != null)
						cbTextOrder ();
		}

}

public class BaseDialogDescriptor
{
		protected GameObject action_bn, cancel_bn, close_bn, btn_3;
		protected string label_0, label_1, label_2, label_3;
		public panel_helper_dialog.talk_type type;
		//the talk_status is to record the conversation status
		public int talk_status = 0 ;

		public BaseDialogDescriptor ()
		{
				
		}

		public void setPanelLookFeel (GameObject panel, string spritename, Color32 color)
		{
				UISprite sp = getSprite (panel);
				sp.spriteName = spritename;
				sp.color = color;
		}

		public void setDialog (GameObject ACTION, GameObject CANCEL, GameObject close)
		{
				action_bn = ACTION;
				cancel_bn = CANCEL;
				close_bn = close;
		}
	
		public void button_label (string l1, string l2, string l3)
		{
				label_0 = l1;
				label_1 = l2;
				label_2 = l3;
		}

		public void button_label (string l1, string l2)
		{
				label_0 = l1;
				label_1 = l2;
		}

		public void set_payment_btn ()
		{
				label_0 = "PAY";
				label_1 = "CANCEL";
		}

		public void set_default_set ()
		{
				label_0 = "OK";
				label_1 = "CANCEL";
		}

		public void set_default_set (string first)
		{
				label_0 = first;
				label_1 = "CANCEL";
		}

		private UISprite getSprite (GameObject go)
		{
				return go.GetComponent<UISprite> ();
		}

		private UILabel getLabelField (GameObject go)
		{
				return go.GetComponentInChildren<UILabel> ();
		}
		//assume the go are all active
		public void apple_settings_buttons ()
		{
				if (label_0 != null && action_bn.activeSelf) {
						getLabelField (action_bn).text = label_0;
				}
				if (label_1 != null && cancel_bn.activeSelf) {
						getLabelField (cancel_bn).text = label_1;
				}
				if (label_2 != null && close_bn.activeSelf) {
						getLabelField (close_bn).text = label_2;
				}
		}
}