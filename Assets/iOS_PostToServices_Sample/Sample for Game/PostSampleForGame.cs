using UnityEngine;
using System.Collections;

public class PostSampleForGame : MonoBehaviour {

	Pong pong;

	public Texture2D texture;
	Rect rectTwitter;
	Rect rectFacebook;
	Rect rectMail;

	string message = "Message";
	string url = "https://www.assetstore.unity3d.com/#/content/9277";


	// Use this for initialization
	void Start () {
		pong = GameObject.Find("Main Camera").GetComponent<Pong>();
	}
	

	// Post to SNS
	const uint PostFlag_Twitter = (1 << 0);
	const uint PostFlag_Facebook = (1 << 1);
	const uint PostFlag_Screenshot = (1 << 15);
	
	IEnumerator PostToSNS (uint flags) {
		if (Application.platform != RuntimePlatform.IPhonePlayer) {
			yield break;
		}

		Texture2D imageTex2D = null;
		if ((flags & PostFlag_Screenshot) != 0) {
			yield return new WaitForEndOfFrame();
			imageTex2D = ScreenCapture.Capture();
		}

		bool succeed = false;
		if ((flags & PostFlag_Twitter) != 0) {
			succeed = iOS_PostToServices.Tweet (message, url, imageTex2D, this.name, "OnFinishedPostToService");
		} else if ((flags & PostFlag_Facebook) != 0) {
			succeed = iOS_PostToServices.PostToFacebook (message, url, imageTex2D, this.name, "OnFinishedPostToService");
		}
		if (succeed) {
			if (pong) {
				pong.Pause(true);
			}
		}
	}

	#if UNITY_IPHONE
	void OnFinishedPostToService (string message) {
		// Resume Game
		// e.g. Pause(false);
		if (pong) {
			pong.Pause(false);
		}
	}
	#endif


	void Update () {
		float scale = Pong.gameScale;
		float iconSize = 48*scale;
		rectTwitter = new Rect(4*scale,4*scale, iconSize,iconSize);
		rectFacebook = new Rect(((4+4)*scale+iconSize),4*scale, iconSize,iconSize);
		rectMail = new Rect(Screen.width-(4*scale+iconSize),4*scale, iconSize,iconSize);

		// Touch Event
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			if (Input.touchCount > 0) {
				for (int i=0; i<Input.touchCount; ++i) {
					Touch touch = Input.GetTouch(i);
					//if (touch.phase == TouchPhase.Began) 
					{
						Vector2 touchPos;
						touchPos.x = touch.position.x;
						touchPos.y = Screen.height-touch.position.y;
						if (TouchEvent(ref touchPos, /*began=*/(touch.phase == TouchPhase.Began))) {
							break;
						} else {
							if (pong) {
								pong.TouchEvent(ref touchPos, /*began=*/(touch.phase == TouchPhase.Began));
							}
						}
					}
				}
			}
		} else {
			if (Input.GetMouseButton(0)) {
				Vector2 touchPos;
				touchPos.x = Input.mousePosition.x;
				touchPos.y = Screen.height-Input.mousePosition.y;
				if (TouchEvent(ref touchPos, /*began=*/Input.GetMouseButtonDown(0))) {
				} else {
					if (pong) {
						pong.TouchEvent(ref touchPos, /*began=*/Input.GetMouseButtonDown(0));
					}
				}
			}
		}
	}

	bool TouchEvent(ref Vector2 touchPos, bool began) {
		bool action = false;
		if (rectTwitter.Contains(touchPos)) {
			// Twitter
			if (began) {
				if (Application.platform == RuntimePlatform.IPhonePlayer) {
					uint postFlags = (PostFlag_Twitter | PostFlag_Screenshot);
					StartCoroutine("PostToSNS", postFlags);
					action = true;
				} else {
					Debug.Log("Touched on Twitter");
				}
			}
		} else if (rectFacebook.Contains(touchPos)) {
			// Facebook
			if (began) {
				if (Application.platform == RuntimePlatform.IPhonePlayer) {
					uint postFlags = (PostFlag_Facebook | PostFlag_Screenshot);
					StartCoroutine("PostToSNS", postFlags);
					action = true;
				} else {
					Debug.Log("Touched on Facebook");
				}
			}
		} else if (rectMail.Contains(touchPos)) {
			// Facebook
			if (began) {
			#if UNITY_IPHONE
				if (Application.platform == RuntimePlatform.IPhonePlayer) {
					// Mail (iOS 3.0 or later)
					iOS_PostToServices.MailInfo mailInfo;
					mailInfo.subject = "[Pong] Support";
					mailInfo.toList = new string[1] { "support+pong@mail.com" };
					mailInfo.ccList = null;
					mailInfo.bccList = null;
					if (iOS_PostToServices.SendMail (ref mailInfo, "", "", null, this.name, "OnFinishedPostToService")) {
						if (pong) {
							pong.Pause(true);
						}
					}
					action = true;
				} else {
					Debug.Log("Touched on Mail");
				}
			#endif
			}
		}
		return action;
	}
	
	
	void OnGUI () {
		// Icons
		GUI.DrawTextureWithTexCoords(rectTwitter, texture, new Rect(0,0,0.5f,0.5f));
		GUI.DrawTextureWithTexCoords(rectFacebook, texture, new Rect(0.5f,0,0.5f,0.5f));
		GUI.DrawTextureWithTexCoords(rectMail, texture, new Rect(0,0.5f,0.5f,0.5f));
	}
}
