using Horizon.Core;
using Horizon.Core.ExtensionMethods;
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
			// point on near clipping plane mouse is over
			Vector3 CorrectedMousePosision = Input.mousePosition;
			CorrectedMousePosision.z = Camera.main.nearClipPlane;

			// Ray from camera through point on clipping plane
			mouseRay = Camera.main.ScreenPointToRay(CorrectedMousePosision);
			
			//plane the grid lies in
			Plane gridPlane = new Plane(model.grid.transform.up,model.grid.transform.position);
			
			// interect ray and plane
			float RayPlaneIntersectionAt;
			gridPlane.Raycast(mouseRay, out RayPlaneIntersectionAt);
			
			//intersection point
			Vector3 planeIntersect = mouseRay.origin + mouseRay.direction * RayPlaneIntersectionAt - model.transform.position;

			//get the grid point under the nouse
			//hmm....... this needs to be more robust if we want to move the grid around
			//TODO: convert the world space point to grid space point (even though they are the same now)
			int a = (int)Mathf.Floor(planeIntersect.x);
			int b = (int)Mathf.Floor(planeIntersect.z);
			PointUnderMouse = new GridPoint(a, b);
			
			if (model.grid.Contains(PointUnderMouse))
			{
				model.MouseOverCell = model.grid[PointUnderMouse];
			}
			else
			{
				//null is interpreted as hovering outside the grid
				model.MouseOverCell = null;
			}

			//raycast to check for units
			// this migth break later if there are other colliders in the scene
			RaycastHit hit; 
			if (Physics.Raycast (mouseRay,out hit)) 
			{
				hitUnit = hit.collider.gameObject.GetComponentInParentRecursive<Unit>();

				if (hitUnit != null) 
				{
					model.MouseOverUnit = hitUnit;
				}
				else 
				{
					model.MouseOverUnit = null;
				}
			} 
			else 
			{
				hitUnit = null;
				model.MouseOverUnit = null;
			}
		}

		//handle clicks
		private void ProcessInput()
		{
			if (Input.GetMouseButtonDown(0) && EventSystem.current.IsPointerOverGameObject() == false)
			{
				if (model.grid.Contains(PointUnderMouse))
				{
					model.ClickCell(PointUnderMouse);
				}

				if(hitUnit != null)
				{
					model.ClickUnit(hitUnit);
				}
			}
		}

		private Ray mouseRay;
		private GridPoint PointUnderMouse;

		private Unit hitUnit;
	}
}

