//
//  PostToServices.mm
//  PostToServicesLib
//
//  Created by Teruaki ONODA on 13/05/30.
//  Copyright (c) 2013-2015 WhiteDev All rights reserved.
//

#import "PostToServices.h"

#define NON_ARC (UNITY_VERSION < 500)


extern uint iOSVersion;
extern BOOL biPad;

extern bool PostToServices_bPopoverEnable;
extern bool PostToServices_bPopoverCenter;
extern CGRect PostToServices_rectPopoverTarget;



#pragma mark -
#pragma mark PostToServices class @Interface

// PostToServices
@interface PostToServices (Private)
+ (PostToServices *) instance;

- (bool) postToActivity:(UIViewController *)parentViewController text:(NSString *)text url:(NSString *)url image:(UIImage *)image disableFlags:(uint)disableFlags callback:(CBFunc_finishedServices *)func;
- (bool) sendMail:(UIViewController *)parentViewController text:(NSString *)text url:(NSString *)url image:(UIImage *)image mailInfo:(const MailInfo &)mailInfo  callback:(CBFunc_finishedServices *)func;
- (void) smartDismiss;
-(void) releaseController;
@end



#pragma mark -
#pragma mark Social

// Social (Twitter/Facebook)
uint getSocialPostState(NSString *serviceType)
{
	if (iOSVersion >= 600) {
		Class checkClass = (NSClassFromString(@"SLComposeViewController"));
		if (checkClass != nil) {
			if ([checkClass isAvailableForServiceType:serviceType]) {
				return kStateAvailable;
			} else {
				return kStateNotAvailable_Account;
			}
		}
	}
	return kStateNotAvailable_iOSVersion;
}

bool canSocialPost(NSString *serviceType, bool forPost/*=true*/)
{
	uint state = getSocialPostState(serviceType);
	if (state != kStateNotAvailable_iOSVersion) {
		return (forPost || (state == kStateAvailable));
	}
	return false;
}

bool postToSocial(UIViewController *parentViewController, NSString *serviceType, NSString *text, NSString *url, UIImage *image, CBFunc_finishedServices *func/*=NULL*/)
{
	if (canSocialPost(serviceType) == false) {
		return false;
	}
//	if ([SLComposeViewController isAvailableForServiceType:serviceType] == false) {
//		return false;
//	}


	SLComposeViewController *viewController = [SLComposeViewController composeViewControllerForServiceType:serviceType];
	if (viewController == nil) {
		return false;
	}
    if (text != nil && text.length > 0) [viewController setInitialText:text];
    if (image != nil)                           [viewController addImage:image];
    if (url != nil && url.length > 0)   [viewController addURL:[NSURL URLWithString:url]];


	UIViewController *parentViewController__ = parentViewController;
#if NON_ARC
	UIViewController *viewController__ = [viewController retain];	// Retain to escape crash
#else
	UIViewController *viewController__ = viewController;
#endif
	CBFunc_finishedServices *func__ = func;
	viewController.completionHandler =
		^(SLComposeViewControllerResult res) {
			if (res == SLComposeViewControllerResultCancelled) {
				//				NSLog(@"Canceled");
				if (func__) func__(false);
			} else if (res == SLComposeViewControllerResultDone) {
				//				NSLog(@"Done");
				if (func__) func__(true);
			} else {
				if (func__) func__(false);
			}
			[parentViewController__ dismissViewControllerAnimated:YES completion:nil];

			// For escape a crash when closed
			[viewController__.view.superview removeFromSuperview];
		};

        [parentViewController presentViewController:viewController animated:YES completion:nil];

	return true;
}



#pragma mark -
#pragma mark Activity

// Activity
uint getActivityState()
{
	if (iOSVersion >= 600) {
		Class checkClass = (NSClassFromString(@"UIActivityViewController"));
		if (checkClass != nil) {
			return kStateAvailable;
		}
	}
	return kStateNotAvailable_iOSVersion;
}

bool canActivity(bool forPost/*=true*/)
{
	uint state = getActivityState();
	if (state != kStateNotAvailable_iOSVersion) {
		return (forPost || (state == kStateAvailable));
	}
	return false;
}

bool postToActivity(UIViewController *parentViewController, NSString *text, NSString *url, UIImage *image, uint disableFlags/*=0*/, CBFunc_finishedServices *func/*=NULL*/)
{
	return [[PostToServices instance] postToActivity:parentViewController text:text url:url image:image disableFlags:disableFlags callback:func];
}



#pragma mark -
#pragma mark Mail

// Mail
uint getMailState()
{
	Class mailClass = (NSClassFromString(@"MFMailComposeViewController"));
	if (mailClass != nil) {
		if ([mailClass canSendMail]) {
			return kStateAvailable;
		} else {
			return kStateNotAvailable_Account;
		}
	}
	return kStateNotAvailable_iOSVersion;
}

bool canMail(bool forPost/*=true*/)
{
	uint state = getMailState();
	if (state != kStateNotAvailable_iOSVersion) {
		return (forPost || (state == kStateAvailable));
	}
	return false;
}

bool sendMail(UIViewController *parentViewController, NSString *text, NSString *url, UIImage *image, const MailInfo& mailInfo, CBFunc_finishedServices *func/*=NULL*/)
{
	return [[PostToServices instance] sendMail:parentViewController text:text url:url image:image mailInfo:mailInfo callback:func];
}



#pragma mark -
#pragma mark PostToServices class @implementation

// PostToServices
@implementation PostToServices
@synthesize popoverController, parentViewController=parentViewController_;

static CBFunc_finishedServices *callbackFunc_ = NULL;

static PostToServices *pInstance = nil;

+ (PostToServices *) instance {
	if (pInstance == nil) {
		pInstance = [[PostToServices alloc] init];
	}
	return pInstance;
}

-(void) releaseController {
	if (popoverController) {
		popoverController.delegate = nil;
	}
	popoverController = nil;
	parentViewController_ = nil;
}

- (void)dealloc {
	[self releaseController];

#if NON_ARC
	[super dealloc];
#endif

	pInstance = nil;
}


// postToActivity
- (bool) postToActivity:(UIViewController *)parentViewController text:(NSString *)text url:(NSString *)url image:(UIImage *)image disableFlags:(uint)disableFlags callback:(CBFunc_finishedServices *)func
{
	[self smartDismiss];

	if (canActivity() == false) {
		return false;
	}

	enum ActivityType {
		ActivityTypePostToFacebook = (1 << 0),
		ActivityTypePostToTwitter = (1 << 1),
		ActivityTypePostToWeibo = (1 << 2),
		ActivityTypeMessage = (1 << 3),
		ActivityTypeMail = (1 << 4),
		ActivityTypePrint = (1 << 5),
		ActivityTypeCopyToPasteboard = (1 << 6),
		ActivityTypeAssignToContact = (1 << 7),
		ActivityTypeSaveToCameraRoll = (1 << 8),
		// iOS 7 over
		ActivityTypeAddToReadingList = (1 << 9),
		ActivityTypePostToFlickr = (1 << 10),
		ActivityTypePostToVimeo = (1 << 11),
		ActivityTypePostToTencentWeibo = (1 << 12),
		ActivityTypeAirDrop = (1 << 13),
	};
	NSMutableArray* items = [NSMutableArray arrayWithCapacity:2];
	if (text) [items addObject: text];
	if (url) [items addObject: [NSURL URLWithString:url]];
	if (image) [items addObject: image];
	UIActivityViewController *viewController = [[UIActivityViewController alloc] initWithActivityItems:items applicationActivities:nil];
#if NON_ARC
	[viewController autorelease];
#endif
	if (disableFlags) {
		NSMutableArray* disableItems = [NSMutableArray arrayWithCapacity:9];
		if (disableFlags & ActivityTypePostToFacebook) [disableItems addObject: UIActivityTypePostToFacebook];
		if (disableFlags & ActivityTypePostToTwitter) [disableItems addObject: UIActivityTypePostToTwitter];
		if (disableFlags & ActivityTypePostToWeibo) [disableItems addObject: UIActivityTypePostToWeibo];
		if (disableFlags & ActivityTypeMessage) [disableItems addObject: UIActivityTypeMessage];
		if (disableFlags & ActivityTypeMail) [disableItems addObject: UIActivityTypeMail];
		if (disableFlags & ActivityTypePrint) [disableItems addObject: UIActivityTypePrint];
		if (disableFlags & ActivityTypeCopyToPasteboard) [disableItems addObject: UIActivityTypeCopyToPasteboard];
		if (disableFlags & ActivityTypeAssignToContact) [disableItems addObject: UIActivityTypeAssignToContact];
		if (disableFlags & ActivityTypeSaveToCameraRoll) [disableItems addObject: UIActivityTypeSaveToCameraRoll];
		// iOS 7 over
		if (iOSVersion >= 700) {
			if (disableFlags & ActivityTypeAddToReadingList) [disableItems addObject: UIActivityTypeAddToReadingList];
			if (disableFlags & ActivityTypePostToFlickr) [disableItems addObject: UIActivityTypePostToFlickr];
			if (disableFlags & ActivityTypePostToVimeo) [disableItems addObject: UIActivityTypePostToVimeo];
			if (disableFlags & ActivityTypePostToTencentWeibo) [disableItems addObject: UIActivityTypePostToTencentWeibo];
			if (disableFlags & ActivityTypeAirDrop) [disableItems addObject: UIActivityTypeAirDrop];
		}
		viewController.excludedActivityTypes = disableItems;
	}
	if (viewController == nil) {
		return false;
	}

	parentViewController_ = parentViewController;
	callbackFunc_ = func;

	UIViewController *parentViewController__ = parentViewController;
	viewController.completionHandler =
		^(NSString *activityType, BOOL completed) {
			if (callbackFunc_) {
				callbackFunc_(completed);
			}
			[parentViewController__ dismissViewControllerAnimated:YES completion:nil];
			[self releaseController];
		};


	if (biPad && PostToServices_bPopoverEnable) {
		// Popover
		popoverController = [[UIPopoverController alloc] initWithContentViewController:viewController];
		popoverController.delegate = self;

		UIView *parentView = parentViewController.view;
		UIPopoverArrowDirection arrow = 0;
		CGRect rect = viewController.view.bounds;
		if (PostToServices_bPopoverCenter) {
			rect.origin.x = (parentView.bounds.size.width - rect.size.width) * 0.5f;
			rect.origin.y = (parentView.bounds.size.height - rect.size.height) * 0.5f;
			arrow = 0;
		} else {
			rect = PostToServices_rectPopoverTarget;
			arrow = UIPopoverArrowDirectionAny;
		}
		[popoverController presentPopoverFromRect:rect inView:parentView permittedArrowDirections:arrow animated:YES];
	} else {
		// Non Popover
	        [parentViewController presentViewController:viewController animated:YES completion:^ {
			}
		];
	}

	return true;
}

- (bool) sendMail:(UIViewController *)parentViewController text:(NSString *)text url:(NSString *)url image:(UIImage *)image mailInfo:(const MailInfo &)mailInfo callback:(CBFunc_finishedServices *)func;
{
	[self smartDismiss];

	if (canMail() == false) {
		return false;
	}

	MFMailComposeViewController *viewController = [[MFMailComposeViewController alloc] init];
#if NON_ARC
	[viewController autorelease];
#endif
	if (viewController == nil) {
		return false;
	}
	
	[viewController setSubject: mailInfo.subject];
	[viewController setToRecipients: mailInfo.toList];
	[viewController setCcRecipients: mailInfo.ccList];
	[viewController setBccRecipients: mailInfo.bccList];
	if (url) {
		[viewController setMessageBody:[NSString stringWithFormat:@"%@\n%@", text, url] isHTML:NO];
	} else {
		[viewController setMessageBody:text isHTML:NO];
	}
	if (image) {
		if (true) {
			// PNG
			NSData *imageData = UIImagePNGRepresentation(image);
			[viewController addAttachmentData:imageData mimeType:@"image/png" fileName:@"image"];
		} else {
			// JPG
			NSData *imageData = UIImageJPEGRepresentation(image, /*compressionQuality=*/0.7f);
			[viewController addAttachmentData:imageData mimeType:@"image/jpg" fileName:@"image"];
		}
	}

	parentViewController_ = parentViewController;
	callbackFunc_ = func;

	viewController.mailComposeDelegate = self;
        [parentViewController presentViewController:viewController animated:YES completion:^ {
		}
	];

	return true;
}
// Delegate of MFMailComposeViewController
- (void) mailComposeController:(MFMailComposeViewController*)controller didFinishWithResult: (MFMailComposeResult)result error: (NSError*)error
{
	BOOL succeed = NO;
	switch (result) {
		case MFMailComposeResultSent:
		case MFMailComposeResultSaved:
			succeed = YES;
			break;
		case MFMailComposeResultCancelled:
		case MFMailComposeResultFailed:
		default:
			break;
	}

	if (callbackFunc_) {
		callbackFunc_(succeed);
	}
	[parentViewController_ dismissViewControllerAnimated:YES completion:nil];
	[self releaseController];
}

- (void) smartDismiss {
	if (biPad == false) {
		[parentViewController_ dismissViewControllerAnimated:YES completion:^ {
			}
		];
	} else {
		if (popoverController) {
			[popoverController dismissPopoverAnimated:YES];
		}
	}
	[self releaseController];
}

- (void) popoverControllerDidDismissPopover:(UIPopoverController *)popoverController
{
	[self smartDismiss];
	if (callbackFunc_) {
		callbackFunc_(false);
	}
}

@end
