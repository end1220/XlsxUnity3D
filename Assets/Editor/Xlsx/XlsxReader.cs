using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using Excel;
using System.Data;
using UnityEngine.UI;


namespace Lite
{
	
	public class XlsxReader
	{
		private static XlsxReader _inst = null;
		public static XlsxReader Instance
		{
			get
			{
				if (_inst == null)
					_inst = new XlsxReader();
				return _inst;
			}
		}

		public class SheetData
		{
			public int rowCount = 0;
			public int columnCount = 0;
			private List<List<string>> table = new List<List<string>>();
			public List<List<string>> Table
			{
				get { return table; }
			}

			public string At(int row, int column)
			{
				return table[row][column];
			}

		}

		/// <summary>
		/// get data from xlsx file by sheet.
		/// </summary>
		/// <param name="filePath">absolute file path on the disk</param>
		/// <param name="sheet">sheet index, from 0 to max</param>
		/// <returns></returns>
		public SheetData AsStringArray(string filePath, int sheet = 0)
		{
			FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
			IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);

			SheetData sheetData = new SheetData();

			int sheetIndex = 0;
			do
			{
				if (sheetIndex != sheet)
				{
					sheetIndex++;
					continue;
				}

				// read rows
				int rowIndex = 0;
				while (excelReader.Read())
				{
					List<string> rowData = new List<string>();
					// read columns
					for (int col = 0; col < excelReader.FieldCount; col++)
					{
						rowData.Add(excelReader.IsDBNull(col) ? "" : excelReader.GetString(col));
					}
					sheetData.Table.Add(rowData);
					rowIndex++;
				}

				sheetData.rowCount = rowIndex;
				sheetData.columnCount = excelReader.FieldCount;

				break;

			} while (excelReader.NextResult());

			excelReader.Close();

			return sheetData;
		}


		#region Example

		// Example
		public void Example(string filePath)
		{
			FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
			IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);

			// way below can cause crash...
			DataSet result = excelReader.AsDataSet();
			int columns = result.Tables[0].Columns.Count;
			int rows = result.Tables[0].Rows.Count;

			string readData = "";
			for (int i = 0; i < rows; i++)
			{
				for (int j = 0; j < columns; j++)
				{
					string nvalue = result.Tables[0].Rows[i][j].ToString();
					Debug.Log(nvalue);
					if (i > 0)
					{
						readData += "\t\t" + nvalue;
					}
					else
					{
						readData += "   \t" + nvalue;
					}
				}
				readData += "\n";
			}

			// sheet->row->column
			do
			{
				// sheet name
				Debug.Log(excelReader.Name);
				while (excelReader.Read())
				{
					for (int i = 0; i < excelReader.FieldCount; i++)
					{
						string value = excelReader.IsDBNull(i) ? "" : excelReader.GetString(i);
						Debug.Log(value);
					}
				}
			} while (excelReader.NextResult());

			excelReader.Close();

		}
		#endregion

	}

}