using UnityEngine;

using System.Collections;

public class HorizonUnitGameView : MonoBehaviour 
{
	private float outlineSize = 0.05f;

	private HPLabel HPDisplay;

	public HorizonUnitModel model;
	private Renderer unitRenderer;

	public Sprite Portrait;

	public float OutlineSize
	{
		get
		{
			return outlineSize;
		}
		set
		{
			outlineSize = Mathf.Clamp(value,0,0.1f);
			MeshRenderer rend = gameObject.GetComponentInChildren<MeshRenderer>();
			foreach(Material mat in rend.materials)
			{
				mat.SetFloat("_Outline",outlineSize);
			}
		}
	}

	public int OutlinePercentage
	{
		get
		{
			return (int)OutlineSize/100;
		}
		set
		{
			OutlineSize = 0.001f * value;
		}
	}

	void Start()
	{
		unitRenderer = gameObject.GetComponentInChildren<Renderer>();
		model = gameObject.GetComponent<HorizonUnitModel>();
		model.view = this;

		HPDisplay = CombatUI.NewHPLabel();
		HPDisplay.gameObject.SetActive(true);
	}

	void Update()
	{
		HPDisplay.MaxHP = model.maxHp;
		HPDisplay.CurrentHP = model.Hp;
	}

	void LateUpdate()
	{
		HPDisplay.rectTransform.position = getHPLablePosition();
	}

	void OnDestroy()
	{
		if(HPDisplay != null && HPDisplay.gameObject != null)
			Destroy(HPDisplay.gameObject);
	}

	Vector3 getHPLablePosition()
	{
		float unitHeight = unitRenderer.bounds.max.y;

		Vector3 worldPoint = new Vector3(transform.position.x,unitHeight,transform.position.z);

		return Camera.main.WorldToScreenPoint(worldPoint);
	}
}
