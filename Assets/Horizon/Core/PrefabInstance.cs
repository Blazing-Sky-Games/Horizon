using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
#endif
using System.Collections.Generic;

[ExecuteInEditMode]
//code from here http://framebunker.com/blog/poor-mans-nested-prefabs/
public class PrefabInstance : MonoBehaviour
{
	public GameObject prefab;
	
	#if UNITY_EDITOR

	// Struct of all components. Used for edit-time visualization and gizmo drawing
	public struct Thingy {
		public SkinnedMeshRenderer meshSource;
		public Mesh mesh;
		public Matrix4x4 matrix;
		public List<Material> materials;

		public Mesh GetMesh()
		{
			meshSource.BakeMesh(mesh);
			return mesh;
		}
	}
	
	[System.NonSerializedAttribute] public List<Thingy> things = new List<Thingy> ();
	
	public void OnValidate () {
		things.Clear();
		if (enabled)
			Rebuild (prefab, Matrix4x4.identity);
	}
	
	void OnEnable () {
		things.Clear();
		if (enabled)
			Rebuild (prefab, Matrix4x4.identity);
	}
	
	void Rebuild (GameObject source, Matrix4x4 inMatrix) {
		if (!source)
			return;
		
		Matrix4x4 baseMat = inMatrix * Matrix4x4.TRS (-source.transform.position, Quaternion.identity, Vector3.one);
		
		foreach (SkinnedMeshRenderer mr in source.GetComponentsInChildren(typeof (SkinnedMeshRenderer), true))
		{
			Thingy t = new Thingy () {
				meshSource = mr,
				mesh = new Mesh(),
				matrix = baseMat * mr.transform.localToWorldMatrix,
				materials = new List<Material> (mr.sharedMaterials)
			};

			things.Add(t);
		}
		
		foreach (PrefabInstance pi in source.GetComponentsInChildren(typeof (PrefabInstance), true))
		{
			if (pi.enabled && pi.gameObject.activeSelf)
				Rebuild (pi.prefab, baseMat * pi.transform.localToWorldMatrix);
		}		
	}
	
	// Editor-time-only update: Draw the meshes so we can see the objects in the scene view
	void Update () {
		if (EditorApplication.isPlaying)
			return;
		Matrix4x4 mat = transform.localToWorldMatrix;
		foreach (Thingy t in things)
		{
			for (int i = 0; i < t.materials.Count; i++)
			{
				Graphics.DrawMesh (t.GetMesh(), mat * t.matrix, t.materials[i], gameObject.layer, null, i);
			}
		}
	}
	
	// Picking logic
	void OnDrawGizmos () { DrawGizmos (new Color (0,0,0,0)); }
	void OnDrawGizmosSelected () { DrawGizmos (new Color (0,1,1,.1f)); }
	void DrawGizmos (Color col) {
		if (EditorApplication.isPlaying)
			return;
		Gizmos.color = col;
		Matrix4x4 mat = transform.localToWorldMatrix;
		foreach (Thingy t in things)
		{
			Gizmos.matrix = mat * t.matrix;
			Gizmos.color = new Color (0,0,0,0);
			Gizmos.DrawMesh(t.GetMesh());
			Gizmos.color = col;
			Gizmos.DrawWireMesh(t.GetMesh());
		}		
	}
	
	// Baking stuff: Copy in all the referenced objects into the scene on play or build
	[PostProcessScene(-2)]
	public static void OnPostprocessScene() { 
		foreach (PrefabInstance pi in UnityEngine.Object.FindObjectsOfType (typeof (PrefabInstance)))
			BakeInstance (pi);
	}
	
	public static void BakeInstance (PrefabInstance pi) {
		if(!pi.prefab || !pi.enabled)
			return;
		pi.enabled = false;
		GameObject go = PrefabUtility.InstantiatePrefab(pi.prefab) as GameObject;
		Quaternion rot = go.transform.localRotation;
		Vector3 scale = go.transform.localScale;
		go.transform.parent = pi.transform;
		go.transform.localPosition = Vector3.zero;
		go.transform.localScale = scale;
		go.transform.localRotation = rot;
		pi.prefab = null;
		foreach (PrefabInstance childPi in go.GetComponentsInChildren<PrefabInstance>())
			BakeInstance (childPi);
	}
	
	#endif
}
