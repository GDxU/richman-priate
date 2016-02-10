using UnityEngine;
using UnityEngine.SocialPlatforms;
using System.Collections;

// This script helps demonstrate the various login and registration 
// methods supported by Lumos
public class LumosSocialDemoCustomLoginGUI : MonoBehaviour {
	
	float margin;
	enum UI { Login, Achievements, Leaderboards, UserProfile, Settings };
	enum LoginOption { None, Annonymous, UsernameOnly, Full, Failed, Success };
	static UI currentUI;
	static LoginOption login;
	
	string username = "demo";
	string password = "";
	static string message = "";
	static bool waitingForResponse;
	
	GUIContent loginBtn = new GUIContent("Login", "Log in with an existing account using these credentials.");
	GUIContent registerBtn = new GUIContent("Register", "Register a new account with these credentials.");
	const string loginSuccessMsg = "Successfully logged in!";
	const string loginFailedMsg = "There was an issue logging you in. Please use the correct username and password.";
	const string registrationSuccessMsg = "Registration complete!";
	const string registrationFailedMsg = "Registration failed. The chosen username may already be taken or your there was an issue with your credentials.";
	
	
	void Awake ()
	{
		margin = Screen.width / 2.5f;
		Lumos.debug = true;
		
		// Set Lumos as the active Unity Social Platform
		Social.Active = new LumosSocial();
	}
	
	void OnGUI ()
	{
		DisplayLogoutButton();
		
		switch (currentUI) {
			case UI.Login:
				LoginScreen();
				break;
			case UI.Achievements:
				// See LumosAchievementsGUI.cs for examples
				break;
			case UI.Leaderboards:
				// See LumosLeaderboardsGUI.cs for examples
				break;
			case UI.UserProfile:
				// See LumosProfileGUI.cs for examples
				break;
			case UI.Settings:
				// See LumosSettingsGUI.cs for examples
				break;
			default:
				// Display nothing
				break;
		}
	}
	
	void LoginScreen ()
	{
		// Messages displayed after login or registration completes (fail or success)
		GUILayout.Label(message, GUILayout.Width(margin));
		
		// Select login/registration style
		if (login == LoginOption.None) {
			if (GUILayout.Button("Annonymous Login (auto register)", GUILayout.Width(margin))) {
				login = LoginOption.Annonymous;
				waitingForResponse = true;
				LumosSocialDemoCustomLogin.AuthenticateAnonymously();
			}
		
			if (GUILayout.Button("Username Only Login (register w/ username)", GUILayout.Width(margin))) {
				login = LoginOption.UsernameOnly;
			}
		
			if (GUILayout.Button("Full Login (register w/ username & password)", GUILayout.Width(margin))) {
				login = LoginOption.Full;
			}
		// Successfully registered or logged in
		} else if (login == LoginOption.Success) {
			if (GUILayout.Button("Continue", GUILayout.Width(margin))) {
				// Move on to a new UI.
				// This ends our demo.
				currentUI = UI.UserProfile;
			}
		// Failed to register/login
		} else if (login == LoginOption.Failed) {
			if (GUILayout.Button("Try Again", GUILayout.Width(margin))) {
				login = LoginOption.None;
				message = "";
			}
		// Waiting for a response from the server
		} else if (waitingForResponse) {
			GUILayout.Label("Your request is being processed...", GUILayout.Width(margin));
		// Entering login/registration credentials
		} else {
			// Username field
			DisplayUsername();
			
			// Password field
			if (login == LoginOption.Full) {
				DisplayPassword();	
			}
			
			// Login and Registration Buttons
			GUILayout.BeginHorizontal();
				var loggingIn = GUILayout.Button(loginBtn, GUILayout.Width(margin));
				var registering = GUILayout.Button(registerBtn, GUILayout.Width(margin));
				
				if (loggingIn || registering) {
					// Login/Register with player defined username
					if (login == LoginOption.UsernameOnly) {
						LumosSocialDemoCustomLogin.AuthenticateWithUsername(username, registering);
					// Login/Register with player defined username and password
					} else {
						LumosSocialDemoCustomLogin.Authenticate(username, password, registering);
					}
				
					waitingForResponse = true;
				}
			GUILayout.EndHorizontal();
		}
	}
	
	void DisplayLogoutButton ()
	{
		var user = Social.localUser;
		
		if (user != null && user.authenticated && currentUI != UI.Login) {
			if (GUILayout.Button("Log out")) {
				Social.Active = new LumosSocial();
				currentUI = UI.Login;
				login = LoginOption.None;
				message = "";
			}
		}
	}
	
	void DisplayUsername ()
	{
		GUILayout.BeginHorizontal();
			GUILayout.Label("Username", GUILayout.Width(margin));
			username = GUILayout.TextField(username, GUILayout.Width(margin));
		GUILayout.EndHorizontal();
	}
	
	void DisplayPassword ()
	{
		GUILayout.BeginHorizontal();
			GUILayout.Label("Password", GUILayout.Width(margin));
			char bullet = '\u2022';
			password = GUILayout.PasswordField(password, bullet, GUILayout.Width(margin));
		GUILayout.EndHorizontal();
	}
	
	public static void LoginComplete (bool success)
	{
		HandleResponse(success, loginSuccessMsg, loginFailedMsg);
	}
	
	public static void RegistrationComplete (bool success)
	{
		HandleResponse(success, registrationSuccessMsg, registrationFailedMsg);
	}
	
	static void HandleResponse (bool success, string successMsg, string failMsg)
	{
		message = (success) ? successMsg : failMsg;
		login = (success) ? LoginOption.Success : LoginOption.Failed;
		waitingForResponse = false;
	}
}
