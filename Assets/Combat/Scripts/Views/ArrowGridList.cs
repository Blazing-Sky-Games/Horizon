using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Slash.Unity.DataBind.Core.Data;

public class ArrowGridList : MonoBehaviour 
{
	public GameObject ItemViewPrefab;
	public RelativeLayoutElement Arrow;
	public GridLayoutGroup grid;

	public List<Context> items;

	public Routine SetHighlightedIndex;
}
