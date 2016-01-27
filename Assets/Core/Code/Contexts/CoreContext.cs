using System;
using Slash.Unity.DataBind.Core.Data;
using System.Collections;
using Core.Code.Services.LoggingService;
using Combat.Code.Data.Context;


public class CoreContext : MainContextBase
{
	//TODO make this bindable
	public Type LoadingContextType
	{
		get
		{
			return typeof(LoadingContext);
		}
	}
			
	//TODO make this bindable
	public Type FirstContextType
	{
		get
		{
			return typeof(CombatContext);
		}
	}

	public override void Update ()
	{
		m_coroutineService.UpdateCoroutines();
	}

	protected override void RegisterCoreServices ()
	{
		ServiceLocator.RegisterService<IContextLoadingService, ContextLoadingService>();
		ServiceLocator.RegisterService<IReflectionService, ReflectionService>();
		ServiceLocator.RegisterService<ICoroutineService, CoroutineService>();
		ServiceLocator.RegisterService<ILoggingService, LoggingService>();
	}

	protected override void InstatiateCoreServices ()
	{
		m_contextLoadingService = ServiceLocator.GetService<IContextLoadingService>();
		m_coroutineService = ServiceLocator.GetService<ICoroutineService>();
		m_loggingService = ServiceLocator.GetService<ILoggingService>();

		m_contextLoadingService.LoadService();
		m_coroutineService.LoadService();
		m_loggingService.LoadService();
	}

	protected override IEnumerator Launch ()
	{
		// load the loading screen
		m_contextLoadingService.LoadingContextType = LoadingContextType;
		m_contextLoadingService.LoadContext(FirstContextType);
		yield break;
	}

	protected override void RemoveServiceReferences ()
	{
		m_contextLoadingService = null;
		m_coroutineService = null;
		m_loggingService = null;
	}

	protected override void RemoveCoreServices ()
	{
		ServiceLocator.RemoveService<IContextLoadingService>();
		ServiceLocator.RemoveService<IReflectionService>();
		ServiceLocator.RemoveService<ICoroutineService>();
		ServiceLocator.RemoveService<ILoggingService>();
	}

	private IContextLoadingService m_contextLoadingService;
	private ICoroutineService m_coroutineService;
	private ILoggingService m_loggingService;
}


