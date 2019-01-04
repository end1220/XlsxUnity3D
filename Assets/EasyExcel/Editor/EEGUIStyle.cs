using UnityEditor;
using UnityEngine;

namespace EasyExcel
{
	public static class EEGUIStyle
	{
		// constants
		public const float spaceSize = 15f;
		public const float leftSpace = 10;
		public const float titleLen = 80;
		public const float textLen = 550;
		public const float buttonLen1 = 100;
		public const float buttonLen2 = 50;
		public const float buttonHeight = 30;
		private static GUISkin skin;
		public static GUIStyle label { get; private set; }
		public static GUIStyle boldLabel { get; private set; }
		public static GUIStyle smallLabel { get; private set; }
		public static GUIStyle largeLabel { get; private set; }
		public static GUIStyle textField { get; private set; }
		public static GUIStyle textFieldPath { get; private set; }
		public static GUIStyle textFieldCell { get; private set; }
		public static GUIStyle textArea { get; private set; }
		public static GUIStyle toggle { get; private set; }
		public static GUIStyle button { get; private set; }
		public static GUIStyle helpBox { get; private set; }
		public static GUIStyle link { get; private set; }
		public static GUIStyle foldout { get; private set; }
		public static GUIStyle red { get; private set; }
		public static GUIStyle yellow { get; private set; }

		public static void Ensure()
		{
			if (skin != null)
				return;

			const int fontSizeSmall = 10;
			const int fontSizeNormal = 12;
			const int fontSizeInput = 12;
			const int fontSizeLarge = 18;

			skin = Resources.Load<GUISkin>("EasyExcelGUISkin");
			link = skin.FindStyle("Link");
			red = skin.FindStyle("red");
			yellow = skin.FindStyle("yellow");

			label = new GUIStyle(EditorStyles.label)
			{
				fontSize = fontSizeNormal,
				wordWrap = true,
				richText = true
			};

			boldLabel = new GUIStyle(EditorStyles.label)
			{
				fontStyle = FontStyle.Bold,
				fontSize = fontSizeNormal
			};

			smallLabel = new GUIStyle(EditorStyles.label)
			{
				fontSize = fontSizeSmall,
				wordWrap = true,
				richText = true
			};

			largeLabel = new GUIStyle(EditorStyles.largeLabel)
			{
				fontStyle = FontStyle.Bold,
				fontSize = fontSizeLarge
			};

			textField = new GUIStyle(EditorStyles.textField) {fontSize = fontSizeInput};
			textFieldPath = new GUIStyle(EditorStyles.textField) {fontSize = fontSizeNormal};
			textFieldCell = new GUIStyle(EditorStyles.textField)
			{
				fontSize = fontSizeNormal,
				margin = new RectOffset(1, 1, 1, 1)
			};
			textArea = new GUIStyle(EditorStyles.textArea) {fontSize = fontSizeInput};
			helpBox = new GUIStyle(EditorStyles.helpBox) {fontSize = fontSizeSmall};
			toggle = new GUIStyle(EditorStyles.toggle) {fontSize = fontSizeNormal};
			button = new GUIStyle(EditorStyles.miniButton)
			{
				fontStyle = FontStyle.Bold,
				fontSize = fontSizeInput
			};
			foldout = new GUIStyle(EditorStyles.foldout)
			{
				fontStyle = FontStyle.Bold,
				fontSize = fontSizeInput,
				fixedWidth = 400,
				fixedHeight = 20
			};
		}

		public static string HandleCopyPaste(int controlID)
		{
			if (controlID == GUIUtility.keyboardControl)
				if (Event.current.type == EventType.KeyUp && (Event.current.modifiers == EventModifiers.Control ||
				                                              Event.current.modifiers == EventModifiers.Command))
				{
					if (Event.current.keyCode == KeyCode.C)
					{
						Event.current.Use();
						var editor =
							(TextEditor) GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
						editor.Copy();
					}
					else if (Event.current.keyCode == KeyCode.V)
					{
						Event.current.Use();
						var editor =
							(TextEditor) GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
						editor.Paste();
#if UNITY_5_3_OR_NEWER || UNITY_5_3
						return editor.text; //以及更高的unity版本中editor.content.text已经被废弃，需使用editor.text代替
#else
                    return editor.content.text;
#endif
					}
					else if (Event.current.keyCode == KeyCode.A)
					{
						Event.current.Use();
						var editor =
							(TextEditor) GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
						editor.SelectAll();
					}
				}

			return null;
		}

		public static string TextField(string value, params GUILayoutOption[] options)
		{
			var textFieldID = GUIUtility.GetControlID("TextField".GetHashCode(), FocusType.Keyboard) + 1;
			if (textFieldID == 0)
				return value;

			value = HandleCopyPaste(textFieldID) ?? value;

			return GUILayout.TextField(value, options);
		}

		public static string TextField(string value, GUIStyle style, params GUILayoutOption[] options)
		{
			var textFieldID = GUIUtility.GetControlID("TextField".GetHashCode(), FocusType.Keyboard) + 1;
			if (textFieldID == 0)
				return value;

			value = HandleCopyPaste(textFieldID) ?? value;

			return GUILayout.TextField(value, style, options);
		}

		public static string TextArea(string value, GUIStyle style, params GUILayoutOption[] options)
		{
			var textFieldID = GUIUtility.GetControlID("TextArea".GetHashCode(), FocusType.Keyboard) + 1;
			if (textFieldID == 0)
				return value;

			value = HandleCopyPaste(textFieldID) ?? value;

			return GUILayout.TextArea(value, style, options);
		}
	}
}