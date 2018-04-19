using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class Test : MonoBehaviour {

	public int size=10;

	[DllImport("Wrapper")]
	private static extern int [,] fillArray(int size);

	[DllImport("Wrapper")]
	private static extern int add (int a, int b);

//	[DllImport("Wrapper", CharSet=CharSet.Auto,
//		CallingConvention=CallingConvention.StdCall)]
//	private static extern int concat (string a, string b);

	[DllImport("Wrapper", CallingConvention = CallingConvention.Cdecl)]
	private static extern byte[] rstr ();

	[DllImport("Wrapper")]
	private static extern int s_in (StringBuilder ch);

	// Use this for initialization
	void Start () {
		int [,] array;
		array = fillArray(size);

		int a = 0;
		int b = add (a, a);

		StringBuilder sb = new StringBuilder ("abc", 30);
		int c = s_in (sb);

		byte[] bt = rstr();
		Debug.Log (bt[0]);
	}
	
	// Update is called once per frame
	void Update () {
	}
}
