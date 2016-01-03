//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

using UnityEditor;
using UnityEngine;

namespace Gamelogic.Editor
{
	/**
		@version1_2
	*/
	public static class GLEditorGUI
	{
		public static readonly GUIStyle SplitterStyle;

		static GLEditorGUI()
		{
			SplitterStyle = new GUIStyle
			{
				normal = {background = EditorGUIUtility.whiteTexture},
				stretchWidth = true,
				margin = new RectOffset(0, 0, 7, 7)
			};
		}

		private static readonly Color splitterColor = EditorGUIUtility.isProSkin ? new Color(0.157f, 0.157f, 0.157f) : new Color(0.5f, 0.5f, 0.5f);

		// GUILayout Style
		public static void Splitter(Color rgb, float thickness = 1)
		{
			Rect position = GUILayoutUtility.GetRect(GUIContent.none, SplitterStyle, GUILayout.Height(thickness));

			if (Event.current.type == EventType.Repaint)
			{
				Color restoreColor = GUI.color;
				GUI.color = rgb;
				SplitterStyle.Draw(position, false, false, false, false);
				GUI.color = restoreColor;
			}
		}

		public static void Splitter(float thickness, GUIStyle splitterStyle)
		{
			Rect position = GUILayoutUtility.GetRect(GUIContent.none, splitterStyle, GUILayout.Height(thickness));

			if (Event.current.type == EventType.Repaint)
			{
				Color restoreColor = GUI.color;
				GUI.color = splitterColor;
				splitterStyle.Draw(position, false, false, false, false);
				GUI.color = restoreColor;
			}
		}

		public static void Splitter(float thickness = 1)
		{
			Splitter(thickness, SplitterStyle);
		}

		// GUI Style
		public static void Splitter(Rect position)
		{
			if (Event.current.type == EventType.Repaint)
			{
				Color restoreColor = GUI.color;
				GUI.color = splitterColor;
				SplitterStyle.Draw(position, false, false, false, false);
				GUI.color = restoreColor;
			}
		}
	}
}