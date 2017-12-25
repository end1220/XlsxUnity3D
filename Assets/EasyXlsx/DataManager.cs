
﻿using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System;


namespace EasyXlsx
{

	using DataDic = Dictionary<int, BaseData>;

	/// <summary>
	/// Base data class
	/// </summary>
	[Serializable]
	public abstract class BaseData
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
	/// Base class of asset files.
	/// </summary>
	public abstract class BaseDataCollection : ScriptableObject
	{
		public abstract void AddData(BaseData data);

		public abstract int GetDataCount();

		public abstract BaseData GetData(int index);

	}

	/// <summary>
	/// Data manager for xlsx
	/// </summary>
	public class DataManager
	{
		private Dictionary<Type, DataDic> dataPool = new Dictionary<Type, DataDic>();

		
		public void Load()
		{
			string dataRootPath = "Assets" + Config.AssetPath;

			Type baseType = typeof(BaseDataCollection);
			Assembly assembly = baseType.Assembly;
			foreach (Type type in assembly.GetTypes().Where(t => t.IsSubclassOf(baseType)))
			{
				string collectionClassName = type.Name;
				string headName = collectionClassName.Substring(0, collectionClassName.IndexOf("Asset"));

				string filePath;
				BaseDataCollection collection = null;
#if UNITY_EDITOR
				filePath = dataRootPath + headName + ".asset";
				collection = AssetDatabase.LoadAssetAtPath<BaseDataCollection>(filePath);
#else
				filePath = "Template" + headName + ".asset";
				collection = Resources.Load(filePath) as BaseDataCollection;
#endif
				if (collection == null)
				{
					Debug.LogError("DataManager: Load asset error, " + filePath);
					continue;
				}
				DataDic dataDic = new DataDic();
				for (int i = 0; i < collection.GetDataCount(); ++i)
				{
					BaseData data = collection.GetData(i);
					dataDic.Add(data.id, data);
				}

				Type dataClassType = Type.GetType(headName + "Data");
				dataPool.Add(dataClassType, dataDic);
			}
		}

		public T Get<T>(int id) where T : BaseData
		{
			DataDic soDic = null;
			dataPool.TryGetValue(typeof(T), out soDic);
			if (soDic != null)
			{
				BaseData data = null;
				soDic.TryGetValue(id, out data);
				return data as T;
			}
			return null;
		}

		public DataDic GetList<T>() where T : BaseData
		{
			DataDic soDic = null;
			dataPool.TryGetValue(typeof(T), out soDic);
			return soDic;
		}
	}
}
