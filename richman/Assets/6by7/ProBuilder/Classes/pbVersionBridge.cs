/**
 *	\brief Hax!  DLLs cannot interpret preprocessor directives, so this class acts as a "bridge"
 */
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

public class pbVersionBridge
{	
	/**
	 *	\brief Returns an integer representation of the major+minor version.  Will be a 3 digit number, so you can perform arithmetic like if(GetMajorVersion() > 410) UseNewUndo() else UseOldUndo();
	 */
	public static int GetMajorVersion()
	{
		#if UNITY_2_6
			return 260;
		#elif UNITY_2_6
			return 261;
		#elif UNITY_3_0
			return 300;
		#elif UNITY_3_0
			return 300;
		#elif UNITY_3_1
			return 310;
		#elif UNITY_3_2
			return 320;
		#elif UNITY_3_3
			return 330;
		#elif UNITY_3_4
			return 340;
		#elif UNITY_3_5
			return 350;
		#elif UNITY_4_0
			return 400;
		#elif UNITY_4_0
			return 401;
		#elif UNITY_4_1
			return 410;
		#elif UNITY_4_2
			return 420;
		#else
			return 0;
		#endif
	}

	public static bool IsPro()
	{
		bool isPro = false;
#if UNITY_EDITOR
		isPro = PlayerSettings.advancedLicense;
#endif
		return isPro;
	}

	public static bool StaticBatchingEnabled(GameObject go)
	{
#if UNITY_EDITOR
		return (GameObjectUtility.GetStaticEditorFlags(go) & StaticEditorFlags.BatchingStatic) == StaticEditorFlags.BatchingStatic;
#else
		return true;
#endif
	}

	public static void SetActive(GameObject go, bool isActive)
	{
#if UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5	
		go.active = isActive;
#else
		go.SetActive(isActive);
#endif
	}
}

public static class GameObjectExtensions
{
	public static void SetActive(this GameObject go, bool isActive)
	{
#if UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5	
		go.active = isActive;
#else
		go.SetActive(isActive);
#endif
	}
}