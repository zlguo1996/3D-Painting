using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Test : MonoBehaviour {
	public PressPen press_pen;

	public GameObject go;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void on_detect_pen(){
		FrameInfo frame_info = press_pen.getFrame();
		//Debug.Log (frame_info.tvec);
		Vector3 tvec = frame_info.rt_mat.ExtractPosition();
		tvec.Scale (new Vector3 (20.0f, 20.0f, 20.0f));
		go.transform.position = tvec;
	}
}
