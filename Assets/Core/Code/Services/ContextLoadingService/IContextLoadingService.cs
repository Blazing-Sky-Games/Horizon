using System;
using System.Collections;

public interface IContextLoadingService : IService
{
	Type LoadingContextType{ get; set; }

	Message<MainContextBase> ContextLoading{ get; }

	Message<MainContextBase> ContextLoaded{ get; }

	Message<MainContextBase> ContextUnloading{ get; }

	Message<MainContextBase> ContextUnloaded{ get; }

	IEnumerator WaitLoadContext<ContextType> ()
			where ContextType : MainContextBase;

	IEnumerator WaitLoadContext (Type contextType);

	IEnumerator WaitUnloadContext<ContextType> ()
			where ContextType : MainContextBase;

	IEnumerator WaitUnloadContext (Type contextType);

	bool IsLoaded<ContextType> ()
		where ContextType : MainContextBase;

	bool IsLoaded(Type contextType);
}

