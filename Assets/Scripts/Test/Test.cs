using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Test : MonoBehaviour {
	public PressPen press_pen;

	private Thread t;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void on_detect_pen(){
		FrameInfo frame_info = press_pen.getFrame();
		Debug.Log (frame_info.tvec);
	}

	public void start_tracking(){
		press_pen.dodeca_tracker_thread.startTracking ();
	}

	public void stop_tracking(){
		press_pen.dodeca_tracker_thread.stopTracking ();
	}
}
