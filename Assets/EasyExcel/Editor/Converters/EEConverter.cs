using System;
using System.Collections.Generic;
using UnityEditor;

namespace EasyExcel
{
	/// <summary>
	///     Excel Converter
	/// </summary>
	public static partial class EEConverter
	{
		public const string excelPathKey = "EasyExcelExcelPath";
		public const string csChangedKey = "EasyExcelCSChanged";
		
		private static SheetData ToSheetData(EEWorksheet sheet)
		{
			var sheetData = new SheetData();
			for (var i = 1; i <= sheet.rowCount; i++)
			{
				var rowData = new List<string>();
				for (var j = 1; j <= sheet.columnCount; j++)
				{
					var cell = sheet.GetCell(i, j);
					rowData.Add(cell != null ? cell.value : "");
				}

				sheetData.Table.Add(rowData);
			}

			sheetData.rowCount = sheet.rowCount;
			sheetData.columnCount = sheet.columnCount;

			return sheetData;
		}
		
		private static bool IsValidSheet(EEWorksheet sheet)
		{
			if (sheet == null || sheet.rowCount <= EESettings.Current.TypeRowIndex + 1 || sheet.columnCount < 1)
				return false;
			var name = sheet.GetCellValue(EESettings.Current.NameRowIndex + 1, 1);
			if (name != "ID")
				return false;
			var tp = sheet.GetCellValue(EESettings.Current.TypeRowIndex + 1, 1);
			if (tp != "int")
				return false;
			return true;
		}
		
		private static bool IsExcelFile(string filePath)
		{
			return EEUtility.IsExcelFileSupported(filePath);
		}
		
		private static bool isDisplayingProgress;
		
		private static void UpdateProgressBar(int progress, int progressMax, string desc)
		{
			var title = "Importing...[" + progress + " / " + progressMax + "]";
			var value = progress / (float) progressMax;
			EditorUtility.DisplayProgressBar(title, desc, value);
			isDisplayingProgress = true;
		}

		private static void ClearProgressBar()
		{
			if (!isDisplayingProgress) return;
			try
			{
				EditorUtility.ClearProgressBar();
			}
			catch (Exception)
			{
				// ignored
			}

			isDisplayingProgress = false;
		}

		private class SheetData
		{
			private readonly List<List<string>> table = new List<List<string>>();
			public int columnCount;
			public int rowCount;

			public List<List<string>> Table
			{
				get { return table; }
			}

			public string Get(int row, int column)
			{
				return table[row][column];
			}

			public void Set(int row, int column, string value)
			{
				table[row][column] = value;
			}
		}
	}
}