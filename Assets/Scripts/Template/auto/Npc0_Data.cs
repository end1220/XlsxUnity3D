using System;
using System.Collections.Generic;


namespace Lite
{
	public class Npc0_Data : IData
	{
		string _res;	//res
		public string res { get { return _res;} }

		string _path;	//path
		public string path { get { return _path;} }

		string _ext;	//扩展
		public string ext { get { return _ext;} }

		int _pos;	//pos
		public int pos { get { return _pos;} }

		int _memType;	//生存时间
		public int memType { get { return _memType;} }

		int _poolSlotID;	//poolSlotID
		public int poolSlotID { get { return _poolSlotID;} }

		public override int init(TabReader reader, int row, int column)
		{
			column = base.init(reader, row, column);

			if(reader.At(row, column) == null)
				_res = "";
			else
				_res = reader.At(row, column);
			column++;

			if(reader.At(row, column) == null)
				_path = "";
			else
				_path = reader.At(row, column);
			column++;

			if(reader.At(row, column) == null)
				_ext = "";
			else
				_ext = reader.At(row, column);
			column++;

			_pos = 0;
			int.TryParse(reader.At(row, column), out _pos);
			column++;

			_memType = 0;
			int.TryParse(reader.At(row, column), out _memType);
			column++;

			_poolSlotID = 0;
			int.TryParse(reader.At(row, column), out _poolSlotID);
			column++;

			return column;
		}
	}
}