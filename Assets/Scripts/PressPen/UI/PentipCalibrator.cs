using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PentipCalibrator : MonoBehaviour {
	public PressPen press_pen;

	public GameObject pivot;
	public GameObject center;
	public GameObject pentip;
    public GameObject pen_model;

    public string model_pose_file_path = "/Resources/parameters/pens/presspen/pen_model_pose.yml";

	// Use this for initialization
	void Start () {
        model_pose_file_path = Application.dataPath + model_pose_file_path;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void VisualizePoints(){
		Debug.Log ("VP");

		pivot.transform.parent = this.transform;
		center.transform.parent = this.transform;
		pentip.transform.parent = this.transform;

		pivot.transform.position = Vector3.zero;
		center.transform.position = press_pen.dodeca_tracker_thread.dodeca_tracker.pen_dodeca_center_pose.ExtractPosition ();
		pentip.transform.position = press_pen.dodeca_tracker_thread.dodeca_tracker.pen_tip_pose.ExtractPosition ();
		pivot.transform.rotation = Quaternion.Euler(Vector3.zero);
		center.transform.rotation = press_pen.dodeca_tracker_thread.dodeca_tracker.pen_dodeca_center_pose.ExtractRotation ();
		pentip.transform.rotation = press_pen.dodeca_tracker_thread.dodeca_tracker.pen_tip_pose.ExtractRotation ();

        Matrix4x4 model_pose = new Matrix4x4();
        if(press_pen.dodeca_tracker_thread.dodeca_tracker.getPoseFromFile(ref model_pose, model_pose_file_path)){
            pen_model.transform.position = model_pose.ExtractPosition();
            pen_model.transform.rotation = model_pose.ExtractRotation();
            pen_model.transform.localScale = model_pose.ExtractScale();
        }
	}

	public void ApplyDefaultRotation(){
		center.transform.LookAt (pentip.transform.position, pivot.transform.position - center.transform.position);
		pentip.transform.LookAt (center.transform.position, pivot.transform.position - pentip.transform.position);
	}

	public void SaveCenterPose(){
		SavePoseToFile (center, Application.dataPath + "/Resources/parameters/pens/presspen/dodeca_center_calibration.yml");
	}
	public void SavePentipPose(){
		SavePoseToFile (pentip, Application.dataPath + "/Resources/parameters/pens/presspen/pentip_calibration.yml");
	}
    public void SavePenModelPose(){
        Matrix4x4 pose = Matrix4x4.TRS(pen_model.transform.position, pen_model.transform.rotation, pen_model.transform.lossyScale);
        press_pen.dodeca_tracker_thread.dodeca_tracker.savePenTipPose(pose, model_pose_file_path);
    }
	public void SavePoseToFile(GameObject go, string file_path){
		Debug.Assert (System.IO.File.Exists (file_path));
		Matrix4x4 pose = Matrix4x4.TRS (go.transform.position, go.transform.rotation, Vector3.one);
		press_pen.dodeca_tracker_thread.dodeca_tracker.savePenTipPose (pose, file_path);
	}
}
