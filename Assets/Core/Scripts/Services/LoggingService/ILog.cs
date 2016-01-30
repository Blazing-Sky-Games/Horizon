using System;

public interface ILog
{
	void Log (string message);

	event Action<string> OnLog;
}