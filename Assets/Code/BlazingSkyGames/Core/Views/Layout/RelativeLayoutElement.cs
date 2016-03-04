/*
The MIT License (MIT)

https://opensource.org/licenses/MIT

Copyright (c) 2016 Matthew Draper

Permission is hereby granted, free of charge, to any person obtaining a copy of this software 
and associated documentation files (the "Software"), 
to deal in the Software without restriction, 
including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, 
subject to the following conditions:

The above copyright notice and this permission notice shall be included 
in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", 
WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
MERCHANTABILITY, 
FITNESS FOR A PARTICULAR PURPOSE 
AND NONINFRINGEMENT. 
IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, 
DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, 
ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using UnityEngine;
using UnityEngine.UI;

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
