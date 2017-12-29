
// Auto generated file. DO NOT MODIFY.

using System;
using System.Collections.Generic;
using UnityEngine;
using EasyExcel;


[Serializable]
public class RoleData : SingleData
{
	public string Name;

	public string Description;

	public string Icon;

	public int HP;

	public int Attack;

	public float Speed;

#if UNITY_EDITOR
	public override int _init(List<List<string>> sheet, int row, int column)
	{
		column = base._init(sheet, row, column);

		if(sheet[row][column] == null)
			Name = "Orc";
		else
			Name = sheet[row][column];
		column++;

		if(sheet[row][column] == null)
			Description = "Orc...";
		else
			Description = sheet[row][column];
		column++;

		if(sheet[row][column] == null)
			Icon = "icon/icon.png";
		else
			Icon = sheet[row][column];
		column++;

		HP = 80;
		int.TryParse(sheet[row][column], out HP);
		column++;

		Attack = 4;
		int.TryParse(sheet[row][column], out Attack);
		column++;

		Speed = 2;
		float.TryParse(sheet[row][column], out Speed);
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
