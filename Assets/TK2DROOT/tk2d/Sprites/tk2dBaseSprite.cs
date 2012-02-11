using UnityEngine;
using System.Collections;

[AddComponentMenu("2D Toolkit/Backend/tk2dBaseSprite")]
public abstract class tk2dBaseSprite : MonoBehaviour
{
    public tk2dSpriteCollectionData collection;
	
	[SerializeField] protected Color _color = Color.white;
	[SerializeField] protected Vector3 _scale = new Vector3(1.0f, 1.0f, 1.0f);
	[SerializeField] protected int _spriteId = 0;
	public bool pixelPerfect = false;
	
	public BoxCollider boxCollider = null;
	public MeshCollider meshCollider = null;
	public Vector3[] meshColliderPositions = null;
	public Mesh meshColliderMesh = null;
	
	public Color color 
	{ 
		get { return _color; } 
		set 
		{
			if (value != _color)
			{
				_color = value;
				UpdateColors();
			}
		} 
	}
	
	public Vector3 scale 
	{ 
		get { return _scale; } 
		set
		{
			if (value != _scale)
			{
				_scale = value;
				UpdateVertices();
#if UNITY_EDITOR
				EditMode__CreateCollider();
#else
				UpdateCollider();
#endif
			}
		}
	}
	
	public int spriteId 
	{ 
		get { return _spriteId; } 
		set 
		{
			if (value != _spriteId)
			{
				value = Mathf.Clamp(value, 0, collection.spriteDefinitions.Length - 1);
				if (GetCurrentVertexCount() != collection.spriteDefinitions[value].indices.Length)
				{
					_spriteId = value;
					UpdateGeometry();
				}
				else
				{
					_spriteId = value;
					UpdateVertices();
				}
				UpdateMaterial();
				UpdateCollider();
			}
		} 
	}
	
	public void SwitchCollectionAndSprite(tk2dSpriteCollectionData newCollection, int newSpriteId)
	{
		if (collection != newCollection)
		{
			collection = newCollection;
		}
		
		_spriteId = -1; // force an update
		spriteId = newSpriteId;
		
		if (collection != newCollection)
		{
			UpdateMaterial();
		}
	}
	
	public void MakePixelPerfect()
	{
		float s = 1.0f;
		tk2dPixelPerfectHelper pph = tk2dPixelPerfectHelper.inst;
		if (pph)
		{
			if (pph.CameraIsOrtho)
			{
				s = pph.scaleK;
			}
			else
			{
				s = pph.scaleK + pph.scaleD * transform.position.z;
			}
		}
		else if (Camera.main)
		{
			if (Camera.main.isOrthoGraphic)
			{
				s = Camera.main.orthographicSize;
			}
			else
			{
				float zdist = (transform.position.z - Camera.main.transform.position.z);
				s = tk2dPixelPerfectHelper.CalculateScaleForPerspectiveCamera(Camera.main.fov, zdist);
			}
		}
		
		s *= collection.invOrthoSize;
		
		scale = new Vector3(Mathf.Sign(scale.x) * s, Mathf.Sign(scale.y) * s, Mathf.Sign(scale.z) * s);
	}	
		
	
	protected abstract void UpdateMaterial(); // update material when switching spritecollection
	protected abstract void UpdateColors(); // reupload color data only
	protected abstract void UpdateVertices(); // reupload vertex data only
	protected abstract void UpdateGeometry(); // update full geometry (including indices)
	protected abstract int  GetCurrentVertexCount(); // return current vertex count
	
	public abstract void Build();
	
	public int GetSpriteIdByName(string name)
	{
		for (int i = 0; i < collection.Count; ++i)
		{
			if (collection.spriteDefinitions[i].name == name) return i;
		}
		return 0; // default to first sprite
	}
	
	protected int GetNumVertices()
	{
		return collection.spriteDefinitions[spriteId].positions.Length;
	}
	
	protected int GetNumIndices()
	{
		return collection.spriteDefinitions[spriteId].indices.Length;
	}
	
	protected void SetPositions(Vector3[] dest)	
	{
		var sprite = collection.spriteDefinitions[spriteId];
		int numVertices = GetNumVertices();
		for (int i = 0; i < numVertices; ++i)
		{
			dest[i].x = sprite.positions[i].x * _scale.x;
			dest[i].y = sprite.positions[i].y * _scale.y;
			dest[i].z = sprite.positions[i].z * _scale.z;
		}
	}
	
	protected void SetColors(Color[] dest)
	{
		Color c = _color;
        if (collection.premultipliedAlpha) { c.r *= c.a; c.g *= c.a; c.b *= c.a; }
		int numVertices = GetNumVertices();
		for (int i = 0; i < numVertices; ++i)
			dest[i] = c;
	}
	
	protected Bounds GetBounds()
	{
		var sprite = collection.spriteDefinitions[_spriteId];
		return new Bounds(new Vector3(sprite.boundsData[0].x * _scale.x, sprite.boundsData[0].y * _scale.y, sprite.boundsData[0].z * _scale.z),
		                  new Vector3(sprite.boundsData[1].x * _scale.x, sprite.boundsData[1].y * _scale.y, sprite.boundsData[1].z * _scale.z));
	}
	
	// Unity functions
	public void Start()
	{
		if (pixelPerfect)
			MakePixelPerfect();
	}	
	
	
	// Collider setup
	
	protected virtual bool NeedBoxCollider() { return false; }
	
	protected void UpdateCollider()
	{
		var sprite = collection.spriteDefinitions[_spriteId];
		if (boxCollider != null)
		{
			if (sprite.colliderType == tk2dSpriteDefinition.ColliderType.Box)
			{
				if (boxCollider == null)
					boxCollider = gameObject.AddComponent<BoxCollider>();
				boxCollider.center = new Vector3(sprite.colliderVertices[0].x * _scale.x, sprite.colliderVertices[0].y * _scale.y, sprite.colliderVertices[0].z * _scale.z);
				boxCollider.extents = new Vector3(sprite.colliderVertices[1].x * _scale.x, sprite.colliderVertices[1].y * _scale.y, sprite.colliderVertices[1].z * _scale.z);
			}
			else if (sprite.colliderType == tk2dSpriteDefinition.ColliderType.Unset)
			{
				// Don't do anything here, for backwards compatibility
			}
			else // in all cases, if the collider doesn't match is requested, null it out
			{
				if (boxCollider != null)
				{
					// move the box far far away, boxes with zero extents still collide
					boxCollider.center = new Vector3(0, 0, -100000.0f);
					boxCollider.extents = Vector3.zero;
				}
			}
		}
	}
	
	// This is separate to UpdateCollider, as UpdateCollider can only work with BoxColliders, and will NOT create colliders
	protected void CreateCollider()
	{
		var sprite = collection.spriteDefinitions[_spriteId];
		if (sprite.colliderType == tk2dSpriteDefinition.ColliderType.Unset)
		{
			// do not attempt to create or modify anything if it is Unset
			return;
		}

		// User has created a collider
		if (collider != null)
		{
			boxCollider = GetComponent<BoxCollider>();
			meshCollider = GetComponent<MeshCollider>();
		}
		
		if ((NeedBoxCollider() || sprite.colliderType == tk2dSpriteDefinition.ColliderType.Box) && meshCollider == null)
		{
			if (boxCollider == null)
				boxCollider = gameObject.AddComponent<BoxCollider>();
		}
		else if (sprite.colliderType == tk2dSpriteDefinition.ColliderType.Mesh && boxCollider == null)
		{
			// this should not be updated again (apart from scale changes in the editor, where we force regeneration of colliders)
			if (meshCollider == null)
				meshCollider = gameObject.AddComponent<MeshCollider>();
			if (meshColliderMesh == null)
				meshColliderMesh = new Mesh();
			
			
			meshColliderPositions = new Vector3[sprite.colliderVertices.Length];
			for (int i = 0; i < meshColliderPositions.Length; ++i)
				meshColliderPositions[i] = new Vector3(sprite.colliderVertices[i].x * _scale.x, sprite.colliderVertices[i].y * _scale.y, sprite.colliderVertices[i].z * _scale.z);
			meshColliderMesh.vertices = meshColliderPositions;
			
			float s = _scale.x * _scale.y * _scale.z;
			
			meshColliderMesh.triangles = (s >= 0.0f)?sprite.colliderIndicesFwd:sprite.colliderIndicesBack;
			meshCollider.sharedMesh = meshColliderMesh;
			meshCollider.convex = sprite.colliderConvex;
			
			// this is required so our mesh pivot is at the right point
			if (rigidbody) rigidbody.centerOfMass = Vector3.zero;
		}
		else if (sprite.colliderType != tk2dSpriteDefinition.ColliderType.None)
		{
			// This warning is not applicable in the editor
			if (Application.isPlaying)
			{
				Debug.LogError("Invalid mesh collider on sprite, please remove and try again.");
			}
		}
		
		UpdateCollider();
	}
	
#if UNITY_EDITOR
	public void EditMode__CreateCollider()
	{
		var sprite = collection.spriteDefinitions[_spriteId];
		if (sprite.colliderType == tk2dSpriteDefinition.ColliderType.Unset)
			return;
		
		PhysicMaterial physicsMaterial = collider?collider.sharedMaterial:null;
		bool isTrigger = collider?collider.isTrigger:false;
		
		if (boxCollider)
		{
			DestroyImmediate(boxCollider);
		}
		if (meshCollider)
		{
			DestroyImmediate(meshCollider);
			if (meshColliderMesh)
				DestroyImmediate(meshColliderMesh);
		}

		CreateCollider();
		
		if (collider)
		{
			collider.isTrigger = isTrigger;
			collider.material = physicsMaterial;
		}
	}
#endif
}
