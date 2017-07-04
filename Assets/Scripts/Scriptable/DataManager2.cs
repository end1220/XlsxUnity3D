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
	public class DataManager2
	{

		private Dictionary<Type, DataDic> templatePool = new Dictionary<Type, DataDic>();

		private static Dictionary<Type, string> files = new Dictionary<Type, string>()
		{
			{typeof(Npc0_Data), "Npc0"},
		};


		void Awake()
		{
			Init();
		}


		public void Init()
		{
			string scriptObjPath = "Assets/scriptable/";

			IDictionaryEnumerator iter = files.GetEnumerator();
			while (iter.MoveNext())
			{
				string filePath = iter.Value as string;

				DataDic soDic = new DataDic();

				BaseDataCollection collection = AssetDatabase.LoadAssetAtPath<BaseDataCollection>(scriptObjPath + filePath + ".asset");
				for (int i = 0; i < collection.GetDataCount(); ++i)
				{
					BaseData data = collection.GetData(i);
					soDic.Add(data.id, data);
				}

				templatePool.Add(iter.Key as Type, soDic);
			}
		}


		public T Get<T>(int id) where T : BaseData
		{
			DataDic soDic = null;
			templatePool.TryGetValue(typeof(T), out soDic);
			if (soDic != null)
			{
				BaseData data = null;
				soDic.TryGetValue(id, out data);
				return data as T;
			}
			return null;
		}


	}
}
