using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;



public class XlsxConverter : EditorWindow
{

	[MenuItem(@"Tools/XlsxConverter")]
	public static void ShowExcelWindow()
	{
		Rect wr = new Rect(100, 100, 500, 400);
		var window = (XlsxConverter)EditorWindow.GetWindowWithRect(typeof(XlsxConverter), wr, true, "Xlsx Converter");
		window.Show();
	}

	//

	string genCodePath;
	string xlsxPath;
	string objectPath;
	string txtPath;

	void OnEnable()
	{
		genCodePath = Application.dataPath;
		xlsxPath = Application.dataPath;
		objectPath = Application.dataPath;
		txtPath = Application.dataPath;
	}


	void OnGUI()
	{
		float spaceSize = 10f;
		float leftSpace = 10;
		float titleLen = 70;
		float textLen = 250;
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
		objectPath = GUILayout.TextField(objectPath, GUILayout.Width(textLen));
		if (GUILayout.Button("Open", GUILayout.Width(buttonLen2)))
		{
			objectPath = EditorUtility.OpenFolderPanel("Select object path", String.Empty, "");
		}
		GUILayout.EndHorizontal();
		GUILayout.Space(spaceSize);

		GUILayout.BeginHorizontal();
		GUILayout.Space(leftSpace);
		GUILayout.Label("txt path", EditorStyles.label, GUILayout.Width(titleLen));
		txtPath = GUILayout.TextField(txtPath, GUILayout.Width(textLen));
		if (GUILayout.Button("Open", GUILayout.Width(buttonLen2)))
		{
			txtPath = EditorUtility.OpenFolderPanel("Select txt path", String.Empty, "");
		}
		GUILayout.EndHorizontal();
		GUILayout.Space(spaceSize);


		GUILayout.BeginHorizontal();
		GUILayout.Space(leftSpace);
		if (GUILayout.Button("To cs", GUILayout.Width(buttonLen1), GUILayout.Height(buttonHeight)))
		{
			to_css2(xlsxPath);
		}
		if (GUILayout.Button("To asset", GUILayout.Width(buttonLen1), GUILayout.Height(buttonHeight)))
		{
			to_asss(xlsxPath);
		}
		if (GUILayout.Button("To txt", GUILayout.Width(buttonLen1), GUILayout.Height(buttonHeight)))
		{
			to_txts(xlsxPath);
		}
		if (GUILayout.Button("To sb", GUILayout.Width(buttonLen1), GUILayout.Height(buttonHeight)))
		{
			UnityEngine.Debug.LogError("dasabi");
		}
		GUILayout.EndHorizontal();
		GUILayout.Space(spaceSize);

	}


	static void to_txt()
	{
		string filePath = EditorUtility.OpenFilePanel("Import xlsx", String.Empty, "xlsx");
		if (!string.IsNullOrEmpty(filePath))
		{
			Xlsx_Txt.xlsx_to_txt(filePath);
			AssetDatabase.Refresh();
			UnityEngine.Debug.Log("Import xlsx done.");
		}
	}


	static void to_txts(string sourcePath)
	{
		try
		{
			string[] filePaths = Directory.GetFiles(sourcePath);
			for (int i = 0; i < filePaths.Length; ++i)
			{
				string filePath = filePaths[i].Replace("\\", "/"); ;
				if (IsXlsxFile(filePath))
				{
					UpdateProgress(i, filePaths.Length, "");
					Xlsx_Txt.xlsx_to_txt(filePath);
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


	
	static void to_cs()
	{
		string filePath = EditorUtility.OpenFilePanel("Import xlsx", String.Empty, "xlsx");
		if (!string.IsNullOrEmpty(filePath))
		{
			Xlsx_Txt.xlsx_to_cs(filePath);
			AssetDatabase.Refresh();
			UnityEngine.Debug.Log("Import xlsx done.");
		}
	}

	
	static void to_css()
	{
		try
		{
			string folderPath = EditorUtility.OpenFolderPanel("Import xlsxs", String.Empty, "xlsx");
			if (string.IsNullOrEmpty(folderPath))
				return;

			string[] filePaths = Directory.GetFiles(folderPath);
			for (int i = 0; i < filePaths.Length; ++i)
			{
				string filePath = filePaths[i].Replace("\\", "/"); ;
				if (IsXlsxFile(filePath))
				{
					UpdateProgress(i, filePaths.Length, "");
					Xlsx_Txt.xlsx_to_cs(filePath);
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


	
	static void to_cs2()
	{
		string filePath = EditorUtility.OpenFilePanel("Import xlsx", String.Empty, "xlsx");
		if (!string.IsNullOrEmpty(filePath))
		{
			Xlsx_ScriptableObject.xlsx_to_cs(filePath);
			AssetDatabase.Refresh();
			UnityEngine.Debug.Log("Import xlsx done.");
		}
	}


	static void to_css2(string sourcePath)
	{
		try
		{
			string[] filePaths = Directory.GetFiles(sourcePath);
			for (int i = 0; i < filePaths.Length; ++i)
			{
				string filePath = filePaths[i].Replace("\\", "/"); ;
				if (IsXlsxFile(filePath))
				{
					UpdateProgress(i, filePaths.Length, "");
					Xlsx_ScriptableObject.xlsx_to_cs(filePath);
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


	
	static void to_ass()
	{
		string filePath = EditorUtility.OpenFilePanel("Import xlsx", String.Empty, "xlsx");
		if (!string.IsNullOrEmpty(filePath))
		{
			Xlsx_ScriptableObject.xlsx_to_asset(filePath);
			AssetDatabase.Refresh();
			UnityEngine.Debug.Log("Import xlsx done.");
		}
	}


	static void to_asss(string sourcePath)
	{
		try
		{
			string[] filePaths = Directory.GetFiles(sourcePath);
			for (int i = 0; i < filePaths.Length; ++i)
			{
				string filePath = filePaths[i].Replace("\\", "/"); ;
				if (IsXlsxFile(filePath))
				{
					UpdateProgress(i, filePaths.Length, "");
					Xlsx_ScriptableObject.xlsx_to_asset(filePath);
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