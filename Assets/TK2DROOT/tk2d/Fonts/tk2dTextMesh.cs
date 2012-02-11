using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[AddComponentMenu("2D Toolkit/tk2dTextMesh")]
public class tk2dTextMesh : MonoBehaviour 
{
	[SerializeField] tk2dFontData _font;
    [SerializeField] string _text = ""; 
    [SerializeField] Color _color = Color.white; 
    [SerializeField] Color _color2 = Color.white; 
    [SerializeField] bool _useGradient = false; 
	[SerializeField] int _textureGradient = 0;
    [SerializeField] TextAnchor _anchor = TextAnchor.LowerLeft; 
    [SerializeField] Vector3 _scale = new Vector3(1.0f, 1.0f, 1.0f); 
	[SerializeField] bool _kerning = false; 
    [SerializeField] int _maxChars = 16; 
	[SerializeField] bool _inlineStyling = false;
	public bool pixelPerfect = false;
	
    Vector3[] vertices;
    Vector2[] uvs;
	Vector2[] uv2;
    Color[] colors;
	
	[System.FlagsAttribute]
	enum UpdateFlags
	{
		UpdateNone		= 0,
		UpdateText		= 1,	// update text vertices & uvs
		UpdateColors	= 2,	// only colors have changed
		UpdateBuffers	= 4,	// update buffers (maxchars has changed)
	};
	UpdateFlags updateFlags = UpdateFlags.UpdateBuffers;

    Mesh mesh;

	// accessors
	public tk2dFontData font { get { return _font ; } set { _font = value; updateFlags |= UpdateFlags.UpdateText; } }
	public string text { get { return _text; } set { _text = value;  updateFlags |= UpdateFlags.UpdateText; } }
	public Color color { get { return _color; } set { _color = value; updateFlags |= UpdateFlags.UpdateColors; } }
	public Color color2 { get { return _color2; } set { _color2 = value; updateFlags |= UpdateFlags.UpdateColors; } }
	public bool useGradient { get { return _useGradient; } set { _useGradient = value; updateFlags |= UpdateFlags.UpdateColors; } }
	public TextAnchor anchor { get { return _anchor; } set { _anchor = value; updateFlags |= UpdateFlags.UpdateText; } }
	public Vector3 scale { get { return _scale; } set { _scale = value; updateFlags |= UpdateFlags.UpdateText; } }
	public bool kerning { get { return _kerning; } set { _kerning = value; updateFlags |= UpdateFlags.UpdateText; } }
	public int maxChars { get { return _maxChars; } set { _maxChars = value; updateFlags |= UpdateFlags.UpdateBuffers; } }
	public int textureGradient { get { return _textureGradient; } set { _textureGradient = value % font.gradientCount; updateFlags |= UpdateFlags.UpdateText; } }
	public bool inlineStyling { get { return _inlineStyling; } set { _inlineStyling = value; updateFlags |= tk2dTextMesh.UpdateFlags.UpdateText; } }
	
	// Use this for initialization
	void Awake() 
	{
		if (pixelPerfect)
			MakePixelPerfect();
        Init();
	}
	
	public void Init(bool force)
	{
		if (force)
		{
			updateFlags |= UpdateFlags.UpdateBuffers;
		}
		Init();
	}
	
	public int NumDrawnCharacters()
	{
		bool useInlineStyling = inlineStyling && _font.textureGradients;
		float unused0 = 0.0f, unused1 = 0.0f;
		int tmpCount = _maxChars;
		_maxChars = int.MaxValue;
		int count = CalcAnchor(useInlineStyling, out unused0, out unused1);
		_maxChars = tmpCount;
		return count;
	}
	
	// returns number of characters written
	int FillTextData()
	{
		Vector2 gradientOffset = new Vector2((float)_textureGradient / font.gradientCount, 0);

		bool useInlineStyling = inlineStyling && _font.textureGradients;
        float offsetX, offsetY;
        CalcAnchor(useInlineStyling, out offsetX, out offsetY);
		
        float cursorX = 0.0f;
		float cursorY = 0.0f;
		
		int target = 0;
		for (int i = 0; i < _text.Length && target < _maxChars; ++i)
		{
            int idx = _text[i];
            if (idx >= _font.chars.Length) idx = 0; // should be space
            tk2dFontChar chr = _font.chars[idx];

			if (idx == '\n')
			{
				cursorX = 0.0f;
				cursorY -= _font.lineHeight * _scale.y;
				continue;
			}
			else if (useInlineStyling)
			{
				if (idx == '^')
				{
					if (i+1 < _text.Length)
					{
						i++;
						if (_text[i] != '^')
						{
							int data = _text[i] - '0';
							gradientOffset = new Vector2((float)data / font.gradientCount, 0);
							continue;
						}
					}
				}
			}
			
            vertices[target * 4 + 0] = new Vector3(offsetX + cursorX + chr.p0.x * _scale.x, offsetY + cursorY + chr.p0.y * _scale.y, 0);
            vertices[target * 4 + 1] = new Vector3(offsetX + cursorX + chr.p1.x * _scale.x, offsetY + cursorY + chr.p0.y * _scale.y, 0);
            vertices[target * 4 + 2] = new Vector3(offsetX + cursorX + chr.p0.x * _scale.x, offsetY + cursorY + chr.p1.y * _scale.y, 0);
            vertices[target * 4 + 3] = new Vector3(offsetX + cursorX + chr.p1.x * _scale.x, offsetY + cursorY + chr.p1.y * _scale.y, 0);

            uvs[target * 4 + 0] = new Vector2(chr.uv0.x, chr.uv0.y);
            uvs[target * 4 + 1] = new Vector2(chr.uv1.x, chr.uv0.y);
            uvs[target * 4 + 2] = new Vector2(chr.uv0.x, chr.uv1.y);
            uvs[target * 4 + 3] = new Vector2(chr.uv1.x, chr.uv1.y);
			
			if (_font.textureGradients)
			{
				uv2[target * 4 + 0] = gradientOffset + chr.gradientUv[0];
				uv2[target * 4 + 1] = gradientOffset + chr.gradientUv[1];
				uv2[target * 4 + 2] = gradientOffset + chr.gradientUv[2];
				uv2[target * 4 + 3] = gradientOffset + chr.gradientUv[3];
			}

            cursorX += chr.advance * _scale.x;
			
			if (_kerning && i < _text.Length - 1)
			{
				foreach (var k in _font.kerning)
				{
					if (k.c0 == _text[i] && k.c1 == _text[i+1])
					{
						cursorX += k.amount * _scale.x;
						break;
					}
				}
			}				
			
			++target;
		}
		
		return target;
	}
	
    public void Init()
    {
        if (_font && (updateFlags & UpdateFlags.UpdateBuffers) != 0)
        {
            MeshFilter meshFilter = GetComponent<MeshFilter>();
            Mesh newMesh = new Mesh();

            Color topColor = _color;
            Color bottomColor = _useGradient?_color2:_color;

            // volatile data
            vertices = new Vector3[_maxChars * 4];
            uvs = new Vector2[_maxChars * 4];
            colors = new Color[_maxChars * 4];
			if (_font.textureGradients)
			{
				uv2 = new Vector2[_maxChars * 4];
			}
            int[] triangles = new int[_maxChars * 6];
			int target = FillTextData();
			
			for (int i = 0; i < target; ++i)
			{
                colors[i * 4 + 0] = colors[i * 4 + 1] = topColor;
                colors[i * 4 + 2] = colors[i * 4 + 3] = bottomColor;

                triangles[i * 6 + 0] = i * 4 + 0;
                triangles[i * 6 + 1] = i * 4 + 1;
                triangles[i * 6 + 2] = i * 4 + 3;
                triangles[i * 6 + 3] = i * 4 + 2;
                triangles[i * 6 + 4] = i * 4 + 0;
                triangles[i * 6 + 5] = i * 4 + 3;
			}
			
			for (int i = target; i < _maxChars; ++i)
			{
                vertices[i * 4 + 0] = vertices[i * 4 + 1] = vertices[i * 4 + 2] = vertices[i * 4 + 3] = Vector3.zero;
                uvs[i * 4 + 0] = uvs[i * 4 + 1] = uvs[i * 4 + 2] = uvs[i * 4 + 3] = Vector2.zero;
				if (_font.textureGradients) 
				{
                    uv2[i * 4 + 0] = uv2[i * 4 + 1] = uv2[i * 4 + 2] = uv2[i * 4 + 3] = Vector2.zero;
				}				

				colors[i * 4 + 0] = colors[i * 4 + 1] = topColor;
                colors[i * 4 + 2] = colors[i * 4 + 3] = bottomColor;

                triangles[i * 6 + 0] = i * 4 + 0;
                triangles[i * 6 + 1] = i * 4 + 1;
                triangles[i * 6 + 2] = i * 4 + 3;
                triangles[i * 6 + 3] = i * 4 + 2;
                triangles[i * 6 + 4] = i * 4 + 0;
                triangles[i * 6 + 5] = i * 4 + 3;
			}

            newMesh.vertices = vertices;
            newMesh.uv = uvs;
			if (font.textureGradients)
			{
				newMesh.uv1 = uv2;
			}
            newMesh.triangles = triangles;
            newMesh.colors = colors;
			newMesh.RecalculateBounds();
   
			meshFilter.mesh = newMesh;
            mesh = meshFilter.sharedMesh;
			
			updateFlags = UpdateFlags.UpdateNone;
    	}
    }
	
    public void Commit()
    {
		// Can come in here without anything initalized when
		// instantiated in code
		if ((updateFlags & UpdateFlags.UpdateBuffers) != 0)
		{
			Init();
		}
        else 
		{
			if ((updateFlags & UpdateFlags.UpdateText) != 0)
	        {
				int target = FillTextData();
				for (int i = target; i < _maxChars; ++i)
				{
					// was/is unnecessary to fill anything else
                    vertices[i * 4 + 0] = vertices[i * 4 + 1] = vertices[i * 4 + 2] = vertices[i * 4 + 3] = Vector3.zero;
	            }
	
	            mesh.vertices = vertices;
	            mesh.uv = uvs;
				if (font.textureGradients)
				{
					mesh.uv1 = uv2;
				}
				
				// comment this in for game if it becomes a problem
	#if UNITY_EDITOR
				mesh.RecalculateBounds();
	#endif
	        }
	
	        if ((updateFlags & UpdateFlags.UpdateColors) != 0)
	        {
	            Color topColor = _color;
	            Color bottomColor = _useGradient ? _color2 : _color;
	
	            for (int i = 0; i < colors.Length; i += 4)
	            {
	                colors[i + 0] = colors[i + 1] = topColor;
	                colors[i + 2] = colors[i + 3] = bottomColor;
	            }
	            mesh.colors = colors;
	        }
		}
		
		updateFlags = UpdateFlags.UpdateNone;
    }
	
    int CalcAnchor(bool useInlineStyling, out float offsetX, out float offsetY)
    {
		int numChars = 0;
        if (_font != null)
        {
            // calc string width
			float maxWidth = 0.0f;
            float width = 0.0f;
			int numLines = 1;
			int count = 0;
			
            for (int i = 0; count < _maxChars && i < _text.Length; ++i)
            {
	            int idx = _text[i];
	            if (idx >= _font.chars.Length) idx = 0; // should be space
	            tk2dFontChar chr = _font.chars[idx];
	
				if (idx == '\n')
				{
					numLines++;
					maxWidth = Mathf.Max(maxWidth, width);
					width = 0.0f;
					continue;
				}
				else if (useInlineStyling)
				{
					if (idx == '^')
					{
						if (i+1 < _text.Length)
						{
							i++;
							if (_text[i] != '^')
							{
								continue;
							}
						}
					}
				}				
				
                width += chr.advance * _scale.x;
				numChars++;
				count++;
            }

			maxWidth = Mathf.Max(maxWidth, width);
			float lineHeight = _font.lineHeight * _scale.y;
            float height = lineHeight * numLines;
			
	        switch (_anchor)
	        {
	            case TextAnchor.LowerLeft: offsetX = 0.0f; 	offsetY = height - lineHeight; break;
	            case TextAnchor.MiddleLeft: offsetX = 0.0f; offsetY = height / 2.0f - lineHeight; break;
	            case TextAnchor.UpperLeft: offsetX = 0.0f; 	offsetY = -lineHeight; break;
	
	            case TextAnchor.LowerCenter: offsetX = -maxWidth / 2.0f; 	offsetY = height - lineHeight; break;
	            case TextAnchor.MiddleCenter: offsetX = -maxWidth / 2.0f; 	offsetY = height / 2.0f - lineHeight; break;
	            case TextAnchor.UpperCenter: offsetX = -maxWidth / 2.0f; 	offsetY = -lineHeight; break;
	
	            case TextAnchor.LowerRight: offsetX = -maxWidth; 	offsetY = height - lineHeight; break;
	            case TextAnchor.MiddleRight: offsetX = -maxWidth; 	offsetY = height / 2.0f - lineHeight; break;
	            case TextAnchor.UpperRight: offsetX = -maxWidth; 	offsetY = -lineHeight; break;
	
	            default:
	                offsetX = 0.0f;
	                offsetY = 0.0f;
	                break;
	        }			
        }
        else
        {
            offsetX = 0.0f;
            offsetY = 0.0f;
        }
		
		return numChars;
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
		scale = new Vector3(Mathf.Sign(scale.x) * s, Mathf.Sign(scale.y) * s, Mathf.Sign(scale.z) * s);
	}	
}
