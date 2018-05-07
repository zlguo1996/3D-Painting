using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ModelBox : MonoBehaviour {

	public GameObject model;
	public Transform origin_parent;

	public bool isActivated = false;
	private PenController pc;
	private UnityAction add_model_action;

	// Use this for initialization
	void Start () {
		add_model_action += AddModel;
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

	void AddModel(){
		Vector3 pos = pc.gameObject.transform.position;
		deactivate ();

		GameObject created_model = Instantiate<GameObject> (model);
		created_model.transform.position = pos;
		created_model.transform.rotation = this.gameObject.transform.rotation;

		created_model.transform.parent = origin_parent;
	}

	private void activate(PenController pc_in){
		this.isActivated = true;
		pc = pc_in;
		pc.isActivated = false;

		pc.OnPressureExceed_01.AddListener (add_model_action);
	}

	private void deactivate(){
		pc.OnPressureExceed_01.RemoveListener (add_model_action);

		this.isActivated = false;
		pc.isActivated = true;
		pc = null;
	}
}
