using System;
using Slash.Unity.DataBind.Core.Data;
using System.Collections;

namespace Core.Scripts.Contexts
{
	public class CoreContext : MainContextBase
	{
		private readonly Property<Type> LoadingContextTypeProperty = new Property<Type>();
		private readonly Property<Type> FirstContextTypeProperty = new Property<Type>();

		[TypeRestriction(typeof(MainContextBase))]
		public Type LoadingContextType
		{
			get
			{
				return LoadingContextTypeProperty.Value;
			}
			set
			{
				LoadingContextTypeProperty.Value = value;
			}
		}

		[TypeRestriction(typeof(MainContextBase))]
		public Type FirstContextType
		{
			get
			{
				return FirstContextTypeProperty.Value;
			}
			set
			{
				FirstContextTypeProperty.Value = value;
			}
		}

		public override void Update ()
		{
			m_coroutineService.UpdateCoroutines();
		}

		public override void RegisterCoreServices ()
		{
			// register core services
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
		}

		protected override void RemoveServiceReferences()
		{
			// remove our references to them
			m_contextLoadingService = null;
			m_coroutineService = null;
			m_loggingService = null;
		}

		protected override void RemoveCoreServices ()
		{
			// unregister core services
			ServiceLocator.RemoveService<IContextLoadingService>();
			ServiceLocator.RemoveService<IReflectionService>();
			ServiceLocator.RemoveService<ICoroutineService>();
			ServiceLocator.RemoveService<ILoggingService>();
		}

		private IContextLoadingService m_contextLoadingService;
		private ICoroutineService m_coroutineService;
		private ILoggingService m_loggingService;
	}
}

