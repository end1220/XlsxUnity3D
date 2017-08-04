
using System;
using System.IO;
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
			if (string.IsNullOrEmpty(xlsxPath))
				xlsxPath = Application.dataPath + Confiig.XlsxPath;
			genCodePath = EditorPrefs.GetString("genCodePath", null);
			if (string.IsNullOrEmpty(genCodePath))
				genCodePath = Application.dataPath + Confiig.OutputCsharpPath;
			assetPath = EditorPrefs.GetString("assetPath", null);
			if (string.IsNullOrEmpty(assetPath))
				assetPath = Application.dataPath + Confiig.OutputAssetPath;
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
			GUILayout.Label("xlsx path", EditorStyles.label, GUILayout.Width(titleLen));
			xlsxPath = GUILayout.TextField(xlsxPath, GUILayout.Width(textLen));
			if (GUILayout.Button("Open", GUILayout.Width(buttonLen2)))
				xlsxPath = EditorUtility.OpenFolderPanel("Select xlsx path", String.Empty, "");
			
			GUILayout.EndHorizontal();
			GUILayout.Space(spaceSize);

			GUILayout.BeginHorizontal();
			GUILayout.Space(leftSpace);
			GUILayout.Label("csharp path", EditorStyles.label, GUILayout.Width(titleLen));
			genCodePath = GUILayout.TextField(genCodePath, GUILayout.Width(textLen));
			if (GUILayout.Button("Open", GUILayout.Width(buttonLen2)))
				genCodePath = EditorUtility.OpenFolderPanel("Select code path", String.Empty, "");

			GUILayout.EndHorizontal();
			GUILayout.Space(spaceSize);

			GUILayout.BeginHorizontal();
			GUILayout.Space(leftSpace);
			GUILayout.Label("asset path", EditorStyles.label, GUILayout.Width(titleLen));
			assetPath = GUILayout.TextField(assetPath, GUILayout.Width(textLen));
			if (GUILayout.Button("Open", GUILayout.Width(buttonLen2)))
				assetPath = EditorUtility.OpenFolderPanel("Select asset path", String.Empty, "");
			
			GUILayout.EndHorizontal();
			GUILayout.Space(spaceSize);

			GUILayout.BeginHorizontal();
			GUILayout.Space(leftSpace);
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


		void ToCSharp()
		{
			string filePath = EditorUtility.OpenFilePanel("Import xlsx", String.Empty, "xlsx");
			if (!string.IsNullOrEmpty(filePath))
			{
				XlsxConverter.ToCSharp(filePath, genCodePath);
				AssetDatabase.Refresh();
				Debug.Log("Convert done.");
			}
		}


		void ToCSharps(string sourcePath)
		{
			try
			{
				if (Directory.Exists(genCodePath))
					Directory.Delete(genCodePath, true);
				Directory.CreateDirectory(genCodePath);

				string[] filePaths = Directory.GetFiles(sourcePath);
				for (int i = 0; i < filePaths.Length; ++i)
				{
					string filePath = filePaths[i].Replace("\\", "/"); ;
					if (IsXlsxFile(filePath))
					{
						UpdateProgress(i, filePaths.Length, "");
						XlsxConverter.ToCSharp(filePath, genCodePath);
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


		void ToAsset()
		{
			string filePath = EditorUtility.OpenFilePanel("Import xlsx", String.Empty, "xlsx");
			if (!string.IsNullOrEmpty(filePath))
			{
				XlsxConverter.ToAsset(filePath, assetPath);
				AssetDatabase.Refresh();
				Debug.Log("Import xlsx done.");
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