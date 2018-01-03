
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;



namespace EasyExcel
{

	public class ExcelConverterMenu
	{
		static string excelPathKey = "EasyExcelExcelPath";
		static string csChangedKey = "EasyExcelCSChanged";

		[MenuItem(@"Tools/EasyExcel/Import")]
		public static void ImportFolder()
		{
			string historyExcelPath = EditorPrefs.GetString(excelPathKey);
			if (string.IsNullOrEmpty(historyExcelPath) || !Directory.Exists(historyExcelPath))
			{
				string fallbackDir = Environment.CurrentDirectory + "/Assets/EasyExcel/Example/ExcelFiles";
				if (Directory.Exists(fallbackDir))
					historyExcelPath = fallbackDir;
				else
					historyExcelPath = Environment.CurrentDirectory;
			}
			string excelPath = EditorUtility.OpenFolderPanel("Select folder of .xlsx", historyExcelPath, "");
			if (string.IsNullOrEmpty(excelPath))
				return;

			DeleteAssetbundleFolder();

			EditorPrefs.SetString(excelPathKey, excelPath);
			ToCSharps(excelPath, Environment.CurrentDirectory + "/" + Config.CSharpPath);
		}

		[MenuItem(@"Tools/EasyExcel/Clean")]
		public static void Clean()
		{
			EditorPrefs.SetBool(csChangedKey, false);

			DeleteCSFolder();
			DeleteAssetFolder();
			DeleteAssetbundleFolder();

			AssetDatabase.Refresh();
		}

		[MenuItem(@"Tools/EasyExcel/Build Assetbundle")]
		public static void BuildAssetbundle()
		{
			if (!Directory.Exists(Config.AssetbundlePath))
			{
				int opt = EditorUtility.DisplayDialogComplex("EasyExcel", string.Format("Config.AssetbundlePath doesn't exist, would you like to create it?\n{0}. ", Config.AssetbundlePath), "Create", "Cancel", "Quit");
				if (opt == 0)
					Directory.CreateDirectory(Config.AssetbundlePath);
				else if (opt == 1 || opt == 2)
					return;
			}
			
			List<string> assetNames = new List<string>();
			string[] files = Directory.GetFiles(Config.AssetPath);
			for (int i = 0; i < files.Length; ++i)
				if (files[i].EndsWith(Config.AssetFileExtension))
					assetNames.Add(files[i].Substring(files[i].IndexOf("Assets")).Replace("\\", "/"));

			AssetBundleBuild build = new AssetBundleBuild();
			build.assetBundleName = Config.AssetbudleName;
			build.assetNames = assetNames.ToArray();
			BuildPipeline.BuildAssetBundles(Config.AssetbundlePath, new AssetBundleBuild[] { build }, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
			EditorUtility.DisplayDialog("EasyExcel", "Build Assetbundle done.", "OK");
			AssetDatabase.Refresh();
		}

		static void ToCSharps(string xlsxPath, string csPath)
		{
			try
			{
				xlsxPath = xlsxPath.Replace("\\", "/");
				csPath = csPath.Replace("\\", "/");

				if (!Directory.Exists(xlsxPath))
				{
					EditorUtility.DisplayDialog("EasyExcel", "Excel files path doesn't exist.", "OK");
					return;
				}
				if (!Directory.Exists(csPath))
				{
					int opt = EditorUtility.DisplayDialogComplex("EasyExcel", string.Format("Config.CSharpPath doesn't exist. Would you like to create it?\n{0}", csPath), "Create", "Cancel", "Quit");
					if (opt == 0)
						Directory.CreateDirectory(csPath);
					else if (opt == 1 || opt == 2)
						return;
				}
				string tmpPath = Environment.CurrentDirectory + "/EasyExcelTmp/";
				if (Directory.Exists(tmpPath))
					Directory.Delete(tmpPath, true);
				Directory.CreateDirectory(tmpPath);
				
				xlsxPath = xlsxPath.Replace("\\", "/");
				csPath = csPath.Replace("\\", "/");
				if (!csPath.EndsWith("/"))
					csPath += "/";
				
				bool csChanged = false;
				string[] filePaths = Directory.GetFiles(xlsxPath);
				for (int i = 0; i < filePaths.Length; ++i)
				{
					string xlsxFilePath = filePaths[i].Replace("\\", "/");
					if (i + 1 < filePaths.Length)
						UpdateProgressBar(i + 1, filePaths.Length, "");
					else
						ClearProgressBar();
					if (IsXlsxFile(xlsxFilePath))
					{
						string newCs = ExcelConverter.ToCSharp(xlsxFilePath);
						int index = xlsxFilePath.LastIndexOf("/") + 1;
						string fileName = xlsxFilePath.Substring(index, xlsxFilePath.LastIndexOf(".") - index);
						string tmpCsFilePath = tmpPath + Config.GetDataTableClassName(fileName) + ".cs";
						string csFilePath = csPath + Config.GetDataTableClassName(fileName) + ".cs";
						bool shouldWrite = true;
						if (File.Exists(csFilePath))
						{
							string oldCs = File.ReadAllText(csFilePath);
							shouldWrite = oldCs != newCs;
						}
						if (shouldWrite)
						{
							csChanged = true;
							StreamWriter streamwriter = new StreamWriter(tmpCsFilePath, false);
							streamwriter.Write(newCs);
							streamwriter.Flush();
							streamwriter.Close();
						}
					}
				}

				if (csChanged)
				{
					EditorPrefs.SetBool(csChangedKey, true);
					string[] files = Directory.GetFiles(tmpPath);
					foreach (string s in files)
					{
						string p = s.Replace("\\", "/");
						File.Copy(s, csPath + p.Substring(p.LastIndexOf("/")), true);
					}
					Directory.Delete(tmpPath, true);
					AssetDatabase.Refresh();
					Debug.Log("EasyExcel: CSharp files are generated, wait for genrerating assets...");
				}
				else
				{
					Debug.Log("EasyExcel: No CSharp files changed, begin genrerating assets...");
					ClearProgressBar();
					string historyExcelPath = EditorPrefs.GetString(excelPathKey);
					if (!string.IsNullOrEmpty(historyExcelPath))
						ToAssets(historyExcelPath, Environment.CurrentDirectory + "/" + Config.AssetPath);
				}

			}
			catch (Exception e)
			{
				Debug.LogError(e.ToString());
				EditorPrefs.SetBool(csChangedKey, false);
				ClearProgressBar();
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
				{
					Debug.Log("EasyExcel: Scripts are reloaded, begin genrerating assets...");
					ToAssets(historyExcelPath, Environment.CurrentDirectory + "/" + Config.AssetPath);
				}
			}
		}

		static void ToAssets(string xlsxPath, string assetPath)
		{
			try
			{
				xlsxPath = xlsxPath.Replace("\\", "/");
				assetPath = assetPath.Replace("\\", "/");

				if (!Directory.Exists(xlsxPath))
				{
					EditorUtility.DisplayDialog("EasyExcel", "Xls/xlsx path doesn't exist.", "OK");
					return;
				}
				if (!Directory.Exists(assetPath))
				{
					int opt = EditorUtility.DisplayDialogComplex("EasyExcel", string.Format("Config.AssetPath doesn't exist. Would you like to create it?\n{0}", assetPath), "Create", "Cancel", "Quit");
					if (opt == 0)
						Directory.CreateDirectory(assetPath);
					else if (opt == 1 || opt == 2)
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
				int count = 0;
				for (int i = 0; i < filePaths.Length; ++i)
				{
					string filePath = filePaths[i].Replace("\\", "/"); ;
					if (IsXlsxFile(filePath))
					{
						UpdateProgressBar(i, filePaths.Length, "");
						ExcelConverter.ToAsset(filePath, assetPath);
						count++;
						// assign asset bundle name.
						int index = filePath.LastIndexOf("/") + 1;
						string fileName = filePath.Substring(index, filePath.LastIndexOf(".") - index);
						string itemPath = assetPath + Config.GetAssetFileName(fileName);
						itemPath = itemPath.Substring(itemPath.IndexOf("Assets"));
						AssetImporter assetImporter = AssetImporter.GetAtPath(itemPath);
						assetImporter.assetBundleName = Config.AssetbudleName;
					}
				}

				Debug.Log("EasyExcel: Assets are genrerated successfully.");

				ClearProgressBar();
				AssetDatabase.Refresh();
				EditorUtility.DisplayDialog("EasyExcel", string.Format("Import done. {0} tables are imported.", count), "OK");
			}
			catch (Exception e)
			{
				Debug.LogError(e.ToString());
				ClearProgressBar();
				AssetDatabase.Refresh();
			}
		}

		static void DeleteCSFolder()
		{
			if (Directory.Exists(Config.CSharpPath))
				Directory.Delete(Config.CSharpPath, true);

			string csMeta = null;
			if (Config.CSharpPath.EndsWith("/") || Config.CSharpPath.EndsWith("\\"))
				csMeta = Config.CSharpPath.Substring(0, Config.CSharpPath.Length - 1) + ".meta";
			if (File.Exists(csMeta))
				File.Delete(csMeta);
		}

		static void DeleteAssetFolder()
		{
			if (Directory.Exists(Config.AssetPath))
				Directory.Delete(Config.AssetPath, true);

			string asMeta = null;
			if (Config.AssetPath.EndsWith("/") || Config.AssetPath.EndsWith("\\"))
				asMeta = Config.AssetPath.Substring(0, Config.AssetPath.Length - 1) + ".meta";
			if (File.Exists(asMeta))
				File.Delete(asMeta);
		}

		static void DeleteAssetbundleFolder()
		{
			if (Directory.Exists(Config.AssetbundlePath))
				Directory.Delete(Config.AssetbundlePath, true);

			string abMeta = null;
			if (Config.AssetbundlePath.EndsWith("/") || Config.AssetbundlePath.EndsWith("\\"))
				abMeta = Config.AssetbundlePath.Substring(0, Config.AssetbundlePath.Length - 1) + ".meta";
			if (File.Exists(abMeta))
				File.Delete(abMeta);
		}

		static bool IsXlsxFile(string filePath)
		{
			return filePath.EndsWith(".xlsx");
		}

		static bool isDisplayingProgress = false;
		static void UpdateProgressBar(int progress, int progressMax, string desc)
		{
			string title = "Importing...[" + progress + " / " + progressMax + "]";
			float value = (float)progress / (float)progressMax;
			EditorUtility.DisplayProgressBar(title, desc, value);
			isDisplayingProgress = true;
		}

		static void ClearProgressBar()
		{
			if (isDisplayingProgress)
			{
				try
				{ EditorUtility.ClearProgressBar(); }
				catch (Exception){}
				isDisplayingProgress = false;
			}
		}
	}

}