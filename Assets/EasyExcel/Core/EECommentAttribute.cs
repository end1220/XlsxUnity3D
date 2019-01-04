using System;

namespace EasyExcel
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class EECommentAttribute : Attribute
	{
		public readonly string content;
		
		public EECommentAttribute(string text)
		{
			content = text;
		}
	}
}