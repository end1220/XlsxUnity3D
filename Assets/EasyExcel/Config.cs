
using System.IO;
using UnityEngine;

namespace EasyExcel
{

	public class Config : ScriptableObject
	{
		public string ExcelPath = "Assets/EasyExcel/Example/ExcelFiles/";
		public string CSharpPath = "Assets/EasyExcel/Example/AutoGenCode/";
		public string AssetPath = "Assets/EasyExcel/Example/AutoGenAsset/";

		const string savePath = "Assets/Resources/EasyExcel.asset";

		private static Config inst;
		public static Config Instance
		{
			get
			{
				if (inst == null)
				{
#if UNITY_EDITOR
					if (!Directory.Exists("Assets/Resources"))
						Directory.CreateDirectory("Assets/Resources");

					inst = UnityEditor.AssetDatabase.LoadAssetAtPath(savePath, typeof(Config)) as Config;
					if (inst == null)
					{
						inst = ScriptableObject.CreateInstance<Config>();
						UnityEditor.AssetDatabase.CreateAsset(inst, savePath);
					}
#else
					inst = Resources.Load("EasyExcel") as Config;
#endif

				}
				return inst;
			}
		}

		public static void Save()
		{
#if UNITY_EDITOR
			if (inst != null)
			{
				var existingAsset = UnityEditor.AssetDatabase.LoadAssetAtPath(savePath, typeof(Config));
				if (existingAsset == null)
				{
					UnityEditor.AssetDatabase.CreateAsset(inst, savePath);
					UnityEditor.AssetDatabase.Refresh();
					existingAsset = inst;
				}
				else
				{
					UnityEditor.EditorUtility.CopySerialized(inst, existingAsset);
				}

				UnityEditor.EditorUtility.SetDirty(existingAsset);
				UnityEditor.AssetDatabase.Refresh();
			}
#endif
		}
	}
	
}