using System;
using System.Collections.Generic;
using UnityEngine;

namespace EasyExcel
{
	/// <summary>
	///     One row in a excel sheet.
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
	///     All RowData in the same excel sheet
	/// </summary>
	public abstract class RowDataCollection : ScriptableObject
	{
		public abstract void AddData(RowData data);
		public abstract int GetDataCount();
		public abstract RowData GetData(int index);
	}
	
	public static class EEConstant
	{
		public const string Version = "2.0";
	}
}