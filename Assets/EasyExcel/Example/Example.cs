
using UnityEngine;
using EasyExcel;


public class Example : MonoBehaviour
{

	void Start()
	{
		DataManager mgr = new DataManager();
		mgr.Load();

		/*var npc = mgr.Get<Npc0Data>(6000);
		Debug.Log(npc.res);

		var dic = mgr.GetList<Npc0Data>();
		foreach (var item in dic.Values)
		{
			Npc0Data np = item as Npc0Data;
			Debug.Log(np.res);
		}*/
	}


	void Update()
	{

	}


	void OnGUI()
	{

	}


}