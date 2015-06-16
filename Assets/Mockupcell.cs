using UnityEngine;
using System.Collections;
using Gamelogic.Grids;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class Mockupcell : SpriteCell
{
	public bool highlighted;
	public bool passable;

    void Update()
    {
        if(passable)
            Color = highlighted ? Color.red : Color.white;
		else
			Color = Color.black;
    }

	
#if UNITY_EDITOR
	public override void OnDrawGizmos ()
	{
		//nothing
	}
	
	void OnDrawGizmosSelected()
	{
		Tools.current = Tool.None;
		base.OnDrawGizmos();
	}
#endif	
}
