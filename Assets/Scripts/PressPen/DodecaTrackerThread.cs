using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Threading;

public class PoseDetectedEvent: UnityEvent<Vector3, Vector3, uint>{}
public class PointDetectedEvent: UnityEvent<Vector3, uint>{}

public class DodecaTrackerThread : MonoBehaviour {
	public DodecaTracker dodeca_tracker;
	public PoseDetectedEvent on_detect_dodeca = new PoseDetectedEvent();
	public PointDetectedEvent on_detect_point = new PointDetectedEvent();
	public PoseDetectedEvent on_detect_cam = new PoseDetectedEvent();

	// dodeca position
	[HideInInspector]
	public Vector3 cur_rotation;
	[HideInInspector]
	public Vector3 cur_translation;
	[HideInInspector]
	public Vector3 cur_point_position;

	// camera position
	[HideInInspector]
	public Vector3 cam_rotation;
	[HideInInspector]
	public Vector3 cam_translation;

	// frame idx
	[HideInInspector]
	public uint cur_frame_idx = 0;

	private Thread track_thread;
	private Thread calib_thread; 
	private List<PoseInfo> track_poses_buffer = new List<PoseInfo>();
	private List<PositionInfo> track_positions_buffer = new List<PositionInfo> ();
	private List<PoseInfo> calib_poses_buffer = new List<PoseInfo>();

	// Use this for initialization
	void Start () {
		track_thread = new Thread (track);
		calib_thread = new Thread (calib);
	}
	
	// Update is called once per frame
	void Update () {
		// 清空缓冲区
		foreach (PoseInfo pose in track_poses_buffer) {
			on_detect_dodeca.Invoke (pose.rvec, pose.tvec, pose.index);
		}
		track_poses_buffer.Clear ();
		foreach (PositionInfo position in track_positions_buffer) {
			on_detect_point.Invoke (position.tvec, position.index);
		}
		track_positions_buffer.Clear ();
		foreach (PoseInfo pose in calib_poses_buffer) {
			on_detect_cam.Invoke (pose.rvec, pose.tvec, pose.index);
		}
		calib_poses_buffer.Clear ();
	}
		
	public IEnumerator reset(){
		// 关闭线程
		dodeca_tracker.stat = DodecaTracker.STATUS.NONE;
		yield return new WaitUntil (() => track_thread.IsAlive==false);
		yield return new WaitUntil (() => calib_thread.IsAlive == false);

		// 重置追踪器
		dodeca_tracker.reset();
	}


	// ---------- tracking -------------
	public bool startTracking(){
		Debug.Assert (dodeca_tracker.stat == DodecaTracker.STATUS.NONE);
		Debug.Assert (dodeca_tracker.isInit);
		dodeca_tracker.stat = DodecaTracker.STATUS.TRACK;

		track_thread.Start ();

		return true;
	}

	public bool stopTracking(){
		Debug.Assert (dodeca_tracker.stat == DodecaTracker.STATUS.TRACK);
		dodeca_tracker.stat = DodecaTracker.STATUS.NONE;

		return true;
	}

	void track(){
		while (dodeca_tracker.stat == DodecaTracker.STATUS.TRACK) {
			if (dodeca_tracker.grab()) {
				if (dodeca_tracker.detect()) {
					dodeca_tracker.getPose (ref cur_rotation, ref cur_translation);
					dodeca_tracker.getPenTipPosition (ref cur_point_position);

					track_poses_buffer.Add (new PoseInfo (cur_rotation, cur_translation, cur_frame_idx));
					track_positions_buffer.Add (new PositionInfo (cur_point_position, cur_frame_idx));
				}
				cur_frame_idx++;
			}
		}
	}

	// -----------calibration-----------
	public bool startCalibration(){
		Debug.Assert (dodeca_tracker.stat == DodecaTracker.STATUS.NONE);
		Debug.Assert (dodeca_tracker.isInit);
		dodeca_tracker.stat = DodecaTracker.STATUS.CALIB;
		 
		calib_thread.Start ();

		return true;
	}

	public bool stopCalibration(){
		Debug.Assert (dodeca_tracker.stat == DodecaTracker.STATUS.CALIB);
		dodeca_tracker.stat = DodecaTracker.STATUS.NONE;

		return true;
	}

	void calib(){
		while (dodeca_tracker.stat == DodecaTracker.STATUS.CALIB) {
			if (dodeca_tracker.grab()) {
				if (dodeca_tracker.detectMarkers()>=10 && dodeca_tracker.calibrateCameraPose()) {
					dodeca_tracker.getCameraPose (ref cam_rotation, ref cam_translation);

					calib_poses_buffer.Add(new PoseInfo (cam_rotation, cam_translation, cur_frame_idx));
				}
				cur_frame_idx++;
			}
		}
	}

	void OnApplicationQuit(){
		StartCoroutine(reset());
	}
}
