using UnityEngine;

using System;
using System.Collections;

public class SimpleCameraControls : MonoBehaviour 
{
	public Rect bounds;
	public float speed;

	public Action PostRenderEvent;

	void Update()
	{
		transform.Translate((new Vector3(1,0,1)).normalized * Input.GetAxisRaw("Horizontal") * Time.deltaTime * speed, Space.World);
		transform.Translate(new Vector3(-1,0,1).normalized * Input.GetAxisRaw("Vertical") * Time.deltaTime * speed, Space.World);

		//transform.position = new Vector3(
			//Mathf.Clamp(transform.position.x,bounds.min.x,bounds.max.x),
			//transform.position.y,
			//Mathf.Clamp(transform.position.y,bounds.min.y,bounds.max.y)
		//);
	}

	void OnPostRender()
	{
		if(PostRenderEvent != null) PostRenderEvent();
	}
}
