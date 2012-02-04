// (c) Copyright HutongGames, LLC 2010-2011. All rights reserved.

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using HutongGames.PlayMakerEditor;

[CustomEditor(typeof(PlayMakerGUI))]
class PlayMakerGUIInspector : Editor
{
	private PlayMakerGUI guiComponent;

	void OnEnable()
	{
		guiComponent = target as PlayMakerGUI;
	}

	public override void OnInspectorGUI()
	{
		EditorGUIUtility.LookLikeInspector();

		GUILayout.Label("NOTES", EditorStyles.boldLabel);
		
		GUILayout.Label("- A scene should only have one PlayMakerGUI component.\n- PlayMaker will auto-add this component.\n- Disable auto-add in Preferences.");
		
		GUILayout.Label("General", EditorStyles.boldLabel);

		EditorGUI.indentLevel = 1;

		guiComponent.enableGUILayout = EditorGUILayout.Toggle(new GUIContent("Enable GUILayout",
		                                                               "Disabling GUILayout can improve the performance of GUI actions, especially on mobile devices. NOTE: You cannot use GUILayout actions with GUILayout disabled."),
																	   guiComponent.enableGUILayout);
		guiComponent.controlMouseCursor = EditorGUILayout.Toggle(new GUIContent("Control Mouse Cursor",
		                                                                  "Disable this if you have scripts that need to control the mouse cursor."),
																		  guiComponent.controlMouseCursor);

		guiComponent.previewOnGUI = EditorGUILayout.Toggle(new GUIContent("Preview GUI Actions While Editing", "This lets you preview GUI actions as you edit them. NOTE: This is an experimental feature, so you might run into some bugs!"), guiComponent.previewOnGUI);

		EditorGUI.indentLevel = 0;
		GUILayout.Label("Debugging", EditorStyles.boldLabel);
		EditorGUI.indentLevel = 1;

		guiComponent.drawStateLabels = EditorGUILayout.Toggle(new GUIContent("Draw Active State Labels", "Draw the currently active state over GameObjects in the Game View. You can enable/disable for each FSM in the PlayMakerFSM Inspector."), guiComponent.drawStateLabels);

		GUI.enabled = guiComponent.drawStateLabels;
		//EditorGUI.indentLevel = 2;

		guiComponent.GUITextureStateLabels = EditorGUILayout.Toggle(new GUIContent("GUITexture State Labels", "Draw active state labels on GUITextures."), guiComponent.GUITextureStateLabels);
		guiComponent.GUITextStateLabels = EditorGUILayout.Toggle(new GUIContent("GUIText State Labels", "Draw active state labels on GUITexts."), guiComponent.GUITextStateLabels);

		GUI.enabled = true;
		//EditorGUI.indentLevel = 1;

		guiComponent.filterLabelsWithDistance = EditorGUILayout.Toggle(new GUIContent("Filter State Labels With Distance", "This is useful if you only want to see nearby state labels as you move in the Game View."), guiComponent.filterLabelsWithDistance);

		GUI.enabled = guiComponent.filterLabelsWithDistance;

		guiComponent.maxLabelDistance = EditorGUILayout.FloatField(new GUIContent("Distance", "Distance is measured from the main camera"), guiComponent.maxLabelDistance);
	}
}
