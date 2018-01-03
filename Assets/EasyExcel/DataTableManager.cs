
using UnityEngine;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System;


namespace EasyExcel
{

	using DataDic = Dictionary<int, RowData>;

	/// <summary>
	/// Data of one row in excel.
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
		private Dictionary<Type, DataDic> tableDataDic = new Dictionary<Type, DataDic>();


		public T Get<T>(int id) where T : RowData
		{
			return Get(id, typeof(T)) as T;
		}

		public RowData Get(int id, Type type)
		{
			DataDic soDic = null;
			tableDataDic.TryGetValue(type, out soDic);
			if (soDic != null)
			{
				RowData data = null;
				soDic.TryGetValue(id, out data);
				return data;
			}
			return null;
		}

		public DataDic GetList<T>() where T : RowData
		{
			return GetList(typeof(T));
		}

		public DataDic GetList(Type type)
		{
			DataDic soDic = null;
			tableDataDic.TryGetValue(type, out soDic);
			return soDic;
		}

		public void Load()
		{
			bool fromAssetbundle = Config.LoadFromAssetbundle;

			AssetBundle bundle = null;

			if (fromAssetbundle)
			{
				string assetbundlePath = Config.AssetbundlePath + Config.AssetbudleName;
#if UNITY_EDITOR
				if (!System.IO.File.Exists(assetbundlePath))
				{
					UnityEditor.EditorUtility.DisplayDialog("EasyExcel", string.Format("When Config.LoadFromAssetbundle is true, you should build assetbundle before playing. \nClick Tools->EasyExcel->Build Assetbundle to build.", assetbundlePath), "OK");
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
					UnityEditor.EditorUtility.DisplayDialog("EasyExcel", string.Format("When Config.LoadFromAssetbundle is false, Config.AssetPath MUST be in Resources folder.\nCurrent is {0}.", Config.AssetPath), "OK");
					return;
				}
#endif
			}

			tableDataDic.Clear();

			Type baseType = typeof(DataTable);
			Assembly assembly = baseType.Assembly;
			foreach (Type type in assembly.GetTypes().Where(t => t.IsSubclassOf(baseType)))
			{
				string dataTableClassName = type.Name;
				string headName = dataTableClassName.Substring(0, dataTableClassName.IndexOf(Config.DataTableClassNamePostfix));

				string filePath = null;
				DataTable table = null;
				if (fromAssetbundle)
				{
					filePath = Config.AssetPath + Config.GetAssetFileName(headName);
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

				Type dataClassType = Type.GetType(Config.GetRowDataClassName(headName));
				tableDataDic.Add(dataClassType, dataDic);
			}

			if (fromAssetbundle && bundle != null)
			{
				bundle.Unload(true);
			}

			Debug.Log(string.Format("EasyExcel: {0} tables loaded.", tableDataDic.Count));
		}

	}
}
