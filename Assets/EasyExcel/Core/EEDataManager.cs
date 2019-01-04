using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace EasyExcel
{
	using RowDataDictionary = Dictionary<int, RowData>;

	public class EEDataManager
	{
		private readonly Dictionary<Type, RowDataDictionary> tableDataDic = new Dictionary<Type, RowDataDictionary>();

		public T Get<T>(int id) where T : RowData
		{
			return Get(id, typeof(T)) as T;
		}

		public RowData Get(int id, Type type)
		{
			RowDataDictionary soDic;
			tableDataDic.TryGetValue(type, out soDic);
			if (soDic == null) return null;
			RowData data;
			soDic.TryGetValue(id, out data);
			return data;
		}

		public RowDataDictionary GetList<T>() where T : RowData
		{
			return GetList(typeof(T));
		}

		public RowDataDictionary GetList(Type type)
		{
			RowDataDictionary soDic;
			tableDataDic.TryGetValue(type, out soDic);
			return soDic;
		}

		public void Load()
		{
#if UNITY_EDITOR
			if (!EESettings.Current.GeneratedAssetPath.Contains("/Resources/"))
			{
				EditorUtility.DisplayDialog("EasyExcel",
					string.Format(
						"AssetPath of EasyExcel Settings MUST be in Resources folder.\nCurrent is {0}.",
						EESettings.Current.GeneratedAssetPath), "OK");
				return;
			}
#endif
			tableDataDic.Clear();

			var baseDataTableType = typeof(RowDataCollection);
			foreach (var dataTableType in baseDataTableType.Assembly.GetTypes().Where(t => t.IsSubclassOf(baseDataTableType)))
				ParseOneDataTable(dataTableType);

			EELog.Log(string.Format("{0} tables loaded.", tableDataDic.Count));
		}

		private void ParseOneDataTable(Type dataTableType)
		{
			try
			{
				//var dataTableClassName = dataTableType.Name;
				var headName = GetDataTableHead(dataTableType);//dataTableClassName.Substring(0, dataTableClassName.IndexOf(EESettings.Current.DataTableClassNamePostfix, StringComparison.Ordinal));
				var filePath = EESettings.Current.GeneratedAssetPath.Substring(EESettings.Current.GeneratedAssetPath.IndexOf("Resources/", StringComparison.Ordinal) + "Resources/".Length) + headName;
				var table = Resources.Load(filePath) as RowDataCollection;
				if (table == null)
				{
					EELog.LogError("EEDataManager: Load asset error with " + filePath);
					return;
				}

				var dataDict = new RowDataDictionary();
				for (var i = 0; i < table.GetDataCount(); ++i)
				{
					var data = table.GetData(i);
					dataDict.Add(data.ID, data);
				}

				var rowDataType = GetRowDataClassType(dataTableType);
				tableDataDic.Add(rowDataType, dataDict);
			}
			catch (Exception e)
			{
				EELog.LogError(e.ToString());
			}
		}

		private static Type GetRowDataClassType(Type dataTableType)
		{
			var headName = GetDataTableHead(dataTableType);
			var type = Type.GetType(EESettings.Current.GetRowDataClassName(headName, true));
			return type;
		}

		private static string GetDataTableHead(Type dataTableType)
		{
			return dataTableType.Name.Substring(0, dataTableType.Name.IndexOf(EESettings.Current.DataTablePostfix, StringComparison.Ordinal));
		}
	}
}