Unity iOS native plugin

iOS Post to Twitter/Facebook/Mail


---------------------------------------------------------------------------
Description: 
This plugin is run on iOS native code. You can post text and image to Twitter, Facebook or Mail.


Features:
- Post text and image to any services.
- Post to Twitter
- Post to Facebook
- Send Mail
- Post to Weibo       (China Only)
- Support for add an URL to message


Must Link Frameworks in Xcode:
- Social.framework
- MessageUI.framework


Demo Video:
http://youtu.be/inH7_smFCJo


How to use (C# Script):

Twitter:
	iOS_PostToServices.CanTweet();
	iOS_PostToServices.Tweet (text, url, image);

Facebook:
	iOS_PostToServices.CanPostToFacebook();
	iOS_PostToServices.PostToFacebook (text, url, image);

Activity:
	iOS_PostToServices.CanPostToActivity();
	iOS_PostToServices.PostToActivity (text, url, image);

Mail:
	iOS_PostToServices.MailInfo mailInfo;
	mailInfo.subject = "Subject";
	mailInfo.toList = new string[1] { "to@mail.com" };
	mailInfo.ccList = new string[2] { "cc@mail.com", "cc2@mail.com" };
	mailInfo.bccList = null;
	iOS_PostToServices.SendMail (ref mailInfo, text, url, image);

Support: whitedev.support@gmail.com


---------------------------------------------------------------------------
Version Changes:
1.9.1:
	- Fixed problem that can’t post image to Facebook
1.9:
	- Support for Unity 5 and Xcode ARC
1.8.2:
	- Fixed errors on Xcode in Unity 4.6.2 p2.
1.8.1:
	- Fixed a crash problem when used the new Activity disable flags (added in iOS7) on iOS6 device.
1.8:
	- Support for add an URL to message
	- Added new disable flags for the Activity in iOS 7
	- Fixed some issues

	**** To support Unity3 in this version is the last. ****
1.6:
	- Added API for Weibo (China Only)
	e.g.  iOS_PostToServices.PostToWeibo(message, image, null, null);
1.5:
	- Added a simple sample scene
	- Added a sample scene for game
	- Fixed crush when callback parameter is null
1.4:
	- Support for Mail
1.3:
	- Include iOS source code
1.2:
	- Added function for check available services.
1.1:
	- Support for Unity 4.2
	- Fixed TextArea issue on iPhone. (Unity 4.1 or higher)
	- Fixed conflict my plugins.
1.0.3:
	- Support for Unity 3.5.7/4.0/4.1 or Higher
1.0.2:
	- Fix Popover Positioning
1.0:
	- Initial version.
