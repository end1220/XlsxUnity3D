
using UnityEngine;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System;


namespace EasyExcel
{

	using DataDic = Dictionary<int, SingleData>;

	/// <summary>
	/// A piece of data(one row in excel)
	/// </summary>
	[Serializable]
	public abstract class SingleData
	{
		public int id;

		public virtual int _init(List<List<string>> sheet, int row, int column)
		{
			column++;
			id = 0;
			int.TryParse(sheet[row][column++], out id);
			return column;
		}
	}

	/// <summary>
	/// Table of SingleData
	/// </summary>
	public abstract class DataTable : ScriptableObject
	{
		public abstract void AddData(SingleData data);

		public abstract int GetDataCount();

		public abstract SingleData GetData(int index);

	}


	public class DataTableManager
	{
		private Dictionary<Type, DataDic> dataPool = new Dictionary<Type, DataDic>();


		/// <summary>
		/// Load all the imported tables.
		/// </summary>
		/// <param name="fromAssetbundle">Set true if load from assetbundle file; Set false if load from Resources folder.</param>
		public void Load(bool fromAssetbundle)
		{
			AssetBundle bundle = null;

			if (fromAssetbundle)
			{
				string assetbundlePath = Config.AssetbundlePath + Config.AssetbudleName;
#if UNITY_EDITOR
				if (!System.IO.File.Exists(assetbundlePath))
				{
					UnityEditor.EditorUtility.DisplayDialog("EasyExcel", string.Format("When loading from assetbundle, assetbundle {0} MUST be built first. Make sure to build it before running by menu Tools->EasyExcel->Build Assetbundle.", assetbundlePath), "OK");
					return;
				}
#endif
				bundle = AssetBundle.LoadFromFile(assetbundlePath);
			}
			else
			{
#if UNITY_EDITOR
				if (!Config.AssetPath.Contains("/Resources/"))
				{
					UnityEditor.EditorUtility.DisplayDialog("EasyExcel", string.Format("When not loading from assetbundle, Config.AssetPath {0} MUST be in Resources folder.", Config.AssetPath), "OK");
					return;
				}
#endif
			}

			Type baseType = typeof(DataTable);
			Assembly assembly = baseType.Assembly;
			foreach (Type type in assembly.GetTypes().Where(t => t.IsSubclassOf(baseType)))
			{
				string collectionClassName = type.Name;
				string headName = collectionClassName.Substring(0, collectionClassName.IndexOf("Asset"));

				string filePath = null;
				DataTable table = null;
				if (fromAssetbundle)
				{
					filePath = Config.AssetPath + headName + ".asset";
					table = bundle.LoadAsset(filePath, typeof(DataTable)) as DataTable;
				}
				else
				{
					filePath = Config.AssetPath.Substring(Config.AssetPath.IndexOf("Resources/") + ("Resources/").Length) + headName;
					table = Resources.Load(filePath) as DataTable;
				}
				if (table == null)
				{
					Debug.LogError("DataManager: Load asset error with " + filePath);
					continue;
				}
				DataDic dataDic = new DataDic();
				for (int i = 0; i < table.GetDataCount(); ++i)
				{
					SingleData data = table.GetData(i);
					dataDic.Add(data.id, data);
				}

				Type dataClassType = Type.GetType(headName + "Data");
				dataPool.Add(dataClassType, dataDic);
			}

			Debug.Log(string.Format("EasyExcel: {0} tables loaded.", dataPool.Count));
		}

		public T Get<T>(int id) where T : SingleData
		{
			DataDic soDic = null;
			dataPool.TryGetValue(typeof(T), out soDic);
			if (soDic != null)
			{
				SingleData data = null;
				soDic.TryGetValue(id, out data);
				return data as T;
			}
			return null;
		}

		public DataDic GetList<T>() where T : SingleData
		{
			DataDic soDic = null;
			dataPool.TryGetValue(typeof(T), out soDic);
			return soDic;
		}
	}
}
