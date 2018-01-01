
using UnityEngine;

namespace EasyExcel
{

	public static class Config
	{
		/// <summary>
		/// This is where the generated csharp files will be.
		/// </summary>
		public const string CSharpPath = "Assets/EasyExcel/Example/AutoGenCode/";

		/// <summary>
		/// This is where the generated ScriptableObject files will be.
		/// </summary>
		public const string AssetPath = "Assets/Resources/EasyExcel/Example/AutoGenAsset/";

		/// <summary>
		/// Assetbudle name of generated ScriptableObject files.
		/// </summary>
		public const string AssetbudleName = "datatable";

		/// <summary>
		/// Assetbundle path
		/// </summary>
		public static string AssetbundlePath = Application.streamingAssetsPath + "/EasyExcel/";
	}
	
}