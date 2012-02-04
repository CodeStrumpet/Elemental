// (c) Copyright HutongGames, LLC 2010-2011. All rights reserved.

using System.Collections;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Level)]
	[Tooltip("Loads a Level by Name. Before you can load a level, you have to add it to the list of levels defined in File->Build Settings...")]
	public class LoadLevel : FsmStateAction
	{
		[RequiredField]
		public FsmString levelName;
		public bool additive;
		public bool async;
		public FsmEvent loadedEvent;
		public FsmBool dontDestroyOnLoad;
		
		public override void Reset()
		{
			levelName = "";
			additive = false;
			async = false;
			loadedEvent = null;
			dontDestroyOnLoad = true;
		}

		public override void OnEnter()
		{
			if (dontDestroyOnLoad.Value)
			{
				// Have to get the root, since this FSM will be destroyed if a parent is destroyed.
				
				Transform root = Owner.transform.root;
				Object.DontDestroyOnLoad(root.gameObject);
			}
			
			if (additive)
				if (async)
				{
					LoadLevelAdditiveAsync();
					return;
				}
				else
				{
					Application.LoadLevelAdditive(levelName.Value);
				}
			else
				if (async)
				{
					LoadLevelAsync();
					return;
				}
				else
				{
					Application.LoadLevel(levelName.Value);
				}

			if (async) return;
			Log("LOAD COMPLETE");
			Fsm.Event(loadedEvent);
			Finish();
		}

		IEnumerator LoadLevelAsync()
		{
			AsyncOperation asyncOperation = Application.LoadLevelAsync(levelName.Value);
			yield return asyncOperation;
			Log("LOAD COMPLETE");
			Fsm.Event(loadedEvent);
			Finish();
		}

		IEnumerator LoadLevelAdditiveAsync()
		{
			AsyncOperation asyncOperation = Application.LoadLevelAdditiveAsync(levelName.Value);
			yield return asyncOperation;
			Log("LOAD COMPLETE");
			Fsm.Event(loadedEvent);
			Finish();
		}
	}
}