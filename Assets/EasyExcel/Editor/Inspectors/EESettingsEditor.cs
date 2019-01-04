using System;
using UnityEditor;
using UnityEngine;

namespace EasyExcel
{
	public class EESettingsEditor : EditorWindow
	{
		[MenuItem("Tools/EasyExcel/Settings", false, 400)]
		public static void OpenSettingsWindow()
		{
			try
			{
				if (EditorApplication.isCompiling)
				{
					EELog.Log("Waiting for Compiling completed.");
					return;
				}
				var window = GetWindowWithRect<EESettingsEditor>(new Rect(0, 0, 480, 640), true, "Settings", true);
				window.Show();
			}
			catch (Exception e)
			{
				EELog.LogError(e.ToString());
			}
		}

		private const int nameLength = 100;
		private const int valueLength = 300;
		private const int nameHeight = 18;
		private const int helpLength = 405;
		
		private EESettings settings;
		private GUILayoutOption[] nameOptions;
		private GUILayoutOption[] valueOptions;

		private void Awake()
		{
			settings = EESettings.Current;
			nameOptions = new[] {GUILayout.Width(nameLength), GUILayout.Height(nameHeight)};
			valueOptions = new[] {GUILayout.Width(valueLength), GUILayout.Height(nameHeight)};
		}

		private void OnGUI()
		{
			EEGUIStyle.Ensure();
			
			if (settings == null)
			{
				EditorGUILayout.HelpBox("Cannot find EasyExcel settings file", MessageType.Error);
				return;
			}

			GUILayout.Space(5);
			GUILayout.Label("EasyExcel Settings", EEGUIStyle.largeLabel);
			GUILayout.Space(10);

			settings.ShowHelp = GUILayout.Toggle(settings.ShowHelp, "Show Help");
			
			GUILayout.Space(5);
			if (settings.ShowHelp)
				GUILayout.Label(GetSettingFieldComment("NameRowIndex"), EEGUIStyle.helpBox, GUILayout.Width(helpLength));
			GUILayout.BeginHorizontal();
			GUILayout.Label("Row of Name", EEGUIStyle.label, nameOptions);
			settings.NameRowIndex = EditorGUILayout.IntField(settings.NameRowIndex, EEGUIStyle.textField, valueOptions);
			GUILayout.EndHorizontal();
			if (settings.ShowHelp)
				GUILayout.Space(10);
			
			GUILayout.Space(5);
			if (settings.ShowHelp)
				GUILayout.Label(GetSettingFieldComment("TypeRowIndex"), EEGUIStyle.helpBox, GUILayout.Width(helpLength));
			GUILayout.BeginHorizontal();
			GUILayout.Label("Row of Type", EEGUIStyle.label, nameOptions);
			settings.TypeRowIndex = EditorGUILayout.IntField(settings.TypeRowIndex, EEGUIStyle.textField, valueOptions);
			GUILayout.EndHorizontal();
			if (settings.ShowHelp)
				GUILayout.Space(10);
			
			GUILayout.Space(5);
			if (settings.ShowHelp)
				GUILayout.Label(GetSettingFieldComment("DataStartIndex"), EEGUIStyle.helpBox, GUILayout.Width(helpLength));
			GUILayout.BeginHorizontal();
			GUILayout.Label("Row of Data", EEGUIStyle.label, nameOptions);
			settings.DataStartIndex = EditorGUILayout.IntField(settings.DataStartIndex, EEGUIStyle.textField, valueOptions);
			GUILayout.EndHorizontal();
			if (settings.ShowHelp)
				GUILayout.Space(10);
			
			GUILayout.Space(5);
			if (settings.ShowHelp)
				GUILayout.Label(GetSettingFieldComment("NameSpace"), EEGUIStyle.helpBox, GUILayout.Width(helpLength));
			GUILayout.BeginHorizontal();
			GUILayout.Label("Name Space", EEGUIStyle.label, nameOptions);
			settings.NameSpace = EditorGUILayout.TextField(settings.NameSpace, EEGUIStyle.textField, valueOptions);
			GUILayout.EndHorizontal();
			if (settings.ShowHelp)
				GUILayout.Space(10);
			
			/*GUILayout.Space(5);
			if (settings.ShowHelp)
				GUILayout.Label(GetSettingFieldComment("RowDataClassNamePostfix"), EEGUIStyle.helpBox, GUILayout.Width(helpLength));
			GUILayout.BeginHorizontal();
			GUILayout.Label("RowDataClassNamePostfix", EEGUIStyle.label, nameOptions);
			settings.RowDataPostfix = EditorGUILayout.TextField(settings.RowDataPostfix, EEGUIStyle.textField, valueOptions);
			GUILayout.EndHorizontal();
			if (settings.ShowHelp)
				GUILayout.Space(10);
			
			GUILayout.Space(5);
			if (settings.ShowHelp)
				GUILayout.Label(GetSettingFieldComment("DataTableClassNamePostfix"), EEGUIStyle.helpBox, GUILayout.Width(helpLength));
			GUILayout.BeginHorizontal();
			GUILayout.Label("DataTableClassNamePostfix", EEGUIStyle.label, nameOptions);
			settings.DataTablePostfix = EditorGUILayout.TextField(settings.DataTablePostfix, EEGUIStyle.textField, valueOptions);
			GUILayout.EndHorizontal();
			if (settings.ShowHelp)
				GUILayout.Space(10);*/
			
			GUILayout.Space(5);
			if (settings.ShowHelp)
				GUILayout.Label(GetSettingFieldComment("GeneratedAssetPath"), EEGUIStyle.helpBox, GUILayout.Width(helpLength));
			GUILayout.BeginHorizontal();
			GUILayout.Label("AssetPath", EEGUIStyle.label, nameOptions);
			settings.GeneratedAssetPath = EditorGUILayout.TextField(settings.GeneratedAssetPath, EEGUIStyle.textField, valueOptions);
			GUILayout.EndHorizontal();
			if (settings.ShowHelp)
				GUILayout.Space(10);
			
			GUILayout.Space(5);
			if (settings.ShowHelp)
				GUILayout.Label(GetSettingFieldComment("GeneratedScriptPath"), EEGUIStyle.helpBox, GUILayout.Width(helpLength));
			GUILayout.BeginHorizontal();
			GUILayout.Label("CSharpPath", EEGUIStyle.label, nameOptions);
			settings.GeneratedScriptPath = EditorGUILayout.TextField(settings.GeneratedScriptPath, EEGUIStyle.textField, valueOptions);
			GUILayout.EndHorizontal();
			if (settings.ShowHelp)
				GUILayout.Space(10);
		}

		private string GetSettingFieldComment(string fieldName)
		{
			return EEUtility.GetFieldComment(typeof(EESettings), fieldName);
		}
		
	}
}