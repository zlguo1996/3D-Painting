using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public struct FrameInfo{
	public Matrix4x4 rt_mat;
	public int pressure;
	public uint index;

	public FrameInfo(Matrix4x4 my_rt_mat, int my_pressure, uint my_index){
		this.rt_mat = my_rt_mat;
		this.pressure = my_pressure;
		this.index = my_index;
	}
}
public struct PoseInfo{
	public Matrix4x4 rt_mat;
	public uint index;

	public PoseInfo(Matrix4x4 my_rt_mat, uint my_index){
		this.rt_mat = my_rt_mat;
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
	private void onDetectCamera(Matrix4x4 rt_mat, uint frame_idx){
		frames_cam.Add(new PoseInfo(rt_mat, frame_idx));

		OnDetectCamera.Invoke ();
	}
	private void onDetectDodeca(Matrix4x4 rt_mat, uint frame_idx){
		int pressure = press_measure.pressure;
		frames_dodeca.Add(new FrameInfo(rt_mat, pressure, frame_idx));

		OnDetectDodeca.Invoke ();
	}
}
