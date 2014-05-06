using UnityEngine;
using System.Collections;

public class CenterGameObject : MonoBehaviour {
	public Transform parent;
	public Transform child;
	private Vector3 newTransform;
	// Use this for initialization
	void Start () {
		if(parent == null|| child == null){
			Debug.LogError("Please add a transform in the inspector");
		}
		else{
			print (parent.localPosition);
			print (child.localPosition);
			newTransform = parent.localPosition + child.localPosition;
			print (newTransform);
			child.localPosition = newTransform;
			parent.localPosition = Vector3.zero;

			newTransform = parent.localEulerAngles + child.localEulerAngles;
			parent.localEulerAngles = Vector3.zero;
			child.localEulerAngles = newTransform;
		}

	}

}
