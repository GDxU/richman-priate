// Copyright (c) 2013 Rebel Hippo Inc. All rights reserved.

public interface ILumosPowerup
{
	/// <summary>
	/// A unique identifier for the powerup.
	/// </summary>
	string id { get; }

	/// <summary>
	/// The version of the powerup.
	/// </summary>
	string version { get; }

	/// <summary>
	/// The API's host domain.
	/// </summary>
	string baseURL { get; }
}
