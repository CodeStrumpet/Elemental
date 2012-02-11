using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class tk2dPreferences
{
	static tk2dPreferences _inst = null;	
	public static tk2dPreferences inst
	{
		get 
		{
			if (_inst == null)
			{
				_inst = new tk2dPreferences();
				_inst.Read();
			}
			return _inst;
		}
	}
	
	bool _displayTextureThumbs;
	bool _horizontalAnimDisplay;

	public bool displayTextureThumbs { get { return _displayTextureThumbs; } set { if (_displayTextureThumbs != value) { _displayTextureThumbs = value; Write(); } } }
	public bool horizontalAnimDisplay { get { return _horizontalAnimDisplay; } set { if (_horizontalAnimDisplay != value) { _horizontalAnimDisplay = value; Write(); } } }
	
	void Read()
	{
		_displayTextureThumbs = EditorPrefs.GetBool("tk2d_displayTextureThumbs", true);
		_horizontalAnimDisplay = EditorPrefs.GetBool("tk2d_horizontalAnimDisplay", false);
	}
	
	public void Write()
	{
		EditorPrefs.SetBool("tk2d_displayTextureThumbs", _displayTextureThumbs);
		EditorPrefs.SetBool("tk2d_horizontalAnimDisplay", _horizontalAnimDisplay);
	}
}

public class tk2dPreferencesEditor : EditorWindow
{
	GUIContent label_spriteThumbnails = new GUIContent("Sprite Thumbnails", "Turn off sprite thumbnails to save memory.");
	
	GUIContent label_animationFrames = new GUIContent("Animation Frame Display", "Select the direction of frames in the SpriteAnimation inspector.");
	GUIContent label_animFrames_Horizontal = new GUIContent("Horizontal");
	GUIContent label_animFrames_Vertical = new GUIContent("Vertical");
	
	void OnGUI()
	{
		tk2dPreferences prefs = tk2dPreferences.inst;
		
		EditorGUIUtility.LookLikeControls(150.0f);
		
		prefs.displayTextureThumbs = EditorGUILayout.Toggle(label_spriteThumbnails, prefs.displayTextureThumbs);
		
		int had = EditorGUILayout.Popup(label_animationFrames, prefs.horizontalAnimDisplay?0:1, new GUIContent[] { label_animFrames_Horizontal, label_animFrames_Vertical } );
		prefs.horizontalAnimDisplay = (had == 0)?true:false;
	}
}