//
//  PostToServices.h
//  PostToServicesLib
//
//  Created by Teruaki ONODA on 13/05/30.
//  Copyright (c) 2013-2015 WhiteDev All rights reserved.
//

#import <UIKit/UIKit.h>
#import <Foundation/Foundation.h>
#import <QuartzCore/QuartzCore.h>
#import <AssetsLibrary/AssetsLibrary.h>
#import <MessageUI/MessageUI.h>
#import <MessageUI/MFMailComposeViewController.h>
#import <Social/Social.h>


typedef void (CBFunc_finishedServices)(bool succeed);

enum {
	kStateAvailable = 0,
	kStateNotAvailable_iOSVersion = 1,
	kStateNotAvailable_Account = 2,
};

extern uint getSocialPostState(NSString *serviceType);
extern bool canSocialPost(NSString *serviceType, bool forPost=true);
extern bool postToSocial(UIViewController *parentViewController, NSString *serviceType, NSString *text, NSString *url, UIImage *image, CBFunc_finishedServices *func=NULL);

extern uint getTweetState();
extern bool canTweet(bool forPost=true);
extern bool postToTwitter(UIViewController *parentViewController, NSString *text, NSString *url, UIImage *image, CBFunc_finishedServices *func=NULL);

extern uint getActivityState();
extern bool canActivity(bool forPost=true);
extern bool postToActivity(UIViewController *parentViewController, NSString *text, NSString *url, UIImage *image, uint disableFlags=0, CBFunc_finishedServices *func=NULL);

struct MailInfo {
	NSString *subject;
	NSMutableArray *toList;
	NSMutableArray *ccList;
	NSMutableArray *bccList;
};
extern uint getMailState();
extern bool canMail(bool forPost=true);
extern bool sendMail(UIViewController *parentViewController, NSString *text, NSString *url, UIImage *image, const MailInfo& mailInfo, CBFunc_finishedServices *func=NULL);



@interface PostToServices : UIViewController <UIPopoverControllerDelegate, MFMailComposeViewControllerDelegate>;
@property (retain, nonatomic) UIPopoverController *popoverController;
@property (retain, nonatomic) UIViewController* parentViewController;
@end
