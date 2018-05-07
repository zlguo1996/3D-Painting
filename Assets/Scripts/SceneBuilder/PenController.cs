using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PenController : MonoBehaviour {

	// active状态可以用于激活其他物体
	public bool isActivated = true;
	public int id = 0;
	public enum PRESSURE_LEVEL
	{
		LEVEL_0, LEVEL_1, LEVEL_2
	}
	public PRESSURE_LEVEL pressure_level = PRESSURE_LEVEL.LEVEL_0;

	public UnityEvent OnPoseChange = new UnityEvent();
	public UnityEvent OnPressureChange = new UnityEvent();
	public UnityEvent OnPressureExceed_01 = new UnityEvent();
	public UnityEvent OnPressureDeceed_01 = new UnityEvent();
	public UnityEvent OnPressureExceed_12 = new UnityEvent();
	public UnityEvent OnPressureDeceed_12 = new UnityEvent();


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKey(KeyCode.Q)){
			this.transform.Translate(new Vector3(0.0f, 0.0f, 0.1f));
		}
		if(Input.GetKey(KeyCode.E)){
			this.transform.Translate(new Vector3(0.0f, 0.0f, -0.1f));
		}
		if(Input.GetKey(KeyCode.W)){
			this.transform.Translate(new Vector3(0.0f, 0.1f, 0.0f));
		}
		if(Input.GetKey(KeyCode.S)){
			this.transform.Translate(new Vector3(0.0f, -0.1f, 0.0f));
		}
		if(Input.GetKey(KeyCode.A)){
			this.transform.Translate(new Vector3(-0.1f, 0.0f, 0.0f));
		}
		if(Input.GetKey(KeyCode.D)){
			this.transform.Translate(new Vector3(0.1f, 0.0f, 0.0f));
		}

		if (Input.GetKeyDown(KeyCode.UpArrow)) {
			pressure_level += 1;
			if (pressure_level > PRESSURE_LEVEL.LEVEL_2)
				pressure_level = PRESSURE_LEVEL.LEVEL_2;
			Debug.Log (pressure_level);
			switch (pressure_level) {
			case PRESSURE_LEVEL.LEVEL_1:
				OnPressureExceed_01.Invoke ();
				break;
			case PRESSURE_LEVEL.LEVEL_2:
				OnPressureExceed_12.Invoke ();
				break;
			}
		}
		if (Input.GetKeyDown(KeyCode.DownArrow)) {
			pressure_level -= 1;
			if (pressure_level < PRESSURE_LEVEL.LEVEL_0)
				pressure_level = PRESSURE_LEVEL.LEVEL_0;
			Debug.Log (pressure_level);
			switch (pressure_level) {
			case PRESSURE_LEVEL.LEVEL_0:
				OnPressureDeceed_01.Invoke ();
				break;
			case PRESSURE_LEVEL.LEVEL_1:
				OnPressureDeceed_12.Invoke ();
				break;
			}
		}
	}
}
