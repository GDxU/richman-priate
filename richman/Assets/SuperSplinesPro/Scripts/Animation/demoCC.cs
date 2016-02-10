using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class demoCC : MonoBehaviour
{
		public FantasticCamera fCam;
		private List<GameObject> animatorlist = new List<GameObject> ();
		private int current_item = 0;
		public static demoCC Instance;

		void awake ()
		{
				Instance = this;
		}

		void Start ()
		{
				animatorlist = new List<GameObject> ();
				initss ();
		}

		public void initss ()
		{
				Debug.Log ("initss");
				foreach (GameObject c in GameObject.FindGameObjectsWithTag("Player")) {
						//RichmanAnimator h = snapob.GetComponent<RichmanAnimator> ();
						animatorlist.Add (c);
						Debug.Log ("snapob");
				}
		}

		private void focus ()
		{
				GameObject snapob = animatorlist [current_item];
				RichmanAnimator h = snapob.GetComponent<RichmanAnimator> ();
				CharacterControllerLogic cl = snapob.GetComponentInChildren<CharacterControllerLogic> ();
				fCam.triggerFocusMain (cl, cl.gameObject.transform);			
		}

		public void CamControl_nex ()
		{
				current_item++;
				int t = animatorlist.Count;
	
				current_item = current_item % t;
				focus ();
		}

		public void CamControl_pre ()
		{
				current_item--;
				int t = animatorlist.Count;

				current_item = current_item < 0 ? t - 1 : current_item;
				focus ();
		}

		public void CamControl_view1 ()
		{
				fCam.isTargeted = true;
		}

		public void CamControl_view2 ()
		{
				fCam.isTargeted = false;
		}

		public void CamControl_view3 ()
		{
		fCam.ToFocusFirst();
		}
}
