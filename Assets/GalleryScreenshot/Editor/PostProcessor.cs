using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
//using UnityEditor.iOS.Xcode;
using System;
using System.IO;

//=============================================================================
// This PostProcessor does all annoying Xcode steps automatically for us
//=============================================================================

public class PostProcessor 
{
    [PostProcessBuild]
	public static void OnPostProcessBuild(BuildTarget target, string path)
	{
		//if(target != BuildTarget.iOS) { return; }

		//string plistPath = Path.Combine(path, "Info.plist");
		//PlistDocument plist = new PlistDocument();
		//plist.ReadFromString(File.ReadAllText(plistPath));

		//PlistElementDict rootDict = plist.root;
		//rootDict.SetString("NSPhotoLibraryUsageDescription", "Requires access to the Photo Library");

		//File.WriteAllText(plistPath, plist.WriteToString());
	}
}