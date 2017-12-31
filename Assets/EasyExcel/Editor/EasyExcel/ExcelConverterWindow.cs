
using System;
using System.IO;
using UnityEngine;
using UnityEditor;



namespace EasyExcel
{

	public class ExcelConverterWindow
	{
		static string excelPathKey = "EasyExcelExcelPath";
		static string csChangedKey = "EasyExcelCSChanged";

		[MenuItem(@"Tools/EasyExcel/ImportFolder")]
		public static void ImportFolder()
		{
			string historyExcelPath = EditorPrefs.GetString(excelPathKey);
			if (string.IsNullOrEmpty(historyExcelPath) || !Directory.Exists(historyExcelPath))
				historyExcelPath = Environment.CurrentDirectory;
			string excelPath = EditorUtility.OpenFolderPanel("Select folder of .xls/xlsx", historyExcelPath, "");
			if (string.IsNullOrEmpty(excelPath))
				return;
			EditorPrefs.SetString(excelPathKey, excelPath);
			ToCSharps(excelPath, Environment.CurrentDirectory + "/" + Config.Instance.CSharpPath);
		}

		[MenuItem(@"Tools/EasyExcel/ImportFile")]
		public static void ImportFile()
		{
			
		}

		[MenuItem(@"Tools/EasyExcel/Clean")]
		public static void Clean()
		{
			EditorPrefs.SetBool(csChangedKey, false);

			if (Directory.Exists(Config.Instance.CSharpPath))
				Directory.Delete(Config.Instance.CSharpPath, true);
			Directory.CreateDirectory(Config.Instance.CSharpPath);

			if (Directory.Exists(Config.Instance.AssetPath))
				Directory.Delete(Config.Instance.AssetPath, true);
			Directory.CreateDirectory(Config.Instance.AssetPath);

			AssetDatabase.Refresh();
		}

		static void ToCSharps(string xlsxPath, string csPath)
		{
			try
			{
				if (!Directory.Exists(xlsxPath))
				{
					EditorUtility.DisplayDialog("EasyExcel", "Excel files path doesn't exist.", "OK");
					return;
				}
				if (!Directory.Exists(csPath))
				{
					EditorUtility.DisplayDialog("EasyExcel", string.Format("Folder {0} doesn't exist.", csPath), "OK");
					return;
				}
				xlsxPath = xlsxPath.Replace("\\", "/");
				csPath = csPath.Replace("\\", "/");
				if (!csPath.EndsWith("/"))
					csPath += "/";
				
				bool csChanged = false;
				string[] filePaths = Directory.GetFiles(xlsxPath);
				for (int i = 0; i < filePaths.Length; ++i)
				{
					string xlsxFilePath = filePaths[i].Replace("\\", "/"); ;
					if (IsXlsxFile(xlsxFilePath))
					{
						UpdateProgress(i, filePaths.Length, "");
						string newCs = ExcelConverter.ToCSharp(xlsxFilePath);
						int index = xlsxFilePath.LastIndexOf("/") + 1;
						string fileName = xlsxFilePath.Substring(index, xlsxFilePath.LastIndexOf(".") - index);
						string csFilePath = csPath + ExcelConverter.GetAssetClassName(fileName) + ".cs";
						bool shouldWrite = true;
						if (File.Exists(csFilePath))
						{
							string oldCs = File.ReadAllText(csFilePath);
							shouldWrite = oldCs != newCs;
						}
						if (shouldWrite)
						{
							csChanged = true;
							StreamWriter streamwriter = new StreamWriter(csFilePath, false);
							streamwriter.Write(newCs);
							streamwriter.Flush();
							streamwriter.Close();
						}
					}
				}

				if (csChanged)
				{
					EditorPrefs.SetBool(csChangedKey, true);
					EditorUtility.ClearProgressBar();
					AssetDatabase.Refresh();
				}
				else
				{
					EditorUtility.ClearProgressBar();
					string historyExcelPath = EditorPrefs.GetString(excelPathKey);
					if (!string.IsNullOrEmpty(historyExcelPath))
						ToAssets(historyExcelPath, Environment.CurrentDirectory + "/" + Config.Instance.AssetPath);
				}

			}
			catch (Exception e)
			{
				Debug.LogError(e.ToString());
				EditorUtility.ClearProgressBar();
				AssetDatabase.Refresh();
			}
		}

		[UnityEditor.Callbacks.DidReloadScripts]
		private static void OnScriptsReloaded()
		{
			if (EditorPrefs.GetBool(csChangedKey, false))
			{
				EditorPrefs.SetBool(csChangedKey, false);
				string historyExcelPath = EditorPrefs.GetString(excelPathKey);
				if (!string.IsNullOrEmpty(historyExcelPath))
					ToAssets(historyExcelPath, Environment.CurrentDirectory + "/" + Config.Instance.AssetPath);
			}
		}

		static void ToAssets(string xlsxPath, string assetPath)
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
				AssetDatabase.Refresh();
				EditorUtility.DisplayDialog("EasyExcel", "Import done.", "OK");
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
			string title = "Importing...[" + progress + " / " + progressMax + "]";
			float value = (float)progress / (float)progressMax;
			EditorUtility.DisplayProgressBar(title, desc, value);
		}

	}

}