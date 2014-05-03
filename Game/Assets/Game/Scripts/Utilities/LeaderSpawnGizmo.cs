using UnityEngine;
using System.Collections;

public class LeaderSpawnGizmo : MonoBehaviour 
{
	public float radius;

	void OnDrawGizmosSelected() 
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawSphere(this.transform.position, radius);
	}
}
