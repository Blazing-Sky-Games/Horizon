using System.Diagnostics;
using System;

public static class CallerInformation
{
	public static int LineNumber
	{
		get
		{
			StackFrame CallStack = new StackFrame(2, true);
			int line = new int();
			line += CallStack.GetFileLineNumber();
			return line;
		}
	}
	
	public static string FilePath
	{
		get
		{
			StackFrame CallStack = new StackFrame(2, true);
			string temp = CallStack.GetFileName();
			String file = String.Copy(String.IsNullOrEmpty(temp) ? "" : temp);
			return String.IsNullOrEmpty(file) ? "" : file;
		}
	}

	public static string MethodName
	{
		get
		{
			StackFrame CallStack = new StackFrame(2, true);
			string temp = CallStack.GetMethod().Name;
			String meth = String.Copy(String.IsNullOrEmpty(temp) ? "" : temp);
			return String.IsNullOrEmpty(meth) ? "" : meth;
		}
	}
}
