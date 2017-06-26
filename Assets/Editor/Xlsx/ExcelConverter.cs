using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;



public class ExcelConverter : EditorWindow
{

	[MenuItem(@"Tools/ExcelConverter")]
	public static void ShowExcelWindow()
	{
		ExcelConverter wnd = EditorWindow.GetWindow<ExcelConverter>("ExcelConverter");
		wnd.Init();
		wnd.position = new Rect(100, 100, 300, 300);
		wnd.maximized = true;
		wnd.Show();
	}


	string singleExcelPath;

	string groupExcelPath;

	string exportPath;


	void Init()
	{
		singleExcelPath = Application.dataPath;

		groupExcelPath = Application.dataPath;

		exportPath = Application.dataPath;
	}


	void OnGUI()
	{
		GUILayout.Label("Convert excel files to universe !", EditorStyles.largeLabel);

		GUILayout.Label("single file Path :", EditorStyles.largeLabel);
		singleExcelPath = GUILayout.TextField(singleExcelPath);
		if (GUILayout.Button("Open"))
		{
			singleExcelPath = EditorUtility.OpenFilePanel("Open file", String.Empty, "xlsx");
		}

		GUILayout.Label("group file Path :", EditorStyles.largeLabel);
		groupExcelPath = GUILayout.TextField(groupExcelPath);
		if (GUILayout.Button("Open"))
		{
			singleExcelPath = EditorUtility.OpenFilePanel("Open Folder", String.Empty, "xlsx");
		}

		GUILayout.Label("export Path :", EditorStyles.largeLabel);
		exportPath = GUILayout.TextField(exportPath);
		if (GUILayout.Button("Open"))
		{
			singleExcelPath = EditorUtility.OpenFilePanel("Open Folder", String.Empty, "xlsx");
		}

	}



}