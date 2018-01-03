
using UnityEngine;

namespace EasyExcel
{

	public static class Config
	{
		// This is where the generated csharp files will be.
		public const string CSharpPath = "Assets/EasyExcel/Example/AutoGenCode/";

		// This is where the generated ScriptableObject files will be.
		public const string AssetPath = "Assets/EasyExcel/Example/AutoGenAsset/";

		// If true, before running, the imported ScriptableObjects should be built into assetbundle by clicking "Tools/EasyExcel/Build Assetbundle".
		// The AssetbudleName below will be applied as the name of the assetbundle.
		// The AssetbundlePath below will be the output folder.
		// If false, you should make sure AssetPath is somewhere in Resources folder, for example "Assets/Resources/YourFolder/",
		// and the two options AssetbudleName and AssetbundlePath will not be used.
		public const bool LoadFromAssetbundle = true;

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

		// Extension of generated ScriptableObject files.
		// for example Item.xlsx corresponds to Item + AssetFileExtension.
		public const string AssetFileExtension = ".asset";

		#region Configs for editor tools

		// This row in a excel sheet is Name
		public const int NAME_ROW_INDEX = 0;

		// This row in a excel sheet is Type
		public const int TYPE_ROW_INDEX = 1;

		// This row in a excel sheet is where real data starts
		public const int DATA_START_INDEX = 2;


		public static string GetRowDataClassName(string excelFileName)
		{
			return excelFileName + RowDataClassNamePostfix;
		}

		public static string GetDataTableClassName(string excelFileName)
		{
			return excelFileName + DataTableClassNamePostfix;
		}

		public static string GetAssetFileName(string excelFileName)
		{
			return excelFileName + AssetFileExtension;
		}

		#endregion

	}
	
}