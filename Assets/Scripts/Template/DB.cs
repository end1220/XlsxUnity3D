

using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

namespace Lite
{
	using Data = Dictionary<int, IData>;
	using StrData = Dictionary<string, IData>;

	public class DB
	{
		private const int mJsonStartIndex = 5;
		private const int mColumnStartIndex = 1;

		private Dictionary<Type, Data> mDataPoolDic = new Dictionary<Type, Data>();
		private Dictionary<Type, StrData> mStrDataPoolDic = new Dictionary<Type, StrData>();

		public bool HasLoaded = false;

		private Dictionary<Type, string> tableList = new Dictionary<Type, string>
			{
				{typeof(Npc0_Data), "Npc0"},
			};

		public Dictionary<Type, string> TableList
		{
			get { return tableList; }
		}

		public IEnumerator Load()
		{
			if (HasLoaded)
			{
				Debug.LogWarning("Skip DB.Load() , _isBuilt == true");
				yield return null;
			}

			IDictionaryEnumerator dicEtor = tableList.GetEnumerator();
			while (dicEtor.MoveNext())
			{
				LoadRes(dicEtor.Key as Type, "Template/" + dicEtor.Value as string);
				yield return new WaitForEndOfFrame();
			}

			HasLoaded = true;

			Debug.Log("db done");

			yield return null;
		}

		public void Release()
		{
			foreach (var node in mDataPoolDic)
				node.Value.Clear();
			mDataPoolDic.Clear();

			foreach (var node in mStrDataPoolDic)
				node.Value.Clear();
			mStrDataPoolDic.Clear();
		}

		public void LoadRes(Type type, string path)
		{
			if (string.IsNullOrEmpty(path))
				return;

			if (mDataPoolDic.ContainsKey(type) || mStrDataPoolDic.ContainsKey(type))
				return;

			try
			{
				string rawText = null;
#if UNITY_EDITOR
				var ta = Resources.Load(path) as TextAsset;
				rawText = ta.text;
#else
                    var ta = Resources.Load(path) as TextAsset;
                    rawText = ta.text;
#endif

				Dictionary<int, IData> dic = new Dictionary<int, IData>();
				Dictionary<string, IData> strDic = new Dictionary<string, IData>();
				mDataPoolDic.Add(type, dic);
				mStrDataPoolDic.Add(type, strDic);

				TabReader tabReader = new TabReader();
				tabReader.Load(rawText);
				int JsonNodeCount = tabReader.RowCount;
				for (int i = mJsonStartIndex; i < JsonNodeCount; i++)
				{
					try
					{
						System.Reflection.ConstructorInfo conInfo = type.GetConstructor(Type.EmptyTypes);
						IData t = conInfo.Invoke(null) as IData;

						t.init(tabReader, i, mColumnStartIndex);
						if (0 != t.id)
						{
							if (!dic.ContainsKey(t.id))
								dic.Add(t.id, t);
							else
								Debug.LogError("Config Data Ready Exist, TableName: " + t.GetType().Name + " ID:" + t.id);
						}
						else
						{
							if (!strDic.ContainsKey(t.strId))
								strDic.Add(t.strId, t);
							else
								Debug.LogError("Config Data Ready Exist, TableName: " + t.GetType().Name + " ID:" + t.strId);
						}
					}
					catch (Exception e)
					{
						Debug.LogError(type.ToString() + " ERROR!!! line " + (i + 2).ToString() + ", " + e.ToString());
					}
				}

			}
			catch (UnityException uex)
			{
				Debug.LogError(uex.ToString());
			}
			catch (Exception e)
			{
				Debug.LogError(e.ToString());
			}
		}

		public IData GetDataByKey(Type type, int key)
		{
			if (mDataPoolDic.ContainsKey(type) && GetDataPool(type).ContainsKey(key))
			{
				return GetDataPool(type)[key];
			}
			if (key > 0)
			{
				Debug.LogError("Config Data Is Null : " + type.Name + " key: " + key);
			}
			return null;
		}

		public IData GetDataByKey(Type type, string strKey)
		{
			if (mStrDataPoolDic.ContainsKey(type) && GetStrDataPool(type).ContainsKey(strKey))
			{
				return GetStrDataPool(type)[strKey];
			}

			Debug.LogError("Config Data Is Null : " + type.Name + " key: " + strKey);
			return null;
		}

		public T GetDataByKey<T>(int key) where T : IData
		{
			return GetDataByKey(typeof(T), key) as T;
		}

		public T GetDataByKey<T>(string strKey) where T : IData
		{
			return GetDataByKey(typeof(T), strKey) as T;
		}

		public Dictionary<int, IData> GetDataPool<T>()
		{
			return GetDataPool(typeof(T));
		}

		public Dictionary<string, IData> GetStrDataPool<T>()
		{
			return GetStrDataPool(typeof(T));
		}

		public Dictionary<int, IData> GetDataPool(Type type)
		{
			if (mDataPoolDic.ContainsKey(type))
			{
				return mDataPoolDic[type];
			}

			return null;
		}

		public Dictionary<string, IData> GetStrDataPool(Type type)
		{
			if (mStrDataPoolDic.ContainsKey(type))
			{
				return mStrDataPoolDic[type];
			}

			return null;
		}

		public int GetPoolSize(Type type)
		{
			if (mDataPoolDic.ContainsKey(type))
			{
				return mDataPoolDic[type].Count;
			}

			return 0;
		}

		public IEnumerator GetElement(Type type)
		{
			return null;
		}
	}
}
