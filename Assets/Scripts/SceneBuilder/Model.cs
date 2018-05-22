using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Model : MonoBehaviour {

	public bool isActivated = false;

	private Transform origin_parent;
	private PenController pc;
	private UnityAction start_follow_pc_action;
	private UnityAction stop_follow_pc_action;

	// Use this for initialization
	void Start () {
		start_follow_pc_action += StartFollowPC;
		stop_follow_pc_action += StopFollowPC;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	// 进入激活状态
	void OnTriggerEnter(Collider col){
        if (!isActivated) {
			PenController pc_in = col.gameObject.GetComponent<PenController> ();
			if (pc_in != null && pc_in.isActivated) {
				activate (pc_in);
			}
		}
	}

	// 退出激活状态
	void OnTriggerExit(Collider col){
		if (isActivated) {
			PenController pc_out = col.gameObject.GetComponent<PenController> ();
			if (pc_out != null && pc_out.id == pc.id) {
				deactivate ();
			}
		}
	}

	void StartFollowPC(){
		Debug.Log ("follow");
		origin_parent = this.gameObject.transform.parent;
		this.gameObject.transform.parent = pc.gameObject.transform;
	}
	void StopFollowPC(){
		this.gameObject.transform.parent = origin_parent;
	}

	void activate(PenController pc_in){
		Debug.Log ("activate");
		isActivated = true;
		pc = pc_in;
		pc.isActivated = false;

		pc.OnPressureExceed_01.AddListener (start_follow_pc_action);
		pc.OnPressureDeceed_01.AddListener (stop_follow_pc_action);

		if (pc.pressure_level!=PenController.PRESSURE_LEVEL.LEVEL_0) {
			StartFollowPC ();
		}
	}

	void deactivate(){
		pc.OnPressureExceed_01.RemoveListener (start_follow_pc_action);
		pc.OnPressureDeceed_01.RemoveListener (stop_follow_pc_action);

		if (pc.pressure_level!=PenController.PRESSURE_LEVEL.LEVEL_0) {
			StopFollowPC ();
		}

		isActivated = false;
		pc.isActivated = true;
		pc = null;
	}
}
