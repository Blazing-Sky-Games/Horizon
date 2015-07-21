using System;
using Horizon.Combat.GameObjects;
using Horizon.Core;
using Horizon.Core.ExtensionMethods;
using Horizon.Core.GL;
using UnityEngine;
using Horizon.Core.Editor;
using Horizon.Core.Editor.Gizmos;

namespace Horizon.Combat.Editor
{
	public class TestSceneView : SceneView<TestObject>
	{
		[SerializeField]
		private Color gizmoColor;

		public Color GizomoColor
		{
			get
			{
				return gizmoColor;
			}
			set
			{
				gizmoColor = value;
				m_rectangleGizmo.color = gizmoColor;
			}
		}

		protected override void Init ()
		{
			base.Init ();
			m_rectangleGizmo = new RectangleGizmo(HorizonObject);
			m_rectangleGizmo.color = gizmoColor;
		}

		private RectangleGizmo m_rectangleGizmo;
	}
}


