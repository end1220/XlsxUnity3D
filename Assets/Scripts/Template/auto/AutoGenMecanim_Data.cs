using System;
using System.Collections.Generic;


namespace Lite
{
	public class AutoGenMecanim_Data : IData
	{
		string _sourcePath;	//sourcePath
		public string sourcePath { get { return _sourcePath;} }

		string _destPath;	//destPath
		public string destPath { get { return _destPath;} }

		float _radius;	//radius
		public float radius { get { return _radius;} }

		float _height;	//height
		public float height { get { return _height;} }

		string _weaponBone1;	//weaponBone1
		public string weaponBone1 { get { return _weaponBone1;} }

		string _weaponBone2;	//weaponBone2
		public string weaponBone2 { get { return _weaponBone2;} }

		public override int init(TabReader reader, int row, int column)
		{
			column = base.init(reader, row, column);

			if(reader.At(row, column) == null)
				_sourcePath = "";
			else
				_sourcePath = reader.At(row, column);
			column++;

			if(reader.At(row, column) == null)
				_destPath = "";
			else
				_destPath = reader.At(row, column);
			column++;

			_radius = 0;
			float.TryParse(reader.At(row, column), out _radius);
			column++;

			_height = 0;
			float.TryParse(reader.At(row, column), out _height);
			column++;

			if(reader.At(row, column) == null)
				_weaponBone1 = "";
			else
				_weaponBone1 = reader.At(row, column);
			column++;

			if(reader.At(row, column) == null)
				_weaponBone2 = "";
			else
				_weaponBone2 = reader.At(row, column);
			column++;

			return column;
		}
	}
}