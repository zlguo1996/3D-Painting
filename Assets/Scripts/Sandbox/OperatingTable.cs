using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OperatingTable : MonoBehaviour {
	public bool move_mode = false;
	public PenController pc;
    public GameObject my_camera;

    public float scale_parameter = 1.0f;

    public float rotation_distance_threashold;
    public float zoom_distance_threashold;
    public float[] zoom_threashold = new float[2] { 10.0f, 100.0f };

	private UnityAction start_move_action;
	private UnityAction stop_move_action;

	// Use this for initialization
	void Start () {
		start_move_action += StartMove;
		stop_move_action += StopMove;

		pc.OnPressureExceed_12.AddListener (start_move_action);
		pc.OnPressureDeceed_12.AddListener (stop_move_action);

        my_camera.transform.LookAt(this.transform);
        my_camera.transform.position = (my_camera.transform.position - this.transform.position).normalized * scale_parameter * pc.press_pen.scale_parameter + this.transform.position;
	}
	
	// Update is called once per frame
	void Update () { 
        if (move_mode) 
        {
            // 缩放
            if (Input.GetKey(KeyCode.LeftAlt))
            {
                DodecaTracker dt = pc.press_pen.dodeca_tracker_thread.dodeca_tracker;
                Matrix4x4 pose1 = pc.press_pen.getFrame().rt_mat, pose2 = pc.press_pen.getPrevFrame(1).rt_mat;

                // 修改scale 笔尖向右放大，向左缩小
                Vector3 pentip_translation = (pose1 * dt.pen_tip_pose).ExtractPosition() - (pose2 * dt.pen_tip_pose).ExtractPosition();
                pc.press_pen.scale_parameter -= 10.0f*pentip_translation.x*(my_camera.transform.position - this.transform.position).magnitude/20.0f;
                if (pc.press_pen.scale_parameter < zoom_threashold[0]) pc.press_pen.scale_parameter = zoom_threashold[0];
                else if (pc.press_pen.scale_parameter > zoom_threashold[1]) pc.press_pen.scale_parameter = zoom_threashold[1];
                my_camera.transform.position = (my_camera.transform.position - this.transform.position).normalized * scale_parameter * pc.press_pen.scale_parameter + this.transform.position;
            }
            // 旋转
            else if(Input.GetKey(KeyCode.LeftControl)){
                Debug.Log("rotate");
                DodecaTracker dt = pc.press_pen.dodeca_tracker_thread.dodeca_tracker;
                Matrix4x4 pose1 = pc.press_pen.getFrame().rt_mat, pose2 = pc.press_pen.getPrevFrame(1).rt_mat;

                // 修改rotation 笔尖左右绕y轴旋转
                Vector3 pentip_translation = (pose1 * dt.pen_tip_pose).ExtractPosition() - (pose2 * dt.pen_tip_pose).ExtractPosition();
                this.transform.Rotate(0.0f, 10.0f * pentip_translation.x, 0.0f, Space.World);
                Debug.Log(10.0f * pentip_translation.x);

                // 修改rotation 前后绕x旋转
                float step = (my_camera.transform.position - this.transform.position).magnitude / 20.0f;
                my_camera.transform.Translate(0.0f, 10.0f * step * pentip_translation.z, 0.0f, Space.Self);
                my_camera.transform.LookAt(this.transform);
                my_camera.transform.position = (my_camera.transform.position - this.transform.position).normalized * scale_parameter * pc.press_pen.scale_parameter + this.transform.position;
            }
            // 平移
            else
            {
                Vector3 t_step = pc.direction.ScaleTo(0.05f*pc.press_pen.scale_parameter/20.0f);
                this.gameObject.transform.Translate(t_step);
            }
		}
	}

	void StartMove(){
		move_mode = true;
	}
	void StopMove(){
		move_mode = false;
	}
}
