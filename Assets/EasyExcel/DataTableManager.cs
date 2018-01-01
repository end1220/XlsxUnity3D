
using UnityEngine;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System;


namespace EasyExcel
{

	using DataDic = Dictionary<int, RowData>;

	/// <summary>
	/// A piece of data(one row in excel)
	/// </summary>
	[Serializable]
	public abstract class RowData
	{
		public int ID;

		public virtual int _init(List<List<string>> sheet, int row, int column)
		{
			ID = 0;
			int.TryParse(sheet[row][column++], out ID);
			return column;
		}
	}

	/// <summary>
	/// Table of RowData
	/// </summary>
	public abstract class DataTable : ScriptableObject
	{
		public abstract void AddData(RowData data);

		public abstract int GetDataCount();

		public abstract RowData GetData(int index);

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
					RowData data = table.GetData(i);
					dataDic.Add(data.ID, data);
				}

				Type dataClassType = Type.GetType(headName + "Data");
				dataPool.Add(dataClassType, dataDic);
			}

			Debug.Log(string.Format("EasyExcel: {0} tables loaded.", dataPool.Count));
		}

		public T Get<T>(int id) where T : RowData
		{
			DataDic soDic = null;
			dataPool.TryGetValue(typeof(T), out soDic);
			if (soDic != null)
			{
				RowData data = null;
				soDic.TryGetValue(id, out data);
				return data as T;
			}
			return null;
		}

		public DataDic GetList<T>() where T : RowData
		{
			DataDic soDic = null;
			dataPool.TryGetValue(typeof(T), out soDic);
			return soDic;
		}
	}
}