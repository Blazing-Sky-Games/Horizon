using System;
using Slash.Unity.DataBind.Core.Data;

public class ScreenLogContext : SubContext
{
	private Property<bool> showScreenLogProperty = new Property<bool>();

	public bool ShowScreenLog
	{
		get
		{
			return showScreenLogProperty.Value;
		}
		set
		{
			showScreenLogProperty.Value = value;
		}
	}

	public ScreenLogContext()
	{
		loggingService = ServiceLocator.GetService<ILoggingService>();
		loggingService.ScreenLog.OnLog += LoggingService_ScreenLog_OnLog;
		loggingService.ShowScreenLog.Changed.AddAction(OnShowScreenLogChanged);
		OnShowScreenLogChanged();
	}

	public override void UnLoad ()
	{
		loggingService.ScreenLog.OnLog -= LoggingService_ScreenLog_OnLog;
		loggingService.ShowScreenLog.Changed.RemoveAction(OnShowScreenLogChanged);
		loggingService = null;
	}

	public event Action<string> OnLog;

	void LoggingService_ScreenLog_OnLog (string obj)
	{
		if(OnLog != null)
			OnLog(obj);
	}

	void OnShowScreenLogChanged ()
	{
		showScreenLogProperty.Value = loggingService.ShowScreenLog.Value;
	}

	private ILoggingService loggingService;
}