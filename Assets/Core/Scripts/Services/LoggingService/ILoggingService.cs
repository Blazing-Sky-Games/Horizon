public interface ILoggingService : IService, ILog
{
	Observable<bool> ShowScreenLog{ get; }

	ILog ScreenLog{ get; }

	ILog ErrorLog { get; }

	ILog NewLogFile (LogFileCreateMode CreateMode = LogFileCreateMode.Overwrite);

	ILog NewLogFile (string name, LogFileCreateMode CreateMode = LogFileCreateMode.Overwrite);

	ILog NewMultiLog (params ILog[] logs);
}



