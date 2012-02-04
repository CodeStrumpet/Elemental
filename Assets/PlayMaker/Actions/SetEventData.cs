// (c) Copyright HutongGames, LLC 2010-2011. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.StateMachine)]
	[Tooltip("Sets Event Data before sending an event. Get the Event Data, along with sender information, using Get Event Info action.")]
	public class SetEventData : FsmStateAction
	{
		public FsmGameObject setGameObjectData;
		public FsmInt setIntData;
		public FsmFloat setFloatData;
		public FsmString setStringData;
		public FsmBool setBoolData;
		public FsmVector3 setVector3Data;
		public FsmRect setRectData;
		public FsmQuaternion setQuaternionData;
		public FsmColor setColorData;
		public FsmMaterial setMaterialData;
		public FsmTexture setTextureData;
		public FsmObject setObjectData;

		public override void Reset()
		{
			setBoolData = null;
			setIntData = null;
			setFloatData = null;
			setVector3Data = null;
			setStringData = null;
			setGameObjectData = null;
		}

		public override void OnEnter()
		{
			Fsm.EventData.BoolData = setBoolData.Value;
			Fsm.EventData.IntData = setIntData.Value;
			Fsm.EventData.FloatData = setFloatData.Value;
			Fsm.EventData.Vector3Data = setVector3Data.Value;
			Fsm.EventData.StringData = setStringData.Value;
			Fsm.EventData.ObjectData = setGameObjectData.Value;
			Fsm.EventData.RectData = setRectData.Value;
			Fsm.EventData.QuaternionData = setQuaternionData.Value;
			Fsm.EventData.ColorData = setColorData.Value;
			Fsm.EventData.MaterialData = setMaterialData.Value;
			Fsm.EventData.TextureData = setTextureData.Value;
			Fsm.EventData.ObjectData = setObjectData.Value;
		
			Finish();
		}
	}
}