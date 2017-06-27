

using System.Data;
using System.IO;

using UnityEngine;
using UnityEditor;

using Excel;
using TwGame;



public class Xlsx2X
{
	private static readonly int StartRow = 5;

	private static string xlsxPath = Application.dataPath + "/Locke/Xlsx/";
	private static string jsonPath = Application.dataPath + "/Locke/json/";
	private static string txtPath = Application.dataPath + "/Locke/txt/";
	private static string csPath = Application.dataPath + "/Locke/cs/";
	private static string scriptObjPath = Application.dataPath + "/Locke/obj/";


	[MenuItem("Tools/Excel/excel to asset")]
	static void xlsx_to_asset()
	{
		try
		{
			System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
			watch.Start();

			Object[] selectedObjects = Selection.GetFiltered(typeof(Object), SelectionMode.Assets);
			if (selectedObjects.Length == 0)
				Debug.LogError("u should select at least one .xlsx file.");
			int convertedCount = 0;
			for (int i = 0; i < selectedObjects.Length; i++)
			{
				Object obj = selectedObjects[i] as Object;

				string srcFilePath = xlsxPath + obj.name + ".xlsx";
				if (!File.Exists(srcFilePath))
					continue;
				var sheetData = ExcelReader.Instance.AsStringArray(srcFilePath);
				_to_asset(obj.name, sheetData);

				convertedCount++;
				UpdateProgress(convertedCount, selectedObjects.Length, (txtPath + obj.name).Replace(Application.dataPath, "Assets"));
			}
			EditorUtility.ClearProgressBar();
			AssetDatabase.Refresh();

			watch.Stop();

			Debug.Log(string.Format("excel to script object done, {0} files converted. take {1} ms.", convertedCount, watch.ElapsedMilliseconds));
		}
		catch (System.Exception e)
		{
			Debug.LogError(e.ToString());
			EditorUtility.ClearProgressBar();
			AssetDatabase.Refresh();
		}
	}


	#region to cs

	[MenuItem("Tools/Excel/excel to cs")]
	static void xlsx_to_cs()
	{
		try
		{
			System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
			watch.Start();

			Object[] selectedObjects = Selection.GetFiltered(typeof(Object), SelectionMode.Assets);
			if (selectedObjects.Length == 0)
				Debug.LogError("u should select at least one .xlsx file.");
			int convertedCount = 0;
			for (int i = 0; i < selectedObjects.Length; i++)
			{
				Object obj = selectedObjects[i] as Object;

				string srcFilePath = xlsxPath + obj.name + ".xlsx";
				if (!File.Exists(srcFilePath))
					continue;
				var sheetData = ExcelReader.Instance.AsStringArray(srcFilePath);
				string txt = _to_cs(sheetData, obj.name);
				System.IO.StreamWriter streamwriter = new System.IO.StreamWriter(csPath + GetClassName(obj.name) + ".cs", false);
				streamwriter.Write(txt);
				streamwriter.Flush();
				streamwriter.Close();

				convertedCount++;
				UpdateProgress(convertedCount, selectedObjects.Length, (txtPath + GetClassName(obj.name) + ".cs").Replace(Application.dataPath, "Assets"));
			}
			EditorUtility.ClearProgressBar();
			AssetDatabase.Refresh();

			watch.Stop();

			Debug.Log(string.Format("excel to cs done, {0} files converted. take {1} ms.", convertedCount, watch.ElapsedMilliseconds));
		}
		catch (System.Exception e)
		{
			Debug.LogError(e.ToString());
			EditorUtility.ClearProgressBar();
			AssetDatabase.Refresh();
		}
	}

	#endregion


	public static void _to_asset(string fileName, ExcelReader.SheetData sheetData)
	{
		try
		{
			string targetDir = scriptObjPath + fileName;
			if (Directory.Exists(targetDir))
				Directory.Delete(targetDir, true);
			Directory.CreateDirectory(targetDir);

			for (int row = StartRow; row < sheetData.rowCount; ++row)
			{
				for (int col = 0; col < sheetData.columnCount; ++col)
					sheetData.At(row, col).Replace("\n", "\\n");

				var asset = ScriptableObject.CreateInstance(GetClassName(fileName));
				BaseData tp = asset as BaseData;
				tp._init(sheetData.Table, row, 0);

				string itemPath = targetDir + "/" + tp.id + ".asset";
				itemPath = itemPath.Substring(itemPath.IndexOf("Assets"));
				AssetDatabase.CreateAsset(asset, itemPath);
			}
			
			AssetDatabase.Refresh();
		}
		catch (System.Exception ex)
		{
			Debug.LogError(ex.ToString());
		}

	}



	private static string _to_cs(ExcelReader.SheetData sheetData, string fileName)
	{
		try
		{
			string csFile = "\n\n// Auto generated. DO NOT MODIFY.\n\n";

			csFile += "using System;\nusing System.Collections.Generic;\nusing System.Linq;\nusing UnityEngine;\n\n\n";

			csFile += "namespace TwGame" + "\n";
			csFile += "{\n\t[CreateAssetMenu(fileName = \"new " + fileName + "\", menuName = \"Template/" + fileName + "\", order = 999)]\n";
			csFile += "\tpublic class " + GetClassName(fileName) + " : BaseData" + "\n";
			csFile += "\t" + "{" + "\n";

			int columnCount = sheetData.columnCount;

			// get variable names from 1st row.
			string[] variableName = new string[columnCount];
			for (int col = 0; col < columnCount; col++)
			{
				variableName[col] = sheetData.At(0, col);
			}

			// Get variableDescribe array from 2nd row
			string[] variableDescribe = new string[columnCount];
			for (int col = 0; col < columnCount; col++)
			{
				variableDescribe[col] = sheetData.At(1, col);
			}

			// Add variableType Info To CS from 3rd row
			string[] variableLength = new string[columnCount];
			string[] variableType = new string[columnCount];
			for (int col = 0; col < columnCount; col++)
			{
				int cellColumnIndex = col;
				if (cellColumnIndex >= 2)
				{
					string cellInfo = sheetData.At(3, col);
					variableLength[cellColumnIndex] = "";
					variableType[cellColumnIndex] = cellInfo;

					if (cellInfo.EndsWith("]"))
					{
						int startIndex = cellInfo.IndexOf('[');
						variableLength[cellColumnIndex] = cellInfo.Substring(startIndex + 1, cellInfo.Length - startIndex - 2);
						variableType[cellColumnIndex] = cellInfo.Substring(0, startIndex);
					}

					if (variableType[cellColumnIndex].Equals("int") || variableType[cellColumnIndex].Equals("float") ||
						variableType[cellColumnIndex].Equals("double") || variableType[cellColumnIndex].Equals("long") ||
						variableType[cellColumnIndex].Equals("string") || variableType[cellColumnIndex].Equals("bool") ||
						variableType[cellColumnIndex].Equals("JObject"))
					{
						if (variableLength[cellColumnIndex].Equals(""))
						{
							csFile += "\t\tpublic " + variableType[cellColumnIndex] + " " + variableName[cellColumnIndex] + ";\n";
							csFile += "\n";
						}
						else
						{
							csFile += "\t\tpublic " + variableType[cellColumnIndex] + "[] " + variableName[cellColumnIndex] + " = new " + variableType[cellColumnIndex] + "[" + variableLength[cellColumnIndex] + "];\t\n";
							csFile += "\n";
						}
					}
				}
			}


			// Add Init() Info To CS
			// Get variableDefaultValue array
			// the fourth row is variableDefaultValue
			string[] variableDefaultValue = new string[columnCount];
			csFile += "\n\n\t\t#region _init (Do not invoke it)\n";
			csFile += "\t\tpublic override int _init(List<List<string>> sheet, int row, int column)" + "\n";
			csFile += "\t\t{" + "\n";
			csFile += "\t\t\tcolumn = base._init(sheet, row, column);\n\n";
			for (int col = 0; col < columnCount; col++)
			{
				int cellColumnIndex = col;
				if (cellColumnIndex >= 2)
				{
					variableDefaultValue[cellColumnIndex] = sheetData.At(4, col);

					//special deal with bool
					if (variableType[cellColumnIndex].Equals("bool"))
					{
						if (variableDefaultValue[cellColumnIndex].Equals("0"))
							variableDefaultValue[cellColumnIndex] = "false";
						else
							variableDefaultValue[cellColumnIndex] = "true";
					}

					if (variableType[cellColumnIndex].Equals("int") || variableType[cellColumnIndex].Equals("float") ||
						variableType[cellColumnIndex].Equals("double") || variableType[cellColumnIndex].Equals("long") ||
						variableType[cellColumnIndex].Equals("bool"))
					{
						if (variableLength[cellColumnIndex].Equals(""))
						{
							csFile += "\t\t\t" + variableName[cellColumnIndex] + " = " + variableDefaultValue[cellColumnIndex] + ";\n";
							csFile += "\t\t\t" + variableType[cellColumnIndex] + ".TryParse(sheet[row][column], out " + variableName[cellColumnIndex] + ");\n";
						}
						else
						{
							// default value
							csFile += "\t\t\tfor(int i=0; i<" + variableLength[cellColumnIndex] + "; i++)" + "\n";
							csFile += "\t\t\t\t" + variableName[cellColumnIndex] + "[i] = " + variableDefaultValue[cellColumnIndex] + ";\n";

							csFile += "\t\t\tstring[] " + variableName[cellColumnIndex] + "Array = sheet[row][column].Split(\',\');" + "\n";
							csFile += "\t\t\tint " + variableName[cellColumnIndex] + "Count = " + variableName[cellColumnIndex] + "Array.Length;" + "\n";
							csFile += "\t\t\tfor(int i=0; i<" + variableLength[cellColumnIndex] + "; i++)\n";
							csFile += "\t\t\t{\n";
							csFile += "\t\t\t\tif(i < " + variableName[cellColumnIndex] + "Count)" + "\n";
							csFile += "\t\t\t\t\t" + variableType[cellColumnIndex] + ".TryParse(" + variableName[cellColumnIndex] + "Array[i], out " + variableName[cellColumnIndex] + "[i]);" + "\n";
							csFile += "\t\t\t\telse" + "\n";
							csFile += "\t\t\t\t\t" + variableName[cellColumnIndex] + "[i] = " + variableDefaultValue[cellColumnIndex] + ";\n";
							csFile += "\t\t\t}\n";
						}
					}
					if (variableType[cellColumnIndex].Equals("string"))
					{
						if (variableLength[cellColumnIndex].Equals(""))
						{
							csFile += "\t\t\tif(sheet[row][column] == null)" + "\n";
							csFile += "\t\t\t\t" + variableName[cellColumnIndex] + " = " + variableDefaultValue[cellColumnIndex] + ";\n";
							csFile += "\t\t\telse" + "\n";
							csFile += "\t\t\t\t" + variableName[cellColumnIndex] + " = sheet[row][column];\n";
						}
						else
						{
							csFile += "\t\t\tfor(int i=0; i<" + variableLength[cellColumnIndex] + "; i++)" + "\n";
							csFile += "\t\t\t\t" + variableName[cellColumnIndex] + "[i] = " + variableDefaultValue[cellColumnIndex] + ";\n";

							csFile += "\t\t\tstring[] " + variableName[cellColumnIndex] + "Array = sheet[row][column].Split(\',\');" + "\n";
							csFile += "\t\t\tint " + variableName[cellColumnIndex] + "Count = " + variableName[cellColumnIndex] + "Array.Length;" + "\n";
							csFile += "\t\t\tfor(int i=0; i<" + variableLength[cellColumnIndex] + "; i++){" + "\n";
							csFile += "\t\t\t\tif(i < " + variableName[cellColumnIndex] + "Count)" + "\n";
							csFile += "\t\t\t\t\t" + variableName[cellColumnIndex] + "[i] = " + variableName[cellColumnIndex] + "Array[i];\n";
							csFile += "\t\t\t\telse" + "\n";
							csFile += "\t\t\t\t\t" + variableName[cellColumnIndex] + "[i] = " + variableDefaultValue[cellColumnIndex] + ";\n";
							csFile += "\t\t\t}\n";
						}
					}
					// JObject
					if (variableType[cellColumnIndex].Equals("JObject"))
					{
						csFile += "\t\t\tfor(int i=0; i<" + variableLength[cellColumnIndex] + "; i++){" + "\n";
						csFile += "\t\t\t\tJArray ja = (JArray)JsonConvert.DeserializeObject(sheet[row][column]);\n";
						csFile += "\t\t\t\t" + variableName[cellColumnIndex] + "[i] = (JObject)ja[i];\n";
						csFile += "\t\t\t}\n";
					}

					csFile += "\t\t\tcolumn++;\n";
					csFile += "\n";

				}
			}
			csFile += "\t\t\treturn column;\n";
			csFile += "\t\t}\n#endregion\n\n\n";


			csFile += "\t}" + "\n";
			csFile += "}";
			
			return csFile;
		}
		catch (System.IO.IOException ex)
		{
			throw new System.IO.IOException(ex.Message);
		}
	}


	static void UpdateProgress(int progress, int progressMax, string desc)
	{
		string title = "Processing...[" + progress + " - " + progressMax + "]";
		float value = (float)progress / (float)progressMax;
		EditorUtility.DisplayProgressBar(title, desc, value);
	}


	static string GetClassName(string fileName)
	{
		return fileName + "_Template";
	}

}


