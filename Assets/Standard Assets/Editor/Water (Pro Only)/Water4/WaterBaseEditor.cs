using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WaterBase))]
public class WaterBaseEditor : Editor 
{    
    public GameObject oceanBase;
    private WaterBase waterBase;
    private Material oceanMaterial = null;
    
    private SerializedObject serObj;
    private SerializedProperty sharedMaterial;
	

	public SerializedProperty waterQuality;
	public SerializedProperty edgeBlend;	
    
	public void OnEnable () 
	{
		serObj = new SerializedObject (target); 
		sharedMaterial = serObj.FindProperty("sharedMaterial"); 
		waterQuality = serObj.FindProperty("waterQuality");   		
		edgeBlend = serObj.FindProperty("edgeBlend");   		

	}
	
    public override void OnInspectorGUI () 
    {		
    	serObj.Update();	
    	
    	waterBase = (WaterBase)serObj.targetObject;
    	oceanBase = ((WaterBase)serObj.targetObject).gameObject;
    	if(!oceanBase) 
      		return;
    	
        GUILayout.Label ("Materials serialized properties tweaking", EditorStyles.miniBoldLabel);

    	EditorGUILayout.PropertyField(sharedMaterial, new GUIContent("Material"));
    	oceanMaterial = (Material)sharedMaterial.objectReferenceValue;

		if (!oceanMaterial) {
			sharedMaterial.objectReferenceValue = (Object)WaterEditorUtility.LocateValidWaterMaterial(oceanBase.transform);		
			serObj.ApplyModifiedProperties();
	        oceanMaterial = (Material)sharedMaterial.objectReferenceValue;
			if (!oceanMaterial)
				return;
		}
		
		EditorGUILayout.Separator ();
		
		GUILayout.Label ("Overall quality", EditorStyles.boldLabel);
   		EditorGUILayout.PropertyField(waterQuality, new GUIContent("Quality"));
   		EditorGUILayout.PropertyField(edgeBlend, new GUIContent("Edge blend?"));    	
		
		EditorGUILayout.Separator ();
		
		bool hasShore = oceanMaterial.HasProperty("_ShoreTex");
		
		GUILayout.Label ("Color Blending", EditorStyles.boldLabel);		
        GUILayout.Label ("Alpha values blending from realtime textures", EditorStyles.miniBoldLabel);
        
		EditorGUILayout.BeginHorizontal ();
		WaterEditorUtility.SetMaterialColor("_BaseColor", EditorGUILayout.ColorField("Refraction", WaterEditorUtility.GetMaterialColor("_BaseColor", oceanMaterial)), oceanMaterial);
		WaterEditorUtility.SetMaterialColor("_ReflectionColor", EditorGUILayout.ColorField("Reflection", WaterEditorUtility.GetMaterialColor("_ReflectionColor", oceanMaterial)), oceanMaterial);		
		EditorGUILayout.EndHorizontal ();
	        
		EditorGUILayout.Separator ();
		
    	GUILayout.Label ("Main Textures", EditorStyles.boldLabel);  	        
		EditorGUILayout.BeginHorizontal();
		WaterEditorUtility.SetMaterialTexture("_BumpMap",(Texture)EditorGUILayout.ObjectField("Normals", WaterEditorUtility.GetMaterialTexture("_BumpMap", waterBase.sharedMaterial), typeof(Texture), false), waterBase.sharedMaterial);  
		if (hasShore)
			WaterEditorUtility.SetMaterialTexture("_ShoreTex", (Texture)EditorGUILayout.ObjectField("Shore & Foam", WaterEditorUtility.GetMaterialTexture("_ShoreTex", waterBase.sharedMaterial), typeof(Texture), false), waterBase.sharedMaterial);  
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.Separator ();

		Vector4 animationTiling;
		Vector4 animationDirection;
		
		Vector2 firstTiling;
		Vector2 secondTiling;
		Vector2 firstDirection;
		Vector2 secondDirection;
    	
    	GUILayout.Label ("Waves (Bump)", EditorStyles.boldLabel);  	
        GUILayout.Label ("Via two scrolling normal maps", EditorStyles.miniBoldLabel);
   	
		animationTiling = WaterEditorUtility.GetMaterialVector("_BumpTiling", oceanMaterial);
		animationDirection = WaterEditorUtility.GetMaterialVector("_BumpDirection", oceanMaterial);
		
		firstTiling = new Vector2(animationTiling.x*100.0F,animationTiling.y*100.0F);
		secondTiling = new Vector2(animationTiling.z*100.0F,animationTiling.w*100.0F);
		
		EditorGUILayout.BeginHorizontal ();
		firstTiling.x = EditorGUILayout.FloatField("1st Tiling U", firstTiling.x);
		firstTiling.y = EditorGUILayout.FloatField("1st Tiling V", firstTiling.y);
		EditorGUILayout.EndHorizontal ();
		EditorGUILayout.BeginHorizontal ();
		secondTiling.x = EditorGUILayout.FloatField("2nd Tiling U", secondTiling.x);
		secondTiling.y = EditorGUILayout.FloatField("2nd Tiling V", secondTiling.y);
		EditorGUILayout.EndHorizontal ();
				
		firstDirection = new Vector2(animationDirection.x,animationDirection.y);
		secondDirection = new Vector2(animationDirection.z,animationDirection.w);

		EditorGUILayout.BeginHorizontal ();
		firstDirection.x = EditorGUILayout.FloatField("1st Animation U", firstDirection.x);
		firstDirection.y = EditorGUILayout.FloatField("1st Animation V", firstDirection.y);
		EditorGUILayout.EndHorizontal ();
		EditorGUILayout.BeginHorizontal ();
		secondDirection.x = EditorGUILayout.FloatField("2nd Animation U", secondDirection.x);
		secondDirection.y = EditorGUILayout.FloatField("2nd Animation V", secondDirection.y);
		EditorGUILayout.EndHorizontal ();
		
		animationTiling = new Vector4(firstTiling.x/100.0F,firstTiling.y/100.0F, secondTiling.x/100.0F,secondTiling.y/100.0F);
		animationDirection = new Vector4(firstDirection.x,firstDirection.y, secondDirection.x,secondDirection.y);
		
		WaterEditorUtility.SetMaterialVector("_BumpTiling", animationTiling, oceanMaterial);
		WaterEditorUtility.SetMaterialVector("_BumpDirection", animationDirection, oceanMaterial);    				

		Vector4 displacementParameter = WaterEditorUtility.GetMaterialVector("_DistortParams", oceanMaterial);
		Vector4 fade = WaterEditorUtility.GetMaterialVector("_InvFadeParemeter", oceanMaterial);

		displacementParameter.x = EditorGUILayout.Slider("Intensity", displacementParameter.x, -4.0F, 4.0F);
		displacementParameter.y = EditorGUILayout.Slider("Displacement", displacementParameter.y, -0.5F, 0.5F);
		fade.z = EditorGUILayout.Slider("Distance fade", fade.z, 0.0f, 0.5f);			
		
		EditorGUILayout.Separator ();
		
    	GUILayout.Label ("Fresnel", EditorStyles.boldLabel);	
		
		if(!oceanMaterial.HasProperty("_Fresnel")) {
			if(oceanMaterial.HasProperty("_FresnelScale")) {
				float fresnelScale = EditorGUILayout.Slider("Intensity", WaterEditorUtility.GetMaterialFloat("_FresnelScale", oceanMaterial), 0.1F, 4.0F);
				WaterEditorUtility.SetMaterialFloat("_FresnelScale", fresnelScale, oceanMaterial);
			}			
			displacementParameter.z = EditorGUILayout.Slider("Power", displacementParameter.z, 0.1F, 10.0F);
			displacementParameter.w = EditorGUILayout.Slider("Bias", displacementParameter.w, -3.0F, 3.0F);
		}
		else
		{
			Texture fresnelTex = (Texture)EditorGUILayout.ObjectField(
					"Ramp", 
					(Texture)WaterEditorUtility.GetMaterialTexture("_Fresnel", 
					oceanMaterial), 
					typeof(Texture),
					false);
			WaterEditorUtility.SetMaterialTexture("_Fresnel", fresnelTex, oceanMaterial);
		}
		
		EditorGUILayout.Separator ();
		    			
		WaterEditorUtility.SetMaterialVector("_DistortParams", displacementParameter, oceanMaterial);
			
		if (edgeBlend.boolValue)
		{
	    	GUILayout.Label ("Fading", EditorStyles.boldLabel);	
			
			fade.x = EditorGUILayout.Slider("Edge fade", fade.x, 0.0f, 0.3f);
			if(hasShore)
				fade.y = EditorGUILayout.Slider("Shore fade", fade.y, 0.0f, 0.3f);			
			fade.w = EditorGUILayout.Slider("Extinction fade", fade.w, 0.0f, 2.5f);			

			WaterEditorUtility.SetMaterialVector("_InvFadeParemeter", fade, oceanMaterial);
		}
		EditorGUILayout.Separator ();					
								
		if(oceanMaterial.HasProperty("_Foam")) {
    		GUILayout.Label ("Foam", EditorStyles.boldLabel);		
		
			Vector4 foam = WaterEditorUtility.GetMaterialVector("_Foam", oceanMaterial);
			
			foam.x = EditorGUILayout.Slider("Intensity", foam.x, 0.0F, 1.0F);
			foam.y = EditorGUILayout.Slider("Cutoff", foam.y, 0.0F, 1.0F);
			
			WaterEditorUtility.SetMaterialVector("_Foam", foam, oceanMaterial);			
		}
				
    	serObj.ApplyModifiedProperties();
    }
       
}