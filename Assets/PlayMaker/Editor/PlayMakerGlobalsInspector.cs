// (c) Copyright HutongGames, LLC 2010-2011. All rights reserved.

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using HutongGames.PlayMakerEditor;

[CustomEditor(typeof(PlayMakerGlobals))]
class PlayMakerGlobalsInspector : Editor
{
	private PlayMakerGlobals globals;
	private bool refresh;

	private List<FsmVariable> variableList;

	void OnEnable()
	{
		globals = target as PlayMakerGlobals;

		BuildVariableList();
	}

	public override void OnInspectorGUI()
	{
		if (refresh)
		{
			Refresh();
			return;
		}

		GUILayout.Label("Global Variables", EditorStyles.boldLabel);

		if (variableList.Count > 0)
		{
			foreach (var fsmVariable in variableList)
			{
				var tooltip = fsmVariable.Name;

				if (!string.IsNullOrEmpty(fsmVariable.Tooltip))
				{
					tooltip += "\n" + fsmVariable.Tooltip;
				}

				fsmVariable.DoValueGUI(new GUIContent(fsmVariable.Name, tooltip));
			}
		}
		else
		{
			GUILayout.Label("[None]");
		}

		GUILayout.Label("Global Events", EditorStyles.boldLabel);

		if (globals.Events.Count > 0)
		{
			foreach (var eventName in globals.Events)
			{
				GUILayout.Label(eventName);
			}
		}
		else
		{
			GUILayout.Label("[None]");
		}

/*		GUILayout.Space(5);

		if (GUILayout.Button(new GUIContent("Refresh Lists", "Edit Globals in the PlayMaker FSM Editor.")) || globals.NeedsRefresh) // TODO
		{
			refresh = true;
			GUIUtility.ExitGUI();
		}*/
	}

	void Refresh()
	{
		refresh = false;
		BuildVariableList();
		Repaint();
	}

	void BuildVariableList()
	{
		variableList = FsmVariable.GetFsmVariableList(globals.Variables, globals);
	}
}
