// (c) Copyright HutongGames, LLC 2010-2011. All rights reserved.

// comment out to compile out iTween editing functions
// E.g., if you've removed iTween actions.
// Can't put this in iTween actions since this is compiled first...
#define iTweenPathEditing

using UnityEditor;
using UnityEngine;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using HutongGames.PlayMakerEditor;
using System.Collections.Generic;

// Basic inspector for FsmComponent.

[CustomEditor(typeof(PlayMakerFSM))]
public class FsmComponentInspector : Editor
{
	public static FsmComponentInspector fsmComponentInspector;
	
	const float indent = 20;
	
	PlayMakerFSM fsmComponent;

	public bool showControls = true;
	//public bool showExposedEvents = true;
	public bool showInfo;
	public bool showStates;
	public bool showEvents;
	public bool showVariables;
	
	
	List<FsmVariable> fsmVariables = new List<FsmVariable>();
	
	GUIStyle textAreaStyle;
	
	void OnEnable()
	{
		fsmComponent = target as PlayMakerFSM;
		
		// can happen when playmaker is updated
		if (fsmComponent != null)
		{
			BuildFsmVariableList();
			fsmComponentInspector = this;
		}
	}

	public override void OnInspectorGUI()
	{
		//EditorGUIUtility.LookLikeControls();

		// can happen when playmaker is updated...?

		if (fsmComponent == null)
		{
			return;
		}

		if (!FsmEditorStyles.IsInitialized())
		{
			FsmEditorStyles.Init();
		}

		if (textAreaStyle == null)
		{
			textAreaStyle = new GUIStyle(EditorStyles.textField) {wordWrap = true};
		}
		
		EditorGUILayout.BeginHorizontal();

		fsmComponent.FsmName = EditorGUILayout.TextField(fsmComponent.FsmName);

		//fsmComponent.Fsm.ShowStateLabel = GUILayout.Toggle(fsmComponent.Fsm.ShowStateLabel, new GUIContent("i", "Show active state label in game view. NOTE: Requires PlayMakerGUI in scene"), "Button", GUILayout.MaxWidth(40));

		if (GUILayout.Button(new GUIContent("Edit","Edit in the PlayMaker FSM Editor"), GUILayout.MaxWidth(45)))
		{
			FsmEditorWindow.OpenWindow(fsmComponent);
			GUIUtility.ExitGUI();
		}

		//showInfo = GUILayout.Toggle(showInfo, new GUIContent("Info","Show overview of States, Events and Variables"), "Button", GUILayout.MaxWidth(50));
		
		EditorGUILayout.EndHorizontal();
		
		fsmComponent.FsmDescription = EditorGUILayout.TextArea(fsmComponent.FsmDescription, textAreaStyle);

		fsmComponent.Fsm.ShowStateLabel = GUILayout.Toggle(fsmComponent.Fsm.ShowStateLabel, new GUIContent("Show State Label","Show active state label in game view.\nNOTE: Requires PlayMakerGUI in scene"));

		// VARIABLES

		FsmEditorGUILayout.LightDivider();
		showControls = EditorGUILayout.Foldout(showControls, new GUIContent("Controls", "FSM Variables and Events exposed in the Inspector."), FsmEditorStyles.CategoryFoldout);

		if (showControls)
		{
			//EditorGUIUtility.LookLikeInspector();

			BuildFsmVariableList();

			foreach (var fsmVar in fsmVariables)
			{
				if (fsmVar.ShowInInspector)
				{
					fsmVar.DoValueGUI(new GUIContent(fsmVar.Name, fsmVar.Name + (!string.IsNullOrEmpty(fsmVar.Tooltip) ? ":\n" + fsmVar.Tooltip : "")));
				}
			}

			if (GUI.changed)
			{
				FsmEditor.RepaintAll();
			}
		}

		// EVENTS

		//FsmEditorGUILayout.LightDivider();
		//showExposedEvents = EditorGUILayout.Foldout(showExposedEvents, new GUIContent("Events", "To expose events here:\nIn PlayMaker Editor, Events tab, select an event and check Inspector."), FsmEditorStyles.CategoryFoldout);

		if (showControls)
		{
			EditorGUI.indentLevel = 1;

			//GUI.enabled = Application.isPlaying;

			foreach (var fsmEvent in fsmComponent.Fsm.ExposedEvents)
			{
				if (GUILayout.Button(fsmEvent.Name))
				{
					fsmComponent.Fsm.Event(fsmEvent);
				}
			}

			if (GUI.changed)
			{
				FsmEditor.RepaintAll();
			}
		}

		//GUI.enabled = true;

		//INFO

		EditorGUI.indentLevel = 0;

		FsmEditorGUILayout.LightDivider();
		showInfo = EditorGUILayout.Foldout(showInfo, "Info", FsmEditorStyles.CategoryFoldout);

		if (showInfo)
		{
			EditorGUI.indentLevel = 1;

			//FsmEditorGUILayout.LightDivider();
			//GUILayout.Label("Summary", EditorStyles.boldLabel);
		
			showStates = EditorGUILayout.Foldout(showStates, "States [" + fsmComponent.FsmStates.Length + "]");
			if (showStates)
			{
				string states = "";
				
				if (fsmComponent.FsmStates.Length > 0)
				{
					foreach (var state in fsmComponent.FsmStates)
					{
						states += "\t\t" + state.Name + "\n";
					}
					states = states.Substring(0,states.Length-1);
				}
				else
				{
					states = "\t\t[none]";
				}
				
				GUILayout.Label(states);
			}
			
			showEvents = EditorGUILayout.Foldout(showEvents, "Events [" + fsmComponent.FsmEvents.Length + "]");
			if (showEvents) 
			{
				string events = "";
				
				if (fsmComponent.FsmEvents.Length > 0)
				{
					foreach (var fsmEvent in fsmComponent.FsmEvents)
					{
						events += "\t\t" + fsmEvent.Name + "\n";
					}
					events = events.Substring(0,events.Length-1);
				}
				else
				{
					events = "\t\t[none]";
				}
				
				GUILayout.Label(events);
			}
			
			showVariables = EditorGUILayout.Foldout(showVariables, "Variables [" + fsmVariables.Count + "]");
			if (showVariables)
			{
				string variables = "";
				
				if (fsmVariables.Count > 0)
				{
					foreach (var fsmVar in fsmVariables)
					{
						variables += "\t\t" + fsmVar.Name + "\n";
					}
					variables = variables.Substring(0,variables.Length-1);
				}
				else
				{
					variables = "\t\t[none]";
				}
				
				GUILayout.Label(variables);
			}
		}
	}
	
	void BuildFsmVariableList()
	{
		fsmVariables = FsmVariable.GetFsmVariableList(fsmComponent.Fsm.Variables);

		fsmVariables.Sort();
	}
	
	#if iTweenPathEditing

	// Live iTween path editing
	
	iTweenMoveTo temp;
	Vector3[] tempVct3;
	FsmState lastSelectedState;
	
	public void OnSceneGUI()
	{
		// can happen when playmaker is updated
		if (fsmComponent == null)
		{
			return;
		}
		
		if(fsmComponent.Fsm.EditState != null){
			for(int k = 0; k<fsmComponent.Fsm.EditState.Actions.Length;k++){
				if(fsmComponent.Fsm.EditState.Actions[k] is iTweenMoveTo){
					temp = (iTweenMoveTo)fsmComponent.Fsm.EditState.Actions[k];
					if(temp.transforms.Length >= 2) {
							Undo.SetSnapshotTarget(fsmComponent.gameObject,"Adjust iTween Path");
							tempVct3 = new Vector3[temp.transforms.Length];
							for(int i = 0;i<temp.transforms.Length;i++){
								if(temp.transforms[i].IsNone) tempVct3[i] = temp.vectors[i].IsNone ? Vector3.zero : temp.vectors[i].Value; 
								else {
									if(temp.transforms[i].Value == null) tempVct3[i] = temp.vectors[i].IsNone ? Vector3.zero : temp.vectors[i].Value; 
									else tempVct3[i] = temp.transforms[i].Value.transform.position + (temp.vectors[i].IsNone ? Vector3.zero : temp.vectors[i].Value);
								}
								tempVct3[i] = Handles.PositionHandle(tempVct3[i], Quaternion.identity);
								if(temp.transforms[i].IsNone) { 
									if(!temp.vectors[i].IsNone) temp.vectors[i].Value = tempVct3[i];
								}
								else {
									if(temp.transforms[i].Value == null) {
										if(!temp.vectors[i].IsNone) temp.vectors[i].Value = tempVct3[i];
									}
									else {
										if(!temp.vectors[i].IsNone){
											temp.vectors[i] = tempVct3[i] - temp.transforms[i].Value.transform.position;
										} 
									}
								}
							}
							Handles.Label(tempVct3[0], "'" + fsmComponent.name + "' Begin");
							Handles.Label(tempVct3[tempVct3.Length-1], "'" + fsmComponent.name + "' End");
							if(GUI.changed) FsmEditor.EditingActions();
					} 
				}
			}	
		}
	}
	
	#endif
}


