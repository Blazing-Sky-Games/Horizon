using System;
namespace Horizon.Core.GL
{
	public abstract class GLObject : IDisposable
	{
		//TODO creat a way to "parent" a globject to a transform
		public abstract void Draw();
		public GLSettings Settings;
		public bool Enabled;

		public GLObject()
		{
			GLManager.__addGLObject(this);
			Enabled = true;
		}

		public void Dispose()
		{
			GLManager.__removeGLObject(this);
		}
	}
}

