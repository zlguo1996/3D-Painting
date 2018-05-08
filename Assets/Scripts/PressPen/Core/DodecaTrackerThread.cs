using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Threading;

public class PoseDetectedEvent: UnityEvent<Matrix4x4, uint>{}

public class DodecaTrackerThread : MonoBehaviour {
	public DodecaTracker dodeca_tracker;
	public PoseDetectedEvent on_detect_dodeca = new PoseDetectedEvent();
	public PoseDetectedEvent on_detect_cam = new PoseDetectedEvent();

	// dodeca position
	[HideInInspector]
	public Matrix4x4 cur_pose;
	[HideInInspector]
	public Vector3 cur_point_position;

	// camera position
	[HideInInspector]
	public Matrix4x4 cam_pose;

	// frame idx
	[HideInInspector]
	public uint cur_frame_idx = 0;

	private Thread track_thread;
	private Thread calib_thread; 
	private List<PoseInfo> track_poses_buffer = new List<PoseInfo>();
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
			on_detect_dodeca.Invoke (pose.rt_mat, pose.index);
		}
		track_poses_buffer.Clear ();
		foreach (PoseInfo pose in calib_poses_buffer) {
			on_detect_cam.Invoke (pose.rt_mat, pose.index);
		}
		calib_poses_buffer.Clear ();
	}

	public void reset(){
		if (dodeca_tracker.stat == DodecaTracker.STATUS.TRACK) {
			stopTracking ();
		} else if (dodeca_tracker.stat == DodecaTracker.STATUS.CALIB) {
			stopCalibration ();
		}

		dodeca_tracker.reset ();
	}

	// ---------- tracking -------------
	public bool startTracking(){
		Debug.Assert (dodeca_tracker.stat == DodecaTracker.STATUS.NONE);
		Debug.Assert (dodeca_tracker.isInit);
		dodeca_tracker.stat = DodecaTracker.STATUS.TRACK;

		track_thread = new Thread (track);
		track_thread.Start ();

		return true;
	}

	public bool stopTracking(){
		Debug.Assert (dodeca_tracker.stat == DodecaTracker.STATUS.TRACK);
		dodeca_tracker.stat = DodecaTracker.STATUS.NONE;

		track_thread.Join ();

		return true;
	}

	void track(){
		while (dodeca_tracker.stat == DodecaTracker.STATUS.TRACK) {
			if (dodeca_tracker.grab()) {
				if (dodeca_tracker.detect()) {
					dodeca_tracker.getPose (ref cur_pose);

					track_poses_buffer.Add (new PoseInfo (cur_pose, cur_frame_idx));
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
		 
		calib_thread = new Thread (calib);
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
					dodeca_tracker.getCameraPose (ref cam_pose);

					calib_poses_buffer.Add(new PoseInfo (cam_pose , cur_frame_idx));
				}
				cur_frame_idx++;
			}
		}
	}

	void OnApplicationQuit(){
		reset ();
	}
}
