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

    public float scale_parameter{
        get { return _scale_parameter;}
        set {
            _scale_parameter = value;
            scale_mat = Matrix4x4.Scale(new Vector3(value, value, value));
        }
    }
    private float _scale_parameter = 20.0f;
    public Matrix4x4 scale_mat;

	public UnityEvent OnDetectCamera = new UnityEvent();
	public UnityEvent OnDetectDodeca = new UnityEvent();

	[HideInInspector]
	public List<PoseInfo> frames_cam = new List<PoseInfo> ();
	[HideInInspector]
	public List<FrameInfo> frames_dodeca = new List<FrameInfo> ();
	[HideInInspector]
	public uint cur_frame_idx = 0;

	private UnityAction<Matrix4x4, uint> cam_detected_action;
	private UnityAction<Matrix4x4, uint> dodeca_detected_action;

	// Use this for initialization
	void Start () {
		cam_detected_action += onDetectCamera;
		dodeca_tracker_thread.on_detect_cam.AddListener (cam_detected_action);
		dodeca_detected_action += onDetectDodeca;
		dodeca_tracker_thread.on_detect_dodeca.AddListener (dodeca_detected_action);

        scale_mat = Matrix4x4.Scale(new Vector3(scale_parameter, scale_parameter, scale_parameter));
	}
	
	// Update is called once per frame
	void Update () {
	}

	// -------- 获取变量 ----------
	public PoseInfo getCameraPose(){
		return frames_cam [frames_cam.Count - 1];
	}
    public PoseInfo getPrevCameraPose(int offset){              // 0开始
        return frames_cam[frames_cam.Count - 1 - offset];
    }

	public FrameInfo getFrame(){
		return frames_dodeca [frames_dodeca.Count - 1];
	}
    public FrameInfo getPrevFrame(int offset){                  // 0开始
        return frames_dodeca[frames_dodeca.Count - 1 - offset];
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

// ------------ get rotation position scale from a matrix -------------
public static class MatrixExtensions
{
	public static Quaternion ExtractRotation(this Matrix4x4 matrix)
	{
		Vector3 forward;
		forward.x = matrix.m02;
		forward.y = matrix.m12;
		forward.z = matrix.m22;

		Vector3 upwards;
		upwards.x = matrix.m01;
		upwards.y = matrix.m11;
		upwards.z = matrix.m21;

		return Quaternion.LookRotation(forward, upwards);
	}

	public static Vector3 ExtractPosition(this Matrix4x4 matrix)
	{
		Vector3 position;
		position.x = matrix.m03;
		position.y = matrix.m13;
		position.z = matrix.m23;
		return position;
	}

	public static Vector3 ExtractScale(this Matrix4x4 matrix)
	{
		Vector3 scale;
		scale.x = new Vector4(matrix.m00, matrix.m10, matrix.m20, matrix.m30).magnitude;
		scale.y = new Vector4(matrix.m01, matrix.m11, matrix.m21, matrix.m31).magnitude;
		scale.z = new Vector4(matrix.m02, matrix.m12, matrix.m22, matrix.m32).magnitude;
		return scale;
	}

	public static Vector3 ScaleTo(this Vector3 position, float scale){
		position.Scale (new Vector3 (scale, scale, scale));
		return position;
	}
}
