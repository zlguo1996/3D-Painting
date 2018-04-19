using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class DodecaTracker : MonoBehaviour {
	public string camera_parameter_path;
	public int device_num = 0;
	public Vector3 camera_rvec;
	public Vector3 camera_tvec;
	public string marker_map_path;
	public string board_marker_map_path;

	public GameObject web_cam;

	public enum STATUS {NONE, CALIB, TRACK};
	[HideInInspector]
	public STATUS stat = STATUS.NONE;

	[DllImport("DodecaTrackerPlugin")]
	private static extern void _DodecaTrackerPlugin ();

	[DllImport("DodecaTrackerPlugin")]
	private static extern bool _reset ();

	[DllImport("DodecaTrackerPlugin")]
	private static extern bool _initCamera(string camera_parameter_path, int deviceNum);

	[DllImport("DodecaTrackerPlugin")]
	private static extern bool _initCameraRvecTvec(string camera_parameter_path, int deviceNum, float[] rvec, float[] tvec);

	[DllImport("DodecaTrackerPlugin")]
	private static extern bool _initPen(string marker_map_path);

	[DllImport("DodecaTrackerPlugin")]
	private static extern bool _initPenDetector();

	[DllImport("DodecaTrackerPlugin")]
	private static extern bool _grab();

	[DllImport("DodecaTrackerPlugin")]
	private static extern bool _detect();

	[DllImport("DodecaTrackerPlugin")]
	private static extern int _detectMarkers();

	[DllImport("DodecaTrackerPlugin")]
	private static extern bool _calibrateCameraPose (string calib_marker_map_path);

	[DllImport("DodecaTrackerPlugin")]
	private static extern bool _getCameraPose (float[] rvec, float[] tvec);

	[DllImport("DodecaTrackerPlugin")]
	private static extern bool _getPose(float[] rvec, float[] tvec);

	[DllImport("DodecaTrackerPlugin")]
	private static extern bool _isValid();

	void Start(){
		camera_parameter_path = Application.dataPath + "/Resources/parameters/cameras/macbook_camera_parameters.yml";
		device_num = 0;

		camera_rvec = Vector3.zero;
		camera_tvec = Vector3.zero;

		marker_map_path = Application.dataPath + "/Resources/parameters/pens/dodecahedron_marker_map.yml";

		board_marker_map_path = Application.dataPath + "/Resources/parameters/board/board_marker_map.yml";

		init ();
	}

	public bool init(){
		bool success = true;

		_DodecaTrackerPlugin ();

		Debug.Assert (System.IO.File.Exists (camera_parameter_path));
		if (false){//camera_rvec == null || camera_tvec == null) {
			success = _initCamera (camera_parameter_path, device_num);
		} else {
			float[] rvec = new float[3]{ camera_rvec.x, camera_rvec.y, camera_rvec.z };
			float[] tvec = new float[3]{ camera_tvec.x, camera_tvec.y, camera_tvec.z };
			success = _initCameraRvecTvec (camera_parameter_path, device_num, rvec, tvec);
		}
		Debug.Assert (success);

		Debug.Assert (System.IO.File.Exists (marker_map_path));
		success = _initPen (marker_map_path);
		Debug.Assert (success);

		Debug.Assert (System.IO.File.Exists (board_marker_map_path));

		success = _initPenDetector ();
		Debug.Assert (success);

		return true;
	}

	void Update(){
		if (grab ()) {
			//将检测到的dodeca位置赋值给当前物体
			if (stat==STATUS.TRACK && detect ()) {
				Vector3 rvec = Vector3.zero;
				Vector3 tvec = Vector3.zero;
				getPose (ref rvec, ref tvec);
				tvec.Scale (new Vector3 (10, 10, 10));
				this.transform.position = tvec;
				this.transform.rotation = Quaternion.Euler(rvec*Mathf.Rad2Deg);
				Debug.Log (tvec);
			}

			// 将camera位置赋值给当前物体
			if (stat==STATUS.CALIB && detectMarkers()>10) {
				if (calibrateCameraPose ()) {
					Vector3 rvec = Vector3.zero;
					Vector3 tvec = Vector3.zero;
					getCameraPose (ref rvec, ref tvec);
					tvec.Scale (new Vector3 (10, 10, 10));
					web_cam.transform.position = tvec;
					web_cam.transform.rotation = Quaternion.Euler(rvec*Mathf.Rad2Deg);
					Debug.Log (tvec);
				}
			}
		}
	}

	public void changeStat(){
		switch (stat) {
		case STATUS.NONE:
			stat = STATUS.CALIB;
			break;
		case STATUS.CALIB:
			stat = STATUS.TRACK;
			break;
		case STATUS.TRACK:
			stat = STATUS.NONE;
			break;
		}
	}

	public bool grab(){
		return _grab ();
	}

	public bool detect(){
		return _detect ();
	}

	public int detectMarkers(){
		return _detectMarkers ();
	}

	public bool calibrateCameraPose(){
		return _calibrateCameraPose (board_marker_map_path);
	}

	public bool getCameraPose(ref Vector3 rvec, ref Vector3 tvec){
		float[] rvec_f = new float[3];
		float[] tvec_f = new float[3];
		bool success = _getCameraPose(rvec_f, tvec_f);

		if(!success) return success;

		rvec.x = rvec_f [0];
		rvec.y = rvec_f [1];
		rvec.z = rvec_f [2];

		tvec.x = tvec_f [0];
		tvec.y = tvec_f [1];
		tvec.z = tvec_f [2];

		return success;
	}

	public bool getPose(ref Vector3 rvec, ref Vector3 tvec){
		float[] rvec_f = new float[3];
		float[] tvec_f = new float[3];
		bool success = _getPose(rvec_f, tvec_f);

		if(!success) return success;

		rvec.x = rvec_f [0];
		rvec.y = rvec_f [1];
		rvec.z = rvec_f [2];

		tvec.x = tvec_f [0];
		tvec.y = tvec_f [1];
		tvec.z = tvec_f [2];

		return success;
	}

	public bool isValid(){
		return _isValid ();
	}

	void OnDisable(){
		_reset ();
	}
}
