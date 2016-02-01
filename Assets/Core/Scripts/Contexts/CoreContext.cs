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

	protected override IEnumerator WaitLoad ()
	{
		yield return new Routine(base.WaitLoad());
		ServiceLocator.RegisterService<IContextLoadingService, ContextLoadingService>();
		ServiceLocator.RegisterService<IReflectionService, ReflectionService>();
		ServiceLocator.RegisterService<ICoroutineService, CoroutineService>();
		ServiceLocator.RegisterService<ILoggingService, LoggingService>();
		ServiceLocator.RegisterService<ICombatScenarioService, CombatScenarioService>();
		ServiceLocator.RegisterService<IResourceService, ResourceService>();

		m_contextLoadingService = ServiceLocator.GetService<IContextLoadingService>();
		m_reflectionService = ServiceLocator.GetService<IReflectionService>();
		m_coroutineService = ServiceLocator.GetService<ICoroutineService>();
		m_loggingService = ServiceLocator.GetService<ILoggingService>();
		m_combatScenarioService = ServiceLocator.GetService<ICombatScenarioService>();
		m_resourseService = ServiceLocator.GetService<IResourceService>();

		//TODO find a way to handle dependecys between services
		yield return new Routine(m_contextLoadingService.WaitLoadService());
		yield return new Routine(m_reflectionService.WaitLoadService());
		yield return new Routine(m_coroutineService.WaitLoadService());
		yield return new Routine(m_loggingService.WaitLoadService());
		yield return new Routine(m_resourseService.WaitLoadService());
		yield return new Routine(m_combatScenarioService.WaitLoadService());


		m_contextLoadingService.IsLoading.Changed.AddAction(IsLoadingChanged);
		IsLoadingChanged();

		m_loggingService.ShowScreenLog.Changed.AddAction(ShowScreenLogChanged);
		ShowScreenLogChanged();

		if(shouldLaunchIfCore)
			yield return new Routine(WaitLaunch());
	}

	protected override IEnumerator WaitLaunch ()
	{
		yield return new Routine(base.WaitLaunch());

		// load the loading screen
		yield return m_coroutineService.StartCoroutine(m_contextLoadingService.WaitLoadContext(FirstContextType));
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
		m_reflectionService.UnloadService();
		m_coroutineService.UnloadService();
		m_loggingService.UnloadService();
		m_combatScenarioService.UnloadService();
		m_resourseService.UnloadService();

		m_contextLoadingService = null;
		m_reflectionService = null;
		m_coroutineService = null;
		m_loggingService = null;
		m_combatScenarioService = null;
		m_resourseService = null;

		ServiceLocator.RemoveService<IContextLoadingService>();
		ServiceLocator.RemoveService<IReflectionService>();
		ServiceLocator.RemoveService<ICoroutineService>();
		ServiceLocator.RemoveService<ILoggingService>();
		ServiceLocator.RemoveService<ICombatScenarioService>();
		ServiceLocator.RemoveService<IResourceService>();
	}

	private IContextLoadingService m_contextLoadingService;
	private IReflectionService m_reflectionService;
	private ICoroutineService m_coroutineService;
	private ILoggingService m_loggingService;
	private ICombatScenarioService m_combatScenarioService;
	private IResourceService m_resourseService;
}