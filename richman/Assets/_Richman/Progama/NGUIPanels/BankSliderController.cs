using UnityEngine;
using System.Collections;

public class BankSliderController : MonoBehaviour
{
		protected UISlider sl;
		public GameObject labelUpper, labelLower;
		protected UILabel m1, m2;
		protected int i1=0, i2=0;
		protected PURPOSE pur = PURPOSE.NA;
		// Use this for initialization
		public	enum PURPOSE
		{
				WITHDRAW,
				DEPOSIT,
				NA
		}
		// Update is called once per frame
		void Update ()
		{
				if (pur == PURPOSE.DEPOSIT) {
						float pro = sl.value * i1;
						int displayi1 = i1 - Mathf.FloorToInt (pro);
						int displayi2 = i2 + Mathf.FloorToInt (pro);
						updateFields (displayi1, displayi2);
				}
				if (pur == PURPOSE.WITHDRAW) {
						float pro = sl.value * i2;
						int displayi1 = i1 + Mathf.FloorToInt (pro);
						int displayi2 = i2 - Mathf.FloorToInt (pro);
						updateFields (displayi1, displayi2);
				}
		}

		protected void updateFields (int d1, int d2)
		{
				m1.text = d1.ToString ();
				m2.text = d2.ToString ();
		}

		public void slider_init ()
		{
				sl = GetComponent<UISlider> ();
				m1 = labelUpper.GetComponent<UILabel> ();
				m2 = labelLower.GetComponent<UILabel> ();
		}

		protected void setUpperLabelVal (int x)
		{
				m1.text = x.ToString ();
				i1 = x;
		}

		protected void setLowerLabelVal (int x)
		{
				m2.text = x.ToString ();
				i2 = x;
		}

		public void startslider (float start_val, int i1, int i2, PURPOSE p)
		{
				setUpperLabelVal (i1);
				setLowerLabelVal (i2);
				if (start_val > 0f)
						sl.value = start_val;
				pur = p;
		}

		public int[] getValues ()
		{
				int f1=0, f2=0;
				if (pur == PURPOSE.DEPOSIT) {
						float pro = sl.value * i1;
						f1 = i1 - Mathf.FloorToInt (pro);
						f2 = i2 + Mathf.FloorToInt (pro);
				}
				if (pur == PURPOSE.WITHDRAW) {
						float pro = sl.value * i2;
						f1 = i1 + Mathf.FloorToInt (pro);
						f2 = i2 - Mathf.FloorToInt (pro);
				}
				pur = PURPOSE.NA;
				return new int[]{f1,f2};
		}
}
