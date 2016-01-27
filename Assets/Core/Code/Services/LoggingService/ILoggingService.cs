using System;
using Slash.Unity.DataBind.Core.Data;

namespace Core.Code.Services.LoggingService
{
	public interface ILoggingService : IService
	{
		ILog ScreenLog{ get; }
	}

}

