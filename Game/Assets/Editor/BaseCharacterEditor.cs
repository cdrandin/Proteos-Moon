using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor (typeof(BaseClass))]
public class BaseCharacterEditor : Editor {

	public override void OnInspectorGUI()
	{
		BaseClass base_class_script = (BaseClass)target;
	}
}
