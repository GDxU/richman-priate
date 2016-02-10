using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract  class BaseGameEngine : MonoBehaviour
{

	public List<RichChar> mplayers = new List<RichChar>();
	//public GameObject UIpanel, UIDialogPanel;
	public float movementSpeed = 1.1f, estimate_build_time = 2.1f;
	protected RichChar currentplayer;
	protected int turn_play;
	public bool stepDebug=false;
	protected PathScanner component_path_scanner;
	protected panel_helper component_panel_UI;
	protected panel_helper_dialog component_uidialog;
	protected FollowTrackingCamera component_camera_tracker;
	//protected diceCon component_dice;
	protected USound component_snd;
	public LayerMask groundonlymask;
	protected bool loading = true;
	public RichChar accessPlayer(int id)
	{
		return mplayers.Find(x => x.owner_id == id);
	}
	//upgrade or buy land
	public void trade_success(int transaction_amount, System.Action cbafterbuild)
	{
		Property landing_property = currentplayer.landing;
		landing_property.owned_by_id = currentplayer.owner_id;
		currentplayer.moneyinhand -= transaction_amount;
		if (currentplayer.stepdata.from_unclaim_to_buy_action())
		{
			Debug.Log("trade action - change land owner");
			component_snd.playFXbuyland();
			landing_property.applyUserStyle(currentplayer);
			cbafterbuild();
		} else {
			Debug.Log("trade action - making new building");
			component_snd.playFXBuilding();
			rezBuild(landing_property, landing_property.shiftNextLevel (), Random.Range (estimate_build_time - 0.5f, estimate_build_time), cbafterbuild);
			}
		//next_turn ();
	}
	protected virtual void rezBuild(Property prop, Property.locType t, float estTime, System.Action continue_action){

	}

	public void trade_success(int transact_amount)
	{
		Property landing_property = currentplayer.landing;
		landing_property.owned_by_id = currentplayer.owner_id;
		currentplayer.moneyinhand -= transact_amount;
		if (currentplayer.stepdata.from_unclaim_to_buy_action())
		{
			component_snd.playFXbuyland();
			landing_property.applyUserStyle(currentplayer);
			next_turn();
		} else
		{
			component_snd.playFXBuilding();
			rezBuild(landing_property, landing_property.shiftNextLevel (), Random.Range (estimate_build_time - 0.5f, estimate_build_time), next_turn);
			//component_path_scanner.RezBuilding(landing_property, landing_property.shiftNextLevel(), Random.Range(estimate_build_time - 0.5f, estimate_build_time), next_turn);
		}
		//next_turn ();
	}

	//pay the rent
	public void collection_money_for_landlord(int transact_amount, System.Action after)
	{
		Property landing_property = currentplayer.landing;
		if (currentplayer.moneyinhand < transact_amount)
		{
			currentplayer.bankmoney -= transact_amount;
		} else
		{
			currentplayer.moneyinhand -= transact_amount;
		}
		component_snd.playFXMoney();
		RichChar landlord = accessPlayer(landing_property.owned_by_id);
		landlord.moneyinhand += transact_amount;
		after();
	}

	public void ai_bank()
	{
		if (currentplayer.moneyinhand < 1000)
		{
			if (currentplayer.bankmoney > 0)
			{
				float f = currentplayer.bankmoney * 0.5f;
				currentplayer.moneyinhand += Mathf.FloorToInt(f);
				currentplayer.bankmoney -= Mathf.FloorToInt(f);
				Debug.Log("Withdraw from the bank with " + Mathf.FloorToInt(f) + " into his pocket...");
				component_snd.playFXbankTransaction();
			} else
			{
				Debug.Log("RichChar name is " + currentplayer.playername + " and it is controlled by...");
			}
		}
	}
	private IEnumerator carry_out_nexturn()
	{
		yield return new WaitForSeconds(2.0f);
		turn_play++;
		turn_play = turn_play % mplayers.Count;
		currentplayer = mplayers [turn_play];
		//component_camera_tracker.startNextTurnPlayer(currentplayer.character_stage.transform);
		NewCameraFocusObject(currentplayer);
		yield return new WaitForSeconds(2.0f);
		cb_nexturn_trigger();
		Debug.Log("RichChar name is " + currentplayer.playername + " and it is controlled by...");
	}
	protected virtual void NewCameraFocusObject(RichChar T){

	}
	public virtual void cb_nexturn_trigger()
	{
		if (currentplayer.controlBy == PlayerData.Brain.AI)
		{
			
			Debug.Log("AI control");
			currentplayer.setPlayerStatus(RichChar.status.PENDING);
			currentplayer.move(PlayerData.direction.FORWARD);
		} else if (currentplayer.controlBy == PlayerData.Brain.HUMAN)
		{
			Debug.Log("HUMAN control");
			currentplayer.setPlayerStatus(RichChar.status.PENDING);
			component_panel_UI.StartPanelWithPlayerControl(currentplayer);
		}
	}
	/**the current RichChar ends the turn **/
	public void next_turn()
	{
		StartCoroutine(carry_out_nexturn());
	}
}