using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PenVisualizer : MonoBehaviour {
	public PressPen press_pen;

    public string model_pose_file_path = "/Resources/parameters/pens/presspen/pen_model_pose.yml";
    private Matrix4x4 pose_mat;

	UnityAction on_detect_pen;

	// Use this for initialization
	void Start () {
		on_detect_pen += change_go_pose;
		press_pen.OnDetectDodeca.AddListener (on_detect_pen);

        model_pose_file_path = Application.dataPath + model_pose_file_path;
        Debug.Assert(press_pen.dodeca_tracker_thread.dodeca_tracker.getPoseFromFile(ref pose_mat, model_pose_file_path)==true);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void change_go_pose(){
        Matrix4x4 pose = press_pen.getFrame ().rt_mat;
        Matrix4x4 new_pose = press_pen.scale_mat * pose * pose_mat * Matrix4x4.Rotate(Quaternion.Euler(0.0f, 180.0f, 0.0f));
        this.gameObject.transform.localPosition = new_pose.ExtractPosition();
        this.gameObject.transform.localRotation = new_pose.ExtractRotation();
        this.gameObject.transform.localScale = new_pose.ExtractScale();
	}
}
