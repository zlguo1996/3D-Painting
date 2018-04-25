using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrokeManager : MonoBehaviour {
	public GameObject stroke_prefab;

	private GameObject cur_stroke = null;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void StartPainting(Vector3 position, float width){
		cur_stroke = Instantiate<GameObject> (stroke_prefab);
		cur_stroke.transform.parent = this.transform;
		stroke_prefab.GetComponent<Stroke> ().init(position, width);
	}

	void AddPoint(Vector3 position){
		stroke_prefab.GetComponent<Stroke> ().addPoint (position);
	}

	void EndPainting(){
		if (stroke_prefab.GetComponent<Stroke>().point_nums<2) {
			Destroy (cur_stroke);
		}
		cur_stroke = null;
	}
}
