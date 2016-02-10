using UnityEngine;
using UnityEngine.SocialPlatforms;
using System.Collections;
using System.Collections.Generic;


// This class demonstrates a common functions developers will likely use with Lumos Social.
// The majority of examples can be found by browsing the LumosSocial GUI scripts.
// In this demonstration all classes are converted to Lumos based classes instead of the 
// classes you would typically see in the Social documentation. This is only to demonstate 
// to you some of the extra functionality that exists when you do cast an object as its 
// Lumos counterpart.
public class LumosSocialDemoBasic : MonoBehaviour 
{
	// Assign this upon registering or logging in
	// It can be used to easily access extra Lumos Social features
	public static LumosUser user;
	
	// Assign this upon loading achievement descriptions
	// It can be used as a convenience for displaying achievment information
	public static List<LumosAchievementDescription> achievementDescriptions;
	
	// Add to this upon loading a leaderboard
	// It can be used as a convenience for displaying leaderboards
	public static List<LumosLeaderboard> leaderboards;
	
	
	// Example for awarding an achievement
	public static void AwardAchievement (string achievementID)
	{
		Social.ReportProgress(achievementID, 100, (success) =>
		{
			if (success) {
				Debug.Log("Awarded achievement: " + achievementID);
			} else {
				Debug.LogWarning("There was a problem awarding the achievement: " + achievementID);
			}
		});
	}
	
	// Loads achievement descriptions which can be used for displaying
	public static void LoadAchievementDescriptions ()
	{
		Social.LoadAchievementDescriptions((results) =>
		{
			if (results == null) {
				Debug.LogWarning("No achievements were loaded.");
				return;
			}
			
			achievementDescriptions = new List<LumosAchievementDescription>();
			
			foreach (var achievementDescription in results) {
				achievementDescriptions.Add(achievementDescription as LumosAchievementDescription);
			}
		});
	}
	
	// Example for recording a score to a leaderboard.
	// Unfortunately with Unity's API you can't record floats (time-based scores will be inconvenient)
	// However, you could potentially convert it to a float when you display scores
	public static void RecordScore (string leaderboardID, long score)
	{
		Social.ReportScore(score, leaderboardID, (success) =>
		{
			if (success) {
				Debug.Log("Recorded score to: " + leaderboardID);
			} else {
				Debug.LogWarning("There was a problem recording the score to: " + leaderboardID);
			}
		});
	}
	
	// Loads leaderboard names and other info (not scores)
	public static void LoadLeaderboardDescriptions ()
	{
		LumosSocial.LoadLeaderboardDescriptions((success) => {			
			if (!success) {
				Debug.LogWarning("Unable to load leaderboard descriptions.");
				return;
			}
			
			leaderboards = new List<LumosLeaderboard>();
			
			// Loaded leaderboards are stored in LumosSocial.leaderboards for your convenience
			// We'll store our own copy of it in this class though for demonstration purposes.
			foreach (var leaderboard in LumosSocial.leaderboards) {
				leaderboards.Add(leaderboard as LumosLeaderboard);
			}
		});
	}
	
	// Loads the scores for a leaderboard
	// It also loads friend scores automatically
	public static void LoadScores (LumosLeaderboard leaderboard, int limit, int offset)
	{
		leaderboard.LoadScores(limit, offset, (success) => {
			if (success) {
				Debug.Log("Loaded scores for: " + leaderboard.id);
				// access scores using leaderboard.scores
			} else {
				Debug.LogWarning("Unable to load scores for " + leaderboard.id);
			}
		});
	}
	
	/* --- Alternative methods to load scores --

	// Gets scores scores above and below the player user
	leaderboard.LoadScoresAroundUser(limit, (success) => {
		
	});
	
	// Loads all of your friends' scores for this leaderboard
	leaderboard.LoadFriendScores((succes) => {
		if (success) {
			Debug.Log("Loaded friend scores for: " + leaderboard.id);
			// access friend scores using leaderboard.friendScores
		} else {
			Debug.LogWarning("Unable to load friend scores for " + leaderboard.id);
		}
	});
	
	*/

	public static void SendFriendRequest(string friendID)
	{
		var user = Social.localUser as LumosUser;
		user.SendFriendRequest(friendID, (success) => {
			if (success) {
				Debug.Log("Sent friend request to: " + friendID);
			} else {
				Debug.LogWarning("Unable to send friend request to " + friendID);
			}
		});
	}

	public static void AcceptFriendRequest(string friendID)
	{
		var user = Social.localUser as LumosUser;
		user.AcceptFriendRequest(friendID, (success) => {
			if (success) {
				Debug.Log("Added friend: " + friendID);
			} else {
				Debug.LogWarning("Unable to add friend " + friendID);
			}
		});
	}

	public static void DeclineFriendRequest(string friendID)
	{
		var user = Social.localUser as LumosUser;
		user.DeclineFriendRequest(friendID, (success) => {
			if (success) {
				Debug.Log("Declined friend request from: " + friendID);
			} else {
				Debug.LogWarning("Unable to decline friend request from " + friendID);
			}
		});
	}

	public static void LoadFriendRequest()
	{
		var user = Social.localUser as LumosUser;
		user.LoadFriendRequests((success) => {
			if (success) {
				// access friend requests from user.friendRequests
				Debug.Log("Loaded friend requests");
			} else {
				Debug.LogWarning("Unable to load friend requests");
			}
		});
	}
}
