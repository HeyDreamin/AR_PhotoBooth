using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;


public class switchObject : MonoBehaviour,IVirtualButtonEventHandler {
	public GameObject VBButtonObject;
	private int currentObjectNumber = 0;
	private string[] names;
	private GameObject[] objects;
//	private string[] buttonNames;
//	private GameObject[] buttons;

	public string buttonName;




	// Use this for initialization
	public void Start () {

		VBButtonObject.GetComponent<VirtualButtonBehaviour> ().RegisterEventHandler (this);

		// initialize the gun objects name array

		if(buttonName.Equals("GunSwitchButton")) {
			names = new string[]{ "gun1", "gun2", "gun3" };
		}

		else if (buttonName.Equals("CatSwitchButton")) {
			names = new string[]{ "Cat01", "Cat02", "Cat03", "Cat04", "Cat05", "Cat06", "Cat07", "Cat08", "Cat09", "Cat10", "Cat11", "Cat12", "Cat13" };
		}

		else if (buttonName.Equals("BearSwitchButton")) {
			names = new string[]{ "Bear01", "Bear02", "Bear03", "Bear04", "Bear05", "Bear06", "Bear07", "Bear08", "Bear09", "Bear10", "Bear11", "Bear12", "Bear13" };
		}

		else if (buttonName.Equals("BunnySwitchButton")) {
			names = new string[]{ "Bunny01", "Bunny02", "Bunny03", "Bunny04", "Bunny05", "Bunny06", "Bunny07", "Bunny08", "Bunny09", "Bunny10", "Bunny11", "Bunny12", "Bunny13" };
		}

		else if (buttonName.Equals("GlassesSwitchButton")) {
			names = new string[]{ "GlassesA2", "GlassesA3", "GlassesA4" };
		}

		else if (buttonName.Equals("AubumaskSwitchButton")) {
			names = new string[]{ "Anbumask_a", "Anbumask_b", "Anbumask_c" };
		}



		if (names != null) {
			objects = new GameObject[names.Length];

			//to hide all the objects except the first one
			for (int i = 0; i < names.Length; i++) {
				objects [i] = GameObject.Find (names [i]);
				if (i != 0)
					objects [i].SetActive (false);

			}
		}








	}
	
	public void OnButtonPressed(VirtualButtonAbstractBehaviour vb) {
		Debug.Log("Pressed");

		//hide the current object
		objects[currentObjectNumber].SetActive (false);

		// increment the currentObjectNumber
		if (currentObjectNumber == names.Length - 1)
			currentObjectNumber = 0;
		else
			currentObjectNumber++;

		//show the next gun
		objects[currentObjectNumber].SetActive (true);

	}

	public void OnButtonReleased(VirtualButtonAbstractBehaviour vb) {
		Debug.Log("Released");

	}



}
