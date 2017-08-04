
using System;
using System.Collections.Generic;
using UnityEngine;


namespace EasyXlsx
{

	/// <summary>
	/// Confiig
	/// </summary>
	public class Confiig
	{
		public const int StartRow = 5;

		public const string XlsxPath = "/EasyXlsx/Example/Xlsx/";

		public const string OutputCsharpPath = "/EasyXlsx/Example/autogen/csharp/";

		public const string OutputAssetPath = "/EasyXlsx/Example/autogen/asset/";
	}


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
			int.TryParse(sheet[row][column], out id);
			column++;

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
}