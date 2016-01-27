using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[AddComponentMenu("Layout/Relative Layout Element", 140)]
[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(LayoutElement))]
[ExecuteInEditMode]
public class RelativeLayoutElement : MonoBehaviour
{
	public bool UseAbsHeight;
	public bool UseAbsWidth;

	public float AbsoluteWidth = 0;
	public float AbsoluteHeight = 0;

	[Range(0,1)]
	public float RelativeWidth = 0;
	[Range(0,1)]
	public float RelativeHeight = 0;

	public bool UseAbsX;
	public bool UseAbsY;

	public float AbsoluteX = 0;
	public float AbsoluteY = 0;

	[Range(-1,1)]
	public float RelativeX = 0;
	[Range(-1,1)]
	public float RelativeY = 0;

	void Awake()
	{
		layoutelem = GetComponent<LayoutElement>();
		rTran = GetComponent<RectTransform>();
	}

	void Update()
	{
		var parentTran = transform.parent.transform as RectTransform;

		if(parentTran.GetComponent<LayoutGroup>() != null)
		{
			layoutelem.preferredHeight = UseAbsHeight ? AbsoluteHeight : parentTran.rect.height * RelativeHeight;
			layoutelem.preferredWidth = UseAbsWidth ? AbsoluteWidth : parentTran.rect.width * RelativeWidth;
		}
		else
		{
			rTran.sizeDelta = new Vector2( UseAbsWidth ? AbsoluteWidth : parentTran.rect.width * RelativeWidth, UseAbsHeight ? AbsoluteHeight : parentTran.rect.height * RelativeHeight);
			rTran.position = new Vector3(UseAbsX ? AbsoluteX + parentTran.position.x : parentTran.position.x + parentTran.rect.width / 2 * RelativeX, UseAbsY ? AbsoluteY + parentTran.position.y : parentTran.position.y + parentTran.rect.height / 2 * RelativeY, rTran.position.x);
		}
	}

	private LayoutElement layoutelem;
	private RectTransform rTran;
}
