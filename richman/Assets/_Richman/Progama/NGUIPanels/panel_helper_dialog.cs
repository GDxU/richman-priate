using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Panel_helper_dialog. 
/// Dependency NGUI
/// </summary>
public class panel_helper_dialog : BaseUiPanel
{

		public static panel_helper_dialog Instance;
	
		void Awake ()
		{
				Instance = this;
		}
		// Use this for initialization

		public void init ()
		{
				setallgo (true);
				soundComponent = GetComponentsInChildren<UIPlaySound> ();
				questionTextView = DIALOG.GetComponentInChildren<UILabel> ();
				setSoundClipOnButton (0.5f, "uibutton", button);
			
				setallgo (false);
				//Debug.Log ("protected init in here");
		}

		//CALLBACK
		protected void onAniDoneForward ()
		{
				CANCEL.SetActive (true);
				ACTION.SetActive (true);
				CLOSEBTN.SetActive (true);
				//firstChild (DIALOG).SetActive (true);
				tweenScale.RemoveOnFinished (new EventDelegate (onAniDoneForward));
				switch (talk) {
				case talk_type.BANK_SERVICE:
						questionTextView.text = BDescr.start_question;
						BDescr.setDialog (ACTION, CANCEL, CLOSEBTN);
						BDescr.apple_settings_buttons ();
						break;
				case talk_type.MONEY:
						Debug.Log (DDescr.start_question);
						DDescr.setDialog (ACTION, CANCEL, CLOSEBTN);
						//DDescr.set_land_btn ();
						DDescr.apple_settings_buttons ();
						questionTextView.text = DDescr.start_question;
						
						//DDescr.apple_settings_buttons ();
						break;
				}
				CLOSEBTN.SetActive (false);
				
		}

		//CALLBACK
		protected void onAniDoneBackward ()
		{
				Debug.Log ("on done backward:");
				tweenScale.RemoveOnFinished (new EventDelegate (onAniDoneBackward));
				TEXTURE.SetActive (false);
				DIALOG.SetActive (false);

				switch (talk) {
				case talk_type.BANK_SERVICE:
						//this.mComplete ();
						break;
				case talk_type.MONEY:
						//this.mComplete ();
						break;
				}
				if (mComplete != null) {
						Debug.Log ("onAniDoneBackward by mComplete");
						mComplete ();
						mComplete = null;
				}
				if (mSuccessEvent != null) {
						Debug.Log ("onAniDoneBackward by mSuccessEvent");
						mSuccessEvent ();
						mSuccessEvent = null;
				}
				if (DDescr != null)
						DDescr.type = talk_type.NA;
				if (BDescr != null)
						BDescr.type = talk_type.NA;
		}
		//button_2
		public void ActionRightTriggered ()
		{
				switch (talk) {
				case talk_type.BANK_SERVICE:
						switch (BDescr.talk_status) {
						case 0:
								BDescr.talk_status = 2;
								panel_invalidate ();
								Debug.Log ("go to 2");
								break;
						case 1: //confirm service withdraw
								BDescr.talk_status = 0;
								panel_invalidate ();
								Debug.Log ("go back from 1");
								break;
						case 2://confirm service deposit
								BDescr.talk_status = 0;
								panel_invalidate ();
								Debug.Log ("go back from 2");
								break;
						case 3: //confirm exit
								playbackward ();
								BG.SetActive (false);
								break;
						}
						break;
				case talk_type.MONEY:
						playbackward ();
						break;
				}
		}

		public void ActionMidTriggered ()
		{
				switch (talk) {
				case talk_type.BANK_SERVICE:
						playbackward ();
						break;
				case talk_type.MONEY:
						playbackward ();
						break;
				}
		}

		//button_1
		public void ActionLeftTriggered ()
		{
				switch (talk) {
				case talk_type.MONEY:
						switch (DDescr.talk_status) {
						case 0: //press the buy button
								DDescr.talk_status = DDescr.consider (focus_player.moneyinhand) ? 1 : 2;
								questionTextView.text = DDescr.getResponse (focus_player.moneyinhand);
								panel_invalidate ();
								break;
						case 1:
								playbackward ();
								DDescr.type = talk_type.NA;
								break;
						case 2:				
								playbackward ();
								DDescr.type = talk_type.NA;
								break;
						}
						
						break;

				case talk_type.BANK_SERVICE:
						switch (BDescr.talk_status) {
						case 0:
								BDescr.talk_status = 1;
								panel_invalidate ();
								Debug.Log ("go to 1");
								break;
						case 1: //confirm service withdraw
								BDescr.confirm_transaction ();
								gameEngine.Instance.getSD ().playFXbankTransaction ();
								BDescr.talk_status = 3;
								panel_invalidate ();
								Debug.Log ("go to 3");
								break;
						case 2://confirm service deposit
								BDescr.confirm_transaction ();
								gameEngine.Instance.getSD ().playFXbankTransaction ();
								BDescr.talk_status = 3;
								panel_invalidate ();
								Debug.Log ("go to 3");
								break;

						case 3://confirm go back
								BDescr.talk_status = 0;
								panel_invalidate ();
								Debug.Log ("go back from 3");
								break;
						}

						break;
				case talk_type.DECISION:

						break;
				case talk_type.SUCCESS:

						break;
				case talk_type.FAILURE:

						break;
				

				default:
						break;
				}
		}
	
		protected void panel_invalidate ()
		{
				switch (talk) {
				case talk_type.BANK_SERVICE:
						switch (BDescr.talk_status) {
						case 0:
				//enter
								this.questionTextView.text = BDescr.start_question;
								BDescr.button_label ("WITHDRAWAL", "DEPOSIT", "EXIT");
								BDescr.apple_settings_buttons ();
								BDescr.noMoneyControl ();
								two_buttons (true);
								break;
						case 1:	
								BDescr.set_default_set ("Confirm");
								StartCoroutine (textOrder (BDescr.getReport (), BDescr.withdraw, BDescr.transition_done));
								break;
						case 2:	
								BDescr.set_default_set ("Confirm");
								StartCoroutine (textOrder (BDescr.getReport (), BDescr.deposit, BDescr.transition_done));
								break;
						case 3:	//confirm from 1 or 2
								//button_label
								BDescr.button_label ("go back", "exit", "OK");
								StartCoroutine (textOrder (ST.bank_confirm, ST.bank_finish, BDescr.apple_settings_buttons));
								break;
						}
						break;
				case talk_type.MONEY:
						switch (DDescr.talk_status) {
						case 1:
								two_buttons (false);
								DDescr.talk_status = 3;
								DDescr.transaction_go (gameEngine.Instance, show_close_btn);
								break;
						case 2:
								if (mSuccessEvent != null) {
										Debug.Log ("trade success by mSuccessEvent");
										mSuccessEvent ();
										mSuccessEvent = null;
								} else {
										//focus_player.trade_success (show_close_btn);
										show_close_btn ();
								}
								break;
						}
			
						break;
				}
		}

		public void setBankServiceFlow (BankDialogDescriptor descriptor, Action exit)
		{
				activate_forward ();
				setActiveGirl (1);

				//UISprite b = DIALOG.GetComponent<UISprite> ();
				descriptor.withdraw = ST.bank_withdrawal;
				descriptor.deposit = ST.bank_deposite;
				descriptor.negative_respose = ST.bank_neg;
				descriptor.positive_response = ST.bank_pos;
				descriptor.report_statement = ST.bank_report;
				descriptor.start_question = ST.bank_enter;
				descriptor.confirm = ST.bank_confirm;
				descriptor.finish = ST.bank_finish;
				descriptor.button_label ("WITHDRAWAL", "DEPOSIT", "EXIT");
				descriptor.setPanelLookFeel (DIALOG, "frame12", dialog_colors [1]);
				descriptor.setupSlider (SLIDER);
				BG.SetActive (true);
				talk = descriptor.type;
				this.mSuccessEvent = exit;
				this.BDescr = descriptor;
				
				playforward ();
		}

		public void setMoneyLogicFlow (DialogDescriptor descriptor, RichChar about_him, Action exit)
		{
				activate_forward ();
				this.setActiveGirl (2);
				focus_player = about_him;

				mComplete = exit;

				descriptor.setPanelLookFeel (DIALOG, "Highlight2", dialog_colors [3]);

				talk = descriptor.type;
				this.DDescr = descriptor;
				playforward ();
		}

//		public void SimpleDFlow (DialogDescriptor descriptor, RichChar about_him, Action transact_event, Action exit)
//		{
//				activate_forward ();
//				this.setActiveGirl (2);
//		
//				focus_player = about_him;
//				mSuccessEvent = transact_event;
//				mComplete = exit;
//
//				descriptor.setPanelLookFeel (DIALOG, "Highlight2", dialog_colors [3]);
//				talk = descriptor.type;
//				this.DDescr = descriptor;
//				playforward ();
//		}
}

public class BankDialogDescriptor:BaseDialogDescriptor
{
		public string start_question, report_statement, loan_statement, negative_respose, positive_response, withdraw, deposit, confirm, finish;
		protected RichChar client;
		protected GameObject slider;

		public BankDialogDescriptor (RichChar person)
		{
				client = person;
				type = panel_helper_dialog.talk_type.BANK_SERVICE;
		}

		public void setupSlider (GameObject s)
		{
				slider = s;
		}

		public string getReport ()
		{

				return string.Format (report_statement, client.bankmoney, client.moneyinhand, client.bankmoney > 0 ? client.moneyinhand / client.bankmoney : 0);
		}

		public void transition_done ()
		{
				apple_settings_buttons ();
				slider.SetActive (true);
				BankSliderController c = slider.GetComponent<BankSliderController> ();
				//	Debug.Log ("get slider");
				c.slider_init ();
				if (talk_status == 1) {
						c.startslider (0f, client.moneyinhand, client.bankmoney, BankSliderController.PURPOSE.WITHDRAW);
				}
				if (talk_status == 2) {
						c.startslider (0f, client.moneyinhand, client.bankmoney, BankSliderController.PURPOSE.DEPOSIT);
				}
		}

		public void noMoneyControl ()
		{
				slider.SetActive (false);
		}

		public void confirm_transaction ()
		{
				BankSliderController c = slider.GetComponent<BankSliderController> ();
				int[] g = c.getValues ();

				client.moneyinhand = g [0];
				client.bankmoney = g [1];
				slider.SetActive (false);
			
		}
}

public class DialogDescriptor:BaseDialogDescriptor
{

		public string start_question, negative_respose, positive_response;
		public int condition_money = 0;
		public lvType mlvType;
		public enum  lvType
		{
				BUYLAND,
				UPGRADE,
				PAYRENT
		}
		public DialogDescriptor ()
		{
			
		}

		public DialogDescriptor (lvType mlv, int cost)
		{
				if (mlv == lvType.BUYLAND) {
						start_question = string.Format (ST.buyq, cost);
						negative_respose = ST.sorry;
						positive_response = ST.positive;
						type = BaseUiPanel.talk_type.MONEY;
						set_land_btn ();
				}
				if (mlv == lvType.UPGRADE) {
						start_question = string.Format (ST.upgrade, cost);
						negative_respose = ST.sorry;
						positive_response = ST.positive_upgrade;
						type = BaseUiPanel.talk_type.MONEY;
						set_upgrade_btn ();
				}
				if (mlv == lvType.PAYRENT) {
						start_question = string.Format (ST.rent_statement, cost);
						negative_respose = ST.sorry_rent;
						positive_response = ST.positive_rent;
						type = BaseUiPanel.talk_type.MONEY;
						set_rent_btn ();
				}
				mlvType = mlv;
				type = panel_helper_dialog.talk_type.MONEY;
				condition_money = cost;
		}

		public DialogDescriptor (string q1, string qPos, string qNeg, panel_helper_dialog.talk_type talk, int cost)
		{
				condition_money = cost;
				start_question = q1;
				negative_respose = qNeg;
				positive_response = qPos;
				type = talk;
		}

		public lvType getTransaction ()
		{
				return mlvType;
		}

		public bool consider (int m)
		{
				return m > condition_money;
		}

		public string getResponse (int m)
		{
				return condition_money < m ? positive_response : negative_respose;
		}

		public void transaction_go (gameEngine ge, Action after)
		{
				if (mlvType == lvType.BUYLAND) {
						Debug.Log ("trade success by BUYLAND");
						gameEngine.Instance.trade_success (condition_money, after);
				}
				if (mlvType == lvType.UPGRADE) {
						Debug.Log ("trade success by UPGRADE");
						gameEngine.Instance.trade_success (condition_money, after);
				}
				if (mlvType == lvType.PAYRENT) {
						Debug.Log ("trade success by PAYRENT");
						gameEngine.Instance.collection_money_for_landlord (condition_money, after);
				}
		}

		private void set_land_btn ()
		{
				label_0 = "BUY";
				label_1 = "CANCEL";
		}

		private void set_upgrade_btn ()
		{
				label_0 = "UPGRADE";
				label_1 = "CANCEL";
		}
	
		private void set_rent_btn ()
		{
				label_0 = "PAY";
				label_1 = "CANCEL";
		}
}
