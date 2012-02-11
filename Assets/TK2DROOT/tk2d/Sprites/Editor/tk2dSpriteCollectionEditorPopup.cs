using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class tk2dSpriteCollectionEditorPopup : EditorWindow 
{
	private static tk2dSpriteCollectionEditorPopup inst = null;
	
	tk2dSpriteCollection gen;
	int currSprite = 0;
	
	public void SetGenerator(tk2dSpriteCollection _gen)
	{
		gen = _gen;
	}
	
	bool alphaBlend = true;
	float displayScale = 1.0f;
	bool previewFoldoutEnabled = true;
	bool drawCollider = true;
	bool drawAnchor = true;
	bool drawColliderNormals = true;
	
	void OnEnable()
	{
		inst = this;
	}
	
	void OnDestroy()
	{
		tk2dSpriteThumbnailCache.ReleaseSpriteThumbnailCache();
		inst = null;
	}
	
    void OnGUI() 
	{
		EditorGUI.indentLevel = 0;
		
		if (!gen)
		{
			EditorGUILayout.BeginVertical();
			GUILayout.Label("not loaded");
			EditorGUILayout.EndVertical();
			return;
		}
		
		if (currSprite < 0 || currSprite >= gen.textureRefs.Length || currSprite >= gen.textureParams.Length) 
		{	
			currSprite = 0;
		}
		
		EditorGUILayout.BeginHorizontal();
		
		EditorGUILayout.BeginVertical( GUILayout.MaxWidth(256.0f) );

		GUILayout.Space(8.0f);
		if (GUILayout.Button("Commit")) tk2dSpriteCollectionBuilder.Rebuild(gen);
		
		
		
		// First half
		GUILayout.Space(32.0f);
		tk2dSpriteCollectionDefinition param = null;
		DrawSpritePropertiesPanel(ref currSprite, ref param);
		
		// Physics
		GUILayout.Space(32.0f);
		DrawPhysicsPropertiesPanel(param);
		
		EditorGUILayout.EndVertical();
		
		GUILayout.Space(8.0f);
		
		
		// Preview part
		EditorGUILayout.BeginVertical();
		GUILayout.Space(8.0f);
		
		if (gen.spriteCollection.version < 1)
		{
			GUILayout.Label("No preview data.\nPlease rebuild sprite collection.");
		}
		else
		{
			previewFoldoutEnabled = EditorGUILayout.Foldout(previewFoldoutEnabled, "Preview");
			if (previewFoldoutEnabled)
			{
				var tex = tk2dSpriteThumbnailCache.GetThumbnailTexture(gen.spriteCollection, currSprite);
				DrawPreviewFoldout(param, tex);
			}
		}

		EditorGUILayout.EndVertical();
		EditorGUILayout.EndHorizontal();
    }
	
	void InlineMessage(string str)
	{
		Color bg = GUI.backgroundColor;
		GUI.backgroundColor = new Color32(154, 176, 203, 255);
		GUILayout.TextArea(str);
		GUI.backgroundColor = bg;	
	}
	
	void DrawPhysicsPropertiesPanel(tk2dSpriteCollectionDefinition param)
	{
		var oldColliderType = param.colliderType;
		param.colliderType = (tk2dSpriteCollectionDefinition.ColliderType)EditorGUILayout.EnumPopup("Collider Type", param.colliderType);

		int w = 4, h = 4;
		if (param.texture != null)
		{
			w = param.texture.width;
			h = param.texture.height;
		}
		
		if (param.colliderType == tk2dSpriteCollectionDefinition.ColliderType.BoxCustom ||
		    param.colliderType == tk2dSpriteCollectionDefinition.ColliderType.Polygon)
		{
			drawCollider = EditorGUILayout.Toggle("Draw collider", drawCollider);
			if (param.colliderType == tk2dSpriteCollectionDefinition.ColliderType.Polygon)
			{
				drawColliderNormals = EditorGUILayout.Toggle("Draw normals", drawColliderNormals);
			}
			currentColliderColor = param.colliderColor = (tk2dSpriteCollectionDefinition.ColliderColor)EditorGUILayout.EnumPopup("Collider color", param.colliderColor);
		}
		
		GUILayout.Space(8);
		
		if (param.colliderType == tk2dSpriteCollectionDefinition.ColliderType.BoxCustom)
		{
			// If just switching to this
			if (oldColliderType != param.colliderType)
			{
				param.boxColliderMin = new Vector2(0, 0);
				param.boxColliderMax = new Vector2(w, h);
			}
			
			param.boxColliderMin = EditorGUILayout.Vector2Field("Min", param.boxColliderMin);
			param.boxColliderMax = EditorGUILayout.Vector2Field("Max", param.boxColliderMax);
		}
		
		if (param.colliderType == tk2dSpriteCollectionDefinition.ColliderType.Polygon)
		{
			EditorGUILayout.PrefixLabel("Properties");
			param.polyColliderCap = (tk2dSpriteCollectionDefinition.PolygonColliderCap)EditorGUILayout.EnumPopup("Collider Cap", param.polyColliderCap);
			param.colliderConvex = EditorGUILayout.Toggle("Convex", param.colliderConvex);
			param.colliderSmoothSphereCollisions = EditorGUILayout.Toggle(new GUIContent("SmoothSphereCollisions", "Smooth Sphere Collisions"), param.colliderSmoothSphereCollisions);

			bool reset = (param.polyColliderIslands == null || param.polyColliderIslands.Length == 0)?true:false;
			int polyCount = 0;
			
			if (!reset)
			{
				foreach (var island in param.polyColliderIslands)
				{
					int numPoints = island.connected?island.points.Length:(island.points.Length-1);
					polyCount += numPoints * 2;
				}
				
				GUILayout.Space(8);
				
				string islandStr = (param.polyColliderIslands.Length == 1)?"island":"islands";
				GUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Info", param.polyColliderIslands.Length.ToString() + " " + islandStr);
				if (GUILayout.Button("Reset"))
					reset = true;
				GUILayout.EndHorizontal();
				EditorGUILayout.LabelField("", polyCount.ToString() + " polys");
			}
			
			if (reset || oldColliderType != param.colliderType &&
			    (param.polyColliderIslands == null ||
			     param.polyColliderIslands.Length == 0 ||
			    !param.polyColliderIslands[0].IsValid()) )
			{
				param.polyColliderIslands = new tk2dSpriteColliderIsland[1];
				param.polyColliderIslands[0] = new tk2dSpriteColliderIsland();
				param.polyColliderIslands[0].connected = true;
				
				Vector2[] p = new Vector2[4];
				p[0] = new Vector2(0, 0);
				p[1] = new Vector2(0, h);
				p[2] = new Vector2(w, h);
				p[3] = new Vector2(w, 0);
				param.polyColliderIslands[0].points = p;
				
				Repaint();
			}
		}
		
		if (param.colliderType == tk2dSpriteCollectionDefinition.ColliderType.Polygon)
		{
			GUILayout.Space(32);
			
			InlineMessage("Points" +
						  "\nClick drag - move point" +
						  "\nClick hold + delete/bkspace - delete point" +
						  "\nDouble click on line - add point");			

			InlineMessage("Islands" +
						  "\nClick hold point + X - delete island" +
						  "\nPress C - create island at cursor" + 
			              "\nClick hold point + T - toggle connected" +
			              "\nClick hold point + F - flip island");
		}
	}
	
	void DrawSpritePropertiesPanel(ref int currSprite, ref tk2dSpriteCollectionDefinition param)
	{
		currSprite = tk2dEditorUtility.SpriteSelectorPopup(null, currSprite, gen.spriteCollection);
		param = gen.textureParams[currSprite];
		
		if (param.fromSpriteSheet)
		{
			EditorGUILayout.LabelField("SpriteSheet", "Frame: " + param.regionId);
			EditorGUILayout.LabelField("Name", param.name);
		}
		else
		{
			param.name = EditorGUILayout.TextField("Name", param.name);
		}
		
		if (!param.fromSpriteSheet)
		{
			param.additive = EditorGUILayout.Toggle("Additive", param.additive);
			param.scale = EditorGUILayout.Vector3Field("Scale", param.scale);
			param.anchor = (tk2dSpriteCollectionDefinition.Anchor)EditorGUILayout.EnumPopup("Anchor", param.anchor);
			if (param.anchor == tk2dSpriteCollectionDefinition.Anchor.Custom)
			{
				EditorGUILayout.BeginHorizontal();
				param.anchorX = EditorGUILayout.FloatField("AnchorX", param.anchorX);
				bool roundAnchorX = GUILayout.Button("R", GUILayout.MaxWidth(32));
				EditorGUILayout.EndHorizontal();
	
				EditorGUILayout.BeginHorizontal();
				param.anchorY = EditorGUILayout.FloatField("AnchorY", param.anchorY);
				bool roundAnchorY = GUILayout.Button("R", GUILayout.MaxWidth(32));
				EditorGUILayout.EndHorizontal();
				
				drawAnchor = EditorGUILayout.Toggle("Draw anchor", drawAnchor);
				
				if (roundAnchorX) param.anchorX = Mathf.Round(param.anchorX);
				if (roundAnchorY) param.anchorY = Mathf.Round(param.anchorY);
			}
			
			if (!gen.allowMultipleAtlases)
			{
				param.dice = EditorGUILayout.Toggle("Dice", param.dice);
				if (param.dice)
				{
					param.diceUnitX = EditorGUILayout.IntField("X", param.diceUnitX);
					param.diceUnitY = EditorGUILayout.IntField("Y", param.diceUnitY);
				}
			}
			
			param.pad = (tk2dSpriteCollectionDefinition.Pad)EditorGUILayout.EnumPopup("Pad", param.pad);
			EditorGUILayout.Separator();
		}
		
		// Warning message
		if (gen.allowMultipleAtlases)
		{
			Color bg = GUI.backgroundColor;
			GUI.backgroundColor = new Color(1.0f, 0.7f, 0.0f, 1.0f);
			GUILayout.TextArea("NOTE: Dicing is not allowed when multiple atlas build is enabled.");
			GUI.backgroundColor = bg;			
		}
	}
	
	Vector2 textureScrollPos = Vector2.zero;
	void DrawPreviewFoldout(tk2dSpriteCollectionDefinition param, Texture2D displayTexture)
	{
		// Top half of inspector
		GUILayout.BeginVertical(GUILayout.MaxWidth(192.0f));
		alphaBlend = GUILayout.Toggle(alphaBlend, "AlphaBlend");
		
		EditorGUILayout.BeginHorizontal();
		displayScale = EditorGUILayout.FloatField("PreviewScale", displayScale);
		displayScale = Mathf.Max(displayScale, 1.0f);
		if (GUILayout.Button("Reset")) displayScale = 1.0f;
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		GUILayout.Label("Width: " + displayTexture.width); GUILayout.Label("Height: " + displayTexture.height); 
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		if (gen.spriteCollection != null && gen.spriteCollection.spriteDefinitions != null)
		{
			EditorGUILayout.BeginHorizontal();
			var thisSpriteData = gen.spriteCollection.spriteDefinitions[currSprite];
			GUILayout.Label("Vertices: " + thisSpriteData.positions.Length); GUILayout.Label("Triangles: " + thisSpriteData.indices.Length / 3);
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
		}
		
		InlineMessage("Mouse wheel - zoom\n" +
					  "Middle mouse drag - pan");
		
		GUILayout.EndVertical();
		
		
		GUILayout.Space(16.0f);
		
		// 
		// Texture view
		//
		Rect rect = GUILayoutUtility.GetRect(128.0f, 128.0f, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
		if (rect.width > displayTexture.width * displayScale) rect.width = displayTexture.width * displayScale + 2;
		if (rect.height > displayTexture.height * displayScale) rect.height = displayTexture.height * displayScale + 2;
		rect.width -= 1.0f; // contract for outline
		
		// middle mouse drag and scroll zoom
		if (rect.Contains(Event.current.mousePosition))
		{
			if (Event.current.type == EventType.MouseDrag && Event.current.button == 2)
			{
				textureScrollPos -= Event.current.delta * displayScale;
				Event.current.Use();
				Repaint();
			}
			if (Event.current.type == EventType.ScrollWheel)
			{
				displayScale -= Event.current.delta.y;
				Event.current.Use();
				Repaint();
			}
		}
		
		
		// Draw outline
		{
			Vector3[] pt = new Vector3[] {
				new Vector3(rect.x - 1, rect.y - 1, 0.0f),
				new Vector3(rect.x + rect.width + 1, rect.y - 1, 0.0f),
				new Vector3(rect.x + rect.width + 1, rect.y + rect.height + 1, 0.0f),
				new Vector3(rect.x - 1, rect.y + rect.height + 1, 0.0f),
				new Vector3(rect.x - 1, rect.y - 1, 0.0f)
			};
			Color c = Handles.color;
			Handles.color = Color.black;
			Handles.DrawPolyLine(pt);
			Handles.color = c;
		}	
		
		textureScrollPos = GUI.BeginScrollView(rect, textureScrollPos, new Rect(0, 0, (displayTexture.width) * displayScale, (displayTexture.height) * displayScale));
		Rect textureRect = new Rect(0, 0, displayTexture.width * displayScale, displayTexture.height * displayScale);
		GUI.DrawTexture(textureRect, displayTexture, ScaleMode.ScaleAndCrop, alphaBlend);

		if (drawCollider)
		{
			if (param.colliderType == tk2dSpriteCollectionDefinition.ColliderType.BoxCustom)
				DrawCustomBoxColliderEditor(textureRect, param, displayTexture);
			if (param.colliderType == tk2dSpriteCollectionDefinition.ColliderType.Polygon)
				DrawPolygonColliderEditor(textureRect, param, displayTexture);
		}

		if (drawAnchor && param.anchor == tk2dSpriteCollectionDefinition.Anchor.Custom)
		{
			Color handleColor = new Color(0,0,0,0.2f);
			Color lineColor = Color.white;
			Vector2 anchor = new Vector2(param.anchorX, param.anchorY);
			
			anchor = tk2dGuiUtility.PositionHandle(99999, anchor * displayScale, 12.0f, handleColor, handleColor ) / displayScale;

			Color oldColor = Handles.color;
			Handles.color = lineColor;
			Handles.DrawLine(new Vector3(0, anchor.y * displayScale, 0), new Vector3(displayTexture.width * displayScale, anchor.y * displayScale, 0));
			Handles.DrawLine(new Vector3(anchor.x * displayScale, 0, 0), new Vector3(anchor.x * displayScale, displayTexture.height * displayScale, 0));
			Handles.color = oldColor;

			// constrain
			param.anchorX = Mathf.Clamp(Mathf.Round(anchor.x), 0.0f, displayTexture.width);
			param.anchorY = Mathf.Clamp(Mathf.Round(anchor.y), 0.0f, displayTexture.height);
			Repaint();
		}
		
		GUI.EndScrollView();
	}
	
	Color[] _handleInactiveColors = new Color[] { 
		new Color32(127, 201, 122, 255), // default
		new Color32(180, 0, 0, 255), // red
		new Color32(255, 255, 255, 255), // white
		new Color32(32, 32, 32, 255), // black
	};
	
	Color[] _handleActiveColors = new Color[] {
		new Color32(228, 226, 60, 255),
		new Color32(255, 0, 0, 255),
		new Color32(255, 0, 0, 255),
		new Color32(96, 0, 0, 255),
	};
	
	tk2dSpriteCollectionDefinition.ColliderColor currentColliderColor = tk2dSpriteCollectionDefinition.ColliderColor.Default;
	Color handleInactiveColor { get { return _handleInactiveColors[(int)currentColliderColor]; } }
	Color handleActiveColor { get { return _handleActiveColors[(int)currentColliderColor]; } }
	
	Vector2 ClosestPointOnLine(Vector2 p, Vector2 p1, Vector2 p2)
	{
		float magSq = (p2 - p1).sqrMagnitude;
		if (magSq < float.Epsilon)
			return p1;
		
		float u = ((p.x - p1.x) * (p2.x - p1.x) + (p.y - p1.y) * (p2.y - p1.y)) / magSq;
		if (u < 0.0f || u > 1.0f)
			return p1;
		
		return p1 + (p2 - p1) * u;
	}
	
	void DrawPolygonColliderEditor(Rect r, tk2dSpriteCollectionDefinition param, Texture2D tex)
	{
		Color previousHandleColor = Handles.color;
		bool insertPoint = false;
		
		if (Event.current.clickCount == 2 && Event.current.type == EventType.MouseDown)
		{
			insertPoint = true;
			Event.current.Use();
		}
		
		if (r.Contains(Event.current.mousePosition) && Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.C)
		{
			Vector2 min = Event.current.mousePosition / displayScale - new Vector2(16.0f, 16.0f);
			Vector3 max = Event.current.mousePosition / displayScale + new Vector2(16.0f, 16.0f);
			
			min.x = Mathf.Clamp(min.x, 0, tex.width * displayScale);
			min.y = Mathf.Clamp(min.y, 0, tex.height * displayScale);
			max.x = Mathf.Clamp(max.x, 0, tex.width * displayScale);
			max.y = Mathf.Clamp(max.y, 0, tex.height * displayScale);
			
			tk2dSpriteColliderIsland island = new tk2dSpriteColliderIsland();
			island.connected = true;
			
			Vector2[] p = new Vector2[4];
			p[0] = new Vector2(min.x, min.y);
			p[1] = new Vector2(min.x, max.y);
			p[2] = new Vector2(max.x, max.y);
			p[3] = new Vector2(max.x, min.y);
			island.points = p;
			
			System.Array.Resize(ref param.polyColliderIslands, param.polyColliderIslands.Length + 1);
			param.polyColliderIslands[param.polyColliderIslands.Length - 1] = island;
			
			Event.current.Use();
		}
		
		// Draw outline lines
		float closestDistanceSq = 1.0e32f;
		Vector2 closestPoint = Vector2.zero;
		int closestPreviousPoint = 0;
		
		int deletedIsland = -1;
		for (int islandId = 0; islandId < param.polyColliderIslands.Length; ++islandId)
		{
			var island = param.polyColliderIslands[islandId];
	
			Handles.color = handleInactiveColor;

			Vector2 ov = (island.points.Length>0)?island.points[island.points.Length-1]:Vector2.zero;
			for (int i = 0; i < island.points.Length; ++i)
			{
				Vector2 v = island.points[i];
				
				// Don't draw last connection if its not connected
				if (!island.connected && i == 0)
				{
					ov = v;
					continue;
				}
				
				if (insertPoint)
				{
					Vector2 closestPointToCursor = ClosestPointOnLine(Event.current.mousePosition, ov * displayScale, v * displayScale);
					float lengthSq = (closestPointToCursor - Event.current.mousePosition).sqrMagnitude;
					if (lengthSq < closestDistanceSq)
					{
						closestDistanceSq = lengthSq;
						closestPoint = (closestPointToCursor) / displayScale;
						closestPreviousPoint = i;
					}
				}
				
				if (drawColliderNormals)
				{
					Vector2 l = (ov - v).normalized;
					Vector2 n = new Vector2(l.y, -l.x);
					Vector2 c = (v + ov) * 0.5f * displayScale;
					Handles.DrawLine(c, c + n * 16.0f);
				}
				
				Handles.DrawLine(v * displayScale, ov * displayScale);
				ov = v;
			}
			Handles.color = previousHandleColor;
			
			if (insertPoint && closestDistanceSq < 16.0f)
			{
				var tmpList = new List<Vector2>(island.points);
				tmpList.Insert(closestPreviousPoint, closestPoint);
				island.points = tmpList.ToArray();
				Repaint();
			}
			
			int deletedIndex = -1;
			bool flipIsland = false;
			
			for (int i = 0; i < island.points.Length; ++i)
			{
				Vector3 cp = island.points[i];
				KeyCode keyCode = KeyCode.None;
				cp = tk2dGuiUtility.PositionHandle(16433 + i, cp * displayScale, 4.0f, handleInactiveColor, handleActiveColor, out keyCode) / displayScale;
				
				if (keyCode == KeyCode.Backspace || keyCode == KeyCode.Delete)
				{
					deletedIndex = i;
				}
				
				if (keyCode == KeyCode.X)
				{
					deletedIsland = islandId;
				}
				
				if (keyCode == KeyCode.T)
				{
					island.connected = !island.connected;
					if (island.connected && island.points.Length < 3)
					{
						Vector2 pp = (island.points[1] - island.points[0]);
						float l = pp.magnitude;
						pp.Normalize();
						Vector2 nn = new Vector2(pp.y, -pp.x);
						nn.y = Mathf.Clamp(nn.y, 0, tex.height);
						nn.x = Mathf.Clamp(nn.x, 0, tex.width);
						System.Array.Resize(ref island.points, island.points.Length + 1);
						island.points[island.points.Length - 1] = (island.points[0] + island.points[1]) * 0.5f + nn * l * 0.5f;
					}
				}
				
				if (keyCode == KeyCode.F)
				{
					flipIsland = true;
				}
				
				cp.x = Mathf.Round(cp.x);
				cp.y = Mathf.Round(cp.y);
				
				// constrain
				cp.x = Mathf.Clamp(cp.x, 0.0f, tex.width);
				cp.y = Mathf.Clamp(cp.y, 0.0f, tex.height);
				
				island.points[i] = cp;
			}
			
			if (flipIsland)
			{
				System.Array.Reverse(island.points);
			}
			
			if (deletedIndex != -1 && 
			    ((island.connected && island.points.Length > 3) ||
			    (!island.connected && island.points.Length > 2)) )
			{
				var tmpList = new List<Vector2>(island.points);
				tmpList.RemoveAt(deletedIndex);
				island.points = tmpList.ToArray();
			}			
		}
		
		// Can't delete the last island
		if (deletedIsland != -1 && param.polyColliderIslands.Length > 1)
		{
			var tmpIslands = new List<tk2dSpriteColliderIsland>(param.polyColliderIslands);
			tmpIslands.RemoveAt(deletedIsland);
			param.polyColliderIslands = tmpIslands.ToArray();
		}
	}
	
	void DrawCustomBoxColliderEditor(Rect r, tk2dSpriteCollectionDefinition param, Texture2D tex)
	{
		Vector3[] pt = new Vector3[] {
			new Vector3(param.boxColliderMin.x * displayScale, param.boxColliderMin.y * displayScale, 0.0f),
			new Vector3(param.boxColliderMax.x * displayScale, param.boxColliderMin.y * displayScale, 0.0f),
			new Vector3(param.boxColliderMax.x * displayScale, param.boxColliderMax.y * displayScale, 0.0f),
			new Vector3(param.boxColliderMin.x * displayScale, param.boxColliderMax.y * displayScale, 0.0f),
		};
		Color32 transparentColor = handleInactiveColor;
		transparentColor.a = 10;
		Handles.DrawSolidRectangleWithOutline(pt, transparentColor, handleInactiveColor);
		
		// Draw grab handles
		Vector3 handlePos;
		
		// Draw top handle
		handlePos = (pt[0] + pt[1]) * 0.5f;
		handlePos = tk2dGuiUtility.PositionHandle(16433 + 0, handlePos, 4.0f, handleInactiveColor, handleActiveColor) / displayScale;
		param.boxColliderMin.y = handlePos.y;
		if (param.boxColliderMin.y > param.boxColliderMax.y) param.boxColliderMin.y = param.boxColliderMax.y;

		// Draw bottom handle
		handlePos = (pt[2] + pt[3]) * 0.5f;
		handlePos = tk2dGuiUtility.PositionHandle(16433 + 1, handlePos, 4.0f, handleInactiveColor, handleActiveColor) / displayScale;
		param.boxColliderMax.y = handlePos.y;
		if (param.boxColliderMax.y < param.boxColliderMin.y) param.boxColliderMax.y = param.boxColliderMin.y;

		// Draw left handle
		handlePos = (pt[0] + pt[3]) * 0.5f;
		handlePos = tk2dGuiUtility.PositionHandle(16433 + 2, handlePos, 4.0f, handleInactiveColor, handleActiveColor) / displayScale;
		param.boxColliderMin.x = handlePos.x;
		if (param.boxColliderMin.x > param.boxColliderMax.x) param.boxColliderMin.x = param.boxColliderMax.x;

		// Draw right handle
		handlePos = (pt[1] + pt[2]) * 0.5f;
		handlePos = tk2dGuiUtility.PositionHandle(16433 + 3, handlePos, 4.0f, handleInactiveColor, handleActiveColor) / displayScale;
		param.boxColliderMax.x = handlePos.x;
		if (param.boxColliderMax.x < param.boxColliderMin.x) param.boxColliderMax.x = param.boxColliderMin.x;

		param.boxColliderMax.x = Mathf.Round(param.boxColliderMax.x);
		param.boxColliderMax.y = Mathf.Round(param.boxColliderMax.y);
		param.boxColliderMin.x = Mathf.Round(param.boxColliderMin.x);
		param.boxColliderMin.y = Mathf.Round(param.boxColliderMin.y);		

		// constrain
		param.boxColliderMax.x = Mathf.Clamp(param.boxColliderMax.x, 0.0f, tex.width);
		param.boxColliderMax.y = Mathf.Clamp(param.boxColliderMax.y, 0.0f, tex.height);
		param.boxColliderMin.x = Mathf.Clamp(param.boxColliderMin.x, 0.0f, tex.width);
		param.boxColliderMin.y = Mathf.Clamp(param.boxColliderMin.y, 0.0f, tex.height);
	}
	
	public static void OnRebuild()
	{
		if (inst)
		{
			inst.Repaint();
		}
	}
}
