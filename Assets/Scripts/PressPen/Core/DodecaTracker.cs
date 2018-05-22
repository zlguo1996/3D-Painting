using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;

public class DodecaTracker : MonoBehaviour {
    public bool useVirtualWebCam = false;

	// ------------ 初始化参数 ------------
	[Tooltip("video capture parameter file path")]
	public string camera_parameter_path = "/Resources/parameters/cameras/logitech_brio_camera_calibration_1080p.yml";
	[Tooltip("video capture id")]
	public int device_num = 0;
	[Tooltip("camera pose - rotation")]
	public Vector3 camera_rvec = Vector3.zero;
	[Tooltip("camera pose - translation")]
	public Vector3 camera_tvec = Vector3.zero;
	[Tooltip("PressPen marker map")]
	public string marker_map_path = "/Resources/parameters/pens/presspen/dodecahedron_marker_map.yml";
	[Tooltip("calibration board marker map")]
	public string board_marker_map_path = "/Resources/parameters/board/board_marker_map.yml";

	public string pentip_path = "/Resources/parameters/pens/presspen/pentip_calibration.yml";
	public string dodeca_center_path = "/Resources/parameters/pens/presspen/dodeca_center_calibration.yml";

	// ----------- pose --------------
	// 局部坐标系（笔坐标系）下的pose
	[HideInInspector]
	public Matrix4x4 pen_tip_pose;
	[HideInInspector]
	public Matrix4x4 pen_dodeca_center_pose;

	// ------------- 可视化pose(测试用) ----------------
	[Tooltip("visualize PressPen pose")]
	public GameObject press_pen;
	[Tooltip("visualize web camera pose")]
	public GameObject web_cam;

	// -------------- 状态 ---------------
	public enum STATUS {NONE, CALIB, TRACK};
	public UnityEvent statChangeEvent = new UnityEvent ();
	[HideInInspector]
	public STATUS stat{
		get { return _stat;}
		set {
			_stat = value;
			statChangeEvent.Invoke ();
		}
	}
	private STATUS _stat;

	[HideInInspector]
	public bool isInit = false;

	// --------------- 基本函数 ---------------
	[DllImport("DodecaTrackerPlugin")]
	private static extern void _DodecaTrackerPlugin ();
	[DllImport("DodecaTrackerPlugin")]
	private static extern bool _reset ();
	[DllImport("DodecaTrackerPlugin")]
	private static extern bool _isValid();

	// ------------ 初始化 ----------------
	[DllImport("DodecaTrackerPlugin")]
	private static extern bool _initCamera(string camera_parameter_path, int deviceNum);
	[DllImport("DodecaTrackerPlugin")]
	private static extern bool _initCameraRvecTvec(string camera_parameter_path, int deviceNum, float[] rvec, float[] tvec);
    [DllImport("DodecaTrackerPlugin")]
    private static extern bool _initCameraS(string camera_parameter_path, string file_path);
    [DllImport("DodecaTrackerPlugin")]
    private static extern bool _initCameraSRvecTvec(string camera_parameter_path, string file_path, float[] rvec, float[] tvec);
	[DllImport("DodecaTrackerPlugin")]
	private static extern bool _initPen(string marker_map_path);
	[DllImport("DodecaTrackerPlugin")]
	private static extern bool _initPenDetector();

	// -------------- 检测 ------------------
	// dodeca
	[DllImport("DodecaTrackerPlugin")]
	private static extern bool _grab();
	[DllImport("DodecaTrackerPlugin")]
	private static extern bool _detect();
	[DllImport("DodecaTrackerPlugin")]
	private static extern bool _getPose(float[] rvec, float[] tvec);
	[DllImport("DodecaTrackerPlugin")]
	private static extern bool _getPoseM(float[] rt_mat);
	public bool grab(){
		return _grab ();
	}
	// detect dodeca (include markers)
	public bool detect(){
		return _detect ();
	}
	// deprecated
	private bool getPose(ref Vector3 rvec, ref Vector3 tvec){
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
	public bool getPose(ref Matrix4x4 rt_mat){
		float[] rt_vec = new float[16];
		bool success = _getPoseM(rt_vec);

		if(!success) return success;

		for(int i=0; i<16; i++){
			rt_mat[i/4, i%4] = rt_vec[i];
		}

		return success;
	}

	// camera
	[DllImport("DodecaTrackerPlugin")]
	private static extern int _detectMarkers();
	[DllImport("DodecaTrackerPlugin")]
	private static extern bool _calibrateCameraPose (string calib_marker_map_path);
	[DllImport("DodecaTrackerPlugin")]
	private static extern bool _getCameraPose (float[] rvec, float[] tvec);
	[DllImport("DodecaTrackerPlugin")]
	private static extern bool _getCameraPoseM (float[] rt_mat);
	// detect markers
	public int detectMarkers(){
		return _detectMarkers ();
	}
	// calib cam using detected markers
	public bool calibrateCameraPose(){
		return _calibrateCameraPose (board_marker_map_path);
	}
	// deprecated
	private bool getCameraPose(ref Vector3 rvec, ref Vector3 tvec){
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
	public bool getCameraPose(ref Matrix4x4 rt_mat){
		float[] rt_vec = new float[16];
		bool success = _getCameraPoseM(rt_vec);

		if(!success) return success;

		for(int i=0; i<16; i++){
			rt_mat[i/4, i%4] = rt_vec[i];
		}

		return success;
	}

	// ----------- 笔尖 & 正十二面体中心 ---------

	[DllImport("DodecaTrackerPlugin")]
	private static extern bool _setPenTip(string file_path);
	[DllImport("DodecaTrackerPlugin")]
	private static extern bool _setPenDodecaCenter(string file_path);
	[DllImport("DodecaTrackerPlugin")]
	private static extern bool _getPenTipPosition(float[] tvec);
	[DllImport("DodecaTrackerPlugin")]
	private static extern bool _getPenDodecaCenterPosition(float[] tvec);
	[DllImport("DodecaTrackerPlugin")]
	private static extern bool _setPenTipM(float[] rt_mat);
	[DllImport("DodecaTrackerPlugin")]
	private static extern bool _setPenDodecaCenterM(float[] rt_mat);
	[DllImport("DodecaTrackerPlugin")]
	private static extern bool _savePenTipPose (float[] rt_mat, string file_path);
	[DllImport("DodecaTrackerPlugin")]
	private static extern bool _getPenTipPose(float[] rt_mat);
	[DllImport("DodecaTrackerPlugin")]
	private static extern bool _getPenDodecaCenterPose(float[] rt_mat);
    [DllImport("DodecaTrackerPlugin")]
	private static extern bool _getPoseFromFile(float[] rt_mat, string file_path);

	public bool setPenTip(string file_path){
		if (_setPenTip (file_path)) {
			getPenTipPose (ref pen_tip_pose);
			return true;
		} else {
			return false;
		}
	}
	public bool setPenTip(Matrix4x4 rt_mat){
		float[] rt_vec = new float[16];
		for(int i=0; i<16; i++){
			rt_vec[i] = rt_mat[i/4, i%4];
		}

		if (_setPenTipM (rt_vec)) {
			getPenTipPose (ref pen_tip_pose);
			return false;
		} else {
			return false;
		}
	}
	public bool setPenDodecaCenter(string file_path){
		if (_setPenDodecaCenter (file_path)) {
			getPenDodecaCenterPose (ref pen_dodeca_center_pose);
			return true;
		} else {
			return false;
		}
	}
	public bool setPenDodecaCenter(Matrix4x4 rt_mat){
		float[] rt_vec = new float[16];
		for(int i=0; i<16; i++){
			rt_vec[i] = rt_mat[i/4, i%4];
		}

		if (_setPenDodecaCenterM (rt_vec)) {
			getPenDodecaCenterPose (ref pen_dodeca_center_pose);
			return true;
		} else {
			return false;
		}
	}
	public bool savePenTipPose (Matrix4x4 rt_mat, string file_path){
		float[] rt_vec = new float[16];
		for(int i=0; i<16; i++){
			rt_vec[i] = rt_mat[i/4, i%4];
		}
		return _savePenTipPose(rt_vec, file_path);
	}
	// deprecated
	private bool getPenTipPosition(ref Vector3 tvec){
		float[] tvec_f = new float[3];
		bool success = _getPenTipPosition (tvec_f);

		if(!success) return success;

		tvec.x = tvec_f [0];
		tvec.y = tvec_f [1];
		tvec.z = tvec_f [2];

		return true;
	}
	// deprecated
	private bool getPenDodecaCenterPosition(ref Vector3 tvec){
		float[] tvec_f = new float[3];
		bool success = _getPenTipPosition (tvec_f);

		if(!success) return success;

		tvec.x = tvec_f [0];
		tvec.y = tvec_f [1];
		tvec.z = tvec_f [2];

		return true;
	}
	public bool getPenTipPose(ref Matrix4x4 rt_mat){
		float[] rt_vec = new float[16];
		bool success = _getPenTipPose(rt_vec);

		if(!success) return success;

		for(int i=0; i<16; i++){
			rt_mat[i/4, i%4] = rt_vec[i];
		}

		return success;
	}
	public bool getPenDodecaCenterPose(ref Matrix4x4 rt_mat){
		float[] rt_vec = new float[16];
		bool success = _getPenDodecaCenterPose(rt_vec);

		if(!success) return success;

		for(int i=0; i<16; i++){
			rt_mat[i/4, i%4] = rt_vec[i];
		}

		return success;
	}
    public bool getPoseFromFile(ref Matrix4x4 rt_mat, string file_path){
        if (!System.IO.File.Exists(file_path)) return false;
        float[] rt_vec = new float[16];
        bool success = _getPoseFromFile(rt_vec, file_path);
        for (int i = 0; i < 16; i++)
        {
            rt_mat[i / 4, i % 4] = rt_vec[i];
        }

        return success;
    }

	void Start(){
		camera_parameter_path = Application.dataPath + camera_parameter_path;
		marker_map_path = Application.dataPath + marker_map_path;
		board_marker_map_path = Application.dataPath + board_marker_map_path;
		pentip_path = Application.dataPath + pentip_path;
		dodeca_center_path = Application.dataPath + dodeca_center_path;

		stat = STATUS.NONE;

		init ();
	}

	public bool init(){
		bool success = true;

		_DodecaTrackerPlugin ();

		Debug.Assert (System.IO.File.Exists (camera_parameter_path));
		float[] rvec = new float[3]{ camera_rvec.x, camera_rvec.y, camera_rvec.z };
		float[] tvec = new float[3]{ camera_tvec.x, camera_tvec.y, camera_tvec.z };
        if(useVirtualWebCam) success = _initCameraSRvecTvec(camera_parameter_path, "udp://127.0.0.1:9999", rvec, tvec);
        else success = _initCameraRvecTvec(camera_parameter_path, device_num, rvec, tvec);
		Debug.Assert (success);

		Debug.Assert (System.IO.File.Exists (marker_map_path));
		success = _initPen (marker_map_path);
		Debug.Assert (success);

		Debug.Assert (System.IO.File.Exists (board_marker_map_path));

		success = _initPenDetector ();
		Debug.Assert (success);

		Debug.Assert (System.IO.File.Exists (pentip_path));
		success = setPenTip (pentip_path);
		Debug.Assert (success);

		Debug.Assert (System.IO.File.Exists (dodeca_center_path));
		success = setPenDodecaCenter (dodeca_center_path);
		Debug.Assert (success);

		isInit = true;
		return true;
	}
	void OnDisable(){
		reset ();
	}

	void Update(){
		//testUpdate ();
	}
	private void testUpdate(){
		if (grab ()) {
			//将检测到的dodeca位置赋值给当前物体
			if (stat==STATUS.TRACK && detect ()) {
				Vector3 rvec = Vector3.zero;
				Vector3 tvec = Vector3.zero;
				getPose (ref rvec, ref tvec);
				tvec.Scale (new Vector3 (10, 10, 10));
				press_pen.transform.position = tvec;
				press_pen.transform.rotation = Quaternion.Euler(rvec*Mathf.Rad2Deg);
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

	public bool reset(){
		bool success = true;
		if (isInit) {
			success = _reset ();
			isInit = false;
		}
		return success;
	}

	public bool isValid(){
		return _isValid ();
	}
}
