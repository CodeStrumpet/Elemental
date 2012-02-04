// (c) Copyright HutongGames, LLC 2010-2011. All rights reserved.

using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Physics)]
	[Tooltip("Tests if a Game Object's Rigid Body is sleeping.")]
	public class IsSleeping : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody))]
		public FsmOwnerDefault gameObject;
		public FsmEvent trueEvent;
		public FsmEvent falseEvent;
		[UIHint(UIHint.Variable)]
		public FsmBool store;
		public bool everyFrame;
		
		public override void Reset()
		{
			gameObject = null;
			trueEvent = null;
			falseEvent = null;
			store = null;
			everyFrame = false;
		}
		
		public override void OnEnter()
		{
			DoIsSleeping();
			
			if (!everyFrame)
				Finish();
		}

		public override void OnUpdate()
		{
			DoIsSleeping();
		}

		void DoIsSleeping()
		{
			GameObject go = gameObject.OwnerOption == OwnerDefaultOption.UseOwner ? Owner : gameObject.GameObject.Value;
			
			if (go == null) return;
			if (go.rigidbody == null) return;
			
			bool isSleeping = go.rigidbody.IsSleeping();
			
			if (store != null)
				store.Value = isSleeping;
			
			if (isSleeping)
				Fsm.Event(trueEvent);
			else
				Fsm.Event(falseEvent);
		}
	}
}

