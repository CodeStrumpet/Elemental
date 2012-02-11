using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public static class tk2dGuiUtility  
{
	public static Vector2 PositionHandle(int id, Vector2 position, float size, Color inactiveColor, Color activeColor)
	{
		KeyCode discardKeyCode = KeyCode.None;
		return PositionHandle(id, position, size, inactiveColor, activeColor, out discardKeyCode);
	}
	
	public static Vector2 PositionHandle(int id, Vector2 position, float size, Color inactiveColor, Color activeColor, out KeyCode keyCode)
	{
		Rect rect = new Rect(position.x - size, position.y - size, size * 2, size * 2);
		int controlID = GUIUtility.GetControlID(id, FocusType.Passive);
		keyCode = KeyCode.None;
		
		switch (Event.current.GetTypeForControl(controlID))
		{
			case EventType.MouseDown:
			{
				if (rect.Contains(Event.current.mousePosition))
				{
					GUIUtility.hotControl = controlID;
					Event.current.Use();
				}
				break;
			}
			
			case EventType.MouseDrag:
			{
				if (GUIUtility.hotControl == controlID)				
				{
					position = Event.current.mousePosition;
					Event.current.Use();					
				}
				break;
			}
			
			case EventType.MouseUp:
			{
				if (GUIUtility.hotControl == controlID)
				{
					GUIUtility.hotControl = 0;
					Event.current.Use();
				}
				break;
			}
			
			case EventType.KeyDown:
			{
				if (rect.Contains(Event.current.mousePosition))
				{
					keyCode = Event.current.keyCode;
					if (GUIUtility.hotControl == controlID)
					{
						GUIUtility.hotControl = 0;
						Event.current.Use();
					}
				}
				break;
			}
			
			case EventType.Repaint:
			{
				Color oc = Handles.color;
				Handles.color = (GUIUtility.hotControl == controlID)?activeColor:inactiveColor;
			
				Vector3[] pts = new Vector3[] {
					new Vector3(rect.xMin, rect.yMin, 0.0f),
					new Vector3(rect.xMax, rect.yMin, 0.0f),
					new Vector3(rect.xMax, rect.yMax, 0.0f),
					new Vector3(rect.xMin, rect.yMax, 0.0f),
				};
				Handles.DrawSolidRectangleWithOutline(pts, oc, oc);			
			
				Handles.color = oc;
			
				break;
			}
		}
		
		return position;
	}
}
