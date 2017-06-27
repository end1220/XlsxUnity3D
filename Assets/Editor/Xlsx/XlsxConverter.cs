using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;



public class XlsxConverter : EditorWindow
{

	[MenuItem(@"Tools/XlsxConverter")]
	public static void ShowExcelWindow()
	{
		XlsxConverter wnd = EditorWindow.GetWindow<XlsxConverter>("XlsxConverter");
		wnd.Init();
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
		singleExcelPath = EditorGUILayout.TextField(singleExcelPath);
		if (GUILayout.Button("Open"))
		{
			singleExcelPath = EditorUtility.OpenFilePanel("Open file", String.Empty, "xlsx");
		}

		GUILayout.Label("group file Path :", EditorStyles.largeLabel);
		groupExcelPath = EditorGUILayout.TextField(groupExcelPath);
		if (GUILayout.Button("Open"))
		{
			singleExcelPath = EditorUtility.OpenFilePanel("Open Folder", String.Empty, "xlsx");
		}

		GUILayout.Label("export Path :", EditorStyles.largeLabel);
		exportPath = EditorGUILayout.TextField(exportPath);
		if (GUILayout.Button("Open"))
		{
			singleExcelPath = EditorUtility.OpenFilePanel("Open Folder", String.Empty, "xlsx");
		}

	}



}