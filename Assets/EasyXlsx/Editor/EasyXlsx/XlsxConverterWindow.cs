
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

		[MenuItem(@"Tools/EasyXlsx")]
		public static void ShowExcelWindow()
		{
			Rect wr = new Rect(100, 100, 600, 400);
			var window = (XlsxConverterWindow)EditorWindow.GetWindowWithRect(typeof(XlsxConverterWindow), wr, true, "EasyXlsx");
			window.Show();
		}



		string genCodePath;
		string xlsxPath;
		string assetPath;



		void OnEnable()
		{
			xlsxPath = EditorPrefs.GetString("XlsxPath", null);
			genCodePath = EditorPrefs.GetString("genCodePath", null);
			assetPath = EditorPrefs.GetString("assetPath", null);
			if (string.IsNullOrEmpty(assetPath))
				assetPath = Application.dataPath + Config.AssetPath;
		}


		private void SavePrefs()
		{
			EditorPrefs.SetString("XlsxPath", xlsxPath);
			EditorPrefs.SetString("genCodePath", genCodePath);
			EditorPrefs.SetString("assetPath", assetPath);
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
			float buttonLen1 = 100;
			float buttonLen2 = 50;
			float buttonHeight = 40;

			GUILayout.Label("  Convert xlsx to csharp, asset.\n", EditorStyles.helpBox);
			GUILayout.Space(spaceSize);

			GUILayout.BeginHorizontal();
			GUILayout.Space(leftSpace);
			GUILayout.Label("Xlsx path", EditorStyles.label, GUILayout.Width(titleLen));
			xlsxPath = GUILayout.TextField(xlsxPath, GUILayout.Width(textLen));
			if (GUILayout.Button("Change", GUILayout.Width(buttonLen2)))
				xlsxPath = EditorUtility.OpenFolderPanel("Select xlsx path", String.Empty, "");
			
			GUILayout.EndHorizontal();
			GUILayout.Space(spaceSize);

			GUILayout.BeginHorizontal();
			GUILayout.Space(leftSpace);
			GUILayout.Label("CS path", EditorStyles.label, GUILayout.Width(titleLen));
			genCodePath = GUILayout.TextField(genCodePath, GUILayout.Width(textLen));

			GUILayout.EndHorizontal();
			GUILayout.Space(spaceSize);

			GUILayout.BeginHorizontal();
			GUILayout.Space(leftSpace);
			GUILayout.Label("Asset path", EditorStyles.label, GUILayout.Width(titleLen));
			assetPath = GUILayout.TextField(assetPath, GUILayout.Width(textLen));
			
			GUILayout.EndHorizontal();
			GUILayout.Space(spaceSize);

			GUILayout.BeginHorizontal();
			GUILayout.Space(leftSpace);
			if (GUILayout.Button("Clear All", GUILayout.Width(buttonLen1), GUILayout.Height(buttonHeight)))
			{
				ClearAll();
			}
			if (GUILayout.Button("To csharp", GUILayout.Width(buttonLen1), GUILayout.Height(buttonHeight)))
			{
				ToCSharps(xlsxPath);
				SavePrefs();
			}

			if (GUILayout.Button("To asset", GUILayout.Width(buttonLen1), GUILayout.Height(buttonHeight)))
			{
				ToAssets(xlsxPath);
				SavePrefs();
			}
			
			GUILayout.EndHorizontal();
			GUILayout.Space(spaceSize);
		}


		void ClearAll()
		{
			string[] filePaths = Directory.GetFiles(genCodePath);
			for (int i = 0; i < filePaths.Length; ++i)
			{
				string filePath = filePaths[i];
				if (!filePath.Contains(XlsxConverter.GetClassListName()))
					File.Delete(filePath);
			}

			if (Directory.Exists(assetPath))
				Directory.Delete(assetPath, true);

			XlsxConverter.ClearClassList(genCodePath);

			AssetDatabase.Refresh();
			Debug.Log("Clear done.");
		}


		void ToCSharps(string sourcePath)
		{
			try
			{
				if (Directory.Exists(genCodePath))
					Directory.Delete(genCodePath, true);
				Directory.CreateDirectory(genCodePath);

				string[] filePaths = Directory.GetFiles(sourcePath);
				List<string> filenames = new List<string>();
				for (int i = 0; i < filePaths.Length; ++i)
				{
					string filePath = filePaths[i].Replace("\\", "/"); ;
					if (IsXlsxFile(filePath))
					{
						UpdateProgress(i, filePaths.Length, "");
						XlsxConverter.ToCSharp(filePath, genCodePath);
						int index = filePath.LastIndexOf("/") + 1;
						string fileName = filePath.Substring(index, filePath.LastIndexOf(".") - index);
						filenames.Add(fileName);
					}
				}

				XlsxConverter.GenerateClassList(filenames.ToArray(), genCodePath);

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


		void ToAssets(string sourcePath)
		{
			try
			{
				if (Directory.Exists(assetPath))
					Directory.Delete(assetPath, true);
				Directory.CreateDirectory(assetPath);

				string[] filePaths = Directory.GetFiles(sourcePath);
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