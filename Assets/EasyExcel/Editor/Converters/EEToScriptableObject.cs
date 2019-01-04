using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace EasyExcel
{
	/// <summary>
	///     Excel Converter
	/// </summary>
	public static partial class EEConverter
	{
		public static void GenerateScriptableObjects(string xlsxPath, string assetPath)
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
					var opt = EditorUtility.DisplayDialogComplex("EasyExcel",
						string.Format(
							"EasyExcelSettings AssetPath doesn't exist. Would you like to create it?\n{0}",
							assetPath),
						"Create", "Cancel", "Quit");
					switch (opt)
					{
						case 0:
							Directory.CreateDirectory(assetPath);
							break;
						case 1:
						case 2:
							return;
					}
				}

				xlsxPath = xlsxPath.Replace("\\", "/");
				assetPath = assetPath.Replace("\\", "/");
				if (!assetPath.EndsWith("/"))
					assetPath += "/";
				if (Directory.Exists(assetPath))
					Directory.Delete(assetPath, true);
				Directory.CreateDirectory(assetPath);

				var filePaths = Directory.GetFiles(xlsxPath);
				var count = 0;
				for (var i = 0; i < filePaths.Length; ++i)
				{
					var filePath = filePaths[i].Replace("\\", "/");
					if (!IsExcelFile(filePath)) continue;
					UpdateProgressBar(i, filePaths.Length, "");
					ToScriptableObject(filePath, assetPath);
					count++;
				}

				EELog.Log("Assets are generated successfully.");

				ClearProgressBar();
				AssetDatabase.Refresh();
				EELog.Log(string.Format("Import done. {0} tables imported.", count));
			}
			catch (Exception e)
			{
				EELog.LogError(e.ToString());
				ClearProgressBar();
				AssetDatabase.Refresh();
			}
		}
		
		private static void ToScriptableObject(string excelPath, string outputPath)
		{
			try
			{
				var book = EEWorkbook.Load(excelPath);
				if (book == null)
					return;
				foreach (var sheet in book.sheets)
				{
					if (sheet == null)
						continue;
					if (!IsValidSheet(sheet))
						continue;
					var sheetData = ToSheetData(sheet);
					ToScriptableObject(sheet.name, outputPath, sheetData);
				}
			}
			catch (Exception e)
			{
				EELog.LogError(e.ToString());
				AssetDatabase.Refresh();
			}
		}

		private static void ToScriptableObject(string sheetName, string outputPath, SheetData sheetData)
		{
			try
			{
				var dataTableClassName = EESettings.Current.GetDataTableClassName(sheetName);
				var asset = ScriptableObject.CreateInstance(dataTableClassName);
				var dataCollect = asset as RowDataCollection;
				if (dataCollect == null)
					return;
				var className = EESettings.Current.GetRowDataClassName(sheetName, true);
				var dataType = Type.GetType(className);
				if (dataType == null)
				{
					var asmb = Assembly.LoadFrom(Environment.CurrentDirectory +
					                             "/Library/ScriptAssemblies/Assembly-CSharp.dll");
					dataType = asmb.GetType(className);
				}

				if (dataType == null)
				{
					EELog.LogError(className + " not exist !");
					return;
				}

				var dataCtor = dataType.GetConstructor(Type.EmptyTypes);
				if (dataCtor == null)
					return;
				var ids = new HashSet<int>();
				for (var row = EESettings.Current.DataStartIndex; row < sheetData.rowCount; ++row)
				{
					for (var col = 0; col < sheetData.columnCount; ++col)
						sheetData.Set(row, col, sheetData.Get(row, col).Replace("\n", "\\n"));

					var inst = dataCtor.Invoke(null) as RowData;
					if (inst == null)
						continue;
					inst._init(sheetData.Table, row, 0);
					if (!ids.Contains(inst.ID))
					{
						dataCollect.AddData(inst);
						ids.Add(inst.ID);
					}
					else
						EELog.LogWarning("More than one rows use ID " + inst.ID + " in " + sheetName);
				}

				var itemPath = outputPath + EESettings.GetAssetFileName(sheetName);
				itemPath = itemPath.Substring(itemPath.IndexOf("Assets", StringComparison.Ordinal));
				AssetDatabase.CreateAsset(asset, itemPath);

				AssetDatabase.Refresh();
			}
			catch (Exception ex)
			{
				EELog.LogError(ex.ToString());
			}
		}

	}
}