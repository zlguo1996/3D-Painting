using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PenVisualizer : MonoBehaviour {
	public PressPen press_pen;
	public float scale_parameter = 20.0f;

	UnityAction on_detect_pen;

	// Use this for initialization
	void Start () {
		on_detect_pen += change_go_pose;
		press_pen.OnDetectDodeca.AddListener (on_detect_pen);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void change_go_pose(){
		FrameInfo fi = press_pen.getFrame ();
		this.gameObject.transform.localPosition = fi.rt_mat.ExtractPosition().ScaleTo(scale_parameter);
		this.gameObject.transform.localRotation = fi.rt_mat.ExtractRotation();
	}
}
