using UnityEngine;
using System.Collections;

public class DestroyParentGameObject : MonoBehaviour {
	
	void DestroyObject(){
		
		Destroy (this.gameObject.transform.parent.gameObject);
	}
}
