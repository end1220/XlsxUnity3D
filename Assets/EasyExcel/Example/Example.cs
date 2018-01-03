
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using EasyExcel;


public class Example : MonoBehaviour
{
	DataTableManager dataTableManager = new DataTableManager();

	void Start()
	{
		// Load all data table like this.
		dataTableManager.Load();

		/*
		
		// Get ItemRowData with id 1001
		var itm = dataTableManager.Get<ItemRowData>(1001);
		Debug.Log(itm.Description);

		// Get RoleRowData list
		Dictionary<int, RowData> dic = dataTableManager.GetList<RoleRowData>();
		foreach (var item in dic.Values)
		{
			RoleRowData np = item as RoleRowData;
			Debug.Log(np.Icon);
		}

		*/
	}

	void OnGUI()
	{
		// Just for test show, you do not have to know the details below.

		int index = 0;
		int labelBottom = 0;
		index++;
		GUI.Label(new Rect(10, labelBottom + index * 40, 800, 40), "API examples:");
		index++;
		GUI.Label(new Rect(30, labelBottom + index * 40, 800, 40), "1.Load all data:\n    DataTableManager.load();");
		index++;
		GUI.Label(new Rect(30, labelBottom + index * 40, 800, 40), "2.Find a ItemRowData by id:\n    DataTableManager.Get<ItemRowData>(id);");
		index++;
		GUI.Label(new Rect(30, labelBottom + index * 40, 800, 40), "3.Find ItemRowData list:\n    DataTableManager.GetList<ItemRowData>();");

		index++;
		index++;
		Type baseType = typeof(DataTable);
		Assembly assembly = baseType.Assembly;
		var types = assembly.GetTypes().Where(t => t.IsSubclassOf(baseType));
		if (types.Count() == 0)
		{
			labelBottom = index * 40;
			GUI.Label(new Rect(10, labelBottom, 800, 100), "No DataTable loaded, you may need to do these before playing:\n\n1.Import xlsx files by Tools->EasyExcel->Import\n\n2.Build assetbundle by Tools->EasyExcel->Build Assetbundle");
		}
		else
		{
			labelBottom = index * 40;
			GUI.Label(new Rect(10, labelBottom, 800, 30), string.Format("Loaded {0} tables:", types.Count()));
			labelBottom += 30;
			int typesIndex = 0;
			foreach (Type tableType in types)
			{
				string dataTableClassName = tableType.Name;
				string headName = dataTableClassName.Substring(0, dataTableClassName.IndexOf(Config.DataTableClassNamePostfix));
				string rowDataClassName = Config.GetRowDataClassName(headName);
				Type rowType = Type.GetType(rowDataClassName);
				var dic = dataTableManager.GetList(rowType);
				GUI.Label(new Rect(30, labelBottom + typesIndex * 20, 1000, 20), "DataTableClass: " + dataTableClassName + "\t\tRowDataClass: " + rowDataClassName + "\t\trow count: " + (dic != null ? dic.Count.ToString() : "empty"));
				typesIndex++;
			}
		}
	}

}