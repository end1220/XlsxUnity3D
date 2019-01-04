using System;
using UnityEditor;
using UnityEngine;

namespace EasyExcel
{
	internal class EEAboutWindow : EditorWindow
	{
		[MenuItem(@"Tools/EasyExcel/About", false, 500)]
		private static void OpenAboutWindow()
		{
			try
			{
				if (EditorApplication.isCompiling)
				{
					EELog.Log("Waiting for Compiling completed.");
					return;
				}
				var window = GetWindowWithRect<EEAboutWindow>(new Rect(0, 0, 480, 320), true, "About EasyExcel", true);
				window.Show();
			}
			catch (Exception e)
			{
				EELog.LogError(e.ToString());
			}
		}

		private void OnGUI()
		{
			EEGUIStyle.Ensure();
			
			GUILayout.Space(10);
			GUILayout.Label("EasyExcel", EEGUIStyle.largeLabel);
			
			GUILayout.BeginHorizontal();
			GUILayout.Space(20);
			GUILayout.Label("Version " + EEConstant.Version, EEGUIStyle.label);
			GUILayout.EndHorizontal();
			
			GUILayout.BeginHorizontal();
			GUILayout.Space(20);
			GUILayout.Label("(c) 2019 Locke. All rights reserved.", EEGUIStyle.label);
			GUILayout.EndHorizontal();
			
			GUILayout.Space(20);
			
			GUILayout.Label("Support", EEGUIStyle.boldLabel);
			GUILayout.BeginHorizontal();
			GUILayout.Space(20);
			GUILayout.Label("email 1534921818@qq.com", EEGUIStyle.label);
			GUILayout.EndHorizontal();
		}
		
	}
}