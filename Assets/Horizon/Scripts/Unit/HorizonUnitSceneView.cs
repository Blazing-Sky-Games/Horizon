// this is an editor game object
// only use it if we are in the editor
#if UNITY_EDITOR
using UnityEngine;
using System.Collections;

// lol we dont event use this right now
// this is where i would impliment a gizmo to posision units in the scene view
public class HorizonUnitSceneView : MonoBehaviour 
{
	void OnDrawGizmos()
	{

	}

	void OnDrawGismosSelected()
	{

	}
}
#endif
