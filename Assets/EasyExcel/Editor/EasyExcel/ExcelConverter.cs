
using System;
using UnityEngine;
using UnityEditor;


namespace EasyExcel
{
	/// <summary>
	/// Excel Converter
	/// </summary>
	public class ExcelConverter
	{
		public static void ToAsset(string xlsxPath, string outputPath)
		{
			try
			{
				int index = xlsxPath.LastIndexOf("/") + 1;
				string fileName = xlsxPath.Substring(index, xlsxPath.LastIndexOf(".") - index);
				var sheetData = ExcelReader.AsStringArray(xlsxPath);

				ToAsset(fileName, outputPath, sheetData);
			}
			catch (Exception e)
			{
				Debug.LogError(e.ToString());
				AssetDatabase.Refresh();
			}
		}

		public static void ToCSharp(string xlsxPath, string outputPath)
		{
			try
			{
				int index = xlsxPath.LastIndexOf("/") + 1;
				string fileName = xlsxPath.Substring(index, xlsxPath.LastIndexOf(".") - index);
				var sheetData = ExcelReader.AsStringArray(xlsxPath);
				string txt = ToCSharp(sheetData, fileName);
				System.IO.StreamWriter streamwriter = new System.IO.StreamWriter(outputPath + Config.GetDataTableClassName(fileName) + ".cs", false);
				streamwriter.Write(txt);
				streamwriter.Flush();
				streamwriter.Close();
			}
			catch (Exception e)
			{
				Debug.LogError(e.ToString());
				AssetDatabase.Refresh();
			}
		}

		public static string ToCSharp(string xlsxPath)
		{
			int index = xlsxPath.LastIndexOf("/") + 1;
			string fileName = xlsxPath.Substring(index, xlsxPath.LastIndexOf(".") - index);
			var sheetData = ExcelReader.AsStringArray(xlsxPath);
			string txt = ToCSharp(sheetData, fileName);
			return txt;
		}

		public static void ToAsset(string fileName, string outputPath, ExcelReader.SheetData sheetData)
		{
			try
			{
				var asset = ScriptableObject.CreateInstance(Config.GetDataTableClassName(fileName));
				DataTable dataCollect = asset as DataTable;

				string className = Config.GetRowDataClassName(fileName);
				Type dataType = Type.GetType(className);
				if (dataType == null)
				{
					System.Reflection.Assembly asmb = System.Reflection.Assembly.LoadFrom(Environment.CurrentDirectory + "/Library/ScriptAssemblies/Assembly-CSharp.dll");
					dataType = asmb.GetType(className);
				}
				if (dataType == null)
				{
					Debug.LogError(className + " not exist !");
					return;
				}

				System.Reflection.ConstructorInfo dataCtor = dataType.GetConstructor(Type.EmptyTypes);

				for (int row = Config.DATA_START_INDEX; row < sheetData.rowCount; ++row)
				{
					for (int col = 0; col < sheetData.columnCount; ++col)
						sheetData.At(row, col).Replace("\n", "\\n");

					RowData inst = dataCtor.Invoke(null) as RowData;
					inst._init(sheetData.Table, row, 0);
					dataCollect.AddData(inst);
				}

				string itemPath = outputPath + Config.GetAssetFileName(fileName);
				itemPath = itemPath.Substring(itemPath.IndexOf("Assets"));
				AssetDatabase.CreateAsset(asset, itemPath);

				AssetDatabase.Refresh();
			}
			catch (Exception ex)
			{
				Debug.LogError(ex.ToString());
			}

		}

		private static string ToCSharp(ExcelReader.SheetData sheetData, string fileName)
		{
			try
			{
				string csFile = "\n// Auto generated file. DO NOT MODIFY.\n\n";

				csFile += "using System;\nusing System.Collections.Generic;\nusing EasyExcel;\n\n\n";
				csFile += "[Serializable]\n";
				csFile += "public class " + Config.GetRowDataClassName(fileName) + " : RowData" + "\n";
				csFile += "" + "{" + "\n";

				int columnCount = sheetData.columnCount;

				// Get variable names
				string[] variableName = new string[columnCount];
				for (int col = 0; col < columnCount; col++)
				{
					variableName[col] = sheetData.At(Config.NAME_ROW_INDEX, col);
				}

				// Get variable types
				string[] variableLength = new string[columnCount];
				string[] variableType = new string[columnCount];
				// skip column 0 for ID
				for (int col = 1; col < columnCount; col++)
				{
					string cellInfo = sheetData.At(Config.TYPE_ROW_INDEX, col);
					variableLength[col] = null;
					variableType[col] = cellInfo;

					if (cellInfo.EndsWith("]"))
					{
						int startIndex = cellInfo.IndexOf('[');
						variableLength[col] = cellInfo.Substring(startIndex + 1, cellInfo.Length - startIndex - 2);
						variableType[col] = cellInfo.Substring(0, startIndex);
					}

					string varName = variableName[col];
					string varLen = variableLength[col];
					string varType = variableType[col];

					if (varType.Equals("int") || varType.Equals("float") ||
						varType.Equals("double") || varType.Equals("long") ||
						varType.Equals("string") || varType.Equals("bool") ||
						varType.Equals("JObject"))
					{
						if (varLen == null)
						{
							csFile += "\tpublic " + varType + " " + varName + ";\n\n";
						}
						else
						{
							csFile += "\tpublic " + varType + "[] " + varName + ";\n\n"; //" = new " + varType + "[" + varLen + "];\n\n";
						}
					}

				}

				//string[] variableDefaults = new string[columnCount];
				csFile += "#if UNITY_EDITOR\n";
				csFile += "\tpublic override int _init(List<List<string>> sheet, int row, int column)" + "\n";
				csFile += "\t{" + "\n";
				csFile += "\t\tcolumn = base._init(sheet, row, column);\n\n";
				// skip column 0 for ID
				for (int col = 1; col < columnCount; col++)
				{
					string varType = variableType[col];

					/*variableDefaults[col] = sheetData.At(Config.DEFAULT_VALUE_ROW_INDEX, col);
					if (varType.Equals("bool"))
					{
						if (variableDefaults[col].Equals("0"))
							variableDefaults[col] = "false";
						else
							variableDefaults[col] = "true";
					}

					string varDefault = variableDefaults[col];*/
					string varLen = variableLength[col];
					string varName = variableName[col];

					if (varType.Equals("int") || varType.Equals("float") || varType.Equals("double") || varType.Equals("long") || varType.Equals("bool"))
					{
						if (varLen == null)
						{
							//csFile += "\t\t" + varName + " = " + varDefault + ";\n";
							csFile += "\t\t" + varType + ".TryParse(sheet[row][column], out " + varName + ");\n";
						}
						else
						{
							csFile += "\t\tstring[] " + varName + "Array = sheet[row][column].Split(\',\');" + "\n";
							csFile += "\t\tint " + varName + "Count = " + varName + "Array.Length;" + "\n";
							csFile += "\t\t" + varName + " = new " + varType + "[" + varName + "Count];\n";
							csFile += "\t\tfor(int i = 0; i < " + varName + "Count; i++)\n";
							csFile += "\t\t{\n";
							csFile += "\t\t\t" + varType + ".TryParse(" + varName + "Array[i], out " + varName + "[i]);" + "\n";
							csFile += "\t\t}\n";
						}
					}
					else if (varType.Equals("string"))
					{
						if (varLen == null)
						{
							csFile += "\t\tif(sheet[row][column] == null)" + "\n";
							csFile += "\t\t\t" + varName + " = \"" + /*varDefault + */"\";\n";
							csFile += "\t\telse" + "\n";
							csFile += "\t\t\t" + varName + " = sheet[row][column];\n";
						}
						else
						{
							csFile += "\t\tstring[] " + varName + "Array = sheet[row][column].Split(\',\');" + "\n";
							csFile += "\t\tint " + varName + "Count = " + varName + "Array.Length;" + "\n";
							csFile += "\t\t" + varName + " = new " + varType + "[" + varName + "Count];\n";
							csFile += "\t\tfor(int i = 0; i < " + varName + "Count; i++)\n";
							csFile += "\t\t{\n";
							csFile += "\t\t\t" + varName + "[i] = " + varName + "Array[i];\n";
							csFile += "\t\t}\n";
						}
					}

					csFile += "\t\tcolumn++;\n";
					csFile += "\n";
				}
				csFile += "\t\treturn column;\n";
				csFile += "\n\t}\n#endif\n";

				csFile += "}\n\n";

				// DataTable class
				csFile += "public class " + Config.GetDataTableClassName(fileName) + " : DataTable\n";
				csFile += "{\n";
				csFile += "\tpublic List<" + Config.GetRowDataClassName(fileName) + "> elements = new List<" + Config.GetRowDataClassName(fileName) + ">();\n\n";

				csFile += "\tpublic override void AddData(RowData data)\n\t{\n\t\telements.Add(data as " + Config.GetRowDataClassName(fileName) + ");\n\t}\n\n";
				csFile += "\tpublic override int GetDataCount()\n\t{\n\t\treturn elements.Count;\n\t}\n\n";
				csFile += "\tpublic override RowData GetData(int index)\n\t{\n\t\treturn elements[index];\n\t}\n";

				csFile += "}\n";

				return csFile;
			}
			catch (Exception ex)
			{
				Debug.LogError(ex.ToString());
			}
			return "";
		}

	}

}

