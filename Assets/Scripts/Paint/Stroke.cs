using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stroke : MonoBehaviour {
	[HideInInspector]
	public int point_nums = 0;

	// Use this for initialization
	void Start () {
		//lr = this.gameObject.GetComponent<LineRenderer> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void init(Vector3 pos, float width){
		this.gameObject.GetComponent<LineRenderer>().SetPosition (0, pos);
		this.gameObject.GetComponent<LineRenderer>().startWidth = width;
		this.gameObject.GetComponent<LineRenderer>().endWidth = width;
		point_nums++;
	}

	public void addPoint(Vector3 pos){
		this.gameObject.GetComponent<LineRenderer>().positionCount = point_nums + 1;
		this.gameObject.GetComponent<LineRenderer>().SetPosition (point_nums, pos);
		point_nums++;
	}
}
