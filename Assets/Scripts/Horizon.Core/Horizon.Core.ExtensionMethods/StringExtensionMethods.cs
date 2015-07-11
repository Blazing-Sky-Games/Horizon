using System;
using System.Text.RegularExpressions;

namespace Horizon.Core.ExtensionMethods
{
	public static class StringExtensionMethods
	{
		public static string SplitCamelCase(this string str)
		{
			if (string.IsNullOrEmpty(str))
			{
				return str;
			}
			
			string camelCase = Regex.Replace(Regex.Replace(str, @"(\P{Ll})(\P{Ll}\p{Ll})", "$1 $2"), @"(\p{Ll})(\P{Ll})", "$1 $2");
			string firstLetter = camelCase.Substring(0, 1).ToUpper();
			
			if (str.Length > 1)
			{
				string rest = camelCase.Substring(1);
				
				return firstLetter + rest;
			}
			else
			{
				return firstLetter;
			}
		}
	}
}

