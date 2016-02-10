using UnityEngine;
using UnityEngine.SocialPlatforms;

public class LumosSocialDemoCustomLogin : MonoBehaviour {
	
	static bool hasRegistered;
	static string hasRegisteredKey;
	
	// Useful if:
	// 1. You don't need cross platform support (the player will not know their login credentials)
	// 2. You don't want users to have to enter a username or password ever
	// 3. You are only using Achievements (otherwise you need a way to make displayable names on leaderboards)
	// 4. You don't plan to use friend lists (you could use a 3rd party login like Facebook to get around this)
	public static void AuthenticateAnonymously ()
	{		
		hasRegisteredKey = "Has_Registered_" + Lumos.playerID;
		hasRegistered = System.Convert.ToBoolean(PlayerPrefs.GetInt(hasRegisteredKey, 0));
		
		// You can use whatever you want for usernames (must be unique) and passwords.
		// If you want to use something other than the Lumos player ID (unique) then
		// be sure to save the login credentials in PlayerPrefs or somewhere else where 
		// you can load them.
		// You must authenticate every time the game runs
		Authenticate(Lumos.playerID, Lumos.playerID, !hasRegistered);
	}
	
	// Useful if:
	// 1. You want players to choose their username
	// 2. You want to use leaderboards and friend lists
	// 3. You don't need cross platform support (the player will not know their password)
	public static void AuthenticateWithUsername (string username, bool register)
	{
		// You can use whatever you want for passwords.
		// If you want to use something other than the Lumos player ID (unique) then
		// be sure to save the password in PlayerPrefs or somewhere else you can load it.
		// You must authenticate every time the game runs
		Authenticate(username, Lumos.playerID, register);
	}	

    // Useful if:
	// 1. You want cross platform support (players will choose their usernames and passwords)
	// 2. You want to use leaderboards and friend lists
	// 3. You don't need cross platform support (the player will not know their password)
    public static void Authenticate (string username, string password, bool register)
    {
		var user = new LumosUser(username, password);
		
		// You could modify these functions to accept more user details and set them here.
		// Example: modify this function to accept an email address and assign it to user.email 
		// before registering. If you set an email when registering a user, you can optionally use 
		// our welcome email feature (configurable on the Lumos Social dashboard).
		// You can also set arbitrary information using user.other. 
		// All user details can be updated after registration except for the username.
		
        // Register new user
        if (register) {
            Register(user);
        // Login with a registered user
        } else {
            Login(user);
        }
    }
	
	static void Register (LumosUser user)
	{
		LumosSocial.RegisterUser(user, success => {
            if (success) {
				PlayerPrefs.SetInt(hasRegisteredKey, 1);
                Debug.Log("Registered and authenticated, assigned as Social.localUser");
            } else {
                Debug.LogError("Unable to register user");
            }
			
			LumosSocialDemoCustomLoginGUI.RegistrationComplete(success);
        });
	}
	
	static void Login (LumosUser user)
	{
		user.Authenticate(success => {
            if (success) {
            	Debug.Log("Authenticated, assigned as Social.localUser");
        	} else {
            	Debug.LogError("Failed to authenticate");
        	}
			
			LumosSocialDemoCustomLoginGUI.LoginComplete(success);
        });
	}
}
