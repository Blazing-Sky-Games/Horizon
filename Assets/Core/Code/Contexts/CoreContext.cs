using System;
using System.Collections;
using Slash.Unity.DataBind.Core.Data;

public class CoreContext : MainContextBase
{		
	private Property<bool> IsLoadingProperty = new Property<bool>(); 

	public bool IsLoading
	{
		get
		{
			return IsLoadingProperty.Value;
		}
	}

	private Property<bool> ShowScreenLogProperty = new Property<bool>(); 

	public bool ShowScreenLog
	{
		get
		{
			return ShowScreenLogProperty.Value;
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

	protected override Coroutine Load ()
	{
		base.Load();
		ServiceLocator.RegisterService<IContextLoadingService, ContextLoadingService>();
		ServiceLocator.RegisterService<IReflectionService, ReflectionService>();
		ServiceLocator.RegisterService<ICoroutineService, CoroutineService>();
		ServiceLocator.RegisterService<ILoggingService, LoggingService>();
		ServiceLocator.RegisterService<ICombatScenarioService, CombatScenarioService>();

		m_contextLoadingService = ServiceLocator.GetService<IContextLoadingService>();
		m_coroutineService = ServiceLocator.GetService<ICoroutineService>();
		m_loggingService = ServiceLocator.GetService<ILoggingService>();

		m_contextLoadingService.LoadService();
		m_coroutineService.LoadService();
		m_loggingService.LoadService();

		m_loggingService.Log("core services instatiated and loaded");

		m_contextLoadingService.IsLoading.Changed.AddAction(IsLoadingChanged);
		IsLoadingChanged();

		m_loggingService.ShowScreenLog.Changed.AddAction(ShowScreenLogChanged);
		ShowScreenLogChanged();

		// load the loading screen
		return m_coroutineService.StartCoroutine(m_contextLoadingService.WaitLoadContext(FirstContextType));
	}

	void IsLoadingChanged()
	{
		IsLoadingProperty.Value = m_contextLoadingService.IsLoading.Value;
	}

	void ShowScreenLogChanged()
	{
		ShowScreenLogProperty.Value = m_loggingService.ShowScreenLog.Value;
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