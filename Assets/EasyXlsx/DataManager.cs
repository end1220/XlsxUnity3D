using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System;
using Lite;


namespace Lite
{
	using DataDic = Dictionary<int, BaseData>;
	public class DataManager
	{
		private Dictionary<Type, DataDic> dataPool = new Dictionary<Type, DataDic>();

		private static Dictionary<Type, string> files = new Dictionary<Type, string>()
		{
			{typeof(Npc0_Data), "Npc0.asset"},
		};


		public void Init()
		{
			string scriptObjPath = "Assets" + XlsxConst.OutputAssetPath;

			IDictionaryEnumerator iter = files.GetEnumerator();
			while (iter.MoveNext())
			{
				string filePath = scriptObjPath + (iter.Value as string);

				var collection = AssetDatabase.LoadAssetAtPath<BaseDataCollection>(filePath);
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

				dataPool.Add(iter.Key as Type, soDic);
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
