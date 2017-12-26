
using System.IO;
using UnityEngine;

namespace EasyExcel
{

	public class Config : ScriptableObject
	{
		public string ExcelPath = "";

		public string CSharpPath = "";

		public string AssetPath = "";


		private static Config inst;
		public static Config Instance
		{
			get
			{
				if (inst == null)
				{
#if UNITY_EDITOR
					string path = "Resources/EasyExcel.asset";
					if (!Directory.Exists("Assets/Resources"))
						Directory.CreateDirectory("Assets/Resources");
					if (!File.Exists(path) || (Resources.Load("EasyExcel") as Config) == null)
					{
						var asset = ScriptableObject.CreateInstance(typeof(Config));
						UnityEditor.AssetDatabase.CreateAsset(asset, "Assets/" + path);
						UnityEditor.AssetDatabase.Refresh();
					}
#endif
					inst = Resources.Load("EasyExcel") as Config;
				}
				return inst;
			}
		}
	}
	
}