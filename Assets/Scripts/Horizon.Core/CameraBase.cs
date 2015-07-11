using System;
using Horizon.Core;
using Horizon.Core.GL;

namespace Horizon.Core
{
	public class CameraBase : HorizonGameObjectBase
	{
		protected virtual void OnPostRender()
		{
			GLManager.DrawGLObjects();
		}
	}
}

