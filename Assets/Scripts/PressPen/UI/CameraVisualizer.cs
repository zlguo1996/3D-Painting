using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CameraVisualizer : MonoBehaviour {
	public PressPen press_pen;

	UnityAction on_detect_camera;

	// Use this for initialization
	void Start () {
		on_detect_camera += change_go_pose;
		press_pen.OnDetectCamera.AddListener (on_detect_camera);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void change_go_pose(){
		PoseInfo pi = press_pen.getCameraPose ();
        this.gameObject.transform.localPosition = pi.rt_mat.ExtractPosition().ScaleTo(press_pen.scale_parameter);
		this.gameObject.transform.localRotation = pi.rt_mat.ExtractRotation();
	}
}
