
using UnityEngine;
using EasyExcel;


public class Example : MonoBehaviour
{

	void Start()
	{
		DataTableManager mgr = new DataTableManager();
		mgr.Load();

		var npc = mgr.Get<ItemData>(2008);
		Debug.Log(npc.Description);

		var dic = mgr.GetList<RoleData>();
		foreach (var item in dic.Values)
		{
			RoleData np = item as RoleData;
			Debug.Log(np.Icon);
		}
	}


	void Update()
	{

	}


	void OnGUI()
	{

	}


}