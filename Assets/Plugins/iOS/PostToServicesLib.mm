//
//  PostToServicesLib.mm
//  PostToServicesLib
//
//  Created by Teruaki ONODA on 13/05/30.
//  Copyright (c) 2013-2015 WhiteDev All rights reserved.
//

#import <UIKit/UIKit.h>
#import <Foundation/Foundation.h>
#import "PostToServices.h"

#define NON_ARC (UNITY_VERSION < 500)


extern UIViewController *UnityGetGLViewController(); // Root view controller of Unity screen.
extern "C" void UnitySendMessage(const char* obj, const char* method, const char* msg);

typedef struct {
	unsigned char r, g, b, a;
} Color32;


static const char *strServiceType_Tweet = "Tweet";
static const char *strServiceType_Facebook = "Facebook";
static const char *strServiceType_SinaWeibo = "SinaWeibo";
static const char *strServiceType_Activity = "Activity";
static const char *strServiceType_Mail = "Mail";

static const char *strCallbackResultMessage_Succeed = "Result: Succeed";
static const char *strCallbackResultMessage_Canceled = "Result: Canceled";

//
static bool bCallback = false;
static char strCallbackGameObjectName[1024] = {0};
static char strCallbackMethodName[1024] = {0};

static uint s_activityDisableFlags = 0;

extern "C" void PostToServices_setActivityDisableFlags(uint flags) {
	s_activityDisableFlags = flags;
}


extern "C" void PostToServices_reset();
extern "C" void PostToServices_setText(const char *text);
extern "C" void PostToServices_setURL(const char *url);
extern "C" void PostToServices_setImage(const Color32 *imageData, int width, int height);
extern "C" bool PostToServices_post(const char *serviceType, const char *callbackGameObjectName, const char *callbackMethodName);
extern "C" bool PostToServices_isAvailable(const char *serviceType);

// For Mail
extern "C" void PostToServices_clearMailInfo();
extern "C" void PostToServices_setMailSubject(const char *str);
extern "C" void PostToServices_addMailToList(const char *str);
extern "C" void PostToServices_addMailCcList(const char *str);
extern "C" void PostToServices_addMailBccList(const char *str);




#pragma mark -
// Implementation
uint iOSVersion = 600;
BOOL biPad = NO;

static NSString *s_text = nil;
static NSString *s_url = nil;
static UIImage *s_image = nil;
static Color32 *s_pixelData = NULL;
static MailInfo s_mailInfo = {0};

static void checkDevice()
{
	static BOOL checked = NO;
	if (checked == NO) {
		iOSVersion = (uint)([[[UIDevice currentDevice] systemVersion] floatValue] * 100.0f);

		biPad = NO;
		{
			#define UI_USER_INTERFACE_IDIOM() \
			   ([[UIDevice currentDevice] respondsToSelector:@selector(userInterfaceIdiom)] ? \
			   [[UIDevice currentDevice] userInterfaceIdiom] : \
			   UIUserInterfaceIdiomPhone)
   			if (UI_USER_INTERFACE_IDIOM() == UIUserInterfaceIdiomPad) {
				biPad = YES;
			}
		}

		checked = YES;
	}
}

static void releaseText()
{
	if (s_text) {
#if NON_ARC
		[s_text release];
#endif
		s_text = nil;
	}
}
static void releaseURL()
{
	if (s_url) {
#if NON_ARC
		[s_url release];
#endif
		s_url = nil;
	}
}
static void releaseImage()
{
	if (s_image) {
#if NON_ARC
		[s_image release];
#endif
		s_image = nil;
	}
	if (s_pixelData) {
		delete[] s_pixelData;
		s_pixelData = NULL;
	}
}
static void releaseMailInfo()
{
#if NON_ARC
	[s_mailInfo.subject release];
	[s_mailInfo.toList release];
	[s_mailInfo.ccList release];
	[s_mailInfo.bccList release];
#endif
	s_mailInfo.subject = nil;
	s_mailInfo.toList = nil;
	s_mailInfo.ccList = nil;
	s_mailInfo.bccList = nil;
}


#pragma mark -
extern "C" void PostToServices_reset()
{
	checkDevice();
	releaseMailInfo();
	releaseText();
	releaseURL();
	releaseImage();
}

extern "C" void PostToServices_setText(const char *text)
{
	checkDevice();

	releaseText();
	s_text = [NSString stringWithUTF8String:text];
#if NON_ARC
	[s_text retain];
#endif
}

extern "C" void PostToServices_setURL(const char *url)
{
	checkDevice();

	releaseURL();
	s_url = [NSString stringWithUTF8String:url];
#if NON_ARC
	[s_url retain];
#endif
}

extern "C" void PostToServices_setImage(const Color32 *imageData, int width, int height)
{
	checkDevice();

	releaseImage();

	// Copy
	s_pixelData = new Color32[width*height];
	if ((uintptr_t)s_pixelData == (uintptr_t)NULL) return;
	const Color32 *pSrc = (const Color32 *)imageData;
	for (int y=0; y<height; ++y) {
		Color32 *pDst = &s_pixelData[width*((height-1)-y)];
		memcpy(pDst, pSrc, width*sizeof(Color32));
		pSrc += width;
	}
	
	// Create UIImage
	CGDataProviderRef dataProviderRef = CGDataProviderCreateWithData(NULL, s_pixelData, width*height*4, NULL);
	CGImageRef imageRef = CGImageCreate(
		width, height, 8, 32, width*4,
		CGColorSpaceCreateDeviceRGB(),
		kCGBitmapByteOrderDefault,
		dataProviderRef,
		NULL, NO, kCGRenderingIntentDefault);

	s_image = [UIImage imageWithCGImage:imageRef];
#if NON_ARC
	[s_image retain];
#endif

	CGDataProviderRelease(dataProviderRef);
	CGImageRelease(imageRef);
}

static void finishedService(bool succeed)
{
	if (bCallback) {
        	UnitySendMessage(
        		strCallbackGameObjectName,
        		strCallbackMethodName,
        		succeed ? strCallbackResultMessage_Succeed : strCallbackResultMessage_Canceled
        	);
        }
}

extern "C" bool PostToServices_post(const char *serviceType, const char *callbackGameObjectName, const char *callbackMethodName)
{
	checkDevice();

	if (callbackGameObjectName && callbackMethodName) {
		strncpy(strCallbackGameObjectName, callbackGameObjectName, sizeof(strCallbackGameObjectName));
		strncpy(strCallbackMethodName, callbackMethodName, sizeof(strCallbackMethodName));
		bCallback = true;
	} else {
		bCallback = false;
	}

	if (strcmp(serviceType, strServiceType_Tweet) == 0) {
		// Twitter
		if (canSocialPost(SLServiceTypeTwitter)) {
			return (postToSocial(UnityGetGLViewController(), SLServiceTypeTwitter, s_text, s_url, s_image, finishedService));
		}
	} else if (strcmp(serviceType, strServiceType_Facebook) == 0) {
		// Facebook
		if (canSocialPost(SLServiceTypeFacebook)) {
            if (s_image != nil) releaseURL();   // Image or URL
			return (postToSocial(UnityGetGLViewController(), SLServiceTypeFacebook, s_text, s_url, s_image, finishedService));
		}
	} else if (strcmp(serviceType, strServiceType_SinaWeibo) == 0) {
		// Weibo
		if (canSocialPost(SLServiceTypeSinaWeibo)) {
			return (postToSocial(UnityGetGLViewController(), SLServiceTypeSinaWeibo, s_text, s_url, s_image, finishedService));
		}
	} else if (strcmp(serviceType, strServiceType_Activity) == 0) {
		// Activity
		if (canActivity()) {
			return (postToActivity(UnityGetGLViewController(), s_text, s_url, s_image, s_activityDisableFlags, finishedService));
		}
	} else if (strcmp(serviceType, strServiceType_Mail) == 0) {
		// Mail
		return (sendMail(UnityGetGLViewController(), s_text, s_url, s_image, s_mailInfo, finishedService));
	}

	return false;
}

extern "C" bool PostToServices_isAvailable(const char *serviceType)
{
	checkDevice();

	if (strcmp(serviceType, strServiceType_Tweet) == 0) {
		// Twitter
		return (canSocialPost(SLServiceTypeTwitter, /*forPost=*/false));
	} else if (strcmp(serviceType, strServiceType_Facebook) == 0) {
		// Facebook
		return (canSocialPost(SLServiceTypeFacebook, /*forPost=*/false));
	} else if (strcmp(serviceType, strServiceType_SinaWeibo) == 0) {
		// Weibo
		return (canSocialPost(SLServiceTypeSinaWeibo, /*forPost=*/false));
	} else if (strcmp(serviceType, strServiceType_Activity) == 0) {
		// Activity
		return (canActivity(/*forPost=*/false));
	} else if (strcmp(serviceType, strServiceType_Mail) == 0) {
		// Mail
		return (canMail(/*forPost=*/false));
	}

	return false;
}

extern "C" uint PostToServices_getState(const char *serviceType)
{
	checkDevice();

	if (strcmp(serviceType, strServiceType_Tweet) == 0) {
		// Twitter
		return getSocialPostState(SLServiceTypeTwitter);
	} else if (strcmp(serviceType, strServiceType_Facebook) == 0) {
		// Facebook
		return getSocialPostState(SLServiceTypeFacebook);
	} else if (strcmp(serviceType, strServiceType_SinaWeibo) == 0) {
		// Weibo
		return getSocialPostState(SLServiceTypeSinaWeibo);
	} else if (strcmp(serviceType, strServiceType_Activity) == 0) {
		// Activity
		return getActivityState();
	} else if (strcmp(serviceType, strServiceType_Mail) == 0) {
		// Mail
		return getMailState();
	}

	return false;
}


#pragma mark - For Mail
// For Mail
extern "C" void PostToServices_clearMailInfo()
{
	releaseMailInfo();
	s_mailInfo.toList = [[NSMutableArray alloc] initWithCapacity:10];
	s_mailInfo.ccList = [[NSMutableArray alloc] initWithCapacity:10];
	s_mailInfo.bccList = [[NSMutableArray alloc] initWithCapacity:10];
}
extern "C" void PostToServices_setMailSubject(const char *str)
{
	if (str) {
		s_mailInfo.subject = [NSString stringWithUTF8String:str];
#if NON_ARC
		[s_mailInfo.subject retain];
#endif
	}
}
extern "C" void PostToServices_addMailToList(const char *str)
{
	if (str) {
		[s_mailInfo.toList addObject:[NSString stringWithUTF8String:str]];
	}
}
extern "C" void PostToServices_addMailCcList(const char *str)
{
	if (str) {
		[s_mailInfo.ccList addObject:[NSString stringWithUTF8String:str]];
	}
}
extern "C" void PostToServices_addMailBccList(const char *str)
{
	if (str) {
		[s_mailInfo.bccList addObject:[NSString stringWithUTF8String:str]];
	}
}



#pragma mark - Auto Rotation
// Auto Rotation
@implementation SLComposeViewController (AutoRotation)
- (BOOL)shouldAutorotateToInterfaceOrientation:(UIInterfaceOrientation)interfaceOrientation {
	return [UnityGetGLViewController() shouldAutorotateToInterfaceOrientation:interfaceOrientation];
}
- (NSUInteger) supportedInterfaceOrientations {
	return [UnityGetGLViewController() supportedInterfaceOrientations];
}
- (BOOL) shouldAutorotate {
	return [UnityGetGLViewController() shouldAutorotate];
}
- (UIInterfaceOrientation) preferredInterfaceOrientationForPresentation {
	return [UnityGetGLViewController() preferredInterfaceOrientationForPresentation];
}
@end

@implementation UIActivityViewController (AutoRotation)
- (BOOL)shouldAutorotateToInterfaceOrientation:(UIInterfaceOrientation)interfaceOrientation {
	return [UnityGetGLViewController() shouldAutorotateToInterfaceOrientation:interfaceOrientation];
}
- (NSUInteger) supportedInterfaceOrientations {
	return [UnityGetGLViewController() supportedInterfaceOrientations];
}
- (BOOL) shouldAutorotate {
	return [UnityGetGLViewController() shouldAutorotate];
}
- (UIInterfaceOrientation) preferredInterfaceOrientationForPresentation {
	return [UnityGetGLViewController() preferredInterfaceOrientationForPresentation];
}
@end

@implementation MFMailComposeViewController (AutoRotation)
- (BOOL)shouldAutorotateToInterfaceOrientation:(UIInterfaceOrientation)interfaceOrientation {
	return [UnityGetGLViewController() shouldAutorotateToInterfaceOrientation:interfaceOrientation];
}
- (NSUInteger) supportedInterfaceOrientations {
	return [UnityGetGLViewController() supportedInterfaceOrientations];
}
- (BOOL) shouldAutorotate {
	return [UnityGetGLViewController() shouldAutorotate];
}
- (UIInterfaceOrientation) preferredInterfaceOrientationForPresentation {
	return [UnityGetGLViewController() preferredInterfaceOrientationForPresentation];
}
@end

