using UnityEngine;
using UnityEditor;
using System.Collections;

public class VPP_Menus : MonoBehaviour {

	[MenuItem("Tools/Vertex Tools Pro/Vertex Tools Pro #%v", false, 10)]
	static void LaunchVT() {

		VT_window.LaunchVT_window ();

	}

	[MenuItem("Tools/Vertex Tools Pro/Vertex Color Animator #%a", false, 10)]
	static void LaunchVVCA() {

		VCA_window.LaunchVCA_window ();

	}

	[MenuItem("Tools/Vertex Tools Pro/SpeedTree Painter (Early Access) #%t", false, 10)]
	static void LaunchVTTree() {

		VT_Tree_window.LaunchVT_Tree_window ();

	}

	/*
	[MenuItem("Tools/LightMap to Vertex #%a", false, 10)]
	static void LaunchLMB() {

		LMB_window.LaunchLMB_window ();

	}
	*/
	/*
	[MenuItem("Tools/Decal Painter Pro #%d", false, 10)]
	static void LaunchDPP() {

		DPP_window.LaunchDPP_window ();

	}
	*/
}
