#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Reflection;

public class FogVolumeCreator : Editor {


	[UnityEditor.MenuItem("GameObject/Create Other/Fog Volume")]
	static public void CreateFogVolume()
	{
		GameObject FogVolume = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //Icon stuff
        Texture Icon = Resources.Load("FogVolumeIcon") as Texture;
        Icon.hideFlags = HideFlags.HideAndDontSave;
        var editorGUI = typeof(EditorGUIUtility);
        var bindingFlags = BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.NonPublic;
        var args = new object[] { FogVolume, Icon };
        editorGUI.InvokeMember("SetIconForObject", bindingFlags, null, null, args);

		FogVolume.name = "Fog Volume";
		FogVolume.AddComponent ("FogVolume");
		FogVolume.renderer.castShadows = false;
		FogVolume.renderer.receiveShadows = false;
		Selection.activeObject = FogVolume;
		DestroyImmediate (FogVolume.GetComponent <BoxCollider> ());
		if (UnityEditor.SceneView.currentDrawingSceneView) UnityEditor.SceneView.currentDrawingSceneView.MoveToView(FogVolume.transform);
	}

	

}
#endif