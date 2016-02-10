// Copyright (c) 2013 Rebel Hippo Inc. All rights reserved.

/// <summary>
/// Convenience functions for calling analytics functions.
/// </summary>
public partial class LumosAnalytics
{
	// Default category:

	public static void RecordEvent (string eventID)
	{
		LumosEvents.Record(null, eventID, null, true);
	}

	public static void RecordEvent (string eventID, float val)
	{
		LumosEvents.Record(null, eventID, val, true);
	}

	public static void RecordEvent (string eventID, bool repeatable)
	{
		LumosEvents.Record(null, eventID, null, repeatable);
	}

	public static void RecordEvent (string eventID, float val, bool repeatable)
	{
		LumosEvents.Record(null, eventID, val, repeatable);
	}

	// Custom category:

	public static void RecordEvent (string category, string eventID)
	{
		LumosEvents.Record(category, eventID, null, true);
	}

	public static void RecordEvent (string category, string eventID, float val)
	{
		LumosEvents.Record(category, eventID, val, true);
	}

	public static void RecordEvent (string category, string eventID, bool repeatable)
	{
		LumosEvents.Record(category, eventID, null, repeatable);
	}

	public static void RecordEvent (string category, string eventID, float val, bool repeatable)
	{
		LumosEvents.Record(category, eventID, val, repeatable);
	}
}
