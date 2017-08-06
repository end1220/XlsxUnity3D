
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;



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

		
		public void Init()
		{
			foreach (var item in ClassList.files)
			{
				string filePath;
				BaseDataCollection collection = null;
#if UNITY_EDITOR
				filePath = "Assets" + Config.AssetPath + item.Value;
				collection = AssetDatabase.LoadAssetAtPath<BaseDataCollection>(filePath);
#else
				filePath = "Template" + item.Value;
				collection = Resources.Load(filePath) as BaseDataCollection;
#endif
				if (collection == null)
				{
					Debug.LogError("DataManager: Load asset error, " + filePath);
					continue;
				}
				DataDic soDic = new DataDic();
				for (int i = 0; i < collection.GetDataCount(); ++i)
				{
					BaseData data = collection.GetData(i);
					soDic.Add(data.id, data);
				}

				dataPool.Add(item.Key, soDic);
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
