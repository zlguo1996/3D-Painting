using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public struct FrameInfo{
	public Vector3 rvec;
	public Vector3 tvec;
	public int pressure;
	public uint index;

	public FrameInfo(Vector3 my_rvec, Vector3 my_tvec, int my_pressure, uint my_index){
		this.rvec = my_rvec;
		this.tvec = my_tvec;
		this.pressure = my_pressure;
		this.index = my_index;
	}
}
public struct PoseInfo{
	public Vector3 rvec;
	public Vector3 tvec;
	public uint index;

	public PoseInfo(Vector3 my_rvec, Vector3 my_tvec, uint my_index){
		this.rvec = my_rvec;
		this.tvec = my_tvec;
		this.index = my_index;
	}
}
public struct PositionInfo{
	public Vector3 tvec;
	public uint index;

	public PositionInfo(Vector3 my_tvec, uint my_index){
		this.tvec = my_tvec;
		this.index = my_index;
	}
}

public class PressPen : MonoBehaviour {
	public PressMeasure press_measure;
	public DodecaTrackerThread dodeca_tracker_thread;

	public UnityEvent OnDetectCamera = new UnityEvent();
	public UnityEvent OnDetectDodeca = new UnityEvent();
	public UnityEvent OnDetectPoint = new UnityEvent ();

	[HideInInspector]
	public List<PoseInfo> frames_cam = new List<PoseInfo> ();
	[HideInInspector]
	public List<FrameInfo> frames_dodeca = new List<FrameInfo> ();
	[HideInInspector]
	public List<PositionInfo> frames_point = new List<PositionInfo> ();
	[HideInInspector]
	public uint cur_frame_idx = 0;

	private UnityAction<Vector3, Vector3, uint> cam_detected_action;
	private UnityAction<Vector3, Vector3, uint> dodeca_detected_action;
	private UnityAction<Vector3, uint> point_detected_action;

	// Use this for initialization
	void Start () {
		cam_detected_action += onDetectCamera;
		dodeca_tracker_thread.on_detect_cam.AddListener (cam_detected_action);
		dodeca_detected_action += onDetectDodeca;
		dodeca_tracker_thread.on_detect_dodeca.AddListener (dodeca_detected_action);
		point_detected_action += onDetectPoint;
		dodeca_tracker_thread.on_detect_point.AddListener(point_detected_action);
	}
	
	// Update is called once per frame
	void Update () {
	}

	// -------- 获取变量 ----------
	public PoseInfo getCameraPose(){
		return frames_cam [frames_cam.Count - 1];
	}

	public FrameInfo getFrame(){
		return frames_dodeca [frames_dodeca.Count - 1];
	}
	public PositionInfo getPointPosition(){
		return frames_point [frames_dodeca.Count - 1];
	}
//	public float getPressure(){
//		Debug.Assert (frames.Count != 0);
//		return frames;
//	}
//
//	public Vector3 getTranslation(){
//		return translation;
//	}
//
//	public Vector3 getRotation(){
//		return rotation;
//	}

	// ---------- 回调函数 -------------
	private void onDetectCamera(Vector3 rvec, Vector3 tvec, uint frame_idx){
		frames_cam.Add(new PoseInfo(rvec, tvec, frame_idx));

		OnDetectCamera.Invoke ();
	}
	private void onDetectDodeca(Vector3 rvec, Vector3 tvec, uint frame_idx){
		int pressure = press_measure.pressure;
		frames_dodeca.Add(new FrameInfo(rvec, tvec, pressure, frame_idx));

		OnDetectDodeca.Invoke ();
	}
	private void onDetectPoint(Vector3 tvec, uint frame_idx){
		frames_point.Add (new PositionInfo (tvec, frame_idx));

		OnDetectPoint.Invoke ();
	}
}
