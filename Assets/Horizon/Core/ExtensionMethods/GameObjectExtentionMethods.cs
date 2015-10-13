using System;
using UnityEngine;

namespace Horizon.Core.ExtensionMethods
{
	public static class GameObjectExtentionMethods
	{
		public static ComponentType GetComponentInParentRecursive<ComponentType>(this GameObject self)
			where ComponentType : class
		{
			ComponentType comp = self.GetComponent<ComponentType> ();

			if (comp != null) 
			{
				return comp;
			} 
			else if (self.transform.parent == null) 
			{
				return null;
			} 
			else 
			{
				return self.transform.parent.gameObject.GetComponentInParentRecursive<ComponentType>();
			}
		}

		public static void SetLayerRecursively(this GameObject self, int newLayer)
		{
			self.layer = newLayer;
			
			foreach(Transform child in self.transform)
			{
				child.gameObject.SetLayerRecursively(newLayer);
			}
		}
	}
}
