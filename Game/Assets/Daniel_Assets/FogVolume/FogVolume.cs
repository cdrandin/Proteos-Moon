using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif
/*CHANGE LOG
 * Added cute icon
 * fixed size hack
 * fixed build warnings
 * added noise color tweaks (contrast, intensity)
 * 
 * 
 * TODO
 * Per object keywords. Needs custom material editor?
 * */
[ExecuteInEditMode]

class FogVolume : MonoBehaviour
{
		GameObject FogVolumeGameObject;
		Vector3 VolumeSize;
		Material FogMaterial;
		[SerializeField]
		Color
				InscatteringColor = Color.white, FogColor = new Color (.5f, .6f, .7f, 1);
		
    [SerializeField]
		float
	Visibility = 5, InscateringExponent = 15, InscatteringIntensity = 2, InscatteringStartDistance = 400, InscatteringTransitionWideness = 1, /*FogMinHeight = 0, FogMaxHeight=20,*/ _3DNoiseScale = 300, _3DNoiseStepSize = 50, NoiseIntensity=1, NoiseContrast=-1;
	
		[SerializeField]
		Light
				Sun;
		[SerializeField]
		int
				DrawOrder = 0;
		
    [SerializeField]
        bool
#if UNITY_EDITOR
 HideWireframe = true,
                #endif
                EnableInscattering = false, EnableNoise=false;
				
				public Vector4 Speed=new Vector4(0,0,0,0);
				public Vector4 Stretch=new Vector4(0,0,0,0);


                void Start ()
		{			
				if (!FogMaterial)
						FogMaterial = new Material (Shader.Find ("Hidden/FogVolume"));
						FogMaterial.name = "Fog Material";
				FogVolumeGameObject = this.gameObject;
				FogVolumeGameObject.renderer.sharedMaterial = FogMaterial;				
				//Camera.main.depthTextureMode |= DepthTextureMode.Depth;
                FogMaterial.hideFlags=HideFlags.HideAndDontSave;

                ToggleKeyword();
               
		}
        static public void Wireframe(GameObject obj, bool Enable)
        {
            #if UNITY_EDITOR
            EditorUtility.SetSelectedWireframeHidden(obj.renderer, Enable);
            #endif
        }
		void Update ()
		{
             #if UNITY_EDITOR

				ToggleKeyword ();
	
         
                Wireframe (FogVolumeGameObject, HideWireframe);
			#endif
				FogMaterial.SetColor ("_Color", FogColor);
				FogMaterial.SetColor ("_InscatteringColor", InscatteringColor);

				if (Sun) {
						InscatteringIntensity = Mathf.Max (0, InscatteringIntensity);
						FogMaterial.SetFloat ("_InscatteringIntensity", InscatteringIntensity);
						FogMaterial.SetVector ("L", -Sun.transform.forward);
				}
				InscateringExponent = Mathf.Max (1, InscateringExponent);
				FogMaterial.SetFloat ("_InscateringExponent", InscateringExponent);
				InscatteringTransitionWideness = Mathf.Max (1, InscatteringTransitionWideness);
				FogMaterial.SetFloat ("InscatteringTransitionWideness", InscatteringTransitionWideness);
            /*Someday
                FogMaterial.SetFloat("FogMaxHeight", FogMaxHeight);
                FogMaterial.SetFloat("FogMinHeight", FogMinHeight);*/
                if (EnableNoise)
                {
                    FogMaterial.SetFloat("gain", NoiseIntensity);
                    FogMaterial.SetFloat("threshold", NoiseContrast);
                    FogMaterial.SetFloat("_3DNoiseScale", _3DNoiseScale * .001f);
                    FogMaterial.SetFloat("_3DNoiseStepSize", _3DNoiseStepSize * .001f);
                    FogMaterial.SetVector("Speed", Speed);
                    FogMaterial.SetVector("Stretch", new Vector4(1, 1, 1, 1) + Stretch * .01f);
                }
				InscatteringStartDistance = Mathf.Max (0, InscatteringStartDistance);
				FogMaterial.SetFloat ("InscatteringStartDistance", InscatteringStartDistance);
				VolumeSize = FogVolumeGameObject.transform.lossyScale;
				//bug fix. If the 3 axes values are equal, something gets broken :/
				transform.localScale = new Vector3 ((float)decimal.Round ((decimal)VolumeSize.x, 2), VolumeSize.y, VolumeSize.z);
                FogMaterial.SetVector("_BoxMin", VolumeSize * -.5f);
                FogMaterial.SetVector("_BoxMax", VolumeSize * .5f);
				Visibility = Mathf.Max (0, Visibility);
				FogMaterial.SetFloat ("_Visibility", Visibility);	
				renderer.sortingOrder = DrawOrder;

	}

		void ToggleKeyword ()
		{
            if(EnableNoise)
                FogMaterial.EnableKeyword("_FOG_VOLUME_NOISE");
            else
                FogMaterial.DisableKeyword("_FOG_VOLUME_NOISE");
            
            if (EnableInscattering)
                FogMaterial.EnableKeyword("_FOG_VOLUME_INSCATTERING");
            else
                FogMaterial.DisableKeyword("_FOG_VOLUME_INSCATTERING");
		}
}
