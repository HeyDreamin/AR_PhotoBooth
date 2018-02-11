//
//  PostToServicesUtil.mm
//  PostToServicesLib
//
//  Created by Teruaki ONODA on 13/05/30.
//  Copyright (c) 2013-2015 WhiteDev All rights reserved.
//


/*
	Popover for iPad
*/

#import "DisplayManager.h"

extern UIView* UnityGetGLView();

extern "C" bool PostToServices_bPopoverEnable = true;
extern "C" bool PostToServices_bPopoverCenter = true;
extern "C" CGRect PostToServices_rectPopoverTarget = {0};

extern "C" void PostToServices_SetPopoverEnable(bool enable) {
	PostToServices_bPopoverEnable = enable;
}

extern "C" void PostToServices_SetPopoverToCenter() {
	PostToServices_bPopoverCenter = true;
}

static float GetUnitySurfaceToUIViewScale() {
	float scale = 1.0f;
	float viewScale = UnityGetGLView().contentScaleFactor;
#if (UNITY_VERSION > 462)
	scale = ((float)GetMainDisplaySurface()->systemW / viewScale) / (float)GetMainDisplaySurface()->targetW;
#elif (UNITY_VERSION == 462)
    #ifdef _TRAMPOLINE_UNITY_DISPLAYMANAGER_H_
	scale = ((float)GetMainRenderingSurface()->systemW / viewScale) / (float)GetMainRenderingSurface()->targetW;
    #else
	scale = ((float)GetMainDisplaySurface()->systemW / viewScale) / (float)GetMainDisplaySurface()->targetW;
    #endif
#else//#elif (UNITY_VERSION >= 410)
	scale = ((float)GetMainDisplay()->surface.systemW / viewScale) / (float)GetMainDisplay()->surface.targetW;
#endif
	return scale;
}

extern "C" void PostToServices_SetPopoverTargetRect(float x, float y, float width, float height) {
	float scale = GetUnitySurfaceToUIViewScale();
	PostToServices_rectPopoverTarget = CGRectMake(x*scale, y*scale, width*scale, height*scale);
	PostToServices_bPopoverCenter = false;
}

