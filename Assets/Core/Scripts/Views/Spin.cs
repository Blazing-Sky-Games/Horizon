using UnityEngine;
using System.Collections;

public class Spin : MonoBehaviour
{
	// Update is called once per frame
	void Update ()
	{
		transform.Rotate(new Vector3(0, 0, 1), -270 * Time.deltaTime);
	}
}
