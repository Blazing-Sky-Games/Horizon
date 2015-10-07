using Horizon.Core;
using Horizon.Combat.Models;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Horizon.Combat.Views
{
	public class CombatMouseInput : ViewBase<CombatModel>
	{
		//should mouse clicks be proccesed
		public bool HandleInput;

		public override void Update ()
		{
			base.Update ();

			UpdatePointUnderMouse();
			
			if(HandleInput)
			{
				ProcessInput();
			}
		}

		void UpdatePointUnderMouse()
		{
			Vector3 CorrectedMousePosision = Input.mousePosition;
			CorrectedMousePosision.z = Camera.main.nearClipPlane;

			mouseRay = Camera.main.ScreenPointToRay(CorrectedMousePosision);
			
			Plane gridPlane = new Plane(new Vector3(0,1,0),model.transform.position);
			
			float RayPlaneIntersectionAt;
			gridPlane.Raycast(mouseRay, out RayPlaneIntersectionAt);
			
			Vector3 planeIntersect = mouseRay.origin + mouseRay.direction * RayPlaneIntersectionAt - model.transform.position;

			int a = (int)Mathf.Floor(planeIntersect.x);
			int b = (int)Mathf.Floor(planeIntersect.z);

			PointUnderMouse = new GridPoint(a, b);
			
			if (model.grid.Contains(PointUnderMouse))
			{
				model.MouseOverCell = model.grid[PointUnderMouse];
			}
			else
			{
				model.MouseOverCell = null;
			}
		}

		private void ProcessInput()
		{
			if (Input.GetMouseButtonDown(0) && EventSystem.current.IsPointerOverGameObject() == false)
			{
				if (model.grid.Contains(PointUnderMouse))
				{
					model.ClickCell(PointUnderMouse);
				}
			}
		}

		private Ray mouseRay;
		private GridPoint PointUnderMouse;
	}
}

