
// Auto generated file. DO NOT MODIFY.

using System;
using System.Collections.Generic;
using UnityEngine;
using EasyXlsx;


[Serializable]
public class Npc0Data : BaseData
{
	public string res;

	public string path;

	public string ext;

	public int pos;

	public int memType;

	public int poolSlotID;



#if UNITY_EDITOR
	public override int _init(List<List<string>> sheet, int row, int column)
	{
		column = base._init(sheet, row, column);

		if(sheet[row][column] == null)
			res = "";
		else
			res = sheet[row][column];
		column++;

		if(sheet[row][column] == null)
			path = "";
		else
			path = sheet[row][column];
		column++;

		if(sheet[row][column] == null)
			ext = "";
		else
			ext = sheet[row][column];
		column++;

		pos = 0;
		int.TryParse(sheet[row][column], out pos);
		column++;

		memType = 0;
		int.TryParse(sheet[row][column], out memType);
		column++;

		poolSlotID = 0;
		int.TryParse(sheet[row][column], out poolSlotID);
		column++;

		return column;

	}
#endif


}
public class Npc0Asset : BaseDataCollection
{
	public List<Npc0Data> elements = new List<Npc0Data>();

	public override void AddData(BaseData data)
	{
		elements.Add(data as Npc0Data);
	}

	public override int GetDataCount()
	{
		return elements.Count;
	}

	public override BaseData GetData(int index)
	{
		return elements[index];
	}

}
