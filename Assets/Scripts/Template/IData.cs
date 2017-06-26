using System;
using System.IO;
using System.Collections;


namespace Lite
{
	public class IData
	{
		private int _id = 0;
		public int id { get {return _id;} }

        private string _strId;
        public string strId { get { return _strId; } }

        public IData()
		{
				
		}

		public virtual int init(TabReader reader, int row, int column)
		{
			if (false == int.TryParse(reader.At(row, column), out _id))
				_strId = reader.At(row, column).ToString();

			return ++column;
		}
	}
}