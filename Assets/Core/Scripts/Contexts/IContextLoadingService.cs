using System;
using Slash.Unity.DataBind.Core.Data;
using System.Collections;

namespace Core.Scripts.Contexts
{
	public interface IContextService : IService
	{
		Type LoadingContextType{ get; set; }

		Message<MainContextBase> ContextLoaded{ get; }

		Message<MainContextBase> ContextUnloaded{ get; }

		IEnumerator LoadContext<ContextType> ()
			where ContextType : MainContextBase;

		IEnumerator LoadContext (Type contextType);

		IEnumerator UnloadContext<ContextType> ()
			where ContextType : MainContextBase;

		IEnumerator UnloadContext(Type contextType);
	}
}

