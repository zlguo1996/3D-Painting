using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 用于获得通过蓝牙获得的压力数据
public class PressMeasure : MonoBehaviour {

	wrmhl myDevice = new wrmhl(); // wrmhl is the bridge beetwen computer and hardware.

	[Tooltip("SerialPort of your device.")]
	public string portName = "/dev/cu.PressPen-DevB";

	[Tooltip("Baudrate")]
	public int baudRate = 9600;


	[Tooltip("Timeout")]
	public int ReadTimeout = 1000;

	[Tooltip("QueueLenght")]
	public int QueueLenght = 1;

	private int pressure;	//存储该帧的压力

	// Use this for initialization
	void Start () {
		myDevice.set (portName, baudRate, ReadTimeout, QueueLenght); // This method set the communication with the following vars;
		//                              Serial Port, Baud Rates, Read Timeout and QueueLenght.
		myDevice.connect (); // This method open the Serial communication with the vars previously given.
	}
	
	// Update is called once per frame
	void Update () {
		// test
		string pres = myDevice.readQueue ();
		if (pres!=null) {
			pressure = System.Convert.ToInt32 (pres);

			Vector3 position = this.transform.position;
			this.GetComponent<Transform> ().position = new Vector3 (position.x, pressure / 100.0f, position.z);
		}
	}

	void OnApplicationQuit() { // close the Thread and Serial Port
		myDevice.close();
	}

	// 获得实时的压力数据
	int getPressure(){
		string pres = myDevice.readQueue ();
		pressure = System.Convert.ToInt32 (pres);
		return pressure;
	}
}
