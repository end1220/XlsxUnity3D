using System.Collections.Generic;
using OfficeOpenXml;
using UnityEngine;

namespace EasyExcel
{
	public class EEWorksheet
	{
		private readonly Dictionary<int, Dictionary<int, EEWorksheetCell>> cells =
			new Dictionary<int, Dictionary<int, EEWorksheetCell>>();

		public int columnCount;
		public int rowCount;
		public Vector2 position;
		public string name;

		public EEWorksheet()
		{
		}

		public EEWorksheet(ExcelWorksheet sheet)
		{
			name = sheet.Name;
			if (sheet.Dimension != null)
			{
				rowCount = sheet.Dimension.Rows;
				columnCount = sheet.Dimension.Columns;
			}
			else
			{
				//empty Sheet
				rowCount = 0;
				columnCount = 0;
			}

			for (var row = 1; row <= rowCount; row++)
			for (var column = 1; column <= columnCount; column++)
			{
				var value = "";
				if (sheet.Cells[row, column].Value != null) value = sheet.Cells[row, column].Value.ToString();
				SetCellValue(row, column, value);
			}
		}

		public EEWorksheetCell SetCellValue(int row, int column, string value)
		{
			if (row < 1 || row > rowCount)
				return null;
			if (column < 1 || column > columnCount)
				return null;
			if (!cells.ContainsKey(row)) cells[row] = new Dictionary<int, EEWorksheetCell>();
			if (cells[row].ContainsKey(column))
			{
				cells[row][column].value = value;
				return cells[row][column];
			}

			var cell = new EEWorksheetCell(row, column, value);
			cells[row][column] = cell;
			return cell;
		}

		public string GetCellValue(int row, int column)
		{
			var cell = GetCell(row, column);
			return cell != null ? cell.value : SetCellValue(row, column, "").value;
		}

		public EEWorksheetCell GetCell(int row, int column)
		{
			if (!cells.ContainsKey(row)) return null;
			return cells[row].ContainsKey(column) ? cells[row][column] : null;
		}

		public void SetCellTypeByRow(int row, CellType type)
		{
			for (var column = 0; column < columnCount; column++)
			{
				var cell = GetCell(row, column);
				if (cell != null) cell.type = type;
			}
		}

		public void SetCellTypeByColumn(int column, CellType type, List<string> values = null)
		{
			for (var row = 1; row <= rowCount; row++)
			{
				var cell = GetCell(row, column);
				if (cell == null) continue;
				cell.type = type;
				if (values != null) cell.ValueSelected = values;
			}
		}

		public void Dump()
		{
			var msg = "";
			for (var row = 1; row <= rowCount; row++)
			{
				for (var column = 1; column <= columnCount; column++)
					msg += string.Format("{0} ", GetCellValue(row, column));
				msg += "\n";
			}
			Debug.Log(msg);
		}
	}
}