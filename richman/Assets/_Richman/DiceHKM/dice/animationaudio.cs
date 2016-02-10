using UnityEngine;
using System.Collections;

public class animationaudio : MonoBehaviour
{
	protected diceCon control;
	public int expecting_value = 1, playing = 0 ;
	public Hashtable h = new Hashtable();
	// Use this for initialization
	void Start()
	{
		control = gameEngine.Instance.GetComponent<diceCon>();
		h.Add(6, new Vector3(0f, 22f, -179.9f));
		h.Add(3, Vector3.up);
		h.Add(2, Vector3.right);
		h.Add(1, -Vector3.right);
		h.Add(5, Vector3.forward);
		h.Add(4, -Vector3.forward);
	}
	// Update is called once per frame
	void Update()
	{
  
	}

	public void onDropDice()
	{
		//play the sound of the bounce
	}

	public void throwDice()
	{
		if (playing == 0)
		{
			playing = 1;
			expecting_value = Random.Range(1, 7);
			animation.Play();
		}
	}
	/**
	 * //find the vector pointing from our position to the target
        _direction = (Target.position - transform.position).normalized;
 
        //create the rotation we need to be in to look at the target
        _lookRotation = Quaternion.LookRotation(_direction);
        
        //rotate us over time according to speed until we are in the required rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, Time.deltaTime * RotationSpeed);

        */
	public void onDone()
	{
		if (playing == 1)
		{
			playing = 0;
			StartCoroutine(triggerafter());
		}
	}

	private IEnumerator triggerafter()
	{
		yield return new WaitForEndOfFrame();
		Vector3 result = (Vector3)h [expecting_value];
		Vector3 f = result;
		if (expecting_value == 6)
			f = result + Vector3.up * Random.Range(1f, 190f);
		else
			f = result * 90f + Vector3.up * Random.Range(1f, 190f);
		
		//Debug.Log (f.ToString ());
		gameObject.transform.rotation = Quaternion.Euler(f);
		control.addDiceResult(transform.parent.gameObject.GetHashCode(), expecting_value);
	}
}
