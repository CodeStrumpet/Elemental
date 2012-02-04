// (c) Copyright HutongGames, LLC 2010-2011. All rights reserved.

using UnityEngine;
using System.Collections.Generic;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GUILayout)]
	[Tooltip("GUILayout Box.")]
	public class GUILayoutBox : GUILayoutAction
	{
		public FsmTexture image;
		public FsmString text;
		public FsmString tooltip;
		public FsmString style;

		public override void Reset()
		{
			text = "";
			image = null;
			tooltip = "";
			style = "Box";
			layoutOptions = new LayoutOption[0];
		}
		
		public override void OnGUI()
		{
			GUILayout.Box(new GUIContent(text.Value, image.Value, tooltip.Value), style.Value, LayoutOptions);
		}
	}
}