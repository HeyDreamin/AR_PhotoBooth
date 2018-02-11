using UnityEngine;
using System.Collections;

public class PostSample : MonoBehaviour {

	string text = "Sample Text";
	string url = "https://www.assetstore.unity3d.com/#/content/9277";

	// Use this for initialization
	void Start () {
	}
	
	void OnGUI () {
		
		// Tweet
		if (GUI.Button(new Rect(0, Screen.height*0.5f, Screen.width*0.5f, Screen.height*0.5f), "Tweet")) {
			#if UNITY_IPHONE
				if (Application.platform == RuntimePlatform.IPhonePlayer) {
					// Tweet (iOS 5.0 or later)
					iOS_PostToServices.Tweet (text, url);
				}
			#endif
		}

		// Facebook
		if (GUI.Button(new Rect(Screen.width*0.5f, Screen.height*0.5f, Screen.width*0.5f, Screen.height*0.5f), "Post to\nFacebook")) {
			#if UNITY_IPHONE
				if (Application.platform == RuntimePlatform.IPhonePlayer) {
					// Facebook (iOS 6.0 or later)
					iOS_PostToServices.PostToFacebook (text, url);
				}
			#endif
		}
	}
}
