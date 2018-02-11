#pragma strict

private var text : String  = "Sample Text";
private var url : String = "https://www.assetstore.unity3d.com/#/content/9277";

private var imageTex2D : Texture2D = null;

private var callbackGameObjectName : String = null;
private var callbackMethodName : String = null;


function Start () {

}

function Update () {

}

function OnGUI () {
    
    // Tweet
    if (GUI.Button(new Rect(0, Screen.height*0.5f, Screen.width*0.5f, Screen.height*0.5f), "Tweet")) {
        #if UNITY_IPHONE
            if (Application.platform == RuntimePlatform.IPhonePlayer) {
                // Tweet
                iOS_PostToServices.Tweet (text, url, imageTex2D, callbackGameObjectName, callbackMethodName);
            }
        #endif
    }

    // Facebook
    if (GUI.Button(new Rect(Screen.width*0.5f, Screen.height*0.5f, Screen.width*0.5f, Screen.height*0.5f), "Post to\nFacebook")) {
        #if UNITY_IPHONE
            if (Application.platform == RuntimePlatform.IPhonePlayer) {
                // Post to Facebook
                iOS_PostToServices.PostToFacebook (text, url, imageTex2D, callbackGameObjectName, callbackMethodName);
            }
        #endif
    }
}
