

// Auto generated file. DO NOT MODIFY.

using System;
using System.Collections.Generic;
using UnityEngine;


namespace Lite
{
	public class AutoGenMecanim_Data : BaseData
	{
		public string sourcePath;

		public string destPath;

		public float radius;

		public float height;

		public string weaponBone1;

		public string weaponBone2;



		#region _init (Do not invoke it)
		public override int _init(List<List<string>> sheet, int row, int column)
		{
			column = base._init(sheet, row, column);

			if(sheet[row][column] == null)
				sourcePath = "";
			else
				sourcePath = sheet[row][column];
			column++;

			if(sheet[row][column] == null)
				destPath = "";
			else
				destPath = sheet[row][column];
			column++;

			radius = 0;
			float.TryParse(sheet[row][column], out radius);
			column++;

			height = 0;
			float.TryParse(sheet[row][column], out height);
			column++;

			if(sheet[row][column] == null)
				weaponBone1 = "";
			else
				weaponBone1 = sheet[row][column];
			column++;

			if(sheet[row][column] == null)
				weaponBone2 = "";
			else
				weaponBone2 = sheet[row][column];
			column++;

			return column;
		}
#endregion


	}


	[CreateAssetMenu(fileName = "new AutoGenMecanim", menuName = "Template/AutoGenMecanim", order = 999)]
	public class AutoGenMecanim_Collection : ScriptableObject
	{
		public List<AutoGenMecanim_Data> elements = new List<AutoGenMecanim_Data>();
	}
}