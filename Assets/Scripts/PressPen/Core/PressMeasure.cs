﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using UnityEngine.Events;

// 用于获得通过蓝牙获得的压力数据
public class PressMeasure : MonoBehaviour {

	wrmhl myDevice = new wrmhl(); // wrmhl is the bridge beetwen computer and hardware.

	[Tooltip("SerialPort of your device.")]
    public string PortName = "/dev/cu.PressPen1-SPPDev";

	[Tooltip("Baudrate")]
	public int BaudRate = 9600;

	[Tooltip("Timeout")]
	public int ReadTimeout = 1000;

	[Tooltip("QueueLenght")]
	public int QueueLenght = 1;

	[Tooltip("Listen rate")]
	public int WaitTime = 1;	// 监听时间间隔(ms)

	[HideInInspector]
	public int pressure = 0;	//存储该帧的压力

	private bool isListening = false; //是否处于监听压力状态

	public UnityEvent OnDetectPressure = new UnityEvent ();

	// Use this for initialization
	void Start () {
		myDevice.set (PortName, BaudRate, ReadTimeout, QueueLenght); // This method set the communication with the following vars;
		//                              Serial Port, Baud Rates, Read Timeout and QueueLenght.

		//connectBT ();
		//startListening ();
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

	// 监听蓝牙数据
	public void startListening(){
		isListening = true;
		StartCoroutine (getPressureLoop());
	}
	public void stopListening(){
		isListening = false;
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
			if (p == 0)
				continue;
			if (success) {
				pressure = p;
				OnDetectPressure.Invoke ();
				yield return new WaitForSeconds (0.001f * WaitTime);
			}

		}
	}

	void OnApplicationQuit(){
		if (isListening) {
			stopListening ();
		}
		closeBT ();
	}
}
