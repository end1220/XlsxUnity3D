
using System;
using System.Data;
using System.IO;

using UnityEngine;
using UnityEditor;

using Excel;
using Lite;



public class Xlsx_ScriptableObject
{
	private static readonly int StartRow = 5;

	private static string csPath = Application.dataPath + "/Scripts/autogen/";
	private static string scriptObjPath = Application.dataPath + "/scriptable/";


	public static void xlsx_to_asset(string srcFilePath)
	{
		try
		{
			if (!Directory.Exists(scriptObjPath))
				Directory.CreateDirectory(scriptObjPath);

			int index = srcFilePath.LastIndexOf("/") + 1;
			string fileName = srcFilePath.Substring(index, srcFilePath.LastIndexOf(".") - index);
			var sheetData = ExcelReader.Instance.AsStringArray(srcFilePath);
			_to_asset(fileName, sheetData);
		}
		catch (System.Exception e)
		{
			Debug.LogError(e.ToString());
			AssetDatabase.Refresh();
		}
	}


	public static void xlsx_to_cs(string srcFilePath)
	{
		try
		{
			int index = srcFilePath.LastIndexOf("/") + 1;
			string fileName = srcFilePath.Substring(index, srcFilePath.LastIndexOf(".") - index);
			var sheetData = ExcelReader.Instance.AsStringArray(srcFilePath);
			string txt = _to_cs(sheetData, fileName);
			System.IO.StreamWriter streamwriter = new System.IO.StreamWriter(csPath + GetAssetClassName(fileName) + ".cs", false);
			streamwriter.Write(txt);
			streamwriter.Flush();
			streamwriter.Close();

		}
		catch (System.Exception e)
		{
			Debug.LogError(e.ToString());
			AssetDatabase.Refresh();
		}
	}



	public static void _to_asset(string fileName, ExcelReader.SheetData sheetData)
	{
		try
		{
			/*if (!Directory.Exists(targetDir))
				Directory.CreateDirectory(targetDir);

			string targetDir = scriptObjPath + fileName;
			if (Directory.Exists(targetDir))
				Directory.Delete(targetDir, true);
			Directory.CreateDirectory(targetDir);*/

			var asset = ScriptableObject.CreateInstance(GetAssetClassName(fileName));
			BaseDataCollection dataCollect = asset as BaseDataCollection;

			string className = GetDataClassName(fileName);
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

			for (int row = StartRow; row < sheetData.rowCount; ++row)
			{
				for (int col = 0; col < sheetData.columnCount; ++col)
					sheetData.At(row, col).Replace("\n", "\\n");

				BaseData inst = dataCtor.Invoke(null) as BaseData;
				inst._init(sheetData.Table, row, 0);
				dataCollect.AddData(inst);
			}

			string itemPath = scriptObjPath + fileName + ".asset";
			itemPath = itemPath.Substring(itemPath.IndexOf("Assets"));
			AssetDatabase.CreateAsset(asset, itemPath);
			
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
			string csFile = "\n\n// Auto generated file. DO NOT MODIFY.\n\n";

			csFile += "using System;\nusing System.Collections.Generic;\nusing UnityEngine;\nusing Lite;\n\n\n";
			csFile += "[Serializable]\n";
			csFile += "public class " + GetDataClassName(fileName) + " : BaseData" + "\n";
			csFile += "" + "{" + "\n";

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
							csFile += "\tpublic " + variableType[cellColumnIndex] + " " + variableName[cellColumnIndex] + ";\n";
							csFile += "\n";
						}
						else
						{
							csFile += "\tpublic " + variableType[cellColumnIndex] + "[] " + variableName[cellColumnIndex] + " = new " + variableType[cellColumnIndex] + "[" + variableLength[cellColumnIndex] + "];\t\n";
							csFile += "\n";
						}
					}
				}
			}


			// Add Init() Info To CS
			// Get variableDefaultValue array
			// the fourth row is variableDefaultValue
			string[] variableDefaultValue = new string[columnCount];
			csFile += "\n\n\t#if UNITY_EDITOR\n";
			csFile += "\n\tpublic override int _init(List<List<string>> sheet, int row, int column)" + "\n";
			csFile += "\t{" + "\n";
			csFile += "\t\tcolumn = base._init(sheet, row, column);\n\n";
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
							csFile += "\t\t" + variableName[cellColumnIndex] + " = " + variableDefaultValue[cellColumnIndex] + ";\n";
							csFile += "\t\t" + variableType[cellColumnIndex] + ".TryParse(sheet[row][column], out " + variableName[cellColumnIndex] + ");\n";
						}
						else
						{
							// default value
							csFile += "\t\tfor(int i=0; i<" + variableLength[cellColumnIndex] + "; i++)" + "\n";
							csFile += "\t\t\t" + variableName[cellColumnIndex] + "[i] = " + variableDefaultValue[cellColumnIndex] + ";\n";

							csFile += "\t\tstring[] " + variableName[cellColumnIndex] + "Array = sheet[row][column].Split(\',\');" + "\n";
							csFile += "\t\tint " + variableName[cellColumnIndex] + "Count = " + variableName[cellColumnIndex] + "Array.Length;" + "\n";
							csFile += "\t\tfor(int i=0; i<" + variableLength[cellColumnIndex] + "; i++)\n";
							csFile += "\t\t{\n";
							csFile += "\t\t\tif(i < " + variableName[cellColumnIndex] + "Count)" + "\n";
							csFile += "\t\t\t\t" + variableType[cellColumnIndex] + ".TryParse(" + variableName[cellColumnIndex] + "Array[i], out " + variableName[cellColumnIndex] + "[i]);" + "\n";
							csFile += "\t\t\telse" + "\n";
							csFile += "\t\t\t\t" + variableName[cellColumnIndex] + "[i] = " + variableDefaultValue[cellColumnIndex] + ";\n";
							csFile += "\t\t}\n";
						}
					}
					if (variableType[cellColumnIndex].Equals("string"))
					{
						if (variableLength[cellColumnIndex].Equals(""))
						{
							csFile += "\t\tif(sheet[row][column] == null)" + "\n";
							csFile += "\t\t\t" + variableName[cellColumnIndex] + " = " + variableDefaultValue[cellColumnIndex] + ";\n";
							csFile += "\t\telse" + "\n";
							csFile += "\t\t\t" + variableName[cellColumnIndex] + " = sheet[row][column];\n";
						}
						else
						{
							csFile += "\t\tfor(int i=0; i<" + variableLength[cellColumnIndex] + "; i++)" + "\n";
							csFile += "\t\t\t" + variableName[cellColumnIndex] + "[i] = " + variableDefaultValue[cellColumnIndex] + ";\n";

							csFile += "\t\tstring[] " + variableName[cellColumnIndex] + "Array = sheet[row][column].Split(\',\');" + "\n";
							csFile += "\t\tint " + variableName[cellColumnIndex] + "Count = " + variableName[cellColumnIndex] + "Array.Length;" + "\n";
							csFile += "\t\tfor(int i=0; i<" + variableLength[cellColumnIndex] + "; i++){" + "\n";
							csFile += "\t\t\tif(i < " + variableName[cellColumnIndex] + "Count)" + "\n";
							csFile += "\t\t\t\t" + variableName[cellColumnIndex] + "[i] = " + variableName[cellColumnIndex] + "Array[i];\n";
							csFile += "\t\t\telse" + "\n";
							csFile += "\t\t\t\t" + variableName[cellColumnIndex] + "[i] = " + variableDefaultValue[cellColumnIndex] + ";\n";
							csFile += "\t\t}\n";
						}
					}
					// JObject
					if (variableType[cellColumnIndex].Equals("JObject"))
					{
						csFile += "\t\tfor(int i=0; i<" + variableLength[cellColumnIndex] + "; i++){" + "\n";
						csFile += "\t\t\tJArray ja = (JArray)JsonConvert.DeserializeObject(sheet[row][column]);\n";
						csFile += "\t\t\t" + variableName[cellColumnIndex] + "[i] = (JObject)ja[i];\n";
						csFile += "\t\t}\n";
					}

					csFile += "\t\tcolumn++;\n";
					csFile += "\n";

				}
			}
			csFile += "\t\treturn column;\n";
			csFile += "\n\t}\n#endif\n\n\n";

			csFile += "}" + "\n";
			
			// collection class
			csFile += "\n\n[CreateAssetMenu(fileName = \"new " + fileName + "\", menuName = \"Template/" + fileName + "\", order = 999)]\n";
			csFile += "public class " + GetAssetClassName(fileName) + " : BaseDataCollection\n";
			csFile += "{\n";
			csFile += "\tpublic List<" + GetDataClassName(fileName) + "> elements = new List<" + GetDataClassName(fileName) + ">();\n\n";

			csFile += "\tpublic override void AddData(BaseData data)\n\t{\n\t\telements.Add(data as " + GetDataClassName(fileName) + ");\n\t}\n\n";
			csFile += "\tpublic override int GetDataCount()\n\t{\n\t\treturn elements.Count;\n\t}\n\n";
			csFile += "\tpublic override BaseData GetData(int index)\n\t{\n\t\treturn elements[index];\n\t}\n\n";

			csFile += "}\n";
			
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


	static string GetDataClassName(string fileName)
	{
		return fileName + "_Data";
	}

	static string GetAssetClassName(string fileName)
	{
		return fileName + "_Asset";
	}

}


