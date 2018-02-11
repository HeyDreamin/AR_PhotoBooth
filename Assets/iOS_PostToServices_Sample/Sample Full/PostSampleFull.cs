using UnityEngine;
using System.Collections;

public class PostSampleFull : MonoBehaviour {

	Demo demo;

	string text = "Message";	
	string url = "https://www.assetstore.unity3d.com/#/content/9277";	

	int unityVersion = 0;
	TouchScreenKeyboard keyboard = null;

	Texture2D texCaptureScreen = null;

	bool activityPopoverEnable = true;


	// Use this for initialization
	void Start () {
		demo = GameObject.Find("Main Camera").GetComponent<Demo>();

		char[] version = Application.unityVersion.ToCharArray();
		unityVersion = (((version[0]-'0') * 100) + ((version[2]-'0') * 10) + (version[4]-'0'));
	}
	
	IEnumerator CaptureScreen() {
		yield return new WaitForEndOfFrame();
		texCaptureScreen = ScreenCapture.Capture();
	}

	void OnGUI () {
		// Scaling
		float scale = (Screen.width > Screen.height) ? (Screen.width/960.0f) : (Screen.height/960.0f);
		Vector2 guiCanvasSize = new Vector2(Screen.width/scale, Screen.height/scale);
		GUIUtility.ScaleAroundPivot(new Vector2(scale, scale), Vector2.zero);
		
		// Buttons
		const int buttonCount = 4;
		float buttonWidth = guiCanvasSize.x/(buttonCount+1);
		float buttonHeight = guiCanvasSize.y/4;
		float buttonMargine = buttonWidth/(buttonCount+1);
		Rect buttonRect = new Rect(0, guiCanvasSize.y-buttonHeight, buttonWidth, buttonHeight);

		// Tweet
		bool doTweet = false;
		buttonRect.x = buttonMargine;
		if (iOS_PostToServices.CanTweet()) {
			doTweet = true;
		} else {
			uint state = iOS_PostToServices.GetTwitterState();
			if (state == iOS_PostToServices.StateNotAvailable_Platform) {
				GUI.Box(buttonRect, "Tweet\n(Unavailable)\n\n-> iOS Only");
			} else if (state == iOS_PostToServices.StateNotAvailable_iOSVersion) {
				GUI.Box(buttonRect, "Tweet\n(Unavailable)\n\n-> Lower iOS Version");
			} else if (state == iOS_PostToServices.StateNotAvailable_Account) {
//				GUI.Box(buttonRect, "Tweet\n(Unavailable)\n\n-> Setup\nTwitter Account");
				doTweet = true;
			} else {
				GUI.Box(buttonRect, "Tweet\n(Unavailable)");
			}
		}
		if (doTweet && GUI.Button(buttonRect, "Tweet")) {
			#if UNITY_IPHONE
				if (Application.platform == RuntimePlatform.IPhonePlayer) {
					// Tweet (iOS 5.0 or later)
					if (iOS_PostToServices.Tweet (text, url, texCaptureScreen, this.name, "OnFinishedPostToService")) {
						demo.Pause(true);
					}
				}
			#endif
		}

		// Facebook
		bool doPostToFacebook = false;
		buttonRect.x += (buttonWidth + buttonMargine);
		if (iOS_PostToServices.CanPostToFacebook()) {
			doPostToFacebook = true;
		} else {
			uint state = iOS_PostToServices.GetFacebookState();
			if (state == iOS_PostToServices.StateNotAvailable_Platform) {
				GUI.Box(buttonRect, "Post to \nFacebook\n(Unavailable)\n\n-> iOS Only");
			} else if (state == iOS_PostToServices.StateNotAvailable_iOSVersion) {
				GUI.Box(buttonRect, "Post to \nFacebook\n(Unavailable)\n\n-> Lower iOS Version");
			} else if (state == iOS_PostToServices.StateNotAvailable_Account) {
//				GUI.Box(buttonRect, "Post to \nFacebook\n(Unavailable)\n\n-> Setup\nFacebook Account");
				doPostToFacebook = true;
			} else {
				GUI.Box(buttonRect, "Post to \nFacebook\n(Unavailable)");
			}
		}
		if (doPostToFacebook && GUI.Button(buttonRect, "Post to\nFacebook")) {
			#if UNITY_IPHONE
				if (Application.platform == RuntimePlatform.IPhonePlayer) {
					// Facebook (iOS 6.0 or later)
					if (iOS_PostToServices.PostToFacebook (text, url, texCaptureScreen, this.name, "OnFinishedPostToService")) {
						demo.Pause(true);
					}
				}
			#endif
		}

		// Mail
		buttonRect.x += (buttonWidth + buttonMargine);
		if (iOS_PostToServices.CanMail() == false) {
			uint state = iOS_PostToServices.GetMailState();
			if (state == iOS_PostToServices.StateNotAvailable_Platform) {
				GUI.Box(buttonRect, "Mail\n(Unavailable)\n\n-> iOS Only");
			} else if (state == iOS_PostToServices.StateNotAvailable_iOSVersion) {
				GUI.Box(buttonRect, "Mail\n(Unavailable)\n\n-> Lower iOS Version");
			} else if (state == iOS_PostToServices.StateNotAvailable_Account) {
				GUI.Box(buttonRect, "Mail\n(Unavailable)\n\n-> Setup\nMail Account");
			} else {
				GUI.Box(buttonRect, "Mail\n(Unavailable)");
			}
		} else if (GUI.Button(buttonRect, "Mail")) {
			#if UNITY_IPHONE
				if (Application.platform == RuntimePlatform.IPhonePlayer) {
					// Mail (iOS 3.0 or later)
					iOS_PostToServices.MailInfo mailInfo;
					mailInfo.subject = "Subject";
					mailInfo.toList = new string[1] { "to@mail.com" };
#if true
					mailInfo.ccList = null;
					mailInfo.bccList = null;
#else
					mailInfo.ccList = new string[2] { "cc@mail.com", "cc2@mail.com" };
					mailInfo.bccList = new string[3] { "bcc@mail.com", "bcc2@mail.com", "bcc3@mail.com" };
#endif
					if (iOS_PostToServices.SendMail (ref mailInfo, text, url, texCaptureScreen, this.name, "OnFinishedPostToService")) {
						demo.Pause(true);
					}
				}
			#endif
		}

		// Activity
		buttonRect.x += (buttonWidth + buttonMargine);
		if (iOS_PostToServices.CanPostToActivity() == false) {
			uint state = iOS_PostToServices.GetActivityState();
			if (state == iOS_PostToServices.StateNotAvailable_Platform) {
				GUI.Box(buttonRect, "Activity\n(Unavailable)\n\n-> iOS Only");
			} else if (state == iOS_PostToServices.StateNotAvailable_iOSVersion) {
				GUI.Box(buttonRect, "Activity\n(Unavailable)\n\n-> Lower iOS Version");
			} else {
				GUI.Box(buttonRect, "Activity\n(Unavailable)");
			}
		} else if (GUI.Button(buttonRect, "Activity")) {
			#if UNITY_IPHONE
				if (Application.platform == RuntimePlatform.IPhonePlayer) {
					// Activity (iOS 6.0 or later)
					iOS_PostToServices.SetActivityPopoverEnable_for_iPad (activityPopoverEnable);
					iOS_PostToServices.SetActivityPopoverTargetRect_for_iPad (buttonRect.x*scale, buttonRect.y*scale, buttonRect.width*scale, buttonRect.height*scale);
					uint disableFlags = 0
//								| iOS_PostToServices.ActivityTypePostToFacebook
//								| iOS_PostToServices.ActivityTypePostToTwitter
//								| iOS_PostToServices.ActivityTypePostToWeibo
//								| iOS_PostToServices.ActivityTypeMessage
//								| iOS_PostToServices.ActivityTypeMail
//								| iOS_PostToServices.ActivityTypePrint
//								| iOS_PostToServices.ActivityTypeCopyToPasteboard
//								| iOS_PostToServices.ActivityTypeAssignToContact
//								| iOS_PostToServices.ActivityTypeSaveToCameraRoll
								// iOS 7 over
//								| iOS_PostToServices.ActivityTypeAddToReadingList
//								| iOS_PostToServices.ActivityTypePostToFlickr
//								| iOS_PostToServices.ActivityTypePostToVimeo
//								| iOS_PostToServices.ActivityTypePostToTencentWeibo
//								| iOS_PostToServices.ActivityTypeAirDrop
					;
					iOS_PostToServices.SetActivityDisableFlags (disableFlags);
					if (iOS_PostToServices.PostToActivity (text, url, texCaptureScreen, this.name, "OnFinishedPostToService")) {
						demo.Pause(true);
					}
				}
			#endif
		}
//		buttonRect.y -= 40/scale;
//		buttonRect.height = 40/scale;
//		activityPopoverEnable = GUI.Toggle(buttonRect, activityPopoverEnable, "Enable\nPopover");

		// Text, URL and Image
		GUI.Box(new Rect(0,0, guiCanvasSize.x*0.25f, guiCanvasSize.y*0.6f), "");
		buttonRect.x = buttonRect.y = 0;
		buttonRect.width = guiCanvasSize.x/4;
		buttonRect.height = guiCanvasSize.y/8;
		if (((unityVersion >= 410) && (unityVersion < 420)) && (Application.platform == RuntimePlatform.IPhonePlayer)) {
			if (GUI.Button(buttonRect, text)) {
				keyboard = TouchScreenKeyboard.Open(text, TouchScreenKeyboardType.NamePhonePad, /*autocorrection=*/true, /*multiline=*/false);
			}
			if (keyboard != null) {
				text = keyboard.text;
			}
			buttonRect.y += buttonRect.height;
			if (GUI.Button(buttonRect, url)) {
				keyboard = TouchScreenKeyboard.Open(url, TouchScreenKeyboardType.NamePhonePad, /*autocorrection=*/true, /*multiline=*/false);
			}
			if (keyboard != null) {
				url = keyboard.text;
			}
		} else {
			text = GUI.TextArea(buttonRect, text);
			buttonRect.y += buttonRect.height;
			url = GUI.TextArea(buttonRect, url);
		}
		buttonRect.y += buttonRect.height;
		buttonRect.height = guiCanvasSize.y/10;
		if (GUI.Button(buttonRect, "Capture\nScreen")) {
			StartCoroutine("CaptureScreen");
		}
		buttonRect.y += buttonRect.height;
		if (texCaptureScreen) {
			GUI.DrawTexture(new Rect(buttonRect.x,buttonRect.y, guiCanvasSize.x/4,guiCanvasSize.y/4), texCaptureScreen);
		}
	}

	#if UNITY_IPHONE
	void OnFinishedPostToService (string message) {
		demo.Pause(false);
	}
	#endif
}
