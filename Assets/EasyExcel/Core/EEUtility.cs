using System;
using System.IO;
using UnityEngine;

namespace EasyExcel
{
	public static class EELog
	{
		public static void Log(string message)
		{
			Debug.Log("[EasyExcel] " + message);
		}
		
		public static void LogWarning(string message)
		{
			Debug.LogWarning("[EasyExcel] " + message);
		}

		public static void LogError(string message)
		{
			Debug.LogError("[EasyExcel] " + message);
		}
	}
	
	public static class EEUtility
	{
		public static bool IsExcelFileSupported(string filePath)
		{
			if (string.IsNullOrEmpty(filePath))
				return false;
			var lower = Path.GetExtension(filePath).ToLower();
			return lower == ".xlsx" || lower == ".xls" || lower == ".xlsm";
		}
		
		public static string GetFieldComment(Type classType, string fieldName)
		{
			try
			{
				var fld = classType.GetField(fieldName);
				var comment = fld.GetCustomAttributes(typeof(EECommentAttribute), true)[0] as EECommentAttribute;
				return comment != null ? comment.content : null;
			}
			catch
			{
				// ignored
			}

			return null;
		}
	}
}