using System;
using System.Diagnostics;

public static class CallerInformation
{
	public static String ReportError(string message)
	{
		// Get the frame one step up the call tree
		StackFrame CallStack = new StackFrame(1, true);
		
		// These will now show the file and line number of the ReportError
		string SourceFile = CallStack.GetFileName();
		int SourceLine = CallStack.GetFileLineNumber();
		
		return "Error: " + message + "\nFile: " + SourceFile + "\nLine: " + SourceLine.ToString();
	}
	
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
