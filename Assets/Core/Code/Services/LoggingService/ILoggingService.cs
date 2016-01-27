using System;
using Slash.Unity.DataBind.Core.Data;

namespace Core.Code.Services.LoggingService
{
	public interface ILoggingService : IService
	{
		ILog ScreenLog{ get; }
		ILog ErrorLog { get;}
		ILog NewLogFile (LogFileCreateMode CreateMode = LogFileCreateMode.Overwrite);
		ILog NewLogFile (string name, LogFileCreateMode CreateMode = LogFileCreateMode.Overwrite);
		ILog NewMultiLog (params ILog[] logs);
	}

}

