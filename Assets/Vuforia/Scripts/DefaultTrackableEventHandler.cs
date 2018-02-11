/*==============================================================================
Copyright (c) 2010-2014 Qualcomm Connected Experiences, Inc.
All Rights Reserved.
Confidential and Proprietary - Protected under copyright and other laws.
==============================================================================*/

using UnityEngine;
using System.Collections;
//using System.IO;
//using UnityEditor;


namespace Vuforia
{
	/// <summary>
	/// A custom handler that implements the ITrackableEventHandler interface.
	/// </summary>
	public class DefaultTrackableEventHandler : MonoBehaviour,
	ITrackableEventHandler
	{
		#region PRIVATE_MEMBER_VARIABLES

		private TrackableBehaviour mTrackableBehaviour;

		#endregion // PRIVATE_MEMBER_VARIABLES


		private GameObject[] frames = new GameObject[3]; 
		public AudioClip cameraShotSound;
		public AudioSource mySource;
		private bool showReady = false;
		private bool showOne = false;
		private bool showTwo = false;
		private bool showThree = false;
		private bool showFour = false;
		private bool showFive = false;


	

		#region UNTIY_MONOBEHAVIOUR_METHODS

		void Start()
		{
			mTrackableBehaviour = GetComponent<TrackableBehaviour>();
			if (mTrackableBehaviour)
			{
				mTrackableBehaviour.RegisterTrackableEventHandler(this);
			}


	

			for (int i = 0; i < frames.Length; i++) {
				frames [i] = GameObject.Find ("ARCamera/Camera/frame" + (i + 1));
			

				if (i != 0) {
					frames[i].SetActive (false);
				}
			}

//			mySource = GetComponent<AudioSource>();;
//			mySource.clip = cameraShotSound;


		}

		#endregion // UNTIY_MONOBEHAVIOUR_METHODS



		#region PUBLIC_METHODS

		/// <summary>
		/// Implementation of the ITrackableEventHandler function called when the
		/// tracking state changes.
		/// </summary>
		public void OnTrackableStateChanged(
			TrackableBehaviour.Status previousStatus,
			TrackableBehaviour.Status newStatus)
		{
			if (newStatus == TrackableBehaviour.Status.DETECTED ||
				newStatus == TrackableBehaviour.Status.TRACKED ||
				newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
			{
				OnTrackingFound();
			}
			else
			{
				OnTrackingLost();
			}
		}

		#endregion // PUBLIC_METHODS







		#region PRIVATE_METHODS
		public ParticleSystem snow_effect;
		private int snow_flag = 0;

		private void OnTrackingFound()
		{
			Renderer[] rendererComponents = GetComponentsInChildren<Renderer>(true);
			Collider[] colliderComponents = GetComponentsInChildren<Collider>(true);

			// Enable rendering:
			foreach (Renderer component in rendererComponents)
			{
				component.enabled = true;
			}

			// Enable colliders:
			foreach (Collider component in colliderComponents)
			{
				component.enabled = true;
			}

			UnityEngine.Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " found");

			if (mTrackableBehaviour.TrackableName == "snow")
			{
				//snow effect
				if (snow_flag == 0)
				{
					snow_effect.Play();
					snow_flag = 1;
				}
				else
				{
					snow_effect.Clear();
					snow_effect.Stop();
					snow_flag = 0;
				}
			}

			//change frames
//			if (mTrackableBehaviour.TrackableName == "passport")
//			{
//
//
//				for (int i = 0; i < frames.Length; i++) {
//					if (i != 1) {
//						frames [i].SetActive (false);
//					}
//				}
//
//				frames [1].SetActive (true);
//
//
//			}



			if (mTrackableBehaviour.TrackableName == "logo1")
			{
				
				for (int i = 0; i < frames.Length; i++) {
					if (i != 1) {
						frames [i].SetActive (false);
					}
				}

				frames [1].SetActive (true);

			}


			if (mTrackableBehaviour.TrackableName == "logo2")
			{

				for (int i = 0; i < frames.Length; i++) {
					if (i != 2) {
						frames [i].SetActive (false);
					}
				}

				frames [2].SetActive (true);

			}


			if (mTrackableBehaviour.TrackableName == "shot") 
			{
				//Screenshot
				StartCoroutine(Screenshot());

			}
		

		}

		string text = "";
		string url = "http://www.photonics2017.org/index.php";
		Texture2D imageTex2D = null;

		IEnumerator Screenshot()
		{
//			showReady = true;
//			yield return new WaitForSeconds(0.5f);
//			showReady = false;
			showFive = true;
			yield return new WaitForSeconds(1);
			showFive = false;
			showFour = true;
			yield return new WaitForSeconds(1);
			showFour = false;
			showThree = true;
			yield return new WaitForSeconds(1);
			showThree = false;
			showTwo = true;
			yield return new WaitForSeconds(1);
			showTwo = false;
			showOne = true;
			yield return new WaitForSeconds(1);
			showOne = false;
			yield return new WaitForSeconds(1);
			mySource.PlayOneShot(cameraShotSound,1F);

//			mySource.Play();
//			yield return new WaitForSeconds(0.1F);
			System.Threading.Thread.Sleep(700);

			ScreenshotManager.SaveScreenshot("MyScreenshot", "ScreenshotApp", "jpeg");
			imageTex2D = ScreenCapture.Capture();
			iOS_PostToServices.PostToFacebook (text, url, imageTex2D);
//			Application.CaptureScreenshot("/var/mobile/Media/DCIM/screenshot.png");


		}

		public Texture ready;
		public Texture one;
		public Texture two;
		public Texture three;
		public Texture four;
		public Texture five;



		void OnGUI()
		{
			int width = 900;
			int height = 400; 

			if (showReady)
			{
				GUI.DrawTexture(new Rect((Screen.width/2) - (width/2), (Screen.height/2) - (height/2), width, height), ready, ScaleMode.ScaleToFit,true);
			}
			if (showOne) {
				GUI.DrawTexture(new Rect((Screen.width/2) - (width/2), (Screen.height/2) - (height/2), width, height), one, ScaleMode.ScaleToFit,true);
			}

			if (showTwo) {
				GUI.DrawTexture(new Rect((Screen.width/2) - (width/2), (Screen.height/2) - (height/2), width, height), two, ScaleMode.ScaleToFit,true);
			}

			if (showThree) {
				GUI.DrawTexture(new Rect((Screen.width/2) - (width/2), (Screen.height/2) - (height/2), width, height), three, ScaleMode.ScaleToFit,true);
			}

			if (showFour) {
				GUI.DrawTexture(new Rect((Screen.width/2) - (width/2), (Screen.height/2) - (height/2), width, height), four, ScaleMode.ScaleToFit,true);
			}


			if (showFive) {
				GUI.DrawTexture(new Rect((Screen.width/2) - (width/2), (Screen.height/2) - (height/2), width, height), five, ScaleMode.ScaleToFit,true);
			}
				
		}

		private void OnTrackingLost()
		{
			Renderer[] rendererComponents = GetComponentsInChildren<Renderer>(true);
			Collider[] colliderComponents = GetComponentsInChildren<Collider>(true);

			// Disable rendering:
			foreach (Renderer component in rendererComponents)
			{
				component.enabled = false;
			}

			// Disable colliders:
			foreach (Collider component in colliderComponents)
			{
				component.enabled = false;
			}

			Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " lost");
		}

		#endregion // PRIVATE_METHODS
	}
}

