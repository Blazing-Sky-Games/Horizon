using System;
using UnityEngine;

namespace Horizon.Core.GL
{
	public class GLLine : GLObject
	{
		public Vector3 StartPoint;
		public Vector3 EndPoint;

		public override void Draw()
		{
			base.Draw();
			UnityEngine.GL.Begin( UnityEngine.GL.LINES );
			
			UnityEngine.GL.Vertex(StartPoint);
			UnityEngine.GL.Vertex(EndPoint);
			
			UnityEngine.GL.End();
		}
	}
}

