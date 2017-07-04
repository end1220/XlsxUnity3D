using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Lite
{
	public abstract class BaseDataCollection : ScriptableObject
	{
		public abstract void AddData(BaseData data);

		public abstract int GetDataCount();

		public abstract BaseData GetData(int index);

	}
}