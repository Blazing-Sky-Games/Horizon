using System.Collections;
using UnityEngine;

public class HorizonUnitGameView : MonoBehaviour 
{
	private float outlineSize = 0.05f; // the size of the outline around the unit

	private HPLabel HPDisplay; // the hp bar

	public HorizonUnitModel model; // uuuhhh ... what is this for??? oohh ... it for for the bad hack where you get a unitview through a model
	private Renderer unitRenderer; // used to find the bounds on the units model

	public Sprite Portrait; // the portrait for this unit hmm... should this be in the model? wait! that would fix the modelview problem! i think?

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
				mat.SetFloat("_Outline",outlineSize); // set the highlight outline on the unit
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
		//init
		unitRenderer = gameObject.GetComponentInChildren<Renderer>();
		model = gameObject.GetComponent<HorizonUnitModel>();
		model.view = this; // ugh ..... this is a HACK

		HPDisplay = CombatUI.NewHPLabel(); // creat a new hp label using the ui system
		HPDisplay.gameObject.SetActive(true);
	}

	void Update()
	{
		HPDisplay.MaxHP = model.maxHp; // ugh, both of these should just be listening to hp events that dont exist in the unit model yet
		HPDisplay.CurrentHP = model.Hp;
	}

	// late update is where you do stuff that trakes movment
	// bassically, you need to do some stuff hear to prevent jittering
	void LateUpdate()
	{
		HPDisplay.rectTransform.position = getHPLablePosition();
	}

	// get rid of the hp bar when we die
	void OnDestroy()
	{
		if(HPDisplay != null && HPDisplay.gameObject != null)
			Destroy(HPDisplay.gameObject);
	}

	// get a point at the top center of the model
	// that is where the hp bar goes
	Vector3 getHPLablePosition()
	{
		float unitHeight = unitRenderer.bounds.max.y;

		Vector3 worldPoint = new Vector3(transform.position.x,unitHeight,transform.position.z);

		return Camera.main.WorldToScreenPoint(worldPoint);
	}
}
