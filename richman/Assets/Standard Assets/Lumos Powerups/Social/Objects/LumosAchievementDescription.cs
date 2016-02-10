// Copyright (c) 2013 Rebel Hippo Inc. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;


// Information about an available achievement.
public class LumosAchievementDescription : IAchievementDescription
{
	// Unique identifier for the achievement.
	public string id { get; set; }

	// The name of this achievement.
	public string title { get; set; }

	// Icon that represents the achievement.
	public Texture2D image { get; set; }

	// Description of the achievement after it has been earned.
	public string achievedDescription { get; set; }

	// Description of the achievement before it has been earned.
	public string unachievedDescription { get; set; }

	// Whether this achievement is publicly visible.
	public bool hidden { get; set; }

	// The number of points the achievement is worth once completed.
	public int points { get; set; }

	// Creates an achievement description object.
	public LumosAchievementDescription (Dictionary<string, object> info)
	{
		this.id = info["achievement_id"] as string;
		this.title = info["name"] as string;
		this.achievedDescription = info["achieved_description"] as string;
		this.unachievedDescription = info["unachieved_description"] as string;
		this.points = Convert.ToInt32(info["points"] as string);

		var hiddenInt = Convert.ToInt32(info["hidden"] as string);
		this.hidden = Convert.ToBoolean(hiddenInt);

		// Load image from remote server.
		if (info.ContainsKey("icon")) {
			var imageLocation = info["icon"] as string;
			image = new Texture2D(512, 512);
			LumosRequest.LoadImage(imageLocation, image);
		}
	}
}
