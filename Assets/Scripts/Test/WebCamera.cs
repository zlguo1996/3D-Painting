using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class WebCamera : MonoBehaviour {

	public WebCamTexture webcam;
	public int id;

	[DllImport("DodecaTrackerPlugin", EntryPoint = "processImage")]
	public static extern void processImage(Color32[] raw, int width, int height, int id);

	// Use this for initialization
	void Start () {
		id = 0;

		WebCamDevice[] devices = WebCamTexture.devices;    
		string DeviceName= devices[0].name;
		webcam = new WebCamTexture (DeviceName);
		webcam.Play ();
	}
	
	// Update is called once per frame
	void Update () {
		if (webcam.isPlaying) {
			Color32[] rawImg = webcam.GetPixels32 ();
			System.Array.Reverse (rawImg);
			//processImage (rawImg, webcam.width, webcam.height, id);
			id += 1;
		}
	}
}
