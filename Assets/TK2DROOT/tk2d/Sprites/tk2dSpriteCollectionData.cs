using UnityEngine;
using System.Collections;

[System.Serializable]
public class tk2dSpriteDefinition
{
	public enum ColliderType
	{
		Unset,	// Do not create or destroy anything
		None,	// If a collider exists, it will be destroyed
		Box,
		Mesh,
	}
	
	public string name;
	public Vector3[] boundsData;
    public Vector3[] positions;
    public Vector2[] uvs;
    public int[] indices = new int[] { 0, 3, 1, 2, 3, 0 };
	public Material material;
	
	public string sourceTextureGUID;
	public bool extractRegion;
	public int regionX, regionY, regionW, regionH;
	
	public bool flipped;
	
	// Collider properties
	public ColliderType colliderType = ColliderType.None;
	// v0 and v1 are center and size respectively for box colliders
	// otherwise, they are simply an array of vertices
	public Vector3[] colliderVertices; 
	public int[] colliderIndicesFwd;
	public int[] colliderIndicesBack;
	public bool colliderConvex;
	public bool colliderSmoothSphereCollisions;
}

[AddComponentMenu("2D Toolkit/Backend/tk2dSpriteCollectionData")]
public class tk2dSpriteCollectionData : MonoBehaviour 
{
	public const int CURRENT_VERSION = 1;
	
	[HideInInspector]
	public int version;
	
    [HideInInspector]
    public tk2dSpriteDefinition[] spriteDefinitions;
	
    [HideInInspector]
    public bool premultipliedAlpha;
	
	// legacy data
    [HideInInspector]
	public Material material;	
	
	[HideInInspector]
	public Material[] materials;
	
	[HideInInspector]
	public Texture[] textures;
	
	[HideInInspector]
	public bool allowMultipleAtlases;
	
	[HideInInspector]
	public string spriteCollectionGUID;
	
	[HideInInspector]
	public string spriteCollectionName;

	[HideInInspector]
	public float invOrthoSize = 1.0f;
	
	[HideInInspector]
	public int buildKey = 0;
	
	[HideInInspector]
	public string guid = "";
	
    public int Count { get { return spriteDefinitions.Length; } }
}
