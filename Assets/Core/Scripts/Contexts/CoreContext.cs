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

	protected override IEnumerator Load ()
	{
		yield return new Routine(base.Load());
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

		yield return new Routine(m_contextLoadingService.LoadService());
		yield return new Routine(m_reflectionService.LoadService());
		yield return new Routine(m_coroutineService.LoadService());
		yield return new Routine(m_loggingService.LoadService());
		yield return new Routine(m_combatScenarioService.LoadService());
		yield return new Routine(m_resourseService.LoadService());

		m_contextLoadingService.IsLoading.Changed.AddAction(IsLoadingChanged);
		IsLoadingChanged();

		m_loggingService.ShowScreenLog.Changed.AddAction(ShowScreenLogChanged);
		ShowScreenLogChanged();

		if(shouldLaunchIfCore)
			yield return new Routine(Launch());
	}

	protected override IEnumerator Launch ()
	{
		yield return new Routine(base.Launch());

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