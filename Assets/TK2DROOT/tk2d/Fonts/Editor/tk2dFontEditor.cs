using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

[CustomEditor(typeof(tk2dFont))]
public class tk2dFontEditor : Editor 
{
	public Shader GetShader(bool gradient)
	{
		if (gradient) return Shader.Find("tk2d/Blend2TexVertexColor");
		else return Shader.Find("tk2d/BlendVertexColor");
	}
	
	public override void OnInspectorGUI()
	{
		tk2dFont gen = (tk2dFont)target;
		EditorGUILayout.BeginVertical();

		DrawDefaultInspector();

		if (GUILayout.Button("Commit..."))
		{
			if (gen.bmFont == null || gen.texture == null)
			{
				EditorUtility.DisplayDialog("BMFont", "Need an bmFont and texture bound to work", "Ok");
				return;
			}
			
			if (gen.material == null)
			{
				gen.material = new Material(GetShader(gen.gradientTexture != null));
				string materialPath = AssetDatabase.GetAssetPath(gen).Replace(".prefab", "material.mat");
				AssetDatabase.CreateAsset(gen.material, materialPath);
			}
			
			if (gen.data == null)
			{
				string bmFontPath = AssetDatabase.GetAssetPath(gen).Replace(".prefab", "data.prefab");
				
				GameObject go = new GameObject();
				go.AddComponent<tk2dFontData>();
				go.active = false;
				
				Object p = EditorUtility.CreateEmptyPrefab(bmFontPath);
				EditorUtility.ReplacePrefab(go, p);
				GameObject.DestroyImmediate(go);
				AssetDatabase.SaveAssets();
				
				gen.data = AssetDatabase.LoadAssetAtPath(bmFontPath, typeof(tk2dFontData)) as tk2dFontData;
			}
			
			ParseBMFont(AssetDatabase.GetAssetPath(gen.bmFont), gen.data, gen);

			if (gen.manageMaterial)
			{
				Shader s = GetShader(gen.gradientTexture != null);
				if (gen.material.shader != s)
				{
					gen.material.shader = s;
					EditorUtility.SetDirty(gen.material);
				}
				if (gen.material.mainTexture != gen.texture)
				{
					gen.material.mainTexture = gen.texture;
					EditorUtility.SetDirty(gen.material);
				}
				if (gen.gradientTexture != null && gen.gradientTexture != gen.material.GetTexture("_GradientTex"))
				{
					gen.material.SetTexture("_GradientTex", gen.gradientTexture);
					EditorUtility.SetDirty(gen.material);
				}
			}
			
			gen.data.material = gen.material;
			gen.data.textureGradients = gen.gradientTexture != null;
			gen.data.gradientCount = gen.gradientCount;
			gen.data.gradientTexture = gen.gradientTexture;
		
            // Rebuild assets already present in the scene
            tk2dTextMesh[] sprs = Resources.FindObjectsOfTypeAll(typeof(tk2dTextMesh)) as tk2dTextMesh[];
            foreach (tk2dTextMesh spr in sprs)
            {
                spr.Init(true);
            }
			
			EditorUtility.SetDirty(gen);
			EditorUtility.SetDirty(gen.data);
        }

		EditorGUILayout.EndVertical();
	}
	
	// Internal structures to fill and process
	class IntChar
	{
		public int id = 0, x = 0, y = 0, width = 0, height = 0, xoffset = 0, yoffset = 0, xadvance = 0;
	};
	
	class IntKerning
	{
		public int first = 0, second = 0, amount = 0;
	};
	
	class IntFontInfo
	{
		public int scaleW = 0, scaleH = 0;
		public int lineHeight = 0;
		
		public List<IntChar> chars = new List<IntChar>();
		public List<IntKerning> kernings = new List<IntKerning>();
	};
	
	
	IntFontInfo ParseBMFontXml(XmlDocument doc)
	{
		IntFontInfo fontInfo = new IntFontInfo();
		
        XmlNode nodeCommon = doc.SelectSingleNode("/font/common");
		fontInfo.scaleW = ReadIntAttribute(nodeCommon, "scaleW");
		fontInfo.scaleH = ReadIntAttribute(nodeCommon, "scaleH");
		fontInfo.lineHeight = ReadIntAttribute(nodeCommon, "lineHeight");
		int pages = ReadIntAttribute(nodeCommon, "pages");
		if (pages != 1)
		{
			EditorUtility.DisplayDialog("Fatal error", "Only one page supported in font. Please change the setting and re-export.", "Ok");
			return null;
		}

		foreach (XmlNode node in doc.SelectNodes(("/font/chars/char")))
		{
			IntChar thisChar = new IntChar();
			thisChar.id = ReadIntAttribute(node, "id");
            thisChar.x = ReadIntAttribute(node, "x");
            thisChar.y = ReadIntAttribute(node, "y");
            thisChar.width = ReadIntAttribute(node, "width");
            thisChar.height = ReadIntAttribute(node, "height");
            thisChar.xoffset = ReadIntAttribute(node, "xoffset");
            thisChar.yoffset = ReadIntAttribute(node, "yoffset");
            thisChar.xadvance = ReadIntAttribute(node, "xadvance");
			
			fontInfo.chars.Add(thisChar);
		}
		
		foreach (XmlNode node in doc.SelectNodes("/font/kernings/kerning"))
		{
			IntKerning thisKerning = new IntKerning();
			thisKerning.first = ReadIntAttribute(node, "first");
			thisKerning.second = ReadIntAttribute(node, "second");
			thisKerning.amount = ReadIntAttribute(node, "amount");
			
			fontInfo.kernings.Add(thisKerning);
		}

		return fontInfo;
	}
	
	string FindKeyValue(string[] tokens, string key)
	{
		string keyMatch = key + "=";
		for (int i = 0; i < tokens.Length; ++i)
		{
			if (tokens[i].Length > keyMatch.Length && tokens[i].Substring(0, keyMatch.Length) == keyMatch)
				return tokens[i].Substring(keyMatch.Length);
		}
		
		return "";
	}
	
	IntFontInfo ParseBMFontText(string path)
	{
		IntFontInfo fontInfo = new IntFontInfo();
		
		FileInfo finfo = new FileInfo(path);
		StreamReader reader = finfo.OpenText();
		string line;
		while ((line = reader.ReadLine()) != null) 
		{
			string[] tokens = line.Split( ' ' );
			
			if (tokens[0] == "common")
			{
				fontInfo.lineHeight = int.Parse( FindKeyValue(tokens, "lineHeight") );
				fontInfo.scaleW = int.Parse( FindKeyValue(tokens, "scaleW") );
				fontInfo.scaleH = int.Parse( FindKeyValue(tokens, "scaleH") );
				int pages = int.Parse( FindKeyValue(tokens, "pages") );
				if (pages != 1)
				{
					EditorUtility.DisplayDialog("Fatal error", "Only one page supported in font. Please change the setting and re-export.", "Ok");
					return null;
				}
			}
			else if (tokens[0] == "char")
			{
				IntChar thisChar = new IntChar();
				thisChar.id = int.Parse(FindKeyValue(tokens, "id"));
				thisChar.x = int.Parse(FindKeyValue(tokens, "x"));
				thisChar.y = int.Parse(FindKeyValue(tokens, "y"));
				thisChar.width = int.Parse(FindKeyValue(tokens, "width"));
				thisChar.height = int.Parse(FindKeyValue(tokens, "height"));
				thisChar.xoffset = int.Parse(FindKeyValue(tokens, "xoffset"));
				thisChar.yoffset = int.Parse(FindKeyValue(tokens, "yoffset"));
				thisChar.xadvance = int.Parse(FindKeyValue(tokens, "xadvance"));
				fontInfo.chars.Add(thisChar);
			}
			else if (tokens[0] == "kerning")
			{
				IntKerning thisKerning = new IntKerning();
				thisKerning.first = int.Parse(FindKeyValue(tokens, "first"));
				thisKerning.second = int.Parse(FindKeyValue(tokens, "second"));
				thisKerning.amount = int.Parse(FindKeyValue(tokens, "amount"));
				fontInfo.kernings.Add(thisKerning);
			}
		}
		reader.Close();
		
		return fontInfo;
	}
	
	bool ParseBMFont(string path, tk2dFontData bmFont, tk2dFont source)
	{
		IntFontInfo fontInfo = null;
		
		try
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(path);
			fontInfo = ParseBMFontXml(doc);
		}
		catch
		{
			fontInfo = ParseBMFontText(path);
		}
		
		if (fontInfo == null || fontInfo.chars.Count == 0)
			return false;
	
		float texWidth = fontInfo.scaleW;
        float texHeight = fontInfo.scaleH;
        float lineHeight = fontInfo.lineHeight;

		float scale = 2.0f * source.targetOrthoSize / source.targetHeight;

        bmFont.lineHeight = lineHeight * scale;
		
		tk2dFontChar[] chars = new tk2dFontChar[source.numCharacters];
		int minChar = 65536;
		int maxCharWithinBounds = 0;
		int numLocalChars = 0;
		float largestWidth = 0.0f;
		foreach (var theChar in fontInfo.chars)
		{
			tk2dFontChar thisChar = new tk2dFontChar();
			int id = theChar.id;
            int x = theChar.x;
            int y = theChar.y;
            int width = theChar.width;
            int height = theChar.height;
            int xoffset = theChar.xoffset;
            int yoffset = theChar.yoffset;
            int xadvance = theChar.xadvance;

            // precompute required data
            float px = xoffset * scale;
            float py = (lineHeight - yoffset) * scale;

            thisChar.p0 = new Vector3(px, py, 0);
            thisChar.p1 = new Vector3(px + width * scale, py - height * scale, 0);
			
			if (source.flipTextureY)
			{
	            thisChar.uv0 = new Vector2(x / texWidth, y / texHeight);
	            thisChar.uv1 = new Vector2(thisChar.uv0.x + width / texWidth, thisChar.uv0.y + height / texHeight);
			}
			else
			{
	            thisChar.uv0 = new Vector2(x / texWidth, 1.0f - y / texHeight);
	            thisChar.uv1 = new Vector2(thisChar.uv0.x + width / texWidth, thisChar.uv0.y - height / texHeight);
			}
            thisChar.advance = xadvance * scale;
			largestWidth = Mathf.Max(thisChar.advance, largestWidth);
			
			// Needs gradient data
			if (source.gradientTexture != null)
			{
				// build it up assuming the first gradient
				float x0 = (float)(0.0f / source.gradientCount);
				float x1 = (float)(1.0f / source.gradientCount);
				float y0 = 1.0f;
				float y1 = 0.0f;

				// align to glyph if necessary
				
				thisChar.gradientUv = new Vector2[4];
				thisChar.gradientUv[0] = new Vector2(x0, y0);
				thisChar.gradientUv[1] = new Vector2(x1, y0);
				thisChar.gradientUv[2] = new Vector2(x0, y1);
				thisChar.gradientUv[3] = new Vector2(x1, y1);
			}

			if (id < source.numCharacters)
			{
				maxCharWithinBounds = (id > maxCharWithinBounds) ? id : maxCharWithinBounds;
				minChar = (id < minChar) ? id : minChar;
				chars[id] = thisChar;
				++numLocalChars;
			}
		}
		
        if (source.dupeCaps)
        {
            for (int uc = 'A'; uc <= 'Z'; ++uc)
            {
                int lc = uc + ('a' - 'A');
                if (chars[lc] == null) chars[lc] = chars[uc];
                else if (chars[uc] == null) chars[uc] = chars[lc];
            }
        }
		
		bmFont.largestWidth = largestWidth;
		bmFont.chars = new tk2dFontChar[source.numCharacters];
		for (int i = 0; i < source.numCharacters; ++i)
		{
			bmFont.chars[i] = chars[i];
			if (bmFont.chars[i] == null)
			{
				bmFont.chars[i] = new tk2dFontChar(); // zero everything, null char
			}
		}
		
		// kerning
		bmFont.kerning = new tk2dFontKerning[fontInfo.kernings.Count];
		for (int i = 0; i < bmFont.kerning.Length; ++i)
		{
			tk2dFontKerning kerning = new tk2dFontKerning();
			kerning.c0 = fontInfo.kernings[i].first;
			kerning.c1 = fontInfo.kernings[i].second;
			kerning.amount = fontInfo.kernings[i].amount * scale;
			bmFont.kerning[i] = kerning;
		}
		
		return true;
	}

	int ReadIntAttribute(XmlNode node, string attribute)
	{
		return int.Parse(node.Attributes[attribute].Value, System.Globalization.NumberFormatInfo.InvariantInfo);
	}
	float ReadFloatAttribute(XmlNode node, string attribute)
	{
		return float.Parse(node.Attributes[attribute].Value, System.Globalization.NumberFormatInfo.InvariantInfo);
	}
	Vector2 ReadVector2Attributes(XmlNode node, string attributeX, string attributeY)
	{
		return new Vector2(ReadFloatAttribute(node, attributeX), ReadFloatAttribute(node, attributeY));
	}
	
	[MenuItem("Assets/Create/tk2d/Font", false, 11000)]
	static void DoBMFontCreate()
	{
		string path = tk2dEditorUtility.CreateNewPrefab("Font");
		if (path != null)
		{
			GameObject go = new GameObject();
			tk2dFont font = go.AddComponent<tk2dFont>();
			font.manageMaterial = true;
			go.active = false;

			Object p = EditorUtility.CreateEmptyPrefab(path);
			EditorUtility.ReplacePrefab(go, p, ReplacePrefabOptions.ConnectToPrefab);

			GameObject.DestroyImmediate(go);
			
			tk2dEditorUtility.GetOrCreateIndex().AddFont(AssetDatabase.LoadAssetAtPath(path, typeof(tk2dFont)) as tk2dFont);
			tk2dEditorUtility.CommitIndex();
		}
	}

    [MenuItem("GameObject/Create Other/tk2d/TextMesh", false, 13905)]
    static void DoCreateBMTextMesh()
    {
		tk2dFontData fontData = null;
		Material material = null;
		
		// Find reference in scene
        tk2dTextMesh dupeMesh = GameObject.FindObjectOfType(typeof(tk2dTextMesh)) as tk2dTextMesh;
		if (dupeMesh) 
		{
			fontData = dupeMesh.font;
			material = dupeMesh.GetComponent<MeshRenderer>().sharedMaterial;
		}
		
		// Find in library
		if (fontData == null)
		{
			tk2dFont[] allFontData = tk2dEditorUtility.GetOrCreateIndex().GetFonts();
			foreach (var v in allFontData)
			{
				if (v.data != null)
				{
					fontData = v.data;
					material = fontData.material;
				}
			}
		}
		
		if (fontData == null)
		{
			EditorUtility.DisplayDialog("Create TextMesh", "Unable to create text mesh as no Fonts have been found.", "Ok");
			return;
		}

		GameObject go = tk2dEditorUtility.CreateGameObjectInScene("TextMesh");
        tk2dTextMesh textMesh = go.AddComponent<tk2dTextMesh>();
		textMesh.font = fontData;
		textMesh.text = "New TextMesh";
		textMesh.Commit();
		textMesh.GetComponent<MeshRenderer>().material = material;
    }
}
