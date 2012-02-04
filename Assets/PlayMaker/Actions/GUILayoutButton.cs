// (c) Copyright HutongGames, LLC 2010-2011. All rights reserved.

using UnityEngine;
using System.Collections.Generic;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GUILayout)]
	[Tooltip("GUILayout Button. Sends an Event when pressed. Optionally stores the button state in a Bool Variable.")]
	public class GUILayoutButton : GUILayoutAction
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
			base.Reset();
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
			if (GUILayout.Button(new GUIContent(text.Value, image.Value, tooltip.Value), style.Value, LayoutOptions))
			{
				Fsm.Event(sendEvent);
				pressed = true;
			}
			
			if (storeButtonState != null)
			{
				storeButtonState.Value = pressed;
			}
		}
	}
}