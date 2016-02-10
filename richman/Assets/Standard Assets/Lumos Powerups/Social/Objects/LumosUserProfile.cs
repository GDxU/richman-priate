// Copyright (c) 2013 Rebel Hippo Inc. All rights reserved.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

/// <summary>
/// Holds information about a player.
/// </summary>
public class LumosUserProfile : IUserProfile
{
	// The user's name.
	public string userName { get; set; }

	// A unique identifier for this user.
	public string userID { get; set; }

	/// <summary>
	/// A unique identifier for this user.
	/// </summary>
	public string id
	{
		get { return userID; }
	}

	// Indicates whether this user is a friend of the current player.
	public bool isFriend { get; set; }

	/// <summary>
	/// The user's state.
	/// Lumos currently doesn't support this.
	/// </summary>
	public UserState state { get; set; }

	// An avatar representing the user.
	public Texture2D image { get; set; }

	// Creates a new user blank profile.
	public LumosUserProfile () {}

	// Creates a new user profile.
	public LumosUserProfile (Dictionary<string, object> info)
	{
		this.userID = info["user_id"] as string;

		if (info.ContainsKey("name")) {
			this.userName = info["name"] as string;
		}

		// Load avatar from remote server.
		if (info.ContainsKey("image")) {
			var imageLocation = info["image"] as string;
			LumosRequest.LoadImage(imageLocation, image);
		}
	}
}
