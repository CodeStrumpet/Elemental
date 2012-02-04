// (c) Copyright HutongGames, LLC 2010-2011. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Physics)]
	[Tooltip("Casts a Ray against all Colliders in the scene. Use either a Game Object or Vector3 world position as the origin of the ray. Use GetRaycastInfo to get more detailed info.")]
	public class Raycast : FsmStateAction
	{
		[Tooltip("Start ray at game object position. \nOr use Origin parameter.")]
		public FsmOwnerDefault fromGameObject;
		[Tooltip("Start ray at a vector3 world position. \nOr use Game Object parameter.")]
		public FsmVector3 fromPosition;
		[Tooltip("A vector3 direction vector")]
		public FsmVector3 direction;
		[Tooltip("The length of the ray. Set to -1 for infinity.")]
		public FsmFloat distance;
		[Tooltip("Event to send if the ray hits an object.")]
		[UIHint(UIHint.Variable)]
		public FsmEvent hitEvent;
		[Tooltip("Set a bool variable to true if hit something, otherwise false.")]
		[UIHint(UIHint.Variable)]
		public FsmBool storeDidHit;
		[Tooltip("Store the game object hit in a variable.")]
		[UIHint(UIHint.Variable)]
		public FsmGameObject storeHitObject;
		[Tooltip("Set how often to cast a ray. 0 = once, don't repeat; 1 = everyFrame; 2 = every other frame... \nSince raycasts can get expensive use the highest repeat interval you can get away with.")]
		public FsmInt repeatInterval;
		[Tooltip("Pick only from these layers.")]
		[UIHint(UIHint.Layer)]
		public FsmInt[] layerMask;
		[Tooltip("Invert the mask, so you pick from all layers except those defined above.")]
		public FsmBool invertMask;
		public FsmBool debug;
		
		int repeat;
		
		public override void Reset()
		{
			fromGameObject = null;
			fromPosition = new FsmVector3 { UseVariable = true };
			direction = new FsmVector3 { UseVariable = true };
			distance = 100;
			hitEvent = null;
			storeDidHit = null;
			storeHitObject = null;
			repeatInterval = 0;
			layerMask = new FsmInt[0];
			invertMask = false;		
			debug = false;
		}

		public override void OnEnter()
		{
			DoRaycast();
			
			if (repeatInterval.Value == 0)
				Finish();		
		}

		public override void OnUpdate()
		{
			repeat--;
			
			if (repeat == 0)
				DoRaycast();
		}
		
		void DoRaycast()
		{
			repeat = repeatInterval.Value;

			if (distance.Value == 0)
				return;
			
			Vector3 originPos;
			
			var go = Fsm.GetOwnerDefaultTarget(fromGameObject);
			
			if (go != null)
				originPos = go.transform.position;
			else
				originPos = fromPosition.Value;
			
			float rayLength = Mathf.Infinity;
			if (distance.Value > 0 )
				rayLength = distance.Value;
			
			RaycastHit hitInfo;
			Physics.Raycast(originPos, direction.Value, out hitInfo, rayLength, ActionHelpers.LayerArrayToLayerMask(layerMask, invertMask.Value)); //TODO LayerMask support
			
			Fsm.RaycastHitInfo = hitInfo;
			
			bool didHit = hitInfo.collider != null;
			
			storeDidHit.Value = didHit;
			
			if (didHit)
			{
				Fsm.Event(hitEvent);
				storeHitObject.Value = hitInfo.collider.collider.gameObject;
			}
			
			if (debug.Value)
			{
				float debugRayLength = Mathf.Min(rayLength, 1000);
				Debug.DrawLine(originPos, originPos + direction.Value * debugRayLength, Fsm.DebugRaycastColor);
			}
		}
	}
}

