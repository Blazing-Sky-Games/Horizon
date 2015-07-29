using System;
using UnityEngine;

namespace Horizon.Core.ExtensionMethods
{
	public static class ColorExtensionMethods
	{
		public static Color SetAlpha(this Color self, float alpha)
		{
			return new Color(self.r,self.g,self.b,alpha);
		}
	}
}

