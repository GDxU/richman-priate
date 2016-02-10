using UnityEngine;
using System.Collections;

public interface ILumosSetup  {	
	string powerupID
	{
		get;
	}
	
	// Ensure this can be run multiple times and won't negatively affect 
	// projects which have already been setup with this powerup
	void Setup();
}
