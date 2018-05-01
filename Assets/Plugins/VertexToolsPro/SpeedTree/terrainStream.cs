using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class terrainStream : MonoBehaviour {


	public void streamToTerrainTrees() {

		TerrainData data = GetComponent<Terrain> ().terrainData;

		foreach(TreeInstance tree in data.treeInstances)
		{


			foreach (instanceStream stream in data.treePrototypes [tree.prototypeIndex].prefab.GetComponentsInChildren<instanceStream>()) {

				stream.rebuildForTerrain ();

			}

		}

	}


	// Use this for initialization
	void Start () {

		TerrainData data = GetComponent<Terrain> ().terrainData;

		foreach(TreeInstance tree in data.treeInstances)
		{


			foreach (instanceStream stream in data.treePrototypes [tree.prototypeIndex].prefab.GetComponentsInChildren<instanceStream>()) {

				stream.rebuildForTerrain ();

			}

		}

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
