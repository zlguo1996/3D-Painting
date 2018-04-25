using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stroke : MonoBehaviour {
	public LineRenderer lr;
	[HideInInspector]
	public int point_nums = 0;

	// Use this for initialization
	void Start () {
		lr = this.gameObject.GetComponent<LineRenderer> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void init(Vector3 pos, float width){
		lr.SetPosition (0, pos);
		lr.startWidth = width;
		lr.endWidth = width;
		point_nums++;
	}

	public void addPoint(Vector3 pos){
		lr.positionCount = point_nums + 1;
		lr.SetPosition (point_nums, pos);
		point_nums++;
	}
}
