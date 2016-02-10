//============================================================================================================
// Weili Zhi Copy right reserved.
//============================================================================================================
/// Action Base
//============================================================================================================
// Created on 18/7/2012 9:06:52 AM by Weili Zhi
//============================================================================================================
using UnityEngine;
using System;
using System.Collections.Generic;

public class AttackShipData
{
	private LinkedList<Transform> mTargets = new LinkedList<Transform>();
	
	public void FillTargetPositions(GameObject ship)
	{
		Transform targets = ship.transform.Find("AttackPoints");
		if( targets!= null ) {
			Transform tfTarget = targets.Find("AttackPointH");
			if( tfTarget != null ) {
				mTargets.AddFirst(tfTarget);
			}
			else return;
			
			Transform tfNext = targets.Find("AttackPointR");
			if( tfNext != null ) {
				mTargets.AddLast(tfNext);
			}
			else return;
			
			GameShip script = ship.GetComponent<GameShip>();
			if( script != null ) {
				GameObject nextShip = script.NextShip;
				if( nextShip != null )
				{
					FillTargetPositions(nextShip);
				}
				else {
					tfNext = targets.Find("AttackPointT");
					if( tfNext != null ) {
						mTargets.AddLast(tfNext);
					}
					else return;
				}
			}
			else {
				DebugExt.Log("Ship " + ship.name + " doesn't have GameShip script");
				return;
			}
			tfNext = targets.Find("AttackPointL");
			if( tfNext != null ) {
				mTargets.AddLast(tfNext);
			}
			return;
		}
	}
	public LinkedList<Transform> GetAllTargetPositions()
	{
		return mTargets;
	}
	
	public Transform GetPreviousTarget(Transform currentTarget)
	{
		LinkedListNode<Transform> node = mTargets.Find(currentTarget);
		if( node != null ) {
			if( node.Previous == null ) {
				return mTargets.Last.Value;
			}
			else {
				return node.Previous.Value;
			}
		}
		else return null;
	}
	
	public Transform GetNextTarget(Transform currentTarget)
	{
		LinkedListNode<Transform> node = mTargets.Find(currentTarget);
		if( node != null ) {
			if( node.Next == null ) {
				return mTargets.First.Value;
			}
			else {
				return node.Next.Value;
			}
		}
		else return null;
	}
}

