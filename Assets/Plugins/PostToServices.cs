using UnityEngine;
using System.Runtime.InteropServices;
using System.Collections;

public class iOS_PostToServices {

	public const string ServiceType_Tweet = "Tweet";
	public const string ServiceType_Facebook = "Facebook";
	public const string ServiceType_SinaWeibo = "SinaWeibo";
	public const string ServiceType_Activity = "Activity";
	const string ServiceType_Mail = "Mail";

	public const uint StateAvailable = 0;
	public const uint StateNotAvailable_iOSVersion = 1;
	public const uint StateNotAvailable_Account = 2;
	public const uint StateNotAvailable_Platform = 9999;
	
	// Interface
	
	// Tweet
	public static bool CanTweet () {
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			return PostToServices_isAvailable(ServiceType_Tweet);
		}
		return false;
	}
	public static uint GetTwitterState () {
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			return PostToServices_getState(ServiceType_Tweet);
		}
		return StateNotAvailable_Platform;
	}
	public static bool Tweet (string text, string url=null, Texture2D imageTex2D=null, string callbackGameObjectName=null, string callbackMethodName=null) {
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			return PostToServices__(ServiceType_Tweet, text, url, imageTex2D, callbackGameObjectName, callbackMethodName);
		}
		return false;
	}

	// Facebook
	public static bool CanPostToFacebook () {
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			return PostToServices_isAvailable(ServiceType_Facebook);
		}
		return false;
	}
	public static uint GetFacebookState () {
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			return PostToServices_getState(ServiceType_Facebook);
		}
		return StateNotAvailable_Platform;
	}
	public static bool PostToFacebook (string text, string url=null, Texture2D imageTex2D=null, string callbackGameObjectName=null, string callbackMethodName=null) {
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			return PostToServices__(ServiceType_Facebook, text, url, imageTex2D, callbackGameObjectName, callbackMethodName);
		}
		return false;
	}

	// Weibo
	public static bool CanPostToWeibo () {
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			return PostToServices_isAvailable(ServiceType_SinaWeibo);
		}
		return false;
	}
	public static uint GetWeiboState () {
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			return PostToServices_getState(ServiceType_SinaWeibo);
		}
		return StateNotAvailable_Platform;
	}
	public static bool PostToWeibo (string text, string url=null, Texture2D imageTex2D=null, string callbackGameObjectName=null, string callbackMethodName=null) {
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			return PostToServices__(ServiceType_SinaWeibo, text, url, imageTex2D, callbackGameObjectName, callbackMethodName);
		}
		return false;
	}

	// Social
	public static bool CanPostToSocial (string serviceType) {
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			return PostToServices_isAvailable(serviceType);
		}
		return false;
	}
	public static uint GetSocialState (string serviceType) {
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			return PostToServices_getState(serviceType);
		}
		return StateNotAvailable_Platform;
	}
	public static bool PostToSocial (string serviceType, string text, string url=null, Texture2D imageTex2D=null, string callbackGameObjectName=null, string callbackMethodName=null) {
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			return PostToServices__(serviceType, text, url, imageTex2D, callbackGameObjectName, callbackMethodName);
		}
		return false;
	}

	// Activity
	public static bool CanPostToActivity () {
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			return PostToServices_isAvailable(ServiceType_Activity);
		}
		return false;
	}
	public static uint GetActivityState () {
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			return PostToServices_getState(ServiceType_Activity);
		}
		return StateNotAvailable_Platform;
	}
	public static bool PostToActivity (string text, string url=null, Texture2D imageTex2D=null, string callbackGameObjectName=null, string callbackMethodName=null) {
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			return PostToServices__(ServiceType_Activity, text, url, imageTex2D, callbackGameObjectName, callbackMethodName);
		}
		return false;
	}

	public static void SetActivityDisableFlags(uint flags) {
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			PostToServices_setActivityDisableFlags(flags);
		}
	}
	public const uint ActivityTypePostToFacebook = (1 << 0);
	public const uint ActivityTypePostToTwitter = (1 << 1);
	public const uint ActivityTypePostToWeibo = (1 << 2);
	public const uint ActivityTypeMessage = (1 << 3);
	public const uint ActivityTypeMail = (1 << 4);
	public const uint ActivityTypePrint = (1 << 5);
	public const uint ActivityTypeCopyToPasteboard = (1 << 6);
	public const uint ActivityTypeAssignToContact = (1 << 7);
	public const uint ActivityTypeSaveToCameraRoll = (1 << 8);
	// iOS 7 over
	public const uint ActivityTypeAddToReadingList = (1 << 9);
	public const uint ActivityTypePostToFlickr = (1 << 10);
	public const uint ActivityTypePostToVimeo = (1 << 11);
	public const uint ActivityTypePostToTencentWeibo = (1 << 12);
	public const uint ActivityTypeAirDrop = (1 << 13);

	// Mail
	public struct MailInfo {
		public string subject;
		public string[] toList;
		public string[] ccList;
		public string[] bccList;
	};
	public static bool CanMail () {
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			return PostToServices_isAvailable(ServiceType_Mail);
		}
		return false;
	}
	public static uint GetMailState () {
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			return PostToServices_getState(ServiceType_Mail);
		}
		return StateNotAvailable_Platform;
	}
	public static bool SendMail (ref MailInfo mailInfo, string text, string url=null, Texture2D imageTex2D=null, string callbackGameObjectName=null, string callbackMethodName=null) {
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			return SendMail__(text, url, imageTex2D, ref mailInfo, callbackGameObjectName, callbackMethodName);
		}
		return false;
	}



	// (Position of Popover for iPad)
	public static void SetActivityPopoverEnable_for_iPad (bool enable) {
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			PostToServices_SetPopoverEnable(enable);
		}
	}
	public static void SetActivityPopoverToCenter_for_iPad () {
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			PostToServices_SetPopoverToCenter();
		}
	}
	public static void SetActivityPopoverTargetRect_for_iPad (float x, float y, float width, float height) {
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			PostToServices_SetPopoverTargetRect(x, y, width, height);
		}
	}


	
	// Implementation
	[DllImport ("__Internal")] private static extern void PostToServices_reset();
	[DllImport ("__Internal")] private static extern void PostToServices_setText(string text);
	[DllImport ("__Internal")] private static extern void PostToServices_setURL(string url);
	[DllImport ("__Internal")] private static extern void PostToServices_setImage(Color32[] imageData, int width, int height);
	[DllImport ("__Internal")] private static extern bool PostToServices_post(string serviceType, string callbackGameObjectName, string callbackMethodName);
	[DllImport ("__Internal")] private static extern bool PostToServices_isAvailable(string serviceType);
	[DllImport ("__Internal")] private static extern uint PostToServices_getState(string serviceType);
	//
	[DllImport ("__Internal")] private static extern void PostToServices_setActivityDisableFlags(uint flags);
	//
	[DllImport ("__Internal")] private static extern void PostToServices_SetPopoverEnable(bool enable);
	[DllImport ("__Internal")] private static extern void PostToServices_SetPopoverToCenter();
	[DllImport ("__Internal")] private static extern void PostToServices_SetPopoverTargetRect(float x, float y, float width, float height);
	// For Mail
	[DllImport ("__Internal")] private static extern void PostToServices_clearMailInfo();
	[DllImport ("__Internal")] private static extern void PostToServices_setMailSubject(string str);
	[DllImport ("__Internal")] private static extern void PostToServices_addMailToList(string str);
	[DllImport ("__Internal")] private static extern void PostToServices_addMailCcList(string str);
	[DllImport ("__Internal")] private static extern void PostToServices_addMailBccList(string str);


	//static string strCallbackResultMessage_Succeed = "Result: Succeed";
	
	static bool PostToServices__ (string serviceType, string text, string url, Texture2D imageTex2D, string callbackGameObjectName, string callbackMethodName) {
		PostToServices_reset();
		if (text != null) {
			PostToServices_setText(text);
		}
		if (url != null) {
			PostToServices_setURL(url);
		}
		if (imageTex2D != null) {
			PostToServices_setImage(imageTex2D.GetPixels32(), imageTex2D.width, imageTex2D.height);
		}
		return PostToServices_post(serviceType, callbackGameObjectName, callbackMethodName);
	}
	
	static bool SendMail__ (string text, string url, Texture2D imageTex2D, ref MailInfo mailInfo, string callbackGameObjectName, string callbackMethodName) {
		PostToServices_reset();
		PostToServices_clearMailInfo();
//		if (mailInfo != null) 
		{
			PostToServices_setMailSubject(mailInfo.subject);
			if (mailInfo.toList != null) {
				foreach (string str in mailInfo.toList) {
					PostToServices_addMailToList(str);
				}
			}
			if (mailInfo.ccList != null) {
				foreach (string str in mailInfo.ccList) {
					PostToServices_addMailCcList(str);
				}
			}
			if (mailInfo.bccList != null) {
				foreach (string str in mailInfo.bccList) {
					PostToServices_addMailBccList(str);
				}
			}
		}
		if (text != null) {
			PostToServices_setText(text);
		}
		if (url != null) {
			PostToServices_setURL(url);
		}
		if (imageTex2D != null) {
			PostToServices_setImage(imageTex2D.GetPixels32(), imageTex2D.width, imageTex2D.height);
		}
		return PostToServices_post(ServiceType_Mail, callbackGameObjectName, callbackMethodName);
	}
}
