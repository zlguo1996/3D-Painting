using UnityEngine;

#if UNITY_EDITOR
[UnityEditor.CanEditMultipleObjects]
[UnityEditor.CustomEditor(typeof(P3D_DestroyAfterTime))]
public class P3D_DestroyAfterTime_Editor : P3D_Editor<P3D_DestroyAfterTime>
{
	protected override void OnInspector()
	{
		DrawDefault("Life");
	}
}
#endif

public class P3D_DestroyAfterTime : MonoBehaviour
{
	public float Life = 1.0f;
	
	protected virtual void Update()
	{
		Life -= Time.deltaTime;
		
		if (Life <= 0.0f)
		{
			Destroy(gameObject);
		}
	}
}