using UnityEngine;

using System;
using System.Collections;

public class SimpleCameraControls : MonoBehaviour 
{
	public Rect bounds; // hmm ... i think this is just cruft
	public float speed; // how fast does the camera move

	public Action PostRenderEvent; // tell people we are doing a post render

	void Update()
	{
		//move hased on input axis
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
