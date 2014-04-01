using UnityEngine;
using System.Collections;
using System;
using UnityEditor;

[CustomEditor (typeof(BaseClass))]
public class BaseCharacterEditor : Editor 
{
	// Play around with inspector so we can customize

	public override void OnInspectorGUI()
	{
		var controller = target as BaseClass;

		EditorGUIUtility.LookLikeInspector();
		foreach(Vital v in controller.vitals)
		{

		}

		/*
		EditorGUIUtility.LookLikeInspector ();
		EditorGUILayout.TextField ("Text Field:", "Hello There");
		EditorGUILayout.IntField("Int Field:", 10);
		EditorGUILayout.FloatField("Float Field:", 11.1f);

		EditorGUILayout.Space();

		EditorGUIUtility.LookLikeControls();
		EditorGUILayout.TextField ("Text Field", "Hi There");
		EditorGUILayout.IntField("Int Field:", 99);
		EditorGUILayout.FloatField("Float Field:", 99.9f);
		*/
	}

}