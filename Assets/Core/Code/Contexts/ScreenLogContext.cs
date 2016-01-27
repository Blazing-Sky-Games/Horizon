using System;
using Slash.Unity.DataBind.Core.Data;
using Core.Code.Services.LoggingService;

namespace AssemblyCSharp
{
	public class ScreenLogContext : SubContext
	{
		public override void UnLoad ()
		{
			loggingService.ScreenLog.OnLog -= LoggingService_ScreenLog_OnLog;
			loggingService = null;
		}

		public event Action<string> OnLog;

		public ScreenLogContext()
		{
			loggingService = ServiceLocator.GetService<ILoggingService>();
			loggingService.ScreenLog.OnLog += LoggingService_ScreenLog_OnLog;
		}

		void LoggingService_ScreenLog_OnLog (string obj)
		{
			if(OnLog != null)
				OnLog(obj);
		} 

		private ILoggingService loggingService;
	}
}

