

// Auto generated file. DO NOT MODIFY.

using System;
using System.Collections.Generic;
using UnityEngine;


namespace Lite
{
	public class Npc0_Data : BaseData
	{
		public string res;

		public string path;

		public string ext;

		public int pos;

		public int memType;

		public int poolSlotID;



		#region _init (Do not invoke it)
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
#endregion


	}


	[CreateAssetMenu(fileName = "new Npc0", menuName = "Template/Npc0", order = 999)]
	public class Npc0_Collection : ScriptableObject
	{
		public List<Npc0_Data> elements = new List<Npc0_Data>();
	}
}