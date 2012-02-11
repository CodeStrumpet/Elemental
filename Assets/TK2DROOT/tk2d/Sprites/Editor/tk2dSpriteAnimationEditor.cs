using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor(typeof(tk2dSpriteAnimation))]
class tk2dSpriteAnimationEditor : Editor
{
	int currentClip = 0;
	Vector2 scrollPosition = Vector3.zero;
	
	bool initialized = false;
	string[] allSpriteCollectionNames = null;
	tk2dSpriteCollectionIndex[] spriteCollectionIndex = null;
	
	void InitializeInspector()
	{
		if (!initialized)
		{
			var index = tk2dEditorUtility.GetOrCreateIndex().GetSpriteCollectionIndex();
			if (index != null)
			{
				allSpriteCollectionNames = new string[index.Length];
				
				for (int i = 0; i < index.Length; ++i)
				{
					allSpriteCollectionNames[i] = index[i].name;
				}
			}
			spriteCollectionIndex = index;
			
			initialized = true;
		}
	}
	
	void OnDestroy()
	{
		tk2dSpriteThumbnailCache.ReleaseSpriteThumbnailCache();
	}
	
	Dictionary<tk2dSpriteCollectionData, int> indexLookup = new Dictionary<tk2dSpriteCollectionData, int>();
	int GetSpriteCollectionId(tk2dSpriteCollectionData data)
	{
		if (indexLookup.ContainsKey(data))
			return indexLookup[data];
		
		var guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(data));	
		for (int i = 0; i < spriteCollectionIndex.Length; ++i)
		{
			if (spriteCollectionIndex[i].spriteCollectionDataGUID == guid)
			{
				indexLookup[data] = i;
				return i;
			}
		}
		return 0; // default
	}
	
	tk2dSpriteCollectionData GetSpriteCollection(int index)
	{
		GameObject scgo = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(spriteCollectionIndex[index].spriteCollectionDataGUID), typeof(GameObject)) as GameObject;
		return scgo.GetComponent<tk2dSpriteCollectionData>();
	}
	
    public override void OnInspectorGUI()
    {
		InitializeInspector();
		
		if (spriteCollectionIndex == null || allSpriteCollectionNames == null)
		{
			GUILayout.Label("data not found");
			if (GUILayout.Button("Refresh"))
			{
				initialized = false;
				InitializeInspector();
			}
			return;
		}

		
        tk2dSpriteAnimation anim = (tk2dSpriteAnimation)target;
        EditorGUILayout.BeginVertical();

		EditorGUI.indentLevel = 1;
		EditorGUILayout.BeginVertical();
		
		if (anim.clips.Length == 0)
		{
			if (GUILayout.Button("Add clip"))
			{
				anim.clips = new tk2dSpriteAnimationClip[1];
				anim.clips[0] = new tk2dSpriteAnimationClip();
				anim.clips[0].name = "New Clip 0";
				anim.clips[0].frames = new tk2dSpriteAnimationFrame[1];
				
				anim.clips[0].frames[0] = new tk2dSpriteAnimationFrame();
				anim.clips[0].frames[0].spriteCollection = GetSpriteCollection(0);
				anim.clips[0].frames[0].spriteId = 0;
			}
		}
		else // has anim clips
		{
			// All clips
			string[] allClipNames = new string[anim.clips.Length];
			for (int i = 0; i < anim.clips.Length; ++i)
				allClipNames[i] = anim.clips[i].name;
			currentClip = Mathf.Clamp(currentClip, 0, anim.clips.Length);
			
			#region AddAndDeleteClipButtons
			EditorGUILayout.BeginHorizontal();
			currentClip = EditorGUILayout.Popup("Clips", currentClip, allClipNames);
			
			// Add new clip
			if (GUILayout.Button("+", GUILayout.MaxWidth(28), GUILayout.MaxHeight(14)))
			{
				int previousClipId = currentClip;
				
				// try to find an empty slot
				currentClip = -1;
				for (int i = 0; i < anim.clips.Length; ++i)
				{
					if (anim.clips[i].name.Length == 0)
					{
						currentClip = i;
						break;
					}
				}
				
				if (currentClip == -1)
				{
					tk2dSpriteAnimationClip[] clips = new tk2dSpriteAnimationClip[anim.clips.Length + 1];
					for (int i = 0; i < anim.clips.Length; ++i)
						clips[i] = anim.clips[i];
					currentClip = anim.clips.Length;
					clips[currentClip] = new tk2dSpriteAnimationClip();
					anim.clips = clips;
				}
				
				string uniqueName = "New Clip ";
				int uniqueId = 0;
				for (int i = 0; i < anim.clips.Length; ++i)
				{
					string uname = uniqueName + uniqueId.ToString();
					if (anim.clips[i].name == uname)
					{
						uniqueId++;
						i = -1;
						continue;
					}
				}
				
				anim.clips[currentClip] = new tk2dSpriteAnimationClip();
				anim.clips[currentClip].name = uniqueName + uniqueId.ToString();
				anim.clips[currentClip].fps = 15;
				anim.clips[currentClip].wrapMode = tk2dSpriteAnimationClip.WrapMode.Loop;
				anim.clips[currentClip].frames = new tk2dSpriteAnimationFrame[1];
				tk2dSpriteAnimationFrame frame = new tk2dSpriteAnimationFrame();
				if (previousClipId < anim.clips.Length
				    && anim.clips[previousClipId] != null 
				    && anim.clips[previousClipId].frames != null
				    && anim.clips[previousClipId].frames.Length != 0
				    && anim.clips[previousClipId].frames[anim.clips[previousClipId].frames.Length - 1] != null
				    && anim.clips[previousClipId].frames[anim.clips[previousClipId].frames.Length - 1].spriteCollection != null)
				{
					var previousClip = anim.clips[previousClipId];
					var lastFrame = previousClip.frames[previousClip.frames.Length - 1];
					frame.spriteCollection = lastFrame.spriteCollection;
					frame.spriteId = lastFrame.spriteId;
				}
				else
				{
					frame.spriteCollection = GetSpriteCollection(0);
					frame.spriteId = 0;
				}
				anim.clips[currentClip].frames[0] = frame;
				
				GUI.changed = true;
			}
			
			// Delete clip
			if (GUILayout.Button("-", GUILayout.MaxWidth(28), GUILayout.MaxHeight(14)))
			{
				anim.clips[currentClip].name = "";
				anim.clips[currentClip].frames = new tk2dSpriteAnimationFrame[0];
				
				currentClip = 0;
				// find first non zero clip
				for (int i = 0; i < anim.clips.Length; ++i)
				{
					if (anim.clips[i].name != "")
					{
						currentClip = i;
						break;
					}
				}
				
				GUI.changed = true;
			}
			EditorGUILayout.EndHorizontal();
			#endregion
			
			#region PruneClipList
			// Prune clip list
			int lastActiveClip = 0;
			for (int i = 0; i < anim.clips.Length; ++i)
			{
				if ( !(anim.clips[i].name == "" && anim.clips[i].frames != null && anim.clips[i].frames.Length == 0) ) lastActiveClip = i;
			}
			if (lastActiveClip != anim.clips.Length - 1)
			{
				System.Array.Resize<tk2dSpriteAnimationClip>(ref anim.clips, lastActiveClip + 1);
				GUI.changed = true;
			}
			#endregion
			
			// If anything has changed up to now, redraw
			if (GUI.changed)
			{
				EditorUtility.SetDirty(anim);
				Repaint();
				return;
			}
			
			EditorGUI.indentLevel = 2;
			tk2dSpriteAnimationClip clip = anim.clips[currentClip];

			// Clip properties
			
			// Name
			clip.name = EditorGUILayout.TextField("Name", clip.name);
			
			#region NumberOfFrames
			// Number of frames
			int clipNumFrames = (clip.frames != null)?clip.frames.Length:0;
			int newFrameCount = 0;
			if (clip.wrapMode == tk2dSpriteAnimationClip.WrapMode.Single)
			{
				newFrameCount = 1; // only one frame, no need to display
			}
			else
			{
				int maxFrameCount = 100;
				string[] numFrameStr = new string[maxFrameCount];
				for (int i = 1; i < maxFrameCount; ++i)
					numFrameStr[i] = i.ToString();
				
				newFrameCount = EditorGUILayout.Popup("Num Frames", clipNumFrames, numFrameStr);
				if (newFrameCount == 0) newFrameCount = 1; // minimum = 1
			}
			
			if (newFrameCount != clipNumFrames)
			{
				tk2dSpriteAnimationFrame[] frames = new tk2dSpriteAnimationFrame[newFrameCount];
				
				int c1 = Mathf.Min(clipNumFrames, frames.Length);
				for (int i = 0; i < c1; ++i)
				{
					frames[i] = new tk2dSpriteAnimationFrame();
					frames[i].spriteCollection = clip.frames[i].spriteCollection;
					frames[i].spriteId = clip.frames[i].spriteId;
				}
				if (c1 > 0)
				{
					for (int i = c1; i < frames.Length; ++i)
					{
						frames[i] = new tk2dSpriteAnimationFrame();
						frames[i].spriteCollection = clip.frames[c1-1].spriteCollection;
						frames[i].spriteId = clip.frames[c1-1].spriteId;
					}
				}
				else
				{
					for (int i = 0; i < frames.Length; ++i)
					{
						frames[i] = new tk2dSpriteAnimationFrame();
						frames[i].spriteCollection = GetSpriteCollection(0);
						frames[i].spriteId = 0;
					}
				}
				
				clip.frames = frames;
				clipNumFrames = newFrameCount;
			}
			#endregion
			
			// Frame rate
			if (clip.wrapMode != tk2dSpriteAnimationClip.WrapMode.Single)
				clip.fps = EditorGUILayout.FloatField("Frame rate", clip.fps);
			
			// Wrap mode
			clip.wrapMode = (tk2dSpriteAnimationClip.WrapMode)EditorGUILayout.EnumPopup("Wrap mode", clip.wrapMode);
			if (clip.wrapMode == tk2dSpriteAnimationClip.WrapMode.LoopSection)
			{
				clip.loopStart = EditorGUILayout.IntField("Loop start", clip.loopStart);
				clip.loopStart = Mathf.Clamp(clip.loopStart, 0, clip.frames.Length - 1);
			}
			
			#region DrawFrames
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel("Frames");
			GUILayout.FlexibleSpace();
			
			// Reverse
			if (clip.wrapMode != tk2dSpriteAnimationClip.WrapMode.Single &&
			    GUILayout.Button("Reverse"))
			{
				System.Array.Reverse(clip.frames);
				GUI.changed = true;
			}
			
			// Auto fill
			if (clip.wrapMode != tk2dSpriteAnimationClip.WrapMode.Single && clip.frames.Length >= 1)
			{
				AutoFill(clip);
			}
			
			if (GUILayout.Button(tk2dPreferences.inst.horizontalAnimDisplay?"H":"V", GUILayout.MaxWidth(24)))
			{
				tk2dPreferences.inst.horizontalAnimDisplay = !tk2dPreferences.inst.horizontalAnimDisplay;
				Repaint();
			}
			EditorGUILayout.EndHorizontal();

			// Sanitize frame data
			for (int i = 0; i < clip.frames.Length; ++i)
			{
				if (clip.frames[i].spriteCollection == null || clip.frames[i].spriteCollection.spriteDefinitions.Length == 0)
				{
					EditorUtility.DisplayDialog("Warning", "Invalid sprite collection found.\nThis clip will now be deleted", "Ok");

					clip.name = "";
					clip.frames = new tk2dSpriteAnimationFrame[0];
					Repaint();
					return;
				}
				
				if (clip.frames[i].spriteId < 0 || clip.frames[i].spriteId >= clip.frames[i].spriteCollection.Count)
				{
					EditorUtility.DisplayDialog("Warning", "Invalid frame found, resetting to frame 0", "Ok");
					clip.frames[i].spriteId = 0;
				}
			}
			
			// Warning when one of the frames has different poly count
			if (clipNumFrames > 0)
			{
				bool differentPolyCount = false;
				int polyCount = clip.frames[0].spriteCollection.spriteDefinitions[clip.frames[0].spriteId].positions.Length;
				for (int i = 1; i < clipNumFrames; ++i)
				{
					int thisPolyCount = clip.frames[i].spriteCollection.spriteDefinitions[clip.frames[i].spriteId].positions.Length;
					if (thisPolyCount != polyCount)
					{
						differentPolyCount = true;
						break;
					}
				}
				
				if (differentPolyCount)
				{
					Color bg = GUI.backgroundColor;
					GUI.backgroundColor = Color.red;
					GUILayout.TextArea("Sprites have different poly counts. Performance will be affected");
					GUI.backgroundColor = bg;
				}
			}
			
			// Draw frames
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.Space(); EditorGUILayout.Space(); EditorGUILayout.Space(); EditorGUILayout.Space();
			if (tk2dPreferences.inst.horizontalAnimDisplay)
			{
				scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(144.0f));
				EditorGUILayout.BeginHorizontal();

				for (int i = 0; i < clipNumFrames; ++i)
				{
					int currSpriteCollectionId = GetSpriteCollectionId(clip.frames[i].spriteCollection);
					EditorGUILayout.BeginHorizontal();
						
					GUILayout.Label(i.ToString());
					DrawSpritePreview(currSpriteCollectionId, clip.frames[i].spriteId);
					
					EditorGUILayout.BeginVertical();
					{
						int newSpriteCollectionId = EditorGUILayout.Popup(currSpriteCollectionId, allSpriteCollectionNames);
						if (newSpriteCollectionId != currSpriteCollectionId)
						{
							clip.frames[i].spriteCollection = GetSpriteCollection(newSpriteCollectionId);
							clip.frames[i].spriteId = 0;
						}
						
						clip.frames[i].spriteId = tk2dEditorUtility.SpriteSelectorPopup(null, clip.frames[i].spriteId, clip.frames[i].spriteCollection);
						
						clip.frames[i].triggerEvent = EditorGUILayout.Toggle("Trigger", clip.frames[i].triggerEvent);
						if (clip.frames[i].triggerEvent)
						{
							clip.frames[i].eventInfo = EditorGUILayout.TextField("Trigger info", clip.frames[i].eventInfo);
							clip.frames[i].eventFloat = EditorGUILayout.FloatField("Trigger float", clip.frames[i].eventFloat);
							clip.frames[i].eventInt = EditorGUILayout.IntField("Trigger int", clip.frames[i].eventInt);
						}
					}					
					EditorGUILayout.EndVertical();
					
					EditorGUILayout.EndHorizontal();
					EditorGUILayout.Space();
					EditorGUILayout.Space();
				}
			
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.EndScrollView();
			}
			else
			{
				scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
				EditorGUILayout.BeginVertical();
				
				for (int i = 0; i < clipNumFrames; ++i)
				{
					EditorGUILayout.BeginHorizontal();
					
					GUILayout.Label(i.ToString());
					
					int currSpriteCollectionId = GetSpriteCollectionId(clip.frames[i].spriteCollection);
					
					EditorGUILayout.BeginVertical();
					{
						int newSpriteCollectionId = EditorGUILayout.Popup(currSpriteCollectionId, allSpriteCollectionNames);
						if (newSpriteCollectionId != currSpriteCollectionId)
						{
							clip.frames[i].spriteCollection = GetSpriteCollection(newSpriteCollectionId);
							clip.frames[i].spriteId = 0;
						}
						
						clip.frames[i].spriteId = tk2dEditorUtility.SpriteSelectorPopup(null, clip.frames[i].spriteId, clip.frames[i].spriteCollection);
						
						clip.frames[i].triggerEvent = EditorGUILayout.Toggle("Trigger", clip.frames[i].triggerEvent);
						if (clip.frames[i].triggerEvent)
						{
							clip.frames[i].eventInfo = EditorGUILayout.TextField("Trigger info", clip.frames[i].eventInfo);
							clip.frames[i].eventFloat = EditorGUILayout.FloatField("Trigger float", clip.frames[i].eventFloat);
							clip.frames[i].eventInt = EditorGUILayout.IntField("Trigger int", clip.frames[i].eventInt);
						}
					}
					EditorGUILayout.EndVertical();
					
					DrawSpritePreview(currSpriteCollectionId, clip.frames[i].spriteId);
					
					EditorGUILayout.EndHorizontal();
				}				
				
				EditorGUILayout.EndVertical();
				EditorGUILayout.EndScrollView();
			}
			
			EditorGUILayout.EndHorizontal();
			#endregion
		}
		
		EditorGUILayout.EndVertical();
		
		if (GUI.changed)
			EditorUtility.SetDirty(anim);
	}
	
	
	// Finds a sprite with the name and id
	// matches "baseName" [ 0..9 ]* as id
	int FindFrameIndex(tk2dSpriteDefinition[] spriteDefs, string baseName, int frameId)
	{
		for (int j = 0; j < spriteDefs.Length; ++j)
		{
			if (System.String.Compare(baseName, 0, spriteDefs[j].name, 0, baseName.Length, true) == 0)
			{
				int thisFrameId = 0;
				if (System.Int32.TryParse( spriteDefs[j].name.Substring(baseName.Length), out thisFrameId ) && 
				    thisFrameId == frameId)
				{
					return j;
				}
			}
		}
		return -1;
	}
	
	void AutoFill(tk2dSpriteAnimationClip clip)
	{
		int lastFrameId = clip.frames.Length - 1;
		if (clip.frames[lastFrameId].spriteCollection != null && clip.frames[lastFrameId].spriteId >= 0 && clip.frames[lastFrameId].spriteId < clip.frames[lastFrameId].spriteCollection.Count)
		{
			string na = clip.frames[lastFrameId].spriteCollection.spriteDefinitions[clip.frames[lastFrameId].spriteId].name;
			
			int numStartA = na.Length - 1;
			if (na[numStartA] >= '0' && na[numStartA] <= '9')
			{
				if (GUILayout.Button("AutoFill"))
				{
			        while (numStartA > 0 && na[numStartA - 1] >= '0' && na[numStartA - 1] <= '9')
			            numStartA--;
					
					string baseName = na.Substring(0, numStartA).ToLower();
					int baseNo = System.Convert.ToInt32(na.Substring(numStartA));

					List<int> pendingFrames = new List<int>();
					for (int frameNo = baseNo + 1; ; ++frameNo)
					{
						int frameIdx = FindFrameIndex(clip.frames[lastFrameId].spriteCollection.spriteDefinitions, baseName, frameNo);
						if (frameIdx == -1)
						{
							break;
						}
						else
						{
							pendingFrames.Add(frameIdx);
						}
					}
					
					if (pendingFrames.Count > 0)
					{
						int startFrame = clip.frames.Length;
						var collection = clip.frames[lastFrameId].spriteCollection;
						
						System.Array.Resize<tk2dSpriteAnimationFrame>(ref clip.frames, clip.frames.Length + pendingFrames.Count);
						for (int j = 0; j < pendingFrames.Count; ++j)
						{	
							clip.frames[startFrame + j] = new tk2dSpriteAnimationFrame();
							clip.frames[startFrame + j].spriteCollection = collection;
							clip.frames[startFrame + j].spriteId = pendingFrames[j];
						}
						
						GUI.changed = true;
					}
				}
			}
		}
	}
	
	void DrawSpritePreview(int collectionId, int spriteId)
	{
		if (!tk2dPreferences.inst.displayTextureThumbs)
			return;
		
		if (spriteCollectionIndex[collectionId].version < 1)
		{
			GUILayout.Label("No thumbnail data.\nPlease rebuild Sprite Collection.");
		}
		else		
		{
			Rect r = GUILayoutUtility.GetRect(64, 64, GUILayout.MaxWidth(64), GUILayout.MaxHeight(64));
			Texture2D tex = tk2dSpriteThumbnailCache.GetThumbnailTexture(GetSpriteCollection(collectionId), spriteId);
			if (tex)
			{
				if (tex.width < r.width && tex.height < r.height)
				{
					r.width = tex.width;
					r.height = tex.height;
				}
				else if (tex.width > tex.height)
				{
					r.height = r.width / tex.width * tex.height;
				}
				else
				{
					r.width = r.height / tex.height * tex.width;
				}
				
				GUI.DrawTexture(r, tex);
			}
		}
	}
	
	
	[MenuItem("Assets/Create/tk2d/Sprite Animation", false, 10001)]
    static void DoCollectionCreate()
    {
		string path = tk2dEditorUtility.CreateNewPrefab("SpriteAnimation");
        if (path != null)
        {
            GameObject go = new GameObject();
            go.AddComponent<tk2dSpriteAnimation>();
            go.active = false;

            Object p = EditorUtility.CreateEmptyPrefab(path);
            EditorUtility.ReplacePrefab(go, p, ReplacePrefabOptions.ConnectToPrefab);
            GameObject.DestroyImmediate(go);
			
			tk2dEditorUtility.GetOrCreateIndex().AddSpriteAnimation(AssetDatabase.LoadAssetAtPath(path, typeof(tk2dSpriteAnimation)) as tk2dSpriteAnimation);
			tk2dEditorUtility.CommitIndex();
        }
    }	
}

