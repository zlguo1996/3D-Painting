using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PentipVisualizer : MonoBehaviour {
    public PressPen press_pen;

	// Use this for initialization
	void Start () {
        press_pen.OnDetectDodeca.AddListener(change_go_pose);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void change_go_pose()
    {
        Matrix4x4 pose = press_pen.getFrame().rt_mat;
        Matrix4x4 new_pose = press_pen.scale_mat * pose * press_pen.dodeca_tracker_thread.dodeca_tracker.pen_tip_pose;
        this.gameObject.transform.localPosition = new_pose.ExtractPosition();
        this.gameObject.transform.localRotation = new_pose.ExtractRotation();
        //this.gameObject.transform.localScale = new_pose.ExtractScale();
    }
}
