using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class light_controller : MonoBehaviour {
	private Light mlight;
	// Use this for initialization
	void Start () {
		mlight = this.GetComponent<Light>();
		LeanTween.value(this.gameObject, Controll, 25.65f, 95.6f, 1f).setOnComplete(New_control_config)
			.setLoopPingPong().setRepeat(-1);
	}
	private Hashtable LTweenLight()
	{
		Hashtable config = new Hashtable ();
		//config.Add("ease", LeanTweenType.easeOutBounce);
		config.Add ("onComplete", "New_control_config");
		config.Add ("onCompleteTarget", this);  
		config.Add ("setUseEstimatedTime", false);
		config.Add ("setEase", LeanTweenType.easeOutCirc);
		config.Add ("setRepeat", -1);
		//config.Add ("setLoopPingPong", true);
		return config;
	}
	private void Controll(float i){
		mlight.cookieSize = i;
	}

	private void New_control_config(){

	}
	// Update is called once per frame
	void Update () {
	
	}
}
