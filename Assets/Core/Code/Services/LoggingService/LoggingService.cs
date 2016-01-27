using System;
using UnityEngine;
using System.Collections;
using System.IO;

namespace Core.Code.Services.LoggingService
{
	public class LoggingService : ILoggingService
	{
		#region IService implementation

		public void LoadService ()
		{
			throw new NotImplementedException();
		}

		public void UnloadService ()
		{
			throw new NotImplementedException();
		}

		#endregion

		public LoggingService()
		{
			m_screenLog = new LambdaLogger((msg) => LogToScreen(msg));
			m_coreLogFile = NewLogFile();
			m_coreLogFileAndScreen = new MultiLog(CoreLogFile, ScreenLog);
		}

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
			return new LogFile(path, CreateMode);
		}

		public ILog NewLogFile (string name, LogFileCreateMode CreateMode = LogFileCreateMode.Overwrite)
		{
			string filename = name + ".txt";
			string path = Path.Combine(Application.dataPath, "Logs");
			path = Path.Combine(path, filename);
			return new LogFile(path, CreateMode);
		}

		public ILog NewMultiLog (params ILog[] logs)
		{
			return new MultiLog(logs);
		}

		public enum LogFileCreateMode
		{
			Overwrite,
			CreateNew
		}

		private class LogFile : ILog
		{
			public event Action<string> OnLog;

			private readonly string filePath;

			public LogFile(string path, LogFileCreateMode createMode = LogFileCreateMode.Overwrite)
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
						if(OnLog != null) OnLog(message);
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

		ILog m_coreLogFile;
		ILog m_coreLogFileAndScreen;
	}
}


