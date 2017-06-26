
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;


namespace Lite
{
	public class TabReader
	{
		private int rowCount;
		public int RowCount { get { return rowCount; } }

		private int columnCount;
		public int ColumnCount { get { return columnCount; } }

		private String[,] rowColumnArray = null;

		public string At(int row, int column)
		{
			return rowColumnArray[row, column];
		}

		public void Load(string src)
		{
			string[] rowArray = src.Split('\n');
			rowCount = rowArray.Length;
			for (int i = 0; i < rowCount; i++)
			{
				string[] columnArray = rowArray[i].Split('\t');
				if (i == 0)
				{
					columnCount = columnArray.Length;
					rowColumnArray = new string[rowCount, columnCount];
					continue;
				}

				for (int j = 0; j < columnCount; ++j)
				{
					rowColumnArray[i, j] = columnArray[j];
				}		
			}

		}

	}
}
