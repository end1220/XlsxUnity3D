
using UnityEngine;
using Lite;


public class Example : MonoBehaviour
{

	void Start()
	{
		DataManager mgr = new DataManager();
		mgr.Init();

		var npc = mgr.Get<Npc0_Data>(6000);
		Debug.Log(npc.res);

		var dic = mgr.GetList<Npc0_Data>();
		foreach (var item in dic.Values)
		{
			Npc0_Data np = item as Npc0_Data;
			Debug.Log(np.res);
		}
	}


	void Update()
	{

	}


	void OnGUI()
	{

	}


}