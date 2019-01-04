using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace EasyExcel
{
	[CreateAssetMenu(fileName = "New EasyExcel Settings", menuName = "EasyExcel/Settings", order = 999)]
	public class EESettings : ScriptableObject
	{
		private const string EESettingsSavedFileName = "EasyExcelSettings";
		private const string EESettingsFileExtension = ".asset";
		private const string EESettingsSavedPath = "Assets/Resources/" + EESettingsSavedFileName + EESettingsFileExtension;
		
		[EEComment("This row in a excel sheet is Name")]
		public int NameRowIndex;
		
		[EEComment("This row in a excel sheet is Type")]
		public int TypeRowIndex;
		
		[EEComment("This row in a excel sheet is where real data starts")]
		public int DataStartIndex;
		
		[EEComment("This is where the generated ScriptableObject files will be.")]
		public string GeneratedAssetPath;
		
		[EEComment("This is where the generated csharp files will be.")]
		public string GeneratedScriptPath;
		
		[EEComment(@"Postfix of generated data table classes.
For example if we set it as 'Aaa' then Item.xlsx will be generated as 'ItemAaa'")]
		public string DataTablePostfix;
		
		[EEComment(@"Postfix of generated row data classes.
For example if we set it as 'Bbb' then Item.xlsx will be generated as 'ItemBbb'")]
		public string RowDataPostfix;

		[EEComment("The namespace of generated classes from excel files.")]
		public string NameSpace;

		[NonSerialized] public bool ShowHelp = true;

		public void ResetAll()
		{
			NameRowIndex = 0;
			TypeRowIndex = 1;
			DataStartIndex = 2;
			GeneratedAssetPath = "Assets/Resources/EasyExcelGeneratedAsset/";
			GeneratedScriptPath = "Assets/EasyExcel/Example/AutoGenCode/";
			DataTablePostfix = "_Generated";
			RowDataPostfix = "";
			NameSpace = "EasyExcelGenerated";
		}

		#region Sigleton

		private static EESettings _current;
		
		public static EESettings Current
		{
			get
			{
				if (_current != null) return _current;
				_current = Resources.Load<EESettings>(EESettingsSavedFileName);
				if (_current != null) return _current;
				_current = CreateInstance<EESettings>();
				_current.ResetAll();
#if UNITY_EDITOR
				var resourcesPath = Application.dataPath + "/Resources";
				if (!Directory.Exists(resourcesPath))
				{
					Directory.CreateDirectory(resourcesPath);
					AssetDatabase.Refresh();
				}

				AssetDatabase.CreateAsset(_current, EESettingsSavedPath);
#endif
				return _current;
			}
		}

		#endregion

		public string GetRowDataClassName(string sheetName, bool includeNameSpace = false)
		{
			return (includeNameSpace? NameSpace + "." : null) + sheetName + RowDataPostfix;
		}

		public string GetDataTableClassName(string sheetName)
		{
			return sheetName + DataTablePostfix;
		}

		public static string GetAssetFileName(string sheetName)
		{
			return sheetName + EESettingsFileExtension;
		}
		
		public string GetCSharpFileName(string sheetName)
		{
			// The file name must not differ from the name of ScriptableObject class
			return GetDataTableClassName(sheetName) + ".cs";
		}
	}
}