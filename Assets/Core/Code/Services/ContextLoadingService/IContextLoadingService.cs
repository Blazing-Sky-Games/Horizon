using System;
using System.Collections;

public interface IContextLoadingService : IService
{
	Type LoadingContextType{ get; set; }

	Message<MainContextBase> ContextLoaded{ get; }

	Message<MainContextBase> ContextUnloaded{ get; }

	IEnumerator LoadContext<ContextType> ()
			where ContextType : MainContextBase;

	IEnumerator LoadContext (Type contextType);

	IEnumerator UnloadContext<ContextType> ()
			where ContextType : MainContextBase;

	IEnumerator UnloadContext (Type contextType);
}

