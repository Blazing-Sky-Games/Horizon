using System;
using UnityEngine;
using System.Collections;
using System.IO;
using Core.Code.Services;

namespace Core.Code.Services.LoggingService
{
	public interface ILog
	{
		void Log (string message);
		event Action<string> OnLog;
	}
}



