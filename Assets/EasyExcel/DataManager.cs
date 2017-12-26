
﻿using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System;


namespace EasyExcel
{

	using DataDic = Dictionary<int, SingleData>;

	/// <summary>
	/// A piece of data represents a row in excel.
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
	/// Collection of SingleData
	/// </summary>
	public abstract class DataCollection : ScriptableObject
	{
		public abstract void AddData(SingleData data);

		public abstract int GetDataCount();

		public abstract SingleData GetData(int index);

	}

	/// <summary>
	/// Data manager for xlsx
	/// </summary>
	public class DataManager
	{
		private Dictionary<Type, DataDic> dataPool = new Dictionary<Type, DataDic>();

		
		public void Load()
		{
			string dataRootPath = "Assets" + Config.Instance.AssetPath;

			Type baseType = typeof(DataCollection);
			Assembly assembly = baseType.Assembly;
			foreach (Type type in assembly.GetTypes().Where(t => t.IsSubclassOf(baseType)))
			{
				string collectionClassName = type.Name;
				string headName = collectionClassName.Substring(0, collectionClassName.IndexOf("Asset"));

				string filePath;
				DataCollection collection = null;
#if UNITY_EDITOR
				filePath = dataRootPath + headName + ".asset";
				collection = AssetDatabase.LoadAssetAtPath(filePath, typeof(DataCollection)) as DataCollection;
#else
				filePath = "Template" + headName + ".asset";
				collection = Resources.Load(filePath) as DataCollection;
#endif
				if (collection == null)
				{
					Debug.LogError("DataManager: Load asset error, " + filePath);
					continue;
				}
				DataDic dataDic = new DataDic();
				for (int i = 0; i < collection.GetDataCount(); ++i)
				{
					SingleData data = collection.GetData(i);
					dataDic.Add(data.id, data);
				}

				Type dataClassType = Type.GetType(headName + "Data");
				dataPool.Add(dataClassType, dataDic);
			}
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
