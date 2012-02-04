// (c) Copyright HutongGames, LLC 2010-2011. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Effects)]
	[Tooltip("Flickers a Game Object on/off.")]
	public class Flicker : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault gameObject;
		[HasFloatSlider(0, 1)]
		public FsmFloat frequency;
		[HasFloatSlider(0, 1)]
		public FsmFloat amountOn;
		public bool rendererOnly;
		public bool realTime;
		
		private float startTime;
		private float timer;
		private bool state;
		
		public override void Reset()
		{
			gameObject = null;
			frequency = 0.1f;
			amountOn = 0.5f;
			rendererOnly = true;	
			realTime = false;
		}
	
		public override void OnEnter()
		{
			startTime = Time.realtimeSinceStartup;
			timer = 0f;
		}
		
		public override void OnUpdate()
		{
			// get target
			
			GameObject go = gameObject.OwnerOption == OwnerDefaultOption.UseOwner ? Owner : gameObject.GameObject.Value;
			if (go == null) return;
			
			// update time
			
			if (realTime)
			{
				timer = Time.realtimeSinceStartup - startTime;
			}
			else
			{
				timer += Time.deltaTime;
			}
			
			if (timer > frequency.Value)
			{
				bool on = Random.Range(0f,1f) < amountOn.Value ? true : false;

				// do flicker
				
				if (rendererOnly)
				{
					if (go.renderer != null)
						go.renderer.enabled = on;
				}
				else
				{
					go.active = on;
				}
				
				// reset timer
				
				startTime = timer;
				timer = 0;
			}
		}


		
	}
}