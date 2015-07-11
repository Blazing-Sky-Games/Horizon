using UnityEngine;
using Horizon.Combat.GameObjects;
using Horizon.Core;
using Horizon.Core.Editor;
using Horizon.Core.Editor.Gizmos;
using Horizon.Core.ExtensionMethods;
using Horizon.Core.GL;

namespace Horizon.Combat.Editor
{
	public class TestSceneView : SceneView<TestObject>
	{
		public TestSceneView(TestObject model) : base(model)
		{
			m_rectangleGizmo = new RectangleGizmo(HorizonObject);
			m_rectangleGizmo.color = Color.cyan.SetAlpha(0.5f);
		}

		private RectangleGizmo m_rectangleGizmo;
	}
}


