using System;
using Slash.Unity.DataBind.Core.Data;
using Core.Code.Services.LoggingService;

namespace AssemblyCSharp
{
	public abstract class SubContext : Context
	{
		public abstract void UnLoad();
	}

}

