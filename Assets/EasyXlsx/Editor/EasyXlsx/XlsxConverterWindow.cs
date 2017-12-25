
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;



namespace EasyXlsx
{
	/// <summary>
	/// Xlsx converter window
	/// </summary>
	public class XlsxConverterWindow : EditorWindow
	{
		string sourceXlsxPath;
		string outputCSPath;
		string outputAssetPath;

		[MenuItem(@"Tools/EasyXlsx")]
		public static void ShowExcelWindow()
		{
			Rect wr = new Rect(100, 100, 640, 480);
			var window = (XlsxConverterWindow)EditorWindow.GetWindowWithRect(typeof(XlsxConverterWindow), wr, true, "EasyXlsx");
			window.Show();
		}

		void OnEnable()
		{
			sourceXlsxPath = EditorPrefs.GetString("XlsxPath", null);
			outputCSPath = EditorPrefs.GetString("genCodePath", null);
			outputAssetPath = EditorPrefs.GetString("assetPath", null);
			if (string.IsNullOrEmpty(outputAssetPath))
				outputAssetPath = Application.dataPath + Config.AssetPath;
		}

		private void SavePrefs()
		{
			EditorPrefs.SetString("XlsxPath", sourceXlsxPath);
			EditorPrefs.SetString("genCodePath", outputCSPath);
			EditorPrefs.SetString("assetPath", outputAssetPath);
		}

		private void OnDestroy()
		{
			SavePrefs();
		}

		void OnGUI()
		{
			float spaceSize = 10f;
			float leftSpace = 10;
			float titleLen = 70;
			float textLen = 450;
			float buttonLen1 = 120;
			float buttonLen2 = 80;
			float buttonHeight = 40;

			GUILayout.Label("  Import *.xlsx files as *.cs and *.asset.\n", EditorStyles.helpBox);
			GUILayout.Space(spaceSize);

			GUILayout.BeginHorizontal();
			GUILayout.Space(leftSpace);
			GUILayout.Label("Xlsx path", EditorStyles.label, GUILayout.Width(titleLen));
			sourceXlsxPath = GUILayout.TextField(sourceXlsxPath, GUILayout.Width(textLen));
			if (GUILayout.Button("Change", GUILayout.Width(buttonLen2)))
				sourceXlsxPath = EditorUtility.OpenFolderPanel("Select .xlsx path", String.Empty, "");
			
			GUILayout.EndHorizontal();
			GUILayout.Space(spaceSize);

			GUILayout.BeginHorizontal();
			GUILayout.Space(leftSpace);
			GUILayout.Label("CS path", EditorStyles.label, GUILayout.Width(titleLen));
			outputCSPath = GUILayout.TextField(outputCSPath, GUILayout.Width(textLen));
			if (GUILayout.Button("Change", GUILayout.Width(buttonLen2)))
				outputCSPath = EditorUtility.OpenFolderPanel("Select .cs path", String.Empty, "");

			GUILayout.EndHorizontal();
			GUILayout.Space(spaceSize);

			GUILayout.BeginHorizontal();
			GUILayout.Space(leftSpace);
			GUILayout.Label("Asset path", EditorStyles.label, GUILayout.Width(titleLen));
			outputAssetPath = GUILayout.TextField(outputAssetPath, GUILayout.Width(textLen));
			if (GUILayout.Button("Change", GUILayout.Width(buttonLen2)))
				outputAssetPath = EditorUtility.OpenFolderPanel("Select .asset path", String.Empty, "");

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
			string[] filePaths = Directory.GetFiles(outputCSPath);
			for (int i = 0; i < filePaths.Length; ++i)
			{
				string filePath = filePaths[i];
				if (!filePath.Contains(XlsxConverter.GetClassListName()))
					File.Delete(filePath);
			}

			if (Directory.Exists(outputAssetPath))
				Directory.Delete(outputAssetPath, true);

			AssetDatabase.Refresh();
			Debug.Log("Clear done.");
		}

		void ToCSharps(string xlsxPath, string csPath)
		{
			try
			{
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
						XlsxConverter.ToCSharp(filePath, csPath);
						int index = filePath.LastIndexOf("/") + 1;
						string fileName = filePath.Substring(index, filePath.LastIndexOf(".") - index);
						filenames.Add(fileName);
					}
				}

				EditorUtility.ClearProgressBar();
				AssetDatabase.Refresh();
				Debug.Log("Convert done.");
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
						XlsxConverter.ToAsset(filePath, assetPath);
					}
				}

				EditorUtility.ClearProgressBar();
				AssetDatabase.Refresh();
				Debug.Log("Convert done.");
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
			string title = "Processing...[" + progress + " / " + progressMax + "]";
			float value = (float)progress / (float)progressMax;
			EditorUtility.DisplayProgressBar(title, desc, value);
		}

	}

}