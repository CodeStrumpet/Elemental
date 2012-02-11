using UnityEngine;
using System.Collections;

[AddComponentMenu("2D Toolkit/tk2dSprite")]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
[ExecuteInEditMode]
public class tk2dSprite : tk2dBaseSprite
{
	Mesh mesh;
	Vector3[] meshVertices;
	Color[] meshColors;
	
	void Awake()
	{
		// This will not be set when instantiating in code
		// In that case, Build will need to be called
		if (collection)
		{
			// reset spriteId if outside bounds
			// this is when the sprite collection data is corrupt
			if (_spriteId < 0 || _spriteId >= collection.Count)
				_spriteId = 0;
			
			Build();
		}
	}
	
	protected void OnDestroy()
	{
		if (mesh)
		{
#if UNITY_EDITOR
			DestroyImmediate(mesh);
#else
			Destroy(mesh);
#endif
		}
	}
	
	public override void Build()
	{
		var sprite = collection.spriteDefinitions[spriteId];

		meshVertices = new Vector3[sprite.positions.Length];
        meshColors = new Color[sprite.positions.Length];
		
		SetPositions(meshVertices);
		SetColors(meshColors);
		
		Mesh newMesh = new Mesh();
		newMesh.vertices = meshVertices;
		newMesh.colors = meshColors;
		newMesh.uv = sprite.uvs;
		newMesh.triangles = sprite.indices;
		
		GetComponent<MeshFilter>().mesh = newMesh;
		mesh = GetComponent<MeshFilter>().sharedMesh;
		
		UpdateMaterial();
		CreateCollider();
	}
	
	protected override void UpdateGeometry() { UpdateGeometryImpl(); }
	protected override void UpdateColors() { UpdateColorsImpl(); }
	protected override void UpdateVertices() { UpdateVerticesImpl(); }
	
	
	protected void UpdateColorsImpl()
	{
		SetColors(meshColors);
		mesh.colors = meshColors;
	}
	
	protected void UpdateVerticesImpl()
	{
		var sprite = collection.spriteDefinitions[spriteId];
		SetPositions(meshVertices);
		mesh.vertices = meshVertices;
		mesh.uv = sprite.uvs;
		mesh.bounds = GetBounds();
	}

	protected void UpdateGeometryImpl()
	{
#if UNITY_EDITOR
		// This can happen with prefabs in the inspector
		if (mesh == null)
			return;
#endif
		
		var sprite = collection.spriteDefinitions[spriteId];
		if (mesh.vertexCount > sprite.positions.Length)
        {
            mesh.triangles = sprite.indices;
			
			meshVertices = new Vector3[sprite.positions.Length];
			meshColors = new Color[sprite.positions.Length];
			SetPositions(meshVertices);
			SetColors(meshColors);
			
			mesh.vertices = meshVertices;
			mesh.colors = meshColors;
			mesh.uv = sprite.uvs;
			mesh.bounds = GetBounds();
		}
        else
        {
			meshVertices = new Vector3[sprite.positions.Length];
			meshColors = new Color[sprite.positions.Length];
			SetPositions(meshVertices);
			SetColors(meshColors);
			
			mesh.vertices = meshVertices;
			mesh.colors = meshColors;
			mesh.uv = sprite.uvs;
			
            mesh.triangles = sprite.indices;
			mesh.bounds = GetBounds();
        }
	}
	
	protected override void UpdateMaterial()
	{
		if (renderer.sharedMaterial != collection.spriteDefinitions[spriteId].material)
			renderer.material = collection.spriteDefinitions[spriteId].material;
	}
	
	protected override int GetCurrentVertexCount()
	{
#if UNITY_EDITOR
		if (meshVertices == null)
			return 0;
#endif
		return meshVertices.Length;
	}
}
