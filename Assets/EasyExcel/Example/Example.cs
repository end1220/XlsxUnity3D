
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using EasyExcel;


public class Example : MonoBehaviour
{
	DataTableManager manager = new DataTableManager();

	void Start()
	{
		//manager.Load(true);

		/*var npc = manager.Get<ItemData>(2008);
		Debug.Log(npc.Description);

		var dic = manager.GetList<RoleData>();
		foreach (var item in dic.Values)
		{
			RoleData np = item as RoleData;
			Debug.Log(np.Icon);
		}*/
	}

	bool showTables = false;
	void OnGUI()
	{
		if (GUI.Button(new Rect(10, 10, 150, 30), "load from assetbundle"))
		{
			manager.Load(true);
		}

		if (GUI.Button(new Rect(170, 10, 150, 30), "load from Resources"))
		{
			manager.Load(false);
		}

		if (GUI.Button(new Rect(330, 10, 80, 30), showTables ? "hide tables" : "show tables"))
		{
			showTables = !showTables;
		}

		if (showTables)
		{
			// Just for show, you have no need to care about details below.
			Type baseType = typeof(DataTable);
			Assembly assembly = baseType.Assembly;
			foreach (Type tableType in assembly.GetTypes().Where(t => t.IsSubclassOf(baseType)))
			{
				string dataTableClassName = tableType.Name;
				string headName = dataTableClassName.Substring(0, dataTableClassName.IndexOf(Config.DataTableClassNamePostfix));
				string rowDataClassName = Config.GetRowDataClassName(headName);
				Type rowType = Type.GetType(rowDataClassName);
				var dic = manager.GetList(rowType);
				if (dic == null)
					continue;
				foreach (var item in dic.Values)
				{
					Debug.Log(item.ToString());
				}
			}
		}
	}


}