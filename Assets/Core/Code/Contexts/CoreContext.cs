using System;
using System.Collections;

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

	public override IEnumerator WaitLoad ()
	{
		ServiceLocator.RegisterService<IContextLoadingService, ContextLoadingService>();
		ServiceLocator.RegisterService<IReflectionService, ReflectionService>();
		ServiceLocator.RegisterService<ICoroutineService, CoroutineService>();
		ServiceLocator.RegisterService<ILoggingService, LoggingService>();

		m_contextLoadingService = ServiceLocator.GetService<IContextLoadingService>();
		m_coroutineService = ServiceLocator.GetService<ICoroutineService>();
		m_loggingService = ServiceLocator.GetService<ILoggingService>();

		m_contextLoadingService.LoadService();
		m_coroutineService.LoadService();
		m_loggingService.LoadService();

		// load the loading screen
		m_contextLoadingService.LoadingContextType = LoadingContextType;
		m_contextLoadingService.WaitLoadContext(FirstContextType);
		yield break;
	}

	public override void Unload ()
	{
		m_contextLoadingService.UnloadService();
		m_coroutineService.UnloadService();
		m_loggingService.UnloadService();

		m_contextLoadingService = null;
		m_coroutineService = null;
		m_loggingService = null;

		ServiceLocator.RemoveService<IContextLoadingService>();
		ServiceLocator.RemoveService<IReflectionService>();
		ServiceLocator.RemoveService<ICoroutineService>();
		ServiceLocator.RemoveService<ILoggingService>();
	}

	private IContextLoadingService m_contextLoadingService;
	private ICoroutineService m_coroutineService;
	private ILoggingService m_loggingService;
}