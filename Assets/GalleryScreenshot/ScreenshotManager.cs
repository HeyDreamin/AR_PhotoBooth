#pragma warning disable 0168 // variable declared but not used.
#pragma warning disable 0219 // variable assigned but not used.

using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Runtime.InteropServices;

public class ScreenshotManager : MonoBehaviour {

	enum ImageType { IMAGE, SCREENSHOT };
	enum SaveStatus { NOTSAVED, SAVED, DENIED, TIMEOUT };

	public static event Action<Texture2D> OnScreenshotTaken;
	public static event Action<string> OnScreenshotSaved;
	public static event Action<string> OnImageSaved;

	static ScreenshotManager instance = null;
	static GameObject go; 
	
	#if UNITY_IOS
	
	[DllImport("__Internal")]
    private static extern int saveToGallery( string path );
	
	#elif UNITY_ANDROID
	
	static AndroidJavaClass obj;
	
	#endif
	
	
	//=============================================================================
	// Init singleton
	//=============================================================================
	
	public static ScreenshotManager Instance 
	{
		get {
			if(instance == null)
			{
				go = new GameObject();
				go.name = "ScreenshotManager";
				instance = go.AddComponent<ScreenshotManager>();
				
				#if UNITY_ANDROID
				
				if(Application.platform == RuntimePlatform.Android)
					obj = new AndroidJavaClass("com.secondfury.galleryscreenshot.MainActivity");
				
				#endif
			}
			
			return instance; 
		}
	}
	
	void Awake() 
	{
		if (instance != null && instance != this) 
		{
			Destroy(this.gameObject);
		}
	}
	
	
	//=============================================================================
	// Grab and save screenshot
	//=============================================================================
	
	public static void SaveScreenshot(string fileName, string albumName = "MyScreenshots", string fileType = "jpg", Rect screenArea = default(Rect))
	{
		Debug.Log("Save screenshot to gallery " + fileName);
		
		if(screenArea == default(Rect))
			screenArea = new Rect(0, 0, Screen.width, Screen.height);
		
		Instance.StartCoroutine(Instance.GrabScreenshot(fileName, albumName, fileType, screenArea));
	}
	
	IEnumerator GrabScreenshot(string fileName, string albumName, string fileType, Rect screenArea)
	{
		yield return new WaitForEndOfFrame();
		
		Texture2D texture = new Texture2D ((int)screenArea.width, (int)screenArea.height, TextureFormat.RGB24, false);
		texture.ReadPixels (screenArea, 0, 0);
		texture.Apply ();

		byte[] bytes;
		string fileExt;
		
		if(fileType == "png")
		{
			bytes = texture.EncodeToPNG();
			fileExt = ".png";
		}
		else
		{
			bytes = texture.EncodeToJPG();
			fileExt = ".jpg";
		}

		if(OnScreenshotTaken != null) 
			OnScreenshotTaken(texture);
		else
			Destroy (texture);
		
		string date = System.DateTime.Now.ToString("hh-mm-ss_dd-MM-yy");
		string screenshotFilename = fileName + "_" + date + fileExt;
		string path = Application.persistentDataPath + "/" + screenshotFilename;
		
		#if UNITY_ANDROID
		
		if(Application.platform == RuntimePlatform.Android) 
		{
			string androidPath = Path.Combine(albumName, screenshotFilename);
			path = Path.Combine(Application.persistentDataPath, androidPath);
			string pathonly = Path.GetDirectoryName(path);
			Directory.CreateDirectory(pathonly);
		}
		
		#endif
		
		Instance.StartCoroutine(Instance.Save(bytes, fileName, path, ImageType.SCREENSHOT));
	}
	
	
	//=============================================================================
	// Save texture
	//=============================================================================
	
	public static void SaveImage(Texture2D texture, string fileName, string albumName = "MyImages", string fileType = "jpg")
	{
		Debug.Log("Save image to gallery " + fileName);
		
		Instance.Awake();
		
		byte[] bytes;
		string fileExt;
		
		if(fileType == "png")
		{
			bytes = texture.EncodeToPNG();
			fileExt = ".png";
		}
		else
		{
			bytes = texture.EncodeToJPG();
			fileExt = ".jpg";
		}
		
		string path = Application.persistentDataPath + "/" + fileName + fileExt;

		#if UNITY_ANDROID
		
		if(Application.platform == RuntimePlatform.Android) 
		{
			string androidPath = Path.Combine(albumName, fileName + fileExt);
			path = Path.Combine(Application.persistentDataPath, androidPath);
			string pathonly = Path.GetDirectoryName(path);
			Directory.CreateDirectory(pathonly);
		}
		
		#endif
		
		Instance.StartCoroutine(Instance.Save(bytes, fileName, path, ImageType.IMAGE));
	}
	
	
	IEnumerator Save(byte[] bytes, string fileName, string path, ImageType imageType)
	{
		int count = 0;
		SaveStatus saved = SaveStatus.NOTSAVED;
		
		#if UNITY_IOS
		
		if(Application.platform == RuntimePlatform.IPhonePlayer) 
		{
			System.IO.File.WriteAllBytes(path, bytes);
			
			while(saved == SaveStatus.NOTSAVED)
			{
				count++;
				if(count > 30) 
					saved = SaveStatus.TIMEOUT;
				else
					saved = (SaveStatus)saveToGallery(path);
			
				yield return Instance.StartCoroutine(Instance.Wait(.5f));
			}
			
			UnityEngine.iOS.Device.SetNoBackupFlag(path);
		}
		
		
		#elif UNITY_ANDROID	
		
		if(Application.platform == RuntimePlatform.Android) 
		{
			System.IO.File.WriteAllBytes(path, bytes);
			
			while(saved == SaveStatus.NOTSAVED) 
			{
				count++;
				if(count > 30) 
					saved = SaveStatus.TIMEOUT;
				else
					saved = (SaveStatus)obj.CallStatic<int>("addImageToGallery", path);
				
				yield return Instance.StartCoroutine(Instance.Wait(.5f));
			}
		}
		
		
		#else
		
		yield return null;
			
		Debug.Log("Gallery Manager: Save file only available in iOS/Android modes");
			
		saved = SaveStatus.SAVED;
		
		#endif

		switch(saved)
		{
			case SaveStatus.DENIED:
				path = "DENIED";
				break;

			case SaveStatus.TIMEOUT:
				path = "TIMEOUT";
				break;
		}
		
		switch(imageType)
		{
			case ImageType.IMAGE:
				if(OnImageSaved != null) 
					OnImageSaved(path);
				break;
				
			case ImageType.SCREENSHOT:
				if(OnScreenshotSaved != null) 
					OnScreenshotSaved(path);
				break;
		}
	}
	
	
	IEnumerator Wait(float delay)
	{
		float pauseTarget = Time.realtimeSinceStartup + delay;
		
		while(Time.realtimeSinceStartup < pauseTarget)
		{
			yield return null;	
		}
	}
}