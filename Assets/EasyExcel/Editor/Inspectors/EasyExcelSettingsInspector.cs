using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

namespace EasyExcel
{
	[CustomEditor(typeof(EESettings))]
	public class EasyExcelSettingsInspector : Editor
	{
		public override void OnInspectorGUI()
		{
			EditorGUILayout.HelpBox(@"To modify this, use the settings window.", MessageType.Info);
			EditorGUILayout.Separator();
			
			var prevGUIState = GUI.enabled;
			GUI.enabled = false;
			base.OnInspectorGUI();
			GUI.enabled = prevGUIState;
			
			EditorGUILayout.Separator();
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Edit", GUILayout.Width(100), GUILayout.Height(20)))
				EESettingsEditor.OpenSettingsWindow();
			GUILayout.Space(50);
			if (GUILayout.Button("Reset", GUILayout.Width(100), GUILayout.Height(20)))
			{
				if (EditorUtility.DisplayDialog("EasyExcel", "Are you sure to reset it?", "Yes", "Cancel"))
				{
					EESettings.Current.ResetAll();
					EditorUtility.SetDirty(EESettings.Current);
				}
			}
			GUILayout.EndHorizontal();
		}
		
		[OnOpenAsset(10)]
		private static bool OnOpenExcelFile(int instanceId, int line)
		{
			try
			{
				var asset = EditorUtility.InstanceIDToObject(instanceId) as EESettings;
				if (asset == null)
					return false;
				EESettingsEditor.OpenSettingsWindow();
				return true;
			}
			catch (Exception e)
			{
				EELog.LogError(e.ToString());
			}

			return false;
		}
		
	}
}