using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

// 用于获得通过蓝牙获得的压力数据
public class PressMeasure : MonoBehaviour {

	wrmhl myDevice = new wrmhl(); // wrmhl is the bridge beetwen computer and hardware.

	[Tooltip("SerialPort of your device.")]
	public string PortName = "/dev/cu.PressPen-DevB";

	[Tooltip("Baudrate")]
	public int BaudRate = 9600;

	[Tooltip("Timeout")]
	public int ReadTimeout = 1000;

	[Tooltip("QueueLenght")]
	public int QueueLenght = 1;

	[Tooltip("Listen rate")]
	public int WaitTime = 10;	// 监听时间间隔(ms)

	[HideInInspector]
	public int pressure = 0;	//存储该帧的压力

	private bool isListening; //是否处于监听压力状态

	// Use this for initialization
	void Start () {
		myDevice.set (PortName, BaudRate, ReadTimeout, QueueLenght); // This method set the communication with the following vars;
		//                              Serial Port, Baud Rates, Read Timeout and QueueLenght.
		// myDevice.connect (); // This method open the Serial communication with the vars previously given.
	}
	
	// Update is called once per frame
	void Update () {
		//testUpdate ();
	}
	private void testUpdate(){
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

	// 监听蓝牙数据
	public bool startListening(){
		isListening = true;
		StartCoroutine (getPressureLoop());

		return true;
	}
	public bool stopListening(){
		isListening = false;

		return true;
	}

	// 连接蓝牙
	public void connectBT(){
		myDevice.connect();
	}
	public void closeBT(){
		myDevice.close ();
	}

	// 获得实时的压力数据
	bool getPressure(ref int p){
		string pres = myDevice.readQueue ();
		try {
			p = System.Convert.ToInt32 (pres);
		} catch (System.Exception ex) {
			return false;
		}

		return true;
	}

	IEnumerator getPressureLoop(){
		while (isListening) {
			int p=0;
			bool success = getPressure (ref p);
			if (success) {
				pressure = p;
			}
			yield return new WaitForSeconds (0.001f * WaitTime);
		}
	}
}
