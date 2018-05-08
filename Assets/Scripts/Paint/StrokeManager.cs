using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
 
enum DrawStatus {START, ADD, END, NONE};

public class StrokeManager : MonoBehaviour {
	public GameObject stroke_prefab;
	public PressPen press_pen;
	public int pressure_threshold = 1000;

	private GameObject cur_stroke = null;
	private DrawStatus draw_status = DrawStatus.NONE;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		// test
		if (Input.GetKeyDown(KeyCode.A)) {
			draw_status = DrawStatus.START;
			Debug.Log ("start painting");
		}
		if (Input.GetKeyUp (KeyCode.A)) {
			draw_status = DrawStatus.END;
			Debug.Log ("stop painting");
		}
	}

	void StartPainting(Vector3 position, float width){
		cur_stroke = Instantiate<GameObject> (stroke_prefab);
		cur_stroke.transform.parent = this.transform;
		cur_stroke.GetComponent<Stroke> ().init(position, width);
	}

	void AddPoint(Vector3 position){
		cur_stroke.GetComponent<Stroke> ().addPoint (position);
	}

	void EndPainting(){
		if (cur_stroke==null) {
			return;
		}
		if (cur_stroke.GetComponent<Stroke>().point_nums<2) {
			Destroy (cur_stroke);
		}
		cur_stroke = null;
	}

	public void on_detect_pen(){
		if (draw_status == DrawStatus.NONE)
			return;

		FrameInfo frame_info = press_pen.getFrame ();
		Debug.Log (frame_info.rt_mat);
		Vector3 tvec = (frame_info.rt_mat*press_pen.dodeca_tracker_thread.dodeca_tracker.pen_tip_pose).ExtractPosition();
		tvec.Scale (new Vector3 (20.0f, 20.0f, 20.0f));
		if (draw_status == DrawStatus.START) {
			StartPainting (tvec, /*(float)frame_info.pressure/10.0f*/0.1f);
			draw_status = DrawStatus.ADD;
		} else if (draw_status == DrawStatus.ADD) {
			AddPoint (tvec);
		} else if (draw_status == DrawStatus.END) {
			EndPainting ();
			draw_status = DrawStatus.NONE;
		}
	}

	public void on_detect_pressure(){
		if (draw_status == DrawStatus.NONE && press_pen.press_measure.pressure > pressure_threshold) {
			draw_status = DrawStatus.START;
			Debug.Log ("start painting");
		} else if ((draw_status == DrawStatus.ADD || draw_status == DrawStatus.START) && press_pen.press_measure.pressure < pressure_threshold) {
			draw_status = DrawStatus.END;
			Debug.Log ("stop painting");
		}
	}
}
