using System;
using UnityEngine;

namespace Horizon.Core.GL
{
	public static class GLUtility
	{
		public static Material DefaultMaterial
		{
			get
			{
				if(m_defaultMaterial == null)
				{
					var shader = Shader.Find ("Hidden/Internal-Colored");
					var mat = new Material (shader);
					mat.hideFlags = HideFlags.HideAndDontSave;
					m_defaultMaterial = mat;
				}

				return m_defaultMaterial;
			}
		}

		private static Material m_defaultMaterial;
	}
}

