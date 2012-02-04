// (c) Copyright HutongGames, LLC 2010-2011. All rights reserved.

using UnityEngine;
using System.Collections.Generic;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GUILayout)]
	[Tooltip("GUILayout Repeat Button. Sends an Event while pressed. Optionally store the button state in a Bool Variable.")]
	public class GUILayoutRepeatButton : GUILayoutAction
	{
		public FsmEvent sendEvent;
		[UIHint(UIHint.Variable)]
		public FsmBool storeButtonState;
		public FsmTexture image;
		public FsmString text;
		public FsmString tooltip;
		public FsmString style;

		public override void Reset()
		{
			sendEvent = null;
			storeButtonState = null;
			text = "";
			image = null;
			tooltip = "";
			style = "Button";
		}
		
		public override void OnGUI()
		{
			bool pressed = false;
			if (GUILayout.RepeatButton(new GUIContent(text.Value, image.Value, tooltip.Value), style.Value, LayoutOptions))
			{
				Fsm.Event(sendEvent);
				pressed = true;
			}
			storeButtonState.Value = pressed;
		}
	}
}