using System;
using UnityEngine;
using System.Collections;

public static class ViewExtentions
{
	public static ViewType AddView<ViewType,LogicType,LogicDataType,ViewDataType>(this GameObject go, LogicType logic,ViewDataType viewData)
		where LogicDataType : Data
		where ViewDataType : Data
		where LogicType : ViewLogic<LogicDataType>
		where ViewType : View<LogicType, LogicDataType, ViewDataType>
	{
		return View<LogicType, LogicDataType, ViewDataType>.AddView<ViewType>(go, logic, viewData);
	}

	public static ViewType AddViewWithNewLogic<ViewType,LogicType,LogicDataType,ViewDataType>(this GameObject go, LogicDataType logicData,ViewDataType viewData)
		where LogicDataType : Data
		where ViewDataType : Data
		where LogicType : ViewLogic<LogicDataType>
		where ViewType : View<LogicType, LogicDataType, ViewDataType>
	{
		return View<LogicType, LogicDataType, ViewDataType>.AddViewWithNewLogic<ViewType>(go, logicData, viewData);
	}
}

public abstract class View<LogicType, LogicDataType, ViewDataType> : MonoBehaviour
	where LogicDataType : Data
	where ViewDataType : Data
	where LogicType : ViewLogic<LogicDataType>
{
	public Message Destroyed; 

	public IEnumerator WaitTillDestroyed()
	{
		while(!destroyed)
		{
			yield return 0;
		}
	}

	public bool IsDestroyed
	{
		get
		{
			return destroyed;
		}
	}

	public bool IsAlive
	{
		get
		{
			return !IsDestroyed;
		}
	}

	//construct a view and inject data and logic
	public static ViewType AddView<ViewType>(GameObject go, LogicType logic,ViewDataType viewData)
		where ViewType : View<LogicType, LogicDataType, ViewDataType>
	{
		injectedData = viewData;
		injectedLogic = logic;
		injectedOwnesLogic = false;
		return (ViewType)go.AddComponent(typeof(ViewType));
	}

	//construct a view, inject data and contruct logic from logic data
	public static ViewType AddViewWithNewLogic<ViewType>(GameObject go, LogicDataType logicData,ViewDataType viewData)
		where ViewType : View<LogicType, LogicDataType, ViewDataType>
	{
		injectedData = viewData;
		injectedLogic = (LogicType)Activator.CreateInstance(typeof(LogicType), new object[]{ viewData.DeepCopy() });
		injectedOwnesLogic = true;
		return (ViewType)go.AddComponent(typeof(ViewType));
	}

	// the logic of a view can be changed at run time
	public void SetLogic(LogicType logic)
	{
		DetachLogic();

		if(m_ownesLogic)
			Logic.Destroy();

		m_logic = logic;

		AttachLogic();
	}

	//-----------------------------
	// overide these methods to control the functionality of a view

	//do set up independant on logic or data
	protected virtual void SetUp ()
	{
	}

	//do set up dependant on data
	protected virtual void InitFromData(ViewDataType data)
	{
	}

	// add handlers to global messages and instance messages
	protected virtual void AttachInstanceHandlers()
	{
	}

	// add handlers to logic messages
	protected virtual void AttachLogic()
	{
	}

	// starts after the view is set up, and runs untill it stops
	protected virtual IEnumerator MainRoutine()
	{
		while(true)
		{
			yield return 0;
		}
	}

	// remove handlers from logic messages
	protected virtual void DetachLogic()
	{
	}

	// remove handlers from global messages and instance messages
	protected virtual void DetachInstanceHandlers()
	{
	}

	// clean up what every you need to, because the view is getting destroyed
	// mostlickly undoing stuff done in SetUp
	protected virtual void TearDown()
	{
	}

	//-----------------------------

	protected LogicType Logic
	{
		get { return m_logic; }
	}

	protected virtual void Awake()
	{
		Destroyed = new Message();
		InjectData(injectedData);
		InjectLogic(injectedLogic,injectedOwnesLogic);
		injectedLogic = null;
		injectedData = null;
	}
		
	protected void InjectLogic(LogicType logic, bool owned)
	{
		m_ownesLogic = owned;
		m_logic = logic;
	}

	protected void InjectData(ViewDataType data)
	{
		m_data = m_data ?? data;
	}
		
	private void Start()
	{
		if(Logic == null || m_data == null)
			throw new InvalidOperationException("a view must have its logic and data set when it 'starts up', by calling injectlogic and injectdata in awake, passing non-null logic and data");

		CoroutineUtility.StartCoroutine(LaunchWhenSceneInLoaded());
	}

	private IEnumerator LaunchWhenSceneInLoaded()
	{
		while(SceneUtility.LoadingScene)
			yield return 0;

		SetUp();
		InitFromData(m_data);
		AttachInstanceHandlers();
		AttachLogic();

		CoroutineUtility.StartCoroutine(MainRoutine());
		yield break;
	}
		
	private bool m_ownesLogic;
	private LogicType m_logic;
	private ViewDataType m_data;

	private static bool injectedOwnesLogic;
	private static LogicType injectedLogic;
	private static ViewDataType injectedData;

	private void OnDestory()
	{
		DetachLogic();
		DetachInstanceHandlers();

		if(m_ownesLogic)
			Logic.Destroy();

		TearDown();
		destroyed = true;
		CoroutineUtility.StartCoroutine(Destroyed.WaitSend());
	}

	private bool destroyed;
}

public abstract class DataFromEditorView<LogicType, LogicDataType, ViewDataType> : View<LogicType, LogicDataType, ViewDataType>
	where LogicDataType : Data
	where ViewDataType : Data
	where LogicType : ViewLogic<LogicDataType>
{
	public LogicDataType InspectorLogicData;
	public ViewDataType InspectorViewData;

	protected override void Awake ()
	{
		base.Awake();
		InjectLogic((LogicType)Activator.CreateInstance(typeof(LogicType), new object[]{ InspectorLogicData.DeepCopy() }),true);
		InjectData((ViewDataType)InspectorViewData.DeepCopy());
	}
}
