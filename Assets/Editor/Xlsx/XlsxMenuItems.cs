
using System;
using System.IO;
using UnityEditor;
using Excel;
using Lite;



public class XlsxMenuItems
{

	[MenuItem("Tools/xlsx/to txt")]
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

	[MenuItem("Tools/xlsx/to txts")]
	static void to_txts()
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


	[MenuItem("Tools/xlsx/to csharp")]
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

	[MenuItem("Tools/xlsx/to csharps")]
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


	[MenuItem("Tools/xlsx/to csharp2")]
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

	[MenuItem("Tools/xlsx/to csharps2")]
	static void to_css2()
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


	[MenuItem("Tools/xlsx/to asset")]
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

	[MenuItem("Tools/xlsx/to assets")]
	static void to_asss()
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