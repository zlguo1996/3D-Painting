using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OperatingTable : MonoBehaviour {
	public bool move_mode = false;
	public PenController pc;

	private UnityAction start_move_action;
	private UnityAction stop_move_action;

	// Use this for initialization
	void Start () {
		start_move_action += StartMove;
		stop_move_action += StopMove;

		pc.OnPressureExceed_12.AddListener (start_move_action);
		pc.OnPressureDeceed_12.AddListener (stop_move_action);
	}
	
	// Update is called once per frame
	void Update () {
		if (move_mode) {
			this.gameObject.transform.Translate (new Vector3 (1.0f, 0.0f, 0.0f));
		}
	}

	void StartMove(){
		move_mode = true;
	}
	void StopMove(){
		move_mode = false;
	}
}
