
using UnityEngine;

namespace EasyExcel
{

	public static class Config
	{
		// This is where the generated csharp files will be.
		public const string CSharpPath = "Assets/EasyExcel/Example/AutoGenCode/";

		// This is where the generated ScriptableObject files will be.
		public const string AssetPath = "Assets/EasyExcel/Example/AutoGenAsset/";

		// Assetbudle name of generated ScriptableObject files.
		public const string AssetbudleName = "datatable";

		// Assetbundle path
		public static string AssetbundlePath = Application.streamingAssetsPath + "/EasyExcel/";

		// Postfix of generated RowData classes.
		// for example Item.xlsx corresponds to Item + RowDataClassNamePostfix.
		public const string RowDataClassNamePostfix = "RowData";

		// Postfix of generated RowData classes.
		// for example Item.xlsx corresponds to Item + DataTableClassNamePostfix.
		public const string DataTableClassNamePostfix = "DataTable";

		// Postfix of generated ScriptableObject files.
		// for example Item.xlsx corresponds to Item + AssetFileExtension.
		public const string AssetFileExtension = ".asset";

		#region Configs for editor tools

		// Row 0 in a excel sheet is Name
		public const int NAME_ROW_INDEX = 0;

		// Row 1 in a excel sheet is Type
		public const int TYPE_ROW_INDEX = 1;

		// Row 2 in a excel sheet is default value
		public const int DEFAULT_VALUE_ROW_INDEX = 2;

		// Row 3 in a excel sheet is where real data starts
		public const int DATA_START_INDEX = 3;

		/// <summary>
		/// Return the row data class name by excel file name.
		/// </summary>
		/// <param name="excelFileName">Excel file name without file extension</param>
		/// <returns></returns>
		public static string GetRowDataClassName(string excelFileName)
		{
			return excelFileName + RowDataClassNamePostfix;
		}

		/// <summary>
		/// Return the data table class name by excel file name.
		/// </summary>
		/// <param name="excelFileName">Excel file name without file extension</param>
		/// <returns></returns>
		public static string GetDataTableClassName(string excelFileName)
		{
			return excelFileName + DataTableClassNamePostfix;
		}

		/// <summary>
		/// Return the ScriptableObject asset file name by excel file name.
		/// </summary>
		/// <param name="excelFileName">Excel file name without file extension</param>
		/// <returns></returns>
		public static string GetAssetFileName(string excelFileName)
		{
			return excelFileName + AssetFileExtension;
		}

		#endregion

	}
	
}