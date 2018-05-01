using UnityEngine;
using System.Collections;

public class RotationAnimation : MonoBehaviour {

	public AnimationCurve rotationCurve;
	public enum RotationVectorEnum {X, Y, Z};
	public RotationVectorEnum RotationVector = RotationVectorEnum.Z;

	public float speed = 0.25f;
	public float intensity = 2f;
	public float timeOffset = 0f;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
	

		Vector3 axis = Vector3.zero;

		if (RotationVector == RotationVectorEnum.X) {
			axis = Vector3.right;
		} else if (RotationVector == RotationVectorEnum.Y) {
			axis = Vector3.up;
		} else if (RotationVector == RotationVectorEnum.Z) {
			axis = Vector3.forward;
		}


		float time = (Time.fixedTime + timeOffset) * speed;
		int direction = 1;

		if (Mathf.Floor (time) % 2 == 0) {
			direction = -1;
		}

		transform.Rotate (direction * intensity * axis * rotationCurve.Evaluate (time));

	}
}
