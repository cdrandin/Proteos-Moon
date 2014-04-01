using UnityEngine;

public class FOWUpdater : MonoBehaviour
{
	public Color unexploredColor = new Color(0.05f, 0.05f, 0.05f, 1f);
	
	public Color exploredColor = new Color(0.2f, 0.2f, 0.2f, 1f);
	
	FOWSystem mFog;
	
	void LateUpdate ()
	{
		if (mFog == null)
		{
			mFog = FOWSystem.instance;
			if (mFog == null) mFog = FindObjectOfType(typeof(FOWSystem)) as FOWSystem;
		}
		
		if (mFog == null || !mFog.enabled)
		{
			enabled = false;
			return;
		}
		
		float invScale = 1f / mFog.worldSize;
		Transform t = mFog.transform;
		float x = t.position.x - mFog.worldSize * 0.5f;
		float z = t.position.z - mFog.worldSize * 0.5f;
		Vector4 p = new Vector4(-x * invScale, -z * invScale, invScale, mFog.blendFactor);
		
		Shader.SetGlobalColor("_Unexplored", unexploredColor);
		Shader.SetGlobalColor("_Explored", exploredColor);
		Shader.SetGlobalVector("_Params", p);
		Shader.SetGlobalTexture("_FogTex0", mFog.texture0);
		Shader.SetGlobalTexture("_FogTex1", mFog.texture1);
	}
}