using UnityEngine;
using System.Collections;

[System.Serializable]
public class tk2dSpriteColliderIsland
{
	public bool connected = true;
	public Vector2[] points;
	
	public bool IsValid()
	{
		if (connected)
		{
			return points.Length >= 3;
		}
		else
		{
			return points.Length >= 2;
		}
	}
	
	public void CopyFrom(tk2dSpriteColliderIsland src)
	{
		connected = src.connected;
		
		points = new Vector2[src.points.Length];
		for (int i = 0; i < points.Length; ++i)
			points[i] = src.points[i];		
	}
	
	public bool CompareTo(tk2dSpriteColliderIsland src)
	{
		if (connected != src.connected) return false;
		if (points.Length != src.points.Length) return false;
		for (int i = 0; i < points.Length; ++i)
			if (points[i] != src.points[i]) return false;
		return true;
	}
}

[System.Serializable]
public class tk2dSpriteCollectionDefinition
{
    public enum Anchor
    {
		UpperLeft,
		UpperCenter,
		UpperRight,
		MiddleLeft,
		MiddleCenter,
		MiddleRight,
		LowerLeft,
		LowerCenter,
		LowerRight,
		Custom
    }
	
	public enum Pad
	{
		Default,
		BlackZeroAlpha,
		Extend,
	}
	
	public enum ColliderType
	{
		Unset,		// don't try to create or destroy anything
		None,		// nothing will be created, if something exists, it will be destroyed
		BoxTrimmed, // box, trimmed to cover visible region
		BoxCustom, 	// box, with custom values provided by user
		Polygon, 	// polygon, can be concave
	}
	
	public enum PolygonColliderCap
	{
		None,
		FrontAndBack,
		Front,
		Back,
	}
	
	public enum ColliderColor
	{
		Default, // default unity color scheme
		Red,
		White,
		Black
	}
	
	public string name = "";
	
    public bool additive = false;
    public Vector3 scale = new Vector3(1,1,1);
    
	[HideInInspector]
    public Texture2D texture;
	
	[HideInInspector] [System.NonSerialized]
	public Texture2D thumbnailTexture;
	
	public Anchor anchor = Anchor.MiddleCenter;
	public float anchorX, anchorY;
    public Object overrideMesh;
	
	public bool dice = false;
	public int diceUnitX = 64;
	public int diceUnitY = 0;
	
	public Pad pad = Pad.Default;
	
	public bool fromSpriteSheet = false;
	public bool extractRegion = false;
	public int regionX, regionY, regionW, regionH;
	public int regionId;
	
	public ColliderType colliderType = ColliderType.Unset;
	public Vector2 boxColliderMin, boxColliderMax;
	public tk2dSpriteColliderIsland[] polyColliderIslands;
	public PolygonColliderCap polyColliderCap = PolygonColliderCap.None;
	public bool colliderConvex = false;
	public bool colliderSmoothSphereCollisions = false;
	public ColliderColor colliderColor = ColliderColor.Default;
	
	public void CopyFrom(tk2dSpriteCollectionDefinition src)
	{
		name = src.name;
		
		additive = src.additive;
		scale = src.scale;
		texture = src.texture;
		anchor = src.anchor;
		anchorX = src.anchorX;
		anchorY = src.anchorY;
		overrideMesh = src.overrideMesh;
		dice = src.dice;
		diceUnitX = src.diceUnitX;
		diceUnitY = src.diceUnitY;
		pad = src.pad;
		
		fromSpriteSheet = src.fromSpriteSheet;
		extractRegion = src.extractRegion;
		regionX = src.regionX;
		regionY = src.regionY;
		regionW = src.regionW;
		regionH = src.regionH;
		regionId = src.regionId;
		
		colliderType = src.colliderType;
		boxColliderMin = src.boxColliderMin;
		boxColliderMax = src.boxColliderMax;
		polyColliderCap = src.polyColliderCap;
		
		colliderColor = src.colliderColor;
		colliderConvex = src.colliderConvex;
		colliderSmoothSphereCollisions = src.colliderSmoothSphereCollisions;
		
		if (src.polyColliderIslands != null)
		{
			polyColliderIslands = new tk2dSpriteColliderIsland[src.polyColliderIslands.Length];
			for (int i = 0; i < polyColliderIslands.Length; ++i)
			{
				polyColliderIslands[i] = new tk2dSpriteColliderIsland();
				polyColliderIslands[i].CopyFrom(src.polyColliderIslands[i]);
			}
		}
		else
		{
			polyColliderIslands = null;
		}
	}
	
	public bool CompareTo(tk2dSpriteCollectionDefinition src)
	{
		if (name != src.name) return false;
		
		if (additive != src.additive) return false;
		if (scale != src.scale) return false;
		if (texture != src.texture) return false;
		if (anchor != src.anchor) return false;
		if (anchorX != src.anchorX) return false;
		if (anchorY != src.anchorY) return false;
		if (overrideMesh != src.overrideMesh) return false;
		if (dice != src.dice) return false;
		if (diceUnitX != src.diceUnitX) return false;
		if (diceUnitY != src.diceUnitY) return false;
		if (pad != src.pad) return false;
		
		if (fromSpriteSheet != src.fromSpriteSheet) return false;
		if (extractRegion != src.extractRegion) return false;
		if (regionX != src.regionX) return false;
		if (regionY != src.regionY) return false;
		if (regionW != src.regionW) return false;
		if (regionH != src.regionH) return false;
		if (regionId != src.regionId) return false;
		
		if (colliderType != src.colliderType) return false;
		if (boxColliderMin != src.boxColliderMin) return false;
		if (boxColliderMax != src.boxColliderMax) return false;
		
		if (polyColliderIslands != src.polyColliderIslands) return false;
		if (polyColliderIslands != null && src.polyColliderIslands != null)
		{
			if (polyColliderIslands.Length != src.polyColliderIslands.Length) return false;
			for (int i = 0; i < polyColliderIslands.Length; ++i)
				if (!polyColliderIslands[i].CompareTo(src.polyColliderIslands[i])) return false;
		}
		
		if (polyColliderCap != src.polyColliderCap) return false;
		
		if (colliderColor != src.colliderColor) return false;
		if (colliderSmoothSphereCollisions != src.colliderSmoothSphereCollisions) return false;
		if (colliderConvex != src.colliderConvex) return false;
		
		return true;
	}	
}

[System.Serializable]
public class tk2dSpriteCollectionDefault
{
    public bool additive = false;
    public Vector3 scale = new Vector3(1,1,1);
	public tk2dSpriteCollectionDefinition.Anchor anchor = tk2dSpriteCollectionDefinition.Anchor.MiddleCenter;
	public tk2dSpriteCollectionDefinition.Pad pad = tk2dSpriteCollectionDefinition.Pad.Default;	
	
	public tk2dSpriteCollectionDefinition.ColliderType colliderType = tk2dSpriteCollectionDefinition.ColliderType.None;
}

[System.Serializable]
public class tk2dSpriteSheetSource
{
    public enum Anchor
    {
		UpperLeft,
		UpperCenter,
		UpperRight,
		MiddleLeft,
		MiddleCenter,
		MiddleRight,
		LowerLeft,
		LowerCenter,
		LowerRight,
    }	
	
	public Texture2D texture;
	public int tilesX, tilesY;
	public int numTiles = 0;
	public Anchor anchor = Anchor.MiddleCenter;
	public tk2dSpriteCollectionDefinition.Pad pad = tk2dSpriteCollectionDefinition.Pad.Default;
	public Vector3 scale = new Vector3(1,1,1);

	public tk2dSpriteCollectionDefinition.ColliderType colliderType = tk2dSpriteCollectionDefinition.ColliderType.None;
	
	public void CopyFrom(tk2dSpriteSheetSource src)
	{
		texture = src.texture;
		tilesX = src.tilesX;
		tilesY = src.tilesY;
		numTiles = src.numTiles;
		anchor = src.anchor;
		pad = src.pad;
		scale = src.scale;
		colliderType = src.colliderType;
	}
	
	public bool CompareTo(tk2dSpriteSheetSource src)
	{
		if (texture != src.texture) return false;
		if (tilesX != src.tilesX) return false;
		if (tilesY != src.tilesY) return false;
		if (numTiles != src.numTiles) return false;
		if (anchor != src.anchor) return false;
		if (pad != src.pad) return false;
		if (scale != src.scale) return false;
		if (colliderType != src.colliderType) return false;
		
		return true;
	}
}

[AddComponentMenu("2D Toolkit/Backend/tk2dSpriteCollection")]
public class tk2dSpriteCollection : MonoBehaviour 
{
    // legacy data
    [HideInInspector]
    public tk2dSpriteCollectionDefinition[] textures;

    // new method
    public Texture2D[] textureRefs;
	public tk2dSpriteSheetSource[] spriteSheets;
	
	public tk2dSpriteCollectionDefault defaults;
	
	[HideInInspector]
	public int maxTextureSize = 1024;
	
	public enum TextureCompression
	{
		Uncompressed,
		Reduced16Bit,
		Compressed
	}
	[HideInInspector]
	public TextureCompression textureCompression = TextureCompression.Uncompressed;
	
	[HideInInspector]
	public int atlasWidth, atlasHeight;
	[HideInInspector]
	public float atlasWastage;
	[HideInInspector]
	public bool allowMultipleAtlases = false;
	
	[HideInInspector]
    public tk2dSpriteCollectionDefinition[] textureParams;
    
	public tk2dSpriteCollectionData spriteCollection;
    public bool premultipliedAlpha = true;
	
	public Material[] atlasMaterials;
	public Texture2D[] atlasTextures;

	public int targetHeight = 640;
	public float targetOrthoSize = 1.0f;
	
	public bool pixelPerfectPointSampled = false;
	
	public float physicsDepth = 0.1f;
	
	[HideInInspector]
	public bool autoUpdate = true;
}
