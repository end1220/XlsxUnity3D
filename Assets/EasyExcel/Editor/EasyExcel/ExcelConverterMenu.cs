
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
				historyExcelPath = Environment.CurrentDirectory;
			string excelPath = EditorUtility.OpenFolderPanel("Select folder of .xls/xlsx", historyExcelPath, "");
			if (string.IsNullOrEmpty(excelPath))
				return;

			EditorPrefs.SetString(excelPathKey, excelPath);
			ToCSharps(excelPath, Environment.CurrentDirectory + "/" + Config.CSharpPath);
		}

		[MenuItem(@"Tools/EasyExcel/Clean")]
		public static void Clean()
		{
			EditorPrefs.SetBool(csChangedKey, false);

			if (Directory.Exists(Config.CSharpPath))
				Directory.Delete(Config.CSharpPath, true);

			string csMeta = null;
			if (Config.CSharpPath.EndsWith("/") || Config.CSharpPath.EndsWith("\\"))
				csMeta = Config.CSharpPath.Substring(0, Config.CSharpPath.Length - 1) + ".meta";
			if (File.Exists(csMeta))
				File.Delete(csMeta);

			if (Directory.Exists(Config.AssetPath))
				Directory.Delete(Config.AssetPath, true);

			string asMeta = null;
			if (Config.AssetPath.EndsWith("/") || Config.AssetPath.EndsWith("\\"))
				asMeta = Config.AssetPath.Substring(0, Config.AssetPath.Length - 1) + ".meta";
			if (File.Exists(asMeta))
				File.Delete(asMeta);

			AssetDatabase.Refresh();
		}

		[MenuItem(@"Tools/EasyExcel/Build Assetbundle")]
		public static void BuildAssetbundle()
		{
			if (!Directory.Exists(Config.AssetbundlePath))
			{
				int opt = EditorUtility.DisplayDialogComplex("EasyExcel", string.Format("{0} doesn't exist. Would you like to create it?", Config.AssetbundlePath), "Create", "Cancel", "Quit");
				if (opt == 0)
					Directory.CreateDirectory(Config.AssetbundlePath);
				else if (opt == 1 || opt == 2)
					return;
			}
			
			List<string> assetNames = new List<string>();
			string[] files = Directory.GetFiles(Config.AssetPath);
			for (int i = 0; i < files.Length; ++i)
				if (files[i].EndsWith(".asset"))
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
					int opt = EditorUtility.DisplayDialogComplex("EasyExcel", string.Format("{0} doesn't exist. Would you like to create it?", csPath), "Create", "Cancel", "Quit");
					if (opt == 0)
						Directory.CreateDirectory(csPath);
					else if (opt == 1 || opt == 2)
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
						ToAssets(historyExcelPath, Environment.CurrentDirectory + "/" + Config.AssetPath);
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
					ToAssets(historyExcelPath, Environment.CurrentDirectory + "/" + Config.AssetPath);
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
					int opt = EditorUtility.DisplayDialogComplex("EasyExcel", string.Format("{0} doesn't exist. Would you like to create it?", assetPath), "Create", "Cancel", "Quit");
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
						UpdateProgress(i, filePaths.Length, "");
						ExcelConverter.ToAsset(filePath, assetPath);
						count++;
						// assign asset bundle name.
						int index = filePath.LastIndexOf("/") + 1;
						string fileName = filePath.Substring(index, filePath.LastIndexOf(".") - index);
						string itemPath = assetPath + ExcelConverter.GetAssetName(fileName);
						itemPath = itemPath.Substring(itemPath.IndexOf("Assets"));
						AssetImporter assetImporter = AssetImporter.GetAtPath(itemPath);
						assetImporter.assetBundleName = Config.AssetbudleName;
					}
				}

				EditorUtility.ClearProgressBar();
				AssetDatabase.Refresh();
				EditorUtility.DisplayDialog("EasyExcel", string.Format("Import done. {0} tables are imported.", count), "OK");
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