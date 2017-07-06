using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Lite
{
	[Serializable]
	public abstract class BaseData
	{
		public int id;

		public virtual int _init(List<List<string>> sheet, int row, int column)
		{
			column++;

			id = 0;
			int.TryParse(sheet[row][column], out id);
			column++;

			return column;
		}

	}
}