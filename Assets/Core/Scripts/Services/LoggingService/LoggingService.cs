using System;
using System.IO;
using UnityEngine;
using System.Collections;

public class LoggingService : Service,  ILoggingService
{
	public event Action<string> OnLog;

	public void Log(string message)
	{
		UnityEngine.Debug.Log(message);
		if(OnLog != null)
			OnLog(message);
	}

	public ILog ErrorLog
	{
		get
		{
			return m_ErrorLog;
		}
	}

	#region IService implementation

	public override IEnumerator LoadService ()
	{
		m_screenLog = new LambdaLogger((msg) => LogToScreen(msg));
		m_coreLogFile = NewLogFile();
		m_coreLogFileAndScreen = new MultiLog(CoreLogFile, ScreenLog);
		m_ErrorLog = new LambdaLogger((msg) => UnityEngine.Debug.LogError(msg));
		yield break;
	}

	#endregion

	public ILog CoreLogFile
	{
		get
		{
			return m_coreLogFile;
		}
	}

	public ILog CoreLogFileAndScreen
	{
		get
		{
			return m_coreLogFileAndScreen;
		}
	}

	public event Action<string> OnLogToScreen;

	public Observable<bool> ShowScreenLog
	{
		get
		{
			return showScreenLog;
		}
	}

	private readonly Observable<bool> showScreenLog = new Observable<bool>();

	public ILog ScreenLog
	{
		get
		{
			return m_screenLog;
		}
	}

	public ILog NewLogFile (LogFileCreateMode CreateMode = LogFileCreateMode.Overwrite)
	{
		string path = Path.Combine(Application.dataPath, "Logs");
		path = Path.Combine(path, "Log.txt");
		return new LogFile(path, CreateMode, CallerInformation.MethodName, CallerInformation.FilePath, CallerInformation.LineNumber);
	}

	public ILog NewLogFile (string name, LogFileCreateMode CreateMode = LogFileCreateMode.Overwrite)
	{
		string filename = name + ".txt";
		string path = Path.Combine(Application.dataPath, "Logs");
		path = Path.Combine(path, filename);
		return new LogFile(path, CreateMode, CallerInformation.MethodName, CallerInformation.FilePath, CallerInformation.LineNumber);
	}

	public ILog NewMultiLog (params ILog[] logs)
	{
		return new MultiLog(logs);
	}

	private class LogFile : ILog
	{
		public event Action<string> OnLog;

		private readonly string filePath;

		public LogFile(string path, 
			LogFileCreateMode createMode = LogFileCreateMode.Overwrite, 
			string method = "", 
			string file = "", 
			int line = 0)
		{
			string directory = Path.GetDirectoryName(path);
			if(!Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
			}

			if(createMode == LogFileCreateMode.CreateNew)
			{
				path = NextAvailableFilename(path);
			}

			if(!File.Exists(path))
			{
				using (var writer = File.CreateText(path))
				{
					writer.WriteLine("log file created on " + DateTime.Now.ToString() + " by " + Environment.UserName + " on Machine " + Environment.MachineName);
					writer.WriteLine("created in method  " + method + " in file " + file + " at line " + line);
				}
			}

			filePath = path;
		}

		public void Log (string message)
		{
			using (var fileWriter = File.OpenWrite(filePath))
			{
				fileWriter.Seek(0, SeekOrigin.End);
				using (var writer = new StreamWriter(fileWriter))
				{
					writer.WriteLine(message);
					if(OnLog != null)
						OnLog(message);
				}
			}
		}

		private static string numberPattern = " ({0})";

		public static string NextAvailableFilename (string path)
		{
			// Short-cut if already available
			if(!File.Exists(path))
				return path;

			// If path has extension then insert the number pattern just before the extension and return next filename
			if(Path.HasExtension(path))
				return GetNextFilename(path.Insert(path.LastIndexOf(Path.GetExtension(path)), numberPattern));

			// Otherwise just append the pattern to the path and return next filename
			return GetNextFilename(path + numberPattern);
		}

		private static string GetNextFilename (string pattern)
		{
			string tmp = string.Format(pattern, 1);
			if(tmp == pattern)
				throw new ArgumentException("The pattern must include an index place-holder", "pattern");

			if(!File.Exists(tmp))
				return tmp; // short-circuit if no matches

			int min = 1, max = 2; // min is inclusive, max is exclusive/untested

			while(File.Exists(string.Format(pattern, max)))
			{
				min = max;
				max *= 2;
			}

			while(max != min + 1)
			{
				int pivot = (max + min) / 2;
				if(File.Exists(string.Format(pattern, pivot)))
					min = pivot;
				else
					max = pivot;
			}

			return string.Format(pattern, max);
		}
	}

	private class MultiLog : ILog
	{
		public event Action<string> OnLog;

		ILog[] logs;

		public MultiLog(params ILog[] logs)
		{
			this.logs = logs;
		}

		public void Log (string message)
		{
			foreach(ILog log in logs)
			{
				log.Log(message);
			}

			if(OnLog != null)
				OnLog(message);
		}
	}

	private class LambdaLogger : ILog
	{
		public event Action<string> OnLog;

		public LambdaLogger(Action<string> logAction)
		{
			this.logAction = logAction;
		}

		public void Log (string message)
		{
			logAction(message);
			if(OnLog != null)
				OnLog(message);
		}

		Action<string> logAction;
	}

	private void LogToScreen (string Message)
	{
		if(OnLogToScreen != null)
		{
			OnLogToScreen(Message);
		}
	}

	private LambdaLogger m_screenLog;

	ILog m_ErrorLog;
	ILog m_coreLogFile;
	ILog m_coreLogFileAndScreen;
}