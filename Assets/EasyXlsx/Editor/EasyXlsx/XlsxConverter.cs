using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;



public class XlsxConverter : EditorWindow
{

	[MenuItem(@"Tools/EasyXlsx")]
	public static void ShowExcelWindow()
	{
		Rect wr = new Rect(100, 100, 600, 400);
		var window = (XlsxConverter)EditorWindow.GetWindowWithRect(typeof(XlsxConverter), wr, true, "EasyXlsx");
		window.Show();
	}



	string genCodePath;
	string xlsxPath;
	string assetPath;

	void OnEnable()
	{
		xlsxPath = Application.dataPath + XlsxConst.XlsxPath;
		genCodePath = Application.dataPath + XlsxConst.OutputCsharpPath;
		assetPath = Application.dataPath + XlsxConst.OutputAssetPath;
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

		GUILayout.Label("  Convert xlsx to csharp, asset, txt...\n  I'm fine fuck you.", EditorStyles.helpBox);
		GUILayout.Space(spaceSize);

		GUILayout.BeginHorizontal();
		GUILayout.Space(leftSpace);
		GUILayout.Label("xlsx path", EditorStyles.label, GUILayout.Width(titleLen));
		xlsxPath = GUILayout.TextField(xlsxPath, GUILayout.Width(textLen));
		if (GUILayout.Button("Open", GUILayout.Width(buttonLen2)))
		{
			xlsxPath = EditorUtility.OpenFolderPanel("Select xlsx path", String.Empty, "");
		}
		GUILayout.EndHorizontal();
		GUILayout.Space(spaceSize);

		GUILayout.BeginHorizontal();
		GUILayout.Space(leftSpace);
		GUILayout.Label("csharp path", EditorStyles.label, GUILayout.Width(titleLen));
		genCodePath = GUILayout.TextField(genCodePath, GUILayout.Width(textLen));
		if (GUILayout.Button("Open", GUILayout.Width(buttonLen2)))
		{
			genCodePath = EditorUtility.OpenFolderPanel("Select code path", String.Empty, "");
		}
		GUILayout.EndHorizontal();
		GUILayout.Space(spaceSize);

		GUILayout.BeginHorizontal();
		GUILayout.Space(leftSpace);
		GUILayout.Label("object path", EditorStyles.label, GUILayout.Width(titleLen));
		assetPath = GUILayout.TextField(assetPath, GUILayout.Width(textLen));
		if (GUILayout.Button("Open", GUILayout.Width(buttonLen2)))
		{
			assetPath = EditorUtility.OpenFolderPanel("Select object path", String.Empty, "");
		}
		GUILayout.EndHorizontal();
		GUILayout.Space(spaceSize);

		GUILayout.BeginHorizontal();
		GUILayout.Space(leftSpace);
		if (GUILayout.Button("To csharp", GUILayout.Width(buttonLen1), GUILayout.Height(buttonHeight)))
		{
			to_csharps(xlsxPath);
		}
		if (GUILayout.Button("To asset", GUILayout.Width(buttonLen1), GUILayout.Height(buttonHeight)))
		{
			to_asss(xlsxPath);
		}
		if (GUILayout.Button("To sb", GUILayout.Width(buttonLen1), GUILayout.Height(buttonHeight)))
		{
			UnityEngine.Debug.LogError("dasabi");
		}
		GUILayout.EndHorizontal();
		GUILayout.Space(spaceSize);

	}

	
	void to_csharp()
	{
		string filePath = EditorUtility.OpenFilePanel("Import xlsx", String.Empty, "xlsx");
		if (!string.IsNullOrEmpty(filePath))
		{
			Xlsx_ScriptableObject.xlsx_to_cs(filePath, genCodePath);
			AssetDatabase.Refresh();
			UnityEngine.Debug.Log("Import xlsx done.");
		}
	}


	void to_csharps(string sourcePath)
	{
		try
		{
			if (!Directory.Exists(genCodePath))
				Directory.CreateDirectory(genCodePath);

			string[] filePaths = Directory.GetFiles(sourcePath);
			for (int i = 0; i < filePaths.Length; ++i)
			{
				string filePath = filePaths[i].Replace("\\", "/"); ;
				if (IsXlsxFile(filePath))
				{
					UpdateProgress(i, filePaths.Length, "");
					Xlsx_ScriptableObject.xlsx_to_cs(filePath, genCodePath);
				}
			}

			EditorUtility.ClearProgressBar();
			AssetDatabase.Refresh();
			UnityEngine.Debug.Log("Import xlsxs done.");
		}
		catch (System.Exception e)
		{
			UnityEngine.Debug.LogError(e.ToString());
			EditorUtility.ClearProgressBar();
			AssetDatabase.Refresh();
		}
	}


	
	void to_ass()
	{
		string filePath = EditorUtility.OpenFilePanel("Import xlsx", String.Empty, "xlsx");
		if (!string.IsNullOrEmpty(filePath))
		{
			Xlsx_ScriptableObject.xlsx_to_asset(filePath, assetPath);
			AssetDatabase.Refresh();
			UnityEngine.Debug.Log("Import xlsx done.");
		}
	}


	void to_asss(string sourcePath)
	{
		try
		{
			if (!Directory.Exists(assetPath))
				Directory.CreateDirectory(assetPath);

			string[] filePaths = Directory.GetFiles(sourcePath);
			for (int i = 0; i < filePaths.Length; ++i)
			{
				string filePath = filePaths[i].Replace("\\", "/"); ;
				if (IsXlsxFile(filePath))
				{
					UpdateProgress(i, filePaths.Length, "");
					Xlsx_ScriptableObject.xlsx_to_asset(filePath, assetPath);
				}
			}

			EditorUtility.ClearProgressBar();
			AssetDatabase.Refresh();
			UnityEngine.Debug.Log("Import xlsxs done.");
		}
		catch (System.Exception e)
		{
			UnityEngine.Debug.LogError(e.ToString());
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