using UnityEngine;
using System.Collections;

[System.Serializable]
public class tk2dBatchedSprite
{
	public string name = ""; // for editing
	public int spriteId = 0;
	public Quaternion rotation = Quaternion.identity;
	public Vector3 position = Vector3.zero;
	public Vector3 localScale = Vector3.one;
	public Color color = Color.white;
	public bool alwaysPixelPerfect = false;
}

[AddComponentMenu("2D Toolkit/tk2dStaticSpriteBatcher")]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
[ExecuteInEditMode]
public class tk2dStaticSpriteBatcher : MonoBehaviour
{
	public tk2dBatchedSprite[] batchedSprites = null;
	public tk2dSpriteCollectionData spriteCollection = null;
	Mesh mesh = null;
	Mesh colliderMesh = null;
	
	void Awake()
	{
		Build();
	}
	
	public void Build()
	{
		if (mesh)
		{
#if UNITY_EDITOR
			DestroyImmediate(mesh);
#else
			Destroy(mesh);
#endif
			mesh = null;
		}
		
		if (colliderMesh)
		{
#if UNITY_EDITOR
			DestroyImmediate(colliderMesh);
#else
			Destroy(colliderMesh);
#endif
			colliderMesh = null;
		}
		
		if (!spriteCollection || batchedSprites == null || batchedSprites.Length == 0)
		{
			mesh = new Mesh();
			GetComponent<MeshFilter>().mesh = mesh;
		}
		else
		{
			int numVertices = 0;
			int numIndices = 0;
			foreach (var sprite in batchedSprites) 
			{
				var spriteData = spriteCollection.spriteDefinitions[sprite.spriteId];
				numVertices += spriteData.positions.Length;
				numIndices += spriteData.indices.Length;
			}
			
			Vector3[] meshVertices = new Vector3[numVertices];
			Color[] meshColors = new Color[numVertices];
			Vector2[] meshUvs = new Vector2[numVertices];
			int[] meshIndices = new int[numIndices];
			
			int currVertex = 0;
			int currIndex = 0;
			
			foreach (var sprite in batchedSprites)
			{
				var spriteData = spriteCollection.spriteDefinitions[sprite.spriteId];
				
				for (int i = 0; i < spriteData.indices.Length; ++i)
				{
					meshIndices[currIndex + i] = currVertex + spriteData.indices[i];
				}
				
				for (int i = 0; i < spriteData.positions.Length; ++i)
				{
					Vector3 pos = spriteData.positions[i];
					pos.x *= sprite.localScale.x;
					pos.y *= sprite.localScale.y;
					pos.z *= sprite.localScale.z;
					pos = sprite.rotation * pos;
					pos += sprite.position;
					meshVertices[currVertex + i] = pos;
					meshUvs[currVertex + i] = spriteData.uvs[i];
					meshColors[currVertex + i] = sprite.color;
				}
				
				currIndex += spriteData.indices.Length;
				currVertex += spriteData.positions.Length;
			}
			
			mesh = new Mesh();
	        mesh.vertices = meshVertices;
	        mesh.uv = meshUvs;
	        mesh.colors = meshColors;
	        mesh.triangles = meshIndices;
			mesh.RecalculateBounds();
			GetComponent<MeshFilter>().mesh = mesh;
			
			// Only one material supported for now
			if (renderer.sharedMaterial != spriteCollection.materials[0])
				renderer.material = spriteCollection.materials[0];
			
			// Build physics mesh
			BuildPhysicsMesh();
		}
	}
	
	void BuildPhysicsMesh()
	{
		MeshCollider meshCollider = GetComponent<MeshCollider>();
		if (meshCollider != null && collider != meshCollider)
		{
			// Already has a collider
			return;
		}
		
		int numIndices = 0;
		int numVertices = 0;
		
		// first pass, count required vertices and indices
		foreach (var sprite in batchedSprites) 
		{
			var spriteData = spriteCollection.spriteDefinitions[sprite.spriteId];
			if (spriteData.colliderType == tk2dSpriteDefinition.ColliderType.Box)
			{
				numIndices += 6 * 4;
				numVertices += 8;
			}
			else if (spriteData.colliderType == tk2dSpriteDefinition.ColliderType.Mesh)
			{
				numIndices += spriteData.colliderIndicesFwd.Length;
				numVertices += spriteData.colliderVertices.Length;
			}
		}
		
		if (numIndices == 0)
		{
			if (colliderMesh)
			{
#if UNITY_EDTIOR
				DestroyImmediate(colliderMesh);
#else
				Destroy(colliderMesh);
#endif
			}
			
			return;
		}
		
		if (meshCollider == null)
			meshCollider = gameObject.AddComponent<MeshCollider>();
		if (colliderMesh == null)
			colliderMesh = new Mesh();
		colliderMesh.Clear();
		
		// second pass, build composite mesh
		int currVertex = 0;
		Vector3[] vertices = new Vector3[numVertices];
		int currIndex = 0;
		int[] indices = new int[numIndices];
		
		foreach (var sprite in batchedSprites) 
		{
			var spriteData = spriteCollection.spriteDefinitions[sprite.spriteId];
			if (spriteData.colliderType == tk2dSpriteDefinition.ColliderType.Box)
			{
				Vector3 origin = new Vector3(spriteData.colliderVertices[0].x * sprite.localScale.x, spriteData.colliderVertices[0].y * sprite.localScale.y, spriteData.colliderVertices[0].z * sprite.localScale.z);
				Vector3 extents = new Vector3(spriteData.colliderVertices[1].x * sprite.localScale.x, spriteData.colliderVertices[1].y * sprite.localScale.y, spriteData.colliderVertices[1].z * sprite.localScale.z);
				Vector3 min = origin - extents;
				Vector3 max = origin + extents;
				
				vertices[currVertex + 0] = sprite.rotation * new Vector3(min.x, min.y, min.z) + sprite.position;
				vertices[currVertex + 1] = sprite.rotation * new Vector3(min.x, min.y, max.z) + sprite.position;
				vertices[currVertex + 2] = sprite.rotation * new Vector3(max.x, min.y, min.z) + sprite.position;
				vertices[currVertex + 3] = sprite.rotation * new Vector3(max.x, min.y, max.z) + sprite.position;
				vertices[currVertex + 4] = sprite.rotation * new Vector3(min.x, max.y, min.z) + sprite.position;
				vertices[currVertex + 5] = sprite.rotation * new Vector3(min.x, max.y, max.z) + sprite.position;
				vertices[currVertex + 6] = sprite.rotation * new Vector3(max.x, max.y, min.z) + sprite.position;
				vertices[currVertex + 7] = sprite.rotation * new Vector3(max.x, max.y, max.z) + sprite.position;
				
				int[] indicesBack = { 0, 1, 2, 2, 1, 3, 6, 5, 4, 7, 5, 6, 3, 7, 6, 2, 3, 6, 4, 5, 1, 4, 1, 0 };
				int[] indicesFwd = { 2, 1, 0, 3, 1, 2, 4, 5, 6, 6, 5, 7, 6, 7, 3, 6, 3, 2, 1, 5, 4, 0, 1, 4 };
				
				float scl = sprite.localScale.x * sprite.localScale.y * sprite.localScale.z;
				int[] srcIndices = (scl >= 0)?indicesFwd:indicesBack;
				
				for (int i = 0; i < srcIndices.Length; ++i)
					indices[currIndex + i] = currVertex + srcIndices[i];
					
				currIndex += 6 * 4;
				currVertex += 8;
			}
			else if (spriteData.colliderType == tk2dSpriteDefinition.ColliderType.Mesh)
			{
				for (int i = 0; i < spriteData.colliderVertices.Length; ++i)
				{
					Vector3 pos = spriteData.colliderVertices[i];
					pos.x *= sprite.localScale.x;
					pos.y *= sprite.localScale.y;
					pos.z *= sprite.localScale.z;
					pos = sprite.rotation * pos;
					pos += sprite.position;
					
					vertices[currVertex + i] = pos;
				}
				
				float scl = sprite.localScale.x * sprite.localScale.y * sprite.localScale.z;
				int[] srcIndices = (scl >= 0)?spriteData.colliderIndicesFwd:spriteData.colliderIndicesBack;
				
				for (int i = 0; i < srcIndices.Length; ++i)
				{
					indices[currIndex + i] = currVertex + srcIndices[i];
				}
				
				currIndex += spriteData.colliderIndicesFwd.Length;
				currVertex += spriteData.colliderVertices.Length;
			}
		}
		
		colliderMesh.vertices = vertices;
		colliderMesh.triangles = indices;
		
		meshCollider.sharedMesh = colliderMesh;
	}
}


