using UnityEngine;
using System.Collections;

[AddComponentMenu("2D Toolkit/Backend/tk2dFont")]
public class tk2dFont : MonoBehaviour 
{
	public Object bmFont;
	public Material material;
	public Texture texture;
	public Texture2D gradientTexture;
    public bool dupeCaps = false; // duplicate lowercase into uc, or vice-versa, depending on which exists
	public bool flipTextureY = false;
	
	public int targetHeight = 640;
	public float targetOrthoSize = 1.0f;
	
	public int gradientCount = 1;
	
	[HideInInspector] [System.NonSerialized]
	public int numCharacters = 256;
	public bool manageMaterial = false;
	
	public tk2dFontData data;
}
