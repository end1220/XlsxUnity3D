
// Auto generated file. DO NOT MODIFY.

using System;
using System.Collections.Generic;
using UnityEngine;
using EasyExcel;


[Serializable]
public class RoleData : SingleData
{
#if UNITY_EDITOR
	public override int _init(List<List<string>> sheet, int row, int column)
	{
		column = base._init(sheet, row, column);

		column++;

		column++;

		column++;

		column++;

		column++;

		column++;

		return column;

	}
#endif
}

public class RoleAsset : DataCollection
{
	public List<RoleData> elements = new List<RoleData>();

	public override void AddData(SingleData data)
	{
		elements.Add(data as RoleData);
	}

	public override int GetDataCount()
	{
		return elements.Count;
	}

	public override SingleData GetData(int index)
	{
		return elements[index];
	}
}
