using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 为所有子物体添加脚本
public class AssignModelScripts : MonoBehaviour {
    // Use this for initialization
    public PenController pc;
	void Start () {
        foreach(Transform child in this.transform){
            child.gameObject.AddComponent<MeshCollider>();
            child.gameObject.AddComponent<Model>();
            //cakeslice.Outline ot = child.gameObject.AddComponent<cakeslice.Outline>();
            //ot.eraseRenderer = true;
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
