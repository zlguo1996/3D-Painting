using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;

public class Bluetooth : MonoBehaviour {

	private SerialPort sp = new SerialPort("/dev/cu.PressPen-DevB", 9600);
	private bool mylight = true;

	// Use this for initialization
	void Start () {
		sp.Open();
		sp.ReadTimeout = 1;
	}
	
	// Update is called once per frame
	void Update () {
		if (sp.IsOpen) {
			int pres = sp.ReadByte ();
			Vector3 position = this.transform.position;
			this.GetComponent<Transform> ().position = new Vector3 (position.x, pres / 100.0f, position.z);
		}
	}

	void OpenLight(){
		if (sp.IsOpen) {
			try {
				sp.Write("1");
			}
			catch(System.Exception) {
			}
		}
	}

	void CloseLight(){
		if (sp.IsOpen) {
			try {
				sp.Write("2");
			}
			catch(System.Exception) {
			}
		}
	}

	public void changeLight(){
		if(mylight) OpenLight();
		else CloseLight();
		mylight = !mylight;
	}
}
