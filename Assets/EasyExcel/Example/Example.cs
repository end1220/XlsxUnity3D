using System;
using System.Linq;
using EasyExcel;
using UnityEngine;

public class Example : MonoBehaviour
{
	private readonly EEDataManager _eeDataManager = new EEDataManager();

	private void Start()
	{
		// Load all data table like this.
		_eeDataManager.Load();

		/*
		// Get ItemRowData with id 1001
		var itm = EEDataManager.Get<ItemRowData>(1001);
		Debug.Log(itm.Description);

		// Get RoleRowData list
		Dictionary<int, RowData> dic = EEDataManager.GetList<RoleRowData>();
		foreach (var item in dic.Values)
		{
			RoleRowData np = item as RoleRowData;
			Debug.Log(np.Icon);
		}
		*/
	}

	private void OnGUI()
	{
		// Just for test show, you do not have to know the details below.

		var index = 0;
		var labelBottom = 0;
		index++;
		GUI.Label(new Rect(10, labelBottom + index * 40, 800, 40), "API examples:");
		index++;
		GUI.Label(new Rect(30, labelBottom + index * 40, 800, 40), "1.Load all data:\n    EEDataManager.load();");
		index++;
		GUI.Label(new Rect(30, labelBottom + index * 40, 800, 40),
			"2.Find a ItemRowData by id:\n    EEDataManager.Get<ItemRowData>(id);");
		index++;
		GUI.Label(new Rect(30, labelBottom + index * 40, 800, 40),
			"3.Find ItemRowData list:\n    EEDataManager.GetList<ItemRowData>();");

		index++;
		var baseType = typeof(RowDataCollection);
		var assembly = baseType.Assembly;
		var types = assembly.GetTypes().Where(t => t.IsSubclassOf(baseType));
		var tableTypes = types as Type[] ?? types.ToArray();
		if (!tableTypes.Any())
		{
			labelBottom = index * 40;
			GUI.Label(new Rect(10, labelBottom, 800, 100),
				"No data loaded, you first need to import excel files by Tools->EasyExcel->Import.");
		}
		else
		{
			labelBottom = index * 40;
			GUI.Label(new Rect(10, labelBottom, 800, 30), string.Format("Loaded {0} tables:", tableTypes.Count()));
			labelBottom += 30;
			var typesIndex = 0;
			foreach (var tableType in tableTypes)
			{
				var dataTableClassName = tableType.Name;
				var headName =
					dataTableClassName.Substring(0, dataTableClassName.IndexOf(EESettings.Current.DataTablePostfix, StringComparison.Ordinal));
				var rowDataClassName = EESettings.Current.GetRowDataClassName(headName, true);
				var rowType = Type.GetType(rowDataClassName);
				var dic = _eeDataManager.GetList(rowType);
				GUI.Label(new Rect(30, labelBottom + typesIndex * 20, 1000, 20),
					"Sheet: " + dataTableClassName + "\tType: " + rowDataClassName +
					"\trow count: " + (dic != null ? dic.Count.ToString() : "empty"));
				typesIndex++;
			}
		}
	}
}