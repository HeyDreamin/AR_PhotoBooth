using UnityEngine;
using System.Collections;

public class Demo : MonoBehaviour {

	public GameObject cube;
	
	GameObject objectsParent;		
	const int cubeCount = 15;
	GameObject[] cubes = new GameObject[cubeCount];

	bool pause = false;

	// Use this for initialization
	void Start () {

		// Camera
		var camera = GameObject.FindWithTag("MainCamera");		
		camera.transform.position = new Vector3(0.0f,0.0f,-10.0f);

		// Light
		var light = GameObject.FindWithTag("MainLight").GetComponent<Light>();
		light.range = 20.0f;
		light.gameObject.transform.position = new Vector3(0.0f,0.0f,-5.0f);		// Camera

		// Cubes
		objectsParent = GameObject.Find("Objects");
		Color color1 = new Color(1.0f, 0.0f, 0.0f, 1.0f);
		Color color2 = new Color(0.0f, 0.0f, 1.0f, 1.0f);
		for (int i=0; i<cubeCount; ++i) {
			float r = (float)i/(float)(cubeCount-1);
			cubes[i] = Instantiate(cube) as GameObject;
			cubes[i].SetActive(true);
			cubes[i].transform.parent = objectsParent.transform;
			cubes[i].transform.Translate(0.0f, (float)(cubeCount/2-i)*0.75f, 0.0f);
			cubes[i].transform.Rotate(0.0f, 90.0f+r*180.0f, 0.0f);
			var renderer = cubes[i].GetComponent<MeshRenderer>();
			renderer.material.color = Color.Lerp(color1, color2, r);
		}
		cube.SetActive(false);
	}
	
	public bool Pause (bool pause) {
		this.pause = pause;
		return this.pause;
	}

	void Update () {
		if (pause == false) {
			objectsParent.transform.Rotate(0.0f, 1.0f, 0.0f);
		}
	}
}
