
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;



namespace EasyExcel
{
	/// <summary>
	/// Xlsx converter window
	/// </summary>
	public class ExcelConverterWindow : EditorWindow
	{
		string sourceXlsxPath;
		string outputCSPath;
		string outputAssetPath;

		[MenuItem(@"Tools/EasyExcel")]
		public static void ShowExcelWindow()
		{
			Rect wr = new Rect(100, 100, 640, 480);
			var window = (ExcelConverterWindow)EditorWindow.GetWindowWithRect(typeof(ExcelConverterWindow), wr, true, "EasyExcel");
			window.Show();
		}

		void OnEnable()
		{
			sourceXlsxPath = Config.Instance.ExcelPath;
			outputCSPath = Config.Instance.CSharpPath;
			outputAssetPath = Config.Instance.AssetPath;
		}

		private void SavePrefs()
		{
			Config.Instance.ExcelPath = sourceXlsxPath;
			Config.Instance.CSharpPath = outputCSPath;
			Config.Instance.AssetPath = outputAssetPath;
			EditorUtility.SetDirty(Config.Instance);
		}

		private void OnDestroy()
		{
			SavePrefs();
		}

		void OnGUI()
		{
			float spaceSize = 10f;
			float leftSpace = 10;
			float titleLen = 80;
			float textLen = 450;
			float buttonLen1 = 120;
			float buttonLen2 = 80;
			float buttonHeight = 40;

			GUILayout.Label("  Import excel files as *.cs and *.asset.\n", EditorStyles.helpBox);
			GUILayout.Space(spaceSize);

			GUILayout.BeginHorizontal();
			GUILayout.Space(leftSpace);
			GUILayout.Label("xls/xlsx path", EditorStyles.label, GUILayout.Width(titleLen));
			sourceXlsxPath = GUILayout.TextField(sourceXlsxPath, GUILayout.Width(textLen));
			if (GUILayout.Button("Change", GUILayout.Width(buttonLen2)))
			{
				sourceXlsxPath = EditorUtility.OpenFolderPanel("Select .xls/xlsx path", String.Empty, "");
				SavePrefs();
			}

			GUILayout.EndHorizontal();
			GUILayout.Space(spaceSize);

			GUILayout.BeginHorizontal();
			GUILayout.Space(leftSpace);
			GUILayout.Label("CS path", EditorStyles.label, GUILayout.Width(titleLen));
			outputCSPath = GUILayout.TextField(outputCSPath, GUILayout.Width(textLen));
			if (GUILayout.Button("Change", GUILayout.Width(buttonLen2)))
			{
				outputCSPath = EditorUtility.OpenFolderPanel("Select .cs path", String.Empty, "");
				SavePrefs();
			}

			GUILayout.EndHorizontal();
			GUILayout.Space(spaceSize);

			GUILayout.BeginHorizontal();
			GUILayout.Space(leftSpace);
			GUILayout.Label("Asset path", EditorStyles.label, GUILayout.Width(titleLen));
			outputAssetPath = GUILayout.TextField(outputAssetPath, GUILayout.Width(textLen));
			if (GUILayout.Button("Change", GUILayout.Width(buttonLen2)))
			{
				outputAssetPath = EditorUtility.OpenFolderPanel("Select .asset path", String.Empty, "");
				SavePrefs();
			}

			GUILayout.EndHorizontal();
			GUILayout.Space(spaceSize);

			GUILayout.BeginHorizontal();
			GUILayout.Space(leftSpace);
			if (GUILayout.Button("Clear All", GUILayout.Width(buttonLen1), GUILayout.Height(buttonHeight)))
			{
				ClearAll();
			}
			if (GUILayout.Button("Generate *.cs", GUILayout.Width(buttonLen1), GUILayout.Height(buttonHeight)))
			{
				ToCSharps(sourceXlsxPath, outputCSPath);
				SavePrefs();
			}

			if (GUILayout.Button("Generate *.asset", GUILayout.Width(buttonLen1), GUILayout.Height(buttonHeight)))
			{
				ToAssets(sourceXlsxPath, outputAssetPath);
				SavePrefs();
			}
			
			GUILayout.EndHorizontal();
			GUILayout.Space(spaceSize);
		}

		void ClearAll()
		{
			if (Directory.Exists(outputCSPath))
				Directory.Delete(outputAssetPath, true);
			if (Directory.Exists(outputAssetPath))
				Directory.Delete(outputAssetPath, true);

			AssetDatabase.Refresh();
			EditorUtility.DisplayDialog("EasyExcel", "Clear done.", "OK");
		}

		void ToCSharps(string xlsxPath, string csPath)
		{
			try
			{
				if (!Directory.Exists(xlsxPath))
				{
					EditorUtility.DisplayDialog("EasyExcel", "Xls/xlsx path doesn't exist.", "OK");
					return;
				}
				if (!Directory.Exists(csPath))
				{
					EditorUtility.DisplayDialog("EasyExcel", "CS path doesn't exist.", "OK");
					return;
				}
				xlsxPath = xlsxPath.Replace("\\", "/");
				csPath = csPath.Replace("\\", "/");
				if (!csPath.EndsWith("/"))
					csPath += "/";
				if (Directory.Exists(csPath))
					Directory.Delete(csPath, true);
				Directory.CreateDirectory(csPath);

				string[] filePaths = Directory.GetFiles(xlsxPath);
				List<string> filenames = new List<string>();
				for (int i = 0; i < filePaths.Length; ++i)
				{
					string filePath = filePaths[i].Replace("\\", "/"); ;
					if (IsXlsxFile(filePath))
					{
						UpdateProgress(i, filePaths.Length, "");
						ExcelConverter.ToCSharp(filePath, csPath);
						int index = filePath.LastIndexOf("/") + 1;
						string fileName = filePath.Substring(index, filePath.LastIndexOf(".") - index);
						filenames.Add(fileName);
					}
				}

				EditorUtility.ClearProgressBar();
				SavePrefs();
				AssetDatabase.Refresh();
				EditorUtility.DisplayDialog("EasyExcel", "Convert done.", "OK");
			}
			catch (Exception e)
			{
				Debug.LogError(e.ToString());
				EditorUtility.ClearProgressBar();
				AssetDatabase.Refresh();
			}
		}

		void ToAssets(string xlsxPath, string assetPath)
		{
			try
			{
				if (!Directory.Exists(xlsxPath))
				{
					EditorUtility.DisplayDialog("EasyExcel", "Xls/xlsx path doesn't exist.", "OK");
					return;
				}
				if (!Directory.Exists(assetPath))
				{
					EditorUtility.DisplayDialog("EasyExcel", "Asset path doesn't exist.", "OK");
					return;
				}
				xlsxPath = xlsxPath.Replace("\\", "/");
				assetPath = assetPath.Replace("\\", "/");
				if (!assetPath.EndsWith("/"))
					assetPath += "/";
				if (Directory.Exists(assetPath))
					Directory.Delete(assetPath, true);
				Directory.CreateDirectory(assetPath);

				string[] filePaths = Directory.GetFiles(xlsxPath);
				for (int i = 0; i < filePaths.Length; ++i)
				{
					string filePath = filePaths[i].Replace("\\", "/"); ;
					if (IsXlsxFile(filePath))
					{
						UpdateProgress(i, filePaths.Length, "");
						ExcelConverter.ToAsset(filePath, assetPath);
					}
				}

				EditorUtility.ClearProgressBar();
				SavePrefs();
				AssetDatabase.Refresh();
				EditorUtility.DisplayDialog("EasyExcel", "Convert done.", "OK");
			}
			catch (Exception e)
			{
				Debug.LogError(e.ToString());
				EditorUtility.ClearProgressBar();
				AssetDatabase.Refresh();
			}
		}
		static bool IsXlsxFile(string filePath)
		{
			return filePath.EndsWith(".xlsx");
		}
		static void UpdateProgress(int progress, int progressMax, string desc)
		{
			string title = "Converting...[" + progress + " / " + progressMax + "]";
			float value = (float)progress / (float)progressMax;
			EditorUtility.DisplayProgressBar(title, desc, value);
		}

	}

}