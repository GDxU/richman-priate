using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//[RequireComponent (typeof(AudioSource))]
//[RequireComponent (typeof(AudioListener))]
public class USound : MonoBehaviour
{
	public float masterVolumeBGM;
	public bool SoundOn = true;
	private static USound instance = null;
	public static USound Instance
	{
		get { return instance; }
	}
	private string basic = "Assets/Resources/sound/", bgm = "Assets/Resources/bgm/";
	private List<int> asource;
	//private static AudioManager instancemgm = null;
	private AudioSource source;

//		void Awake ()
//		{
//				if (instance != null && instance != this) {
//						Destroy (this.gameObject);
//						return;
//				} else {
//						instancemgm = this;
//				}
//		
//				//DontDestroyOnLoad( this.gameObject );
//		}


	//http://answers.unity3d.com/questions/11314/audio-or-music-to-continue-playing-between-scene-c.html
//	void Awake() {
//		if (instance != null && instance != this) {
//			Destroy(this.gameObject);
//			return;
//		} else {
//			instance = this;
//		}
//		DontDestroyOnLoad(this.gameObject);
//	}
	// any other methods you need
	void Awake()
	{
		instance = this;
		DontDestroyOnLoad(this.gameObject);
	}

	private AudioClip money, build1, buyland, build2, build3, moneycount;

	public void loadSoundInit()
	{
		StartCoroutine(setup());
	}

	private IEnumerator setup()
	{
//				money = getSnd ("checkching.mp3");
//				build1 = getSnd ("construct4.mp3");
//				build2 = getSnd ("construct3.mp3");
//				build3 = getSnd ("construct2.mp3");
//				moneycount = getSnd ("moneycount.mp3");
//				buyland = getSnd ("buyland.mp3");

//				money = getSndRes ("checkching");
//				build1 = getSndRes ("construct4");
//				build2 = getSndRes ("construct3");
//				build3 = getSndRes ("construct2");
//				moneycount = getSndRes ("moneycount");
//				buyland = getSndRes ("buyland");
		gameObject.AddComponent<AudioListener>();
		source = gameObject.AddComponent<AudioSource>();
		yield return new WaitForEndOfFrame();
	}

	private void playsound(AudioClip ac)
	{
		Debug.Log("playsound");
		//source.clip = ac;
		source.PlayOneShot(ac);
		//	int id = source.GetInstanceID;
		//	asource.Add (id);
	}

	private AudioClip getSndRes(string str)
	{
		return Resources.Load("sound/" + str, typeof(AudioClip)) as AudioClip;
	}

	private AudioClip getSnd(string str)
	{
		return Resources.LoadAssetAtPath(basic + str, typeof(AudioClip)) as AudioClip;
	}

	public void playFXBuilding()
	{
		int k = Random.Range(1, 3);
		if (k == 1)
		{
			//	playsound (build1);
		}
		if (k == 2)
		{
			//	playsound (build2);
		}
		if (k == 3)
		{
			//	playsound (build3);
		}
	}

	public void playFXbankTransaction()
	{
		//	playsound (moneycount);
	}

	public void playFXbuyland()
	{
	
		//	playsound (buyland);
	}

	public void playFXMoney()
	{
		//	playsound (money);
	}
}
